using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop
{
    /// <summary>
    /// Owns the lifetime of the <see cref="DockingGuideOverlay"/> during a drag: shows it centred
    /// over the dock-site, updates the hovered diamond as the cursor moves, and hides/disposes it
    /// when the drag ends. Keeps overlay plumbing out of the controller and the manager.
    /// </summary>
    internal sealed class DockGuideController : IDisposable
    {
        private DockingGuideOverlay _overlay;

        /// <summary>The guide position currently under the cursor, or null.</summary>
        public DockPosition? ActiveTarget => _overlay?.ActiveTarget;

        /// <summary>Shows the guide overlay centred over <paramref name="hostForm"/>.</summary>
        public void Show(Form hostForm)
        {
            if (hostForm == null)
                return;

            if (_overlay == null || _overlay.IsDisposed)
                _overlay = new DockingGuideOverlay();

            _overlay.ShowOver(hostForm);
        }

        /// <summary>
        /// Updates the hovered diamond for the given screen point and returns the hit position.
        /// </summary>
        public DockPosition? Update(Point screenPoint)
        {
            if (_overlay == null || _overlay.IsDisposed || !_overlay.Visible)
                return null;

            return _overlay.HitTest(screenPoint);
        }

        /// <summary>
        /// Shows a thin accent bar at the given screen-coord rect indicating where a tab-drag
        /// would insert or split a dockspace. Pass <see cref="Rectangle.Empty"/> to clear.
        /// </summary>
        public void ShowSnapGuide(Rectangle screenRect, DockPosition orientation)
        {
            if (_overlay == null || _overlay.IsDisposed)
                return;
            _overlay.ShowSnapGuide(screenRect, orientation);
        }

        /// <summary>Removes any active snap-line indicator on the overlay.</summary>
        public void ClearSnapGuide()
        {
            if (_overlay == null || _overlay.IsDisposed)
                return;
            _overlay.ClearSnapGuide();
        }

        /// <summary>Hides the overlay (kept alive for reuse).</summary>
        public void Hide()
        {
            if (_overlay != null && !_overlay.IsDisposed && _overlay.Visible)
                _overlay.Hide();
        }

        public void Dispose()
        {
            _overlay?.Dispose();
            _overlay = null;
        }
    }
}
