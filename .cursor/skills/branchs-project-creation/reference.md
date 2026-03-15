# Branchs Project Reference

This file provides concrete scaffolding for creating a new domain branch project (example: Accounting) following `TheTechIdea.Beep.TreeNodes`.

## Learn vs Recreate

Before creating any class shown in this reference:

1. Check if equivalent functionality already exists in:
   - the current project,
   - sibling branch projects,
   - referenced Beep packages.
2. If it exists, reuse it and only add domain-specific behavior.
3. If it does not exist, scaffold a new implementation from these templates.

Do not duplicate shared framework/helper code just because a template exists here.

## Recommended Folder Layout

```text
TheTechIdea.Beep.AccountingTreeNodes/
├── TheTechIdea.Beep.AccountingTreeNodes.csproj
├── globleusings.cs
├── Accounting/
│   ├── AccountingRootNode.cs
│   ├── AccountingNode.cs
│   ├── AccountingEntitiesNode.cs
│   └── AccountingCategoryNode.cs          (optional)
└── AccountingNodesHelpers.cs               (optional)
```

## csproj Starter

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
    <RootNamespace>TheTechIdea.Beep</RootNamespace>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <AssemblyName>TheTechIdea.Beep.AccountingTreeNodes</AssemblyName>
    <PackageId>TheTechIdea.Beep.AccountingTreeNodes</PackageId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="TheTechIdea.Beep.DataManagementEngine" Version="2.0.83" />
    <PackageReference Include="TheTechIdea.Beep.DataManagementModels" Version="2.0.137" />
    <PackageReference Include="TheTechIdea.Beep.Vis.Modules" Version="2.0.38" />
  </ItemGroup>
</Project>
```

## Global Usings Starter

```csharp
global using TheTechIdea.Beep.Vis.Modules;
global using TheTechIdea.Beep.ConfigUtil;
global using TheTechIdea.Beep.Editor;
global using TheTechIdea.Beep.DataBase;
global using TheTechIdea.Beep.Utilities;
global using TheTechIdea.Beep.Addin;
global using TheTechIdea.Beep.Vis;
```

## Root Node Template

```csharp
[AddinAttribute(
    Caption = "Accounting",
    BranchType = EnumPointType.Root,
    Name = "AccountingRootNode.Beep",
    iconimage = "accountingroot.svg",
    menu = "DataSource",
    ObjectType = "Beep",
    Category = DatasourceCategory.RDBMS)]
[AddinVisSchema(
    BranchType = EnumPointType.Root,
    BranchClass = "DATASOURCEROOT",
    RootNodeName = "DataSourcesRootNode")]
public class AccountingRootNode : IBranch
{
    public string BranchClass { get; set; } = "ACCOUNTING";
    public string BranchText { get; set; } = "Accounting";
    public EnumPointType BranchType { get; set; } = EnumPointType.Root;
    public string GuidID { get; set; } = Guid.NewGuid().ToString();
    public int ID { get; set; }
    public int BranchID { get; set; }
    public int ParentBranchID { get; set; }
    public string ParentGuidID { get; set; }
    public string DataSourceConnectionGuidID { get; set; }
    public string EntityGuidID { get; set; }
    public string MiscStringID { get; set; }
    public bool Visible { get; set; } = true;
    public bool IsDataSourceNode { get; set; } = true;
    public string MenuID { get; set; }
    public string Name { get; set; }
    public string IconImageName { get; set; } = "accountingroot.svg";
    public string BranchStatus { get; set; }
    public string BranchDescription { get; set; }
    public ITree TreeEditor { get; set; }
    public IDMEEditor DMEEditor { get; set; }
    public IDataSource DataSource { get; set; }
    public string DataSourceName { get; set; }
    public List<IBranch> ChildBranchs { get; set; } = new();
    public IBranch ParentBranch { get; set; }
    public List<string> BranchActions { get; set; }
    public EntityStructure EntityStructure { get; set; }
    public int Level { get; set; }
    public int MiscID { get; set; }
    public IAppManager Visutil { get; set; }
    public object TreeStrucure { get; set; }
    public string ObjectType { get; set; } = "Beep";

    public IErrorsInfo SetConfig(ITree tree, IDMEEditor editor, IBranch parent, string text, int id, EnumPointType type, string image)
    {
        TreeEditor = tree;
        DMEEditor = editor;
        ParentBranchID = parent?.ID ?? -1;
        if (id != 0) { ID = id; BranchID = id; }
        return DMEEditor.ErrorObject;
    }

