// Menus Phase 06 — BeepListBox substrate for BeepContextMenu item area.
//
// Implements the user directive: "we are using control and form style and BeepList
// for theming and styling." When UseHostedListSubstrate is enabled, the popup's
// item area is rendered by a docked BeepListBox child control, gaining (for free):
//   * BeepStyling / Background / Border / Shadow painter chrome on each item
//   * Hover and selection state machine
//   * Keyboard navigation (BeepListBox.Keyboard.cs)
//   * Accessibility (BeepListBox.Accessibility.cs)
//   * High-contrast colour resolution (BeepListBox.HighContrast.cs)
//
// The substrate is COMPOSITION (not inheritance). The BeepContextMenu form continues
// to own the popup chrome (shadow, border, optional search box, scrollbar) and
// lifecycle. Selection raised by the hosted list is forwarded to the existing
// OnItemClicked / OnSubmenuOpening pipeline so consumers see zero contract change.
//
// SCOPE NOTE (Phase 06): the flag is OPT-IN (default false). The hand-rolled item
// rendering in BeepContextMenu.Drawing.cs continues to ship as the default so
// existing pixel-perfect behaviour, the Phase 04B submenu triangle tracker, the
// shortcut column, subtext, separator, and submenu-arrow features remain stable.
// Flipping the default to ON is tracked as a future micro-phase (06.1).

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    public partial class BeepContextMenu
    {
        #region Substrate Fields

        private BeepListBox _hostedList;
        private bool _useHostedListSubstrate = true; // Phase 06.1: default ON
        private bool _hostedListWiring;       // guard against re-entrant sync
        private bool _hostedListEventsBound;

        #endregion

        #region Public Surface

        /// <summary>
        /// When <c>true</c>, the popup's item area is rendered by a hosted
        /// <see cref="BeepListBox"/> instead of the hand-rolled drawing path.
        /// Default <c>false</c> for backwards compatibility. See Phase 06 plan.
        /// </summary>
        [Category("Beep")]
        // Phase 06.1: Default ON — BeepListBox substrate is the primary item rendering path
        [Description("Use a hosted BeepListBox as the popup item substrate. Default ON per Phase 06.1.")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool UseHostedListSubstrate
        {
            get => _useHostedListSubstrate;
            set
            {
                if (_useHostedListSubstrate == value) return;
                _useHostedListSubstrate = value;
                if (_useHostedListSubstrate)
                {
                    EnsureHostedListCreated();
                    SyncHostedListFromMenuState();
                }
                else
                {
                    TeardownHostedList();
                }
                InvalidateLayoutCache();
                InvalidateSizeCache();
                RecalculateSize();
                Invalidate();
            }
        }

        /// <summary>
        /// True only when the substrate is enabled AND the hosted list is alive.
        /// Drawing / hit-test partials use this to skip the hand-rolled path.
        /// </summary>
        [Browsable(false)]
        public bool IsHostedListSubstrateActive
            => _useHostedListSubstrate && _hostedList != null && !_hostedList.IsDisposed;

        /// <summary>
        /// Exposes the hosted list for advanced consumers that need to tune it
        /// (e.g. <c>ListBoxType</c>, type-ahead). Returns <c>null</c> when the
        /// substrate is disabled.
        /// </summary>
        [Browsable(false)]
        public BeepListBox HostedList => IsHostedListSubstrateActive ? _hostedList : null;

        #endregion

        #region Lifecycle

        private void EnsureHostedListCreated()
        {
            if (_hostedList != null && !_hostedList.IsDisposed) return;

            _hostedList = new BeepListBox
            {
                Dock = DockStyle.Fill,
                ShowImage = _showImage,
                ShowCheckBox = _showCheckBox,
                MultiSelect = _multiSelect,
                EnableHoverAnimation = true,
                TabStop = false,
                Theme = _themeName,
            };

            // Pull current items into the hosted list.
            try
            {
                _hostedList.ListItems = _menuItems ?? new System.ComponentModel.BindingList<SimpleItem>();
            }
            catch
            {
                // best-effort — leave the list empty if assignment fails
            }

            BindHostedListEvents();

            // When substrate is active the BeepListBox owns its own scrolling, so the
            // menu form's hand-rolled scrollbar must stand down.
            try
            {
                if (_scrollBar != null) _scrollBar.Visible = false;
                _needsScrolling = false;
                _scrollOffset = 0;
            }
            catch { }

            // If the consumer asked for a search box, materialise it and dock it on top
            // so the hosted list (Dock = Fill) sits below it.
            try
            {
                if (_showSearchBox)
                {
                    EnsureSearchTextBox();
                    if (_searchTextBox != null)
                    {
                        _searchTextBox.Dock = DockStyle.Top;
                    }
                }
            }
            catch { }

            try
            {
                Controls.Add(_hostedList);
                _hostedList.SendToBack();

                if (_searchTextBox != null) _searchTextBox.BringToFront();
            }
            catch
            {
                // defensive — surface nothing to consumers if Controls collection rejects
            }

            try { _hostedList.ApplyTheme(); } catch { }
        }

        private void TeardownHostedList()
        {
            if (_hostedList == null) return;

            UnbindHostedListEvents();

            try { Controls.Remove(_hostedList); } catch { }
            try { _hostedList.Dispose(); } catch { }
            _hostedList = null;
        }

        private void BindHostedListEvents()
        {
            if (_hostedList == null || _hostedListEventsBound) return;
            _hostedList.SelectedItemChanged += HostedList_SelectedItemChanged;
            _hostedList.ItemClicked += HostedList_ItemClicked;
            _hostedListEventsBound = true;
        }

        private void UnbindHostedListEvents()
        {
            if (_hostedList == null || !_hostedListEventsBound) return;
            _hostedList.SelectedItemChanged -= HostedList_SelectedItemChanged;
            _hostedList.ItemClicked -= HostedList_ItemClicked;
            _hostedListEventsBound = false;
        }

        #endregion

        #region State Sync (menu -> list)

        /// <summary>
        /// Pushes the current BeepContextMenu visual flags into the hosted list.
        /// Called when the substrate is enabled and after property mutations.
        /// </summary>
        internal void SyncHostedListFromMenuState()
        {
            if (!IsHostedListSubstrateActive || _hostedListWiring) return;
            _hostedListWiring = true;
            try
            {
                _hostedList.ShowImage = _showImage;
                _hostedList.ShowCheckBox = _showCheckBox;
                _hostedList.MultiSelect = _multiSelect;

                // Theme sync (best-effort; ignore if Theme property is locked).
                try
                {
                    if (!string.Equals(_hostedList.Theme, _themeName, StringComparison.Ordinal))
                    {
                        _hostedList.Theme = _themeName;
                        _hostedList.ApplyTheme();
                    }
                }
                catch { }

                // Item list sync — only reassign if the BindingList reference changed.
                if (!ReferenceEquals(_hostedList.ListItems, _menuItems))
                {
                    try { _hostedList.ListItems = _menuItems; } catch { }
                }
            }
            finally
            {
                _hostedListWiring = false;
            }
        }

        #endregion

        #region Selection Forwarding (list -> menu)

        private void HostedList_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e?.SelectedItem;
            if (item == null) return;

            // Push selection state into the menu's existing fields so downstream
            // consumers / SubmenuTracking continue to observe the same data.
            try
            {
                _selectedItem = item;
                _selectedIndex = _menuItems?.IndexOf(item) ?? -1;
                OnSelectedItemChanged();
            }
            catch { }

            // If the item has children, treat the click as a submenu trigger.
            if (item.Children != null && item.Children.Count > 0)
            {
                OnSubmenuOpening(item);
                return;
            }

            // Otherwise fire the click pipeline — preserves existing event contract.
            if (_multiSelect && _showCheckBox)
            {
                // Multi-select: the BeepListBox itself toggles the check state.
                // Mirror the toggled set into _selectedItems for SelectedItems consumers.
                try
                {
                    _selectedItems.Clear();
                    if (_hostedList?.SelectedItems != null)
                    {
                        _selectedItems.AddRange(_hostedList.SelectedItems);
                    }
                }
                catch { }
            }

            OnItemClicked(item);

            // Honour the existing auto-close contract for single-select clicks.
            if (!_multiSelect && _closeOnItemClick)
            {
                try { Close(); } catch { }
            }
        }

        private void HostedList_ItemClicked(object sender, SimpleItem item)
        {
            // BeepListBox raises ItemClicked alongside SelectedItemChanged. We only
            // need one forwarder; SelectedItemChanged already covers the contract.
            // Keep this hook reserved for future per-click telemetry.
        }

        #endregion

        #region Layout Helper

        /// <summary>
        /// Returns the preferred height of the hosted list's item area, used by
        /// RecalculateSize when the substrate is active. Returns 0 when inactive.
        /// </summary>
        internal int GetHostedListPreferredHeight()
        {
            if (!IsHostedListSubstrateActive || _menuItems == null) return 0;
            try
            {
                int per = Math.Max(1, _hostedList.PreferredItemHeight);
                return per * _menuItems.Count;
            }
            catch
            {
                return 0;
            }
        }

        #endregion
    }
}
