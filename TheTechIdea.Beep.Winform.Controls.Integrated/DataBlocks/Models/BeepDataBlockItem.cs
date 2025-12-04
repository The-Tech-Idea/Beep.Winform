using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor.UOWManager.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// Represents an item/field in a BeepDataBlock with Oracle Forms-compatible properties
    /// </summary>
    public class BeepDataBlockItem
    {
        #region Basic Properties
        
        /// <summary>
        /// Item name (matches component name)
        /// </summary>
        public string ItemName { get; set; }
        
        /// <summary>
        /// Bound property name (database field)
        /// </summary>
        public string BoundProperty { get; set; }
        
        /// <summary>
        /// Reference to the UI component
        /// </summary>
        public IBeepUIComponent Component { get; set; }
        
        #endregion
        
        #region Oracle Forms Item Properties
        
        /// <summary>
        /// Oracle Forms: REQUIRED property
        /// Whether the item must have a value before record can be saved
        /// </summary>
        public bool Required { get; set; }
        
        /// <summary>
        /// Oracle Forms: ENABLED property
        /// Whether the item is enabled (can be edited)
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Oracle Forms: VISIBLE property
        /// Whether the item is visible
        /// </summary>
        public bool Visible { get; set; } = true;
        
        /// <summary>
        /// Oracle Forms: QUERY_ALLOWED property
        /// Whether the item can be used in query mode
        /// </summary>
        public bool QueryAllowed { get; set; } = true;
        
        /// <summary>
        /// Oracle Forms: INSERT_ALLOWED property
        /// Whether the item can be modified during insert
        /// </summary>
        public bool InsertAllowed { get; set; } = true;
        
        /// <summary>
        /// Oracle Forms: UPDATE_ALLOWED property
        /// Whether the item can be modified during update
        /// </summary>
        public bool UpdateAllowed { get; set; } = true;
        
        /// <summary>
        /// Oracle Forms: DEFAULT_VALUE property
        /// Default value for new records
        /// </summary>
        public object DefaultValue { get; set; }
        
        /// <summary>
        /// Oracle Forms: PROMPT_TEXT property
        /// Label text for the item
        /// </summary>
        public string PromptText { get; set; }
        
        /// <summary>
        /// Oracle Forms: HINT_TEXT property
        /// Tooltip/hint text for the item
        /// </summary>
        public string HintText { get; set; }
        
        /// <summary>
        /// Oracle Forms: LOV_NAME property
        /// Name of the LOV attached to this item
        /// </summary>
        public string LOVName { get; set; }
        
        /// <summary>
        /// Oracle Forms: MAX_LENGTH property
        /// Maximum length for text items
        /// </summary>
        public int MaxLength { get; set; }
        
        /// <summary>
        /// Oracle Forms: FORMAT_MASK property
        /// Display format mask
        /// </summary>
        public string FormatMask { get; set; }
        
        /// <summary>
        /// Validation formula/expression
        /// </summary>
        public string ValidationFormula { get; set; }
        
        /// <summary>
        /// Validation rules for this item (string-based for now)
        /// </summary>
        public List<string> ValidationRules { get; set; } = new List<string>();
        
        #endregion
        
        #region Item State
        
        /// <summary>
        /// Whether the item value has changed
        /// </summary>
        public bool IsDirty { get; set; }
        
        /// <summary>
        /// Old value (before change)
        /// </summary>
        public object OldValue { get; set; }
        
        /// <summary>
        /// Current value
        /// </summary>
        public object CurrentValue { get; set; }
        
        /// <summary>
        /// Whether the item is currently focused
        /// </summary>
        public bool HasFocus { get; set; }
        
        /// <summary>
        /// Whether the item has validation errors
        /// </summary>
        public bool HasError { get; set; }
        
        /// <summary>
        /// Current error message
        /// </summary>
        public string ErrorMessage { get; set; }
        
        #endregion
        
        #region Navigation Properties
        
        /// <summary>
        /// Tab index for keyboard navigation
        /// </summary>
        public int TabIndex { get; set; }
        
        /// <summary>
        /// Next item in navigation order
        /// </summary>
        public string NextNavigationItem { get; set; }
        
        /// <summary>
        /// Previous item in navigation order
        /// </summary>
        public string PreviousNavigationItem { get; set; }
        
        #endregion
        
        #region Display Properties
        
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
        
        #region Helper Methods
        
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

