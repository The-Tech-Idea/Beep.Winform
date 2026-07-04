using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Multi-select chip field — synced with BeepListBox (ListBoxType.ChipStyle).
    /// Popup shows: selected-chips area at top, dashed separator, checkbox list below.
    /// Field echoes: inline chips via ChipPainter, dashed separator line below chips,
    /// count badge matching popup chip accent, focus ring using FocusBorderColor.
    /// </summary>
    internal class MultiChipCompactComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override int CornerRadius => 8;
        protected override bool ShowButtonSeparator => false;

        public override int GetPreferredButtonWidth() => 36;
        public override Padding GetPreferredPadding() => new Padding(10, 6, 8, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Light tinted background for chip container area
            Color tint = Color.Empty;
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
                tint = Color.FromArgb(10, _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210));
            else if (istate == ComboBoxInteractionState.Hover)
                tint = Color.FromArgb(6, _theme?.ForeColor ?? Color.Black);

            if (tint != Color.Empty)
            {
                using var brush = new SolidBrush(tint);
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