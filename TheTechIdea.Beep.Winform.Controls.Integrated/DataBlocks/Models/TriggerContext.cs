using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Integrated.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// Context object passed to trigger handlers
    /// Provides access to block, item, values, and control flow
    /// </summary>
    public class TriggerContext
    {
        #region Block & Item Information
        
        /// <summary>
        /// The block that fired this trigger
        /// </summary>
        public IBeepDataBlock Block { get; set; }
        
        /// <summary>
        /// Block name (convenience property)
        /// </summary>
        public string BlockName => Block?.Name;
        
        /// <summary>
        /// The item/component that triggered this (for item-level triggers)
        /// </summary>
        public IBeepUIComponent Item { get; set; }
        
        /// <summary>
        /// Item name (convenience property)
        /// </summary>
        public string ItemName => Item?.ComponentName;
        
        /// <summary>
        /// Field name being validated/changed (for item triggers)
        /// </summary>
        public string FieldName { get; set; }
        
        #endregion
        
        #region Value Information
        
        /// <summary>
        /// Old value (before change) for item triggers
        /// </summary>
        public object OldValue { get; set; }
        
        /// <summary>
        /// New value (after change) for item triggers
        /// </summary>
        public object NewValue { get; set; }
        
        /// <summary>
        /// All field values for the current record (for record-level validation)
        /// </summary>
        public Dictionary<string, object> RecordValues { get; set; } = new Dictionary<string, object>();
        
        #endregion
        
        #region Trigger Information
        
        /// <summary>
        /// The type of trigger being executed
        /// </summary>
        public TriggerType TriggerType { get; set; }
        
        /// <summary>
        /// When the trigger was fired
        /// </summary>
        public DateTime TriggerTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Trigger name (if named trigger)
        /// </summary>
        public string TriggerName { get; set; }
        
        #endregion
        
        #region Control Flow
        
        /// <summary>
        /// Set to true to cancel the operation
        /// </summary>
        public bool Cancel { get; set; }
        
        /// <summary>
        /// Error message to display if validation fails
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Warning messages (operation continues but user is informed)
        /// </summary>
        public List<string> Warnings { get; set; } = new List<string>();
        
        /// <summary>
        /// Info messages (operation continues, informational only)
        /// </summary>
        public List<string> InfoMessages { get; set; } = new List<string>();
        
        #endregion
        
        #region Parameters & Data Passing
        
        /// <summary>
        /// Custom parameters for passing data between triggers
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Execution context data (can be used for caching, etc.)
        /// </summary>
        public Dictionary<string, object> ContextData { get; set; } = new Dictionary<string, object>();
        
        #endregion
        
        #region System Variables Access
        
        /// <summary>
        /// Access to system variables (if block has them)
        /// Returns null if Block doesn't implement system variables yet
        /// </summary>
        public SystemVariables SYSTEM
        {
            get
            {
                if (Block is BeepDataBlock beepBlock)
                {
                    return beepBlock.SYSTEM;
                }
                return null;
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Add a warning message
        /// </summary>
        public void AddWarning(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Warnings.Add(message);
            }
        }
        
        /// <summary>
        /// Add an info message
        /// </summary>
        public void AddInfo(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                InfoMessages.Add(message);
            }
        }
        
        /// <summary>
        /// Set error and cancel operation
        /// </summary>
        public void SetError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Cancel = true;
        }
        
        /// <summary>
        /// Get a parameter value with type casting
        /// </summary>
        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (Parameters.ContainsKey(key) && Parameters[key] is T value)
            {
                return value;
            }
            return defaultValue;
        }
        
        /// <summary>
        /// Set a parameter value
        /// </summary>
        public void SetParameter(string key, object value)
        {
            Parameters[key] = value;
        }
        
        /// <summary>
        /// Get a record value with type casting
        /// </summary>
        public T GetRecordValue<T>(string fieldName, T defaultValue = default)
        {
            if (RecordValues.ContainsKey(fieldName) && RecordValues[fieldName] is T value)
            {
                return value;
            }
            return defaultValue;
        }
        
        #endregion
    }
}