    public IErrorsInfo CreateChildNodes()
    {
        // Pattern from ConnectorRootNode/DataViewRootNode:
        // enumerate matching data connections and add AccountingNode children.
        return DMEEditor.ErrorObject;
    }

    public IErrorsInfo RemoveChildNodes() => TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
    public IErrorsInfo ExecuteBranchAction(string actionName) => DMEEditor.ErrorObject;
    public IErrorsInfo MenuItemClicked(string actionName) => DMEEditor.ErrorObject;
    public IBranch CreateCategoryNode(CategoryFolder p) => throw new NotImplementedException();
}
```

## Data-Point Node Template

```csharp
[AddinAttribute(
    Caption = "Accounting",
    BranchType = EnumPointType.DataPoint,
    Name = "AccountingNode.Beep",
    iconimage = "accountingnode.svg",
    menu = "Beep",
    ObjectType = "Beep")]
public class AccountingNode : IBranch
{
    // Same contract properties as root node.
    // Important values:
    // BranchClass = "ACCOUNTING"
    // BranchType = EnumPointType.DataPoint
    // DataSourceConnectionGuidID = connection.GuidID

    [CommandAttribute(Caption = "Get Entities", iconimage = "getchilds.png", PointType = EnumPointType.DataPoint, ObjectType = "Beep")]
    public IErrorsInfo GetEntities()
    {
        // Reuse DataSourceDefaultMethods.GetEntities(this, DMEEditor, Visutil) pattern
        return DMEEditor.ErrorObject;
    }

    public IErrorsInfo CreateChildNodes() => GetEntities();
    public IErrorsInfo SetConfig(ITree tree, IDMEEditor editor, IBranch parent, string text, int id, EnumPointType type, string image) { return DMEEditor.ErrorObject; }
    public IErrorsInfo RemoveChildNodes() => TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
    public IErrorsInfo ExecuteBranchAction(string actionName) => DMEEditor.ErrorObject;
    public IErrorsInfo MenuItemClicked(string actionName) => DMEEditor.ErrorObject;
    public IBranch CreateCategoryNode(CategoryFolder p) => throw new NotImplementedException();
}
```

## Entity Node Template

```csharp
[AddinAttribute(
    Caption = "Accounting Entity",
    BranchType = EnumPointType.Entity,
    Name = "AccountingEntitiesNode.Beep",
    iconimage = "databaseentities.png",
    menu = "Beep",
    ObjectType = "Beep")]
public class AccountingEntitiesNode : IBranch
{
    // Same required properties.
    // BranchType = EnumPointType.Entity
    // BranchClass = "ACCOUNTING"

    [CommandAttribute(Caption = "Open", iconimage = "select.png", ObjectType = "Beep")]
    public IErrorsInfo OpenEntity()
    {
        // Optional: raise/show UI page for this entity
        return DMEEditor.ErrorObject;
    }

    public IErrorsInfo CreateChildNodes() => DMEEditor.ErrorObject;
    public IErrorsInfo SetConfig(ITree tree, IDMEEditor editor, IBranch parent, string text, int id, EnumPointType type, string image) { return DMEEditor.ErrorObject; }
    public IErrorsInfo RemoveChildNodes() => TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
    public IErrorsInfo ExecuteBranchAction(string actionName) => DMEEditor.ErrorObject;
    public IErrorsInfo MenuItemClicked(string actionName) => DMEEditor.ErrorObject;
    public IBranch CreateCategoryNode(CategoryFolder p) => throw new NotImplementedException();
}
```

## Category Node Template (Missing Piece)

Use this node to group data-point branches under folders (pattern from `RDBMS/DatabaseCategoryNode.cs`):

```csharp
[AddinAttribute(
    Caption = "Accounting",
    BranchType = EnumPointType.Category,
    Name = "AccountingCategoryNode.Beep",
    iconimage = "category.png",
    menu = "Beep",
    ObjectType = "Beep")]
public class AccountingCategoryNode : IBranch
{
    public string BranchClass { get; set; } = "ACCOUNTING";
    public EnumPointType BranchType { get; set; } = EnumPointType.Category;
    // ... other IBranch/IBranchID properties

