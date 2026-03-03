using System;
using System.Drawing;
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

        // ════════════════════════════════════════════════════════════════════════════
        //  Inner accessible-object classes
        // ════════════════════════════════════════════════════════════════════════════

        internal sealed class BeepListBoxAccessible : ControlAccessibleObject
        {
            private BeepListBox Owner => (BeepListBox)base.Owner;

            public BeepListBoxAccessible(BeepListBox owner) : base(owner) { }

            public override AccessibleRole Role => AccessibleRole.List;

            public override string Name => Owner.AccessibleName ?? "List Box";

            public override int GetChildCount()
                => Owner._listItems?.Count ?? 0;

            public override AccessibleObject? GetChild(int index)
            {
                if (Owner._listItems == null || index < 0 || index >= Owner._listItems.Count)
                    return null;
                return new BeepListItemAccessible(Owner, index);
            }

            public override AccessibleObject? GetFocused()
            {
                int fi = Owner._focusedIndex;
                if (fi < 0 || Owner._listItems == null || fi >= Owner._listItems.Count)
                    return null;
                return new BeepListItemAccessible(Owner, fi);
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
            private SimpleItem Item => _owner._listItems[_index];

            public BeepListItemAccessible(BeepListBox owner, int index)
            {
                _owner = owner;
                _index = index;
            }

            public override string Name
            {
                get
                {
                    string text = Item.Text ?? "";
                    int total = _owner._listItems.Count;
                    // Announce sub-text if item is BeepListItem
                    if (Item is ListBoxs.Models.BeepListItem rich && !string.IsNullOrEmpty(rich.SubText))
                        return $"{text}, {rich.SubText}, item {_index + 1} of {total}";
                    return $"{text}, item {_index + 1} of {total}";
                }
            }

            public override AccessibleRole Role => AccessibleRole.ListItem;

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
