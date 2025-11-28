using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Rounded combo box painter - Style #3
    /// Rounded corners with prominent border radius
    /// </summary>
    internal class RoundedComboBoxPainter : BaseComboBoxPainter
    {
        private const int BorderRadius = 12; // More prominent rounding
        
     
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Blue
                : (_owner.HasError ? Color.Red : (_theme?.BorderColor ?? Color.Gray));
            
            float borderWidth = 1.5f;
            var basePen = PaintersFactory.GetPen(borderColor, borderWidth);
            var pen = (System.Drawing.Pen)basePen.Clone();
            try
            {
                pen.Alignment = PenAlignment.Inset;
                using (var path = GetRoundedRectPath(rect, BorderRadius))
                {
                    g.DrawPath(pen, path);
                }
            }
            finally
            {
                pen.Dispose();
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // No separator for rounded Style - cleaner look
            
            // Draw arrow
            Color arrowColor = _theme?.SecondaryColor ?? Color.Gray;
            DrawDropdownArrow(g, buttonRect, arrowColor);
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
        
        public override Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(16, 8, 12, 8); // More padding for rounded Style
        }
    }
}
