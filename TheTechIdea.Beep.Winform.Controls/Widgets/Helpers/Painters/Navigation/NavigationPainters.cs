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
                    g.FillRoundedRectangle(bgBrush, bgRect, 4);
                }
                
                // Item text
                var textRect = new Rectangle(x, (int)(y - textSize.Height / 2), (int)textSize.Width, (int)textSize.Height);
                g.DrawString(item.Text, font, brush, textRect);
                
                x += (int)textSize.Width + 8;
                
                // Draw separator
                if (!isLast)
                {
                    g.DrawString("/", regularFont, separatorBrush, new Point(x, (int)(y - textSize.Height / 2)));
                    x += 16;
                }
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

    /// <summary>
    /// StepIndicator - Multi-step process indicator with enhanced visual presentation
    /// </summary>
    internal sealed class StepIndicatorPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public StepIndicatorPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
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
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.CustomData.ContainsKey("Items") ? 
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleSteps();
            
            int currentIndex = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] : 1;
            
            if (!items.Any()) return;
            
            DrawModernStepIndicator(g, ctx, items, currentIndex);
        }

        private List<NavigationItem> CreateSampleSteps()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Setup", IsActive = false },
                new NavigationItem { Text = "Configuration", IsActive = true },
                new NavigationItem { Text = "Review", IsActive = false },
                new NavigationItem { Text = "Complete", IsActive = false }
            };
        }

        private void DrawModernStepIndicator(Graphics g, WidgetContext ctx, List<NavigationItem> items, int currentIndex)
        {
            int stepSize = 32;
            int stepSpacing = (ctx.ContentRect.Width - stepSize * items.Count) / Math.Max(items.Count - 1, 1);
            int y = ctx.ContentRect.Y + ctx.ContentRect.Height / 2;
            
            var primaryColor = ctx.AccentColor ?? Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            var successColor = Color.FromArgb(76, 175, 80);
            var pendingColor = Color.FromArgb(189, 189, 189);
            
            using var completedBrush = new SolidBrush(successColor);
            using var currentBrush = new SolidBrush(primaryColor);
            using var pendingBrush = new SolidBrush(pendingColor);
            using var textFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Medium);
            using var completedPen = new Pen(successColor, 2);
            using var currentPen = new Pen(primaryColor, 3);
            using var pendingPen = new Pen(pendingColor, 2);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int x = ctx.ContentRect.X + i * (stepSize + stepSpacing);
                
                // Draw connecting line
                if (i > 0)
                {
                    int lineY = y;
                    int lineStart = x - stepSpacing;
                    int lineEnd = x;
                    using var linePen = new Pen(i <= currentIndex ? ctx.AccentColor : Color.FromArgb(150, Color.Gray), 2);
                    g.DrawLine(linePen, lineStart, lineY, lineEnd, lineY);
                }
                
                // Draw step circle
                var stepRect = new Rectangle(x, y - stepSize / 2, stepSize, stepSize);
                Brush stepBrush = i < currentIndex ? completedBrush : 
                                 i == currentIndex ? currentBrush : pendingBrush;
                
                g.FillEllipse(stepBrush, stepRect);
                
                // Draw step border
                using var borderPen = new Pen(i <= currentIndex ? ctx.AccentColor : Color.FromArgb(150, Color.Gray), 2);
                g.DrawEllipse(borderPen, stepRect);
                
                // Draw step number
                string stepText = (i + 1).ToString();
                var textSize = g.MeasureString(stepText, textFont);
                var textPoint = new PointF(x + stepSize / 2 - textSize.Width / 2, y - textSize.Height / 2);
                
                Color numberColor = i == currentIndex ? ctx.AccentColor : Color.White;
                using var numberBrush = new SolidBrush(numberColor);
                g.DrawString(stepText, textFont, numberBrush, textPoint);
                
                // Draw step label
                if (!string.IsNullOrEmpty(item.Text))
                {
                    var labelSize = g.MeasureString(item.Text, textFont);
                    var labelPoint = new PointF(x + stepSize / 2 - labelSize.Width / 2, y + stepSize / 2 + 4);
                    g.DrawString(item.Text, textFont, textBrush, labelPoint);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw progress percentage
            var items = ctx.CustomData.ContainsKey("Items") ? 
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleSteps();
            int currentIndex = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] : 1;
                
            float progress = (float)currentIndex / Math.Max(items.Count - 1, 1);
            
            if (ctx.CustomData.ContainsKey("ShowProgress") && (bool)ctx.CustomData["ShowProgress"])
            {
                using var progressFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Medium);
                using var progressBrush = new SolidBrush(Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243));
                
                var progressText = $"{progress:P0}";
                var progressRect = new Rectangle(ctx.ContentRect.Right - 50, ctx.ContentRect.Y, 50, 20);
                var format = new StringFormat { Alignment = StringAlignment.Far };
                g.DrawString(progressText, progressFont, progressBrush, progressRect, format);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

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

    /// <summary>
    /// Pagination - Page navigation
    /// </summary>
    internal sealed class PaginationPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public PaginationPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
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
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            int currentPage = ctx.CustomData.ContainsKey("CurrentIndex") ? 
                (int)ctx.CustomData["CurrentIndex"] + 1 : 1;
            
            // For demo, assume 5 pages
            int totalPages = 5;
            int buttonSize = 32;
            int buttonSpacing = 4;
            
            using var pageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var activePageBrush = new SolidBrush(ctx.AccentColor);
            using var inactivePageBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
            using var activeTextBrush = new SolidBrush(Color.White);
            using var inactiveTextBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            
            int totalWidth = totalPages * buttonSize + (totalPages - 1) * buttonSpacing;
            int startX = ctx.ContentRect.X + (ctx.ContentRect.Width - totalWidth) / 2;
            int y = ctx.ContentRect.Y + (ctx.ContentRect.Height - buttonSize) / 2;
            
            for (int i = 1; i <= totalPages; i++)
            {
                bool isActive = i == currentPage;
                int x = startX + (i - 1) * (buttonSize + buttonSpacing);
                
                var pageRect = new Rectangle(x, y, buttonSize, buttonSize);
                
                // Draw page button background
                var pageBrush = isActive ? activePageBrush : inactivePageBrush;
                using var pagePath = CreateRoundedPath(pageRect, 4);
                g.FillPath(pageBrush, pagePath);
                
                // Draw page number
                var textBrush = isActive ? activeTextBrush : inactiveTextBrush;
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(i.ToString(), pageFont, textBrush, pageRect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw prev/next arrows
        }
    }

    // Modern navigation painter implementations
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
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.FromArgb(250, 250, 250));
            using var borderPen = new Pen(Color.FromArgb(30, Color.Gray), 1);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
            g.DrawRectangle(borderPen, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.CustomData.ContainsKey("Items") ?
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleMenuItems();

            if (!items.Any()) return;

            int itemWidth = ctx.ContentRect.Width / items.Count;
            using var menuFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Medium);
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);

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
                using var textBrush = new SolidBrush(item.IsActive ? primaryColor : Theme?.ForeColor ?? Color.Black);
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
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var items = ctx.CustomData.ContainsKey("Items") ?
                (List<NavigationItem>)ctx.CustomData["Items"] : CreateSampleSidebarItems();

            int activeIndex = ctx.CustomData.ContainsKey("ActiveIndex") ?
                (int)ctx.CustomData["ActiveIndex"] : 0;

            if (!items.Any()) return;

            int itemHeight = 36;
            int spacing = 4;
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);

            using var navFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Medium);

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
                var textRect = new Rectangle(itemRect.X + 40, itemRect.Y, itemRect.Width - 40, itemRect.Height);
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
            var text = itemText?.ToLower() ?? "";
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



    internal sealed class ProcessFlowPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ProcessFlowPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 12, ctx.DrawingRect.Y + 12,
                ctx.DrawingRect.Width - 24, ctx.DrawingRect.Height - 24);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var processes = ctx.CustomData.ContainsKey("Processes") ?
                (List<NavigationItem>)ctx.CustomData["Processes"] : CreateSampleProcessFlow();

            int activeProcess = ctx.CustomData.ContainsKey("ActiveProcess") ?
                (int)ctx.CustomData["ActiveProcess"] : 1;

            if (!processes.Any()) return;

            DrawFlowDiagram(g, ctx, processes, activeProcess);
        }

        private List<NavigationItem> CreateSampleProcessFlow()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "Input", IsActive = false },
                new NavigationItem { Text = "Process", IsActive = true },
                new NavigationItem { Text = "Validate", IsActive = false },
                new NavigationItem { Text = "Output", IsActive = false }
            };
        }

        private void DrawFlowDiagram(Graphics g, WidgetContext ctx, List<NavigationItem> processes, int activeIndex)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            var successColor = Color.FromArgb(76, 175, 80);
            var pendingColor = Color.FromArgb(189, 189, 189);

            int nodeWidth = 80;
            int nodeHeight = 50;
            int spacing = (ctx.ContentRect.Width - nodeWidth * processes.Count) / Math.Max(processes.Count - 1, 1);

            using var processFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Medium);

            for (int i = 0; i < processes.Count; i++)
            {
                var process = processes[i];
                bool isCompleted = i < activeIndex;
                bool isActive = i == activeIndex;
                bool isPending = i > activeIndex;

                int x = ctx.ContentRect.X + i * (nodeWidth + spacing);
                int y = ctx.ContentRect.Y + (ctx.ContentRect.Height - nodeHeight) / 2;
                var nodeRect = new Rectangle(x, y, nodeWidth, nodeHeight);

                // Node styling
                Color nodeColor = isCompleted ? successColor : isActive ? primaryColor : pendingColor;
                using var nodeBrush = new SolidBrush(Color.FromArgb(20, nodeColor));
                using var nodePen = new Pen(nodeColor, isActive ? 3 : 2);
                using var nodePath = CreateRoundedPath(nodeRect, 8);

                // Node shadow for active states
                if (isActive || isCompleted)
                {
                    var shadowRect = new Rectangle(nodeRect.X + 2, nodeRect.Y + 2, nodeRect.Width, nodeRect.Height);
                    using var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
                    using var shadowPath = CreateRoundedPath(shadowRect, 8);
                    g.FillPath(shadowBrush, shadowPath);
                }

                g.FillPath(nodeBrush, nodePath);
                g.DrawPath(nodePen, nodePath);

                // Process icon
                var iconRect = new Rectangle(x + (nodeWidth - 20) / 2, y + 8, 20, 20);
                string iconName = GetProcessIcon(process.Text, i);
                _imagePainter.DrawSvg(g, iconName, iconRect, nodeColor, 0.9f);

                // Process label
                using var textBrush = new SolidBrush(nodeColor);
                var textRect = new Rectangle(x, y + 28, nodeWidth, 20);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(process.Text, processFont, textBrush, textRect, format);

                // Flow arrow (except for last process)
                if (i < processes.Count - 1)
                {
                    var arrowColor = i < activeIndex ? successColor : pendingColor;
                    using var arrowPen = new Pen(arrowColor, 2);
                    
                    int arrowStartX = nodeRect.Right + 4;
                    int arrowEndX = nodeRect.Right + spacing - 4;
                    int arrowY = y + nodeHeight / 2;
                    
                    // Arrow line
                    g.DrawLine(arrowPen, arrowStartX, arrowY, arrowEndX, arrowY);
                    
                    // Arrow head
                    var arrowHead = new Point[] {
                        new Point(arrowEndX - 6, arrowY - 4),
                        new Point(arrowEndX, arrowY),
                        new Point(arrowEndX - 6, arrowY + 4)
                    };
                    using var arrowBrush = new SolidBrush(arrowColor);
                    g.FillPolygon(arrowBrush, arrowHead);
                }
            }
        }

        private string GetProcessIcon(string processText, int index)
        {
            var text = processText?.ToLower() ?? "";
            if (text.Contains("input")) return "upload";
            if (text.Contains("process")) return "cpu";
            if (text.Contains("validate")) return "shield-check";
            if (text.Contains("output")) return "download";
            if (text.Contains("decision")) return "help-circle";
            return "circle";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

    internal sealed class TreeNavigationPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public TreeNavigationPainter()
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
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var treeItems = ctx.CustomData.ContainsKey("TreeItems") ?
                (List<TreeNodeItem>)ctx.CustomData["TreeItems"] : CreateSampleTreeStructure();

            if (!treeItems.Any()) return;

            DrawTreeStructure(g, ctx, treeItems, 0, 0);
        }

        private List<TreeNodeItem> CreateSampleTreeStructure()
        {
            return new List<TreeNodeItem>
            {
                new TreeNodeItem { Text = "Root", Level = 0, IsExpanded = true, HasChildren = true },
                new TreeNodeItem { Text = "Documents", Level = 1, IsExpanded = true, HasChildren = true },
                new TreeNodeItem { Text = "Report.pdf", Level = 2, IsExpanded = false, HasChildren = false },
                new TreeNodeItem { Text = "Images", Level = 1, IsExpanded = false, HasChildren = true },
                new TreeNodeItem { Text = "Settings", Level = 1, IsExpanded = false, HasChildren = false }
            };
        }

        private int DrawTreeStructure(Graphics g, WidgetContext ctx, List<TreeNodeItem> items, int startY, int currentIndex)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            int itemHeight = 24;
            int indentSize = 20;
            
            using var treeFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var textBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            using var linePen = new Pen(Color.FromArgb(100, Color.Gray), 1);

            int currentY = startY;

            foreach (var item in items)
            {
                if (currentY + itemHeight > ctx.ContentRect.Bottom) break;

                int x = ctx.ContentRect.X + item.Level * indentSize;
                var itemRect = new Rectangle(x, ctx.ContentRect.Y + currentY, 
                    ctx.ContentRect.Width - item.Level * indentSize, itemHeight);

                // Tree line connections
                if (item.Level > 0)
                {
                    int lineX = ctx.ContentRect.X + (item.Level - 1) * indentSize + 8;
                    g.DrawLine(linePen, lineX, itemRect.Y, lineX + 12, itemRect.Y + itemHeight / 2);
                    g.DrawLine(linePen, lineX, itemRect.Y + itemHeight / 2, lineX + 12, itemRect.Y + itemHeight / 2);
                }

                // Expand/collapse icon
                if (item.HasChildren)
                {
                    var expandRect = new Rectangle(x, itemRect.Y + (itemHeight - 12) / 2, 12, 12);
                    string expandIcon = item.IsExpanded ? "chevron-down" : "chevron-right";
                    _imagePainter.DrawSvg(g, expandIcon, expandRect, primaryColor, 0.8f);
                }

                // Node icon
                var iconRect = new Rectangle(x + (item.HasChildren ? 16 : 4), itemRect.Y + (itemHeight - 16) / 2, 16, 16);
                string nodeIcon = GetTreeNodeIcon(item);
                _imagePainter.DrawSvg(g, nodeIcon, iconRect, 
                    item.HasChildren ? primaryColor : Color.FromArgb(140, Theme?.ForeColor ?? Color.Black), 0.8f);

                // Node text
                var textRect = new Rectangle(iconRect.Right + 4, itemRect.Y, 
                    itemRect.Width - (iconRect.Right + 4 - itemRect.X), itemHeight);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, treeFont, textBrush, textRect, format);

                currentY += itemHeight;
            }

            return currentY;
        }

        private string GetTreeNodeIcon(TreeNodeItem item)
        {
            if (item.HasChildren && item.IsExpanded) return "folder-open";
            if (item.HasChildren) return "folder";
            
            var text = item.Text?.ToLower() ?? "";
            if (text.Contains(".pdf")) return "file-text";
            if (text.Contains(".jpg") || text.Contains(".png") || text.Contains("image")) return "image";
            if (text.Contains(".doc")) return "file-text";
            if (text.Contains("setting")) return "settings";
            
            return "file";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

    internal sealed class QuickActionsPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public QuickActionsPainter()
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
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.FromArgb(250, 250, 250));
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);

            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(20, Color.Gray), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            var actions = ctx.CustomData.ContainsKey("Actions") ?
                (List<NavigationItem>)ctx.CustomData["Actions"] : CreateSampleQuickActions();

            if (!actions.Any()) return;

            DrawQuickActionGrid(g, ctx, actions);
        }

        private List<NavigationItem> CreateSampleQuickActions()
        {
            return new List<NavigationItem>
            {
                new NavigationItem { Text = "New", IsActive = false },
                new NavigationItem { Text = "Save", IsActive = false },
                new NavigationItem { Text = "Print", IsActive = false },
                new NavigationItem { Text = "Share", IsActive = false }
            };
        }

        private void DrawQuickActionGrid(Graphics g, WidgetContext ctx, List<NavigationItem> actions)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            int cols = Math.Min(actions.Count, 4); // Max 4 columns
            int rows = (int)Math.Ceiling((double)actions.Count / cols);
            
            int buttonSize = Math.Min((ctx.ContentRect.Width - (cols - 1) * 8) / cols, 
                                    (ctx.ContentRect.Height - (rows - 1) * 8) / rows);
            
            using var actionFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Medium);
            
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                int col = i % cols;
                int row = i / cols;
                
                int x = ctx.ContentRect.X + col * (buttonSize + 8);
                int y = ctx.ContentRect.Y + row * (buttonSize + 8);
                var actionRect = new Rectangle(x, y, buttonSize, buttonSize);
                
                // Action button background
                using var actionBrush = new SolidBrush(Color.FromArgb(10, primaryColor));
                using var actionPath = CreateRoundedPath(actionRect, 8);
                g.FillPath(actionBrush, actionPath);
                
                // Hover effect border
                using var hoverPen = new Pen(Color.FromArgb(30, primaryColor), 1);
                g.DrawPath(hoverPen, actionPath);
                
                // Action icon
                var iconSize = buttonSize / 2;
                var iconRect = new Rectangle(x + (buttonSize - iconSize) / 2, y + 8, iconSize, iconSize);
                string iconName = GetQuickActionIcon(action.Text);
                _imagePainter.DrawSvg(g, iconName, iconRect, primaryColor, 0.9f);
                
                // Action label
                using var textBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                var textRect = new Rectangle(x, y + iconRect.Bottom + 4, buttonSize, 20);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(action.Text, actionFont, textBrush, textRect, format);
            }
        }

        private string GetQuickActionIcon(string actionText)
        {
            var text = actionText?.ToLower() ?? "";
            if (text.Contains("new") || text.Contains("add")) return "plus";
            if (text.Contains("save")) return "save";
            if (text.Contains("print")) return "printer";
            if (text.Contains("share")) return "share-2";
            if (text.Contains("edit")) return "edit";
            if (text.Contains("delete")) return "trash-2";
            if (text.Contains("copy")) return "copy";
            if (text.Contains("search")) return "search";
            return "zap";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

    // Supporting classes for navigation painters
    public class NavigationItem
    {
        public string Text { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string IconName { get; set; } = string.Empty;
        public object Data { get; set; }
    }

    public class TreeNodeItem
    {
        public string Text { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsExpanded { get; set; }
        public bool HasChildren { get; set; }
        public string IconName { get; set; } = string.Empty;
        public object Data { get; set; }
    }
}