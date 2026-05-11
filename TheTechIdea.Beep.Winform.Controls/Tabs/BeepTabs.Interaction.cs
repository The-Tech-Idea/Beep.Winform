using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private bool _isDragging;

        private void BeepTabs_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data?.GetDataPresent(typeof(int)) == true) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void BeepTabs_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(typeof(int)))
            {
                e.Effect = DragDropEffects.None;
                ResetDragState();
                return;
            }

            int draggedIndex = (int)e.Data.GetData(typeof(int));
            int itemCount = GetHostedSourceItemCount();
            if (draggedIndex < 0 || draggedIndex >= itemCount)
            {
                e.Effect = DragDropEffects.None;
                ResetDragState();
                return;
            }

            e.Effect = DragDropEffects.Move;

            Point clientPoint = PointToClient(new Point(e.X, e.Y));
            SyncHeaderSnapshot();
            _headerHost.ResolveDragFeedback(clientPoint, draggedIndex, itemCount);

            InvalidateHeader();
        }

        private void BeepTabs_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(typeof(int)))
            {
                ResetDragState();
                return;
            }

            int draggedIndex = (int)e.Data.GetData(typeof(int));
            BeepTabHeaderDragFeedback dragFeedback = _headerHost.DragFeedback;
            int itemCount = GetHostedSourceItemCount();
            if (!dragFeedback.WouldReorder || draggedIndex < 0 || draggedIndex >= itemCount)
            {
                ResetDragState();
                return;
            }

            int newIndex = dragFeedback.InsertIndex;
            if (TryMoveHostedSourceItem(draggedIndex, newIndex))
            {
                LastTabSelected = GetHostedSourceSelectedIndex();
            }

            ResetDragState();
        }

        private void BeepTabs_DragLeave(object sender, EventArgs e)
        {
            ResetDragState();
        }

        private void ResetDragState()
        {
            _isDragging = false;
            ResetHeaderPointerState();
            InvalidateHeader();
        }

        private void BeepTabs_MouseClick(object sender, MouseEventArgs e)
        {
            if (_isDragging || GetHostedSourceItemCount() == 0)
            {
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                TryShowHeaderTabContextMenu(e.Location);
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            BeepTabHeaderAction action = ResolveHeaderAction(e.Location);
            if (BeepTabHeaderActionRouter.TryExecute(this, action))
            {
                // Selection change already invalidates the header inside SetSelectedHostedPage.
                // Avoid repainting the content area which is handled by the page visibility change.
                InvalidateHeader();
            }
        }

        private void BeepTabs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || GetHostedSourceItemCount() == 0)
            {
                return;
            }

            _isDragging = false;
            Capture = true;

            BeepTabHeaderAction action = BeginHeaderPointerInteraction(e.Location);
            InvalidateHeader();
            if (action.ActionKind == BeepTabHeaderActionKind.CloseTab)
            {
                return;
            }
        }

        private void BeepTabs_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateHeaderHoverState(e.Location);

            if (_isDragging)
            {
                return;
            }

            if (!TryBeginHeaderDrag(e.Location, out int draggedTabIndex))
            {
                return;
            }

            _isDragging = true;
            DoDragDrop(draggedTabIndex, DragDropEffects.Move);
            ResetHeaderPointerState();
            _isDragging = false;
            Capture = false;
            InvalidateHeader();
        }

        private void BeepTabs_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            Capture = false;
            ResetHeaderPointerState();
        }

        private void BeepTabs_MouseLeave(object sender, EventArgs e)
        {
            if (_headerHost.ClearHoverState())
            {
                InvalidateHeader();
            }

            UpdateHostedContentBounds();
        }

        private void UpdateHeaderHoverState(Point location)
        {
            if (GetHostedSourceItemCount() == 0 || _isDragging)
            {
                return;
            }

            SyncHeaderSnapshot();
            if (_headerHost.UpdateHoverState(location))
            {
                InvalidateHeader();
            }
        }

        private BeepTabHeaderAction ResolveHeaderAction(Point location)
        {
            SyncHeaderSnapshot();
            return _headerHost.ResolvePrimaryAction(location, ShowCloseButtons);
        }

        private BeepTabHeaderAction BeginHeaderPointerInteraction(Point location)
        {
            SyncHeaderSnapshot();
            return _headerHost.BeginPrimaryPointerInteraction(location, ShowCloseButtons);
        }

        private void SyncHeaderSnapshot()
        {
            if (!IsHandleCreated || IsDisposed)
            {
                _headerHost.SyncSnapshot();
                return;
            }

            // Avoid expensive CreateGraphics() + full layout if the snapshot is still valid.
            // The snapshot is rebuilt during every paint cycle, so it is fresh unless
            // the layout has changed programmatically between paints.
            BeepTabHeaderLayoutSnapshot snapshot = _headerHost.LayoutSnapshot;
            int itemCount = GetHostedSourceItemCount();
            if (snapshot != null && snapshot.Items.Count == itemCount && itemCount > 0)
            {
                return;
            }

            using Graphics graphics = CreateGraphics();
            SyncHeaderSurface(graphics);
        }

        private bool TryBeginHeaderDrag(Point location, out int draggedTabIndex)
        {
            SyncHeaderSnapshot();
            if (!_headerHost.TryBeginDragInteraction(location, out draggedTabIndex))
            {
                return false;
            }

            InvalidateHeader();
            return true;
        }

        private void ResetHeaderPointerState()
        {
            if (_headerHost.ResetPointerInteractionState())
            {
                InvalidateHeader();
            }
        }

        internal bool TrySelectHeaderTab(int tabIndex)
        {
            if (!CanSelectHeaderTab(tabIndex))
            {
                return false;
            }

            if (!TrySelectHostedSourceItem(tabIndex))
            {
                return false;
            }

            LastTabSelected = GetHostedSourceSelectedIndex();
            return true;
        }

        internal bool TryCloseHeaderTab(int tabIndex)
        {
            if (!CanCloseHeaderTab(tabIndex))
            {
                return false;
            }

            // Dirty-close guard (Documents / Workspace mode only).
            // Fire a cancellable event so the host can show a "save?" prompt.
            if (TabMode != BeepTabMode.Navigation)
            {
                BeepTabPage? candidate = GetHostedSourcePageAt(tabIndex);
                if (candidate != null)
                {
                    BeepTabItem meta = GetOrCreateHostedTabMetadata(candidate);
                    if (meta.IsDirty)
                    {
                        var args = new BeepTabCloseRequestedEventArgs(candidate, meta);
                        TabCloseRequested?.Invoke(this, args);
                        if (args.Cancel)
                        {
                            return false;
                        }
                    }
                }
            }

            if (!TryRemoveHostedSourceItem(tabIndex, out string? tabText))
            {
                return false;
            }

            TabRemoved?.Invoke(this, new TabRemovedEventArgs { TabText = tabText });
            return true;
        }

        // ── Preview-tab double-click: promote preview → permanent ─────────────
        private void BeepTabs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (TabMode == BeepTabMode.Navigation)
            {
                return;
            }

            SyncHeaderSnapshot();

            if (!_headerHost.TryHitTab(e.Location, out int tabIndex))
            {
                return;
            }

            BeepTabPage? page = GetHostedSourcePageAt(tabIndex);
            if (page == null)
            {
                return;
            }

            BeepTabItem meta = GetOrCreateHostedTabMetadata(page);
            if (!meta.WorkspaceState.IsPreview)
            {
                return;
            }

            // Promote: mark as permanent, repaint header.
            meta.WorkspaceState.IsPreview = false;
            Invalidate();
        }

        public void ReceiveMouseClick(Point clientLocation)
        {
            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }

        public void ReceiveMouseMove(Point clientLocation)
        {
            OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }

        public void ReceiveMouseUp(Point clientLocation)
        {
            OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }

        public void ReceiveMouseDown(Point clientLocation)
        {
            OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, clientLocation.X, clientLocation.Y, 0));
        }
    }
}