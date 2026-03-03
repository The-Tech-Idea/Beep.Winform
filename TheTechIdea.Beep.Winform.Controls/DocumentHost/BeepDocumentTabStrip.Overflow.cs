// BeepDocumentTabStrip.Overflow.cs
// Tab overflow dropdown (▾ chevron) — lists all tabs when the strip is scrolling.
// Clicking an entry activates and scrolls that tab into view.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────────────
        // ShowOverflowMenu
        // Called when the user left-clicks the ▾ chevron overflow button.
        // ─────────────────────────────────────────────────────────────────────────────

        // Internal forwarder so BeepTabOverflowPopup (same assembly) can fire the TabSelected event
        internal void RaiseTabSelected(TabEventArgs args)
            => TabSelected?.Invoke(this, args);

        private void ShowOverflowMenu()
        {
            // Sprint 14: replaced ContextMenuStrip with searchable BeepTabOverflowPopup
            var popup    = new BeepTabOverflowPopup(this, _theme);
            var screenPt = PointToScreen(new Point(_overflowButtonRect.Left, Height));
            popup.ShowBelow(screenPt);
        }
    }
}
