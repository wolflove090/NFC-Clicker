using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class ChartLegend : MonoBehaviour
    {
        [SerializeField, Tooltip("Template of element.")]
        ChartLegendElement _elementTemplate;

        readonly List<ChartLegendElement> _elements = new();

        void Awake()
        {
            Assert.IsNotNull(_elementTemplate);

            _elementTemplate.gameObject.SetActive(false);
        }

        public int AddElement(string text, Color color)
        {
            ChartLegendElement element = Instantiate(_elementTemplate, transform);
            element.gameObject.SetActive(true);
            element.Text = text;
            element.Color = color;
            _elements.Add(element);
            return _elements.Count - 1;
        }

        public void UpdateText(int index, string text)
        {
            _elements[index].Text = text;
        }

        public void UpdateColor(int index, Color color)
        {
            _elements[index].Color = color;
        }

        public void ToggleShow(int index, bool shows)
        {
            _elements[index].gameObject.SetActive(shows);
        }
    }
}
