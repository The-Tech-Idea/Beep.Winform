using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Gradient Modern painter for NavBar
    /// Features: Modern gradient backgrounds with glassmorphism effects
    /// </summary>
    public sealed class GradientModernNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Gradient Modern";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Gradient Modern: vibrant gradient backgrounds
            Color startColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.FromArgb(240, 242, 255);

            Color endColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(Math.Min(255, context.Theme.BackgroundColor.R + 15),
                                 Math.Max(0, context.Theme.BackgroundColor.G - 2),
                                 Math.Min(255, context.Theme.BackgroundColor.B + 10))
                : Color.FromArgb(255, 240, 245);

            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(99, 102, 241);

            Color textColor = Color.FromArgb(71, 85, 105);

            // Gradient background
            using (var bgBrush = new LinearGradientBrush(bounds, startColor, endColor, 
                context.Orientation == NavBarOrientation.Horizontal ? 0f : 90f))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Glassmorphism overlay
            using (var overlayBrush = new SolidBrush(Color.FromArgb(10, 255, 255, 255)))
            {
                g.FillRectangle(overlayBrush, bounds);
            }

            // Draw gradient-styled nav items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 12;
            int iconSize = 24;

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
                        var iconRect = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2, itemRect.Y + 8, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(
                            itemRect.X + padding, 
                            itemRect.Y + iconSize + 10, 
                            itemRect.Width - padding * 2, 
                            itemRect.Height - iconSize - 14);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "Segoe UI", 9f, FontStyle.Bold);
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
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Near, "Segoe UI", 10f, FontStyle.Bold);
                    }

                    currentY += itemHeight + 4;
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            Color startColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(147, 51, 234);

            Color endColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(Math.Max(0, context.Theme.AccentColor.R - 30),
                                 Math.Max(0, context.Theme.AccentColor.G - 30),
                                 Math.Min(255, context.Theme.AccentColor.B + 30))
                : Color.FromArgb(99, 102, 241);

            var pillRect = new Rectangle(
                selectedRect.X + 4,
                selectedRect.Y + 4,
                selectedRect.Width - 8,
                selectedRect.Height - 8);

            using (var path = CreateRoundedPath(pillRect, 20))
            {
                // Gradient fill
                using (var gradientBrush = new LinearGradientBrush(
                    pillRect, startColor, endColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(gradientBrush, path);
                }

                // Glass overlay
                using (var glassBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                {
                    g.FillPath(glassBrush, path);
                }

                // Gradient stroke
                using (var pen = new Pen(Color.FromArgb(150, startColor), 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            var pillRect = new Rectangle(
                hoverRect.X + 4,
                hoverRect.Y + 4,
                hoverRect.Width - 8,
                hoverRect.Height - 8);

            using (var path = CreateRoundedPath(pillRect, 20))
            using (var hoverBrush = new SolidBrush(Color.FromArgb(20, 100, 100, 255)))
            {
                g.FillPath(hoverBrush, path);
            }
        }
    }
}
