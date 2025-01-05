using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;

#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditorInternal;
#endif

namespace NoaDebugger
{
    sealed class PackageEditorWindow : EditorWindow
    {
        public enum TabMenu
        {
            General,
            Settings,
            Tools
        }

        static bool _isWindowOpen;

        Vector2 _scrollPos;

        List<string> _tabMenu;

        TabMenu _selectTab;

        int _selectedTabIndex;

        public static bool IsWait;

        bool _isPlayingLayoutMode;

#pragma warning disable 0414
        bool _isInitialized;
#pragma warning restore 0414

#if NOA_DEBUGGER
        NoaDebuggerSettings _noaDebuggerSettings;

        ButtonPosition _startButtonPosition;

        float _startButtonScale;

        ButtonMovementType _startButtonMovementType;

        bool _saveStartButtonPosition;

        float _toolStartButtonAlpha;

        float _backgroundAlpha;

        float _floatingWindowAlpha;

        bool _isUIReversePortrait;

        List<MenuInfo> _menuList;

        ReorderableList _reorderableMenuList;

        List<CustomMenuInfo> _customMenuList;

        ReorderableList _reorderableCustomMenuList;

        bool _isAutoInitialize;

        bool _isAutoCreateEventSystem;

        ErrorNotificationType _errorNotificationType;

        int _noaDebuggerCanvasSortOrder;

        int _consoleLogCount;

        int _apiLogCount;

        bool _isCustomFontSettingsEnabled;

        FontSettings _fontSetting;

        int _hierarchyLevels;

        CommandDisplayFormat _commandFormatLandscape;

        CommandDisplayFormat _commandFormatPortrait;

        bool _isAutoSave;

        bool _isSettingsChanged;

#endif
        [MenuItem("Window/NOA Debugger", priority = 100)]
        static void _EditorWindow()
        {
            ShowWindow();
        }

        public static void ShowWindow(TabMenu selectedTab = TabMenu.General)
        {
            if (PackageEditorWindow._isWindowOpen)
            {
                return;
            }

            PackageEditorWindow._isWindowOpen = true;
            var window = CreateInstance<PackageEditorWindow>();
            window.ShowUtility();

            int x = (Screen.currentResolution.width - EditorDefine.NOA_DEBUGGER_EDITOR_WINDOW_WIDTH) / 2;
            int y = (Screen.currentResolution.height - EditorDefine.NOA_DEBUGGER_EDITOR_WINDOW_HEIGHT) / 2;
            window.position = new Rect(x, y, EditorDefine.NOA_DEBUGGER_EDITOR_WINDOW_WIDTH, EditorDefine.NOA_DEBUGGER_EDITOR_WINDOW_HEIGHT);
            var fixedSize = new Vector2() { x = EditorDefine.NOA_DEBUGGER_EDITOR_WINDOW_WIDTH, y = EditorDefine.NOA_DEBUGGER_EDITOR_WINDOW_HEIGHT };
            window.minSize = fixedSize;
            window.maxSize = fixedSize;
            if (!Enum.IsDefined(typeof(TabMenu), selectedTab))
            {
                selectedTab = TabMenu.General;
            }
            window._selectTab = selectedTab;
            window._selectedTabIndex = window._GetTabIndex(window._selectTab);
        }

