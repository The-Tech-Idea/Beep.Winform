using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;

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
                string key = ComposeItemKey(info.Item);
                if (!info.RowRect.IsEmpty)
                    _owner._hitTest.AddHitArea($"row_{key}", info.RowRect);
                if (!info.CheckRect.IsEmpty && _owner.ShowCheckBox)
                    _owner._hitTest.AddHitArea($"check_{key}", info.CheckRect);
                if (!info.IconRect.IsEmpty)
                    _owner._hitTest.AddHitArea($"icon_{key}", info.IconRect);
                if (!info.TextRect.IsEmpty)
                    _owner._hitTest.AddHitArea($"text_{key}", info.TextRect);
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
                string key = parts[1];
                var info = _layout.GetCachedLayout().FirstOrDefault(i => ComposeItemKey(i.Item) == key);
                item = info?.Item;
            }
            return item != null;
        }

        private static string ComposeItemKey(SimpleItem item)
        {
            if (item is BeepListItem rich && rich.IsGroupHeader)
            {
                return $"group::{(rich.Category ?? rich.Text ?? string.Empty)}";
            }

            if (!string.IsNullOrWhiteSpace(item.GuidId))
            {
                return item.GuidId.Trim();
            }

            return $"item::{item.Text ?? item.Name ?? string.Empty}";
        }
    }
}
