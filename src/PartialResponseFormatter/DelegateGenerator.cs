using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PartialResponseFormatter
{
    public static class DelegateGenerator
    {
        public static Func<object, object> GeneratePropertyGetter(PropertyInfo property)
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(object), new[] { typeof(object) }, property.DeclaringType, true);
            var il = method.GetILGenerator();
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, property.DeclaringType);
            var getMethod = property
                .DeclaringType
                .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .GetGetMethod(true);
            il.Emit(OpCodes.Callvirt, getMethod);
            if (getMethod.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, getMethod.ReturnType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, typeof(object));
            }
            il.Emit(OpCodes.Ret);

            return (Func<object, object>) method.CreateDelegate(typeof(Func<object, object>));
        }
    }
}