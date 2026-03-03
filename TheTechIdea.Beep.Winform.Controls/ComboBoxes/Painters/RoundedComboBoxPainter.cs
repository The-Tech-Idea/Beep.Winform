using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Rounded combo box painter - Style #3
    /// Rounded corners with prominent border radius
    /// </summary>
    internal class RoundedComboBoxPainter : BaseComboBoxPainter
    {
        private const int BorderRadiusLogical = 12; // More prominent rounding
        
     
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused
                ? _theme?.PrimaryColor ?? Color.Blue
                : (_owner.HasError ? Color.Red : (_theme?.BorderColor ?? Color.Gray));

            float borderWidth = 1.5f;
            using (var path = GetRoundedRectPath(rect, _owner.ScaleLogicalX(BorderRadiusLogical)))
            {
                // Use Center alignment (PenAlignment.Inset is unreliable for GraphicsPath in GDI+)
                var pen = PaintersFactory.GetPen(borderColor, borderWidth);
                pen.Alignment = PenAlignment.Center;
                g.DrawPath(pen, path);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Subtle hover fill (no separator for rounded style - cleaner look)
            if (_owner.IsButtonHovered && _owner.Enabled)
            {
                Color hoverFill = PathPainterHelpers.WithAlphaIfNotEmpty(
                    _theme?.ComboBoxHoverBackColor != Color.Empty
                        ? _theme.ComboBoxHoverBackColor
                        : (_theme?.PrimaryColor ?? Color.Empty), 50);
                if (hoverFill != Color.Empty && hoverFill.A > 0)
                {
                    int r = Math.Max(ScaleX(4), Math.Min(buttonRect.Width, buttonRect.Height) / 3);
                    using (var path = GetRoundedRectPath(Rectangle.Inflate(buttonRect, -ScaleX(1), -ScaleY(1)), r))
                        g.FillPath(PaintersFactory.GetSolidBrush(hoverFill), path);
                }
            }

            DrawDropdownArrow(g, buttonRect, GetArrowColor(), _owner.IsDropdownOpen);
        }
        

        public override Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(16, 8, 12, 8); // More padding for rounded Style
        }
    }
}
