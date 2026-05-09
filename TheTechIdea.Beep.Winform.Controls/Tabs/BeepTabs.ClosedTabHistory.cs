// BeepTabs.ClosedTabHistory.cs
// Maintains a bounded stack of recently closed tabs (Documents / Workspace mode).
// Ctrl+Shift+T (or TryReopenLastClosedTab) pops the stack and re-inserts the tab.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        // ── Storage ───────────────────────────────────────────────────────────

        // List used as a bounded stack: items are appended at the end (top of stack)
        // and the oldest entry (index 0) is dropped when the cap is exceeded.
        private readonly List<BeepTabClosedRecord> _closedTabStack = new();

        private int _maxClosedHistory = 10;

        // ── Public property ───────────────────────────────────────────────────

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Maximum number of closed tabs remembered for Ctrl+Shift+T reopen. Applies only in Documents and Workspace mode.")]
        [DefaultValue(10)]
        public int MaxClosedHistory
        {
            get => _maxClosedHistory;
            set
            {
                int normalized = Math.Max(1, value);
                if (_maxClosedHistory == normalized) return;
                _maxClosedHistory = normalized;
                TrimClosedTabStack();
            }
        }

        // ── Push (called from RemoveHostedSourcePage) ─────────────────────────

        private void PushClosedTabRecord(BeepTabPage page, int removedIndex)
        {
            // Only track in document / workspace mode.
            if (TabMode == BeepTabMode.Navigation) return;

            BeepTabItem meta  = GetOrCreateHostedTabMetadata(page);
            string      title = !string.IsNullOrWhiteSpace(meta.Title)
                                ? meta.Title
                                : (!string.IsNullOrWhiteSpace(page.Text) ? page.Text : $"Tab {removedIndex + 1}");

            _closedTabStack.Add(new BeepTabClosedRecord(page, title, removedIndex, meta.IsPinned));
            TrimClosedTabStack();
        }

        private void TrimClosedTabStack()
        {
            while (_closedTabStack.Count > _maxClosedHistory)
                _closedTabStack.RemoveAt(0); // drop oldest
        }

        // ── Reopen (pop) ──────────────────────────────────────────────────────

        /// <summary>
        /// Reopens the most recently closed tab (equivalent to Ctrl+Shift+T in IDEs).
        /// Only operates in Documents or Workspace mode. Returns <c>false</c> if the
        /// history is empty or the mode is Navigation.
        /// </summary>
        internal bool TryReopenLastClosedTab()
        {
            if (TabMode == BeepTabMode.Navigation)       return false;
            if (_closedTabStack.Count == 0)              return false;

            BeepTabClosedRecord record = _closedTabStack[_closedTabStack.Count - 1];
            _closedTabStack.RemoveAt(_closedTabStack.Count - 1);

            // Guard: refuse to re-add if the page is already open.
            if (ContainsHostedSourcePage(record.Page)) return false;

            // Re-insert the tab, selecting it.
            AddHostedSourcePage(record.Page, selectPage: true);

            // Restore visual metadata (title + pin state).
            BeepTabItem restoredMeta = GetOrCreateHostedTabMetadata(record.Page);
            restoredMeta.Title     = record.Title;
            restoredMeta.IsPinned  = record.WasPinned;

            // Move back toward the saved position if possible.
            int insertedIndex  = GetHostedSourceItemCount() - 1;
            int targetIndex    = Math.Min(record.LastIndex, insertedIndex);
            if (targetIndex < insertedIndex)
                TryMoveHostedSourceItem(insertedIndex, targetIndex);

            // Notify observers so they can refresh content if needed.
            OnTabReopenRequested(new BeepTabReopenEventArgs(record));
            return true;
        }

        private void OnTabReopenRequested(BeepTabReopenEventArgs e)
        {
            TabReopenRequested?.Invoke(this, e);
        }

        // ── Clear on mode change ──────────────────────────────────────────────

        private void ClearClosedTabHistory()
        {
            _closedTabStack.Clear();
        }
    }
}
