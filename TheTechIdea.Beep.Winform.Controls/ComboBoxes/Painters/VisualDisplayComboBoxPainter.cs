using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Visual-first variant: optimized for options with icons/swatches/images.
    /// Keeps regular text fallback while emphasizing the leading visual.
    /// </summary>
    internal sealed class VisualDisplayComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override int CornerRadius => 8;
        protected override bool ShowButtonSeparator => true;

        public override int GetPreferredButtonWidth() => 32;
        public override Padding GetPreferredPadding() => new Padding(10, 6, 8, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var state = GetInteractionState();
            if (state == ComboBoxInteractionState.Hover)
            {
                using var hover = new SolidBrush(Color.FromArgb(8, _theme?.ForeColor ?? Color.Black));
                g.FillRectangle(hover, textAreaRect);
            }
        }
    }
}
