using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ECS.Scripts.Real
{
    internal class ComponentAnymap
    {
        private readonly Dictionary<Type, IAnyEntityComponentContainer> mapping;

        public ComponentAnymap()
        {
            var mapper = new ComponentMapper();
            mapping = mapper.ScanForTypes();
        }

        public void Add<T>(T item) where T : struct, IComponentECS
        {
            GetList<T>()!.Add(item);
        }

        public ref T Get<T>(in Entity entity) where T : struct, IComponentECS
        {
            return ref GetList<T>()!.Get(entity);
        }

        [CanBeNull]
        private IComponentContainer<T> GetList<T>() where T : struct, IComponentECS
        {
            if (!mapping.ContainsKey(typeof(T)))
                return null;

            return mapping[typeof(T)] as IComponentContainer<T>;
        }
    }

    internal class ComponentMapper
    {
        public Dictionary<Type, IAnyEntityComponentContainer> ScanForTypes()
        {
            var componentAnymap = new Dictionary<Type, IAnyEntityComponentContainer>();
            var types = AssemblyMagic.ScanForFromAssembly();
            foreach (var type in types)
            {
                var containerClass = typeof(ComponentList<>);

                var containerType = containerClass.MakeGenericType(type);

                var reserveSize = type.GetCustomAttribute<ReserveInComponentArray>()?.ReserveSize;

                var container =  (IAnyEntityComponentContainer)Activator.CreateInstance(containerType, reserveSize);

                componentAnymap.Add(type, container);
            }

            return componentAnymap;
        }
    }

    public static class AssemblyMagic
    {
        public static IEnumerable<TypeInfo> ScanForFromAssembly()
        {
            return Scan(Assembly.GetExecutingAssembly());
        }
        public static IEnumerable<TypeInfo> ScanForFromAssemblyContaining<TMarker>()
        {
            return Scan(typeof(TMarker).Assembly);
        }  

        private static IEnumerable<TypeInfo> Scan(params Assembly[] assemblies)
        {
            IEnumerable<TypeInfo> componentTypes = null;
            foreach (var assembly in assemblies)
            {
                // Get all concrete types implementing the IComponentECS interface
                componentTypes = assembly.DefinedTypes.Where(x =>
                    typeof(IComponentECS).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }
            return componentTypes;
        }
        
    }
}