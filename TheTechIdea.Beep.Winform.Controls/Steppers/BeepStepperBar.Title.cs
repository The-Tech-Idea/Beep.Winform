using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// The stepper's own title — the heading for the whole control, not the per-step captions.
    /// </summary>
    /// <remarks>
    /// The control had no way to show a heading at all. <see cref="Control.Text"/> existed but was
    /// never painted, so setting it did nothing, and callers had to park a separate
    /// <c>BeepLabel</c> above the bar and keep it aligned by hand — which is what the wizard forms
    /// were doing with their own step-count label.
    /// <para>
    /// <c>Text</c> is the title and <see cref="SubText"/> is the optional line under it. The band
    /// is measured, and the step area shrinks by exactly its height, so the steps can never be
    /// drawn over the heading.
    /// </para>
    /// </remarks>
    public partial class BeepStepperBar
    {
        private string _subText = string.Empty;
        private string _titleImagePath = string.Empty;
        private ContentAlignment _titleAlignment = ContentAlignment.MiddleLeft;

        /// <summary>The second line under the control's title.</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Optional line shown under the stepper's title.")]
        public string SubText
        {
            get => _subText;
            set
            {
                if (_subText == value) return;
                _subText = value ?? string.Empty;
                InitializeSteps();   // the band's height changed, so the step area did too
                Invalidate();
            }
        }

        /// <summary>
        /// An icon shown to the left of the title, sized to the band.
        /// </summary>
        /// <remarks>
        /// Painted through <c>StyledImagePainter</c>, which is what renders and themes SVGs in
        /// this library - not a hand-rolled DrawImage. Its box is square and matches the title
        /// band's height, so the icon and the heading share a baseline at any DPI or font size.
        /// </remarks>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Icon shown beside the stepper's title. SVG or raster path.")]
        public string TitleImagePath
        {
            get => _titleImagePath;
            set
            {
                if (_titleImagePath == value) return;
                _titleImagePath = value ?? string.Empty;
                InitializeSteps();
                Invalidate();
            }
        }

        /// <summary>Where the title sits within the title band.</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [Description("Alignment of the stepper's title within its band.")]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set
            {
                if (_titleAlignment == value) return;
                _titleAlignment = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Height reserved for the title band, or 0 when there is no title.
        /// </summary>
        internal int GetTitleBandHeight()
        {
            if (string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(_subText)
                && string.IsNullOrWhiteSpace(_titleImagePath)) return 0;

            Font titleFont = TitleFont;
            int h = 0;
            if (!string.IsNullOrWhiteSpace(Text)) h += TextRenderer.MeasureText("Ag", titleFont).Height;
            if (!string.IsNullOrWhiteSpace(_subText)) h += TextRenderer.MeasureText("Ag", SubTextFont).Height;
            return h + DpiScalingHelper.ScaleValue(10, this);
        }

        /// <summary>
        /// The font the title is drawn in: the theme's own title typography, falling back to a
        /// bold form of the step font.
        /// </summary>
        /// <remarks>
        /// Fetched through <see cref="BeepThemesManager"/>, so it is cache-owned - never disposed
        /// here (CLAUDE.md hard rule 2).
        /// </remarks>
        private Font TitleFont
        {
            get
            {
                var themed = ThemeManagement.BeepThemesManager.ToFont(_currentTheme?.TitleStyle);
                if (themed != null) return themed;
                var basis = _textFont ?? Font ?? SystemFonts.DefaultFont;
                return FontManagement.BeepFontManager.GetFont(basis.FontFamily.Name, basis.Size + 1f, FontStyle.Bold)
                       ?? basis;
            }
        }

        /// <summary>
        /// Draws the title band at the top of <paramref name="bounds"/>.
        /// </summary>
        internal void DrawTitleBand(Graphics g, Rectangle bounds)
        {
            int band = GetTitleBandHeight();
            if (band <= 0 || g == null) return;

            var rect = new Rectangle(bounds.Left, bounds.Top, bounds.Width, band);
            var flags = TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine
                      | AlignmentFlags(_titleAlignment);

            Font titleFont = TitleFont;
            Color ink = StepperThemeHelpers.GetStepLabelColor(_currentTheme, StepState.Active);

            int y = rect.Top + DpiScalingHelper.ScaleValue(4, this);
            int textLeft = rect.Left;

            if (!string.IsNullOrWhiteSpace(_titleImagePath))
            {
                // Square, band-height, vertically centred - so icon and heading share a baseline.
                int side = Math.Max(1, band - DpiScalingHelper.ScaleValue(8, this));
                var iconRect = new Rectangle(rect.Left, rect.Top + ((band - side) / 2), side, side);
                Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, _titleImagePath, ControlStyle);
                textLeft = iconRect.Right + DpiScalingHelper.ScaleValue(8, this);
            }

            if (!string.IsNullOrWhiteSpace(Text))
            {
                int h = TextRenderer.MeasureText(g, "Ag", titleFont).Height;
                TextRenderer.DrawText(g, Text, titleFont,
                    new Rectangle(textLeft, y, rect.Right - textLeft, h), ink, flags);
                y += h;
            }

            if (!string.IsNullOrWhiteSpace(_subText))
            {
                // Its OWN font, not the title's. `_textFont ?? titleFont` fell through to the
                // title font whenever _textFont was unset, so the sub line rendered in the same
                // bold heading face and read as a second title.
                Font sub = SubTextFont;
                int h = TextRenderer.MeasureText(g, "Ag", sub).Height;
                TextRenderer.DrawText(g, _subText, sub, new Rectangle(textLeft, y, rect.Right - textLeft, h),
                    Color.FromArgb(170, ink), flags);
            }
        }

        /// <summary>The sub line's font: the step font, or the title font one size down.</summary>
        private Font SubTextFont
        {
            get
            {
                if (_textFont != null) return _textFont;
                var t = TitleFont;
                return FontManagement.BeepFontManager.GetFont(t.FontFamily.Name, Math.Max(6f, t.Size - 1.5f), FontStyle.Regular) ?? t;
            }
        }

        private static TextFormatFlags AlignmentFlags(ContentAlignment a) => a switch
        {
            ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter
                => TextFormatFlags.HorizontalCenter,
            ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight
                => TextFormatFlags.Right,
            _ => TextFormatFlags.Left
        };
    }
}
