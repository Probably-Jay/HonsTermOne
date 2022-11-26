using System;
using System.Reflection;
using ECS.Internal.Exceptions;
using JetBrains.Annotations;

namespace ECS.Internal.Extensions
{
    internal static class TypeParameterToGenericFunctionHelper
    {
        public static object CallGenericFunctionFromType(this object callingObject, Type type, string function, params object[] parameters)
        {
            try
            {
                var method = GetMethod(callingObject, type, function);
                return CallMethod(callingObject, method, parameters);
            }
            catch (NullReferenceException)
            {
                throw new InvalidTypeListException();
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException!;
            }
        }

        
        [NotNull]
        private static MethodInfo GetMethod([NotNull] object callingObject, Type type, [NotNull] string function)
        {
            var method = callingObject.GetType()
                .GetMethod(function, BindingFlags.Instance | BindingFlags.NonPublic)
                !.MakeGenericMethod(type)!;
            return method;
        }

        private static object CallMethod(object callingObject, [NotNull] MethodBase method, params object[] parameters)
        {
            return method!.Invoke(callingObject, parameters);
        }

    }
}