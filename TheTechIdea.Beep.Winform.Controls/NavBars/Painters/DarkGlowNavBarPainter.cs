using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Dark Glow painter for NavBar
    /// Features: Dark background with neon glow effects for a cyberpunk aesthetic
    /// </summary>
    public sealed class DarkGlowNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Dark Glow";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Dark Glow cyberpunk colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.FromArgb(18, 18, 18);

            Color glowColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 255, 200);

            // Deep dark background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Subtle grid texture pattern
            DrawGridPattern(g, bounds);

            // Glowing edge based on orientation
            if (context.Orientation == NavBarOrientation.Horizontal)
            {
                using (var glowBrush = new LinearGradientBrush(
                    new Rectangle(bounds.X, bounds.Bottom - 4, bounds.Width, 4),
                    Color.FromArgb(150, glowColor),
                    Color.FromArgb(0, glowColor),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(glowBrush, bounds.X, bounds.Bottom - 4, bounds.Width, 4);
                }

                // Accent line
                using (var accentPen = new Pen(glowColor, 2f))
                {
                    g.DrawLine(accentPen, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
                }
            }
            else
            {
                using (var glowBrush = new LinearGradientBrush(
                    new Rectangle(bounds.X, bounds.Y, 6, bounds.Height),
                    Color.FromArgb(150, glowColor),
                    Color.FromArgb(0, glowColor),
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(glowBrush, bounds.X, bounds.Y, 6, bounds.Height);
                }

                using (var accentPen = new Pen(glowColor, 2f))
                {
                    g.DrawLine(accentPen, bounds.X, bounds.Y, bounds.X, bounds.Bottom);
                }
            }

            // Draw nav items with glow effects
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 12;
            int iconSize = 24;
            Color textColor = Color.FromArgb(200, 200, 200);

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

                    currentY += itemHeight + 4;
                }
            }
        }

        private void DrawGridPattern(Graphics g, Rectangle bounds)
        {
            using (var gridPen = new Pen(Color.FromArgb(8, 255, 255, 255), 1f))
            {
                int gridSize = 20;
                for (int y = bounds.Y; y < bounds.Bottom; y += gridSize)
                {
                    g.DrawLine(gridPen, bounds.X, y, bounds.Right, y);
                }
                for (int x = bounds.X; x < bounds.Right; x += gridSize)
                {
                    g.DrawLine(gridPen, x, bounds.Y, x, bounds.Bottom);
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 255, 200);

            var itemRect = Rectangle.Inflate(selectedRect, -8, -3);
            int radius = 6;

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Multi-layer outer glow effect
                for (int i = 3; i > 0; i--)
                {
                    var glowRect = Rectangle.Inflate(itemRect, i * 2, i * 2);
                    using (var glowPath = CreateRoundedPath(glowRect, radius + i))
                    using (var glowBrush = new SolidBrush(Color.FromArgb(30 / i, accentColor)))
                    {
                        g.FillPath(glowBrush, glowPath);
                    }
                }

                // Inner gradient fill
                using (var innerBrush = new LinearGradientBrush(
                    itemRect,
                    Color.FromArgb(80, accentColor),
                    Color.FromArgb(40, accentColor),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(innerBrush, path);
                }

                // Glowing border
                using (var borderPen = new Pen(Color.FromArgb(200, accentColor), 2f))
                {
                    g.DrawPath(borderPen, path);
                }

                // Inner highlight
                var highlightRect = new Rectangle(itemRect.X + 2, itemRect.Y + 2, itemRect.Width - 4, itemRect.Height / 2);
                using (var highlightBrush = new LinearGradientBrush(
                    highlightRect,
                    Color.FromArgb(60, 255, 255, 255),
                    Color.Transparent,
                    LinearGradientMode.Vertical))
                using (var highlightPath = CreateRoundedPath(highlightRect, radius - 1))
                {
                    g.SetClip(path);
                    g.FillPath(highlightBrush, highlightPath);
                    g.ResetClip();
                }
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 255, 200);

            var itemRect = Rectangle.Inflate(hoverRect, -8, -3);
            int radius = 6;

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Subtle outer glow
                for (int i = 2; i > 0; i--)
                {
                    var glowRect = Rectangle.Inflate(itemRect, i, i);
                    using (var glowPath = CreateRoundedPath(glowRect, radius + i))
                    using (var glowBrush = new SolidBrush(Color.FromArgb(15 / i, accentColor)))
                    {
                        g.FillPath(glowBrush, glowPath);
                    }
                }

                // Dark hover fill
                using (var hoverBrush = new SolidBrush(Color.FromArgb(30, 30, 30)))
                {
                    g.FillPath(hoverBrush, path);
                }

                // Subtle border glow
                using (var borderPen = new Pen(Color.FromArgb(80, accentColor), 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }
    }
}
