using TheTechIdea.Beep.Winform.Controls.Rendering;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        private sealed class BeepRibbonToolStripRenderer(BeepRibbonControl owner) : ToolStripProfessionalRenderer(new ProfessionalColorTable())
        {
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                BeepRibbonRenderer.DrawToolStripSurface(
                    e.Graphics,
                    new Rectangle(Point.Empty, e.ToolStrip.Size),
                    owner._theme);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                var button = e.Item as ToolStripButton;
                if (button == null && e.Item is not ToolStripDropDownButton)
                {
                    base.OnRenderButtonBackground(e);
                    return;
                }

                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                BeepRibbonRenderer.DrawInteractiveItem(
                    e.Graphics,
                    bounds,
                    owner._theme,
                    hovered: e.Item.Selected,
                    pressed: e.Item.Pressed,
                    enabled: e.Item.Enabled,
                    selected: (e.Item as ToolStripButton)?.Checked == true);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using var pen = new Pen(owner._theme.Separator);
                if (e.Vertical)
                {
                    int x = e.Item.Width / 2;
                    e.Graphics.DrawLine(pen, x, 4, x, e.Item.Height - 4);
                }
                else
                {
                    int y = e.Item.Height / 2;
                    e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? owner._theme.Text : owner._theme.DisabledText;
                base.OnRenderItemText(e);
            }
        }
    }
}
