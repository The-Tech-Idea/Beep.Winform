using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// macOS Big Sur painter for NavBar
    /// Features: Mac OS vibrancy effects with depth and translucency
    /// </summary>
    public sealed class MacOSBigSurNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "macOS Big Sur";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // macOS Big Sur translucent colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(245, context.Theme.BackgroundColor)
                : Color.FromArgb(246, 246, 246);

            Color separatorColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(60, 0, 0, 0);

            Color textColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.FromArgb(60, 60, 60);

            // macOS translucent background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Subtle gradient for depth
            if (!context.UseThemeColors)
            {
                using (var gradientBrush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(10, 255, 255, 255),
                    Color.FromArgb(5, 0, 0, 0),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(gradientBrush, bounds);
                }
            }

            // macOS separator
            using (var separatorPen = new Pen(separatorColor, 0.5f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(separatorPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(separatorPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            // Highlight line
            if (!context.UseThemeColors)
            {
                using (var highlightPen = new Pen(Color.FromArgb(30, 255, 255, 255), 1f))
                {
                    if (context.Orientation == NavBarOrientation.Horizontal)
                        g.DrawLine(highlightPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                    else
                        g.DrawLine(highlightPen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                }
            }

            // Draw macOS-style nav items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 10;
            int iconSize = 22;

            if (isHorizontal)
            {
                int itemWidth = context.ItemWidth > 0 ? context.ItemWidth : (bounds.Width - padding * 2) / itemCount;
                int currentX = bounds.Left + padding;

                for (int i = 0; i < itemCount; i++)
                {
                    var item = context.Items[i];
                    var itemRect = new Rectangle(currentX, bounds.Top + padding, itemWidth - 4, bounds.Height - padding * 2);

                    if (i == context.HoveredItemIndex) DrawHover(g, context, itemRect);
                    if (item == context.SelectedItem) DrawSelection(g, context, itemRect);

                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2, itemRect.Y + 6, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(
                            itemRect.X + padding, 
                            itemRect.Y + iconSize + 8, 
                            itemRect.Width - padding * 2, 
                            itemRect.Height - iconSize - 12);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "Segoe UI", 9f);
                    }

                    currentX += itemWidth;
                }
            }
            else
            {
                int itemHeight = context.ItemHeight;
                int currentY = bounds.Top + padding;

                for (int i = 0; i < itemCount; i++)
                {
                    var item = context.Items[i];
                    var itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, itemHeight);

                    if (i == context.HoveredItemIndex) DrawHover(g, context, itemRect);
                    if (item == context.SelectedItem) DrawSelection(g, context, itemRect);

                    int x = itemRect.X + padding;
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                        x += iconSize + padding;
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - padding, itemRect.Height);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Near, "Segoe UI", 10f);
                    }

                    currentY += itemHeight + 2;
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 122, 255);
            
            var itemRect = Rectangle.Inflate(selectedRect, -8, -4);
            int radius = itemRect.Height / 2; // Full capsule

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Drop shadow
                var shadowRect = new Rectangle(itemRect.X + 1, itemRect.Y + 2, itemRect.Width, itemRect.Height);
                using (var shadowPath = CreateRoundedPath(shadowRect, radius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(25, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }

                // macOS accent background
                using (var bgBrush = new SolidBrush(Color.FromArgb(230, accentColor)))
                {
                    g.FillPath(bgBrush, path);
                }

                // Subtle highlight
                using (var highlightBrush = new LinearGradientBrush(
                    new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 2),
                    Color.FromArgb(40, 255, 255, 255),
                    Color.Transparent,
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(highlightBrush, path);
                }
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuHoverBackColor
                : Color.FromArgb(238, 238, 238);

            var itemRect = Rectangle.Inflate(hoverRect, -8, -4);
            int radius = 8;

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                using (var hoverBrush = new SolidBrush(hoverColor))
                {
                    g.FillPath(hoverBrush, path);
                }

                // Subtle top highlight
                using (var highlightBrush = new LinearGradientBrush(
                    new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, itemRect.Height / 3),
                    Color.FromArgb(20, 255, 255, 255),
                    Color.Transparent,
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(highlightBrush, path);
                }
            }
        }
    }
}
