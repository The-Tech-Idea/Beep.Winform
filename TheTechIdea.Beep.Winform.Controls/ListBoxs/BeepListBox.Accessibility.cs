using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// WCAG 2.2 Level AA accessibility support for BeepListBox.
    /// Provides per-item AccessibleObjects, live-region notifications,
    /// and high-contrast colour overrides.
    /// </summary>
    public partial class BeepListBox
    {
        // ── Control-level accessible object ──────────────────────────────────────

        protected override AccessibleObject CreateAccessibilityInstance()
            => new BeepListBoxAccessible(this);

        // ── Announce events to AT ─────────────────────────────────────────────────

        private void NotifyA11ySelectionChanged(int index)
        {
            try { AccessibilityNotifyClients(AccessibleEvents.Selection, index); } catch { }
        }

        private void NotifyA11yOrderChanged()
        {
            try { AccessibilityNotifyClients(AccessibleEvents.Reorder, -1); } catch { }
        }

        private void NotifyA11yFocused(int index)
        {
            try { AccessibilityNotifyClients(AccessibleEvents.Focus, index); } catch { }
        }

        // ── High-contrast overrides ───────────────────────────────────────────────

        /// <summary>
        /// Returns the effective hover fill colour, respecting High Contrast mode.
        /// Painters should call this instead of using a hard-coded colour.
        /// </summary>
        internal Color GetHoverFillColor()
        {
            if (SystemInformation.HighContrast)
                return SystemColors.Highlight;
            if (SelectionBackColor != Color.Empty)
                return Color.FromArgb(ListBoxs.Tokens.ListBoxTokens.HoverOverlayAlpha, SelectionBackColor);
            return Color.FromArgb(ListBoxs.Tokens.ListBoxTokens.HoverOverlayAlpha,
                _currentTheme?.PrimaryColor ?? SystemColors.Highlight);
        }

        /// <summary>
        /// Returns the effective selection fill colour, respecting High Contrast mode.
        /// </summary>
        internal Color GetSelectionFillColor()
        {
            if (SystemInformation.HighContrast)
                return SystemColors.Highlight;
            if (SelectionBackColor != Color.Empty)
                return Color.FromArgb(SelectionOverlayAlpha, SelectionBackColor);
            return Color.FromArgb(SelectionOverlayAlpha,
                _currentTheme?.PrimaryColor ?? SystemColors.Highlight);
        }

        /// <summary>
        /// Returns the effective text colour for a selected item, respecting High Contrast.
        /// </summary>
        internal Color GetSelectionTextColor()
        {
            if (SystemInformation.HighContrast) return SystemColors.HighlightText;
            return _currentTheme?.ForeColor ?? SystemColors.HighlightText;
        }

        /// <summary>
        /// Returns the effective focus ring colour, respecting High Contrast.
        /// </summary>
        internal Color GetFocusRingColor()
        {
            if (SystemInformation.HighContrast) return SystemColors.Highlight;
            return FocusOutlineColor != Color.Empty
                ? FocusOutlineColor
                : (_currentTheme?.PrimaryColor ?? SystemColors.Highlight);
        }

        internal void SyncAccessibilityRole()
        {
            AccessibleRole = ShowHierarchy ? AccessibleRole.Outline : AccessibleRole.List;
        }

        // ════════════════════════════════════════════════════════════════════════════
        //  Inner accessible-object classes
        // ════════════════════════════════════════════════════════════════════════════

        internal sealed class BeepListBoxAccessible : ControlAccessibleObject
        {
            private BeepListBox Owner => (BeepListBox)base.Owner;

            public BeepListBoxAccessible(BeepListBox owner) : base(owner) { }

            public override AccessibleRole Role => Owner.ShowHierarchy ? AccessibleRole.Outline : AccessibleRole.List;

            public override string Name => Owner.AccessibleName ?? "List Box";

            private System.Collections.Generic.List<SimpleItem> GetVisibleItemsForA11y()
                => Owner._helper?.GetVisibleItems()
                   ?? Owner._listItems?.ToList()
                   ?? new System.Collections.Generic.List<SimpleItem>();

            public override int GetChildCount()
                => GetVisibleItemsForA11y().Count;

            public override AccessibleObject? GetChild(int index)
            {
                var visible = GetVisibleItemsForA11y();
                if (index < 0 || index >= visible.Count)
                    return null;
                return new BeepListItemAccessible(Owner, visible[index], index, visible.Count);
            }

            public override AccessibleObject? GetFocused()
            {
                var visible = GetVisibleItemsForA11y();
                int fi = Owner._focusedIndex;
                if (fi < 0 || fi >= visible.Count)
                    return null;
                return new BeepListItemAccessible(Owner, visible[fi], fi, visible.Count);
            }

            public override AccessibleObject? GetSelected()
                => GetFocused();

            public override Rectangle Bounds
            {
                get
                {
                    var r = Owner.DrawingRect;
                    return Owner.RectangleToScreen(r);
                }
            }
        }

        internal sealed class BeepListItemAccessible : AccessibleObject
        {
            private readonly BeepListBox _owner;
            private readonly int _index;
            private readonly int _totalCount;
            private readonly SimpleItem _item;
            private SimpleItem Item => _item;

            public BeepListItemAccessible(BeepListBox owner, SimpleItem item, int index, int totalCount)
            {
                _owner = owner;
                _item = item;
                _index = index;
                _totalCount = totalCount;
            }

            public override string Name
            {
                get
                {
                    string text = Item.Text ?? "";
                    int total = Math.Max(0, _totalCount);
                    // Announce sub-text if item is BeepListItem
                    if (Item is ListBoxs.Models.BeepListItem rich && !string.IsNullOrEmpty(rich.SubText))
                        return $"{text}, {rich.SubText}, item {_index + 1} of {total}";
                    return $"{text}, item {_index + 1} of {total}";
                }
            }

            public override AccessibleRole Role => _owner.ShowHierarchy ? AccessibleRole.OutlineItem : AccessibleRole.ListItem;

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates s = AccessibleStates.Focusable | AccessibleStates.Selectable;

                    // Selected
                    if (_owner.IsItemSelected(Item))
                        s |= AccessibleStates.Selected;

                    // Focused
                    if (_owner._focusedIndex == _index)
                        s |= AccessibleStates.Focused;

                    // Checked
                    if (_owner.GetItemCheckbox(Item))
                        s |= AccessibleStates.Checked;

                    // Disabled
                    if (Item is ListBoxs.Models.BeepListItem rich && rich.IsDisabled)
                        s |= AccessibleStates.Unavailable;

                    if (_owner.ShowHierarchy && _owner._helper?.ItemHasChildren(Item) == true)
                    {
                        s |= Item.IsExpanded ? AccessibleStates.Expanded : AccessibleStates.Collapsed;
                    }

                    return s;
                }
            }

            public override string? Value
            {
                get
                {
                    if (Item is ListBoxs.Models.BeepListItem rich)
                        return rich.SubText;
                    return null;
                }
            }

            public override AccessibleObject? Parent
                => _owner.AccessibilityObject;

            public override Rectangle Bounds
            {
                get
                {
                    var cache = _owner._layoutHelper?.GetCachedLayout();
                    if (cache == null || _index >= cache.Count) return Rectangle.Empty;
                    var info = cache.FirstOrDefault(x => x.Item == Item);
                    if (info == null) return Rectangle.Empty;
                    return _owner.RectangleToScreen(info.RowRect);
                }
            }

            public override void DoDefaultAction()
            {
                _owner.SelectedItem = Item;
                _owner.OnItemActivated(_index, Item);
            }
        }
    }
}
