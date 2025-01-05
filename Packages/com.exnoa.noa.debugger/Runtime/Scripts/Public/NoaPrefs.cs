using System.Collections.Generic;

namespace NoaDebugger
{
    /// <summary>
    /// Handles data in NOA Debugger's custom storage area.
    /// </summary>
    public class NoaPrefs
    {
        /// <summary>
        /// Saves value with the string type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetString(string key, string value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetString(saveKey, value);
        }

        /// <summary>
        /// Saves value with the sbyte type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetSByte(string key, sbyte value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetSByte(saveKey, value);
        }

        /// <summary>
        /// Saves value with the byte type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetByte(string key, byte value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetByte(saveKey, value);
        }

        /// <summary>
        /// Saves value with the short type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetShort(string key, short value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetShort(saveKey, value);
        }

        /// <summary>
        /// Saves value with the ushort type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetUShort(string key, ushort value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetUShort(saveKey, value);
        }

        /// <summary>
        /// Saves value with the int type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetInt(string key, int value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetInt(saveKey, value);
        }

        /// <summary>
        /// Saves value with the uint type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetUInt(string key, uint value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetUInt(saveKey, value);
        }

        /// <summary>
        /// Saves value with the long type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetLong(string key, long value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetLong(saveKey, value);
        }

        /// <summary>
        /// Saves value with the ulong type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetULong(string key, ulong value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetULong(saveKey, value);
        }

        /// <summary>
        /// Saves value with the char type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetChar(string key, char value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetChar(saveKey, value);
        }

        /// <summary>
        /// Saves value with the float type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetFloat(string key, float value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetFloat(saveKey, value);
        }

        /// <summary>
        /// Saves value with the double type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetDouble(string key, double value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetDouble(saveKey, value);
        }

        /// <summary>
        /// Saves value with the decimal type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetDecimal(string key, decimal value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetDecimal(saveKey, value);
        }

        /// <summary>
        /// Saves value with the bool type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="value">Specifies the save value.</param>
        public static void SetBoolean(string key, bool value)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.SetBoolean(saveKey, value);
        }

        /// <summary>
        /// Retrieves value with the string type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static string GetString(string key, string defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetString(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the sbyte type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static sbyte GetSByte(string key, sbyte defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetSByte(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the byte type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static byte GetByte(string key, byte defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetByte(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the short type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static short GetShort(string key, short defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetShort(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the ushort type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static ushort GetUShort(string key, ushort defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetUShort(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the int type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static int GetInt(string key, int defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetInt(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the uint type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static uint GetUInt(string key, uint defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetUInt(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the long type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static long GetLong(string key, long defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetLong(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the ulong type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static ulong GetULong(string key, ulong defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetULong(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the char type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static char GetChar(string key, char defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetChar(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the float type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static float GetFloat(string key, float defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetFloat(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the double type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static double GetDouble(string key, double defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetDouble(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the decimal type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static decimal GetDecimal(string key, decimal defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetDecimal(saveKey, defaultValue);
        }

        /// <summary>
        /// Retrieves value with the bool type.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <param name="defaultValue">Specifies the default value when value cannot be retrieved.</param>
        public static bool GetBoolean(string key, bool defaultValue)
        {
            string saveKey = _GetSaveKey(key);
            return NoaDebuggerPrefs.GetBoolean(saveKey, defaultValue);
        }

        /// <summary>
        /// Deletes the value of the specified key.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        public static void DeleteAt(string key)
        {
            string saveKey = _GetSaveKey(key);
            NoaDebuggerPrefs.DeleteAt(saveKey);
        }

        /// <summary>
        /// Deletes all values saved via NoaPrefs.
        /// </summary>
        public static void DeleteAllSaveData()
        {
            List<string> targetKeys =
                NoaDebuggerPrefs.GetKeyListFilterAt(NoaDebuggerPrefsDefine.PrefsKeyNoaPrefsDataPrefix);

            foreach (string key in targetKeys)
            {
                NoaDebuggerPrefs.DeleteAt(key);
            }
        }

        /// <summary>
        /// Deletes all the values used by NoaDebugger Tool.
        /// </summary>
        public static void DeleteAllToolData()
        {
            NoaDebuggerPrefs.DeleteAllToolData();
        }

        /// <summary>
        /// Get the formatted save key.
        /// </summary>
        /// <param name="key">Specifies the save key.</param>
        /// <returns>Returns the dedicated save key.</returns>
        static string _GetSaveKey(string key)
        {
            string prefix = NoaDebuggerPrefsDefine.PrefsKeyNoaPrefsDataPrefix;
            string delimiter = NoaDebuggerPrefsDefine.PrefsKeyDelimiter;
            return $"{prefix}{delimiter}{key}";
        }
    }
}
