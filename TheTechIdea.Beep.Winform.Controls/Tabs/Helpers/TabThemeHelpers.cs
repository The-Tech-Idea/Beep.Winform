using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// The single colour-resolution seam for tab rendering: every painter and the control itself
    /// resolve colours here, so this is also where Windows High Contrast is honoured.
    /// </summary>
    /// <remarks>
    /// High contrast is a colour concern, not a rendering concern. It used to be a second, parallel
    /// paint implementation (<c>BeepTabHeaderHost.PaintHighContrast</c>) that re-derived the tab
    /// background, border, title, dirty marker and close glyph itself - and which nothing ever
    /// called, so high contrast simply did not work at all. Resolving system colours here instead
    /// means the one painter pipeline is correct in both modes, and icons, badges, subtext and
    /// header actions keep working in high contrast rather than being dropped by a reduced
    /// second path.
    /// </remarks>
    public static class TabThemeHelpers
    {
        /// <summary>
        /// <see langword="true"/> when Windows is in a high-contrast accessibility theme, in which
        /// case system colours override theme colours <i>and</i> any explicit custom colour.
        /// Custom colours are deliberately overridden: honouring them is what breaks high contrast.
        /// </summary>
        public static bool IsHighContrast => SystemInformation.HighContrast;

        /// <summary>
        /// Gets the background color for the tab control
        /// Priority: Custom color > Theme Background > Default
        /// </summary>
        public static Color GetTabControlBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (IsHighContrast) return SystemColors.Window;

            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TabBackColor != Color.Empty)
                    return theme.TabBackColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
            }

            return Color.White;
        }

        /// <summary>
        /// Gets the header background color
        /// </summary>
        public static Color GetHeaderBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (IsHighContrast) return SystemColors.Control;

            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.TabBackColor != Color.Empty)
                    return theme.TabBackColor;
                if (theme.SurfaceColor != Color.Empty)
                    return theme.SurfaceColor;
                if (theme.BackgroundColor != Color.Empty)
                    return theme.BackgroundColor;
            }

            return Color.FromArgb(245, 245, 250);
        }

        /// <summary>
        /// Gets the tab background color
        /// </summary>
        public static Color GetTabBackgroundColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false,
            Color? customColor = null)
        {
            // Mapping preserved from the deleted BeepTabHeaderHost.GetHighContrastTabBackground.
            if (IsHighContrast)
            {
                if (isSelected) return SystemColors.Highlight;
                if (isHovered) return SystemColors.HotTrack;
                return SystemColors.ButtonFace;
            }

            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    // The selected tab must be visibly distinct from the strip it sits on. Some
                    // themes (MaterialDesign among them) define TabSelectedBackColor as the same
                    // near-white as the header, which rendered the selected tab as no tab at all —
                    // every unfilled style looked identical on the contact sheet.
                    Color selected = theme.TabSelectedBackColor != Color.Empty
                        ? theme.TabSelectedBackColor
                        : (theme.PrimaryColor != Color.Empty
                            ? ShiftLuminance(theme.PrimaryColor, -0.08f)
                            : Color.Empty);

                    if (selected != Color.Empty)
                    {
                        Color strip = GetHeaderBackgroundColor(theme, true);
                        if (IsPerceptiblyDifferent(selected, strip))
                            return selected;

                        if (theme.PrimaryColor != Color.Empty)
                            return theme.PrimaryColor;
                        return ShiftLuminance(strip, strip.GetBrightness() > 0.5f ? -0.18f : 0.18f);
                    }
                }
                else if (isHovered)
                {
                    if (theme.TabBackColor != Color.Empty)
                        return ShiftLuminance(theme.TabBackColor, 0.05f);
                    if (theme.SurfaceColor != Color.Empty)
                        return ShiftLuminance(theme.SurfaceColor, 0.05f);
                }
                else
                {
                    if (theme.TabBackColor != Color.Empty)
                        return theme.TabBackColor;
                    if (theme.SurfaceColor != Color.Empty)
                        return theme.SurfaceColor;
                }
            }

            if (isSelected)
                return Color.White;
            if (isHovered)
                return Color.FromArgb(250, 250, 250);
            return Color.FromArgb(240, 240, 245);
        }

        /// <summary>
        /// Gets the tab text color
        /// </summary>
        public static Color GetTabTextColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false)
        {
            // Mapping preserved from the deleted BeepTabHeaderHost.GetHighContrastTabForeground.
            if (IsHighContrast)
            {
                if (isSelected || isHovered) return SystemColors.HighlightText;
                return SystemColors.WindowText;
            }

            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.TabSelectedForeColor != Color.Empty)
                        return theme.TabSelectedForeColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
                else
                {
                    if (theme.TabForeColor != Color.Empty)
                        return theme.TabForeColor;
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return isSelected
                ? Color.FromArgb(33, 37, 41) // Dark gray
                : Color.FromArgb(97, 97, 97); // Medium gray
        }

        /// <summary>
        /// Gets the border color for tabs
        /// </summary>
        public static Color GetTabBorderColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false)
        {
            // Mapping preserved from the deleted BeepTabHeaderHost.GetHighContrastBorderColor.
            if (IsHighContrast)
            {
                return isSelected ? SystemColors.Highlight : SystemColors.WindowFrame;
            }

            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.AccentColor != Color.Empty)
                        return theme.AccentColor;
                }
                else if (isHovered)
                {
                    if (theme.SecondaryColor != Color.Empty)
                        return theme.SecondaryColor;
                }
                else
                {
                    if (theme.BorderColor != Color.Empty)
                        return theme.BorderColor;
                }
            }

            if (isSelected)
                return Color.FromArgb(33, 150, 243); // Material Blue
            if (isHovered)
                return Color.FromArgb(158, 158, 158); // Material Gray
            return Color.FromArgb(224, 224, 224); // Light Gray
        }

        /// <summary>
        /// Gets the underline/indicator color for selected tabs
        /// </summary>
        public static Color GetTabIndicatorColor(
            IBeepTheme theme,
            bool useThemeColors)
        {
            if (IsHighContrast) return SystemColors.Highlight;

            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryColor != Color.Empty)
                    return theme.PrimaryColor;
                if (theme.AccentColor != Color.Empty)
                    return theme.AccentColor;
            }

            return Color.FromArgb(33, 150, 243); // Material Blue
        }

        /// <summary>
        /// Gets the fill colour for a status badge. In high contrast every kind collapses to
        /// <see cref="SystemColors.Highlight"/>: high-contrast themes deliberately offer no
        /// semantic palette, and inventing one defeats the point of the mode.
        /// </summary>
        /// <remarks>
        /// The badge kinds used to read <c>Theme.ErrorColor</c> / <c>WarningColor</c> /
        /// <c>SuccessColor</c> directly from the painter, bypassing this seam — so badges were the
        /// one adornment that stayed themed in high contrast, on a background that had switched to
        /// system colours.
        /// </remarks>
        public static Color GetBadgeColor(IBeepTheme theme, bool useThemeColors, BeepTabBadgeKind kind)
        {
            if (IsHighContrast) return SystemColors.Highlight;

            if (useThemeColors && theme != null)
            {
                Color candidate = kind switch
                {
                    BeepTabBadgeKind.Error => theme.ErrorColor,
                    BeepTabBadgeKind.Warning => theme.WarningColor,
                    BeepTabBadgeKind.Success => theme.SuccessColor,
                    _ => theme.PrimaryColor
                };

                if (candidate != Color.Empty) return candidate;
                if (theme.PrimaryColor != Color.Empty) return theme.PrimaryColor;
            }

            return SystemColors.Highlight;
        }

        /// <summary>
        /// Gets the colour of the busy (loading) indicator. Previously hardcoded to
        /// <see cref="SystemColors.ControlDark"/>, so it ignored the theme entirely and was the only
        /// adornment that did not change with it.
        /// </summary>
        public static Color GetBusyIndicatorColor(IBeepTheme theme, bool useThemeColors)
        {
            if (IsHighContrast) return SystemColors.ControlText;

            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryColor != Color.Empty) return theme.PrimaryColor;
                if (theme.ForeColor != Color.Empty) return theme.ForeColor;
            }

            return SystemColors.ControlDark;
        }

        /// <summary>
        /// Gets the unsaved-changes dot colour. In high contrast this is
        /// <see cref="SystemColors.ControlText"/> so the dot stays visible on any background -
        /// the mapping preserved from the deleted <c>GetHighContrastDirtyMarkerColor</c>.
        /// </summary>
        public static Color GetDirtyMarkerColor(IBeepTheme theme, bool useThemeColors)
        {
            if (IsHighContrast) return SystemColors.ControlText;

            if (useThemeColors && theme != null && theme.PrimaryColor != Color.Empty)
                return theme.PrimaryColor;

            return SystemColors.Highlight;
        }

        /// <summary>
        /// Gets all theme colors for a tab in one call
        /// </summary>
        public static (Color tabBg, Color border, Color text, Color indicator) GetTabColors(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false)
        {
            return (
                GetTabBackgroundColor(theme, useThemeColors, isSelected, isHovered),
                GetTabBorderColor(theme, useThemeColors, isSelected, isHovered),
                GetTabTextColor(theme, useThemeColors, isSelected, isHovered),
                GetTabIndicatorColor(theme, useThemeColors)
            );
        }

        /// <summary>
        /// Whether two fills read as different areas rather than one continuous surface. Deliberately
        /// a low bar: this is "can you see the tab at all", not a WCAG text-contrast question.
        /// </summary>
        private static bool IsPerceptiblyDifferent(Color a, Color b)
        {
            return Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B) > 24;
        }

        private static Color ShiftLuminance(Color color, float shift)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            r = Math.Clamp(r + shift, 0f, 1f);
            g = Math.Clamp(g + shift, 0f, 1f);
            b = Math.Clamp(b + shift, 0f, 1f);

            return Color.FromArgb(
                color.A,
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255));
        }
    }
}
