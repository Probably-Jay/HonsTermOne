using System;
using System.Collections;
using System.Collections.Generic;
using ECS.Scripts.Real;
using UnityEngine;

namespace ECS.Scripts.New_Folder
{
    public class ComponentLister : MonoBehaviour
    {
        private readonly CustomList<MyComponent> myList = new();
      //  private ComponentArrayContainer collection;

        private void Start()
        {
           // collection = new ComponentArrayContainer();

           // Init();
            //Foo();
        }

        void Init()
        {
            myList.Add(new MyComponent{ });
            myList.Add(new MyComponent{ });
            myList.Add(new MyComponent{ });
 
        }
        
        void Foo()
        {
            // ref var a =  ref myList.Get(0);
            //
            // a.Data = 10;
            // a.Data++;
            
            var sys = new System();

            DoWork(sys, (ref MyComponent item) => item.Data++);


            
            
            
            // print("done");
            // foreach (ref var item in myList)
            // {
            //     print(item +", " + item.Data.ToString());
            // }
        }

        private void DoWork(System sys, System.ActionRef<MyComponent> func)
        {
            foreach (var item in sys.Foo2(myList.GetEnumerator(), func))
            {
                print(item + ", " + item.Data.ToString());
            }
        } 
        
        private void DoWork2(System.ActionRef<MyComponent> func)
        {
            foreach (ref var item in myList)
            {
                func(ref item);
            }
        }
    }


    public class System 
    {
        public IEnumerable<MyComponent> Foo(CustomList<MyComponent>.Enumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
               Boo();
               yield return enumerator.Current;
            }

            void Boo()
            {
                ref var item = ref enumerator.Current;
                Debug.Log(item +", " + item.Data.ToString());
                item.Data++;
            }
        }

        public void Oop(CustomList<MyComponent>.Enumerator enumerator, ActionRef<MyComponent> func)
        {
            while (enumerator.MoveNext())
            {
                func(ref enumerator.Current);
            }
        }

        public delegate void ActionRef<T>(ref T item);
        
        public IEnumerable<MyComponent> Foo2(CustomList<MyComponent>.Enumerator enumerator, ActionRef<MyComponent> func)
        {
            while (enumerator.MoveNext())
            { 
                func(ref enumerator.Current); 
                yield return enumerator.Current;
            }
        }
    }
    
    public class CustomList<T> : IComponentContainer where T :struct, IComponentECS
    {
        private T[] data;
        public int Count { get; private set; }

        public CustomList() : this(4)
        {
        }
        public CustomList(int initialCapacity)
        {
            data = new T[initialCapacity];
        }
        
        public void Add(T element)
        {
            if (Count >= data.Length)
            {
                Array.Resize(ref data, data.Length * 2);
            }

            data[Count++] = element;
        }

        public ref T Get(int index) => ref data[index];
        public ref T this[int index] => ref Get(index);

        public Enumerator GetEnumerator() => new(this);

        public class Enumerator
        {
            private readonly CustomList<T> data;
            public Enumerator(CustomList<T> data) => this.data = data;

            private int index = -1;
            public bool MoveNext() => ++index < data.Count;

            public void Reset() => index = -1;
            public ref T Current => ref data.Get(index);

            public void Dispose()
            { }
        }
    }

    public class ComponentArray<T> where T : struct,  IComponentECS 
    {
        private T[] data;

        // ComponentView<T> Get(int i)
        // {
        //     Span<T> listSpan = System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref data[i], 1);
        //     return new ComponentView<T>(listSpan);
        // }

        // void Foo()
        // {
        //     var a = Get(1);
        //
        //     data[0] = default;
        // }
        
    }

    readonly ref struct ComponentView<T> 
    {
        private readonly Span<T> data;
        public ComponentView(Span<T> data)
        {
            this.data = data;
        }
        public ref T Data => ref data[0];
    }
        
        
    public interface IComponentECS : IEquatable<IComponentECS>
    {
        
    }

    public struct MyComponent : IComponentECS
    {
  
        public int Data { get; set; }
        public bool Equals(MyComponent other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IComponentECS other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is MyComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}