using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// ImageCard - Card with background image and overlay text (now with hit areas for image and overlay)
    /// </summary>
    internal sealed class ImageCardPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _imagePainter;
        private Rectangle _imageRectCache;
        private Rectangle _overlayRectCache;

        public ImageCardPainter()
        {
            _imagePainter = new ImagePainter();
            _imagePainter.ScaleMode = Vis.Modules.ImageScaleMode.Fill;
            _imagePainter.ClipShape = Vis.Modules.ImageClipShape.RoundedRect;
            _imagePainter.CornerRadius = 8f;
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Full card is the image area
            ctx.ContentRect = Rectangle.Inflate(ctx.DrawingRect, -pad, -pad);
            _imageRectCache = ctx.ContentRect;

            // Overlay area at bottom if enabled
            if (ctx.ShowIcon && ctx.ShowImageOverlay)
            {
                ctx.FooterRect = new Rectangle(
                    ctx.ContentRect.Left,
                    ctx.ContentRect.Bottom - 40,
                    ctx.ContentRect.Width,
                    40
                );
                _overlayRectCache = ctx.FooterRect;
            }
            else
            {
                _overlayRectCache = Rectangle.Empty;
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);

            var image = ctx.Image;
            var imagePath = ctx.ImagePath;
            
            if (image != null || !string.IsNullOrEmpty(imagePath))
            {
                _imagePainter.CurrentTheme = Theme;
                _imagePainter.CornerRadius = ctx.CornerRadius;
                _imagePainter.ScaleMode = Vis.Modules.ImageScaleMode.Fill;
                _imagePainter.ClipShape = Vis.Modules.ImageClipShape.RoundedRect;
                if (!string.IsNullOrEmpty(imagePath))
                {
                    try
                    {
                        using var roundedClip = PathPainterHelpers.CreateRoundedRectangle(ctx.DrawingRect, (int)ctx.CornerRadius);
                        g.SetClip(roundedClip);
                        StyledImagePainter.Paint(g, ctx.DrawingRect, imagePath);
                        g.ResetClip();
                    }
                    catch
                    {
                        _imagePainter.ImagePath = imagePath;
                        _imagePainter.DrawImage(g, ctx.DrawingRect);
                    }
                }
                else
                {
                    // Draw directly from Image instance
                    try
                    {
                        using var roundedClip = PathPainterHelpers.CreateRoundedRectangle(ctx.DrawingRect, (int)ctx.CornerRadius);
                        g.SetClip(roundedClip);
                        g.DrawImage(image, ctx.DrawingRect);
                        g.ResetClip();
                    }
                    catch
                    {
                        _imagePainter.Image = image;
                        _imagePainter.DrawImage(g, ctx.DrawingRect);
                    }
                }
            }
            else
            {
                using var bgBrush = new SolidBrush(Color.FromArgb(30, ctx.AccentColor));
                using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
                g.FillPath(bgBrush, bgPath);
            }
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            bool showOverlay = ctx.ShowImageOverlay;
            if (showOverlay && !_overlayRectCache.IsEmpty)
            {
                using var overlayBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                using var overlayPath = CreateRoundedPath(_overlayRectCache, 0);
                g.FillPath(overlayBrush, overlayPath);
                string overlayText = !string.IsNullOrEmpty(ctx.OverlayText) ? ctx.OverlayText : ctx.Title;
                using var overlayFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var overlayTextBrush = new SolidBrush(Color.White);
                var textRect = Rectangle.Inflate(_overlayRectCache, -12, -8);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(overlayText, overlayFont, overlayTextBrush, textRect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Hover cues
            if (IsAreaHovered("ImageCard_Image"))
            {
                using var pen = new Pen(Color.FromArgb(120, Theme?.PrimaryColor ?? Color.Blue), 2);
                using var path = CreateRoundedPath(_imageRectCache, 8);
                g.DrawPath(pen, path);
            }
            if (!_overlayRectCache.IsEmpty && IsAreaHovered("ImageCard_Overlay"))
            {
                using var glow = new SolidBrush(Color.FromArgb(60, Color.Black));
                g.FillRectangle(glow, _overlayRectCache);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            if (!_imageRectCache.IsEmpty)
            {
                owner.AddHitArea("ImageCard_Image", _imageRectCache, null, () =>
                {
                    ctx.ImageClicked = true;
                    notifyAreaHit?.Invoke("ImageCard_Image", _imageRectCache);
                    Owner?.Invalidate();
                });
            }
            if (!_overlayRectCache.IsEmpty)
            {
                owner.AddHitArea("ImageCard_Overlay", _overlayRectCache, null, () =>
                {
                    ctx.OverlayClicked = true;
                    notifyAreaHit?.Invoke("ImageCard_Overlay", _overlayRectCache);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}
