using AOT;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace NoaDebugger
{
    sealed class NoaDebuggerPrefs
    {
        static readonly string FileName = "noa_debugger_prefs";

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void SetFsSyncfsToUnloadEvent(Action prefsSaveCallback);
#endif

        static NoaDebuggerPrefs _singletonValue;
        static NoaDebuggerPrefs Singleton
        {
            get
            {
                if (_singletonValue == null)
                {
                    _singletonValue = new NoaDebuggerPrefs();
                    _singletonValue._Load();
                }

                return _singletonValue;
            }
        }

        Dictionary<string, string> _prefsDictionary = new Dictionary<string, string>();

        string CachePath
        {
            get { return Path.Combine(Application.persistentDataPath, "NoaDebuggerCache"); }
        }

        NoaDebuggerPrefs()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SetFsSyncfsToUnloadEvent(NoaDebuggerPrefs.Save);
#endif
        }


        string _GetFilePath()
        {
            string result = Path.Combine(CachePath, FileName);
            return result;
        }

        void _Load()
        {
            string filePath = _GetFilePath();
            if (!File.Exists(filePath))
            {
                _prefsDictionary = new Dictionary<string, string>();
                return;
            }

            string json = "";
            try
            {
                json = File.ReadAllText(filePath);
            }
            catch (Exception)
            {
                json = "{\"keys\":[],\"values\":[]}";
                LogModel.DebugLogWarning("Failed to load prefs.json, so it has been initialized with default values.");
            }
            finally
            {
                if (string.IsNullOrEmpty(json))
                {
                    json = "{\"keys\":[],\"values\":[]}";
                    LogModel.DebugLogWarning("Successfully loaded prefs.json, but it was empty, so it has been initialized with default values.");
                }
            }
            var prefsDic = JsonUtility.FromJson<PrefsDictionary>(json);
            _prefsDictionary = PrefsDictionary.Deserialize(prefsDic);
        }

        void _SaveData()
        {
            string filePath = _GetFilePath();
            if (!File.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }

            var prefsDic = PrefsDictionary.Serialize(_prefsDictionary);
            string dataJson = JsonUtility.ToJson(prefsDic);
            File.WriteAllText(filePath, dataJson);

#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(filePath);
#endif
        }

        bool _HasKey(string key)
        {
            return _prefsDictionary.ContainsKey(key);
        }

        List<string> _GetKeyAll()
        {
            return _prefsDictionary.Keys.ToList();
        }

        string _Get(string key, string defaultValue)
        {
            if (!_prefsDictionary.ContainsKey(key))
            {
                return defaultValue;
            }

            return _prefsDictionary[key];
        }

        void _Set(string key, string value)
        {
            _prefsDictionary[key] = value;
        }

        void _DeleteAt(string key)
        {
            _prefsDictionary.Remove(key);
        }

        void _DeleteAll()
        {
            _prefsDictionary = new Dictionary<string, string>();
        }


        public static bool HasKey(string key)
        {
            return Singleton._HasKey(key);
        }

        public static List<string> GetKeyList()
        {
            return Singleton._GetKeyAll();
        }

        public static List<string> GetKeyListFilterAt(string prefix)
        {
            return Singleton._GetKeyAll().Where(key => key.StartsWith(prefix)).ToList();
        }

        public static List<string> GetKeyListForToolOnly()
        {
            string debugCommandPrefix = NoaDebuggerPrefsDefine.PrefsKeyDebugCommandPropertiesPrefix;
            string noaPrefsPrefix = NoaDebuggerPrefsDefine.PrefsKeyNoaPrefsDataPrefix;

            return Singleton._GetKeyAll().Where(key => !key.StartsWith(debugCommandPrefix) && !key.StartsWith(noaPrefsPrefix)).ToList();
        }

        public static string GetString(string key, string defaultValue)
        {
            return Singleton._Get(key, defaultValue);
        }

        public static sbyte GetSByte(string key, sbyte defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            sbyte.TryParse(value, out sbyte result);
            return result;
        }

        public static byte GetByte(string key, byte defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            byte.TryParse(value, out byte result);
            return result;
        }

        public static short GetShort(string key, short defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            short.TryParse(value, out short result);
            return result;
        }

        public static ushort GetUShort(string key, ushort defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            ushort.TryParse(value, out ushort result);
            return result;
        }

        public static int GetInt(string key, int defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            int.TryParse(value, out int result);
            return result;
        }

        public static uint GetUInt(string key, uint defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            uint.TryParse(value, out uint result);
            return result;
        }

        public static long GetLong(string key, long defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            long.TryParse(value, out long result);
            return result;
        }

        public static ulong GetULong(string key, ulong defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            ulong.TryParse(value, out ulong result);
            return result;
        }

        public static char GetChar(string key, char defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            char.TryParse(value, out char result);
            return result;
        }

        public static float GetFloat(string key, float defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            float.TryParse(value, out float result);
            return result;
        }

        public static double GetDouble(string key, double defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            double.TryParse(value, out double result);
            return result;
        }

        public static decimal GetDecimal(string key, decimal defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString(CultureInfo.InvariantCulture));
            decimal.TryParse(value, out decimal result);
            return result;
        }

        public static bool GetBoolean(string key, bool defaultValue)
        {
            string value = Singleton._Get(key, defaultValue.ToString());
            bool.TryParse(value, out bool result);
            return result;
        }

        public static void SetString(string key, string value)
        {
            Singleton._Set(key, value);
        }

        public static void SetSByte(string key, sbyte value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetByte(string key, byte value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetShort(string key, short value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetUShort(string key, ushort value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetInt(string key, int value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetUInt(string key, uint value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetLong(string key, long value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetULong(string key, ulong value)
        {
            Singleton._Set(key, value.ToString());
        }

        public static void SetChar(string key, char value)
        {
            Singleton._Set(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void SetFloat(string key, float value)
        {
            Singleton._Set(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void SetDouble(string key, double value)
        {
            Singleton._Set(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void SetDecimal(string key, decimal value)
        {
            Singleton._Set(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void SetBoolean(string key, bool value)
        {
            Singleton._Set(key, value.ToString());
        }

        [MonoPInvokeCallback(typeof(Action))]
        public static void Save()
        {
            Singleton._SaveData();
        }

        public static void DeleteAt(string key)
        {
            Singleton._DeleteAt(key);
        }

        public static void DeleteAllToolData()
        {
            List<string> targetKeys = NoaDebuggerPrefs.GetKeyListForToolOnly();

            foreach (string key in targetKeys)
            {
                NoaDebuggerPrefs.DeleteAt(key);
            }
        }

        public static void DeleteAll()
        {
            Singleton._DeleteAll();
        }
    }

    sealed class PrefsDictionary
    {
        public List<string> keys;
        public List<string> values;

        public static PrefsDictionary Serialize(Dictionary<string, string> dictionary)
        {
            var result = new PrefsDictionary()
            {
                keys = new List<string>(dictionary.Keys),
                values = new List<string>(dictionary.Values),
            };

            return result;
        }

        public static Dictionary<string, string> Deserialize(PrefsDictionary prefsDic)
        {
            int length = Math.Min(prefsDic.keys.Count, prefsDic.values.Count);
            var dic = new Dictionary<string, string>(length);

            for (int i = 0; i < length; i++)
            {
                dic.Add(prefsDic.keys[i], prefsDic.values[i]);
            }

            return dic;
        }
    }

    sealed class NoaDebuggerPrefsBehaviour : MonoBehaviour
    {
        public static void Initialize(Transform parent)
        {
            GameObject obj = new GameObject("NoaDebuggerPrefs");
            obj.gameObject.transform.parent = parent;
            obj.AddComponent<NoaDebuggerPrefsBehaviour>();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                NoaDebuggerPrefs.Save();
            }
        }

        void OnApplicationQuit()
        {
            NoaDebuggerPrefs.Save();
        }
    }
}
