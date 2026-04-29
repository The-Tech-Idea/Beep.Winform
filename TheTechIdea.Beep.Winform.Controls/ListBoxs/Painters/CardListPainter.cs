using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Card-Style list with elevated items, distinct card styling with shadows and borders
    /// </summary>
    internal class CardListPainter : FilledListBoxPainter
    {
        public override int GetPreferredItemHeight()
        {
            return Scale(60); // Taller for card appearance
        }

        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(Scale(16), Scale(8), Scale(16), Scale(8));
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // IMPORTANT: Clear and draw background first to prevent overlap
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Calculate layout
            var padding = GetPreferredPadding();
            var contentRect = Rectangle.Inflate(itemRect, -padding.Left, -padding.Top);

            // Render icon/image
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var imageBounds = new Rectangle(contentRect.X, contentRect.Y, Scale(44), Scale(44));
                DrawItemImage(g, imageBounds, item.ImagePath);
                contentRect.X += Scale(48);
                contentRect.Width -= Scale(48);
            }

            // Draw text
            Color textColor = isSelected ? Color.White : (_theme?.ListItemForeColor ?? Color.Black);
            DrawItemText(g, contentRect, item.Text, textColor, _owner.TextFont);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Card with rounded corners
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, Scale(8)))
            {
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.LightBlue;

                    // Multiple shadow layers for card elevation
                    var shadowRect1 = itemRect;
                    shadowRect1.Offset(0, Scale(3));
                    using (var shadowBrush = new LinearGradientBrush(shadowRect1,
                        Color.FromArgb(30, Color.Black),
                        Color.Transparent,
                        90f))
                    {
                        g.FillRectangle(shadowBrush, shadowRect1);
                    }

                    // Card background with gradient
                    using (var brush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(140, selColor.R, selColor.G, selColor.B),
                        Color.FromArgb(100, selColor.R, selColor.G, selColor.B),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    using (var pen = new Pen(selColor, Scale(3)))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    // Hover shadow
                    var shadowRect = itemRect;
                    shadowRect.Offset(0, Scale(2));
                    using (var shadowBrush = new LinearGradientBrush(shadowRect,
                        Color.FromArgb(20, Color.Black),
                        Color.Transparent,
                        90f))
                    {
                        g.FillRectangle(shadowBrush, shadowRect);
                    }

                    // Hover background
                    using (var brush = new SolidBrush(Color.FromArgb(248, 248, 248)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new Pen(_theme?.AccentColor ?? Color.Blue, Scale(2)))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal card style with subtle shadow
                    var shadowRect = itemRect;
                    shadowRect.Offset(0, Scale(1));
                    using (var shadowBrush = new LinearGradientBrush(shadowRect,
                        Color.FromArgb(10, Color.Black),
                        Color.Transparent,
                        90f))
                    {
                        g.FillRectangle(shadowBrush, shadowRect);
                    }

                    // Card background
                    using (var brush = new SolidBrush(Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    // Card border
                    using (var pen = new Pen(Color.FromArgb(225, 225, 225), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }
}
