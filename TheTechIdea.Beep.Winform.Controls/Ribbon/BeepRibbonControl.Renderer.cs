using TheTechIdea.Beep.Winform.Controls.Rendering;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        // RB-C05: Uses the new BeepRibbonPainter for a real ribbon appearance
        private sealed class BeepRibbonToolStripRenderer(BeepRibbonControl owner) : ToolStripProfessionalRenderer(new ProfessionalColorTable())
        {
            private BeepRibbonPainter? _painter;
            private BeepRibbonPainter Painter => _painter ??= new BeepRibbonPainter(owner._theme, owner);

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                var r = new Rectangle(Point.Empty, e.ToolStrip.Size);
                Painter.PaintQatBackground(e.Graphics, r);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle r = new Rectangle(Point.Empty, e.Item.Size);

                // Background only - no text. This passed btn.Text to PaintSmallButton, which draws the
                // label, and WinForms then called OnRenderItemText, which drew it a second time. Every
                // command in the ribbon rendered its caption twice: once where the painter put it and
                // once where the standard pipeline centres it, a few dozen pixels apart.
                //
                // OnRenderItemText owns the text. It already applies the theme colour, and going
                // through the standard pipeline is what gets alignment, TextImageRelation and
                // ellipsis right for both the large and small button layouts.
                if (e.Item is ToolStripButton btn && !string.IsNullOrEmpty(btn.Text))
                    Painter.PaintSmallButton(e.Graphics, r, null, null, e.Item.Selected, e.Item.Pressed, e.Item.Enabled);
                else
                    Painter.PaintQatButton(e.Graphics, r, null, e.Item.Selected, e.Item.Pressed);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                var r = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                Painter.PaintDropDownSeparator(e.Graphics, r);
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? owner._theme.Text : owner._theme.DisabledText;
                base.OnRenderItemText(e);
            }
        }
    }
}
