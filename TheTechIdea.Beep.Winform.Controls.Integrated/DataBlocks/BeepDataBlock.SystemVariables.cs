using System;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - System Variables
    /// Provides Oracle Forms :SYSTEM.* equivalent variables
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Fields
        
        /// <summary>
        /// System variables instance (lazy initialization)
        /// </summary>
        private SystemVariables _systemVariables;
        
        /// <summary>
        /// Block ID for identification
        /// </summary>
        public string BlockID { get; set; }
        
        /// <summary>
        /// Whether this is a master block
        /// </summary>
        public bool IsMasterBlock => ParentBlock == null;
        
        #endregion
        
        #region System Variables Access
        
        /// <summary>
        /// Oracle Forms :SYSTEM.* equivalent
        /// Access system variables like: block.SYSTEM.CURSOR_RECORD, block.SYSTEM.MODE, etc.
        /// </summary>
        public SystemVariables SYSTEM
        {
            get
            {
                if (_systemVariables == null)
                {
                    _systemVariables = new SystemVariables(this);
                    UpdateSystemVariables();
                }
                return _systemVariables;
            }
        }
        
        #endregion
        
        #region System Variable Updates
        
        /// <summary>
        /// Update system variables based on current block state
        /// Should be called after every significant operation
        /// </summary>
        public void UpdateSystemVariables()
        {
            if (_systemVariables == null)
                return;
                
            // Update block information (CURRENT_BLOCK is read-only, computed from block.Name)
            SYSTEM.MASTER_BLOCK = this.ParentBlock?.Name;
            
            // Update mode
            SYSTEM.MODE = this.BlockMode.ToString();
            
            // Update record information
            var units = Data?.Units;
            if (units != null)
            {
                SYSTEM.CURSOR_RECORD = units.CurrentIndex + 1;  // 1-based
                SYSTEM.LAST_RECORD = units.Count;
                SYSTEM.RECORDS_DISPLAYED = units.Count;
            }
            else
            {
                SYSTEM.CURSOR_RECORD = 0;
                SYSTEM.LAST_RECORD = 0;
                SYSTEM.RECORDS_DISPLAYED = 0;
            }
            
            // Update status
            SYSTEM.BLOCK_STATUS = this.IsInQueryMode ? "Query" : 
                                 (Data?.IsDirty == true ? "Changed" : "Normal");
            
            // Update timestamp
            SYSTEM.LAST_OPERATION_TIME = DateTime.Now;
        }
        
        /// <summary>
        /// Update system variables for trigger execution
        /// </summary>
        protected void UpdateSystemVariablesForTrigger(TriggerType triggerType, string itemName = null)
        {
            if (_systemVariables == null)
                return;
                
            SYSTEM.TRIGGER_BLOCK = this.Name;
            SYSTEM.TRIGGER_RECORD = SYSTEM.CURSOR_RECORD;
            
            if (!string.IsNullOrEmpty(itemName))
            {
                SYSTEM.TRIGGER_ITEM = itemName;
                SYSTEM.TRIGGER_FIELD = itemName;  // Assuming item name = field name
            }
            
            UpdateSystemVariables();
        }
        
        #endregion
        
        #region Property Helpers (for System Variables)
        
        /// <summary>
        /// Current record number (1-based, Oracle Forms compatible)
        /// </summary>
        public int CurrentRecord
        {
            get => SYSTEM?.CURSOR_RECORD ?? 0;
            set
            {
                if (SYSTEM != null)
                {
                    SYSTEM.CURSOR_RECORD = value;
                }
            }
        }
        
        /// <summary>
        /// Total records in block
        /// </summary>
        public int RecordsDisplayed => SYSTEM?.RECORDS_DISPLAYED ?? 0;
        
        /// <summary>
        /// Number of records returned by last query
        /// </summary>
        public int QueryHits
        {
            get => SYSTEM?.QUERY_HITS ?? 0;
            set
            {
                if (SYSTEM != null)
                {
                    SYSTEM.QUERY_HITS = value;
                }
            }
        }
        
        #endregion
    }
}

