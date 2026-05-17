# Phase 09 — Runtime Wizard Demo (Acceptance Sample)

## Phase Metadata
- Phase: `09`
- Program: `DocumentHost MDI Commercial-Grade Enhancement`
- Tracking Prefix: `DOCMDI-P9-`
- Scope:
  - `C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform.Sample\WinFormsApp.UI.Test\DocumentHostMdiDemoForm.cs` (new)
  - `C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform.Sample\WinFormsApp.UI.Test\Program.cs` (`--demo document-host-mdi` routing)
- Status: `Shipped — Wave 1 (Tabbed / Browser / Native MDI mode switch + seeded samples)`
- Owner: Design-time UX
- Predecessors:
  - Phase 07 — Design-Time First (`DOCMDI-P7-001..009`)
  - Phase 08 — Design-Time Wizard Polish (theme + a11y)

## Goal
Close the loop on Phase 07 + Phase 08 by giving the wizard a **runnable
acceptance target**. Without a sample, the "drop-once, wizard-Apply, fully-
wired host" pipeline can only be exercised inside Visual Studio's designer
process — there was no programmatic surface for QA, doc screenshots, or
hands-on theme validation.

The sample addresses three concrete asks from the Phase 07/08 reviews:

1. **Round-trip the wizard at runtime.** Open `DocumentSetupWizardDialog`,
   read back the `DocumentSetupResult`, and prove it can build the same
   manager/view/host trio the designer assembles at design time.
2. **Validate `BeepThemesManager` ↔ `WizardPalette` integration on a real
   form.** Switching theme before the demo launches must produce a wizard
   chrome that matches the host application.
3. **Demonstrate Tabbed ↔ Browser ↔ Native MDI mode switching at runtime**
   using `BeepDocumentManager.View = …` without leaking state or freezing
   the UI.

## TODO Checklist

### Runtime acceptance surface
- [x] `DOCMDI-P9-001` Add `DocumentHostMdiDemoForm` to `WinFormsApp.UI.Test`.
- [x] `DOCMDI-P9-002` Wire `--demo document-host-mdi` route in `Program.TryRunRequestedDemo`.
- [x] `DOCMDI-P9-003` Auto-trigger the wizard on first launch (`BeginInvoke` so the form is sized before the modal opens).
- [x] `DOCMDI-P9-004` Toolbar verbs: "Run Setup Wizard…", "Add Document", "Close All".
- [x] `DOCMDI-P9-005` Status strip reports active mode + open document count, updated on every change.

### Wizard pipeline parity
- [x] `DOCMDI-P9-010` `RunWizard` passes the currently-applied mode as `initialMode` so the wizard re-opens on the same tile.
- [x] `DOCMDI-P9-011` `RunWizard` passes `_manager.DocumentCount` as `existingDocumentCount` so the wizard suppresses the "add sample tabs" checkbox once documents exist (matches the designer's hasExistingDocs branch).
- [x] `DOCMDI-P9-012` `ApplySetupResult` swaps `BeepDocumentManager.View` between `BeepTabbedView` (Tabbed / Browser) and `BeepNativeMdiView` without recreating the form.
- [x] `DOCMDI-P9-013` Browser-style tweak applied via `TrySet` reflection helper so the demo never breaks on property renames.
- [x] `DOCMDI-P9-014` `SeedSamples` uses `BeginBatchAddDocuments` / `EndBatchAddDocuments` for the same one-layout-pass guarantee the designer's `SeedSampleDocuments` provides.
- [x] `DOCMDI-P9-015` `BeepNativeMdiView.DocumentFormCreated` subscriber gives each MDI child a default size, background, and content label so empty children don't look broken.

### Accessibility carry-over
- [x] `DOCMDI-P9-020` Form, toolbar, host, status strip all have `AccessibleName` set.

### Verification
- [x] `DOCMDI-P9-030` `dotnet build WinFormsApp.UI.Test.csproj` succeeds (0 errors).
- [x] `DOCMDI-P9-031` Lints clean on the new + edited files.

## Files Touched
- `WinFormsApp.UI.Test/DocumentHostMdiDemoForm.cs` — new sample form (~270 lines).
- `WinFormsApp.UI.Test/Program.cs` — added `document-host-mdi` route.
- `.plans/MASTER-TODO-TRACKER.md` — `DOCMDI-NEXT-018` (this slice).

## Deferred / Backlog
- [ ] `DOCMDI-P9-040` Browser mode: apply true browser-style preset (tab style enum, etc.) via `_host.TabStyle = ...` once the smart-tag preset helper is extracted into a public runtime API.
- [ ] `DOCMDI-P9-041` "Apply layout template" leg of `DocumentSetupResult` is currently ignored at runtime — wire it once the template catalogue ships (see Phase 08 `DOCMDI-P8-036`).
- [ ] `DOCMDI-P9-042` Host picker — demo creates a single host so the wizard never shows the picker strip; future multi-host demo would exercise that path.
- [ ] `DOCMDI-P9-043` Persist `DoNotShowAgain` to local app data so the auto-trigger respects the user's choice across launches.

## Verification Criteria
1. `WinFormsApp.UI.Test.exe --demo document-host-mdi` launches the demo
   form and, after the modal becomes interactive, opens the
   `DocumentSetupWizardDialog` with **Tabbed Documents** pre-selected.
2. Clicking **Apply & Close** with default settings produces a `BeepDocumentHost` filled
   with 3 sample document tabs, and the status strip reads
   `Mode: Tabbed Documents | Documents: 3`.
3. Re-running the wizard, picking **Native MDI**, and clicking **Apply & Close**
   converts the form to an MDI container, the status strip switches to
   `Mode: Native MDI`, and existing tabs are dropped per the designer's
   semantics.
4. Re-running the wizard a third time, picking **Tabbed Documents**, and
   leaving **Add sample tabs** unchecked (because there are no docs in the
   new view) restores the tabbed view without seeding extras.
5. With `BeepThemesManager.CurrentTheme` set to any registered Beep theme
   before launching the demo, the wizard chrome (header, tiles, preview,
   footer) visibly inherits that theme's `BackColor`, `AccentColor`,
   `BorderColor` palette — proving the Phase 08 `WizardPalette` integration
   works end-to-end.

## Notes
- The sample lives in `WinFormsApp.UI.Test` rather than under
  `Beep.Winform.Default.Views` because demos there already follow the same
  `--demo <name>` argument convention and the project carries all the
  referenced projects (`Design.Server`, `Controls`, `Vis.Modules`).
- `BeepNativeMdiView` requires its parent `Form.IsMdiContainer = true`. The
  view sets that automatically when `ParentForm` is assigned, so we don't
  toggle it ourselves except when reverting back to Tabbed (set false then
  detach view).
- The `TrySet` reflection helper is deliberately narrow — it only coerces
  enum-from-string and never throws on missing properties. This insulates
  the demo from ongoing refactors of `BeepDocumentHost.Properties.cs`.
