# Phase 7 — Advanced Features

**Status:** `[ ]` | **Tasks:** 12 | **Complexity:** High | **Depends on:** Phase 3 (navigation infra), Phase 6 (accessibility)

---

## Objective

Add power-user features that differentiate Beep.Winform from all surveyed wizard implementations: save/resume wizard state, branching navigation, undo/redo, breadcrumb navigation mode, data export/import, auto-play mode, and a fluent builder API.

## Background

- `WizardContext` has dictionary-based data storage (`_data` + `_stepData`) — inherently serializable
- `WizardStep.ShouldSkip` exists for skip-only branching, but no explicit "next step = X if condition Y" API
- `WizardContext.NavigationHistory` is a `Stack<int>` tracking visited steps — good foundation for undo
- **Research:** No surveyed implementation has save/resume wizard state. Beep.Winform's dictionary-based context is uniquely positioned to pioneer this. Blazor.Wizard has the best branching model via `Evaluate`.

## Tasks

### WZ-52 — WizardContext JSON Serialization (Medium)
Add to `WizardContext`:
- `string SaveState()` — serialize `_data`, `_stepData`, `CurrentStepIndex`, `StepValidation`, `NavigationHistory` to JSON via `System.Text.Json`
- `void RestoreState(string json)` — deserialize and restore
- Custom `JsonConverter` for non-serializable objects (store as `"__non_serializable__"` with type name)
- Preserve typed values where possible

**File:** `Core/WizardModels.cs` (WizardContext)

### WZ-53 — Auto-Save on Step Transition (Low)
In `WizardInstance`, call `Context.SaveState()` after each successful navigation. Store to `WizardConfig.AutoSavePath` or `Path.GetTempPath()/BeepWizards/{key}.json`.

**Files:** `Core/WizardInstance.cs`, `Core/WizardModels.cs`

### WZ-54 — Resume Wizard Workflow (Medium)
Add `WizardManager.ResumeWizard(string key)`:
- Load saved state JSON
- Create new `WizardInstance` with original config
- Restore context, navigate to saved step
- Add `WizardConfig.EnableAutoSave` property (default false)

**Files:** `Core/WizardManager.cs`, `Core/WizardModels.cs`

### WZ-55 — Branching Navigation (Medium)
Extend `WizardStep` with:
```csharp
public Func<WizardContext, string> BranchCondition { get; set; }
```
When non-null, `NavigateNextAsync` evaluates the condition to determine the next step key instead of sequential next. Add `WizardConfig.BranchingEnabled`.

**Files:** `Core/WizardModels.cs`, `Core/WizardInstance.cs`

### WZ-56 — Branch Step Model (High)
Create `WizardBranchStep : WizardStep`:
```csharp
public List<WizardBranch> Branches { get; set; }
// WizardBranch: { string Label, string Description, Func<WizardContext, bool> Condition, string TargetStepKey }
```
Wizard form renders branch choices as radio buttons or clickable cards. Navigation follows selected branch. Add branch choice UI to `WizardFormBase`.

**Files:** Create `Core/WizardBranchModels.cs`; modify `Core/WizardInstance.cs`, `Forms/WizardFormBase.cs`

### WZ-57 — Undo Last Step (Medium)
Add `WizardInstance.UndoLastStepAsync()`:
- Reverse the last `NavigateNextAsync` call
- Pop `NavigationHistory` stack
- Restore previous step state to Pending
- Re-enter previous step (call `EnterStepAsync`)
- Add `WizardConfig.EnableUndo` and Undo button to button panel

**Files:** `Core/WizardInstance.cs`, `Core/WizardModels.cs`, `Forms/WizardFormBase.cs`

### WZ-58 — Redo (Medium)
Symmetric to undo: maintain `RedoStack<int>` in `WizardContext`. Redo re-applies the last undone forward navigation.

**Files:** `Core/WizardInstance.cs`, `Core/WizardModels.cs`

### WZ-59 — Breadcrumb Navigation Mode (Medium)
Add `NavigationMode` enum: `Sequential`, `Breadcrumb`. When breadcrumb mode, clicking any step in the path navigates to it (not just completed steps). Distinct from the breadcrumb display style in WZ-23.

**Files:** `Core/WizardEnums.cs`, `Core/WizardInstance.cs`, `Forms/WizardFormBase.cs`

### WZ-60 — Data Export/Import (Low)
```csharp
// WizardContext
Dictionary<string, object> ExportData();  // merged _data + _stepData values
void ImportData(Dictionary<string, object> data);
```
Useful for testing, templates, and inter-wizard data sharing.

**File:** `Core/WizardModels.cs` (WizardContext)

### WZ-61 — Conditional Step Visibility (Medium)
Add `Func<WizardContext, bool> VisibleCondition` to `WizardStep`. Steps where condition returns false are excluded from step indicators and navigation. Different from `ShouldSkip` (which shows steps but skips over them).

**Files:** `Core/WizardModels.cs`, `Core/WizardInstance.cs`, all painters

### WZ-62 — Theme Preference Persistence (Low)
Store last-used theme name in `WizardManager` as static property. New wizards without explicit theme apply the last-used theme.

**File:** `Core/WizardManager.cs`

### WZ-63 — Step Completion History (Medium)
Track per-step completion history:
```csharp
public class StepHistoryEntry { string StepKey; StepState State; DateTime Timestamp; List<string> ValidationErrors; }
```
Expose via `WizardContext.StepHistory` (read-only list). Useful for audit trails and debugging.

**Files:** Create `Core/WizardHistoryModels.cs`; modify `Core/WizardModels.cs`, `Core/WizardInstance.cs`

### Bonus: WZ-64 — Fluent Builder API (Medium)
```csharp
var wizard = WizardBuilder.Create("setup")
    .WithTitle("Application Setup")
    .WithStyle(WizardStyle.HorizontalStepper)
    .AddStep("welcome", "Welcome", "Get started with setup")
    .AddStep("config", "Configuration", "Configure your settings")
    .AddStep("finish", "Complete", "Setup complete")
    .OnComplete(ctx => { /* handle completion */ })
    .Build();
WizardManager.ShowWizard(wizard);
```

**File:** Create `Core/WizardBuilder.cs`

## Acceptance Criteria
- [ ] Wizard state serializes and deserializes round-trip (all typed data preserved)
- [ ] Auto-save fires on each step transition
- [ ] Resume creates wizard at correct step with all data intact
- [ ] Branching navigation routes to different steps based on conditions
- [ ] Undo reverses last step; redo re-applies it
- [ ] Data export produces complete flat dictionary; import seeds context
- [ ] Fluent builder creates and shows a wizard in one chain
- [ ] Step history tracks timestamps and validation errors
- [ ] Build with 0 errors
