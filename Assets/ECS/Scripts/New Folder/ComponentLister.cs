using System;
using System.Collections;
using System.Collections.Generic;
using ECS.Scripts.Real;
using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Types;
using UnityEngine;

namespace ECS.Scripts.New_Folder
{
    public class ComponentLister : MonoBehaviour
    {
       // private readonly ComponentList<MyComponent> myList = new();
      //  private ComponentArrayContainer collection;

        private void Start()
        {
           // collection = new ComponentArrayContainer();

           // Init();
            //Foo();
        }

        void Init()
        {
            // myList.Add(new MyComponent{ });
            // myList.Add(new MyComponent{ });
            // myList.Add(new MyComponent{ });
            //
        }
        
        void Foo()
        {
            // ref var a =  ref myList.Get(0);
            //
            // a.Data = 10;
            // a.Data++;
            
            var sys = new System();

           // DoWork(sys, (ref MyComponent item) => item.Data++);


            
            
            
            // print("done");
            // foreach (ref var item in myList)
            // {
            //     print(item +", " + item.Data.ToString());
            // }
        }

        // private void DoWork(System sys, System.ActionRef<MyComponent> func)
        // {
        //     foreach (var item in sys.Foo2(myList.GetEnumerator(), func))
        //     {
        //         print(item + ", " + item.Data.ToString());
        //     }
        // } 
        
        // private void DoWork2(System.ActionRef<MyComponent> func)
        // {
        //     foreach (ref var item in myList)
        //     {
        //         func(ref item);
        //     }
        // }
    }


    public class System 
    {
        // public IEnumerable<MyComponent> Foo(ComponentList<MyComponent>.Enumerator enumerator)
        // {
        //     while (enumerator.MoveNext())
        //     {
        //        Boo();
        //        yield return enumerator.Current;
        //     }
        //
        //     void Boo()
        //     {
        //         ref var item = ref enumerator.Current;
        //         Debug.Log(item +", " + item.Data.ToString());
        //         item.Data++;
        //     }
        // }

        // public void Oop(ComponentList<MyComponent>.Enumerator enumerator, ActionRef<MyComponent> func)
        // {
        //     while (enumerator.MoveNext())
        //     {
        //         func(ref enumerator.Current);
        //     }
        // }
        //
         public delegate void ActionRef<T>(ref T item);
        //
        // public IEnumerable<MyComponent> Foo2(ComponentList<MyComponent>.Enumerator enumerator, ActionRef<MyComponent> func)
        // {
        //     while (enumerator.MoveNext())
        //     { 
        //         func(ref enumerator.Current); 
        //         yield return enumerator.Current;
        //     }
        // }
    }

    public class ComponentArray<T> where T : struct,  IComponentData 
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


    public struct MyComponentData : IComponentData
    {
  
        public int Data { get; set; }
        public bool Equals(MyComponentData other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IComponentData other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is MyComponentData other && Equals(other);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public Entity EntityID { get; }
        public void SetEntity(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}