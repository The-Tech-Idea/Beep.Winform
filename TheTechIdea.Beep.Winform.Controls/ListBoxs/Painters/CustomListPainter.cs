using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Custom list painter that allows developers to provide their own item rendering logic.
    /// Developers can set a custom drawing delegate via the CustomItemRenderer property.
    /// </summary>
    internal class CustomListPainter : BaseListBoxPainter
    {
        /// <summary>
        /// Custom item rendering delegate that developers can provide
        /// </summary>
        public Action<Graphics, Rectangle, SimpleItem, bool, bool> CustomItemRenderer { get; set; }
        
        /// <summary>
        /// Custom item height calculator (optional)
        /// </summary>
        public Func<int> CustomItemHeightProvider { get; set; }
        
        /// <summary>
        /// Custom padding provider (optional)
        /// </summary>
        public Func<Padding> CustomPaddingProvider { get; set; }
        
        public override bool SupportsCheckboxes() => true;
        public override bool SupportsSearch() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (CustomItemRenderer != null)
            {
                // Use custom renderer provided by developer
                CustomItemRenderer(g, itemRect, item, isHovered, isSelected);
            }
            else
            {
                // Fall back to default rendering
                DrawDefaultItem(g, itemRect, item, isHovered, isSelected);
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for CustomList background, border, and shadow
          
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);
            }
        }
        
        private void DrawDefaultItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            var rect = itemRect;
            rect.Inflate(-4, -2);
            
            DrawItemBackground(g, rect, isHovered, isSelected);
            
            int currentX = rect.Left + 8;
            
            // Draw checkbox if enabled
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, rect.Y + (rect.Height - 16) / 2, 16, 16);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawCheckbox(g, checkRect, isChecked, isHovered);
                currentX += 24;
            }
            
            // Draw image if available
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle imgRect = new Rectangle(currentX, rect.Y + (rect.Height - 24) / 2, 24, 24);
                // ImagePath is used by ImagePainter and other framework tools
                // DrawItemImage in base class handles the ImagePath loading
                DrawItemImage(g, imgRect, item.ImagePath);
                currentX += 32;
            }
            
            // Draw text
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Width - currentX + rect.Left, rect.Height);
            Color textColor = isSelected ? Color.White : Color.FromArgb(60, 60, 60);
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        public override int GetPreferredItemHeight()
        {
            if (CustomItemHeightProvider != null)
            {
                return CustomItemHeightProvider();
            }
            return 32; // Default height
        }
        
        public override Padding GetPreferredPadding()
        {
            if (CustomPaddingProvider != null)
            {
                return CustomPaddingProvider();
            }
            return new Padding(4);
        }
    }
}
