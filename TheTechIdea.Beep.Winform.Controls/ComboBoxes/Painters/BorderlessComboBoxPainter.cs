using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Borderless combo box painter - Style #6
    /// Clean minimal borderless design
    /// </summary>
    internal class BorderlessComboBoxPainter : BaseComboBoxPainter
    {
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            Color bgColor = _helper.GetBackgroundColor();
            
            // Draw subtle background on hover or focus
            if (_owner.Focused || _owner.IsHovered)
            {
                Color hoverBg = Color.FromArgb(245, bgColor.R, bgColor.G, bgColor.B);
                using (var brush = new SolidBrush(hoverBg))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            // Only draw bottom border on focus
            if (_owner.Focused)
            {
                Color borderColor = _owner.HasError 
                    ? Color.Red 
                    : (_theme?.PrimaryColor ?? Color.Blue);
                
                using (var pen = new Pen(borderColor, 2f))
                {
                    g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                }
            }
            else if (_owner.HasError)
            {
                // Show error state even when not focused
                using (var pen = new Pen(Color.Red, 1f))
                {
                    g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                }
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // No separator - completely borderless
            Color arrowColor = _owner.Focused 
                ? (_theme?.PrimaryColor ?? Color.Blue)
                : Color.FromArgb(180, _theme?.SecondaryColor ?? Color.Gray);
            
            DrawDropdownArrow(g, buttonRect, arrowColor);
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(4, 6, 4, 6); // Minimal padding
        }
        
        public override int GetPreferredButtonWidth()
        {
            return 20; // Smaller button for borderless style
        }
    }
}
