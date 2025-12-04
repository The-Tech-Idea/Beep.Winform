using System;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// Oracle Forms-compatible trigger types
    /// Covers Form, Block, Record, Item, Navigation, and Error triggers
    /// </summary>
    public enum TriggerType
    {
        // ========================================
        // FORM-LEVEL TRIGGERS (6)
        // ========================================
        
        /// <summary>Oracle Forms: WHEN-NEW-FORM-INSTANCE - Form initialization</summary>
        WhenNewFormInstance = 1,
        
        /// <summary>Oracle Forms: PRE-FORM - Before form opens</summary>
        PreForm = 2,
        
        /// <summary>Oracle Forms: POST-FORM - After form closes</summary>
        PostForm = 3,
        
        /// <summary>Oracle Forms: WHEN-FORM-NAVIGATE - Form navigation</summary>
        WhenFormNavigate = 4,
        
        /// <summary>Oracle Forms: PRE-FORM-COMMIT - Before form commit</summary>
        PreFormCommit = 5,
        
        /// <summary>Oracle Forms: POST-FORM-COMMIT - After form commit</summary>
        PostFormCommit = 6,
        
        // ========================================
        // BLOCK-LEVEL TRIGGERS (10)
        // ========================================
        
        /// <summary>Oracle Forms: WHEN-NEW-BLOCK-INSTANCE - Block initialization</summary>
        WhenNewBlockInstance = 100,
        
        /// <summary>Oracle Forms: PRE-BLOCK - Before block operations</summary>
        PreBlock = 101,
        
        /// <summary>Oracle Forms: POST-BLOCK - After block operations</summary>
        PostBlock = 102,
        
        /// <summary>Oracle Forms: WHEN-CLEAR-BLOCK - Block clearing</summary>
        WhenClearBlock = 103,
        
        /// <summary>Oracle Forms: WHEN-CREATE-RECORD - Record creation</summary>
        WhenCreateRecord = 104,
        
        /// <summary>Oracle Forms: WHEN-REMOVE-RECORD - Record removal</summary>
        WhenRemoveRecord = 105,
        
        /// <summary>Oracle Forms: PRE-BLOCK-COMMIT - Before block commit</summary>
        PreBlockCommit = 106,
        
        /// <summary>Oracle Forms: POST-BLOCK-COMMIT - After block commit</summary>
        PostBlockCommit = 107,
        
        /// <summary>Oracle Forms: WHEN-BLOCK-NAVIGATE - Block navigation</summary>
        WhenBlockNavigate = 108,
        
        /// <summary>Oracle Forms: ON-POPULATE-DETAILS - Detail population</summary>
        OnPopulateDetails = 109,
        
        // ========================================
        // RECORD-LEVEL TRIGGERS (15)
        // ========================================
        
        /// <summary>Oracle Forms: WHEN-NEW-RECORD-INSTANCE - New record initialization</summary>
        WhenNewRecordInstance = 200,
        
        /// <summary>Oracle Forms: PRE-INSERT - Before record insert</summary>
        PreInsert = 201,
        
        /// <summary>Oracle Forms: POST-INSERT - After record insert</summary>
        PostInsert = 202,
        
        /// <summary>Oracle Forms: PRE-UPDATE - Before record update</summary>
        PreUpdate = 203,
        
        /// <summary>Oracle Forms: POST-UPDATE - After record update</summary>
        PostUpdate = 204,
        
        /// <summary>Oracle Forms: PRE-DELETE - Before record delete</summary>
        PreDelete = 205,
        
        /// <summary>Oracle Forms: POST-DELETE - After record delete</summary>
        PostDelete = 206,
        
        /// <summary>Oracle Forms: PRE-QUERY - Before query execution</summary>
        PreQuery = 207,
        
        /// <summary>Oracle Forms: POST-QUERY - After query execution</summary>
        PostQuery = 208,
        
        /// <summary>Oracle Forms: WHEN-VALIDATE-RECORD - Record validation</summary>
        WhenValidateRecord = 209,
        
        /// <summary>Oracle Forms: ON-LOCK - Record locking</summary>
        OnLock = 210,
        
        /// <summary>Oracle Forms: ON-CHECK-DELETE-MASTER - Master delete check</summary>
        OnCheckDeleteMaster = 211,
        
        /// <summary>Oracle Forms: ON-CLEAR-DETAILS - Detail clearing</summary>
        OnClearDetails = 212,
        
        /// <summary>Oracle Forms: ON-COUNT-QUERY - Query count</summary>
        OnCountQuery = 213,
        
        /// <summary>Oracle Forms: ON-FETCH-RECORDS - Record fetching</summary>
        OnFetchRecords = 214,
        
        // ========================================
        // ITEM-LEVEL TRIGGERS (12)
        // ========================================
        
        /// <summary>Oracle Forms: WHEN-NEW-ITEM-INSTANCE - Item initialization</summary>
        WhenNewItemInstance = 300,
        
        /// <summary>Oracle Forms: WHEN-VALIDATE-ITEM - Item validation</summary>
        WhenValidateItem = 301,
        
        /// <summary>Oracle Forms: PRE-TEXT-ITEM - Before text change</summary>
        PreTextItem = 302,
        
        /// <summary>Oracle Forms: POST-TEXT-ITEM - After text change</summary>
        PostTextItem = 303,
        
        /// <summary>Oracle Forms: WHEN-LIST-CHANGED - List value change</summary>
        WhenListChanged = 304,
        
        /// <summary>Oracle Forms: KEY-NEXT-ITEM - Tab/Enter key</summary>
        KeyNextItem = 305,
        
        /// <summary>Oracle Forms: KEY-PREV-ITEM - Shift+Tab key</summary>
        KeyPrevItem = 306,
        
        /// <summary>Item receives focus</summary>
        WhenItemFocus = 307,
        
        /// <summary>Item loses focus</summary>
        WhenItemBlur = 308,
        
        /// <summary>Item clicked</summary>
        OnItemClick = 309,
        
        /// <summary>Item double-clicked</summary>
        OnItemDoubleClick = 310,
        
        /// <summary>Item value changed</summary>
        OnItemChange = 311,
        
        // ========================================
        // NAVIGATION TRIGGERS (4)
        // ========================================
        
        /// <summary>Oracle Forms: PRE-RECORD-NAVIGATE - Before record navigation</summary>
        PreRecordNavigate = 400,
        
        /// <summary>Oracle Forms: POST-RECORD-NAVIGATE - After record navigation</summary>
        PostRecordNavigate = 401,
        
        /// <summary>Before block navigation</summary>
        PreBlockNavigate = 402,
        
        /// <summary>After block navigation</summary>
        PostBlockNavigate = 403,
        
        // ========================================
        // ERROR & MESSAGE TRIGGERS (3)
        // ========================================
        
        /// <summary>Oracle Forms: ON-ERROR - Error handling</summary>
        OnError = 500,
        
        /// <summary>Oracle Forms: ON-MESSAGE - Message handling</summary>
        OnMessage = 501,
        
        /// <summary>Database error handling</summary>
        OnDatabaseError = 502,
        
        // ========================================
        // ADDITIONAL TRIGGERS (5)
        // ========================================
        
        /// <summary>Oracle Forms: PRE-ROLLBACK - Before rollback</summary>
        PreBlockRollback = 600,
        
        /// <summary>Oracle Forms: POST-ROLLBACK - After rollback</summary>
        PostBlockRollback = 601,
        
        /// <summary>Before record duplication</summary>
        PreDuplicateRecord = 602,
        
        /// <summary>After record duplication</summary>
        PostDuplicateRecord = 603,
        
        /// <summary>When record status changes</summary>
        OnRecordStatusChange = 604
    }
    
    /// <summary>
    /// Trigger timing classification
    /// </summary>
    public enum TriggerTiming
    {
        /// <summary>Before operation (PRE-*)</summary>
        Before,
        
        /// <summary>After operation (POST-*)</summary>
        After,
        
        /// <summary>During operation (ON-*)</summary>
        On,
        
        /// <summary>Conditional operation (WHEN-*)</summary>
        When
    }
    
    /// <summary>
    /// Trigger scope classification
    /// </summary>
    public enum TriggerScope
    {
        /// <summary>Form-level trigger</summary>
        Form,
        
        /// <summary>Block-level trigger</summary>
        Block,
        
        /// <summary>Record-level trigger</summary>
        Record,
        
        /// <summary>Item/Field-level trigger</summary>
        Item,
        
        /// <summary>Navigation trigger</summary>
        Navigation,
        
        /// <summary>Error/Message trigger</summary>
        System
    }
}

