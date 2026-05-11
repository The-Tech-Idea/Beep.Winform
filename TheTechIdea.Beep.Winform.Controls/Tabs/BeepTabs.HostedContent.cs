using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Hosts;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private readonly List<BeepTabPage> _hostedPages = new();
        private BeepTabPage? _selectedHostedPage;
        private bool _suspendHostedSourceControlTreeSync;
        private bool _applyingHostedContentBounds;

        internal List<BeepTabItem> GetHostedSourceItemsSnapshot()
        {
            List<BeepTabItem> items = new List<BeepTabItem>(_hostedPages.Count);
            int selectedIndex = GetHostedSourceSelectedIndex();

            for (int index = 0; index < _hostedPages.Count; index++)
            {
                items.Add(CreateHostedTabItemSnapshot(_hostedPages[index], index, selectedIndex));
            }

            return items;
        }

        internal int GetHostedSourceItemCount()
        {
            return _hostedPages.Count;
        }

        internal int GetHostedSourceSelectedIndex()
        {
            return _selectedHostedPage == null ? -1 : _hostedPages.IndexOf(_selectedHostedPage);
        }

        internal string GetHostedSourceItemTitle(int index)
        {
            if (index < 0 || index >= _hostedPages.Count)
            {
                return string.Empty;
            }

            BeepTabPage page = _hostedPages[index];
            BeepTabItem metadata = GetOrCreateHostedTabMetadata(page);
            if (!string.IsNullOrWhiteSpace(metadata.Title))
            {
                return metadata.Title;
            }

            return page.Text ?? string.Empty;
        }

        internal IReadOnlyList<BeepTabPage> GetHostedSourcePagesSnapshot()
        {
            List<BeepTabPage> pages = new List<BeepTabPage>(_hostedPages.Count);
            foreach (BeepTabPage page in _hostedPages)
            {
                pages.Add(page);
            }

            return pages;
        }

        internal BeepTabPage? GetHostedSourcePageAt(int index)
        {
            if (index < 0 || index >= _hostedPages.Count)
            {
                return null;
            }

            return _hostedPages[index];
        }

        internal bool ContainsHostedSourcePage(BeepTabPage? page)
        {
            return page != null && _hostedPages.Contains(page);
        }

        // ── Public page-management API ────────────────────────────────────────

        /// <summary>Gets the currently selected <see cref="BeepTabPage"/>, or <see langword="null"/> if no tab is selected.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTabPage? SelectedPage => _selectedHostedPage;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTabPage? SelectedTab => SelectedPage;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<BeepTabPage> Pages => GetHostedSourcePagesSnapshot();

        public BeepTabPage CreatePage(string? text = null, bool select = false)
        {
            try
            {
                int pageNumber = _hostedPages.Count + 1;
                BeepTabPage page = new BeepTabPage
                {
                    Name = CreateHostedPageName(pageNumber),
                    Text = string.IsNullOrWhiteSpace(text) ? $"Tab {pageNumber}" : text
                };

                AddPage(page, select);
                ClearError();
                return page;
            }
            catch (Exception ex)
            {
                ReportError("CreatePage failed.", ex);
                throw;
            }
        }

        public void AddPage(BeepTabPage page, bool select = false)
        {
            try
            {
                AddHostedSourcePage(page, select);
                ClearError();
            }
            catch (Exception ex)
            {
                ReportError($"AddPage failed for page '{page?.Text}'.", ex);
            }
        }

        /// <summary>
        /// Adds a <see cref="BeepTabPage"/> to the end of the tab list.
        /// </summary>
        /// <param name="page">The page to add.</param>
        /// <param name="select">When <see langword="true"/>, the new page is selected immediately.</param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddTab(BeepTabPage page, bool select = false)
        {
            AddPage(page, select);
        }

        /// <summary>
        /// Removes a <see cref="BeepTabPage"/> by reference.
        /// </summary>
        /// <returns><see langword="true"/> if the page was found and removed.</returns>
        public bool RemovePage(BeepTabPage page)
        {
            try
            {
                bool result = RemoveHostedSourcePage(page);
                ClearError();
                return result;
            }
            catch (Exception ex)
            {
                ReportError($"RemovePage failed for page '{page?.Text}'.", ex);
                return false;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool RemoveTab(BeepTabPage? page) => RemoveHostedSourcePage(page);

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="page"/> is currently hosted by this control.
        /// </summary>
        public bool ContainsPage(BeepTabPage page) => ContainsHostedSourcePage(page);

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ContainsTab(BeepTabPage? page) => ContainsHostedSourcePage(page);

        /// <summary>
        /// Removes all hosted <see cref="BeepTabPage"/> instances.
        /// </summary>
        public void ClearPages()
        {
            try
            {
                ClearHostedSourcePages();
                ClearError();
            }
            catch (Exception ex)
            {
                ReportError("ClearPages failed.", ex);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearTabs()
        {
            ClearPages();
        }

        /// <summary>Gets the <see cref="BeepTabPage"/> at the given index, or <see langword="null"/> if out of range.</summary>
        public BeepTabPage? GetPageAt(int index) => GetHostedSourcePageAt(index);

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BeepTabPage? GetTabAt(int index) => GetPageAt(index);

        public bool MovePage(BeepTabPage page, int newIndex)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            try
            {
                int currentIndex = _hostedPages.IndexOf(page);
                if (currentIndex < 0)
                {
                    return false;
                }

                bool moved = TryMoveHostedSourceItem(currentIndex, newIndex);
                ClearError();
                return moved;
            }
            catch (Exception ex)
            {
                ReportError($"MovePage failed for page '{page.Text}'.", ex);
                return false;
            }
        }

        public void InsertPageAt(int index, BeepTabPage page, bool select = false)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            try
            {
                if (!_hostedPages.Contains(page))
                {
                    int clampedIndex = Math.Max(0, Math.Min(_hostedPages.Count, index));
                    _hostedPages.Insert(clampedIndex, page);
                    _cachedHeaderTabRects.Clear();
                    GetOrCreateHostedTabMetadata(page);
                    ApplyThemeToHostedPage(page, _currentTheme, Theme);
                    if (ShouldKeepHostedPagesOnOwnerControlTree())
                    {
                        EnsureHostedPageAttachedToOwner(page);
                    }
                    else
                    {
                        BeepTabContentHost contentHost = EnsureContentHost();
                        EnsureHostedPageAttachedToContentHost(contentHost, page);
                        SyncContentHostPageOrder(contentHost);
                    }
                }

                if (select || _selectedHostedPage == null)
                    SetSelectedHostedPage(page, raiseSelectionChanged: true);
                else
                    UpdateHostedContentBounds();

                ClearError();
            }
            catch (Exception ex)
            {
                ReportError($"InsertPageAt({index}) failed for page '{page.Text}'.", ex);
            }
        }

        /// <summary>
        /// Inserts a <see cref="BeepTabPage"/> at the specified index without applying preview-slot eviction.
        /// Intended for reorder operations such as designer move commands.
        /// </summary>
        /// <param name="index">Zero-based insert position; clamped to valid range.</param>
        /// <param name="page">The page to insert.</param>
        /// <param name="select">When <see langword="true"/>, the inserted page is selected immediately.</param>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void InsertTabAt(int index, BeepTabPage page, bool select = false)
        {
            InsertPageAt(index, page, select);
        }

        internal void AddHostedSourcePage(BeepTabPage page, bool selectPage = false)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (!_hostedPages.Contains(page))
            {
                // Preview-slot replacement: if the incoming page should reuse the
                // preview slot (ReusePreviewSlot is true by default), evict the
                // current preview tab first so there is at most one preview open.
                if (TabMode != BeepTabMode.Navigation)
                {
                    BeepTabPage? existingPreview = FindExistingPreviewSlotPage();
                    if (existingPreview != null)
                    {
                        RemoveHostedSourcePage(existingPreview);
                    }
                }

                // Register in the list BEFORE Controls.Add so that OnControlAdded
                // sees it already registered and does not double-add when design-time
                // ownership keeps the page on BeepTabs.Controls.
                _hostedPages.Add(page);
                _cachedHeaderTabRects.Clear();
                GetOrCreateHostedTabMetadata(page);
                ApplyThemeToHostedPage(page, _currentTheme, Theme);
                if (ShouldKeepHostedPagesOnOwnerControlTree())
                {
                    EnsureHostedPageAttachedToOwner(page);
                }
                else
                {
                    EnsureHostedPageAttachedToContentHost(EnsureContentHost(), page);
                }
            }

            if (selectPage || _selectedHostedPage == null)
            {
                SetSelectedHostedPage(page, raiseSelectionChanged: true);
            }
            else
            {
                UpdateHostedContentBounds();
            }
        }

        /// <summary>
        /// Returns the first open tab whose <see cref="BeepTabWorkspaceState.IsPreview"/>
        /// and <see cref="BeepTabWorkspaceState.ReusePreviewSlot"/> flags are both true,
        /// or <see langword="null"/> if none exists.
        /// </summary>
        private BeepTabPage? FindExistingPreviewSlotPage()
        {
            foreach (BeepTabPage page in _hostedPages)
            {
                BeepTabItem meta = GetOrCreateHostedTabMetadata(page);
                if (meta.WorkspaceState.IsPreview
                    && meta.WorkspaceState.ReusePreviewSlot)
                {
                    return page;
                }
            }

            return null;
        }

        internal bool RemoveHostedSourcePage(BeepTabPage? page)
        {
            if (page == null || !_hostedPages.Contains(page))
            {
                return false;
            }

            int removedIndex = _hostedPages.IndexOf(page);
            bool wasSelected = ReferenceEquals(_selectedHostedPage, page);

            // Capture history record before metadata is removed.
            PushClosedTabRecord(page, removedIndex);

            DetachHostedPageFromCurrentParent(page);

            _hostedPages.Remove(page);
            _cachedHeaderTabRects.Clear();
            RemoveHostedPageFromMru(page);
            page.Visible = false;

            if (wasSelected)
            {
                BeepTabPage? nextPage = ResolveHostedPageAfterClose(page, removedIndex);

                SetSelectedHostedPage(nextPage, raiseSelectionChanged: true);
            }
            else
            {
                UpdateHostedContentBounds();
                Invalidate();
            }

            return true;
        }

        internal void ClearHostedSourcePages()
        {
            // Null selection first so OnControlRemoved never fires SelectedIndexChanged mid-loop.
            _selectedHostedPage = null;
            if (_contentHost != null)
            {
                _contentHost.ClearPages();
                _contentHost.Visible = false;
            }

            _dontresize = true;
            try
            {
                // Remove all pages from Controls first
                foreach (BeepTabPage page in _hostedPages.ToArray())
                {
                    DetachHostedPageFromCurrentParent(page);
                    page.Visible = false;
                }
            }
            finally
            {
                _dontresize = false;
            }

            _hostedPages.Clear();
            _cachedHeaderTabRects.Clear();
            ClearHostedPageMru();
            ClearClosedTabHistory();
            UpdateLayout();
            UpdateItemSize();
            Invalidate();
        }

        internal bool TrySelectHostedSourceItem(int index)
        {
            if (index < 0 || index >= _hostedPages.Count)
            {
                return false;
            }

            SetSelectedHostedPage(_hostedPages[index], raiseSelectionChanged: true);
            return true;
        }

        internal bool TryMoveHostedSourceItem(int currentIndex, int newIndex)
        {
            if (currentIndex < 0 || currentIndex >= _hostedPages.Count)
            {
                return false;
            }

            int selectedIndexBeforeMove = GetHostedSourceSelectedIndex();
            BeepTabPage page = _hostedPages[currentIndex];
            _hostedPages.RemoveAt(currentIndex);
            int insertionIndex = Math.Max(0, Math.Min(newIndex, _hostedPages.Count));
            _hostedPages.Insert(insertionIndex, page);
            _cachedHeaderTabRects.Clear();
            SyncHostedPageControlTreeOrder(page, insertionIndex);
            SyncHostedPageOrderFromOwnerControlTree();
            SyncContentHostPageOrder();

            UpdateHostedContentBounds();
            UpdateItemSize();
            Invalidate();

            if (selectedIndexBeforeMove != GetHostedSourceSelectedIndex())
            {
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }

            return true;
        }

        private void SyncHostedPageControlTreeOrder(BeepTabPage page, int hostedIndex)
        {
            if (!ShouldKeepHostedPagesOnOwnerControlTree() || page.Parent != this)
            {
                return;
            }

            int targetControlIndex = GetControlTreeIndexForHostedPageIndex(hostedIndex);
            if (targetControlIndex < 0)
            {
                return;
            }

            int currentControlIndex = Controls.GetChildIndex(page, throwException: false);
            if (currentControlIndex == targetControlIndex)
            {
                return;
            }

            Controls.SetChildIndex(page, targetControlIndex);
        }

        private void SyncHostedPageOrderFromOwnerControlTree()
        {
            if (!ShouldKeepHostedPagesOnOwnerControlTree() || _hostedPages.Count <= 1)
            {
                return;
            }

            List<BeepTabPage> orderedPages = new List<BeepTabPage>(_hostedPages.Count);
            HashSet<BeepTabPage> orderedPageSet = new HashSet<BeepTabPage>();
            foreach (Control child in Controls)
            {
                if (child is BeepTabPage page && _hostedPages.Contains(page) && orderedPageSet.Add(page))
                {
                    orderedPages.Add(page);
                }
            }

            if (orderedPages.Count == 0)
            {
                return;
            }

            bool changed = orderedPages.Count != _hostedPages.Count;
            int compareCount = Math.Min(orderedPages.Count, _hostedPages.Count);
            for (int index = 0; !changed && index < compareCount; index++)
            {
                if (!ReferenceEquals(orderedPages[index], _hostedPages[index]))
                {
                    changed = true;
                }
            }

            if (!changed)
            {
                return;
            }

            foreach (BeepTabPage page in _hostedPages)
            {
                if (orderedPageSet.Add(page))
                {
                    orderedPages.Add(page);
                }
            }

            _hostedPages.Clear();
            _hostedPages.AddRange(orderedPages);
            _cachedHeaderTabRects.Clear();
        }

        private int GetControlTreeIndexForHostedPageIndex(int hostedIndex)
        {
            int pageOrdinal = 0;
            for (int controlIndex = 0; controlIndex < Controls.Count; controlIndex++)
            {
                if (Controls[controlIndex] is not BeepTabPage)
                {
                    continue;
                }

                if (pageOrdinal == hostedIndex)
                {
                    return controlIndex;
                }

                pageOrdinal++;
            }

            return Controls.Count;
        }

        internal bool TryRemoveHostedSourceItem(int index, out string? title)
        {
            title = null;
            if (index < 0 || index >= _hostedPages.Count)
            {
                return false;
            }

            BeepTabPage page = _hostedPages[index];
            title = page.Text;
            return RemoveHostedSourcePage(page);
        }

        internal void ApplyHostedSourceContentBounds(Rectangle bounds)
        {
            if (_applyingHostedContentBounds)
            {
                return;
            }

            if (_selectedHostedPage == null)
            {
                if (UseContentHostPresentation() && _contentHost != null)
                {
                    _contentHost.SetSelectedPage(null);
                    _contentHost.Visible = false;
                }

                return;
            }

            if (UseContentHostPresentation())
            {
                _applyingHostedContentBounds = true;
                try
                {
                    BeepTabContentHost contentHost = EnsureContentHost();
                    if (contentHost.Bounds != bounds)
                    {
                        contentHost.Bounds = bounds;
                    }

                    // Only ensure the selected page is attached, not all pages
                    EnsureHostedPageAttachedToContentHost(contentHost, _selectedHostedPage);
                    bool hasContentArea = bounds.Width > 0 && bounds.Height > 0;
                    if (contentHost.Visible != hasContentArea)
                    {
                        contentHost.Visible = hasContentArea;
                    }

                    if (hasContentArea && Controls.GetChildIndex(contentHost, throwException: false) != 0)
                    {
                        contentHost.BringToFront();
                    }

                    contentHost.SetSelectedPage(_selectedHostedPage, contentHost.ClientRectangle);
                }
                finally
                {
                    _applyingHostedContentBounds = false;
                }

                return;
            }

            EnsureHostedPageAttachedToOwner(_selectedHostedPage);

            _selectedHostedPage.Bounds = bounds;
            _selectedHostedPage.Visible = bounds.Width > 0 && bounds.Height > 0;
            if (_selectedHostedPage.Visible)
                _selectedHostedPage.BringToFront();
        }

        internal Control? GetHostedSourceSelectedContent()
        {
            if (UseContentHostPresentation())
            {
                return _contentHost?.HostedContent ?? _selectedHostedPage;
            }

            return _selectedHostedPage;
        }

        internal BeepTabPage? GetHostedSourceSelectedPage()
        {
            return _selectedHostedPage;
        }

        internal bool TrySelectHostedSourcePage(BeepTabPage? page)
        {
            if (page == null || !_hostedPages.Contains(page))
            {
                return false;
            }

            SetSelectedHostedPage(page, raiseSelectionChanged: true);
            return true;
        }

        internal bool TryGetHostedSourceHeaderBounds(BeepTabPage? page, Graphics graphics, out Rectangle bounds)
        {
            bounds = Rectangle.Empty;
            if (page == null)
            {
                return false;
            }

            List<RectangleF> headerRects = GetCurrentHeaderTabRects(graphics);
            foreach (BeepTabItem item in GetHostedSourceItemsSnapshot())
            {
                if (!ReferenceEquals(item.Content, page))
                {
                    continue;
                }

                if (item.Index < 0 || item.Index >= headerRects.Count)
                {
                    return false;
                }

                bounds = Rectangle.Ceiling(headerRects[item.Index]);
                return true;
            }

            return false;
        }

        internal IReadOnlyList<Control> GetHostedSourceSelectedContentChildren()
        {
            Control? selectedContent = GetHostedSourceSelectedContent();
            if (selectedContent == null)
            {
                return Array.Empty<Control>();
            }

            List<Control> children = new List<Control>(selectedContent.Controls.Count);
            foreach (Control child in selectedContent.Controls)
            {
                children.Add(child);
            }

            return children;
        }

        internal Size GetHostedSourceSelectedContentClientSize()
        {
            return GetHostedSourceSelectedContent()?.ClientSize ?? Size.Empty;
        }

        internal void InvalidateHostedSourceSelectedContent()
        {
            GetHostedSourceSelectedContent()?.Invalidate();
        }

        internal void ApplyThemeToHostedSource(IBeepTheme theme, string themeName)
        {
            ApplyThemeToContentHost(theme, themeName);

            foreach (BeepTabPage page in _hostedPages)
            {
                ApplyThemeToHostedPage(page, theme, themeName);
            }
        }

        private void SetSelectedHostedPage(BeepTabPage? page, bool raiseSelectionChanged)
        {
            if (page != null && !_hostedPages.Contains(page))
            {
                return;
            }

            if (ReferenceEquals(_selectedHostedPage, page))
            {
                UpdateHostedContentBounds();
                return;
            }

            int oldSelectedIndex = GetHostedSourceSelectedIndex();
            _selectedHostedPage = page;
            RecordHostedPageSelection(page);

            SuspendLayout();
            try
            {
                if (UseContentHostPresentation())
                {
                    ApplyHostedSourceContentBounds(DisplayRectangle);
                }
                else
                {
                    Rectangle contentRect = DisplayRectangle;
                    foreach (BeepTabPage p in _hostedPages)
                    {
                        if (ReferenceEquals(p, page))
                        {
                            p.Dock = DockStyle.None;
                            p.Bounds = contentRect;
                            p.Visible = true;
                            p.BringToFront();
                        }
                        else
                        {
                            p.Visible = false;
                        }
                    }
                }
            }
            finally
            {
                ResumeLayout(false);
            }

            if (raiseSelectionChanged)
            {
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }

            // Only invalidate the header region that changed (old + new selected tabs)
            // instead of repainting the entire control including content area
            InvalidateHeaderSelectionChange(oldSelectedIndex, GetHostedSourceSelectedIndex());
        }

        private void UpdateHostedContentBounds()
        {
            if (_selectedHostedPage == null)
            {
                if (UseContentHostPresentation())
                {
                    ApplyHostedSourceContentBounds(DisplayRectangle);
                }

                return;
            }

            if (UseContentHostPresentation())
            {
                ApplyHostedSourceContentBounds(DisplayRectangle);
                return;
            }

            EnsureHostedPageAttachedToOwner(_selectedHostedPage);
            _selectedHostedPage.Bounds = DisplayRectangle;
        }

        private bool UseContentHostPresentation()
        {
            return !IsInHostedContentDesignMode();
        }

        private bool IsInHostedContentDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || (Site?.DesignMode ?? false);
        }

        private bool ShouldKeepHostedPagesOnOwnerControlTree()
        {
            return !UseContentHostPresentation();
        }

        private string CreateHostedPageName(int suggestedIndex)
        {
            int index = Math.Max(1, suggestedIndex);
            while (true)
            {
                string candidate = $"beepTabPage{index}";
                bool exists = false;
                foreach (BeepTabPage hostedPage in _hostedPages)
                {
                    if (string.Equals(hostedPage.Name, candidate, StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    return candidate;
                }

                index++;
            }
        }

        private void PerformWithoutHostedSourceControlTreeSync(Action action)
        {
            if (action == null)
            {
                return;
            }

            bool previous = _suspendHostedSourceControlTreeSync;
            _suspendHostedSourceControlTreeSync = true;
            try
            {
                action();
            }
            finally
            {
                _suspendHostedSourceControlTreeSync = previous;
            }
        }

        private void EnsureHostedPageAttachedToOwner(BeepTabPage page)
        {
            if (page.Parent == this)
            {
                return;
            }

            DetachHostedPageFromCurrentParent(page);
            page.Dock = DockStyle.None;
            page.Visible = false;
            PerformWithoutHostedSourceControlTreeSync(() => Controls.Add(page));
        }

        private void DetachHostedPageFromCurrentParent(BeepTabPage page)
        {
            if (_contentHost != null && page.Parent == _contentHost)
            {
                _contentHost.RemovePage(page);
                return;
            }

            if (page.Parent == this)
            {
                PerformWithoutHostedSourceControlTreeSync(() => Controls.Remove(page));
                return;
            }

            page.Parent?.Controls.Remove(page);
        }

        private void EnsureHostedPagesAttachedToContentHost(BeepTabContentHost contentHost)
        {
            for (int index = contentHost.Controls.Count - 1; index >= 0; index--)
            {
                if (contentHost.Controls[index] is BeepTabPage page && !_hostedPages.Contains(page))
                {
                    contentHost.RemovePage(page);
                }
            }

            foreach (BeepTabPage page in _hostedPages)
            {
                EnsureHostedPageAttachedToContentHost(contentHost, page);
            }
        }

        private void SyncContentHostPageOrder(BeepTabContentHost? contentHost = null)
        {
            if (!UseContentHostPresentation())
            {
                return;
            }

            BeepTabContentHost? host = contentHost ?? _contentHost;
            if (host == null || host.IsDisposed)
            {
                return;
            }

            PerformWithoutHostedSourceControlTreeSync(() => host.SetPageOrder(_hostedPages));
        }

        private void EnsureHostedPageAttachedToContentHost(BeepTabContentHost contentHost, BeepTabPage page)
        {
            if (contentHost.ContainsPage(page))
            {
                return;
            }

            page.Dock = DockStyle.None;
            if (!ReferenceEquals(_selectedHostedPage, page))
            {
                page.Visible = false;
            }

            PerformWithoutHostedSourceControlTreeSync(() => contentHost.AddPage(page));
        }

        private BeepTabContentHost EnsureContentHost()
        {
            if (_contentHost != null && !_contentHost.IsDisposed)
            {
                if (!Controls.Contains(_contentHost))
                {
                    Controls.Add(_contentHost);
                }

                return _contentHost;
            }

            _contentHost = new BeepTabContentHost
            {
                Name = "beepTabContentHost",
                Visible = false,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            ApplyThemeToContentHost(_currentTheme, Theme, _contentHost);
            Controls.Add(_contentHost);
            _contentHost.SendToBack();
            return _contentHost;
        }

        private void ApplyThemeToContentHost(IBeepTheme? theme, string themeName, BeepTabContentHost? contentHost = null)
        {
            BeepTabContentHost? host = contentHost ?? _contentHost;
            if (host == null || host.IsDisposed || theme == null)
            {
                return;
            }

            host.Theme = themeName;
            host.BackColor = TabThemeHelpers.GetTabControlBackgroundColor(theme, true);
            host.ForeColor = TabThemeHelpers.GetTabTextColor(theme, true);
        }

        private static void ApplyThemeToHostedPage(BeepTabPage page, IBeepTheme? theme, string themeName)
        {
            if (page == null || theme == null)
            {
                return;
            }

            Color pageBackColor = TabThemeHelpers.GetTabControlBackgroundColor(theme, true);
            Color pageForeColor = TabThemeHelpers.GetTabTextColor(theme, true);
            page.BackColor = pageBackColor;
            page.ForeColor = pageForeColor;

            if (page.Controls.Count <= 0)
            {
                return;
            }

            foreach (Control control in page.Controls)
            {
                if (control is IBeepUIComponent component)
                {
                    component.Theme = themeName;
                }

                if (control is IDM_Addin)
                {
                    foreach (Control child in control.Controls)
                    {
                        if (child is IBeepUIComponent childComponent)
                        {
                            childComponent.Theme = themeName;
                        }
                    }
                }
            }
        }

    }
}