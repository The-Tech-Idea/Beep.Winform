using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Exposes a dockspace's tab strip to assistive technology.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The strip is painted, not built from child controls, so MSAA has nothing to enumerate on its
    /// own: without this a screen reader sees one anonymous client area and none of the tabs. The
    /// same problem was solved for <c>BeepDisplayContainer2</c> and <c>BeepTabs</c>; none of that
    /// carries over, because the three share no painting or layout code.
    /// </para>
    /// <para>
    /// Overriding <see cref="AccessibleObject.GetChildCount"/> and
    /// <see cref="AccessibleObject.GetChild"/> is what actually publishes the tabs. The defaults
    /// return <c>-1</c> and MSAA falls back to walking the <i>window</i> hierarchy — which is why a
    /// traversal check against an un-overridden control measures nothing and proves nothing.
    /// </para>
    /// <para>
    /// Tab geometry comes from <c>_captionLayout.TabRects</c>, the same rectangles the painter drew
    /// and hit-testing uses. Deriving separate bounds here would be a second implementation that
    /// could disagree with what is on screen.
    /// </para>
    /// </remarks>
    public partial class BeepDockspace
    {
        protected override AccessibleObject CreateAccessibilityInstance()
            => new DockspaceAccessibleObject(this);

        private sealed class DockspaceAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly BeepDockspace _owner;

            public DockspaceAccessibleObject(BeepDockspace owner) : base(owner) => _owner = owner;

            /// <summary>
            /// A dockspace showing its header is a tab list; with
            /// <see cref="HeaderPosition.None"/> — which is also what zen mode sets — there is no
            /// strip to describe, so it reports as a plain region instead of an empty tab list.
            /// </summary>
            public override AccessibleRole Role
                => _owner.TabPosition == HeaderPosition.None
                   ? AccessibleRole.Pane
                   : AccessibleRole.PageTabList;

            public override int GetChildCount()
                => _owner.TabPosition == HeaderPosition.None
                   ? 0
                   : _owner._captionLayout.TabRects.Count;

            public override AccessibleObject GetChild(int index)
            {
                if (_owner.TabPosition == HeaderPosition.None)
                    return null;

                var rects = _owner._captionLayout.TabRects;
                if (index < 0 || index >= rects.Count)
                    return null;

                return new DockTabAccessibleObject(_owner, index);
            }
        }

        private sealed class DockTabAccessibleObject : AccessibleObject
        {
            private readonly BeepDockspace _owner;
            private readonly int _index;

            public DockTabAccessibleObject(BeepDockspace owner, int index)
            {
                _owner = owner;
                _index = index;
            }

            public override AccessibleRole Role => AccessibleRole.PageTab;

            public override AccessibleObject Parent => _owner.AccessibilityObject;

            public override string Name => Model?.Title ?? string.Empty;

            public override Rectangle Bounds
            {
                get
                {
                    var rects = _owner._captionLayout.TabRects;
                    if (_index < 0 || _index >= rects.Count)
                        return Rectangle.Empty;

                    return _owner.RectangleToScreen(rects[_index].Value);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    var state = AccessibleStates.Selectable;
                    if (Model?.IsActive == true)
                        state |= AccessibleStates.Selected;
                    return state;
                }
            }

            /// <summary>Activating a tab selects its panel, the same action a click performs.</summary>
            public override string DefaultAction => "Select";

            public override void DoDefaultAction()
            {
                if (Model?.Tag is DockPanel panel)
                    _owner.ActivatePanel(panel, true);
            }

            public override void Select(AccessibleSelection flags)
            {
                if ((flags & (AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus)) != 0)
                    DoDefaultAction();
            }

            private CaptionTabModel Model
            {
                get
                {
                    var rects = _owner._captionLayout.TabRects;
                    return _index >= 0 && _index < rects.Count ? rects[_index].Key : null;
                }
            }
        }
    }
}
