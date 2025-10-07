using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Material Design Outlined combo box painter - Style #4
    /// Material Design outlined variant with floating label support
    /// </summary>
    internal class MaterialOutlinedComboBoxPainter : BaseComboBoxPainter
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
                : (_owner.HasError ? Color.Red : Color.FromArgb(180, _theme?.BorderColor ?? Color.Gray));
            
            float borderWidth = _owner.Focused ? 2f : 1f;
            
            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = GetRoundedRectPath(rect, BorderRadius))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }
            
            // Draw floating label if present and focused or has value
            if (!string.IsNullOrEmpty(_owner.LabelText))
            {
                DrawFloatingLabel(g, rect, borderColor);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // Material design doesn't use separator - just the icon
            Color arrowColor = _owner.Focused 
                ? (_theme?.PrimaryColor ?? Color.Blue)
                : (_theme?.SecondaryColor ?? Color.Gray);
            
            DrawDropdownArrow(g, buttonRect, arrowColor);
        }
        
        private void DrawFloatingLabel(Graphics g, Rectangle rect, Color labelColor)
        {
            // Position label at top-left with gap in border
            var labelFont = new Font(_theme?.LabelFont?.FontFamily , 
                                     8f, FontStyle.Regular);
            
            string labelText = _owner.LabelText;
            var labelSize = TextRenderer.MeasureText(labelText, labelFont);
            
            // Position label
            int labelX = rect.X + 12;
            int labelY = rect.Y - (labelSize.Height / 2);
            
            // Clear background behind label
            var bgColor = _helper.GetBackgroundColor();
            using (var bgBrush = new SolidBrush(bgColor))
            {
                var clearRect = new Rectangle(labelX - 4, labelY, labelSize.Width + 8, labelSize.Height);
                g.FillRectangle(bgBrush, clearRect);
            }
            
            // Draw label text
            TextRenderer.DrawText(g, labelText, labelFont, 
                new Point(labelX, labelY), labelColor, bgColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
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
            return new Padding(12, 12, 8, 12); // More top padding for floating label
        }
    }
}
