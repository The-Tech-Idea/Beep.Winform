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
            var card = GetCardRectangle(bounds, config);
            int imageH   = ImageBandHeight(config);
            var imageRect = new Rectangle(card.X, card.Y, card.Width, imageH);
            var textRect  = new Rectangle(
                card.X + TextPaddingH,
                card.Y + imageH + TextPaddingV,
                card.Width - TextPaddingH * 2,
                card.Height - imageH - TextPaddingV * 2);

            if (imageH > 0) PaintPreviewImage(g, imageRect, config, card);
            PaintTextSection(g, textRect, config, theme);

            // No arrow pass: the caret is part of the silhouette PaintBackground already
            // fills. Drawing it again here aimed at the FULL window bounds, which put the tip
            // outside the window where it was clipped away, and double-stroked its border.
        }

        public override Size CalculateSize(Graphics g, ToolTipConfig config)
        {
            int imageH   = ImageBandHeight(config);
            int imageW   = config.PreviewImageSize.Width  > 0 ? config.PreviewImageSize.Width  : 280;

            var (titleFont, bodyFont) = TextFonts();
            int textW = imageW - TextPaddingH * 2;
            string body = BodyText(config);

            int textH = TextPaddingV;
            if (!string.IsNullOrEmpty(config.Title))
                textH += TextRenderer.MeasureText(g, config.Title, titleFont).Height + TextSpacing;
            if (!string.IsNullOrEmpty(body))
                textH += TextRenderer.MeasureText(g, body, bodyFont,
                             new Size(textW, int.MaxValue), TextFormatFlags.WordBreak).Height + TextSpacing;
            if (!string.IsNullOrEmpty(config.PreviewFooterText))
                textH += TextRenderer.MeasureText(g, config.PreviewFooterText, bodyFont,
                             new Size(textW, int.MaxValue), TextFormatFlags.WordBreak).Height;
            textH += TextPaddingV;

            return new Size(imageW, imageH + textH);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Image / skeleton
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The height to reserve for the preview image: zero when the config names no image
        /// source at all.
        /// </summary>
        /// <remarks>
        /// This used to reserve the band unconditionally and fill it with the loading skeleton,
        /// so a Preview tooltip with only text rendered as a 280x199 window that was 160px of
        /// empty placeholder for an image that was never coming - and the body text was pushed
        /// out of the card entirely. A skeleton means "still loading"; with no path and no
        /// loader there is nothing to wait for.
        /// </remarks>
        /// <summary>
        /// The body line: <see cref="ToolTipConfig.PreviewSubtitle"/> when set, otherwise the
        /// ordinary <see cref="ToolTipConfig.Text"/>.
        /// </summary>
        /// <remarks>
        /// This painter read only PreviewSubtitle, so a Preview tooltip built the usual way - with
        /// Title and Text - rendered its title and silently dropped its body. Text is the property
        /// every other variant uses and every caller sets.
        /// </remarks>
        private static string BodyText(ToolTipConfig config)
            => !string.IsNullOrWhiteSpace(config.PreviewSubtitle) ? config.PreviewSubtitle : config.Text;

        /// <summary>
        /// The fonts this painter measures AND draws with, from the theme's typography.
        /// </summary>
        /// <remarks>
        /// One source so CalculateSize and PaintTextSection cannot disagree - measuring with one
        /// font and drawing with another is what clips text. These were literal
        /// <c>new Font("Segoe UI", ...)</c> instances built afresh on every paint, which also
        /// ignored the theme entirely. They are cache-owned: never dispose them.
        /// </remarks>
        private static (Font title, Font body) TextFonts()
        {
            var theme = BeepThemesManager.CurrentTheme;
            var title = BeepThemesManager.ToFont(theme?.TitleStyle) ?? BeepFontManager.DefaultFont;
            var body  = BeepThemesManager.ToFont(theme?.BodyStyle)  ?? BeepFontManager.DefaultFont;
            return (title, body);
        }

        private static int ImageBandHeight(ToolTipConfig config)
        {
            bool hasSource = config.ResolvedPreviewImage != null
                             || !string.IsNullOrWhiteSpace(config.PreviewImagePath)
                             || config.LoadPreviewAsync != null;
            if (!hasSource) return 0;
            return config.PreviewImageSize.Height > 0 ? config.PreviewImageSize.Height : 160;
        }

        private void PaintPreviewImage(
            Graphics g, Rectangle imageRect, ToolTipConfig config, Rectangle tooltipBounds)
        {
            // Clip to rounded top corners only
            using var clipPath = TopRoundedRect(imageRect, CornerRadius);
            g.SetClip(clipPath);

            try
            {
                // Read the already-resolved image. This used to call Image.FromFile here and
                // dispose it again, so the file was re-read from disk on every single repaint —
                // and an async LoadPreviewAsync result had nowhere to go. ToolTipInstance now
                // resolves the image once (from the delegate or the path) and owns its lifetime.
                var img = config.ResolvedPreviewImage;

                if (img != null)
                {
                    g.DrawImage(img, imageRect);
                }
                else
                {
                    // Still loading, or no image supplied.
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
            if (r.Width <= 0 || r.Height <= 0) return;

            // Background base
            using (var brush = new SolidBrush(Color.FromArgb(24, ToolTipThemeHelpers.GetToolTipForeColor(null, ToolTipType.Default))))
                g.FillRectangle(brush, r);

            // Animated shimmer bar
            int shimmerW = Math.Max(1, (int)(r.Width * 0.6f));
            float fraction = _skeletonPhase / 255f;
            int shimmerX  = r.Left + (int)((r.Width + shimmerW) * fraction) - shimmerW;
            var shimmerRect = new Rectangle(shimmerX, r.Top, shimmerW, r.Height);

            // NO WrapMode here.
            //
            // This used to set WrapMode.Clamp, which GDI+ rejects on a LinearGradientBrush — only
            // the Tile modes are legal — so the setter threw ArgumentException("Parameter is not
            // valid") on every paint. That exception escaped Paint and WinForms drew its red-X
            // error box in place of the entire tooltip. It went unnoticed because this painter was
            // never actually instantiated: ToolTipPainterFactory had no call sites.
            //
            // The default (Tile) is correct here anyway: the fill rectangle and the gradient
            // rectangle are identical, so the gradient never repeats.
            using var shimmerBrush = new LinearGradientBrush(
                shimmerRect,
                Color.Transparent,
                Color.FromArgb(40, 255, 255, 255),
                LinearGradientMode.Horizontal);

            g.FillRectangle(shimmerBrush, shimmerRect);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Text section
        // ──────────────────────────────────────────────────────────────────────

        private void PaintTextSection(
            Graphics g, Rectangle r, ToolTipConfig config, IBeepTheme theme)
        {
            var colors = ToolTipThemeHelpers.GetThemeColors(theme, config.Type,
                config.BackColor, config.ForeColor, config.BorderColor);

            var (titleFont, bodyFont) = TextFonts();
            int y = r.Top;
            string body = BodyText(config);

            if (!string.IsNullOrEmpty(config.Title))
            {
                var sz = TextRenderer.MeasureText(g, config.Title, titleFont);
                TextRenderer.DrawText(g, config.Title, titleFont,
                    new Rectangle(r.Left, y, r.Width, sz.Height),
                    colors.foreColor, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                y += sz.Height + TextSpacing;
            }

            if (!string.IsNullOrEmpty(body))
            {
                var sz = TextRenderer.MeasureText(g, body, bodyFont,
                             new Size(r.Width, int.MaxValue), TextFormatFlags.WordBreak);
                TextRenderer.DrawText(g, body, bodyFont,
                    new Rectangle(r.Left, y, r.Width, sz.Height),
                    Color.FromArgb(180, colors.foreColor.R, colors.foreColor.G, colors.foreColor.B),
                    TextFormatFlags.WordBreak);
                y += sz.Height + TextSpacing;
            }

            if (!string.IsNullOrEmpty(config.PreviewFooterText))
            {
                TextRenderer.DrawText(g, config.PreviewFooterText, bodyFont,
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
