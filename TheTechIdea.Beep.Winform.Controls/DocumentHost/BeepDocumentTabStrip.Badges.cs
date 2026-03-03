// BeepDocumentTabStrip.Badges.cs
// Public API for setting notification badges on individual tabs (Feature 7.4).
// Delegates to the internal tab data model and starts the badge pulse animation.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // Public badge API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets the notification badge on the tab identified by <paramref name="documentId"/>.
        /// </summary>
        /// <param name="documentId">The document-id of the target tab.</param>
        /// <param name="badgeText">
        /// Short label shown on the badge (e.g. "3", "!", "●").
        /// Pass <c>null</c> or an empty string to clear the badge.
        /// </param>
        /// <param name="badgeColor">
        /// Optional badge background colour.  Pass <see cref="Color.Empty"/> (or omit) to
        /// use the theme's <c>ErrorColor</c>.
        /// </param>
        public void SetBadge(string documentId, string? badgeText,
                              Color badgeColor = default)
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                if (tab.Id != documentId) continue;

                bool changed = tab.BadgeText != badgeText;
                tab.BadgeText  = badgeText;
                tab.BadgeColor = badgeColor;

                // Pulse animation only fires when badge appears or changes value
                if (changed && !string.IsNullOrEmpty(badgeText))
                    StartBadgePulse(tab.Id);

                Invalidate();
                return;
            }
        }

        /// <summary>
        /// Clears the notification badge on the tab identified by <paramref name="documentId"/>.
        /// </summary>
        public void ClearBadge(string documentId)
            => SetBadge(documentId, null);
    }
}
