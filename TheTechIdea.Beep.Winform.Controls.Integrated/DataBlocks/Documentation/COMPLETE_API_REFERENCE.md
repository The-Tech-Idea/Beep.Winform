# 📖 BeepDataBlock Complete API Reference

**Version**: 2.0 (Enterprise Edition)  
**Last Updated**: December 3, 2025

---

## 📚 **QUICK NAVIGATION**

- [Properties](#properties)
- [Methods](#methods)
- [Events](#events)
- [Triggers](#triggers)
- [System Variables](#system-variables)
- [Enums](#enums)

---

## 🔧 **PROPERTIES**

### **Data & Entity**
| Property | Type | Description |
|----------|------|-------------|
| `Data` | `IUnitofWork` | Unit of work for data operations |
| `EntityStructure` | `IEntityStructure` | Entity metadata |
| `EntityName` | `string` | Entity name |
| `SelectedEntityType` | `Type` | Entity type |
| `Fields` | `List<EntityField>` | Entity fields |

### **Block Configuration**
| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Block name (unique identifier) |
| `BlockMode` | `DataBlockMode` | CRUD or Query mode |
| `IsInQueryMode` | `bool` | Whether in query mode |
| `Status` | `string` | Current status message |

### **Master-Detail**
| Property | Type | Description |
|----------|------|-------------|
| `ParentBlock` | `IBeepDataBlock` | Parent block reference |
| `ChildBlocks` | `List<IBeepDataBlock>` | Child blocks |
| `MasterRecord` | `dynamic` | Current master record |
| `MasterKeyPropertyName` | `string` | Master key field |
| `ForeignKeyPropertyName` | `string` | Foreign key field |
| `Relationships` | `List<RelationShipKeys>` | All relationships |

### **Form Coordination** ⭐ NEW
| Property | Type | Description |
|----------|------|-------------|
| `FormManager` | `FormsManager` | FormsManager instance |
| `FormName` | `string` | Form name |
| `DMEEditor` | `IDMEEditor` | DME Editor instance |
| `IsCoordinated` | `bool` | Registered with FormsManager |

### **UI Components**
| Property | Type | Description |
|----------|------|-------------|
| `UIComponents` | `Dictionary<string, IBeepUIComponent>` | All UI components |
| `Components` | `BindingList<BeepComponents>` | Component metadata |

### **System Variables** (Oracle Forms :SYSTEM.*)
| Property | Type | Description |
|----------|------|-------------|
| `SYSTEM` | `SystemVariables` | System variables instance |
| `CurrentRecord` | `int` | Current record number (1-based) |
| `RecordsDisplayed` | `int` | Total records in block |
| `QueryHits` | `int` | Records returned by last query |

### **Locking** ⭐ NEW
| Property | Type | Description |
|----------|------|-------------|
| `LockMode` | `LockMode` | Locking strategy |
| `LockOnEdit` | `bool` | Auto-lock on edit |
| `LockedRecordCount` | `int` | Count of locked records |

### **Error Handling** ⭐ NEW
| Property | Type | Description |
|----------|------|-------------|
| `SuppressErrorDialogs` | `bool` | Suppress MessageBox (for testing) |
| `ErrorLog` | `IReadOnlyList<ErrorInfo>` | Error history |

### **Block Properties** (Oracle Forms block-level)
| Property | Type | Description |
|----------|------|-------------|
| `WhereClause` | `string` | WHERE clause for queries |
| `OrderByClause` | `string` | ORDER BY clause |
| `QueryDataSourceName` | `string` | Data source for queries |
| `InsertAllowed` | `bool` | Block allows inserts |
| `UpdateAllowed` | `bool` | Block allows updates |
| `DeleteAllowed` | `bool` | Block allows deletes |
| `QueryAllowed` | `bool` | Block allows queries |

---

## 🛠️ **METHODS**

### **Initialization**
```csharp
void InitializeIntegrations()
```
One-call initialization for all systems (triggers, LOV, properties, navigation, FormsManager).

---

### **Data Operations**
```csharp
Task<bool> ApplyMasterDetailFilterAsync(CancellationToken ct = default)
Task ExecuteQueryAsync(CancellationToken ct = default)
Task MoveNextAsync()
Task MovePreviousAsync()
Task InsertRecordAsync(Entity newRecord)
Task DeleteCurrentRecordAsync()
Task RollbackAsync()
void UndoLastChange()
```

---

### **Mode Switching**
```csharp
void SwitchBlockMode(DataBlockMode newMode)
Task SwitchBlockModeAsync(DataBlockMode newMode)
void HandleDataChanges()
```

---

### **Trigger Management**
```csharp
void RegisterTrigger(TriggerType type, Func<TriggerContext, Task<bool>> handler, int executionOrder = 0)
void RegisterTrigger(string triggerName, TriggerType type, Func<TriggerContext, Task<bool>> handler, string description = null)
void EnableTrigger(string triggerName)
void DisableTrigger(string triggerName)
void RemoveTrigger(string triggerName)
void RemoveTriggersOfType(TriggerType type)
void ClearAllTriggers()
void DisableAllTriggers()
void EnableAllTriggers()
List<BeepDataBlockTrigger> GetAllTriggers()
List<BeepDataBlockTrigger> GetTriggersOfType(TriggerType type)
bool HasTrigger(string triggerName)
int GetTriggerCount(TriggerType type)
int GetTotalTriggerCount()
```

---

### **Trigger Execution**
```csharp
Task<bool> FireWhenNewFormInstance()
Task<bool> FirePreForm()
Task<bool> FirePostForm()
Task<bool> FireWhenNewBlockInstance()
Task<bool> FireWhenClearBlock()
Task<bool> FireWhenCreateRecord()
Task<bool> FireWhenNewRecordInstance()
Task<bool> FireWhenValidateRecord()
Task<bool> FirePostQuery()
Task<bool> FireWhenValidateItem(IBeepUIComponent item, object oldValue, object newValue)
Task<bool> FirePostTextItem(IBeepUIComponent item, object newValue)
Task<bool> FireKeyNextItem(IBeepUIComponent item)
Task<bool> FireKeyPrevItem(IBeepUIComponent item)
```

---

### **LOV Management**
```csharp
void RegisterLOV(string itemName, BeepDataBlockLOV lov)
void UnregisterLOV(string itemName)
bool HasLOV(string itemName)
BeepDataBlockLOV GetLOV(string itemName)
Task<bool> ShowLOV(string itemName)
Task<bool> ValidateLOVValue(string itemName, object value)
Dictionary<string, BeepDataBlockLOV> GetAllLOVs()
int GetLOVCount()
void ClearAllLOVCaches()
```

---

### **Item Properties**
```csharp
void RegisterItem(string itemName, IBeepUIComponent component)
void RegisterAllItems()
BeepDataBlockItem GetItem(string itemName)
Dictionary<string, BeepDataBlockItem> GetAllItems()
void SetItemProperty(string itemName, string propertyName, object value)
object GetItemProperty(string itemName, string propertyName)
Task SetItemPropertyAsync(string itemName, string propertyName, object value)
void ApplyAllItemProperties()
void ApplyModeBasedProperties()
void ApplyDefaultValues()
bool ValidateRequiredFields(out List<string> errors)
```

---

### **Validation**
```csharp
void RegisterValidationRule(string fieldName, ValidationRule rule)
void RegisterRecordValidationRule(ValidationRule rule)
void UnregisterValidationRules(string fieldName)
List<ValidationRule> GetValidationRules(string fieldName)
Task<IErrorsInfo> ValidateField(string fieldName, object value, CancellationToken ct = default)
Task<IErrorsInfo> ValidateCurrentRecord(CancellationToken ct = default)
void ClearValidationErrors()
List<string> GetFieldsWithErrors()
ValidationRuleBuilder ForField(string fieldName)
```

---

### **Navigation**
```csharp
bool NextItem()
bool PreviousItem()
bool FirstItem()
bool LastItem()
bool GoToItem(string itemName)
void SetupKeyboardNavigation()
```

---

### **Coordination** ⭐ NEW
```csharp
bool RegisterWithFormsManager()
void UnregisterFromFormsManager()
Task<IErrorsInfo> CoordinatedCommit()
Task<IErrorsInfo> CoordinatedRollback()
Task<bool> CoordinatedQuery(List<AppFilter> filters = null)
bool IsBlockReady()
void SyncWithFormsManager()
Task<IErrorsInfo> CoordinatedValidation()
```

---

### **Locking** ⭐ NEW
```csharp
Task<bool> LockCurrentRecord()
Task<bool> LockRecord(int recordIndex)
void UnlockCurrentRecord()
void UnlockRecord(int recordIndex)
void UnlockAllRecords()
bool IsRecordLocked()
bool IsRecordLocked(int recordIndex)
RecordLockInfo GetLockInfo(int recordIndex)
```

---

### **Alerts** ⭐ NEW
```csharp
DialogResult ShowAlert(string title, string message, AlertStyle style = AlertStyle.Information, AlertButtons buttons = AlertButtons.Ok)
bool ConfirmAction(string message)
void ShowInformation(string message)
void ShowWarning(string message)
void ShowError(string message)
```

---

### **Enhanced QBE** ⭐ NEW
```csharp
void SetQueryOperator(string fieldName, QueryOperator op)
QueryOperator GetQueryOperator(string fieldName)
void ClearQueryOperators()
Task<bool> ExecuteEnhancedQuery()
List<AppFilter> GetCurrentQueryFilters()
void SaveQueryTemplate(string name, List<AppFilter> filters, string description = null)
QueryTemplate GetQueryTemplate(string name)
List<QueryTemplate> GetAllQueryTemplates()
void DeleteQueryTemplate(string name)
List<AppFilter> GetLastQuery()
Task<bool> ReExecuteLastQuery()
List<List<AppFilter>> GetQueryHistory(int count = 10)
```

---

### **Savepoints** ⭐ NEW
```csharp
void CreateSavepoint(string name)
Task RollbackToSavepoint(string name)
void ReleaseSavepoint(string name)
List<string> ListSavepoints()
bool HasSavepoint(string name)
void ClearAllSavepoints()
```

---

### **Messages** ⭐ NEW
```csharp
void SetMessage(string text, MessageLevel level = MessageLevel.Info, int? autoClearSeconds = null)
void ClearMessage()
string GetCurrentMessage()
void EnableMessageQueue(int maxSize = 10)
void DisableMessageQueue()
List<MessageInfo> GetMessageHistory()
void ClearMessageQueue()
```

---

### **Error Handling** ⭐ NEW
```csharp
void HandleError(Exception ex, string context, ErrorSeverity severity = ErrorSeverity.Error)
void HandleWarning(string message, string context)
void ClearErrorLog()
string ExportErrorLog()
```

---

### **Performance** ⭐ NEW
```csharp
void EnablePerformanceOptimizations()
void DisablePerformanceOptimizations()
void EnableTriggerCaching()
void EnableLOVCaching()
void EnableValidationDebouncing(int delayMs = 300)
PerformanceStats GetPerformanceStats()
void ResetPerformanceStats()
```

---

### **Helper Methods**
```csharp
void SetItemValue(string fieldName, object value)
object GetItemValue(string fieldName)
Dictionary<string, object> GetCurrentRecordValues()
```

---

## 🎯 **EVENTS**

### **Data Events**
```csharp
event EventHandler<UnitofWorkParams> EventDataChanged
event EventHandler<UnitofWorkParams> OnPreQuery
event EventHandler<UnitofWorkParams> OnPostQuery
event EventHandler<UnitofWorkParams> OnPreInsert
event EventHandler<UnitofWorkParams> OnPostInsert
event EventHandler<UnitofWorkParams> OnPreUpdate
event EventHandler<UnitofWorkParams> OnPostUpdate
event EventHandler<UnitofWorkParams> OnPreDelete
event EventHandler<UnitofWorkParams> OnPostDelete
event EventHandler<UnitofWorkParams> OnValidateRecord
event EventHandler<UnitofWorkParams> OnValidateItem
event EventHandler<UnitofWorkParams> OnNewRecord
event EventHandler<UnitofWorkParams> OnNewBlock
```

### **Block Events**
```csharp
event EventHandler<BeepDataBlockEventArgs> OnAction
event EventHandler<BlockDirtyEventArgs> OnUnsavedChanges
```

### **Error Events** ⭐ NEW
```csharp
event EventHandler<DataBlockErrorEventArgs> OnError
event EventHandler<DataBlockErrorEventArgs> OnWarning
```

### **Message Events** ⭐ NEW
```csharp
event EventHandler<MessageEventArgs> OnMessageChanged
```

---

## 🏛️ **TRIGGERS (50+ TYPES)**

### **Form-Level**
- `PreForm`
- `PostForm`
- `WhenNewFormInstance`

### **Block-Level**
- `PreBlock`
- `PostBlock`
- `WhenNewBlockInstance`
- `WhenClearBlock`
- `WhenCreateRecord`

### **Record-Level**
- `PreQuery`
- `PostQuery`
- `PreInsert`
- `PostInsert`
- `PreUpdate`
- `PostUpdate`
- `PreDelete`
- `PostDelete`
- `WhenNewRecordInstance`
- `WhenValidateRecord`
- `PreRecordNavigate`
- `PostRecordNavigate`

### **Item-Level**
- `WhenValidateItem`
- `PostTextItem`
- `WhenItemFocus`
- `WhenItemBlur`
- `KeyNextItem`
- `KeyPrevItem`

### **Transaction-Level**
- `PreCommit`
- `PostCommit`
- `PreRollback`
- `PostRollback`
- `PreFormCommit`
- `PostFormCommit`

### **Error Handling**
- `OnError`
- `OnMessage`

---

## 📊 **SYSTEM VARIABLES (30+)**

### **Form Context**
```csharp
SYSTEM.CURRENT_FORM
SYSTEM.CURRENT_BLOCK
SYSTEM.CURSOR_BLOCK
SYSTEM.MASTER_BLOCK
```

### **Item Context**
```csharp
SYSTEM.TRIGGER_ITEM
SYSTEM.TRIGGER_FIELD
SYSTEM.CURRENT_ITEM
```

### **Record Context**
```csharp
SYSTEM.CURSOR_RECORD
SYSTEM.LAST_RECORD
SYSTEM.RECORDS_DISPLAYED
SYSTEM.TRIGGER_RECORD
SYSTEM.QUERY_HITS
```

### **Mode & Status**
```csharp
SYSTEM.MODE
SYSTEM.FORM_STATUS
SYSTEM.BLOCK_STATUS
SYSTEM.RECORD_STATUS
```

### **Date & Time**
```csharp
SYSTEM.CURRENT_DATE
SYSTEM.CURRENT_TIME
SYSTEM.CURRENT_DATETIME
SYSTEM.EFFECTIVE_DATE
```

### **User & Environment**
```csharp
SYSTEM.CURRENT_USER
SYSTEM.CURRENT_OS_USER
SYSTEM.CURRENT_OS_NAME
SYSTEM.CURRENT_PRODUCT_VERSION
```

*(See `Models/SystemVariables.cs` for complete list)*

---

## 📋 **ENUMS**

### **DataBlockMode**
```csharp
public enum DataBlockMode
{
    CRUD,   // Normal mode (Create, Read, Update, Delete)
    Query   // Query mode (Enter-Query)
}
```

### **LockMode** ⭐ NEW
```csharp
public enum LockMode
{
    Automatic,  // Lock when editing starts
    Immediate,  // Lock on navigation
    Delayed,    // Lock on commit
    Manual      // Explicit lock calls only
}
```

### **AlertStyle** ⭐ NEW
```csharp
public enum AlertStyle
{
    Information,
    Caution,
    Stop,
    Question
}
```

### **AlertButtons** ⭐ NEW
```csharp
public enum AlertButtons
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel,
    RetryCancel
}
```

### **QueryOperator** ⭐ NEW
```csharp
public enum QueryOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterOrEqual,
    LessOrEqual,
    Like,
    StartsWith,
    EndsWith,
    In,
    Between,
    IsNull,
    IsNotNull
}
```

### **MessageLevel** ⭐ NEW
```csharp
public enum MessageLevel
{
    Info,
    Success,
    Warning,
    Error
}
```

### **ErrorSeverity** ⭐ NEW
```csharp
public enum ErrorSeverity
{
    Information,
    Warning,
    Error,
    Critical
}
```

---

## 📖 **USAGE PATTERNS**

### **Pattern 1: Basic Form**
```csharp
var block = new BeepDataBlock
{
    Name = "CUSTOMERS",
    DMEEditor = dmeEditor
};
block.Data = new UnitofWork<Customer>(dmeEditor);
block.InitializeIntegrations();
```

### **Pattern 2: Master-Detail**
```csharp
var masterBlock = new BeepDataBlock { Name = "MASTER", DMEEditor = dmeEditor };
var detailBlock = new BeepDataBlock 
{ 
    Name = "DETAIL", 
    DMEEditor = dmeEditor,
    ParentBlock = masterBlock,
    MasterKeyPropertyName = "ID",
    ForeignKeyPropertyName = "MasterID"
};

masterBlock.InitializeIntegrations();
detailBlock.InitializeIntegrations();
```

### **Pattern 3: Coordinated Form**
```csharp
var formManager = new FormsManager(dmeEditor);

var block1 = new BeepDataBlock
{
    Name = "BLOCK1",
    FormName = "MyForm",
    FormManager = formManager,
    DMEEditor = dmeEditor
};

var block2 = new BeepDataBlock
{
    Name = "BLOCK2",
    FormName = "MyForm",
    FormManager = formManager,
    DMEEditor = dmeEditor
};

block1.InitializeIntegrations();
block2.InitializeIntegrations();

// Coordinated commit (both blocks)
await block1.CoordinatedCommit();
```

---

## 🔗 **SEE ALSO**

- `ADVANCED_FEATURES_GUIDE.md` - Advanced usage scenarios
- `Examples/` folder - 40+ copy-paste examples
- `ORACLE_FORMS_COMPLETE.md` - Complete feature list
- `TRIGGER_SYSTEM_DESIGN.md` - Trigger architecture
- `VALIDATION_BUSINESS_RULES_DESIGN.md` - Validation design

---

**Complete API reference for BeepDataBlock 2.0 Enterprise Edition!** 📖

