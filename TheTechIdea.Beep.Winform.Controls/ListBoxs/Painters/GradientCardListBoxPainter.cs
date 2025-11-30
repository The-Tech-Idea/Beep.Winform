using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Gradient Card style painter - colorful gradient backgrounds with card-like items
    /// Modern and vibrant design with smooth color transitions
    /// </summary>
    internal class GradientCardListBoxPainter : BaseListBoxPainter
    {
        private readonly int _cornerRadius = 10;
        private readonly int _itemGap = 6;

        // Predefined gradient palettes
        private static readonly Color[][] GradientPalettes = new Color[][]
        {
            new[] { Color.FromArgb(102, 126, 234), Color.FromArgb(118, 75, 162) },  // Purple-Blue
            new[] { Color.FromArgb(67, 206, 162), Color.FromArgb(24, 90, 157) },    // Teal-Blue
            new[] { Color.FromArgb(255, 154, 158), Color.FromArgb(250, 208, 196) }, // Pink-Peach
            new[] { Color.FromArgb(161, 196, 253), Color.FromArgb(194, 233, 251) }, // Light Blue
            new[] { Color.FromArgb(245, 175, 25), Color.FromArgb(241, 39, 17) },    // Orange-Red
            new[] { Color.FromArgb(76, 184, 196), Color.FromArgb(60, 211, 173) },   // Cyan-Green
        };

        public override int GetPreferredItemHeight()
        {
            return Math.Max(_owner.Font.Height + 28, 52);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Get item index for gradient selection
            int itemIndex = _owner.ListItems.IndexOf(item);
            
            // Draw gradient card background
            DrawGradientCard(g, itemRect, itemIndex, isHovered, isSelected);

            // Get layout info
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;
            Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;
            Rectangle textRect = info?.TextRect ?? itemRect;

            // Checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.IsItemSelected(item);
                DrawGradientCheckbox(g, checkRect, isChecked, itemIndex);
            }

            // Icon with circular background
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawCircularIcon(g, iconRect, item.ImagePath, isSelected);
            }

            // Text - always white for contrast on gradient
            Color textColor = Color.White;
            if (!isSelected && !isHovered)
            {
                textColor = _theme?.ListItemForeColor ?? Color.FromArgb(40, 40, 40);
            }
            
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);

            // Subtext
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var subRect = new Rectangle(textRect.X, textRect.Y + textRect.Height / 2 + 2, 
                    textRect.Width, textRect.Height / 2 - 4);
                var subColor = Color.FromArgb(200, textColor);
                using (var subFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1, FontStyle.Regular))
                {
                    DrawItemText(g, subRect, item.SubText, subColor, subFont);
                }
            }
        }

        private void DrawGradientCard(Graphics g, Rectangle itemRect, int itemIndex, bool isHovered, bool isSelected)
        {
            var cardRect = Rectangle.Inflate(itemRect, -3, -2);
            
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(cardRect, _cornerRadius))
            {
                if (isSelected)
                {
                    // Use theme primary color gradient for selected
                    var primaryColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                    var secondaryColor = _theme?.AccentColor ?? DarkenColor(primaryColor, 0.3f);
                    
                    using (var brush = new LinearGradientBrush(cardRect,
                        primaryColor, secondaryColor, LinearGradientMode.Horizontal))
                    {
                        g.FillPath(brush, path);
                    }

                    // Glow effect
                    DrawCardGlow(g, cardRect, primaryColor);
                }
                else if (isHovered)
                {
                    // Subtle gradient on hover
                    var palette = GetPalette(itemIndex);
                    using (var brush = new LinearGradientBrush(cardRect,
                        Color.FromArgb(100, palette[0]),
                        Color.FromArgb(100, palette[1]),
                        LinearGradientMode.Horizontal))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new Pen(Color.FromArgb(80, palette[0]), 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Default: subtle background
                    using (var brush = new SolidBrush(_theme?.BackgroundColor ?? Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    // Subtle border
                    using (var pen = new Pen(Color.FromArgb(40, 0, 0, 0), 0.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private void DrawCardGlow(Graphics g, Rectangle cardRect, Color glowColor)
        {
            // Outer glow effect
            for (int i = 3; i >= 1; i--)
            {
                var glowRect = Rectangle.Inflate(cardRect, i, i);
                using (var path = GraphicsExtensions.CreateRoundedRectanglePath(glowRect, _cornerRadius + i))
                using (var pen = new Pen(Color.FromArgb(20 * (4 - i), glowColor), 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private void DrawCircularIcon(Graphics g, Rectangle iconRect, string imagePath, bool isSelected)
        {
            // Draw circular background
            var circleRect = iconRect;
            using (var brush = new SolidBrush(isSelected 
                ? Color.FromArgb(60, 255, 255, 255) 
                : Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillEllipse(brush, circleRect);
            }

            // Draw icon
            var innerRect = Rectangle.Inflate(circleRect, -4, -4);
            DrawItemImage(g, innerRect, imagePath);
        }

        private void DrawGradientCheckbox(Graphics g, Rectangle checkRect, bool isChecked, int itemIndex)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkRect, 4))
            {
                if (isChecked)
                {
                    var palette = GetPalette(itemIndex);
                    using (var brush = new LinearGradientBrush(checkRect,
                        palette[0], palette[1], LinearGradientMode.ForwardDiagonal))
                    {
                        g.FillPath(brush, path);
                    }

                    // Checkmark
                    using (var pen = new Pen(Color.White, 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        Point[] checkPoints = new Point[]
                        {
                            new Point(checkRect.Left + 4, checkRect.Top + checkRect.Height / 2),
                            new Point(checkRect.Left + checkRect.Width / 2 - 1, checkRect.Bottom - 4),
                            new Point(checkRect.Right - 4, checkRect.Top + 4)
                        };
                        g.DrawLines(pen, checkPoints);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(80, 0, 0, 0), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private Color[] GetPalette(int index)
        {
            return GradientPalettes[Math.Abs(index) % GradientPalettes.Length];
        }

        private Color DarkenColor(Color c, float amount)
        {
            int r = Math.Max(0, (int)(c.R * (1 - amount)));
            int g = Math.Max(0, (int)(c.G * (1 - amount)));
            int b = Math.Max(0, (int)(c.B * (1 - amount)));
            return Color.FromArgb(c.A, r, g, b);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Background is handled in DrawGradientCard
        }
    }
}

