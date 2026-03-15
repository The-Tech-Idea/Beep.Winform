using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Command-menu style field. Keeps a neutral shell while allowing
    /// popup rows to carry command metadata (shortcut text).
    /// </summary>
    internal sealed class CommandMenuComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override int CornerRadius => 6;
        protected override bool ShowButtonSeparator => false;
        protected override bool IsSearchIconButton => true;

        public override int GetPreferredButtonWidth() => 32;
        public override Padding GetPreferredPadding() => new Padding(10, 6, 8, 6);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            var state = GetInteractionState();

            if (state == ComboBoxInteractionState.Hover)
            {
                using var hover = new SolidBrush(Color.FromArgb(10, _theme?.ForeColor ?? Color.Black));
                g.FillRectangle(hover, textAreaRect);
            }
            else if (state == ComboBoxInteractionState.Focused || state == ComboBoxInteractionState.Open)
            {
                using var focus = new SolidBrush(Color.FromArgb(14, _theme?.ComboBoxHoverBackColor ?? _theme?.PrimaryColor ?? Color.SteelBlue));
                g.FillRectangle(focus, textAreaRect);
            }
        }
    }
}
