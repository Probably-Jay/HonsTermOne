using System;
using System.Collections.Generic;
using ECS.Scripts.Real.Helper;
using JetBrains.Annotations;
using UnityEngine;

namespace ECS.Scripts.Real
{
    internal interface IAnyEntityComponentContainer
    {
    }
    
    internal interface IComponentContainer<T> : IAnyEntityComponentContainer where T : struct, IEntityComponentECS
    {
        void Add(T newComponent);
        ref T Get(in Entity entity);
    }
    
    internal class ComponentList<T> : IComponentContainer<T> where T :  struct, IEntityComponentECS
    {
        private readonly NonBoxingList<T> list;
        public ComponentList(int? initialCapacity)
        {
            list = new NonBoxingList<T>(initialCapacity);
        }
        
        public void Add(T newComponent)
        {
            var newComponentEntityID = newComponent.EntityID;
            if (newComponentEntityID.IsNullEntity())
                throw new EntityNullException();
            
            var existingComponentEntityID = list.GetEntityOwningObjectAtIndex(newComponentEntityID.IdIndex);

            if (existingComponentEntityID.IsNullEntity())
            {
                list.Add(newComponent);
                return;
            }

            if(existingComponentEntityID.IsSupersededBy(newComponentEntityID))
            {
                list.Add(newComponent);
                return;
            }

            throw new Exception("Entity already has component attached");
        }

        public ref T Get(in Entity entity)
        {
            var entityOwningObjectAtIndex = list.GetEntityOwningObjectAtIndex(entity.IdIndex);
            if (entityOwningObjectAtIndex != entity) 
                return ref list.NullEntity;
            return ref list.GetByRef(entity.IdIndex);
        }
    }
    
    internal class NonBoxingList<T> where T : struct, IEntityComponentECS
    {
        private T[] data;
        public NonBoxingList(int? initialCapacity)
        {
            data = new T[initialCapacity ?? 128];
        }

        // private NonBoxingList() : this(null)
        // { }

        public void Add(T element)
        {
            var index = element.EntityID.IdIndex;
            
            if (index == 0)
                throw new EntityNullException();
            
            if (IndexOutOfRange(index)) 
                Reserve(index);

            data[index] = element;
        }

        private bool IndexOutOfRange(ulong index)
        {
            if (index <= 0)
                return true;
            return index >= (ulong)data.Length;
        }

        private void Reserve(ulong dataLength)
        {
            if(dataLength > (ulong)data.Length)
                Resize(BitOperations.RoundUpToPowerOf2(dataLength));
        }
        private void Resize(ulong dataLength) => Array.Resize(ref data, (int)dataLength);

        public ref T GetByRef(ulong index)
        {
            if (IndexOutOfRange(index))
                return ref NullEntity;
            return ref data[index];
        } 
        public Entity GetEntityOwningObjectAtIndex(ulong index)
        {
            if (IndexOutOfRange(index))
                return NullEntity.EntityID; 
            return data[index].EntityID;
        }

        public ref T NullEntity
        {
            get
            {
                data[0] = new(); // First element is guaranteed to always be nullEntity
                return ref data[0];
            }
        }

        //  public ref T this[ulong index] => ref GetByRef(index);

        public Enumerator GetEnumerator() => new(this);

        public class Enumerator
        {
            private readonly NonBoxingList<T> data;
            public Enumerator(NonBoxingList<T> data) => this.data = data;

            private long index = -1;
            
            //todo make this valid only
            public bool MoveNext() => ++index < data.Count;

            public void Reset() => index = -1;
            public ref T Current => ref data.GetByRef((ulong)index);

            public void Dispose()
            { }
        }

        private long Count => data.Length;
    }
}