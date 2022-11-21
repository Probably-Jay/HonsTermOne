using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.New_Folder;
using UnityEngine;

namespace ECS.Scripts.Real
{

    public class World
    {
        private EntityContainer entityContainer;
    }
    
    public class EntityContainer
    {
        private EntityIDArray entityArray;
        private ComponentAnymap componentArrays;
    }

    internal class EntityIDArray
    {
        private ComponentList<Entity> entites;
        
    }


  
    
}