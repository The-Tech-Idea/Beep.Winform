using System;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    public partial class BeepRadioGroup
    {
        #region Event Handlers
        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            // Skip if item is disabled
            if (e.Item != null && IsItemDisabled(e.Item.Text))
            {
                return;
            }
            
            // Handle selection logic
            if (AllowMultipleSelection)
            {
                _stateHelper.ToggleItem(e.Item);
            }
            else
            {
                _stateHelper.SelectItem(e.Item);
            }
            
            // Validate selection after change
            ValidateSelection();

            // Raise public event
            ItemClicked?.Invoke(this, e);
        }

        private void OnItemHoverEnter(object sender, ItemHoverEventArgs e)
        {
            ItemHoverEnter?.Invoke(this, e);
        }

        private void OnItemHoverLeave(object sender, ItemHoverEventArgs e)
        {
            ItemHoverLeave?.Invoke(this, e);
        }

        private void OnHoveredIndexChanged(object sender, IndexChangedEventArgs e)
        {
            UpdateItemStates();
            Invalidate();
        }

        private void OnFocusedIndexChanged(object sender, IndexChangedEventArgs e)
        {
            UpdateItemStates();
            Invalidate();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = GetValue();
            if (!Equals(_oldValue, newValue))
            {
                _oldValue = newValue;
                // Raise IBeepUIComponent value-changed via BaseControl helper
                base.InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
            }

            SelectionChanged?.Invoke(this, e);
            UpdateItemStates();
            Invalidate();
        }

        private void OnItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            var newValue = GetValue();
            if (!Equals(_oldValue, newValue))
            {
                _oldValue = newValue;
                base.InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
            }

            ItemSelectionChanged?.Invoke(this, e);
        }
        #endregion
    }
}
