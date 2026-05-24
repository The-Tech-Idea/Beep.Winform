using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Stateless helper that maps a screen point to a dock target by testing it against
    /// the five guide diamond rectangles exposed by a <see cref="DockingGuideOverlay"/>.
    ///
    /// Usage in BeepDockingManager.FloatPanel():
    /// <code>
    ///   _guideOverlay.ShowOver(_hostForm);
    ///   var target = DockingDropTarget.HitTest(_guideOverlay, Control.MousePosition);
    ///   if (target.HasValue)
    ///       DockFloatingPanel(key, target.Value);
    /// </code>
    ///
    /// Reference:
    ///   DockPanelSuite DockPanel.DockDragHandler.DockIndicator.HitTest (concept)
    /// </summary>
    public static class DockingDropTarget
    {
        /// <summary>
        /// Returns the <see cref="DockPosition"/> diamond under the given screen point,
        /// or null if the point is not over any guide diamond.
        /// </summary>
        /// <param name="overlay">The visible guide overlay.</param>
        /// <param name="screenPt">Current cursor position in screen coordinates.</param>
        public static DockPosition? HitTest(DockingGuideOverlay overlay, Point screenPt)
        {
            if (overlay == null || !overlay.Visible)
                return null;
            return overlay.HitTest(screenPt);
        }
    }
}
