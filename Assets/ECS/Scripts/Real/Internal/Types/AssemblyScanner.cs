using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Internal.Types
{
    internal static class AssemblyScanner
    {
        internal static IEnumerable<TypeInfo> ScanFromCurrentlyExecutingAssembly<TScanForType>()
        {
            return Scan<TScanForType>(Assembly.GetExecutingAssembly());
        }

        internal static IEnumerable<TypeInfo> ScanForFromAssemblyContaining<TScanForType, TFromAssemblyContainingMarker>()
        {
            return Scan<TScanForType>(typeof(TFromAssemblyContainingMarker).Assembly);
        }

        internal static IEnumerable<TypeInfo> ScanForFromAssembliesContaining<TScanForType>(params Type[] assemblyMarkers)
        {
            return Scan<TScanForType>(assemblyMarkers.Select(m => m.Assembly).ToArray());
        }

        private static IEnumerable<TypeInfo> Scan<TScanForType>(params Assembly[] assemblies)
        {
            IEnumerable<TypeInfo> componentTypes = null;
            foreach (var assembly in assemblies)
            {
                // Get all concrete types implementing the IComponentECS interface
                componentTypes = assembly.DefinedTypes
                    .Where(IsConcreteAndAssignableFrom<TScanForType>);
            }

            return componentTypes;
        }

        public static bool IsConcreteAndAssignableFrom<TScanForType>(Type x)
        {
            return 
                typeof(TScanForType).IsAssignableFrom(x)
                && !x.IsInterface
                && !x.IsAbstract;
        }
    }
}