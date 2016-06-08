using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scallion.Tools.Essentials
{
    /// <summary>
    /// Contains several extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Retrieves a custom attribute applied to the assembly.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to retrieve</typeparam>
        /// <param name="src">An object to search for the attribute</param>
        /// <returns>
        /// A reference to the attribute of type <typeparamref name="T"/> that is applied to <paramref name="src"/>,
        /// or null if there is no such attribute
        /// </returns>
        public static T GetCustomAttribute<T>(this Assembly src) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(src, typeof(T));
        }
    }
}
