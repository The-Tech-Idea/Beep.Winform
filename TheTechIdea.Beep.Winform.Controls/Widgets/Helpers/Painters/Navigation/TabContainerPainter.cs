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
    /// TabContainer - Tab navigation
    /// </summary>
    internal sealed class TabContainerPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public TabContainerPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 4;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            
            // Tab area
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
            // Tab container background
            using var bgBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 4);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.CustomData.ContainsKey("Items") ? 
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleTabs();
            
            int currentIndex = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] : 0;
            
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
            int tabWidth = ctx.ContentRect.Width / items.Count;
            var primaryColor = ctx.AccentColor ?? Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            
            using var tabFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Medium);
            using var activeTabBrush = new SolidBrush(Color.White);
            using var inactiveTabBrush = new SolidBrush(Color.FromArgb(248, 249, 250));
            using var activeTextBrush = new SolidBrush(primaryColor);
            using var inactiveTextBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool isActive = i == currentIndex;
                
                var tabRect = new Rectangle(
                    ctx.ContentRect.X + i * tabWidth,
                    ctx.ContentRect.Y,
                    tabWidth,
                    ctx.ContentRect.Height
                );
                
                // Modern tab styling with subtle elevation
                if (isActive)
                {
                    // Active tab shadow
                    var shadowRect = new Rectangle(tabRect.X + 1, tabRect.Y + 1, tabRect.Width, tabRect.Height);
                    using var shadowBrush = new SolidBrush(Color.FromArgb(10, Color.Black));
                    using var shadowPath = CreateRoundedPath(shadowRect, 6);
                    g.FillPath(shadowBrush, shadowPath);
                    
                    // Active tab background
                    using var tabPath = CreateRoundedPath(tabRect, 6);
                    g.FillPath(activeTabBrush, tabPath);
                    
                    // Active accent border
                    using var accentPen = new Pen(primaryColor, 2);
                    g.DrawPath(accentPen, tabPath);
                }
                else
                {
                    // Inactive tab background
                    using var tabPath = CreateRoundedPath(tabRect, 6);
                    g.FillPath(inactiveTabBrush, tabPath);
                }
                
                // Tab icon (optional)
                var iconName = GetTabIcon(item.Text, i);
                if (!string.IsNullOrEmpty(iconName))
                {
                    var iconRect = new Rectangle(tabRect.X + 8, tabRect.Y + (tabRect.Height - 16) / 2, 16, 16);
                    _imagePainter.DrawSvg(g, iconName, iconRect, 
                        isActive ? primaryColor : Color.FromArgb(140, Color.Black), 0.8f);
                }
                
                // Tab text
                var textBrush = isActive ? activeTextBrush : inactiveTextBrush;
                var textRect = new Rectangle(tabRect.X + (string.IsNullOrEmpty(GetTabIcon(item.Text, i)) ? 0 : 24), 
                    tabRect.Y, tabRect.Width - (string.IsNullOrEmpty(GetTabIcon(item.Text, i)) ? 0 : 24), tabRect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, tabFont, textBrush, textRect, format);
            }
        }

        private string GetTabIcon(string tabText, int index)
        {
            // Map common tab names to icons
            if (string.IsNullOrEmpty(tabText)) return null;
            
            var text = tabText.ToLower();
            if (text.Contains("overview") || text.Contains("dashboard")) return "home";
            if (text.Contains("detail") || text.Contains("info")) return "info";
            if (text.Contains("setting") || text.Contains("config")) return "settings";
            if (text.Contains("chart") || text.Contains("analytic")) return "bar-chart-2";
            if (text.Contains("user") || text.Contains("profile")) return "user";
            
            return null; // No icon for unrecognized tabs
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw tab separators for better visual separation
            if (ctx.CustomData.ContainsKey("ShowSeparators") && (bool)ctx.CustomData["ShowSeparators"])
            {
                var items = ctx.CustomData.ContainsKey("Items") ? 
                    (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleTabs();
                
                int tabWidth = ctx.ContentRect.Width / items.Count;
                using var separatorPen = new Pen(Color.FromArgb(30, Color.Gray), 1);
                
                for (int i = 1; i < items.Count; i++)
                {
                    int x = ctx.ContentRect.X + i * tabWidth;
                    g.DrawLine(separatorPen, x, ctx.ContentRect.Y + 8, x, ctx.ContentRect.Bottom - 8);
                }
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}