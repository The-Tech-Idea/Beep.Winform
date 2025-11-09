using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Public Methods and Theme
    /// </summary>
    public partial class BeepDock
    {
        #region Public Methods
        /// <summary>
        /// Adds an item to the dock
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item != null && !_items.Contains(item))
            {
                _items.Add(item);
            }
        }

        /// <summary>
        /// Removes an item from the dock
        /// </summary>
        public void RemoveItem(SimpleItem item)
        {
            if (item != null && _items.Contains(item))
            {
                if (item == _selectedItem)
                {
                    SelectedItem = null;
                }
                _items.Remove(item);
            }
        }

        /// <summary>
        /// Clears all items from the dock
        /// </summary>
        public void ClearItems()
        {
            SelectedItem = null;
            _items.Clear();
        }

        /// <summary>
        /// Gets the item at the specified screen point
        /// </summary>
        public SimpleItem GetItemAtPoint(Point point)
        {
            int index = DockHitTestHelper.HitTest(point, _itemStates);
            return index >= 0 && index < _itemStates.Count ? _itemStates[index].Item : null;
        }
        #endregion

        #region Theme
        /// <summary>
        /// Applies the current theme to the dock
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme != null)
            {
                // Apply theme colors to config if not set
                if (!_config.BackgroundColor.HasValue)
                {
                    BackColor = _currentTheme.PanelBackColor;
                }

                ForeColor = _currentTheme.LabelForeColor;
            }

            // Maintain frameless appearance
            IsChild = true;
            if (Parent != null)
                BackColor = Parent.BackColor;
            
            IsFrameless = true;
            ShowAllBorders = false;
            IsBorderAffectedByTheme = false;

            Invalidate();
        }
        #endregion
    }
}
