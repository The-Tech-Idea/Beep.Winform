# Phase 6 — Accessibility & Keyboard UX

**Status:** `[ ]` | **Tasks:** 8 | **Complexity:** Medium | **Depends on:** Phase 3 (keyboard infrastructure from step navigation)

---

## Objective

Achieve WCAG 2.2 AA compliance: full keyboard navigation, screen reader support, high contrast mode, focus indicators, 44px minimum touch targets, RTL layout support, and keyboard shortcut documentation.

## Background

- Only 2 keyboard shortcuts exist: Enter (next) and Escape (cancel), identically in all 4 forms
- No `AccessibleRole`, `AccessibleName`, or `AccessibleDescription` on any wizard control
- No step-change announcements for screen readers
- No high contrast mode detection
- No RTL support
- Step indicator hit areas below 44px at 100% DPI (circleSize=36 in HorizontalStepper)
- **Research gap:** This is a universal gap in open-source WinForms wizard implementations — even commercial libraries have minimal accessibility. Implementing this makes Beep.Winform best-in-class.

## Tasks

### WZ-44 — Full Keyboard Shortcuts (Low)
Add to `WizardFormBase`:
- `Ctrl+N` — Next
- `Ctrl+B` — Back
- `Ctrl+Enter` — Finish (on last step)
- `Escape` — Cancel
- `F1` — Help
- `Left Arrow` — Back (when not in text field)
- `Right Arrow` — Next (when not in text field)
- `Ctrl+Home` — Jump to first step
- `Ctrl+End` — Jump to last step
- `Tab/Shift+Tab` — Cycle through nav buttons

All implemented as overridable virtual methods.

**File:** `Forms/WizardFormBase.cs`

### WZ-45 — Screen Reader Support (Medium)
- Set `AccessibleRole` on step indicators, navigation buttons, content panel
- On step change, update `AccessibleName` to "Step X of Y: [Title]"
- Call `AccessibilityNotifyClients(AccessibleEvents.Focus, childIndex)` on the content panel
- Announce validation errors via live region equivalent
- Set `AccessibleDescription` with step description

**Files:** `Forms/WizardFormBase.cs`, all 4 painters

### WZ-46 — Focus Ring + Arrow Key Navigation (Medium)
- When user tabs to step indicator area, show visible focus rectangle (2px offset, `theme.OutlineColor`)
- Track `_focusedStepIndex` within step indicators
- Arrow keys navigate between steps within the indicator area
- Enter activates focused step (if clickable/completed)
- Focus returns to content panel after step change

**Files:** `Forms/WizardFormBase.cs`, all 4 painters

### WZ-47 — High Contrast Mode (Medium)
- Detect `SystemInformation.HighContrast` in `ApplyTheme()`
- When active: use system colors instead of theme colors for all painting
- Increase border widths to 2px minimum
- Increase text contrast ratios
- Provide `WizardConfig.HighContrastMode` override

**Files:** `Helpers/WizardHelpers.cs`, all 4 painters, `Forms/WizardFormBase.cs`

### WZ-48 — RTL Support (High)
Add `WizardConfig.RightToLeft` property. When set:
- Set `Form.RightToLeft = RightToLeft.Yes`
- Mirror step indicator order (right-to-left)
- Swap Next/Back button positions
- Reverse slide animation direction
- Mirror card panel layout (right side instead of left)
- Layout managers compute RTL-aware bounds

**Files:** `Core/WizardModels.cs`, `Forms/WizardFormBase.cs`, `Layout/WizardLayoutManagers.cs`, all 4 painters

### WZ-49 — Minimum 44px Touch Targets (Low)
Scale step indicator clickable area to minimum 44x44 logical pixels using `DpiScalingHelper.ScaleValue(44, _host)`. Inflate hit area if circle/dot size is below threshold.

**Files:** `Layout/WizardLayoutManagers.cs`, all 4 forms

### WZ-50 — Keyboard Shortcut Help Overlay (Low)
When F1 is pressed, display a modal or panel showing all available keyboard shortcuts. Respect `WizardConfig.ShowHelp`.

**File:** Create `Forms/WizardShortcutHelpPanel.cs`; modify `Forms/WizardFormBase.cs`

### WZ-51 — Focus Trapping (Low)
Ensure Tab cycles only within the modal wizard form. Verify with explicit `TabStop` management on all controls. Prevent focus from escaping to disabled parent windows.

**File:** `Forms/WizardFormBase.cs`

## Acceptance Criteria
- [ ] All keyboard shortcuts documented in WZ-44 function correctly
- [ ] Narrator/NVDA announces step transitions with title and number
- [ ] Focus ring visible on keyboard-navigated step indicators
- [ ] High contrast Black/White themes render correctly
- [ ] RTL mode mirrors entire wizard layout
- [ ] All step indicator hit areas ≥ 44×44 logical pixels
- [ ] Build with 0 errors
