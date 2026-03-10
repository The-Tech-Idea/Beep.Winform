using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    public sealed class DraculaDockPainter : ClassicTaskbarDockPainter
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            config.BackgroundColor ??= Color.FromArgb(40, 42, 54);
            config.BorderColor ??= Color.FromArgb(98, 114, 164);
            config.BackgroundOpacity = 0.94f;
            base.PaintDockBackground(g, bounds, config, theme);
        }
    }
}
