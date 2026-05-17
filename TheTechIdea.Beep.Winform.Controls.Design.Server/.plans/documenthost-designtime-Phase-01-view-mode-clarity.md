# Phase 01 — TabbedView vs NativeView Clarity & Auto-Wire

**Goal.** Eliminate the confusion between `BeepTabbedView` and
`BeepNativeMdiView`. The user must be able to drop a
`BeepDocumentManager`, hit one verb, and end up with a fully working
view — with a clear, written explanation of the trade-offs.

**Status.** Foundation landed in this iteration. Polish items below are
pending.

**Status legend.** `[x]` shipped — `[~]` partially done — `[ ]` pending.

---

## 1. Background

`BeepDocumentManager` is a non-visual orchestrator. It renders documents
through whatever `IBeepDocumentManagerView` is assigned to its `View`
property. Two implementations exist:

- **`BeepTabbedView`** — wraps a `BeepDocumentHost` control on the form.
  Provides tabs, docking, splits, floating windows, pinning, themes,
  layout persistence.
- **`BeepNativeMdiView`** — uses native WinForms MDI. Each document is a
  separate `Form` parented to a host form with `IsMdiContainer = true`.
  No tabs, no docking, no splits — only what classic WinForms MDI offers.

Before this iteration the smart-tag verbs simply created the view
component and left both required wire-up properties (`Host` and
`ParentForm`) unset. Users were silently left with a non-functioning
view.

## 2. Target files

| File                                                                                 | Responsibility                                                                  |
| ------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------- |
| `Designers/BeepDocumentManagerDesigner.cs`                                           | Verb registration, verb dispatch, `CreateAndAssignView<T>` transaction wrapper. |
| `Designers/BeepDocumentManagerDesigner.ViewMode.cs` *(new)*                          | `ApplyTabbedViewMode`, `ApplyNativeMdiViewMode`, `ShowViewModeChooserDialog`, `ViewModeChooserDialog` inline class. |
| `Designers/BeepDocumentManagerActionList.cs` *(part of `BeepDocumentManagerDesigner.cs`)* | Smart-tag entry "Choose View Mode…", entries for both modes with descriptions.  |
| `.plans/documenthost-designtime-Phase-01-view-mode-clarity.md`                       | This document.                                                                  |

## 3. Implementation steps

### 3.1 Done

- [x] Make `BeepDocumentManagerDesigner` a `sealed partial` class.
- [x] Add `BeepDocumentManagerDesigner.ViewMode.cs` partial with:
  - `ApplyTabbedViewMode(showInfo)` — creates `BeepTabbedView`, resolves
    a single `BeepDocumentHost` from the design surface and wires
    `View.Host`. Reports back via dialog when `showInfo` is `true`.
  - `ApplyNativeMdiViewMode(showInfo)` — creates `BeepNativeMdiView`,
    sets the root form's `IsMdiContainer = true` (through
    `IComponentChangeService`), wires `View.ParentForm`.
  - `ShowViewModeChooserDialog()` — modal explanation chooser.
  - `ResolveBeepDocumentHost(out status)` helper that distinguishes
    *no host*, *one host* and *many hosts* with a human-readable status.
- [x] Rename `SetViewType<T>` → `CreateAndAssignView<T>` and make it
  `internal` so the partial can reuse the transaction wrapper.
- [x] Add a "Choose View Mode…" `DesignerVerb` and matching
  `DesignerActionMethodItem` so both the verb list and the smart-tag
  surface the chooser.
- [x] Update smart-tag descriptions for both single-mode entries so
  users know exactly what each verb will do (auto-wire behaviour,
  required prerequisites).

### 3.2 Pending — polish & coverage

- [ ] **Mode-aware property visibility.** When `View` is null or a
  non-tabbed view, hide `TabStyle`, `TabPosition`, `CloseMode`,
  `AllowSplit`, "Save Layout Now", "Load Layout Now" from the smart-tag
  panel. Today the action list filters `TabbedHost != null` for the
  appearance group; extend this to the rest.
- [ ] **First-drop wizard.** Detect "no `View` assigned" right after
  `Initialize` and surface a non-blocking info bar on the smart-tag
  ("This BeepDocumentManager has no View. Click 'Choose View Mode…' to
  set one up.").
- [ ] **Multi-host chooser dialog.** When more than one
  `BeepDocumentHost` exists on the design surface, replace the warning
  text with a small picker so the user can choose which host to wire.
- [ ] **Switch confirmation.** If the user already has documents seeded
  and switches mode, prompt to confirm and ensure the previous view is
  deleted from the designer container (avoid orphan components).
- [ ] **NativeView capability badge.** Add a read-only smart-tag
  property "Mode Capabilities" that lists which features are supported
  for the currently selected view, e.g.
  - Tabbed: `Float ✔ Pin ✔ Split ✔ FloatingWindows ✔ Theming ✔`
  - NativeMdi: `Float ✘ Pin ✘ Split ✘ FloatingWindows ✘ Theming partial`
- [ ] **Tab-strip preview thumbnail** in the chooser dialog (icon +
  shading).
- [ ] **Localisation.** Move all chooser strings to a designer resource
  file so they can be translated alongside the rest of the designer.
- [ ] **Telemetry hook.** Optional: emit a designer-time event when the
  user picks a mode so the team can see which mode wins in practice.

## 4. Acceptance criteria

| #   | Test                                                                                                                                                                                  | Result |
| --- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------ |
| 1   | Drop `BeepDocumentManager`, click **Choose View Mode…** → modal opens, both modes are explained with bullet points, current mode is shown, Apply enables the chosen mode.             | Pending verification |
| 2   | With **no** `BeepDocumentHost` on the form, click **Use Tabbed View** → view is created, message tells the user to drop a `BeepDocumentHost` and how to wire it.                       | Verified (manual) |
| 3   | With exactly **one** `BeepDocumentHost`, click **Use Tabbed View** → view is created and `View.Host` is auto-set; message confirms the wiring.                                          | Verified (manual) |
| 4   | With **two or more** hosts, click **Use Tabbed View** → view is created, message asks the user to set `View.Host` manually. *(picker is a Pending item above.)*                        | Verified (manual) |
| 5   | Click **Use Native MDI View** on a form → `IsMdiContainer = true`, `View.ParentForm = rootForm`; message confirms wiring and explains the missing features (no tabs/splits/float).     | Verified (manual) |
| 6   | Undo after any of the above → view component is removed, `IsMdiContainer` reverts, properties are restored. *(verifies `IComponentChangeService` plumbing.)*                            | Pending verification |
| 7   | Build `TheTechIdea.Beep.Winform.Controls.Design.Server` → 0 errors, no new warnings on the touched files.                                                                              | Verified (build clean) |

## 5. Risks & open questions

- The chooser dialog is inline in the partial as a small `Form`.
  Long-term we may want to move it to `Dialogs/ViewModeChooserDialog.cs`
  to match the rest of the designer dialog layout. Tracked under
  Phase 03 (refactor).
- `IComponentChangeService` semantics for `IsMdiContainer` differ across
  designer hosts (in-proc WinForms vs out-of-proc DesignTools server).
  Verification step #6 must be exercised in both.
- If the user has manually built a custom `IBeepDocumentManagerView`,
  the chooser should not overwrite it without confirmation. The chooser
  currently shows the current view name; we should also disable both
  radio buttons if `View` is not one of the two built-ins and add a
  "(custom view in use)" note.

## 6. Done definition

Phase 01 is done when all `[ ]` items above are checked and acceptance
tests 1–7 pass on both in-proc WinForms designer and the out-of-proc
DesignTools server.
