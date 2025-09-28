using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// IconGrid - Grid layout of icons with labels
    /// </summary>
    internal sealed class IconGridPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _iconPainter;

        public IconGridPainter()
        {
            _iconPainter = new ImagePainter();
            _iconPainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
            _iconPainter.ClipShape = ImageClipShape.None;
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Title area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            }
            
            // Grid area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );
            
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
            // Update theme configuration
            if (Theme != null)
            {
                _iconPainter.Theme = Theme;
            }

            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Draw icon grid (3x3 or dynamic based on content area)
            DrawIconGrid(g, ctx.ContentRect, ctx);
        }

        private void DrawIconGrid(Graphics g, Rectangle area, WidgetContext ctx)
        {
            int cols = 4;
            int rows = 3;
            int iconSize = Math.Min((area.Width - 20) / cols, (area.Height - 20) / rows) - 8;
            
            var iconNames = new[] { "home", "settings", "user", "folder", "mail", "phone", "camera", "calendar", "star", "heart", "check", "plus" };
            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int index = row * cols + col;
                    if (index >= iconNames.Length) break;
                    
                    var iconRect = new Rectangle(
                        area.X + col * (iconSize + 8) + 8,
                        area.Y + row * (iconSize + 8) + 8,
                        iconSize,
                        iconSize
                    );
                    
                    // Try to draw custom icon using ImagePainter if IconPath exists
                    bool iconDrawn = false;
                    if (!string.IsNullOrEmpty(ctx.IconPath))
                    {
                        try
                        {
                            _iconPainter.DrawImage(g, ctx.IconPath, iconRect);
                            iconDrawn = true;
                        }
                        catch { }
                    }
                    
                    // Draw placeholder icon if no custom icon
                    if (!iconDrawn)
                    {
                        DrawPlaceholderIcon(g, iconRect, ctx.AccentColor, iconNames[index]);
                    }
                    
                    // Draw icon label
                    var labelRect = new Rectangle(iconRect.X - 10, iconRect.Bottom + 2, iconRect.Width + 20, 16);
                    using var labelFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
                    using var labelBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(iconNames[index], labelFont, labelBrush, labelRect, format);
                }
            }
        }

        private void DrawPlaceholderIcon(Graphics g, Rectangle rect, Color color, string iconType)
        {
            using var iconPen = new Pen(color, 2);
            using var iconBrush = new SolidBrush(Color.FromArgb(30, color));
            
            // Draw background circle
            g.FillEllipse(iconBrush, rect);
            g.DrawEllipse(iconPen, rect);
            
            // Draw simple icon based on type
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
            // Optional: Draw hover effects or selection indicators
        }

        public void Dispose()
        {
            _iconPainter?.Dispose();
        }
    }
}
