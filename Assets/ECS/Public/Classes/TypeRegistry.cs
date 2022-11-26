using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Internal.Types;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    public class TypeRegistry
    {
        public IReadOnlyCollection<TypeInfo> ComponentTypes { get; private set; } = new List<TypeInfo>();
        public IReadOnlyCollection<TypeInfo> SystemTypes { get; private set; } = new List<TypeInfo>();

        internal event Action OnUpdatedTypeRegistry;

        public void RegisterTypesFromCurrentlyExecutingAssembly()
        {
            ComponentTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<IComponentData>().ToList();
            SystemTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<ISystemLogic>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }
        public void RegisterTypesFromAssemblyContaining<TMarker>()
        {
            ComponentTypes = AssemblyScanner.ScanForFromAssemblyContaining<IComponentData,TMarker>().ToList();
            SystemTypes = AssemblyScanner.ScanForFromAssemblyContaining<ISystemLogic,TMarker>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        } 
        public void RegisterTypesFromAssembliesContaining(Type assemblyMarker, [NotNull] params Type[] otherAssemblyMarkers)
        {
            var assemblyMarkers = new [] { assemblyMarker }.Concat(otherAssemblyMarkers).ToArray();
            ComponentTypes = AssemblyScanner.ScanForFromAssembliesContaining<IComponentData>(assemblyMarkers).ToList();
            SystemTypes = AssemblyScanner.ScanForFromAssembliesContaining<ISystemLogic>(assemblyMarkers).ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }
    }
}