    public IErrorsInfo CreateChildNodes()
    {
        try
        {
            TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
            foreach (var folder in DMEEditor.ConfigEditor.CategoryFolders
                         .Where(x => x.RootName == "ACCOUNTING" && x.FolderName == BranchText))
            {
                foreach (var connectionName in folder.items)
                {
                    var cn = DMEEditor.ConfigEditor.DataConnections
                        .FirstOrDefault(x => x.ConnectionName == connectionName);
                    if (cn == null) continue;

                    var node = new AccountingNode(
                        cn, TreeEditor, DMEEditor, this,
                        cn.ConnectionName, TreeEditor.SeqID,
                        EnumPointType.DataPoint, "database.png");

                    // Runtime add via ITree/handler:
                    TreeEditor.AddBranchToParentInBranchsOnly(this, node); // optional
                    TreeEditor.Treebranchhandler.AddBranch(this, node);     // required
                }
            }
        }
        catch (Exception ex)
        {
            DMEEditor.AddLogMessage(ex.Message, "Could not create category child nodes", DateTime.Now, -1, null, Errors.Failed);
        }
        return DMEEditor.ErrorObject;
    }
}
```

## Add/Refresh Pattern (from TreeNodes)

1. Root `CreateChildNodes()`:
   - Query `DMEEditor.ConfigEditor.DataConnections` filtered by target category/type.
   - Skip duplicates by `GuidID` and existing branch checks.
   - Add child nodes via `TreeEditor.Treebranchhandler.AddBranch(this, childNode)`.
2. Data-point `CreateChildNodes()`:
   - Open datasource.
   - Discover entity list.
   - Build entity branches and attach to current node.
3. Any refresh command:
   - Call `TreeEditor.Treebranchhandler.RemoveChildBranchs(this)`.
   - Re-run child creation.

## BranchType Usage Matrix (Runtime + Metadata)

Use `BranchType` in both attribute metadata and property defaults:

| Node role | AddinAttribute.BranchType | Property default |
|---|---|---|
| Root | `EnumPointType.Root` | `public EnumPointType BranchType { get; set; } = EnumPointType.Root;` |
| Category | `EnumPointType.Category` | `... = EnumPointType.Category;` |
| Data source/connection | `EnumPointType.DataPoint` | `... = EnumPointType.DataPoint;` |
| Entity | `EnumPointType.Entity` | `... = EnumPointType.Entity;` |

Runtime usage examples:
- Filter selected branches by type before actions.
- Route command visibility by `CommandAttribute.PointType`.
- Send actions between branches based on expected types.
- Locate root branches by `BranchType == EnumPointType.Root` and `BranchClass`.

## Consistency Rules

- `BranchClass` must be identical across domain nodes (`ACCOUNTING`).
- `BranchType` must match behavior:
  - Root: `EnumPointType.Root`
  - Source: `EnumPointType.DataPoint`
  - Entity: `EnumPointType.Entity`
- `GuidID` is unique per node instance.
- `DataSourceConnectionGuidID` should reference the backing `ConnectionProperties.GuidID`.
- Use `DMEEditor.AddLogMessage(...)` in all catches.

## Attribute Mapping Reference

Use these metadata classes directly:
- `AddinAttribute` (`DataManagementModelsStandard/Vis/AddinAttribute.cs`) for class registration
- `CommandAttribute` (`DataManagementModelsStandard/Vis/CommandItemAttribute.cs`) for branch functions
- `EnumPointType` (`DataManagementModelsStandard/Vis/Enums.cs`) for branch/command scope
- `IOrder` (`DataManagementModelsStandard/Vis/IOrder.cs`) if branch ordering is needed

Example class registration (from `DatabaseNode` style):

```csharp
[AddinAttribute(
    Caption = "RDBMS",
    BranchType = EnumPointType.DataPoint,
    Name = "DatabaseNode.Beep",
    misc = "Beep",
    iconimage = "database.png",
    menu = "Beep",
    ObjectType = "Beep")]
public class DatabaseNode : IBranch
{
    // ...
}
```

## How To Extend a Branch With Functions

Pattern based on `RDBMS/DatabaseNode.cs` (`GetDatabaseEntites()`):

```csharp
[CommandAttribute(
    Caption = "Get Entities",
    iconimage = "getchilds.png",
    PointType = EnumPointType.DataPoint,
    ObjectType = "Beep")]
