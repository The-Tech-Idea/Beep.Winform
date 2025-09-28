using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    internal sealed class MediaViewerPainter : WidgetPainterBase, IDisposable
    {
        private readonly ImagePainter _mediaPainter;
        private bool _disposed = false;

        public MediaViewerPainter()
        {
            _mediaPainter = new ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) 
        {
            ctx.DrawingRect = drawingRect;
            
            // Layout for media viewer with main viewing area and controls
            int padding = 12;
            int controlsHeight = 40;
            
            // Main media viewing area
            ctx.ContentRect = new Rectangle(
                drawingRect.X + padding, 
                drawingRect.Y + padding,
                drawingRect.Width - (padding * 2),
                drawingRect.Height - (padding * 2) - controlsHeight
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            // Dark viewer background for better media contrast
            using var bgBrush = new SolidBrush(Color.FromArgb(20, 20, 20));
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Media viewing area background
            using var mediaBgBrush = new SolidBrush(Color.FromArgb(30, 30, 30));
            using var borderPen = new Pen(Theme.BorderColor, 1);
            g.FillRectangle(mediaBgBrush, ctx.ContentRect);
            g.DrawRectangle(borderPen, ctx.ContentRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            DrawMainMediaView(g, ctx);
            DrawMediaControls(g, ctx);
        }

        private void DrawMainMediaView(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter for main media display
            _mediaPainter.Theme = Theme;
            _mediaPainter.ClippingShape = ImagePainter.ClipShape.Rectangle;
            _mediaPainter.ScalingMode = ImagePainter.ImageScaleMode.KeepAspectRatio;
            
            // Calculate centered media display area
            int margin = 20;
            Rectangle mediaRect = new Rectangle(
                ctx.ContentRect.X + margin,
                ctx.ContentRect.Y + margin,
                ctx.ContentRect.Width - (margin * 2),
                ctx.ContentRect.Height - (margin * 2)
            );

            // Draw main media with ImagePainter
            DrawMainMediaWithImagePainter(g, mediaRect, ctx);
        }

        private void DrawMainMediaWithImagePainter(Graphics g, Rectangle mediaRect, WidgetContext ctx)
        {
            try
            {
                // Try to render with ImagePainter for custom media
                if (ctx.CustomImagePaths != null && ctx.CustomImagePaths.Count > 0)
                {
                    _mediaPainter.DrawImage(g, ctx.CustomImagePaths[0], mediaRect);
                }
                else
                {
                    // Fallback: draw media placeholder
                    DrawMediaViewerPlaceholder(g, mediaRect, ctx);
                }

                // Add media info overlay
                DrawMediaInfoOverlay(g, mediaRect, ctx);
            }
            catch
            {
                // Fallback on any error
                DrawMediaViewerPlaceholder(g, mediaRect, ctx);
            }
        }

        private void DrawMediaViewerPlaceholder(Graphics g, Rectangle mediaRect, WidgetContext ctx)
        {
            // Create a large media placeholder
            using var placeholderBrush = new SolidBrush(Color.FromArgb(60, Theme.AccentColor));
            g.FillRectangle(placeholderBrush, mediaRect);

            // Draw large media icon
            using var iconFont = new Font("Segoe UI Emoji", 48, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Color.FromArgb(150, Theme.ForegroundColor));
            
            string mediaIcon = "🖼️";
            var iconSize = g.MeasureString(mediaIcon, iconFont);
            float iconX = mediaRect.X + (mediaRect.Width - iconSize.Width) / 2;
            float iconY = mediaRect.Y + (mediaRect.Height - iconSize.Height) / 2;
            
            g.DrawString(mediaIcon, iconFont, iconBrush, iconX, iconY);

            // Draw placeholder text
            string placeholderText = "No Media Selected";
            using var textFont = new Font("Segoe UI", 12, FontStyle.Regular);
            using var textBrush = new SolidBrush(Color.FromArgb(150, Theme.ForegroundColor));
            
            var textSize = g.MeasureString(placeholderText, textFont);
            float textX = mediaRect.X + (mediaRect.Width - textSize.Width) / 2;
            float textY = iconY + iconSize.Height + 10;
            
            g.DrawString(placeholderText, textFont, textBrush, textX, textY);
        }

        private void DrawMediaInfoOverlay(Graphics g, Rectangle mediaRect, WidgetContext ctx)
        {
            // Draw semi-transparent info overlay at top
            int overlayHeight = 30;
            Rectangle overlayRect = new Rectangle(
                mediaRect.X, 
                mediaRect.Y, 
                mediaRect.Width, 
                overlayHeight
            );

            using var overlayBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            g.FillRectangle(overlayBrush, overlayRect);

            // Draw media filename/info
            string mediaInfo = ctx.Title ?? "Sample Media File.jpg";
            using var infoFont = new Font("Segoe UI", 9, FontStyle.Regular);
            using var infoBrush = new SolidBrush(Color.White);
            
            g.DrawString(mediaInfo, infoFont, infoBrush, overlayRect.X + 8, overlayRect.Y + 8);
        }

        private void DrawMediaControls(Graphics g, WidgetContext ctx)
        {
            // Draw media controls at bottom
            int controlsY = ctx.ContentRect.Bottom + 8;
            int controlsHeight = 32;
            Rectangle controlsRect = new Rectangle(
                ctx.DrawingRect.X + 12,
                controlsY,
                ctx.DrawingRect.Width - 24,
                controlsHeight
            );

            // Controls background
            using var controlsBgBrush = new SolidBrush(Color.FromArgb(40, 40, 40));
            g.FillRoundedRectangle(controlsBgBrush, controlsRect, 4);

            // Draw control buttons
            DrawControlButtons(g, controlsRect, ctx);
        }

        private void DrawControlButtons(Graphics g, Rectangle controlsRect, WidgetContext ctx)
        {
            int buttonSize = 24;
            int buttonSpacing = 8;
            int startX = controlsRect.X + (controlsRect.Width - (buttonSize * 5 + buttonSpacing * 4)) / 2;
            int buttonY = controlsRect.Y + (controlsRect.Height - buttonSize) / 2;

            string[] controlIcons = { "⏮️", "⏸️", "▶️", "⏭️", "🔄" };
            
            for (int i = 0; i < controlIcons.Length; i++)
            {
                int buttonX = startX + i * (buttonSize + buttonSpacing);
                Rectangle buttonRect = new Rectangle(buttonX, buttonY, buttonSize, buttonSize);

                // Button background
                using var buttonBrush = new SolidBrush(Color.FromArgb(60, Theme.AccentColor));
                g.FillRoundedRectangle(buttonBrush, buttonRect, 4);

                // Button icon
                using var iconFont = new Font("Segoe UI Emoji", 12, FontStyle.Regular);
                using var iconBrush = new SolidBrush(Color.White);
                
                var iconSize = g.MeasureString(controlIcons[i], iconFont);
                float iconX = buttonRect.X + (buttonRect.Width - iconSize.Width) / 2;
                float iconY = buttonRect.Y + (buttonRect.Height - iconSize.Height) / 2;
                
                g.DrawString(controlIcons[i], iconFont, iconBrush, iconX, iconY);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            // Draw viewer title if needed
            if (!string.IsNullOrEmpty(ctx.Subtitle))
            {
                using var titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.White);
                
                g.DrawString(ctx.Subtitle, titleFont, titleBrush, 
                    ctx.DrawingRect.X + 12, ctx.DrawingRect.Y + 4);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _mediaPainter?.Dispose();
                _disposed = true;
            }
        }
    }
}
