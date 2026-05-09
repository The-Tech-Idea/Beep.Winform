# BeepTabs Overview and Gap Matrix

## Purpose

This document maps the current architectural and product gaps between the existing `BeepTabs` implementation and the commercial-quality tab control it needs to become. The benchmark set is based on mature desktop and product-shell controls, not on cloning a single library API.

## Benchmark Families

- Desktop commercial: DevExpress, Telerik, Syncfusion
- Product shells: Visual Studio, VS Code
- Modern app tabs: Material UI, Ant Design, browser/workbench patterns

## Current Architecture Summary

The codebase already includes valuable premium infrastructure:

- `Hosts/BeepTabHeaderHost*.cs` for owned header behavior
- `Hosts/BeepTabContentHost.cs` for runtime page hosting and selected-content presentation
- `Models/BeepTabItem.cs` and related state models for richer tab metadata
- `Helpers/` for layout, metrics, overflow, commands, MRU, accessibility, RTL, and touch scaffolding
- `Painters/` for multiple visual styles

The core blocker is not lack of features on paper. `BeepTabPage` is now in place as the owned page model, but parts of the hosted-content and designer workflow still carry `TabPage`-shaped assumptions inside a control that is no longer a `TabControl`.

## Gap Matrix

| Capability Family | Current State | Core Gap / Risk | Target State | Phase |
|---|---|---|---|---|
| Content ownership | `BeepTabPage` is now the canonical page model, but runtime and design-time hosting still split across temporary seams | The owned page model exists, but the host workflow is not yet unified into one polished premium path | Finish the Beep-owned host workflow and remove the last `TabPage`-shaped assumptions from the seams | Phase 1 |
| Runtime content hosting | Runtime pages now attach under one stable `BeepTabContentHost` outside design mode, with selection handled by bounds, visibility, and z-order | Regression risk remains if future layout, paint, hover, or header sync paths reintroduce page reparenting or visibility churn | Runtime pages live under one stable `BeepTabContentHost`; layout only resizes; selection changes visibility/z-order; add/remove/reorder are the only parent-changing operations | Phase 1 |
| Public API shape | Primary hosted-page seams now use `BeepTabPage`, but some compatibility-oriented wrapper vocabulary still exists | Commercial surface can still drift toward legacy semantics if cleanup stops here | Page-centric API: `Pages`, `SelectedPage`, `AddPage`, `RemovePage`, `MovePage`, `CreatePage` | Phase 1 |
| Runtime architecture | `BeepTabs` shell, hosted-content code, and adapters still share long-term responsibilities | Bootstrap seams risk becoming permanent architecture | Thin facade plus authoritative hosts and simplified internal controller/bridge | Phase 1 |
| Metadata ownership | Metadata now lives on `BeepTabPage`, with projection into `BeepTabItem` for header rendering | Remaining risk is regression if future work reintroduces parallel ownership | Metadata owned by the Beep page/container model and projected once into `BeepTabItem` | Phase 1 |
| Painter/render contract | Rich host state exists, but painter evolution is still mixed between legacy and newer seams | Styles stay harder to extend consistently | Single item-based painter/render contract with authoritative host-built render context | Phase 2 |
| Overflow behavior | Overflow/action infrastructure exists, but parity depends on the new page model and render unification | Risk of polishing on unstable foundations | Stable overflow policies, popup behavior, and header action layout comparable to commercial controls | Phase 2 |
| Rich header content | Icons, badges, subtext, dirty/busy state, and actions are partially scaffolded | State, layout, and designer support are not yet fully aligned | Rich header metadata driven from one canonical page model | Phase 2 |
| Document/workspace behavior | Mode, MRU, commands, and workspace state scaffolding exist | Needs harder rules, deterministic selection, and product-level polish | Pinned, preview, dirty, MRU, quick-switch, context-menu, and close-policy parity | Phase 3 |
| DocumentHost relationship | Adapter seam exists but the long-term ownership model is undecided | Two premium tab stacks could diverge | Make `BeepTabs` the shared substrate or define a strict adapter boundary | Phase 3 |
| Accessibility and input | Accessibility, focus, RTL, high-contrast, and touch helpers exist | Must be validated against the final page/content model and designer | Full WCAG-minded accessibility and robust keyboard/touch behavior | Phase 4 |
| Design-time workflow | Designer now creates/selects `BeepTabPage`, but the authoring workflow is still thin and partially split from runtime hosting | Prevents a professional authoring experience | Beep-owned page designer flow with page metadata editing and smart presets | Phase 4 |
| Samples, docs, and regression | Demo exists, but docs and status narratives were stale | Teams can implement against the wrong mental model | Roadmap, README, tracker, and demos aligned with the active commercialization plan | Phase 4 |

## Strengths To Preserve

- Multiple painter styles
- Four header positions
- Theme and DPI integration
- Owned header-host direction
- Command-routing and MRU scaffolding
- Accessibility and touch-helper groundwork

## Code-Level Benchmark Findings

The open-source WinForms controls that are closest to commercial behavior converge on the same hosting rules:

- Native WinForms `TabControl` and Cyotek `TabList` keep pages in a stable page collection/control tree; selection sizes the selected page and hides non-selected pages.
- Krypton `KryptonNavigator` uses a stable child panel for pages; selected-page changes update layout and z-order rather than reparenting on every layout pass.
- DockPanelSuite separates tab-strip visuals from content lifetime; clicking a tab switches active content, while header paint and hit testing do not own content parenting.

For `BeepTabs`, the commercial target is the Krypton/native hybrid: Beep-owned pages, one stable runtime content host, and no page reparenting from ordinary hover, paint, resize, or layout.

## Architectural Direction

### Short-Term Direction

- Stop expanding any premium-facing surface that exposes `TabPage`.
- Build all remaining seams on the Beep-owned page/container model first.
- Keep `BeepTabHeaderHost` authoritative for header behavior.
- Move runtime page hosting and selected-content presentation fully behind `BeepTabContentHost`.
- Make `ApplyHostedSourceContentBounds` and content-host layout idempotent: ordinary layout resizes existing host/page controls only.
- Keep the selected page visible/topmost and hide or send back unselected runtime pages without removing them from the host.
- Treat adapters as internal migration seams only.

### Long-Term Direction

- BeepTabs owns its own header and content architecture end to end.
- The render path is item-based, not page-based.
- Design-time works with Beep-owned pages and metadata.
- `DocumentHost` uses the same contracts or an explicit adapter boundary, not a competing premium tab model.

## Non-Goals

- Preserving stock `TabControl` as the long-term architecture
- Keeping `TabPage` as the premium page/content contract
- Building full docking/tear-out behavior in the first commercialization pass
- Copying one third-party API exactly

## Acceptance Baseline For The Roadmap

- Every phase maps to concrete files or planned files in `Tabs/` or the design server.
- No phase adds new premium-facing APIs that expose `TabPage`.
- The README, `.plans/`, and tracker stay aligned with actual status.
- The roadmap optimizes for a professional, shippable control rather than incremental compatibility patches.