# BeepTabs

## Overview

`BeepTabs` is being repositioned as a fully owned commercial WinForms tab control for navigation, document, and workspace scenarios. The target is parity with mature products such as DevExpress, Telerik, Syncfusion, Visual Studio, and VS Code while still fitting the Beep control, theme, and painter architecture.

This control is **not** meant to evolve as a wrapper over stock `TabControl` semantics. The premium end-state is:

- a Beep-owned header system
- a Beep-owned page/content system
- rich header metadata and action slots
- document/workspace rules such as pinned, preview, dirty, MRU, and overflow behavior
- professional accessibility, keyboard, touch, and design-time support

## Why The Roadmap Was Reset

The codebase already contains strong infrastructure in `Hosts/`, `Helpers/`, `Models/`, and `Painters/`. The public page model has now been cut over to `BeepTabPage`, but the remaining content-hosting and design-time seams still need to be finished around the Beep-owned host architecture. That gap is the main reason the control does not yet behave like a true commercial tab product.

The documentation in `Tabs/` has been reset to match the actual state of the work:

- the commercial architecture is still in progress
- the remaining cutover is primarily architectural, not stylistic
- no new premium-facing API should expose `TabPage`
- no new work should assume native `TabControl` header behavior
- a clean break is preferred over preserving legacy compatibility inside the premium model

## Current Strengths

The existing implementation already provides a strong base worth keeping:

- custom painter stack with multiple styles
- `BeepTabHeaderHost` partials for layout, painting, mouse, keyboard, context menu, overflow, accessibility, high contrast, and touch
- `BeepTabContentHost` as the stable runtime page host and selected-content presentation surface
- `BeepTabPage` as the canonical Beep-owned page model for runtime and designer usage
- `BeepTabItem`, `BeepTabWorkspaceState`, and related runtime models for richer tab state
- overflow/action infrastructure, MRU tracking, workspace commands, and header metadata scaffolding
- theme, font, icon, and DPI helpers consistent with the broader Beep control architecture

## Primary Blockers

The remaining blockers are architectural and design-time related:

- runtime page hosting now flows through `BeepTabContentHost`, while the remaining design-time work is to polish the commercial authoring workflow around that Beep-owned model
- the page-model cutover needs follow-through in the remaining host/design seams, not a return to native `TabPage`
- the bootstrap adapters still carry too much long-term responsibility
- the current designer path is now Beep-owned, but still needs a more polished commercial authoring workflow
- the README and `.plans/` documents previously overstated delivery status and need to track the real cutover work

## Target Architecture

The commercial target model is:

- `BeepTabs`
	Shell/facade that owns configuration, events, and orchestration, but not low-level header composition.
- `BeepTabPage`
	Beep-owned child container for hosted content plus serializable tab metadata.
- `BeepTabHeaderHost`
	Authoritative owner of layout snapshots, hit testing, pointer state, keyboard routing, overflow, focus visuals, accessibility, and header actions.
- `BeepTabContentHost`
	Stable runtime page host/panel. It owns runtime `BeepTabPage` controls and presents the selected page through bounds, visibility, and z-order.
- `BeepTabItem`
	Immutable render/header snapshot built from page state and consumed by painters and hosts.
- Runtime adapters
	Temporary internal seams only. They may exist while the cutover is in progress, but they are not the premium API.

## Active Roadmap

The active planning set lives under `.plans/`:

- `00-overview-gap-matrix.md`
	Capability matrix, current blockers, and phase ownership.
- `01-phase1-foundation-and-architecture.md`
	Page-model cutover, runtime simplification, metadata ownership, and content-host authority.
- `02-phase2-overflow-header-actions-and-rich-tabs.md`
	Overflow policies, header actions, painter unification, and rich header metadata.
- `03-phase3-document-workspace-and-advanced-interactions.md`
	Document/workspace behaviors, commands, MRU, quick switch, and `DocumentHost` alignment.
- `04-phase4-accessibility-design-time-and-quality.md`
	Accessibility, RTL/high-contrast/touch, designer rebuild, samples, and quality gates.

The canonical file-ownership and execution rules live in `.plans/README.md`.

## Current Status

As of May 2026:

- the planning reset is complete
- the commercial cutover is **not** complete
- the existing host/helpers/model scaffolding is usable and worth keeping
- the remaining work is concentrated in content ownership, API cleanup, designer rebuilding, and roadmap-quality validation

Do **not** treat the current implementation as feature-complete simply because many helper files already exist. The first shipping-quality milestone is the point where the control owns its own page model and the designer/runtime story no longer relies on `TabPage` semantics.

## Direction Rules

- No new premium-facing API may expose `TabPage`.
- No new feature work should rely on native `TabControl` behavior.
- `BeepTabHeaderHost` remains the authoritative header owner.
- `BeepTabContentHost` is the authoritative runtime page host and selected-content presenter.
- `BeepTabItem` remains the canonical header/render snapshot.
- `DocumentHost` integration should be built on shared contracts, not on a competing premium tab model.

