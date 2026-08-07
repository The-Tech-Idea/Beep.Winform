using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal interface IBottomBarPainter
    {
        string Name { get; }

        /// <summary>
        /// Pixels this style needs ABOVE the bar band, for a shape that protrudes past its top edge.
        /// </summary>
        /// <remarks>
        /// The floating-CTA family centres a circle on the bar's top edge, so roughly half of it falls
        /// outside the band. The control reserves this much headroom inside its own bounds and paints
        /// the band below it, which is how the reference designs are composed: the CTA overlaps the
        /// bar, but the whole thing is one box.
        ///
        /// Drawing outside the control instead was considered and rejected: BaseControl's external
        /// drawing only reaches the parent when that parent implements IExternalDrawingProvider, and
        /// only BaseControl and BeepiFormPro do. Hosted in a plain Panel or Form the CTA would not be
        /// clipped - it would vanish, which is worse.
        /// </remarks>
        int GetTopOverhang(int contentHeight);

        /// <summary>
        /// Whether this style has motion that must keep repainting even when nothing is happening.
        /// </summary>
        /// <remarks>
        /// Only the styles that read <c>AnimationPhase</c> - a continuous sine driven by the ticker -
        /// need it. The rest are static between interactions, and repainting them twenty times a
        /// second was pure cost.
        /// </remarks>
        bool WantsContinuousAnimation { get; }
        void Paint(BottomBarPainterContext context);
        void CalculateLayout(BottomBarPainterContext context);
        void RegisterHitAreas(BottomBarPainterContext context);
        void Dispose();
    }
}
