using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>Typography role — sets the default font, size, and weight from the theme.</summary>
    public enum LabelVariant
    {
        /// <summary>Standard body text (BodyMedium).</summary>
        Body,
        /// <summary>Bold title / heading (TitleMedium).</summary>
        Title,
        /// <summary>Small subtitle (BodySmall).</summary>
        Subtitle,
        /// <summary>Smallest caption / overline (Caption).</summary>
        Caption,
        /// <summary>Pill / badge with rounded background.</summary>
        Badge,
        /// <summary>Chip / tag with rounded background.</summary>
        Chip,
        /// <summary>Inline code / monospace snippet.</summary>
        Code,
        /// <summary>Blockquote with left accent bar.</summary>
        Blockquote,
        /// <summary>Stat / metric (large number + small label row).</summary>
        Metric
    }

    /// <summary>Visual decoration applied over the label's background.</summary>
    public enum LabelEffect
    {
        /// <summary>Standard flat label — no effect.</summary>
        None,
        /// <summary>Subtle inner glow using the accent color.</summary>
        Glow,
        /// <summary>Raised surface — light top-left highlight + dark bottom-right shadow.</summary>
        Raised,
        /// <summary>Pressed / sunken surface — dark inner shadow.</summary>
        Sunken,
        /// <summary>Animated shimmer / skeleton placeholder.</summary>
        Shimmer,
        /// <summary>Gradient fill from left to right using accent + background.</summary>
        GradientFill,
        /// <summary>Left-edge accent bar (3 px).</summary>
        AccentBar
    }

    /// <summary>Shape of the label surface.</summary>
    public enum LabelShape
    {
        Rectangle,
        Rounded,
        Pill,
        Circle
    }

    /// <summary>Optical size override for <see cref="LabelVariant.Metric"/>.</summary>
    public enum LabelMetricSize
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public partial class BeepLabel
    {
        #region "Style properties"

        private LabelVariant _variant = LabelVariant.Body;
        private LabelEffect _effect = LabelEffect.None;
        private LabelShape _shape = LabelShape.Rectangle;
        private LabelMetricSize _metricSize = LabelMetricSize.Medium;

        [Browsable(true), Category("Appearance"), Description("Typography role that sets font defaults from the theme.")]
        [DefaultValue(LabelVariant.Body)]
        public LabelVariant Variant
        {
            get => _variant;
            set { _variant = value; ApplyVariantFont(); Invalidate(); }
        }

        [Browsable(true), Category("Appearance"), Description("Visual decoration effect applied over the label.")]
        [DefaultValue(LabelEffect.None)]
        public LabelEffect Effect
        {
            get => _effect;
            set { _effect = value; Invalidate(); }
        }

        [Browsable(true), Category("Appearance"), Description("Shape of the label's surface background.")]
        [DefaultValue(LabelShape.Rectangle)]
        public LabelShape Shape
        {
            get => _shape;
            set { _shape = value; Invalidate(); }
        }

        [Browsable(true), Category("Appearance"), Description("Optical size for the Metric variant.")]
        [DefaultValue(LabelMetricSize.Medium)]
        public LabelMetricSize MetricSize
        {
            get => _metricSize;
            set { _metricSize = value; ApplyVariantFont(); Invalidate(); }
        }

        /// <summary>When true, the label text colour auto-adjusts for contrast on its own background.</summary>
        [Browsable(true), Category("Appearance"), DefaultValue(false)]
        public bool AutoContrastText { get; set; }

        /// <summary>Badge / chip background colour (only used when Shape is Rounded or Pill with Badge/Chip variant).</summary>
        [Browsable(true), Category("Appearance")]
        public Color FillColor { get; set; } = Color.Empty;

        /// <summary>Accent colour for effects like Glow, AccentBar, GradientFill.</summary>
        [Browsable(true), Category("Appearance")]
        public Color EffectColor { get; set; } = Color.Empty;

        [Browsable(true), Category("Animation"), DefaultValue(true)]
        public bool EnableAnimations { get; set; } = true;

        #endregion

        #region "Font resolution by variant"

        private void ApplyVariantFont()
        {
            var theme = BeepThemesManager.CurrentTheme;
            Font? resolved = null;

            switch (_variant)
            {
                case LabelVariant.Title:
                    resolved = theme?.TitleMedium != null ? BeepThemesManager.ToFont(theme.TitleMedium) : null;
                    break;
                case LabelVariant.Subtitle:
                    resolved = theme?.BodySmall != null ? BeepThemesManager.ToFont(theme.BodySmall) : null;
                    break;
                case LabelVariant.Caption:
                    resolved = theme?.CaptionStyle != null ? BeepThemesManager.ToFont(theme.CaptionStyle) : null;
                    break;
                case LabelVariant.Metric:
                    // Large numeric value — size driven by MetricSize, family from theme.
                    float fontSize = _metricSize switch
                    {
                        LabelMetricSize.Small      => 16f,
                        LabelMetricSize.Medium     => 24f,
                        LabelMetricSize.Large      => 36f,
                        LabelMetricSize.ExtraLarge => 48f,
                        _                          => 24f
                    };
                    string family = theme?.ListTitleFont?.FontFamily ?? Font.FontFamily.Name;
                    resolved = BeepThemesManager.ToFont(family, fontSize, FontWeight.Bold, FontStyle.Bold);
                    break;
                case LabelVariant.Code:
                    resolved = BeepThemesManager.ToFont("Consolas", 9f, FontWeight.Normal, FontStyle.Regular);
                    break;
                case LabelVariant.Badge:
                case LabelVariant.Chip:
                case LabelVariant.Body:
                default:
                    resolved = theme?.BodyMedium != null ? BeepThemesManager.ToFont(theme.BodyMedium) : null;
                    break;
            }

            if (resolved != null && resolved != _textFont)
            {
                _textFont = resolved;
                Font = resolved;
            }
        }

        #endregion

        #region "Effect rendering helpers (called from the painter)"

        /// <summary>Draws the label's shape background (Rectangle / Rounded / Pill / Circle) before text.</summary>
        internal void DrawStyleShape(Graphics g, Rectangle bounds)
        {
            if (_shape == LabelShape.Rectangle && _variant != LabelVariant.Badge && _variant != LabelVariant.Chip)
                return;

            Color fill = FillColor;
            if (fill == Color.Empty)
            {
                fill = _variant switch
                {
                    LabelVariant.Badge => Color.FromArgb(30, _currentTheme?.PrimaryColor ?? Color.FromArgb(33, 150, 243)),
                    LabelVariant.Chip  => _currentTheme?.SurfaceColor ?? Color.FromArgb(245, 245, 245),
                    LabelVariant.Code  => Color.FromArgb(30, 30, 30),
                    _                  => BackColor
                };
            }

            int radius = _shape switch
            {
                LabelShape.Rounded => DpiScalingHelper.ScaleValue(6, this),
                LabelShape.Pill    => bounds.Height / 2,
                LabelShape.Circle  => Math.Min(bounds.Width, bounds.Height) / 2,
                _                  => DpiScalingHelper.ScaleValue(3, this)
            };

            // Padding for badge/chip so text doesn't touch the edges.
            int pad = (_variant is LabelVariant.Badge or LabelVariant.Chip) ? DpiScalingHelper.ScaleValue(8, this) : 0;
            var fillRect = pad > 0
                ? new Rectangle(bounds.X + pad / 2, bounds.Y, bounds.Width - pad, bounds.Height)
                : bounds;

            using var path = RoundedRectPath(fillRect, radius);
            using var brush = new SolidBrush(fill);
            g.FillPath(brush, path);

            // Subtle border for chip
            if (_variant == LabelVariant.Chip)
            {
                using var pen = new Pen(Color.FromArgb(25, 0, 0, 0), 1f);
                g.DrawPath(pen, path);
            }
        }

        /// <summary>Draws the selected effect overlay on top of the shape background.</summary>
        internal void DrawStyleEffect(Graphics g, Rectangle bounds)
        {
            Color accent = (EffectColor != Color.Empty) ? EffectColor
                : Color.FromArgb(40, _currentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243));

            switch (_effect)
            {
                case LabelEffect.AccentBar:
                    int w = DpiScalingHelper.ScaleValue(3, this);
                    g.FillRectangle(new SolidBrush(accent), bounds.X, bounds.Y, w, bounds.Height);
                    break;

                case LabelEffect.Glow:
                    // Soft inner glow: semi-transparent concentric rectangle.
                    int inset = DpiScalingHelper.ScaleValue(2, this);
                    var inner = new Rectangle(bounds.X + inset, bounds.Y + inset,
                        bounds.Width - inset * 2, bounds.Height - inset * 2);
                    using (var glowPath = RoundedRectPath(inner, DpiScalingHelper.ScaleValue(4, this)))
                    using (var glowBrush = new PathGradientBrush(glowPath))
                    {
                        glowBrush.CenterColor = Color.FromArgb(35, accent);
                        glowBrush.SurroundColors = new[] { Color.Transparent };
                        g.FillPath(glowBrush, glowPath);
                    }
                    break;

                case LabelEffect.Raised:
                    // Light top-left highlight + dark bottom-right shadow.
                    int hw = DpiScalingHelper.ScaleValue(2, this);
                    using (var topPen = new Pen(Color.FromArgb(40, Color.White), hw))
                    {
                        topPen.Alignment = PenAlignment.Inset;
                        g.DrawLine(topPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                        g.DrawLine(topPen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                    }
                    using (var botPen = new Pen(Color.FromArgb(30, 0, 0, 0), hw))
                    {
                        botPen.Alignment = PenAlignment.Inset;
                        g.DrawLine(botPen, bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                        g.DrawLine(botPen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                    }
                    break;

                case LabelEffect.Sunken:
                    int sw = DpiScalingHelper.ScaleValue(2, this);
                    using (var inPen = new Pen(Color.FromArgb(40, 0, 0, 0), sw))
                    {
                        inPen.Alignment = PenAlignment.Inset;
                        g.DrawRectangle(inPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }
                    break;

                case LabelEffect.GradientFill:
                    using (var gradBrush = new LinearGradientBrush(bounds, accent, BackColor, LinearGradientMode.Horizontal))
                        g.FillRectangle(gradBrush, bounds);
                    break;

                case LabelEffect.Shimmer:
                    // Subtle animated shimmer stripe (caller should drive invalidation via a timer).
                    long ms = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                    float phase = (ms % 2000) / 2000f;
                    int stripeW = bounds.Width / 3;
                    int sx = (int)(phase * (bounds.Width + stripeW)) - stripeW;
                    var stripeRect = new Rectangle(sx, bounds.Top, stripeW, bounds.Height);
                    using (var shimBrush = new LinearGradientBrush(stripeRect, Color.Transparent,
                        Color.FromArgb(25, Color.White), LinearGradientMode.Horizontal))
                        g.FillRectangle(shimBrush, stripeRect);
                    break;
            }
        }

        /// <summary>Applies auto-contrast to the provided text color based on the label's fill.</summary>
        internal Color ApplyAutoContrast(Color textColor)
        {
            if (!AutoContrastText) return textColor;
            Color bg = FillColor != Color.Empty ? FillColor : BackColor;
            return bg.GetBrightness() > 0.55f ? Color.Black : Color.White;
        }

        /// <summary>
        /// Returns the derived text colour for the Blockquote accent bar and Code text,
        /// computed once per paint so it stays consistent with the current theme.
        /// </summary>
        internal Color GetVariantOverlayColor()
        {
            return _variant switch
            {
                LabelVariant.Code => _currentTheme?.PrimaryColor ?? Color.FromArgb(33, 150, 243),
                _                 => Color.Empty
            };
        }

        private static GraphicsPath RoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(rect); return path; }
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
