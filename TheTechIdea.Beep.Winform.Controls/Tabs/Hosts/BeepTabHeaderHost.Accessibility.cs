// BeepTabHeaderHost.Accessibility.cs
// Per-tab AccessibleObject implementation for BeepTabHeaderHost.
// Exposes each rendered tab (and its close button) as navigable accessible children
// of the header host's own AccessibleObject.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        // ── Factory hook ──────────────────────────────────────────────────────

        /// <summary>
        /// Returns the custom accessible object for this header host.
        /// Called by Windows on WM_GETOBJECT; the result is passed to screen readers.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new BeepTabHeaderHostAccessibleObject(this);
        }

        // ── Private helpers used by accessible objects ────────────────────────

        /// <summary>Returns current layout items, or an empty list when the snapshot is unset.</summary>
        private IReadOnlyList<BeepTabHeaderItemLayout> GetAccessibleLayoutItems()
        {
            return LayoutSnapshot?.Items ?? (IReadOnlyList<BeepTabHeaderItemLayout>)Array.Empty<BeepTabHeaderItemLayout>();
        }

        // ── Accessible object for the header host as a whole ─────────────────

        /// <summary>
        /// The accessible root for the entire header strip.  Child objects correspond
        /// 1-to-1 with the rendered tabs in <see cref="BeepTabHeaderLayoutSnapshot.Items"/>.
        /// </summary>
        private sealed class BeepTabHeaderHostAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepTabHeaderHost _host;

            internal BeepTabHeaderHostAccessibleObject(BeepTabHeaderHost host)
                : base(host)
            {
                _host = host;
            }

            public override string? Name        => _host.AccessibleName ?? "Tab Header";
            public override AccessibleRole Role => AccessibleRole.PageTabList;

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;
                    if (_host.ContainsFocus) state |= AccessibleStates.Focused;
                    if (!_host.Enabled)     state |= AccessibleStates.Unavailable;
                    return state;
                }
            }

            // ── Children ─────────────────────────────────────────────────────

            public override int GetChildCount()
            {
                return _host.GetAccessibleLayoutItems().Count;
            }

            public override AccessibleObject? GetChild(int index)
            {
                IReadOnlyList<BeepTabHeaderItemLayout> items = _host.GetAccessibleLayoutItems();
                if (index < 0 || index >= items.Count)
                {
                    return null;
                }

                BeepTabHeaderItemLayout layout = items[index];
                return CreateAccessibleTabObject(layout.Item, items);
            }

            public override AccessibleObject? GetFocused()
            {
                IReadOnlyList<BeepTabHeaderItemLayout> items = _host.GetAccessibleLayoutItems();
                foreach (BeepTabHeaderItemLayout layout in items)
                {
                    if (layout.Item.IsFocused)
                    {
                        return CreateAccessibleTabObject(layout.Item, items);
                    }
                }

                return null;
            }

            public override AccessibleObject? GetSelected()
            {
                IReadOnlyList<BeepTabHeaderItemLayout> items = _host.GetAccessibleLayoutItems();
                foreach (BeepTabHeaderItemLayout layout in items)
                {
                    if (layout.Item.IsSelected)
                    {
                        return CreateAccessibleTabObject(layout.Item, items);
                    }
                }

                return null;
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                Point clientPt = _host.PointToClient(new Point(x, y));
                IReadOnlyList<BeepTabHeaderItemLayout> items = _host.GetAccessibleLayoutItems();

                foreach (BeepTabHeaderItemLayout layout in items)
                {
                    // Check close button first (tighter target).
                    if (layout.HasCloseButton && layout.CloseButtonBounds.Contains(clientPt))
                    {
                        AccessibleObject? closeObj =
                            CreateAccessibleCloseButtonObject(layout.Item, items);

                        return closeObj ?? base.HitTest(x, y);
                    }

                    if (layout.Bounds.Contains(clientPt))
                    {
                        return CreateAccessibleTabObject(layout.Item, items);
                    }
                }

                return base.HitTest(x, y);
            }

            // ── Keyboard navigation helpers ───────────────────────────────────

            public override AccessibleObject? Navigate(AccessibleNavigation navdir)
            {
                IReadOnlyList<BeepTabHeaderItemLayout> items = _host.GetAccessibleLayoutItems();
                if (items.Count == 0)
                {
                    return base.Navigate(navdir);
                }

                switch (navdir)
                {
                    case AccessibleNavigation.FirstChild:
                        return GetChild(0);

                    case AccessibleNavigation.LastChild:
                        return GetChild(items.Count - 1);

                    default:
                        return base.Navigate(navdir);
                }
            }

            // ── Bound helpers (look up live layout rect by item identity) ──────

            private static Rectangle GetTabBoundsForItem(
                BeepTabItem target,
                IReadOnlyList<BeepTabHeaderItemLayout> items)
            {
                foreach (BeepTabHeaderItemLayout layout in items)
                {
                    if (layout.Item.Index == target.Index)
                    {
                        return layout.Bounds;
                    }
                }

                return Rectangle.Empty;
            }

            private static Rectangle GetCloseBoundsForItem(
                BeepTabItem target,
                IReadOnlyList<BeepTabHeaderItemLayout> items)
            {
                foreach (BeepTabHeaderItemLayout layout in items)
                {
                    if (layout.Item.Index == target.Index)
                    {
                        return layout.CloseButtonBounds;
                    }
                }

                return Rectangle.Empty;
            }

            private AccessibleObject CreateAccessibleTabObject(
                BeepTabItem item,
                IReadOnlyList<BeepTabHeaderItemLayout> items)
            {
                return BeepTabAccessibleObjectFactory.CreateTabObject(
                    _host,
                    item,
                    candidate => GetTabBoundsForItem(candidate, items),
                    index =>
                    {
                        _host.TabsOwner?.TrySelectHeaderTab(index);
                        _host.Invalidate();
                    });
            }

            private AccessibleObject? CreateAccessibleCloseButtonObject(
                BeepTabItem item,
                IReadOnlyList<BeepTabHeaderItemLayout> items)
            {
                return BeepTabAccessibleObjectFactory.CreateCloseButtonObject(
                    _host,
                    item,
                    candidate => GetCloseBoundsForItem(candidate, items),
                    index =>
                    {
                        _host.TabsOwner?.TryCloseHeaderTab(index);
                        _host.Invalidate();
                    });
            }
        }
    }
}
