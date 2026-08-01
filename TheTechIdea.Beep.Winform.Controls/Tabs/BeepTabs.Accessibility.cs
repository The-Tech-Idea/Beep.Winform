using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        /// <summary>
        /// Exposes one accessible child per tab, so assistive technology can enumerate, describe and
        /// select tabs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This did not exist. <see cref="BeepTabAccessibleObjectFactory"/> — 228 lines building tab
        /// and close-button accessible objects with the right roles, states, names and Select/Close
        /// actions — had <b>zero callers</b>, and <c>BeepTabHeaderHost.Accessibility.cs</c> was an
        /// empty partial whose only content was a comment saying per-tab objects would be exposed
        /// "in a future update". A screen reader therefore saw one opaque control: the tabs could not
        /// be enumerated, named, or activated.
        /// </para>
        /// <para>
        /// Bounds come from the layout snapshot, which is what is actually painted, so the rectangle
        /// reported to a screen reader is the rectangle the user sees and clicks.
        /// </para>
        /// </remarks>
        protected override AccessibleObject CreateAccessibilityInstance()
            => new BeepTabsAccessibleObject(this);

        /// <summary>Screen bounds of a tab, taken from the snapshot the painter used.</summary>
        internal Rectangle GetAccessibleTabBounds(BeepTabItem item)
        {
            if (item == null) return Rectangle.Empty;

            foreach (BeepTabHeaderItemLayout layout in _headerHost.LayoutSnapshot.Items)
            {
                if (layout.Item?.Index == item.Index)
                {
                    return layout.Bounds.IsEmpty ? Rectangle.Empty : RectangleToScreen(layout.Bounds);
                }
            }

            return Rectangle.Empty;
        }

        /// <summary>Screen bounds of a tab's close button, or empty when it has none.</summary>
        internal Rectangle GetAccessibleCloseBounds(BeepTabItem item)
        {
            if (item == null) return Rectangle.Empty;

            foreach (BeepTabHeaderItemLayout layout in _headerHost.LayoutSnapshot.Items)
            {
                if (layout.Item?.Index == item.Index && layout.HasCloseButton)
                {
                    return layout.CloseButtonBounds.IsEmpty
                        ? Rectangle.Empty
                        : RectangleToScreen(layout.CloseButtonBounds);
                }
            }

            return Rectangle.Empty;
        }

        private sealed class BeepTabsAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepTabs _tabs;

            public BeepTabsAccessibleObject(BeepTabs owner) : base(owner) => _tabs = owner;

            public override AccessibleRole Role => AccessibleRole.PageTabList;

            public override int GetChildCount() => BuildChildren().Count;

            public override AccessibleObject? GetChild(int index)
            {
                var children = BuildChildren();
                return index >= 0 && index < children.Count ? children[index] : null;
            }

            /// <summary>
            /// Built per call rather than cached: tabs are added, closed and reordered at runtime,
            /// and a cached tree would hand assistive technology stale rectangles after any of those.
            /// </summary>
            private List<AccessibleObject> BuildChildren()
            {
                var children = new List<AccessibleObject>();
                if (_tabs.IsDisposed || _tabs.Disposing) return children;

                foreach (BeepTabItem item in _tabs.GetHostedSourceItemsSnapshot())
                {
                    if (!item.IsVisible) continue;

                    children.Add(BeepTabAccessibleObjectFactory.CreateTabObject(
                        _tabs,
                        item,
                        i => _tabs.GetAccessibleTabBounds(i),
                        index => _tabs.TrySelectHeaderTab(index)));

                    AccessibleObject? close = BeepTabAccessibleObjectFactory.CreateCloseButtonObject(
                        _tabs,
                        item,
                        i => _tabs.GetAccessibleCloseBounds(i),
                        index => _tabs.TryCloseHeaderTab(index));

                    if (close != null) children.Add(close);
                }

                return children;
            }
        }
    }
}
