using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    internal sealed class BeepTabWorkspaceMruTracker
    {
        private readonly LinkedList<BeepTabPage> _recentPages = new();
        private List<BeepTabPage>? _activeCycle;
        private int _activeCycleIndex = -1;
        private bool _activeCycleForward = true;
        private DateTime _lastCycleUtc = DateTime.MinValue;

        public int MaxRecentHistory { get; set; } = 20;

        public void RecordSelection(BeepTabPage? page, bool preserveActiveCycle = false)
        {
            if (page == null)
            {
                if (!preserveActiveCycle)
                {
                    ResetCycle();
                }

                return;
            }

            _recentPages.Remove(page);
            _recentPages.AddFirst(page);
            Trim();

            if (!preserveActiveCycle)
            {
                ResetCycle();
                return;
            }

            if (_activeCycle == null || _activeCycle.Count == 0)
            {
                return;
            }

            int cycleIndex = _activeCycle.IndexOf(page);
            if (cycleIndex < 0)
            {
                ResetCycle();
                return;
            }

            _activeCycleIndex = cycleIndex;
            _lastCycleUtc = DateTime.UtcNow;
        }

        public void Remove(BeepTabPage? page)
        {
            if (page == null)
            {
                return;
            }

            _recentPages.Remove(page);

            if (_activeCycle == null)
            {
                return;
            }

            int cycleIndex = _activeCycle.IndexOf(page);
            if (cycleIndex < 0)
            {
                return;
            }

            _activeCycle.RemoveAt(cycleIndex);
            if (_activeCycle.Count == 0)
            {
                ResetCycle();
                return;
            }

            if (cycleIndex <= _activeCycleIndex)
            {
                _activeCycleIndex = Math.Max(0, _activeCycleIndex - 1);
            }

            if (_activeCycleIndex >= _activeCycle.Count)
            {
                _activeCycleIndex = _activeCycle.Count - 1;
            }
        }

        public void Clear()
        {
            _recentPages.Clear();
            ResetCycle();
        }

        public void ResetCycle()
        {
            _activeCycle = null;
            _activeCycleIndex = -1;
            _lastCycleUtc = DateTime.MinValue;
        }

        public BeepTabPage? GetMostRecentAvailablePage(IReadOnlyList<BeepTabPage> availablePages, BeepTabPage? excludedPage = null)
        {
            if (availablePages == null || availablePages.Count == 0)
            {
                return null;
            }

            HashSet<BeepTabPage> availableSet = new HashSet<BeepTabPage>(availablePages);
            foreach (BeepTabPage page in _recentPages)
            {
                if (ReferenceEquals(page, excludedPage) || !availableSet.Contains(page))
                {
                    continue;
                }

                return page;
            }

            foreach (BeepTabPage page in availablePages)
            {
                if (!ReferenceEquals(page, excludedPage))
                {
                    return page;
                }
            }

            return null;
        }

        public bool TryGetCycleTarget(IReadOnlyList<BeepTabPage> availablePages, BeepTabPage? currentPage, bool forward, out BeepTabPage? targetPage)
        {
            targetPage = null;

            List<BeepTabPage> cyclePages = BuildCyclePages(availablePages, currentPage);
            if (cyclePages.Count <= 1)
            {
                ResetCycle();
                return false;
            }

            if (!CanReuseCycle(currentPage, forward))
            {
                _activeCycle = cyclePages;
                _activeCycleForward = forward;
                _activeCycleIndex = 0;
            }

            if (_activeCycle == null || _activeCycle.Count <= 1)
            {
                return false;
            }

            int direction = forward ? 1 : -1;
            int nextIndex = (_activeCycleIndex + direction + _activeCycle.Count) % _activeCycle.Count;
            targetPage = _activeCycle[nextIndex];
            _activeCycleIndex = nextIndex;
            _lastCycleUtc = DateTime.UtcNow;
            return targetPage != null;
        }

        private bool CanReuseCycle(BeepTabPage? currentPage, bool forward)
        {
            if (_activeCycle == null || _activeCycle.Count == 0)
            {
                return false;
            }

            if (_activeCycleForward != forward)
            {
                return false;
            }

            if ((DateTime.UtcNow - _lastCycleUtc) > TimeSpan.FromSeconds(1.5))
            {
                return false;
            }

            if (_activeCycleIndex < 0 || _activeCycleIndex >= _activeCycle.Count)
            {
                return false;
            }

            return ReferenceEquals(_activeCycle[_activeCycleIndex], currentPage);
        }

        /// <summary>
        /// Returns <paramref name="availablePages"/> sorted in MRU order for display
        /// in the quick-switch popup. <paramref name="currentPage"/> is placed first
        /// (so the popup can pre-select index 1 as the "previous" tab). Pages that
        /// have never been recorded appear after the MRU run, in their original order.
        /// </summary>
        public IReadOnlyList<BeepTabPage> GetMruOrderedPages(
            IReadOnlyList<BeepTabPage> availablePages,
            BeepTabPage? currentPage = null)
        {
            return BuildCyclePages(availablePages, currentPage);
        }

        private List<BeepTabPage> BuildCyclePages(IReadOnlyList<BeepTabPage> availablePages, BeepTabPage? currentPage)
        {
            List<BeepTabPage> cyclePages = new List<BeepTabPage>();
            if (availablePages == null || availablePages.Count == 0)
            {
                return cyclePages;
            }

            HashSet<BeepTabPage> availableSet = new HashSet<BeepTabPage>(availablePages);

            if (currentPage != null && availableSet.Contains(currentPage))
            {
                cyclePages.Add(currentPage);
            }

            foreach (BeepTabPage page in _recentPages)
            {
                if (!availableSet.Contains(page) || cyclePages.Contains(page))
                {
                    continue;
                }

                cyclePages.Add(page);
            }

            foreach (BeepTabPage page in availablePages)
            {
                if (!cyclePages.Contains(page))
                {
                    cyclePages.Add(page);
                }
            }

            return cyclePages;
        }

        private void Trim()
        {
            while (_recentPages.Count > Math.Max(1, MaxRecentHistory))
            {
                _recentPages.RemoveLast();
            }
        }
    }
}