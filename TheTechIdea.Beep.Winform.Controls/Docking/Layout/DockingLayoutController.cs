using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Canonical layout authority for the docking system. Computes a dock-site edge layout
    /// (Top/Bottom span the width, then Left/Right span the remaining height, Fill takes the
    /// rest) from the <see cref="DockLayoutTree"/> root children. Each edge group is sized
    /// proportionally from its <see cref="DockGroup.SplitRatio"/> and clamped by per-group
    /// minimum sizes. Produces an immutable <see cref="DockLayoutResult"/> with panel bounds,
    /// group bounds and splitter rectangles, and is the single source of truth for splitter
    /// hit-testing and drag-resize.
    /// </summary>
    public class DockingLayoutController
    {
        private readonly DockLayoutTree _layoutTree;
        private readonly IDockingPainter _painter;
        private readonly LayoutCalculator _calculator;
        private Rectangle _containerBounds;
        private bool _isDirty = true;

        private DockLayoutResult _result;

        // Transient: which panel currently owns the whole container. Never persisted, never
        // reflected into the tree - see MaximisedPanelKey.
        private string _maximisedPanelKey;

        // Constants
        private const int MIN_PANEL_WIDTH = 50;
        private const int MIN_PANEL_HEIGHT = 50;
        private const int SPLITTER_WIDTH = 4;
        private const int TAB_STRIP_HEIGHT = 30;
        private const int CHROME_HEIGHT = 24;

        // Order in which edges claim space; Top/Bottom span full width first, then Left/Right.
        private static readonly DockPosition[] EdgeOrder =
            { DockPosition.Top, DockPosition.Bottom, DockPosition.Left, DockPosition.Right };

        public DockingLayoutController(DockLayoutTree layoutTree, IDockingPainter painter)
        {
            _layoutTree = layoutTree ?? throw new ArgumentNullException(nameof(layoutTree));
            _painter = painter ?? throw new ArgumentNullException(nameof(painter));
            _calculator = new LayoutCalculator(MIN_PANEL_WIDTH, MIN_PANEL_HEIGHT, SPLITTER_WIDTH);
        }

        /// <summary>
        /// Gets or sets the container bounds for all layout calculations.
        /// Set this whenever the host client area is resized.
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
        /// Key of the panel that temporarily occupies the whole container, or <c>null</c> when the
        /// layout is arranged normally.
        /// </summary>
        /// <remarks>
        /// Maximising is a <b>transient property of the layout pass, never a change to the tree</b>.
        /// Nothing about groups, split ratios, active tabs or panel states is touched while a panel
        /// is maximised, so restoring is a matter of clearing this key and laying out again — not a
        /// reconstruction that can drift. Implementations that maximise by rearranging the tree are
        /// exactly the ones that lose splitter positions on restore.
        /// </remarks>
        public string MaximisedPanelKey
        {
            get => _maximisedPanelKey;
            set
            {
                string normalized = string.IsNullOrWhiteSpace(value) ? null : value;
                if (_maximisedPanelKey == normalized)
                    return;

                _maximisedPanelKey = normalized;
                InvalidateLayout();
            }
        }

        /// <summary>True while a panel is maximised.</summary>
        public bool IsMaximised => _maximisedPanelKey != null;

        /// <summary>
        /// Metrics used for layout calculations (tab height, chrome height, splitter width).
        /// </summary>
        public LayoutMetrics Metrics => new LayoutMetrics
        {
            TabStripHeight = TAB_STRIP_HEIGHT,
            ChromeHeight = CHROME_HEIGHT,
            SplitterWidth = SPLITTER_WIDTH,
            MinPanelWidth = MIN_PANEL_WIDTH,
            MinPanelHeight = MIN_PANEL_HEIGHT
        };

        /// <summary>Thickness of every splitter strip (DPI scaling applied by the caller).</summary>
        public int SplitterWidth => SPLITTER_WIDTH;

        /// <summary>
        /// Computes (or returns cached) layout for the current <see cref="ContainerBounds"/>.
        /// </summary>
        public DockLayoutResult CalculateLayoutResult()
        {
            if (!_isDirty && _result != null)
                return _result;

            _result = BuildLayout(_containerBounds);
            _isDirty = false;
            return _result;
        }

        /// <summary>
        /// Sets <see cref="ContainerBounds"/> to <paramref name="rootBounds"/> and recomputes.
        /// </summary>
        public DockLayoutResult CalculateLayout(Rectangle rootBounds)
        {
            ContainerBounds = rootBounds;   // invalidates if changed
            return CalculateLayoutResult();
        }

        /// <summary>
        /// Back-compat: returns a panel-key → bounds map for the current container bounds.
        /// </summary>
        public Dictionary<string, Rectangle> CalculateLayout()
        {
            var result = CalculateLayoutResult();
            return new Dictionary<string, Rectangle>(
                result.PanelBounds.ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        /// <summary>Gets the bounds for a specific panel (or null if not found).</summary>
        public Rectangle? GetPanelBounds(string panelKey)
            => CalculateLayoutResult().GetPanelBounds(panelKey);

        /// <summary>Gets the content bounds for a panel (excluding the caption/chrome).</summary>
        public Rectangle? GetPanelContentBounds(string panelKey)
        {
            var panelBounds = GetPanelBounds(panelKey);
            if (!panelBounds.HasValue)
                return null;

            var bounds = panelBounds.Value;
            bounds.Y += CHROME_HEIGHT;
            bounds.Height = Math.Max(0, bounds.Height - CHROME_HEIGHT);
            return bounds;
        }

        /// <summary>Gets the bounds for the splitter that controls the given edge group.</summary>
        public Rectangle GetSplitterBounds(string groupId)
        {
            foreach (var hit in CalculateLayoutResult().Splitters)
            {
                if (hit.GroupId == groupId)
                    return hit.Bounds;
            }
            return Rectangle.Empty;
        }

        /// <summary>Finds which panel is at the given point in container coordinates.</summary>
        public string FindPanelAtPoint(Point point)
        {
            foreach (var kvp in CalculateLayoutResult().PanelBounds)
            {
                if (kvp.Value.Contains(point))
                    return kvp.Key;
            }
            return null;
        }

        /// <summary>Finds which splitter (if any) is at the given point.</summary>
        public DockSplitterHit? FindSplitterAtPoint(Point point)
            => CalculateLayoutResult().FindSplitterAt(point);

        /// <summary>
        /// Applies a pixel drag delta to an edge group's splitter and re-lays out.
        /// Delta is along the resize axis (positive = grow the edge group). The axis is
        /// derived from the group's <see cref="DockGroup.Position"/>.
        /// Handles root-level edge splitters and nested child splitters (format: {parentId}_child_{i}).
        /// </summary>
        public void DragSplitter(string groupId, int deltaPx)
        {
            var group = _layoutTree.GetGroup(groupId);
            DockGroup adjustGroup = null;

            if (group != null)
            {
                adjustGroup = group;
            }
            else
            {
                // Try nested child splitter: parse "{parentId}_child_{i}"
                int lastUnderscore = groupId.LastIndexOf("_child_", StringComparison.Ordinal);
                if (lastUnderscore >= 0)
                {
                    string parentId = groupId.Substring(0, lastUnderscore);
                    var parent = _layoutTree.GetGroup(parentId);
                    if (parent != null)
                        adjustGroup = parent;
                }
            }

            if (adjustGroup == null)
                return;

            bool horizontalAxis = adjustGroup.Position == DockPosition.Left || adjustGroup.Position == DockPosition.Right
                || (adjustGroup.Children.Count >= 2 && adjustGroup.SplitOrientation == SplitOrientation.Horizontal);

            // For nested groups, use the parent group's laid-out bounds as the axis reference.
            int axisSize;
            var cachedResult = _result;
            if (cachedResult != null && cachedResult.GroupBounds.TryGetValue(adjustGroup.Id, out var parentBounds) && !parentBounds.IsEmpty)
            {
                axisSize = horizontalAxis ? parentBounds.Width : parentBounds.Height;
            }
            else
            {
                axisSize = horizontalAxis ? _containerBounds.Width : _containerBounds.Height;
            }

            // Right/Bottom edges grow when dragged toward the center (negative screen delta),
            // so invert the sign for root-level edge splitters. For nested child splitters
            // (group was not found directly, adjustGroup is the parent), the first child is
            // always left/top within its parent — delta sign is always positive.
            int signedDelta = deltaPx;
            if (group != null &&
                (adjustGroup.Position == DockPosition.Right || adjustGroup.Position == DockPosition.Bottom))
            {
                signedDelta = -deltaPx;
            }

            float ratio = _calculator.RatioFromDelta(adjustGroup.SplitRatio, signedDelta, axisSize);
            adjustGroup.SplitRatio = ratio;
            adjustGroup.RatioInitialized = true;
            InvalidateLayout();
        }

        /// <summary>
        /// Invalidates the layout cache, forcing recalculation on next request.
        /// </summary>
        public void InvalidateLayout()
        {
            _isDirty = true;
            _result = null;
        }

        /// <summary>Gets diagnostic information about the current layout.</summary>
        public LayoutDiagnostics GetDiagnostics()
        {
            var result = CalculateLayoutResult();
            return new LayoutDiagnostics
            {
                TotalPanels = _layoutTree.GetAllPanels().Count,
                TotalGroups = _layoutTree.GetAllGroups().Count,
                CalculatedPanels = result.PanelBounds.Count,
                CacheValid = !_isDirty,
                ContainerBounds = _containerBounds,
                PanelBounds = result.PanelBounds.ToDictionary(kv => kv.Key, kv => kv.Value)
            };
        }

        #region Private dock-site layout

        private DockLayoutResult BuildLayout(Rectangle container)
        {
            var panelBounds = new Dictionary<string, Rectangle>();
            var groupBounds = new Dictionary<string, Rectangle>();
            var splitters = new List<DockSplitterHit>();

            if (container.Width <= 0 || container.Height <= 0 || _layoutTree.Root == null)
                return new DockLayoutResult(container, panelBounds, groupBounds, splitters);

            // A maximised panel takes the whole container and nothing else is allocated. The tree is
            // read but never written, so clearing MaximisedPanelKey and laying out again reproduces
            // the previous arrangement exactly, ratios included.
            if (_maximisedPanelKey != null &&
                BuildMaximisedLayout(container, panelBounds, groupBounds, out var maximisedResult))
                return maximisedResult;

            var remaining = container;

            // Resolve the edge groups that actually have visible panels.
            var edgeGroups = _layoutTree.Root.Children
                .Where(g => g.Position != DockPosition.Fill && HasVisiblePanels(g))
                .ToDictionary(g => g.Position, g => g);

            foreach (var position in EdgeOrder)
            {
                if (!edgeGroups.TryGetValue(position, out var group))
                    continue;

                bool horizontalAxis = position == DockPosition.Left || position == DockPosition.Right;
                int axisTotal = horizontalAxis ? remaining.Width : remaining.Height;
                int available = axisTotal - SPLITTER_WIDTH;
                if (available <= 0)
                    continue;

                int minThis = horizontalAxis ? group.MinWidth : group.MinHeight;
                int minOther = horizontalAxis ? MIN_PANEL_WIDTH : MIN_PANEL_HEIGHT;

                int desired = (int)Math.Round(available * group.SplitRatio);
                int size = _calculator.ClampSplit(desired, available, minThis, minOther);

                Rectangle groupRect;
                Rectangle splitterRect;

                switch (position)
                {
                    case DockPosition.Left:
                        groupRect = new Rectangle(remaining.X, remaining.Y, size, remaining.Height);
                        splitterRect = new Rectangle(groupRect.Right, remaining.Y, SPLITTER_WIDTH, remaining.Height);
                        remaining = new Rectangle(splitterRect.Right, remaining.Y,
                            remaining.Width - size - SPLITTER_WIDTH, remaining.Height);
                        break;

                    case DockPosition.Right:
                        groupRect = new Rectangle(remaining.Right - size, remaining.Y, size, remaining.Height);
                        splitterRect = new Rectangle(groupRect.Left - SPLITTER_WIDTH, remaining.Y, SPLITTER_WIDTH, remaining.Height);
                        remaining = new Rectangle(remaining.X, remaining.Y,
                            remaining.Width - size - SPLITTER_WIDTH, remaining.Height);
                        break;

                    case DockPosition.Top:
                        groupRect = new Rectangle(remaining.X, remaining.Y, remaining.Width, size);
                        splitterRect = new Rectangle(remaining.X, groupRect.Bottom, remaining.Width, SPLITTER_WIDTH);
                        remaining = new Rectangle(remaining.X, splitterRect.Bottom,
                            remaining.Width, remaining.Height - size - SPLITTER_WIDTH);
                        break;

                    default: // Bottom
                        groupRect = new Rectangle(remaining.X, remaining.Bottom - size, remaining.Width, size);
                        splitterRect = new Rectangle(remaining.X, groupRect.Top - SPLITTER_WIDTH, remaining.Width, SPLITTER_WIDTH);
                        remaining = new Rectangle(remaining.X, remaining.Y,
                            remaining.Width, remaining.Height - size - SPLITTER_WIDTH);
                        break;
                }

                groupBounds[group.Id] = groupRect;
                splitters.Add(new DockSplitterHit(group.Id, splitterRect, horizontalAxis));
                AssignPanelsRecursive(group, groupRect, panelBounds, groupBounds, splitters);
            }

            // Fill group(s) take whatever space is left.
            foreach (var fill in _layoutTree.Root.Children.Where(g => g.Position == DockPosition.Fill && HasVisiblePanels(g)))
            {
                groupBounds[fill.Id] = remaining;
                AssignPanelsRecursive(fill, remaining, panelBounds, groupBounds, splitters);
            }

            return new DockLayoutResult(container, panelBounds, groupBounds, splitters);
        }

        /// <summary>
        /// Allocates the whole container to the maximised panel and its ancestor groups.
        /// </summary>
        /// <returns>
        /// <c>true</c> when the maximised key resolved to a panel that can hold the container.
        /// <c>false</c> means the key is stale — the panel was closed, floated or auto-hidden while
        /// maximised — and the caller falls through to the normal arrangement rather than producing
        /// an empty layout. Ancestor groups get the container too, because a dockspace hosting
        /// several groups positions each from <see cref="DockLayoutResult.GetGroupBounds"/>.
        /// </returns>
        private bool BuildMaximisedLayout(Rectangle container,
            Dictionary<string, Rectangle> panelBounds,
            Dictionary<string, Rectangle> groupBounds,
            out DockLayoutResult result)
        {
            result = null;

            var panel = _layoutTree.GetPanel(_maximisedPanelKey);
            if (panel == null || panel.State != DockPanelState.Docked)
                return false;

            panelBounds[panel.Key] = container;

            for (var group = panel.Group; group != null; group = group.Parent)
                groupBounds[group.Id] = container;

            // No splitters: with one panel filling the container there is no boundary to drag.
            result = new DockLayoutResult(container, panelBounds, groupBounds,
                                          new List<DockSplitterHit>());
            return true;
        }

        // A group reserves layout space only when it holds at least one docked panel.
        // Floating/auto-hidden panels are removed from their group; hidden (Closed) panels
        // remain registered but must not keep their edge alive or reserve space.
        private static bool HasVisiblePanels(DockGroup group)
            => group != null && group.GetAllPanelsRecursive().Any(p => p != null && p.State == DockPanelState.Docked);

        private void AssignPanelsRecursive(DockGroup group, Rectangle groupRect,
            Dictionary<string, Rectangle> panelBounds,
            Dictionary<string, Rectangle> groupBounds,
            List<DockSplitterHit> splitters)
        {
            groupBounds[group.Id] = groupRect;

            var visibleChildren = group.Children.Where(c => HasVisiblePanels(c)).ToList();

            if (visibleChildren.Count == 0)
            {
                foreach (var panel in group.Panels)
                {
                    if (panel?.Key != null && panel.State == DockPanelState.Docked)
                        panelBounds[panel.Key] = groupRect;
                }
                return;
            }

            // Split group rect among visible child groups.
            bool horizontal = group.SplitOrientation == SplitOrientation.Horizontal;
            int axisStart = horizontal ? groupRect.X : groupRect.Y;
            int axisTotal = horizontal ? groupRect.Width : groupRect.Height;
            int available = axisTotal - (SPLITTER_WIDTH * (visibleChildren.Count - 1));
            if (available <= 0) return;

            // Distribute space proportionally: first child gets SplitRatio of the available space.
            int firstSize = visibleChildren.Count == 1
                ? available
                : (int)Math.Round(available * group.SplitRatio);
            firstSize = Math.Max(MIN_PANEL_WIDTH, Math.Min(available - MIN_PANEL_WIDTH, firstSize));

            int remainingSize = available - firstSize;
            int offset = axisStart;

            for (int i = 0; i < visibleChildren.Count; i++)
            {
                int remainderChildren = visibleChildren.Count - 1;
                int childSize;
                if (i == 0)
                {
                    childSize = firstSize;
                }
                else if (visibleChildren.Count == 2)
                {
                    childSize = remainingSize;
                }
                else
                {
                    childSize = remainingSize / remainderChildren;
                    // Distribute the remainder so the last children absorb the extra pixels.
                    if (i - 1 < remainingSize % remainderChildren)
                        childSize++;
                }

                Rectangle childRect;
                if (horizontal)
                {
                    childRect = new Rectangle(offset, groupRect.Y, childSize, groupRect.Height);
                    offset += childSize;
                }
                else
                {
                    childRect = new Rectangle(groupRect.X, offset, groupRect.Width, childSize);
                    offset += childSize;
                }

                if (i < visibleChildren.Count - 1)
                {
                    Rectangle splitterRect = horizontal
                        ? new Rectangle(offset, groupRect.Y, SPLITTER_WIDTH, groupRect.Height)
                        : new Rectangle(groupRect.X, offset, groupRect.Width, SPLITTER_WIDTH);

                    offset += SPLITTER_WIDTH;
                    splitters.Add(new DockSplitterHit($"{group.Id}_child_{i}", splitterRect, horizontal));
                }

                AssignPanelsRecursive(visibleChildren[i], childRect, panelBounds, groupBounds, splitters);
            }
        }

        #endregion
    }

    /// <summary>Metrics used for layout calculations.</summary>
    public struct LayoutMetrics
    {
        public int TabStripHeight { get; set; }
        public int ChromeHeight { get; set; }
        public int SplitterWidth { get; set; }
        public int MinPanelWidth { get; set; }
        public int MinPanelHeight { get; set; }
    }

    /// <summary>Diagnostic information about current layout state.</summary>
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
