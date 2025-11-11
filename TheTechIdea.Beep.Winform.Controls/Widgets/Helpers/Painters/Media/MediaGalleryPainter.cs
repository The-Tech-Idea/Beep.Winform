using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// MediaGallery - grid of images with selection and hover
    /// </summary>
    internal sealed class MediaGalleryPainter : WidgetPainterBase, IDisposable
    {
        private readonly ImagePainter _imagePainter;
        private bool _disposed = false;
        private readonly List<Rectangle> _itemRects = new();

        public MediaGalleryPainter()
        {
            _imagePainter = new ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) 
        {
            ctx.DrawingRect = drawingRect;
            int padding = 8;
            int galleryWidth = drawingRect.Width - (padding * 2);
            int galleryHeight = drawingRect.Height - (padding * 2);
            ctx.ContentRect = new Rectangle(drawingRect.X + padding, drawingRect.Y + padding, galleryWidth, galleryHeight);

            // Precompute grid rects
            _itemRects.Clear();
            int cols = 3, rows = 2, itemSpacing = 6;
            int itemWidth = (ctx.ContentRect.Width - (itemSpacing * (cols + 1))) / cols;
            int itemHeight = (ctx.ContentRect.Height - (itemSpacing * (rows + 1))) / rows;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = ctx.ContentRect.X + itemSpacing + col * (itemWidth + itemSpacing);
                    int y = ctx.ContentRect.Y + itemSpacing + row * (itemHeight + itemSpacing);
                    _itemRects.Add(new Rectangle(x, y, itemWidth, itemHeight));
                }
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;
            using var bgBrush = new SolidBrush(Theme.BackgroundColor);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
            using var patternBrush = new SolidBrush(Color.FromArgb(10, Theme.AccentColor));
            using var pen = new Pen(Theme.BorderColor, 1);
            g.FillRectangle(patternBrush, ctx.ContentRect);
            g.DrawRectangle(pen, ctx.ContentRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;

            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ClipShape = Vis.Modules.ImageClipShape.RoundedRect;
            _imagePainter.ScaleMode = Vis.Modules.ImageScaleMode.Fill;
            _imagePainter.CornerRadius = 8;

            for (int i = 0; i < _itemRects.Count; i++)
            {
                var itemRect = _itemRects[i];
                bool drawn = false;
                if (ctx.CustomImagePaths != null && i < ctx.CustomImagePaths.Count)
                {
                    try { _imagePainter.DrawImage(g, ctx.CustomImagePaths[i], itemRect); drawn = true; } catch { }
                }
                if (!drawn)
                {
                    DrawMediaPlaceholder(g, itemRect, ctx, i);
                }

                // Hover overlay
                if (IsAreaHovered($"MediaGallery_Item_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(60, Theme.AccentColor));
                    g.FillRoundedRectangle(hover, itemRect, 8);
                }

                // Bottom info overlay
                DrawMediaOverlay(g, itemRect, ctx, i);
            }
        }

        private void DrawMediaPlaceholder(Graphics g, Rectangle itemRect, WidgetContext ctx, int index)
        {
            Color[] mediaColors =
            {
                Color.FromArgb(100, 255, 182, 193),
                Color.FromArgb(100, 173, 216, 230),
                Color.FromArgb(100, 144, 238, 144),
                Color.FromArgb(100, 255, 218, 185),
                Color.FromArgb(100, 221, 160, 221),
                Color.FromArgb(100, 255, 255, 224)
            };
            using var mediaBrush = new SolidBrush(mediaColors[index % mediaColors.Length]);
            g.FillRoundedRectangle(mediaBrush, itemRect, 8);
            string[] mediaIcons = { "🖼️", "🎥", "📷", "🎬", "🖼️", "📹" };
            string icon = mediaIcons[index % mediaIcons.Length];
            using var iconFont = new Font("Segoe UI Emoji", 16, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Theme.ForeColor);
            var iconSize = TextUtils.MeasureText(g,icon, iconFont);
            g.DrawString(icon, iconFont, iconBrush, itemRect.X + (itemRect.Width - iconSize.Width) / 2, itemRect.Y + (itemRect.Height - iconSize.Height) / 2);
        }

        private void DrawMediaOverlay(Graphics g, Rectangle itemRect, WidgetContext ctx, int index)
        {
            int overlayHeight = 20;
            Rectangle overlayRect = new Rectangle(itemRect.X, itemRect.Bottom - overlayHeight, itemRect.Width, overlayHeight);
            using var overlayBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            g.FillRectangle(overlayBrush, overlayRect);
            string[] mediaTitles = { "Photo 1", "Video 1", "Image 1", "Clip 1", "Photo 2", "Video 2" };
            string title = mediaTitles[index % mediaTitles.Length];
            using var titleFont = new Font("Segoe UI", 8, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.White);
            g.DrawString(title, titleFont, titleBrush, overlayRect.X + 4, overlayRect.Y + 2);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = i;
                var rect = _itemRects[i];
                owner.AddHitArea($"MediaGallery_Item_{idx}", rect, null, () =>
                {
                    ctx.SelectedMediaIndex = idx;
                    notifyAreaHit?.Invoke($"MediaGallery_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;
            // Title centered at top handled elsewhere; selection accents drawn via hover
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
