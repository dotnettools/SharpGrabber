using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    /// <summary>
    /// Where implemented, has the ability to convert values to other types.
    /// </summary>
    public interface IApiTypeConverter
    {
        /// <summary>
        /// Converts <paramref name="value"/> to <paramref name="targetType"/>.
        /// </summary>
        object ChangeType(object value, Type targetType);
    }
}
