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
            // Suppress the trailing single click that always follows a double click on
            // Windows. The dblclick has already been processed (selection animation +
            // public ItemDoubleClicked event) and the user does not expect a second
            // selection change immediately after.
            var now = DateTime.UtcNow;
            if ((now - _lastDoubleClickUtc).TotalMilliseconds < DoubleClickSuppressionMs)
            {
                return;
            }

            // Skip if item is disabled
            if (e.Item != null && IsItemDisabled(e.Item.Text))
            {
                return;
            }

            // Animate the clicked item
            if (e.Index >= 0)
            {
                StartItemAnimation(e.Index);
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

        private void OnItemDoubleClicked(object sender, ItemClickEventArgs e)
        {
            // Record timestamp so the trailing single click is suppressed.
            _lastDoubleClickUtc = DateTime.UtcNow;
            if (e.Index >= 0)
            {
                StartItemAnimation(e.Index);
            }
            ItemDoubleClicked?.Invoke(this, e);
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
            if (e.Index >= 0) StartItemAnimation(e.Index);
            UpdateItemStates();
            RequestVisualRefresh();
        }

        private void OnFocusedIndexChanged(object sender, IndexChangedEventArgs e)
        {
            if (e.Index >= 0) StartItemAnimation(e.Index);
            UpdateItemStates();
            RequestVisualRefresh();
        }

        private void OnPressedIndexChanged(object sender, IndexChangedEventArgs e)
        {
            if (e.Index >= 0) StartItemAnimation(e.Index);
            UpdateItemStates();
            RequestVisualRefresh();
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
            RequestVisualRefresh();
            UpdateAccessibilityMetadata();
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
            UpdateAccessibilityMetadata();
        }
        #endregion
    }
}

