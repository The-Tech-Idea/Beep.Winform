using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// ProfileList - User/profile listings with avatar and name hit areas
    /// </summary>
    internal sealed class ProfileListPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _avatarRects = new();
        private readonly List<Rectangle> _nameRects   = new();
        private WidgetContext? _lastCtx;
        private bool _wheelHooked;

        private Font? _titleFont;
        private Font? _nameFont;
        private Font? _roleFont;

        private const int PadDp        = 16;
        private const int HeaderHDp    = 24;
        private const int ItemHeightDp = 48;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            if (!_wheelHooked && Owner != null) { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
        }

        protected override void RebuildFonts()
        {
            _titleFont?.Dispose(); _nameFont?.Dispose(); _roleFont?.Dispose();
            _titleFont = BeepThemesManager.ToFont(Theme?.LabelMedium  ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold }, true);
            _nameFont  = BeepThemesManager.ToFont(Theme?.LabelSmall   ?? new TypographyStyle { FontSize = 9f,  FontWeight = FontWeight.Bold }, true);
            _roleFont  = BeepThemesManager.ToFont(Theme?.CaptionStyle ?? new TypographyStyle { FontSize = 8f }, true);
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            int maxY = Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height);
            _lastCtx.ScrollOffsetY = Math.Max(0, Math.Min(_lastCtx.ScrollOffsetY - e.Delta / 120 * Dp(ItemHeightDp) * 3, maxY));
            Owner?.Invalidate();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = Dp(PadDp);
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            ctx.HeaderRect  = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, Dp(HeaderHDp));
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + Dp(8), ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

            _avatarRects.Clear(); _nameRects.Clear();
            var items = ctx.ListItems;
            int stride = Dp(ItemHeightDp);
            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    int y = ctx.ContentRect.Y + i * stride;
                    int av = stride - Dp(16);
                    _avatarRects.Add(new Rectangle(ctx.ContentRect.X + Dp(8), y + Dp(8), av, av));
                    _nameRects.Add(new Rectangle(ctx.ContentRect.X + stride + Dp(8), y + Dp(8), ctx.ContentRect.Width - stride - Dp(16), av / 2));
                }
                ctx.TotalContentHeight = items.Count * stride;
            }
            else ctx.TotalContentHeight = 0;
            ClampScrollOffset(ctx);
            _lastCtx = ctx;
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            var bgBrush = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                var titleBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                if (_titleFont != null) g.DrawString(ctx.Title, _titleFont, titleBrush, ctx.HeaderRect);
            }
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
                DrawProfileItems(g, ctx, ctx.ContentRect, items, ctx.AccentColor);
        }

        private void DrawProfileItems(Graphics g, WidgetContext ctx, Rectangle rect, List<ListItem> items, Color accentColor)
        {
            if (!items.Any()) return;

            int stride = Dp(ItemHeightDp);
            var nameBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(180, Theme?.ForeColor ?? Color.Black));
            var roleBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray));
            var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
            var roleFormat = new StringFormat { LineAlignment = StringAlignment.Center };

            var savedClip = g.Clip;
            g.SetClip(rect);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + stride < rect.Y) continue;
                if (y > rect.Bottom) break;

                int av = stride - Dp(16);
                var avatarRect = _avatarRects.Count > i
                    ? new Rectangle(_avatarRects[i].X, _avatarRects[i].Y - ctx.ScrollOffsetY + (i == 0 ? 0 : 0), _avatarRects[i].Width, _avatarRects[i].Height)
                    : new Rectangle(rect.X + Dp(8), y + Dp(8), av, av);
                avatarRect = new Rectangle(rect.X + Dp(8), y + Dp(8), av, av);

                using var avatarFill = new SolidBrush(Color.FromArgb(30, accentColor));
                g.FillEllipse(avatarFill, avatarRect);
                var avatarOutline = PaintersFactory.GetPen(Color.FromArgb(100, accentColor), 1);
                g.DrawEllipse(avatarOutline, avatarRect);

                var nameRect = new Rectangle(rect.X + stride + Dp(8), y + Dp(8), rect.Width - stride - Dp(16), av / 2);
                var roleRect = new Rectangle(nameRect.X, nameRect.Bottom, nameRect.Width, av / 2);

                if (!string.IsNullOrEmpty(item.Title) && _nameFont != null)
                    g.DrawString(item.Title, _nameFont, nameBrush, nameRect, nameFormat);
                if (!string.IsNullOrEmpty(item.Subtitle) && _roleFont != null)
                    g.DrawString(item.Subtitle, _roleFont, roleBrush, roleRect, roleFormat);
            }

            g.Clip = savedClip;
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, IsAreaHovered("ProfileList_Scroll"));
            for (int i = 0; i < _avatarRects.Count; i++)
            {
                if (IsAreaHovered($"ProfileList_Avatar_{i}"))
                {
                    int dy = i * Dp(ItemHeightDp) - ctx.ScrollOffsetY;
                    var ar = new Rectangle(_avatarRects[i].X, ctx.ContentRect.Y + dy + Dp(8), _avatarRects[i].Width, _avatarRects[i].Height);
                    var pen = PaintersFactory.GetPen(Theme?.AccentColor ?? Color.Blue, 1.2f);
                    g.DrawEllipse(pen, ar);
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
                    ctx.SelectedProfileAvatarIndex = idx;
                    notifyAreaHit?.Invoke($"ProfileList_Avatar_{idx}", ar);
                    Owner?.Invalidate();
                });
                owner.AddHitArea($"ProfileList_Name_{idx}", nr, null, () =>
                {
                    ctx.SelectedProfileNameIndex = idx;
                    notifyAreaHit?.Invoke($"ProfileList_Name_{idx}", nr);
                    Owner?.Invalidate();
                });
            }
        }
    }
}