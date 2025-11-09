using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Mouse Interaction
    /// </summary>
    public partial class BeepDock
    {
        #region Mouse Interaction
        /// <summary>
        /// Handles mouse move events
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Hit test to find hovered item
            int newHoveredIndex = DockHitTestHelper.HitTest(e.Location, _itemStates);

            if (newHoveredIndex != _hoveredIndex)
            {
                // Update hover states
                for (int i = 0; i < _itemStates.Count; i++)
                {
                    _itemStates[i].IsHovered = (i == newHoveredIndex);
                }

                _hoveredIndex = newHoveredIndex;

                // Fire hover event
                if (_hoveredIndex >= 0 && _hoveredIndex < _itemStates.Count)
                {
                    ItemHovered?.Invoke(this, new DockItemEventArgs(_itemStates[_hoveredIndex].Item, _hoveredIndex));
                }

                UpdateItemBounds();
                Invalidate();
            }
        }

        /// <summary>
        /// Handles mouse leave events
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredIndex >= 0)
            {
                foreach (var state in _itemStates)
                {
                    state.IsHovered = false;
                }

                _hoveredIndex = -1;
                UpdateItemBounds();
                Invalidate();
            }
        }

        /// <summary>
        /// Handles mouse click events
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button != MouseButtons.Left)
                return;

            int clickedIndex = DockHitTestHelper.HitTest(e.Location, _itemStates);

            if (clickedIndex >= 0 && clickedIndex < _itemStates.Count)
            {
                var clickedItem = _itemStates[clickedIndex].Item;
                SelectedItem = clickedItem;
                ItemClicked?.Invoke(this, new DockItemEventArgs(clickedItem, clickedIndex));
            }
        }
        #endregion
    }
}
