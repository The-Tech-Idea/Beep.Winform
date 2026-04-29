using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

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
            return Scale(_chipHeight) + Scale(4);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Calculate chip bounds (centered in row)
            int scaledChipH = Scale(_chipHeight);
            var chipRect = new Rectangle(
                itemRect.X + Scale(4),
                itemRect.Y + (itemRect.Height - scaledChipH) / 2,
                itemRect.Width - Scale(8),
                scaledChipH);

            // Draw chip background
            DrawChipBackground(g, chipRect, isHovered, isSelected);

            // Get layout info
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;

            int contentX = chipRect.X + Scale(_chipPadding);

            // Icon (if available)
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                var chipIconRect = new Rectangle(contentX, chipRect.Y + (chipRect.Height - Scale(20)) / 2, Scale(20), Scale(20));
                DrawChipIcon(g, chipIconRect, item.ImagePath, isSelected);
                contentX += Scale(24);
            }

            // Checkbox indicator (checkmark inside chip)
            if (_owner.ShowCheckBox && _owner.IsItemSelected(item))
            {
                var checkRect = new Rectangle(contentX, chipRect.Y + (chipRect.Height - Scale(16)) / 2, Scale(16), Scale(16));
                DrawChipCheckmark(g, checkRect, isSelected);
                contentX += Scale(20);
            }

            // Text
            var textRect = new Rectangle(contentX, chipRect.Y, chipRect.Right - contentX - Scale(_chipPadding), chipRect.Height);
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
                var closeRect = new Rectangle(chipRect.Right - Scale(24), chipRect.Y + (chipRect.Height - Scale(16)) / 2, Scale(16), Scale(16));
                DrawCloseButton(g, closeRect, isHovered);
            }
        }

        private void DrawChipBackground(Graphics g, Rectangle chipRect, bool isHovered, bool isSelected)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(chipRect, new CornerRadius(Scale(_chipCornerRadius))))
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
                    using (var highlightPath = GraphicsExtensions.CreateRoundedRectanglePath(highlightRect, new CornerRadius(Scale(_chipCornerRadius))))
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

            // Draw icon using StyledImagePainter circular rendering
            float cx = iconRect.X + iconRect.Width / 2f;
            float cy = iconRect.Y + iconRect.Height / 2f;
            float radius = (iconRect.Width / 2f) - 2;
            StyledImagePainter.PaintInCircle(g, cx, cy, radius, imagePath);
        }

        private void DrawChipCheckmark(Graphics g, Rectangle checkRect, bool isSelected)
        {
            Color checkColor = isSelected ? Color.White : (_theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215));
            int s2 = Scale(2);
            int s3 = Scale(3);
            
            using (var pen = new Pen(checkColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                Point[] checkPoints = new Point[]
                {
                    new Point(checkRect.Left + s2, checkRect.Top + checkRect.Height / 2),
                    new Point(checkRect.Left + checkRect.Width / 2 - Scale(1), checkRect.Bottom - s3),
                    new Point(checkRect.Right - s2, checkRect.Top + s3)
                };
                g.DrawLines(pen, checkPoints);
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle closeRect, bool isHovered)
        {
            // Draw X
            Color xColor = isHovered ? Color.White : Color.FromArgb(200, 255, 255, 255);
            int s3 = Scale(3);
            using (var pen = new Pen(xColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, closeRect.Left + s3, closeRect.Top + s3, closeRect.Right - s3, closeRect.Bottom - s3);
                g.DrawLine(pen, closeRect.Right - s3, closeRect.Top + s3, closeRect.Left + s3, closeRect.Bottom - s3);
            }
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Background is handled in DrawChipBackground
        }
    }
}

