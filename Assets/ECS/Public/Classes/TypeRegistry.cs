using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Internal.Types;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    /// <summary>
    /// Runtime registry of all user-defined Component and System types.
    /// </summary>
    public class TypeRegistry
    {
        /// <summary>
        /// The user defined component types
        /// </summary>
        public IReadOnlyCollection<TypeInfo> ComponentTypes { get; private set; } = new List<TypeInfo>();
        
        /// <summary>
        /// The user defined System types
        /// </summary>
        public IReadOnlyCollection<TypeInfo> SystemTypes { get; private set; } = new List<TypeInfo>();

        internal event Action OnUpdatedTypeRegistry;

        /// <summary>
        /// Register all user-defined Component and System types from the currently executing assembly (and no other)
        /// </summary>
        public void RegisterTypesFromCurrentlyExecutingAssembly()
        {
            ComponentTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<IComponentData>().ToList();
            SystemTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<ISystemLogic>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }
        
        /// <summary>
        /// Register all user-defined Component and System types from the assembly containing <typeparamref name="TMarker"/> 
        /// </summary>
        /// <typeparam name="TMarker">Any type that is a part of the assembly to be scanned</typeparam>
        public void RegisterTypesFromAssemblyContaining<TMarker>()
        {
            ComponentTypes = AssemblyScanner.ScanForFromAssemblyContaining<IComponentData,TMarker>().ToList();
            SystemTypes = AssemblyScanner.ScanForFromAssemblyContaining<ISystemLogic,TMarker>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        } 
        
        /// <summary>
        /// Register all user-defined Component and System types from the assembly/assemblies containing the type(s) <paramref name="assemblyMarker"/> and <paramref name="otherAssemblyMarkers"/> 
        /// </summary>
        /// <param name="assemblyMarker">Any type that is a part of the assembly to be scanned</param>
        /// <param name="otherAssemblyMarkers">Any other types that are a part of the assemblies to be scanned</param>
        public void RegisterTypesFromAssembliesContaining(Type assemblyMarker, [NotNull] params Type[] otherAssemblyMarkers)
        {
            var assemblyMarkers = new [] { assemblyMarker }.Concat(otherAssemblyMarkers).ToArray();
            ComponentTypes = AssemblyScanner.ScanForFromAssembliesContaining<IComponentData>(assemblyMarkers).ToList();
            SystemTypes = AssemblyScanner.ScanForFromAssembliesContaining<ISystemLogic>(assemblyMarkers).ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }
    }
}
