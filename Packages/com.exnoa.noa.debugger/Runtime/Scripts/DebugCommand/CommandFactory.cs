using System;
using System.Collections;
using System.Reflection;

namespace NoaDebugger.DebugCommand
{
    static class CommandFactory
    {
        public static ICommand CreateCommand(object obj, PropertyInfo property, string categoryName)
        {
            if (Attribute.GetCustomAttribute(property, typeof(CommandExcludeAttribute)) is CommandExcludeAttribute)
            {
                return null;
            }

            Type propertyType = property.PropertyType;
            string displayName = GetDisplayName(property);
            var attributes = (Attribute[]) property.GetCustomAttributes(typeof(Attribute), false);

            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod == null || !setMethod.IsPublic)
            {
                if (propertyType == typeof(string)
                    || propertyType == typeof(sbyte)
                    || propertyType == typeof(byte)
                    || propertyType == typeof(short)
                    || propertyType == typeof(ushort)
                    || propertyType == typeof(int)
                    || propertyType == typeof(uint)
                    || propertyType == typeof(long)
                    || propertyType == typeof(ulong)
                    || propertyType == typeof(char)
                    || propertyType == typeof(float)
                    || propertyType == typeof(double)
                    || propertyType == typeof(decimal)
                    || propertyType == typeof(bool)
                    || propertyType.BaseType == typeof(Enum))
                {
                    return new GetOnlyPropertyCommandBuilder(
                            categoryName,
                            displayName,
                            () => property.GetValue(obj),
                            attributes)
                        .Build();
                }
                return null;
            }

            string saveKey = (Attribute.GetCustomAttribute(property, typeof(SaveOnUpdateAttribute)) is not null)
                ? DebugCommandRegister.GetSavePropertyKey(categoryName, property.Name)
                : null;
            if (propertyType == typeof(string))
            {
                return new StringPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (string) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(sbyte))
            {
                return new SBytePropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (sbyte) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(byte))
            {
                return new BytePropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (byte) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(short))
            {
                return new ShortPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (short) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(ushort))
            {
                return new UShortPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (ushort) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(int))
            {
                return new IntPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (int) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(uint))
            {
                return new UIntPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (uint) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(long))
            {
                return new LongPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (long) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(ulong))
            {
                return new ULongPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (ulong) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(char))
            {
                return new CharPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (char) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(float))
            {
                return new FloatPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (float) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(double))
            {
                return new DoublePropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (double) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(decimal))
            {
                return new DecimalPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (decimal) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType == typeof(bool))
            {
                return new BoolPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (bool) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        attributes,
                        saveKey)
                    .Build();
            }
            if (propertyType.BaseType == typeof(Enum))
            {
                return new EnumPropertyCommandBuilder(
                        categoryName,
                        displayName,
                        () => (Enum) property.GetValue(obj),
                        value => property.SetValue(obj, value),
                        property.PropertyType,
                        attributes,
                        saveKey)
                    .Build();
            }
            return null;
        }

        public static ICommand CreateCommand(object obj, MethodInfo method, string categoryName)
        {
            if (Attribute.GetCustomAttribute(method, typeof(CommandExcludeAttribute)) is CommandExcludeAttribute)
            {
                return null;
            }

            Type returnType = method.ReturnType;
            string displayName = GetDisplayName(method);
            var attributes = (Attribute[]) method.GetCustomAttributes(typeof(Attribute), false);
            if (returnType == typeof(void))
            {
                return new MethodCommandBuilder(
                        categoryName,
                        displayName,
                        () => method.Invoke(obj, null),
                        attributes)
                    .Build();
            }
            if (returnType == typeof(IEnumerator))
            {
                return new CoroutineCommandBuilder(
                        categoryName,
                        displayName,
                        () => (IEnumerator) method.Invoke(obj, null),
                        attributes)
                    .Build();
            }
            if (returnType == typeof(MethodHandler))
            {
                return new HandleMethodCommandBuilder(
                        categoryName,
                        displayName,
                        () => (MethodHandler) method.Invoke(obj, null),
                        attributes)
                    .Build();
            }
            return null;
        }

        static string GetDisplayName(MemberInfo member)
        {
            return (Attribute.GetCustomAttribute(member, typeof(DisplayNameAttribute))
                is DisplayNameAttribute attribute)
                ? attribute._name
                : member.Name;
        }
    }
}
