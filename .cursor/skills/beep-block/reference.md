# BeepBlock Reference

## Scenario 1 — Minimal design-time setup (record mode)

```csharp
// Designer.cs generated output after smart-tag wizard for a Customer block

this.beepBlock1.BlockName = "customers";
this.beepBlock1.Definition = new BeepBlockDefinition
{
    BlockName = "customers",
    Caption = "Customer",
    PresentationMode = BeepBlockPresentationMode.Record,
    Entity = new BeepBlockEntityDefinition
    {
        ConnectionName = "NorthwindDB",
        EntityName = "Customers",
        DatasourceEntityName = "Customers"
    },
    Fields = new List<BeepFieldDefinition>
    {
        new() { FieldName = "CustomerId",  Label = "ID",      EditorKey = "text",    Order = 0, IsReadOnly = true },
        new() { FieldName = "CompanyName", Label = "Company", EditorKey = "text",    Order = 1, Width = 240 },
        new() { FieldName = "Country",     Label = "Country", EditorKey = "combo",   Order = 2 },
        new() { FieldName = "Since",       Label = "Since",   EditorKey = "date",    Order = 3 },
        new() { FieldName = "IsActive",    Label = "Active",  EditorKey = "checkbox",Order = 4 }
    }
};
```

---

## Scenario 2 — Grid mode with entity snapshot

```csharp
// Grid mode block that derives field columns from entity metadata
this.beepBlock1.BlockName = "orders";
this.beepBlock1.Definition = new BeepBlockDefinition
{
    BlockName = "orders",
    Caption = "Orders",
    PresentationMode = BeepBlockPresentationMode.Grid,
    Entity = new BeepBlockEntityDefinition
    {
        ConnectionName = "NorthwindDB",
        EntityName = "Orders"
    }
    // No explicit Fields — BeepBlock falls back to entity runtime snapshot
};
```

---

## Scenario 3 — LOV field with related-field mapping

```csharp
// ProductId is a LOV field. Selecting a product also fills ProductName.
Fields = new List<BeepFieldDefinition>
{
    new()
    {
        FieldName      = "ProductId",
        Label          = "Product",
        EditorKey      = "lov",      // triggers picker button + F9 + popup
        Order          = 0,
        Width          = 200
    },
    new()
    {
        FieldName      = "ProductName",
        Label          = "Name",
        EditorKey      = "text",
        IsReadOnly     = true,       // filled by LOV map-back
        Order          = 1
    }
};

// FormsManager-side LOV registration (done at app startup)
formsManager.RegisterLov("orders", "ProductId", "products", "ProductId", "ProductName",
    relatedFieldMappings: new Dictionary<string, string> { ["ProductName"] = "ProductName" });
```

---

## Scenario 4 — Registering a custom presenter

```csharp
// Color picker presenter used for a "StatusColor" field
public class ColorPickerPresenter : IBeepFieldPresenter
{
    public string Key => "colorpicker";

    public bool CanPresent(BeepFieldDefinition fd) =>
        string.Equals(fd.EditorKey, "colorpicker", StringComparison.OrdinalIgnoreCase);

    public Control CreateEditor(BeepFieldDefinition fd)
    {
        var btn = new BeepButton();
        btn.Tag = fd.FieldName;
        return btn;
    }
}

// Register before scaffold runs
block.PresenterRegistry.Register(new ColorPickerPresenter());

// Use in definition
new BeepFieldDefinition { FieldName = "StatusColor", EditorKey = "colorpicker", Order = 5 }
```

---

## Scenario 5 — Explicit control-type override (reflective presenter)

```csharp
// Force a specific Beep control by CLR type name; binding via SelectedValue
new BeepFieldDefinition
{
    FieldName       = "RegionCode",
    Label           = "Region",
    ControlType     = "TheTechIdea.Beep.Winform.Controls.BeepComboBox",
    BindingProperty = "SelectedValue",
    Order           = 2
}
// ReflectiveControlBeepFieldPresenter instantiates BeepComboBox
// and registers a SelectedValue ↔ RegionCode binding.
```

---

## Scenario 6 — Query mode criteria packaging

```csharp
// Block in Query mode produces AppFilter list for manager
// Triggered by BeepBlockNavigationBar "Execute Query" button or F8

// Example: filter OrderDate between two dates and Country in list
// After user fills criteria rows:
var filters = block.PackageFiltersForManager();
// Returns:
// [
//   AppFilter { FieldName="OrderDate", Operator="between", FilterValue="2025-01-01", FilterValue1="2025-12-31" },
//   AppFilter { FieldName="Country",   Operator="in",      FilterValue="UK,US,DE" }
// ]

await formsManager.ExecuteQueryAsync("orders", filters);
```

---

## Scenario 7 — Master/detail block wiring

```csharp
// Master block: Customers
this.masterBlock.BlockName = "customers";
this.masterBlock.Definition = new BeepBlockDefinition
{
    BlockName = "customers",
    Entity    = new BeepBlockEntityDefinition
    {
        ConnectionName = "NorthwindDB",
        EntityName     = "Customers",
        IsMasterBlock  = true
    }
};

// Detail block: Orders filtered by current customer
this.detailBlock.BlockName = "orders";
this.detailBlock.Definition = new BeepBlockDefinition
{
    BlockName = "orders",
    Entity    = new BeepBlockEntityDefinition
    {
        ConnectionName  = "NorthwindDB",
        EntityName      = "Orders",
        MasterBlockName = "customers",
        MasterKeyField  = "CustomerId",
        ForeignKeyField = "CustomerId"
    }
};
```

---

## Scenario 8 — Field-control-type policy override

```csharp
// Override the default editor key for all integer fields project-wide
// (written to %LocalAppData%\TheTechIdea\Beep.Winform\field-control-defaults.json)

var policy = BeepFieldControlTypeRegistry.LoadPolicy();
policy.Rules.Add(new BeepFieldControlTypeRule
{
    Category        = DbFieldCategory.Integer,
    EditorKey       = "numeric",
    ControlType     = "TheTechIdea.Beep.Winform.Controls.BeepNumericUpDown",
    BindingProperty = "Value"
});
BeepFieldControlTypeRegistry.SavePolicy(policy);

// Runtime resolution now prefers this rule over the built-in default
var resolution = BeepFieldControlTypeRegistry.ResolveDefaultFieldSettings(DbFieldCategory.Integer);
// resolution.EditorKey → "numeric"  (from policy)
```
