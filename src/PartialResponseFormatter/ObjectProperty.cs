using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PartialResponseFormatter
{
    internal class ObjectProperty
    {
        private readonly PropertyInfo property;
        private readonly Func<object, object> func;

        public ObjectProperty(PropertyInfo property, TreeNode tree)
        {
            this.property = property;
            ResponseName = ReflectionProvider.GetPropertyResponseName(property);
            Tree = tree;

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
            func = (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        public string ResponseName { get; }

        public TreeNode Tree { get; }

        //todo: slow, i know. shall we emit some IL-code for this?
        public object GetValue(object obj)
        {
            //Console.WriteLine($"Evaluating property {property.Name}");
            return func(obj);
            return property.GetValue(obj);
        }
    }
}