using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        public BeepTabHeaderDragFeedback ResolveDragFeedback(System.Drawing.Point location, int draggedIndex, int tabCount)
        {
            if (LayoutSnapshot == null || LayoutSnapshot.Items.Count == 0 || draggedIndex < 0 || draggedIndex >= tabCount)
            {
                DragFeedback = BeepTabHeaderDragFeedback.Empty;
                return DragFeedback;
            }

            BeepTabHeaderDragFeedback feedback = LayoutSnapshot.HeaderPosition switch
            {
                TabHeaderPosition.Top or TabHeaderPosition.Bottom => ResolveHorizontalDragFeedback(location, draggedIndex, tabCount),
                _ => ResolveVerticalDragFeedback(location, draggedIndex, tabCount)
            };

            DragFeedback = feedback;
            return DragFeedback;
        }

        public bool ClearDragFeedback()
        {
            if (!DragFeedback.HasDropTarget && !DragFeedback.HasMarker)
            {
                return false;
            }

            DragFeedback = BeepTabHeaderDragFeedback.Empty;
            return true;
        }

        public bool TryBeginDragInteraction(System.Drawing.Point currentLocation, out int tabIndex)
        {
            tabIndex = -1;
            if (!_hasActivePointerInteraction || _pressedTabIndex < 0)
            {
                return false;
            }

            if (!TryGetItemLayout(_pressedTabIndex, out BeepTabHeaderItemLayout? itemLayout) || !itemLayout.Item.CanReorder)
            {
                return false;
            }

            Size dragSize = SystemInformation.DragSize;
            Rectangle dragThreshold = new Rectangle(
                _pointerDownLocation.X - dragSize.Width / 2,
                _pointerDownLocation.Y - dragSize.Height / 2,
                dragSize.Width,
                dragSize.Height);

            if (dragThreshold.Contains(currentLocation))
            {
                return false;
            }

            _draggingTabIndex = _pressedTabIndex;
            ApplyItemState();
            tabIndex = _draggingTabIndex;
            return true;
        }

        public bool ResetPointerInteractionState()
        {
            if (!_hasActivePointerInteraction && _pressedTabIndex < 0 && _draggingTabIndex < 0 && string.IsNullOrEmpty(_pressedActionCommandName))
            {
                return ClearDragFeedback();
            }

            _hasActivePointerInteraction = false;
            _pointerDownLocation = System.Drawing.Point.Empty;
            _pressedTabIndex = -1;
            _pressedCloseTabIndex = -1;
            _pressedActionCommandName = string.Empty;
            _draggingTabIndex = -1;
            DragFeedback = BeepTabHeaderDragFeedback.Empty;
            ApplyItemState();
            return true;
        }

        public BeepTabHeaderAction BeginPrimaryPointerInteraction(System.Drawing.Point location, bool showCloseButtons)
        {
            BeepTabHeaderAction action = ResolvePrimaryAction(location, showCloseButtons);
            string pressedActionCommandName = action.IsActionSlot ? action.CommandName : string.Empty;
            int pressedTabIndex = action.ActionKind == BeepTabHeaderActionKind.SelectTab ? action.TabIndex : -1;
            int pressedCloseTabIndex = action.ActionKind == BeepTabHeaderActionKind.CloseTab ? action.TabIndex : -1;

            _hasActivePointerInteraction = pressedTabIndex >= 0 || pressedCloseTabIndex >= 0;
            _pointerDownLocation = location;
            _pressedTabIndex = pressedTabIndex;
            _pressedCloseTabIndex = pressedCloseTabIndex;
            _pressedActionCommandName = pressedActionCommandName;
            _draggingTabIndex = -1;
            ApplyItemState();
            return action;
        }

        public BeepTabHeaderAction ResolvePrimaryAction(System.Drawing.Point location, bool showCloseButtons)
        {
            if (TryHitActionSlot(location, out BeepTabHeaderAction? actionSlot) && actionSlot != null)
            {
                return actionSlot;
            }

            if (showCloseButtons && TryHitCloseButton(location, out int closeTabIndex))
            {
                return BeepTabHeaderAction.CloseTab(closeTabIndex, location);
            }

            if (TryHitTab(location, out int selectedTabIndex))
            {
                return BeepTabHeaderAction.SelectTab(selectedTabIndex, location);
            }

            return BeepTabHeaderAction.None(location);
        }

        public bool TryHitTab(System.Drawing.Point location, out int tabIndex)
        {
            return BeepTabHitTestHelper.TryHitTab(LayoutSnapshot, location, out tabIndex);
        }

        public bool TryHitCloseButton(System.Drawing.Point location, out int tabIndex)
        {
            return BeepTabHitTestHelper.TryHitCloseButton(LayoutSnapshot, location, out tabIndex);
        }

        public bool UpdateHoverState(System.Drawing.Point location)
        {
            string hoveredActionCommandName = string.Empty;
            if (TryHitActionSlot(location, out BeepTabHeaderAction? actionSlot) && actionSlot != null)
            {
                hoveredActionCommandName = actionSlot.CommandName;
            }

            int hoveredCloseIndex = -1;
            TryHitCloseButton(location, out hoveredCloseIndex);

            int hoveredIndex = -1;
            BeepTabHitTestHelper.TryHitTab(LayoutSnapshot, location, out hoveredIndex);
            if (hoveredIndex == _hoveredTabIndex && hoveredCloseIndex == _hoveredCloseTabIndex && hoveredActionCommandName == _hoveredActionCommandName)
            {
                return false;
            }

            _hoveredTabIndex = hoveredIndex;
            _hoveredCloseTabIndex = hoveredCloseIndex;
            _hoveredActionCommandName = hoveredActionCommandName;
            ApplyItemState();
            return true;
        }

        public bool ClearHoverState()
        {
            if (_hoveredTabIndex < 0 && _hoveredCloseTabIndex < 0 && string.IsNullOrEmpty(_hoveredActionCommandName))
            {
                return false;
            }

            _hoveredTabIndex = -1;
            _hoveredCloseTabIndex = -1;
            _hoveredActionCommandName = string.Empty;
            ApplyItemState();
            return true;
        }

        private bool TryGetItemLayout(int tabIndex, out BeepTabHeaderItemLayout? itemLayout)
        {
            itemLayout = null;
            if (LayoutSnapshot == null)
            {
                return false;
            }

            foreach (BeepTabHeaderItemLayout candidate in LayoutSnapshot.Items)
            {
                if (candidate.Item.Index != tabIndex)
                {
                    continue;
                }

                itemLayout = candidate;
                return true;
            }

            return false;
        }

        private BeepTabHeaderDragFeedback ResolveHorizontalDragFeedback(System.Drawing.Point location, int draggedIndex, int tabCount)
        {
            int targetIndex = -1;
            float markerX = -1f;

            foreach (BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
            {
                Rectangle tabRect = itemLayout.Bounds;
                if (!tabRect.Contains(location))
                {
                    continue;
                }

                targetIndex = itemLayout.Item.Index;
                if (itemLayout.Item.Index != draggedIndex)
                {
                    float midpoint = tabRect.Left + tabRect.Width / 2f;
                    markerX = location.X < midpoint ? tabRect.Left : tabRect.Right;
                }

                break;
            }

            if (targetIndex == -1)
            {
                Rectangle lastBounds = LayoutSnapshot.Items[LayoutSnapshot.Items.Count - 1].Bounds;
                if (location.X >= lastBounds.Right)
                {
                    targetIndex = tabCount;
                    markerX = lastBounds.Right;
                }
            }

            return CreateHorizontalFeedback(targetIndex, markerX, draggedIndex);
        }

        private BeepTabHeaderDragFeedback ResolveVerticalDragFeedback(System.Drawing.Point location, int draggedIndex, int tabCount)
        {
            int targetIndex = -1;
            float markerY = -1f;

            foreach (BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
            {
                Rectangle tabRect = itemLayout.Bounds;
                if (!tabRect.Contains(location))
                {
                    continue;
                }

                targetIndex = itemLayout.Item.Index;
                if (itemLayout.Item.Index != draggedIndex)
                {
                    float midpoint = tabRect.Top + tabRect.Height / 2f;
                    markerY = location.Y < midpoint ? tabRect.Top : tabRect.Bottom;
                }

                break;
            }

            if (targetIndex == -1)
            {
                Rectangle lastBounds = LayoutSnapshot.Items[LayoutSnapshot.Items.Count - 1].Bounds;
                if (location.Y >= lastBounds.Bottom)
                {
                    targetIndex = tabCount;
                    markerY = lastBounds.Bottom;
                }
            }

            return CreateVerticalFeedback(targetIndex, markerY, draggedIndex);
        }

        private BeepTabHeaderDragFeedback CreateHorizontalFeedback(int targetIndex, float markerX, int draggedIndex)
        {
            if (targetIndex < 0)
            {
                return BeepTabHeaderDragFeedback.Empty;
            }

            int insertIndex = GetInsertIndex(targetIndex, draggedIndex);
            bool wouldReorder = targetIndex != draggedIndex && insertIndex >= 0;

            if (markerX < 0f)
            {
                return new BeepTabHeaderDragFeedback
                {
                    TargetIndex = targetIndex,
                    InsertIndex = insertIndex,
                    WouldReorder = wouldReorder
                };
            }

            return new BeepTabHeaderDragFeedback
            {
                TargetIndex = targetIndex,
                InsertIndex = insertIndex,
                WouldReorder = wouldReorder,
                MarkerStart = new System.Drawing.PointF(markerX, LayoutSnapshot.HeaderBounds.Top),
                MarkerEnd = new System.Drawing.PointF(markerX, LayoutSnapshot.HeaderBounds.Bottom)
            };
        }

        private BeepTabHeaderDragFeedback CreateVerticalFeedback(int targetIndex, float markerY, int draggedIndex)
        {
            if (targetIndex < 0)
            {
                return BeepTabHeaderDragFeedback.Empty;
            }

            int insertIndex = GetInsertIndex(targetIndex, draggedIndex);
            bool wouldReorder = targetIndex != draggedIndex && insertIndex >= 0;

            if (markerY < 0f)
            {
                return new BeepTabHeaderDragFeedback
                {
                    TargetIndex = targetIndex,
                    InsertIndex = insertIndex,
                    WouldReorder = wouldReorder
                };
            }

            return new BeepTabHeaderDragFeedback
            {
                TargetIndex = targetIndex,
                InsertIndex = insertIndex,
                WouldReorder = wouldReorder,
                MarkerStart = new System.Drawing.PointF(LayoutSnapshot.HeaderBounds.Left, markerY),
                MarkerEnd = new System.Drawing.PointF(LayoutSnapshot.HeaderBounds.Right, markerY)
            };
        }

        private static int GetInsertIndex(int targetIndex, int draggedIndex)
        {
            if (targetIndex < 0 || draggedIndex < 0)
            {
                return -1;
            }

            return targetIndex > draggedIndex ? targetIndex - 1 : targetIndex;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (UpdateHoverState(e.Location))
            {
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (ClearHoverState())
            {
                Invalidate();
            }
        }
    }
}