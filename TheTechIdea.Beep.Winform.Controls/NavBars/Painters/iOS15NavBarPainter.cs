using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// iOS 15 painter for NavBar
    /// Features: Clean minimal design, subtle animations, translucent blur
    /// </summary>
    public class iOS15NavBarPainter : BaseNavBarPainter
    {
        public override string Name => "iOS 15";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // iOS 15 translucent colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(250, context.Theme.BackgroundColor)
                : Color.FromArgb(250, 255, 255, 255);

            Color separatorColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(60, 60, 67, 60);

            // iOS translucent background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Subtle gradient overlay (iOS feel)
            if (!context.UseThemeColors)
            {
                using (var gradientBrush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(5, 255, 255, 255),
                    Color.FromArgb(8, 0, 0, 0),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(gradientBrush, bounds);
                }
            }

            // iOS separator line
            using (var separatorPen = new Pen(separatorColor, 0.5f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(separatorPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(separatorPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            // Draw iOS-Style nav items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 8;
            int iconSize = 28;

            Color textColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.FromArgb(60, 60, 67);

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

                    // iOS: Large centered icon
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2, itemRect.Y + 4, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                    }

                    // iOS: Small text below icon
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(itemRect.X, itemRect.Y + iconSize + 6, itemRect.Width, itemRect.Height - iconSize - 10);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "SF Pro Text", 9f);
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

                    int x = itemRect.X + 10;
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                        x += iconSize + 12;
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 10, itemRect.Height);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Near, "SF Pro Display", 11f);
                    }

                    currentY += itemHeight + 2;
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            var pillRect = new Rectangle(
                selectedRect.X + 6,
                selectedRect.Y + 6,
                selectedRect.Width - 12,
                selectedRect.Height - 12);

            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 122, 255); // iOS blue

            FillRoundedRect(g, pillRect, 18, Color.FromArgb(25, accentColor));
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            var pillRect = new Rectangle(
                hoverRect.X + 6,
                hoverRect.Y + 6,
                hoverRect.Width - 12,
                hoverRect.Height - 12);

            FillRoundedRect(g, pillRect, 18, Color.FromArgb(10, 0, 0, 0));
        }
    }
}
