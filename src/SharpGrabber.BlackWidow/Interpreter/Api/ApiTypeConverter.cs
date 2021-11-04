using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    /// <summary>
    /// Default implementation for <see cref="IApiTypeConverter"/>
    /// </summary>
    public class ApiTypeConverter : IApiTypeConverter
    {
        /// <summary>
        /// Gets the default instance of this class.
        /// </summary>
        public static readonly ApiTypeConverter Default = new();

        public object ChangeType(object value, Type targetType)
        {
            // init
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (value == null)
                return null;

            // test same type
            var valueType = value.GetType();
            if (valueType == targetType)
                return value;

            // try internal conversion
            if (ChangeTypeInternal(value, targetType, out var result))
                return result;

            // fallback
            return Convert.ChangeType(value, targetType);
        }

        protected virtual bool ChangeTypeInternal(object value, Type targetType, out object newValue)
        {
            var t = targetType;
            var nullable = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (nullable)
                t = t.GetGenericArguments()[0];

            if (t == typeof(string))
            {
                newValue = value.ToString();
                return true;
            }
            if (t == typeof(Uri))
            {
                newValue = new Uri(value.ToString());
                return true;
            }
            if (t == typeof(TimeSpan))
            {
                var milliseconds = (double)Convert.ChangeType(value, typeof(double));
                newValue = TimeSpan.FromMilliseconds(milliseconds);
                return true;
            }
            if (t.IsEnum)
            {
                newValue = Enum.Parse(t, value.ToString(), true);
                return true;
            }

            newValue = value;
            return false;
        }
    }
}