## Tracking and Validation

- Roadmap source: `Tabs/.plans/`
- Progress tracker: `MASTER-TODO-TRACKER.md`
- Manual validation surface: `Beep.Sample.Winform.Features/Forms/BeepTabsDemoForm.cs`

The commercialization plan is tracked as active work. Documentation, tracker state, and implementation should stay aligned as each phase is executed.

---

## Current Cutover Checkpoints

The current implementation has crossed several important Phase 1 checkpoints, but it is not release-ready:

- `BeepTabPage` is now the canonical runtime and design-time page model.
- `BeepTabs` stores hosted pages as `BeepTabPage` instances and exposes page-centric hosted-page members, including `CreatePage(...)`, `AddPage(...)`, `InsertPageAt(...)`, `ClearPages()`, the read-only `Pages` surface, and direct `MovePage(...)` reordering.
- Legacy wrapper names such as `SelectedTab`, `AddTab(...)`, `GetTabAt(...)`, and `InsertTabAt(...)` remain only as hidden compatibility aliases so the premium surface stops advertising generic-tab vocabulary.
- The designer now keeps dropped content inside the Beep-owned page model even after all pages were cleared by creating/selecting a page before parent resolution instead of falling back to the outer tabs shell.
- Remaining in-repo callers were migrated off native `TabPage` usage.
- Runtime pages now attach under one stable `BeepTabContentHost` outside design mode instead of being swapped in and out as selected content.
- Runtime content layout is idempotent: normal layout passes resize the existing `BeepTabContentHost` and selected page without detaching, re-showing, or reparenting content. Selection changes update page visibility and z-order only.
- `BeepTabPage` and `BeepTabContentHost` bypass `BaseControl` owner-paint/background-paint and transparent-window behavior locally so hosted controls behave like normal WinForms child content and do not disappear on mouse leave/repaint.
- `BeepTabs`, `BeepTabPage`, and `BeepTabContentHost` opt out of BaseControl mouse input repaint routing so header hover state can repaint only the header and passive content containers do not flicker while the mouse moves over hosted content.
- `BeepTabPage.Visible` is treated as selected-content presentation state only; header snapshots keep all hosted pages visible so unselected page content can be hidden without removing the corresponding tab header or hit-test target.
- `BeepTabHeaderHost` now snapshots directly from `BeepTabs`, and the old `BeepTabsRuntimeBridge` plus its unused header-metrics cache were removed from the tabs core.
- Header painting, hover, pointer, drag, context-menu, and keyboard paths refresh through the same owner-to-host synchronization so action slots, overflow state, selected items, and hit-test rectangles do not drift.
- Keyboard shortcuts now include direct `Ctrl+1` through `Ctrl+9` page selection, including numpad equivalents, matching the shortcut text exposed by the tab accessible objects.
- Accessible tab selection and close-button default actions now activate the same Beep-owned select/close command pipeline used by pointer and keyboard interactions.
- Designer page initialization now works from the canonical `Pages` surface; page order self-heals between the Beep-owned page list and owner control tree for serialization/reload stability; page removal now refreshes designer selection/smart-tag state even when the page is removed through the broader design surface; default page creation now runs only for newly dropped controls so intentionally empty tab sets can persist across designer reloads; and reordering raises `SelectedIndexChanged` when the selected page's index changes.
- The designer smart-tag surface is now page-centric and edits page-owned `BeepTabPage` metadata properties that persist through designer serialization/reload, including title, icon, subtext, badge, selection and close behavior, close-button visibility override, busy state, grouping, preview-slot reuse, and visible workspace state such as pinned, preview, and dirty markers; metadata reset now clears those same serializer-visible page properties through component-change notifications; it also previews current header actions and overflow state without widening the public tabs API.
- Internal header/content host controls are hidden from the toolbox; authoring goes through `BeepTabs` and designer-created `BeepTabPage` instances rather than direct infrastructure drops.
- Runtime state and command-like surfaces such as `SelectedIndex`, `SelectedPage`, `SelectedTab`, `TabCount`, `IsPopupOpen`, and `SelectTabByIndex` are explicitly hidden from designer serialization so generated form code persists pages and authored children rather than transient selection state.
- The remaining gap is a fully unified commercial host workflow that brings runtime and design-time onto the same polished Beep-owned architecture.

Code inspection confirms the serializer-friendly path: pages are designer-host components, design-time pages stay in `BeepTabs.Controls`, dropped controls are parented into the selected `BeepTabPage`, and runtime reload tracks serialized pages through `OnControlAdded`. The focused `BeepTabsPersistenceTests` smoke suite now passes for generated-`InitializeComponent`-style page/control rehydration, active-page child removal, page removal, metadata retention, and empty runtime tab sets. Final proof still requires a manual Visual Studio designer save/reopen/run pass against generated `InitializeComponent` output.

Existing demo and checklist assets remain useful validation surfaces, but they should not be treated as evidence that the commercialization cutover is complete.
