using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Standard combo box painter - default Windows-like Style
    /// Clean rectangular design with subtle border
    /// </summary>
    internal class StandardComboBoxPainter : BaseComboBoxPainter
    {
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Blue
                : (_owner.HasError ? Color.Red : (_theme?.BorderColor ?? Color.Gray));
            
            var pen = PaintersFactory.GetPen(borderColor,1f);
            // Draw simple rectangular border
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width -1, rect.Height -1);
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            Rectangle visualRect = Rectangle.Inflate(buttonRect, -ScaleX(1), -ScaleY(1));
            int radius = Math.Max(ScaleX(5), Math.Min(visualRect.Height / 2, ScaleX(9)));

            Color hoverTone = _theme?.ComboBoxHoverBackColor ?? Color.Empty;
            Color accent = _theme?.ComboBoxHoverBorderColor ?? _theme?.PrimaryColor ?? Color.Empty;
            Color buttonBg = Color.Transparent;

            if (!_owner.Enabled)
            {
                buttonBg = Color.FromArgb(32, _theme?.ComboBoxForeColor ?? _theme?.ForeColor ?? Color.Gray);
            }
            else if (_owner.IsButtonHovered)
            {
                buttonBg = hoverTone != Color.Empty
                    ? PathPainterHelpers.WithAlphaIfNotEmpty(hoverTone, 140)
                    : PathPainterHelpers.WithAlphaIfNotEmpty(accent, 52);
            }
            else if (_owner.Focused)
            {
                buttonBg = PathPainterHelpers.WithAlphaIfNotEmpty(accent, 38);
            }

            if (buttonBg != Color.Empty && buttonBg.A > 0)
            {
                var brush = PaintersFactory.GetSolidBrush(buttonBg);
                using (var path = GetRoundedRectPath(visualRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }

            int separatorTop = buttonRect.Top + ScaleY(6);
            int separatorBottom = buttonRect.Bottom - ScaleY(6);
            if (separatorBottom > separatorTop)
            {
                Color separatorColor = PathPainterHelpers.WithAlphaIfNotEmpty(
                    _theme?.ComboBoxBorderColor != Color.Empty ? _theme.ComboBoxBorderColor : (_theme?.BorderColor ?? Color.Gray),
                    160);
                var pen = PaintersFactory.GetPen(separatorColor, 1f);
                g.DrawLine(pen, buttonRect.Left, separatorTop, buttonRect.Left, separatorBottom);
            }

            Color arrowColor;
            if (!_owner.Enabled)
            {
                arrowColor = Color.FromArgb(120, _theme?.ForeColor ?? Color.Gray);
            }
            else if (_owner.Focused)
            {
                arrowColor = _theme?.ComboBoxHoverBorderColor != Color.Empty
                    ? _theme.ComboBoxHoverBorderColor
                    : (_theme?.PrimaryColor ?? Color.Black);
            }
            else if (_owner.IsButtonHovered)
            {
                arrowColor = _theme?.ComboBoxHoverForeColor != Color.Empty
                    ? _theme.ComboBoxHoverForeColor
                    : (_theme?.SecondaryColor ?? Color.Black);
            }
            else
            {
                arrowColor = _theme?.ComboBoxForeColor != Color.Empty
                    ? _theme.ComboBoxForeColor
                    : (_theme?.SecondaryColor ?? Color.Gray);
            }

            DrawDropdownArrow(g, buttonRect, arrowColor);
        }

        public override int GetPreferredButtonWidth()
        {
            return 34;
        }

        public override Padding GetPreferredPadding()
        {
            return new Padding(10, 6, 8, 6);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
