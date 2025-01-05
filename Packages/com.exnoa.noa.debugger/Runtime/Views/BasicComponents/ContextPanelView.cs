using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class ContextPanelView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _header;
        [SerializeField] TextMeshProUGUI _context;
        [SerializeField] Color _keyContextColor;
        [SerializeField] Color _valueContextColor;
        [SerializeField] float _valuePosition = 100;
        [SerializeField] LayoutElement _lineLayoutElement;

        public Color KeyContextColor { set => _keyContextColor = value; }
        public Color ValueContextColor { set => _valueContextColor = value; }

        public void SetText(string header, Dictionary<string, string> context, string prefix = "", string suffix = "", string missingValue = NoaDebuggerDefine.MISSING_VALUE)
        {
            _header.text = header;
            _context.text = _ConvertContext(context, prefix, suffix, missingValue);
        }

        string _ConvertContext(Dictionary<string, string> contextList, string prefix, string suffix, string missingValue)
        {
            var sb = new StringBuilder(prefix);
            var keyColorCode = ColorUtility.ToHtmlStringRGB(_keyContextColor);
            var valueColorCode = ColorUtility.ToHtmlStringRGB(_valueContextColor);

            if (contextList != null)
            {
                foreach (var item in contextList)
                {
                    sb.Append($"<color=#{keyColorCode}>{item.Key}:</color>");
                    sb.Append($"<pos={_valuePosition}><color=#{valueColorCode}>{_ConvertLabel(item.Value, missingValue)}</color></pos>");
                    sb.AppendLine();
                }
            }

            sb.Append(suffix);

            return sb.ToString();
        }

        string _ConvertLabel(string label, string missingValue)
        {
            if (!_IsAcquireLabel(label))
            {
                return missingValue;
            }

            return $"{label}";
        }

        bool _IsAcquireLabel(string label)
        {
            if (string.IsNullOrEmpty(label))
                return false;

            if (label == SystemInfo.unsupportedIdentifier)
                return false;

            return true;
        }

        public void SetMinWidthForLine(float width)
        {
            _lineLayoutElement.minWidth = width;
        }

        public void SetActiveLine(bool active)
        {
            _lineLayoutElement.gameObject.SetActive(active);
        }
    }
}
