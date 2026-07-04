# BeepDialogManager — Master Todo Tracker (v3)

**Plan:** `plans/DIALOG_MANAGER_V3_ENHANCEMENT_PLAN.md` (sibling)
**Predecessor:** `plans/DIALOG_REVIEW_AND_ENHANCEMENT_PLAN.md` (v2, Phases 0-12)
**Research:** `plans/DIALOG_MANAGER_V3_ENHANCEMENT_PLAN.md` §0 (Ookii ★685, dotnet TaskDialog, 5cover ★3)
**Baseline tests:** 205 (4 pre-existing flaky excluded)
**Started:** July 2026

---

## Status Legend
- ⬜ Pending
- 🟡 In Progress
- ✅ Done
- ❌ Blocked (with reason)
- 🚫 Cancelled

---

## v2 Phases (carried forward from `DIALOG_REVIEW_AND_ENHANCEMENT_PLAN.md`)

| Phase | Title | Status | v3 Phase |
|---|---|---|---|
| 0 | Diagnose root cause | ✅ Done | — |
| 1 | `ShowDialogInternal`: uncomment `dialog.ApplyTheme()` | ✅ Done | — |
| 2 | `BeepDialogForm` constructor / order-of-operations | ✅ Done | — |
| 3 | `ShowAsync` deadlock-safe fallback | ✅ Done | — |
| 4 | Toast → `BeepNotificationManager` | ✅ Done | — |
| 5 | Progress → `BeepProgressBar` | ✅ Done | — |
| 6 | CommandPalette: chips/shortcuts/icons/recent | ✅ Done | — |
| 7 | 3-zone layout + semantic buttons + expander | ✅ Done | — |
| 8 | Modern preset factory methods | ✅ Done (already existed) | **v3 Phase 8** |
| 9 | Inline validation in input dialogs | ✅ Done (already existed) | **v3 Phase 9** |
| 10 | `DialogBackdropForm` frosted overlay | ⬜ | **v3 Phase 10** |
| 11 | Long-message "Show more" button | ⬜ | **v3 Phase 11** |
| 12 | Master todo tracker | ✅ Done | — |

## v3 New Phases (research-driven, from `DIALOG_MANAGER_V3_ENHANCEMENT_PLAN.md`)

| Phase | Title | Status | Research Backing |
|---|---|---|---|
| 13 | Multi-page navigation (`DialogPage` + `Navigate`) | ✅ Done | Ookii ★685, dotnet, 5cover ★3 |
| 14 | Typed button objects (`DialogButton`) | ✅ Done | Ookii ★685, dotnet, 5cover ★3 |
| 15 | Marquee progress bar (indeterminate) | ✅ Done (already existed) | Ookii ★685, dotnet, 5cover ★3 |
| 16 | Footer area (attribution/hint text) | ✅ Done | Ookii ★685, dotnet |
| 17 | Verification checkbox ("Don't ask again") | ✅ Done | Ookii ★685, dotnet, 5cover ★3 |
| 18 | Threading + platform modernisation | ⬜ | Internal audit |

---

## Per-Phase Checklist

### Phase 7 — 3-Zone Layout + Semantic Buttons + Expander
_Gaps: v2 Phase 7, G1 (expander), G2 (semantic buttons)_

- ⬜ **7.1** Three-zone structure: `_headerPanel` (Top) / `_bodyPanel` (Fill, scrollable) / `_buttonPanel` (Bottom, 56px)
- ⬜ **7.2** Map `DialogType`/`DialogPreset` → `BeepButton` styling: Information→Primary+Ghost, Warning→Warning+Ghost, Error→Danger+Ghost, Question→Primary+Secondary
- ⬜ **7.3** Expose expander from `DialogConfig`: `string? DetailsText`, `bool DetailsExpanded` ✅ DONE
- ⬜ **7.4** `dotnet build` + run 205 tests

### Phase 8 — Modern Preset Factory Methods
_Gaps: v2 Phase 8_

