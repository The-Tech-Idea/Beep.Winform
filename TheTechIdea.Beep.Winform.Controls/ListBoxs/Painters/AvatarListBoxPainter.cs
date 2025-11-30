using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Avatar list painter - items with circular avatar images and modern layout
    /// Ideal for user/contact lists with profile pictures
    /// </summary>
    internal class AvatarListBoxPainter : BaseListBoxPainter
    {
        private readonly int _avatarSize = 40;
        private readonly int _cornerRadius = 8;

        public override int GetPreferredItemHeight()
        {
            return Math.Max(_owner.Font.Height + 28, 56);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Draw background
            DrawAvatarItemBackground(g, itemRect, isHovered, isSelected);

            var padding = GetPreferredPadding();
            int contentX = itemRect.X + padding.Left;
            int contentY = itemRect.Y + (itemRect.Height - _avatarSize) / 2;

            // Draw avatar (circular)
            var avatarRect = new Rectangle(contentX, contentY, _avatarSize, _avatarSize);
            DrawAvatar(g, avatarRect, item, isSelected);

            contentX += _avatarSize + 12;

            // Get layout info for checkbox
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;

            // Text area
            int textWidth = itemRect.Right - contentX - padding.Right;
            if (_owner.ShowCheckBox && !checkRect.IsEmpty)
            {
                textWidth -= checkRect.Width + 8;
            }

            // Primary text (name)
            var primaryTextRect = new Rectangle(contentX, itemRect.Y + 10, textWidth, 20);
            Color primaryColor = isSelected 
                ? Color.White 
                : (_theme?.ListItemForeColor ?? Color.FromArgb(30, 30, 30));
            
            using (var boldFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size, FontStyle.Bold))
            {
                DrawItemText(g, primaryTextRect, item.Text ?? item.DisplayField, primaryColor, boldFont);
            }

            // Secondary text (subtext/email/role)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var secondaryTextRect = new Rectangle(contentX, itemRect.Y + 30, textWidth, 18);
                Color secondaryColor = isSelected 
                    ? Color.FromArgb(200, 255, 255, 255) 
                    : (_theme?.DisabledForeColor ?? Color.FromArgb(120, 120, 120));
                
                using (var smallFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1, FontStyle.Regular))
                {
                    DrawItemText(g, secondaryTextRect, item.SubText, secondaryColor, smallFont);
                }
            }

            // Checkbox (right side)
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.IsItemSelected(item);
                DrawModernCheckbox(g, checkRect, isChecked, isHovered, isSelected);
            }

            // Online status indicator (if badge is set)
            if (!string.IsNullOrEmpty(item.BadgeText))
            {
                DrawStatusIndicator(g, avatarRect, item.BadgeBackColor);
            }
        }

        private void DrawAvatarItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            var bgRect = Rectangle.Inflate(itemRect, -2, -1);
            
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(bgRect, _cornerRadius))
            {
                if (isSelected)
                {
                    var accentColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                    using (var brush = new SolidBrush(accentColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else if (isHovered)
                {
                    var hoverColor = _theme?.ListItemHoverBackColor ?? Color.FromArgb(245, 245, 245);
                    using (var brush = new SolidBrush(hoverColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        private void DrawAvatar(Graphics g, Rectangle avatarRect, SimpleItem item, bool isSelected)
        {
            // Draw circular clip
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(avatarRect);
                
                var state = g.Save();
                g.SetClip(path);

                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    // Draw actual avatar image
                    DrawItemImage(g, avatarRect, item.ImagePath);
                }
                else
                {
                    // Draw initials avatar
                    DrawInitialsAvatar(g, avatarRect, item.Text ?? item.DisplayField, isSelected);
                }

                g.Restore(state);
            }

            // Draw avatar border
            using (var pen = new Pen(isSelected 
                ? Color.FromArgb(100, 255, 255, 255) 
                : Color.FromArgb(60, 0, 0, 0), 2f))
            {
                g.DrawEllipse(pen, avatarRect);
            }
        }

        private void DrawInitialsAvatar(Graphics g, Rectangle avatarRect, string name, bool isSelected)
        {
            // Generate color from name
            int hash = (name ?? "").GetHashCode();
            Color[] avatarColors = new Color[]
            {
                Color.FromArgb(239, 83, 80),   // Red
                Color.FromArgb(171, 71, 188),  // Purple
                Color.FromArgb(66, 165, 245),  // Blue
                Color.FromArgb(38, 166, 154),  // Teal
                Color.FromArgb(102, 187, 106), // Green
                Color.FromArgb(255, 167, 38),  // Orange
                Color.FromArgb(141, 110, 99),  // Brown
            };
            Color bgColor = avatarColors[Math.Abs(hash) % avatarColors.Length];

            // Draw background
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillEllipse(brush, avatarRect);
            }

            // Draw initials
            string initials = GetInitials(name);
            using (var font = new Font(_owner.TextFont.FontFamily, 14, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(initials, font, brush, avatarRect, sf);
            }
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            }
            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        }

        private void DrawStatusIndicator(Graphics g, Rectangle avatarRect, Color statusColor)
        {
            // Draw status dot at bottom-right of avatar
            int dotSize = 12;
            var dotRect = new Rectangle(
                avatarRect.Right - dotSize + 2,
                avatarRect.Bottom - dotSize + 2,
                dotSize, dotSize);

            // White border
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillEllipse(brush, Rectangle.Inflate(dotRect, 2, 2));
            }

            // Status color
            using (var brush = new SolidBrush(statusColor))
            {
                g.FillEllipse(brush, dotRect);
            }
        }

        private void DrawModernCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered, bool isSelected)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkRect, 4))
            {
                if (isChecked)
                {
                    var accentColor = isSelected 
                        ? Color.White 
                        : (_theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215));
                    
                    using (var brush = new SolidBrush(isSelected ? Color.FromArgb(60, 255, 255, 255) : accentColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Checkmark
                    Color checkColor = isSelected ? Color.White : Color.White;
                    using (var pen = new Pen(checkColor, 2f))
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
                    // Unchecked border
                    Color borderColor = isSelected 
                        ? Color.FromArgb(150, 255, 255, 255) 
                        : (_theme?.BorderColor ?? Color.FromArgb(180, 180, 180));
                    
                    using (var pen = new Pen(borderColor, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Background is handled in DrawAvatarItemBackground
        }
    }
}

