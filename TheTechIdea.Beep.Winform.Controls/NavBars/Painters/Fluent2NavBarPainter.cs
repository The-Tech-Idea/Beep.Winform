using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Fluent 2 painter for NavBar
    /// Features: Microsoft Fluent 2 design system with acrylic effects
    /// </summary>
    public class Fluent2NavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Fluent 2";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(240, context.Theme.BackgroundColor)
                : Color.FromArgb(243, 242, 241); // Fluent neutral gray
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }
            
            // Draw all nav items - Fluent 2 style
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 8;
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
                        var iconRect = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2, itemRect.Y + 4, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(itemRect.X + padding, itemRect.Y + iconSize + 8, itemRect.Width - padding * 2, itemRect.Height - iconSize - 12);
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

                    currentY += itemHeight + 4;
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            var pillRect = new Rectangle(
                selectedRect.X + 4,
                selectedRect.Y + 4,
                selectedRect.Width - 8,
                selectedRect.Height - 8);
            
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 120, 212); // Fluent blue
            
            // Subtle background with rounded corners
            using (var path = CreateRoundedPath(pillRect, 4))
            using (var bgBrush = new SolidBrush(Color.FromArgb(30, accentColor)))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Bottom accent line
            using (var pen = new Pen(accentColor, 2f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(pen, selectedRect.Left + 4, selectedRect.Bottom - 2, selectedRect.Right - 4, selectedRect.Bottom - 2);
                else
                    g.DrawLine(pen, selectedRect.Left + 1, selectedRect.Top + 4, selectedRect.Left + 1, selectedRect.Bottom - 4);
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            var pillRect = new Rectangle(
                hoverRect.X + 4,
                hoverRect.Y + 4,
                hoverRect.Width - 8,
                hoverRect.Height - 8);
            
            using (var path = CreateRoundedPath(pillRect, 4))
            using (var hoverBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
            {
                g.FillPath(hoverBrush, path);
            }
        }
    }
}
