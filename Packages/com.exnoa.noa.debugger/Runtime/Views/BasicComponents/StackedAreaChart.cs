using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    sealed class StackedAreaChart : MaskableGraphic
    {
        const int MINIMUM_HISTORY_CAPACITY = 2;

        const int MINIMUM_STACK_COUNT = 1;

        const int VERTEX_COUNT_PER_HISTORY = 2;

        const int INDEX_COUNT_BETWEEN_HISTORIES = 6;

        [Serializable]
        struct StackDisplayAttributes
        {
            public string _text;
            public Color _color;
            public bool _shows;
        }

        [SerializeField, Tooltip("Stack attributes. Also applied to legend. At least an element is required.")]
        StackDisplayAttributes[] _stackDisplayAttributes;

        [SerializeField, Tooltip("Legend for this chart.")]
        ChartLegend _legend;

        [SerializeField, Tooltip("Ruler count to show.")]
        int _rulerCount = 1;

        [SerializeField, Tooltip("Template of ruler. It will be cloned if multiple rulers are shown.")]
        StackedAreaChartRuler _rulerTemplate;

        [SerializeField, Tooltip("Root Transform which rulers are instantiated.")]
        Transform _rulerRoot;

        List<UIVertex> _vertices;
        List<int> _indices;
        RingBuffer<float[]> _valueHistories;
        StackedAreaChartRuler[] _rulers;
        float[] _rulerPositions;
        string[] _rulerLabels;
        Action<float, float[], string[]> _onUpdateRulers;

        public void SetUpdateRulersCallback(Action<float, float[], string[]> callback)
        {
            _onUpdateRulers = callback;
        }

        public void SetValueHistoryBuffer(RingBuffer<float[]> buffer)
        {
            if (buffer == null || buffer.Capacity < StackedAreaChart.MINIMUM_HISTORY_CAPACITY)
            {
                throw new ArgumentException(
                    $"Length of {nameof(buffer)} must be equal or higher than {StackedAreaChart.MINIMUM_HISTORY_CAPACITY}.");
            }

            int? formerBufferCapacity = _valueHistories?.Capacity;
            _valueHistories = buffer;

            if (formerBufferCapacity != buffer.Capacity)
            {
                InitializeVertices();
            }

            SetVerticesDirty();
        }

        public void ToggleStacksShown(bool[] shows)
        {
            if (shows == null || shows.Length != StackCount)
            {
                throw new ArgumentException(
                    $"Length of {nameof(shows)} must be the same as the length of {nameof(StackedAreaChart._stackDisplayAttributes)}.");
            }

            for (var stack = 0; stack < StackCount; ++stack)
            {
                _stackDisplayAttributes[stack]._shows = shows[stack];
            }

            SetVerticesDirty();
        }

        protected override void Awake()
        {
            Assert.IsTrue(StackCount >= StackedAreaChart.MINIMUM_STACK_COUNT);
            Assert.IsNotNull(_legend);
            Assert.IsTrue(_rulerCount > 0);
            Assert.IsNotNull(_rulerTemplate);

            if (!Application.IsPlaying(gameObject))
            {
                return;
            }

            foreach (StackDisplayAttributes stack in _stackDisplayAttributes)
            {
                _legend.AddElement(stack._text, stack._color);
            }

            _rulers = new StackedAreaChartRuler[_rulerCount];
            _rulerPositions = new float[_rulerCount];
            _rulerLabels = new string[_rulerCount];

            for (var i = 0; i < _rulerCount; ++i)
            {
                StackedAreaChartRuler ruler = Instantiate(_rulerTemplate, _rulerRoot);
                ruler.gameObject.SetActive(true);
                _rulers[i] = ruler;
            }

            _rulerTemplate.gameObject.SetActive(false);
        }

        bool CanPopulateMesh => Application.IsPlaying(gameObject) && _valueHistories != null;

        int StackCount => _stackDisplayAttributes.Length;

        void InitializeVertices()
        {
            Assert.IsNotNull(_valueHistories);

            int historyCapacity = _valueHistories.Capacity;
            int vertexCount = historyCapacity * StackedAreaChart.VERTEX_COUNT_PER_HISTORY * StackCount;
            int indexCount = (historyCapacity - 1) * StackedAreaChart.INDEX_COUNT_BETWEEN_HISTORIES * StackCount;
            _vertices = new List<UIVertex>(vertexCount);
            _indices = new List<int>(indexCount);

            for (var stack = 0; stack < StackCount; ++stack)
            {
                _vertices.Add(new UIVertex { position = Vector3.zero, color = _stackDisplayAttributes[stack]._color });
                _vertices.Add(new UIVertex { position = Vector3.zero, color = _stackDisplayAttributes[stack]._color });

                for (var history = 1; history < historyCapacity; ++history)
                {
                    _vertices.Add(new UIVertex { position = Vector3.zero, color = _stackDisplayAttributes[stack]._color });
                    _vertices.Add(new UIVertex { position = Vector3.zero, color = _stackDisplayAttributes[stack]._color });

                    int baseIndex = GetFirstVertexOf(history - 1, stack);
                    _indices.Add(baseIndex + 0);
                    _indices.Add(baseIndex + 1);
                    _indices.Add(baseIndex + 2);
                    _indices.Add(baseIndex + 1);
                    _indices.Add(baseIndex + 2);
                    _indices.Add(baseIndex + 3);
                }
            }
        }

        void Update()
        {
            if (!CanPopulateMesh)
            {
                return;
            }

            for (var i = 0; i < _rulerLabels.Length; ++i)
            {
                _rulerLabels[i] ??= string.Empty;

                if (!_rulers[i].Text.Equals(_rulerLabels[i]))
                {
                    _rulers[i].Text = _rulerLabels[i];
                }
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (!CanPopulateMesh)
            {
                HideRulers();
                return;
            }

            UpdateVertices();
            vh.AddUIVertexStream(_vertices, _indices);
        }

        void UpdateVertices()
        {
            Rect rect = rectTransform.rect;

            var history = 0;
            int historyWithNoValuesCount = _valueHistories.Capacity - _valueHistories.Count;

            while (history < historyWithNoValuesCount)
            {
                float x = GetHorizontalPositionOf(history);
                var bottom = new Vector3(x, rect.y);
                var top = new Vector3(x, rect.y);

                for (int stack = StackCount - 1; stack >= 0; --stack)
                {
                    int vertex = GetFirstVertexOf(history, stack);
                    _vertices[vertex + 0] = new UIVertex { position = bottom, color = _vertices[vertex + 0].color };
                    _vertices[vertex + 1] = new UIVertex { position = top, color = _vertices[vertex + 1].color };
                }

                ++history;
            }

            var maxValue = float.MinValue;

            foreach (float[] values in _valueHistories)
            {
                Assert.AreEqual(values.Length, StackCount);

                float x = GetHorizontalPositionOf(history);
                float y = rect.y;

                for (int stack = StackCount - 1; stack >= 0; --stack)
                {
                    float value = _stackDisplayAttributes[stack]._shows ? values[stack] : 0.0f;
                    var bottom = new Vector3(x, y);
                    var top = new Vector3(x, y + value);
                    int vertex = GetFirstVertexOf(history, stack);
                    _vertices[vertex + 0] = new UIVertex { position = bottom, color = _vertices[vertex + 0].color };
                    _vertices[vertex + 1] = new UIVertex { position = top, color = _vertices[vertex + 1].color };
                    y += value;
                }

                float historyValue = y - rect.y;

                if (historyValue > maxValue)
                {
                    maxValue = historyValue;
                }

                ++history;
            }

            if (maxValue <= 0.0f)
            {
                HideRulers();
            }
            else
            {
                float scaleY = 1 / maxValue * rect.height;
                rectTransform.localScale = new Vector3(1, scaleY, 1);

                for (var i = 0; i < _rulers.Length; ++i)
                {
                    _rulerPositions[i] = 0.0f;
                    _rulerLabels[i] = string.Empty;
                }

                _onUpdateRulers?.Invoke(maxValue, _rulerPositions, _rulerLabels);

                for (var i = 0; i < _rulers.Length; ++i)
                {
                    if (_rulerPositions[i] > 0.0f && !string.IsNullOrEmpty(_rulerLabels[i]))
                    {
                        _rulers[i].RectTransform.localPosition = new Vector3(0, _rulerPositions[i] * scaleY, 0);
                        _rulers[i].RectTransform.localScale = Vector3.one;
                    }
                    else
                    {
                        _rulers[i].RectTransform.localScale = Vector3.zero;
                    }
                }
            }
        }

        void HideRulers()
        {
            if (_rulers == null)
            {
                return;
            }

            foreach (StackedAreaChartRuler ruler in _rulers)
            {
                ruler.RectTransform.localScale = Vector3.zero;
            }
        }

        int GetFirstVertexOf(int history, int stack)
        {
            return (stack * _valueHistories.Capacity + history) * StackedAreaChart.VERTEX_COUNT_PER_HISTORY;
        }

        float GetHorizontalPositionOf(int history)
        {
            Rect rect = rectTransform.rect;
            return rect.x + rect.width / (_valueHistories.Capacity - 1) * history;
        }
    }
}
