using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Raised/elevated checkbox list (from image 2 - bottom right "ELEVATED ERROR")
    /// Shows elevated error states with raised appearance
    /// </summary>
    internal class RaisedCheckboxesPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            var rect = itemRect;
            rect.Inflate(-Scale(6), -Scale(3));
            
            DrawItemBackgroundEx(g, rect, item, isHovered, isSelected);
            
            int currentX = rect.Left + Scale(12);
            
            // Draw raised checkbox
            int cbSize = Scale(22);
            Rectangle checkRect = new Rectangle(currentX, rect.Y + (rect.Height - cbSize) / 2, cbSize, cbSize);
            bool isChecked = _owner.SelectedItems?.Contains(item) == true;
            DrawRaisedCheckbox(g, checkRect, isChecked, item);
            currentX += Scale(30);
            
            // Draw text
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Right - currentX - Scale(12), rect.Height);
            Color textColor = GetItemTextColor(item);
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        // Enhanced hover effects and selection indicators
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for RaisedCheckboxes background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);

                // Add hover effect with subtle shadow
                if (isHovered && !isSelected)
                {
                    g.FillPath(GetBrush(Color.FromArgb(ListBoxTokens.HoverOverlayAlpha, _theme?.AccentColor ?? _theme?.ErrorColor ?? Color.Gray)), path);
                }
            }
        }
        
        private Color GetItemTextColor(SimpleItem item)
        {
            bool isDisabled = item.Text?.ToLower().Contains("disabled") == true;
            return isDisabled
                ? Color.FromArgb(ListBoxTokens.DisabledAlpha, _theme?.ListItemForeColor ?? Color.Gray)
                : (_theme?.ListItemForeColor ?? Color.FromArgb(60, 60, 60));
        }
        
        private void DrawRaisedCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, SimpleItem item)
        {
            bool isDisabled = item.Text?.ToLower().Contains("disabled") == true;
            Color accent = _theme?.ErrorColor ?? _theme?.AccentColor ?? Color.FromArgb(220, 53, 69);
            Color borderColor = isDisabled ? (_theme?.BorderColor ?? Color.FromArgb(200, 200, 200)) : accent;
            Color bgColor = isChecked ? accent : (_theme?.BackgroundColor ?? Color.White);
            
            if (isDisabled)
            {
                bgColor = _theme?.BackgroundColor ?? Color.FromArgb(240, 240, 240);
            }
            
            // Draw checkbox shadow for raised effect
            var shadowRect = checkboxRect;
            shadowRect.Offset(0, 2);
            g.FillRectangle(GetBrush(Color.FromArgb(40, 0, 0, 0)), shadowRect);

            // Draw background
            g.FillRectangle(GetBrush(bgColor), checkboxRect);

            // Draw border
            g.DrawRectangle(GetPen(borderColor, 2f), checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);

            // Draw checkmark if checked
            if (isChecked && !isDisabled)
            {
                Point[] checkPoints = new Point[]
                {
                    new Point(checkboxRect.Left + Scale(4), checkboxRect.Top + checkboxRect.Height / 2),
                    new Point(checkboxRect.Left + checkboxRect.Width / 2, checkboxRect.Bottom - Scale(5)),
                    new Point(checkboxRect.Right - Scale(4), checkboxRect.Top + Scale(4))
                };
                g.DrawLines(GetPen(Color.White, 2.5f), checkPoints);
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return Scale(52);
        }
    }
}
