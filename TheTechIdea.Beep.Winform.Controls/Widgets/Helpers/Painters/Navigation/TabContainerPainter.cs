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
    /// TabContainer - Tab navigation
    /// </summary>
    internal sealed class TabContainerPainter : WidgetPainterBase, IDisposable
    {
        private Font? _tabFont;

        protected override void RebuildFonts()
        {
            _tabFont?.Dispose();
            _tabFont = BeepThemesManager.ToFont(Theme?.LabelSmall ?? new TypographyStyle { FontSize = 9f }, true);
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = Dp(4);
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(245, 245, 245));
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, Dp(4));
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList() ?? CreateSampleTabs();
            int currentIndex = ctx.CurrentIndex >= 0 ? ctx.CurrentIndex : 0;

            if (!items.Any()) return;

            DrawModernTabs(g, ctx, items, currentIndex);
        }

        private List<NavigationItem> CreateSampleTabs()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Overview", IsActive = true },
                new NavigationItem { Text = "Details", IsActive = false },
                new NavigationItem { Text = "Settings", IsActive = false }
            };
        }

        private void DrawModernTabs(Graphics g, WidgetContext ctx, List<NavigationItem> items, int currentIndex)
        {
            int tabWidth       = ctx.ContentRect.Width / items.Count;
            var primaryColor   = ctx.AccentColor != Color.Empty ? ctx.AccentColor
                : (Theme != null ? Theme.PrimaryColor : Color.FromArgb(33, 150, 243));
            var font           = _tabFont ?? SystemFonts.DefaultFont;
            var activeTabBrush = PaintersFactory.GetSolidBrush(Color.White);
            var inactiveTabBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(248, 249, 250));
            var activeTextBrush  = PaintersFactory.GetSolidBrush(primaryColor);
            var inactiveTextBrush= PaintersFactory.GetSolidBrush(Color.FromArgb(120, Color.Black));
            int iconSz = Dp(16);

            for (int i = 0; i < items.Count; i++)
            {
                var item    = items[i];
                bool isActive = i == currentIndex;

                var tabRect = new Rectangle(
                    ctx.ContentRect.X + i * tabWidth,
                    ctx.ContentRect.Y,
                    tabWidth,
                    ctx.ContentRect.Height
                );

                if (isActive)
                {
                    // Active tab shadow
                    var shadowRect = new Rectangle(tabRect.X + 1, tabRect.Y + 1, tabRect.Width, tabRect.Height);
                    var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(10, Color.Black));
                    using var shadowPath = CreateRoundedPath(shadowRect, Dp(6));
                    g.FillPath(shadowBrush, shadowPath);

                    using var tabPath = CreateRoundedPath(tabRect, Dp(6));
                    g.FillPath(activeTabBrush, tabPath);

                    var accentPen = PaintersFactory.GetPen(primaryColor, 2f);
                    g.DrawPath(accentPen, tabPath);
                }
                else
                {
                    using var tabPath = CreateRoundedPath(tabRect, Dp(6));
                    g.FillPath(inactiveTabBrush, tabPath);
                }

                // Tab icon
                var iconSvg = GetTabIconSvg(item.Text, i);
                bool hasIcon = iconSvg != null;
                if (hasIcon)
                {
                    var iconRect = new Rectangle(tabRect.X + Dp(8), tabRect.Y + (tabRect.Height - iconSz) / 2, iconSz, iconSz);
                    using var iconPath = CreateRoundedPath(iconRect, 0);
                    StyledImagePainter.PaintWithTint(g, iconPath, iconSvg!, isActive ? primaryColor : (Theme?.ForeColor ?? Color.Gray), 0.8f);
                }

                // Tab text
                var textBrush = isActive ? activeTextBrush : inactiveTextBrush;
                int textOffsetX = hasIcon ? Dp(24) : 0;
                var textRect = new Rectangle(tabRect.X + textOffsetX, tabRect.Y,
                    tabRect.Width - textOffsetX, tabRect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, font, textBrush, textRect, format);
            }
        }

        private string? GetTabIconSvg(string tabText, int index)
        {
            if (string.IsNullOrEmpty(tabText)) return null;
            var text = tabText.ToLower();
            if (text.Contains("overview") || text.Contains("dashboard")) return SvgsUI.Home;
            if (text.Contains("detail")   || text.Contains("info"))      return SvgsUI.InfoCircle;
            if (text.Contains("setting")  || text.Contains("config"))    return SvgsUI.Settings;
            if (text.Contains("chart")    || text.Contains("analytic"))   return SvgsUI.ChartDotsN2;
            if (text.Contains("user")     || text.Contains("profile"))    return SvgsUI.User;
            return null;
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowTabSeparators)
            {
                var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList() ?? CreateSampleTabs();
                int tabWidth = ctx.ContentRect.Width / items.Count;
                var separatorPen = PaintersFactory.GetPen(Color.FromArgb(30, Color.Gray), 1f);

                for (int i = 1; i < items.Count; i++)
                {
                    int x = ctx.ContentRect.X + i * tabWidth;
                    g.DrawLine(separatorPen, x, ctx.ContentRect.Y + Dp(8), x, ctx.ContentRect.Bottom - Dp(8));
                }
            }
        }

        public void Dispose()
        {
            _tabFont?.Dispose();
        }
    }
}
