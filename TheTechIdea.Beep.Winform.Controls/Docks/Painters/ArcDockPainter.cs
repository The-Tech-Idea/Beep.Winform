using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    public sealed class ArcDockPainter : MinimalDockPainter
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            config.BackgroundColor ??= Color.FromArgb(244, 245, 247);
            config.BorderColor ??= Color.FromArgb(220, 225, 230);
            config.BackgroundOpacity = 0.95f;
            base.PaintDockBackground(g, bounds, config, theme);
        }
    }
}
