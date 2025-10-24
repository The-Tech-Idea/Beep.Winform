using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Outlined combo box painter - Style #2
    /// Outlined Style with clear border and rounded corners
    /// </summary>
    internal class OutlinedComboBoxPainter : BaseComboBoxPainter
    {
        private const int BorderRadius = 4;
        
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            Color bgColor = _helper.GetBackgroundColor();
            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(rect, BorderRadius))
            {
                g.FillPath(brush, path);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Blue
                : (_owner.HasError ? Color.Red : (_theme?.BorderColor ?? Color.Gray));
            
            float borderWidth = _owner.Focused ? 2f : 1f;
            
            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = GetRoundedRectPath(rect, BorderRadius))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // Draw subtle separator line (not full height for modern look)
            Color separatorColor = Color.FromArgb(230, _theme?.BorderColor ?? Color.Gray);
            int margin = 8;
            using (var pen = new Pen(separatorColor, 1f))
            {
                g.DrawLine(pen, 
                    buttonRect.Left, buttonRect.Top + margin, 
                    buttonRect.Left, buttonRect.Bottom - margin);
            }
            
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
            return new System.Windows.Forms.Padding(12, 6, 8, 6);
        }
    }
}
