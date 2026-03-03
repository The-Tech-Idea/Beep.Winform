using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// SidebarNav – Vertical sidebar navigation with DPI scaling, virtual scroll,
    /// cached fonts (BeepThemesManager.ToFont) and StyledImagePainter icons.
    /// </summary>
    internal sealed class SidebarNavPainter : WidgetPainterBase, IDisposable
    {
        // ── Cached fonts (never created inside draw) ──────────────────────
        private Font? _itemFont;
        private Font? _itemSelectedFont;

        // ── Mouse-wheel state ─────────────────────────────────────────────
        private bool _wheelHooked;

        // ── Layout constants (logical dp) ─────────────────────────────────
        private const int ItemHeightDp  = 40;
        private const int SpacingDp     =  4;
        private const int PadDp         =  8;
        private const int IconSizeDp    = 20;
        private const int IconOffsetDp  = 12;
        private const int TextOffsetDp  = 40;
        private const int IndicatorWDp  =  3;
        private const int CornerDp      =  6;

        // ─────────────────────────────────────────────────────────────────
        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            HookMouseWheel();
        }

        private void HookMouseWheel()
        {
            if (_wheelHooked || Owner == null) return;
            try { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
            catch { }
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            int lineH = Dp(ItemHeightDp + SpacingDp);
            _lastCtx.ScrollOffsetY = Math.Max(0,
                Math.Min(_lastCtx.ScrollOffsetY - e.Delta / 120 * lineH * 3,
                         Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height)));
            Owner?.Invalidate();
        }

        protected override void RebuildFonts()
        {
            _itemFont?.Dispose();
            _itemSelectedFont?.Dispose();

            var unselStyle = Theme?.NavigationUnSelectedFont
                          ?? Theme?.LabelSmall
                          ?? new TypographyStyle { FontSize = 9f };
            var selStyle   = Theme?.NavigationSelectedFont
                          ?? Theme?.LabelMedium
                          ?? new TypographyStyle { FontSize = 9f, FontWeight = FontWeight.Bold };

            _itemFont         = BeepThemesManager.ToFont(unselStyle, applyDpiScaling: true);
            _itemSelectedFont = BeepThemesManager.ToFont(selStyle,   applyDpiScaling: true);
        }

        // ─────────────────────────────────────────────────────────────────
        private WidgetContext? _lastCtx;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            _lastCtx = ctx;
            int pad = Dp(PadDp);
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.X + pad, ctx.DrawingRect.Y + pad,
                ctx.DrawingRect.Width  - pad * 2,
                ctx.DrawingRect.Height - pad * 2);

            // Compute virtual content height
            var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList()
                     ?? CreateSampleSidebarItems();
            int itemH = Dp(ItemHeightDp) + Dp(SpacingDp);
            ctx.TotalContentHeight = items.Count * itemH;
            ctx.TotalContentWidth  = 0;
            ClampScrollOffset(ctx);
            return ctx;
        }

        // ─────────────────────────────────────────────────────────────────
        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bgBrush    = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.White);
            var borderPen  = PaintersFactory.GetPen(Color.FromArgb(20, Theme?.SecondaryTextColor ?? Color.Gray), 1f);
            using var path = CreateRoundedPath(ctx.DrawingRect, Dp(8));
            g.FillPath(bgBrush, path);
            g.DrawPath(borderPen, path);
        }

        // ─────────────────────────────────────────────────────────────────
        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList()
                     ?? CreateSampleSidebarItems();
            if (!items.Any()) return;

            int activeIndex  = ctx.ActiveIndex;
            int itemH        = Dp(ItemHeightDp);
            int spacing      = Dp(SpacingDp);
            int stride       = itemH + spacing;
            var primaryColor = Theme?.PrimaryColor ?? Color.DodgerBlue;
            var fgColor      = Theme?.ForeColor    ?? Color.Black;

            // Clip to content viewport
            var savedClip = g.Clip;
            g.SetClip(ctx.ContentRect);

            using var fmt = new StringFormat
            {
                Alignment     = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter
            };

            for (int i = 0; i < items.Count; i++)
            {
                int y = ctx.ContentRect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + itemH < ctx.ContentRect.Y) continue;  // above viewport
                if (y         > ctx.ContentRect.Bottom) break; // below viewport

                var item     = items[i];
                bool isActive = i == activeIndex;
                var itemRect  = new Rectangle(ctx.ContentRect.X, y,
                                    ctx.ContentRect.Width - (NeedsVerticalScroll(ctx) ? Dp(10) : 0), itemH);

                // ── Active background ──
                if (isActive)
                {
                    using var activeBrush = new SolidBrush(Color.FromArgb(30, primaryColor));
                    using var activePath  = CreateRoundedPath(itemRect, Dp(CornerDp));
                    g.FillPath(activeBrush, activePath);

                    // Left indicator bar
                    var indicatorRect = new Rectangle(itemRect.X, itemRect.Y + Dp(10), Dp(IndicatorWDp), Dp(20));
                    using var indBrush = new SolidBrush(primaryColor);
                    using var indPath  = CreateRoundedPath(indicatorRect, Dp(2));
                    g.FillPath(indBrush, indPath);
                }
                else if (IsAreaHovered($"SidebarNav_Item_{i}"))
                {
                    using var hoverBrush = new SolidBrush(Color.FromArgb(10, primaryColor));
                    using var hoverPath  = CreateRoundedPath(itemRect, Dp(CornerDp));
                    g.FillPath(hoverBrush, hoverPath);
                }

                // ── Icon ──
                int iconX    = itemRect.X + Dp(IconOffsetDp);
                int iconSize = Dp(IconSizeDp);
                int iconY    = itemRect.Y + (itemH - iconSize) / 2;
                var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
                var iconSvg  = GetSidebarIconPath(item.Text ?? "", i);
                var iconColor = isActive ? primaryColor : Color.FromArgb(140, fgColor);

                using var iconPath = new GraphicsPath();
                iconPath.AddRectangle(iconRect);
                StyledImagePainter.PaintWithTint(g, iconPath, iconSvg, iconColor, 0.9f);

                // ── Label ──
                var textRect = new Rectangle(
                    itemRect.X + Dp(TextOffsetDp), itemRect.Y,
                    itemRect.Width - Dp(TextOffsetDp), itemH);
                var textColor  = isActive ? primaryColor : fgColor;
                var textBrush  = PaintersFactory.GetSolidBrush(textColor);
                var font       = isActive ? _itemSelectedFont : _itemFont;
                if (font != null)
                    g.DrawString(item.Text ?? string.Empty, font, textBrush, textRect, fmt);
            }

            g.Clip = savedClip;
        }

        // ─────────────────────────────────────────────────────────────────
        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            bool scrollHovered = IsAreaHovered("SidebarNav_Scroll");
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, scrollHovered);
        }

        // ─────────────────────────────────────────────────────────────────
        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            _lastCtx = ctx;

            var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList()
                     ?? CreateSampleSidebarItems();
            int itemH  = Dp(ItemHeightDp);
            int stride = itemH + Dp(SpacingDp);

            for (int i = 0; i < items.Count; i++)
            {
                int idx   = i;
                int y     = ctx.ContentRect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + itemH < ctx.ContentRect.Y || y > ctx.ContentRect.Bottom)
                    continue;
                var rect  = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, itemH);
                owner.AddHitArea($"SidebarNav_Item_{idx}", rect, null, () =>
                {
                    ctx.ActiveIndex = idx;
                    notifyAreaHit?.Invoke($"SidebarNav_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        // ─────────────────────────────────────────────────────────────────
        private static List<NavigationItem> CreateSampleSidebarItems() =>
            new List<NavigationItem>
            {
                new NavigationItem { Text = "Dashboard" },
                new NavigationItem { Text = "Analytics" },
                new NavigationItem { Text = "Reports" },
                new NavigationItem { Text = "Settings" }
            };

        private static string GetSidebarIconPath(string text, int index)
        {
            var t = text.ToLowerInvariant();
            if (t.Contains("dashboard") || t.Contains("home")) return SvgsUI.Home;
            if (t.Contains("analytic"))                         return SvgsUI.BarChart2;
            if (t.Contains("report"))                           return SvgsUI.FileText;
            if (t.Contains("setting"))                          return SvgsUI.Settings;
            if (t.Contains("user") || t.Contains("profile"))   return SvgsUI.User;
            if (t.Contains("message"))                          return SvgsUI.MessageCircle;
            if (t.Contains("chart"))                            return SvgsUI.PieChart;
            if (t.Contains("list"))                             return SvgsUI.List;
            if (t.Contains("grid") || t.Contains("table"))     return SvgsUI.Grid;
            return SvgsUI.Circle;
        }

        // ─────────────────────────────────────────────────────────────────
        public void Dispose()
        {
            if (_wheelHooked && Owner != null)
            {
                try { Owner.MouseWheel -= OnMouseWheel; } catch { }
                _wheelHooked = false;
            }
            _itemFont?.Dispose();
            _itemSelectedFont?.Dispose();
        }
    }
}