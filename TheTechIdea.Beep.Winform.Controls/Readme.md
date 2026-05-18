# TheTechIdea.Beep.Winform.Controls

This project hosts reusable Beep WinForms controls.

## Document Host

`BeepDocumentHost` / `BeepTabbedView` is the main MDI and docking surface.
`BeepDocumentManager` is the companion non-visual component (component tray) that
orchestrates the host — wiring menu, status strip, keyboard shortcuts, layout
persistence, workspace management, and cloud sync.

- [DocumentHost/Readme.md](DocumentHost/Readme.md) — overview, feature list, quick start, class reference
- [DocumentHost/BeepDocumentManager.Readme.md](DocumentHost/BeepDocumentManager.Readme.md) — manager component full reference
- [DocumentHost/Tutorials/01-IdeShell.md](DocumentHost/Tutorials/01-IdeShell.md) — 10-minute IDE shell tutorial
- [DocumentHost/Tutorials/02-Migrate-from-host-only.md](DocumentHost/Tutorials/02-Migrate-from-host-only.md) — migration guide

## Recent update

- 2026-05-18: runtime document operations are now aligned across `BeepDocumentManager`, `BeepTabbedView`, and `BeepNativeMdiView`. The manager's `force` remove flag now reaches the active view, native MDI child windows are tracked with real document descriptors/events, manager-hosted MDI `Form` addins no longer bypass native-MDI tracking, and tabbed-view persistence now clears stale session paths correctly.
- 2026-05-18: follow-up runtime hardening closed the remaining manager/display-container gaps: auto-added extended controls no longer duplicate documents on reconfiguration, addins are now disposed from the actual document-removal path, cancelled closes no longer produce false `AddinRemoved` notifications, and failed addin hosting cleans up orphan tabs or MDI children.
- 2026-05-18: manager/view contract parity tightened again: `DocumentCount` now comes from the active view instead of assuming a tabbed host, and Window-menu wiring is explicitly detached and reattached across view or menu-owner changes so tabbed and native MDI do not leave stale or duplicate menu behavior behind.
- 2026-05-18: `BeepDocumentManager` now enforces unique `IDisplayContainer` title keys before hosting an addin, preventing duplicate-title adds from overwriting the tracked entry and orphaning an older tab or MDI document.
- 2026-05-18: manager status-strip parity improved across views. The active document now comes from the shared view contract, and manager-side document events refresh the status strip so native MDI no longer falls back to `No document` just because there is no `BeepDocumentPanel`.
- 2026-05-18: manager-created runtime documents now survive view swaps. `BeepDocumentManager` keeps a runtime descriptor list for documents opened through `AddDocument(...)`, removes those manager-owned documents from the old view during transfer, and replays the list when a new view attaches, while addin-hosted documents stay on their dedicated hosting pipeline to avoid duplicate embedded controls/forms.
- 2026-05-18: native-MDI addin hosting now follows the current view instance across manager view switches. The manager detaches its old `DocumentFormCreated` hook and re-hooks the new `BeepNativeMdiView`, so `AddControl(...)` keeps working after switching MDI views.
- 2026-05-18: hosted-content migration tightened again. Rehostable addins, extender-backed controls, and form-backed addins now detach from the old manager view and reattach to the new one during view changes; `BeepNativeMdiView` now uses removable child-form handlers so transferred MDI forms do not accumulate duplicate event wiring.
- 2026-05-18: manager view-transfer cleanup now uses detach semantics instead of normal close semantics. Manager-owned documents are removed from the old view without going through user-close cancellation, and live child controls hosted directly inside those documents are reattached after the destination view recreates the document shell.
- 2026-05-18: `BeepDocumentHost` design-time document creation/removal is now centralized in `DesignTimeDocumentCoordinator`. Host-designer verbs, manager add, seeded sample documents, layout-assistant growth/shrink, split creation, and reopen all use the same coordinator so the `DocumentDescriptor` and real `BeepDocumentPanel` component are added or removed together in Designer.cs. Host-side document edits now also mirror metadata back to any bound `BeepDocumentManager`.
- 2026-05-18: the shared document coordinator now blocks descriptor-only adds when panel creation fails, and the custom document collection editor preserves full `DocumentDescriptor` metadata during edit round-trips.
- 2026-05-18: document collection editor sync now stays inside the active designer edit transaction, and layout presets bail out cleanly when the required design-time document panels cannot be provisioned.
- 2026-05-17 (Phase 12): `BeepDocumentHost` design-time serialization reworked. `BeepDocumentPanel` instances now appear in Designer.cs as real components via the new `DocumentPanels` collection (`[DesignerSerializationVisibility(Content)]`). `DocumentId` is now serialized (stable GUID survives form re-open). `ApplyDesignTimeDocuments()` is guarded against double-registration when `DocumentPanels` has already been restored from `InitializeComponent`. `BeepDocumentManagerDesigner` now delegates panel creation to `BeepDocumentHostDesigner.SyncFromManagerDocuments()` so the manager's "Add Document" smart-tag action also produces Designer.cs entries. See [DocumentHost/Readme.md § 13](DocumentHost/Readme.md) for the full breakdown.
- 2026-05-12: `Calendar/BeepCalendar` was mechanically split into focused partial files to reduce file size and improve maintainability, with no logic rewrite.
- New partial files include `BeepCalendar.Core.cs`, `BeepCalendar.Fields.cs`, `BeepCalendar.Painting.cs`, `BeepCalendar.LayoutTheme.cs`, `BeepCalendar.EventOperations.cs`, and `BeepCalendar.Types.cs`.
