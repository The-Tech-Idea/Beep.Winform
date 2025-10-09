using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Helper class for tree-specific hit testing logic.
    /// Uses BaseControl hit-test infrastructure.
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
        /// Registers all hit areas for visible nodes with BaseControl hit-test infra.
        /// Aligns with BeepMenuBar approach: compute client rects and call AddHitArea.
        /// </summary>
        public void RegisterHitAreas()
        {
            // Ensure latest layout rects
            _owner.UpdateDrawingRect();

            // Clear previous hit areas via BaseControl API
            _owner.ClearHitList();

            var layoutCache = _layoutHelper.GetCachedLayout();
            if (layoutCache == null || layoutCache.Count == 0)
                return;

            // Helper to transform content -> viewport (client) coordinates
            Rectangle ToViewport(Rectangle rc) => new Rectangle(
                _owner.DrawingRect.Left + rc.X - _owner.XOffset,
                _owner.DrawingRect.Top + rc.Y - _owner.YOffset,
                rc.Width,
                rc.Height);

            foreach (var node in layoutCache)
            {
                if (!_layoutHelper.IsNodeInViewport(node))
                    continue;

                int rowH = node.RowHeight > 0 ? node.RowHeight : _owner.GetScaledMinRowHeight();

                // Row rect spans full drawing width at node Y (viewport coords)
                Rectangle rowVp = new Rectangle(
                    _owner.DrawingRect.Left,
                    _owner.DrawingRect.Top + (node.Y - _owner.YOffset),
                    _owner.DrawingRect.Width,
                    rowH);

                // Toggle (only when node has children and rect is valid)
                if (node.Item.Children != null && node.Item.Children.Count > 0 && !node.ToggleRectContent.IsEmpty)
                {
                    var toggleVp = ToViewport(node.ToggleRectContent);
                    _owner.AddHitArea($"toggle_{node.Item.GuidId}", toggleVp);
                }

                // Checkbox
                if (_owner.ShowCheckBox && !node.CheckRectContent.IsEmpty)
                {
                    var checkVp = ToViewport(node.CheckRectContent);
                    _owner.AddHitArea($"check_{node.Item.GuidId}", checkVp);
                }

                // Icon
                if (!node.IconRectContent.IsEmpty)
                {
                    var iconVp = ToViewport(node.IconRectContent);
                    _owner.AddHitArea($"icon_{node.Item.GuidId}", iconVp);
                }

                // Text
                if (!node.TextRectContent.IsEmpty)
                {
                    var textVp = ToViewport(node.TextRectContent);
                    _owner.AddHitArea($"text_{node.Item.GuidId}", textVp);
                }

                // Row catch-all last
                _owner.AddHitArea($"row_{node.Item.GuidId}", rowVp);
            }
        }

        /// <summary>
        /// Performs hit testing using BaseControl API.
        /// </summary>
        public bool HitTest(Point point, out string hitName, out SimpleItem item, out Rectangle rect)
        {
            hitName = string.Empty;
            item = null;
            rect = Rectangle.Empty;

            if (!_owner.HitTest(point, out var hitTest))
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
