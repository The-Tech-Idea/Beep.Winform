using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Material Design 3 painter for NavBar
    /// Features: Tonal surfaces, elevation shadows, Material You colors
    /// </summary>
    public class Material3NavBarPainter : BaseNavBarPainter
    {
        public override string Name => "Material Design 3";

        public override void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Material 3 tonal surface colors
            Color surface = context.UseThemeColors && context.Theme != null
                ? context.Theme.BackgroundColor
                : Color.FromArgb(255, 251, 254);

            Color primary = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : Color.FromArgb(103, 80, 164);

            Color onSurface = context.UseThemeColors && context.Theme != null
                ? context.Theme.ForeColor
                : Color.FromArgb(28, 27, 31);

            // Tonal surface: blend of surface and primary
            var tonal = Color.FromArgb(20, primary);
            using (var bg = new SolidBrush(BlendColors(surface, tonal)))
            {
                g.FillRectangle(bg, bounds);
            }

            // Elevation shadow based on orientation
            if (context.EnableShadow)
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                {
                    var shadowRect = new Rectangle(bounds.X, bounds.Bottom - 4, bounds.Width, 4);
                    using (var shadowBrush = new LinearGradientBrush(shadowRect,
                        Color.FromArgb(60, 0, 0, 0), Color.Transparent, 90f))
                    {
                        g.FillRectangle(shadowBrush, shadowRect);
                    }
                }
                else
                {
                    var shadowRect = new Rectangle(bounds.Right - 12, bounds.Top, 12, bounds.Height);
                    using (var shadowBrush = new LinearGradientBrush(shadowRect,
                        Color.FromArgb(60, 0, 0, 0), Color.Transparent, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(shadowBrush, shadowRect);
                    }
                }
            }

            // MD3 keyline
            using (var keyline = new Pen(Color.FromArgb(50, onSurface), 1f))
            {
                if (context.Orientation == NavBarOrientation.Horizontal)
                    g.DrawLine(keyline, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                else
                    g.DrawLine(keyline, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            // Draw nav items with Material 3 Style
            if (context.Items == null || context.Items.Count == 0) return;

            bool isHorizontal = context.Orientation == NavBarOrientation.Horizontal;
            int itemCount = context.Items.Count;
            int padding = 12;
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

                    // MD3: centered icon above text
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2, itemRect.Y + 6, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(itemRect.X, itemRect.Y + iconSize + 10, itemRect.Width, itemRect.Height - iconSize - 14);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Center, "Roboto", 9f);
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

                    // MD3: icon left, text right
                    int x = itemRect.X + padding;
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                        DrawNavItemIcon(g, context, item, iconRect);
                        x += iconSize + 12;
                    }

                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        var textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - padding, itemRect.Height);
                        DrawNavItemText(g, context, item, textRect, StringAlignment.Near, "Roboto", 10f);
                    }

                    currentY += itemHeight + 4;
                }
            }
        }

        private Color BlendColors(Color a, Color b)
        {
            int r = (a.R * (255 - b.A) + b.R * b.A) / 255;
            int g = (a.G * (255 - b.A) + b.G * b.A) / 255;
            int bl = (a.B * (255 - b.A) + b.B * b.A) / 255;
            return Color.FromArgb(255, r, g, bl);
        }

        public override void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            // Material 3 pill indicator
            var pillRect = new Rectangle(
                selectedRect.X + 4,
                selectedRect.Y + 4,
                selectedRect.Width - 8,
                selectedRect.Height - 8);

            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : context.AccentColor;

            FillRoundedRect(g, pillRect, 20, Color.FromArgb(40, accentColor));
            
            // Accent indicator line (bottom for horizontal, left for vertical)
            if (context.Orientation == NavBarOrientation.Horizontal)
            {
                using (var pen = new Pen(accentColor, 3f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    int lineWidth = selectedRect.Width / 2;
                    int lineX = selectedRect.X + (selectedRect.Width - lineWidth) / 2;
                    g.DrawLine(pen, lineX, selectedRect.Bottom - 2, lineX + lineWidth, selectedRect.Bottom - 2);
                }
            }
            else
            {
                using (var pen = new Pen(accentColor, 3f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    int lineHeight = selectedRect.Height / 2;
                    int lineY = selectedRect.Y + (selectedRect.Height - lineHeight) / 2;
                    g.DrawLine(pen, selectedRect.Left + 2, lineY, selectedRect.Left + 2, lineY + lineHeight);
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

            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : context.AccentColor;

            FillRoundedRect(g, pillRect, 20, Color.FromArgb(15, accentColor));
        }
    }
}
