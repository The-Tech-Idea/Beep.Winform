using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// ProfileList - User/profile listings with avatar and name hit areas
    /// </summary>
    internal sealed class ProfileListPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _avatarRects = new();
        private readonly List<Rectangle> _nameRects = new();

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

            _avatarRects.Clear();
            _nameRects.Clear();
            if (ctx.CustomData.TryGetValue("Items", out var raw) && raw is List<Dictionary<string, object>> items)
            {
                int itemHeight = Math.Min(48, ctx.ContentRect.Height / Math.Max(items.Count, 1));
                for (int i = 0; i < items.Count; i++)
                {
                    int y = ctx.ContentRect.Y + i * itemHeight;
                    var avatarRect = new Rectangle(ctx.ContentRect.X + 8, y + 8, itemHeight - 16, itemHeight - 16);
                    _avatarRects.Add(avatarRect);
                    _nameRects.Add(new Rectangle(ctx.ContentRect.X + itemHeight + 8, y + 8, ctx.ContentRect.Width - itemHeight - 16, (itemHeight - 16) / 2));
                }
            }
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            if (ctx.CustomData.TryGetValue("Items", out var raw) && raw is List<Dictionary<string, object>> items)
            {
                DrawProfileItems(g, ctx.ContentRect, items, ctx.AccentColor);
            }
        }

        private void DrawProfileItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items, Color accentColor)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(48, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var roleFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                var avatarRect = _avatarRects.Count > i ? _avatarRects[i] : new Rectangle(rect.X + 8, y + 8, itemHeight - 16, itemHeight - 16);
                using var avatarBrush = new SolidBrush(Color.FromArgb(30, accentColor));
                g.FillEllipse(avatarBrush, avatarRect);
                using var avatarPen = new Pen(Color.FromArgb(100, accentColor), 1);
                g.DrawEllipse(avatarPen, avatarRect);
                
                var nameRect = _nameRects.Count > i ? _nameRects[i] : new Rectangle(rect.X + itemHeight + 8, y + 8, rect.Width - itemHeight - 16, (itemHeight - 16) / 2);
                var roleRect = new Rectangle(nameRect.X, nameRect.Bottom, nameRect.Width, (itemHeight - 16) / 2);
                
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect, nameFormat);
                }
                
                if (item.ContainsKey("Value"))
                {
                    using var roleBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
                    var roleFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Value"].ToString(), roleFont, roleBrush, roleRect, roleFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            for (int i = 0; i < _avatarRects.Count; i++)
            {
                if (IsAreaHovered($"ProfileList_Avatar_{i}"))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.Blue, 1.2f);
                    g.DrawEllipse(pen, _avatarRects[i]);
                }
                if (IsAreaHovered($"ProfileList_Name_{i}"))
                {
                    using var underline = new Pen(Theme?.AccentColor ?? Color.Blue, 1f);
                    var nr = _nameRects[i];
                    g.DrawLine(underline, nr.Left, nr.Bottom + 1, nr.Right, nr.Bottom + 1);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            for (int i = 0; i < _avatarRects.Count; i++)
            {
                int idx = i;
                var ar = _avatarRects[i];
                var nr = _nameRects[i];
                owner.AddHitArea($"ProfileList_Avatar_{idx}", ar, null, () =>
                {
                    ctx.CustomData["SelectedProfileAvatarIndex"] = idx;
                    notifyAreaHit?.Invoke($"ProfileList_Avatar_{idx}", ar);
                    Owner?.Invalidate();
                });
                owner.AddHitArea($"ProfileList_Name_{idx}", nr, null, () =>
                {
                    ctx.CustomData["SelectedProfileNameIndex"] = idx;
                    notifyAreaHit?.Invoke($"ProfileList_Name_{idx}", nr);
                    Owner?.Invalidate();
                });
            }
        }
    }
}