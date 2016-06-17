using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Confirms that a user allows you to overwrite the specified file.
        /// </summary>
        /// <param name="path">The file path to confirm</param>
        /// <returns>true if the user allowed to overwrite the file or it does not exist; otherwise, false.</returns>
        public static bool ConfirmOverwrite(this string path)
        {
            if (!File.Exists(path)) return true;

            Console.WriteLine("出力先のファイル {0} は既に存在しています。", path);
            while (true)
            {
                Console.Write("上書きしますか？ (y/N) > ");
                string res = Console.ReadLine();
                if (res == "" || Regex.IsMatch(res, "^(y|n)$", RegexOptions.IgnoreCase))
                {
                    return res.ToLower() == "y";
                }
            }
        }
    }
}