        void OnEnable()
        {
            PackageEditorWindow._isWindowOpen = true;

            _SwitchEditorLayout();
            _selectedTabIndex = _GetTabIndex(_selectTab);

#if NOA_DEBUGGER

            if (_isInitialized)
            {
                _isInitialized = false;
                NoaPackageManager.InitializeOnPackageUpdate();
            }

            _SettingsAssetLoad();

            if (_noaDebuggerSettings == null)
            {
                return;
            }

            _startButtonPosition = _noaDebuggerSettings.StartButtonPosition;
            _startButtonScale = _noaDebuggerSettings.StartButtonScale;
            _startButtonMovementType = _noaDebuggerSettings.StartButtonMovementType;
            _saveStartButtonPosition = _noaDebuggerSettings.SaveStartButtonPosition;

            _toolStartButtonAlpha = _noaDebuggerSettings.ToolStartButtonAlpha;
            _backgroundAlpha = _noaDebuggerSettings.BackgroundAlpha;
            _floatingWindowAlpha = _noaDebuggerSettings.FloatingWindowAlpha;

            _menuList = new List<MenuInfo>();
            foreach (var menuInfo in _noaDebuggerSettings.MenuList)
            {
                _menuList.Add(new MenuInfo()
                {
                    Name = menuInfo.Name,
                    Enabled = menuInfo.Enabled,
                    SortNo = menuInfo.SortNo
                });
            }

            _customMenuList = new List<CustomMenuInfo>();
            foreach (var menuInfo in _noaDebuggerSettings.CustomMenuList)
            {
                _customMenuList.Add(new CustomMenuInfo()
                {
                    _script = menuInfo._script,
                    _sortNo = menuInfo._sortNo,
                    _scriptName = menuInfo._scriptName
                });
            }

            void onDrawMenuElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                MenuInfo data = _menuList[index];

                data.Enabled = EditorGUI.Toggle(rect, data.Name, data.Enabled);
            }

            void onDrawCustomMenuElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                CustomMenuInfo data = _customMenuList[index];
                Rect scriptRect = rect;
                scriptRect.width *= 0.5f;
                scriptRect.x += 200;
                data._script = EditorGUI.ObjectField(scriptRect, data._script, typeof(MonoScript), false) as MonoScript;

                string viewName = "";
                if (data._script != null)
                {
                    if (data.IsInvalidScript())
                    {
                        data._script = null;
                        EditorUtility.DisplayDialog("Error", "Can't Set No Inheritance Script", "OK");
                    }

                    viewName = data.GetViewName();
                    data.RefreshScriptName();
                }

                EditorGUI.LabelField(rect, viewName);
            }

            _reorderableMenuList = new ReorderableList(_menuList, typeof(MenuInfo))
            {
                headerHeight = 0,
                footerHeight = 0,
                displayAdd = false,
                displayRemove = false,
                drawElementCallback = onDrawMenuElement,
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "");
                },
            };

            _reorderableCustomMenuList = new ReorderableList(_customMenuList, typeof(CustomMenuInfo))
            {
                headerHeight = 0,
                footerHeight = 20,
                displayAdd = true,
                displayRemove = true,
                drawElementCallback = onDrawCustomMenuElement,
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "");
                },
            };

            _isAutoInitialize = _noaDebuggerSettings.AutoInitialize;

            _noaDebuggerCanvasSortOrder = _noaDebuggerSettings.NoaDebuggerCanvasSortOrder;

            _isUIReversePortrait = _noaDebuggerSettings.IsUIReversePortrait;

            _consoleLogCount = _noaDebuggerSettings.ConsoleLogCount;
            _apiLogCount = _noaDebuggerSettings.ApiLogCount;

            _isAutoCreateEventSystem = _noaDebuggerSettings.AutoCreateEventSystem;
            _errorNotificationType = _noaDebuggerSettings.ErrorNotificationType;

            _isCustomFontSettingsEnabled = _noaDebuggerSettings.IsCustomFontSettingsEnabled;
            _fontSetting = new FontSettings(
                _noaDebuggerSettings.FontAsset, _noaDebuggerSettings.FontMaterial, _noaDebuggerSettings.FontSizeRate);

            _hierarchyLevels = _noaDebuggerSettings.HierarchyLevels;

            _commandFormatLandscape = _noaDebuggerSettings.CommandFormatLandscape;
            _commandFormatPortrait = _noaDebuggerSettings.CommandFormatPortrait;

            _isAutoSave = _noaDebuggerSettings.AutoSave;
#endif
        }
