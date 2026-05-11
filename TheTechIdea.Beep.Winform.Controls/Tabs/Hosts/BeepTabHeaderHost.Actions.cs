using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        private readonly List<BeepTabHeaderAction> _headerActions = new();
        private string _hoveredActionCommandName = string.Empty;
        private string _pressedActionCommandName = string.Empty;

        public IReadOnlyList<BeepTabHeaderAction> HeaderActions => _headerActions;

        public void SetActionSlots(IEnumerable<BeepTabHeaderAction>? actions)
        {
            _headerActions.Clear();

            if (actions != null)
            {
                foreach (BeepTabHeaderAction action in actions)
                {
                    if (!action.IsActionSlot)
                    {
                        continue;
                    }

                    _headerActions.Add(CloneActionSlot(action));
                }
            }

            _headerActions.Sort((left, right) => left.Order.CompareTo(right.Order));
            LayoutActionSlots();
            ApplyActionSlotState();
        }

        public bool TryHitActionSlot(Point location, out BeepTabHeaderAction? action)
        {
            foreach (BeepTabHeaderAction candidate in _headerActions)
            {
                if (!candidate.HitTest(location))
                {
                    continue;
                }

                action = candidate;
                return true;
            }

            action = null;
            return false;
        }

        private void LayoutActionSlots()
        {
            if (_headerActions.Count == 0)
            {
                return;
            }

            if (LayoutSnapshot == null || LayoutSnapshot.HeaderBounds.IsEmpty)
            {
                foreach (BeepTabHeaderAction action in _headerActions)
                {
                    action.Bounds = Rectangle.Empty;
                }

                return;
            }

            Rectangle headerBounds = LayoutSnapshot.HeaderBounds;
            int slotSize = GetSharedActionSlotSize(headerBounds);
            int slotGap = GetSharedActionSlotGap();

            switch (LayoutSnapshot.HeaderPosition)
            {
                case TabHeaderPosition.Top:
                case TabHeaderPosition.Bottom:
                    LayoutHorizontalActionSlots(headerBounds, slotSize, slotGap);
                    break;
                default:
                    LayoutVerticalActionSlots(headerBounds, slotSize, slotGap);
                    break;
            }
        }

        private void LayoutHorizontalActionSlots(Rectangle headerBounds, int slotSize, int slotGap)
        {
            int x = headerBounds.Right - slotGap - slotSize;
            int y = headerBounds.Top + (headerBounds.Height - slotSize) / 2;

            for (int index = _headerActions.Count - 1; index >= 0; index--)
            {
                BeepTabHeaderAction action = _headerActions[index];
                action.Bounds = action.IsVisible
                    ? new Rectangle(x, y, slotSize, slotSize)
                    : Rectangle.Empty;
                x -= slotSize + slotGap;
            }
        }

        private void LayoutVerticalActionSlots(Rectangle headerBounds, int slotSize, int slotGap)
        {
            int x = headerBounds.Left + (headerBounds.Width - slotSize) / 2;
            int y = headerBounds.Bottom - slotGap - slotSize;

            for (int index = _headerActions.Count - 1; index >= 0; index--)
            {
                BeepTabHeaderAction action = _headerActions[index];
                action.Bounds = action.IsVisible
                    ? new Rectangle(x, y, slotSize, slotSize)
                    : Rectangle.Empty;
                y -= slotSize + slotGap;
            }
        }

        private int GetSharedActionSlotSize(Rectangle headerBounds)
        {
            if (TabsOwner != null)
            {
                return TabsOwner.GetHeaderActionSlotSize(headerBounds);
            }

            int referenceSize = LayoutSnapshot.HeaderPosition == TabHeaderPosition.Top || LayoutSnapshot.HeaderPosition == TabHeaderPosition.Bottom
                ? headerBounds.Height
                : headerBounds.Width;

            return Math.Max(14, referenceSize - GetSharedActionSlotGap() * 2);
        }

        private int GetSharedActionSlotGap()
        {
            return TabsOwner?.GetHeaderActionSlotGap() ?? 4;
        }

        private void ApplyActionSlotState()
        {
            foreach (BeepTabHeaderAction action in _headerActions)
            {
                action.IsHovered = action.CommandName == _hoveredActionCommandName;
                action.IsPressed = action.CommandName == _pressedActionCommandName;
            }
        }

        private void PaintHeaderActions(Graphics graphics)
        {
            if (_headerActions.Count == 0)
            {
                return;
            }

            Font? font = Font;
            if (font == null)
            {
                return;
            }

            using Pen borderPen = new Pen(Color.FromArgb(120, ForeColor));
            using SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(24, ForeColor));
            using SolidBrush pressedBrush = new SolidBrush(Color.FromArgb(40, ForeColor));

            foreach (BeepTabHeaderAction action in _headerActions)
            {
                if (!action.IsVisible || action.Bounds.IsEmpty)
                {
                    continue;
                }

                if (action.IsPressed)
                {
                    graphics.FillRectangle(pressedBrush, action.Bounds);
                }
                else if (action.IsHovered)
                {
                    graphics.FillRectangle(hoverBrush, action.Bounds);
                }

                graphics.DrawRectangle(borderPen, action.Bounds);
                TextRenderer.DrawText(
                    graphics,
                    action.DisplayText,
                    font,
                    action.Bounds,
                    ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }
        }

        private static BeepTabHeaderAction CloneActionSlot(BeepTabHeaderAction action)
        {
            return new BeepTabHeaderAction
            {
                ActionKind = action.ActionKind,
                CommandName = action.CommandName,
                DisplayText = action.DisplayText,
                Order = action.Order,
                TabIndex = action.TabIndex,
                HitLocation = action.HitLocation,
                IsVisible = action.IsVisible,
                IsEnabled = action.IsEnabled,
                IsActionSlot = action.IsActionSlot,
                Bounds = action.Bounds
            };
        }
    }
}