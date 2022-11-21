﻿using System;
using System.Collections.Generic;
using ECS.Scripts.Real.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace ECS.Scripts.Real
{
    internal interface IAnyEntityComponentContainer
    {
    }


    internal interface IEntityComponentWrapped : IEntityComponentECS
    {
    }

    public struct EntityComponentWrapped<T> : IEntityComponentWrapped where T : struct, IComponentECS
    {
        public Entity EntityID { get; }
        public T Component;

        public EntityComponentWrapped(T componentEcs, Entity entity)
        {
            Component = componentEcs;
            EntityID = entity;
        }

        public override string ToString()
        {
            return $"Entity component {EntityID.ToString()} component: {Component.ToString()}";
        }
    }


    internal interface IComponentContainer<T> : IAnyEntityComponentContainer  where T :  struct, IEntityComponentWrapped
    {
        void Add(T newComponent);
        ref T Get(in Entity entity);
    }

    internal class ComponentList<T> : IComponentContainer<T> where T : struct, IEntityComponentWrapped
    {
        private readonly NonBoxingList<T> list;
        public ComponentList(ulong? initialCapacity)
        {
            list = new NonBoxingList<T>(initialCapacity);
        }
        
        public void Add(T newComponent)
        {
            if(ElementAtIndexIsValidComponentOfEntity(newComponent.EntityID))
                throw new Exception("Entity already has component attached");
            
            list.Add(newComponent);
        }

        public ref T Get(in Entity entity)
        {
            if (!ElementAtIndexIsValidComponentOfEntity(entity)) 
                return ref NullEntityRef;
            
            return ref list[entity.IdIndex];
        }

        private bool ElementAtIndexIsValidComponentOfEntity(in Entity newComponentEntityID)
        {
            if (newComponentEntityID.IsNullEntity())
                throw new EntityNullException();
            
            // component at this entity index is out of array bounds
            if(list.IndexOutOfRange(newComponentEntityID.IdIndex))
                return false;

            var existingComponentEntityID = list[newComponentEntityID.IdIndex];

            // component at this entity index is null
            if (existingComponentEntityID.EntityID.IsNullEntity())
                return false;

            // component at this entity index was valid, but that entity no longer exists and it's id has been reused
            if(existingComponentEntityID.EntityID.IsSupersededBy(newComponentEntityID))
                return false;

            return true;
        }


        private ref T NullEntityRef
        {
            get
            {
                list[0] = new(); // First element is guaranteed to always be nullEntity
                return ref list[0];
            }
        }
    }
    
    internal class NonBoxingList<T> where T : struct, IEntityComponentECS
    {
        private T[] data;
        private ulong SizeReserved => (ulong)data.Length;

        public NonBoxingList(ulong? initialCapacity)
        {
            data = initialCapacity.HasValue 
                ? new T[BitOperations.RoundUpToPowerOf2(initialCapacity.Value)] 
                : new T[128];
        }
        
        public void Add(T element)
        {
            var index = element.EntityID.IdIndex;

            if (IndexOutOfRange(index)) 
                Reserve(index);

            data[index] = element;
        }

        public bool IndexOutOfRange(ulong index) => index >= SizeReserved;

        private void Reserve(ulong dataLength)
        {
            if(dataLength > SizeReserved)
            {
                Array.Resize(ref data, (int)BitOperations.RoundUpToPowerOf2(dataLength));
            }
        }

        public ref T this[ulong index] => ref data[index];
        

        public Enumerator GetEnumerator() => new(this);

        public class Enumerator
        {
            private readonly NonBoxingList<T> data;
            public Enumerator(NonBoxingList<T> data)
            {
                this.data = data;
            }

            private ulong index = 0; // intentionally start at 0 to skip 0, first valid index always 1
            
            //todo make this valid only?
            public bool MoveNext() => ++index < data.SizeReserved;

            public void Reset() => index = 0;
            public ref T Current => ref data[(ulong)index];

            public void Dispose()
            { }
        }

    }
}