using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PwC.Base.Common
{
    public static class AssemblyTypesCache
    {
        private static Type[] allTypes;

        static AssemblyTypesCache()
        {
            var executinAssembly = Assembly.GetExecutingAssembly();

            allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a == executinAssembly || a.GetCustomAttribute<RegisterAssemblyTypesInCacheAttribute>() != null)
                .SelectMany(a => a.GetTypesSafely())
                .ToArray();
        }

        public static IEnumerable<Type> AllTypes
        {
            get { return allTypes; }
        }

        private static IEnumerable<Type> GetTypesSafely(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }
    }
}
