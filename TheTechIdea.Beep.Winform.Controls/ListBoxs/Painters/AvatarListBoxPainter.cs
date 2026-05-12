using System;
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
    /// Avatar list painter - items with circular avatar images and modern layout
    /// Ideal for user/contact lists with profile pictures
    /// </summary>
    internal class AvatarListBoxPainter : BaseListBoxPainter
    {
        private readonly int _avatarSize = ListBoxTokens.AvatarSize;
        private readonly int _cornerRadius = 8;

        public override int GetPreferredItemHeight()
        {
            return Math.Max(_owner.TextFont.Height + Scale(28), Scale(56));
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Draw avatar background
            DrawAvatarItemBackground(g, itemRect, isHovered, isSelected);

            var padding = GetPreferredPadding();
            int contentX = itemRect.X + padding.Left;
            int scaledAvatar = Scale(_avatarSize);
            int contentY = itemRect.Y + (itemRect.Height - scaledAvatar) / 2;

            // Draw avatar (circular)
            var avatarRect = new Rectangle(contentX, contentY, scaledAvatar, scaledAvatar);
            DrawAvatar(g, avatarRect, item, isSelected);

            contentX += scaledAvatar + Scale(ListBoxTokens.IconTextGap);

            // Get layout info for checkbox
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;

            // Text area
            int textWidth = itemRect.Right - contentX - padding.Right;
            if (_owner.ShowCheckBox && !checkRect.IsEmpty)
            {
                textWidth -= checkRect.Width + Scale(8);
            }

            // Primary text (name)
            var primaryTextRect = new Rectangle(contentX, itemRect.Y + Scale(10), textWidth, Scale(20));
            Color primaryColor = isSelected 
                ? (_theme?.OnPrimaryColor ?? Color.White)
                : (_theme?.ListItemForeColor ?? _theme?.ListForeColor ?? Color.FromArgb(30, 30, 30));
            
            using (var boldFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Bold))
            {
                DrawItemText(g, primaryTextRect, item.Text ?? item.DisplayField, primaryColor, boldFont);
            }

            // Secondary text (subtext/email/role)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var secondaryTextRect = new Rectangle(contentX, itemRect.Y + Scale(30), textWidth, Scale(18));
                Color secondaryColor = isSelected 
                    ? Color.FromArgb(ListBoxTokens.SubTextAlpha, _theme?.OnPrimaryColor ?? Color.White)
                    : (_theme?.DisabledForeColor ?? Color.FromArgb(120, 120, 120));
                
                using (var smallFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size - 1, FontStyle.Regular))
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
            var bgRect = Rectangle.Inflate(itemRect, -Scale(2), -Scale(1));
            
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(bgRect, new CornerRadius(Scale(_cornerRadius))))
            {
                if (isSelected)
                {
                    var accentColor = _theme?.AccentColor ?? _theme?.PrimaryColor ?? Color.DodgerBlue;
                    using (var brush = new SolidBrush(accentColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else if (isHovered)
                {
                    var hoverColor = _theme?.ListItemHoverBackColor ?? _theme?.BackgroundColor ?? Color.FromArgb(245, 245, 245);
                    using (var brush = new SolidBrush(hoverColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        private void DrawAvatar(Graphics g, Rectangle avatarRect, SimpleItem item, bool isSelected)
        {
            DrawCircularAvatar(g, avatarRect, item);

            // Draw avatar border
            using (var pen = new Pen(isSelected 
                ? Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, _theme?.OnPrimaryColor ?? Color.White)
                : Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, _theme?.BorderColor ?? Color.Gray), 2f))
            {
                g.DrawEllipse(pen, avatarRect);
            }
        }

        private void DrawStatusIndicator(Graphics g, Rectangle avatarRect, Color statusColor)
        {
            // Draw status dot at bottom-right of avatar
            int dotSize = Scale(12);
            var dotRect = new Rectangle(
                avatarRect.Right - dotSize + Scale(2),
                avatarRect.Bottom - dotSize + Scale(2),
                dotSize, dotSize);

            // White border
            using (var brush = new SolidBrush(_theme?.BackgroundColor ?? _owner?.BackColor ?? Color.White))
            {
                g.FillEllipse(brush, Rectangle.Inflate(dotRect, 2, 2));
            }

            // Status color
            Color effectiveStatus = statusColor != Color.Empty
                ? statusColor
                : (_theme?.SuccessColor ?? _theme?.AccentColor ?? Color.LimeGreen);
            using (var brush = new SolidBrush(effectiveStatus))
            {
                g.FillEllipse(brush, dotRect);
            }
        }

        private void DrawModernCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered, bool isSelected)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkRect, new CornerRadius(Scale(4))))
            {
                if (isChecked)
                {
                    var accentColor = isSelected 
                        ? (_theme?.OnPrimaryColor ?? Color.White)
                        : (_theme?.AccentColor ?? _theme?.PrimaryColor ?? Color.DodgerBlue);
                    
                    using (var brush = new SolidBrush(isSelected
                        ? Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, _theme?.OnPrimaryColor ?? Color.White)
                        : accentColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Checkmark
                    Color checkColor = _theme?.OnPrimaryColor ?? Color.White;
                    using (var pen = new Pen(checkColor, 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        int cp = Scale(4);
                        Point[] checkPoints = new Point[]
                        {
                            new Point(checkRect.Left + cp, checkRect.Top + checkRect.Height / 2),
                            new Point(checkRect.Left + checkRect.Width / 2 - Scale(1), checkRect.Bottom - cp),
                            new Point(checkRect.Right - cp, checkRect.Top + cp)
                        };
                        g.DrawLines(pen, checkPoints);
                    }
                }
                else
                {
                    // Unchecked border
                    Color borderColor = isSelected 
                        ? Color.FromArgb(ListBoxTokens.SubTextAlpha, _theme?.OnPrimaryColor ?? Color.White)
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

