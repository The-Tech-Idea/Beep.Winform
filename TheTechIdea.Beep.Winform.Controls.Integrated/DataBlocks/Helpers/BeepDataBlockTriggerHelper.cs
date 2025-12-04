using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Helpers
{
    /// <summary>
    /// Helper class for trigger-related operations
    /// </summary>
    public static class BeepDataBlockTriggerHelper
    {
        #region Trigger Statistics
        
        /// <summary>
        /// Get trigger execution statistics for a block
        /// </summary>
        public static TriggerStatistics GetTriggerStatistics(BeepDataBlock block)
        {
            var allTriggers = block.GetAllTriggers();
            
            return new TriggerStatistics
            {
                TotalTriggers = allTriggers.Count,
                EnabledTriggers = allTriggers.Count(t => t.IsEnabled),
                DisabledTriggers = allTriggers.Count(t => !t.IsEnabled),
                TotalExecutions = allTriggers.Sum(t => t.ExecutionCount),
                TotalCancellations = allTriggers.Sum(t => t.CancellationCount),
                TotalErrors = allTriggers.Sum(t => t.ErrorCount),
                AverageExecutionMs = allTriggers.Any() ? allTriggers.Average(t => t.AverageExecutionMs) : 0,
                TriggersByType = allTriggers.GroupBy(t => t.TriggerType).ToDictionary(g => g.Key, g => g.Count()),
                MostExecutedTrigger = allTriggers.OrderByDescending(t => t.ExecutionCount).FirstOrDefault()
            };
        }
        
        #endregion
        
        #region Trigger Scope Helpers
        
        /// <summary>
        /// Get all form-level triggers
        /// </summary>
        public static List<BeepDataBlockTrigger> GetFormLevelTriggers(BeepDataBlock block)
        {
            return block.GetAllTriggers().Where(t => t.Scope == TriggerScope.Form).ToList();
        }
        
        /// <summary>
        /// Get all block-level triggers
        /// </summary>
        public static List<BeepDataBlockTrigger> GetBlockLevelTriggers(BeepDataBlock block)
        {
            return block.GetAllTriggers().Where(t => t.Scope == TriggerScope.Block).ToList();
        }
        
        /// <summary>
        /// Get all record-level triggers
        /// </summary>
        public static List<BeepDataBlockTrigger> GetRecordLevelTriggers(BeepDataBlock block)
        {
            return block.GetAllTriggers().Where(t => t.Scope == TriggerScope.Record).ToList();
        }
        
        /// <summary>
        /// Get all item-level triggers
        /// </summary>
        public static List<BeepDataBlockTrigger> GetItemLevelTriggers(BeepDataBlock block)
        {
            return block.GetAllTriggers().Where(t => t.Scope == TriggerScope.Item).ToList();
        }
        
        #endregion
        
        #region Field Value Helpers
        
        /// <summary>
        /// Get field value from record using reflection
        /// </summary>
        public static object GetFieldValue(object record, string fieldName)
        {
            if (record == null || string.IsNullOrEmpty(fieldName))
                return null;
                
            try
            {
                var property = record.GetType().GetProperty(fieldName, 
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    
                return property?.GetValue(record);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Set field value on record using reflection
        /// </summary>
        public static bool SetFieldValue(object record, string fieldName, object value)
        {
            if (record == null || string.IsNullOrEmpty(fieldName))
                return false;
                
            try
            {
                var property = record.GetType().GetProperty(fieldName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    
                if (property != null && property.CanWrite)
                {
                    // Convert value to property type if needed
                    var convertedValue = ConvertValue(value, property.PropertyType);
                    property.SetValue(record, convertedValue);
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null;
                
            if (targetType.IsAssignableFrom(value.GetType()))
                return value;
                
            try
            {
                // Handle nullable types
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }
                
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return value;
            }
        }
        
        #endregion
        
        #region Common Trigger Patterns
        
        /// <summary>
        /// Create an audit trail trigger (sets Created/Modified fields)
        /// </summary>
        public static void RegisterAuditTriggers(BeepDataBlock block, string username = null)
        {
            username = username ?? Environment.UserName;
            
            // PRE-INSERT: Set created audit fields
            block.RegisterTrigger(TriggerType.PreInsert, async (context) =>
            {
                if (context.Block is BeepDataBlock beepBlock)
                {
                    beepBlock.SetItemValue("CreatedDate", DateTime.Now);
                    beepBlock.SetItemValue("CreatedBy", username);
                    beepBlock.SetItemValue("ModifiedDate", DateTime.Now);
                    beepBlock.SetItemValue("ModifiedBy", username);
                }
                return true;
            }, executionOrder: 10);
            
            // PRE-UPDATE: Set modified audit fields
            block.RegisterTrigger(TriggerType.PreUpdate, async (context) =>
            {
                if (context.Block is BeepDataBlock beepBlock)
                {
                    beepBlock.SetItemValue("ModifiedDate", DateTime.Now);
                    beepBlock.SetItemValue("ModifiedBy", username);
                    
                    // Increment version if exists
                    var currentVersion = context.GetRecordValue<int>("Version", 0);
                    if (currentVersion > 0)
                    {
                        beepBlock.SetItemValue("Version", currentVersion + 1);
                    }
                }
                
                return true;
            }, executionOrder: 10);
        }
        
        /// <summary>
        /// Create a default value trigger for new records
        /// </summary>
        public static void RegisterDefaultValueTrigger(BeepDataBlock block, 
            Dictionary<string, object> defaultValues)
        {
            block.RegisterTrigger(TriggerType.WhenNewRecordInstance, async (context) =>
            {
                if (context.Block is BeepDataBlock beepBlock)
                {
                    foreach (var kvp in defaultValues)
                    {
                        beepBlock.SetItemValue(kvp.Key, kvp.Value);
                    }
                }
                return true;
            });
        }
        
        /// <summary>
        /// Create a computed field trigger
        /// </summary>
        public static void RegisterComputedFieldTrigger(BeepDataBlock block,
            string resultField, string[] sourceFields, Func<Dictionary<string, object>, object> computation)
        {
            // Register for each source field change
            block.RegisterTrigger(TriggerType.WhenValidateItem, async (context) =>
            {
                if (sourceFields.Contains(context.FieldName) && context.Block is BeepDataBlock beepBlock)
                {
                    var result = computation(context.RecordValues);
                    beepBlock.SetItemValue(resultField, result);
                }
                return true;
            });
            
            // Register for POST-QUERY to compute on load
            block.RegisterTrigger(TriggerType.PostQuery, async (context) =>
            {
                if (context.Block is BeepDataBlock beepBlock)
                {
                    var result = computation(context.RecordValues);
                    beepBlock.SetItemValue(resultField, result);
                }
                return true;
            });
        }
        
        #endregion
        
        #region Trigger Templates
        
        /// <summary>
        /// Register standard CRUD triggers for a block
        /// </summary>
        public static void RegisterStandardCRUDTriggers(BeepDataBlock block)
        {
            // Register audit triggers
            RegisterAuditTriggers(block);
            
            // POST-QUERY: Coordinate detail blocks
            block.RegisterTrigger(TriggerType.PostQuery, async (context) =>
            {
                // This will be called after query
                // Details should be coordinated automatically
                return true;
            });
            
            // WHEN-VALIDATE-RECORD: Basic validation
            block.RegisterTrigger(TriggerType.WhenValidateRecord, async (context) =>
            {
                // This can be extended with custom validation
                return true;
            });
        }
        
        #endregion
    }
    
    /// <summary>
    /// Trigger execution statistics
    /// </summary>
    public class TriggerStatistics
    {
        public int TotalTriggers { get; set; }
        public int EnabledTriggers { get; set; }
        public int DisabledTriggers { get; set; }
        public int TotalExecutions { get; set; }
        public int TotalCancellations { get; set; }
        public int TotalErrors { get; set; }
        public double AverageExecutionMs { get; set; }
        public Dictionary<TriggerType, int> TriggersByType { get; set; }
        public BeepDataBlockTrigger MostExecutedTrigger { get; set; }
        
        public override string ToString()
        {
            return $"Triggers: {TotalTriggers} (Enabled: {EnabledTriggers}, Disabled: {DisabledTriggers})\n" +
                   $"Executions: {TotalExecutions} (Cancelled: {TotalCancellations}, Errors: {TotalErrors})\n" +
                   $"Average Duration: {AverageExecutionMs:F2}ms";
        }
    }
}

