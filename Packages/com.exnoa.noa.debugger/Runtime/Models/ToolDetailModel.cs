using System;
using UnityEngine;

namespace NoaDebugger
{
    sealed class ToolDetailModel : ModelBase
    {
        public const string OPERATIONAL_VALUE = "operational";

        public enum OSType
        {
            Unknown,
            Editor,
            Ios,
            Android,
            Standalone,
            Webgl
        }

        public ToolDetailInformation ToolDetailInformation { private set; get; }

        NoaDebuggerInfo _noaDebuggerInfo;

        public ToolDetailModel()
        {
            _LoadAsset();
            string osVersion = OsType == OSType.Webgl ? SystemInfo.operatingSystem : OsVersion;
            SetOperatingEnv(osVersion, OsType, SystemInfo.deviceModel);
        }


        void _LoadAsset()
        {
            _noaDebuggerInfo = NoaDebuggerInfoManager.GetNoaDebuggerInfo();
            ToolDetailInformation = new ToolDetailInformation()
            {
                _copyright = _noaDebuggerInfo.NoaDebuggerCopyright,
            };
        }

        public void SetOperatingEnv (string osVersion, OSType osType, string device = "")
        {
            ToolDetailInformation._operatingEnv = _CheckSupported(osVersion, osType, device) ? ToolDetailModel.OPERATIONAL_VALUE : _SupportedVersionText(osType);
        }

        string OsVersion
        {
            get
            {
#if UNITY_EDITOR
                return SemanticVersion.ExtractSemanticVersionString(Application.unityVersion);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_WEBGL
                return SemanticVersion.ExtractSemanticVersionString(SystemInfo.operatingSystem);
#else
                return "";
#endif
            }
        }

        OSType OsType
        {
            get
            {
#if UNITY_EDITOR
                return OSType.Editor;
#elif UNITY_IOS
                return OSType.Ios;
#elif UNITY_ANDROID
                return OSType.Android;
#elif UNITY_STANDALONE_WIN
                return OSType.Standalone;
#elif UNITY_WEBGL
                return OSType.Webgl;
#else
                return OSType.Unknown;
#endif
            }
        }

        bool _CheckSupported(string osVersion, OSType osType, string device)
        {
            switch (osType)
            {
                case OSType.Editor:
                    return _noaDebuggerInfo.IsSupportedUnityVersion(osVersion);
                case OSType.Android:
                    return _noaDebuggerInfo.IsSupportedAndroidVersion(osVersion);
                case OSType.Ios:
                    return _noaDebuggerInfo.IsSupportedIOSVersion(osVersion);
                case OSType.Standalone:
                    return _noaDebuggerInfo.IsSupportedWindowsVersion(osVersion);
                case OSType.Webgl:
                    return _noaDebuggerInfo.IsSupportedBrowser(device, osVersion);
                default:
                    return false;
            }
        }

        string _SupportedVersionText(OSType osType)
        {
            switch (osType)
            {
                case OSType.Editor:
                    return $"Unity {_noaDebuggerInfo.MinimumUnityVersion}+";
                case OSType.Android:
                    return $"Android {_noaDebuggerInfo.MinimumAndroidVersion}+";
                case OSType.Ios:
                    return $"iOS {_noaDebuggerInfo.MinimumIOSVersion}+";
                case OSType.Standalone:
                    return $"Windows {_noaDebuggerInfo.MinimumWindowsVersion}+";
                case OSType.Webgl:
                    return $"{string.Join(',', _noaDebuggerInfo.SupportedBrowsers)} ({string.Join(',', _noaDebuggerInfo.SupportedOperatingSystemOnWebGL)})";
                default:
                    return String.Empty;
            }
        }
    }


    sealed class ToolDetailInformation
    {
        public string _operatingEnv;

        public string _copyright;
    }
}
