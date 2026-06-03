# WinForm Integrated IDE Extension — Reference

## Scenario A: Adding a Connection via Navigator

1. Right-click "Connections" → "Add Connection" (or click ➕ in toolbar)
2. `ConnectionEditorDialog` opens
3. Fill in: Connection Name, Category, Database Type, Class Handler, Endpoint, Credentials
4. Click "Test Connection" → calls `IDMEEditor.CreateNewDataSourceConnection()` + `OpenDataSource()`
5. Click "Save" → calls `ConfigEditor.AddDataConnection()` + `SaveDataconnectionsValues()`
6. Navigator refreshes connection tree

## Scenario B: Creating a Form from Template

1. Click "📄 New Form" in toolbar
2. `FormTemplateSelectorDialog` — pick template (Master-Detail, Simple CRUD, etc.)
3. `FormTemplateManager` generates:
   - Form `.cs` file with `FormsManager` bootstrap code
   - Form `.Designer.cs` with `BeepDataConnection` component, `BeepForms` host, `BeepBlock` controls
4. Form appears in navigator tree under "Forms"

## Scenario C: Scaffolding a Block onto Existing Form

1. Expand connection → entity → select entity node
2. Click "Send to Form" → `FormWorkflowCoordinator` shows `BlockCreationDialog`
3. Configure block name, fields, presentation mode
4. `DesignerCodeGenerator` adds block definition to `Designer.cs`
5. `DesignerCodeGenerator` adds bootstrap registration to `Form.cs`
6. Navigator refreshes form tree

## Scenario D: Editing Block Fields

1. Select block → item in navigator tree
2. `BlockFieldsEditorDialog` opens in the workspace pane
3. Edit field properties: label, control type, binding property, options
4. Click "Apply" → `DesignerCodeGenerator.ReplaceIntegratedFieldDefinitionsAsync()` updates Designer.cs
5. Click "Sync" → `BeepDataBlockConverter.SyncToFormDesignerAsync()` pushes changes back

## Scenario E: Driver/DataSource Download

1. Tools → "Download Drivers" → `DriverDownloadDialog`
2. Select driver from list → click Download
3. `NuGetPackageService.DownloadAndInstallPackageAsync()`:
   - Searches configured NuGet repos
   - Downloads + extracts to `ConnectionDrivers` folder
   - Loads assemblies via `LoadAllAssembly()`
   - Creates `ConnectionDriversConfig` entry
4. Driver appears as available class handler in Connection dialog

## Navigator Tree Structure

```
🗄️ Connections (N)
  ├─ 🗄️ RDBMS (N)
  │   ├─ 🟢🗄️ Northwind (SqlServer)
  │   │   └─ 📚 Entities (N)
  │   │       ├─ 📋 Customers (N fields)
  │   │       │   ├─ 🔑 CustomerID — nchar(5) • PK • Req
  │   │       │   ├─ 🧾 CompanyName — nvarchar(40) • Req
  │   │       │   └─ 🧾 ContactName — nvarchar(30) • Null
  │   │       └─ ... N more
  │   └─ 🟢🐬 Sakila (Mysql)
  └─ 📁 FILE (N)
      └─ 🟢💾 LocalDB (SqlLite)
📋 Forms (N)
  └─ 📄 Form1.cs
      ├─ 📦 Customers (Block)
      │   ├─ 📝 CustomerID (Item) • TextBox
      │   └─ 📝 CompanyName (Item) • TextBox
      └─ 📦 Orders (Block)
```

## Extension Entry Point

```csharp
// ExtensionEntrypoint.cs
public static class ExtensionEntrypoint
{
    public static IDMEEditor? GetEditor() => BeepDesktopServices.BeepService?.DMEEditor;
    // Profile config: %AppData%/TheTechIdea/BeepIDE/Profiles/default
}
```
