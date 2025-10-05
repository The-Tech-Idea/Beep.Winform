using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Tailwind Card painter for NavBar
    /// Features: Utility-first design with clean borders and modern spacing
    /// </summary>
    public sealed class TailwindCardNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Tailwind Card";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Tailwind CSS colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.FromArgb(249, 250, 251);

            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(229, 231, 235);

            Color textColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.FromArgb(55, 65, 81);
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Drop shadow (Tailwind shadow-lg)
            if (context.EnableShadow)
            {
                Rectangle shadowRect = context.Orientation == NavBarOrientation.Horizontal
                    ? new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 15)
                    : new Rectangle(bounds.Right, bounds.Top, 15, bounds.Height);
                    
                using (var shadowBrush = new LinearGradientBrush(
                    shadowRect,
                    Color.FromArgb(50, 0, 0, 0),
                    Color.Transparent,
                    context.Orientation == NavBarOrientation.Horizontal ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(shadowBrush, shadowRect);
                }
            }
            
            // Tailwind border
            using (var borderPen = new Pen(borderColor, 1f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(borderPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }
            
            // Draw Tailwind-styled items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 12;
            int iconSize = 20;

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

                    currentY += itemHeight + 2;
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(40, context.Theme.AccentColor)
                : Color.FromArgb(239, 246, 255);

            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(37, 99, 235);
            
            var itemRect = Rectangle.Inflate(selectedRect, -12, -4);
            int radius = 6; // Tailwind rounded-md

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Background
                using (var bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, path);
                }

                // Border
                using (var borderPen = new Pen(borderColor, 1.5f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuHoverBackColor
                : Color.FromArgb(243, 244, 246);

            Color borderColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(50, context.Theme.SideMenuBorderColor)
                : Color.FromArgb(209, 213, 219);
            
            var itemRect = Rectangle.Inflate(hoverRect, -12, -4);
            int radius = 6;

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Hover background
                using (var hoverBrush = new SolidBrush(hoverColor))
                {
                    g.FillPath(hoverBrush, path);
                }
                
                // Subtle border
                using (var borderPen = new Pen(borderColor, 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }
    }
}
