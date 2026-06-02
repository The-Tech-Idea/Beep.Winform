using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime.DragDrop;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Drag-and-drop docking for <see cref="BeepDockingManager"/>. Caption/tab mouse events (raised
    /// by <c>DockPanel</c> via its layout managers) are funneled into a single
    /// <see cref="DockDragController"/>; commits route back through <see cref="IDockDragHost"/> and
    /// the existing cancelable <see cref="PageDockedRequest"/> / <see cref="PageFloatingRequest"/>
    /// events. No <c>IMessageFilter</c> — all input comes from real control mouse events.
    /// </summary>
    public partial class BeepDockingManager
    {
        private DockDragController _dragController;

        private DockDragController DragController
        {
            get
            {
                if (_dragController == null)
                {
                    _dragController = new DockDragController(this);
                    _dragController.ShowSnapGuides = _showSnapGuides;
                }
                return _dragController;
            }
        }

        // ── Entry points called by DockPanel caption/tab mouse handlers ──────────────

        /// <summary>Records a caption/tab mouse-down candidate (screen coords). No-op at design time
        /// or when the manager has <see cref="BeepDockingManager.AllowEndUserDocking"/> disabled.</summary>
        internal void BeginCaptionDrag(DockPanel panel, Point screenPoint)
        {
            if (IsDesignHosted || !_allowEndUserDocking || panel == null)
                return;
            DragController.BeginCandidate(panel, screenPoint);
        }

        /// <summary>Feeds a caption/tab mouse-move (screen coords) into the active drag.</summary>
        internal void UpdateCaptionDrag(Point screenPoint)
        {
            if (IsDesignHosted)
                return;
            _dragController?.Update(screenPoint);
        }

        /// <summary>Ends a caption/tab drag (screen coords); commits the resolved target unless cancelled.</summary>
        internal void EndCaptionDrag(Point screenPoint, bool commit)
        {
            if (IsDesignHosted)
                return;

            bool wasDragging = _dragController != null && _dragController.IsDragging;
            _dragController?.End(screenPoint, commit);

            if (wasDragging)
            {
                if (commit)
                    OnDoDragDropEnd();
                else
                    OnDoDragDropQuit();
            }
        }

        /// <summary>Cancels any in-flight caption/tab drag and restores the original layout.</summary>
        internal void CancelCaptionDrag()
        {
            if (_dragController != null && _dragController.IsDragging)
                OnDoDragDropQuit();

            _dragController?.Cancel();
        }

        /// <summary>True while a panel drag has started (used to suppress the trailing click).</summary>
        internal bool IsPanelDragging => _dragController != null && _dragController.IsDragging;

        // ── Commit helpers (raise cancelable events, then apply) ─────────────────────

        private bool CommitDragFloat(DockPanel panel)
        {
            if (panel == null || !panel.CanFloat)
                return false;

            var args = new CancelPanelRequestEventArgs(panel.Key, panel);
            OnPageFloatingRequest(args);
            if (args.Cancel)
                return false;

            FloatPanel(panel.Key);
            return true;
        }

        private bool CommitDragDockSiteEdge(DockPanel panel, DockPosition position)
        {
            if (panel == null)
                return false;

            if (!IsPositionAllowed(panel, position))
                return false;

            if (position == DockPosition.Fill || position == DockPosition.Floating)
                return CommitDragCenterStack(panel, null, -1);

            var args = new CancelPanelRequestEventArgs(panel.Key, panel);
            OnPageDockedRequest(args);
            if (args.Cancel)
                return false;

            panel.Group?.RemovePanel(panel);
            PreparePanelForDock(panel);
            panel.DockPosition = position;
            panel.State = DockPanelState.Docked;

            var group = GetOrCreateGroupAtPosition(position);
            group.AddPanel(panel);

            ApplyThemeToPanel(panel);
            _layoutController?.InvalidateLayout();
            ActivatePanel(panel.Key);
            RecalculateLayout();   // reposition the dropped panel + reconcile splitters
            return true;
        }

        private bool CommitDragCenterStack(DockPanel panel, DockGroup targetGroup, int insertIndex)
        {
            if (panel == null)
                return false;

            if (!IsPositionAllowed(panel, DockPosition.Fill))
                return false;

            var args = new CancelPanelRequestEventArgs(panel.Key, panel);
            OnPageDockedRequest(args);
            if (args.Cancel)
                return false;

            panel.Group?.RemovePanel(panel);
            PreparePanelForDock(panel);
            panel.DockPosition = DockPosition.Fill;
            panel.State = DockPanelState.Docked;

            var group = targetGroup ?? GetOrCreateGroupAtPosition(DockPosition.Fill);
            group.AddPanel(panel);
            if (insertIndex >= 0)
                group.MovePanelToIndex(panel, insertIndex);

            ApplyThemeToPanel(panel);
            _layoutController?.InvalidateLayout();
            ActivatePanel(panel.Key);
            RecalculateLayout();   // reposition the stacked panel + reconcile splitters
            return true;
        }

        /// <summary>
        /// Checks whether the given <see cref="DockPosition"/> is permitted by the panel's
        /// <see cref="DockPanel.AllowedAreas"/>.
        /// </summary>
        internal static bool IsPositionAllowed(DockPanel panel, DockPosition position)
        {
            if (panel == null)
                return false;

            return (DockTargetResolver.AreaForPosition(position) & panel.AllowedAreas) != 0;
        }

        // ── IDockDragHost (explicit so the public surface stays clean) ───────────────

        Form IDockDragHost.HostForm => _hostForm;

        DockingThemeColors IDockDragHost.DockingColors => _themeColors;

        System.Collections.Generic.IReadOnlyDictionary<string, System.Drawing.Rectangle> IDockDragHost.GroupBounds
        {
            get
            {
                var result = _layoutController?.CalculateLayoutResult();
                return result?.GroupBounds ?? new System.Collections.Generic.Dictionary<string, System.Drawing.Rectangle>();
            }
        }

        DockGroup IDockDragHost.GetGroup(string groupId) => _layoutTree?.GetGroup(groupId);

        bool IDockDragHost.CommitFloat(DockPanel panel) => CommitDragFloat(panel);

        bool IDockDragHost.CommitDockSiteEdge(DockPanel panel, DockPosition position) =>
            CommitDragDockSiteEdge(panel, position);

        bool IDockDragHost.CommitCenterStack(DockPanel panel, DockGroup targetGroup, int insertIndex) =>
            CommitDragCenterStack(panel, targetGroup, insertIndex);

        bool IDockDragHost.CommitGroupEdge(DockPanel panel, DockGroup targetGroup, DockPosition position)
        {
            if (panel == null || targetGroup == null)
                return false;

            if (!IsPositionAllowed(panel, position))
                return false;

            var args = new CancelPanelRequestEventArgs(panel.Key, panel);
            OnPageDockedRequest(args);
            if (args.Cancel)
                return false;

            panel.Group?.RemovePanel(panel);
            PreparePanelForDock(panel);
            panel.DockPosition = targetGroup.Position;
            panel.State = DockPanelState.Docked;

            // If targetGroup already has children, add the new panel as another child.
            // If it's a leaf group, split it: wrap its existing panels + new panel as children.
            // Handle the edge case of 1 child (shouldn't normally exist) the same as 2+.
            if (targetGroup.Children.Count > 0)
            {
                // Move any lingering direct panels into a child group first.
                var lingering = targetGroup.Panels.ToArray();
                if (lingering.Length > 0)
                {
                    var leftoverChild = new DockGroup
                    {
                        Id = $"child_{Guid.NewGuid():N}",
                        Position = targetGroup.Position,
                        TabStyle = targetGroup.TabStyle
                    };
                    foreach (var ep in lingering)
                    {
                        targetGroup.RemovePanel(ep);
                        leftoverChild.AddPanel(ep);
                    }
                    leftoverChild.ActivePanel = leftoverChild.Panels.FirstOrDefault();
                    targetGroup.AddChild(leftoverChild);
                    _layoutTree.RegisterGroup(leftoverChild);
                }

                var childGroup = new DockGroup
                {
                    Id = $"child_{Guid.NewGuid():N}",
                    Position = targetGroup.Position,
                    TabStyle = targetGroup.TabStyle
                };
                childGroup.AddPanel(panel);
                targetGroup.AddChild(childGroup);
                _layoutTree.RegisterGroup(childGroup);
            }
            else
            {
                // Promote existing panels to child groups and split with new panel.
                bool horizontal = targetGroup.Position == DockPosition.Left || targetGroup.Position == DockPosition.Right;
                targetGroup.SplitOrientation = horizontal ? SplitOrientation.Horizontal : SplitOrientation.Vertical;

                if (targetGroup.Panels.Count > 0)
                {
                    var existingChild = new DockGroup
                    {
                        Id = $"child_{Guid.NewGuid():N}",
                        Position = targetGroup.Position,
                        TabStyle = targetGroup.TabStyle
                    };
                    var directPanels = targetGroup.Panels.ToArray();
                    foreach (var ep in directPanels)
                    {
                        targetGroup.RemovePanel(ep);
                        existingChild.AddPanel(ep);
                    }
                    existingChild.ActivePanel = existingChild.Panels.FirstOrDefault();
                    targetGroup.AddChild(existingChild);
                    _layoutTree.RegisterGroup(existingChild);
                }

                var newChild = new DockGroup
                {
                    Id = $"child_{Guid.NewGuid():N}",
                    Position = targetGroup.Position,
                    TabStyle = targetGroup.TabStyle,
                    ActivePanel = panel
                };
                newChild.AddPanel(panel);
                targetGroup.AddChild(newChild);
                _layoutTree.RegisterGroup(newChild);
            }

            ApplyThemeToPanel(panel);
            _layoutController?.InvalidateLayout();
            ActivatePanel(panel.Key);
            RecalculateLayout();
            return true;
        }
    }
}