#if NOA_DEBUGGER
        void _SettingsAssetLoad()
        {
            _noaDebuggerSettings = Resources.Load<NoaDebuggerSettings>(nameof(NoaDebuggerSettings));
        }

        void _SettingsAssetSave()
        {
            if (_noaDebuggerSettings == null)
            {
                _SettingsAssetLoad();
            }

            _noaDebuggerSettings.StartButtonPosition = _startButtonPosition;
            _noaDebuggerSettings.StartButtonScale = _startButtonScale;
            _noaDebuggerSettings.StartButtonMovementType = _startButtonMovementType;
            _noaDebuggerSettings.SaveStartButtonPosition = _saveStartButtonPosition;
            _noaDebuggerSettings.ToolStartButtonAlpha = _toolStartButtonAlpha;
            _noaDebuggerSettings.BackgroundAlpha = _backgroundAlpha;
            _noaDebuggerSettings.FloatingWindowAlpha = _floatingWindowAlpha;
            _noaDebuggerSettings.NoaDebuggerCanvasSortOrder = _noaDebuggerCanvasSortOrder;
            _noaDebuggerSettings.IsUIReversePortrait = _isUIReversePortrait;

            List<MenuInfo> infos = new List<MenuInfo>();

            for (int i = 0; i < _menuList.Count; i++)
            {
                var info = _menuList[i];

                infos.Add(
                    new MenuInfo()
                    {
                        Name = info.Name,
                        Enabled = info.Enabled,
                        SortNo = i
                    });
            }

            _noaDebuggerSettings.MenuList = infos;

            List<CustomMenuInfo> monoInfos = new List<CustomMenuInfo>();

            for (int i = 0; i < _customMenuList.Count; i++)
            {
                var monoInfo = _customMenuList[i];

                monoInfos.Add(
                    new CustomMenuInfo()
                    {
                        _script = monoInfo._script,
                        _sortNo = i,
                        _scriptName = monoInfo._scriptName
                    });
            }

            _noaDebuggerSettings.CustomMenuList = monoInfos;

            _noaDebuggerSettings.AutoInitialize = _isAutoInitialize;

            _noaDebuggerSettings.ConsoleLogCount = _consoleLogCount;
            _noaDebuggerSettings.ApiLogCount = _apiLogCount;

            _noaDebuggerSettings.IsCustomFontSettingsEnabled = _isCustomFontSettingsEnabled;
            _noaDebuggerSettings.FontAsset = _fontSetting._fontAsset;
            _noaDebuggerSettings.FontMaterial = _fontSetting.FontMaterialPreset;
            _noaDebuggerSettings.FontSizeRate = _fontSetting._fontSizeRate;

            _noaDebuggerSettings.HierarchyLevels = _hierarchyLevels;

            _noaDebuggerSettings.CommandFormatLandscape = _commandFormatLandscape;
            _noaDebuggerSettings.CommandFormatPortrait = _commandFormatPortrait;

            _noaDebuggerSettings.AutoCreateEventSystem = _isAutoCreateEventSystem;
            _noaDebuggerSettings.ErrorNotificationType = _errorNotificationType;

            _noaDebuggerSettings.AutoSave = _isAutoSave;

            EditorUtility.SetDirty(_noaDebuggerSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _isSettingsChanged = false;
        }
#endif
        void OnGUI()
        {
#if NOA_DEBUGGER

            if (_noaDebuggerSettings == null)
            {
                OnEnable();
            }
#endif

            using (new EditorGUILayout.VerticalScope(EditorBaseStyle.Box))
            {
                EditorUtil.DrawTitle("NOA Debugger Editor");
            }

            using (new EditorGUILayout.VerticalScope(EditorBaseStyle.Box))
            {
                EditorGUILayout.LabelField($"<b>Version : {NoaPackageManager.NoaDebuggerPackageInfo.version}</b> | <i>{NoaPackageManager.NoaDebuggerPackageInfo.name}</i>", EditorBaseStyle.RichText());
            }

            EditorGUI.BeginDisabledGroup(IsWait);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                _selectedTabIndex = GUILayout.Toolbar(_selectedTabIndex, _tabMenu.ToArray(), EditorStyles.toolbarButton);

                if(check.changed)
                {
                    _selectTab = (TabMenu)Enum.Parse(typeof(TabMenu), _tabMenu[_selectedTabIndex]);
                }
            }

            using (new EditorGUILayout.VerticalScope(EditorBaseStyle.Box))
            {
                if (_selectTab == TabMenu.General)
                {
                    EditorGUILayout.LabelField("<b>Package</b>", EditorBaseStyle.Headline());

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Initialize"))
                    {
                        if (NoaPackageManager.HasRuntimePackage())
                        {
                            IsWait = true;
                            NoaPackageManager.ClearEditorPrefs();
                            NoaPackageManager.InitializeOnPackageUpdate();
                            _isInitialized = true;
                        }
                    }

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Exclude from compile"))
                    {
                        if (EditorUtility.DisplayDialog("Attention", "Are you sure you want to exclude \"NOA Debugger\" from compilation?", "OK", "Cancel"))
                        {
                            IsWait = true;
                            NoaPackageManager.ExcludeFromCompile();
                        }
                    }

                    if (GUILayout.Button("Include in compile"))
                    {
                        if (EditorUtility.DisplayDialog("Attention", "Are you sure you want to include \"NOA Debugger\" in the compilation?", "OK", "Cancel"))
                        {
                            IsWait = true;
                            NoaPackageManager.IncludeInCompile();
                        }
                    }

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog(
                                "Attention", "Are you sure you want to delete the \"NOA Debugger\" package?", "OK", "Cancel"))
                        {
                            IsWait = true;
                            NoaPackageManager.DeletePackage();
                            AssetDatabase.Refresh();
                            Close();
                        }
                    }

                    EditorBaseStyle.DrawUILine();

                    EditorGUILayout.LabelField("<b>Document</b>", EditorBaseStyle.Headline());

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Open README.md"))
                    {
                        var path = $"{NoaPackageManager.NoaDebuggerPackagePath}/README.md";
                        EditorUtil.OpenFile(path);
                    }

                    EditorBaseStyle.DrawUILine();

                    EditorGUILayout.LabelField("<b>Support</b>", EditorBaseStyle.Headline());

                    EditorGUILayout.Separator();

                    GUILayout.Label("We are committed to supporting a diverse community, so feel free to post in English or Japanese. Our team will respond to your inquiries in the language you used. Please note that our support hours are from 10:00 to 18:00 JST, Monday through Friday.", EditorStyles.wordWrappedLabel);

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Open Unity Discussions"))
                    {
                        Application.OpenURL(EditorDefine.UnityDiscussionsUrl);
                    }

                    EditorBaseStyle.DrawUILine();

                    EditorGUILayout.LabelField("<b>Review</b>", EditorBaseStyle.Headline());

                    EditorGUILayout.Separator();

                    GUILayout.Label("If you find NOA Debugger helpful, we would greatly appreciate it if you could leave a rating and review on the Asset Store. Your feedback is invaluable and helps us improve. Thank you for your support!", EditorStyles.wordWrappedLabel);

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Rate and review on Asset Store"))
                    {
                        Application.OpenURL(EditorDefine.AssetStoreUrl + "#reviews");
                    }
                }
