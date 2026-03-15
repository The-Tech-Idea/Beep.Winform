---
name: branchs-project-creation
description: Build new Beep Branchs modules (for Accounting, CRM, HR, or other domains) that implement IBranch/IBranchID and integrate with ITree/ITreeBranchHandler. Use when creating a new tree-node project from TheTechIdea.Beep.TreeNodes patterns.
---

# Branchs Project Creation

Use this skill when creating a new branch-node project like `TheTechIdea.Beep.TreeNodes` for a domain such as Accounting, CRM, HR, Inventory, or custom application modules.

## Use this skill when
- Creating a new project under `Beep.Branchs` with domain-specific nodes
- Implementing nodes that must satisfy `IBranch`, `IBranchID`, `ITree`, and `ITreeBranchHandler` contracts
- Wiring root/data-point/entity/category branches discovered by addin metadata
- Reusing the architecture style from `TheTechIdea.Beep.TreeNodes`

## Learn-First Rule (Important)
- The referenced classes are primarily **learning/reference sources**.
- First inspect what is already included in the current project or referenced packages.
- If functionality already exists, **use/extend it** instead of recreating the same class.
- Only create new classes when the domain behavior is genuinely missing.

## Do not use this skill when
- You only need to edit one existing node behavior in an existing project
- You are building datasource drivers instead of visual tree nodes
- You are changing core interfaces in `TheTechIdea.Beep.Vis.Modules2.0`
- You plan to duplicate helper/base classes that are already present in the project

## Contract Checklist (Required)
- Implement all identity members from `IBranchID` (`GuidID`, `ParentGuidID`, `DataSourceConnectionGuidID`, `EntityGuidID`, `BranchID`, etc.)
- Implement all required members from `IBranch`:
  - `SetConfig(...)`
  - `CreateChildNodes()`
  - `RemoveChildNodes()`
  - `MenuItemClicked(...)`
  - `CreateCategoryNode(...)`
  - `ExecuteBranchAction(...)`
- Keep `ITree` integration valid:
  - Add nodes through `TreeEditor.Treebranchhandler.AddBranch(...)`
  - Remove/refresh children through `Treebranchhandler.RemoveChildBranchs(...)` and tree refresh methods
- Keep tree identity stable:
  - Set `ID`, `BranchID`, `ParentBranchID`, `BranchClass`, `BranchType`, and `GuidID` consistently

## Beep Vis Metadata (Required)
- Class-level node registration uses `AddinAttribute` from `DataManagementModelsStandard/Vis/AddinAttribute.cs`
- Method-level branch commands use `CommandAttribute` from `DataManagementModelsStandard/Vis/CommandItemAttribute.cs`
- Branch and command point types come from `EnumPointType` in `DataManagementModelsStandard/Vis/Enums.cs`
- Optional display ordering for root collections can use `IOrder` from `DataManagementModelsStandard/Vis/IOrder.cs`
- Keep `AddinAttribute.BranchType` aligned with node role:
  - Root node: `EnumPointType.Root`
  - Connection/data source node: `EnumPointType.DataPoint`
  - Entity node: `EnumPointType.Entity`
- Keep `CommandAttribute.PointType` aligned with where command appears (usually same as the owning node type)

## Baseline Project Pattern
1. Create a multi-target class library project like `TheTechIdea.Beep.TreeNodes` (`net8.0;net9.0;net10.0`).
2. Reference:
   - `TheTechIdea.Beep.DataManagementEngine`
   - `TheTechIdea.Beep.DataManagementModels`
   - `TheTechIdea.Beep.Vis.Modules`
3. Add `globleusings.cs` with common Beep namespaces.
4. Implement at least four node types:
   - Root node (domain entry point)
   - Data-point/source node (connection-level)
   - Category node (folder/grouping under root)
   - Entity node (table/view/object-level)
