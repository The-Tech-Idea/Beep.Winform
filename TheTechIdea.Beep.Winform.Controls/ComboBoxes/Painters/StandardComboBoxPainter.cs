using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Standard combo box painter - default Windows-like Style
    /// Clean rectangular design with subtle border
    /// </summary>
    internal class StandardComboBoxPainter : BaseComboBoxPainter
    {
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            Color bgColor = _helper.GetBackgroundColor();
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Blue
                : (_owner.HasError ? Color.Red : (_theme?.BorderColor ?? Color.Gray));
            
            using (var pen = new Pen(borderColor, 1f))
            {
                // Draw simple rectangular border
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // Draw button background
            Color buttonBg = _owner.IsButtonHovered 
                ? Color.FromArgb(230, 230, 230)
                : Color.FromArgb(245, 245, 245);
            
            using (var brush = new SolidBrush(buttonBg))
            {
                g.FillRectangle(brush, buttonRect);
            }
            
            // Draw separator line
            Color separatorColor = _theme?.BorderColor ?? Color.Gray;
            using (var pen = new Pen(separatorColor, 1f))
            {
                g.DrawLine(pen, buttonRect.Left, buttonRect.Top, buttonRect.Left, buttonRect.Bottom);
            }
            
            // Draw arrow
            Color arrowColor = _theme?.SecondaryColor ?? Color.Gray;
            DrawDropdownArrow(g, buttonRect, arrowColor);
        }
    }
}
