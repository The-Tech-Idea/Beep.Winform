using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities
{
    internal static class ThemeContrastHelper
    {
        // Compute the relative luminance of a color (WCAG)
        public static double RelativeLuminance(Color c)
        {
            double srgb(byte v) => v / 255.0;
            double toLinear(double v) => v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);

            var r = toLinear(srgb(c.R));
            var g = toLinear(srgb(c.G));
            var b = toLinear(srgb(c.B));

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        public static double ContrastRatio(Color a, Color b)
        {
            var la = RelativeLuminance(a);
            var lb = RelativeLuminance(b);
            var lighter = Math.Max(la, lb);
            var darker = Math.Min(la, lb);
            return Math.Round((lighter + 0.05) / (darker + 0.05), 2);
        }

        // Blend color with black/white to increase contrast until ratio target or max iterations reached
        public static Color AdjustForegroundToContrast(Color fg, Color bg, double targetRatio = 4.5, int maxIter = 24)
        {
            var cur = fg;
            var ratio = ContrastRatio(cur, bg);
            if (ratio >= targetRatio) return fg;

            // Determine whether to darken (blend with black) or lighten (blend with white)
            var fgL = RelativeLuminance(fg);
            var bgL = RelativeLuminance(bg);
            bool needDarken = fgL > bgL;

            for (int i = 0; i < maxIter && ratio < targetRatio; i++)
            {
                cur = needDarken ? Blend(cur, Color.Black, 0.06) : Blend(cur, Color.White, 0.06);
                ratio = ContrastRatio(cur, bg);
                // if we've reached a limit near white/black, stop early
                if (cur == Color.Black || cur == Color.White) break;
            }

            return cur;
        }

        private static Color Blend(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0, 1);
            int interp(int x, int y) => (int)Math.Round(x + (y - x) * t);
            return Color.FromArgb(255, interp(a.R, b.R), interp(a.G, b.G), interp(a.B, b.B));
        }

        // Validate an IBeepTheme instance's ForeColor tokens versus corresponding BackColor tokens
        public static void ValidateTheme(object themeObj, double targetRatio = 4.5, bool autofix = false)
        {
            if (themeObj == null) return;
            var type = themeObj.GetType();

            // Collect color properties
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(Color))
                .ToList();

            // Helper to find a background property for a foreground property (e.g., CalendarFooterColor -> CalendarBackColor)
            Color GetBackgroundColor(string forePropName)
            {
                var baseName = forePropName.Replace("ForeColor", "");
                var backName = baseName + "BackColor";
                var backProp = props.FirstOrDefault(p => p.Name.Equals(backName, StringComparison.OrdinalIgnoreCase));
                if (backProp != null)
                    return (Color)backProp.GetValue(themeObj)!;

                // last resort: BackgroundColor or SurfaceColor
                var fallbackProp = props.FirstOrDefault(p => p.Name.Equals("BackgroundColor", StringComparison.OrdinalIgnoreCase))
                                   ?? props.FirstOrDefault(p => p.Name.Equals("SurfaceColor", StringComparison.OrdinalIgnoreCase));
                return fallbackProp != null ? (Color)fallbackProp.GetValue(themeObj)! : Color.White;
            }

            foreach (var foreProp in props.Where(p => p.Name.EndsWith("ForeColor", StringComparison.OrdinalIgnoreCase)))
            {
                var bg = GetBackgroundColor(foreProp.Name);
                var fg = (Color)foreProp.GetValue(themeObj)!;
                // If transparent/empty, skip
                if (fg.A == 0) continue;
                var ratio = ContrastRatio(fg, bg);
                if (ratio < targetRatio && autofix)
                {
                    var fixedColor = AdjustForegroundToContrast(fg, bg, targetRatio);
                    foreProp.SetValue(themeObj, fixedColor);
                }
            }

            // Also inspect specific token classes (Svg/lines or placeholder colors)
            var specialTokens = new[] { "TextBoxPlaceholderColor", "CalendarFooterColor", "ChartAxisColor", "TabHoverForeColor", "InactiveTabForeColor", "SwitchForeColor", "ScrollBarActiveThumbColor" };
            foreach (var name in specialTokens)
            {
                var prop = props.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (prop == null) continue;
                var fg = (Color)prop.GetValue(themeObj)!;
                if (fg.A == 0) continue;
                // Find the nearest BackColor candidate
                var bg = GetBackgroundColor(name.Replace("PlaceholderColor", "TextBoxBackColor").Replace("FooterColor", "BackColor"));
                var ratio = ContrastRatio(fg, bg);
                if (ratio < targetRatio && autofix)
                {
                    var fixedColor = AdjustForegroundToContrast(fg, bg, targetRatio);
                    prop.SetValue(themeObj, fixedColor);
                }
            }

            // Typography styles are often used directly by controls for text rendering.
            // Normalize TextColor against the corresponding background token as a final pass.
            var typographyProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType.Name == "TypographyStyle")
                .ToList();

            Color GetTypographyBackground(string typographyPropName)
            {
                string name = typographyPropName.Replace("Font", "", StringComparison.OrdinalIgnoreCase);
                var candidateBackNames = new[]
                {
                    name + "BackColor",
                    name + "BackgroundColor"
                };

                // Common shorthand mappings used across themes.
                if (typographyPropName.Contains("Button", StringComparison.OrdinalIgnoreCase))
                {
                    candidateBackNames = candidateBackNames
                        .Concat(new[]
                        {
                            "ButtonBackColor",
                            "ButtonHoverBackColor",
                            "ButtonSelectedBackColor"
                        })
                        .ToArray();
                }
                else if (typographyPropName.Contains("ComboBox", StringComparison.OrdinalIgnoreCase))
                {
                    candidateBackNames = candidateBackNames
                        .Concat(new[]
                        {
                            "ComboBoxBackColor",
                            "ComboBoxHoverBackColor",
                            "ComboBoxSelectedBackColor"
                        })
                        .ToArray();
                }
                else if (typographyPropName.Contains("Label", StringComparison.OrdinalIgnoreCase))
                {
                    candidateBackNames = candidateBackNames
                        .Concat(new[]
                        {
                            "LabelBackColor",
                            "BackgroundColor",
                            "SurfaceColor"
                        })
                        .ToArray();
                }

                foreach (var backName in candidateBackNames)
                {
                    var backProp = props.FirstOrDefault(p => p.Name.Equals(backName, StringComparison.OrdinalIgnoreCase));
                    if (backProp != null)
                    {
                        return (Color)backProp.GetValue(themeObj)!;
                    }
                }

                var fallbackProp = props.FirstOrDefault(p => p.Name.Equals("BackgroundColor", StringComparison.OrdinalIgnoreCase))
                                   ?? props.FirstOrDefault(p => p.Name.Equals("SurfaceColor", StringComparison.OrdinalIgnoreCase));
                return fallbackProp != null ? (Color)fallbackProp.GetValue(themeObj)! : Color.White;
            }

            foreach (var typographyProp in typographyProps)
            {
                var styleObj = typographyProp.GetValue(themeObj);
                if (styleObj == null) continue;

                var textColorProp = typographyProp.PropertyType.GetProperty("TextColor", BindingFlags.Public | BindingFlags.Instance);
                if (textColorProp == null || !textColorProp.CanRead || !textColorProp.CanWrite || textColorProp.PropertyType != typeof(Color))
                    continue;

                var fg = (Color)textColorProp.GetValue(styleObj)!;
                if (fg.A == 0) continue;

                var bg = GetTypographyBackground(typographyProp.Name);
                var ratio = ContrastRatio(fg, bg);
                if (ratio < targetRatio && autofix)
                {
                    var fixedColor = AdjustForegroundToContrast(fg, bg, targetRatio);
                    textColorProp.SetValue(styleObj, fixedColor);
                }
            }
        }
    }
}
