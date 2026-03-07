## Plan: Dialog Manager 2026 Preset Modernization

Modernize `BeepDialogManager` with Figma-aligned dialog presets, web-grade accessibility behavior, and production-safe interaction patterns while preserving existing Beep control/theming conventions. Reuse the current partial-class architecture and add a preset/token layer, interaction policy layer, and validation/content patterns that match Material/Radix/NNGroup guidance.

**Implementation Status (2026-03-05)**
1. Completed: modern preset model expansion (`DestructiveConfirm`, `UnsavedChanges`, `BlockingError`, `SessionTimeout`, `InlineValidation`, `SuccessWithUndo`, `MultiStepProgress`, `Announcement`).
2. Completed: new `DialogConfig` factories and safer default labels/policies for destructive, unsaved-changes, update, session-timeout, and blocking-error flows.
3. Completed: runtime wiring from `DialogConfig` to `BeepDialogForm` for custom labels, details text, close policy, default action, and typed-confirmation controls.
4. Completed: progressive details disclosure (`Show details` toggle), typed confirmation gating, and primary-action enablement control.
5. Completed: keyboard interaction hardening (focus loop with `Tab`/`Shift+Tab`, ESC policy enforcement, Enter preferring configured default action).
6. Completed: focus return restoration to previous owner control after dialog closes.
7. Completed: preset-aware icon mapping and semantic button coloring in dialog theme application.
8. Completed: scroll-safe long-content body reflow in `BeepDialogForm` with dynamic message/details sizing and auto-scroll minimum size.
9. Completed: command-palette parity features (favorites-first ordering, persistent recents, empty state, shortcut hint refresh).
10. Completed: progress modernization in `BeepDialogManager.Progress.cs` with semantic states (`Pending`, `InProgress`, `Completed`, `Failed`, `Retrying`) and safer UI refresh behavior.
11. Completed: notification modernization in `BeepDialogManager.Notifications.cs` with severity-based priority/lifetime policy and dedupe-capable toast/snackbar flows.
12. Completed: added optional advanced notification policy object (`NotificationPolicy`) with channel-level defaults, dedupe windows, default positions, and priority/lifetime tuning wired into toast/snackbar flows.
13. Completed: adopted policy/dedupe-aware APIs for delete and overwrite callsites (`ConfirmDestructiveWithUndoAsync`, `SaveFileWithConfirm`, and `ConfirmOverwriteAsync`) including deduped undo snackbar and consequence-label overwrite prompts.
14. Completed groundwork: added import/export policy-aware notification helpers (`NotifyImportSuccess`, `NotifyImportFailure`, `NotifyExportSuccess`, `NotifyExportFailure`) for dedupe/severity-consistent rollout.
15. Completed: wired import/export workflow callsites in `BeepFilter` to use dialog-manager file dialogs and `NotifyImport*` / `NotifyExport*` helpers with dedupe-aware severity behavior.
16. Completed: propagated adoption pattern to additional placeholder workflows by replacing direct `MessageBox` feedback in `BeepFilter` keyboard placeholder actions and `BeepGridPro` export placeholders with dialog-manager deduped notifications.
17. Completed: added reusable `NotifyFeaturePending(...)` helper in dialog manager and refactored recent placeholder callsites (`BeepFilter`, `BeepGridPro`) to centralize pending-feature messaging and dedupe behavior.
18. Completed: migrated remaining direct `MessageBox` notification callsites in `BeepFilter` keyboard workflows to dialog-manager helpers (`NotifyFeaturePending`, `ShowInfo`).
19. Completed: migrated all GridX helper-level `MessageBox` callsites to dialog-manager notifications — `BeepAdvancedFilterDialog` (save/load via `NotifyExportSuccess`/`NotifyImportSuccess`/`NotifyImportFailure`), `GridDialogHelper.InlineCriterionMenu` (validation warnings via `ToastDeduped`), and `GridClipboardHelper` (error catch blocks via private `NotifyClipboardError` helper using `ToastDeduped`). Verified zero `MessageBox.Show` calls remain anywhere in `TheTechIdea.Beep.Winform.Controls`.
20. Convention established: `BeepSimpleGrid` is not to be used; all grid usage must go through `BeepGridPro`. Grid-related notification stubs in legacy `BeepSimpleGrid` code are excluded from migration scope.
21. Completed: wrote notification usage cookbook at `DialogsManagers/NOTIFICATIONS.md` covering `NotifyFeaturePending`, `NotifyImport*`/`NotifyExport*`, `ToastDeduped` validation-warning pattern, private-helper error-catch pattern, snackbar-undo, dedupe key naming convention, and anti-patterns table. Phase 7 (Integration + migration) is now complete — zero `MessageBox.Show` calls remain in `TheTechIdea.Beep.Winform.Controls`.
22. Completed: implemented `AutoCloseTimeout` feature — `DialogConfig.AutoCloseTimeout` (previously declared but wired to nothing) is now fully functional. `BeepDialogForm.StartAutoClose(int timeoutMs)` starts a 1-second ticker that appends a countdown to the primary button label (e.g. "OK (5)") and auto-dismisses as Cancel when the timer expires. `BuildDialogForm` in `BeepDialogManager.Core.cs` wires it from the config. `Dispose(bool)` in `BeepDialogForm` cleans up the timer safely. Updated `NOTIFICATIONS.md` cookbook with auto-close usage pattern.
23. Completed: added `DialogStyleAdapter.GetPresetPrimaryColor(DialogPreset, IBeepTheme?)` and `DialogStyleAdapter.GetPresetHeaderTint(DialogPreset, IBeepTheme?)` helpers. Refactored `BeepDialogForm.ApplyTheme()` to use both — inline preset-switch color logic is gone from the form, all preset-to-color resolution is now centralized in `DialogStyleAdapter`. Header panel receives a subtle 18-alpha tint (red/amber/green) for semantic presets; neutral presets leave the header untinted. Added `using DialogsManagers.Helpers` to `BeepDialogForm.cs`. Both files validate clean.
24. Next: write the Phase 8 Figma-to-code mapping section — token names, component anatomy (header / body / footer slots), and interaction state table — as a new `DialogsManagers/DIALOG_DESIGN_TOKENS.md` reference document.

