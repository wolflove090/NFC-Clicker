using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class ProfilerDrawerComponent : MonoBehaviour
    {
        static readonly string BatteryInProgressLabel = "InProgress...";
        static readonly string BatteryChargingLabel = "Charging";

        [Header("FPS")]
        [SerializeField]
        TextMeshProUGUI _currentFps;

        [SerializeField]
        TextMeshProUGUI _maxAndMinFps;

        [SerializeField]
        TextMeshProUGUI _averageFps;

        [Header("FrameTime")]
        [SerializeField]
        StackedAreaChart _frameTimeChart;

        [Header("Memory")]
        [SerializeField]
        StackedAreaChart _memoryChart;

        [SerializeField]
        GaugeChart _memoryGauge;

        [SerializeField]
        TextMeshProUGUI _totalMemory;

        [SerializeField]
        TextMeshProUGUI _currentMemory;

        [SerializeField]
        TextMeshProUGUI _maxMemory;

        [SerializeField]
        TextMeshProUGUI _minMemory;

        [SerializeField]
        TextMeshProUGUI _averageMemory;

        [Header("Rendering")]
        [SerializeField]
        StackedAreaChart _renderingChart;

        [SerializeField]
        TextMeshProUGUI _setPassCalls;

        [SerializeField]
        TextMeshProUGUI _drawCalls;

        [SerializeField]
        TextMeshProUGUI _batches;

        [SerializeField]
        TextMeshProUGUI _triangles;

        [SerializeField]
        TextMeshProUGUI _vertices;

        [Header("Battery")]
        [SerializeField]
        TextMeshProUGUI _currentBatteryAndMinute;

        [SerializeField]
        TextMeshProUGUI _operatingTime;

        [Header("Thermal")]
        [SerializeField]
        TextMeshProUGUI _currentThermal;

        [SerializeField]
        TextMeshProUGUI _maxThermal;

        [SerializeField]
        TextMeshProUGUI _minThermal;

        [SerializeField]
        TextMeshProUGUI _averageThermal;

        static readonly float[] FrameTimeChartFpsRulerValues =
        {
            1 / 64f,
            1 / 32f,
            1 / 16f,
            1 / 8f,
            1 / 4f,
            1 / 2f,
            1f,
            2f,
            4f,
            8f,
            15f,
            30f,
            60f,
            120f,
            240f,
            500f,
            1000f
        };

        static readonly Dictionary<float, string> FrameTimeRulerStringCache = new();
        static readonly Dictionary<float, string> MemoryRulerStringCache = new();
        static readonly Dictionary<float, string> IntegerValueRulerStringCache = new();

        void Awake()
        {
            Assert.IsNotNull(_currentFps);
            Assert.IsNotNull(_maxAndMinFps);
            Assert.IsNotNull(_averageFps);
            Assert.IsNotNull(_totalMemory);
            Assert.IsNotNull(_currentMemory);
            Assert.IsNotNull(_maxMemory);
            Assert.IsNotNull(_minMemory);
            Assert.IsNotNull(_averageMemory);
            Assert.IsNotNull(_setPassCalls);
            Assert.IsNotNull(_drawCalls);
            Assert.IsNotNull(_batches);
            Assert.IsNotNull(_triangles);
            Assert.IsNotNull(_vertices);
            Assert.IsNotNull(_currentBatteryAndMinute);
            Assert.IsNotNull(_operatingTime);
            Assert.IsNotNull(_currentThermal);
            Assert.IsNotNull(_maxThermal);
            Assert.IsNotNull(_minThermal);
            Assert.IsNotNull(_averageThermal);

            ProfilerDrawerComponent.CreateFrameTimeRulerStringCache();

            if (_frameTimeChart != null)
            {
                _frameTimeChart.SetUpdateRulersCallback(ProfilerDrawerComponent.OnUpdateFrameTimeChartRulers);
            }

            if (_memoryChart != null)
            {
                _memoryChart.SetUpdateRulersCallback(ProfilerDrawerComponent.OnUpdateMemoryChartRulers);
            }

            if (_renderingChart != null)
            {
                _renderingChart.SetUpdateRulersCallback(ProfilerDrawerComponent.OnUpdateRenderingChartRulers);
            }
        }

        public void OnShowFps(FpsUnchangingInfo info)
        {
            _currentFps.color = NoaDebuggerDefine.TextColors.Dynamic;
            _maxAndMinFps.color = NoaDebuggerDefine.TextColors.Dynamic;
            _averageFps.color = NoaDebuggerDefine.TextColors.Dynamic;

            if (info.IsViewHyphen)
            {
                _currentFps.text = $"{NoaDebuggerDefine.HyphenValue}";
                _maxAndMinFps.text = $"{NoaDebuggerDefine.HyphenValue}";
                _averageFps.text = $"{NoaDebuggerDefine.HyphenValue}";

                _currentFps.color = NoaDebuggerDefine.TextColors.Default;
                _maxAndMinFps.color = NoaDebuggerDefine.TextColors.Default;
                _averageFps.color = NoaDebuggerDefine.TextColors.Default;
            }
            else
            {
                _currentFps.text = $"{info.CurrentFps} fps ({info.ElapsedTime} ms)";
                _maxAndMinFps.text = $"{info.MaxStr} fps / {info.MinStr} fps";
                _averageFps.text = $"{info.AverageFps} fps";
            }
        }

        public void OnShowFrameTime(ProfilerFrameTimeViewInformation info)
        {
            _frameTimeChart.SetValueHistoryBuffer(info._histories);
        }

        static void CreateFrameTimeRulerStringCache()
        {
            foreach (float fps in ProfilerDrawerComponent.FrameTimeChartFpsRulerValues)
            {
                if (ProfilerDrawerComponent.FrameTimeRulerStringCache.ContainsKey(fps))
                {
                    continue;
                }

                float frameTime = ProfilerDrawerComponent.FpsToFrameTimeMilliseconds(fps);
                ProfilerDrawerComponent.FrameTimeRulerStringCache[fps]= $"{(int)frameTime}ms ({fps}FPS)";
            }
        }

        static void OnUpdateFrameTimeChartRulers(float maxValue, float[] positions, string[] labels)
        {
            Assert.AreEqual(positions.Length, 2, "Check 'Ruler Count' of StackedAreaChart.");
            Assert.AreEqual(labels.Length, 2, "Check 'Ruler Count' of StackedAreaChart.");

            for (var i = 0; i < ProfilerDrawerComponent.FrameTimeChartFpsRulerValues.Length; ++i)
            {
                float maxFps = ProfilerDrawerComponent.FrameTimeChartFpsRulerValues[i];
                float maxFrameTime = ProfilerDrawerComponent.FpsToFrameTimeMilliseconds(maxFps);

                if (maxValue > maxFrameTime)
                {
                    labels[0] = ProfilerDrawerComponent.FrameTimeRulerStringCache[maxFps];
                    positions[0] = maxFrameTime;

                    if (i < ProfilerDrawerComponent.FrameTimeChartFpsRulerValues.Length - 1)
                    {
                        float altFps = ProfilerDrawerComponent.FrameTimeChartFpsRulerValues[i + 1];
                        labels[1] = ProfilerDrawerComponent.FrameTimeRulerStringCache[altFps];
                        positions[1] = ProfilerDrawerComponent.FpsToFrameTimeMilliseconds(altFps);
                    }

                    break;
                }
            }
        }

        static float FpsToFrameTimeMilliseconds(float fps) => (1 / fps) * 1000f;

        public void OnShowRendering(RenderingUnchangingInfo info)
        {
            if (_renderingChart != null)
            {
                _renderingChart.SetValueHistoryBuffer(info.CurrentValueHistory);
            }
            _setPassCalls.color = NoaDebuggerDefine.TextColors.Dynamic;
            _drawCalls.color = NoaDebuggerDefine.TextColors.Dynamic;
            _batches.color = NoaDebuggerDefine.TextColors.Dynamic;
            _triangles.color = NoaDebuggerDefine.TextColors.Dynamic;
            _vertices.color = NoaDebuggerDefine.TextColors.Dynamic;

            if (info.IsViewHyphen)
            {
                _setPassCalls.text = $"{NoaDebuggerDefine.HyphenValue}";
                _drawCalls.text = $"{NoaDebuggerDefine.HyphenValue}";
                _batches.text = $"{NoaDebuggerDefine.HyphenValue}";
                _triangles.text = $"{NoaDebuggerDefine.HyphenValue}";
                _vertices.text = $"{NoaDebuggerDefine.HyphenValue}";

                _setPassCalls.color = NoaDebuggerDefine.TextColors.Default;
                _drawCalls.color = NoaDebuggerDefine.TextColors.Default;
                _batches.color = NoaDebuggerDefine.TextColors.Default;
                _triangles.color = NoaDebuggerDefine.TextColors.Default;
                _vertices.color = NoaDebuggerDefine.TextColors.Default;
            }
            else
            {
                _setPassCalls.text = $"{info.CurrentSetPassCalls} ({info.MaxSetPassCallsStr})";
                _drawCalls.text = $"{info.CurrentDrawCalls} ({info.MaxDrawCallsStr})";
                _batches.text = $"{info.CurrentBatches} ({info.MaxBatchesStr})";
                _triangles.text = $"{info.CurrentTriangles} ({info.MaxTrianglesStr})";
                _vertices.text = $"{info.CurrentVertices} ({info.MaxVerticesStr})";
            }
        }

        static void OnUpdateRenderingChartRulers(float maxValue, float[] positions, string[] labels)
        {
            Assert.AreEqual(positions.Length, 2, "Check 'Ruler Count' of StackedAreaChart.");
            Assert.AreEqual(labels.Length, 2, "Check 'Ruler Count' of StackedAreaChart.");

            positions[0] = ToRulerValue(maxValue);
            labels[0] = toRulerText(positions[0]);
            positions[1] = positions[0] / 2;
            labels[1] = toRulerText(positions[1]);
            return;

            string toRulerText(float value)
            {
                if (ProfilerDrawerComponent.IntegerValueRulerStringCache.TryGetValue(value, out string rulerText))
                {
                    return rulerText;
                }
                rulerText = value.ToString(CultureInfo.InvariantCulture);
                ProfilerDrawerComponent.IntegerValueRulerStringCache[value] = rulerText;
                return rulerText;
            }
        }

        public void OnShowMemory(MemoryUnchangingInfo info)
        {
            if (_memoryChart != null)
            {
                _memoryChart.SetValueHistoryBuffer(info.CurrentMemoryHistory);
            }
            _totalMemory.color = NoaDebuggerDefine.TextColors.Dynamic;
            _currentMemory.color = NoaDebuggerDefine.TextColors.Dynamic;
            _maxMemory.color = NoaDebuggerDefine.TextColors.Dynamic;
            _minMemory.color = NoaDebuggerDefine.TextColors.Dynamic;
            _averageMemory.color = NoaDebuggerDefine.TextColors.Dynamic;

            if (info.IsViewHyphen)
            {
                if (_memoryGauge != null)
                {
                    _memoryGauge.MaxValue = 0;
                    _memoryGauge.Value = 0;
                }
                _currentMemory.text = $"{NoaDebuggerDefine.HyphenValue}";
                _maxMemory.text = $"{NoaDebuggerDefine.HyphenValue}";
                _minMemory.text = "";
                _averageMemory.text = $"{NoaDebuggerDefine.HyphenValue}";
                _totalMemory.text = $"{NoaDebuggerDefine.HyphenValue}";

                _totalMemory.color = NoaDebuggerDefine.TextColors.Default;
                _currentMemory.color = NoaDebuggerDefine.TextColors.Default;
                _minMemory.color = NoaDebuggerDefine.TextColors.Default;
                _maxMemory.color = NoaDebuggerDefine.TextColors.Default;
                _averageMemory.color = NoaDebuggerDefine.TextColors.Default;
                return;
            }

            if (info.CurrentMemoryMB <= -1)
            {
                if (_memoryGauge != null)
                {
                    _memoryGauge.MaxValue = 0;
                    _memoryGauge.Value = 0;
                }
                _totalMemory.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _currentMemory.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _maxMemory.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _minMemory.text = "";
                _averageMemory.text = $"{NoaDebuggerDefine.MISSING_VALUE}";

                _totalMemory.color = NoaDebuggerDefine.TextColors.Default;
                _currentMemory.color = NoaDebuggerDefine.TextColors.Default;
                _minMemory.color = NoaDebuggerDefine.TextColors.Default;
                _maxMemory.color = NoaDebuggerDefine.TextColors.Default;
                _averageMemory.color = NoaDebuggerDefine.TextColors.Default;
            }
            else
            {
                if (_memoryGauge != null)
                {
                    _memoryGauge.MaxValue = info.TotalMemoryMB;
                    _memoryGauge.Value = info.CurrentMemoryMB;
                }
                var totalMemory = (long)DataUnitConverterModel.MBToByte(info.TotalMemoryMB);
                _totalMemory.text = $"{DataUnitConverterModel.ToHumanReadableBytes(totalMemory)}";

                _currentMemory.text = $"{info.CurrentMemoryMB}MB";
                _maxMemory.text = $"{info.MaxMemoryMB}MB /";
                _minMemory.text = $" {info.MinMemoryMB}MB";
                _averageMemory.text = $"{info.AverageMemoryMB}MB";
            }
        }

        static void OnUpdateMemoryChartRulers(float maxValue, float[] positions, string[] labels)
        {
            Assert.AreEqual(positions.Length, 2, "Check 'Ruler Count' of StackedAreaChart.");
            Assert.AreEqual(labels.Length, 2, "Check 'Ruler Count' of StackedAreaChart.");

            positions[0] = ToRulerValue(maxValue);
            labels[0] = toRulerText(positions[0]);
            positions[1] = positions[0] / 2;
            labels[1] = toRulerText(positions[1]);
            return;

            string toRulerText(float value)
            {
                if (ProfilerDrawerComponent.MemoryRulerStringCache.TryGetValue(value, out string rulerText))
                {
                    return rulerText;
                }
                rulerText = $"{value.ToString(CultureInfo.InvariantCulture)}MB";
                ProfilerDrawerComponent.MemoryRulerStringCache[value] = rulerText;
                return rulerText;
            }
        }

        public void OnShowBattery(BatteryUnchangingInfo info)
        {
            if (info.IsViewHyphen)
            {
                _currentBatteryAndMinute.text = $"{NoaDebuggerDefine.HyphenValue}";
                _operatingTime.text = $"{NoaDebuggerDefine.HyphenValue}";
                _currentBatteryAndMinute.color = NoaDebuggerDefine.TextColors.Default;
                _operatingTime.color = NoaDebuggerDefine.TextColors.Default;
            }

            else if (info.Status == BatteryStatus.Unknown)
            {
                _currentBatteryAndMinute.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _operatingTime.text = $"{NoaDebuggerDefine.MISSING_VALUE}";

                _currentBatteryAndMinute.color = NoaDebuggerDefine.TextColors.Default;
                _operatingTime.color = NoaDebuggerDefine.TextColors.Default;
            }
            else
            {
                string minuteLabel = $"{info.ConsumptionPerMinute.ToString(CultureInfo.InvariantCulture)}%";
                Color currentBatteryLabelColor = NoaDebuggerDefine.TextColors.Dynamic;

                if (info.Status == BatteryStatus.Profiling)
                {
                    minuteLabel = ProfilerDrawerComponent.BatteryInProgressLabel;
                    currentBatteryLabelColor = NoaDebuggerDefine.TextColors.InProgress;
                }

                if (info.Status == BatteryStatus.Charging)
                {
                    minuteLabel = ProfilerDrawerComponent.BatteryChargingLabel;
                    currentBatteryLabelColor = NoaDebuggerDefine.TextColors.InProgress;
                }

                _currentBatteryAndMinute.text = $"{info.CurrentLevelPercent}%({minuteLabel})";
                _currentBatteryAndMinute.color = currentBatteryLabelColor;
                var span = new TimeSpan(0, 0, info.OperatingTimeSec);
                _operatingTime.text = span.ToString(@"hh\:mm\:ss");
                _operatingTime.color = NoaDebuggerDefine.TextColors.Dynamic;
            }
        }

        public void OnShowThermal(ThermalUnchangingInfo info)
        {
            _currentThermal.color = NoaDebuggerDefine.TextColors.Dynamic;
            _maxThermal.color = NoaDebuggerDefine.TextColors.Dynamic;
            _minThermal.color = NoaDebuggerDefine.TextColors.Dynamic;
            _averageThermal.color = NoaDebuggerDefine.TextColors.Dynamic;

            if (info.IsViewHyphen)
            {
                _currentThermal.text = $"{NoaDebuggerDefine.HyphenValue}";
                _maxThermal.text = $"{NoaDebuggerDefine.HyphenValue}";
                _minThermal.text = "";
                _averageThermal.text = $"{NoaDebuggerDefine.HyphenValue}";

                _maxThermal.color = NoaDebuggerDefine.TextColors.Default;
                _minThermal.color = NoaDebuggerDefine.TextColors.Default;
                _currentThermal.color = NoaDebuggerDefine.TextColors.Default;
                _averageThermal.color = NoaDebuggerDefine.TextColors.Default;
            }

            else if (info.CurrentThermalStatus == ThermalStatus.Unknown)
            {
                _currentThermal.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _maxThermal.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _minThermal.text = "";
                _averageThermal.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _maxThermal.color = NoaDebuggerDefine.TextColors.Default;
                _minThermal.color = NoaDebuggerDefine.TextColors.Default;
                _currentThermal.color = NoaDebuggerDefine.TextColors.Default;
                _averageThermal.color = NoaDebuggerDefine.TextColors.Default;
            }

            else if (info.CurrentTemperature <= -1)
            {
                _currentThermal.text =
                    $"{ThermalUnchangingInfo.ConvertThermalStatusText(info.CurrentThermalStatus)}";

                _maxThermal.text =
                    $"{ThermalUnchangingInfo.ConvertThermalStatusText(info.MaxThermalStatus)} /";

                _minThermal.text =
                    $" {ThermalUnchangingInfo.ConvertThermalStatusText(info.MinThermalStatus)}";

                _averageThermal.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _averageThermal.color = NoaDebuggerDefine.TextColors.Default;
            }
            else
            {
                _currentThermal.text = $"{info.CurrentTemperature} {NoaDebuggerDefine.DegreesCelsiusLabel}";
                _maxThermal.text = $"{info.MaxTemperature} {NoaDebuggerDefine.DegreesCelsiusLabel} /";
                _minThermal.text = $" {info.MinTemperature} {NoaDebuggerDefine.DegreesCelsiusLabel}";
                _averageThermal.text = $"{info.AverageTemperature} {NoaDebuggerDefine.DegreesCelsiusLabel}";
            }
        }

        static float ToRulerValue(float value)
        {
            float log = Mathf.Floor(Mathf.Log10(value));
            float rulerValue = Mathf.Pow(10, log);
            rulerValue *= Mathf.Floor(value / rulerValue);
            return Mathf.Floor(value / rulerValue) * rulerValue;
        }

        void OnDestroy()
        {
            _currentFps = default;
            _maxAndMinFps = default;
            _averageFps = default;
            _frameTimeChart = default;
            _memoryChart = default;
            _memoryGauge = default;
            _totalMemory = default;
            _currentMemory = default;
            _maxMemory = default;
            _minMemory = default;
            _averageMemory = default;
            _renderingChart = default;
            _setPassCalls = default;
            _drawCalls = default;
            _batches = default;
            _triangles = default;
            _vertices = default;
            _currentBatteryAndMinute = default;
            _operatingTime = default;
            _currentThermal = default;
            _maxThermal = default;
            _minThermal = default;
            _averageThermal = default;
        }
    }
}
