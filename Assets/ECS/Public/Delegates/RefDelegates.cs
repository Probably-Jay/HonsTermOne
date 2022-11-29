using ECS.Public.Classes;

namespace ECS.Public.Delegates
{
    public delegate void ActionRef<T>(ref T item);
    public delegate TRet FunctionRef<out TRet,T1>(ref T1 item);
    public delegate void EntityActionRef(ref Entity entity);
    public delegate TRet EntityActionFunc<out TRet>(ref Entity entity);
}