#if NOA_DEBUGGER
                EditorGUI.BeginChangeCheck();

                void displaySettingsCategoryHeader(string categoryName, Action reset = null)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField($"<b>{categoryName}</b>", EditorBaseStyle.Headline());

                        if (reset != null &&
                            GUILayout.Button("Reset", GUILayout.Width(110)))
                        {
                            if (EditorUtility.DisplayDialog("Attention", $"Are you sure you want to reset {categoryName} section?", "OK", "Cancel"))
                            {
                                reset.Invoke();

                                GUI.FocusControl("");
                            }
                        }
                    }

                    EditorGUILayout.Separator();
                }

                if (_selectTab == TabMenu.Settings)
                {
                    using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
                    {
                        _scrollPos = scrollView.scrollPosition;

                        float originalValue = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = EditorDefine.NOA_DEBUGGER_EDITOR_LABEL_WIDTH;

                        displaySettingsCategoryHeader("Loading", reset: () =>
                        {
                            _isAutoInitialize = NoaDebuggerDefine.DEFAULT_AUTO_INITIALIZE;
                        });

                        _isAutoInitialize = EditorGUILayout.Toggle("Automatically initialize", _isAutoInitialize);

                        EditorBaseStyle.DrawUILine();


                        displaySettingsCategoryHeader("Display", reset: () =>
                        {
                            _startButtonPosition = NoaDebuggerDefine.DEFAULT_START_BUTTON_POSITION;
                            _startButtonScale = NoaDebuggerDefine.DEFAULT_START_BUTTON_SCALE;
                            _startButtonMovementType = NoaDebuggerDefine.DEFAULT_START_BUTTON_MOVEMENT_TYPE;
                            _saveStartButtonPosition = NoaDebuggerDefine.DEFAULT_SAVE_START_BUTTON_POSITION;
                            _toolStartButtonAlpha = NoaDebuggerDefine.TOOL_START_BUTTON_ALPHA_DEFAULT;
                            _backgroundAlpha = NoaDebuggerDefine.CANVAS_ALPHA_DEFAULT;
                            _floatingWindowAlpha = NoaDebuggerDefine.CANVAS_ALPHA_DEFAULT;
                            _noaDebuggerCanvasSortOrder = NoaDebuggerDefine.NOA_DEBUGGER_CANVAS_SORT_ORDER_DEFAULT;
                            _isUIReversePortrait = NoaDebuggerDefine.IS_UI_REVERSE_PORTRAIT_DEFAULT;
                        });

                        _startButtonPosition = (ButtonPosition)EditorGUILayout.EnumPopup("Start button position", _startButtonPosition);
                        _startButtonScale = EditorGUILayout.Slider(
                            "Start button scale",
                            _startButtonScale,
                            NoaDebuggerDefine.StartButtonScaleMin,
                            NoaDebuggerDefine.StartButtonScaleMax);
                        _startButtonMovementType = (ButtonMovementType)EditorGUILayout.EnumPopup(new GUIContent("Start button movement type*", "Specifies whether the NOA Debugger start button is draggable or fixed. Options include: Draggable, Fixed."), _startButtonMovementType);
                        _saveStartButtonPosition = EditorGUILayout.Toggle("Save start button position", _saveStartButtonPosition);

                        _toolStartButtonAlpha = EditorGUILayout.Slider(
                            "Start button opacity",
                            _toolStartButtonAlpha,
                            NoaDebuggerDefine.ToolStartButtonAlphaMin,
                            NoaDebuggerDefine.ToolStartButtonAlphaMax);

                        _backgroundAlpha = EditorGUILayout.Slider(
                            "Background opacity",
                            _backgroundAlpha,
                            NoaDebuggerDefine.CanvasAlphaMin,
                            NoaDebuggerDefine.CanvasAlphaMax);

                        _floatingWindowAlpha = EditorGUILayout.Slider(
                            "Floating window opacity",
                            _floatingWindowAlpha,
                            NoaDebuggerDefine.CanvasAlphaMin,
                            NoaDebuggerDefine.CanvasAlphaMax);

                        _noaDebuggerCanvasSortOrder = EditorGUILayout.IntField("Canvas sort order", _noaDebuggerCanvasSortOrder);
                        _isUIReversePortrait = EditorGUILayout.Toggle(new GUIContent("Optimize UI for portrait*", "Optimizes the display order of the NOA Debugger's UI elements for mobile devices when displayed in portrait orientation. If this setting is enabled, some UI elements are arranged in reverse order from the bottom to the top of the screen, allowing for easier access from the bottom of the screen."), _isUIReversePortrait);

                        EditorBaseStyle.DrawUILine();

                        displaySettingsCategoryHeader("Font", reset: () =>
                        {
                            _isCustomFontSettingsEnabled = NoaDebuggerDefine.IS_CUSTOM_FONT_SETTINGS_ENABLED_DEFAULT;
                            _fontSetting = new FontSettings(null, null, NoaDebuggerDefine.DEFAULT_FONT_SIZE_RATE);
                        });

                        _isCustomFontSettingsEnabled = EditorGUILayout.Toggle("Custom font settings enabled", _isCustomFontSettingsEnabled);

                        if (_isCustomFontSettingsEnabled)
                        {
                            EditorGUI.BeginChangeCheck();
                            TMP_FontAsset beforeFontAsset = _fontSetting._fontAsset;

                            _fontSetting._fontAsset = EditorGUILayout.ObjectField(
                                "Font asset", _fontSetting._fontAsset, typeof(TMP_FontAsset), false) as TMP_FontAsset;

                            if (EditorGUI.EndChangeCheck())
                            {
                                _fontSetting.GetMaterialPresets();

                                if (beforeFontAsset == null)
                                {
                                    _fontSetting._fontSizeRate = NoaDebuggerDefine.DEFAULT_FONT_SIZE_RATE;
                                }
                            }

                            _fontSetting._materialIndex = EditorGUILayout.Popup(
                                "Material preset", _fontSetting._materialIndex, _fontSetting.MaterialPresetNames);

                            _fontSetting._fontSizeRate = EditorGUILayout.FloatField(
                                "Font size rate", _fontSetting._fontSizeRate);

                            EditorGUILayout.HelpBox("Specify the font included within the application for the Font asset.\nIf a Font asset is specified, the font asset included with NOA Debugger is excluded at build time.", MessageType.Info);
                        }

                        EditorBaseStyle.DrawUILine();

                        displaySettingsCategoryHeader("Menu", reset: () =>
                        {
                            _menuList = NoaDebuggerSettings.GetDefaultMenuSettings();
                        });

                        _reorderableMenuList.DoLayoutList();

                        EditorBaseStyle.DrawUILine();

                        displaySettingsCategoryHeader("Custom Menu");

                        _reorderableCustomMenuList.DoLayoutList();

                        EditorBaseStyle.DrawUILine();

                        displaySettingsCategoryHeader("Logs", reset: () =>
                        {
                            _consoleLogCount = NoaDebuggerDefine.CONSOLE_LOG_COUNT_DEFAULT;
                            _apiLogCount = NoaDebuggerDefine.API_LOG_COUNT_DEFAULT;
                        });

                        _consoleLogCount = EditorGUILayout.IntSlider(
                            "Console log count",
                            _consoleLogCount,
                            NoaDebuggerDefine.ConsoleLogCountMin,
                            NoaDebuggerDefine.ConsoleLogCountMax);

                        _apiLogCount = EditorGUILayout.IntSlider(
                            "API log count",
                            _apiLogCount,
                            NoaDebuggerDefine.ApiLogCountMin,
                            NoaDebuggerDefine.ApiLogCountMax);

                        EditorBaseStyle.DrawUILine();

                        displaySettingsCategoryHeader("Hierarchy", reset: () =>
                        {
                            _hierarchyLevels = NoaDebuggerDefine.DEFAULT_HIERARCHY_LEVELS;
                        });

                        _hierarchyLevels = EditorGUILayout.IntSlider(
                            "Hierarchy levels",
                            _hierarchyLevels,
                            NoaDebuggerDefine.HierarchyLevelsMin,
                            NoaDebuggerDefine.HierarchyLevelsMax);

                        EditorBaseStyle.DrawUILine();

                        displaySettingsCategoryHeader("Command", reset: () =>
                        {
                            _commandFormatLandscape = NoaDebuggerDefine.DEFAULT_COMMAND_FORMAT_LANDSCAPE;
                            _commandFormatPortrait = NoaDebuggerDefine.DEFAULT_COMMAND_FORMAT_PORTRAIT;
                        });

                        _commandFormatLandscape = (CommandDisplayFormat)EditorGUILayout.EnumPopup("Landscape format", _commandFormatLandscape);
                        _commandFormatPortrait = (CommandDisplayFormat)EditorGUILayout.EnumPopup("Portrait format", _commandFormatPortrait);

                        EditorBaseStyle.DrawUILine();

                        displaySettingsCategoryHeader("Others", reset: () =>
                        {
                            _isAutoCreateEventSystem = NoaDebuggerDefine.DEFAULT_AUTO_CREATE_EVENT_SYSTEM;
                            _errorNotificationType = NoaDebuggerDefine.DEFAULT_ERROR_NOTIFICATION_TYPE;
                        });

                        _isAutoCreateEventSystem = EditorGUILayout.Toggle("Auto create EventSystem", _isAutoCreateEventSystem );
                        _errorNotificationType = (ErrorNotificationType)EditorGUILayout.EnumPopup("Error notification", _errorNotificationType);

                        EditorGUIUtility.labelWidth = originalValue;
                    }


                    EditorBaseStyle.DrawUILine();

                    _isAutoSave = EditorGUILayout.ToggleLeft("Auto save", _isAutoSave);

                    if (EditorGUI.EndChangeCheck())
                    {
                        _isSettingsChanged = true;

                        if (_isAutoSave)
                        {
                            _SettingsAssetSave();
                        }
                    }

                    if (!_isAutoSave && _isSettingsChanged)
                    {
                        var color = GUI.backgroundColor;
                        GUI.backgroundColor = Color.cyan;
                        if (GUILayout.Button("Save", EditorBaseStyle.ButtonHighlighted))
                        {
                            _SettingsAssetSave();
                        }
                        GUI.backgroundColor = color;
                    }
                }


                if (_selectTab == TabMenu.Tools)
                {
                    EditorGUILayout.LabelField("<b>Views</b>", EditorBaseStyle.Headline());
                    EditorGUILayout.Separator();
                    GUILayout.Label("Opens the Debug Command window.", EditorStyles.wordWrappedLabel);
                    GUILayout.Label("Debug commands can be set and registered within an application that incorporates the NOA Debugger, allowing for the execution of methods, display of properties, etc. For more information, please refer to the instruction guide.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Open Debug Command View"))
                    {
                        CommandEditorWindow.ShowWindow(this);
                    }
                }
