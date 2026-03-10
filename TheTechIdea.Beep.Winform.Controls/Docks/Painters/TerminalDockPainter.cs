using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    public sealed class TerminalDockPainter : ClassicTaskbarDockPainter
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            config.BackgroundColor ??= Color.FromArgb(16, 22, 16);
            config.BorderColor ??= Color.FromArgb(80, 220, 160);
            config.BackgroundOpacity = 0.96f;
            base.PaintDockBackground(g, bounds, config, theme);
        }
    }
}
