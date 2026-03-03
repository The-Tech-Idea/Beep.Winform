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
            int bgA    = (int)(GlassBgAlpha * 255);

            // Base frosted fill
            using var path = CreateRoundedRect(bounds, CornerRadius);
            using var bg   = new SolidBrush(Color.FromArgb(bgA,
                colors.background.R, colors.background.G, colors.background.B));
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

        public override void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config,
                                          IBeepTheme theme)
        {
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
