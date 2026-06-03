---
name: winform-integrated-ide
description: Overview of the Beep DataBlocks Navigator VS extension that integrates WinForms Integrated Controls (BeepBlock, BeepForms, BeepDataConnection) into Visual Studio. Use when modifying the IDE tool window, navigator providers, dialogs, or workflow coordinators.
---

# WinForm Integrated IDE Extension Guide

Use this skill as the entry point for the Visual Studio extension that manages Oracle Forms-style data blocks in the IDE.

## Project Location
- `Beep.Desktop\TheTechIdea.Beep.Desktop.IDE.Extensions\`
- Extension ID: `TheTechIdea.Beep.Winform.IDE.Extensions.07e039f1-55fd-425d-90d0-fe25b37967b3`
- VS Extensibility SDK (not VSIX/COM): `Microsoft.VisualStudio.Extensibility.Sdk`

## Three-Layer Architecture

```
┌─ IDE Extensions (VS Tool Window) ────────────────────────────┐
│  BeepDataBlocksNavigator (30+ partial files)                  │
│  Navigator Providers (ConnectionProvider, FormProvider)        │
│  Workflow Coordinators (Connection, Form, BlockItem)           │
│  Dialogs (20+ WPF RemoteUserControl dialogs)                  │
│  DesignerCodeGenerator (regex-based Designer.cs manipulation)  │
│  WinFormsScanner (Designer.cs parser)                          │
│  NuGetPackageService (driver/package download)                 │
├───────────────────────────────────────────────────────────────┤
│  WinForm Integrated Controls                                  │
│  BeepForms, BeepBlock, BeepDataConnection                     │
├───────────────────────────────────────────────────────────────┤
│  BeepDM Engine                                                 │
│  FormsManager, SharedContextAssemblyHandler, IDataSource        │
└───────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. BeepDataBlocksNavigator (Tool Window)
- **30+ partial class files** on `BeepDataBlocksNavigatorData` (the ViewModel)
- Four-zone workbench: Navigator (left), Workspace (center), Inspector (right), Output (bottom)
- Uses `RemoteUserControl` with `DataTemplate`-based XAML
- Provider-based tree: `ConnectionNavigatorProvider` + `FormNavigatorProvider`

### 2. Navigator Providers
- **ConnectionNavigatorProvider**: `Connections` → `Category (RDBMS/FILE/WEBAPI/NOSQL)` → `Connection` → `Entities` → `Fields`
- **FormNavigatorProvider**: `Forms` → `Form` → `Blocks` → `Items`

### 3. Workflow Coordinators
- **ConnectionWorkflowCoordinator**: Open/close connections, add/edit/delete, test connection
- **FormWorkflowCoordinator**: Create form from template, scan projects, set preferred form
- **BlockItemWorkflowCoordinator**: Create/edit/delete blocks, scaffold fields, manage LOVs/triggers/validation

### 4. Dialog Pattern
Every dialog follows a 3-file pattern:
- `*Dialog.cs` — `RemoteUserControl` wrapper
- `*Dialog.xaml` — Embedded `DataTemplate` XAML
- `*DialogData.cs` — ViewModel inheriting `NotifyPropertyChangedObject`

Key dialogs:
- `ConnectionEditorDialog` — Add/edit datasource connections
- `BlockCreationDialog` / `BlockFieldsEditorDialog` — Block definition editing
- `BlockEntityEditorDialog` / `BlockMetadataEditorDialog` — Entity metadata
- `BlockNavigationEditorDialog` — Navigation bar configuration
- `QueryBuilderDialog` — Query string builder
- `MasterDetailRelationshipEditor` — Master-detail relationship config
- `TriggerEditorDialog` / `LOVEditorDialog` / `ValidationRulesEditorDialog`
- `FormTemplateSelectorDialog` / `FormPropertiesEditorDialog`

### 5. DesignerCodeGenerator
- Regex-based code generation for `*.Designer.cs` files
- Marker-region management (copilot-generated blocks)
- Supports: BeepDataConnection, BeepForms, BeepBlock definitions, bootstrap registrations, relationships, LOVs, validation, triggers

### 6. WinFormsScanner
- Scans solution/project for WinForms forms and their designer files
- Parses `Designer.cs` to discover: legacy `BeepDataBlock`, modern `BeepBlock`, `BeepForms` hosts, definition-based blocks
- Extracts entity metadata, field definitions, navigation config, master-detail relationships, LOVs, triggers, validation rules

## Known Gaps (Tracker Items)

| Item | Description |
|------|-------------|
| I1 | `CreateDataBlock` opens datasources synchronously |
| I2 | No Roslyn-based code generation (regex only) |
| I4 | Boomerang coupling: generator ↔ scanner pattern mismatch |
| I6 | Connection editor bypasses `IConnectionStorageProvider` |
| I9 | Connection test bypasses `SharedContextAssemblyHandler` |
| I10 | No "Install Driver" button in connection dialog |
| N2 | IDE NuGet ops bypass `SharedContextAssemblyHandler` |

## File Organization
```
IDE.Extensions/
├── Dialogs/          (20+ WPF dialogs, 3 files each)
├── Helpers/          (DesignerCodeGenerator, WinFormsScanner, converters)
├── Infrastructure/   (NuGetPackageService, DriverPackageTracker, theme)
├── Navigator/
│   ├── Models/       (DataBlockTreeNode, NavigatorProviderContext)
│   ├── Providers/    (ConnectionNavigatorProvider, FormNavigatorProvider)
│   └── Workflows/    (ConnectionWorkflowCoordinator, FormWorkflowCoordinator, BlockItemWorkflowCoordinator)
├── Services/         (BeepDataBlockService, BlockStatusUpdater, FormValidator, ObjectSynchronizer)
├── Templates/        (FormTemplateManager)
├── ToolWindows/      (PropertyPalette)
└── BeepDataBlocksNavigatorData.*.cs  (30+ partial class files)
```

## Related Skills
- [`datablock-connection-integration`](../datablock-connection-integration/SKILL.md) — Block-connection wiring
- [`designer-code-generator`](../designer-code-generator/SKILL.md) — Code generation details
- [`forms`](../forms/SKILL.md) — FormsManager orchestration
- [`shared-context-assemblyhandler`](../shared-context-assemblyhandler/SKILL.md) — Driver/NuGet loading

## Detailed Reference
Use [`reference.md`](./reference.md) for end-to-end scenarios.
