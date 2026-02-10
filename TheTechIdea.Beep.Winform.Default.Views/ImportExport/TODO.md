# TODO - Import/Export Wizard (Continue Tomorrow)

Date: 2026-02-10
Target: Convert ImportExport flow to Beep Wizard with 3 guided steps.

## Current Status
- [x] Confirmed Beep wizard framework exists in `TheTechIdea.Beep.Winform.Controls.Wizards`.
- [x] Confirmed wizard supports `UserControl` step content through `IWizardStepContent`.
- [x] Confirmed `DataImportManager` API can run import with `DataImportConfiguration`.
- [x] Added `ImportExportContextStore.cs` to hold step context/mapping keys.
- [x] Implemented step controls (`uc_Import_SelectDSandEntity`, `uc_Import_MapFields`, `uc_Import_Run`) with `IWizardStepContent`.
- [x] Added wizard launcher control (`uc_ImportExportWizardLauncher`) and completion wiring to `DataImportManager`.

## Next Work Items
- [x] Implement `IWizardStepContent` in `uc_Import_SelectDSandEntity`.
  - [x] Load available data sources from `Editor.ConfigEditor.DataConnections`.
  - [x] Load entities via `Editor.GetDataSource(...).GetEntitesList()`.
  - [x] Save selected source/destination + create-if-missing to wizard/context store.

- [x] Implement `IWizardStepContent` in `uc_Import_MapFields`.
  - [x] Read source/destination selection from wizard context.
  - [x] Load structures via `GetEntityStructure(...)`.
  - [x] Build field mapping grid/list.
  - [x] Store mapping as `EntityDataMap` in wizard context.

- [x] Implement `IWizardStepContent` in `uc_Import_Run`.
  - [x] Show summary of selections + mapping count.
  - [x] Add run toggle/option for "Run import on Finish".
  - [x] Validate readiness before finish.

- [x] Add wizard launcher control/form for ImportExport.
  - [x] Build `WizardConfig` with 3 steps.
  - [x] Host step controls in wizard steps.
  - [x] On complete, build `DataImportConfiguration` and call `DataImportManager.RunImportAsync(...)`.

- [ ] Integrate logging/progress to final step UI.
- [ ] Keep old `uc_CopyEntities` path untouched for backward compatibility.

## Build/Verify Checklist
- [x] Build `TheTechIdea.Beep.Winform.Default.Views.csproj`.
- [ ] Validate step-to-step data persistence.
- [ ] Validate mapping roundtrip between steps.
- [ ] Validate import runs and destination entity auto-create behaves correctly.

## Notes
- User direction: use strong typed `ConnectionProperties` primarily; reserve `ParameterList` only for extras.
- For this ImportExport work, focus on guided wizard UX first, then complete import execution wiring.
