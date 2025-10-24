using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Vercel Clean painter for NavBar
    /// Features: Monochrome black and white aesthetic with clean lines
    /// </summary>
    public sealed class VercelCleanNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Vercel Clean";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Vercel monochrome aesthetic
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.White;

            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(234, 234, 234);

            Color textColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.Black;
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }
            
            // Vercel Style border
            using (var borderPen = new Pen(borderColor, 1f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(borderPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }
            
            // Draw Vercel-styled items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 12;
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
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "Segoe UI", 8.5f, FontStyle.Bold);
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
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Near, "Segoe UI", 9.5f, FontStyle.Bold);
                    }

                    currentY += itemHeight + 2;
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            // Bold black line - signature Vercel Style
            using (var pen = new Pen(Color.Black, 2.5f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(pen, selectedRect.Left, selectedRect.Bottom - 2, selectedRect.Right, selectedRect.Bottom - 2);
                else
                    g.DrawLine(pen, selectedRect.Left, selectedRect.Top, selectedRect.Left, selectedRect.Bottom);
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
            {
                g.FillRectangle(brush, hoverRect);
            }
        }
    }
}
