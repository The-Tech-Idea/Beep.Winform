using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Breadcrumb - Breadcrumb navigation with enhanced visual presentation and hit areas
    /// </summary>
    internal sealed class BreadcrumbPainter : WidgetPainterBase, IDisposable
    {
        private Font? _itemFont;
        private readonly List<(Rectangle rect, int index)> _itemRects = new();
        private Rectangle _homeRect;

        protected override void RebuildFonts()
        {
            _itemFont?.Dispose();
            _itemFont = BeepThemesManager.ToFont(Theme?.LabelSmall ?? new TypographyStyle { FontSize = 10f }, true);
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = Dp(8);
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);

            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );

            _itemRects.Clear();
            _homeRect = Rectangle.Empty;

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bgBrush = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList() ?? CreateSampleBreadcrumb();
            int currentIndex = ctx.CurrentIndex >= 0 ? ctx.CurrentIndex : items.Count - 1;

            if (!items.Any()) return;

            DrawModernBreadcrumb(g, ctx, items, currentIndex);
        }

        private List<NavigationItem> CreateSampleBreadcrumb()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Dashboard", IsActive = false },
                new NavigationItem { Text = "Analytics", IsActive = false },
                new NavigationItem { Text = "Reports", IsActive = true }
            };
        }

        private void DrawModernBreadcrumb(Graphics g, WidgetContext ctx, List<NavigationItem> items, int currentIndex)
        {
            var font         = _itemFont ?? SystemFonts.DefaultFont;
            var regularBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray));
            var activeBrush  = PaintersFactory.GetSolidBrush(Theme?.ForeColor ?? Color.Black);

            int x = ctx.ContentRect.X + Dp(8);
            int y = ctx.ContentRect.Y + ctx.ContentRect.Height / 2;

            // Home icon
            int iconSz = Dp(16);
            _homeRect = new Rectangle(x, y - iconSz / 2, iconSz, iconSz);
            using (var homePath = CreateRoundedPath(_homeRect, 0))
                StyledImagePainter.PaintWithTint(g, homePath, SvgsUI.Home, Theme?.ForeColor ?? Color.Black, 0.9f);
            x += Dp(24);

            // Measure all items to determine if we need ellipsis
            var measuredWidths = items.Select(item => (int)TextUtils.MeasureText(g, item.Text, font).Width).ToList();
            int chevW = Dp(16);
            int totalW = Dp(24) + items.Count * (measuredWidths.Max() + chevW);
            bool ellipsis = totalW > ctx.ContentRect.Width - Dp(32) && items.Count > 3;

            // Determine which items to show: always first, last, and active; collapse middle to "..."
            var showIndices = new HashSet<int> { 0, items.Count - 1, currentIndex };
            bool ellipsisInserted = false;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool isActive = i == currentIndex;

                if (ellipsis && !showIndices.Contains(i))
                {
                    if (!ellipsisInserted)
                    {
                        // Draw separator + ellipsis
                        var chevRect = new Rectangle(x, y - iconSz / 2, iconSz, iconSz);
                        using (var chevPath = CreateRoundedPath(chevRect, 0))
                            StyledImagePainter.PaintWithTint(g, chevPath, SvgsUI.ChevronRight,
                                Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray), 0.8f);
                        x += chevW;

                        var ellipsisBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray));
                        g.DrawString("\u2026", font, ellipsisBrush, x, y - font.Height / 2);
                        x += (int)TextUtils.MeasureText(g, "\u2026", font).Width + Dp(8);
                        ellipsisInserted = true;
                    }
                    continue;
                }

                // Separator chevron (except before first item)
                if (i > 0)
                {
                    var chevronRect = new Rectangle(x, y - iconSz / 2, iconSz - Dp(4), iconSz - Dp(4));
                    using (var chevPath = CreateRoundedPath(chevronRect, 0))
                        StyledImagePainter.PaintWithTint(g, chevPath, SvgsUI.ChevronRight,
                            Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray), 0.8f);
                    x += chevW;
                }

                // Item text  
                var brush    = isActive ? activeBrush : regularBrush;
                var textSize = TextUtils.MeasureText(g, item.Text, font);
                var textRect = new Rectangle(x - Dp(4), y - Dp(10), (int)textSize.Width + Dp(8), Dp(20));

                // Item background for active or hovered
                if (isActive || IsAreaHovered($"Breadcrumb_Item_{i}"))
                {
                    var bgBrush = PaintersFactory.GetSolidBrush(
                        Color.FromArgb(isActive ? 15 : 8, Theme?.PrimaryColor ?? Color.Blue));
                    using var bgPath = CreateRoundedPath(textRect, Dp(4));
                    g.FillPath(bgBrush, bgPath);
                }

                g.DrawString(item.Text, font, brush, new PointF(x, y - textSize.Height / 2));
                _itemRects.Add((textRect, i));
                x += (int)textSize.Width + Dp(8);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Home hover accent
            if (!_homeRect.IsEmpty && IsAreaHovered("Breadcrumb_Home"))
            {
                var hover = PaintersFactory.GetSolidBrush(Color.FromArgb(20, Theme?.PrimaryColor ?? Color.Blue));
                g.FillEllipse(hover, Rectangle.Inflate(_homeRect, Dp(4), Dp(4)));
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Home
            if (!_homeRect.IsEmpty)
            {
                owner.AddHitArea("Breadcrumb_Home", _homeRect, null, () =>
                {
                    ctx.BreadcrumbHomeClicked = true;
                    notifyAreaHit?.Invoke("Breadcrumb_Home", _homeRect);
                    Owner?.Invalidate();
                });
            }

            // Items
            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = _itemRects[i].index;
                var rect = _itemRects[i].rect;
                owner.AddHitArea($"Breadcrumb_Item_{idx}", rect, null, () =>
                {
                    ctx.BreadcrumbIndex = idx;
                    notifyAreaHit?.Invoke($"Breadcrumb_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _itemFont?.Dispose();
        }
    }
}