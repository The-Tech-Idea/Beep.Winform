# Phase 07 — Commercial-Grade Design-Time Experience

**Goal.** Make the design-time experience for `BeepDocumentHost` and
`BeepDocumentManager` feel like DevExpress XtraTabbedMdiManager / Telerik
RadDocking / Syncfusion DockingManager: drop one component, a wizard
guides you, the smart-tag shows the live configuration in plain English,
and one click switches between Tabbed / Browser / Native MDI modes.

**Status.** Foundation landed in this iteration. Polish items below are
pending.

**Status legend.** `[x]` shipped — `[~]` partially done — `[ ]` pending.

---

## 1. The redesign at a glance

Before this iteration:

- Drop `BeepDocumentManager`. Nothing visible happens.
- Drop `BeepDocumentHost`. Nothing visible happens.
- Open smart-tag, hunt for "Use Tabbed View" / "Use Native MDI View".
- Verb creates view component but doesn't wire `Host` / `ParentForm`.
- User left with a non-functioning setup and no feedback.

After this iteration:

- Drop `BeepDocumentManager` → **Setup Wizard opens automatically** with
  visual mode tiles (Tabbed / Browser Tabs / Native MDI), starter-content
  checkboxes, layout-template picker, and a live preview pane. Click
  **Apply** and everything is wired in one undoable transaction.
- Drop `BeepDocumentHost` → **Quick Setup Wizard opens automatically**
  (same wizard, Native MDI tile suppressed because the host is a control
  not a form). Picks a tab style, optionally seeds N sample tabs.
