using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    public interface IProgressPainter
    {
        string Key { get; }
        void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> parameters);
        void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, System.Action<string, Rectangle> register);
    }
}
