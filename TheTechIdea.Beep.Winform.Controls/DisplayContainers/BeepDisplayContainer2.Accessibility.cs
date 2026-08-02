using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    /// <summary>
    /// Exposes the tab strip to assistive technology.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This container paints its tabs itself rather than hosting child controls, so there is nothing
    /// for MSAA to enumerate on its own: without this, a screen reader sees one anonymous client area
    /// and none of the tabs. <c>BeepTabs</c> solved the same problem for its own strip; that work does
    /// not carry over here, because the two share no painting or layout code.
    /// </para>
    /// <para>
    /// The tree is a <see cref="AccessibleRole.PageTabList"/> whose children are
    /// <see cref="AccessibleRole.PageTab"/>, one per visible tab, named by caption. Icon-only tabs -
    /// pinned tabs and the Left/Right rail - still report their title, which is the only thing
    /// identifying them once the caption is suppressed.
    /// </para>
    /// </remarks>
    public partial class BeepDisplayContainer2
    {
        protected override AccessibleObject CreateAccessibilityInstance()
            => new DisplayContainerAccessibleObject(this);

        /// <summary>Screen bounds of a tab, taken from the rectangle the painter laid out.</summary>
        internal Rectangle GetAccessibleTabBounds(AddinTab tab)
        {
            if (tab == null || tab.Bounds.IsEmpty) return Rectangle.Empty;
            return RectangleToScreen(tab.Bounds);
        }

        private sealed class DisplayContainerAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepDisplayContainer2 _owner;

            public DisplayContainerAccessibleObject(BeepDisplayContainer2 owner) : base(owner)
                => _owner = owner;

            public override AccessibleRole Role => AccessibleRole.PageTabList;

            public override int GetChildCount() => _owner._tabs?.Count ?? 0;

            public override AccessibleObject GetChild(int index)
            {
                var tabs = _owner._tabs;
                if (tabs == null || index < 0 || index >= tabs.Count) return null;
                return new TabAccessibleObject(_owner, tabs[index]);
            }
        }

        private sealed class TabAccessibleObject : AccessibleObject
        {
            private readonly BeepDisplayContainer2 _owner;
            private readonly AddinTab _tab;

            public TabAccessibleObject(BeepDisplayContainer2 owner, AddinTab tab)
            {
                _owner = owner;
                _tab = tab;
            }

            public override AccessibleObject Parent => _owner.AccessibilityObject;

            public override AccessibleRole Role => AccessibleRole.PageTab;

            /// <summary>The caption, which for an icon-only tab is the only identifying text.</summary>
            public override string Name => _tab?.Title ?? string.Empty;

            public override string Description => _owner.GetTooltipTextFor(_tab);

            public override Rectangle Bounds => _owner.GetAccessibleTabBounds(_tab);

            public override AccessibleStates State
            {
                get
                {
                    var state = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    if (ReferenceEquals(_tab, _owner._activeTab))
                        state |= AccessibleStates.Selected | AccessibleStates.Focused;
                    if (_tab != null && !_tab.IsVisible)
                        state |= AccessibleStates.Invisible;
                    return state;
                }
            }

            public override string DefaultAction => "Activate";

            public override void DoDefaultAction()
            {
                if (_tab != null) _owner.ActivateTab(_tab);
            }
        }
    }
}
