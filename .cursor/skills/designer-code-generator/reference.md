# Designer Code Generator — Reference

## Scenario A: Adding a Block Definition to BeepForms

```csharp
var generator = new DesignerCodeGenerator();
var fields = new List<IntegratedFieldDefinition>
{
    new() { FieldName = "CustomerID", ControlType = "BeepTextBox" },
    new() { FieldName = "CompanyName", ControlType = "BeepTextBox" }
};

await generator.AddBeepBlockDefinitionToBeepFormsAsync(
    designerFilePath: @"C:\Project\Form1.Designer.cs",
    blockName: "Customers",
    entityName: "Customers",
    connectionName: "Northwind",
    fields: fields,
    cancellationToken: ct);
```

**Generated output:**
```csharp
// <copilot-generated-integrated-block:Customers>
var customersDefinition = new BeepBlockDefinition();
customersDefinition.BlockName = "Customers";
customersDefinition.ManagerBlockName = "Customers";
customersDefinition.Entity = new BeepBlockEntityDefinition
{
    ConnectionName = "Northwind",
    EntityName = "Customers"
};
// Fields
var customerIDField = new BeepFieldDefinition();
customerIDField.FieldName = "CustomerID";
customerIDField.ControlType = "BeepTextBox";
customersDefinition.Fields.Add(customerIDField);
var companyNameField = new BeepFieldDefinition();
companyNameField.FieldName = "CompanyName";
companyNameField.ControlType = "BeepTextBox";
customersDefinition.Fields.Add(companyNameField);
// </copilot-generated-integrated-block:Customers>
```

## Scenario B: Adding a Bootstrap Registration

```csharp
await generator.AddBeepFormsRuntimeBootstrapRegistrationAsync(
    formFilePath: @"C:\Project\Form1.cs",
    formsHostName: "beepForms1",
    connectionComponentName: "beepDataConnection1",
    registration: new IntegratedBootstrapRegistration
    {
        BlockName = "Customers",
        EntityName = "Customers",
        ConnectionName = "Northwind"
    },
    cancellationToken: ct);
```

**Generated output:**
```csharp
// <copilot-generated-integrated-bootstrap-shared>
var formsManager = new FormsManager(_editor);
// </copilot-generated-integrated-bootstrap-shared>

// <copilot-generated-integrated-bootstrap-registrations>
// <copilot-generated-integrated-bootstrap:Customers>
await formsManager.SetupBlockAsync("Customers", "Northwind", "Customers", isMaster: true);
beepForms1.Blocks.Add(customersBlock);
// </copilot-generated-integrated-bootstrap:Customers>
```

## Scenario C: Setting Up a Data Connection Component

```csharp
await generator.EnsureBeepDataConnectionComponentAsync(
    designerFilePath: @"C:\Project\Form1.Designer.cs",
    componentName: "beepDataConnection1",
    connection: new ConnectionProperties
    {
        ConnectionName = "Northwind",
        DatabaseType = DataSourceType.SqlServer,
        Host = "localhost"
    },
    ideAppRepoName: "BeepPlatformConnections",
    ideBeepDirectory: @"C:\Project"
    cancellationToken: ct);
```

## Scenario D: Update Block Definition Properties (Sync)

```csharp
var generator = new DesignerCodeGenerator();

// Update caption
await generator.UpdateBlockDefinitionCorePropertiesAsync(
    designerFilePath, "customersDefinition",
    caption: "Customer List",
    managerBlockName: "Customers",
    presentationMode: "Record",
    ct);

// Update entity
await generator.UpdateBlockDefinitionEntityAsync(
    designerFilePath, "customersDefinition",
    entityDefinition, ct);

// Update navigation
await generator.UpdateBlockDefinitionNavigationAsync(
    designerFilePath, "customersDefinition",
    navigationDefinition, ct);

// Update metadata
await generator.UpdateBlockDefinitionMetadataAsync(
    designerFilePath, "customersDefinition",
    "QueryString", "SELECT * FROM Customers WHERE Country = 'USA'", ct);
```

## Scenario E: Removing a Block

```csharp
await generator.RemoveBeepBlockDefinitionFromBeepFormsAsync(
    designerFilePath, "Customers", ct);

await generator.RemoveBeepFormsRuntimeBootstrapRegistrationAsync(
    formFilePath, "Customers", ct);

await generator.RemoveBeepFormsRuntimeRelationshipRegistrationsAsync(
    formFilePath, "Customers", ct);
```

## Marker Format Reference

### Block Definition
```
// <copilot-generated-integrated-block:{BlockName}>
... definition code ...
// </copilot-generated-integrated-block:{BlockName}>
```

### Bootstrap
```
// <copilot-generated-integrated-bootstrap:{BlockName}>
... registration code ...
// </copilot-generated-integrated-bootstrap:{BlockName}>
```

### Relationship
```
// <copilot-generated-integrated-relation:{MasterBlock}->{DetailBlock}>
... relation code ...
// </copilot-generated-integrated-relation:{MasterBlock}->{DetailBlock}>
```

### LOV
```
// <copilot-generated-integrated-lov:{BlockName}::{FieldName}>
... LOV registration ...
// </copilot-generated-integrated-lov:{BlockName}::{FieldName}>
```

### Validation
```
// <copilot-generated-integrated-validation:{BlockName}::{FieldName}::{RuleName}>
... validation registration ...
// </copilot-generated-integrated-validation:{BlockName}::{FieldName}::{RuleName}>
```

### Trigger
```
// <copilot-generated-integrated-trigger:{BlockName}::{FieldName}::{TriggerType}::{Handler}>
... trigger registration ...
// </copilot-generated-integrated-trigger:{BlockName}::{FieldName}::{TriggerType}::{Handler}>
```
