using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Media
{
    /// <summary>
    /// AvatarGroup - Clustered circular profile pictures with hit areas and hover
    /// </summary>
    internal sealed class AvatarGroupPainter : WidgetPainterBase, IDisposable
    {
        private ImagePainter _avatarPainter;
        private readonly List<(Rectangle rect, int index)> _avatarRects = new();

        public AvatarGroupPainter()
        {
            _avatarPainter = new ImagePainter
            {
                ScaleMode = ImageScaleMode.Fill,
                ClipShape = ImageClipShape.Circle
            };
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);

            // Title area if present
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);
            }

            // Avatar group area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );

            _avatarRects.Clear();
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }

            // Draw avatar cluster
            var mediaItems = ctx.CustomData.ContainsKey("MediaItems") ?
                (List<MediaItem>)ctx.CustomData["MediaItems"] : new List<MediaItem>();

            DrawAvatarCluster(g, ctx, ctx.ContentRect, mediaItems.Where(x => x.IsAvatar).Take(6).ToList(), ctx.AccentColor);
        }

        private void DrawAvatarCluster(Graphics g, WidgetContext ctx, Rectangle rect, List<MediaItem> avatars, Color accentColor)
        {
            if (!avatars.Any()) return;

            int avatarSize = Math.Min(40, Math.Min(rect.Width / 3, rect.Height / 2));
            int overlap = avatarSize / 4;

            _avatarRects.Clear();
            for (int i = 0; i < Math.Min(avatars.Count, 6); i++)
            {
                var avatar = avatars[i];

                // Calculate position in cluster
                int x = rect.X + (i * (avatarSize - overlap));
                int y = rect.Y + (i % 2) * (avatarSize / 3);

                var avatarRect = new Rectangle(x, y, avatarSize, avatarSize);
                _avatarRects.Add((avatarRect, i));

                bool hovered = IsAreaHovered($"AvatarGroup_Item_{i}");

                // Draw avatar background
                using var avatarBrush = new SolidBrush(Color.FromArgb(hovered ? 50 : 30, accentColor));
                g.FillEllipse(avatarBrush, avatarRect);

                // Draw avatar border
                using var borderPen = new Pen(Theme?.BackColor ?? Color.White, hovered ? 3 : 2);
                g.DrawEllipse(borderPen, avatarRect);

                // Draw avatar image if available
                if (avatar.Image != null || !string.IsNullOrEmpty(avatar.ImagePath))
                {
                    // Configure avatar painter
                    _avatarPainter.CurrentTheme = Theme;
                    _avatarPainter.ClipShape = ImageClipShape.Circle;
                    _avatarPainter.ScaleMode = ImageScaleMode.Fill;
                    
                    // Set image source
                    if (!string.IsNullOrEmpty(avatar.ImagePath))
                    {
                        _avatarPainter.ImagePath = avatar.ImagePath;
                    }
                    else if (avatar.Image != null)
                    {
                        _avatarPainter.Image = avatar.Image;
                    }
                    
                    // Draw avatar with ImagePainter
                    _avatarPainter.DrawImage(g, avatarRect);
                }
                else
                {
                    // Draw initials
                    string initials = GetInitials(avatar.Title);
                    using var initialsFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, avatarSize / 4, FontStyle.Bold);
                    using var initialsBrush = new SolidBrush(accentColor);

                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(initials, initialsFont, initialsBrush, avatarRect, format);
                }
            }

            // Show count if more than 6 avatars
            if (avatars.Count > 6)
            {
                int extraCount = avatars.Count - 5;
                int x = rect.X + (5 * (avatarSize - overlap));
                int y = rect.Y;
                var extraRect = new Rectangle(x, y, avatarSize, avatarSize);

                using var extraBrush = new SolidBrush(Color.FromArgb(100, Color.Gray));
                using var extraTextBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
                using var extraFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, avatarSize / 5, FontStyle.Bold);

                g.FillEllipse(extraBrush, extraRect);

                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString($"+{extraCount}", extraFont, extraTextBrush, extraRect, format);
            }
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Hover outline for each avatar
            for (int i = 0; i < _avatarRects.Count; i++)
            {
                if (IsAreaHovered($"AvatarGroup_Item_{i}"))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.SteelBlue, 1.5f);
                    g.DrawEllipse(pen, _avatarRects[i].rect);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _avatarRects.Count; i++)
            {
                int idx = _avatarRects[i].index;
                var rect = _avatarRects[i].rect;
                owner.AddHitArea($"AvatarGroup_Item_{idx}", rect, null, () =>
                {
                    ctx.CustomData["SelectedAvatarIndex"] = idx;
                    notifyAreaHit?.Invoke($"AvatarGroup_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _avatarPainter?.Dispose();
        }
    }
}
