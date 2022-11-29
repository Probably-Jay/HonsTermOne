using ECS.Public.Classes;

namespace ECS.Public.Delegates
{
    /// <summary>
    /// Equivalent to <see cref="System.Action"/> but with a ref type parameter
    /// </summary>
    public delegate void ActionRef<T>(ref T item);
    /// <summary>
    /// Equivalent to <see cref="System.Func{T}"/> but with a ref type parameter
    /// </summary>
    public delegate TRet FunctionRef<out TRet,T1>(ref T1 item);
    /// <summary>
    /// <inheritdoc cref="ActionRef{T}"/>
    /// </summary>
    public delegate void EntityActionRef(ref Entity entity);
    /// <summary>
    /// <inheritdoc cref="FunctionRef{TRet,T1}"/>
    /// </summary>
    public delegate TRet EntityActionFunc<out TRet>(ref Entity entity);
}
