using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Discord ProgressBarStyle painter for NavBar
    /// Features: Discord-inspired design with blurple accent and dark gray backgrounds
    /// </summary>
    public sealed class DiscordStyleNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Discord ProgressBarStyle";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Discord dark gray colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.FromArgb(47, 49, 54);

            Color stripColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(Math.Max(0, context.Theme.BackgroundColor.R - 15), 
                                 Math.Max(0, context.Theme.BackgroundColor.G - 15), 
                                 Math.Max(0, context.Theme.BackgroundColor.B - 15))
                : Color.FromArgb(32, 34, 37);

            // Discord background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Darker strip (simulating server list side)
            if (context.Orientation == NavBarOrientation.Horizontal)
            {
                using (var stripBrush = new SolidBrush(stripColor))
                {
                    g.FillRectangle(stripBrush, bounds.X, bounds.Y, bounds.Width, 4);
                }
            }
            else
            {
                using (var stripBrush = new SolidBrush(stripColor))
                {
                    g.FillRectangle(stripBrush, bounds.X, bounds.Y, 4, bounds.Height);
                }
            }

            // Separator
            using (var separatorPen = new Pen(stripColor, 1f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(separatorPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(separatorPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            // Draw Discord-style nav items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 8;
            int iconSize = 24;
            Color textColor = Color.FromArgb(185, 187, 190);

            if (isHorizontal)
            {
                int itemWidth = context.ItemWidth > 0 ? context.ItemWidth : (bounds.Width - padding * 2) / itemCount;
                int currentX = bounds.Left + padding;

                for (int i = 0; i < itemCount; i++)
                {
                    var item = context.Items[i];
                    var itemRect = new Rectangle(currentX, bounds.Top + padding + 4, itemWidth - 4, bounds.Height - padding * 2 - 4);

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
                            itemRect.Y + iconSize + 6, 
                            itemRect.Width - padding * 2, 
                            itemRect.Height - iconSize - 10);
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
                    var itemRect = new Rectangle(bounds.Left + padding + 4, currentY, bounds.Width - padding * 2 - 4, itemHeight);

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
            Color blurple = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(88, 101, 242);

            var itemRect = Rectangle.Inflate(selectedRect, -8, -3);
            int radius = 4;

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Slightly lighter background
                using (var bgBrush = new SolidBrush(Color.FromArgb(66, 70, 77)))
                {
                    g.FillPath(bgBrush, path);
                }

                // Discord signature: WHITE pill indicator
                if (context.Orientation == NavBarOrientation.Horizontal)
                {
                    // Top pill indicator
                    int pillWidth = itemRect.Width - 16;
                    var pillRect = new Rectangle(itemRect.X + 8, itemRect.Y - 6, pillWidth, 4);
                    using (var pillBrush = new SolidBrush(Color.White))
                    using (var pillPath = CreateRoundedPath(pillRect, 2))
                    {
                        g.FillPath(pillBrush, pillPath);
                    }
                }
                else
                {
                    // Left pill indicator
                    int pillHeight = itemRect.Height - 8;
                    var pillRect = new Rectangle(itemRect.X - 8, itemRect.Y + 4, 4, pillHeight);
                    using (var pillBrush = new SolidBrush(Color.White))
                    using (var pillPath = CreateRoundedPath(pillRect, 2))
                    {
                        g.FillPath(pillBrush, pillPath);
                    }
                }

                // Subtle blurple tint
                using (var tintBrush = new SolidBrush(Color.FromArgb(10, blurple)))
                {
                    g.FillPath(tintBrush, path);
                }
            }
        }

        public override void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            var itemRect = Rectangle.Inflate(hoverRect, -8, -3);
            int radius = 4;

            using (var path = CreateRoundedPath(itemRect, radius))
            {
                // Hover background (lighter gray)
                using (var hoverBrush = new SolidBrush(Color.FromArgb(54, 57, 63)))
                {
                    g.FillPath(hoverBrush, path);
                }

                // Small pill indicator on hover
                if (context.Orientation == NavBarOrientation.Horizontal)
                {
                    int pillWidth = 8;
                    var pillRect = new Rectangle(itemRect.X + (itemRect.Width - pillWidth) / 2, itemRect.Y - 4, pillWidth, 3);
                    using (var pillBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                    using (var pillPath = CreateRoundedPath(pillRect, 2))
                    {
                        g.FillPath(pillBrush, pillPath);
                    }
                }
                else
                {
                    int pillHeight = 8;
                    var pillRect = new Rectangle(itemRect.X - 6, itemRect.Y + (itemRect.Height - pillHeight) / 2, 3, pillHeight);
                    using (var pillBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                    using (var pillPath = CreateRoundedPath(pillRect, 2))
                    {
                        g.FillPath(pillBrush, pillPath);
                    }
                }
            }
        }
    }
}
