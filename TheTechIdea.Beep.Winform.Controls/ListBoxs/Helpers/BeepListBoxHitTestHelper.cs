using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers
{
    internal class BeepListBoxHitTestHelper
    {
        private readonly BeepListBox _owner;
        private readonly BeepListBoxLayoutHelper _layout;

        public BeepListBoxHitTestHelper(BeepListBox owner, BeepListBoxLayoutHelper layout)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public void RegisterHitAreas()
        {
            _owner._hitTest.ClearHitList();
            var cache = _layout.GetCachedLayout();
            if (cache == null || cache.Count == 0) return;

            foreach (var info in cache)
            {
                if (!info.RowRect.IsEmpty)
                    _owner._hitTest.AddHitArea($"row_{info.Item.GuidId}", info.RowRect);
                if (!info.CheckRect.IsEmpty && _owner.ShowCheckBox)
                    _owner._hitTest.AddHitArea($"check_{info.Item.GuidId}", info.CheckRect);
                if (!info.IconRect.IsEmpty)
                    _owner._hitTest.AddHitArea($"icon_{info.Item.GuidId}", info.IconRect);
                if (!info.TextRect.IsEmpty)
                    _owner._hitTest.AddHitArea($"text_{info.Item.GuidId}", info.TextRect);
            }
        }

        public bool HitTest(Point point, out string hitName, out SimpleItem item, out Rectangle rect)
        {
            hitName = string.Empty;
            item = null;
            rect = Rectangle.Empty;

            if (!_owner._hitTest.HitTest(point, out var hit)) return false;
            hitName = hit.Name;
            rect = hit.TargetRect;
            var parts = hitName.Split('_');
            if (parts.Length == 2)
            {
                string guid = parts[1];
                var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item.GuidId == guid);
                item = info?.Item;
            }
            return item != null;
        }
    }
}
