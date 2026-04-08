# Phase 10 — Update Examples and Smoke-Test Checklist

**Repo:** Beep.Sample / Beep.Winform.Sample  
**Scope:** All example projects that use `BeepDataBlock`.  
**Depends on:** Phases 01–09 complete and building.  
**Build check:** `dotnet build Beep.Sample.sln` — zero errors.

---

## What changes in this phase

| Area | Change |
|---|---|
| Example setup code | Remove bridge classes; use `LOVDefinition`, `ValidationRuleDefinition`, etc. directly |
| `OnPreQuery` / `OnPostQuery` subscriptions | Replace with `RegisterTrigger` |
| `BeepDataBlockProperties` usage | Replace with `SetItemProperty` / `GetItemProperty` |
| `IsCoordinated` checks | Remove — block is always coordinated |
| `FormsManager` setup in examples | Explicit `BeepDataBlock.FormsManager = ...` via `BeepServiceProvider` |

---

## Clean Code Rules

- Examples should be the **simplest correct** usage — no showcasing deprecated APIs.
- Every example that shows LOV setup uses `LOVDefinition` from `TheTechIdea.Beep.Editor.Forms.Models`.
- Every example that shows triggers uses `RegisterTrigger` API.
- No example uses `IsCoordinated` as a conditional — it reads as noise after Phase 02.

---

## Reference Example: Full Block Setup After Migration

```csharp
using TheTechIdea.Beep.Editor.Forms.Models;
using TheTechIdea.Beep.Winform.Controls;

/// <summary>
/// Demonstrates a minimal OrdersBlock setup using the delegated API.
/// All business state lives in FormsManager; this code is purely wiring.
/// </summary>
public class OrdersFormController
{
    private readonly BeepDataBlock _ordersBlock;

    public OrdersFormController(BeepDataBlock ordersBlock, IFormsManager formsManager)
    {
        _ordersBlock = ordersBlock;
        _ordersBlock.FormsManager = formsManager;
        _ordersBlock.Name         = "OrdersBlock";
        _ordersBlock.EntityName   = "Orders";
        _ordersBlock.FormName     = "MainOrdersForm";
    }

    public void SetupBlock()
    {
        // 1. Register fields (items) by name.
        _ordersBlock.RegisterItem("OrderId",      Components["OrderId"]);
        _ordersBlock.RegisterItem("CustomerName", Components["CustomerName"]);
        _ordersBlock.RegisterItem("OrderDate",    Components["OrderDate"]);
        _ordersBlock.RegisterItem("Status",       Components["Status"]);

        // 2. Attach a LOV to the Status field.
        _ordersBlock.RegisterLOV("Status", new LOVDefinition
        {
            Title         = "Order Status",
            DataSource    = "StatusLookup",
            ValueColumn   = "Code",
            DisplayColumn = "Description"
        });

        // 3. Register a validation rule.
        _ordersBlock.RegisterValidationRule("OrderDate", new ValidationRuleDefinition
        {
            RuleName      = "FutureDateCheck",
            ValidationType = ValidationType.Custom,
            IsRequired    = true,
            ErrorMessage  = "Order date cannot be in the past.",
            Validate      = (value, _) => value is DateTime dt && dt.Date >= DateTime.Today
        });

        // 4. Register a Pre-Query trigger.
        _ordersBlock.RegisterTrigger(TriggerType.PreQuery, async ctx =>
        {
            return CurrentUser.HasPermission("Orders.Query");
        });

        // 5. Register a When-New-Record-Instance trigger.
        _ordersBlock.RegisterTrigger(TriggerType.WhenNewRecordInstance, async ctx =>
        {
            _ordersBlock.SetItemProperty("Status", "DefaultValue", "PENDING");
            return true;
        });
    }

    public async Task LoadInitialData()
    {
        await _ordersBlock.ExecuteQueryAsync();
    }

    // Visual Studio designer-provided; typed as IBeepUIComponent in real code.
    private Dictionary<string, IBeepUIComponent> Components =>
        _ordersBlock.UIComponents;
}
```

---

## Example: Setting Up a Master–Detail Relationship

```csharp
public class OrdersWithLinesController
{
    public void SetupRelationships(IFormsManager formsManager)
    {
        // Register the relationship — FormsManager handles re-query of Lines on Orders navigation.
        formsManager.Relationships.Register(new BlockRelationship
        {
            MasterBlockName = "OrdersBlock",
            DetailBlockName = "OrderLinesBlock",
            MasterKey       = "OrderId",
            DetailKey       = "OrderId"
        });
    }
}
```

---

## Smoke-Test Checklist

Execute each scenario manually in the WinForms test application (`WinFormsApp.UI.Test`).

### A. Block Setup
- [ ] Create a form with one `BeepDataBlock`, assign `FormsManager` via designer property.
- [ ] Call `RegisterItem` for 3 fields — verify `UIComponents` has 3 entries.
- [ ] Call `SetItemEnabled("fieldName", false)` — verify the WinForms component becomes disabled.

### B. LOV
- [ ] Call `RegisterLOV("Status", definition)` — verify `_formsManager.LOV.HasLOV(...)` returns true.
- [ ] Click the LOV button on the Status component — verify `ShowLOV` opens the dialog.
- [ ] Select a row in the dialog — verify the Status field updates.

### C. Validation
- [ ] Leave OrderDate blank and call `SaveAsync()` — verify error message appears.
- [ ] Enter a past date and call `SaveAsync()` — verify custom validator fires and rejects.
- [ ] Enter a valid future date — verify save proceeds.

### D. Triggers
- [ ] Verify `PreQuery` trigger blocks `ExecuteQueryAsync` when permission check returns false.
- [ ] Verify `WhenNewRecordInstance` sets the Status default on insert.
- [ ] Call `EnableTrigger` / `DisableTrigger` by name — verify trigger fires/skips accordingly.

### E. Navigation
- [ ] Call `NextRecord()` / `PreviousRecord()` — verify `:SYSTEM.CURSOR_RECORD` updates.
- [ ] Press Tab through all items — verify `_currentItemName` changes and WinForms focus follows.

### F. Master–Detail
- [ ] Navigate Orders — verify OrderLines re-queries with the master `OrderId`.
- [ ] Delete master record — verify detail records are cleared (if `CascadeDelete` is configured).

### G. Data Operations
- [ ] Call `ExecuteQueryAsync()` — verify records load into UI components.
- [ ] Edit a field and call `SaveAsync()` — verify the change persists.
- [ ] Edit a field and call `RollbackAsync()` — verify the change is discarded.

### H. System Variables
- [ ] Read `SYSTEM.CURSOR_ITEM` after `GoToItem("OrderDate")` — verify value is `"OrderDate"`.
- [ ] Read `SYSTEM.BLOCK_STATUS` after edit — verify value is `"CHANGED"`.

---

## Checklist

- [ ] Update all `Beep.Sample.*` example projects — remove bridge classes and deprecated APIs
- [ ] Add full block setup example (see `OrdersFormController` above)
- [ ] Add master-detail relationship example
- [ ] Execute all smoke tests (sections A–H) in `WinFormsApp.UI.Test`
- [ ] `dotnet build Beep.Sample.sln` — zero errors
- [ ] All smoke tests pass  
