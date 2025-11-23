using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// PhotoGrid - Grid layout of photos/images with overlays and interactive cells
    /// </summary>
    internal sealed class PhotoGridPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _photoPainter;
        private readonly List<Rectangle> _photoRects = new();
        private int _cols = 3;
        private int _rows = 2;
        private int _spacing = 6;

        public PhotoGridPainter()
        {
            _photoPainter = new ImagePainter();
            _photoPainter.ScaleMode = Vis.Modules.ImageScaleMode.Fill;
            _photoPainter.ClipShape = Vis.Modules.ImageClipShape.RoundedRect;
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

            // Precompute photo cell rectangles
            _photoRects.Clear();
            var area = ctx.ContentRect;
            int photoWidth = (area.Width - _spacing * (_cols - 1)) / _cols;
            int photoHeight = (area.Height - _spacing * (_rows - 1)) / _rows;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    var rect = new Rectangle(
                        area.X + col * (photoWidth + _spacing),
                        area.Y + row * (photoHeight + _spacing),
                        photoWidth,
                        photoHeight
                    );
                    _photoRects.Add(rect);
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
                _photoPainter.CurrentTheme = Theme;
            }

            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Draw photos using precomputed rects
            for (int i = 0; i < _photoRects.Count; i++)
            {
                var photoRect = _photoRects[i];

                // Try custom image path list in CustomImagePaths
                bool photoDrawn = false;
                if (ctx.CustomImagePaths != null && i < ctx.CustomImagePaths.Count)
                {
                    var path = ctx.CustomImagePaths[i];
                    try
                    {
                        StyledImagePainter.Paint(g, photoRect, path);
                        photoDrawn = true;
                    }
                    catch
                    {
                        try { _photoPainter.DrawImage(g, ctx.CustomImagePaths[i], photoRect); photoDrawn = true; } catch { }
                    }
                }

                if (!photoDrawn)
                {
                    DrawPlaceholderPhoto(g, photoRect, ctx.AccentColor);
                }

                // Hover effect
                if (IsAreaHovered($"PhotoGrid_Cell_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(30, Theme?.PrimaryColor ?? Color.Blue));
                    using var path = CreateRoundedPath(photoRect, 6);
                    g.FillPath(hover, path);
                }

                // Overlay with index
                DrawPhotoOverlay(g, photoRect, i + 1);
            }
        }

        private void DrawPlaceholderPhoto(Graphics g, Rectangle rect, Color color)
        {
            using var gradientBrush = new LinearGradientBrush(
                rect, 
                Color.FromArgb(100, color), 
                Color.FromArgb(200, color), 
                LinearGradientMode.Vertical);
            
            using var path = CreateRoundedPath(rect, 6);
            g.FillPath(gradientBrush, path);
            
            using var sceneryPen = new Pen(Color.FromArgb(150, Color.White), 2);
            var mountainPoints = new Point[]
            {
                new Point(rect.X + rect.Width / 4, rect.Y + rect.Height * 2 / 3),
                new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 3),
                new Point(rect.X + rect.Width * 3 / 4, rect.Y + rect.Height * 2 / 3),
                new Point(rect.Right, rect.Y + rect.Height / 2)
            };
            g.DrawLines(sceneryPen, mountainPoints);
            var sunRect = new Rectangle(rect.Right - 20, rect.Y + 8, 12, 12);
            g.DrawEllipse(sceneryPen, sunRect);
        }

        private void DrawPhotoOverlay(Graphics g, Rectangle rect, int index)
        {
            var overlayRect = new Rectangle(rect.X, rect.Bottom - 24, rect.Width, 24);
            using var overlayBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            g.FillRectangle(overlayBrush, overlayRect);
            using var indexFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var indexBrush = new SolidBrush(Color.White);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString($"#{index}", indexFont, indexBrush, overlayRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: selection border could be drawn here
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _photoRects.Count; i++)
            {
                int idx = i;
                var rect = _photoRects[i];
                owner.AddHitArea($"PhotoGrid_Cell_{idx}", rect, null, () =>
                {
                    ctx.SelectedPhotoIndex = idx;
                    notifyAreaHit?.Invoke($"PhotoGrid_Cell_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _photoPainter?.Dispose();
        }
    }
}
