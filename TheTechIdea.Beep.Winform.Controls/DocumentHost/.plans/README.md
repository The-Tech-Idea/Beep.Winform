# BeepDocumentHost Plans

## Goal

Turn `BeepDocumentHost` into the default MDI and docking surface for the Beep
WinForms stack: easy to use for normal applications, safe in the designer,
predictable at runtime, and extensible enough for more advanced IDE-style shells.

## Audit Basis

- full-folder read across the local `DocumentHost` implementation
- public GitHub examples from DockPanel Suite and AvalonDock
- public commercial-product guidance from DevExpress, Telerik, and Syncfusion

## Planning Files

### Active Plan (start here)

- **[MASTER-TODO-TRACKER.md](MASTER-TODO-TRACKER.md)** — the single source of truth.
  Indexes 8 phases of work toward DevExpress-grade UX. New work goes here.
- `PHASE-01-stabilise-designer.md` — fix the designer delete-crash. **Must ship first.**
- `PHASE-02-document-manager-component.md` — new `BeepDocumentManager` non-visual component.
- `PHASE-03-view-modes.md` — Tabbed / NativeMdi / WindowsUI switchable views.
- `PHASE-04-documents-collection.md` — design-time collection editor.
- `PHASE-05-dock-panels.md` — `BeepDockManager` tool-window panels.
- `PHASE-06-commands-and-menu.md` — Window menu / shortcuts / palette.
- `PHASE-07-persistence-pack.md` — wrap & validate existing persistence.
- `PHASE-08-samples-docs.md` — sample form, tutorials, NuGet readme.

### Historic / Background

- `CURRENT-STATE-AUDIT.md` — full-folder capability and gap summary.
- `COMMERCIAL-REFERENCE-NOTES.md` — public-product and GitHub patterns used as guidance.
- `IMPLEMENTATION-ROADMAP.md` — earlier phased roadmap (superseded by the master tracker).

## Planning Rules

1. Simple path first.
  - A developer should be able to drop the host on a form and open documents without learning internal group or layout details.
2. Stable identity first.
  - Every document should have a stable `DocumentId` and a persistence identity that does not depend on display text.
3. Dock state should be explicit.
  - Docked, floating, auto-hide, and restored states should be modeled cleanly and surfaced publicly.
4. One mutation pipeline.
  - Close, float, dock, split, move, auto-hide, and restore should run through the same coordination path.
5. Designer safety before cleverness.
  - Design-time add, remove, reparent, duplicate, and serialization flows must not trigger runtime-only mutations.
6. Advanced features stay opt-in.
  - Cloud sync, diff view, terminal panels, breadcrumb, mini-toolbar, and status surfaces must not complicate the default MDI story.

## Current Read

- Already strong: tab-strip UX, split groups, auto-hide, float and dock, persistence, keyboard workflows, and MVVM support.
- Main blockers: incomplete designer regression coverage, shell discoverability, stable persistence identity, and broader multi-group restore validation.
- Optional and scaffolded: breadcrumb, undo and redo surfaces, cloud sync, diff tooling, terminal tooling, status bar, mini-toolbar, and git integration.