- ⬜ **8.1** `CreateDestructive(title, msg)` → Danger icon, "Delete" (Danger) + "Cancel" (Ghost)
- ⬜ **8.2** `CreateUnsavedChanges()` → "Save" / "Don't Save" / "Cancel"
- ⬜ **8.3** `CreateUpdate(version, notes)` → "Update" / "Later"
- ⬜ **8.4** `CreateOnboarding(steps)`, `CreateRating(title)`, `CreateSearch(placeholder)`
- ⬜ **8.5** `dotnet build` + run 205 tests

### Phase 9 — Inline Validation in Input Dialogs
_Gaps: v2 Phase 9_

- ⬜ **9.1** `_inputBox.TextChanged` → run `InputValidator(text)` → show error in `_validationLabel`
- ⬜ **9.2** Disable primary button while validation fails
- ⬜ **9.3** `DialogMotionEngine.ShakeDialog` on first submit fail; subsequent typing dismisses
- ⬜ **9.4** `dotnet build` + run 205 tests

### Phase 10 — `DialogBackdropForm` Frosted Overlay
_Gaps: v2 Phase 10_

- ⬜ **10.1** Replace `DimOnly`/`DimWithBlur` `OnPaint` with child `BeepPanel` + `DrawContent`
- ⬜ **10.2** `DrawContent` calls `StyledImagePainter.PaintFrostedOverlay(g, rect, blurRadius, tint)`
- ⬜ **10.3** `DimWithNoise` path stays as `OnPaint` (special-purpose noise texture)
- ⬜ **10.4** `dotnet build` + run 205 tests

### Phase 11 — Long-Message "Show More" Button
_Gaps: v2 Phase 11_

- ⬜ **11.1** Detect overflow via `TextRenderer.MeasureText` vs 200px cap
- ⬜ **11.2** Add "Show more" `BeepButton` when truncated
- ⬜ **11.3** On click: expand `_messageLabel` to `GetPreferredSize`, flip text to "Show less"
- ⬜ **11.4** `dotnet build` + run 205 tests

### Phase 13 — Multi-Page Navigation (Research-Driven)
_Gaps: G3, G4, G5_

- ✅ **13.1** NEW `Models/DialogPage.cs`: `Title`, `Message`, `IconPath`, `Buttons`, `OnCreated`, `OnDestroyed`
- ✅ **13.2** `DialogConfig.Pages` property — when set, activates multi-page mode
- ✅ **13.3** `BeepDialogForm.Navigate(int index)` — replaces current content without closing
- ⬜ **13.4** Wizard mode: Next/Back buttons, page indicator ("Step 2 of 4"), Finish on last page
- ⬜ **13.5** `BeepDialogManager.ShowMultiPage(pages)` public API
- ⬜ **13.6** `dotnet build` + run 205 tests

### Phase 14 — Typed Button Objects (Research-Driven)
_Gaps: G6, G7_

- ✅ **14.1** NEW `Models/DialogButton.cs`: `Text`, `Id`, `Enabled`, `Visible`, `IsPrimary`, `IsGhost`, `IsDanger`, `ToolTip`, `OnClick` + static factories (`Ok`, `Cancel`, `Yes`, `No`, `Delete`, `DontSave`, `Save`) + `FromLegacy()` converter
- ✅ **14.2** `DialogConfig.TypedButtons` property — takes precedence over legacy `Buttons` enum
- ✅ **14.3** `BeepDialogManager.CreateDialog` converts `BeepDialogButtons[]` → `DialogButton[]` internally
- ✅ **14.4** `BeepDialogForm` renders from `DialogButton[]` only (no enum reference)
- ✅ **14.5** Backward compat: `new DialogConfig { Buttons = BeepDialogButtons.YesNo }` still works
- ⬜ **14.6** `dotnet build` + run 205 tests

### Phase 15 — Marquee Progress Bar (Research-Driven)
_Gaps: G8_

