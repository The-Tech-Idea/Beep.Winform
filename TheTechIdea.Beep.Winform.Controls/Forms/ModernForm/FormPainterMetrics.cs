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

        public static FormPainterMetrics DefaultFor(FormStyle style)
        {
            var m = new FormPainterMetrics();
            switch (style)
            {
                case FormStyle.Minimal:
                    m.CaptionHeight = 28;
                    m.ButtonWidth = 28;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    break;
                case FormStyle.Material:
                    m.CaptionHeight = 36;
                    m.ButtonWidth = 32;
                    m.IconSize = 24;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    break;
                case FormStyle.Fluent:
                    m.CaptionHeight = 40;
                    m.ButtonWidth = 36;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    break;
                case FormStyle.MacOS:
                    m.CaptionHeight = 40;
                    m.ButtonWidth = 28;
                    m.IconSize = 20;
                    m.ButtonsPlacement = SystemButtonsSide.Left;
                    break;
                default:
                    // Modern / Classic fallbacks
                    m.CaptionHeight = 32;
                    m.ButtonWidth = 32;
                    m.IconSize = 24;
                    m.ButtonsPlacement = SystemButtonsSide.Right;
                    break;
            }
            return m;
        }
    }
}