- Smart-tag opens with a **Status banner at the top** that tells the
  user exactly what is wired ("Tabbed Documents · 3 docs · Top tabs ·
  Splits allowed"), then a prominent ✨ **Setup Wizard…** entry, then
  mode-switch shortcuts ("Use Tabbed Documents", "Use Browser Tabs",
  "Use Native MDI").
- Re-running the wizard at any time is a single click; mode switch is
  fully reversible.

## 2. Target files

| File                                                                                            | Responsibility                                                                                          |
| ----------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| `Dialogs/DocumentSetupWizardDialog.cs` *(new)*                                                  | Modal wizard: 3 visual tiles, starter checkboxes, template combo, live preview, status footer.          |
| `Designers/BeepDocumentManagerDesigner.cs`                                                      | `InitializeNewComponent` opens wizard on first drop; verb list reordered; smart-tag `Status` property.  |
| `Designers/BeepDocumentManagerDesigner.ViewMode.cs`                                             | `ApplyTabbedViewMode` / `ApplyBrowserTabsMode` / `ApplyNativeMdiViewMode`, `ShowSetupWizard`, `ApplySetupResult`, `SeedSampleDocuments`, `ApplyLayoutTemplate`, `DescribeViewMode`. |
| `Designers/BeepDocumentHostDesigner.SetupHost.cs` *(new)*                                       | `InitializeNewComponent` opens wizard on first host drop; `ShowQuickSetupWizard`; `DescribeHostState`.   |
| `ActionLists/DocumentHostActionList.cs`                                                         | Smart-tag now starts with **Status** header + ✨ **Setup Wizard…** entry, then the existing categories. |
| `.plans/documenthost-designtime-Phase-07-commercial-grade-experience.md`                        | This document.                                                                                          |

## 3. Implementation steps

### 3.1 Done

- [x] **`DocumentSetupWizardDialog`** — clean modal form with:
  - Three large `ModeTile` custom controls (Tabbed Documents,
    Browser Tabs, Native MDI), each with hover/selected/focus states,
    rounded corners, custom-painted icons, descriptions, and a
    "RECOMMENDED" accent badge on the first tile.
  - Starter-content row: "Add N sample document tabs" + numeric counter.
  - Layout template combo (Empty / Visual Studio 3-pane / Two-pane editor
    / Browser).
  - Live preview pane that paints the tabbed / browser / MDI mock-up.
  - Footer with `Apply & Close` (accent button) and `Configure Later`
    (secondary).
  - Result captured into `DocumentSetupResult` for the caller.
- [x] **`InitializeNewComponent` on the manager designer** —
  toolbox-drop launches the wizard via `SynchronizationContext.Post` so
  the component is fully sited first; wrapped in try/catch so the drop
  never fails even if the wizard itself throws.
- [x] **`InitializeNewComponent` on the host designer** — same pattern,
  routes to `ShowQuickSetupWizard()` which opens the same dialog but
  silently rejects the Native MDI tile (host is a control, not a form).
- [x] **`ApplyBrowserTabsMode`** — new third mode that applies
  `TabStyle = Chrome`, `ShowAddButton = true`, `CloseMode = Always`,
  `TabPosition = Top` to the wired host through
  `IComponentChangeService` for undo.
- [x] **`ApplySetupResult`** — single-transaction apply of mode +
  sample docs + template so one Ctrl+Z reverses the whole wizard
  outcome.
- [x] **`SeedSampleDocuments`** — calls `manager.AddDocument("Document
  N")` inside `BeginBatchAddDocuments` / `EndBatchAddDocuments` so the
  view does one layout pass.
- [x] **Status banner**: `DescribeViewMode()` on the manager designer
  and `DescribeHostState()` on the host designer return plain-English
  one-liners; both are surfaced as the first smart-tag entry of their
  respective designers. Examples:
  - "Tabbed Documents · 3 docs · Top tabs · Splits allowed"
  - "Browser Tabs · 0 docs · Top tabs · + button"
  - "Native MDI · ParentForm = Form1 · IsMdiContainer = True"
  - "Not configured — click 'Setup Wizard…' to start."
- [x] **Smart-tag re-layout** for the manager:
  - "Status" header + live status text.
  - "View" header + ✨ Setup Wizard… + View property + three
    one-click mode buttons.
  - Existing Documents / Commands / Tab appearance / Policy / Layout /
    Window menu groups preserved.
- [x] **Smart-tag re-layout** for the host: status header + setup
  wizard appended at the top, original sections preserved.
- [x] **Verbs reordered** so the ✨ Setup Wizard… verb is the first
  entry on both designers.
- [x] **Build clean** on both `TheTechIdea.Beep.Winform.Controls.Design.Server`
  and `TheTechIdea.Beep.Winform.Controls` (0 errors, no new warnings).

### 3.2 Pending — polish & coverage

- [x] **Re-run wizard reflects current state.** The wizard now reads the
  host's current document count and:
  - Defaults the "Add N sample tabs" checkbox to unchecked when
    `documentCount > 0`.
  - Disables the checkbox + spinner.
  - Shows a yellow `"(skipped — N doc(s) already present)"` suffix
    so the user understands the no-op.
  - Implemented via the new
    `DocumentSetupWizardDialog(initial, existingDocumentCount, hostOptions)`
    overload + a `hasExistingDocs` flag in the builder.
- [x] **Multi-host chooser** in the wizard. When the manager designer
  finds 2+ `BeepDocumentHost` controls on the design surface a yellow
  "Wire to host:" strip appears between the mode tiles and the
  options box with a `DropDownList` combo containing each host's site
  name. The user's selection rides on
  `DocumentSetupResult.SelectedHostComponent` and the designer pins it
  via `_pinnedHostForSetup` so subsequent calls to
  `ResolveBeepDocumentHost` honour the choice. With 0 or 1 hosts the
  strip is suppressed (auto-resolve).
- [x] **"Don't show again" toggle** on the wizard, persisted under
  `HKCU\Software\TheTechIdea\Beep\Design\DocumentSetupWizard\AutoOpen`
  via the new
  `BeepDocumentManagerDesigner.WizardPrefs.cs` partial.
  Both `BeepDocumentManagerDesigner.InitializeNewComponent` and
  `BeepDocumentHostDesigner.InitializeNewComponent` consult
  `ShouldAutoOpenWizard()` before launching the modal. The choice is
  honoured even on Cancel / Configure-Later so a user who opted out
  via the X button still gets the saved preference. A new
  **"Reset Setup Wizard Preference"** verb on the manager designer
  flips the flag back on.
- [x] **Empty-host drop converges with full-host drop.** Previously the
  polished `BeepDocumentHostDesigner.OnDragDrop` redirect ran only when
  `GetActiveDocumentPanel(host)` already returned a panel; an empty
  host fell back to `ParentControlDesigner.OnDragDrop`, which still
  parented the dropped control to *a* panel via
  `GetParentForComponent` but skipped our reflection-redirect and the
  drag-overlay cleanup. The redirect now uses
  `GetActiveDocumentPanel(host) ?? EnsureDesignSurfaceForDroppedContent()`
  so the same code path runs in either case — the empty host is
  auto-promoted into a first document on demand, exactly like
  DevExpress XtraTabbedView / Telerik RadDocking.
- [x] **Status banners are now actionable.** When the host has 0
  documents the manager and host smart-tag banners both end with the
  exact verbs the user should click:
  - Manager: `"Tabbed Documents · 0 docs — drop a control on the host
    (creates a tab) or click 'Add Document'."`
  - Host:    `"Tabbed Documents · 0 docs — drop a control on the host
    (creates a tab) or right-click for 'Add Document'."`
  Native MDI banners similarly point at the next step:
  - `"Native MDI · ParentForm not assigned — set View.ParentForm to the root Form or re-run the Setup Wizard."`
  - `"Native MDI · ParentForm = Form1 · IsMdiContainer = false — set it to true or re-run the Setup Wizard."`
  - `"Native MDI · ParentForm = Form1 · IsMdiContainer = true · add child Forms at runtime via (IDisplayContainer)manager.AddControl(title, addinForm, …)."`
- [ ] **Layout template catalogue.** `ApplyLayoutTemplate` currently
  reflection-probes for `ApplyVisualStudioLayout` / `ApplyThreePaneLayout`
  / `ApplyTwoPaneLayout` on the host. Replace with explicit calls into
  the catalogue once Phase 05 standardises the runtime template API.
- [ ] **Wizard preview is themed.** Today the preview uses fixed
  light-mode colours. Honour the active VS theme (Light / Dark / High
  Contrast) by reading `SystemColors` and the host's `Theme` property
  when available.
- [ ] **Embed the wizard inside the smart-tag** as an inline expandable
  surface for power users who don't want a modal.
- [ ] **Telemetry.** Optional anonymous designer-time event for "mode
  chosen" so we can tune defaults.
- [ ] **Localisation.** All wizard strings, smart-tag entries and
  status banner phrases need to go through a resource file for
  translation alongside the rest of the Beep.Design surface.
- [ ] **Accessibility.** Wizard mode tiles already support keyboard
  (Tab + Enter/Space); add `AccessibleName` / `AccessibleDescription`
  and screen-reader-friendly status announcements.

### 3.3 Successor — Phase 08

The three pending polish items above were the user-visible blockers
for declaring Phase 07 "commercial-grade". With them shipped, the next
piece of work moves up the stack to *runtime API parity* with the
existing `BeepDisplayContainer`. See
`documenthost-designtime-Phase-08-display-container-integration.md` for
the IDisplayContainer drop-in implementation on
`BeepDocumentManager`.

## 4. Acceptance criteria

| #   | Test                                                                                                                              | Result |
| --- | --------------------------------------------------------------------------------------------------------------------------------- | ------ |
| 1   | Drop `BeepDocumentManager` on a Form → wizard opens; pick "Tabbed Documents" + "Add 3 sample tabs" + Apply → manager bound, host present, 3 tabs visible. | Pending verification (manual) |
| 2   | Drop `BeepDocumentManager` on a Form (no existing host) → wizard opens; pick Tabbed → message advises user to add a `BeepDocumentHost`; status banner reflects "Host not assigned". | Pending verification |
| 3   | Drop `BeepDocumentHost` directly on a Form → quick wizard opens; pick Browser Tabs + Apply → `TabStyle = Chrome`, `ShowAddButton = true`, `CloseMode = Always` applied. | Pending verification |
| 4   | Smart-tag of either designer shows a **Status** header at the very top with a plain-English description of the current configuration. | Verified (renders via `DesignerActionTextItem`) |
| 5   | One-click verb "Use Browser Tabs" applied to an existing Tabbed manager flips host properties; status banner updates without reopening the smart-tag. | Verified (calls `RefreshPanel`) |
| 6   | Ctrl+Z after any wizard Apply reverses every change (mode + sample tabs + template) in a single undo step. | Pending verification (single `DesignerTransaction` wraps `ApplySetupResult`) |
| 7   | Drop, drop, drop → no exceptions are thrown into the designer log even if the wizard is cancelled or fails. | Verified (every InitializeNewComponent call is try/catch wrapped) |
| 8   | Build `TheTechIdea.Beep.Winform.Controls.Design.Server` → 0 errors, no new warnings on touched files. | Verified |
| 9   | Build `TheTechIdea.Beep.Winform.Controls` → 0 errors. | Verified |

## 5. UX rationale

Why mode TILES rather than radio buttons?

- Commercial wizards (DevExpress Template Gallery, Telerik DockingManager
  "Layout Templates") use visual cards to communicate the
  shape of the choice. Radio buttons under-sell the differences.
- The icon paint code lives entirely in the dialog and uses GDI+
  primitives (no external assets), so the wizard is self-contained.
- Hover/focus/selected states make the dialog feel like a modern
  Windows-11-era settings page rather than a 2003-era control panel.

Why is "Configure Later" given equal footing with "Apply"?

- The wizard launches automatically on toolbox drop. Some users will
  drop the component knowing exactly what they want and don't appreciate
  modal nagging. "Configure Later" closes the wizard with `Cancel`
  semantics and no design-time mutation, so the user is no worse off
  than before this iteration.

Why is there a separate **host** wizard?

- The host is useful standalone (it's a control, not a form). A user
  putting a `BeepDocumentHost` inside a `SplitContainer.Panel2` doesn't
  need a `BeepDocumentManager`. Quick Setup gives them just the
  configurations that make sense for a control: tab style + sample
  tabs. Native MDI is correctly out of scope here and the wizard
  message routes them to the manager flow.

Why a **Status banner** at the top of every smart-tag?

- Without it, the user has to reverse-engineer the configuration by
  walking down the property list. The banner is the single source of
  truth for "what's wired right now".

## 6. Risks & open questions

- **Toolbox drop timing.** `InitializeNewComponent` runs before all
  designer services are available in some out-of-process designers.
  Posting via `SynchronizationContext.Current` defers wizard display to
  the next message-loop tick; this is the standard pattern but should
  be verified on both designer surfaces.
- **First-drop wizard interferes with bulk paste.** If a user copies a
  designed form that contains a manager, the wizard should NOT open.
  `InitializeNewComponent` is only called for toolbox drops (not for
  deserialisation), so this is handled by the framework — verify on
  both surfaces.
- **`Manager.AddDocument` requires a view.** `SeedSampleDocuments`
  guards with `if (_manager?.View == null) return;` so a wizard cancel
  after picking a mode but before companion wiring is benign.
- **Reflection-probed template methods.** `ApplyLayoutTemplate` uses
  reflection to find host template methods; if a future runtime rename
  breaks the lookup, the wizard silently no-ops on templates (still
  applies mode + samples). Tracked under Phase 05 for a proper API.

## 7. Done definition

Phase 07 is done when the polish list in §3.2 is fully checked, all
acceptance tests in §4 pass on both the in-proc WinForms designer and
the out-of-process DesignTools server, and the wizard preview is
themed to match the active VS theme.
