using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Sprint 10 — Glassmorphism painter.
    /// 
    /// Produces a semi-transparent frosted-glass style tooltip:
    ///  • Translucent background using per-pixel alpha (composited by the form's AllowTransparency).
    ///  • Soft coloured glow border.
    ///  • Subtle inner gradient highlight ("sheen").
    ///  • No opaque shadow — instead a blurred dark outline is simulated via layered ellipses.
    /// 
    /// NOTE: For true blur-behind on Windows 10/11 the host form should
    /// enable DWM ACRYLIC via <c>DwmBlurBehind</c>.  This painter provides a
    /// best-effort visual when that is unavailable.
    /// </summary>
    public class GlassToolTipPainter : ToolTipPainterBase
    {
        // ──────────────────────────────────────────────────────────────
        // Tuneable defaults
        // ──────────────────────────────────────────────────────────────
        private const int   CornerRadius    = 14;
        private const int   GlowLayers      = 4;
        private const int   GlowSpread      = 3;   // pixels per blur layer
        private const float GlassBgAlpha    = 0.28f;
        private const float SheenAlpha      = 0.18f;
        private const float BorderAlpha     = 0.60f;

        // ──────────────────────────────────────────────────────────────
        // ToolTipPainterBase — main entry point
        // ──────────────────────────────────────────────────────────────

        public override void Paint(Graphics g, Rectangle bounds, ToolTipConfig config,
                                   ToolTipPlacement placement, IBeepTheme theme)
        {
            _lastBounds = bounds;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            PaintShadow(g, bounds, config);
            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);
            if (config.ShowArrow)
                PaintArrow(g, Point.Empty, placement, config, theme);
            PaintContent(g, bounds, config, theme);
        }

        // ──────────────────────────────────────────────────────────────
        // Shadow — soft multi-layer glow instead of drop shadow
        // ──────────────────────────────────────────────────────────────

        public override void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config)
        {
            for (int i = GlowLayers; i >= 1; i--)
            {
                int   inflate = i * GlowSpread;
                var   r       = Rectangle.Inflate(bounds, inflate, inflate);
                float alpha   = (float)(GlowLayers - i + 1) / (GlowLayers * 2) * 0.55f;
                int   a       = (int)(alpha * 255);
                using var pen = new Pen(Color.FromArgb(a, 0, 0, 0));
                using var path = CreateRoundedRect(r, CornerRadius + inflate / 2);
                g.DrawPath(pen, path);
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Background — translucent fill + inner sheen
        // ──────────────────────────────────────────────────────────────

        public override void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config,
                                             IBeepTheme theme)
        {
            var colors = ToolTipStyleAdapter.GetColors(config, theme);

            // Base frosted fill — OPAQUE.
            //
            // This used to fill with alpha (GlassBgAlpha), which looks right on a normal surface
            // but not here: CustomToolTip sets TransparencyKey = Color.Magenta and paints its form
            // background magenta, so those pixels are punched out to transparent. Alpha-blending
            // over that base produces a magenta-tinted colour that is *not* exactly the key, so it
            // is not punched out — the tooltip rendered as a solid magenta box.
            //
            // Colour-key transparency and alpha blending cannot be combined. True per-pixel glass
            // would need a layered window (UpdateLayeredWindow), which is a much larger change; the
            // frosted *look* is achieved here by compositing the same colour against a light base
            // and filling opaquely. Layers drawn on top of this fill may use alpha freely, because
            // they now blend against an opaque surface rather than the key colour.
            using var path = CreateRoundedRect(bounds, CornerRadius);
            using var bg   = new SolidBrush(CompositeOverLight(colors.background, GlassBgAlpha));
            g.FillPath(bg, path);

            // Sheen — top-quarter highlight gradient
            var sheenRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 2);
            using var sheenPath = CreateRoundedRect(sheenRect, CornerRadius);
            using var sheen = new LinearGradientBrush(sheenRect,
                Color.FromArgb((int)(SheenAlpha * 255), Color.White),
                Color.Transparent,
                LinearGradientMode.Vertical);
            g.FillPath(sheen, sheenPath);
        }

        /// <summary>
        /// Composites <paramref name="colour"/> at <paramref name="alpha"/> over a light base and
        /// returns an opaque result — the frosted appearance without relying on window alpha, which
        /// this form's colour-key transparency cannot support.
        /// </summary>
        private static Color CompositeOverLight(Color colour, float alpha)
        {
            const int BaseTone = 246;   // near-white frost backing
            alpha = Math.Max(0f, Math.Min(1f, alpha));

            int Blend(int channel) => (int)Math.Round(channel * alpha + BaseTone * (1 - alpha));

            return Color.FromArgb(255, Blend(colour.R), Blend(colour.G), Blend(colour.B));
        }

        // ──────────────────────────────────────────────────────────────
        // Border — coloured glow
        // ──────────────────────────────────────────────────────────────

        public override void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config,
                                         IBeepTheme theme)
        {
            var colors = ToolTipStyleAdapter.GetColors(config, theme);
            int borderA = (int)(BorderAlpha * 255);

            using var path = CreateRoundedRect(bounds, CornerRadius);
            using var pen  = new Pen(Color.FromArgb(borderA,
                colors.border.R, colors.border.G, colors.border.B), 1.5f);
            g.DrawPath(pen, path);
        }

        // ──────────────────────────────────────────────────────────────
        // Arrow — delegates to shared arrow painter
        // ──────────────────────────────────────────────────────────────

        public override void PaintArrow(Graphics g, Point position, ToolTipPlacement placement,
                                        ToolTipConfig config, IBeepTheme theme)
        {
            // Re-use the last painted bounds (stored by caller via the bounds parameter in Paint).
            // For simplicity, we use an empty rect and let ToolTipArrowPainter handle placement.
            var colors = ToolTipStyleAdapter.GetColors(config, theme);
            int fillA  = (int)(GlassBgAlpha * 1.5 * 255);
            ToolTipArrowPainter.DrawArrow(g, _lastBounds, placement,
                config.ArrowStyle, config.ArrowSize, config.ArrowOffset,
                Color.FromArgb(Math.Min(fillA, 255), colors.background),
                Color.FromArgb((int)(BorderAlpha * 255), colors.border));
        }

        // ──────────────────────────────────────────────────────────────
        // Content — text with soft drop shadow for legibility
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Measures with the same fonts <see cref="PaintContent"/> draws with.
        /// <para>
        /// Without this override the inherited <c>CalculateSize</c> measures using the base
        /// painter's title/body fonts, while this painter renders with the theme's much larger
        /// <c>TitleStyle</c> and <c>BodyStyle</c>. The window was therefore sized for one font and
        /// filled with another, and the body text was clipped off the bottom — the same
        /// measure-with-one-font-draw-with-another defect that clipped every label in BeepTree.
        /// </para>
        /// </summary>
        public override Size CalculateSize(Graphics g, ToolTipConfig config)
        {
            if (config?.ContentItems is { Count: > 0 })
                return base.CalculateSize(g, config);

            var theme = BeepThemesManager.CurrentTheme;
            var titleFont = BeepThemesManager.ToFont(theme?.TitleStyle) ?? BeepFontManager.DefaultFont;
            var bodyFont = BeepThemesManager.ToFont(theme?.BodyStyle) ?? BeepFontManager.DefaultFont;

            const int pad = 10;
            int maxWidth = config.MaxSize?.Width > 0 ? config.MaxSize.Value.Width : 320;
            int maxInner = Math.Max(1, maxWidth - pad * 2);

            // Two passes, because height depends on the width the text will actually wrap at.
            //
            // Pass 1 picks the content width. Pass 2 measures heights *at that same width* — the
            // width PaintContent will use. Measuring heights against maxInner while painting at
            // the narrower final width lets a line wrap at paint time that did not wrap at measure
            // time, which grows the content past the window and clips the body text.
            int contentWidth = 0;
            if (!string.IsNullOrEmpty(config.Title))
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(g.MeasureString(config.Title, titleFont, maxInner).Width));
            if (!string.IsNullOrEmpty(config.Text))
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(g.MeasureString(config.Text, bodyFont, maxInner).Width));

            // A couple of pixels of slack: MeasureString and DrawString disagree by a fraction of a
            // pixel on trailing glyphs, which is enough to force an unwanted wrap at exactly the
            // measured width.
            contentWidth = Math.Min(maxInner, contentWidth + 2);

            int height = pad * 2;
            if (!string.IsNullOrEmpty(config.Title))
                height += (int)Math.Ceiling(g.MeasureString(config.Title, titleFont, contentWidth).Height) + 4;
            if (!string.IsNullOrEmpty(config.Text))
                height += (int)Math.Ceiling(g.MeasureString(config.Text, bodyFont, contentWidth).Height);

            return new Size(contentWidth + pad * 2, height);
        }

        public override void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config,
                                          IBeepTheme theme)
        {
            // B1: When ContentItems is populated, delegate to the styled
            // painter so the glass variant picks up icons, code, links,
            // dividers, footers, and rich markup instead of falling back
            // to plain Title+Text.
            if (config.ContentItems != null && config.ContentItems.Count > 0)
            {
                _contentDelegate.PaintContentItems(g, bounds, config, theme);
                return;
            }

            var colors = ToolTipStyleAdapter.GetColors(config, theme);

            int pad  = 10;
            var area = Rectangle.Inflate(bounds, -pad, -pad);
            if (area.IsEmpty) return;

            var titleFont = BeepThemesManager.ToFont(theme?.TitleStyle) ?? BeepFontManager.DefaultFont;
            var bodyFont = BeepThemesManager.ToFont(theme?.BodyStyle) ?? BeepFontManager.DefaultFont;

            if (!string.IsNullOrEmpty(config.Title))
            {
                // shadow
                using var shadow = new SolidBrush(Color.FromArgb(80, Color.Black));
                g.DrawString(config.Title, titleFont, shadow,
                    new Rectangle(area.X + 1, area.Y + 1, area.Width, area.Height));
                // text
                using var fg = new SolidBrush(colors.foreground);
                g.DrawString(config.Title, titleFont, fg, area);

                int th = (int)g.MeasureString(config.Title, titleFont, area.Width).Height;
                area   = new Rectangle(area.X, area.Y + th + 4, area.Width, area.Height - th - 4);
            }

            if (!string.IsNullOrEmpty(config.Text) && !area.IsEmpty)
            {
                using var shadow = new SolidBrush(Color.FromArgb(50, Color.Black));
                g.DrawString(config.Text, bodyFont, shadow,
                    new Rectangle(area.X + 1, area.Y + 1, area.Width, area.Height));
                using var fg = new SolidBrush(Color.FromArgb(230, colors.foreground));
                g.DrawString(config.Text, bodyFont, fg, area);
            }
        }

        // B1: cached delegate to avoid allocating a BeepStyledToolTipPainter
        // on every show. Painter is stateless w.r.t. its inputs so sharing
        // across shows is safe.
        private static readonly BeepStyledToolTipPainter _contentDelegate = new BeepStyledToolTipPainter();

        // ──────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────

        private Rectangle _lastBounds;

        private static GraphicsPath CreateRoundedRect(Rectangle r, int radius)
        {
            int d    = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
