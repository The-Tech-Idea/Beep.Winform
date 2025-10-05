using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Notion Minimal painter for NavBar
    /// Features: Ultra-minimal content-focused design with subtle backgrounds
    /// </summary>
    public sealed class NotionMinimalNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Notion Minimal";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Notion minimal colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.FromArgb(251, 251, 250);

            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(235, 235, 235);

            Color textColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.FromArgb(55, 53, 47);
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }
            
            // Ultra-subtle border
            using (var borderPen = new Pen(borderColor, 1f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(borderPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }
            
            // Draw Notion-styled minimal items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 8;
            int iconSize = 18;

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
            Color selectedColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(30, context.Theme.AccentColor)
                : Color.FromArgb(239, 239, 238);
            
            var itemRect = Rectangle.Inflate(selectedRect, -8, -3);
            int radius = 3; // Small radius

            using (var path = CreateRoundedPath(itemRect, radius))
            using (var bgBrush = new SolidBrush(selectedColor))
            {
                g.FillPath(bgBrush, path);
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            Color hoverColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(15, context.Theme.SideMenuHoverBackColor)
                : Color.FromArgb(247, 247, 246);
            
            var itemRect = Rectangle.Inflate(hoverRect, -8, -3);
            int radius = 3;

            using (var path = CreateRoundedPath(itemRect, radius))
            using (var hoverBrush = new SolidBrush(hoverColor))
            {
                g.FillPath(hoverBrush, path);
            }
        }
    }
}
