using System;
using UnityEngine;
using TMPro;

namespace NoaDebugger
{
    sealed class UnityInformationView : ViewBase<UnityInformationViewLinker>
    {
        [SerializeField, Header("Unity Info")]
        TextMeshProUGUI _version;
        [SerializeField]
        GameObject _debugCheck;
        [SerializeField]
        GameObject _debugCross;
        [SerializeField]
        GameObject _il2CppCheck;
        [SerializeField]
        GameObject _il2CppCross;
        [SerializeField]
        TextMeshProUGUI _vSyncCount;
        [SerializeField]
        TextMeshProUGUI _targetFrameRate;

        [SerializeField, Header("Runtime")]
        TextMeshProUGUI _playTime;
        [SerializeField]
        TextMeshProUGUI _levelPlayTime;
        [SerializeField]
        TextMeshProUGUI _currentLevel;
        [SerializeField]
        TextMeshProUGUI _qualityLevel;

        [SerializeField, Header("Features")]
        TextMeshProUGUI _location;
        [SerializeField]
        TextMeshProUGUI _accelerometer;
        [SerializeField]
        TextMeshProUGUI _gyroscope;
        [SerializeField]
        TextMeshProUGUI _vibration;

        [SerializeField, Header("Graphics")]
        TextMeshProUGUI _maxTexSize;
        [SerializeField]
        TextMeshProUGUI _npotSupport;
        [SerializeField]
        TextMeshProUGUI _computeShaders;
        [SerializeField]
        TextMeshProUGUI _shadows;
        [SerializeField]
        TextMeshProUGUI _sparseTextures;

        protected override void _OnShow(UnityInformationViewLinker linker)
        {
            if (linker._unityInfo != null)
            {
                _OnShowUnityInfo(linker._unityInfo);
            }
            if (linker._runtimeInfo != null)
            {
                _OnShowRuntime(linker._runtimeInfo);
            }
            if (linker._featuresInfo != null)
            {
                _OnShowFeatures(linker._featuresInfo);
            }
            if (linker._graphicsInfo != null)
            {
                _OnShowGraphics(linker._graphicsInfo);
            }
            gameObject.SetActive(true);
        }

        protected override void _OnHide()
        {
            gameObject.SetActive(false);
        }

        void _OnShowUnityInfo(UnityInformationViewLinker.UnityInfo info)
        {
            _version.text = _ConvertLabel(info._version);
            _debugCheck.SetActive(info._debug);
            _debugCross.SetActive(!info._debug);
            _il2CppCheck.SetActive(info._il2CPP);
            _il2CppCross.SetActive(!info._il2CPP);
            _vSyncCount.text = _ConvertLabel($"{info._vSyncCount}");
            _targetFrameRate.text = _ConvertLabel($"{info._targetFrameRate}");
        }

        void _OnShowRuntime(UnityInformationViewLinker.Runtime info)
        {
            var spanPlayTime = TimeSpan.FromSeconds(info._playTime);
            _playTime.text =
                $"{(int) spanPlayTime.TotalHours}:{spanPlayTime.Minutes:D2}:{spanPlayTime.Seconds:D2}.{spanPlayTime.Milliseconds:D3}";
            var spanLevelPlayTime = TimeSpan.FromSeconds(info._levelPlayTime);
            _levelPlayTime.text = $"{(int)spanLevelPlayTime.TotalSeconds}.{spanLevelPlayTime.Milliseconds:D3}";
            _currentLevel.text =
                _ConvertLabel($"{info._currentLevelSceneName} (Index: {info._currentLevelBuildIndex})");
            _qualityLevel.text = _ConvertLabel($"{QualitySettings.names[info._qualityLevel]} ({info._qualityLevel})");
        }

        void _OnShowFeatures(UnityInformationViewLinker.Features info)
        {
            _location.text = _ConvertFlag(info._location);
            _location.color =
                info._location ? NoaDebuggerDefine.TextColors.Success : NoaDebuggerDefine.TextColors.Default;
            _accelerometer.text = _ConvertFlag(info._accelerometer);
            _accelerometer.color = info._accelerometer
                ? NoaDebuggerDefine.TextColors.Success
                : NoaDebuggerDefine.TextColors.Default;
            _gyroscope.text = _ConvertFlag(info._gyroscope);
            _gyroscope.color = info._gyroscope
                ? NoaDebuggerDefine.TextColors.Success
                : NoaDebuggerDefine.TextColors.Default;
            _vibration.text = _ConvertFlag(info._vibration);
            _vibration.color = info._vibration
                ? NoaDebuggerDefine.TextColors.Success
                : NoaDebuggerDefine.TextColors.Default;
        }

        void _OnShowGraphics(UnityInformationViewLinker.Graphics info)
        {
            _maxTexSize.text = _ConvertLabel(info._maxTexSize.ToString());
            _npotSupport.text = _ConvertLabel(info._npotSupport.ToString());
            _computeShaders.text = _ConvertFlag(info._computeShaders);
            _computeShaders.color = info._computeShaders
                ? NoaDebuggerDefine.TextColors.Success
                : NoaDebuggerDefine.TextColors.Default;
            _shadows.text = _ConvertFlag(info._shadows);
            _shadows.color =
                info._shadows ? NoaDebuggerDefine.TextColors.Success : NoaDebuggerDefine.TextColors.Default;
            _sparseTextures.text = _ConvertFlag(info._sparseTextures);
            _sparseTextures.color = info._sparseTextures
                ? NoaDebuggerDefine.TextColors.Success
                : NoaDebuggerDefine.TextColors.Default;
        }

        string _ConvertLabel(string label)
        {
            if (!_IsAcquireLabel(label))
            {
                return $"{NoaDebuggerDefine.MISSING_VALUE}";
            }
            return $"{label}";
        }

        string _ConvertFlag(bool flag)
        {
            if (flag)
            {
                return $"{NoaDebuggerDefine.SupportedValue}";
            }
            return $"{NoaDebuggerDefine.MISSING_VALUE}";
        }

        bool _IsAcquireLabel(string label)
        {
            if (string.IsNullOrEmpty(label))
            {
                return false;
            }
            if (label == SystemInfo.unsupportedIdentifier)
            {
                return false;
            }
            return true;
        }
    }

    sealed class UnityInformationViewLinker : ViewLinkerBase
    {
        public sealed class UnityInfo
        {
            public string _version;

            public bool _debug;

            public bool _il2CPP;

            public int _vSyncCount;

            public int _targetFrameRate;
        }

        public sealed class Runtime
        {
            public float _playTime;

            public float _levelPlayTime;

            public string _currentLevelSceneName;
            public int _currentLevelBuildIndex;

            public int _qualityLevel;
        }

        public sealed class Features
        {
            public bool _location;

            public bool _accelerometer;

            public bool _gyroscope;

            public bool _vibration;
        }

        public sealed class Graphics
        {
            public int _maxTexSize;

            public NPOTSupport _npotSupport;

            public bool _computeShaders;

            public bool _shadows;

            public bool _sparseTextures;
        }

        public UnityInfo _unityInfo;

        public Runtime _runtimeInfo;

        public Features _featuresInfo;

        public Graphics _graphicsInfo;
    }
}
