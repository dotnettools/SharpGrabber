using DotNetTools.ConvertEx;
using DotNetTools.SharpGrabber.BlackWidow.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.TypeConversion
{
    internal class ToBoolConverter : ITypeConverter
    {
        public bool TryConvert(object value, Type targetType, IFormatProvider formatProvider, out object convertedValue)
        {
            if (targetType != typeof(bool))
            {
                convertedValue = null;
                return false;
            }

            if (value is string)
            {
                var str = value?.ToString();
                convertedValue = !string.IsNullOrEmpty(str);
            }
            else if (value.GetType().IsNumericType())
            {
                var num = (double)Convert.ChangeType(value, typeof(double));
                convertedValue = num != 0;
                return true;
            }
            else
            {
                convertedValue = null;
                return false;
            }
            return true;
        }
    }
}
