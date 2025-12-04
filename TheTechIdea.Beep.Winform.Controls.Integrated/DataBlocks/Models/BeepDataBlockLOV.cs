using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// List of Values (LOV) definition for BeepDataBlock
    /// Oracle Forms-compatible LOV system
    /// </summary>
    public class BeepDataBlockLOV
    {
        #region Basic Properties
        
        /// <summary>
        /// Unique LOV name
        /// </summary>
        public string LOVName { get; set; }
        
        /// <summary>
        /// LOV dialog title
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Data source name to query LOV data from
        /// </summary>
        public string DataSourceName { get; set; }
        
        /// <summary>
        /// Entity/table name to query
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// Field to display in the LOV (visible to user)
        /// </summary>
        public string DisplayField { get; set; }
        
        /// <summary>
        /// Field to return to the calling item (actual value)
        /// </summary>
        public string ReturnField { get; set; }
        
        #endregion
        
        #region Column Configuration
        
        /// <summary>
        /// Columns to display in the LOV grid
        /// </summary>
        public List<LOVColumn> Columns { get; set; } = new List<LOVColumn>();
        
        #endregion
        
        #region Filtering & Sorting
        
        /// <summary>
        /// Additional filters to apply to LOV query
        /// </summary>
        public List<AppFilter> Filters { get; set; } = new List<AppFilter>();
        
        /// <summary>
        /// WHERE clause for LOV query (alternative to Filters)
        /// </summary>
        public string WhereClause { get; set; }
        
        /// <summary>
        /// ORDER BY clause for LOV query
        /// </summary>
        public string OrderByClause { get; set; }
        
        /// <summary>
        /// Whether to allow user to filter/search LOV data
        /// </summary>
        public bool AllowSearch { get; set; } = true;
        
        /// <summary>
        /// Search mode (Contains, StartsWith, EndsWith)
        /// </summary>
        public LOVSearchMode SearchMode { get; set; } = LOVSearchMode.Contains;
        
        #endregion
        
        #region Display Properties
        
        /// <summary>
        /// LOV dialog width
        /// </summary>
        public int Width { get; set; } = 600;
        
        /// <summary>
        /// LOV dialog height
        /// </summary>
        public int Height { get; set; } = 400;
        
        /// <summary>
        /// Whether to allow multiple row selection
        /// </summary>
        public bool AllowMultiSelect { get; set; }
        
        /// <summary>
        /// Whether to show row numbers in LOV grid
        /// </summary>
        public bool ShowRowNumbers { get; set; } = true;
        
        /// <summary>
        /// Whether to auto-size columns
        /// </summary>
        public bool AutoSizeColumns { get; set; } = true;
        
        #endregion
        
        #region Behavior Properties
        
        /// <summary>
        /// Whether to refresh LOV data each time it's opened
        /// </summary>
        public bool AutoRefresh { get; set; } = true;
        
        /// <summary>
        /// Validation type for the LOV
        /// </summary>
        public LOVValidationType ValidationType { get; set; } = LOVValidationType.ListOnly;
        
        /// <summary>
        /// Whether to auto-drop down LOV on field entry (like combo box)
        /// </summary>
        public bool AutoDisplay { get; set; }
        
        /// <summary>
        /// Minimum characters before auto-display (if AutoDisplay = true)
        /// </summary>
        public int AutoDisplayMinChars { get; set; } = 2;
        
        /// <summary>
        /// Whether to automatically populate related fields (like Oracle Forms)
        /// Example: Selecting customer also populates customer name, address, etc.
        /// </summary>
        public bool AutoPopulateRelatedFields { get; set; } = true;
        
        /// <summary>
        /// Related field mappings (LOV field → Block field)
        /// Example: "CompanyName" → "CustomerName", "Phone" → "CustomerPhone"
        /// </summary>
        public Dictionary<string, string> RelatedFieldMappings { get; set; } = new Dictionary<string, string>();
        
        #endregion
        
        #region Cache Properties
        
        /// <summary>
        /// Whether to cache LOV data in memory
        /// </summary>
        public bool UseCache { get; set; } = true;
        
        /// <summary>
        /// Cache duration (minutes, 0 = no expiration)
        /// </summary>
        public int CacheDurationMinutes { get; set; } = 30;
        
        /// <summary>
        /// Cached data (internal use)
        /// </summary>
        internal List<object> CachedData { get; set; }
        
        /// <summary>
        /// Cache timestamp (internal use)
        /// </summary>
        internal DateTime? CacheTimestamp { get; set; }
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Fired before LOV is displayed (can cancel)
        /// </summary>
        public event EventHandler<LOVEventArgs> BeforeDisplay;
        
        /// <summary>
        /// Fired after LOV selection is made
        /// </summary>
        public event EventHandler<LOVEventArgs> AfterSelection;
        
        /// <summary>
        /// Fired when LOV is cancelled
        /// </summary>
        public event EventHandler<LOVEventArgs> OnCancel;
        
        internal void OnBeforeDisplay(LOVEventArgs args) => BeforeDisplay?.Invoke(this, args);
        internal void OnAfterSelection(LOVEventArgs args) => AfterSelection?.Invoke(this, args);
        internal void OnLOVCancel(LOVEventArgs args) => OnCancel?.Invoke(this, args);
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Check if cache is valid
        /// </summary>
        public bool IsCacheValid()
        {
            if (!UseCache || CachedData == null || !CacheTimestamp.HasValue)
                return false;
                
            if (CacheDurationMinutes == 0)
                return true;  // No expiration
                
            return (DateTime.Now - CacheTimestamp.Value).TotalMinutes < CacheDurationMinutes;
        }
        
        /// <summary>
        /// Clear cached data
        /// </summary>
        public void ClearCache()
        {
            CachedData = null;
            CacheTimestamp = null;
        }
        
        #endregion
    }
    
    /// <summary>
    /// LOV column definition
    /// </summary>
    public class LOVColumn
    {
        /// <summary>
        /// Field name in the entity
        /// </summary>
        public string FieldName { get; set; }
        
        /// <summary>
        /// Display name in LOV grid header
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Column width in pixels
        /// </summary>
        public int Width { get; set; } = 100;
        
        /// <summary>
        /// Whether column is visible
        /// </summary>
        public bool Visible { get; set; } = true;
        
        /// <summary>
        /// Whether this column is searchable
        /// </summary>
        public bool Searchable { get; set; } = true;
        
        /// <summary>
        /// Column format (for dates, numbers, etc.)
        /// </summary>
        public string Format { get; set; }
        
        /// <summary>
        /// Column alignment
        /// </summary>
        public LOVColumnAlignment Alignment { get; set; } = LOVColumnAlignment.Left;
    }
    
    /// <summary>
    /// LOV validation type (how user input is validated)
    /// </summary>
    public enum LOVValidationType
    {
        /// <summary>
        /// Oracle Forms: Validate From List = Yes
        /// User MUST select from LOV, cannot type custom value
        /// </summary>
        ListOnly,
        
        /// <summary>
        /// Oracle Forms: Validate From List = No
        /// User can type any value, LOV is optional
        /// </summary>
        Unrestricted,
        
        /// <summary>
        /// User can type value, but it must match a value in the LOV
        /// </summary>
        Validated
    }
    
    /// <summary>
    /// LOV search mode
    /// </summary>
    public enum LOVSearchMode
    {
        /// <summary>Search anywhere in the string</summary>
        Contains,
        
        /// <summary>Search at start of string</summary>
        StartsWith,
        
        /// <summary>Search at end of string</summary>
        EndsWith,
        
        /// <summary>Exact match</summary>
        Exact
    }
    
    /// <summary>
    /// LOV column alignment
    /// </summary>
    public enum LOVColumnAlignment
    {
        Left,
        Center,
        Right
    }
    
    /// <summary>
    /// Event args for LOV events
    /// </summary>
    public class LOVEventArgs : EventArgs
    {
        /// <summary>
        /// The LOV that fired the event
        /// </summary>
        public BeepDataBlockLOV LOV { get; set; }
        
        /// <summary>
        /// Selected value(s)
        /// </summary>
        public List<object> SelectedValues { get; set; } = new List<object>();
        
        /// <summary>
        /// Selected record (full object)
        /// </summary>
        public object SelectedRecord { get; set; }
        
        /// <summary>
        /// Search text entered by user
        /// </summary>
        public string SearchText { get; set; }
        
        /// <summary>
        /// Whether to cancel the operation
        /// </summary>
        public bool Cancel { get; set; }
        
        /// <summary>
        /// Custom data
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
    }
}

