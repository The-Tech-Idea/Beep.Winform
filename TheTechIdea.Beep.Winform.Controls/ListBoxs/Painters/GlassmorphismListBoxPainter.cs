using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Glassmorphism style painter - frosted glass effect with blur-like backgrounds
    /// Modern UI trend with semi-transparent cards and subtle borders
    /// </summary>
    internal class GlassmorphismListBoxPainter : BaseListBoxPainter
    {
        private readonly int _cornerRadius = 12;
        private readonly int _itemCornerRadius = 8;

        public override int GetPreferredItemHeight()
        {
            return Math.Max(_owner.Font.Height + 20, 44);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Draw glassmorphism background
            DrawGlassBackground(g, itemRect, isHovered, isSelected);

            // Get layout info
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;
            Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;
            Rectangle textRect = info?.TextRect ?? itemRect;

            // Checkbox with glass style
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.IsItemSelected(item);
                DrawGlassCheckbox(g, checkRect, isChecked, isHovered);
            }

            // Icon with subtle shadow
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            // Text with proper contrast
            Color textColor = isSelected 
                ? Color.White 
                : (_theme?.ListItemForeColor ?? Color.FromArgb(40, 40, 40));
            
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);

            // Subtext if available
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var subRect = new Rectangle(textRect.X, textRect.Y + textRect.Height / 2 + 2, 
                    textRect.Width, textRect.Height / 2 - 4);
                var subColor = Color.FromArgb(140, textColor);
                using (var subFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1, FontStyle.Regular))
                {
                    DrawItemText(g, subRect, item.SubText, subColor, subFont);
                }
            }
        }

        private void DrawGlassBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Inset the item rect slightly
            var glassRect = Rectangle.Inflate(itemRect, -2, -1);

            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(glassRect, _itemCornerRadius))
            {
                if (isSelected)
                {
                    // Selected: vibrant glass with accent color
                    var accentColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                    
                    // Glass background
                    using (var brush = new LinearGradientBrush(glassRect,
                        Color.FromArgb(200, accentColor.R, accentColor.G, accentColor.B),
                        Color.FromArgb(160, accentColor.R, accentColor.G, accentColor.B),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Inner glow/highlight
                    var highlightRect = new Rectangle(glassRect.X, glassRect.Y, glassRect.Width, glassRect.Height / 3);
                    using (var highlightPath = GraphicsExtensions.CreateRoundedRectanglePath(highlightRect, _itemCornerRadius))
                    using (var highlightBrush = new LinearGradientBrush(highlightRect,
                        Color.FromArgb(80, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(highlightBrush, highlightPath);
                    }

                    // Glass border
                    using (var pen = new Pen(Color.FromArgb(120, 255, 255, 255), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    // Hovered: subtle frosted glass
                    using (var brush = new LinearGradientBrush(glassRect,
                        Color.FromArgb(60, 255, 255, 255),
                        Color.FromArgb(30, 255, 255, 255),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new Pen(Color.FromArgb(80, _theme?.AccentColor ?? Color.Gray), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Default: very subtle glass
                    using (var brush = new SolidBrush(Color.FromArgb(15, 255, 255, 255)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        private void DrawGlassCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkRect, 4))
            {
                if (isChecked)
                {
                    // Checked: accent color glass
                    var accentColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                    using (var brush = new SolidBrush(Color.FromArgb(200, accentColor)))
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
                    // Unchecked: glass border
                    using (var brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(100, _theme?.BorderColor ?? Color.Gray), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Background is handled in DrawGlassBackground
        }
    }
}

