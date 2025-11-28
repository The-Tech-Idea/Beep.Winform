using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

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
                var brush = PaintersFactory.GetSolidBrush(hoverBg);
                g.FillRectangle(brush, rect);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            // Only draw bottom border on focus
            if (_owner.Focused)
            {
                Color borderColor = _owner.HasError
                    ? Color.Red
                    : (_theme?.PrimaryColor ?? Color.Empty);
                
                var pen = PaintersFactory.GetPen(borderColor,2f);
                g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            }
            else if (_owner.HasError)
            {
                var pen = PaintersFactory.GetPen(Color.Red,1f);
                g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // No separator - completely borderless
            Color arrowColor = _owner.Focused
                ? (_theme?.PrimaryColor ?? Color.Empty)
                : PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.SecondaryColor ?? Color.Empty, 180);
            
            DrawDropdownArrow(g, buttonRect, arrowColor);
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(4, 6, 4, 6); // Minimal padding
        }
        
        public override int GetPreferredButtonWidth()
        {
            return 20; // Smaller button for borderless Style
        }
    }
}
