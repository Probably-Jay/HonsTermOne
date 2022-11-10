using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.New_Folder;
using JetBrains.Annotations;
using UnityEngine;

namespace ECS.Scripts.Real
{
    
    public class World
    {
        private ComponentArrayContainer componentArrays;
    }

    internal class ComponentArrayContainer
    {
        public void ScanForTypes()
        {
            var types = AssemblyMagic.ScanForFromAssembly();
            foreach (var type in types)
            {
                var containerClass = typeof(CustomList<>);
                var containerType = containerClass.MakeGenericType(type);
                
                var container =  (IComponentContainer)Activator.CreateInstance(containerType);
                
                mapping.Add(type, container);
            }
        }
        
        private readonly Dictionary<Type, IComponentContainer> mapping = new();

        [CanBeNull]
        public CustomList<T> GetList<T>() where T : struct, IComponentECS
        {
            if (!mapping.ContainsKey(typeof(T)))
            {
                return null;
            }

            var container = mapping[typeof(T)] as CustomList<T>;

            return container;
        }

        public void Add<T>(T item) where T : struct, IComponentECS
        {
            GetList<T>()!.Add(item);
        }
    }

    internal interface IComponentContainer
    {
       
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
        public static IEnumerable<TypeInfo> Scan(params Assembly[] assemblies)
        {
            IEnumerable<TypeInfo> componentTypes = null;
            foreach (var assembly in assemblies)
            {
                componentTypes = assembly.DefinedTypes.Where(x =>
                    typeof(IComponentECS).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }
            return componentTypes;
        }
        
    }
    
}