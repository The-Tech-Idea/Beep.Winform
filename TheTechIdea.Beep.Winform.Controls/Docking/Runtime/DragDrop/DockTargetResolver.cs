using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop
{
    /// <summary>
    /// Maps the current cursor position to a <see cref="DockDropResult"/> using the active guide
    /// diamond (via <see cref="DockGuideController"/>) plus the dock-site geometry. Center-guide
    /// hits resolve to a tab stack; edge guides resolve to a site-edge dock; no hit resolves to a
    /// float preview that follows the cursor. When group bounds are supplied with a lookup, edge-
    /// guide hits close to an existing group resolve to a group-edge split instead.
    /// </summary>
    internal sealed class DockTargetResolver
    {
        private const float EdgeFraction = 0.33f;
        private const int GroupEdgeTolerance = 18;

        private readonly Form _hostForm;
        private readonly DockGuideController _guides;

        public DockTargetResolver(Form hostForm, DockGuideController guides)
        {
            _hostForm = hostForm;
            _guides = guides;
        }

        /// <summary>
        /// Maps a <see cref="DockPosition"/> to its corresponding <see cref="DockAreas"/> flag.
        /// </summary>
        internal static DockAreas AreaForPosition(DockPosition pos) => pos switch
        {
            DockPosition.Left => DockAreas.Left,
            DockPosition.Right => DockAreas.Right,
            DockPosition.Top => DockAreas.Top,
            DockPosition.Bottom => DockAreas.Bottom,
            DockPosition.Fill => DockAreas.Fill,
            _ => DockAreas.None
        };

        /// <summary>
        /// Resolves a drop result for the dragged <paramref name="panel"/> at <paramref name="screenPoint"/>.
        /// Filters guide targets against <see cref="DockPanel.AllowedAreas"/>.
        /// When <paramref name="groupBounds"/> and <paramref name="groupLookup"/> are supplied,
        /// proximity to group edges is tested and may upgrade a site-edge dock to a
        /// <see cref="DockDropKind.GroupEdge"/> split.
        /// </summary>
        public DockDropResult Resolve(DockPanel panel, Point screenPoint,
            IReadOnlyDictionary<string, Rectangle> groupBounds = null,
            Func<string, DockGroup> groupLookup = null)
        {
            DockPosition? rawTarget = _guides?.Update(screenPoint);

            DockPosition? target = rawTarget;
            if (target.HasValue && (AreaForPosition(target.Value) & panel.AllowedAreas) == 0)
                target = null;

            if (_hostForm == null || target == null)
                return DockDropResult.Float(FloatPreview(panel, screenPoint));

            Rectangle client = _hostForm.DisplayRectangle;

            if (target == DockPosition.Fill)
            {
                return new DockDropResult
                {
                    Kind = DockDropKind.GroupCenterStack,
                    Position = DockPosition.Fill,
                    InsertIndex = -1,
                    PreviewBounds = ToScreen(CenterPreview(client))
                };
            }

            if (groupBounds != null && groupBounds.Count > 0 && groupLookup != null)
            {
                var hit = HitTestGroupEdge(client, screenPoint, target.Value, groupBounds, groupLookup);
                if (hit != null)
                    return hit;
            }

            return new DockDropResult
            {
                Kind = DockDropKind.DockSiteEdge,
                Position = target.Value,
                PreviewBounds = ToScreen(EdgePreview(client, target.Value))
            };
        }

        private DockDropResult HitTestGroupEdge(
            Rectangle client, Point screenPoint, DockPosition position,
            IReadOnlyDictionary<string, Rectangle> groupBounds,
            Func<string, DockGroup> groupLookup)
        {
            var clientPt = _hostForm.PointToClient(screenPoint);
            string bestGroupId = null;
            Rectangle bestBounds = Rectangle.Empty;
            double bestDistance = double.MaxValue;

            foreach (var kv in groupBounds)
            {
                if (kv.Value.IsEmpty)
                    continue;

                Rectangle hotZone;
                switch (position)
                {
                    case DockPosition.Left:
                        hotZone = new Rectangle(kv.Value.Right - GroupEdgeTolerance, kv.Value.Top,
                            GroupEdgeTolerance * 2, kv.Value.Height);
                        break;
                    case DockPosition.Right:
                        hotZone = new Rectangle(kv.Value.Left - GroupEdgeTolerance, kv.Value.Top,
                            GroupEdgeTolerance * 2, kv.Value.Height);
                        break;
                    case DockPosition.Top:
                        hotZone = new Rectangle(kv.Value.Left, kv.Value.Bottom - GroupEdgeTolerance,
                            kv.Value.Width, GroupEdgeTolerance * 2);
                        break;
                    case DockPosition.Bottom:
                        hotZone = new Rectangle(kv.Value.Left, kv.Value.Top - GroupEdgeTolerance,
                            kv.Value.Width, GroupEdgeTolerance * 2);
                        break;
                    default:
                        continue;
                }

                if (hotZone.Contains(clientPt))
                {
                    double dist = DistanceToRectCenter(hotZone, clientPt);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestGroupId = kv.Key;
                        bestBounds = kv.Value;
                    }
                }
            }

            if (bestGroupId == null)
                return null;

            var group = groupLookup(bestGroupId);
            return new DockDropResult
            {
                Kind = DockDropKind.GroupEdge,
                Position = position,
                TargetGroup = group,
                PreviewBounds = ToScreen(EdgePreview(client, position))
            };
        }

        private static double DistanceToRectCenter(Rectangle rect, Point p)
        {
            double cx = rect.X + rect.Width / 2.0;
            double cy = rect.Y + rect.Height / 2.0;
            double dx = p.X - cx;
            double dy = p.Y - cy;
            return dx * dx + dy * dy;
        }

        private Rectangle ToScreen(Rectangle clientRect) =>
            _hostForm == null ? clientRect : _hostForm.RectangleToScreen(clientRect);

        private static Rectangle EdgePreview(Rectangle client, DockPosition position)
        {
            int w = (int)(client.Width * EdgeFraction);
            int h = (int)(client.Height * EdgeFraction);

            return position switch
            {
                DockPosition.Left => new Rectangle(client.Left, client.Top, w, client.Height),
                DockPosition.Right => new Rectangle(client.Right - w, client.Top, w, client.Height),
                DockPosition.Top => new Rectangle(client.Left, client.Top, client.Width, h),
                DockPosition.Bottom => new Rectangle(client.Left, client.Bottom - h, client.Width, h),
                _ => client
            };
        }

        private static Rectangle CenterPreview(Rectangle client)
        {
            int dx = client.Width / 4;
            int dy = client.Height / 4;
            return Rectangle.Inflate(client, -dx, -dy);
        }

        private static Rectangle FloatPreview(DockPanel panel, Point screenPoint)
        {
            int w = panel != null && panel.PreferredWidth > 0 ? panel.PreferredWidth : 280;
            int h = panel != null && panel.PreferredHeight > 0 ? panel.PreferredHeight : 180;
            return new Rectangle(screenPoint.X + 12, screenPoint.Y + 12, w, h);
        }
    }
}