**Steps**
1. Phase 1: Baseline and guardrails
2. Define non-regression baseline from existing plans and current implementation in `DialogsManagers/plans/DIALOG_SYSTEM_FIX_PLAN.md` and `DialogsManagers/plans/DIALOG_ENHANCEMENT_PLAN.md`.
3. Freeze compatibility scope: keep `IDialogManager` signatures unchanged; implement all modernization behind `DialogConfig`, `DialogPreset`, and manager internals.
4. Add explicit support matrix in plan for modal, non-modal, toast/snackbar, progress, and command-palette patterns.
5. Phase 2: Figma preset system (depends on 1)
6. Extend `DialogPreset` and `DialogConfig` with modern semantic presets: `DestructiveConfirm`, `UnsavedChanges`, `BlockingError`, `SessionTimeout`, `InlineValidation`, `SuccessWithUndo`, `MultiStepProgress`, `Announcement`.
7. Add preset tokens in `DialogStyleAdapter` for icon/background/button emphasis tiers, spacing, radius, and state variants; map tokens to `BeepControlStyle` without bypassing theme manager.
8. Add explicit action-label policy: avoid `Yes/No`, use consequence labels (`Delete File`, `Keep File`, `Discard Changes`, `Stay`).
9. Phase 3: Interaction behavior hardening (depends on 1, parallel with 2)
10. Standardize accessibility/UX interaction contract in manager + form: focus trap, initial focus target, escape behavior policy, backdrop click policy, and focus return target.
11. Add configurable close-guard rules for destructive presets (`RequireTypedConfirm`, `DisablePrimaryUntilAcknowledged`, optional countdown for irreversible actions).
12. Add scroll-safe content behavior and long-message progressive disclosure section (summary + expandable details).
13. Phase 4: Form composition upgrade (depends on 2,3)
14. Refactor `BeepDialogForm` content zones into preset-aware slots: header (severity icon + title + context chip), body (message/details/custom control), footer (button groups with semantic hierarchy).
15. Add validation rail and helper-text slots for input presets; connect to `DialogConfig.FieldValidators`/`ValidationState` for real-time feedback.
16. Ensure all icons/images remain `ImagePath`/`StyledImagePainter` compatible and all controls remain Beep controls.
17. Phase 5: Progress/notification parity with web patterns (depends on 2)
18. Replace remaining progress GDI look with `BeepProgressBar`-based visual variants and status labels (`Pending`, `In Progress`, `Completed`, `Failed`, `Retrying`).
19. Expand snackbar/toast patterns for undo, stacking priority, dedupe key, and lifetime policy by severity.
20. Phase 6: Command palette dialog preset parity (parallel with 5)
21. Add modern command palette affordances: category chips, shortcut hints, recent actions, and empty/error/loading states aligned with same token system.
22. Phase 7: Integration + migration (depends on 2-6)
23. Add non-breaking factory helpers in `DialogConfig` for new presets and map old presets forward.
24. Update callsites gradually: keep old methods operational, adopt new preset factories in high-risk flows first (delete/overwrite/unsaved changes).
25. Phase 8: Documentation and design handoff (depends on all)
26. Create a Figma-to-code mapping section in the plan artifact with token names, component anatomy, and interaction state table.
27. Document preset usage cookbook and anti-patterns (overuse of confirmation, generic CTA labels, unsafe defaults).

