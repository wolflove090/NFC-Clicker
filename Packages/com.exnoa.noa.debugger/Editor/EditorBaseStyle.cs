using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    static class EditorBaseStyle
    {
        public static Color FontColorBlack
        {
            get { return new Color(0.1f, 0.1f, 0.1f, 1); }
        }

        public static Color FontColorWhite
        {
            get { return new Color(0.9f, 0.9f, 0.9f, 1); }
        }

        public static Color FontColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                    return FontColorWhite;

                return FontColorBlack;
            }
        }

        public static GUIStyle Title (int fontSize)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.padding = new RectOffset(0, 0, 0, 0);

            style.alignment = TextAnchor.MiddleLeft;
            style.clipping = TextClipping.Overflow;

            style.wordWrap = true;

            style.fontSize = fontSize;

            style.richText = true;

            style.normal.textColor = FontColor;
            style.onNormal.textColor = FontColor;

            return style;
        }

        public static GUIStyle RichText ()
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.richText = true;

            return style;
        }

        public static GUIStyle Headline (int fontSize = 15)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);

            style.fontSize = fontSize;
            style.richText = true;

            return style;
        }

        public static GUIStyle Icon (float width, float height)
        {
            return new GUIStyle
            {
                margin = new RectOffset(5, 5, 5, 5),

                fixedWidth = width + 5,
                fixedHeight = height + 5,
            };
        }

        public static GUIStyle Padding (int padding)
        {
            return new GUIStyle
            {
                padding = new RectOffset(padding, padding, padding, padding),
            };
        }

        public static GUIStyle Padding (int left, int right, int top, int bottom)
        {
            return new GUIStyle
            {
                padding = new RectOffset(left, right, top, bottom),
            };
        }

        public static GUIStyle Box
        {
            get
            {
                GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
                boxStyle.padding.right += 3;
                boxStyle.padding.top += 3;
                boxStyle.padding.left += 3;
                boxStyle.padding.bottom += 3;

                return boxStyle;
            }
        }

        public static GUIStyle ContentBox
        {
            get
            {
                GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);

                boxStyle.padding.top = 3;
                boxStyle.padding.right = 3;
                boxStyle.padding.left = 3;
                boxStyle.padding.bottom = 3;

                boxStyle.margin = new RectOffset(3, 3, 5, 3);

                return boxStyle;
            }
        }

        public static void DrawUILine(Color color = default, int thickness = 1, int padding = 10, int margin = 0)
        {
            color = color != default ? color : Color.grey;
            Rect r = EditorGUILayout.GetControlRect(false, GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding * 0.5f;

            switch (margin)
            {
                case < 0:
                    r.x = 0;
                    r.width = EditorGUIUtility.currentViewWidth;
                    break;
                case > 0:
                    r.x += margin;
                    r.width -= margin * 2;
                    break;
            }

            EditorGUI.DrawRect(r, color);
        }

        public static GUIStyle ButtonHighlighted
        {
            get
            {
                var buttonStyle = new GUIStyle(GUI.skin.button);

                buttonStyle.normal.textColor = Color.cyan;
                buttonStyle.hover.textColor = Color.white;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.margin = new RectOffset(10, 10, 10, 10);
                buttonStyle.padding = new RectOffset(5, 5, 5, 5);

                return buttonStyle;
            }
        }
    }
}
