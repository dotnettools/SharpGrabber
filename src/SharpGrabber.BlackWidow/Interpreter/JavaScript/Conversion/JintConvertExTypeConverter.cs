using DotNetTools.ConvertEx;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript
{
    internal class JintConvertExTypeConverter : Jint.Runtime.Interop.ITypeConverter
    {
        private readonly ITypeConverter _converter;

        public JintConvertExTypeConverter(ITypeConverter converter)
        {
            _converter = converter;
        }

        public object Convert(object value, Type type, IFormatProvider formatProvider)
        {
            return _converter.Convert(value, type, formatProvider);
        }

        public bool TryConvert(object value, Type type, IFormatProvider formatProvider, out object converted)
        {
            return _converter.TryConvert(value, type, formatProvider, out converted);
        }
    }
}
