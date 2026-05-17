// BeepContextMenu.SubmenuTracking.cs
// Phase 04B — Commercial-Grade Menubar UX.
//
// Owns the "is the cursor heading toward the open submenu?" gate that
// stops a submenu from being dismissed when the user is diagonally
// drifting toward it. Three responsibilities:
//
//   1. Track the open child-submenu's screen-space bounds (set by
//      ContextMenuManager when it shows the child popup, cleared
//      when the child is closed).
//   2. Track the cursor's previous screen position so the triangle
//      tracker has the trajectory's tail end.
//   3. Expose ShouldDeferSubmenuDismissal — UpdateHoveredItem consults
//      this before stopping its dismissal timer when the hovered item
//      changes.
//
// Pure geometry lives in Menus/Helpers/SubmenuTriangleTracker.cs.
// This partial is the integration glue + state holder.
//
// See .plans/Menus-Phase-04-CommercialMenuBarUX.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    public partial class BeepContextMenu
    {
        // ─────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────

        private Rectangle _openSubmenuScreenBounds = Rectangle.Empty;
        private Point     _lastCursorScreen        = Point.Empty;

        /// <summary>
        /// Screen-space bounds of the child submenu currently open from
        /// this menu, or <see cref="Rectangle.Empty"/> when none. Updated
        /// by <see cref="ContextMenuManager"/> when it shows/closes a
        /// child popup.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Rectangle OpenSubmenuScreenBounds => _openSubmenuScreenBounds;

        // ─────────────────────────────────────────────────────────────────
        // Public-ish (internal) hooks called by ContextMenuManager
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called by <see cref="ContextMenuManager"/> immediately after
        /// it shows a child submenu from this menu. <paramref name="screenBounds"/>
        /// is the submenu's screen-space rectangle.
        /// </summary>
        internal void NoteSubmenuOpened(Rectangle screenBounds)
        {
            _openSubmenuScreenBounds = screenBounds;
        }

        /// <summary>
        /// Called by <see cref="ContextMenuManager"/> when the child
        /// submenu opened from this menu is closed. Clears the bounds
        /// so the triangle test stops firing.
        /// </summary>
        internal void NoteSubmenuClosed()
        {
            _openSubmenuScreenBounds = Rectangle.Empty;
        }

        // ─────────────────────────────────────────────────────────────────
        // Cursor-trajectory gate
        //
        // UpdateHoveredItem calls ShouldDeferSubmenuDismissal before it
        // tears down the dismissal timer for the previous hovered item.
        // When true, the timer stays running so the submenu stays
        // visible while the user finishes their diagonal drag.
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// True when the cursor's trajectory (last-recorded to current)
        /// is heading toward the currently open submenu and the
        /// pending-dismissal timer should NOT be reset yet. Updates the
        /// recorded last-cursor-screen as a side-effect so successive
        /// calls track the actual motion.
        /// </summary>
        /// <param name="currentClientPos">
        /// The cursor's current position in this menu's CLIENT coordinates
        /// (caller already has these from MouseMove).
        /// </param>
        internal bool ShouldDeferSubmenuDismissal(Point currentClientPos)
        {
            // No open submenu → nothing to defer.
            if (_openSubmenuScreenBounds.IsEmpty) return false;

            Point currentScreen;
            try { currentScreen = PointToScreen(currentClientPos); }
            catch { return false; }

            // First call this session — record and don't defer
            // (no trajectory yet).
            if (_lastCursorScreen == Point.Empty)
            {
                _lastCursorScreen = currentScreen;
                return false;
            }

            bool trackingToward = SubmenuTriangleTracker.IsCursorTrackingTowardSubmenu(
                _lastCursorScreen,
                currentScreen,
                _openSubmenuScreenBounds);

            _lastCursorScreen = currentScreen;
            return trackingToward;
        }

        /// <summary>
        /// Clears the recorded last-cursor-screen position. Called when
        /// the mouse leaves the menu so a re-entry starts a fresh
        /// trajectory.
        /// </summary>
        internal void ResetCursorTrajectory()
        {
            _lastCursorScreen = Point.Empty;
        }
    }
}
