using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docks;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Exposes the dock's launchers to assistive technology.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The items are painted, not child controls, so MSAA has nothing to enumerate on its own:
    /// without this a screen reader hears "Dock, toolbar, application dock with 8 items" and cannot
    /// name, reach or activate any of the eight. Keyboard navigation was already complete - arrows,
    /// Home/End, reorder, focus visuals - so a keyboard user could move through a dock while a
    /// screen-reader user was told nothing had changed. That combination is worse than neither.
    /// </para>
    /// <para>
    /// Overriding <see cref="AccessibleObject.GetChildCount"/> and
    /// <see cref="AccessibleObject.GetChild"/> is what actually publishes the items. The defaults
    /// return <c>-1</c> and MSAA falls back to walking the <i>window</i> hierarchy - which is why a
    /// traversal check against an un-overridden control measures nothing. The probe demonstrates
    /// this rather than asserting it: a stock <see cref="Panel"/> reports <c>-1</c> too.
    /// </para>
    /// <para>
    /// Bounds come from <c>_itemStates[i].Bounds</c>, the same rectangles the painter drew and
    /// hit-testing uses. Deriving separate geometry here would be a second implementation that could
    /// disagree with what is on screen - the shape stage 03 exists to remove.
    /// </para>
    /// <para>
    /// Modelled on <c>Docking/BeepDockspace.Accessibility.cs</c> and <c>Tabs/BeepTabs.Accessibility.cs</c>,
    /// which solve the same painted-not-child problem. None of that code carries over - the three
    /// share no painting or layout - but the shape does, and following it keeps them consistent.
    /// </para>
    /// </remarks>
    public partial class BeepDock
    {
        protected override AccessibleObject CreateAccessibilityInstance()
            => new DockAccessibleObject(this);

        /// <summary>Number of items actually on screen; items past the overflow cut are not.</summary>
        private int VisibleItemCount
            => _overflowStartIndex >= 0
                ? System.Math.Min(_overflowStartIndex, _itemStates.Count)
                : _itemStates.Count;

        private bool HasOverflowAffordance
            => _overflowStartIndex >= 0 && !_overflowBounds.IsEmpty;

        private sealed class DockAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly BeepDock _owner;

            public DockAccessibleObject(BeepDock owner) : base(owner) => _owner = owner;

            public override AccessibleRole Role => AccessibleRole.ToolBar;

            /// <summary>
            /// Visible items, plus the overflow button when there is one. Overflowed items are
            /// deliberately not published: a tree that advertises children a user cannot reach is
            /// worse than a short one.
            /// </summary>
            public override int GetChildCount()
                => _owner.VisibleItemCount + (_owner.HasOverflowAffordance ? 1 : 0);

            public override AccessibleObject GetChild(int index)
            {
                if (index < 0)
                    return null;

                int visible = _owner.VisibleItemCount;
                if (index < visible)
                    return new DockItemAccessibleObject(_owner, index);

                if (index == visible && _owner.HasOverflowAffordance)
                    return new DockOverflowAccessibleObject(_owner);

                return null;
            }
        }

        private sealed class DockItemAccessibleObject : AccessibleObject
        {
            private readonly BeepDock _owner;
            private readonly int _index;

            public DockItemAccessibleObject(BeepDock owner, int index)
            {
                _owner = owner;
                _index = index;
            }

            public override AccessibleRole Role => AccessibleRole.PushButton;

            public override AccessibleObject Parent => _owner.AccessibilityObject;

            public override string Name => State_()?.Item?.Text ?? string.Empty;

            public override string Description
            {
                get
                {
                    var state = State_();
                    if (state?.Item == null)
                        return string.Empty;

                    var description = state.Item.Description ?? string.Empty;
                    if (state.BadgeCount > 0)
                    {
                        description = string.IsNullOrEmpty(description)
                            ? $"{state.BadgeCount} notifications"
                            : $"{description}, {state.BadgeCount} notifications";
                    }
                    return description;
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    var state = State_();
                    return state == null || state.Bounds.IsEmpty
                        ? Rectangle.Empty
                        : _owner.RectangleToScreen(state.Bounds);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    var state = State_();
                    if (state == null)
                        return AccessibleStates.Invisible;

                    var flags = AccessibleStates.Focusable | AccessibleStates.Selectable;
                    if (state.IsDisabled) flags |= AccessibleStates.Unavailable;
                    if (state.IsSelected) flags |= AccessibleStates.Selected;
                    if (state.IsPressed) flags |= AccessibleStates.Pressed;
                    if (state.IsFocused && _owner.Focused) flags |= AccessibleStates.Focused;
                    return flags;
                }
            }

            public override string DefaultAction => "Activate";

            public override void DoDefaultAction()
            {
                var state = State_();
                if (state?.Item == null || state.IsDisabled)
                    return;

                _owner.SelectedItem = state.Item;
                _owner.RaiseItemClicked(state.Item, _index);
            }

            public override void Select(AccessibleSelection flags)
            {
                if ((flags & (AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus)) != 0)
                    DoDefaultAction();
            }

            private Docks.DockItemState State_()
                => _index >= 0 && _index < _owner._itemStates.Count ? _owner._itemStates[_index] : null;
        }

        private sealed class DockOverflowAccessibleObject : AccessibleObject
        {
            private readonly BeepDock _owner;

            public DockOverflowAccessibleObject(BeepDock owner) => _owner = owner;

            public override AccessibleRole Role => AccessibleRole.PushButton;

            public override AccessibleObject Parent => _owner.AccessibilityObject;

            public override string Name => "More items";

            public override string Description
            {
                get
                {
                    int hidden = _owner._itemStates.Count - _owner.VisibleItemCount;
                    return hidden > 0 ? $"{hidden} more items" : string.Empty;
                }
            }

            public override Rectangle Bounds
                => _owner._overflowBounds.IsEmpty
                    ? Rectangle.Empty
                    : _owner.RectangleToScreen(_owner._overflowBounds);

            public override AccessibleStates State => AccessibleStates.Focusable;

            public override string DefaultAction => "Open";
        }
    }
}
