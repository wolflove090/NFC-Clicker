using System;
using System.Linq;
using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Display information
    /// </summary>
    public sealed class DisplayInfo
    {
        /// <summary>
        /// Number of decimal places for the aspect ratio value
        /// </summary>
        static readonly int AspectRatioValueDigits = 2;

        /// <summary>
        /// Number of decimals for the display aspect ratio
        /// </summary>
        static readonly int AspectRatioXYDigits = 1;

        /// <summary>
        /// A common aspect ratio used as a reference.\n
        /// If there is a match, it will be displayed as is. If not, it will be adjusted and displayed based on the closest value.
        /// </summary>
        /// <remarks>
        /// For calculation convenience, make sure that Item1>=Item2
        /// </remarks>
        static readonly (int, int)[] CommonAspectRatios =
        {
            (1, 1), 
            (6, 5), 
            (11, 9), 
            (5, 4), 
            (4, 3), 
            (3, 2), 
            (16, 10), 
            (15, 9), 
            (16, 9), 
            (17, 9), 
            (2, 1), 
            (19, 9), 
            (20, 9), 
            (21, 9), 
            (32, 9), 
        };

        /// <summary>
        /// Refresh rate
        /// </summary>
        public int RefreshRate { private set; get; }

        /// <summary>
        /// Screen width
        /// </summary>
        public int ScreenWidth { private set; get; }

        /// <summary>
        /// Screen height
        /// </summary>
        public int ScreenHeight { private set; get; }

        /// <summary>
        /// The numerical representation of the aspect ratio, adjusted to be greater than or equal to 1.\n
        /// For example, the 1.33 in 3:4 (1:1.33)
        /// </summary>
        public float AspectRatioValue { private set; get; }

        /// <summary>
        /// The X-direction aspect ratio for display
        /// </summary>
        public float AspectX { private set; get; }

        /// <summary>
        /// The Y-direction aspect ratio for display
        /// </summary>
        public float AspectY { private set; get; }

        /// <summary>
        /// Terminal's DPI
        /// </summary>
        public string Dpi { private set; get; }

        /// <summary>
        /// Terminal's screen orientation type
        /// </summary>
        public string Orientation { private set; get; }

        /// <summary>
        /// Generates DisplayInfo
        /// </summary>
        internal DisplayInfo()
        {
            Refresh();
        }

        /// <summary>
        /// Updates the Display information
        /// </summary>
        internal void Refresh()
        {
#if UNITY_2022_2_OR_NEWER
            RefreshRate = (int) System.Math.Round(Screen.currentResolution.refreshRateRatio.value);
#else
            RefreshRate = Screen.currentResolution.refreshRate;
#endif
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
            (float, float, float) aspect = _GetAspectRatio(Screen.width, Screen.height);
            AspectRatioValue = aspect.Item1;
            AspectX = aspect.Item2;
            AspectY = aspect.Item3;
            Dpi = Screen.dpi.ToString();
            Orientation = _GetScreenOrientationString();
        }

        /// <summary>
        /// Returns the appropriate aspect ratio from the screen size.
        /// </summary>
        /// <param name="screenWidth">Specifies the screen width size</param>
        /// <param name="screenHeight">Specifies the screen height size</param>
        /// <returns>
        /// Item1: Returns aspect ratio as a float greater than or equal to 1.\n
        /// Item2: Returns X-direction aspect ratio for display.\n
        /// Item3: Returns Y-direction aspect ratio for display.
        /// </returns>
        (float, float, float) _GetAspectRatio(int screenWidth, int screenHeight)
        {
            bool isLandscape = screenWidth > screenHeight;

            float aspectRatioValue = isLandscape
                ? _GetRoundedAspectRatioValue(screenWidth, screenHeight)
                : _GetRoundedAspectRatioValue(screenHeight, screenWidth);

            (float, float) sameRatio = default;

            (float, float) closestRatio
                = DisplayInfo.CommonAspectRatios
                             .OrderBy(
                                 common =>
                                 {
                                     float diff = Math.Abs(
                                         _GetRoundedAspectRatioValue(common.Item1, common.Item2) - aspectRatioValue);

                                     if (diff == 0)
                                     {
                                         sameRatio = common;
                                     }

                                     return diff;
                                 })
                             .First();

            if (sameRatio != default)
            {
                if (isLandscape)
                {
                    return (aspectRatioValue, sameRatio.Item1, sameRatio.Item2);
                }
                else
                {
                    return (aspectRatioValue, sameRatio.Item2, sameRatio.Item1);
                }
            }
            else
            {
                float greater = _GetRoundedValue(
                    closestRatio.Item2 * aspectRatioValue, DisplayInfo.AspectRatioXYDigits);

                if (isLandscape)
                {
                    return (aspectRatioValue, greater, closestRatio.Item2);
                }
                else
                {
                    return (aspectRatioValue, closestRatio.Item2, greater);
                }
            }
        }

        /// <summary>
        /// Gets rounded aspect ratio value
        /// </summary>
        /// <param name="top">Dividend</param>
        /// <param name="bottom">Divisor</param>
        /// <returns></returns>
        float _GetRoundedAspectRatioValue(float top, float bottom)
        {
            return _GetRoundedValue(top / bottom, DisplayInfo.AspectRatioValueDigits);
        }

        /// <summary>
        /// Gets the value rounded to the specified number of decimal places
        /// </summary>
        /// <param name="value">Target value</param>
        /// <param name="digits">Number decimal places</param>
        /// <returns></returns>
        float _GetRoundedValue(float value, int digits)
        {
            return (float)Math.Round(value, digits);
        }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        /// <summary>
        /// Gets the terminal orientation
        /// </summary>
        /// <returns>Returns the terminal orientation string</returns>
        string _GetScreenOrientationString()
        {
            return Screen.orientation.ToString();
        }
#else
        /// <summary>
        /// Gets the terminal orientation
        /// </summary>
        /// <remarks>It's deprecated for non-mobile devices, so a non-support string will return for non-simulate modes</remarks>
        /// <returns>Returns the terminal orientation string</returns>
        string _GetScreenOrientationString()
        {
            if (Application.isEditor && UnityEngine.Device.Application.isMobilePlatform)
            {
                return UnityEngine.Device.Screen.orientation.ToString();
            }

            return SystemInfo.unsupportedIdentifier;
        }
#endif
    }
}
