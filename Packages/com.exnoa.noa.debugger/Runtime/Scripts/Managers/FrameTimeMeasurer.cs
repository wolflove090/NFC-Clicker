using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NoaDebugger
{
    [RequireComponent(typeof(EndOfUpdateListener))]
    sealed class FrameTimeMeasurer : MonoBehaviour
    {
        public event Action<double, double, double> OnUpdateFrameTime;

        readonly Stopwatch _stopwatch = new();
        EndOfUpdateListener _endOfFrameListener;
        double _updateDuration;
        double _renderingStartTime;
        double _renderingDuration;
        bool _isRenderingStartedOnThisFrame;

        static FrameTimeMeasurer _instance;

        public static void Instantiate(Transform parent)
        {
            var obj = new GameObject("FrameTimeMeasurer");
            obj.gameObject.transform.parent = parent;
            FrameTimeMeasurer._instance = obj.AddComponent<FrameTimeMeasurer>();
        }

        public static FrameTimeMeasurer GetInstance() => FrameTimeMeasurer._instance;

        void Awake()
        {
            _endOfFrameListener = GetComponent<EndOfUpdateListener>();
            _endOfFrameListener.OnLateUpdate += OnEndOfUpdate;
            Camera.onPreRender += OnCameraPreRender;
            Camera.onPostRender += OnCameraPostRender;
        }

        void Update()
        {
            double frameDuration = _stopwatch.Elapsed.TotalSeconds;
            double updateMilliseconds = _updateDuration * 1000;
            double renderingMilliseconds = _renderingDuration * 1000;
            double othersMilliseconds = (frameDuration - _updateDuration - _renderingDuration) * 1000;
            OnUpdateFrameTime?.Invoke(updateMilliseconds, renderingMilliseconds, othersMilliseconds);

            _updateDuration = 0;
            _renderingDuration = 0;
            _isRenderingStartedOnThisFrame = false;
            _stopwatch.Restart();
        }

        void OnEndOfUpdate()
        {
            _updateDuration = _stopwatch.Elapsed.TotalSeconds;
        }

        void OnCameraPreRender(Camera _)
        {
            if (!_isRenderingStartedOnThisFrame)
            {
                _renderingStartTime = _stopwatch.Elapsed.TotalSeconds;
                _isRenderingStartedOnThisFrame = true;
            }
        }

        void OnCameraPostRender(Camera _)
        {
            _renderingDuration = _stopwatch.Elapsed.TotalSeconds - _renderingStartTime;
        }

        void OnDestroy()
        {
            OnUpdateFrameTime = default;
            _endOfFrameListener = default;
            FrameTimeMeasurer._instance = null;
        }
    }
}
