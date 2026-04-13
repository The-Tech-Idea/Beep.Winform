# Forms Helper Managers Reference

This reference demonstrates end-to-end helper manager usage covering triggers, validation, LOV, savepoints, audit, and security.

## Scenario A: Triggered Master-Detail Form With Validation

```csharp
public static async Task ConfigureHelpersAndRunAsync(IDMEEditor editor)
{
    var forms = new FormsManager(editor);

    using var customerUow = new UnitofWork<Customer>(editor, "MyDb", "Customers", "Id");
    using var orderUow = new UnitofWork<Order>(editor, "MyDb", "Orders", "Id");
    forms.RegisterBlock<Customer>("CUSTOMERS", customerUow, "MyDb", isMasterBlock: true);
    forms.RegisterBlock<Order>("ORDERS", orderUow, "MyDb");
    forms.CreateMasterDetailRelation("CUSTOMERS", "ORDERS", "Id", "CustomerId");

    // ValidationManager — fluent item rule
    forms.Validation.AddItemRule("CUSTOMERS", "Email", value =>
        string.IsNullOrWhiteSpace(value?.ToString())
            ? ValidationResult.Fail("Email is required")
            : ValidationResult.Ok());

    // TriggerManager — use TriggerLibrary factory
    forms.Triggers.RegisterBlockTrigger(
        "CUSTOMERS",
        TriggerType.PreInsert,
        TriggerLibrary.AuditStamp(Environment.UserName));

    // DirtyStateManager — auto-save policy
    forms.OnUnsavedChanges += async (s, e) =>
    {
        e.UserChoice = UnsavedChangesAction.Save;
        await Task.CompletedTask;
    };

    await forms.OpenFormAsync("CustomerOrderForm");
    await forms.ExecuteQueryAndEnterCrudModeAsync("CUSTOMERS");
    await forms.SynchronizeDetailBlocksAsync("CUSTOMERS");
    await forms.CloseFormAsync();
}
```

## Scenario B: LOV Registration + Cascading LOV

```csharp
// Register primary LOV
forms.LOV.RegisterLOV("COUNTRY_LOV", new LOVDefinition
{
    DataSourceName = "MyDb",
    EntityName = "Countries",
    DisplayField = "CountryName",
    ReturnField = "CountryCode",
    RelatedFields = new[] { ("Capital", "CapitalCity") }
});
forms.LOV.BindLOVToField("CUSTOMERS", "CountryCode", "COUNTRY_LOV");

// Register dependent cascading LOV
forms.LOV.RegisterLOV("CITY_LOV", new LOVDefinition
{
    DataSourceName = "MyDb",
    EntityName = "Cities",
    DisplayField = "CityName",
    ReturnField = "CityCode",
    ParentLOV = "COUNTRY_LOV",
    ParentField = "CountryCode"
});
forms.LOV.BindLOVToField("CUSTOMERS", "CityCode", "CITY_LOV");

// Show LOV and populate related fields automatically
var selected = await forms.ShowLOVAsync("CUSTOMERS", "CountryCode");
```

## Scenario C: Savepoints and Cross-Block Validation

```csharp
// Named savepoint before a batch of changes
forms.Savepoints.CreateSavepoint("ORDERS", "before-discount-run");

foreach (var order in pendingOrders)
{
    await forms.UpdateCurrentRecordAsync("ORDERS");
    // ... apply discount logic ...
}

// If cross-block rule violations, restore
var crossResult = forms.CrossBlockValidation.Validate();
if (crossResult.HasErrors)
{
    forms.Savepoints.RestoreSavepoint("ORDERS", "before-discount-run");
}
else
{
    forms.Savepoints.ClearSavepoints("ORDERS");
    await forms.CommitFormAsync();
}
```

## Scenario D: Audit and Security Together

```csharp
// Security context
forms.SetSecurityContext(new SecurityContext { UserName = "bob", Roles = new[] { "sales" } });
forms.SetBlockSecurity("CUSTOMERS", new BlockSecurity { AllowDelete = false });
forms.SetFieldSecurity("CUSTOMERS", "CreditLimit", new FieldSecurity { Masked = true, ReadOnly = true });

// Audit user + configure store
forms.SetAuditUser("bob");
forms.ConfigureAudit(cfg =>
{
    cfg.Enabled = true;
    cfg.FlushIntervalSeconds = 60;
    cfg.Store = new FileAuditStore("./session-audit.json");
});

// After some operations ...
var changes = forms.GetAuditLog(blockName: "CUSTOMERS", from: DateTime.Today);
foreach (var entry in changes)
    Console.WriteLine($"{entry.UserName} {entry.Operation} {entry.BlockName} at {entry.Timestamp}");
```

## Scenario E: System Variables

```csharp
// Read Oracle Forms :SYSTEM.* equivalents
var mode = forms.SystemVariables.GetVariable(":SYSTEM.MODE");           // QUERY / ENTER-QUERY / NORMAL
var formName = forms.SystemVariables.GetVariable(":SYSTEM.CURRENT_FORM");
var lastQuery = forms.SystemVariables.GetVariable(":SYSTEM.LAST_QUERY");
```

## Notes
- `TriggerLibrary` factories cover ~80% of common triggers; use them before writing custom lambdas.
- LOV `GetRelatedFieldValues` uses an internal `__RETURN_VALUE__` key; use `ShowLOVAsync` at the FormsManager level to get correct field binding.
- `PagingManager` is state-only; set total-record count after every query when paging is active.
- Security enforces at the CRUD entry-point level; UI disabling is supplemental.

