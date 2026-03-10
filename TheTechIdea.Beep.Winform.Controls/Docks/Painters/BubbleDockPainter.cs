using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    public sealed class BubbleDockPainter : FloatingDockPainter
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            config.BackgroundColor ??= Color.FromArgb(230, 246, 251, 255);
            config.BorderColor ??= Color.FromArgb(180, 206, 228, 244);
            config.BackgroundOpacity = 0.9f;
            base.PaintDockBackground(g, bounds, config, theme);
        }
    }
}
