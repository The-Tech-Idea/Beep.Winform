using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Material Design Outlined combo box painter - Style #4
    /// Material Design outlined variant with floating label support
    /// </summary>
    internal class MaterialOutlinedComboBoxPainter : BaseComboBoxPainter
    {
        private const int BorderRadius = 4;
        
       
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Empty
                : (_owner.HasError ? Color.Red : PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.BorderColor ?? Color.Empty, 180));
            
            float borderWidth = _owner.Focused ? 2f : 1f;
            
            var pen = PaintersFactory.GetPen(borderColor, borderWidth);
            using (var path = GetRoundedRectPath(rect, BorderRadius))
            {
                // Pen alignment requires a clone if modification needed; here we don't modify
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
            // Use state-aware arrow coloring
            DrawDropdownArrow(g, buttonRect, GetArrowColor());
        }
        
        private void DrawFloatingLabel(Graphics g, Rectangle rect, Color labelColor)
        {
            // Position label at top-left with gap in border
            Font labelFont;
            // _theme.LabelFont is a TypographyStyle in this project
            if (_theme?.LabelFont != null)
            {
                var tf = _theme.LabelFont; // TypographyStyle
                var family = string.IsNullOrEmpty(tf.FontFamily) ? "Segoe UI" : tf.FontFamily;
                var size = tf.FontSize > 0 ? tf.FontSize : 8f;
                var style = tf.FontStyle;
                labelFont = PaintersFactory.GetFont(family, size, style);
            }
            else
            {
                labelFont = PaintersFactory.GetFont("Segoe UI", 8f, FontStyle.Regular);
            }
            
            string labelText = _owner.LabelText;
            SizeF labelSizeF = TextUtils.MeasureText(labelText, labelFont, int.MaxValue);
            var labelSize = new Size((int)labelSizeF.Width, (int)labelSizeF.Height);
            
            // Position label
            int labelX = rect.X + 12;
            int labelY = rect.Y - (labelSize.Height / 2);
            
            // Clear background behind label
            var bgColor = _helper.GetBackgroundColor();
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            var clearRect = new Rectangle(labelX - 4, labelY, labelSize.Width + 8, labelSize.Height);
            g.FillRectangle(bgBrush, clearRect);
            
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