5. Decorate root and node classes with `[AddinAttribute(...)]`; root nodes also include `[AddinVisSchema(...)]`.
6. Expose actions using `[CommandAttribute(...)]` for menu-driven operations.
7. Prefer shared helper orchestration for repeated behaviors:
   - `DataSourceDefaultMethods` for entity load/refresh flows
   - `NodesHelpers` for cross-domain node/category/project/file utility workflows

## Naming and Structure Conventions
- Use folder-per-domain style:
  - `Accounting/AccountingRootNode.cs`
  - `Accounting/AccountingNode.cs`
  - `Accounting/AccountingEntitiesNode.cs`
  - optional `Accounting/AccountingCategoryNode.cs`
- Keep `BranchClass` constant for the domain (example: `"ACCOUNTING"`).
- Keep `BranchText` user-friendly and stable; use connection names for data-point nodes.
- Use icon names consistently (`*.png` or `*.svg`) and keep resources embedded when needed.

## Behavioral Rules
- `CreateChildNodes()` should be idempotent:
  - avoid duplicate child creation
  - refresh children safely when rerun
- Never bypass `Treebranchhandler` for parent/child modifications.
- Always log failures through `DMEEditor.AddLogMessage(...)`.
- On datasource operations, open/check datasource before entity traversal.
- For destructive operations (remove/clear), ask for confirmation via dialog manager when UI context exists.
- Category nodes should resolve items from `ConfigEditor.CategoryFolders` and add matching data-point children.
- Use `BranchType` as runtime routing state, not just attribute metadata. Logic should branch on `EnumPointType` where appropriate.

## AppManager Navigation Pattern (Use, Do Not Recreate)
- Do not re-implement navigation/show logic inside branch nodes.
- Build `PassedArgs`, then call `Visutil` (`IAppManager`) methods from node commands.
- Preferred routing method for addins is `Visutil.ShowPage(...)`.
- Choose display behavior explicitly when needed:
  - `DisplayType.InControl` for in-container navigation
  - `DisplayType.Popup` for popup navigation
- Reuse other `IAppManager` capabilities when relevant:
  - `NavigateTo(...)` / `NavigateToAsync(...)` for route-driven navigation
  - `ShowUserControlPopUp(...)` for popup user controls
  - `ShowHome()`, `ShowLogin()`, `ShowProfile()`, `ShowAdmin()` for shell-level actions
- Keep this skill aligned with the existing `AppManager` behavior; branch nodes should adapt identifiers/args before calling `Visutil`, not change `AppManager`.

## Function-to-AppManager Mapping Table
Use this table when wiring branch functions to UI actions (based on `RDBMS` and `Config` patterns):

| Branch function intent | Typical node type | AppManager call pattern | Display mode | Notes |
|---|---|---|---|---|
| Edit connection properties | DataPoint (`DatabaseNode`) | `Visutil.ShowPage(configAddinClassName, passedArgs)` | InControl | Use datasource connection info in `PassedArgs` (`DatasourceName`, `ObjectName`, `DataSource`). |
| Open datasource config module | Entity/Config node (`ConfigEntityNode`) | `Visutil.ShowPage(AddinTreeStructure.className, passedArgs)` | InControl | Prefer addin class identifier from node metadata (`AddinTreeStructure`). |
| Open entity viewer/grid | Entity (`DatabaseEntitesNode`-style) | `Visutil.ShowPage(viewerAddinClassName, passedArgs, DisplayType.Popup)` | Popup | Use popup for quick inspect flows without replacing main container view. |
| Open entity editor/designer | Entity (`ConfigEntityNode`-style) | `Visutil.ShowPage(editorAddinClassName, passedArgs)` | InControl | Use when editor should live in main routed container. |
| Open relation builder | DataPoint/Entity | `Visutil.ShowPage("uc_RelationBuilder", passedArgs)` | InControl or Popup | Use popup when action is transient; in-control when part of workflow. |
| Open data viewer | Entity/DataView style | `Visutil.ShowPage("uc_DataViewer", passedArgs)` | Popup (preferred) | Keep `CurrentEntity`, `DatasourceName`, and IDs in args for context. |
| Navigate to shell page (Home/Login/Profile/Admin) | Any command node | `Visutil.ShowHome()` / `ShowLogin()` / `ShowProfile()` / `ShowAdmin()` | N/A | Use for top-level shell navigation, not addin module invocation. |
| Route-first module navigation | Any node with route key | `Visutil.NavigateTo(routeName, parameters)` | Depends on route | Prefer when route is stable and not tied to addin class identity. |

