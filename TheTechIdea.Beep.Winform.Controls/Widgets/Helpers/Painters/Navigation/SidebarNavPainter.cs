using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// SidebarNav - Vertical sidebar navigation
    /// </summary>
    internal sealed class SidebarNavPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public SidebarNavPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 8, ctx.DrawingRect.Y + 8,
                ctx.DrawingRect.Width - 16, ctx.DrawingRect.Height - 16);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.FromArgb(248, 249, 250));
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);

            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(20, Color.Gray), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.CustomData.ContainsKey("Items") ?
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleSidebarItems();

            int activeIndex = ctx.CustomData.ContainsKey("ActiveIndex") ?
                (int)ctx.CustomData["ActiveIndex"] : 0;

            if (!items.Any()) return;

            int itemHeight = 36;
            int spacing = 4;
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);

            using var navFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var itemRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + i * (itemHeight + spacing),
                    ctx.ContentRect.Width, itemHeight);

                bool isActive = i == activeIndex;

                // Active item background
                if (isActive)
                {
                    using var activeBrush = new SolidBrush(Color.FromArgb(30, primaryColor));
                    using var activePath = CreateRoundedPath(itemRect, 6);
                    g.FillPath(activeBrush, activePath);

                    // Active indicator
                    var indicatorRect = new Rectangle(itemRect.X, itemRect.Y + 8, 3, 20);
                    using var indicatorBrush = new SolidBrush(primaryColor);
                    using var indicatorPath = CreateRoundedPath(indicatorRect, 2);
                    g.FillPath(indicatorBrush, indicatorPath);
                }

                // Navigation icon
                var iconRect = new Rectangle(itemRect.X + 12, itemRect.Y + (itemRect.Height - 20) / 2, 20, 20);
                var iconName = GetSidebarIcon(item.Text, i);
                _imagePainter.DrawSvg(g, iconName, iconRect,
                    isActive ? primaryColor : Color.FromArgb(140, Theme?.ForeColor ?? Color.Black), 0.9f);

                // Navigation text
                using var textBrush = new SolidBrush(isActive ? primaryColor : Theme?.ForeColor ?? Color.Black);
                var textRect = new Rectangle(itemRect.X + 40, itemRect.Y, itemRect.Width - 40, itemHeight);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, navFont, textBrush, textRect, format);
            }
        }

        private List<NavigationItem> CreateSampleSidebarItems()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Dashboard", IsActive = true },
                new NavigationItem { Text = "Analytics", IsActive = false },
                new NavigationItem { Text = "Reports", IsActive = false },
                new NavigationItem { Text = "Settings", IsActive = false }
            };
        }

        private string GetSidebarIcon(string itemText, int index)
        {
            var text = itemText?.ToLower() ?? string.Empty;
            if (text.Contains("dashboard")) return "home";
            if (text.Contains("analytic")) return "bar-chart-2";
            if (text.Contains("report")) return "file-text";
            if (text.Contains("setting")) return "settings";
            if (text.Contains("user") || text.Contains("profile")) return "user";
            if (text.Contains("message")) return "message-circle";
            return "circle";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}