public IErrorsInfo GetDatabaseEntites()
{
    DMEEditor.ErrorObject.Flag = Errors.Ok;
    PassedArgs passedArgs = new PassedArgs { DatasourceName = BranchText };

    try
    {
        DataSourceDefaultMethods.GetEntities(this, DMEEditor, Visutil);
    }
    catch (Exception ex)
    {
        DMEEditor.Logger.WriteLog($"Error in Connecting to DataSource ({ex.Message}) ");
        DMEEditor.ErrorObject.Flag = Errors.Failed;
        DMEEditor.ErrorObject.Ex = ex;
        passedArgs.Messege = "Could not Open Connection";
        Visutil.PasstoWaitForm(passedArgs);
        Visutil.CloseWaitForm();
    }

    return DMEEditor.ErrorObject;
}
```

### Extension Checklist For New Functions

1. Choose the right point scope:
   - Data source actions -> `PointType = EnumPointType.DataPoint`
   - Entity actions -> `PointType = EnumPointType.Entity`
2. Keep function signature as `public IErrorsInfo MethodName()`.
3. Use shared default helpers where possible (`DataSourceDefaultMethods` pattern) to avoid duplicating traversal logic.
4. Preserve Beep error object semantics (`ErrorObject.Flag`, `ErrorObject.Ex`) instead of throwing.
5. For UI-triggered actions:
   - Build `PassedArgs` with `DatasourceName`/`ObjectType`/`EventType` as needed
   - Route UI through `Visutil.ShowPage(...)` when opening editors/pages
6. If you add a refresh function, pair it with removal + rehydrate pattern:
   - `TreeEditor.Treebranchhandler.RemoveChildBranchs(this)`
   - then re-run child load.

## AppManager Calling Pattern (Branch-Side)

When a branch command needs to open UI, follow this pattern:

1. Build `PassedArgs` from current branch context (`BranchText`, datasource/entity info, IDs).
2. Resolve/select the addin identifier to pass (typically addin `className`).
3. Call `Visutil` methods only; do not duplicate navigation internals in node classes.

Common calls:
- `Visutil.ShowPage("addinClassOrRoute", passedArgs)` for standard navigation.
- `Visutil.ShowPage("addinClassOrRoute", passedArgs, DisplayType.Popup)` for popup behavior.
- `Visutil.NavigateTo("routeName", parameters)` when route navigation is needed.
- `Visutil.ShowHome()` / `ShowLogin()` / `ShowProfile()` / `ShowAdmin()` for shell flows.

Minimal command pattern:

```csharp
[CommandAttribute(Caption = "Show", PointType = EnumPointType.Entity, ObjectType = "Beep")]
public IErrorsInfo Show()
{
    var passedArgs = new PassedArgs
    {
        CurrentEntity = BranchText,
        ObjectName = BranchText,
        Id = BranchID,
        DataSource = DataSource,
        EventType = "Run"
    };

    // Learn and align with existing AppManager routes/identifiers.
    Visutil.ShowPage(AddinTreeStructure.className, passedArgs);
    return DMEEditor.ErrorObject;
}
```

### Function-to-AppManager Mapping Table (RDBMS + Config)

| Branch function | RDBMS/Config pattern anchor | AppManager call | Typical `PassedArgs` fields |
|---|---|---|---|
| Edit datasource connection | `DatabaseNode` command-style action | `Visutil.ShowPage(configAddinClassName, passedArgs)` | `DatasourceName`, `ObjectName`, `Id`, `DataSource`, `EventType="Run"` |
| Show config module from tree item | `ConfigEntityNode.Show()` | `Visutil.ShowPage(AddinTreeStructure.className, passedArgs)` | `CurrentEntity`, `ObjectName`, `Id`, `DataSource` |
| Open entity viewer quick view | `DatabaseEntitesNode`-style open/view action | `Visutil.ShowPage(viewerAddinClassName, passedArgs, DisplayType.Popup)` | `CurrentEntity`, `DatasourceName`, `ObjectName`, `Id` |
| Open editor in main container | Config/entity edit flows | `Visutil.ShowPage(editorAddinClassName, passedArgs)` | `CurrentEntity`, `ObjectName`, `DataSource`, `EventType` |
| Open relation builder | Data/entity tooling commands | `Visutil.ShowPage("uc_RelationBuilder", passedArgs)` | `DatasourceName`, `CurrentEntity`, `ObjectName`, `Id` |
| Open data viewer | Data/entity inspect commands | `Visutil.ShowPage("uc_DataViewer", passedArgs)` | `DatasourceName`, `CurrentEntity`, `ObjectName`, `Id` |
| Use route-based navigation | Module/shell route flows | `Visutil.NavigateTo(routeName, parameters)` | Route params dictionary (entity/datasource keys) |
| Open shell pages | Cross-module navigation | `Visutil.ShowHome()` / `ShowLogin()` / `ShowProfile()` / `ShowAdmin()` | none (or app-level context) |

Notes:
- Prefer `ShowPage(...)` for addin-driven modules discovered from metadata (`className` pattern).
- Use popup display for transient inspection actions; use in-control display for primary workflow screens.
- Keep mapping logic in branch command methods; do not move/duplicate `AppManager` internals.

### PassedArgs Presets (Micro-Section)

Use these ready-made templates and adjust only the fields needed by your addin.

#### 1) Viewer preset (entity inspection, often popup)

```csharp
var passedArgs = new PassedArgs
{
    EventType = "Run",
    DatasourceName = DataSourceName ?? BranchText,
    CurrentEntity = EntityStructure?.EntityName ?? BranchText,
    ObjectName = BranchText,
    Id = BranchID,
    DataSource = DataSource
};

