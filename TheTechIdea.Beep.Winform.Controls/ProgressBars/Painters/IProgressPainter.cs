using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    public interface IProgressPainter
    {
        string Key { get; }
        void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> parameters);
        void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, System.Action<string, Rectangle> register);
    }

    /// <summary>
    /// Optional v2 painter contract. Existing painters can continue implementing IProgressPainter.
    /// </summary>
    public interface IProgressPainterV2 : IProgressPainter
    {
        void Paint(Graphics g, ProgressPainterContext context, BeepProgressBar owner);
        void UpdateHitAreas(ProgressPainterContext context, BeepProgressBar owner, System.Action<string, Rectangle> register);
    }
}
