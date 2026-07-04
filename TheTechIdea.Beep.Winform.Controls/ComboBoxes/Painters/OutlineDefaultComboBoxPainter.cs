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
            // Decorations removed: the previous 1px focus/hover border was
            // drawn on the control edge and produced an 'inside border' visual.
            // The textArea tint in DrawTextArea still provides the hover/focus
            // feedback without compressing the textarea.
        }
    }
}