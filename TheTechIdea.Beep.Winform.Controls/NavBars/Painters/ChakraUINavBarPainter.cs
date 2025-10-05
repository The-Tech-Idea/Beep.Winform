using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Chakra UI painter for NavBar
    /// Features: Modern design with smooth color modes and clean aesthetics
    /// </summary>
    public sealed class ChakraUINavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Chakra UI";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Chakra UI adaptive colors
            bool isDark = context.Theme != null && context.Theme.BackgroundColor.GetBrightness() < 0.5f;
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : isDark ? Color.FromArgb(26, 32, 44) : Color.FromArgb(247, 250, 252);

            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : isDark ? Color.FromArgb(45, 55, 72) : Color.FromArgb(226, 232, 240);

            Color textColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : isDark ? Color.FromArgb(237, 242, 247) : Color.FromArgb(45, 55, 72);
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }
            
            // Chakra UI style border
            using (var borderPen = new Pen(borderColor, 1f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(borderPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            // Optional elevation shadow
            if (context.EnableShadow)
            {
                Rectangle shadowRect = context.Orientation == NavBarOrientation.Horizontal
                    ? new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 20)
                    : new Rectangle(bounds.Right, bounds.Top, 20, bounds.Height);
                    
                using (var shadowBrush = new LinearGradientBrush(
                    shadowRect,
                    Color.FromArgb(40, 0, 0, 0),
                    Color.Transparent,
                    context.Orientation == NavBarOrientation.Horizontal ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(shadowBrush, shadowRect);
                }
            }
            
            // Draw Chakra-styled items
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
            bool isDark = context.Theme != null && context.Theme.BackgroundColor.GetBrightness() < 0.5f;
            Color startColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : isDark ? Color.FromArgb(19, 78, 74) : Color.FromArgb(204, 251, 241);

            Color endColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(Math.Max(0, context.Theme.AccentColor.R - 20),
                                 Math.Min(255, context.Theme.AccentColor.G + 10),
                                 Math.Min(255, context.Theme.AccentColor.B + 10))
                : isDark ? Color.FromArgb(17, 94, 89) : Color.FromArgb(153, 246, 228);

            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : isDark ? Color.FromArgb(45, 212, 191) : Color.FromArgb(20, 184, 166);
            
            var itemRect = Rectangle.Inflate(selectedRect, -8, -3);
            int radius = itemRect.Height / 2; // Full pill

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Gradient background
                using (var gradientBrush = new LinearGradientBrush(
                    itemRect, startColor, endColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(gradientBrush, path);
                }

                // Teal accent border
                using (var borderPen = new Pen(borderColor, 1.5f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            bool isDark = context.Theme != null && context.Theme.BackgroundColor.GetBrightness() < 0.5f;
            Color hoverColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuHoverBackColor
                : isDark ? Color.FromArgb(45, 55, 72) : Color.FromArgb(237, 242, 247);
            
            var itemRect = Rectangle.Inflate(hoverRect, -8, -3);
            int radius = 8;

            using (var path = CreateRoundedPath(itemRect, radius))
            using (var hoverBrush = new SolidBrush(hoverColor))
            {
                g.FillPath(hoverBrush, path);
            }
        }
    }
}
