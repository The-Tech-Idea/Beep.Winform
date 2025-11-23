using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// IconGrid - Grid layout of icons with labels, now with icon cell hit areas
    /// </summary>
    internal sealed class IconGridPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _iconPainter;
        private readonly List<Rectangle> _cellRects = new();

        public IconGridPainter()
        {
            _iconPainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            }
            
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );

            // Precompute cell rectangles
            _cellRects.Clear();
            var area = ctx.ContentRect;
            int cols = 4;
            int rows = 3;
            int iconSize = Math.Min((area.Width - 20) / cols, (area.Height - 20) / rows) - 8;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var iconRect = new Rectangle(
                        area.X + col * (iconSize + 8) + 8,
                        area.Y + row * (iconSize + 8) + 8,
                        iconSize,
                        iconSize
                    );
                    _cellRects.Add(iconRect);
                }
            }
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            
            using var borderPen = new Pen(Theme?.BorderColor ?? Color.LightGray, 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (Theme != null)
            {
                _iconPainter.CurrentTheme = Theme;
                _iconPainter.ApplyThemeOnImage = true;
            }

            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            string[] iconNames = { "home", "settings", "user", "folder", "mail", "phone", "camera", "calendar", "star", "heart", "check", "plus" };
            for (int i = 0; i < _cellRects.Count && i < iconNames.Length; i++)
            {
                var iconRect = _cellRects[i];

                // Draw icon background and hover
                using var bg = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.FillEllipse(bg, iconRect);
                if (IsAreaHovered($"IconGrid_Cell_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(40, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillEllipse(hover, Rectangle.Inflate(iconRect, 4, 4));
                }

                // Try custom icon
                bool drawn = false;
                var customIconPath = ctx.IconPath;
                if (!string.IsNullOrEmpty(customIconPath))
                {
                    try
                    {
                        // Draw strongly-typed / external SVG icon with tint (white) over colored background
                        StyledImagePainter.PaintWithTint(g, iconRect, customIconPath, Color.White, 1f, cornerRadius: 2);
                        drawn = true;
                    }
                    catch
                    {
                        try { _iconPainter.DrawImage(g, customIconPath, iconRect); drawn = true; } catch { }
                    }
                }
                if (!drawn)
                {
                    DrawPlaceholderIcon(g, iconRect, ctx.AccentColor, iconNames[i]);
                }

                // Label
                var labelRect = new Rectangle(iconRect.X - 10, iconRect.Bottom + 2, iconRect.Width + 20, 16);
                using var labelFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
                using var labelBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(iconNames[i], labelFont, labelBrush, labelRect, format);
            }
        }

        private void DrawPlaceholderIcon(Graphics g, Rectangle rect, Color color, string iconType)
        {
            using var iconPen = new Pen(color, 2);
            using var iconBrush = new SolidBrush(Color.FromArgb(30, color));
            g.FillEllipse(iconBrush, rect);
            g.DrawEllipse(iconPen, rect);
            var centerX = rect.X + rect.Width / 2;
            var centerY = rect.Y + rect.Height / 2;
            var size = rect.Width / 4;
            switch (iconType.ToLower())
            {
                case "home":
                    var homePoints = new Point[] {
                        new Point(centerX, centerY - size),
                        new Point(centerX - size, centerY),
                        new Point(centerX + size, centerY),
                        new Point(centerX, centerY - size)
                    };
                    g.DrawLines(iconPen, homePoints);
                    break;
                case "settings":
                    g.DrawEllipse(iconPen, centerX - size/2, centerY - size/2, size, size);
                    break;
                default:
                    g.DrawRectangle(iconPen, centerX - size/2, centerY - size/2, size, size);
                    break;
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: title underline hover via owner
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _cellRects.Count; i++)
            {
                int idx = i;
                var rect = _cellRects[i];
                owner.AddHitArea($"IconGrid_Cell_{idx}", rect, null, () =>
                {
                    ctx.SelectedIconIndex = idx;
                    notifyAreaHit?.Invoke($"IconGrid_Cell_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _iconPainter?.Dispose();
        }
    }
}
