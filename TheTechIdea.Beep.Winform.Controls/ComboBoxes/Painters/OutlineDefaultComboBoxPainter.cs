using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Primary enterprise outlined dropdown — synced with <see cref="Popup.ComboBoxPopupContent"/>.
    /// Popup uses PopupRowHoverColor+HoverBorderColor on hover, PopupRowSelectedColor+FocusBorderColor
    /// on selected. Field echoes: focus ring uses FocusBorderColor, hover adds subtle border accent,
    /// tinted bg on focus/hover.
    /// </summary>
    internal sealed class OutlineDefaultComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override int CornerRadius => 6;
        protected override bool ShowButtonSeparator => true;

        public override int GetPreferredButtonWidth() => 36;
        public override Padding GetPreferredPadding() => new Padding(12, 6, 8, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Focus: subtle primary-tinted background (echoes popup PopupRowFocusColor)
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                Color primary = _theme?.ButtonSelectedBorderColor ?? _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                using var brush = new SolidBrush(Color.FromArgb(8, primary));
                g.FillRectangle(brush, textAreaRect);
            }
            else if (istate == ComboBoxInteractionState.Hover)
            {
                // Matches popup PopupRowHoverColor — subtle tint
                Color hoverBg = _theme?.ComboBoxHoverBackColor ?? Color.Empty;
                Color fill = hoverBg != Color.Empty
                    ? Color.FromArgb(18, hoverBg)
                    : Color.FromArgb(8, _theme?.ForeColor ?? Color.Black);
                using var brush = new SolidBrush(fill);
                g.FillRectangle(brush, textAreaRect);
            }
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                // Focus ring using FocusBorderColor — matches popup selected row border
                Color focusColor = _theme?.ButtonHoverForeColor ?? _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                int radius = ScaleX(CornerRadius);
                using var path = GetRoundedRectPath(Rectangle.Inflate(drawingRect, -1, -1), radius);
                using var pen = new Pen(Color.FromArgb(90, focusColor), ScaleX(2));
                g.DrawPath(pen, path);
            }
            else if (istate == ComboBoxInteractionState.Hover)
            {
                // Hover: subtle border accent — matches popup HoverBorderColor on hover rows
                Color hoverBorder = _theme?.ButtonHoverBorderColor ?? Color.FromArgb(180, 180, 195);
                int radius = ScaleX(CornerRadius);
                using var path = GetRoundedRectPath(Rectangle.Inflate(drawingRect, -1, -1), radius);
                using var pen = new Pen(Color.FromArgb(50, hoverBorder), ScaleX(1));
                g.DrawPath(pen, path);
            }
        }
    }
}
