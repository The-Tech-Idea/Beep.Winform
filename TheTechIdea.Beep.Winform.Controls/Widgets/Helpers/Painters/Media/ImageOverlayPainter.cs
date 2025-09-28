using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    internal sealed class ImageOverlayPainter : WidgetPainterBase, IDisposable
    {
        private readonly ImagePainter _imagePainter;
        private bool _disposed = false;

        public ImageOverlayPainter()
        {
            _imagePainter = new ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) 
        {
            ctx.DrawingRect = drawingRect;
            
            // Full area for image with overlay
            ctx.ContentRect = new Rectangle(
                drawingRect.X + 4, 
                drawingRect.Y + 4,
                drawingRect.Width - 8,
                drawingRect.Height - 8
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            using var bgBrush = new SolidBrush(Theme.BackgroundColor);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            DrawImageWithOverlay(g, ctx);
        }

        private void DrawImageWithOverlay(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter for overlay display
            _imagePainter.Theme = Theme;
            _imagePainter.ClippingShape = ImagePainter.ClipShape.RoundedRect;
            _imagePainter.ScalingMode = ImagePainter.ImageScaleMode.Fill;
            _imagePainter.CornerRadius = 8;

            // Draw main image with ImagePainter
            DrawMainImageWithImagePainter(g, ctx.ContentRect, ctx);

            // Add overlay elements
            DrawOverlayElements(g, ctx.ContentRect, ctx);
        }

        private void DrawMainImageWithImagePainter(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            try
            {
                // Try to render with ImagePainter for custom image
                if (ctx.CustomImagePaths != null && ctx.CustomImagePaths.Count > 0)
                {
                    _imagePainter.DrawImage(g, ctx.CustomImagePaths[0], imageRect);
                }
                else
                {
                    // Fallback: draw image placeholder
                    DrawImagePlaceholder(g, imageRect, ctx);
                }
            }
            catch
            {
                // Fallback on any error
                DrawImagePlaceholder(g, imageRect, ctx);
            }
        }

        private void DrawImagePlaceholder(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            // Create gradient background placeholder
            using var gradientBrush = new LinearGradientBrush(
                imageRect,
                Color.FromArgb(100, Theme.AccentColor),
                Color.FromArgb(50, Theme.AccentColor),
                LinearGradientMode.Vertical
            );
            
            g.FillRoundedRectangle(gradientBrush, imageRect, 8);

            // Draw placeholder image icon
            using var iconFont = new Font("Segoe UI Emoji", 32, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Color.FromArgb(120, Theme.ForegroundColor));
            
            string imageIcon = "🖼️";
            var iconSize = g.MeasureString(imageIcon, iconFont);
            float iconX = imageRect.X + (imageRect.Width - iconSize.Width) / 2;
            float iconY = imageRect.Y + (imageRect.Height - iconSize.Height) / 2;
            
            g.DrawString(imageIcon, iconFont, iconBrush, iconX, iconY);
        }

        private void DrawOverlayElements(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            // Draw top overlay with title/info
            DrawTopOverlay(g, imageRect, ctx);

            // Draw bottom overlay with actions
            DrawBottomOverlay(g, imageRect, ctx);

            // Draw corner badges/indicators
            DrawCornerBadges(g, imageRect, ctx);

            // Draw center action buttons (optional)
            DrawCenterActions(g, imageRect, ctx);
        }

        private void DrawTopOverlay(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            int overlayHeight = 40;
            Rectangle topOverlay = new Rectangle(
                imageRect.X, 
                imageRect.Y, 
                imageRect.Width, 
                overlayHeight
            );

            // Gradient overlay background
            using var overlayBrush = new LinearGradientBrush(
                topOverlay,
                Color.FromArgb(180, Color.Black),
                Color.FromArgb(0, Color.Black),
                LinearGradientMode.Vertical
            );
            g.FillRectangle(overlayBrush, topOverlay);

            // Draw title text
            string title = ctx.Title ?? "Image Overlay";
            using var titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.White);
            
            g.DrawString(title, titleFont, titleBrush, topOverlay.X + 12, topOverlay.Y + 8);

            // Draw close/menu button
            DrawOverlayButton(g, new Rectangle(topOverlay.Right - 30, topOverlay.Y + 5, 20, 20), "✕", ctx);
        }

        private void DrawBottomOverlay(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            int overlayHeight = 50;
            Rectangle bottomOverlay = new Rectangle(
                imageRect.X, 
                imageRect.Bottom - overlayHeight, 
                imageRect.Width, 
                overlayHeight
            );

            // Gradient overlay background
            using var overlayBrush = new LinearGradientBrush(
                bottomOverlay,
                Color.FromArgb(0, Color.Black),
                Color.FromArgb(180, Color.Black),
                LinearGradientMode.Vertical
            );
            g.FillRectangle(overlayBrush, bottomOverlay);

            // Draw action buttons
            int buttonSize = 30;
            int buttonSpacing = 10;
            int startX = bottomOverlay.X + 12;
            int buttonY = bottomOverlay.Y + (bottomOverlay.Height - buttonSize) / 2;

            string[] actionIcons = { "❤️", "💬", "📤", "⋯" };
            
            for (int i = 0; i < actionIcons.Length; i++)
            {
                int buttonX = startX + i * (buttonSize + buttonSpacing);
                Rectangle buttonRect = new Rectangle(buttonX, buttonY, buttonSize, buttonSize);
                
                DrawOverlayButton(g, buttonRect, actionIcons[i], ctx);
            }

            // Draw image info text
            string info = ctx.Subtitle ?? "Image details";
            using var infoFont = new Font("Segoe UI", 8, FontStyle.Regular);
            using var infoBrush = new SolidBrush(Color.FromArgb(200, Color.White));
            
            Rectangle infoRect = new Rectangle(
                bottomOverlay.Right - 150, 
                bottomOverlay.Y + 5, 
                140, 
                bottomOverlay.Height - 10
            );
            
            g.DrawString(info, infoFont, infoBrush, infoRect, 
                new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
        }

        private void DrawCornerBadges(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            // Top-right corner badge (e.g., favorite, type indicator)
            Rectangle badgeRect = new Rectangle(
                imageRect.Right - 25, 
                imageRect.Y + 5, 
                20, 
                20
            );

            using var badgeBrush = new SolidBrush(Color.FromArgb(200, Theme.AccentColor));
            g.FillEllipse(badgeBrush, badgeRect);

            using var badgeFont = new Font("Segoe UI Emoji", 10, FontStyle.Regular);
            using var badgeTextBrush = new SolidBrush(Color.White);
            
            string badgeIcon = "⭐";
            var iconSize = g.MeasureString(badgeIcon, badgeFont);
            float iconX = badgeRect.X + (badgeRect.Width - iconSize.Width) / 2;
            float iconY = badgeRect.Y + (badgeRect.Height - iconSize.Height) / 2;
            
            g.DrawString(badgeIcon, badgeFont, badgeTextBrush, iconX, iconY);
        }

        private void DrawCenterActions(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            // Optional center play/action button
            int centerButtonSize = 50;
            Rectangle centerButton = new Rectangle(
                imageRect.X + (imageRect.Width - centerButtonSize) / 2,
                imageRect.Y + (imageRect.Height - centerButtonSize) / 2,
                centerButtonSize,
                centerButtonSize
            );

            using var centerBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            g.FillEllipse(centerBrush, centerButton);

            using var centerBorder = new Pen(Color.White, 2);
            g.DrawEllipse(centerBorder, centerButton);

            // Draw play icon
            using var playFont = new Font("Segoe UI Emoji", 20, FontStyle.Regular);
            using var playBrush = new SolidBrush(Color.White);
            
            string playIcon = "▶️";
            var playSize = g.MeasureString(playIcon, playFont);
            float playX = centerButton.X + (centerButton.Width - playSize.Width) / 2;
            float playY = centerButton.Y + (centerButton.Height - playSize.Height) / 2;
            
            g.DrawString(playIcon, playFont, playBrush, playX, playY);
        }

        private void DrawOverlayButton(Graphics g, Rectangle buttonRect, string icon, WidgetContext ctx)
        {
            // Button background
            using var buttonBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            g.FillRoundedRectangle(buttonBrush, buttonRect, 4);

            // Button border
            using var buttonBorder = new Pen(Color.FromArgb(100, Color.White), 1);
            g.DrawRoundedRectangle(buttonBorder, buttonRect, 4);

            // Button icon
            using var iconFont = new Font("Segoe UI Emoji", 12, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Color.White);
            
            var iconSize = g.MeasureString(icon, iconFont);
            float iconX = buttonRect.X + (buttonRect.Width - iconSize.Width) / 2;
            float iconY = buttonRect.Y + (buttonRect.Height - iconSize.Height) / 2;
            
            g.DrawString(icon, iconFont, iconBrush, iconX, iconY);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (g == null || ctx?.Theme == null) return;

            // Additional decorative elements if needed
            using var accentPen = new Pen(Theme.AccentColor, 2);
            g.DrawRoundedRectangle(accentPen, ctx.ContentRect, 8);
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
