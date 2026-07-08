# Phase 3 — Progress & Navigation Enhancement

**Status:** `[ ]` | **Tasks:** 10 | **Complexity:** Medium | **Depends on:** Phase 2 (WizardFormBase + layout managers)

---

## Objective

Implement progress indicators (progress bar, breadcrumb, step count), non-linear step navigation (click completed steps), loading state during async transitions, and branching navigation support.

## Background

- `WizardConfig.ShowProgressBar` exists (default `true`) but is NEVER read — no form renders a progress bar
- `WizardContext.CompletionPercentage` is calculated but never displayed
- `WizardLayoutManager.HitTestStep()` exists but forms never wire click events to it
- `WizardInstance.NavigateToAsync(int)` works but UI has no way to trigger it
- No loading/processing indicator during async transitions
- Breadcrumb-style navigation is a popular pattern (Primer, Sky UI) but not implemented

## Tasks

### WZ-19 — Progress Bar Rendering (Medium)
Add a `BeepProgressBar` or custom-painted progress bar in the header area, respecting `ShowProgressBar`:
- Render between step indicator and content
- Value binds to `Context.CompletionPercentage`
- Wire to `Config.OnProgress` callback
- Style-appropriate rendering per wizard style

**Files:** `Forms/WizardFormBase.cs` (shared), all 4 forms

### WZ-20 — Animated Progress Bar Fill (Low)
Animate the progress bar from old percentage to new percentage over `TransitionDurationMs` using the selected easing function.

**File:** `Helpers/WizardAnimationEngine.cs`, `Forms/WizardFormBase.cs`

### WZ-21 — Step Count Label (Low)
Display "Step X of Y" text in the header/title area. Respect `ShowStepList` config option. Already partially done in `MinimalPainter` — standardize across all styles.

**Files:** All 4 forms, `Core/IWizardFormHost.cs`

### WZ-22 — Non-Linear Step Navigation (Medium)
Wire click events on step indicators to enable jumping to completed steps:
- Use `WizardLayoutManager.HitTestStep(Point)` for hit detection
- Call `WizardInstance.NavigateToAsync(clickedIndex)`
- Only enable for steps with `State == StepState.Completed`
- Visual hover state on clickable steps (cursor change, subtle highlight)

**Files:** All 4 forms, `Layout/WizardLayoutManagers.cs`

### WZ-23 — Breadcrumb Wizard Style (Medium)
Add `WizardStyle.Breadcrumb` to enum. Creates path display: "Step 1 → Step 2 → [Step 3]" where completed steps are clickable links:
- Create `BreadcrumbWizardForm : WizardFormBase`
- Create `BreadcrumbPainter : IWizardPainter`
- Register in `WizardFormFactory`

**Files:** Create `Forms/BreadcrumbWizardForm.cs`, `Painters/BreadcrumbPainter.cs`; modify `Core/WizardEnums.cs`, `Core/WizardFormFactory.cs`

### WZ-24 — Loading State During Async Transitions (Medium)
Add to `IWizardFormHost`:
- `ShowLoading(string message)` — semi-transparent overlay with spinner + message
- `HideLoading()` — removes overlay

Called during `WizardInstance.NavigateNextAsync()` when validation or `OnLeaveAsync` is running. Use a timer: show overlay only if transition takes >200ms (avoid flash for instant transitions).

**Files:** `Core/IWizardFormHost.cs`, `Forms/WizardFormBase.cs`, `Core/WizardInstance.cs`

### WZ-25 — IsNavigating Exposure (Low)
Expose `WizardInstance._isNavigating` as public read-only `IsNavigating`. Disable all nav buttons while navigating. Add visual disabled state.

**File:** `Core/WizardInstance.cs`

### WZ-26 — Step Indicator Tooltips (Low)
Set `ToolTip` on step indicator areas showing `WizardStep.Title` + `WizardStep.Description` + status. Use shared `ToolTip` instance per form. 400ms hover delay.

**Files:** `Forms/WizardFormBase.cs`, all painters

### WZ-27 — Configurable Cancel Confirmation (Low)
Add to `WizardConfig`:
- `bool ConfirmOnCancel { get; set; } = true`
- `string CancelConfirmationMessage { get; set; } = "Are you sure you want to cancel?"`

Replace hardcoded message in all forms.

**Files:** `Core/WizardModels.cs`, `Forms/WizardFormBase.cs`

### WZ-28 — NavigationDirection in Event Args (Low)
Add `NavigationDirection` enum (`Forward`, `Backward`) to `StepChangingEventArgs` and `StepChangedEventArgs`. Set based on whether the navigation is Next or Back.

**File:** `Core/WizardInstance.cs`

## Acceptance Criteria
- [ ] Progress bar renders and animates between steps
- [ ] Clicking a completed step indicator navigates back to it
- [ ] Breadcrumb style renders functional clickable path
- [ ] Loading overlay appears for async transitions >200ms
- [ ] "Step X of Y" displayed in all styles
- [ ] Cancel confirmation is configurable
- [ ] Navigation direction available in event args
- [ ] Build with 0 errors
