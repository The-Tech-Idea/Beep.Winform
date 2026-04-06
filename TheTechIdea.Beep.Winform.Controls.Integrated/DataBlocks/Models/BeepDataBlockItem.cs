using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor.UOWManager.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// Extends ItemInfo with UI-specific members for WinForms data blocks.
    /// All data/state properties are inherited from ItemInfo (BeepDM).
    /// </summary>
    public class BeepDataBlockItem : ItemInfo
    {
        #region UI-Specific Properties
        
        /// <summary>
        /// Reference to the UI component
        /// </summary>
        public IBeepUIComponent Component { get; set; }
        
        /// <summary>
        /// Item X coordinate
        /// </summary>
        public int X { get; set; }
        
        /// <summary>
        /// Item Y coordinate
        /// </summary>
        public int Y { get; set; }
        
        /// <summary>
        /// Item width
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// Item height
        /// </summary>
        public int Height { get; set; }
        
        #endregion
        
        #region UI Helper Methods
        
        /// <summary>
        /// Check if property value should be validated
        /// </summary>
        public bool ShouldValidate()
        {
            return Enabled && Visible;
        }
        
        /// <summary>
        /// Check if item can be modified in current mode
        /// </summary>
        public bool CanModify(DataBlockMode mode)
        {
            if (!Enabled || !Visible)
                return false;
                
            return mode switch
            {
                DataBlockMode.Query => QueryAllowed,
                DataBlockMode.CRUD => InsertAllowed || UpdateAllowed,
                _ => false
            };
        }
        
        /// <summary>
        /// Update item state from component
        /// </summary>
        public void UpdateFromComponent()
        {
            if (Component != null)
            {
                OldValue = CurrentValue;
                CurrentValue = Component.GetValue();
                IsDirty = !Equals(OldValue, CurrentValue);
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// Block-level properties (Oracle Forms block properties)
    /// </summary>
    public class BeepDataBlockProperties
    {
        #region Query Properties
        
        /// <summary>
        /// Oracle Forms: WHERE_CLAUSE property
        /// WHERE clause for block query
        /// </summary>
        public string WhereClause { get; set; }
        
        /// <summary>
        /// Oracle Forms: ORDER_BY_CLAUSE property
        /// ORDER BY clause for block query
        /// </summary>
        public string OrderByClause { get; set; }
        
        /// <summary>
        /// Oracle Forms: QUERY_DATA_SOURCE_NAME property
        /// Data source to query from
        /// </summary>
        public string QueryDataSourceName { get; set; }
        
        /// <summary>
        /// Oracle Forms: QUERY_HITS property
        /// Number of records returned by last query
        /// </summary>
        public int QueryHits { get; set; }
        
        /// <summary>
        /// Maximum records to fetch
        /// </summary>
        public int MaxRecords { get; set; } = 1000;
        
        #endregion
        
        #region Block Status
        
        /// <summary>
        /// Oracle Forms: CURRENT_RECORD property
        /// Current record number (1-based)
        /// </summary>
        public int CurrentRecord { get; set; }
        
        /// <summary>
        /// Oracle Forms: RECORDS_DISPLAYED property
        /// Number of records currently displayed
        /// </summary>
        public int RecordsDisplayed { get; set; }
        
        /// <summary>
        /// Block status
        /// </summary>
        public BlockStatus BlockStatus { get; set; } = BlockStatus.Normal;
        
        /// <summary>
        /// Record status
        /// </summary>
        public RecordStatus RecordStatus { get; set; } = RecordStatus.Query;
        
        #endregion
        
        #region Block Behavior
        
        /// <summary>
        /// Whether the block allows inserts
        /// </summary>
        public bool InsertAllowed { get; set; } = true;
        
        /// <summary>
        /// Whether the block allows updates
        /// </summary>
        public bool UpdateAllowed { get; set; } = true;
        
        /// <summary>
        /// Whether the block allows deletes
        /// </summary>
        public bool DeleteAllowed { get; set; } = true;
        
        /// <summary>
        /// Whether the block allows queries
        /// </summary>
        public bool QueryAllowed { get; set; } = true;
        
        /// <summary>
        /// Whether the block is currently locked
        /// </summary>
        public bool IsLocked { get; set; }
        
        #endregion
    }
    
    /// <summary>
    /// Block status enumeration
    /// </summary>
    public enum BlockStatus
    {
        /// <summary>Normal mode - no changes</summary>
        Normal,
        
        /// <summary>Query mode - entering search criteria</summary>
        Query,
        
        /// <summary>Changed - has unsaved changes</summary>
        Changed,
        
        /// <summary>New - creating new record</summary>
        New
    }
    
    /// <summary>
    /// Record status enumeration
    /// </summary>
    public enum RecordStatus
    {
        /// <summary>Record from query</summary>
        Query,
        
        /// <summary>New record being created</summary>
        New,
        
        /// <summary>Existing record being modified</summary>
        Changed,
        
        /// <summary>Record ready for insert</summary>
        Insert
    }
}