**Relevant files**
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\BeepDialogManager.Core.cs` — orchestration, async/sync flow, backdrop/modal behavior policies.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\Forms\BeepDialogForm.cs` — slot-based UI composition, button hierarchy, details expander, validation rail.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\Models\DialogConfig.cs` — preset config surface, accessibility/focus options, destructive guard options.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\Models\DialogPreset.cs` — new preset enum values and semantic intent mapping.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\Helpers\DialogStyleAdapter.cs` — Figma token mapping to Beep theme/style.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\Forms\DialogBackdropForm.cs` — backdrop policy by preset (dismissibility/blur/opacity tiers).
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\BeepDialogManager.Progress.cs` — progress dialog modernization with Beep controls.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\BeepDialogManager.Notifications.cs` — toast/snackbar policy alignment.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\CommandPalette\BeepCommandPaletteDialog.cs` — command preset parity.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\DialogsManagers\plans\DIALOG_ENHANCEMENT_PLAN.md` — prior roadmap reference; avoid duplicate scope.
- `c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\plans\dialog-manager-figma-web-enhancement-plan.md` — target artifact path requested by user.

**Verification**
1. Preset matrix validation: verify each new preset resolves correct icon, button set/order, default focus target, dismiss policy, and severity style.
2. Accessibility validation: keyboard-only run for `Tab`, `Shift+Tab`, `Enter`, `Esc`; verify focus trap and return to invoking control.
3. Destructive flow validation: ensure no default destructive action, explicit consequence labels, and optional typed confirmation on high-risk operations.
4. Content stress validation: long title/message/details, multiline inputs, custom controls, and small-screen host forms.
5. Theme/style validation: run through key `BeepControlStyle` values and ensure preset tokens remain theme-compliant.
6. Regression validation: existing `IDialogManager` consumers compile and behave the same unless explicitly opting into new presets.

**Decisions**
- Include: preset system modernization, interaction policy hardening, accessibility parity, tokenized Figma alignment, and non-breaking migration.
- Exclude: changing `IDialogManager` public contract, replacing Beep controls with non-Beep UI controls, or introducing external UI frameworks.
- Constraint: all visual media stays path-based (`ImagePath`) and theme propagation remains centralized.

**Further Considerations**
1. Figma source of truth: Option A use existing internal Beep design tokens, Option B import a dedicated dialog token sheet, Option C hybrid (recommended).
2. Destructive safeguards level: Option A simple confirm labels, Option B typed-confirm only for severe actions (recommended), Option C typed-confirm for all destructive actions.
3. Rollout strategy: Option A big-bang replace, Option B feature-flag by preset family (recommended), Option C only apply to new dialogs first.