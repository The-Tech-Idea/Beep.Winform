# Phase 5 — Design-Time Experience

**Status:** `[ ]` | **Tasks:** 7 | **Complexity:** Medium | **Depends on:** Phase 4 (painters finalized)

---

## Objective

Make wizard configuration intuitive in the Visual Studio designer: smart tags for quick settings, step collection editor with reorder, live style/theme preview, design-time sample data, and toolbox integration.

## Background

- All 4 forms have parameterless constructors with "Design Mode" data (3 dummy steps) — good foundation
- `WizardConfig` uses `ExpandableObjectConverter` for property grid expansion
- No smart tag (DesignerActionList) exists
- `WizardPage` has `[ToolboxItem(true)]` but could benefit from better designer integration
- **Research:** Actipro WinForms is best-in-class with New Page Wizard, live Next/Back navigation in designer, Go To Page dialog
- Beep.Winform already has design-time infrastructure (`WizardPageDesigner`, `WizardConfigActionList`, `WizardStepCollectionEditor`, `WizardStepContentEditor` in the design-server project)

## Tasks

### WZ-37 — WizardStepCollectionEditor (Medium)
Custom `CollectionEditor` with Add/Remove/Move Up/Move Down buttons. Shows step Title, Key, State in a multi-column list. Apply `[Editor(typeof(WizardStepCollectionEditor), typeof(UITypeEditor))]` on `WizardConfig.Steps`.

**File:** Create `Design/WizardStepCollectionEditor.cs`; modify `Core/WizardModels.cs`

### WZ-38 — Smart Tag / DesignerActionList (Medium)
Create `WizardActionList` with actions:
- **Style:** dropdown of all WizardStyle values (live update)
- **Add Step / Remove Step**
- **Edit Steps...** (opens collection editor)
- **Step Count:** numeric up-down
- **Show Progress Bar / Show Step List** checkboxes

Apply `[Designer(typeof(WizardFormDesigner), typeof(IRootDesigner))]` to WizardFormBase.

**Files:** Create `Design/WizardActionList.cs`, `Design/WizardFormDesigner.cs`; modify `Forms/WizardFormBase.cs`

### WZ-39 — Enhanced Design-Time Data (Low)
Replace generic "Step 1/2/3" with realistic names per style:
- HorizontalStepper: "Account Setup", "Profile Details", "Confirmation"
- VerticalStepper: "Select Database", "Configure Connection", "Test & Finish"
- Cards: "Choose Plan", "Billing Info", "Review Order"

**Files:** All 4 form parameterless constructors

### WZ-40 — Live Theme Switching (Low)
When `CurrentTheme` changes at design time, immediately repaint all step indicators. Wire `ApplyTheme()` to designer property change notifications. Use `IComponentChangeService` to detect theme property changes.

**File:** `Forms/WizardFormBase.cs`

### WZ-41 — WizardPage Toolbox Integration (Low)
Ensure `WizardPage` has `[ToolboxBitmap]` attribute. Add XML toolbox registration if needed for the VS toolbox to show it.

**File:** `Forms/WizardPage.cs`, project `.csproj`

### WZ-42 — Design-Time Serialization (Low)
`WizardStep.Content` is `[Browsable(false)]` which prevents design-time binding. Add design-time proxy or `DesignerSerializationVisibility(Content)` on the Steps collection. Document the programmatic configuration approach.

**File:** `Core/WizardModels.cs`

### WZ-43 — Empty-Steps Warning (Low)
When `WizardConfig.Steps.Count == 0` in design mode, paint a warning overlay: "No steps defined. Add steps using the Steps collection editor."

**Files:** All 4 form parameterless constructors

## Acceptance Criteria
- [ ] Smart tag appears with Style, Step Count, Show Progress Bar options
- [ ] Steps collection editor supports reorder via Move Up/Move Down
- [ ] All 4 styles render with realistic sample data in designer
- [ ] Theme change at design time immediately repaints
- [ ] WizardPage visible in VS toolbox
- [ ] Build with 0 errors
