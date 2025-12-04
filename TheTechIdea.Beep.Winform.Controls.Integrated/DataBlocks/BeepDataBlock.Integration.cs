using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Cross-Feature Integration
    /// Ensures Triggers, LOV, Properties, Validation, and Navigation work together seamlessly
    /// </summary>
    public partial class BeepDataBlock
    {
        #region LOV Integration
        
        /// <summary>
        /// Enhanced ShowLOV with property and validation checks
        /// Integrates: LOV → Properties → Validation → Navigation
        /// </summary>
        private async Task<bool> ShowLOVIntegrated(string itemName)
        {
            // Check item properties first
            if (_items.ContainsKey(itemName))
            {
                var item = _items[itemName];
                
                // Check if item allows queries (Oracle Forms: QUERY_ALLOWED property)
                if (!item.QueryAllowed)
                {
                    MessageBox.Show(
                        $"LOV not allowed for {itemName} (QUERY_ALLOWED = false)",
                        "LOV Restricted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
                
                // Check if item is enabled
                if (!item.Enabled)
                {
                    MessageBox.Show(
                        $"LOV not available for {itemName} (item is disabled)",
                        "LOV Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }
            }
            
            // Show LOV
            var result = await ShowLOV(itemName);
            
            if (result)
            {
                // After LOV selection, validate the new value
                var newValue = GetItemValue(itemName);
                
                if (_validationRules.ContainsKey(itemName))
                {
                    var validationResult = await ValidateField(itemName, newValue);
                    
                    if (validationResult.Flag != ConfigUtil.Errors.Ok)
                    {
                        // Validation failed - show error but keep the value
                        MessageBox.Show(
                            validationResult.Message,
                            "Validation Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
                
                // Fire POST-TEXT-ITEM trigger after LOV selection
                var component = UIComponents.Values.FirstOrDefault(c => c.BoundProperty == itemName);
                if (component != null)
                {
                    await FirePostTextItem(component, newValue);
                }
                
                // Set focus to next item (Oracle Forms standard behavior)
                NextItem();
            }
            
            return result;
        }
        
        #endregion
        
        #region Property-Triggered Events
        
        /// <summary>
        /// Fire WHEN-VALIDATE-ITEM trigger when an item property changes
        /// This reuses the existing trigger type for property change validation
        /// </summary>
        private async Task FirePropertyChanged(string itemName, string propertyName, object oldValue, object newValue)
        {
            // Note: Using WhenValidateItem as the closest Oracle Forms equivalent
            // for property change validation
            var component = UIComponents.Values.FirstOrDefault(c => c.BoundProperty == itemName);
            
            if (component != null)
            {
                await FireWhenValidateItem(component, oldValue, newValue);
            }
        }
        
        /// <summary>
        /// Enhanced SetItemProperty with trigger firing
        /// </summary>
        public async Task SetItemPropertyAsync(string itemName, string propertyName, object value)
        {
            var oldValue = GetItemProperty(itemName, propertyName);
            
            // Set the property
            SetItemProperty(itemName, propertyName, value);
            
            // Fire property changed trigger (validation)
            await FirePropertyChanged(itemName, propertyName, oldValue, value);
        }
        
        #endregion
        
        #region Validation Integration
        
        /// <summary>
        /// Validate required fields automatically when Required property is set
        /// </summary>
        private void AutoValidateRequiredFields()
        {
            foreach (var item in _items.Values.Where(i => i.Required))
            {
                var value = item.Component?.GetValue();
                
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    item.HasError = true;
                    item.ErrorMessage = "This field is required";
                    
                    // Update UI component
                    if (item.Component is BaseControl beepControl)
                    {
                        beepControl.HasError = true;
                        beepControl.ErrorText = item.ErrorMessage;
                    }
                }
                else
                {
                    item.HasError = false;
                    item.ErrorMessage = null;
                    
                    // Clear UI error
                    if (item.Component is BaseControl beepControl)
                    {
                        beepControl.HasError = false;
                        beepControl.ErrorText = "";
                    }
                }
            }
        }
        
        #endregion
        
        #region Navigation Integration
        
        /// <summary>
        /// Enhanced navigation that respects item properties and fires triggers
        /// Uses existing WHEN-ITEM-FOCUS trigger (already implemented in Navigation.cs)
        /// </summary>
        private async Task<bool> NavigateToItemIntegrated(string itemName, string direction)
        {
            // Check if current item has validation errors
            if (!string.IsNullOrEmpty(CurrentItemName) && _items.ContainsKey(CurrentItemName))
            {
                var currentItem = _items[CurrentItemName];
                
                if (currentItem.HasError)
                {
                    var result = MessageBox.Show(
                        $"Field '{CurrentItemName}' has validation errors. Continue anyway?",
                        "Validation Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                        
                    if (result != DialogResult.Yes)
                    {
                        return false;
                    }
                }
            }
            
            // Perform navigation (GoToItem already fires WHEN-ITEM-FOCUS trigger)
            var success = GoToItem(itemName);
            
            return success;
        }
        
        /// <summary>
        /// Get next navigable item (respects Navigable property)
        /// </summary>
        private BeepDataBlockItem? GetNextNavigableItemIntegrated(string currentItemName)
        {
            var items = GetNavigableItems();
            
            if (items.Count == 0)
                return null;
                
            int currentIndex = items.FindIndex(i => i.ItemName == currentItemName);
            int nextIndex = (currentIndex + 1) % items.Count;
            
            return items[nextIndex];
        }
        
        /// <summary>
        /// Get previous navigable item (respects Navigable property)
        /// </summary>
        private BeepDataBlockItem? GetPreviousNavigableItemIntegrated(string currentItemName)
        {
            var items = GetNavigableItems();
            
            if (items.Count == 0)
                return null;
                
            int currentIndex = items.FindIndex(i => i.ItemName == currentItemName);
            int prevIndex = currentIndex <= 0 ? items.Count - 1 : currentIndex - 1;
            
            return items[prevIndex];
        }
        
        #endregion
        
        #region Initialization Integration
        
        /// <summary>
        /// Initialize all integrations after block is fully loaded
        /// Call this after setting Data, EntityStructure, and registering all items/triggers/LOVs
        /// </summary>
        public void InitializeIntegrations()
        {
            // Register all UI components as items if not already registered
            RegisterAllItems();
            
            // Auto-validate required fields
            AutoValidateRequiredFields();
            
            // Setup keyboard navigation
            SetupKeyboardNavigation();
            
            // Register with FormsManager if available
            if (_formsManager != null)
            {
                RegisterWithFormsManager();
            }
            
            // Fire WHEN-NEW-BLOCK-INSTANCE trigger
            _ = FireWhenNewBlockInstance();
        }
        
        #endregion
    }
}

