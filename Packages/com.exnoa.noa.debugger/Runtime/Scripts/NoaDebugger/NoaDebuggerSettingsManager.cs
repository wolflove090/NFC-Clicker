using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoaDebugger
{
    sealed class NoaDebuggerSettingsManager
    {
        const string TOOL_SETTINGS_ASSET_NAME = "NoaDebuggerSettings";

        static NoaDebuggerSettingsManager _instance;
        static NoaDebuggerSettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NoaDebuggerSettingsManager();
                }

                return _instance;
            }
        }

        NoaDebuggerSettings _noaDebuggerSettings;

        public static void ValidateMenuSettings(List<INoaDebuggerTool> allPresenters)
        {
            Instance._ValidateMenuSettings(allPresenters);
        }

        void _ValidateMenuSettings(List<INoaDebuggerTool> allPresenters)
        {
            List<MenuInfo> menuInfos = _noaDebuggerSettings.MenuList.GroupBy(x => x.Name).Select(x => x.First()).ToList();

            for (int i = 0; i < allPresenters.Count; i++)
            {
                var presenter = allPresenters[i];

                if (!menuInfos.Exists(info => info.Name.Equals(presenter.MenuInfo().MenuName)))
                {
                    menuInfos.Add(new MenuInfo()
                    {
                        Name = presenter.MenuInfo().MenuName,
                        Enabled = true
                    });
                }
            }

            for (var i = 0; i < menuInfos.Count; i++)
            {
                var menuInfo = menuInfos[i];

                if (!allPresenters.Exists(tool => tool.MenuInfo().MenuName.Equals(menuInfo.Name)))
                {
                    menuInfos.RemoveAt(i);
                }
            }

            _noaDebuggerSettings.MenuList = menuInfos;
        }

        public static NoaDebuggerSettings GetNoaDebuggerSettings()
        {
            return Instance._GetNoaDebuggerSettings();
        }

        NoaDebuggerSettings _GetNoaDebuggerSettings()
        {
            if (_noaDebuggerSettings == null)
            {
                NoaDebuggerSettings settings = Resources.Load<NoaDebuggerSettings>(TOOL_SETTINGS_ASSET_NAME);
                if (settings == null)
                {
#if NOA_DEBUGGER
                    settings = ScriptableObject.CreateInstance<NoaDebuggerSettings>().Init();
#endif
                }

                _noaDebuggerSettings = settings;
            }

            return _noaDebuggerSettings;
        }

        public static void Dispose()
        {
            NoaDebuggerSettingsManager._instance = null;
        }
    }
}
