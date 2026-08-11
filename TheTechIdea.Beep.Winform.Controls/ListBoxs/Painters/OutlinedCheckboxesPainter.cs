using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
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

        /// <summary>Disabled comes from the item, never from what its text happens to say.</summary>
        private static bool IsDisabled(SimpleItem item)
            => item is BeepListItem rich ? rich.IsDisabled : item?.IsEnabled == false;

        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Use layout-provided rectangles for consistent UX
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            var checkRect = info?.CheckRect ?? Rectangle.Empty;
            var textRect = info?.TextRect ?? new Rectangle(itemRect.Left + Scale(12), itemRect.Y, itemRect.Width - Scale(24), itemRect.Height);

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
                    g.FillPath(GetBrush(Color.FromArgb(50, Theme.ShadowColor)), path);
                }
            }
        }
        
        private void DrawOutlinedCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered, SimpleItem item)
        {
            // Determine state based on item text
            bool isDisabled = IsDisabled(item);
            Color borderColor = isDisabled ? Theme.BorderColor : Theme.ErrorColor; // Red
            Color bgColor = isChecked ? Color.FromArgb(30, Theme.ErrorColor) : Theme.ListBackColor;
            
            if (isDisabled)
            {
                bgColor = Theme.PanelBackColor;
            }
            
            // Draw background
            g.FillRectangle(GetBrush(bgColor), checkboxRect);

            // Draw border (2px for outlined Style)
            g.DrawRectangle(GetPen(borderColor, 2f), checkboxRect.X + 1, checkboxRect.Y + 1, checkboxRect.Width - 3, checkboxRect.Height - 3);

            // Draw checkmark if checked
            if (isChecked && !isDisabled)
            {
                Point[] checkPoints = new Point[]
                {
                    new Point(checkboxRect.Left + Scale(5), checkboxRect.Top + checkboxRect.Height / 2),
                    new Point(checkboxRect.Left + checkboxRect.Width / 2, checkboxRect.Bottom - Scale(6)),
                    new Point(checkboxRect.Right - Scale(5), checkboxRect.Top + Scale(5))
                };
                g.DrawLines(GetPen(Theme.ErrorColor, 2.5f), checkPoints);
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return Scale(50);
        }
    }
}