Visutil.ShowPage("uc_DataViewer", passedArgs, DisplayType.Popup);
```

#### 2) Edit preset (entity editor/designer in main container)

```csharp
var passedArgs = new PassedArgs
{
    EventType = "Edit",
    DatasourceName = DataSourceName ?? BranchText,
    CurrentEntity = EntityStructure?.EntityName ?? BranchText,
    ObjectName = BranchText,
    Id = BranchID,
    DataSource = DataSource
};

Visutil.ShowPage(editorAddinClassName, passedArgs);
```

#### 3) Config preset (datasource/config module)

```csharp
var passedArgs = new PassedArgs
{
    EventType = "Run",
    DatasourceName = DataSourceName ?? BranchText,
    ObjectName = BranchText,
    Id = BranchID,
    DataSource = DataSource
};

Visutil.ShowPage(configAddinClassName, passedArgs);
```

#### 4) Relation preset (relation builder workflow)

```csharp
var passedArgs = new PassedArgs
{
    EventType = "Run",
    DatasourceName = DataSourceName ?? BranchText,
    CurrentEntity = EntityStructure?.EntityName ?? BranchText,
    ObjectName = BranchText,
    Id = BranchID,
    DataSource = DataSource
};

Visutil.ShowPage("uc_RelationBuilder", passedArgs);
```

Preset notes:
- Keep `EventType` consistent with module expectation (`Run`, `Edit`, custom values if required).
- Always include `DataSource`, `DatasourceName`, and `Id` for context-aware modules.
- For config-tree nodes that use addin metadata, `configAddinClassName` can come from `AddinTreeStructure.className`.

#### Required vs Optional PassedArgs fields (tiny checklist)

| Field | Status | Why |
|---|---|---|
| `DataSource` | Required | Most modules need the active datasource instance. |
| `DatasourceName` | Required | Route/module logic frequently keys by datasource name. |
| `Id` | Required | Helps target the selected branch/node context. |
| `EventType` | Required | Signals action intent (`Run`, `Edit`, etc.). |
| `ObjectName` | Optional (recommended) | Improves module context and diagnostics/logging. |
| `CurrentEntity` | Optional (entity flows: required) | Needed for viewer/editor/relation actions on a specific entity. |
| `AddinName` | Optional | Useful when module inspects addin-level metadata. |
| `ObjectType` | Optional | Useful for generic modules with behavior by object type. |

### Suggested Commands To Add in Domain Nodes

- `Get Entities` (load children)
- `Refresh Entities` (re-fetch and rebuild child nodes)
- `Edit <Domain> Connection` (open connection setup UI)
- `Create Category` (create folder and attach under root)
- `Assign To Category` / `Remove From Category` (manage category membership)
- `Create <Domain> Entity` (optional, if domain requires provisioning)
- `Validate <Domain> Structure` (optional integrity check)

## Runtime Add Branches Using ITree (RDBMS Pattern)

Pattern from `DatabaseRootNode` + `DatabaseCategoryNode`:

```csharp
public IErrorsInfo CreateDBNode(ConnectionProperties cn)
{
    try
    {
        var dbNode = new DatabaseNode(
            cn,
            TreeEditor,
            DMEEditor,
            this,
            cn.ConnectionName,
            TreeEditor.SeqID,
            EnumPointType.DataPoint,
            "database.png");

        dbNode.DataSourceConnectionGuidID = cn.GuidID;
        dbNode.GuidID = cn.GuidID;
        dbNode.BranchClass = "RDBMS";

        // Runtime insertion:
        TreeEditor.Treebranchhandler.AddBranch(this, dbNode);
        // Optional sync call:
        // TreeEditor.AddBranchToParentInBranchsOnly(this, dbNode);
    }
    catch (Exception ex)
    {
        DMEEditor.AddLogMessage(ex.Message, "Could not add DB node", DateTime.Now, -1, null, Errors.Failed);
    }
    return DMEEditor.ErrorObject;
}
```

For root rebuild flow:
1. Iterate `ConfigEditor.DataConnections` by category.
2. Skip duplicates (by `GuidID` and existing branch checks).
3. Add via `Treebranchhandler.AddBranch(...)`.
4. Build category branches from `ConfigEditor.CategoryFolders`.
5. `CreateChildNodes()` on each category to hydrate members.

## Shared Helpers You Should Reuse

### DataSourceDefaultMethods

Use `DataSourceDefaultMethods` for standardized datasource entity flows instead of duplicating code in each domain node:
- `GetEntities(IBranch, IDMEEditor, IAppManager)`
- `RefreshEntities(IBranch, IDMEEditor, IAppManager)`

Recommended usage (same style as `DatabaseNode.GetDatabaseEntites()`):

```csharp
[CommandAttribute(Caption = "Get Entities", PointType = EnumPointType.DataPoint, iconimage = "getchilds.png", ObjectType = "Beep")]
public IErrorsInfo GetEntities()
{
    DMEEditor.ErrorObject.Flag = Errors.Ok;
    try
    {
        DataSourceDefaultMethods.GetEntities(this, DMEEditor, Visutil);
    }
    catch (Exception ex)
    {
        DMEEditor.ErrorObject.Flag = Errors.Failed;
        DMEEditor.ErrorObject.Ex = ex;
    }
    return DMEEditor.ErrorObject;
}
```

### NodesHelpers

Use `NodesHelpers` as the central place for shared runtime node operations:
- Category creation helpers
- File/project tree hydration
- Category membership checks (`CheckifBranchExistinCategory(...)`)
- Reusable node creation helpers (`CreateFileNode`, `CreateDataViewNodes`, etc.)

If the same logic appears in 2+ domain node classes, move it into `NodesHelpers` and call it from branch methods.

## Genre Node Pattern (Top-Level Navigation)

For top-level groups (Admin/Data Management), use `EnumPointType.Genre` and keep the node lightweight:
- `ConfigGenereNode`
- `DataManagementGenereNode`

Genre nodes typically:
- Provide discoverability and grouping
- Carry minimal/empty `CreateChildNodes` until functional modules are plugged in
- Use distinct `BranchClass` values (`...GENRE` / `...ROOT`) for schema routing

## Config Folder Pattern

Follow `TheTechIdea.Beep.TreeNodes/Config` structure when building configuration-focused branches:
- `ConfigRootNode` (`EnumPointType.Root`)
- `ConfigCategoryNode` (`EnumPointType.Category`)
- `ConfigEntityNode` (`EnumPointType.Entity`)

This gives a clean, consistent 3-level model:
1. Root (feature area)
2. Category (subgroup)
3. Entity (actionable item)

## When to Use Which BranchType

- `Genre`: high-level navigation headers (no direct datasource behavior)
- `Root`: feature root with child creation orchestration
- `Category`: folder/group node resolving membership from configuration
- `DataPoint`: datasource/connection runtime node
- `Entity`: table/view/item branch node

## Common Pitfalls

- Forgetting to set both `ID` and `BranchID`.
- Adding nodes directly without `Treebranchhandler`.
- Creating children without duplicate checks.
- Mixing root-name/category strings (`VIEW` vs `VIEWS` style mismatches).
- Leaving `SetConfig(...)` empty without minimum assignment.

## Quick Validation Commands

Run from the new project folder:

```powershell
dotnet build
```

Then validate runtime behavior:
- Root appears in the tree.
- Expanding root creates source nodes.
- Expanding source creates entity nodes.
- Re-expanding does not duplicate nodes.
