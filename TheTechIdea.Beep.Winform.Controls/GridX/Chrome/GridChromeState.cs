using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Chrome
{
    public enum GridChromeVisualState
    {
        Normal,
        Hover,
        Focused,
        Pressed,
        Active,
        Disabled,
        Error
    }

    /// <summary>
    /// Shared state colours for chrome surfaces. Colors come from the current
    /// theme; literal values are fallbacks only, not a new palette.
    /// </summary>
    public sealed class GridChromeState
    {
        public Color Background { get; init; }
        public Color Surface { get; init; }
        public Color Foreground { get; init; }
        public Color MutedForeground { get; init; }
        public Color Border { get; init; }
        public Color HoverBackground { get; init; }
        public Color PressedBackground { get; init; }
        public Color ActiveBackground { get; init; }
        public Color FocusRing { get; init; }
        public Color DisabledForeground { get; init; }
        public Color Error { get; init; }

        public static GridChromeState Resolve(BeepGridPro grid)
        {
            // High contrast owns the palette; theme colours must not override it.
            if (SystemInformation.HighContrast)
            {
                return new GridChromeState
                {
                    Background = SystemColors.Window,
                    Surface = SystemColors.Control,
                    Foreground = SystemColors.WindowText,
                    MutedForeground = SystemColors.GrayText,
                    Border = SystemColors.WindowFrame,
                    HoverBackground = SystemColors.Highlight,
                    PressedBackground = SystemColors.Highlight,
                    ActiveBackground = SystemColors.Highlight,
                    FocusRing = SystemColors.Highlight,
                    DisabledForeground = SystemColors.GrayText,
                    Error = SystemColors.WindowText
                };
            }

            IBeepTheme? theme = grid?._currentTheme ?? BeepThemesManager.CurrentTheme;
            if (theme == null)
            {
                return new GridChromeState
                {
                    Background = Color.White,
                    Surface = Color.White,
                    Foreground = Color.FromArgb(31, 41, 55),
                    MutedForeground = Color.FromArgb(107, 114, 128),
                    Border = Color.FromArgb(209, 213, 219),
                    HoverBackground = Color.FromArgb(243, 244, 246),
                    PressedBackground = Color.FromArgb(229, 231, 235),
                    ActiveBackground = Color.FromArgb(219, 234, 254),
                    FocusRing = Color.FromArgb(37, 99, 235),
                    DisabledForeground = Color.FromArgb(156, 163, 175),
                    Error = Color.FromArgb(185, 28, 28)
                };
            }

            var background = Pick(theme.BackgroundColor, theme.BackColor, Color.White);
            return new GridChromeState
            {
                Background = background,
                Surface = Pick(theme.SurfaceColor, theme.PanelBackColor, Color.White),
                Foreground = ColorUtils.EnsureReadable(
                    Pick(theme.ForeColor, theme.OnBackgroundColor, Color.FromArgb(31, 41, 55)), background, 4.5),
                MutedForeground = ColorUtils.EnsureReadable(
                    Pick(theme.DisabledForeColor, theme.ForeColor, Color.FromArgb(107, 114, 128)), background, 3.0),
                Border = Pick(theme.BorderColor, theme.GridLineColor, Color.FromArgb(209, 213, 219)),
                HoverBackground = Pick(theme.ButtonHoverBackColor, Color.FromArgb(243, 244, 246)),
                PressedBackground = Pick(theme.ButtonPressedBackColor, theme.ButtonHoverBackColor, Color.FromArgb(229, 231, 235)),
                ActiveBackground = Pick(theme.SelectedRowBackColor, theme.AccentColor, Color.FromArgb(219, 234, 254)),
                FocusRing = Pick(theme.FocusIndicatorColor, theme.AccentColor, Color.FromArgb(37, 99, 235)),
                DisabledForeground = Pick(theme.DisabledForeColor, theme.ForeColor, Color.FromArgb(156, 163, 175)),
                Error = Pick(theme.ErrorColor, Color.FromArgb(185, 28, 28))
            };
        }

        public Color BackgroundFor(GridChromeVisualState state) => state switch
        {
            GridChromeVisualState.Hover => HoverBackground,
            GridChromeVisualState.Pressed => PressedBackground,
            GridChromeVisualState.Active => ActiveBackground,
            GridChromeVisualState.Disabled => Surface,
            _ => Background
        };

        public Color ForegroundFor(GridChromeVisualState state) => state switch
        {
            GridChromeVisualState.Disabled => DisabledForeground,
            GridChromeVisualState.Error => Error,
            _ => Foreground
        };

        public Color BorderFor(GridChromeVisualState state) => state switch
        {
            GridChromeVisualState.Focused => FocusRing,
            GridChromeVisualState.Error => Error,
            GridChromeVisualState.Disabled => Border,
            _ => Border
        };

        private static Color Pick(Color first, Color second, Color fallback)
        {
            if (first != Color.Empty) return first;
            if (second != Color.Empty) return second;
            return fallback;
        }

        private static Color Pick(Color first, Color fallback)
            => first != Color.Empty ? first : fallback;
    }
}
