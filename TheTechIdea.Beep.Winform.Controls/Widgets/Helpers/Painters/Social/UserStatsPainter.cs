using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Social
{
    /// <summary>
    /// UserStats - User statistics display painter
    /// </summary>
    internal sealed class UserStatsPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            // Ensure painters draw within BaseControl.DrawingRect as the canonical bounds
            ctx.DrawingRect = Owner?.DrawingRect ?? drawingRect;

            // Title area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            }
            else
            {
                ctx.HeaderRect = Rectangle.Empty;
            }

            // Stats content area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                Math.Max(0, ctx.DrawingRect.Bottom - contentTop - pad)
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Optional subtle shadow then background
            DrawSoftShadow(g, ctx.DrawingRect, Math.Max(4, ctx.CornerRadius), layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                var baseFont = Owner?.Font ?? SystemFonts.DefaultFont;
                using var titleFont = new Font(baseFont.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                var titleFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, titleFormat);
            }

            // Draw stats content
            int count = Math.Min(ctx.Labels?.Count ?? 0, ctx.Values?.Count ?? 0);
            if (count <= 0)
            {
                var baseFont = Owner?.Font ?? SystemFonts.DefaultFont;
                using var contentFont = new Font(baseFont.FontFamily, 9f, FontStyle.Regular);
                using var contentBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                g.DrawString("User statistics implementation pending...", contentFont, contentBrush, ctx.ContentRect);
                return;
            }

            var statRects = ComputeStatRects(ctx.ContentRect, count);

            var baseOwnerFont = Owner?.Font ?? SystemFonts.DefaultFont;
            for (int i = 0; i < count; i++)
            {
                var rect = statRects[i];
                string label = ctx.Labels[i];
                double value = ctx.Values[i];
                var color = (ctx.Colors != null && ctx.Colors.Count > i) ? ctx.Colors[i] : ctx.AccentColor;

                // Value on top
                var valueRect = new Rectangle(rect.X, rect.Y, rect.Width, (int)(rect.Height * 0.55));
                using var valueFont = new Font(baseOwnerFont.FontFamily, Math.Max(10f, Math.Min(16f, rect.Height * 0.22f)), FontStyle.Bold);
                DrawValue(g, valueRect, value.ToString("#,##0"), ctx.Units ?? string.Empty, valueFont, color);

                // Label below
                var labelRect = new Rectangle(rect.X, valueRect.Bottom, rect.Width, rect.Bottom - valueRect.Bottom);
                using var labelFont = new Font(baseOwnerFont.FontFamily, Math.Max(8f, Math.Min(11f, rect.Height * 0.18f)), FontStyle.Regular);
                using var labelBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                var labelFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
                g.DrawString(label, labelFont, labelBrush, labelRect, labelFormat);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Hover highlight for stat items
            int count = Math.Min(ctx.Labels?.Count ?? 0, ctx.Values?.Count ?? 0);
            if (count <= 0)
                return;

            var hovered = GetHoveredAreaName();
            if (string.IsNullOrEmpty(hovered) || !hovered.StartsWith("Stat:"))
                return;

            var statRects = ComputeStatRects(ctx.ContentRect, count);
            if (int.TryParse(hovered.Substring("Stat:".Length), out int idx) && idx >= 0 && idx < statRects.Count)
            {
                var r = Rectangle.Inflate(statRects[idx], 1, 1);
                using var pen = new Pen(Color.FromArgb(60, ctx.AccentColor), 2);
                g.DrawRectangle(pen, r);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Register interactive areas for each stat cell
            ClearOwnerHitAreas();

            int count = Math.Min(ctx.Labels?.Count ?? 0, ctx.Values?.Count ?? 0);
            if (count <= 0)
                return;

            var statRects = ComputeStatRects(ctx.ContentRect, count);
            for (int i = 0; i < count; i++)
            {
                int capture = i; // avoid modified closure
                var name = $"Stat:{capture}";
                AddHitAreaToOwner(name, statRects[i], () => notifyAreaHit?.Invoke(name, statRects[capture]));
            }
        }

        private static List<Rectangle> ComputeStatRects(Rectangle contentRect, int count)
        {
            // Try 3 columns if possible, fallback to 2
            int columns = count >= 3 ? 3 : 2;
            if (count == 1) columns = 1;
            int rows = (int)Math.Ceiling(count / (double)columns);

            int hGap = 8;
            int vGap = 8;

            int cellWidth = (contentRect.Width - (columns - 1) * hGap) / columns;
            int cellHeight = (contentRect.Height - (rows - 1) * vGap) / rows;

            var rects = new List<Rectangle>(count);
            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                int x = contentRect.X + col * (cellWidth + hGap);
                int y = contentRect.Y + row * (cellHeight + vGap);
                rects.Add(new Rectangle(x, y, cellWidth, cellHeight));
            }
            return rects;
        }
    }
}