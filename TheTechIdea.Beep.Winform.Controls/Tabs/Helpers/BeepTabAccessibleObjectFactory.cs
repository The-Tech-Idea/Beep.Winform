// BeepTabAccessibleObjectFactory.cs
// Shared accessible-object creation so painters and hosts do not build ad hoc
// accessibility trees.  All concrete accessible types in this file are internal
// — the factory is the only public surface.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Creates <see cref="AccessibleObject"/> instances for each logical element of a
    /// <see cref="TheTechIdea.Beep.Winform.Controls.Tabs.Hosts.BeepTabHeaderHost"/>.
    /// Call <see cref="CreateTabObject"/> once per rendered tab and cache the result
    /// alongside the <see cref="BeepTabItem"/> snapshot.
    /// </summary>
    public static class BeepTabAccessibleObjectFactory
    {
        /// <summary>
        /// Returns a live accessible object for a single tab body.
        /// </summary>
        /// <param name="owner">The header-host control that owns the tab.</param>
        /// <param name="item">Snapshot of the tab's model at paint time.</param>
        /// <param name="boundsProvider">
        /// Delegate called each time screen coordinates are required so the object
        /// always reflects the current layout (accounts for scroll / DPI changes).
        /// </param>
        public static AccessibleObject CreateTabObject(
            Control owner,
            BeepTabItem item,
            Func<BeepTabItem, Rectangle> boundsProvider,
            Action<int>? selectTab = null)
        {
            if (owner == null)        throw new ArgumentNullException(nameof(owner));
            if (item == null)         throw new ArgumentNullException(nameof(item));
            if (boundsProvider == null) throw new ArgumentNullException(nameof(boundsProvider));

            return new BeepTabBodyAccessibleObject(owner, item, boundsProvider, selectTab);
        }

        /// <summary>
        /// Returns an accessible object for the close button embedded in a tab.
        /// Returns <see langword="null"/> when the item's <see cref="BeepTabItem.CanClose"/>
        /// is <see langword="false"/> so callers do not expose a hidden close target.
        /// </summary>
        public static AccessibleObject? CreateCloseButtonObject(
            Control owner,
            BeepTabItem item,
            Func<BeepTabItem, Rectangle> closeBoundsProvider,
            Action<int>? closeTab = null)
        {
            if (owner == null)             throw new ArgumentNullException(nameof(owner));
            if (item == null)              throw new ArgumentNullException(nameof(item));
            if (!item.CanClose)            return null;
            if (closeBoundsProvider == null) throw new ArgumentNullException(nameof(closeBoundsProvider));

            return new BeepTabCloseButtonAccessibleObject(owner, item, closeBoundsProvider, closeTab);
        }

        // ── Internal accessible types ─────────────────────────────────────────

        /// <summary>Accessible object for a tab body.</summary>
        private sealed class BeepTabBodyAccessibleObject : AccessibleObject
        {
            private readonly Control _owner;
            private readonly BeepTabItem _item;
            private readonly Func<BeepTabItem, Rectangle> _boundsProvider;
            private readonly Action<int>? _selectTab;

            internal BeepTabBodyAccessibleObject(
                Control owner,
                BeepTabItem item,
                Func<BeepTabItem, Rectangle> boundsProvider,
                Action<int>? selectTab)
            {
                _owner         = owner;
                _item          = item;
                _boundsProvider = boundsProvider;
                _selectTab = selectTab;
            }

            public override string? Name        => _item.Title;
            public override string? Description => BuildDescription();
            public override AccessibleRole Role => AccessibleRole.PageTab;

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;

                    if (_item.IsSelected)       state |= AccessibleStates.Selected | AccessibleStates.Focused;
                    if (!_item.IsEnabled)       state |= AccessibleStates.Unavailable;
                    if (_item.IsDirty)          state |= AccessibleStates.Mixed;    // indicates unsaved content
                    if (_item.WorkspaceState.IsPinned)
                                                state |= AccessibleStates.Protected;
                    if (_item.WorkspaceState.IsPreview)
                                                state |= AccessibleStates.ReadOnly;

                    return state;
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    Rectangle clientRect = _boundsProvider(_item);
                    return _owner.RectangleToScreen(clientRect);
                }
            }

            public override AccessibleObject? Parent  => _owner.AccessibilityObject;
            public override string? KeyboardShortcut  => GetKeyboardShortcutText();

            public override void Select(AccessibleSelection flags)
            {
                if (flags.HasFlag(AccessibleSelection.TakeSelection)
                    || flags.HasFlag(AccessibleSelection.AddSelection))
                {
                    SelectTab();
                }
            }

            public override void DoDefaultAction()
            {
                SelectTab();
            }

            // ── child: close button ───────────────────────────────────────────
            public override int GetChildCount() => _item.CanClose ? 1 : 0;

            public override AccessibleObject? GetChild(int index) => null;

            // ── helpers ──────────────────────────────────────────────────────
            private void SelectTab()
            {
                _owner.Focus();
                _selectTab?.Invoke(_item.Index);
            }

            private string? GetKeyboardShortcutText()
            {
                if (_item.IsSelected || _item.Index < 0 || _item.Index > 8)
                {
                    return null;
                }

                return $"Ctrl+{_item.Index + 1}";
            }

            private string BuildDescription()
            {
                if (!_item.IsDirty
                    && !_item.WorkspaceState.IsPinned
                    && !_item.WorkspaceState.IsPreview
                    && string.IsNullOrEmpty(_item.BadgeText))
                {
                    return string.Empty;
                }

                var parts = new System.Text.StringBuilder();
                if (_item.IsDirty)                    parts.Append("Unsaved changes. ");
                if (_item.WorkspaceState.IsPinned)    parts.Append("Pinned. ");
                if (_item.WorkspaceState.IsPreview)   parts.Append("Preview. ");
                if (!string.IsNullOrEmpty(_item.BadgeText))
                    parts.Append($"Badge: {_item.BadgeText}. ");

                return parts.ToString().TrimEnd();
            }
        }

        /// <summary>Accessible object for the close (×) button inside a tab.</summary>
        private sealed class BeepTabCloseButtonAccessibleObject : AccessibleObject
        {
            private readonly Control _owner;
            private readonly BeepTabItem _item;
            private readonly Func<BeepTabItem, Rectangle> _boundsProvider;
            private readonly Action<int>? _closeTab;

            internal BeepTabCloseButtonAccessibleObject(
                Control owner,
                BeepTabItem item,
                Func<BeepTabItem, Rectangle> boundsProvider,
                Action<int>? closeTab)
            {
                _owner          = owner;
                _item           = item;
                _boundsProvider = boundsProvider;
                _closeTab = closeTab;
            }

            public override string? Name        => $"Close {_item.Title}";
            public override string? Description => "Closes the tab.";
            public override AccessibleRole Role => AccessibleRole.PushButton;

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;
                    if (_item.IsCloseButtonHovered) state |= AccessibleStates.HotTracked;
                    if (_item.IsCloseButtonPressed) state |= AccessibleStates.Pressed;
                    return state;
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    Rectangle clientRect = _boundsProvider(_item);
                    return _owner.RectangleToScreen(clientRect);
                }
            }

            public override AccessibleObject? Parent => _owner.AccessibilityObject;

            public override void DoDefaultAction()
            {
                _owner.Focus();
                _closeTab?.Invoke(_item.Index);
            }
        }
    }
}
