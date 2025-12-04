using System;
using TheTechIdea.Beep.Winform.Controls.Integrated.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// System variables equivalent to Oracle Forms :SYSTEM.* variables
    /// Provides runtime information about block, record, and form state
    /// </summary>
    public class SystemVariables
    {
        private readonly IBeepDataBlock _block;
        
        public SystemVariables(IBeepDataBlock block)
        {
            _block = block ?? throw new ArgumentNullException(nameof(block));
        }
        
        #region Record Information (Oracle Forms :SYSTEM.CURSOR_RECORD, etc.)
        
        /// <summary>Oracle Forms: :SYSTEM.CURSOR_RECORD - Current record number (1-based)</summary>
        public int CURSOR_RECORD { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.LAST_RECORD - Total number of records</summary>
        public int LAST_RECORD { get; set; }
        
        /// <summary>First record in block (1-based)</summary>
        public int FIRST_RECORD => 1;
        
        /// <summary>Whether current record is the first record</summary>
        public bool IS_FIRST_RECORD => CURSOR_RECORD == FIRST_RECORD;
        
        /// <summary>Whether current record is the last record</summary>
        public bool IS_LAST_RECORD => CURSOR_RECORD == LAST_RECORD;
        
        #endregion
        
        #region Block Status (Oracle Forms :SYSTEM.BLOCK_STATUS)
        
        /// <summary>Oracle Forms: :SYSTEM.BLOCK_STATUS - Block status (Normal, Query, Changed, New)</summary>
        public string BLOCK_STATUS { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.RECORD_STATUS - Record status (Query, New, Changed, Insert)</summary>
        public string RECORD_STATUS { get; set; }
        
        /// <summary>Number of records currently displayed in block</summary>
        public int RECORDS_DISPLAYED { get; set; }
        
        /// <summary>Number of records returned by last query</summary>
        public int QUERY_HITS { get; set; }
        
        #endregion
        
        #region Mode Information (Oracle Forms :SYSTEM.MODE)
        
        /// <summary>Oracle Forms: :SYSTEM.MODE - Current mode (CRUD, Query)</summary>
        public string MODE { get; set; }
        
        /// <summary>Whether block is in query mode</summary>
        public bool QUERY_MODE => MODE == "Query";
        
        /// <summary>Whether block is in CRUD mode</summary>
        public bool NORMAL_MODE => MODE == "CRUD";
        
        #endregion
        
        #region Trigger Information (Oracle Forms :SYSTEM.TRIGGER_*)
        
        /// <summary>Oracle Forms: :SYSTEM.TRIGGER_RECORD - Record that triggered the current trigger</summary>
        public int TRIGGER_RECORD { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.TRIGGER_BLOCK - Block that triggered the current trigger</summary>
        public string TRIGGER_BLOCK { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.TRIGGER_ITEM - Item that triggered the current trigger</summary>
        public string TRIGGER_ITEM { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.TRIGGER_FIELD - Field that triggered the current trigger</summary>
        public string TRIGGER_FIELD { get; set; }
        
        #endregion
        
        #region Form/Block Information (Oracle Forms :SYSTEM.CURRENT_*)
        
        /// <summary>Oracle Forms: :SYSTEM.CURRENT_FORM - Current form name</summary>
        public string CURRENT_FORM { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.CURRENT_BLOCK - Current block name</summary>
        public string CURRENT_BLOCK => _block?.Name;
        
        /// <summary>Oracle Forms: :SYSTEM.CURRENT_ITEM - Current item with focus</summary>
        public string CURRENT_ITEM { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.CURRENT_VALUE - Current value of current item</summary>
        public object CURRENT_VALUE { get; set; }
        
        #endregion
        
        #region Message Information (Oracle Forms :SYSTEM.MESSAGE_*)
        
        /// <summary>Oracle Forms: :SYSTEM.MESSAGE_LEVEL - Message level (Error, Warning, Info)</summary>
        public string MESSAGE_LEVEL { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.MESSAGE_CODE - Message code</summary>
        public string MESSAGE_CODE { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.MESSAGE_TEXT - Message text</summary>
        public string MESSAGE_TEXT { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.MESSAGE_SEVERITY - Message severity (0-25)</summary>
        public int MESSAGE_SEVERITY { get; set; }
        
        #endregion
        
        #region Coordination Information
        
        /// <summary>Oracle Forms: :SYSTEM.MASTER_BLOCK - Master block name</summary>
        public string MASTER_BLOCK { get; set; }
        
        /// <summary>Oracle Forms: :SYSTEM.COORDINATION_OPERATION - Whether in coordination operation</summary>
        public bool COORDINATION_OPERATION { get; set; }
        
        /// <summary>Whether block has a parent (is a detail block)</summary>
        public bool HAS_MASTER => _block?.ParentBlock != null;
        
        /// <summary>Whether block has children (is a master block)</summary>
        public bool HAS_DETAILS => _block?.ChildBlocks?.Count > 0;
        
        #endregion
        
        #region Transaction Information
        
        /// <summary>Whether the block has uncommitted changes</summary>
        public bool IS_DIRTY => _block?.Data?.IsDirty ?? false;
        
        /// <summary>Whether in a transaction</summary>
        public bool IN_TRANSACTION { get; set; }
        
        /// <summary>Transaction start time</summary>
        public DateTime? TRANSACTION_START { get; set; }
        
        #endregion
        
        #region Validation State
        
        /// <summary>Whether current record has validation errors</summary>
        public bool HAS_ERRORS { get; set; }
        
        /// <summary>Whether current record has validation warnings</summary>
        public bool HAS_WARNINGS { get; set; }
        
        /// <summary>Number of validation errors in current record</summary>
        public int ERROR_COUNT { get; set; }
        
        /// <summary>Number of validation warnings in current record</summary>
        public int WARNING_COUNT { get; set; }
        
        #endregion
        
        #region Navigation State
        
        /// <summary>Last navigation direction (First, Next, Previous, Last)</summary>
        public string LAST_NAVIGATION { get; set; }
        
        /// <summary>Whether currently navigating</summary>
        public bool IS_NAVIGATING { get; set; }
        
        #endregion
        
        #region Timestamp Information
        
        /// <summary>When the block was loaded</summary>
        public DateTime? BLOCK_LOADED_TIME { get; set; }
        
        /// <summary>When the current record was loaded</summary>
        public DateTime? RECORD_LOADED_TIME { get; set; }
        
        /// <summary>When the last operation was performed</summary>
        public DateTime? LAST_OPERATION_TIME { get; set; }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Update all system variables based on current block state
        /// Should be called after every operation
        /// </summary>
        public void UpdateAll()
        {
            if (_block == null)
                return;
                
            // Update block information (CURRENT_BLOCK is read-only, computed from _block.Name)
            MASTER_BLOCK = _block.ParentBlock?.Name;
            
            // Update mode
            MODE = _block.BlockMode.ToString();
            
            // Update record counts
            RECORDS_DISPLAYED = _block.Data?.Units?.Count ?? 0;
            LAST_RECORD = RECORDS_DISPLAYED;
            
            // Update timestamp
            LAST_OPERATION_TIME = DateTime.Now;
        }
        
        /// <summary>
        /// Set message variables
        /// </summary>
        public void SetMessage(string level, string code, string text, int severity = 0)
        {
            MESSAGE_LEVEL = level;
            MESSAGE_CODE = code;
            MESSAGE_TEXT = text;
            MESSAGE_SEVERITY = severity;
        }
        
        /// <summary>
        /// Clear message variables
        /// </summary>
        public void ClearMessages()
        {
            MESSAGE_LEVEL = null;
            MESSAGE_CODE = null;
            MESSAGE_TEXT = null;
            MESSAGE_SEVERITY = 0;
        }
        
        /// <summary>
        /// Set error message
        /// </summary>
        public void SetError(string code, string text)
        {
            SetMessage("Error", code, text, 25);
        }
        
        /// <summary>
        /// Set warning message
        /// </summary>
        public void SetWarning(string code, string text)
        {
            SetMessage("Warning", code, text, 15);
        }
        
        /// <summary>
        /// Set info message
        /// </summary>
        public void SetInfo(string code, string text)
        {
            SetMessage("Info", code, text, 5);
        }
        
        #endregion
    }
}

