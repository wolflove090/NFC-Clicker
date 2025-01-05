using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    /// <summary>
    /// Manages the registration of debug commands
    /// </summary>
    public static class DebugCommandRegister
    {
        /// <summary>
        /// Event called when a debug command category is received.
        /// </summary>
        internal static event UnityAction OnAddCategory;

        /// <summary>
        /// List of registered categories
        /// </summary>
        internal static Dictionary<string, DebugCategory> CategoryTypes { get; } = new();

        /// <summary>
        /// A number to be appended as a suffix to duplicate category names.
        /// </summary>
        static readonly Dictionary<string, int> CategoryNameSuffixNumber = new();

        /// <summary>
        /// Destination for registering dynamic command definitions.
        /// </summary>
        internal static readonly DebugCommandDefinitionRegister DebugCommandDefinitionRegister = new();

        /// <summary>
        /// Cleanup process.
        /// </summary>
        internal static void Dispose()
        {
            OnAddCategory = null;
            CategoryTypes.Clear();
            CategoryNameSuffixNumber.Clear();
            DebugCommandDefinitionRegister.Dispose();
        }

        /// <summary>
        /// Registers a debug command category.
        /// </summary>
        /// <param name="categoryName">Specify the category name.</param>
        /// <param name="order">Specifies the order of the category. They are displayed in order from the smallest value on the category list.</param>
        /// <param name="displayName">Specify the display-only category name.</param>
        /// <typeparam name="T">Type of the category</typeparam>
        public static void AddCategory<T>(string categoryName = "", int order = int.MaxValue, string displayName = "")
            where T : DebugCategoryBase
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = typeof(T).Name;
            }

            int duplicationCount = _CountNameDuplication(categoryName);

            if (duplicationCount >= 1)
            {
                categoryName = $"{categoryName}-{duplicationCount}";
            }

            if (string.IsNullOrEmpty(displayName))
            {
                displayName = categoryName;
            }

            CategoryTypes.Add(categoryName, new DebugCategory(order, displayName, typeof(T)));
            DebugCommandRegister.OnAddCategory?.Invoke();
            DebugCommandPresenter.RefreshView();
        }

        /// <summary>
        /// Returns a reference to an instance of a registered category.
        /// </summary>
        /// <param name="categoryName">Specify the category name.</param>
        /// <typeparam name="T">Type of the category</typeparam>
        /// <returns>Returns a reference to an instance that matches the specified category name. If there is no matching category instance or it's a dynamic command category, return null.</returns>
        public static T GetCategoryInstance<T>(string categoryName = "")
            where T : DebugCategoryBase
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = typeof(T).Name;
            }

            return DebugCommandPresenter.GetCategoryInstance(categoryName) as T;
        }

        /// <summary>
        /// Counts the number of times the target category name is duplicated
        /// Returns the number of duplications
        /// </summary>
        /// <param name="name">Target category name</param>
        /// <returns>Returns the number of duplications</returns>
        static int _CountNameDuplication(string name)
        {
            if (!CategoryTypes.ContainsKey(name))
            {
                return 0;
            }

            if (DebugCommandRegister.CategoryNameSuffixNumber.ContainsKey(name))
            {
                DebugCommandRegister.CategoryNameSuffixNumber[name]++;
            }
            else
            {
                DebugCommandRegister.CategoryNameSuffixNumber[name] = 1;
            }

            return DebugCommandRegister.CategoryNameSuffixNumber[name];
        }

        /// <summary>
        /// Creates a property command definition that has only a string getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyStringProperty(string categoryName, string displayName,
                                                                    Func<string> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, getter.Invoke, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a sbyte getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlySByteProperty(string categoryName, string displayName,
                                                                   Func<sbyte> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a byte getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyByteProperty(string categoryName, string displayName,
                                                                  Func<byte> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a short getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyShortProperty(string categoryName, string displayName,
                                                                   Func<short> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only an ushort getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyUShortProperty(string categoryName, string displayName,
                                                                    Func<ushort> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only an int getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyIntProperty(string categoryName, string displayName,
                                                                 Func<int> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only an uint getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyUIntProperty(string categoryName, string displayName,
                                                                  Func<uint> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a long getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyLongProperty(string categoryName, string displayName,
                                                                  Func<long> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only an ulong getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyULongProperty(string categoryName, string displayName,
                                                                   Func<ulong> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a char getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyCharProperty(string categoryName, string displayName,
                                                                  Func<char> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a float getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyFloatProperty(string categoryName, string displayName,
                                                                   Func<float> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a double getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyDoubleProperty(string categoryName, string displayName,
                                                                    Func<double> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a decimal getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyDecimalProperty(string categoryName, string displayName,
                                                                     Func<decimal> getter,
                                                                     Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only a bool getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyBoolProperty(string categoryName, string displayName,
                                                                  Func<bool> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, () => getter.Invoke(), attributes);
        }

        /// <summary>
        /// Creates a property command definition that has only an Enum getter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateGetOnlyEnumProperty(string categoryName, string displayName,
                                                                  Func<Enum> getter, Attribute[] attributes = null)
        {
            return new GetOnlyPropertyCommandDefinition(categoryName, displayName, getter.Invoke, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a string getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">These are the attributes to be applied to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableStringProperty(string categoryName, string displayName,
                                                                    Func<string> getter, Action<string> setter,
                                                                    Attribute[] attributes = null)
        {
            return new StringPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a sbyte getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableSByteProperty(string categoryName, string displayName,
                                                                   Func<sbyte> getter, Action<sbyte> setter,
                                                                   Attribute[] attributes = null)
        {
            return new SBytePropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a byte getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableByteProperty(string categoryName, string displayName,
                                                                  Func<byte> getter, Action<byte> setter,
                                                                  Attribute[] attributes = null)
        {
            return new BytePropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a short getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableShortProperty(string categoryName, string displayName,
                                                                   Func<short> getter, Action<short> setter,
                                                                   Attribute[] attributes = null)
        {
            return new ShortPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has an ushort getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableUShortProperty(string categoryName, string displayName,
                                                                    Func<ushort> getter, Action<ushort> setter,
                                                                    Attribute[] attributes = null)
        {
            return new UShortPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has an int getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableIntProperty(string categoryName, string displayName,
                                                                 Func<int> getter, Action<int> setter,
                                                                 Attribute[] attributes = null)
        {
            return new IntPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has an uint getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableUIntProperty(string categoryName, string displayName,
                                                                  Func<uint> getter, Action<uint> setter,
                                                                  Attribute[] attributes = null)
        {
            return new UIntPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a long getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableLongProperty(string categoryName, string displayName,
                                                                  Func<long> getter, Action<long> setter,
                                                                  Attribute[] attributes = null)
        {
            return new LongPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has an ulong getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableULongProperty(string categoryName, string displayName,
                                                                   Func<ulong> getter, Action<ulong> setter,
                                                                   Attribute[] attributes = null)
        {
            return new ULongPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a char getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">These are the attributes to be applied to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableCharProperty(string categoryName, string displayName,
                                                                   Func<char> getter, Action<char> setter,
                                                                   Attribute[] attributes = null)
        {
            return new CharPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a float getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">These are the attributes to be applied to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableFloatProperty(string categoryName, string displayName,
                                                                   Func<float> getter, Action<float> setter,
                                                                   Attribute[] attributes = null)
        {
            return new FloatPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a double getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableDoubleProperty(string categoryName, string displayName,
                                                                    Func<double> getter, Action<double> setter,
                                                                    Attribute[] attributes = null)
        {
            return new DoublePropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a decimal getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">These are the attributes to be applied to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableDecimalProperty(string categoryName, string displayName,
                                                                   Func<decimal> getter, Action<decimal> setter,
                                                                   Attribute[] attributes = null)
        {
            return new DecimalPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has a bool getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">These are the attributes to be applied to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableBoolProperty(string categoryName, string displayName,
                                                                  Func<bool> getter, Action<bool> setter,
                                                                  Attribute[] attributes = null)
        {
            return new BoolPropertyCommandDefinition(categoryName, displayName, getter, setter, attributes);
        }

        /// <summary>
        /// Creates a property command definition that has an Enum getter and setter.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="getter">The getter method.</param>
        /// <param name="setter">The setter method.</param>
        /// <param name="attributes">These are the attributes to be applied to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMutableEnumProperty<T>(string categoryName, string displayName,
                                                                     Func<T> getter, Action<T> setter,
                                                                     Attribute[] attributes = null)
            where T : Enum
        {
            return new EnumPropertyCommandDefinition(
                categoryName, displayName, () => getter.Invoke(), value => setter.Invoke((T)value), typeof(T),
                attributes);
        }

        /// <summary>
        /// Creates a definition for a method command.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="method">The method to execute.</param>
        /// <param name="attributes">Attributes to be attached to the property.</param>
        /// <returns>Returns the created command definition.</returns>
        public static CommandDefinition CreateMethod(string categoryName, string displayName, Action method,
                                                     Attribute[] attributes = null)
        {
            return new MethodCommandDefinition(categoryName, displayName, method, attributes);
        }

        /// <summary>
        /// Creates a definition for a coroutine command.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="coroutine">The coroutine to be executed.</param>
        /// <param name="attributes">These are the attributes to be added to the property.</param>
        /// <returns>Returns the generated command definition.</returns>
        public static CommandDefinition CreateCoroutine(string categoryName, string displayName,
                                                        Func<IEnumerator> coroutine, Attribute[] attributes = null)
        {
            return new CoroutineCommandDefinition(categoryName, displayName, coroutine, attributes);
        }

        /// <summary>
        /// Creates a definition for a handle method command.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="method">The handle method to be executed.</param>
        /// <param name="attributes">These are the attributes to be added to the property.</param>
        /// <returns>Returns the generated command definition.</returns>
        public static CommandDefinition CreateHandleMethod(string categoryName, string displayName,
                                                           Func<MethodHandler> method, Attribute[] attributes = null)
        {
            return new HandleMethodCommandDefinition(categoryName, displayName, method, attributes);
        }

        /// <summary>
        /// Registers dynamic debug commands.
        /// </summary>
        /// <param name="commandDefinition">The definition of the command to be registered.</param>
        public static void AddCommand(CommandDefinition commandDefinition)
        {
            if (DebugCommandRegister.DebugCommandDefinitionRegister.Add(commandDefinition))
            {
                DebugCommandRegister.OnAddCategory?.Invoke();
                DebugCommandPresenter.RefreshView();
            }
        }

        /// <summary>
        /// Deletes registered dynamic debug commands.
        /// </summary>
        /// <param name="commandDefinition">The command definition specified at the time of registration.</param>
        public static void RemoveCommand(CommandDefinition commandDefinition)
        {
            if (DebugCommandRegister.DebugCommandDefinitionRegister.Remove(commandDefinition))
            {
                DebugCommandRegister.OnAddCategory?.Invoke();
                DebugCommandPresenter.RefreshView();
            }
        }

        /// <summary>
        /// It refreshes the rendering of the property
        /// </summary>
        public static void RefreshProperty()
        {
            DebugCommandPresenter.RefreshProperty();
        }

        /// <summary>
        /// Deletes the values of all saved properties.
        /// </summary>
        public static void DeleteAllSavedProperties()
        {
            string targetKeyFilter = NoaDebuggerPrefsDefine.PrefsKeyDebugCommandPropertiesPrefix;
            IEnumerable<string> targetKeys = NoaDebuggerPrefs.GetKeyListFilterAt(targetKeyFilter);

            foreach (string key in targetKeys)
            {
                NoaDebuggerPrefs.DeleteAt(key);
            }
        }

        /// <summary>
        /// Deletes the values of all saved properties in the specified category.
        /// </summary>
        /// <param name="categoryName">Category name</param>
        public static void DeleteAllPropertiesInCategory(string categoryName)
        {
            string prefix = NoaDebuggerPrefsDefine.PrefsKeyDebugCommandPropertiesPrefix;
            string delimiter = NoaDebuggerPrefsDefine.PrefsKeyDelimiter;
            string targetKeyFilter = $"{prefix}{delimiter}{categoryName}";
            IEnumerable<string> targetKeys = NoaDebuggerPrefs.GetKeyListFilterAt(targetKeyFilter);

            foreach (string key in targetKeys)
            {
                NoaDebuggerPrefs.DeleteAt(key);
            }
        }

        /// <summary>
        /// Deletes the saved property value of the specified category and property name.
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="propertyName">Property name</param>
        public static void DeleteSavedProperty(string categoryName, string propertyName)
        {
            string key = GetSavePropertyKey(categoryName, propertyName);
            NoaDebuggerPrefs.DeleteAt(key);
        }

        /// <summary>
        /// Returns the key to save the specified category and property name.
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Returns the key to save the specified category and property name.</returns>
        public static string GetSavePropertyKey(string categoryName, string propertyName)
        {
            string prefix = NoaDebuggerPrefsDefine.PrefsKeyDebugCommandPropertiesPrefix;
            string delimiter = NoaDebuggerPrefsDefine.PrefsKeyDelimiter;

            return $"{prefix}{delimiter}{categoryName}{delimiter}{propertyName}";
        }

        /// <summary>
        /// Toggles the interactable state of the commands those have specified tag.
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="commandTag">Tag name</param>
        /// <param name="isInteractable">Interactable state of the commands those have specified tag, where true toggles interactable.</param>
        public static void SetInteractable(string categoryName, string commandTag, bool isInteractable)
        {
            DebugCommandPresenter.SetCommandInteractable(categoryName, commandTag, isInteractable);
            DebugCommandPresenter.RefreshView();
        }

        /// <summary>
        /// Returns the interactable state of the specified commands those have specified tag.
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="commandTag">Tag name</param>
        /// <returns>Interactable state of the commands those have specified tag, where true means interactable.</returns>
        public static bool IsInteractable(string categoryName, string commandTag)
        {
            return DebugCommandPresenter.IsCommandInteractable(categoryName, commandTag);
        }

        /// <summary>
        /// Toggles the visibility of the commands those have specified tag.
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="commandTag">Tag name</param>
        /// <param name="isVisible">Visibility of the commands those have specified tag, where true toggles visible.</param>
        public static void SetVisible(string categoryName, string commandTag, bool isVisible)
        {
            DebugCommandPresenter.SetCommandVisible(categoryName, commandTag, isVisible);
            DebugCommandPresenter.RefreshView();
        }

        /// <summary>
        /// Returns the visibility of the specified commands those have specified tag.
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="commandTag">Tag name</param>
        /// <returns>Visibility of the commands those have specified tag, where true means visible.</returns>
        public static bool IsVisible(string categoryName, string commandTag)
        {
            return DebugCommandPresenter.IsCommandVisible(categoryName, commandTag);
        }

        /// <summary>
        /// Category information
        /// </summary>
        public struct DebugCategory
        {
            public readonly int _order;
            public readonly string _displayName;
            public readonly Type _type;

            public DebugCategory(int order, string displayName, Type type)
            {
                _order = order;
                _displayName = displayName;
                _type = type;
            }
        }
    }
}
