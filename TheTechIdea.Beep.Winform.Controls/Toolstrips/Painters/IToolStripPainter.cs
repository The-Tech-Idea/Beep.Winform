using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips.Painters
{
    public interface IToolStripPainter
    {
        string Key { get; }
        void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepToolStrip owner, IReadOnlyDictionary<string, object> parameters);
        void UpdateHitAreas(BeepToolStrip owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, System.Action<string, Rectangle> register);
    }
}
