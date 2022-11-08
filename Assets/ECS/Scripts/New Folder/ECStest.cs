using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;




public class ECStest : MonoBehaviour
{

   private List<ContiguousData> data;
   
    // Start is called before the first frame update
    void Start()
    {
        var entityData = new ContiguousData
        {
            [10] = 10
        };
        data.Add(entityData);
    }

    void Update()
    {
        
    }
}

internal unsafe struct ContiguousData
{
    public const int Size = 100;
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = Size)]
    private fixed byte data[Size];

    public List<int> A;
    
    public byte this[int index]
    {
        get
        {
            RangeAssertion(index);
            return data[index];
        }
        set
        {
            RangeAssertion(index);
            data[index]= value;
        }
    }

    private static void RangeAssertion(int index)
    {
        if (index is < 0 or >= 100)
            throw new IndexOutOfRangeException();
    }
}
