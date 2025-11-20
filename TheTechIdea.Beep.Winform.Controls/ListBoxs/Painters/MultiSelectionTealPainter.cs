using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Multiselection mode with light teal selected items (from images 1 & 3)
    /// Teal/cyan background for selected items with checkboxes
    /// </summary>
    internal class MultiSelectionTealPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            var rect = itemRect;
            rect.Inflate(-4, -2);
            
            DrawItemBackgroundEx(g, rect, item, isHovered, isSelected);
            
            int currentX = rect.Left + 12;
            
            // Draw teal-styled checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, rect.Y + (rect.Height - 18) / 2, 18, 18);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawTealCheckbox(g, checkRect, isChecked, isHovered);
                currentX += 26;
            }
            
            // Draw text
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Right - currentX - 12, rect.Height);
            Color textColor = isSelected ? Color.FromArgb(20, 80, 90) : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            
            // Draw description text if available
            if (!string.IsNullOrEmpty(item.Description))
            {
                Font smallFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1);
                Rectangle descRect = new Rectangle(currentX, rect.Y + rect.Height / 2 + 2, rect.Right - currentX - 12, rect.Height / 2 - 4);
                Color descColor = Color.FromArgb(100, 120, 130);
                System.Windows.Forms.TextRenderer.DrawText(g, item.Description, smallFont, descRect, descColor,
                    System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.Top);
            }
        }
        
        // Enhanced hover effects and selection indicators
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for MultiSelectionTeal background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);

                // Add hover effect with gradient
                if (isHovered && !isSelected)
                {
                    using (var hoverBrush = new LinearGradientBrush(itemRect, Color.FromArgb(30, Color.Teal), Color.Transparent, LinearGradientMode.Vertical))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }

                // Add subtle shadow for selected items
                if (isSelected)
                {
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                    {
                        g.FillPath(shadowBrush, path);
                    }
                }
            }
        }
        
        private void DrawTealCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered)
        {
            Color tealColor = Color.FromArgb(13, 148, 136); // Teal
            
            // Draw background
            Color bgColor = isChecked ? tealColor : Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw border
            using (var pen = new Pen(tealColor, 1.5f))
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
                        new Point(checkboxRect.Left + 3, checkboxRect.Top + checkboxRect.Height / 2),
                        new Point(checkboxRect.Left + checkboxRect.Width / 2 - 1, checkboxRect.Bottom - 4),
                        new Point(checkboxRect.Right - 3, checkboxRect.Top + 3)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
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
            return 56; // Taller for description text
        }
    }
}
