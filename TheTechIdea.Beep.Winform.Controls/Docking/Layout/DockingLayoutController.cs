using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Interop;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Orchestrates hierarchical layout calculations for docking panels.
    /// Manages panel positioning, splitter sizing, and dynamic layout recalculation.
    /// </summary>
    public class DockingLayoutController
    {
        private readonly DockLayoutTree _layoutTree;
        private readonly IDockingPainter _painter;
        private readonly LayoutCalculator _calculator;
        private Rectangle _containerBounds;
        private bool _isDirty = true;

        // Cache for layout calculations (invalidated on structural changes)
        private Dictionary<string, Rectangle> _layoutCache = new Dictionary<string, Rectangle>();

        // Constants
        private const int MIN_PANEL_WIDTH = 50;
        private const int MIN_PANEL_HEIGHT = 50;
        private const int SPLITTER_WIDTH = 4;
        private const int TAB_STRIP_HEIGHT = 30;
        private const int CHROME_HEIGHT = 24;

        public DockingLayoutController(DockLayoutTree layoutTree, IDockingPainter painter)
        {
            _layoutTree = layoutTree ?? throw new ArgumentNullException(nameof(layoutTree));
            _painter = painter ?? throw new ArgumentNullException(nameof(painter));
            _calculator = new LayoutCalculator(MIN_PANEL_WIDTH, MIN_PANEL_HEIGHT, SPLITTER_WIDTH);
        }

        /// <summary>
        /// Gets or sets the container bounds for all layout calculations.
        /// Set this whenever the MDI client is resized.
        /// </summary>
        public Rectangle ContainerBounds
        {
            get => _containerBounds;
            set
            {
                if (_containerBounds != value)
                {
                    _containerBounds = value;
                    InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// Gets the metrics used for layout calculations (tab height, chrome height, splitter width).
        /// </summary>
        public LayoutMetrics Metrics => new LayoutMetrics
        {
            TabStripHeight = TAB_STRIP_HEIGHT,
            ChromeHeight = CHROME_HEIGHT,
            SplitterWidth = SPLITTER_WIDTH,
            MinPanelWidth = MIN_PANEL_WIDTH,
            MinPanelHeight = MIN_PANEL_HEIGHT
        };

        /// <summary>
        /// Calculates layout for all panels in the hierarchy.
        /// Returns a dictionary mapping panel keys to their calculated bounds.
        /// </summary>
        public Dictionary<string, Rectangle> CalculateLayout()
        {
            if (!_isDirty && _layoutCache.Count > 0)
                return new Dictionary<string, Rectangle>(_layoutCache);

            _layoutCache.Clear();

            if (_layoutTree.Root == null)
                return _layoutCache;

            // Calculate bounds for root group (fills entire container)
            CalculateGroupBounds(_layoutTree.Root, _containerBounds);

            _isDirty = false;
            return new Dictionary<string, Rectangle>(_layoutCache);
        }

        /// <summary>
        /// Gets the bounds for a specific panel (or null if not found).
        /// </summary>
        public Rectangle? GetPanelBounds(string panelKey)
        {
            var layout = CalculateLayout();
            if (layout.TryGetValue(panelKey, out var bounds))
                return bounds;
            return null;
        }

        /// <summary>
        /// Gets the bounds for the content area of a panel (excluding title bar).
        /// </summary>
        public Rectangle? GetPanelContentBounds(string panelKey)
        {
            var panelBounds = GetPanelBounds(panelKey);
            if (!panelBounds.HasValue)
                return null;

            var bounds = panelBounds.Value;
            bounds.Y += CHROME_HEIGHT;
            bounds.Height -= CHROME_HEIGHT;
            return bounds;
        }

        /// <summary>
        /// Gets the bounds for the splitter between two groups.
        /// </summary>
        public Rectangle GetSplitterBounds(string groupId)
        {
            var group = _layoutTree.GetGroup(groupId);
            if (group == null)
                return Rectangle.Empty;

            // TODO: Implement based on group orientation and children
            return Rectangle.Empty;
        }

        /// <summary>
        /// Finds which panel is at the given point in container coordinates.
        /// </summary>
        public string FindPanelAtPoint(Point point)
        {
            var layout = CalculateLayout();
            foreach (var kvp in layout)
            {
                if (kvp.Value.Contains(point))
                    return kvp.Key;
            }
            return null;
        }

        /// <summary>
        /// Finds which splitter is at the given point (if any).
        /// </summary>
        public string FindSplitterAtPoint(Point point)
        {
            // TODO: Implement splitter hit-testing
            return null;
        }

        /// <summary>
        /// Applies a drag delta to a splitter, recalculating layout.
        /// dragDelta is the mouse movement (positive = drag right/down, negative = drag left/up).
        /// </summary>
        public void DragSplitter(string groupId, int dragDelta, bool isVertical)
        {
            var group = _layoutTree.GetGroup(groupId);
            if (group == null)
                return;

            // Calculate new ratio based on drag delta
            float ratio = _calculator.CalculateNewRatio(group.SplitRatio, dragDelta, 
                isVertical ? _containerBounds.Width : _containerBounds.Height);

            // Clamp ratio to valid range
            ratio = Math.Max(0.1f, Math.Min(0.9f, ratio));

            group.SplitRatio = ratio;
            InvalidateLayout();
        }

        /// <summary>
        /// Invalidates the layout cache, forcing recalculation on next call to CalculateLayout().
        /// Called when panels are added/removed or properties change.
        /// </summary>
        public void InvalidateLayout()
        {
            _isDirty = true;
            _layoutCache.Clear();
        }

        /// <summary>
        /// Gets diagnostic information about the current layout.
        /// </summary>
        public LayoutDiagnostics GetDiagnostics()
        {
            var layout = CalculateLayout();
            return new LayoutDiagnostics
            {
                TotalPanels = _layoutTree.GetAllPanels().Count,
                TotalGroups = _layoutTree.GetAllGroups().Count,
                CalculatedPanels = layout.Count,
                CacheValid = !_isDirty,
                ContainerBounds = _containerBounds,
                PanelBounds = new Dictionary<string, Rectangle>(layout)
            };
        }

        #region Private Layout Calculation

        /// <summary>
        /// Recursively calculates bounds for a group and its children.
        /// </summary>
        private void CalculateGroupBounds(DockGroup group, Rectangle groupBounds)
        {
            if (group == null || groupBounds.Width <= 0 || groupBounds.Height <= 0)
                return;

            // If group has no children, this shouldn't happen but handle gracefully
            if (group.Children.Count == 0 && group.Panels.Count == 0)
                return;

            // If group has only panels (leaf group), distribute space among them
            if (group.Children.Count == 0 && group.Panels.Count > 0)
            {
                LayoutPanelsInGroup(group, groupBounds);
                return;
            }

            // If group has child groups, split space according to orientation
            if (group.Children.Count > 0)
            {
                LayoutChildGroups(group, groupBounds);
                return;
            }
        }

        /// <summary>
        /// Distributes bounds among panels in a leaf group (tabs).
        /// </summary>
        private void LayoutPanelsInGroup(DockGroup group, Rectangle groupBounds)
        {
            if (group.Panels.Count == 0)
                return;

            // All panels in a group share the same bounds (they're tabbed)
            var panelBounds = new Rectangle(
                groupBounds.X,
                groupBounds.Y + TAB_STRIP_HEIGHT,  // Account for tab strip
                groupBounds.Width,
                Math.Max(MIN_PANEL_HEIGHT, groupBounds.Height - TAB_STRIP_HEIGHT)
            );

            foreach (var panel in group.Panels)
            {
                _layoutCache[panel.Key] = panelBounds;
            }
        }

        /// <summary>
        /// Distributes bounds among child groups based on orientation and split ratio.
        /// </summary>
        private void LayoutChildGroups(DockGroup parentGroup, Rectangle parentBounds)
        {
            if (parentGroup.Children.Count == 0)
                return;

            if (parentGroup.SplitOrientation == SplitOrientation.Vertical)
            {
                LayoutVerticalGroups(parentGroup, parentBounds);
            }
            else
            {
                LayoutHorizontalGroups(parentGroup, parentBounds);
            }
        }

        /// <summary>
        /// Distributes bounds vertically (left/right split).
        /// </summary>
        private void LayoutVerticalGroups(DockGroup parentGroup, Rectangle parentBounds)
        {
            var childGroups = parentGroup.Children.ToList();
            if (childGroups.Count == 0)
                return;

            // Calculate width of first group using split ratio
            float totalWidth = parentBounds.Width - (SPLITTER_WIDTH * (childGroups.Count - 1));
            int firstGroupWidth = (int)(totalWidth * parentGroup.SplitRatio);
            firstGroupWidth = Math.Max(MIN_PANEL_WIDTH, Math.Min((int)totalWidth - MIN_PANEL_WIDTH, firstGroupWidth));

            // First group bounds (left side)
            var firstGroupBounds = new Rectangle(
                parentBounds.X,
                parentBounds.Y,
                firstGroupWidth,
                parentBounds.Height
            );

            CalculateGroupBounds(childGroups[0], firstGroupBounds);

            // Remaining groups get the rest (right side)
            if (childGroups.Count > 1)
            {
                int remainingX = parentBounds.X + firstGroupWidth + SPLITTER_WIDTH;
                int remainingWidth = parentBounds.Width - firstGroupWidth - SPLITTER_WIDTH;

                var remainingBounds = new Rectangle(
                    remainingX,
                    parentBounds.Y,
                    remainingWidth,
                    parentBounds.Height
                );

                // If multiple remaining groups, create a container group
                if (childGroups.Count > 2)
                {
                    // Recursively layout remaining groups
                    foreach (var child in childGroups.Skip(1))
                    {
                        CalculateGroupBounds(child, remainingBounds);
                    }
                }
                else
                {
                    CalculateGroupBounds(childGroups[1], remainingBounds);
                }
            }
        }

        /// <summary>
        /// Distributes bounds horizontally (top/bottom split).
        /// </summary>
        private void LayoutHorizontalGroups(DockGroup parentGroup, Rectangle parentBounds)
        {
            var childGroups = parentGroup.Children.ToList();
            if (childGroups.Count == 0)
                return;

            // Calculate height of first group using split ratio
            float totalHeight = parentBounds.Height - (SPLITTER_WIDTH * (childGroups.Count - 1));
            int firstGroupHeight = (int)(totalHeight * parentGroup.SplitRatio);
            firstGroupHeight = Math.Max(MIN_PANEL_HEIGHT, Math.Min((int)totalHeight - MIN_PANEL_HEIGHT, firstGroupHeight));

            // First group bounds (top)
            var firstGroupBounds = new Rectangle(
                parentBounds.X,
                parentBounds.Y,
                parentBounds.Width,
                firstGroupHeight
            );

            CalculateGroupBounds(childGroups[0], firstGroupBounds);

            // Remaining groups get the rest (bottom)
            if (childGroups.Count > 1)
            {
                int remainingY = parentBounds.Y + firstGroupHeight + SPLITTER_WIDTH;
                int remainingHeight = parentBounds.Height - firstGroupHeight - SPLITTER_WIDTH;

                var remainingBounds = new Rectangle(
                    parentBounds.X,
                    remainingY,
                    parentBounds.Width,
                    remainingHeight
                );

                if (childGroups.Count > 2)
                {
                    foreach (var child in childGroups.Skip(1))
                    {
                        CalculateGroupBounds(child, remainingBounds);
                    }
                }
                else
                {
                    CalculateGroupBounds(childGroups[1], remainingBounds);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Metrics used for layout calculations.
    /// </summary>
    public struct LayoutMetrics
    {
        public int TabStripHeight { get; set; }
        public int ChromeHeight { get; set; }
        public int SplitterWidth { get; set; }
        public int MinPanelWidth { get; set; }
        public int MinPanelHeight { get; set; }
    }

    /// <summary>
    /// Diagnostic information about current layout state.
    /// </summary>
    public class LayoutDiagnostics
    {
        public int TotalPanels { get; set; }
        public int TotalGroups { get; set; }
        public int CalculatedPanels { get; set; }
        public bool CacheValid { get; set; }
        public Rectangle ContainerBounds { get; set; }
        public Dictionary<string, Rectangle> PanelBounds { get; set; }
    }
}