## Extending Branches With Functions
When adding functions to a branch class (example `DatabaseNode`):
1. Add a method and decorate it with `[CommandAttribute(...)]`.
2. Keep the method return type `IErrorsInfo`.
3. Start by setting `DMEEditor.ErrorObject.Flag = Errors.Ok`.
4. Wrap action logic in `try/catch`, log through `DMEEditor.Logger` and set `ErrorObject` on failure.
5. For entity loading/refresh commands, delegate to shared helpers (for example `DataSourceDefaultMethods.GetEntities(...)` or `RefreshEntities(...)`).
6. If wait-form/progress UI is used, always close it in failure paths.
7. Ensure `CreateChildNodes()` can call the function safely (idempotent behavior).

## Runtime Branch Creation via ITree
- Use `TreeEditor.Treebranchhandler.AddBranch(parent, child)` as the primary runtime insertion API.
- Optionally call `TreeEditor.AddBranchToParentInBranchsOnly(parent, child)` when you need in-memory parent list consistency before UI binding catches up.
- Use `TreeEditor.Treebranchhandler.RemoveChildBranchs(branch)` before full rebuilds.
- Call `TreeEditor.RefreshTree(branch)` (or `RefreshTree()`) after bulk runtime add/remove flows.
- Use `TreeEditor.SeqID` for new branch IDs to maintain tree-wide ID uniqueness conventions.

## Genre and Config Entry Nodes
- Use `EnumPointType.Genre` for high-level top navigation groups (example patterns: `ConfigGenereNode`, `DataManagementGenereNode`).
- Genre nodes are usually lightweight and act as discovery/menu anchors, while root/category/data-point/entity nodes hold operational logic.
- For configuration-centric trees, follow `Config` folder patterns:
  - `ConfigRootNode` (root)
  - `ConfigCategoryNode` (category grouping)
  - `ConfigEntityNode` (entity/item leaf)

## Working References
- `TheTechIdea.Beep.Vis.Modules2.0/IBranch.cs`
- `TheTechIdea.Beep.Vis.Modules2.0/IBranchID.cs`
- `TheTechIdea.Beep.Vis.Modules2.0/ITree.cs`
- `TheTechIdea.Beep.Vis.Modules2.0/ITreeBranchHandler.cs`
- `..\BeepDM\DataManagementModelsStandard\Vis\AddinAttribute.cs`
- `..\BeepDM\DataManagementModelsStandard\Vis\CommandItemAttribute.cs`
- `..\BeepDM\DataManagementModelsStandard\Vis\Enums.cs`
- `..\BeepDM\DataManagementModelsStandard\Vis\IOrder.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\Connector\ConnectorRootNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\Connector\ConnectorNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\Connector\ConnectorEntitiesNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\RDBMS\DatabaseNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\RDBMS\DatabaseRootNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\RDBMS\DatabaseCategoryNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\ConfigGenereNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\DataManagementGenereNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\DataSourceDefaultMethods.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\NodesHelpers.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\Config\ConfigRootNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\Config\ConfigCategoryNode.cs`
- `..\Beep.Branchs\TheTechIdea.Beep.TreeNodes\Config\ConfigEntityNode.cs`

## Acceptance Checklist
- Project builds for all target frameworks.
- Root node appears under the expected visual schema root.
- Node expansion creates children with no duplicates.
- Commands are visible and callable from menu/branch actions.
- All required `IBranch`/`IBranchID` members are implemented.
- Remove/refresh operations keep tree state consistent.

## Detailed Reference
Use [reference.md](./reference.md) for starter templates, an Accounting example scaffold, and `PassedArgs` presets (viewer/edit/config/relation).
