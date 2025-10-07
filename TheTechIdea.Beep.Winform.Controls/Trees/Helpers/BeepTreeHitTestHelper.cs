using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Helper class for tree-specific hit testing logic.
    /// Uses BaseControl._hitTest for the actual hit testing infrastructure.
    /// </summary>
    public class BeepTreeHitTestHelper
    {
        private readonly BeepTree _owner;
        private readonly BeepTreeLayoutHelper _layoutHelper;
        private SimpleItem _lastHoveredItem;
        private Rectangle _lastHoveredRect;

        public BeepTreeHitTestHelper(BeepTree owner, BeepTreeLayoutHelper layoutHelper)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _layoutHelper = layoutHelper ?? throw new ArgumentNullException(nameof(layoutHelper));
            _lastHoveredRect = Rectangle.Empty;
        }

        #region Hit Testing

        /// <summary>
        /// Registers all hit areas for visible nodes with BaseControl._hitTest.
        /// Call this after layout calculation.
        /// </summary>
        public void RegisterHitAreas()
        {
            // Clear previous hit areas
            _owner._hitTest.ClearHitList();

            var layoutCache = _layoutHelper.GetCachedLayout();
            if (layoutCache == null || layoutCache.Count == 0)
                return;

            // Register hit areas for visible nodes in viewport
            foreach (var node in layoutCache)
            {
                if (!_layoutHelper.IsNodeInViewport(node))
                    continue;

                // Transform rectangles to viewport coordinates
                Rectangle rowVp = _layoutHelper.TransformToViewport(node.RowRectContent);

                // Register toggle area (if has children)
                if (node.Item.Children != null && node.Item.Children.Count > 0)
                {
                    Rectangle toggleVp = _layoutHelper.TransformToViewport(node.ToggleRectContent);
                    _owner._hitTest.AddHitArea($"toggle_{node.Item.GuidId}", toggleVp);
                }

                // Register checkbox area (if enabled)
                if (_owner.ShowCheckBox && !node.CheckRectContent.IsEmpty)
                {
                    Rectangle checkVp = _layoutHelper.TransformToViewport(node.CheckRectContent);
                    _owner._hitTest.AddHitArea($"check_{node.Item.GuidId}", checkVp);
                }

                // Register icon area (if exists)
                if (!string.IsNullOrEmpty(node.Item.ImagePath) && !node.IconRectContent.IsEmpty)
                {
                    Rectangle iconVp = _layoutHelper.TransformToViewport(node.IconRectContent);
                    _owner._hitTest.AddHitArea($"icon_{node.Item.GuidId}", iconVp);
                }

                // Register text area
                if (!node.TextRectContent.IsEmpty)
                {
                    Rectangle textVp = _layoutHelper.TransformToViewport(node.TextRectContent);
                    _owner._hitTest.AddHitArea($"text_{node.Item.GuidId}", textVp);
                }

                // Register row area (fallback for entire row)
                _owner._hitTest.AddHitArea($"row_{node.Item.GuidId}", rowVp);
            }
        }

        /// <summary>
        /// Performs hit testing to find what part of which node was clicked.
        /// Uses BaseControl._hitTest.HitTest() internally.
        /// </summary>
        public bool HitTest(Point point, out string hitName, out SimpleItem item, out Rectangle rect)
        {
            hitName = string.Empty;
            item = null;
            rect = Rectangle.Empty;

            // Use BaseControl's hit test
            if (!_owner._hitTest.HitTest(point, out var hitTest))
                return false;

            hitName = hitTest.Name;
            rect = hitTest.TargetRect;

            // Extract GUID from hit name (format: "type_guid")
            var parts = hitName.Split('_');
            if (parts.Length == 2)
            {
                string guid = parts[1];
                var layoutCache = _layoutHelper.GetCachedLayout();
                var nodeInfo = layoutCache.FirstOrDefault(n => n.Item.GuidId == guid);
                item = nodeInfo.Item;
            }

            return item != null;
        }

        /// <summary>
        /// Gets the node at the specified point.
        /// </summary>
        public SimpleItem GetNodeAt(Point point)
        {
            if (HitTest(point, out _, out var item, out _))
                return item;
            return null;
        }

        /// <summary>
        /// Gets the type of area hit (toggle, check, icon, text, row).
        /// </summary>
        public string GetHitPartType(Point point)
        {
            if (HitTest(point, out var hitName, out _, out _))
            {
                var parts = hitName.Split('_');
                return parts.Length > 0 ? parts[0] : string.Empty;
            }
            return string.Empty;
        }

        #endregion

        #region Hover Management

        /// <summary>
        /// Updates hover state based on current mouse position.
        /// </summary>
        public SimpleItem UpdateHover(Point mousePosition)
        {
            if (HitTest(mousePosition, out var hitName, out var hitItem, out var hitRect))
            {
                if (_lastHoveredItem != hitItem)
                {
                    // Clear previous hover
                    if (_lastHoveredItem != null && !_lastHoveredRect.IsEmpty)
                    {
                        _owner.Invalidate(_lastHoveredRect);
                    }

                    // Set new hover
                    _lastHoveredItem = hitItem;
                    _lastHoveredRect = hitRect;

                    // Invalidate new hover area
                    if (!_lastHoveredRect.IsEmpty)
                    {
                        _owner.Invalidate(_lastHoveredRect);
                    }
                }
                return hitItem;
            }
            else
            {
                // No hover
                ClearHover();
                return null;
            }
        }

        /// <summary>
        /// Clears the current hover state.
        /// </summary>
        public void ClearHover()
        {
            if (_lastHoveredItem != null && !_lastHoveredRect.IsEmpty)
            {
                _owner.Invalidate(_lastHoveredRect);
            }

            _lastHoveredItem = null;
            _lastHoveredRect = Rectangle.Empty;
        }

        /// <summary>
        /// Gets the currently hovered item.
        /// </summary>
        public SimpleItem GetHoveredItem()
        {
            return _lastHoveredItem;
        }

        #endregion
    }
}