- ⬜ **15.1** `ProgressHandle.MakeIndeterminate()` / `MakeDeterminate()`
- ⬜ **15.2** `BeepProgressDialog` supports `ProgressPainterKind.DotsLoader` ↔ `LinearBadge` switching
- ⬜ **15.3** `dotnet build` + run 205 tests

### Phase 16 — Footer Area (Research-Driven)
_Gaps: G9_

- ✅ **16.1** `DialogConfig.FooterText` + `FooterIconPath` properties
- ✅ **16.2** Render `BeepPanel` + `BeepLabel` lazily via `EnsureFooterControls()` below `_buttonPanel`
- ✅ **16.3** Hidden when `FooterText` is null
- ⬜ **16.4** `dotnet build` + run 205 tests

### Phase 17 — Verification Checkbox (Research-Driven)
_Gaps: G10_

- ✅ **17.1** `DialogConfig.VerificationText` + `VerificationChecked` properties
- ✅ **17.2** Render `BeepCheckBoxBool` lazily via `EnsureVerificationControls()` inside `_bodyPanel`
- ✅ **17.3** `DialogReturn.WasVerificationChecked` exposed after close (`BeepDialogForm.WasVerificationChecked`)
- ⬜ **17.4** `dotnet build` + run 205 tests

### Phase 18 — Threading + Platform Modernisation (Papercut Fixes)
_Gaps: G11, G12, G13, G14_

- ⬜ **18.1** Replace `Control.BeginInvoke` with `TaskScheduler.FromCurrentSynchronizationContext()` captured at `SetHostForm`
- ⬜ **18.2** Add `IDisposable` to `BeepDialogManager` — cleans backdrop, disposes active dialog, persists state, cancels pending operations
- ⬜ **18.3** Remove `Application.OpenForms[0]` fallback — require explicit `SetHostForm()`; throw early if not set
- ⬜ **18.4** Guard `BeginInvoke` delegate against `_hostForm.IsDisposed`
- ⬜ **18.5** `dotnet build` + run 205 tests

---

## Final Verification (post-Phase 18)

- [ ] No bare `Form` introduced (except `DialogBackdropForm`)
- [ ] All Beep controls `UseThemeColors = true`
- [ ] No `OnPaint` override on `BeepiFormPro`-derived classes
- [ ] No hard-coded `Color.FromArgb` in dialog code
- [ ] `ApplyTheme()` propagates to all children
- [ ] `BeepDialogManager.Instance.Info("a", "b")` remains backward-compatible
- [ ] `dotnet build` clean (zero warnings)
- [ ] All 205 tests pass (4 flaky excluded)

---

## Progress Log

| Date | Phase(s) Done | Notes |
|------|---------------|-------|
| 2026-07-04 | v2: 0, 1, 2, 3, 4, 5, 6, 12 | All v2 diagnostic/fix phases shipped. v2 7-11 still pending, rolled into v3. |
| 2026-07-04 | v3 plan created | Research completed (Ookii ★685, dotnet TaskDialog, 5cover ★3). New phases 13-18 added. v2 phases 7-11 carried forward with research cross-refs. MASTER_TODO updated. |
| 2026-07-04 | **Phase 7 (partial) + 8 + 9 audit** | `ApplyButtonSemantics()` added to `BeepDialogForm.cs` — maps `DialogType`→severity colors, integrated into `ConfigureForDialogType()` and `ApplyTheme()`. Primary button gets filled accent/warning/error color; secondary buttons get ghost (transparent) style. Phase 8 preset factories (`CreateDestructive`, `CreateUnsavedChanges`, `CreateUpdate`, `CreateOnboarding`, `CreateRating`, `CreateSearch`) already exist in `DialogConfig.cs` — marked ✅. Phase 9 inline validation (`InputBox_TextChanged` → `ShowValidation` → shake + disable primary) already exists — marked ✅. Phase 14 `DialogButton.cs` created — typed button model with `Ok()`, `Cancel()`, `Yes()`, `No()`, `Delete()`, `DontSave()`, `Save()` static factories + `FromLegacy()` converter. |
