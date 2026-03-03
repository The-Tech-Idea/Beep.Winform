using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Sprint 6 — Painter for guided-tour / walkthrough tooltip steps.
    /// Layout:
    ///   ┌───────────────────────────────────┐
    ///   │  Step N of M                      │  ← step counter
    ///   ├───────────────────────────────────┤
    ///   │  [Image?]                         │
    ///   │  Title (bold)                     │
    ///   │  Body text                        │
    ///   │  ● ● ○ ○ ○   (progress dots)     │
    ///   │  [Skip]         [← Back] [Next →] │  ← nav buttons
    ///   └───────────────────────────────────┘
    /// </summary>
    public class TourToolTipPainter : ToolTipPainterBase
    {
        // Delegate shared section painters to the styled base painter.
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

        // ──────────────────────────────────────────────────────────────────────
        // Layout constants
        // ──────────────────────────────────────────────────────────────────────
        private const int PaddingH       = 14;
        private const int PaddingV       = 12;
        private const int Spacing        = 6;
        private const int DotSize        = 7;
        private const int DotSpacing     = 5;
        private const int BtnH           = 26;
        private const int CornerRadius   = 10;
        private const int StepBadgeH     = 20;

        // ──────────────────────────────────────────────────────────────────────
        // IToolTipPainter
        // ──────────────────────────────────────────────────────────────────────

        public override void Paint(
            Graphics g, Rectangle bounds, ToolTipConfig config,
            ToolTipPlacement placement, IBeepTheme theme)
        {
            if (config == null || bounds.IsEmpty) return;

            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.PixelOffsetMode   = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var colors = ToolTipThemeHelpers.GetThemeColors(theme, config.Type,
                config.UseBeepThemeColors, config.BackColor, config.ForeColor, config.BorderColor);

            PaintBackground(g, bounds, config, theme);
            PaintBorder(g, bounds, config, theme);

            int y = bounds.Y + PaddingV;

            y = PaintStepBadge(g, bounds, config, colors, y);
            y = PaintSeparator(g, bounds, colors, y);
            y = PaintTourContent(g, bounds, config, colors, y);
            y = PaintProgressDots(g, bounds, config, colors, y);
            PaintNavButtons(g, bounds, config, colors, theme);

            // Arrow
            if (config.ShowArrow)
                ToolTipArrowPainter.DrawArrow(g, bounds, placement,
                    config.ArrowStyle, config.ArrowSize, config.ArrowOffset,
                    colors.backColor, colors.borderColor);
        }

        public override Size CalculateSize(Graphics g, ToolTipConfig config)
        {
            int w = 300;
            int h = PaddingV;

            h += StepBadgeH + Spacing;         // step counter
            h += 1          + Spacing;          // separator
            if (!string.IsNullOrEmpty(config.Title))
            {
                using var f = new Font("Segoe UI", 11f, FontStyle.Bold);
                h += TextRenderer.MeasureText(g, config.Title, f, new Size(w - PaddingH*2, 0), TextFormatFlags.WordBreak).Height + Spacing;
            }
            if (!string.IsNullOrEmpty(config.Text))
            {
                using var f = new Font("Segoe UI", 9.5f);
                h += TextRenderer.MeasureText(g, config.Text, f, new Size(w - PaddingH*2, 0), TextFormatFlags.WordBreak).Height + Spacing;
            }
            h += DotSize    + Spacing;          // progress dots
            h += BtnH       + PaddingV;         // nav buttons

            return new Size(w, h);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Section painters
        // ──────────────────────────────────────────────────────────────────────

        private int PaintStepBadge(
            Graphics g, Rectangle bounds, ToolTipConfig config,
            (Color backColor, Color foreColor, Color borderColor) colors, int y)
        {
            string badge = $"Step {config.CurrentStep} of {config.TotalSteps}";
            using var font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(100, colors.foreColor));

            // Right-align
            int tw = TextRenderer.MeasureText(g, badge, font).Width;
            TextRenderer.DrawText(g, badge, font,
                new Point(bounds.Right - PaddingH - tw, y),
                Color.FromArgb(130, colors.foreColor));

            return y + StepBadgeH + Spacing;
        }

        private int PaintSeparator(
            Graphics g, Rectangle bounds,
            (Color backColor, Color foreColor, Color borderColor) colors, int y)
        {
            using var pen = new Pen(Color.FromArgb(40, colors.foreColor));
            g.DrawLine(pen, bounds.Left + PaddingH, y, bounds.Right - PaddingH, y);
            return y + 1 + Spacing;
        }

        private int PaintTourContent(
            Graphics g, Rectangle bounds, ToolTipConfig config,
            (Color backColor, Color foreColor, Color borderColor) colors, int y)
        {
            int textW = bounds.Width - PaddingH * 2;

            if (!string.IsNullOrEmpty(config.Title))
            {
                using var font = new Font("Segoe UI", 11f, FontStyle.Bold);
                var sz = TextRenderer.MeasureText(g, config.Title, font, new Size(textW, 0), TextFormatFlags.WordBreak);
                TextRenderer.DrawText(g, config.Title, font,
                    new Rectangle(bounds.Left + PaddingH, y, textW, sz.Height),
                    colors.foreColor, TextFormatFlags.WordBreak);
                y += sz.Height + Spacing;
            }

            if (!string.IsNullOrEmpty(config.Text))
            {
                using var font = new Font("Segoe UI", 9.5f);
                var sz = TextRenderer.MeasureText(g, config.Text, font, new Size(textW, 0), TextFormatFlags.WordBreak);
                TextRenderer.DrawText(g, config.Text, font,
                    new Rectangle(bounds.Left + PaddingH, y, textW, sz.Height),
                    Color.FromArgb(210, colors.foreColor), TextFormatFlags.WordBreak);
                y += sz.Height + Spacing;
            }

            return y;
        }

        private int PaintProgressDots(
            Graphics g, Rectangle bounds, ToolTipConfig config,
            (Color backColor, Color foreColor, Color borderColor) colors, int y)
        {
            if (config.TotalSteps <= 1) return y;

            int totalDotsW = config.TotalSteps * DotSize + (config.TotalSteps - 1) * DotSpacing;
            int startX = bounds.Left + PaddingH;

            for (int i = 0; i < config.TotalSteps; i++)
            {
                int dotX  = startX + i * (DotSize + DotSpacing);
                bool active = i == config.CurrentStep - 1;
                var dotColor = active ? colors.borderColor : Color.FromArgb(60, colors.foreColor);

                using var brush = new SolidBrush(dotColor);
                g.FillEllipse(brush, dotX, y, DotSize, DotSize);
            }

            return y + DotSize + Spacing;
        }

        private void PaintNavButtons(
            Graphics g, Rectangle bounds, ToolTipConfig config,
            (Color backColor, Color foreColor, Color borderColor) colors, IBeepTheme theme)
        {
            int btnY    = bounds.Bottom - BtnH - PaddingV;
            int margin  = PaddingH;
            int accentR = 4;

            // "Skip" button — left side
            if (config.ShowNavigationButtons)
            {
                string skipText = "Skip";
                using var font  = new Font("Segoe UI", 9f);
                var skipColor   = Color.FromArgb(130, colors.foreColor);
                TextRenderer.DrawText(g, skipText, font,
                    new Point(bounds.Left + margin, btnY + (BtnH - 16) / 2),
                    skipColor);
            }

            // "Next →" button — right
            if (config.ShowNavigationButtons && config.CurrentStep <= config.TotalSteps)
            {
                bool isLast    = config.CurrentStep == config.TotalSteps;
                string nextTxt = isLast ? "Done" : "Next →";
                using var fnt  = new Font("Segoe UI", 9f, FontStyle.Bold);
                var sz         = TextRenderer.MeasureText(g, nextTxt, fnt);
                int bw         = sz.Width + 16;
                var btnRect    = new Rectangle(bounds.Right - margin - bw, btnY, bw, BtnH);

                using var bPath = RoundedRect(btnRect, accentR);
                using var br    = new SolidBrush(colors.borderColor);
                g.FillPath(br, bPath);

                TextRenderer.DrawText(g, nextTxt, fnt,
                    new Point(btnRect.Left + 8, btnRect.Top + (BtnH - sz.Height) / 2),
                    Color.White);

                // "← Back" if not first step
                if (config.CurrentStep > 1)
                {
                    string backTxt  = "← Back";
                    using var fnt2  = new Font("Segoe UI", 9f);
                    var sz2         = TextRenderer.MeasureText(g, backTxt, fnt2);
                    int backX       = btnRect.Left - sz2.Width - 12;
                    TextRenderer.DrawText(g, backTxt, fnt2,
                        new Point(backX, btnRect.Top + (BtnH - sz2.Height) / 2),
                        Color.FromArgb(170, colors.foreColor));
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Geometry
        // ──────────────────────────────────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            p.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            p.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            p.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
