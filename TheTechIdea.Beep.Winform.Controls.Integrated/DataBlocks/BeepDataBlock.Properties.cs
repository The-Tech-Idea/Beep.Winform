using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;
using TheTechIdea.Beep.Editor.UOWManager.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Item & Block Properties
    /// Provides Oracle Forms-compatible property system
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        /// <summary>
        /// Dictionary of items with their properties
        /// Key: Item name, Value: Item properties
        /// </summary>
        private Dictionary<string, BeepDataBlockItem> _items = new Dictionary<string, BeepDataBlockItem>();
        
        /// <summary>
        /// Block-level properties
        /// </summary>
        private BeepDataBlockProperties _blockProperties = new BeepDataBlockProperties();
        
        #endregion
        
        #region Item Registration & Management
        
        /// <summary>
        /// Register an item with the block
        /// </summary>
        public void RegisterItem(string itemName, IBeepUIComponent component)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentException("Item name cannot be null or empty", nameof(itemName));
            if (component == null)
                throw new ArgumentNullException(nameof(component));
                
            var item = new BeepDataBlockItem
            {
                ItemName = itemName,
                BoundProperty = component.BoundProperty,
                Component = component,
                Enabled = true,
                Visible = true,
                QueryAllowed = true,
                InsertAllowed = true,
                UpdateAllowed = true
            };
            
            _items[itemName] = item;
        }
        
        /// <summary>
        /// Register all UIComponents as items automatically
        /// </summary>
        public void RegisterAllItems()
        {
            foreach (var kvp in UIComponents)
            {
                if (!_items.ContainsKey(kvp.Key))
                {
                    RegisterItem(kvp.Key, kvp.Value);
                }
            }
        }
        
        /// <summary>
        /// Get an item by name
        /// </summary>
        public BeepDataBlockItem GetItem(string itemName)
        {
            return _items.ContainsKey(itemName) ? _items[itemName] : null;
        }
        
        /// <summary>
        /// Get all items
        /// </summary>
        public Dictionary<string, BeepDataBlockItem> GetAllItems()
        {
            return new Dictionary<string, BeepDataBlockItem>(_items);
        }
        
        #endregion
        
        #region Item Property Get/Set
        
        /// <summary>
        /// Set an item property
        /// Oracle Forms equivalent: SET_ITEM_PROPERTY
        /// </summary>
        public void SetItemProperty(string itemName, string propertyName, object value)
        {
            if (!_items.ContainsKey(itemName))
            {
                // Auto-register if component exists
                if (UIComponents.ContainsKey(itemName))
                {
                    RegisterItem(itemName, UIComponents[itemName]);
                }
                else
                {
                    throw new ArgumentException($"Item '{itemName}' not found", nameof(itemName));
                }
            }
            
            var item = _items[itemName];
            var property = typeof(BeepDataBlockItem).GetProperty(propertyName);
            
            if (property != null && property.CanWrite)
            {
                property.SetValue(item, value);
                ApplyItemProperty(item, propertyName);
            }
            else
            {
                throw new ArgumentException($"Property '{propertyName}' not found or is read-only", nameof(propertyName));
            }
        }
        
        /// <summary>
        /// Get an item property value
        /// Oracle Forms equivalent: GET_ITEM_PROPERTY
        /// </summary>
        public object GetItemProperty(string itemName, string propertyName)
        {
            if (!_items.ContainsKey(itemName))
                return null;
                
            var item = _items[itemName];
            var property = typeof(BeepDataBlockItem).GetProperty(propertyName);
            
            return property?.GetValue(item);
        }
        
        /// <summary>
        /// Apply item property to the UI component
        /// </summary>
        private void ApplyItemProperty(BeepDataBlockItem item, string propertyName)
        {
            if (item.Component == null)
                return;
                
            if (item.Component is Control control)
            {
                switch (propertyName)
                {
                    case nameof(BeepDataBlockItem.Enabled):
                        control.Enabled = item.Enabled;
                        break;
                        
                    case nameof(BeepDataBlockItem.Visible):
                        control.Visible = item.Visible;
                        break;
                        
                    case nameof(BeepDataBlockItem.Required):
                        // Add visual indicator for required fields
                        if (item.Component is BaseControl beepControl)
                        {
                            if (item.Required)
                            {
                                // Check if field has value
                                var value = item.Component.GetValue();
                                beepControl.HasError = value == null || string.IsNullOrEmpty(value.ToString());
                                beepControl.ErrorText = item.Required && beepControl.HasError 
                                    ? "This field is required" 
                                    : "";
                            }
                            else
                            {
                                beepControl.HasError = false;
                                beepControl.ErrorText = "";
                            }
                        }
                        break;
                        
                    case nameof(BeepDataBlockItem.HintText):
                        if (item.Component is BaseControl beepCtrl)
                        {
                            beepCtrl.ToolTipText = item.HintText;
                        }
                        break;
                        
                    case nameof(BeepDataBlockItem.DefaultValue):
                        // Default value is applied in WHEN-NEW-RECORD-INSTANCE
                        break;
                }
            }
        }
        
        #endregion
        
        #region Block Property Get/Set
        
        /// <summary>
        /// Block properties (Oracle Forms block-level properties)
        /// </summary>
        public BeepDataBlockProperties BlockProperties
        {
            get => _blockProperties;
            set => _blockProperties = value ?? new BeepDataBlockProperties();
        }
        
        /// <summary>
        /// Oracle Forms: WHERE_CLAUSE property
        /// </summary>
        public string WhereClause
        {
            get => _blockProperties.WhereClause;
            set => _blockProperties.WhereClause = value;
        }
        
        /// <summary>
        /// Oracle Forms: ORDER_BY_CLAUSE property
        /// </summary>
        public string OrderByClause
        {
            get => _blockProperties.OrderByClause;
            set => _blockProperties.OrderByClause = value;
        }
        
        /// <summary>
        /// Oracle Forms: QUERY_DATA_SOURCE_NAME property
        /// </summary>
        public string QueryDataSourceName
        {
            get => _blockProperties.QueryDataSourceName;
            set => _blockProperties.QueryDataSourceName = value;
        }
        
        /// <summary>
        /// Oracle Forms: INSERT_ALLOWED property (block-level)
        /// </summary>
        public bool InsertAllowed
        {
            get => _blockProperties.InsertAllowed;
            set => _blockProperties.InsertAllowed = value;
        }
        
        /// <summary>
        /// Oracle Forms: UPDATE_ALLOWED property (block-level)
        /// </summary>
        public bool UpdateAllowed
        {
            get => _blockProperties.UpdateAllowed;
            set => _blockProperties.UpdateAllowed = value;
        }
        
        /// <summary>
        /// Oracle Forms: DELETE_ALLOWED property (block-level)
        /// </summary>
        public bool DeleteAllowed
        {
            get => _blockProperties.DeleteAllowed;
            set => _blockProperties.DeleteAllowed = value;
        }
        
        /// <summary>
        /// Oracle Forms: QUERY_ALLOWED property (block-level)
        /// </summary>
        public bool QueryAllowed
        {
            get => _blockProperties.QueryAllowed;
            set => _blockProperties.QueryAllowed = value;
        }
        
        #endregion
        
        #region Property Application Helpers
        
        /// <summary>
        /// Apply all item properties to UI components
        /// </summary>
        public void ApplyAllItemProperties()
        {
            foreach (var item in _items.Values)
            {
                ApplyAllPropertiesForItem(item);
            }
        }
        
        private void ApplyAllPropertiesForItem(BeepDataBlockItem item)
        {
            if (item.Component == null)
                return;
                
            // Apply all properties
            ApplyItemProperty(item, nameof(BeepDataBlockItem.Enabled));
            ApplyItemProperty(item, nameof(BeepDataBlockItem.Visible));
            ApplyItemProperty(item, nameof(BeepDataBlockItem.Required));
            ApplyItemProperty(item, nameof(BeepDataBlockItem.HintText));
        }
        
        /// <summary>
        /// Set item properties based on mode (Query vs CRUD)
        /// </summary>
        public void ApplyModeBasedProperties()
        {
            var mode = this.BlockMode;
            
            foreach (var item in _items.Values)
            {
                if (item.Component is Control control)
                {
                    // Enable/disable based on mode and item properties
                    control.Enabled = item.CanModify(mode);
                }
            }
        }
        
        /// <summary>
        /// Apply default values to all items with DefaultValue set
        /// Called during WHEN-NEW-RECORD-INSTANCE
        /// </summary>
        public void ApplyDefaultValues()
        {
            foreach (var item in _items.Values.Where(i => i.DefaultValue != null))
            {
                try
                {
                    item.Component?.SetValue(item.DefaultValue);
                    item.CurrentValue = item.DefaultValue;
                }
                catch
                {
                    // Ignore errors in default value application
                }
            }
        }
        
        /// <summary>
        /// Validate all required fields
        /// </summary>
        public bool ValidateRequiredFields(out List<string> errors)
        {
            errors = new List<string>();
            
            foreach (var item in _items.Values.Where(i => i.Required && i.ShouldValidate()))
            {
                var value = item.Component?.GetValue();
                
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    errors.Add($"{item.ItemName} is required");
                    
                    // Set error state on component
                    if (item.Component is BaseControl beepControl)
                    {
                        beepControl.HasError = true;
                        beepControl.ErrorText = "This field is required";
                    }
                }
            }
            
            return errors.Count == 0;
        }
        
        #endregion
    }
}

