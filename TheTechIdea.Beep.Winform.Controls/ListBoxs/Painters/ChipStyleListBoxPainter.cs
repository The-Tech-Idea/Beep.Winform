using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Chip style painter - items rendered as selectable chips similar to BeepMultiChipGroup
    /// Ideal for tag/category selection with wrap layout
    /// </summary>
    internal class ChipStyleListBoxPainter : BaseListBoxPainter
    {
        private readonly int _chipHeight = 32;
        private readonly int _chipPadding = 8;
        private readonly int _chipGap = 6;
        private readonly int _chipCornerRadius = 16;

        public override int GetPreferredItemHeight()
        {
            return _chipHeight + 4;
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Calculate chip bounds (centered in row)
            var chipRect = new Rectangle(
                itemRect.X + 4,
                itemRect.Y + (itemRect.Height - _chipHeight) / 2,
                itemRect.Width - 8,
                _chipHeight);

            // Draw chip background
            DrawChipBackground(g, chipRect, isHovered, isSelected);

            // Get layout info
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;

            int contentX = chipRect.X + _chipPadding;

            // Icon (if available)
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var chipIconRect = new Rectangle(contentX, chipRect.Y + (chipRect.Height - 20) / 2, 20, 20);
                DrawChipIcon(g, chipIconRect, item.ImagePath, isSelected);
                contentX += 24;
            }

            // Checkbox indicator (checkmark inside chip)
            if (_owner.ShowCheckBox && _owner.IsItemSelected(item))
            {
                var checkRect = new Rectangle(contentX, chipRect.Y + (chipRect.Height - 16) / 2, 16, 16);
                DrawChipCheckmark(g, checkRect, isSelected);
                contentX += 20;
            }

            // Text
            var textRect = new Rectangle(contentX, chipRect.Y, chipRect.Right - contentX - _chipPadding, chipRect.Height);
            Color textColor = isSelected 
                ? Color.White 
                : (_theme?.ListItemForeColor ?? Color.FromArgb(60, 60, 60));
            
            using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(item.Text ?? item.DisplayField, _owner.TextFont, new SolidBrush(textColor), textRect, sf);
            }

            // Close button (X) for selected chips
            if (isSelected && _owner.SelectionMode != SelectionModeEnum.Single)
            {
                var closeRect = new Rectangle(chipRect.Right - 24, chipRect.Y + (chipRect.Height - 16) / 2, 16, 16);
                DrawCloseButton(g, closeRect, isHovered);
            }
        }

        private void DrawChipBackground(Graphics g, Rectangle chipRect, bool isHovered, bool isSelected)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(chipRect, _chipCornerRadius))
            {
                if (isSelected)
                {
                    // Selected: solid accent color
                    var accentColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                    using (var brush = new SolidBrush(accentColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Subtle inner highlight
                    var highlightRect = new Rectangle(chipRect.X, chipRect.Y, chipRect.Width, chipRect.Height / 2);
                    using (var highlightPath = GraphicsExtensions.CreateRoundedRectanglePath(highlightRect, _chipCornerRadius))
                    using (var highlightBrush = new LinearGradientBrush(highlightRect,
                        Color.FromArgb(40, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(highlightBrush, highlightPath);
                    }
                }
                else if (isHovered)
                {
                    // Hovered: light background with accent border
                    var accentColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                    using (var brush = new SolidBrush(Color.FromArgb(20, accentColor)))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var pen = new Pen(Color.FromArgb(100, accentColor), 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Default: outlined chip
                    using (var brush = new SolidBrush(_theme?.BackgroundColor ?? Color.White))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(200, 200, 200), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private void DrawChipIcon(Graphics g, Rectangle iconRect, string imagePath, bool isSelected)
        {
            // Draw circular background
            using (var brush = new SolidBrush(isSelected 
                ? Color.FromArgb(40, 255, 255, 255) 
                : Color.FromArgb(20, 0, 0, 0)))
            {
                g.FillEllipse(brush, iconRect);
            }

            // Draw icon
            var innerRect = Rectangle.Inflate(iconRect, -2, -2);
            DrawItemImage(g, innerRect, imagePath);
        }

        private void DrawChipCheckmark(Graphics g, Rectangle checkRect, bool isSelected)
        {
            Color checkColor = isSelected ? Color.White : (_theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215));
            
            using (var pen = new Pen(checkColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                Point[] checkPoints = new Point[]
                {
                    new Point(checkRect.Left + 2, checkRect.Top + checkRect.Height / 2),
                    new Point(checkRect.Left + checkRect.Width / 2 - 1, checkRect.Bottom - 3),
                    new Point(checkRect.Right - 2, checkRect.Top + 3)
                };
                g.DrawLines(pen, checkPoints);
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle closeRect, bool isHovered)
        {
            // Draw X
            Color xColor = isHovered ? Color.White : Color.FromArgb(200, 255, 255, 255);
            using (var pen = new Pen(xColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, closeRect.Left + 3, closeRect.Top + 3, closeRect.Right - 3, closeRect.Bottom - 3);
                g.DrawLine(pen, closeRect.Right - 3, closeRect.Top + 3, closeRect.Left + 3, closeRect.Bottom - 3);
            }
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Background is handled in DrawChipBackground
        }
    }
}

