using JetBrains.Annotations;
using NoaDebugger.DebugCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    sealed class NoaDebuggerSettings : ScriptableObject
    {
        [SerializeField]
        ButtonPosition _startButtonPosition = NoaDebuggerDefine.DEFAULT_START_BUTTON_POSITION;
        [SerializeField]
        float _startButtonScale = NoaDebuggerDefine.DEFAULT_START_BUTTON_SCALE;
        [SerializeField]
        ButtonMovementType _startButtonMovementType = NoaDebuggerDefine.DEFAULT_START_BUTTON_MOVEMENT_TYPE;
        [SerializeField]
        bool _saveStartButtonPosition = NoaDebuggerDefine.DEFAULT_SAVE_START_BUTTON_POSITION;
        [SerializeField]
        float _toolStartButtonAlpha = NoaDebuggerDefine.TOOL_START_BUTTON_ALPHA_DEFAULT;
        [SerializeField]
        float _backgroundAlpha = NoaDebuggerDefine.CANVAS_ALPHA_DEFAULT;
        [SerializeField]
        float _floatingWindowAlpha = NoaDebuggerDefine.CANVAS_ALPHA_DEFAULT;
        [SerializeField]
        int _noaDebuggerCanvasSortOrder = NoaDebuggerDefine.NOA_DEBUGGER_CANVAS_SORT_ORDER_DEFAULT;

        [SerializeField]
        bool _isUIReversePortrait = NoaDebuggerDefine.IS_UI_REVERSE_PORTRAIT_DEFAULT;

        [SerializeField]
        List<MenuInfo> _menuList;
        [SerializeField]
        bool _autoCreateEventSystem = NoaDebuggerDefine.DEFAULT_AUTO_CREATE_EVENT_SYSTEM;

        [SerializeField]
        ErrorNotificationType _errorNotificationType = NoaDebuggerDefine.DEFAULT_ERROR_NOTIFICATION_TYPE;

        [SerializeField]
        bool _autoInitialize = NoaDebuggerDefine.DEFAULT_AUTO_INITIALIZE;
        [SerializeField]
        List<CustomMenuInfo> _customMenuList;
        [SerializeField]
        int _consoleLogCount = NoaDebuggerDefine.CONSOLE_LOG_COUNT_DEFAULT;
        [SerializeField]
        int _apiLogCount = NoaDebuggerDefine.API_LOG_COUNT_DEFAULT;

        [SerializeField]
        bool _isCustomFontSettingsEnabled = NoaDebuggerDefine.IS_CUSTOM_FONT_SETTINGS_ENABLED_DEFAULT;
        [SerializeField]
        TMP_FontAsset _fontAsset;
        [SerializeField]
        Material _fontMaterial;
        [SerializeField]
        float _fontSizeRate = NoaDebuggerDefine.DEFAULT_FONT_SIZE_RATE;

        [SerializeField]
        int _hierarchyLevels = NoaDebuggerDefine.DEFAULT_HIERARCHY_LEVELS;

        [SerializeField]
        CommandDisplayFormat _commandFormatLandscape = NoaDebuggerDefine.DEFAULT_COMMAND_FORMAT_LANDSCAPE;
        [SerializeField]
        CommandDisplayFormat _commandFormatPortrait = NoaDebuggerDefine.DEFAULT_COMMAND_FORMAT_PORTRAIT;
        [SerializeField]
        bool _autoSave = NoaDebuggerDefine.DEFAULT_AUTO_SAVE;

        public ButtonPosition StartButtonPosition
        {
            get => _startButtonPosition;
            set => _startButtonPosition = value;
        }

        public float StartButtonScale
        {
            get => _startButtonScale;
            set => _startButtonScale = value;
        }

        public ButtonMovementType StartButtonMovementType
        {
            get => _startButtonMovementType;
            set => _startButtonMovementType = value;
        }

        public bool SaveStartButtonPosition
        {
            get => _saveStartButtonPosition;
            set => _saveStartButtonPosition = value;
        }

        public float ToolStartButtonAlpha
        {
            get => _toolStartButtonAlpha;
            set => _toolStartButtonAlpha = value;
        }

        public float BackgroundAlpha
        {
            get => _backgroundAlpha;
            set => _backgroundAlpha = value;
        }

        public float FloatingWindowAlpha
        {
            get => _floatingWindowAlpha;
            set => _floatingWindowAlpha = value;
        }

        public int NoaDebuggerCanvasSortOrder
        {
            get => _noaDebuggerCanvasSortOrder;
            set => _noaDebuggerCanvasSortOrder = value;
        }

        public bool IsUIReversePortrait
        {
            get => _isUIReversePortrait;
            set => _isUIReversePortrait = value;
        }

        public List<MenuInfo> MenuList
        {
            get => _menuList;
            set => _menuList = value;
        }

        public List<CustomMenuInfo> CustomMenuList
        {
            get => _customMenuList;
            set => _customMenuList = value;
        }

        public bool AutoCreateEventSystem
        {
            get => _autoCreateEventSystem;
            set => _autoCreateEventSystem = value;
        }

        public ErrorNotificationType ErrorNotificationType
        {
            get => _errorNotificationType;
            set => _errorNotificationType = value;
        }

        public bool AutoInitialize
        {
            get => _autoInitialize;
            set => _autoInitialize = value;
        }

        public int ConsoleLogCount
        {
            get => _consoleLogCount;
            set => _consoleLogCount = value;
        }

        public int ApiLogCount
        {
            get => _apiLogCount;
            set => _apiLogCount = value;
        }

        public bool IsCustomFontSpecified => IsCustomFontSettingsEnabled && FontAsset != null;

        public bool IsCustomFontSettingsEnabled
        {
            get => _isCustomFontSettingsEnabled;
            set => _isCustomFontSettingsEnabled = value;
        }

        public TMP_FontAsset FontAsset
        {
            get => _fontAsset;
            set => _fontAsset = value;
        }

        public Material FontMaterial
        {
            get => _fontMaterial;
            set => _fontMaterial = value;
        }

        public float FontSizeRate
        {
            get => _fontSizeRate;
            set => _fontSizeRate = value;
        }

        public int HierarchyLevels
        {
            get => _hierarchyLevels;
            set => _hierarchyLevels = value;
        }

        public CommandDisplayFormat CommandFormatLandscape
        {
            get => _commandFormatLandscape;
            set => _commandFormatLandscape = value;
        }

        public CommandDisplayFormat CommandFormatPortrait
        {
            get => _commandFormatPortrait;
            set => _commandFormatPortrait = value;
        }

        public bool AutoSave
        {
            get => _autoSave;
            set => _autoSave = value;
        }

#if NOA_DEBUGGER
        public NoaDebuggerSettings Init()
        {
            StartButtonPosition = NoaDebuggerDefine.DEFAULT_START_BUTTON_POSITION;
            StartButtonScale = NoaDebuggerDefine.DEFAULT_START_BUTTON_SCALE;
            StartButtonMovementType = NoaDebuggerDefine.DEFAULT_START_BUTTON_MOVEMENT_TYPE;
            SaveStartButtonPosition = NoaDebuggerDefine.DEFAULT_SAVE_START_BUTTON_POSITION;

            ToolStartButtonAlpha = NoaDebuggerDefine.TOOL_START_BUTTON_ALPHA_DEFAULT;
            MenuList = NoaDebuggerSettings.GetDefaultMenuSettings();

            CustomMenuList = new List<CustomMenuInfo>() { };

            AutoInitialize = NoaDebuggerDefine.DEFAULT_AUTO_INITIALIZE;

            AutoCreateEventSystem = NoaDebuggerDefine.DEFAULT_AUTO_CREATE_EVENT_SYSTEM;
            ErrorNotificationType = NoaDebuggerDefine.DEFAULT_ERROR_NOTIFICATION_TYPE;

            BackgroundAlpha = NoaDebuggerDefine.CANVAS_ALPHA_DEFAULT;
            FloatingWindowAlpha = NoaDebuggerDefine.CANVAS_ALPHA_DEFAULT;

            NoaDebuggerCanvasSortOrder = NoaDebuggerDefine.NOA_DEBUGGER_CANVAS_SORT_ORDER_DEFAULT;

            IsUIReversePortrait = NoaDebuggerDefine.IS_UI_REVERSE_PORTRAIT_DEFAULT;

            ConsoleLogCount = NoaDebuggerDefine.CONSOLE_LOG_COUNT_DEFAULT;
            ApiLogCount = NoaDebuggerDefine.API_LOG_COUNT_DEFAULT;

            IsCustomFontSettingsEnabled = NoaDebuggerDefine.IS_CUSTOM_FONT_SETTINGS_ENABLED_DEFAULT;
            FontAsset = null;
            FontMaterial = null;
            FontSizeRate = NoaDebuggerDefine.DEFAULT_FONT_SIZE_RATE;

            HierarchyLevels = NoaDebuggerDefine.DEFAULT_HIERARCHY_LEVELS;

            CommandFormatLandscape = NoaDebuggerDefine.DEFAULT_COMMAND_FORMAT_LANDSCAPE;
            CommandFormatPortrait = NoaDebuggerDefine.DEFAULT_COMMAND_FORMAT_PORTRAIT;

            AutoSave = NoaDebuggerDefine.DEFAULT_AUTO_SAVE;
#if UNITY_EDITOR
            _SaveSettingsData();
#endif
            return this;
        }

        public static List<MenuInfo> GetDefaultMenuSettings()
        {
            var menuList = new List<MenuInfo>() { };

            List<IMenuInfo> infos = NoaDebuggerSettings.GetIMenuInfoList();

            foreach (IMenuInfo info in infos)
            {
                if (info.GetType().Name == nameof(ToolDetailPresenter.ToolDetailMenuInfo))
                {
                    continue;
                }

                var menuInfo = new MenuInfo()
                {
                    Name = info.MenuName,
                    Enabled = true,
                    SortNo = info.SortNo
                };
                menuList.Add(menuInfo);
            }

            return menuList;
        }

        static List<IMenuInfo> GetIMenuInfoList()
        {
            List<IMenuInfo> infos = new List<IMenuInfo>();

            if (Application.isPlaying)
            {
                List<INoaDebuggerTool> tools = NoaDebugger.AllPresenters().ToList();
                infos.AddRange(tools.Select(t => t.MenuInfo()));
            }
            else
            {
                infos = AssemblyUtils.CreateInterfaceInstances<IMenuInfo>().ToList();
                infos.RemoveAll(m => m.GetType().Name == nameof(NoaCustomMenuBase.CustomMenuInfo));
            }
            infos.Sort((a, b) => a.SortNo - b.SortNo);

            return infos;
        }

#if UNITY_EDITOR
        public NoaDebuggerSettings Update()
        {
            var jsonDictionary = _LoadSettingsData();
            if (jsonDictionary == null)
            {
                jsonDictionary = _SaveSettingsData();
            }


            var defaultInfos = new List<IMenuInfo>();
            var updateInfos = new List<MenuInfo>();

            defaultInfos = NoaDebuggerSettings.GetIMenuInfoList();

            foreach (MenuInfo menu in _menuList)
            {
                var updateInfo = updateInfos.FirstOrDefault(updateInfo => updateInfo.Name.Equals(menu.Name));
                var defaultInfo = defaultInfos.FirstOrDefault(defaultInfo => defaultInfo.MenuName.Equals(menu.Name));
                if (updateInfo != null || defaultInfo == null)
                {
                    continue;
                }

                var menuInfo = new MenuInfo
                {
                    Name = menu.Name,
                    Enabled = menu.Enabled,
                };
                updateInfos.Add(menuInfo);
            }

            foreach (IMenuInfo defaultInfo in defaultInfos)
            {
                if (defaultInfo.GetType().Name == nameof(ToolDetailPresenter.ToolDetailMenuInfo))
                {
                    continue;
                }

                var customMenuInfo = _menuList.FirstOrDefault(customMenuInfo => customMenuInfo.Name.Equals(defaultInfo.MenuName));

                if (customMenuInfo != null)
                {
                    continue;
                }

                var menuInfo = new MenuInfo
                {
                    Name = defaultInfo.MenuName,
                    Enabled = true,
                };

                updateInfos.Insert(defaultInfo.SortNo, menuInfo);
            }

            var sortNo = 0;
            foreach (MenuInfo updateInfo in updateInfos)
            {
                updateInfo.SortNo = sortNo;
                sortNo++;
            }

            MenuList = updateInfos;

            _SaveSettingsData();

            return this;
        }

        public void UpdateCustomMenu()
        {
            foreach(CustomMenuInfo customMenuInfo in CustomMenuList)
            {
                customMenuInfo.RefreshScriptName();
            }
        }

        Dictionary<string, object> _SaveSettingsData()
        {
            var fields = typeof(NoaDebuggerSettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var dictionary = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                dictionary.Add(field.Name, field.FieldType.Name);
            }
            var jsonDictionary = new JsonDictionary<string, object>( dictionary );
            var json = JsonUtility.ToJson( jsonDictionary, true );
            EditorPrefs.SetString(NoaDebuggerDefine.EditorPrefsKeyPackageSettingsData, json);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return jsonDictionary.Dictionary;
        }

        Dictionary<string, object> _LoadSettingsData()
        {
            var json = EditorPrefs.GetString(NoaDebuggerDefine.EditorPrefsKeyPackageSettingsData);
            var jsonDictionary = JsonUtility.FromJson<JsonDictionary<string, object>>(json);
            return jsonDictionary?.Dictionary;
        }
#endif 
#endif 
    }

    [Serializable]
    sealed class MenuInfo
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public bool Enabled;
        [SerializeField]
        public int SortNo;
    }

    [Serializable]
    sealed class CustomMenuInfo
    {
        [SerializeField]
        public int _sortNo;

        public String _scriptName;

#if UNITY_EDITOR
        [SerializeField]
        public MonoScript _script;

        public bool IsInvalidScript()
        {
            if (_script.GetClass().BaseType != typeof(NoaCustomMenuBase))
            {
                return true;
            }

            return false;
        }

        public void RefreshScriptName()
        {
            if (_script == null)
            {
                return;
            }

            if (IsInvalidScript())
            {
                return;
            }

            _scriptName =_script.GetClass().AssemblyQualifiedName;
        }

        public string GetViewName()
        {
            return _script == null ? "" : _script.GetClass().Name;
        }
#endif
    }

    [Serializable]
    sealed class JsonDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [Serializable]
        struct KeyValuePair
        {
            [SerializeField][UsedImplicitly]
            TKey key;
            [SerializeField][UsedImplicitly]
            TValue value;

            public TKey   Key   => key;
            public TValue Value => value;

            public KeyValuePair( TKey key, TValue value )
            {
                this.key   = key;
                this.value = value;
            }
        }

        [SerializeField][UsedImplicitly]
        KeyValuePair[] dictionary = default;

        Dictionary<TKey, TValue> _dictionary;

        public Dictionary<TKey, TValue> Dictionary => _dictionary;

        public JsonDictionary(Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            dictionary = _dictionary.Select( x => new KeyValuePair( x.Key, x.Value ) ).ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _dictionary = dictionary.ToDictionary( x => x.Key, x => x.Value );
            dictionary   = null;
        }
    }
}
