using ECS.Public.Attributes;
using ECS.Public.Classes;

namespace ECS.Public.Interfaces
{
    /// <summary>
    /// A representation of an entity to a custom system
    /// </summary>
    public interface ISystemEntityView
    {
        /// <summary>
        /// The entity being operated on
        /// </summary>
        Entity Entity { get; }
        
        /// <summary>
        /// Get the component the system needs. This operation is very fast
        /// </summary>
        /// <typeparam name="T">The type of the component, must be a <see cref="IComponentData"/> struct</typeparam>
        /// <returns>A mutable reference to that component</returns>
        /// <remarks>This operation will fail if <typeparamref name="T"/> is not a type included in this system's <see cref="SystemOperatesOn"/></remarks>
        ref T GetComponent<T>() where T : struct, IComponentData; 
    }
}
