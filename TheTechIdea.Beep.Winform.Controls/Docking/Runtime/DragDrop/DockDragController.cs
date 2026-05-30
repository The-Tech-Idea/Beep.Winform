using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop
{
    /// <summary>
    /// Owns a single caption/tab drag session: detects the drag threshold, shows the themed guides
    /// and translucent ghost, resolves the live drop target on each move, and commits (float / dock
    /// to edge / stack) or cancels on release. The dragged panel is <b>not</b> reparented until
    /// commit, so cancel is a no-op restore. All commits route through <see cref="IDockDragHost"/>.
    /// </summary>
    internal sealed class DockDragController : IDisposable
    {
        private readonly IDockDragHost _host;
        private readonly DockGuideController _guides = new DockGuideController();
        private readonly DockDragGhost _ghost = new DockDragGhost();

        private DockTargetResolver _resolver;
        private DockDragSession _session;

        public DockDragController(IDockDragHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>True while a drag has crossed the threshold and is actively tracking.</summary>
        public bool IsDragging => _session != null && _session.Started;

        /// <summary>True while a candidate (mouse-down) is recorded but the threshold isn't met yet.</summary>
        public bool HasCandidate => _session != null;

        /// <summary>Records a mouse-down candidate on <paramref name="panel"/>'s caption/tab.</summary>
        public void BeginCandidate(DockPanel panel, Point screenPoint)
        {
            if (panel == null)
                return;
            _session = new DockDragSession(panel, screenPoint);
        }

        /// <summary>Feeds a move; starts the drag once past the threshold, then tracks the target.</summary>
        public void Update(Point screenPoint)
        {
            if (_session == null)
                return;

            if (!_session.Started)
            {
                if (!ExceedsThreshold(_session.OriginScreen, screenPoint))
                    return;
                StartDrag();
            }

            var result = _resolver.Resolve(_session.Panel, screenPoint,
                _host.GroupBounds, _host.GetGroup);
            _session.Current = result;
            _ghost.MoveTo(result.PreviewBounds);
        }

        /// <summary>Ends the drag: commits the resolved target (or cancels).</summary>
        public void End(Point screenPoint, bool commit)
        {
            if (_session == null)
                return;

            bool wasDragging = _session.Started;
            DockDragSession session = _session;
            _session = null;

            if (wasDragging)
            {
                _guides.Hide();
                _ghost.End();

                if (commit)
                    Commit(session);
            }
        }

        /// <summary>Cancels any in-flight drag and tears down the visuals.</summary>
        public void Cancel()
        {
            if (_session == null)
                return;

            _session = null;
            _guides.Hide();
            _ghost.End();
        }

        private void StartDrag()
        {
            _session.Started = true;
            _resolver ??= new DockTargetResolver(_host.HostForm, _guides);

            _guides.Show(_host.HostForm);

            var initial = DockDropResult.Float(
                new Rectangle(_session.OriginScreen.X + 12, _session.OriginScreen.Y + 12, 280, 180));
            _session.Current = initial;
            _ghost.Begin(_session.Panel, _host.DockingColors, _host.HostForm, initial.PreviewBounds);
        }

        private void Commit(DockDragSession session)
        {
            var result = session.Current ?? DockDropResult.Float(Rectangle.Empty);
            DockPanel panel = session.Panel;

            switch (result.Kind)
            {
                case DockDropKind.DockSiteEdge:
                    _host.CommitDockSiteEdge(panel, result.Position);
                    break;

                case DockDropKind.GroupEdge:
                    if (result.TargetGroup != null)
                        _host.CommitGroupEdge(panel, result.TargetGroup, result.Position);
                    else
                        _host.CommitDockSiteEdge(panel, result.Position);
                    break;

                case DockDropKind.GroupCenterStack:
                    _host.CommitCenterStack(panel, result.TargetGroup, result.InsertIndex);
                    break;

                default:
                    _host.CommitFloat(panel);
                    break;
            }
        }

        private static bool ExceedsThreshold(Point origin, Point current)
        {
            Size drag = SystemInformation.DragSize;
            return Math.Abs(current.X - origin.X) >= drag.Width ||
                   Math.Abs(current.Y - origin.Y) >= drag.Height;
        }

        public void Dispose()
        {
            _guides.Dispose();
            _ghost.Dispose();
            _session = null;
        }
    }
}
