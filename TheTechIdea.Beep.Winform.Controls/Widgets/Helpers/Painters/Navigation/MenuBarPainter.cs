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
    /// MenuBar - Horizontal menu bar navigation
    /// </summary>
    internal sealed class MenuBarPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public MenuBarPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 8, ctx.DrawingRect.Y + 4,
                ctx.DrawingRect.Width - 16, ctx.DrawingRect.Height - 8);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.Empty);
            using var borderPen = new Pen(Color.FromArgb(30, Theme?.SecondaryTextColor ?? Color.Empty), 1);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
            g.DrawRectangle(borderPen, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.NavigationItems?.OfType<NavigationItem>().ToList() ?? CreateSampleMenuItems();

            if (!items.Any()) return;

            int itemWidth = ctx.ContentRect.Width / items.Count;
            using var menuFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            var primaryColor = Theme?.PrimaryColor ?? Color.Empty;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var itemRect = new Rectangle(ctx.ContentRect.X + i * itemWidth, ctx.ContentRect.Y,
                    itemWidth, ctx.ContentRect.Height);

                // Hover effect
                if (item.IsActive)
                {
                    using var hoverBrush = new SolidBrush(Color.FromArgb(20, primaryColor));
                    g.FillRectangle(hoverBrush, itemRect);
                }

                // Menu item icon
                var iconRect = new Rectangle(itemRect.X + 8, itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                var iconName = GetMenuIcon(item.Text, i);
                if (!string.IsNullOrEmpty(iconName))
                {
                    _imagePainter.DrawSvg(g, iconName, iconRect, primaryColor, 0.8f);
                }

                // Menu item text
                using var textBrush = new SolidBrush(item.IsActive ? primaryColor : (Theme?.ForeColor ?? Color.Empty));
                var textRect = new Rectangle(itemRect.X + 28, itemRect.Y, itemRect.Width - 28, itemRect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, menuFont, textBrush, textRect, format);
            }
        }

        private List<NavigationItem> CreateSampleMenuItems()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "File", IsActive = false },
                new NavigationItem { Text = "Edit", IsActive = true },
                new NavigationItem { Text = "View", IsActive = false },
                new NavigationItem { Text = "Help", IsActive = false }
            };
        }

        private string GetMenuIcon(string menuText, int index)
        {
            var text = menuText?.ToLower() ?? "";
            if (text.Contains("file")) return "file";
            if (text.Contains("edit")) return "edit";
            if (text.Contains("view")) return "eye";
            if (text.Contains("help")) return "help-circle";
            return "menu";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}