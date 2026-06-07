using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class BeepLayoutMetrics
    {
        // ── Dialog Sizes ──────────────────────────────────────────────────
        public static readonly Size DialogSmall  = new(420, 320);
        public static readonly Size DialogMedium = new(600, 460);
        public static readonly Size DialogLarge  = new(840, 560);

        // ── Padding ────────────────────────────────────────────────────────
        public static readonly Padding DialogPadding    = new(12);
        public static readonly Padding ContainerPadding = new(8);
        public static readonly Padding ButtonStripPd    = new(10);
        public static readonly Padding HeaderPadding    = new(16, 8, 16, 8);

        public const int ButtonGap  = 8;
        public const int SmallGap   = 4;

        // ── Layout Grid ────────────────────────────────────────────────────
        public const int LabelColumnWidth = 130;
        public const int TextRowHeight    = 35;
        public const int InterRowSpacing  = 5;
        public const int CornerRadius     = 4;

        // ── Accessibility ──────────────────────────────────────────────────
        public const int MinTouchTarget   = 44;
        public const double MinContrast   = 4.5;

        // ── Button Sizing ─────────────────────────────────────────────────
        public static readonly Size ButtonStandard = new(100, 32);
        public static readonly Size ButtonSmall    = new(80, 28);
        public static readonly Size ButtonLarge    = new(130, 36);
        public static readonly Size ButtonToolbar  = new(110, 32);

        // ── Font Defaults ──────────────────────────────────────────────────
        public const float TitleFontSize    = 16f;
        public const float SubtitleFontSize  = 12f;
        public const float BodyFontSize      = 9f;

        // ── DPI-aware helpers ──────────────────────────────────────────────
        public static Size ScaleSize(this Size size, Control control) => DpiScalingHelper.ScaleSize(size, control);
        public static int ScaleValue(this int value, Control control) => DpiScalingHelper.ScaleValue(value, control);
        public static Padding ScalePadding(this Padding padding, Control control)
        {
            return new Padding(
                DpiScalingHelper.ScaleValue(padding.Left, control),
                DpiScalingHelper.ScaleValue(padding.Top, control),
                DpiScalingHelper.ScaleValue(padding.Right, control),
                DpiScalingHelper.ScaleValue(padding.Bottom, control));
        }
    }
}
