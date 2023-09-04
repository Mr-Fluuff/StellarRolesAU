using System;
using System.Linq.Expressions;

namespace StellarRoles;

public static class Il2CppHelpers
{
    private static class CastHelper<T> where T : Il2CppObjectBase
    {
        public static Func<IntPtr, T> Cast;
        static CastHelper()
        {
            System.Reflection.ConstructorInfo constructor = typeof(T).GetConstructor(new[] { typeof(IntPtr) });
            ParameterExpression ptr = Expression.Parameter(typeof(IntPtr));
            NewExpression create = Expression.New(constructor!, ptr);
            Expression<Func<IntPtr, T>> lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
            Cast = lambda.Compile();
        }
    }

    public static T CastFast<T>(this Il2CppObjectBase obj) where T : Il2CppObjectBase
    {
        if (obj is T casted) return casted;
        return obj.Pointer.CastFast<T>();
    }

    public static T CastFast<T>(this IntPtr ptr) where T : Il2CppObjectBase
    {
        return CastHelper<T>.Cast(ptr);
    }
}