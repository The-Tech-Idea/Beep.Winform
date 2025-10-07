using System.Drawing;
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
            DrawItemBackground(g, itemRect, isHovered, isSelected);
            
            int currentX = itemRect.Left + 12;
            
            // Draw checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawCheckbox(g, checkRect, isChecked, isHovered);
                currentX += 24;
            }
            
            // Draw text
            int avatarSpace = 40; // Reserve space for avatar on right
            Rectangle textRect = new Rectangle(currentX, itemRect.Y, itemRect.Width - currentX - avatarSpace, itemRect.Height);
            Color textColor = _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            
            // Draw avatar circle on right
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int avatarSize = 28;
                Rectangle avatarRect = new Rectangle(itemRect.Right - avatarSize - 12, 
                    itemRect.Y + (itemRect.Height - avatarSize) / 2, avatarSize, avatarSize);
                
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(avatarRect);
                    g.SetClip(path);
                    DrawItemImage(g, avatarRect, item.ImagePath);
                    g.ResetClip();
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 48; // Taller for avatar display
        }
    }
}
