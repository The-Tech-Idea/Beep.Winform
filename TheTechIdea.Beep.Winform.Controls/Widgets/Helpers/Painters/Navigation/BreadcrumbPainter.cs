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
    /// Breadcrumb - Breadcrumb navigation with enhanced visual presentation
    /// </summary>
    internal sealed class BreadcrumbPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public BreadcrumbPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Breadcrumb items take full area
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
            // Minimal background for breadcrumbs
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.CustomData.ContainsKey("Items") ? 
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleBreadcrumb();
            
            int currentIndex = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] : items.Count - 1;
            
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
            using var regularFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var activeFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Medium);
            using var regularBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray));
            using var activeBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            
            int x = ctx.ContentRect.X + 8;
            int y = ctx.ContentRect.Y + ctx.ContentRect.Height / 2;
            
            // Home icon
            var homeIconRect = new Rectangle(x, y - 8, 16, 16);
            _imagePainter.DrawSvg(g, "home", homeIconRect, 
                Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.8f);
            x += 24;
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool isActive = i == currentIndex;
                bool isLast = i == items.Count - 1;
                
                // Separator chevron (except before first item)
                if (i > 0)
                {
                    var chevronRect = new Rectangle(x, y - 6, 12, 12);
                    _imagePainter.DrawSvg(g, "chevron-right", chevronRect, 
                        Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray), 0.6f);
                    x += 16;
                }
                
                // Item background for active item
                var font = isActive ? activeFont : regularFont;
                var brush = isActive ? activeBrush : regularBrush;
                var textSize = g.MeasureString(item.Text, font);
                
                if (isActive)
                {
                    var bgRect = new Rectangle(x - 4, y - 10, (int)textSize.Width + 8, 20);
                    using var bgBrush = new SolidBrush(Color.FromArgb(15, Theme?.PrimaryColor ?? Color.Blue));
                    using var bgPath = CreateRoundedPath(bgRect, 4);
                    g.FillPath(bgBrush, bgPath);
                }
                
                // Item text
                var textPoint = new PointF(x, y - textSize.Height / 2);
                g.DrawString(item.Text, font, brush, textPoint);
                
                x += (int)textSize.Width + 8;
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Hover effects for breadcrumb items
            if (ctx.CustomData.ContainsKey("HoveredIndex"))
            {
                int hoveredIndex = (int)ctx.CustomData["HoveredIndex"];
                // Add hover highlighting logic here
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}