using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    public interface IProjectCardPainter
    {
        string Key { get; }
        void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, IReadOnlyDictionary<string, object> parameters);
        void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, Action<string, Rectangle> register);
    }
}
