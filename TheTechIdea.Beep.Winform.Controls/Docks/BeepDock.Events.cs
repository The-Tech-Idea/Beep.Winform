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
        /// Event fired when a dock item is hovered
        /// </summary>
        public event EventHandler<DockItemEventArgs>? ItemHovered;

        /// <summary>
        /// Raises the SelectedItemChanged event
        /// </summary>
        protected virtual void OnSelectedItemChanged(SimpleItem? selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        #endregion
    }
}
