using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

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
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

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
            int leftStart = !checkRect.IsEmpty ? (checkRect.Right + Scale(8)) : (itemRect.Left + Scale(12));

            // Avatar space on the right
            int baseAvatar = (_owner.ImageSize > 0 ? _owner.ImageSize : Scale(28));
            int avatarSize = Math.Max(Scale(20), Math.Min(Scale(40), baseAvatar));
            int avatarPadding = Scale(12);
            int avatarSpace = !string.IsNullOrEmpty(item.ImagePath) ? (avatarSize + avatarPadding) : 0;

            Rectangle textRect = new Rectangle(leftStart, itemRect.Y, Math.Max(0, itemRect.Right - leftStart - avatarSpace), itemRect.Height);
            Color textColor = isSelected ? Color.White : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);

            // Draw circular avatar on the right using StyledImagePainter
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle avatarRect = new Rectangle(itemRect.Right - avatarSize - avatarPadding,
                    itemRect.Y + (itemRect.Height - avatarSize) / 2, avatarSize, avatarSize);

                float cx = avatarRect.X + avatarRect.Width / 2f;
                float cy = avatarRect.Y + avatarRect.Height / 2f;
                float radius = avatarRect.Width / 2f;
                StyledImagePainter.PaintInCircle(g, cx, cy, radius, item.ImagePath);
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for consistent background and border styling
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);

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
            int baseH = Scale(48);
            if (_owner != null && _owner.ImageSize > 0)
            {
                baseH = Math.Max(baseH, _owner.ImageSize + Scale(16));
            }
            return baseH;
        }
    }
}
