using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Job type list with error states (from image 2 - bottom left "Job Type")
    /// Shows items with error badges and prohibited states
    /// </summary>
    internal class ErrorStatesPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackground(g, itemRect, isHovered, isSelected);
            
            int currentX = itemRect.Left + 16;
            
            // Draw checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, itemRect.Y + (itemRect.Height - 20) / 2, 20, 20);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                bool hasError = HasError(item);
                DrawErrorCheckbox(g, checkRect, isChecked, hasError);
                currentX += 28;
            }
            
            // Draw main text
            Rectangle textRect = new Rectangle(currentX, itemRect.Y + 6, itemRect.Width - currentX - 100, itemRect.Height / 2);
            Color textColor = GetTextColor(item);
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            
            // Draw description/error text
            if (!string.IsNullOrEmpty(item.Description) || HasError(item))
            {
                string errorText = HasError(item) ? "Option now prohibited" : item.Description;
                Font smallFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1);
                Rectangle descRect = new Rectangle(currentX, itemRect.Y + itemRect.Height / 2, itemRect.Width - currentX - 100, itemRect.Height / 2 - 6);
                Color descColor = HasError(item) ? Color.FromArgb(180, 50, 50) : Color.FromArgb(120, 120, 120);
                System.Windows.Forms.TextRenderer.DrawText(g, errorText, smallFont, descRect, descColor,
                    System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.Top);
            }
            
            // Draw error badge on right
            if (HasError(item))
            {
                DrawErrorBadge(g, itemRect);
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for ErrorStates background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
                {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);
            }
        }
        
        private bool HasError(SimpleItem item)
        {
            return item.Text?.ToLower().Contains("part-time") == true ||
                   item.Description?.ToLower().Contains("prohibited") == true;
        }
        
        private Color GetTextColor(SimpleItem item)
        {
            return HasError(item) ? Color.FromArgb(180, 60, 60) : Color.FromArgb(40, 40, 40);
        }
        
        private void DrawErrorCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool hasError)
        {
            Color checkColor = hasError ? Color.FromArgb(220, 53, 69) : Color.FromArgb(108, 117, 125);
            
            // Draw background
            Color bgColor = isChecked ? checkColor : Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw border
            using (var pen = new Pen(checkColor, 1.5f))
            {
                g.DrawRectangle(pen, checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);
            }
            
            // Draw checkmark if checked
            if (isChecked)
            {
                using (var pen = new Pen(Color.White, 2f))
                {
                    Point[] checkPoints = new Point[]
                    {
                        new Point(checkboxRect.Left + 4, checkboxRect.Top + checkboxRect.Height / 2),
                        new Point(checkboxRect.Left + checkboxRect.Width / 2, checkboxRect.Bottom - 5),
                        new Point(checkboxRect.Right - 4, checkboxRect.Top + 4)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }
        
        private void DrawErrorBadge(Graphics g, Rectangle itemRect)
        {
            string badgeText = "Error state!";
            Font badgeFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1);
            var textSize = System.Windows.Forms.TextRenderer.MeasureText(badgeText, badgeFont);
            
            int badgeWidth = textSize.Width + 16;
            int badgeHeight = 22;
            
            Rectangle badgeRect = new Rectangle(
                itemRect.Right - badgeWidth - 16,
                itemRect.Y + (itemRect.Height - badgeHeight) / 2,
                badgeWidth,
                badgeHeight);
            
            // Draw badge background
            Color badgeColor = Color.FromArgb(255, 243, 205); // Amber/Yellow
            using (var brush = new SolidBrush(badgeColor))
            using (var path = GetRoundedRectPath(badgeRect, 11))
            {
                g.FillPath(brush, path);
            }
            
            // Draw badge border
            using (var pen = new Pen(Color.FromArgb(255, 193, 7), 1f))
            using (var path = GetRoundedRectPath(badgeRect, 11))
            {
                g.DrawPath(pen, path);
            }
            
            // Draw badge text
            Color badgeTextColor = Color.FromArgb(120, 80, 0);
            System.Windows.Forms.TextRenderer.DrawText(g, badgeText, badgeFont, badgeRect, badgeTextColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }
        
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
        
        public override int GetPreferredItemHeight()
        {
            return 64; // Taller for two-line display with badges
        }
    }
}
