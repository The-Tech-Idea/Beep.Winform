# BeepDocumentHost Commercial-Grade Design-Time Enhancement Plan

## Goal
Bring `BeepDocumentHost` to a design-time and runtime experience comparable to commercial WinForms suites (DevExpress/Telerik/Syncfusion), with strong Visual Studio designer integration, robust docking/document workflows, and safe serialization.

## Scope
- Runtime control surface in `TheTechIdea.Beep.Winform.Controls/DocumentHost`
- Designer/server components in `TheTechIdea.Beep.Winform.Controls.Design.Server`
- Optional integrated package hooks in `TheTechIdea.Beep.Winform.Controls.Integrated`

## Success Criteria
- Drag/drop, split, float, dock, auto-hide, and tab groups work reliably at runtime.
- Visual Studio designer supports creating and arranging document layout without manual code.
- Layout persists/restores with schema versioning and migration.
- Control is test-backed (unit + integration), performant, and theme-consistent.

---

## Phase 1: Baseline Audit and Contract Freeze (1 week)
1. Inventory current capabilities in:
   - `DocumentHost/BeepDocumentHost*.cs`
   - `DocumentHost/BeepDocumentTabStrip*.cs`
   - `Design.Server/Designers/BeepDocumentHostDesigner.cs`
   - `Design.Server/ActionLists/DocumentHostActionList.cs`
2. Define public API contract v2:
   - Immutable behavior guarantees (activation, close, reorder, float/dock).
   - Event sequence guarantees (`DocumentClosing`, `DocumentClosed`, `ActiveDocumentChanged`).
3. Mark unstable APIs with `[EditorBrowsable(EditorBrowsableState.Advanced)]` or internalize.
4. Produce a gap matrix against commercial features.

Deliverables:
- `plans/documenthost-gap-matrix.md`
- `plans/documenthost-api-contract-v2.md`

---

## Phase 2: Document Model Hardening (1 week)
1. Introduce explicit document model interfaces:
   - `IDocumentHostItem`
   - `IDocumentLayoutNode`
2. Add close pipeline:
   - `DocumentClosing` (cancelable)
   - async-aware close request routing (for unsaved prompts).
3. Add activation pipeline:
   - `ActiveDocumentChanging` (cancelable) and changed event.
4. Add command service abstraction:
   - `IDocumentHostCommandService` for New/Close/CloseAll/Pin/Float/Dock.

Deliverables:
- Updated `DocumentHost/*.cs` partials
- Contract tests for event ordering

---

## Phase 3: Docking Engine and Layout Tree (2-3 weeks)
1. Replace implicit panel stacking with explicit layout tree:
   - Nodes: `DocumentGroup`, `SplitGroup`, `AutoHideGroup`, `FloatGroup`.
2. Implement split operations:
   - Left/Right/Top/Bottom docking with proportional sizing.
3. Add tabbed groups and group merging.
4. Add document preview overlay (dock guides) with hit-testing.

Deliverables:
- `DocumentHost/Layout/*` new engine files
- Runtime overlay + interaction tests

---

## Phase 4: Layout Persistence and Migration (1 week)
1. Define layout DTO schema v1:
   - ids, group hierarchy, splitter ratios, selected tab, floating bounds.
2. Build serializer/deserializer with validation.
3. Add schema version migration service for forward compatibility.
4. Add fail-safe loading mode:
   - partial recovery if one node is invalid.

Deliverables:
- `BeepDocumentHost.Serialisation.cs` refactor
- `plans/documenthost-layout-schema-v1.md`

---

## Phase 5: Design-Time Foundation (2 weeks)
1. Upgrade designer integration:
   - Selection rules, glyphs, snap lines, drag handles.
2. Provide design-time services:
   - `ISelectionService`, `IDesignerHost`, `IComponentChangeService` integration for undo/redo.
3. Add custom designers for:
   - `BeepDocumentHost`
   - `BeepDocumentGroup` (if group exposed as component)
4. Ensure all design-time operations are transaction-based.

Deliverables:
- `Design.Server/Designers/*` expanded designer classes
- Undo/redo verified for add/remove/split/merge

---

## Phase 6: Smart Tag + Designer Verbs + Editors (1-2 weeks)
1. Expand smart-tag actions:
   - Add Document
   - Add Tab Group
   - Split Group (4 directions)
   - Float/Dock Active
   - Save/Load Layout
2. Add collection editor(s):
   - Design-time document seed collection with titles/icons/keys.
3. Add type editors:
   - icon picker, theme selector, startup layout selector.
4. Add action list categorization and validation feedback.

Deliverables:
- `Design.Server/ActionLists/*`
- `Design.Server/Editors/*`

---

## Phase 7: Visual Design Surface Experience (2 weeks)
1. Add docking guide UI similar to commercial products:
   - center + side guides with hover preview.
2. Add live translucent preview of target layout before drop.
3. Add context menu on tabs/groups in designer:
   - close, pin, move to new group, float.
4. Add keyboard-assisted layout ops in designer (optional advanced).

Deliverables:
- Designer adorners/behaviors
- Usability pass for common scenarios

---

## Phase 8: Theming and Visual Fidelity (1 week)
1. Define explicit visual tokens for document host:
   - tab normal/hover/active text+back
   - group border
   - guide overlay colors
   - splitter hover/drag colors
2. Add per-theme mapping files for document host visuals.
3. Validate contrast and consistency manually per theme (no auto blending).

Deliverables:
- Theme token map updates under `Themes`
- Visual baseline screenshots in `plans/assets/documenthost-*`

---

## Phase 9: Reliability, Testing, and Performance (1-2 weeks)
1. Unit tests:
   - document lifecycle
   - layout operations
   - serialization roundtrip
2. Design-time integration tests:
   - action list operations
   - code serialization output
3. Performance tests:
   - 100-300 tabs
   - frequent drag/reorder
4. Memory/leak checks for float windows and event handlers.

Deliverables:
- New test projects or expanded existing test suite
- Benchmark results in `plans/documenthost-benchmarks.md`

---

## Phase 10: Packaging, Samples, and Adoption (1 week)
1. Add sample app scenarios:
   - IDE-style document workspace
   - data-entry workspace with split views
2. Add migration notes for existing users.
3. Publish design-time usage guide and troubleshooting section.
4. Ship behind feature flags if needed, then default-enable in next release.

Deliverables:
- `samples/DocumentHost.*`
- `DocumentHost/Readme.md` major update
- Release notes + migration doc

---

## Work Breakdown by Priority
1. Must-have:
   - Layout tree
   - split/tab group operations
   - design-time add/split/dock actions
   - serializer v1
2. Should-have:
   - guide overlay polish
   - editors/collection designers
   - strong undo/redo reliability
3. Nice-to-have:
   - keyboard designer operations
   - advanced animation options

## Risk Register
1. WinForms designer API differences between runtime and design server packages.
   - Mitigation: isolate design-time adapters and version guard.
2. Backward compatibility with current saved layouts.
   - Mitigation: migration service + fallback loader.
3. Event storms/re-entrancy during docking operations.
   - Mitigation: operation scope + deferred notifications.
4. Theme inconsistency across many themes.
   - Mitigation: explicit per-theme token review checklist.

## Definition of Done
1. All must-have features implemented and demoable in designer.
2. No blocking designer exceptions in common workflows.
3. Serialization roundtrip passes for canonical layouts.
4. Documentation and samples updated.
5. Build passes with no new errors in control + design-server projects.
