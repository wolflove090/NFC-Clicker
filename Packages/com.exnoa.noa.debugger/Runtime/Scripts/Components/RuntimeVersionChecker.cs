using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class RuntimeVersionChecker : MonoBehaviour
    {
        [SerializeField]
        Image[] _unsupportedColorChangeTarget;

        NoaDebuggerInfo _noaDebuggerInfo;

        public void DoCheck(NoaDebuggerInfo noaDebuggerInfo)
        {
            _noaDebuggerInfo = noaDebuggerInfo;

            if (!_CheckSupported())
            {
                _IndicateUnsupported(true);
            }
        }

        void _IndicateUnsupported(bool isUnsupported)
        {
            foreach (Image target in _unsupportedColorChangeTarget)
            {
                target.color = isUnsupported
                    ? NoaDebuggerDefine.ImageColors.Warning
                    : NoaDebuggerDefine.ImageColors.Default;
            }
        }

        bool _CheckSupported()
        {
#if UNITY_EDITOR
            string version = SemanticVersion.ExtractSemanticVersionString(Application.unityVersion);
            return _noaDebuggerInfo.IsSupportedUnityVersion(version);
#elif UNITY_IOS
            string version = SemanticVersion.ExtractSemanticVersionString(SystemInfo.operatingSystem);
            return _noaDebuggerInfo.IsSupportedIOSVersion(version);
#elif UNITY_ANDROID
            string version = SemanticVersion.ExtractSemanticVersionString(SystemInfo.operatingSystem);
            return _noaDebuggerInfo.IsSupportedAndroidVersion(version);
#elif UNITY_STANDALONE_WIN
            string version = SemanticVersion.ExtractSemanticVersionString(SystemInfo.operatingSystem);
            return _noaDebuggerInfo.IsSupportedWindowsVersion(version);
#elif UNITY_WEBGL
            return _noaDebuggerInfo.IsSupportedBrowser(SystemInfo.deviceModel, SystemInfo.operatingSystem);
#else
            return false;
#endif
        }
    }
}
