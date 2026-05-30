using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Describes where a splitter sits between an edge group and the remaining client area.
    /// Returned by <see cref="DockingLayoutController.FindSplitterAtPoint"/>.
    /// </summary>
    public readonly struct DockSplitterHit
    {
        public DockSplitterHit(string groupId, Rectangle bounds, bool isVertical)
        {
            GroupId = groupId;
            Bounds = bounds;
            IsVertical = isVertical;
        }

        /// <summary>Id of the edge group whose size this splitter controls.</summary>
        public string GroupId { get; }

        /// <summary>Splitter rectangle in container coordinates.</summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// True when the splitter resizes along the X axis (a Left/Right edge group),
        /// i.e. the splitter is a vertical bar the user drags horizontally.
        /// </summary>
        public bool IsVertical { get; }
    }

    /// <summary>
    /// Immutable snapshot of a single layout pass produced by
    /// <see cref="DockingLayoutController"/>. Maps panel/group ids to bounds and
    /// records the splitter rectangles between edge groups and the fill area.
    /// </summary>
    public sealed class DockLayoutResult
    {
        private readonly Dictionary<string, Rectangle> _panelBounds;
        private readonly Dictionary<string, Rectangle> _groupBounds;
        private readonly List<DockSplitterHit> _splitters;

        public DockLayoutResult(
            Rectangle containerBounds,
            Dictionary<string, Rectangle> panelBounds,
            Dictionary<string, Rectangle> groupBounds,
            List<DockSplitterHit> splitters)
        {
            ContainerBounds = containerBounds;
            _panelBounds = panelBounds ?? new Dictionary<string, Rectangle>();
            _groupBounds = groupBounds ?? new Dictionary<string, Rectangle>();
            _splitters = splitters ?? new List<DockSplitterHit>();
        }

        public Rectangle ContainerBounds { get; }

        public IReadOnlyDictionary<string, Rectangle> PanelBounds => _panelBounds;
        public IReadOnlyDictionary<string, Rectangle> GroupBounds => _groupBounds;
        public IReadOnlyList<DockSplitterHit> Splitters => _splitters;

        public Rectangle? GetPanelBounds(string panelKey)
            => panelKey != null && _panelBounds.TryGetValue(panelKey, out var r) ? r : (Rectangle?)null;

        public Rectangle GetGroupBounds(string groupId)
            => groupId != null && _groupBounds.TryGetValue(groupId, out var r) ? r : Rectangle.Empty;

        public DockSplitterHit? FindSplitterAt(Point p, int grabTolerance = 4)
        {
            foreach (var hit in _splitters)
            {
                var zone = hit.Bounds;
                zone.Inflate(grabTolerance, grabTolerance);
                if (zone.Contains(p))
                    return hit;
            }
            return null;
        }

        public static DockLayoutResult Empty(Rectangle container)
            => new DockLayoutResult(container,
                new Dictionary<string, Rectangle>(),
                new Dictionary<string, Rectangle>(),
                new List<DockSplitterHit>());
    }
}
