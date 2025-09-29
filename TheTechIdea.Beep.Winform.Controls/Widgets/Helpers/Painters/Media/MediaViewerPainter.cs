using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// MediaViewer - main media area with control buttons (interactive)
    /// </summary>
    internal sealed class MediaViewerPainter : WidgetPainterBase, IDisposable
    {
        private readonly ImagePainter _mediaPainter;
        private bool _disposed = false;
        private readonly List<(Rectangle rect, string id)> _controls = new();
        private Rectangle _controlsRectCache;
        private Rectangle _mediaRectCache;
        private Rectangle _infoRectCache;
        private Rectangle _fullRectCache;

        public MediaViewerPainter()
        {
            _mediaPainter = new ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) 
        {
            ctx.DrawingRect = drawingRect;
            int padding = 12;
            int controlsHeight = 40;
            ctx.ContentRect = new Rectangle(drawingRect.X + padding, drawingRect.Y + padding, drawingRect.Width - (padding * 2), drawingRect.Height - (padding * 2) - controlsHeight);

            // Precompute control buttons
            _controls.Clear();
            _controlsRectCache = new Rectangle(ctx.DrawingRect.X + 12, ctx.ContentRect.Bottom + 8, ctx.DrawingRect.Width - 24, 32);
            int buttonSize = 24;
            int buttonSpacing = 8;
            string[] ids = { "prev", "pause", "play", "next", "refresh" };
            int startX = _controlsRectCache.X + (_controlsRectCache.Width - (buttonSize * ids.Length + buttonSpacing * (ids.Length - 1))) / 2;
            int buttonY = _controlsRectCache.Y + (_controlsRectCache.Height - buttonSize) / 2;
            for (int i = 0; i < ids.Length; i++)
            {
                int buttonX = startX + i * (buttonSize + buttonSpacing);
                _controls.Add((new Rectangle(buttonX, buttonY, buttonSize, buttonSize), ids[i]));
            }

            // Media rect cache and top overlay utilities (info / fullscreen)
            int margin = 20;
            _mediaRectCache = new Rectangle(ctx.ContentRect.X + margin, ctx.ContentRect.Y + margin, ctx.ContentRect.Width - (margin * 2), ctx.ContentRect.Height - (margin * 2));
            int overlayBtn = 22;
            _fullRectCache = new Rectangle(_mediaRectCache.Right - overlayBtn, _mediaRectCache.Top + 6, overlayBtn, overlayBtn);
            _infoRectCache = new Rectangle(_fullRectCache.Left - overlayBtn - 6, _mediaRectCache.Top + 6, overlayBtn, overlayBtn);

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;
            using var bgBrush = new SolidBrush(Color.FromArgb(20, 20, 20));
            g.FillRectangle(bgBrush, ctx.DrawingRect);
            using var mediaBgBrush = new SolidBrush(Color.FromArgb(30, 30, 30));
            using var borderPen = new Pen(Theme.BorderColor, 1);
            g.FillRectangle(mediaBgBrush, ctx.ContentRect);
            g.DrawRectangle(borderPen, ctx.ContentRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (g == null || Theme == null) return;
            DrawMainMediaView(g, ctx);
            DrawMediaControls(g, ctx);
        }

        private void DrawMainMediaView(Graphics g, WidgetContext ctx)
        {
            _mediaPainter.CurrentTheme = Theme;
            _mediaPainter.ClipShape = Vis.Modules.ImageClipShape.RoundedRect;
            _mediaPainter.ScaleMode = Vis.Modules.ImageScaleMode.Fill;
            // _mediaRectCache computed in AdjustLayout
            DrawMainMediaWithImagePainter(g, _mediaRectCache, ctx);
            DrawMediaInfoOverlay(g, _mediaRectCache, ctx);
        }

        private void DrawMainMediaWithImagePainter(Graphics g, Rectangle mediaRect, WidgetContext ctx)
        {
            try
            {
                if (ctx.CustomImagePaths != null && ctx.CustomImagePaths.Count > 0)
                {
                    _mediaPainter.DrawImage(g, ctx.CustomImagePaths[0], mediaRect);
                }
                else
                {
                    DrawMediaViewerPlaceholder(g, mediaRect, ctx);
                }
            }
            catch { DrawMediaViewerPlaceholder(g, mediaRect, ctx); }
        }

        private void DrawMediaViewerPlaceholder(Graphics g, Rectangle mediaRect, WidgetContext ctx)
        {
            using var placeholderBrush = new SolidBrush(Color.FromArgb(60, Theme.AccentColor));
            g.FillRectangle(placeholderBrush, mediaRect);
            using var iconFont = new Font("Segoe UI Emoji", 48, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Color.FromArgb(150, Theme.ForeColor));
            string mediaIcon = "🖼️";
            var iconSize = g.MeasureString(mediaIcon, iconFont);
            g.DrawString(mediaIcon, iconFont, iconBrush, mediaRect.X + (mediaRect.Width - iconSize.Width) / 2, mediaRect.Y + (mediaRect.Height - iconSize.Height) / 2);
            string placeholderText = "No Media Selected";
            using var textFont = new Font("Segoe UI", 12, FontStyle.Regular);
            using var textBrush = new SolidBrush(Color.FromArgb(150, Theme.ForeColor));
            var textSize = g.MeasureString(placeholderText, textFont);
            g.DrawString(placeholderText, textFont, textBrush, mediaRect.X + (mediaRect.Width - textSize.Width) / 2, mediaRect.Y + (mediaRect.Height - textSize.Height) / 2 + iconSize.Height + 10);
        }

        private void DrawMediaInfoOverlay(Graphics g, Rectangle mediaRect, WidgetContext ctx)
        {
            int overlayHeight = 30;
            Rectangle overlayRect = new Rectangle(mediaRect.X, mediaRect.Y, mediaRect.Width, overlayHeight);
            using var overlayBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            g.FillRectangle(overlayBrush, overlayRect);
            string mediaInfo = ctx.Title ?? "Sample Media File.jpg";
            using var infoFont = new Font("Segoe UI", 9, FontStyle.Regular);
            using var infoBrush = new SolidBrush(Color.White);
            g.DrawString(mediaInfo, infoFont, infoBrush, overlayRect.X + 8, overlayRect.Y + 8);
        }

        private void DrawMediaControls(Graphics g, WidgetContext ctx)
        {
            using var controlsBgBrush = new SolidBrush(Color.FromArgb(40, 40, 40));
            g.FillRoundedRectangle(controlsBgBrush, _controlsRectCache, 4);
            foreach (var (rect, id) in _controls)
            {
                bool hv = IsAreaHovered($"MediaViewer_{id}");
                DrawControlButton(g, rect, id, hv);
            }
        }

        private void DrawControlButton(Graphics g, Rectangle rect, string id, bool hovered)
        {
            using var buttonBrush = new SolidBrush(Color.FromArgb(hovered ? 120 : 60, Theme.AccentColor));
            g.FillRoundedRectangle(buttonBrush, rect, 4);
            using var iconFont = new Font("Segoe UI Emoji", 12, FontStyle.Regular);
            using var iconBrush = new SolidBrush(Color.White);
            string icon = id switch { "prev" => "⏮️", "pause" => "⏸️", "play" => "▶️", "next" => "⏭️", _ => "🔄" };
            var iconSize = g.MeasureString(icon, iconFont);
            g.DrawString(icon, iconFont, iconBrush, rect.X + (rect.Width - iconSize.Width) / 2, rect.Y + (rect.Height - iconSize.Height) / 2);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            foreach (var (rect, id) in _controls)
            {
                owner.AddHitArea($"MediaViewer_{id}", rect, null, () =>
                {
                    ctx.CustomData[$"MediaViewerAction"] = id;
                    notifyAreaHit?.Invoke($"MediaViewer_{id}", rect);
                    Owner?.Invalidate();
                });
            }

            // Click on media toggles play/pause
            if (!_mediaRectCache.IsEmpty)
            {
                owner.AddHitArea("MediaViewer_Media", _mediaRectCache, null, () =>
                {
                    string current = ctx.CustomData.ContainsKey("MediaViewerAction") ? ctx.CustomData["MediaViewerAction"].ToString() : "play";
                    ctx.CustomData["MediaViewerAction"] = current == "play" ? "pause" : "play";
                    notifyAreaHit?.Invoke("MediaViewer_Media", _mediaRectCache);
                    Owner?.Invalidate();
                });
            }

            // Top overlay utility buttons
            if (!_infoRectCache.IsEmpty)
            {
                owner.AddHitArea("MediaViewer_Info", _infoRectCache, null, () =>
                {
                    bool show = ctx.CustomData.ContainsKey("ShowMediaInfo") && (bool)ctx.CustomData["ShowMediaInfo"];
                    ctx.CustomData["ShowMediaInfo"] = !show;
                    notifyAreaHit?.Invoke("MediaViewer_Info", _infoRectCache);
                    Owner?.Invalidate();
                });
            }
            if (!_fullRectCache.IsEmpty)
            {
                owner.AddHitArea("MediaViewer_Fullscreen", _fullRectCache, null, () =>
                {
                    bool full = ctx.CustomData.ContainsKey("Fullscreen") && (bool)ctx.CustomData["Fullscreen"];
                    ctx.CustomData["Fullscreen"] = !full;
                    notifyAreaHit?.Invoke("MediaViewer_Fullscreen", _fullRectCache);
                    Owner?.Invalidate();
                });
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Hover outlines for control buttons
            foreach (var (rect, id) in _controls)
            {
                if (IsAreaHovered($"MediaViewer_{id}"))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.DeepSkyBlue, 1.5f);
                    g.DrawRoundedRectangle(pen, rect, 4);
                }
            }

            // Media hover cue
            if (IsAreaHovered("MediaViewer_Media") && !_mediaRectCache.IsEmpty)
            {
                using var glow = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.DeepSkyBlue));
                g.FillRoundedRectangle(glow, _mediaRectCache, 8);
            }

            // Draw overlay utility buttons (info / fullscreen) with hover
            if (!_infoRectCache.IsEmpty)
            {
                bool hv = IsAreaHovered("MediaViewer_Info");
                using var bg = new SolidBrush(Color.FromArgb(hv ? 160 : 110, Color.Black));
                using var pen = new Pen(Theme?.AccentColor ?? Color.White, hv ? 1.5f : 1f);
                g.FillEllipse(bg, _infoRectCache);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                // draw 'i'
                using var f = new Font("Segoe UI", 10, FontStyle.Bold);
                using var br = new SolidBrush(Color.White);
                var sz = g.MeasureString("i", f);
                g.DrawString("i", f, br, _infoRectCache.X + (_infoRectCache.Width - sz.Width) / 2, _infoRectCache.Y + (_infoRectCache.Height - sz.Height) / 2);
                g.DrawEllipse(pen, _infoRectCache);
            }
            if (!_fullRectCache.IsEmpty)
            {
                bool hv = IsAreaHovered("MediaViewer_Fullscreen");
                using var bg = new SolidBrush(Color.FromArgb(hv ? 160 : 110, Color.Black));
                using var pen = new Pen(Theme?.AccentColor ?? Color.White, hv ? 1.5f : 1f);
                g.FillEllipse(bg, _fullRectCache);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                // draw simple fullscreen corners
                var r = Rectangle.Inflate(_fullRectCache, -6, -6);
                g.DrawLine(pen, r.Left, r.Top + 4, r.Left, r.Top);
                g.DrawLine(pen, r.Left, r.Top, r.Left + 4, r.Top);
                g.DrawLine(pen, r.Right, r.Top + 4, r.Right, r.Top);
                g.DrawLine(pen, r.Right, r.Top, r.Right - 4, r.Top);
                g.DrawLine(pen, r.Left, r.Bottom - 4, r.Left, r.Bottom);
                g.DrawLine(pen, r.Left, r.Bottom, r.Left + 4, r.Bottom);
                g.DrawLine(pen, r.Right, r.Bottom - 4, r.Right, r.Bottom);
                g.DrawLine(pen, r.Right, r.Bottom, r.Right - 4, r.Bottom);
                g.DrawEllipse(pen, _fullRectCache);
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
