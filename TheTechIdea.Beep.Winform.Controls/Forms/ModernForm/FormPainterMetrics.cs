using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// Metrics used by BeepiFormProLayoutManager to size and place caption elements.
    /// Painters can provide custom metrics via IFormPainterMetricsProvider.
    /// </summary>
    public sealed class FormPainterMetrics
    {
        public enum SystemButtonsSide
        {
            Right,
            Left
        }
        // Colors for various form elements in caption and borders
        public Color BorderColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
        public Color CaptionColor { get; set; } = Color.FromArgb(255, 240, 240, 240);
        public Color CaptionTextColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionTextColorInactive { get; set; } = Color.FromArgb(255, 100, 100, 100);
        public Color CaptionTextColorMaximized { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonHoverColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonPressedColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        public Color CaptionButtonInactiveColor { get; set; } = Color.FromArgb(255, 100, 100, 100);
        public Color CaptionButtonMaximizedColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
        //--------------------------------

        public IBeepTheme beepTheme { get; set; }
        public int BorderWidth { get; set; } = 1;
        public int ResizeBorderWidth { get; set; } = 6;
        public int OuterMargin { get; set; } = 8;
        public int OuterMarginWhenMaximized { get; set; } = 0;
        public int InnerMargin { get; set; } = 0;
        public int InnerMarginWhenMaximized { get; set; } = 0;
        public int CaptionMargin { get; set; } = 0;
        public int BorderRadius { get; set; } = 16;
        public int BorderRadiusWhenMaximized { get; set; } = 0;
        public int CaptionButtonRadius { get; set; } = 6;

        public int CaptionHeight { get; set; } = 32;
        public float FontHeightMultiplier { get; set; } = 2.5f;
        public int ButtonWidth { get; set; } = 32;
        public int ButtonSpacing { get; set; } = 4;
        public int TitleLeftPadding { get; set; } = 8;
        public int IconLeftPadding { get; set; } = 8;
        public int IconSize { get; set; } = 24;
        public SystemButtonsSide ButtonsPlacement { get; set; } = SystemButtonsSide.Right;

        // Extra buttons
        public bool ShowThemeButton { get; set; }
        public bool ShowStyleButton { get; set; }
        public bool ShowSearchButton { get; set; }
        public bool ShowProfileButton { get; set; }
        public bool ShowMailButton { get; set; }

        public static FormPainterMetrics DefaultFor(FormStyle style, IBeepTheme theme)
        {
            var m = new FormPainterMetrics();
            switch (style)
            {
                case FormStyle.Minimal:
                    m.CaptionHeight = 28;
                    m.ButtonWidth = 28;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 4;
                    break;
                case FormStyle.Material:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 32;
                    m.IconSize = 24;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 2;
                    m.BorderRadius = 8;
                    break;
                case FormStyle.Fluent:
                    m.CaptionHeight = 40;
                    m.ButtonWidth = 36;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 6;
                    break;
                case FormStyle.MacOS:
                    m.CaptionHeight = 40;
                    m.ButtonWidth = 28;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Left;
                    m.BorderWidth = 1;
                    m.BorderRadius = 12;
                    break;
                default:
                    // Modern / Classic fallbacks
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 24;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    m.BorderWidth = 1;
                    m.BorderRadius = 8;
                    break;
            }

            // Fill colors from the provided theme
            m.BorderColor = theme.BorderColor;
            m.CaptionColor = theme.BackColor;
            m.CaptionTextColor = theme.ForeColor;
            m.CaptionTextColorInactive = theme.InactiveBorderColor;
            m.CaptionTextColorMaximized = theme.ActiveBorderColor;
            m.CaptionButtonColor = theme.ButtonBorderColor;
            m.CaptionButtonHoverColor = theme.ButtonHoverBorderColor;
            m.CaptionButtonPressedColor = theme.ButtonPressedBorderColor;
            m.CaptionButtonInactiveColor = theme.DisabledBorderColor;
            m.CaptionButtonMaximizedColor = theme.ActiveBorderColor;

            return m;
        }

       
    }
}
