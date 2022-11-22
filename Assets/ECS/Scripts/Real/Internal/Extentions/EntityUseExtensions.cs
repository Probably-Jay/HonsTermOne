﻿using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;

namespace ECS.Scripts.Real.Internal.Extentions
{
    public static class EntityUseExtensions
    {
        public static void AddComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            World.AddComponent<T>(entity);
        }
        public static ref Component<T> GetComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return ref World.GetComponent<T>(entity);
        }   
        public static void DestroyFromWorld(this ref Entity entity)
        {
            World.DestroyEntity(ref entity);
        }
    }
}