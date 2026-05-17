# Phase 05 — Onboarding, Wizards & Layout Templates

**Goal.** Make the first 5 minutes with `BeepDocumentHost` /
`BeepDocumentManager` self-explanatory. A user with no prior
documentation should be able to drop a manager + host pair, hit the
smart-tag, and end up with a working tabbed application.

**Status.** Pending. The chooser dialog from Phase 01 is the first
step; this phase rounds it out with templates and contextual help.

---

## 1. Background

Today the onboarding path is:

1. Drop `BeepDocumentManager` (no immediate feedback).
2. Drop `BeepDocumentHost` (no immediate feedback unless the user
   knows to look at the smart-tag).
3. Wire `BeepTabbedView.Host` manually OR run "Choose View Mode…"
   (Phase 01 fix).
4. Click "Edit Documents…" or "Add Document" to seed at least one
   panel.
5. Drop child controls onto the active tab (Phase 02 fix made this
   work).

That is still a lot to discover. Commercial alternatives ship with
templates: "Empty MDI Form", "Visual-Studio-like layout", "Browser
layout", "Two-pane editor".

## 2. Target files

| File                                                                              | Responsibility                                                                              |
| --------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `Designers/Dialogs/DocumentHostTemplateWizard.cs` *(new)*                         | Modal dialog showing a gallery of starter layouts with previews.                            |
| `Designers/BeepDocumentHostDesigner.Templates.cs` *(new partial)*                  | `ApplyTemplate(TemplateId)` helpers — calls existing `BeepDocumentHost.Templates.cs` runtime APIs. |
| `Designers/BeepDocumentManagerDesigner.ViewMode.cs`                                | Add "Apply Layout Template…" verb that opens the wizard once a view is wired.                |
| `Designers/Dialogs/FirstRunHintBar.cs` *(new)*                                    | Non-blocking info bar shown on the smart-tag when no view / no documents.                   |
| `Designers/Resources/templates/*.png`                                              | Thumbnail previews.                                                                          |

## 3. Implementation steps

- [ ] **Catalogue runtime templates** in
  `TheTechIdea.Beep.Winform.Controls/DocumentHost/BeepDocumentHost.Templates.cs`.
  Identify which are appropriate to expose at design time.
- [ ] **`DocumentHostTemplateWizard`** — modal form with:
  - Left panel: template list (Empty, Editor, Browser, IDE 3-pane, etc.).
  - Right panel: thumbnail + description + "What this gives you" bullet
    list.
  - "Apply" wraps the chosen template in a single `DesignerTransaction`
    and seeds:
    - `DesignTimeDocuments`,
    - `DesignTimeLayoutJson`,
    - host policy flags,
    - tab style/position,
    - close mode.
- [ ] **`BeepDocumentHostDesigner.Templates.cs` partial** — wraps
  template application so the existing transaction plumbing in
  `ExecuteDesignTimeDocumentsAction` handles change notifications.
- [ ] **First-run hint bar.** When `_manager.View == null` or
  `_manager.View is BeepTabbedView { Host: null }`, surface a
  `DesignerActionTextItem` (or a banner adorner) at the top of the
  smart-tag that says:
  - "No View assigned — click 'Choose View Mode…' to start."
  - "Tabbed view has no Host — click 'Wire Tabbed View Host…'."
- [ ] **Help link.** Every smart-tag header carries a
  `DesignerActionTextItem` with a short docs link
  (`https://...beep-docs/.../document-host/design-time`).
- [ ] **VS toolbox tooltip text** for `BeepDocumentHost`,
  `BeepDocumentManager`, `BeepTabbedView`, `BeepNativeMdiView`,
  `BeepDocumentPanel` — accurate one-liners that match the mode-chooser
  copy.

## 4. Acceptance criteria

| #   | Test                                                                                                                                  | Result |
| --- | ------------------------------------------------------------------------------------------------------------------------------------- | ------ |
| 1   | Fresh user can build a "Visual-Studio-like 3-pane" layout in under 60 seconds without docs.                                            | Pending |
| 2   | The template wizard previews accurately match the result on the form.                                                                  | Pending |
| 3   | First-run hint bar disappears once a view is wired and at least one document is seeded.                                                | Pending |
| 4   | Smart-tag header link opens the right docs page.                                                                                       | Pending |
| 5   | Apply Template can be undone in one Ctrl+Z (verified through Phase 04 transaction plumbing).                                            | Pending |

## 5. Risks & open questions

- **Template fidelity.** The runtime templates today may not include
  every layout variant the wizard wants to show. We may need to add new
  template entries to `BeepDocumentHost.Templates.cs`.
- **Thumbnail authoring effort.** Designer PNGs need to be created and
  embedded as resources. Allocate UX time.
- **Theme-aware previews.** Thumbnails should match the user's chosen
  theme; consider rendering at runtime instead of static PNGs.

## 6. Done definition

Phase 05 is done when the wizard ships with at least 4 templates, the
hint bar is in place, the help link is wired, and acceptance tests 1–5
pass.
