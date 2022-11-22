using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Internal.Types
{
    internal static class AssemblyScanner<TScannedFor>
    {
        internal static IEnumerable<TypeInfo> ScanFromCurrentlyExecutingAssembly()
        {
            return Scan(Assembly.GetExecutingAssembly());
        }

        internal static IEnumerable<TypeInfo> ScanForFromAssemblyContaining<TMarker>()
        {
            return Scan(typeof(TMarker).Assembly);
        }

        internal static IEnumerable<TypeInfo> ScanForFromAssembliesContaining(params Type[] assemblyMarkers)
        {
            return Scan(assemblyMarkers.Select(m => m.Assembly).ToArray());
        }

        private static IEnumerable<TypeInfo> Scan(params Assembly[] assemblies)
        {
            IEnumerable<TypeInfo> componentTypes = null;
            foreach (var assembly in assemblies)
            {
                // Get all concrete types implementing the IComponentECS interface
                componentTypes = assembly.DefinedTypes
                    .Where(x =>
                        typeof(TScannedFor).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }

            return componentTypes;
        }
    }
}