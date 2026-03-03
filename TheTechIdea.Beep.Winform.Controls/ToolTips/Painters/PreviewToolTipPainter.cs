using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Sprint 4 — Painter for Preview / hover-card style tooltips.
    /// Layout:
    ///   ┌──────────────────────────┐
    ///   │  [Image / skeleton]      │  ← PreviewImageSize
    ///   ├──────────────────────────┤
    ///   │  Title                   │
    ///   │  Subtitle                │
    ///   │  Footer text             │
    ///   └──────────────────────────┘
    /// </summary>
    public class PreviewToolTipPainter : ToolTipPainterBase
    {
        // Delegate shadow/background/border/content/arrow to the shared styled painter.
        private static readonly BeepStyledToolTipPainter _shared = new BeepStyledToolTipPainter();

        public override void PaintBackground(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
            => _shared.PaintBackground(g, bounds, config, theme);

        public override void PaintBorder(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
            => _shared.PaintBorder(g, bounds, config, theme);

        public override void PaintShadow(Graphics g, Rectangle bounds, ToolTipConfig config)
            => _shared.PaintShadow(g, bounds, config);

        public override void PaintArrow(Graphics g, Point position, ToolTipPlacement placement, ToolTipConfig config, IBeepTheme theme)
            => _shared.PaintArrow(g, position, placement, config, theme);

        public override void PaintContent(Graphics g, Rectangle bounds, ToolTipConfig config, IBeepTheme theme)
            => _shared.PaintContent(g, bounds, config, theme);

        private const int TextPaddingH = 12;
        private const int TextPaddingV = 8;
        private const int TextSpacing  = 4;
        private const int CornerRadius = 10;

        // Skeleton animation
        private int  _skeletonPhase = 0;   // 0–255, animated by caller
        public  int  SkeletonPhase { get => _skeletonPhase; set => _skeletonPhase = value % 256; }

        // ──────────────────────────────────────────────────────────────────────
        // IToolTipPainter
        // ──────────────────────────────────────────────────────────────────────

        public override void Paint(
            Graphics g, Rectangle bounds, ToolTipConfig config,
            ToolTipPlacement placement, IBeepTheme theme)
        {
            if (config == null || bounds.IsEmpty) return;

            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.PixelOffsetMode    = PixelOffsetMode.HighQuality;
            g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Background
            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);

            // Split bounds
            int imageH   = config.PreviewImageSize.Height > 0 ? config.PreviewImageSize.Height : 160;
            var imageRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, imageH);
            var textRect  = new Rectangle(
                bounds.X + TextPaddingH,
                bounds.Y + imageH + TextPaddingV,
                bounds.Width - TextPaddingH * 2,
                bounds.Height - imageH - TextPaddingV * 2);

            PaintPreviewImage(g, imageRect, config, bounds);
            PaintTextSection(g, textRect, config, theme);

            // Arrow
            if (config.ShowArrow)
            {
                var colors = ToolTipStyleAdapter.GetColors(config, theme);
                ToolTipArrowPainter.DrawArrow(g, bounds, placement,
                    config.ArrowStyle, config.ArrowSize, config.ArrowOffset,
                    colors.background, colors.border);
            }
        }

        public override Size CalculateSize(Graphics g, ToolTipConfig config)
        {
            int imageH   = config.PreviewImageSize.Height > 0 ? config.PreviewImageSize.Height : 160;
            int imageW   = config.PreviewImageSize.Width  > 0 ? config.PreviewImageSize.Width  : 280;

            using var titleFont    = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            using var subtitleFont = new Font("Segoe UI", 9f);
            using var footerFont   = new Font("Segoe UI", 8.5f, FontStyle.Italic);

            int textH = TextPaddingV;
            if (!string.IsNullOrEmpty(config.Title))
                textH += TextRenderer.MeasureText(g, config.Title, titleFont).Height + TextSpacing;
            if (!string.IsNullOrEmpty(config.PreviewSubtitle))
                textH += TextRenderer.MeasureText(g, config.PreviewSubtitle, subtitleFont).Height + TextSpacing;
            if (!string.IsNullOrEmpty(config.PreviewFooterText))
                textH += TextRenderer.MeasureText(g, config.PreviewFooterText, footerFont).Height;
            textH += TextPaddingV;

            return new Size(imageW, imageH + textH);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Image / skeleton
        // ──────────────────────────────────────────────────────────────────────

        private void PaintPreviewImage(
            Graphics g, Rectangle imageRect, ToolTipConfig config, Rectangle tooltipBounds)
        {
            // Clip to rounded top corners only
            using var clipPath = TopRoundedRect(imageRect, CornerRadius);
            g.SetClip(clipPath);

            try
            {
                Image img = null;

                if (!string.IsNullOrEmpty(config.PreviewImagePath) &&
                    File.Exists(config.PreviewImagePath))
                {
                    try { img = Image.FromFile(config.PreviewImagePath); }
                    catch { /* fall through to skeleton */ }
                }

                if (img != null)
                {
                    g.DrawImage(img, imageRect);
                    img.Dispose();
                }
                else
                {
                    PaintSkeleton(g, imageRect);
                }
            }
            finally
            {
                g.ResetClip();
            }
        }

        private void PaintSkeleton(Graphics g, Rectangle r)
        {
            // Background base
            using (var brush = new SolidBrush(Color.FromArgb(50, 50, 55)))
                g.FillRectangle(brush, r);

            // Animated shimmer bar
            int shimmerW = (int)(r.Width * 0.6f);
            float fraction = _skeletonPhase / 255f;
            int shimmerX  = r.Left + (int)((r.Width + shimmerW) * fraction) - shimmerW;

            using var shimmerBrush = new LinearGradientBrush(
                new Rectangle(shimmerX, r.Top, shimmerW, r.Height),
                Color.Transparent,
                Color.FromArgb(40, 255, 255, 255),
                LinearGradientMode.Horizontal)
            { WrapMode = WrapMode.Clamp };

            g.FillRectangle(shimmerBrush, shimmerX, r.Top, shimmerW, r.Height);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Text section
        // ──────────────────────────────────────────────────────────────────────

        private void PaintTextSection(
            Graphics g, Rectangle r, ToolTipConfig config, IBeepTheme theme)
        {
            var colors = ToolTipThemeHelpers.GetThemeColors(theme, config.Type,
                config.UseBeepThemeColors, config.BackColor, config.ForeColor, config.BorderColor);

            int y = r.Top;

            if (!string.IsNullOrEmpty(config.Title))
            {
                using var font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                var sz = TextRenderer.MeasureText(g, config.Title, font);
                TextRenderer.DrawText(g, config.Title, font,
                    new Rectangle(r.Left, y, r.Width, sz.Height),
                    colors.foreColor, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                y += sz.Height + TextSpacing;
            }

            if (!string.IsNullOrEmpty(config.PreviewSubtitle))
            {
                using var font = new Font("Segoe UI", 9f);
                var sz = TextRenderer.MeasureText(g, config.PreviewSubtitle, font,
                             new Size(r.Width, int.MaxValue), TextFormatFlags.WordBreak);
                TextRenderer.DrawText(g, config.PreviewSubtitle, font,
                    new Rectangle(r.Left, y, r.Width, sz.Height),
                    Color.FromArgb(180, colors.foreColor.R, colors.foreColor.G, colors.foreColor.B),
                    TextFormatFlags.WordBreak);
                y += sz.Height + TextSpacing;
            }

            if (!string.IsNullOrEmpty(config.PreviewFooterText))
            {
                using var font = new Font("Segoe UI", 8.5f, FontStyle.Italic);
                TextRenderer.DrawText(g, config.PreviewFooterText, font,
                    new Rectangle(r.Left, y, r.Width, r.Bottom - y),
                    Color.FromArgb(120, colors.foreColor.R, colors.foreColor.G, colors.foreColor.B),
                    TextFormatFlags.WordBreak);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Geometry helpers
        // ──────────────────────────────────────────────────────────────────────

        private static System.Drawing.Drawing2D.GraphicsPath TopRoundedRect(Rectangle r, int radius)
        {
            var p = new System.Drawing.Drawing2D.GraphicsPath();
            p.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            p.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            p.AddLine(r.Right, r.Top + radius, r.Right, r.Bottom);
            p.AddLine(r.Right, r.Bottom, r.Left, r.Bottom);
            p.AddLine(r.Left, r.Bottom, r.Left, r.Top + radius);
            p.CloseFigure();
            return p;
        }
    }
}
