using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// ImageOverlay - full overlay UI with interactive buttons/badges
    /// </summary>
    internal sealed class ImageOverlayPainter : WidgetPainterBase, IDisposable
    {
        private readonly ImagePainter _imagePainter;
        private bool _disposed = false;
        private Rectangle _topCloseRect;
        private readonly List<(Rectangle rect, string id)> _bottomButtons = new();

        public ImageOverlayPainter()
        {
            _imagePainter = new ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) 
        {
            ctx.DrawingRect = drawingRect;
            ctx.ContentRect = new Rectangle(drawingRect.X + 4, drawingRect.Y + 4, drawingRect.Width - 8, drawingRect.Height - 8);

            // Precompute interactive rects
            _topCloseRect = new Rectangle(ctx.ContentRect.Right - 30, ctx.ContentRect.Y + 10, 20, 20);
            _bottomButtons.Clear();
            int overlayHeight = 50;
            int buttonSize = 30;
            int buttonSpacing = 10;
            int startX = ctx.ContentRect.X + 12;
            int buttonY = ctx.ContentRect.Bottom - overlayHeight + (overlayHeight - buttonSize) / 2;
            string[] actionIds = { "like", "comment", "share", "more" };
            for (int i = 0; i < actionIds.Length; i++)
            {
                int buttonX = startX + i * (buttonSize + buttonSpacing);
                _bottomButtons.Add((new Rectangle(buttonX, buttonY, buttonSize, buttonSize), actionIds[i]));
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;
            using var bgBrush = new SolidBrush(Theme.BackgroundColor);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;
            DrawImageWithOverlay(g, ctx);
        }

        private void DrawImageWithOverlay(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ClipShape = Vis.Modules.ImageClipShape.RoundedRect;
            _imagePainter.ScaleMode = Vis.Modules.ImageScaleMode.Fill;
            _imagePainter.CornerRadius = 8;
            DrawMainImageWithImagePainter(g, ctx.ContentRect, ctx);
            DrawOverlayElements(g, ctx.ContentRect, ctx);
        }

        private void DrawMainImageWithImagePainter(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            try
            {
                if (ctx.CustomImagePaths != null && ctx.CustomImagePaths.Count > 0)
                {
                    _imagePainter.DrawImage(g, ctx.CustomImagePaths[0], imageRect);
                }
                else
                {
                    DrawImagePlaceholder(g, imageRect, ctx);
                }
            }
            catch { DrawImagePlaceholder(g, imageRect, ctx); }
        }

        private void DrawImagePlaceholder(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            using var gradientBrush = new LinearGradientBrush(imageRect, Color.FromArgb(100, Theme.AccentColor), Color.FromArgb(50, Theme.AccentColor), LinearGradientMode.Vertical);
            g.FillRoundedRectangle(gradientBrush, imageRect, 8);
            using var iconFont = new Font("Segoe UI Emoji", 32, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Color.FromArgb(120, Theme.ForeColor));
            string imageIcon = "🖼️";
            var iconSize = TextUtils.MeasureText(g,imageIcon, iconFont);
            g.DrawString(imageIcon, iconFont, iconBrush, imageRect.X + (imageRect.Width - iconSize.Width) / 2, imageRect.Y + (imageRect.Height - iconSize.Height) / 2);
        }

        private void DrawOverlayElements(Graphics g, Rectangle imageRect, WidgetContext ctx)
        {
            // Top overlay + close
            int topH = 40;
            Rectangle topOverlay = new Rectangle(imageRect.X, imageRect.Y, imageRect.Width, topH);
            using (var overlayBrush = new LinearGradientBrush(topOverlay, Color.FromArgb(180, Color.Black), Color.FromArgb(0, Color.Black), LinearGradientMode.Vertical))
                g.FillRectangle(overlayBrush, topOverlay);
            using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.White))
                g.DrawString(ctx.Title ?? "Image Overlay", titleFont, titleBrush, topOverlay.X + 12, topOverlay.Y + 8);
            DrawOverlayButton(g, _topCloseRect, "✕", ctx, IsAreaHovered("ImageOverlay_Close"));

            // Bottom overlay + buttons
            int overlayHeight = 50;
            Rectangle bottomOverlay = new Rectangle(imageRect.X, imageRect.Bottom - overlayHeight, imageRect.Width, overlayHeight);
            using (var overlayBrush = new LinearGradientBrush(bottomOverlay, Color.FromArgb(0, Color.Black), Color.FromArgb(180, Color.Black), LinearGradientMode.Vertical))
                g.FillRectangle(overlayBrush, bottomOverlay);

            foreach (var (rect, id) in _bottomButtons)
            {
                bool hv = IsAreaHovered($"ImageOverlay_Action_{id}");
                string emoji = id switch { "like" => "❤️", "comment" => "💬", "share" => "📤", _ => "⋯" };
                DrawOverlayButton(g, rect, emoji, ctx, hv);
            }
        }

        private void DrawOverlayButton(Graphics g, Rectangle buttonRect, string icon, WidgetContext ctx, bool hovered)
        {
            using var buttonBrush = new SolidBrush(Color.FromArgb(hovered ? 160 : 120, Color.Black));
            g.FillRoundedRectangle(buttonBrush, buttonRect, 4);
            using var buttonBorder = new Pen(Color.FromArgb(100, Color.White), 1);
            g.DrawRoundedRectangle(buttonBorder, buttonRect, 4);
            using var iconFont = new Font("Segoe UI Emoji", 12, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Color.White);
            var iconSize = TextUtils.MeasureText(g,icon, iconFont);
            g.DrawString(icon, iconFont, iconBrush, buttonRect.X + (buttonRect.Width - iconSize.Width) / 2, buttonRect.Y + (buttonRect.Height - iconSize.Height) / 2);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;
            using var accentPen = new Pen(Theme.AccentColor, 2);
            g.DrawRoundedRectangle(accentPen, ctx.ContentRect, 8);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            owner.AddHitArea("ImageOverlay_Close", _topCloseRect, null, () =>
            {
                ctx.CustomData["OverlayClosed"] = true;
                notifyAreaHit?.Invoke("ImageOverlay_Close", _topCloseRect);
                Owner?.Invalidate();
            });
            foreach (var (rect, id) in _bottomButtons)
            {
                owner.AddHitArea($"ImageOverlay_Action_{id}", rect, null, () =>
                {
                    ctx.CustomData[$"OverlayAction"] = id;
                    notifyAreaHit?.Invoke($"ImageOverlay_Action_{id}", rect);
                    Owner?.Invalidate();
                });
            }
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
