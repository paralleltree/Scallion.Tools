using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scallion.Tools.Essentials
{
    internal static class StaticInfo
    {
        internal static readonly string Name;
        internal static readonly string AssemblyVersion;

        static StaticInfo()
        {
            var asm = Assembly.GetEntryAssembly();
            Name = asm.GetName().Name;
            AssemblyVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}
