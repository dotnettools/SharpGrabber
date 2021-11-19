using DotNetTools.ConvertEx;
using DotNetTools.ConvertEx.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.TypeConversion
{
    internal static class TypeConverters
    {
        public static readonly ITypeConverter Default;

        static TypeConverters()
        {
            var converter = new TypeConverter();
            converter
                .AddDigester<NullableDigester>()
                .AddConverter<ToBoolConverter>()
                .AddConverter<NullableConverter>()
                .AddConverter<ToStringConverter>()
                .AddConverter<EnumConverter>()
                .AddConverter<UriConverter>()
                .AddConverter<TimeSpanConverter>()
                .AddConverter<SystemConverter>();
            Default = converter;
        }
    }
}
