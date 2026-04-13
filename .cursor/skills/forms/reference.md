# Forms Management Quick Reference

## Initialization (typed blocks + security + audit)

```csharp
var forms = new FormsManager(dmeEditor)
{
    CurrentFormName = "CUSTOMER_ORDERS"
};

// Security context
forms.SetSecurityContext(new SecurityContext { UserName = "alice", Roles = new[] { "admin" } });

// Audit user
forms.SetAuditUser("alice");
```

## Block Registration

```csharp
// Typed registration (preferred)
forms.RegisterBlock<Customer>("CUSTOMERS", customerUow, customerEntity, "Northwind", isMasterBlock: true);
forms.RegisterBlock<Order>("ORDERS", orderUow, orderEntity, "Northwind");

// Master-detail relationship
forms.CreateMasterDetailRelation("CUSTOMERS", "ORDERS", "Id", "CustomerId");
```

## Mode Transitions

```csharp
// Enter query mode
await forms.EnterQueryModeAsync("CUSTOMERS");

// Execute query and enter CRUD mode
await forms.ExecuteQueryAndEnterCrudModeAsync("CUSTOMERS", filters);

// New record in CRUD mode
await forms.EnterCrudModeForNewRecordAsync("CUSTOMERS");
var record = forms.CreateNewRecord("CUSTOMERS");
```

## Form Lifecycle

```csharp
await forms.OpenFormAsync("CustomerOrderForm");
await forms.CommitFormAsync();        // COMMIT_FORM
await forms.RollbackFormAsync();      // ROLLBACK_FORM
await forms.ClearAllBlocksAsync();    // CLEAR_FORM
await forms.CloseFormAsync();
```

## Navigation

```csharp
await forms.FirstRecordAsync("CUSTOMERS");
await forms.NextRecordAsync("CUSTOMERS");
await forms.PreviousRecordAsync("CUSTOMERS");
await forms.LastRecordAsync("CUSTOMERS");
await forms.NavigateToRecordAsync("CUSTOMERS", 3);
var info = forms.GetCurrentRecordInfo("CUSTOMERS");
```

## Oracle Forms Built-in Navigation

```csharp
forms.GoBlock("ORDERS");              // GO_BLOCK
forms.GoRecord("CUSTOMERS", 2);       // GO_RECORD
forms.GoItem("CUSTOMERS", "Email");   // GO_ITEM
forms.NextBlock();                    // NEXT_BLOCK
forms.PreviousBlock();                // PREVIOUS_BLOCK
```

## Multi-Form Navigation

```csharp
// Modal call (suspends current form)
await forms.CallFormAsync("ORDER_DETAIL", new Dictionary<string, object> { ["orderId"] = 42 });

// Modeless open
await forms.OpenFormAsync("AUDIT_VIEWER");

// Close and open replacement
await forms.NewFormAsync("CUSTOMER_SEARCH");

// Return from called form
await forms.ReturnToCallerAsync(selectedRecord);
```

## Inter-Form Communication

```csharp
forms.SetGlobalVariable("SELECTED_CUSTOMER_ID", 99); // :GLOBAL.SELECTED_CUSTOMER_ID
var val = forms.GetGlobalVariable("SELECTED_CUSTOMER_ID");
await forms.PostMessageAsync("ORDER_FORM", "CustomerChanged", 99);
forms.OnFormMessage += (s, e) => Console.WriteLine(e.MessageType);
```

## Undo / Redo

```csharp
forms.SetBlockUndoEnabled("CUSTOMERS", true, maxDepth: 20);
forms.UndoBlock("CUSTOMERS");
forms.RedoBlock("CUSTOMERS");
var summary = forms.GetBlockChangeSummary("CUSTOMERS");
```

## Paging

```csharp
forms.SetBlockPageSize("ORDERS", 50);
forms.SetTotalRecordCount("ORDERS", totalFromQuery);
var pageInfo = await forms.LoadPageAsync("ORDERS", pageNumber: 2);
Console.WriteLine($"Page {pageInfo.PageNumber} of {pageInfo.TotalPages}");
```

## Audit

```csharp
forms.ConfigureAudit(cfg => { cfg.Enabled = true; cfg.FlushIntervalSeconds = 30; });
var log = forms.GetAuditLog(blockName: "CUSTOMERS", from: DateTime.Today);
await forms.ExportAuditToJsonAsync("./audit.json");
```

## Security

```csharp
forms.SetBlockSecurity("ORDERS", new BlockSecurity
{
    AllowInsert = false,
    AllowUpdate = true,
    AllowDelete = false,
    RequiredRoles = new[] { "sales" }
});
forms.SetFieldSecurity("CUSTOMERS", "CreditCard", new FieldSecurity { Masked = true });
bool canInsert = forms.IsBlockAllowed("ORDERS", SecurityPermission.Insert);
```

## Alerts and Messages

```csharp
var result = await forms.ShowAlertAsync("Confirm", "Delete record?", AlertStyle.Caution, "Yes", "No");
if (result == AlertResult.Button2) return;
forms.SetMessage("Record saved", MessageLevel.Info);
forms.ClearMessage();
```

## Savepoints and Locking

```csharp
forms.Savepoints.CreateSavepoint("CUSTOMERS", "before-batch");
// ... batch edits ...
forms.Savepoints.RestoreSavepoint("CUSTOMERS", "before-batch");

forms.Locking.LockCurrentRecord("CUSTOMERS");
forms.Locking.UnlockCurrentRecord("CUSTOMERS");
```
