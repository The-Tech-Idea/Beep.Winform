using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Team members list with avatars (from image 1 - top right)
    /// Shows user avatars on the right and selection state
    /// </summary>
    internal class TeamMembersPainter : OutlinedListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Background and divider come from OutlinedListBoxPainter base
            DrawItemBackground(g, itemRect, isHovered, isSelected);

            // Use precomputed layout for checkbox, but custom text to reserve right avatar space
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;

            // Checkbox (left)
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawCheckbox(g, checkRect, isChecked, isHovered);
            }

            // Compute text rect starting after checkbox area and reserving space for right-side avatar
            int leftStart = !checkRect.IsEmpty ? (checkRect.Right + 8) : (itemRect.Left + 12);

            // Avatar space on the right
            int baseAvatar = (_owner.ImageSize > 0 ? _owner.ImageSize : 28);
            int avatarSize = Math.Max(20, Math.Min(40, baseAvatar));
            int avatarPadding = 12;
            int avatarSpace = !string.IsNullOrEmpty(item.ImagePath) ? (avatarSize + avatarPadding) : 0;

            Rectangle textRect = new Rectangle(leftStart, itemRect.Y, Math.Max(0, itemRect.Right - leftStart - avatarSpace), itemRect.Height);
            Color textColor = isSelected ? Color.White : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);

            // Draw circular avatar on the right using StyledImagePainter
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle avatarRect = new Rectangle(itemRect.Right - avatarSize - avatarPadding,
                    itemRect.Y + (itemRect.Height - avatarSize) / 2, avatarSize, avatarSize);

                var state = g.Save();
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(avatarRect);
                    g.SetClip(path);
                    DrawItemImage(g, avatarRect, item.ImagePath);
                }
                g.Restore(state);
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for consistent background and border styling
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);

                // Add hover effect with subtle gradient
                if (isHovered && !isSelected)
                {
                    using (var hoverBrush = new LinearGradientBrush(itemRect, Color.FromArgb(30, Color.LightBlue), Color.Transparent, LinearGradientMode.Vertical))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }

                // Add shadow for selected items
                if (isSelected)
                {
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                    {
                        g.FillPath(shadowBrush, path);
                    }
                }
            }
        }

        public override int GetPreferredItemHeight()
        {
            // Slightly taller for avatar display
            int baseH = 48;
            if (_owner != null && _owner.ImageSize > 0)
            {
                baseH = Math.Max(baseH, _owner.ImageSize + 16);
            }
            return baseH;
        }
    }
}
