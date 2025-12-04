# 🚀 BeepDataBlock Advanced Features Guide

**Audience**: Experienced developers building complex multi-block forms  
**Prerequisites**: Familiarity with basic BeepDataBlock usage

---

## 📚 **TABLE OF CONTENTS**

1. [Record Locking](#record-locking)
2. [Alert System](#alert-system)
3. [Enhanced Query-by-Example](#enhanced-qbe)
4. [Transactional Savepoints](#savepoints)
5. [Message System](#message-system)
6. [Error Handling](#error-handling)
7. [Performance Optimization](#performance)
8. [Multi-Block Coordination](#coordination)
9. [Advanced Scenarios](#advanced-scenarios)

---

## 🔒 **RECORD LOCKING**

### **Overview**
Oracle Forms `LOCK_RECORD` equivalent for pessimistic locking.

### **Lock Modes**
```csharp
public enum LockMode
{
    Automatic,  // Lock when user starts editing
    Immediate,  // Lock on record navigation
    Delayed,    // Lock only on commit
    Manual      // Explicit lock calls only
}
```

### **Basic Usage**
```csharp
// Set lock mode
customerBlock.LockMode = LockMode.Automatic;
customerBlock.LockOnEdit = true;

// Manual locking
await customerBlock.LockCurrentRecord();

// Check lock status
if (customerBlock.IsRecordLocked())
{
    MessageBox.Show("Record is locked for editing");
}

// Unlock
customerBlock.UnlockCurrentRecord();

// Get all locked records
var lockedCount = customerBlock.LockedRecordCount;
```

### **Advanced Locking**
```csharp
// Lock with timeout
await customerBlock.LockCurrentRecord(timeoutSeconds: 30);

// Lock specific record
await customerBlock.LockRecord(recordIndex: 5);

// Unlock all
customerBlock.UnlockAllRecords();

// Handle lock conflicts
customerBlock.OnError += (s, e) =>
{
    if (e.Context == "RecordLock" && e.Severity == ErrorSeverity.Warning)
    {
        // Record locked by another user
        var result = MessageBox.Show(
            "Record is locked by another user. Wait and retry?",
            "Lock Conflict",
            MessageBoxButtons.YesNo);
            
        if (result == DialogResult.Yes)
        {
            // Retry logic
        }
        
        e.Handled = true;
    }
};
```

---

## 🚨 **ALERT SYSTEM**

### **Overview**
Oracle Forms `SHOW_ALERT` equivalent with modern styling.

### **Alert Styles**
```csharp
public enum AlertStyle
{
    Information,  // Blue icon
    Caution,      // Yellow warning
    Stop,         // Red error
    Question      // Question mark
}

public enum AlertButtons
{
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel,
    RetryCancel
}
```

### **Basic Usage**
```csharp
// Simple alert
customerBlock.ShowAlert("Record saved successfully!", "Success", 
    AlertStyle.Information);

// Confirmation
var result = customerBlock.ShowAlert(
    "Delete this customer and all orders?",
    "Confirm Delete",
    AlertStyle.Caution,
    AlertButtons.YesNo);
    
if (result == DialogResult.Yes)
{
    await customerBlock.DeleteCurrentRecordAsync();
}
```

### **Quick Alerts**
```csharp
// Yes/No confirmation
if (customerBlock.ConfirmAction("Save changes?"))
{
    await customerBlock.CoordinatedCommit();
}

// Information
customerBlock.ShowInformation("Query returned 150 records");

// Warning
customerBlock.ShowWarning("Some fields are empty");

// Error
customerBlock.ShowError("Failed to connect to database");
```

### **Custom Alerts with Triggers**
```csharp
// Register trigger to intercept alerts
customerBlock.RegisterTrigger(TriggerType.OnMessage, async context =>
{
    var message = context.Parameters["Message"] as string;
    var style = context.Parameters["Style"];
    
    // Custom logic (e.g., log to database)
    await LogMessageToDatabase(message, style);
    
    // Allow alert to show
    return true;
});
```

---

## 🔍 **ENHANCED QUERY-BY-EXAMPLE**

### **Overview**
Advanced QBE beyond Oracle Forms' basic Enter-Query mode.

### **Query Operators**
```csharp
public enum QueryOperator
{
    Equals,           // =
    NotEquals,        // !=
    GreaterThan,      // >
    LessThan,         // <
    GreaterOrEqual,   // >=
    LessOrEqual,      // <=
    Like,             // LIKE %value%
    StartsWith,       // LIKE value%
    EndsWith,         // LIKE %value
    In,               // IN (val1, val2, ...)
    Between,          // BETWEEN val1 AND val2
    IsNull,           // IS NULL
    IsNotNull         // IS NOT NULL
}
```

### **Basic Usage**
```csharp
// Enter query mode
await customerBlock.SwitchBlockModeAsync(DataBlockMode.Query);

// Set operators for fields
customerBlock.SetQueryOperator("Salary", QueryOperator.GreaterThan);
customerBlock.SetQueryOperator("Name", QueryOperator.Like);
customerBlock.SetQueryOperator("City", QueryOperator.In);

// User enters values in UI controls
// Salary: 50000
// Name: Smith
// City: New York, Boston, Chicago

// Execute enhanced query
await customerBlock.ExecuteEnhancedQuery();
// Generates: WHERE Salary > 50000 AND Name LIKE '%Smith%' 
//            AND City IN ('New York', 'Boston', 'Chicago')
```

### **Query Templates**
```csharp
// Save current query as template
var filters = customerBlock.GetCurrentQueryFilters();
customerBlock.SaveQueryTemplate("HighEarners", filters);

// Load query template
var template = customerBlock.GetQueryTemplate("HighEarners");
await customerBlock.ExecuteQueryFromTemplate(template);

// List all templates
var templates = customerBlock.GetAllQueryTemplates();
foreach (var tmpl in templates)
{
    Console.WriteLine($"{tmpl.Name}: {tmpl.Description}");
}

// Delete template
customerBlock.DeleteQueryTemplate("HighEarners");
```

### **Query History**
```csharp
// Get last query
var lastQuery = customerBlock.GetLastQuery();

// Re-execute last query
await customerBlock.ReExecuteLastQuery();

// Get query history
var history = customerBlock.GetQueryHistory(count: 10);
```

---

## 💾 **TRANSACTIONAL SAVEPOINTS**

### **Overview**
Database savepoints for nested transaction control.

### **Basic Usage**
```csharp
// Start transaction
await customerBlock.CoordinatedQuery();

// Create savepoint before risky operation
customerBlock.CreateSavepoint("BeforeDelete");

try
{
    // Delete some records
    await customerBlock.DeleteCurrentRecordAsync();
    await customerBlock.MoveNextAsync();
    await customerBlock.DeleteCurrentRecordAsync();
    
    // Commit if successful
    await customerBlock.CoordinatedCommit();
}
catch (Exception ex)
{
    // Rollback to savepoint (undo deletes, keep other changes)
    await customerBlock.RollbackToSavepoint("BeforeDelete");
}
finally
{
    // Clean up savepoint
    customerBlock.ReleaseSavepoint("BeforeDelete");
}
```

### **Nested Savepoints**
```csharp
// Outer savepoint
customerBlock.CreateSavepoint("Level1");

// Do some work
await customerBlock.InsertRecordAsync(newCustomer);

// Inner savepoint
customerBlock.CreateSavepoint("Level2");

// More work
await customerBlock.UpdateCurrentAsync();

// Rollback inner only
await customerBlock.RollbackToSavepoint("Level2");

// Level1 changes still intact
await customerBlock.CoordinatedCommit();
```

### **Savepoint Management**
```csharp
// List all savepoints
var savepoints = customerBlock.ListSavepoints();

// Check if savepoint exists
if (customerBlock.HasSavepoint("BeforeDelete"))
{
    await customerBlock.RollbackToSavepoint("BeforeDelete");
}

// Clear all savepoints
customerBlock.ClearAllSavepoints();
```

---

## 💬 **MESSAGE SYSTEM**

### **Overview**
Status/message line like Oracle Forms message area.

### **Message Levels**
```csharp
public enum MessageLevel
{
    Info,       // Blue
    Success,    // Green
    Warning,    // Yellow
    Error       // Red
}
```

### **Basic Usage**
```csharp
// Show message
customerBlock.SetMessage("Loading records...", MessageLevel.Info);

// Auto-clear after 3 seconds
customerBlock.SetMessage("Saved!", MessageLevel.Success, autoClearSeconds: 3);

// Clear manually
customerBlock.ClearMessage();

// Get current message
var currentMessage = customerBlock.GetCurrentMessage();
```

### **Message Queue**
```csharp
// Enable message queue (show multiple messages)
customerBlock.EnableMessageQueue(maxSize: 10);

// Messages queue up
customerBlock.SetMessage("Loading...", MessageLevel.Info);
customerBlock.SetMessage("Validating...", MessageLevel.Info);
customerBlock.SetMessage("Saving...", MessageLevel.Info);

// Get message history
var history = customerBlock.GetMessageHistory();

// Clear queue
customerBlock.ClearMessageQueue();
```

### **Custom Message Display**
```csharp
// Subscribe to message events
customerBlock.OnMessageChanged += (s, e) =>
{
    // Update custom status bar
    statusLabel.Text = e.Message;
    statusLabel.ForeColor = e.Level switch
    {
        MessageLevel.Error => Color.Red,
        MessageLevel.Warning => Color.Orange,
        MessageLevel.Success => Color.Green,
        _ => Color.Black
    };
};
```

---

## 🎯 **ERROR HANDLING**

### **Overview**
Centralized, event-driven error handling for testability and flexibility.

### **Basic Usage**
```csharp
// Subscribe to errors
customerBlock.OnError += (s, e) =>
{
    // Log to file
    Logger.LogError(e.Exception, e.Context);
    
    // Show custom UI
    CustomErrorDialog.Show(e);
    
    // Mark as handled (prevents default MessageBox)
    e.Handled = true;
};

// Subscribe to warnings
customerBlock.OnWarning += (s, e) =>
{
    // Show toast notification instead of MessageBox
    ToastNotification.Show(e.Message, ToastType.Warning);
    e.Handled = true;
};
```

### **Error Severity**
```csharp
public enum ErrorSeverity
{
    Information,  // FYI only
    Warning,      // Potential issue
    Error,        // Operation failed
    Critical      // System-level failure
}
```

### **Error Logging**
```csharp
// Get error log
var errors = customerBlock.ErrorLog;

foreach (var error in errors)
{
    Console.WriteLine($"[{error.Timestamp}] {error.Severity}: {error.Message}");
    Console.WriteLine($"   Context: {error.Context}");
    Console.WriteLine($"   Exception: {error.Exception?.GetType().Name}");
}

// Clear error log
customerBlock.ClearErrorLog();

// Export error log
var errorReport = customerBlock.ExportErrorLog();
File.WriteAllText("errors.json", errorReport);
```

### **Testing with Suppressed Dialogs**
```csharp
[Test]
public async Task TestValidation()
{
    // Suppress MessageBox for unit tests
    customerBlock.SuppressErrorDialogs = true;
    
    // Subscribe to errors
    DataBlockErrorEventArgs capturedError = null;
    customerBlock.OnError += (s, e) => capturedError = e;
    
    // Trigger error
    await customerBlock.ValidateField("Email", "invalid");
    
    // Assert
    Assert.IsNotNull(capturedError);
    Assert.AreEqual(ErrorSeverity.Error, capturedError.Severity);
}
```

---

## ⚡ **PERFORMANCE OPTIMIZATION**

### **Overview**
20-30% performance improvement through caching and optimization.

### **Enable Optimizations**
```csharp
// Enable all optimizations
customerBlock.EnablePerformanceOptimizations();

// Optimizations include:
// - Trigger lookup caching
// - LOV data lazy-loading
// - Validation debouncing
// - SystemVariables update optimization
```

### **Selective Optimization**
```csharp
// Enable specific optimizations
customerBlock.EnableTriggerCaching();
customerBlock.EnableLOVCaching();
customerBlock.EnableValidationDebouncing(delayMs: 300);
```

### **Performance Monitoring**
```csharp
// Get performance stats
var stats = customerBlock.GetPerformanceStats();

Console.WriteLine($"Trigger cache hits: {stats.TriggerCacheHits}");
Console.WriteLine($"LOV cache hits: {stats.LOVCacheHits}");
Console.WriteLine($"Avg trigger execution: {stats.AvgTriggerExecutionMs}ms");
Console.WriteLine($"Avg validation: {stats.AvgValidationMs}ms");

// Reset stats
customerBlock.ResetPerformanceStats();
```

### **Disable for Debugging**
```csharp
// Disable optimizations when debugging
customerBlock.DisablePerformanceOptimizations();

// This ensures:
// - All triggers execute fresh
// - All LOVs load fresh data
// - All validations execute immediately
// - All SystemVariables update every time
```

---

## 🔗 **MULTI-BLOCK COORDINATION**

### **Overview**
Coordinating multiple BeepDataBlocks in a single form.

### **3-Level Master-Detail**
```csharp
public class OrderEntryForm : Form
{
    private FormsManager _formManager;
    private BeepDataBlock customerBlock;    // Level 1: Master
    private BeepDataBlock ordersBlock;      // Level 2: Detail
    private BeepDataBlock orderDetailsBlock; // Level 3: Detail-Detail
    
    private void InitializeBlocks()
    {
        _formManager = new FormsManager(DMEEditor);
        
        // Level 1: Customers (Master)
        customerBlock = new BeepDataBlock
        {
            Name = "CUSTOMERS",
            FormName = "OrderEntry",
            FormManager = _formManager,
            DMEEditor = DMEEditor
        };
        customerBlock.Data = new UnitofWork<Customer>(DMEEditor);
        
        // Level 2: Orders (Detail of Customers)
        ordersBlock = new BeepDataBlock
        {
            Name = "ORDERS",
            FormName = "OrderEntry",
            FormManager = _formManager,
            DMEEditor = DMEEditor,
            ParentBlock = customerBlock,
            MasterKeyPropertyName = "CustomerID",
            ForeignKeyPropertyName = "CustomerID"
        };
        ordersBlock.Data = new UnitofWork<Order>(DMEEditor);
        
        // Level 3: Order Details (Detail of Orders)
        orderDetailsBlock = new BeepDataBlock
        {
            Name = "ORDER_DETAILS",
            FormName = "OrderEntry",
            FormManager = _formManager,
            DMEEditor = DMEEditor,
            ParentBlock = ordersBlock,
            MasterKeyPropertyName = "OrderID",
            ForeignKeyPropertyName = "OrderID"
        };
        orderDetailsBlock.Data = new UnitofWork<OrderDetail>(DMEEditor);
        
        // Initialize all
        customerBlock.InitializeIntegrations();
        ordersBlock.InitializeIntegrations();
        orderDetailsBlock.InitializeIntegrations();
        
        // Setup navigation between blocks
        SetupInterBlockNavigation();
    }
    
    private void SetupInterBlockNavigation()
    {
        // When customer changes, refresh orders
        customerBlock.Data.Units.CurrentChanged += async (s, e) =>
        {
            await ordersBlock.ApplyMasterDetailFilterAsync();
        };
        
        // When order changes, refresh order details
        ordersBlock.Data.Units.CurrentChanged += async (s, e) =>
        {
            await orderDetailsBlock.ApplyMasterDetailFilterAsync();
        };
    }
    
    private async void btnSave_Click(object sender, EventArgs e)
    {
        // Coordinated commit (all 3 levels)
        var result = await customerBlock.CoordinatedCommit();
        
        if (result.Flag == Errors.Ok)
        {
            customerBlock.SetMessage("All changes saved!", MessageLevel.Success);
        }
    }
}
```

---

## 🎯 **ADVANCED SCENARIOS**

### **Scenario 1: Cross-Block Validation**
```csharp
// Validate that order total doesn't exceed customer credit limit
ordersBlock.RegisterTrigger(TriggerType.PreInsert, async context =>
{
    var orderTotal = (decimal)ordersBlock.GetItemValue("OrderTotal");
    var customerId = ordersBlock.GetItemValue("CustomerID");
    
    // Get customer credit limit from master block
    var customerRecord = customerBlock.Data.Units.Current;
    var creditLimit = GetPropertyValue(customerRecord, "CreditLimit");
    
    if (orderTotal > creditLimit)
    {
        context.Cancel = true;
        context.ErrorMessage = $"Order total ({orderTotal:C}) exceeds " +
                              $"customer credit limit ({creditLimit:C})";
        return false;
    }
    
    return true;
});
```

### **Scenario 2: Cascading LOVs**
```csharp
// Country LOV
customerBlock.RegisterLOV("CountryID", new BeepDataBlockLOV
{
    LOVName = "COUNTRIES",
    Title = "Select Country",
    DataSourceName = "MainDB",
    EntityName = "Countries",
    DisplayField = "CountryName",
    ReturnField = "CountryID"
});

// State LOV (filtered by selected country)
customerBlock.RegisterLOV("StateID", new BeepDataBlockLOV
{
    LOVName = "STATES",
    Title = "Select State",
    DataSourceName = "MainDB",
    EntityName = "States",
    DisplayField = "StateName",
    ReturnField = "StateID",
    // Dynamic filter based on CountryID
    Filters = new List<AppFilter>
    {
        new AppFilter
        {
            FieldName = "CountryID",
            Operator = "=",
            FilterValue = () => customerBlock.GetItemValue("CountryID")?.ToString()
        }
    }
});

// When country changes, clear state
customerBlock.RegisterTrigger(TriggerType.PostTextItem, async context =>
{
    if (context.FieldName == "CountryID")
    {
        customerBlock.SetItemValue("StateID", null);
        customerBlock.ClearLOVCache("StateID");  // Refresh state LOV
    }
    return true;
});
```

### **Scenario 3: Conditional Required Fields**
```csharp
// Make "ShipDate" required only if "Status" is "Shipped"
ordersBlock.RegisterTrigger(TriggerType.WhenValidateRecord, async context =>
{
    var status = ordersBlock.GetItemValue("Status")?.ToString();
    var shipDate = ordersBlock.GetItemValue("ShipDate");
    
    if (status == "Shipped" && shipDate == null)
    {
        context.Cancel = true;
        context.ErrorMessage = "Ship Date is required when Status is Shipped";
        
        // Highlight the field
        ordersBlock.SetItemProperty("ShipDate", "HasError", true);
        ordersBlock.SetItemProperty("ShipDate", "ErrorMessage", 
            "Required for Shipped orders");
        
        return false;
    }
    
    return true;
});
```

### **Scenario 4: Audit Trail with Triggers**
```csharp
// Auto-populate audit fields
customerBlock.RegisterTrigger(TriggerType.PreInsert, async context =>
{
    customerBlock.SetItemValue("CreatedBy", Environment.UserName);
    customerBlock.SetItemValue("CreatedDate", DateTime.Now);
    return true;
});

customerBlock.RegisterTrigger(TriggerType.PreUpdate, async context =>
{
    customerBlock.SetItemValue("ModifiedBy", Environment.UserName);
    customerBlock.SetItemValue("ModifiedDate", DateTime.Now);
    return true;
});
```

### **Scenario 5: Complex Business Rules**
```csharp
// Order must have at least one order detail
ordersBlock.RegisterTrigger(TriggerType.PreFormCommit, async context =>
{
    var orderDetailsCount = orderDetailsBlock.Data?.Units?.Count ?? 0;
    
    if (orderDetailsCount == 0)
    {
        context.Cancel = true;
        context.ErrorMessage = "Order must have at least one line item";
        
        // Show alert
        ordersBlock.ShowAlert(
            "Please add at least one product to the order",
            "Validation Error",
            AlertStyle.Caution);
        
        // Navigate to order details block
        orderDetailsBlock.FirstItem();
        
        return false;
    }
    
    return true;
});
```

---

## 🧪 **TESTING PATTERNS**

### **Unit Testing with Mocks**
```csharp
[TestClass]
public class BeepDataBlockTests
{
    [TestMethod]
    public async Task TestTriggerExecution()
    {
        // Arrange
        var mockUOW = new Mock<IUnitofWork>();
        var block = new BeepDataBlock
        {
            Data = mockUOW.Object,
            SuppressErrorDialogs = true
        };
        
        bool triggerFired = false;
        block.RegisterTrigger(TriggerType.PreInsert, async context =>
        {
            triggerFired = true;
            return true;
        });
        
        // Act
        await block.FirePreInsert();
        
        // Assert
        Assert.IsTrue(triggerFired);
    }
}
```

### **Integration Testing**
```csharp
[TestMethod]
public async Task TestMasterDetailCoordination()
{
    // Arrange
    var formManager = new FormsManager(DMEEditor);
    var masterBlock = CreateTestBlock("MASTER", formManager);
    var detailBlock = CreateTestBlock("DETAIL", formManager, masterBlock);
    
    // Act
    await masterBlock.CoordinatedQuery();
    await masterBlock.MoveNextAsync();
    
    // Assert
    Assert.AreEqual(masterBlock.CurrentRecord, detailBlock.MasterRecord);
}
```

---

## 🎓 **BEST PRACTICES**

### **1. Always Use Coordinated Operations**
```csharp
// ✅ GOOD: Coordinated commit
await customerBlock.CoordinatedCommit();

// ❌ BAD: Direct commit (bypasses FormsManager)
await customerBlock.Data.Commit();
```

### **2. Initialize Integrations**
```csharp
// ✅ GOOD: One-call initialization
customerBlock.InitializeIntegrations();

// ❌ BAD: Manual setup (error-prone)
customerBlock.RegisterAllItems();
customerBlock.SetupKeyboardNavigation();
// ... (easy to forget steps)
```

### **3. Use Async Throughout**
```csharp
// ✅ GOOD: Proper async
await customerBlock.CoordinatedQuery();

// ❌ BAD: Blocking (deadlock risk!)
customerBlock.CoordinatedQuery().Wait();
```

### **4. Handle Errors Gracefully**
```csharp
// ✅ GOOD: Subscribe to OnError
customerBlock.OnError += HandleError;

// ❌ BAD: Let exceptions bubble up
// (users see ugly error dialogs)
```

### **5. Use Cancellation Tokens**
```csharp
// ✅ GOOD: Cancellable operations
var cts = new CancellationTokenSource();
btnCancel.Click += (s, e) => cts.Cancel();
await customerBlock.ExecuteQueryAsync(filters, cts.Token);

// ❌ BAD: No cancellation (unresponsive UI)
await customerBlock.ExecuteQueryAsync(filters);
```

---

## 📖 **FURTHER READING**

- `ORACLE_FORMS_COMPLETE.md` - Complete Oracle Forms feature list
- `TRIGGER_SYSTEM_DESIGN.md` - Trigger architecture
- `VALIDATION_BUSINESS_RULES_DESIGN.md` - Validation design
- `Examples/` folder - 40+ copy-paste examples

---

**🏛️ Master the advanced features and build enterprise-grade forms!** 🚀

