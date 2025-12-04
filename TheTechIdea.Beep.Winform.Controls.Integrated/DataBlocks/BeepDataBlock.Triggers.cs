using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Trigger System
    /// Provides Oracle Forms-compatible trigger functionality
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        /// <summary>
        /// Dictionary of registered triggers organized by type
        /// Key: TriggerType, Value: List of triggers (ordered by ExecutionOrder)
        /// </summary>
        private Dictionary<TriggerType, List<BeepDataBlockTrigger>> _triggers = new Dictionary<TriggerType, List<BeepDataBlockTrigger>>();
        
        /// <summary>
        /// Dictionary of all triggers by name (for named trigger lookup)
        /// </summary>
        private Dictionary<string, BeepDataBlockTrigger> _namedTriggers = new Dictionary<string, BeepDataBlockTrigger>();
        
        /// <summary>
        /// Whether trigger execution is currently suppressed
        /// </summary>
        private bool _suppressTriggers = false;
        
        #endregion
        
        #region Trigger Registration
        
        /// <summary>
        /// Register a trigger with automatic execution order
        /// </summary>
        /// <param name="type">Trigger type</param>
        /// <param name="handler">Handler function</param>
        /// <param name="executionOrder">Execution order (lower executes first)</param>
        public void RegisterTrigger(TriggerType type, Func<TriggerContext, Task<bool>> handler, int executionOrder = 0)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
                
            var trigger = new BeepDataBlockTrigger
            {
                TriggerName = $"{type}_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                TriggerType = type,
                Handler = handler,
                ExecutionOrder = executionOrder,
                Timing = BeepDataBlockTrigger.GetTimingFromType(type),
                Scope = BeepDataBlockTrigger.GetScopeFromType(type),
                IsEnabled = true,
                RegisteredBy = Environment.UserName
            };
            
            RegisterTriggerInternal(trigger);
        }
        
        /// <summary>
        /// Register a named trigger
        /// </summary>
        /// <param name="triggerName">Unique trigger name</param>
        /// <param name="type">Trigger type</param>
        /// <param name="handler">Handler function</param>
        /// <param name="description">Trigger description (optional)</param>
        public void RegisterTrigger(string triggerName, TriggerType type, 
            Func<TriggerContext, Task<bool>> handler, string description = null)
        {
            if (string.IsNullOrEmpty(triggerName))
                throw new ArgumentException("Trigger name cannot be null or empty", nameof(triggerName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (_namedTriggers.ContainsKey(triggerName))
                throw new InvalidOperationException($"Trigger with name '{triggerName}' is already registered");
                
            var trigger = new BeepDataBlockTrigger
            {
                TriggerName = triggerName,
                TriggerType = type,
                Handler = handler,
                ExecutionOrder = 0,
                Timing = BeepDataBlockTrigger.GetTimingFromType(type),
                Scope = BeepDataBlockTrigger.GetScopeFromType(type),
                IsEnabled = true,
                Description = description,
                RegisteredBy = Environment.UserName
            };
            
            RegisterTriggerInternal(trigger);
            _namedTriggers[triggerName] = trigger;
        }
        
        private void RegisterTriggerInternal(BeepDataBlockTrigger trigger)
        {
            if (!_triggers.ContainsKey(trigger.TriggerType))
            {
                _triggers[trigger.TriggerType] = new List<BeepDataBlockTrigger>();
            }
            
            _triggers[trigger.TriggerType].Add(trigger);
            
            // Sort by execution order
            _triggers[trigger.TriggerType] = _triggers[trigger.TriggerType]
                .OrderBy(t => t.ExecutionOrder)
                .ThenBy(t => t.RegisteredDate)
                .ToList();
        }
        
        #endregion
        
        #region Trigger Execution
        
        /// <summary>
        /// Execute all triggers of a specific type
        /// Returns false if any trigger cancels the operation
        /// </summary>
        protected async Task<bool> ExecuteTriggers(TriggerType type, TriggerContext context)
        {
            if (_suppressTriggers)
                return true;
                
            if (!_triggers.ContainsKey(type))
                return true;  // No triggers registered for this type
                
            var triggers = _triggers[type].Where(t => t.IsEnabled).ToList();
            
            if (triggers.Count == 0)
                return true;
                
            foreach (var trigger in triggers)
            {
                try
                {
                    var result = await trigger.ExecuteAsync(context);
                    
                    if (!result || context.Cancel)
                    {
                        // Trigger cancelled the operation
                        if (!string.IsNullOrEmpty(context.ErrorMessage))
                        {
                            MessageBox.Show(
                                context.ErrorMessage,
                                "Operation Cancelled",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        
                        return false;
                    }
                    
                    // Check for warnings
                    if (context.Warnings.Count > 0)
                    {
                        var warningMessage = string.Join("\n", context.Warnings);
                        MessageBox.Show(
                            warningMessage,
                            "Validation Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
                catch (TriggerExecutionException tex)
                {
                    // Trigger threw an exception → Fire ON-ERROR trigger
                    var errorContext = new TriggerContext
                    {
                        Block = this,
                        TriggerType = TriggerType.OnError,
                        ErrorMessage = tex.Message,
                        Parameters = new Dictionary<string, object>
                        {
                            ["Exception"] = tex.InnerException ?? tex,
                            ["OriginalTrigger"] = type,
                            ["FailedTriggerName"] = trigger.TriggerName
                        }
                    };
                    
                    // Execute ON-ERROR trigger (if registered)
                    await ExecuteTriggers(TriggerType.OnError, errorContext);
                    
                    // Operation failed
                    return false;
                }
                catch (Exception ex)
                {
                    // Unexpected exception
                    var errorContext = new TriggerContext
                    {
                        Block = this,
                        TriggerType = TriggerType.OnError,
                        ErrorMessage = $"Unexpected error in trigger: {ex.Message}",
                        Parameters = new Dictionary<string, object>
                        {
                            ["Exception"] = ex,
                            ["OriginalTrigger"] = type,
                            ["FailedTriggerName"] = trigger.TriggerName
                        }
                    };
                    
                    await ExecuteTriggers(TriggerType.OnError, errorContext);
                    
                    return false;
                }
            }
            
            return true;
        }
        
        #endregion
        
        #region Form-Level Trigger Execution
        
        /// <summary>
        /// Fire WHEN-NEW-FORM-INSTANCE trigger
        /// </summary>
        public async Task<bool> FireWhenNewFormInstance()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.WhenNewFormInstance
            };
            
            return await ExecuteTriggers(TriggerType.WhenNewFormInstance, context);
        }
        
        /// <summary>
        /// Fire PRE-FORM trigger
        /// </summary>
        public async Task<bool> FirePreForm()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreForm
            };
            
            return await ExecuteTriggers(TriggerType.PreForm, context);
        }
        
        /// <summary>
        /// Fire POST-FORM trigger
        /// </summary>
        public async Task<bool> FirePostForm()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostForm
            };
            
            return await ExecuteTriggers(TriggerType.PostForm, context);
        }
        
        #endregion
        
        #region Block-Level Trigger Execution
        
        /// <summary>
        /// Fire WHEN-NEW-BLOCK-INSTANCE trigger
        /// </summary>
        public async Task<bool> FireWhenNewBlockInstance()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.WhenNewBlockInstance
            };
            
            return await ExecuteTriggers(TriggerType.WhenNewBlockInstance, context);
        }
        
        /// <summary>
        /// Fire WHEN-CLEAR-BLOCK trigger
        /// </summary>
        public async Task<bool> FireWhenClearBlock()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.WhenClearBlock
            };
            
            return await ExecuteTriggers(TriggerType.WhenClearBlock, context);
        }
        
        /// <summary>
        /// Fire WHEN-CREATE-RECORD trigger
        /// </summary>
        public async Task<bool> FireWhenCreateRecord()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.WhenCreateRecord
            };
            
            return await ExecuteTriggers(TriggerType.WhenCreateRecord, context);
        }
        
        #endregion
        
        #region Record-Level Trigger Execution
        
        /// <summary>
        /// Fire WHEN-NEW-RECORD-INSTANCE trigger
        /// </summary>
        public async Task<bool> FireWhenNewRecordInstance()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.WhenNewRecordInstance,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.WhenNewRecordInstance, context);
        }
        
        /// <summary>
        /// Fire WHEN-VALIDATE-RECORD trigger
        /// </summary>
        public async Task<bool> FireWhenValidateRecord()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.WhenValidateRecord,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.WhenValidateRecord, context);
        }
        
        /// <summary>
        /// Fire POST-QUERY trigger
        /// </summary>
        public async Task<bool> FirePostQuery()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostQuery,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PostQuery, context);
        }
        
        /// <summary>
        /// Fire PRE-INSERT trigger
        /// </summary>
        protected async Task<bool> FirePreInsert()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreInsert,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PreInsert, context);
        }
        
        /// <summary>
        /// Fire POST-INSERT trigger
        /// </summary>
        protected async Task<bool> FirePostInsert()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostInsert,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PostInsert, context);
        }
        
        /// <summary>
        /// Fire PRE-UPDATE trigger
        /// </summary>
        protected async Task<bool> FirePreUpdate()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreUpdate,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PreUpdate, context);
        }
        
        /// <summary>
        /// Fire POST-UPDATE trigger
        /// </summary>
        protected async Task<bool> FirePostUpdate()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostUpdate,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PostUpdate, context);
        }
        
        /// <summary>
        /// Fire PRE-DELETE trigger
        /// </summary>
        protected async Task<bool> FirePreDelete()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreDelete,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PreDelete, context);
        }
        
        /// <summary>
        /// Fire POST-DELETE trigger
        /// </summary>
        protected async Task<bool> FirePostDelete()
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostDelete,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PostDelete, context);
        }
        
        #endregion
        
        #region Item-Level Trigger Execution
        
        /// <summary>
        /// Fire WHEN-VALIDATE-ITEM trigger
        /// </summary>
        public async Task<bool> FireWhenValidateItem(IBeepUIComponent item, object oldValue, object newValue)
        {
            var context = new TriggerContext
            {
                Block = this,
                Item = item,
                FieldName = item?.BoundProperty,
                OldValue = oldValue,
                NewValue = newValue,
                TriggerType = TriggerType.WhenValidateItem,
                RecordValues = GetCurrentRecordValues()
            };
            
            if (SYSTEM != null)
            {
                SYSTEM.TRIGGER_ITEM = item?.ComponentName;
                SYSTEM.TRIGGER_FIELD = item?.BoundProperty;
            }
            
            return await ExecuteTriggers(TriggerType.WhenValidateItem, context);
        }
        
        /// <summary>
        /// Fire POST-TEXT-ITEM trigger
        /// </summary>
        public async Task<bool> FirePostTextItem(IBeepUIComponent item, object newValue)
        {
            var context = new TriggerContext
            {
                Block = this,
                Item = item,
                FieldName = item?.BoundProperty,
                NewValue = newValue,
                TriggerType = TriggerType.PostTextItem,
                RecordValues = GetCurrentRecordValues()
            };
            
            return await ExecuteTriggers(TriggerType.PostTextItem, context);
        }
        
        /// <summary>
        /// Fire KEY-NEXT-ITEM trigger
        /// </summary>
        public async Task<bool> FireKeyNextItem(IBeepUIComponent item)
        {
            var context = new TriggerContext
            {
                Block = this,
                Item = item,
                TriggerType = TriggerType.KeyNextItem
            };
            
            return await ExecuteTriggers(TriggerType.KeyNextItem, context);
        }
        
        /// <summary>
        /// Fire KEY-PREV-ITEM trigger
        /// </summary>
        public async Task<bool> FireKeyPrevItem(IBeepUIComponent item)
        {
            var context = new TriggerContext
            {
                Block = this,
                Item = item,
                TriggerType = TriggerType.KeyPrevItem
            };
            
            return await ExecuteTriggers(TriggerType.KeyPrevItem, context);
        }
        
        #endregion
        
        #region Navigation Trigger Execution
        
        /// <summary>
        /// Fire PRE-RECORD-NAVIGATE trigger
        /// </summary>
        protected async Task<bool> FirePreRecordNavigate(string direction)
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PreRecordNavigate,
                Parameters = new Dictionary<string, object> { ["Direction"] = direction }
            };
            
            return await ExecuteTriggers(TriggerType.PreRecordNavigate, context);
        }
        
        /// <summary>
        /// Fire POST-RECORD-NAVIGATE trigger
        /// </summary>
        protected async Task<bool> FirePostRecordNavigate(string direction)
        {
            var context = new TriggerContext
            {
                Block = this,
                TriggerType = TriggerType.PostRecordNavigate,
                RecordValues = GetCurrentRecordValues(),
                Parameters = new Dictionary<string, object> { ["Direction"] = direction }
            };
            
            return await ExecuteTriggers(TriggerType.PostRecordNavigate, context);
        }
        
        #endregion
        
        #region Trigger Management
        
        /// <summary>
        /// Enable a named trigger
        /// </summary>
        public void EnableTrigger(string triggerName)
        {
            if (_namedTriggers.ContainsKey(triggerName))
            {
                _namedTriggers[triggerName].IsEnabled = true;
            }
        }
        
        /// <summary>
        /// Disable a named trigger
        /// </summary>
        public void DisableTrigger(string triggerName)
        {
            if (_namedTriggers.ContainsKey(triggerName))
            {
                _namedTriggers[triggerName].IsEnabled = false;
            }
        }
        
        /// <summary>
        /// Remove a named trigger
        /// </summary>
        public void RemoveTrigger(string triggerName)
        {
            if (_namedTriggers.ContainsKey(triggerName))
            {
                var trigger = _namedTriggers[triggerName];
                
                if (_triggers.ContainsKey(trigger.TriggerType))
                {
                    _triggers[trigger.TriggerType].Remove(trigger);
                }
                
                _namedTriggers.Remove(triggerName);
            }
        }
        
        /// <summary>
        /// Remove all triggers of a specific type
        /// </summary>
        public void RemoveTriggersOfType(TriggerType type)
        {
            if (_triggers.ContainsKey(type))
            {
                // Remove from named triggers dictionary
                var triggersToRemove = _triggers[type].Where(t => !string.IsNullOrEmpty(t.TriggerName)).ToList();
                foreach (var trigger in triggersToRemove)
                {
                    _namedTriggers.Remove(trigger.TriggerName);
                }
                
                _triggers[type].Clear();
            }
        }
        
        /// <summary>
        /// Clear all triggers
        /// </summary>
        public void ClearAllTriggers()
        {
            _triggers.Clear();
            _namedTriggers.Clear();
        }
        
        /// <summary>
        /// Disable all triggers temporarily
        /// </summary>
        public void DisableAllTriggers()
        {
            _suppressTriggers = true;
        }
        
        /// <summary>
        /// Re-enable all triggers
        /// </summary>
        public void EnableAllTriggers()
        {
            _suppressTriggers = false;
        }
        
        /// <summary>
        /// Get all registered triggers
        /// </summary>
        public List<BeepDataBlockTrigger> GetAllTriggers()
        {
            return _triggers.Values.SelectMany(list => list).ToList();
        }
        
        /// <summary>
        /// Get triggers of a specific type
        /// </summary>
        public List<BeepDataBlockTrigger> GetTriggersOfType(TriggerType type)
        {
            return _triggers.ContainsKey(type) ? _triggers[type].ToList() : new List<BeepDataBlockTrigger>();
        }
        
        /// <summary>
        /// Check if a named trigger exists
        /// </summary>
        public bool HasTrigger(string triggerName)
        {
            return _namedTriggers.ContainsKey(triggerName);
        }
        
        /// <summary>
        /// Get count of triggers for a specific type
        /// </summary>
        public int GetTriggerCount(TriggerType type)
        {
            return _triggers.ContainsKey(type) ? _triggers[type].Count : 0;
        }
        
        /// <summary>
        /// Get total trigger count
        /// </summary>
        public int GetTotalTriggerCount()
        {
            return _triggers.Values.Sum(list => list.Count);
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Get current record values as dictionary (for trigger context)
        /// </summary>
        private Dictionary<string, object> GetCurrentRecordValues()
        {
            var values = new Dictionary<string, object>();
            
            foreach (var component in UIComponents.Values)
            {
                if (!string.IsNullOrEmpty(component.BoundProperty))
                {
                    try
                    {
                        var value = component.GetValue();
                        values[component.BoundProperty] = value;
                    }
                    catch
                    {
                        // Ignore components that can't provide value
                    }
                }
            }
            
            return values;
        }
        
        /// <summary>
        /// Set an item value (helper for triggers)
        /// </summary>
        public void SetItemValue(string fieldName, object value)
        {
            var component = UIComponents.Values.FirstOrDefault(c => c.BoundProperty == fieldName);
            component?.SetValue(value);
        }
        
        /// <summary>
        /// Get an item value (helper for triggers)
        /// </summary>
        public object GetItemValue(string fieldName)
        {
            var component = UIComponents.Values.FirstOrDefault(c => c.BoundProperty == fieldName);
            return component?.GetValue();
        }
        
        #endregion
    }
}

