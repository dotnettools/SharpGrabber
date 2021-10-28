using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetTools.SharpGrabber.Grabbed;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Provides extension methods for <see cref="GrabResult"/>.
    /// </summary>
    public static class GrabResultExtensions
    {
        /// <summary>
        /// Gets all resources of type <typeparamref name="T"/>.
        /// </summary>
        public static IEnumerable<T> Resources<T>(this GrabResult result)
            where T : IGrabbed
        {
            return result.Resources.OfType<T>();
        }

        /// <summary>
        /// Gets the single resource of type <typeparamref name="T"/> if available; otherwise NULL.
        /// </summary>
        public static T Resource<T>(this GrabResult result)
            where T : IGrabbed
        {
            return result.Resources<T>().SingleOrDefault();
        }
    }
}