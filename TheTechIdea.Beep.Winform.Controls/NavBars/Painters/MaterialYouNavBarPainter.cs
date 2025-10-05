using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters    
{
    /// <summary>
    /// Material You painter for NavBar
    /// Features: Google's Material You with dynamic color system
    /// </summary>
    public class MaterialYouNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Material You";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Material You tonal surface colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.FromArgb(254, 247, 255);

            Color primary = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(103, 80, 164);

            Color onSurface = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.FromArgb(33, 33, 33);

            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Material You dynamic color system - draw nav items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 12;
            int iconSize = 26;

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

                    // Material You: Large icons centered
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2, itemRect.Y + 8, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(
                            itemRect.X + padding, 
                            itemRect.Y + iconSize + 12, 
                            itemRect.Width - padding * 2, 
                            itemRect.Height - iconSize - 16);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "Segoe UI Variable", 10f);
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

                    int x = itemRect.X + padding + 8;
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                        x += iconSize + padding;
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - padding, itemRect.Height);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Near, "Segoe UI Variable", 11f);
                    }

                    currentY += itemHeight + 6;
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(103, 80, 164);
            
            var pillRect = new Rectangle(
                selectedRect.X + 8,
                selectedRect.Y + 8,
                selectedRect.Width - 16,
                selectedRect.Height - 16);
            
            // Material You signature: LARGE rounded pill (28px radius)
            using (var path = CreateRoundedPath(pillRect, 28))
            using (var br = new SolidBrush(Color.FromArgb(50, accentColor)))
            {
                g.FillPath(br, path);
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            var pillRect = new Rectangle(
                hoverRect.X + 8,
                hoverRect.Y + 8,
                hoverRect.Width - 16,
                hoverRect.Height - 16);
            
            // Large pill hover
            using (var path = CreateRoundedPath(pillRect, 28))
            using (var br = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                g.FillPath(br, path);
            }
        }
    }
}
