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
    /// PhotoGrid - Grid layout of photos/images with overlays
    /// </summary>
    internal sealed class PhotoGridPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _photoPainter;

        public PhotoGridPainter()
        {
            _photoPainter = new ImagePainter();
            _photoPainter.ScaleMode = ImageScaleMode.Fill;
            _photoPainter.ClipShape = ImageClipShape.RoundedRect;
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
                _photoPainter.Theme = Theme;
            }

            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Draw photo grid
            DrawPhotoGrid(g, ctx.ContentRect, ctx);
        }

        private void DrawPhotoGrid(Graphics g, Rectangle area, WidgetContext ctx)
        {
            int cols = 3;
            int rows = 2;
            int spacing = 6;
            int photoWidth = (area.Width - spacing * (cols - 1)) / cols;
            int photoHeight = (area.Height - spacing * (rows - 1)) / rows;
            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var photoRect = new Rectangle(
                        area.X + col * (photoWidth + spacing),
                        area.Y + row * (photoHeight + spacing),
                        photoWidth,
                        photoHeight
                    );
                    
                    // Try to draw custom photo using ImagePainter if ImagePath exists
                    bool photoDrawn = false;
                    if (!string.IsNullOrEmpty(ctx.ImagePath))
                    {
                        try
                        {
                            _photoPainter.DrawImage(g, ctx.ImagePath, photoRect);
                            photoDrawn = true;
                        }
                        catch { }
                    }
                    
                    // Draw placeholder photo if no custom image
                    if (!photoDrawn)
                    {
                        DrawPlaceholderPhoto(g, photoRect, ctx.AccentColor);
                    }
                    
                    // Draw photo overlay with index
                    DrawPhotoOverlay(g, photoRect, row * cols + col + 1);
                }
            }
        }

        private void DrawPlaceholderPhoto(Graphics g, Rectangle rect, Color color)
        {
            // Draw gradient background
            using var gradientBrush = new LinearGradientBrush(
                rect, 
                Color.FromArgb(100, color), 
                Color.FromArgb(200, color), 
                LinearGradientMode.Vertical);
            
            using var path = CreateRoundedPath(rect, 6);
            g.FillPath(gradientBrush, path);
            
            // Draw mountain/landscape placeholder
            using var sceneryPen = new Pen(Color.FromArgb(150, Color.White), 2);
            
            // Mountains
            var mountainPoints = new Point[]
            {
                new Point(rect.X + rect.Width / 4, rect.Y + rect.Height * 2 / 3),
                new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 3),
                new Point(rect.X + rect.Width * 3 / 4, rect.Y + rect.Height * 2 / 3),
                new Point(rect.Right, rect.Y + rect.Height / 2)
            };
            g.DrawLines(sceneryPen, mountainPoints);
            
            // Sun
            var sunRect = new Rectangle(rect.Right - 20, rect.Y + 8, 12, 12);
            g.DrawEllipse(sceneryPen, sunRect);
        }

        private void DrawPhotoOverlay(Graphics g, Rectangle rect, int index)
        {
            // Semi-transparent overlay at bottom
            var overlayRect = new Rectangle(rect.X, rect.Bottom - 24, rect.Width, 24);
            using var overlayBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            g.FillRectangle(overlayBrush, overlayRect);
            
            // Photo number
            using var indexFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var indexBrush = new SolidBrush(Color.White);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString($"#{index}", indexFont, indexBrush, overlayRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw selection indicators or hover effects
        }

        public void Dispose()
        {
            _photoPainter?.Dispose();
        }
    }
}
