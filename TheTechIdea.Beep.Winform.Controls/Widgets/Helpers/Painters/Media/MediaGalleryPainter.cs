using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    internal sealed class MediaGalleryPainter : WidgetPainterBase, IDisposable
    {
        private readonly ImagePainter _imagePainter;
        private bool _disposed = false;

        public MediaGalleryPainter()
        {
            _imagePainter = new ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) 
        {
            ctx.DrawingRect = drawingRect;
            
            // Layout for media gallery with grid of images
            int padding = 8;
            int galleryWidth = drawingRect.Width - (padding * 2);
            int galleryHeight = drawingRect.Height - (padding * 2);
            
            ctx.ContentRect = new Rectangle(
                drawingRect.X + padding, 
                drawingRect.Y + padding,
                galleryWidth,
                galleryHeight
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            using var bgBrush = new SolidBrush(Theme.BackgroundColor);
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Draw gallery background with subtle pattern
            using var patternBrush = new SolidBrush(Color.FromArgb(10, Theme.AccentColor));
            using var pen = new Pen(Theme.BorderColor, 1);
            g.FillRectangle(patternBrush, ctx.ContentRect);
            g.DrawRectangle(pen, ctx.ContentRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            DrawMediaGallery(g, ctx);
        }

        private void DrawMediaGallery(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter for gallery display
            _imagePainter.Theme = Theme;
            _imagePainter.ClippingShape = ImagePainter.ClipShape.RoundedRect;
            _imagePainter.ScalingMode = ImagePainter.ImageScaleMode.Fill;
            _imagePainter.CornerRadius = 8;

            // Create 3x2 media gallery grid
            int cols = 3, rows = 2;
            int itemSpacing = 6;
            int itemWidth = (ctx.ContentRect.Width - (itemSpacing * (cols + 1))) / cols;
            int itemHeight = (ctx.ContentRect.Height - (itemSpacing * (rows + 1))) / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = ctx.ContentRect.X + itemSpacing + col * (itemWidth + itemSpacing);
                    int y = ctx.ContentRect.Y + itemSpacing + row * (itemHeight + itemSpacing);
                    Rectangle itemRect = new Rectangle(x, y, itemWidth, itemHeight);

                    // Draw media item with ImagePainter
                    DrawMediaItemWithImagePainter(g, itemRect, ctx, row * cols + col);
                }
            }
        }

        private void DrawMediaItemWithImagePainter(Graphics g, Rectangle itemRect, WidgetContext ctx, int index)
        {
            try
            {
                // Try to render with ImagePainter for custom media
                if (ctx.CustomImagePaths != null && index < ctx.CustomImagePaths.Count)
                {
                    _imagePainter.DrawImage(g, ctx.CustomImagePaths[index], itemRect);
                }
                else
                {
                    // Fallback: generate placeholder media thumbnails
                    DrawMediaPlaceholder(g, itemRect, ctx, index);
                }

                // Add media overlay indicators
                DrawMediaOverlay(g, itemRect, ctx, index);
            }
            catch
            {
                // Fallback on any error
                DrawMediaPlaceholder(g, itemRect, ctx, index);
            }
        }

        private void DrawMediaPlaceholder(Graphics g, Rectangle itemRect, WidgetContext ctx, int index)
        {
            // Create media type placeholders (photos, videos, etc.)
            Color[] mediaColors = 
            {
                Color.FromArgb(100, 255, 182, 193), // Light pink
                Color.FromArgb(100, 173, 216, 230), // Light blue
                Color.FromArgb(100, 144, 238, 144), // Light green
                Color.FromArgb(100, 255, 218, 185), // Peach
                Color.FromArgb(100, 221, 160, 221), // Plum
                Color.FromArgb(100, 255, 255, 224)  // Light yellow
            };

            Color mediaColor = mediaColors[index % mediaColors.Length];
            
            using var mediaBrush = new SolidBrush(mediaColor);
            g.FillRoundedRectangle(mediaBrush, itemRect, 8);

            // Draw media type icon placeholder
            string[] mediaIcons = { "🖼️", "🎥", "📷", "🎬", "🖼️", "📹" };
            string icon = mediaIcons[index % mediaIcons.Length];
            
            using var iconFont = new Font("Segoe UI Emoji", 16, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Theme.ForegroundColor);
            
            var iconSize = g.MeasureString(icon, iconFont);
            float iconX = itemRect.X + (itemRect.Width - iconSize.Width) / 2;
            float iconY = itemRect.Y + (itemRect.Height - iconSize.Height) / 2;
            
            g.DrawString(icon, iconFont, iconBrush, iconX, iconY);
        }

        private void DrawMediaOverlay(Graphics g, Rectangle itemRect, WidgetContext ctx, int index)
        {
            // Draw media info overlay at bottom
            int overlayHeight = 20;
            Rectangle overlayRect = new Rectangle(
                itemRect.X, 
                itemRect.Bottom - overlayHeight, 
                itemRect.Width, 
                overlayHeight
            );

            using var overlayBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            g.FillRectangle(overlayBrush, overlayRect);

            // Draw media title/info
            string[] mediaTitles = { "Photo 1", "Video 1", "Image 1", "Clip 1", "Photo 2", "Video 2" };
            string title = mediaTitles[index % mediaTitles.Length];

            using var titleFont = new Font("Segoe UI", 8, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.White);
            
            g.DrawString(title, titleFont, titleBrush, overlayRect.X + 4, overlayRect.Y + 2);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            // Draw gallery title
            string title = ctx.Title ?? "Media Gallery";
            using var titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme.ForegroundColor);
            
            Rectangle titleRect = new Rectangle(
                ctx.DrawingRect.X + 8,
                ctx.DrawingRect.Y + 8,
                ctx.DrawingRect.Width - 16,
                20
            );
            
            g.DrawString(title, titleFont, titleBrush, titleRect, 
                new StringFormat { Alignment = StringAlignment.Center });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _imagePainter?.Dispose();
                _disposed = true;
            }
        }
    }
}