#endif
            }

            EditorGUI.EndDisabledGroup();


            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.VerticalScope(EditorBaseStyle.Box))
            {
                EditorGUILayout.LabelField("<i>Copyright (c) 2024 EXNOA LLC. All Rights Reserved.</i>", EditorBaseStyle.RichText());
            }
        }

        void Awake()
        {
            CompilationPipeline.compilationStarted  += PackageEditorWindow._OnCompilationStarted;
            CompilationPipeline.compilationFinished += PackageEditorWindow._OnCompilationFinished;
            IsWait = false;
        }

        void OnDestroy()
        {
            PackageEditorWindow._isWindowOpen = false;
#if NOA_DEBUGGER
            _noaDebuggerSettings = null;
#endif
            CompilationPipeline.compilationStarted  -= PackageEditorWindow._OnCompilationStarted;
            CompilationPipeline.compilationFinished -= PackageEditorWindow._OnCompilationFinished;
        }

        void Update()
        {
            if (Application.isPlaying && !_isPlayingLayoutMode)
            {
                _SwitchPlayingLayout();
                Repaint();
            }

            if(!Application.isPlaying && _isPlayingLayoutMode)
            {
                _SwitchEditorLayout();
                Repaint();
            }
        }

        void _SwitchEditorLayout()
        {
            _isPlayingLayoutMode = false;

            _tabMenu = new List<string>();
            _tabMenu.Add(TabMenu.General.ToString());
#if NOA_DEBUGGER
            _tabMenu.Add(TabMenu.Settings.ToString());
            _tabMenu.Add(TabMenu.Tools.ToString());
#endif
            _selectedTabIndex = _GetTabIndex(_selectTab);
        }

        void _SwitchPlayingLayout()
        {
            _isPlayingLayoutMode = true;
#if NOA_DEBUGGER
            _tabMenu = new List<string>();
            _tabMenu.Add(TabMenu.Tools.ToString());
            _selectTab = TabMenu.Tools;
            _selectedTabIndex = _GetTabIndex(_selectTab);
#endif
        }

        int _GetTabIndex(TabMenu selectTab)
        {
            if(_tabMenu == null || _tabMenu.Count == 0)
                return 0;

            int index = Array.IndexOf(_tabMenu.ToArray(), selectTab.ToString());

            if (index == -1)
                return 0;

            return index;
        }

        static void _OnCompilationStarted(object obj)
        {
            IsWait = true;
        }

        static void _OnCompilationFinished(object obj)
        {
            IsWait = false;
        }

        class FontSettings
        {
            Material[] _materialPresets;
            string[] _materialPresetNames;

            public TMP_FontAsset _fontAsset;

            public Material FontMaterialPreset
            {
                get
                {
                    if (_materialPresets.Length == 0 || _materialIndex < 0)
                    {
                        return null;
                    }

                    return _materialPresets[_materialIndex];
                }
            }

            public float _fontSizeRate;

            public string[] MaterialPresetNames => _materialPresetNames;

            public int _materialIndex;

            public FontSettings(TMP_FontAsset fontAsset, Material material, float fontSizeRate)
            {
                _fontAsset = fontAsset;
                _fontSizeRate = fontSizeRate;

                GetMaterialPresets();

                if (material != null)
                {
                    _materialIndex = Array.IndexOf(_materialPresetNames, material.name);
                    if (_materialIndex == -1)
                    {
                        _materialIndex = 0;
                    }
                }
            }

            public void GetMaterialPresets()
            {
                TMP_FontAsset fontAsset = _fontAsset;
                if (fontAsset == null)
                {
                    _materialPresets = Array.Empty<Material>();
                    _materialPresetNames = Array.Empty<string>();
                    _materialIndex = 0;
                    return;
                }

                _materialPresets = FindMaterialReferences(fontAsset);
                _materialPresetNames = new string[_materialPresets.Length];

                for(int i = 0; i < _materialPresets.Length; i++)
                {
                    Material material = _materialPresets[i];
                    _materialPresetNames[i] = material.name;
                }

                _materialIndex = 0;
            }
        }

        static Material[] FindMaterialReferences(TMP_FontAsset fontAsset)
        {
            List<Material> refs = new List<Material>();
            Material mat = fontAsset.material;
            refs.Add(mat);

            string searchPattern = "t:Material" + " " + fontAsset.name.Split(new char[] { ' ' })[0];
            string[] materialAssetGUIDs = AssetDatabase.FindAssets(searchPattern);

            for (int i = 0; i < materialAssetGUIDs.Length; i++)
            {
                string materialPath = AssetDatabase.GUIDToAssetPath(materialAssetGUIDs[i]);
                Material targetMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                if (targetMaterial.HasProperty(ShaderUtilities.ID_MainTex) && targetMaterial.GetTexture(ShaderUtilities.ID_MainTex) != null && mat.GetTexture(ShaderUtilities.ID_MainTex) != null && targetMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() == mat.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
                {
                    if (!refs.Contains(targetMaterial))
                        refs.Add(targetMaterial);
                }
                else
                {
                }
            }

            return refs.ToArray();
        }
    }
}
