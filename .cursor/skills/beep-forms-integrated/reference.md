# BeepForms Integrated Reference

## Scenario 1 — Definition-first form composition

```csharp
// WinForms Form constructor — definition-driven setup
// beepForms1 is the BeepForms component dropped onto the form.

this.beepForms1.FormName = "OrderEntry";
this.beepForms1.AutoCreateBlocksFromDefinition = true;
this.beepForms1.Definition = new BeepFormsDefinition
{
    FormName = "OrderEntry",
    Title    = "Order Entry",
    Blocks   = new List<BeepBlockDefinition>
    {
        new()
        {
            BlockName        = "customers",
            Caption          = "Customer",
            PresentationMode = BeepBlockPresentationMode.Record,
            Entity           = new BeepBlockEntityDefinition
            {
                ConnectionName = "NorthwindDB",
                EntityName     = "Customers",
                IsMasterBlock  = true
            }
        },
        new()
        {
            BlockName        = "orders",
            Caption          = "Orders",
            PresentationMode = BeepBlockPresentationMode.Grid,
            Entity           = new BeepBlockEntityDefinition
            {
                ConnectionName  = "NorthwindDB",
                EntityName      = "Orders",
                MasterBlockName = "customers",
                MasterKeyField  = "CustomerId",
                ForeignKeyField = "CustomerId"
            }
        }
    }
};

// Attach the FormsManager — triggers SyncFromManager on all blocks
this.beepForms1.FormsManager = myFormsManager;
```

---

## Scenario 2 — Manual block registration (runtime or designer drag)

```csharp
// Blocks dropped on designer or created programmatically and registered at runtime

this.beepForms1.FormName = "CustomerEditor";
this.beepForms1.FormsManager = myFormsManager;

// Register existing BeepBlock instances
this.beepForms1.RegisterBlock(this.customerBlock);
this.beepForms1.RegisterBlock(this.contactBlock);

// Activate a specific block
this.beepForms1.TrySetActiveBlock("customers");
```

---

## Scenario 3 — Shell surfaces wired to BeepForms

```csharp
// Designer.cs — shell surfaces receive the host reference at design time
// Each control reads BeepForms.ViewState directly.

// BeepFormsHeader shows form title and active block name
this.beepFormsHeader1.Host = this.beepForms1;

// BeepFormsCommandBar shows block tabs and sync button
this.beepFormsCommandBar1.Host = this.beepForms1;

// BeepFormsQueryShelf renders "Enter Query" / "Execute Query"
this.beepFormsQueryShelf1.Host           = this.beepForms1;
this.beepFormsQueryShelf1.CaptionMode    = BeepFormsQueryShelfCaptionMode.OracleForms;

// BeepFormsPersistenceShelf renders Commit / Rollback with dirty-state badge
this.beepFormsPersistenceShelf1.Host = this.beepForms1;

// BeepFormsToolbar — savepoints + alert presets
this.beepFormsToolbar1.Host = this.beepForms1;

// BeepFormsStatusStrip — live status / message / workflow lines
this.beepFormsStatusStrip1.Host = this.beepForms1;
```

---

## Scenario 4 — Handling form messages and view state changes

```csharp
// Subscribe to ViewStateChanged to drive custom chrome outside the built-in surfaces
this.beepForms1.ViewStateChanged += (s, e) =>
{
    var state = this.beepForms1.ViewState;

    // Update a custom title bar
    this.Text = state.IsDirty
        ? $"* {state.ActiveBlockName}"
        : state.ActiveBlockName ?? "Form";

    // Flash a custom status label
    if (state.MessageSeverity == BeepFormsMessageSeverity.Error)
    {
        this.customStatusLabel.ForeColor = Color.Red;
    }
    this.customStatusLabel.Text = state.CurrentMessage ?? string.Empty;
};
```

---

## Scenario 5 — FormsManager attachment and sync (current manual path)

> ⚠ Phase 7 not yet implemented — blocks must be registered manually before assigning FormsManager.

```csharp
// CURRENT REQUIREMENT: manually register blocks with FormsManager BEFORE assigning it.
// BeepForms does not yet call FormsManager.RegisterBlockAsync() from definition data.

private async Task AttachManagerAsync(IUnitofWorksManager manager)
{
    // Step 1 — register each block: FormsManager opens connection, loads EntityStructure, creates UoW
    await manager.RegisterBlockAsync("customers", "NorthwindDB", "Customers");
    await manager.RegisterBlockAsync("orders",    "NorthwindDB", "Orders");

    // Step 2 — open the form (registers form in manager, applies relationships)
    await manager.OpenFormAsync("OrderEntry");

    // Step 3 — assign to host — triggers AttachToFormsManager + SyncFromManager on all blocks
    this.beepForms1.FormsManager = manager;
    // BeepBlock.SyncFromManager() now finds registered blocks → EntityStructure flows to field scaffold.
}

// FUTURE (Phase 7 auto-bootstrap path — not yet implemented):
// this.beepForms1.DataConnection = this.beepDataConnection1;
// this.beepForms1.Definition     = myDefinition;
// this.beepForms1.FormsManager   = manager;  // → auto-registers blocks from definition
```

---

## Scenario 6 — Savepoint workflow

```csharp
// Triggered by BeepFormsToolbar savepoint button or programmatically

// Create a named savepoint before a risky edit sequence
await this.beepForms1.CreateSavepointAsync("BeforeDiscount");

// If the user decides to revert:
await this.beepForms1.RestoreSavepointAsync(savepointId);
// HasActiveSavepoint → false; IsDirty reflects restored state.

// On successful commit, savepoints are cleared automatically by FormsManager.
```

---

## Scenario 7 — Master/detail refresh coordination

```csharp
// BeepForms.MasterDetail.cs listens to FormsManager relationship events.
// No manual wiring needed — register the relationship in FormsManager:

formsManager.RegisterRelationship(
    masterBlock: "customers",
    detailBlock: "orders",
    masterKeyField: "CustomerId",
    foreignKeyField: "CustomerId");

// When the user navigates to a different customer record,
// BeepForms receives the master-changed event and publishes a
// coordinated refresh message to "orders" block automatically.

// To observe detail-refresh events:
this.beepForms1.ViewStateChanged += (s, e) =>
{
    if (this.beepForms1.ViewState.HasActiveCoordination)
    {
        // E.g., lock an unrelated UI element while detail reloads
    }
};
```

---

## Scenario 8 — Adding a new custom shell surface (extracted-surface pattern)

```csharp
// Follow the same pattern as BeepFormsPersistenceShelf.
// New surface reads ViewState; does not own workflow logic.

public class MyCustomApprovalShelf : Control
{
    private IBeepFormsHost? _host;

    public IBeepFormsHost? Host
    {
        get => _host;
        set
        {
            if (_host != null) _host.ViewStateChanged -= OnViewStateChanged;
            _host = value;
            if (_host != null) _host.ViewStateChanged += OnViewStateChanged;
            Refresh();
        }
    }

    private void OnViewStateChanged(object? sender, EventArgs e)
    {
        // Re-render based on ViewState
        approveButton.Enabled = _host?.ViewState.IsDirty == false;
    }

    private void approveButton_Click(object sender, EventArgs e)
    {
        // Route through command router — never call FormsManager directly
        (_host as BeepForms)?.CommandRouter.ExecuteApproveAsync();
    }
}
```
