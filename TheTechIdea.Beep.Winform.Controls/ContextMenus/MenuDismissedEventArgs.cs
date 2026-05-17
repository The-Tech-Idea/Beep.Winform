// MenuDismissedEventArgs.cs
// Phase 01 — Dismissal/Re-open Hot-Fix.
//
// Raised by ContextMenuManager when a context menu closes. Subscribers
// (notably BeepMenuBar) use this to suppress the immediate re-open echo
// caused when the same WM_LBUTTONDOWN that dismissed the popup is
// subsequently delivered to the owner control's OnMouseClick.
//
// See .plans/Menus-Phase-01-DismissalReopenHotFix.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Event data for <see cref="ContextMenuManager.MenuDismissed"/>.
    /// Reports the owner control that originally invoked the menu, the
    /// close reason, and the dismissing screen-coordinate point so the
    /// owner can correlate the dismissal with its own click pipeline.
    /// </summary>
    public sealed class MenuDismissedEventArgs : EventArgs
    {
        /// <summary>
        /// The control that owned the dismissed menu. May be <c>null</c>
        /// when the menu was launched without an explicit owner.
        /// </summary>
        public Control Owner { get; }

        /// <summary>Why the menu closed.</summary>
        public BeepContextMenuCloseReason Reason { get; }

        /// <summary>
        /// Screen-coordinate point at which the dismissal occurred — for
        /// click-driven dismissals this is the click location; for focus
        /// / programmatic dismissals it falls back to <see cref="Cursor.Position"/>.
        /// </summary>
        public Point ScreenPoint { get; }

        /// <summary>UTC timestamp captured the moment the dismissal fired.</summary>
        public DateTime UtcTimestamp { get; }

        public MenuDismissedEventArgs(
            Control owner,
            BeepContextMenuCloseReason reason,
            Point screenPoint)
        {
            Owner        = owner;
            Reason       = reason;
            ScreenPoint  = screenPoint;
            UtcTimestamp = DateTime.UtcNow;
        }
    }
}
