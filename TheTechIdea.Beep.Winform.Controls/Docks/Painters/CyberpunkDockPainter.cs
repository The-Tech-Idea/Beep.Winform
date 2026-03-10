using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    public sealed class CyberpunkDockPainter : ClassicTaskbarDockPainter
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            var cyber = config;
            cyber.BackgroundColor ??= Color.FromArgb(28, 14, 45);
            cyber.BorderColor ??= Color.FromArgb(0, 255, 222);
            cyber.BackgroundOpacity = 0.92f;
            base.PaintDockBackground(g, bounds, cyber, theme);
        }
    }
}
