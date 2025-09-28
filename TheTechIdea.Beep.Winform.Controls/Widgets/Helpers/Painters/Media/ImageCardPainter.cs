using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// ImageCard - Card with background image and overlay text
    /// </summary>
    internal sealed class ImageCardPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _imagePainter;

        public ImageCardPainter()
        {
            _imagePainter = new ImagePainter();
            _imagePainter.ScaleMode = ImageScaleMode.Fill;
            _imagePainter.ClipShape = ImageClipShape.RoundedRect;
            _imagePainter.CornerRadius = 8f;
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Full card is the image area
            ctx.ContentRect = Rectangle.Inflate(ctx.DrawingRect, -pad, -pad);

            // Overlay area at bottom if enabled
            if (ctx.ShowIcon && ctx.CustomData.ContainsKey("ShowOverlay") && (bool)ctx.CustomData["ShowOverlay"])
            {
                ctx.FooterRect = new Rectangle(
                    ctx.ContentRect.Left,
                    ctx.ContentRect.Bottom - 40,
                    ctx.ContentRect.Width,
                    40
                );
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);

            // Draw image background if available
            var image = ctx.CustomData.ContainsKey("Image") ? ctx.CustomData["Image"] as Image : null;
            var imagePath = ctx.CustomData.ContainsKey("ImagePath") ? ctx.CustomData["ImagePath"]?.ToString() : null;
            
            if (image != null || !string.IsNullOrEmpty(imagePath))
            {
                // Configure ImagePainter
                _imagePainter.CurrentTheme = Theme;
                _imagePainter.CornerRadius = ctx.CornerRadius;
                _imagePainter.ScaleMode = ImageScaleMode.Fill;
                _imagePainter.ClipShape = ImageClipShape.RoundedRect;
                
                // Set image source
                if (!string.IsNullOrEmpty(imagePath))
                {
                    _imagePainter.ImagePath = imagePath;
                }
                else if (image != null)
                {
                    _imagePainter.Image = image;
                }

                // Draw with ImagePainter
                _imagePainter.DrawImage(g, ctx.DrawingRect);
            }
            else
            {
                // Fallback gradient background
                using var bgBrush = new SolidBrush(Color.FromArgb(30, ctx.AccentColor));
                using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
                g.FillPath(bgBrush, bgPath);
            }
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw overlay if enabled
            bool showOverlay = ctx.CustomData.ContainsKey("ShowOverlay") && (bool)ctx.CustomData["ShowOverlay"];
            if (showOverlay && !ctx.FooterRect.IsEmpty)
            {
                // Semi-transparent overlay background
                using var overlayBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                using var overlayPath = CreateRoundedPath(ctx.FooterRect, 0);
                g.FillPath(overlayBrush, overlayPath);

                // Overlay text
                string overlayText = ctx.CustomData.ContainsKey("OverlayText") ?
                    ctx.CustomData["OverlayText"].ToString() : ctx.Title;

                using var overlayFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var overlayTextBrush = new SolidBrush(Color.White);

                var textRect = Rectangle.Inflate(ctx.FooterRect, -12, -8);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(overlayText, overlayFont, overlayTextBrush, textRect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw corner accents or badges
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}
