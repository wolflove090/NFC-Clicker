using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NoaDebugger
{
    sealed class NoaDebuggerText : TextMeshProUGUI
    {

        static TMP_FontAsset _runtimeDefaultFontAsset;
        static Material _runtimeDefaultFontMaterial;
        static float _runtimeDefaultFontSizeRate;

        static TMP_FontAsset _fontAsset;
        static Material _fontMaterial;
        static float _fontSizeRate;
        static bool _isInit;

        public static void Init(NoaDebuggerSettings settings)
        {
            if (settings.IsCustomFontSpecified)
            {
                NoaDebuggerText._runtimeDefaultFontAsset = settings.FontAsset;
                NoaDebuggerText._runtimeDefaultFontMaterial = settings.FontMaterial;
                NoaDebuggerText._runtimeDefaultFontSizeRate = settings.FontSizeRate;

                NoaDebuggerText.ChangeFont(settings.FontAsset, settings.FontMaterial, settings.FontSizeRate);
            }
            _isInit = true;
        }

        public static void ChangeFont(TMP_FontAsset fontAsset, Material fontMaterial, float fontSizeRate)
        {
            _fontAsset = fontAsset;
            _fontMaterial = fontMaterial;
            _fontSizeRate = fontSizeRate;
        }

        public static void ResetFont()
        {
            ChangeFont(NoaDebuggerText._runtimeDefaultFontAsset,
                       NoaDebuggerText._runtimeDefaultFontMaterial,
                       NoaDebuggerText._runtimeDefaultFontSizeRate);
        }

        public static bool HasFontAsset => NoaDebuggerText._fontAsset != null;

        public static string GetHasFontAssetString(TMP_FontAsset targetFont, string text)
        {
            if (targetFont == null || string.IsNullOrEmpty(text))
            {
                return text;
            }

            text = _DecodeUnicodeEscapedString(text);

            targetFont.HasCharacters(text, out List<char> missingCharacters);

            if (targetFont.atlasPopulationMode == AtlasPopulationMode.Dynamic && missingCharacters.Count > 0)
            {
                targetFont.TryAddCharacters(text);
            }

            string result = text;
            foreach(char character in missingCharacters)
            {
                var unicodeEscape = _GetUnicodeEscapeSequence(character);
                result = result.Replace(character.ToString(), unicodeEscape);
            }

            return result;
        }

        static string _DecodeUnicodeEscapedString(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, @"\\u([0-9A-Fa-f]{4})", match =>
            {
                var unicodeHex = match.Groups[1].Value;
                var unicodeDecimal = int.Parse(unicodeHex, System.Globalization.NumberStyles.HexNumber);

                if (unicodeDecimal == 0x0020)
                {
                    return "u0020";
                }

                return ((char)unicodeDecimal).ToString();
            });
        }

        static string _GetUnicodeEscapeSequence(char character)
        {
            return $"u{((int)character):X4}";
        }


        public override string text
        {
            get => base.text;
            set
            {
                ApplyFont();
                base.text = _TruncateToMaxLength(value);
            }
        }

        bool _isApplied;

        float? _defaultFontSize = null;
        float? _defaultFontSizeMin = null;
        float? _defaultFontSizeMax = null;

        protected override void Awake()
        {
            base.Awake();

            ApplyFont();
        }

        public void ApplyFont(bool isForce = false)
        {
            if (!isForce)
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                if (_isApplied)
                {
                    return;
                }

                _isApplied = true;
            }

            if (!_isInit)
            {
                LogModel.DebugLogWarning("NoaDebuggerText is not initialized.");
            }

            if (NoaDebuggerText._runtimeDefaultFontAsset == null)
            {
                NoaDebuggerText._runtimeDefaultFontAsset = font;
                NoaDebuggerText._runtimeDefaultFontMaterial = fontMaterial;
                NoaDebuggerText._runtimeDefaultFontSizeRate = NoaDebuggerDefine.DEFAULT_FONT_SIZE_RATE;
            }

            _defaultFontSize ??= fontSize;
            _defaultFontSizeMin ??= fontSizeMin;
            _defaultFontSizeMax ??= fontSizeMax;

            if (_fontAsset == null)
            {
                return;
            }

            font = _fontAsset;
            fontMaterial = _fontMaterial;

            fontSize = _defaultFontSize.Value * _fontSizeRate;

            if (enableAutoSizing)
            {
                fontSizeMin = _defaultFontSizeMin.Value * _fontSizeRate;
                fontSizeMax = _defaultFontSizeMax.Value * _fontSizeRate;
            }
        }

        string _TruncateToMaxLength(string text)
        {
            const int maxLength = 99999;
            if (!string.IsNullOrEmpty(text) && text.Length > maxLength)
            {
                string result = text.Substring(0, maxLength);
                return result;
            }

            return text;
        }
    }
}
