using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Tabs;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private readonly BeepTabWorkspaceMruTracker _workspaceMruTracker = new();
        private int _maxRecentHistory = 20;
        private bool _preserveWorkspaceMruCycle;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Maximum number of recently-used tabs tracked for document and workspace MRU navigation.")]
        [DefaultValue(20)]
        public int MaxRecentHistory
        {
            get => _maxRecentHistory;
            set
            {
                int normalizedValue = value < 1 ? 1 : value;
                if (_maxRecentHistory == normalizedValue)
                {
                    return;
                }

                _maxRecentHistory = normalizedValue;
                _workspaceMruTracker.MaxRecentHistory = normalizedValue;
            }
        }

        private void RecordHostedPageSelection(BeepTabPage? page)
        {
            if (page == null)
            {
                _workspaceMruTracker.ResetCycle();
                _preserveWorkspaceMruCycle = false;
                return;
            }

            if (TabMode == BeepTabMode.Navigation)
            {
                _workspaceMruTracker.ResetCycle();
                _preserveWorkspaceMruCycle = false;
                return;
            }

            _workspaceMruTracker.RecordSelection(page, preserveActiveCycle: _preserveWorkspaceMruCycle);
            _preserveWorkspaceMruCycle = false;
        }

        private void RemoveHostedPageFromMru(BeepTabPage? page)
        {
            _workspaceMruTracker.Remove(page);
        }

        private void ClearHostedPageMru()
        {
            _workspaceMruTracker.Clear();
            _preserveWorkspaceMruCycle = false;
        }

        private void ResetWorkspaceMruCycle()
        {
            _workspaceMruTracker.ResetCycle();
            _preserveWorkspaceMruCycle = false;
        }

        private BeepTabPage? ResolveHostedPageAfterClose(BeepTabPage removedPage, int removedIndex)
        {
            if (TabMode != BeepTabMode.Navigation)
            {
                BeepTabPage? mruPage = _workspaceMruTracker.GetMostRecentAvailablePage(GetHostedSourcePagesSnapshot(), removedPage);
                if (mruPage != null)
                {
                    return mruPage;
                }
            }

            if (_hostedPages.Count == 0)
            {
                return null;
            }

            int nextIndex = removedIndex - 1;
            if (nextIndex < 0)
            {
                nextIndex = 0;
            }

            if (nextIndex >= _hostedPages.Count)
            {
                nextIndex = _hostedPages.Count - 1;
            }

            return _hostedPages[nextIndex];
        }

        private bool TrySelectWorkspaceMruHeaderTab()
        {
            if (TabMode == BeepTabMode.Navigation)
            {
                return false;
            }

            BeepTabPage? currentPage = GetHostedSourceSelectedPage();
            if (!_workspaceMruTracker.TryGetCycleTarget(GetHostedSourcePagesSnapshot(), currentPage, forward: true, out BeepTabPage? targetPage) || targetPage == null)
            {
                return false;
            }

            _preserveWorkspaceMruCycle = true;
            if (TrySelectHostedSourcePage(targetPage))
            {
                return true;
            }

            _preserveWorkspaceMruCycle = false;
            return false;
        }

        /// <summary>
        /// Shows a floating type-to-filter quick-switch popup listing all open
        /// tabs in MRU order. Only active in Documents or Workspace mode.
        /// Returns <c>true</c> if a tab was selected and activated.
        /// </summary>
        internal bool TryShowWorkspaceQuickSwitch()
        {
            if (TabMode == BeepTabMode.Navigation)
                return false;

            int count = GetHostedSourceItemCount();
            if (count < 2)
                return false;

            // Build MRU-ordered page list (current page first, then MRU desc).
            IReadOnlyList<BeepTabPage> allPages = GetHostedSourcePagesSnapshot();
            BeepTabPage? current = GetHostedSourceSelectedPage();
            IReadOnlyList<BeepTabPage> mruOrdered = _workspaceMruTracker.GetMruOrderedPages(allPages, current);

            // Map each page to a quick-switch entry.
            var entries = new List<BeepTabQuickSwitchEntry>(mruOrdered.Count);
            foreach (BeepTabPage page in mruOrdered)
            {
                int tabIndex = _hostedPages.IndexOf(page);
                if (tabIndex < 0) continue;

                BeepTabItem meta  = GetOrCreateHostedTabMetadata(page);
                string      title = !string.IsNullOrWhiteSpace(meta.Title)
                                    ? meta.Title
                                    : (!string.IsNullOrWhiteSpace(page.Text) ? page.Text : $"Tab {tabIndex + 1}");

                entries.Add(new BeepTabQuickSwitchEntry
                {
                    TabIndex   = tabIndex,
                    Title      = title,
                    IsDirty    = meta.IsDirty,
                    IsPinned   = meta.IsPinned,
                    IsSelected = ReferenceEquals(page, current),
                });
            }

            if (entries.Count < 2)
                return false;

            // Position popup centred over the control.
            Point origin = PointToScreen(new Point(Math.Max(0, Width / 2 - 220), Math.Max(0, Height / 2 - 170)));

            using var popup = new BeepTabQuickSwitch(entries, _currentTheme, origin);
            popup.ShowDialog((IWin32Window?)ParentForm ?? this);

            int selectedTabIndex = popup.SelectedTabIndex;
            if (selectedTabIndex < 0)
                return false;

            // Reset the MRU cycle so the selection records cleanly.
            _workspaceMruTracker.ResetCycle();
            return TrySelectHeaderTab(selectedTabIndex);
        }
    }
}