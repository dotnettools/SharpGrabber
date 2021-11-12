using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript
{
    /// <summary>
    /// Handles type conversion for Jint.
    /// </summary>
    public class JintMultiTypeConverter : ITypeConverter
    {
        private readonly Engine _engine;
        private readonly IList<ITypeConverter> _converters;

        protected JintMultiTypeConverter(Engine engine,
            Func<JintMultiTypeConverter, IEnumerable<ITypeConverter>> getConverters)
        {
            _engine = engine;
            _converters = getConverters?.Invoke(this)?.ToArray() ??
                          throw new ArgumentNullException(nameof(getConverters));
        }

        /// <summary>
        /// Creates an instance of <see cref="JintMultiTypeConverter"/> with the specified <paramref name="converters"/>.
        /// </summary>
        public static JintMultiTypeConverter Create(Engine engine, IEnumerable<ITypeConverter> converters)
        {
            return new JintMultiTypeConverter(engine, _ => converters);
        }

        /// <summary>
        /// Creates an instance of <see cref="JintMultiTypeConverter"/> with the built-in <see cref="ITypeConverter"/> stack, with
        /// the option to add arbitrary converters.
        /// </summary>
        public static JintMultiTypeConverter CreateDefault(Engine engine,
            IEnumerable<ConvertEx.ITypeConverter> arbitraryConverters = null)
        {
            IEnumerable<ITypeConverter> GetConverters(JintMultiTypeConverter self)
            {
                var conversionManager =
                    new ConvertEx.TypeConverter((ConvertEx.TypeConverter) ConvertEx.ConvertEx.DefaultConverter);
                var builtin = new BuiltInTypeConverter(self);
                conversionManager.AddDigester(builtin);
                conversionManager.AddConverter(builtin);

                var converters = new List<ITypeConverter>()
                {
                    new JintConvertExTypeConverter(conversionManager),
                    new DefaultTypeConverter(engine),
                };
                if (arbitraryConverters != null)
                    converters.InsertRange(0, arbitraryConverters.Select(c => new JintConvertExTypeConverter(c)));
                return converters;
            }

            return new JintMultiTypeConverter(engine, GetConverters);
        }

        public object Convert(object value, Type type, IFormatProvider formatProvider)
        {
            if (TryConvert(value, type, formatProvider, out var result))
                return result;
            throw new InvalidCastException($"Could not convert value of type {value?.GetType()} to {type}");
        }

        public bool TryConvert(object value, Type type, IFormatProvider formatProvider, out object converted)
        {
            foreach (var converter in _converters)
                if (converter.TryConvert(value, type, formatProvider, out converted))
                    return true;
            converted = null;
            return false;
        }

        private sealed class DelegateTarget
        {
            private readonly JintMultiTypeConverter _self;
            private readonly Delegate _delegate;
            private readonly Type[] _targetTypes;
            private readonly Type _returnType;

            public DelegateTarget(JintMultiTypeConverter self, Delegate @delegate, MethodInfo targetMethod)
            {
                _self = self;
                _delegate = @delegate;
                _returnType = targetMethod.ReturnType;
                _targetTypes = @delegate.Method.GetParameters().Select(p => p.ParameterType).ToArray();
            }

            public object InvokeDelegate(params object[] args)
            {
                var convertedArgs = new object[args.Length];
                for (var iparam = 0; iparam < _targetTypes.Length; iparam++)
                {
                    var type = _targetTypes[iparam];
                    var value = args[iparam];
                    var convertedArg = _self.Convert(value, type, null);
                    convertedArgs[iparam] = convertedArg;
                }

                var result = _delegate.DynamicInvoke(convertedArgs);
                var convertedResult = _self.Convert(result, _returnType, null);
                return convertedResult;
            }
        }

        private sealed class BuiltInTypeConverter : ConvertEx.TypeConverterBase, ConvertEx.ITypeDigester
        {
            private readonly JintMultiTypeConverter _self;

            public BuiltInTypeConverter(JintMultiTypeConverter self)
            {
                _self = self;
            }

            private Engine _engine => _self._engine;

            public IEnumerable<Type> Offer(Type valueType, Type targetType)
            {
                if (valueType.IsSubclassOf(typeof(JsValue)) && !targetType.IsSubclassOf(typeof(JsValue)))
                {
                    return new[] {typeof(object)};
                }

                return Array.Empty<Type>();
            }

            public override bool TryConvert(object value, Type type, IFormatProvider formatProvider,
                out object converted)
            {
                var valueType = value.GetType();
                if (type.IsSubclassOf(typeof(Delegate)))
                {
                    converted = ConvertDelegate((Delegate) value, type);
                    return true;
                }

                if (type == typeof(JsValue))
                {
                    converted = JsValue.FromObject(_engine, value);
                    return true;
                }

                if (type == typeof(JsValue[]))
                {
                    converted = new[] {JsValue.FromObject(_engine, value)};
                    return true;
                }

                if (valueType.IsSubclassOf(typeof(JsValue)) && !type.IsSubclassOf(typeof(JsValue)))
                {
                    converted = (value as JsValue).ToObject();
                    return true;
                }

                converted = null;
                return false;
            }

            private Delegate ConvertDelegate(Delegate value, Type targetType)
            {
                var targetMethod = targetType.GetMethod("Invoke");
                var parameters = targetMethod.GetParameters();
                var paramTypes = new Type[] {typeof(DelegateTarget)}
                    .Concat(parameters.Select(p => p.ParameterType))
                    .ToArray();

                var newMethod = new DynamicMethod("InvokeConverted", targetMethod.ReturnType, paramTypes,
                    typeof(DelegateTarget));
                var dynamicInvoker = typeof(DelegateTarget).GetMethod(nameof(DelegateTarget.InvokeDelegate),
                                         BindingFlags.Instance | BindingFlags.Public) ??
                                     throw new Exception("Could not find the invocation middleware method.");

                var il = newMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldc_I4, parameters.Length);
                il.Emit(OpCodes.Newarr, typeof(object));
                var paramIndex = 0;
                foreach (var paramInfo in parameters)
                {
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Ldc_I4, paramIndex);
                    il.Emit(OpCodes.Ldarg, paramIndex + 1);
                    il.Emit(OpCodes.Stelem_Ref);
                    paramIndex++;
                }

                il.EmitCall(OpCodes.Callvirt, dynamicInvoker, null);
                il.Emit(OpCodes.Ret);

                var target = new DelegateTarget(_self, value, targetMethod);
                return newMethod.CreateDelegate(targetType, target);
            }
        }
    }
}