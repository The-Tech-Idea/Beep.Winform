using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Windows 11 Mica painter for NavBar
    /// Features: Mica material effect with layered translucent surfaces
    /// </summary>
    public sealed class Windows11MicaNavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Windows 11 Mica";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Windows 11 Mica translucent colors
            Color bgColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(230, context.Theme.BackgroundColor)
                : Color.FromArgb(243, 243, 243);

            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 120, 212);

            Color textColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.FromArgb(50, 50, 50);

            // Mica translucent background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Layered Mica effect with subtle noise texture
            DrawMicaTexture(g, bounds);

            // Subtle acrylic border
            using (var borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(borderPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            // Draw Windows 11 Style nav items
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 10;
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

                    // Windows 11: Smaller centered icon
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
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "Segoe UI Variable", 9f);
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
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Near, "Segoe UI Variable", 10f);
                    }

                    currentY += itemHeight + 2;
                }
            }
        }

        private void DrawMicaTexture(Graphics g, Rectangle bounds)
        {
            // Subtle noise pattern for Mica effect
            Random rand = new Random(bounds.GetHashCode());
            using (var noisePen = new Pen(Color.FromArgb(3, 0, 0, 0)))
            {
                for (int i = 0; i < 50; i++)
                {
                    int x = bounds.X + rand.Next(bounds.Width);
                    int y = bounds.Y + rand.Next(bounds.Height);
                    g.DrawRectangle(noisePen, x, y, 1, 1);
                }
            }
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(0, 120, 212);
            
            var pillRect = new Rectangle(
                selectedRect.X + 4,
                selectedRect.Y + 4,
                selectedRect.Width - 8,
                selectedRect.Height - 8);
            
            // Windows 11 Mica selection with subtle acrylic effect
            using (var path = CreateRoundedPath(pillRect, 6))
            {
                using (var br = new SolidBrush(Color.FromArgb(40, accentColor)))
                {
                    g.FillPath(br, path);
                }
                using (var pen = new Pen(Color.FromArgb(100, accentColor), 1.5f))
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
            
            using (var path = CreateRoundedPath(pillRect, 6))
            using (var br = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
            {
                g.FillPath(br, path);
            }
        }
    }
}
