using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.New_Folder;
using UnityEngine;

namespace ECS.Scripts.Real
{

    public static class World
    {
        private static EntityList EntityArray { get; } = new(100);
        private static ComponentAnymap ComponentArrays { get; } = new();

        public static Entity CreateEntity()
        {
            return EntityArray.CreateEntity();
        }

        public static ref EntityComponentWrapped<T> AddComponent<T>(in Entity entity) where T : struct, IComponentECS
        {
            var component = new EntityComponentWrapped<T>(new T(), entity);
            ComponentArrays.Add(component);
            return ref GetComponent<T>(entity);
        }

        public static ref EntityComponentWrapped<T> GetComponent<T>(in Entity entity) where T : struct, IComponentECS
        {
            return ref ComponentArrays.Get<T>(entity);
        }
    }
    
 



    public static class WorldExtensions
    {
        public static void AddComponent<T>(this in Entity entity) where T : struct, IComponentECS
        {
            World.AddComponent<T>(entity);
        }
        public static ref EntityComponentWrapped<T> GetComponent<T>(this in Entity entity) where T : struct, IComponentECS
        {
           return ref World.GetComponent<T>(entity);
        }
    }
  
    
}