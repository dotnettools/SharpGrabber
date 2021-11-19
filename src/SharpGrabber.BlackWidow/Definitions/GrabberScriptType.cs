using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    /// <summary>
    /// Defines all possible script types.
    /// </summary>
    public enum GrabberScriptType
    {
        /// <summary>
        /// ECMAScript
        /// </summary>
        [GrabberScriptType(FileExtension = "js")]
        JavaScript = 1,
    }

    public static class GrabberScriptTypeExtensions
    {
        /// <summary>
        /// Gets the <see cref="GrabberScriptTypeAttribute"/> associated with the value.
        /// </summary>
        public static GrabberScriptTypeAttribute GetScriptTypeAttribute(this GrabberScriptType value, bool orDefault = true)
        {
            GrabberScriptTypeAttribute GetDefault()
                => orDefault ? GrabberScriptTypeAttribute.Default : null;

            var enumType = typeof(GrabberScriptType);
            var member = enumType.GetMember(value.ToString()).FirstOrDefault(m => m.DeclaringType == enumType);
            return member.GetCustomAttribute<GrabberScriptTypeAttribute>() ?? GetDefault();
        }
    }
}
