using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Filled combo box painter - Style #5
    /// Filled background with subtle shadow elevation (Material Design filled variant)
    /// </summary>
    internal class FilledComboBoxPainter : BaseComboBoxPainter
    {
        private const int BorderRadius = 4;
        
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            // Draw shadow first
            DrawShadow(g, rect);
            
            // Filled background - slightly darker than normal
            Color bgColor = _helper.GetBackgroundColor();
            Color filledBg = _owner.Focused
                ? Color.FromArgb(Math.Max(0, bgColor.R - 10), Math.Max(0, bgColor.G - 10), Math.Max(0, bgColor.B - 10))
                : Color.FromArgb(Math.Max(0, bgColor.R - 20), Math.Max(0, bgColor.G - 20), Math.Max(0, bgColor.B - 20));
            
            using (var brush = new SolidBrush(filledBg))
            using (var path = GetRoundedRectPath(rect, BorderRadius))
            {
                g.FillPath(brush, path);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            // Filled Style has bottom border only (Material Design pattern)
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Blue
                : (_owner.HasError ? Color.Red : (_theme?.BorderColor ?? Color.Gray));
            
            float borderWidth = _owner.Focused ? 2f : 1f;
            
            using (var pen = new Pen(borderColor, borderWidth))
            {
                // Draw bottom border only
                g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // No separator for filled Style
            Color arrowColor = _owner.Focused 
                ? (_theme?.PrimaryColor ?? Color.Blue)
                : (_theme?.SecondaryColor ?? Color.Gray);
            
            DrawDropdownArrow(g, buttonRect, arrowColor);
        }
        
        private void DrawShadow(Graphics g, Rectangle rect)
        {
            // Subtle drop shadow for elevation
            var shadowRect = rect;
            shadowRect.Offset(0, 2);
            
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            using (var path = GetRoundedRectPath(shadowRect, BorderRadius))
            {
                g.FillPath(shadowBrush, path);
            }
        }
        
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            // Top corners rounded, bottom corners sharp for filled Style
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            path.AddLine(rect.Right - 1, rect.Y + radius, rect.Right - 1, rect.Bottom - 1);
            path.AddLine(rect.Right - 1, rect.Bottom - 1, rect.X, rect.Bottom - 1);
            path.AddLine(rect.X, rect.Bottom - 1, rect.X, rect.Y + radius);
            path.CloseFigure();
            
            return path;
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(12, 8, 8, 8);
        }
    }
}
