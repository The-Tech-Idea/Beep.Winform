using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Integrated.Dialogs;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - LOV (List of Values) System
    /// Provides Oracle Forms-compatible LOV functionality
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        /// <summary>
        /// Dictionary of LOVs registered for this block
        /// Key: Item/Field name, Value: LOV definition
        /// </summary>
        private Dictionary<string, BeepDataBlockLOV> _lovs = new Dictionary<string, BeepDataBlockLOV>();
        
        #endregion
        
        #region LOV Registration
        
        /// <summary>
        /// Register a LOV for a specific item/field
        /// Oracle Forms equivalent: Attaching LOV to an item
        /// </summary>
        /// <param name="itemName">Item/field name</param>
        /// <param name="lov">LOV definition</param>
        public void RegisterLOV(string itemName, BeepDataBlockLOV lov)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentException("Item name cannot be null or empty", nameof(itemName));
            if (lov == null)
                throw new ArgumentNullException(nameof(lov));
                
            _lovs[itemName] = lov;
            
            // Attach LOV to component if it exists
            if (UIComponents.ContainsKey(itemName))
            {
                AttachLOVToComponent(itemName, lov);
            }
        }
        
        /// <summary>
        /// Unregister a LOV from an item
        /// </summary>
        public void UnregisterLOV(string itemName)
        {
            if (_lovs.ContainsKey(itemName))
            {
                _lovs.Remove(itemName);
                DetachLOVFromComponent(itemName);
            }
        }
        
        /// <summary>
        /// Check if an item has a LOV
        /// </summary>
        public bool HasLOV(string itemName)
        {
            return _lovs.ContainsKey(itemName);
        }
        
        /// <summary>
        /// Get LOV for an item
        /// </summary>
        public BeepDataBlockLOV GetLOV(string itemName)
        {
            return _lovs.ContainsKey(itemName) ? _lovs[itemName] : null;
        }
        
        #endregion
        
        #region LOV Component Attachment
        
        private void AttachLOVToComponent(string itemName, BeepDataBlockLOV lov)
        {
            if (!UIComponents.ContainsKey(itemName))
                return;
                
            var component = UIComponents[itemName];
            
            if (component is Control control)
            {
                // Add double-click handler (Oracle Forms standard)
                control.DoubleClick += async (s, e) => await ShowLOV(itemName);
                
                // Add F9 key handler (Oracle Forms standard LOV key)
                control.KeyDown += async (s, e) =>
                {
                    if (e.KeyCode == Keys.F9)
                    {
                        await ShowLOV(itemName);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                };
                
                // Add tooltip to indicate LOV availability (if BaseControl)
                if (control is BaseControl beepControl)
                {
                    beepControl.ToolTipText = $"Press F9 or double-click to show List of Values\n" +
                                             $"LOV: {lov.LOVName} - {lov.Title}";
                }
            }
        }
        
        private void DetachLOVFromComponent(string itemName)
        {
            // Note: Removing event handlers requires keeping references
            // For now, we'll just clear the tooltip
            if (UIComponents.ContainsKey(itemName) && UIComponents[itemName] is BaseControl beepControl)
            {
                beepControl.ToolTipText = "";
            }
        }
        
        #endregion
        
        #region LOV Display
        
        /// <summary>
        /// Show LOV for a specific item
        /// Oracle Forms equivalent: SHOW_LOV built-in
        /// </summary>
        /// <param name="itemName">Item name to show LOV for</param>
        /// <returns>True if value was selected, false if cancelled</returns>
        public async Task<bool> ShowLOV(string itemName)
        {
            if (!_lovs.ContainsKey(itemName))
            {
                MessageBox.Show($"No LOV registered for item '{itemName}'", "LOV Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            var lov = _lovs[itemName];
            
            // Fire BEFORE-DISPLAY event
            var beforeArgs = new LOVEventArgs { LOV = lov };
            lov.OnBeforeDisplay(beforeArgs);
            
            if (beforeArgs.Cancel)
                return false;
                
            try
            {
                // Load LOV data
                var lovData = await LoadLOVData(lov);
                
                if (lovData == null || lovData.Count == 0)
                {
                    MessageBox.Show($"No data available in LOV '{lov.LOVName}'", "LOV Empty",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                
                // Show LOV dialog
                using (var dialog = new BeepLOVDialog(lov, lovData))
                {
                    // Set current value as initial selection
                    var currentValue = GetItemValue(itemName);
                    dialog.InitialValue = currentValue;
                    
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get selected value
                        var selectedValue = dialog.SelectedValue;
                        var selectedRecord = dialog.SelectedRecord;
                        
                        if (selectedValue != null)
                        {
                            // Set return value to item
                            SetItemValue(itemName, selectedValue);
                            
                            // Auto-populate related fields
                            if (lov.AutoPopulateRelatedFields && selectedRecord != null)
                            {
                                PopulateRelatedFields(lov, selectedRecord);
                            }
                            
                            // Fire AFTER-SELECTION event
                            var afterArgs = new LOVEventArgs
                            {
                                LOV = lov,
                                SelectedValues = new List<object> { selectedValue },
                                SelectedRecord = selectedRecord
                            };
                            lov.OnAfterSelection(afterArgs);
                            
                            return true;
                        }
                    }
                    else
                    {
                        // Fire ON-CANCEL event
                        var cancelArgs = new LOVEventArgs { LOV = lov };
                        lov.OnLOVCancel(cancelArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing LOV '{lov.LOVName}': {ex.Message}", "LOV Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                // Fire ON-ERROR trigger
                var errorContext = new TriggerContext
                {
                    Block = this,
                    TriggerType = TriggerType.OnError,
                    ErrorMessage = ex.Message,
                    Parameters = new Dictionary<string, object>
                    {
                        ["Exception"] = ex,
                        ["LOVName"] = lov.LOVName,
                        ["ItemName"] = itemName
                    }
                };
                await ExecuteTriggers(TriggerType.OnError, errorContext);
            }
            
            return false;
        }
        
        #endregion
        
        #region LOV Data Loading
        
        private async Task<List<object>> LoadLOVData(BeepDataBlockLOV lov)
        {
            // Check cache first
            if (lov.UseCache && lov.IsCacheValid())
            {
                return lov.CachedData;
            }
            
            try
            {
                if (beepService?.DMEEditor == null)
                {
                    throw new InvalidOperationException("DMEEditor not available. Cannot load LOV data.");
                }
                
                // Get data source
                var dataSource = beepService.DMEEditor.GetDataSource(lov.DataSourceName);
                if (dataSource == null)
                {
                    throw new InvalidOperationException($"Data source '{lov.DataSourceName}' not found");
                }
                
                // Build query
                List<object> data;
                
                if (lov.Filters != null && lov.Filters.Count > 0)
                {
                    // Query with filters
                    data = (await dataSource.GetEntityAsync(lov.EntityName, lov.Filters))
                        ?.Cast<object>()
                        .ToList();
                }
                else if (!string.IsNullOrEmpty(lov.WhereClause))
                {
                    // Query with WHERE clause
                    // Note: This requires DataSource to support WHERE clause queries
                    data = (await dataSource.GetEntityAsync(lov.EntityName, null))
                        ?.Cast<object>()
                        .ToList();
                }
                else
                {
                    // Query all
                    data = (await dataSource.GetEntityAsync(lov.EntityName, null))
                        ?.Cast<object>()
                        .ToList();
                }
                
                // Apply ORDER BY if specified
                if (!string.IsNullOrEmpty(lov.OrderByClause) && data != null)
                {
                    data = ApplyOrderBy(data, lov.OrderByClause);
                }
                
                // Cache the data
                if (lov.UseCache && data != null)
                {
                    lov.CachedData = data;
                    lov.CacheTimestamp = DateTime.Now;
                }
                
                return data ?? new List<object>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading LOV data for '{lov.LOVName}': {ex.Message}", ex);
            }
        }
        
        private List<object> ApplyOrderBy(List<object> data, string orderByClause)
        {
            // Simple ORDER BY implementation
            // Format: "FieldName ASC" or "FieldName DESC"
            var parts = orderByClause.Split(' ');
            if (parts.Length == 0)
                return data;
                
            var FieldName = parts[0];
            var ascending = parts.Length == 1 || parts[1].ToUpper() != "DESC";
            
            try
            {
                var property = data.First()?.GetType().GetProperty(FieldName);
                if (property != null)
                {
                    return ascending
                        ? data.OrderBy(item => property.GetValue(item)).ToList()
                        : data.OrderByDescending(item => property.GetValue(item)).ToList();
                }
            }
            catch
            {
                // If ordering fails, return original data
            }
            
            return data;
        }
        
        #endregion
        
        #region Related Field Population
        
        private void PopulateRelatedFields(BeepDataBlockLOV lov, object selectedRecord)
        {
            if (lov.RelatedFieldMappings == null || lov.RelatedFieldMappings.Count == 0)
                return;
                
            foreach (var mapping in lov.RelatedFieldMappings)
            {
                var lovField = mapping.Key;      // Field name in LOV record
                var blockField = mapping.Value;  // Field name in block
                
                try
                {
                    // Get value from selected record
                    var property = selectedRecord.GetType().GetProperty(lovField);
                    if (property != null)
                    {
                        var value = property.GetValue(selectedRecord);
                        
                        // Set value to block field
                        SetItemValue(blockField, value);
                    }
                }
                catch
                {
                    // Ignore errors in related field population
                }
            }
        }
        
        #endregion
        
        #region LOV Validation
        
        /// <summary>
        /// Validate that a value exists in the LOV
        /// </summary>
        public async Task<bool> ValidateLOVValue(string itemName, object value)
        {
            if (!_lovs.ContainsKey(itemName))
                return true;  // No LOV = no validation
                
            var lov = _lovs[itemName];
            
            // If validation type is Unrestricted, any value is OK
            if (lov.ValidationType == LOVValidationType.Unrestricted)
                return true;
                
            // Load LOV data
            var lovData = await LoadLOVData(lov);
            
            if (lovData == null || lovData.Count == 0)
                return true;  // Can't validate against empty LOV
                
            // Check if value exists in LOV
            var valueString = value?.ToString();
            
            var exists = lovData.Any(record =>
            {
                var property = record.GetType().GetProperty(lov.ReturnField);
                if (property != null)
                {
                    var recordValue = property.GetValue(record)?.ToString();
                    return string.Equals(recordValue, valueString, StringComparison.OrdinalIgnoreCase);
                }
                return false;
            });
            
            if (!exists && lov.ValidationType == LOVValidationType.ListOnly)
            {
                MessageBox.Show(
                    $"Invalid value for {itemName}. Please select from the List of Values (F9).",
                    "LOV Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                    
                return false;
            }
            
            return true;
        }
        
        #endregion
        
        #region LOV Query
        
        /// <summary>
        /// Get all registered LOVs for this block
        /// </summary>
        public Dictionary<string, BeepDataBlockLOV> GetAllLOVs()
        {
            return new Dictionary<string, BeepDataBlockLOV>(_lovs);
        }
        
        /// <summary>
        /// Get LOV count
        /// </summary>
        public int GetLOVCount()
        {
            return _lovs.Count;
        }
        
        /// <summary>
        /// Clear all LOV caches
        /// </summary>
        public void ClearAllLOVCaches()
        {
            foreach (var lov in _lovs.Values)
            {
                lov.ClearCache();
            }
        }
        
        #endregion
    }
}

