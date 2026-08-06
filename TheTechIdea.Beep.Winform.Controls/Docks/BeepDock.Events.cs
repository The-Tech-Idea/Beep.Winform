using System;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Docks;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Events
    /// </summary>
    public partial class BeepDock
    {
        #region Events
        /// <summary>
        /// Event fired when the selected item changes
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs>? SelectedItemChanged;

        /// <summary>
        /// Event fired when a dock item is clicked
        /// </summary>
        public event EventHandler<DockItemEventArgs>? ItemClicked;

        /// <summary>
        /// Raises <see cref="ItemClicked"/> for an activation that did not come from the mouse.
        /// </summary>
        /// <remarks>
        /// The accessible tree's <c>DoDefaultAction</c> must take the same path a click takes, or
        /// activating an item with a screen reader would select it visually and tell no one.
        /// </remarks>
        internal void RaiseItemClicked(Models.SimpleItem item, int index)
            => ItemClicked?.Invoke(this, new DockItemEventArgs(item, index));

        /// <summary>
        /// Event fired when a dock item is hovered
        /// </summary>
        public event EventHandler<DockItemEventArgs>? ItemHovered;

        /// <summary>
        /// Raises the SelectedItemChanged event
        /// </summary>
        protected virtual void OnSelectedItemChanged(SimpleItem? selectedItem)
        {
            if (selectedItem != null)
            {
                AccessibleDescription = $"Dock selected item: {selectedItem.Text}";
            }
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        #endregion
    }
}
