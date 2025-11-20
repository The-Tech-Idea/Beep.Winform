using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Outlined checkbox states (from image 2 - right side)
    /// Shows various checkbox states with red outlines
    /// </summary>
    internal class OutlinedCheckboxesPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Use layout-provided rectangles for consistent UX
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            var checkRect = info?.CheckRect ?? Rectangle.Empty;
            var textRect = info?.TextRect ?? new Rectangle(itemRect.Left + 12, itemRect.Y, itemRect.Width - 24, itemRect.Height);

            // Draw outlined checkbox inside provided rect if available
            if (!checkRect.IsEmpty)
            {
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawOutlinedCheckbox(g, checkRect, isChecked, isHovered, item);
            }

            // Draw text
            Color textColor = _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for OutlinedCheckboxes background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);
                if (isHovered)
                {
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(50, Color.Gray)))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }
        
        private void DrawOutlinedCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered, SimpleItem item)
        {
            // Determine state based on item text
            bool isDisabled = item.Text?.ToLower().Contains("disabled") == true;
            Color borderColor = isDisabled ? Color.FromArgb(200, 200, 200) : Color.FromArgb(220, 53, 69); // Red
            Color bgColor = isChecked ? Color.FromArgb(255, 240, 240) : Color.White;
            
            if (isDisabled)
            {
                bgColor = Color.FromArgb(245, 245, 245);
            }
            
            // Draw background
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw border (2px for outlined Style)
            using (var pen = new Pen(borderColor, 2f))
            {
                g.DrawRectangle(pen, checkboxRect.X + 1, checkboxRect.Y + 1, checkboxRect.Width - 3, checkboxRect.Height - 3);
            }
            
            // Draw checkmark if checked
            if (isChecked && !isDisabled)
            {
                using (var pen = new Pen(Color.FromArgb(220, 53, 69), 2.5f))
                {
                    Point[] checkPoints = new Point[]
                    {
                        new Point(checkboxRect.Left + 5, checkboxRect.Top + checkboxRect.Height / 2),
                        new Point(checkboxRect.Left + checkboxRect.Width / 2, checkboxRect.Bottom - 6),
                        new Point(checkboxRect.Right - 5, checkboxRect.Top + 5)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 50;
        }
    }
}
