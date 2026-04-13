# Forms Enhanced Data Operations Reference

This reference demonstrates robust end-to-end CRUD, LOV usage, undo/redo, and batch export using enhanced operations in `FormsManager`.

## Scenario A: Insert + Update + Query Roundtrip

```csharp
public static async Task RunCrudRoundtripAsync(IDMEEditor editor)
{
    var forms = new FormsManager(editor);

    using var orderUow = new UnitofWork<Order>(editor, "MyDb", "Orders", "Id");
    forms.RegisterBlock<Order>("ORDERS", orderUow, "MyDb", isMasterBlock: true);

    await forms.OpenFormAsync("OrderForm");

    // Enter CRUD-ready state for new record
    var enterCrud = await forms.EnterCrudModeForNewRecordAsync("ORDERS");
    if (enterCrud.Flag != Errors.Ok) throw new InvalidOperationException(enterCrud.Message);

    // Create and fill record
    var newRecord = (Order)forms.CreateNewRecord("ORDERS");
    newRecord.Status = "NEW";
    forms.ApplyAuditDefaults(newRecord, Environment.UserName);

    var insert = await forms.InsertRecordEnhancedAsync("ORDERS", newRecord);
    if (insert.Flag != Errors.Ok) throw new InvalidOperationException(insert.Message);

    // Query with filters
    var filters = new List<AppFilter>
    {
        new AppFilter { FieldName = "Status", Operator = "=", FilterValue = "NEW" }
    };
    var query = await forms.ExecuteQueryEnhancedAsync("ORDERS", filters);
    Console.WriteLine($"Records: {forms.GetRecordCount("ORDERS")}");

    await forms.CommitFormAsync();
    await forms.CloseFormAsync();
}
```

## Scenario B: Typed Registration + InsertRecordAsync<T>

```csharp
public static async Task TypedInsertAsync(IDMEEditor editor)
{
    var forms = new FormsManager(editor);
    using var uow = new UnitofWork<Customer>(editor, "MyDb", "Customers", "Id");

    // Typed registration stamps EntityType on DataBlockInfo
    forms.RegisterBlock<Customer>("CUSTOMERS", uow, customerEntity, "MyDb", isMasterBlock: true);

    // CreateNewRecord returns a Customer instance, not ExpandoObject
    var customer = (Customer)forms.CreateNewRecord("CUSTOMERS");
    customer.Name = "Acme Corp";

    // Shorthand: create default T and insert in one call
    await forms.InsertRecordAsync<Customer>("CUSTOMERS", customer);
}
```

## Scenario C: LOV Selection

```csharp
// Register a LOV on the Customers block for the Country field
forms.LOV.RegisterLOV("COUNTRY_LOV", new LOVDefinition
{
    DataSourceName = "MyDb",
    EntityName     = "Countries",
    DisplayField   = "CountryName",
    ReturnField    = "CountryCode",
    RelatedFields  = new[] { ("Capital", "CapitalCity") }
});
forms.LOV.BindLOVToField("CUSTOMERS", "CountryCode", "COUNTRY_LOV");

// Show LOV dialog and apply selection (including related fields) to current record
var selected = await forms.ShowLOVAsync("CUSTOMERS", "CountryCode");
```

## Scenario D: Undo / Redo + Change Summary

```csharp
// Requires backing UoW to implement IUndoable
forms.SetBlockUndoEnabled("ORDERS", true, maxDepth: 20);

// ... make changes ...
var summary = forms.GetBlockChangeSummary("ORDERS");
Console.WriteLine($"Added={summary.AddedCount}, Changed={summary.ChangedCount}");

if (forms.CanUndoBlock("ORDERS"))
    forms.UndoBlock("ORDERS");

var allSummaries = forms.GetFormChangeSummary();
```

## Scenario E: Batch Commit + Export

```csharp
var progress = new Progress<PassedArgs>(p => Console.WriteLine(p.Messege));
var batchResult = await forms.BatchCommitAsync(
    new[] { "CUSTOMERS", "ORDERS", "ORDER_LINES" },
    progress);

if (batchResult.Flag == Errors.Ok)
    await forms.ExportBlockDataAsync("ORDERS", ExportFormat.Json, "./orders-backup.json");
```

## Scenario F: Copy Fields Between Records

```csharp
var source = forms.GetCurrentRecord("ORDERS");
var target = forms.CreateNewRecord("ORDERS");
if (target != null)
{
    forms.CopyFields(source, target, "CustomerId", "OrderDate", "CurrencyCode");
    forms.ApplyAuditDefaults(target, Environment.UserName);
}
```

## Notes
- `InsertRecordAsync<T>` is shorthand for `CreateNewRecord` + `InsertRecordEnhancedAsync`.
- Always inspect `IErrorsInfo` and treat `Warning` distinctly from `Failed`.
- `CopyFields` requires an explicit field list; never rely on order-based copy.
- `BatchCommitAsync` stops on the first failure unless configured to continue.

