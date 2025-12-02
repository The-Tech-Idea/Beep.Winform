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
