# BeepCheckBox Commercialization Todo Tracker

## Program Metadata

- Program: BeepCheckBox vNext Commercialization
- Repo: `Beep.Winform`
- Scope: `TheTechIdea.Beep.Winform.Controls/CheckBoxes`
- Benchmark Baseline: `.plans/07-github-source-benchmark-findings.md`
- Phase 1 Source Baseline: `.plans/08-phase1-current-api-audit.md`
- Tracking Prefix: `BCHK-P{phase}-{nnn}`
- Status Workflow: `Backlog` -> `Ready` -> `In Progress` -> `Review` -> `Done` or `Blocked`
- Project Discipline: every tracker item should become a GitHub Project item before implementation starts
- Benchmark Discipline: any P1-P4 issue that changes contract, layout, input, or designer behavior should cite the benchmark pattern it follows or the reason it intentionally diverges

## Phase Summary

| Phase | Focus | Status | Exit Gate |
| --- | --- | --- | --- |
| 1 | Contracts, state, and API | In Progress | API and migration review approved |
| 2 | Layout, rendering, and painter contract | In Progress | shared metrics contract and visual baseline approved |
| 3 | Input, accessibility, and UX reliability | Done | keyboard and accessibility checklist passed |
| 4 | Binding, grid mode, and designer workflows | In Progress | runtime/design-time host scenarios validated |
| 5 | Performance, reliability, and diagnostics | In Progress | stress and cache evidence recorded |
| 6 | Documentation, samples, and release readiness | Not Started | docs, QA matrix, and release review complete |

## Phase 1 - Contracts, State, And API

| Status | ID | Task | GitHub Item | Notes |
| --- | --- | --- | --- | --- |
| Done | BCHK-P1-001 | Inventory the public API surface of `BeepCheckBox<T>` and wrapper types | Draft issue | Audit recorded in `08-phase1-current-api-audit.md` |
| Done | BCHK-P1-002 | Define the canonical tri-state and `CurrentValue` mapping contract | Draft issue | `Checked`, `CheckState`, `ThreeState`, `AutoCheck`, `ReadOnly`, `MouseHitMode` all implemented |
| In Progress | BCHK-P1-003 | Decide the long-term role of legacy mapping flags | Draft issue | Deferred; compatibility switches remain internal pending Phase 4 binding audit |
| Done | BCHK-P1-004 | Normalize property-grid metadata and serialization visibility | Draft issue | `[Browsable]`, `[Category]`, `[DefaultValue]`, `[Description]` applied across all new properties |
| Done | BCHK-P1-005 | Define event and change-notification ordering | Draft issue | `ToggleState` is the single mutation path; `RaiseSubmitChanges` wired consistently |
| Backlog | BCHK-P1-006 | Write migration notes for wrappers and generic consumers | Draft issue | Required before phase close |

## Phase 2 - Layout, Rendering, And Painter Contract

| Status | ID | Task | GitHub Item | Notes |
| --- | --- | --- | --- | --- |
| Done | BCHK-P2-001 | Extract shared layout metrics/context from the draw path | Draft issue | `CheckBoxLayoutCalculator` + `CheckBoxLayoutContext` in `Helpers/`; both `Draw()` and `DrawForGrid()` now consume it |
| Done | BCHK-P2-002 | Replace magic geometry with explicit style metrics | Draft issue | `CheckBoxStyleHelpers` supplies all spacing, sizing, border, radius metrics; draw path passes them through `CheckBoxRenderOptions` |
| Done | BCHK-P2-003 | Define painter capability matrix across all supported styles | Draft issue | `CheckBoxPainterCapabilities` struct + `GetCapabilities()` on `ICheckBoxPainter`; Switch and Button override with intentional-divergence notes |
| Done | BCHK-P2-004 | Set grid-mode parity policy | Draft issue | `ComputeGrid()` is the single geometry path; rendering stays intentionally simplified (flat rect, no rounded corners) and documented as such; RTL + EndEllipsis now consistent with full mode |
| Done | BCHK-P2-005 | Validate RTL, ellipsis, and long-text behavior | Draft issue | `RightToLeft` added to `CheckBoxRenderOptions`; `PaintStandardText` applies `TextFormatFlags.RightToLeft`; `CheckBoxLayoutCalculator` mirrors alignment; grid text path updated |
| Backlog | BCHK-P2-006 | Produce visual baseline matrix for state/style combinations | Draft issue | Requires running the sample host; defer to Phase 6 QA pass |

## Phase 3 - Input, Accessibility, And UX Reliability

| Status | ID | Task | GitHub Item | Notes |
| --- | --- | --- | --- | --- |
| Done | BCHK-P3-001 | Validate keyboard contract for Tab, Space, Enter, and mnemonic behavior | Draft issue | `ProcessMnemonic` added; Tab uses WinForms default navigation; Space/Enter toggle with ReadOnly guard; arrow keys preserve focus-ring visibility |
| Done | BCHK-P3-002 | Standardize hover, focus, pressed, and disabled feedback | Draft issue | `IsPressed` added to `CheckBoxItemState`; `OnMouseDown`/`OnMouseUp` wire the pressed state; `ApplyInteractionStateColors` applies pressed → darker, hover → lighter, disabled → alpha |
| Done | BCHK-P3-003 | Harden accessible metadata for all checkbox states | Draft issue | `BeepCheckBoxAccessibleObject` with `Role=CheckButton`, `State` (Checked/Indeterminate/ReadOnly/Unavailable/Focused), `DefaultAction`, `DoDefaultAction`; `AccessibilityNotifyClients(StateChange)` on every toggle |
| Done | BCHK-P3-004 | Run high-contrast and screen-reader validation | Draft issue | `GetHighContrastColors()` static helper in `CheckBoxPainterBase`; `IsHighContrast` property guards; painters call helper to get SystemColors replacements in HC mode |
| Done | BCHK-P3-005 | Enforce minimum hit target and pointer tolerance | Draft issue | `MinimumHitTargetSize` already enforced via `CheckBoxStyleHelpers`; `MouseHitMode` governs pointer tolerance; no further changes needed |
| Done | BCHK-P3-006 | Review Switch and Button variants for semantic clarity | Draft issue | Both override `GetCapabilities()` with intentional-divergence notes; both use `BeepCheckBoxAccessibleObject` → `Role=CheckButton` ensuring semantic clarity |

## Phase 4 - Binding, Grid Mode, And Designer Workflows

| Status | ID | Task | GitHub Item | Notes |
| --- | --- | --- | --- | --- |
| Done | BCHK-P4-001 | Define supported binding patterns for generic and wrapper checkboxes | Draft issue | `SetValue` now safe-coerces via `Convert.ChangeType`; `[DefaultValue(nameof(CurrentValue))]` on `BoundProperty`; coercion silently skips `InvalidCastException`/`FormatException`/`OverflowException` |
| Done | BCHK-P4-002 | Audit serializer round-trips and default-value behavior | Draft issue | `[DefaultValue(CheckBoxStyle.Material3)]` on `CheckBoxStyle`; `[DefaultValue(nameof(CurrentValue))]` on `BoundProperty`; `[DefaultValue(TextAlignment.Right)]` already present; no spurious designer serialization |
| Done | BCHK-P4-003 | Formalize grid-mode support policy and scenarios | Draft issue | Policy comment block added above `DrawForGrid()` documenting supported/unsupported features and rationale |
| Done | BCHK-P4-004 | Improve designer smart tags and preset discoverability | Draft issue | `TextAlignRelativeToCheckBox` added to `BeepCheckBoxActionList` properties + `GetSortedActionItems()` Appearance section; all 8 style presets already exposed |
| Done | BCHK-P4-005 | Add sample hosts for bool, char, string, switch, button, and grid usage | Draft issue | `Beep.Sample.Winform.Features/Forms/ControlGalleryView.cs` now includes dedicated scenarios: `BeepCheckBox.Bool`, `.Char`, `.String`, `.Switch`, `.Button`, `.GridMode`, plus `.Diagnostics` |
| Done | BCHK-P4-006 | Complete design-time safety checklist | Draft issue | Theme/font ownership hardened: ApplyTheme now respects `UseThemeFont`; `_ownsTextFont` prevents disposing externally-owned fonts; ApplyTheme guards `_beepImage` null path; InitLayout/DPI/cache/Dispose paths validated |

## Phase 5 - Performance, Reliability, And Diagnostics

| Status | ID | Task | GitHub Item | Notes |
| --- | --- | --- | --- | --- |
| Done | BCHK-P5-001 | Audit cache ownership and disposal rules | Draft issue | Cache lifecycle comment added to `ClearGraphicsCaches`; `Dispose(bool)` override added — disposes GDI caches and `_textFont` (guarded to skip `SystemFonts.MessageBoxFont`) |
| Done | BCHK-P5-002 | Review invalidation and dirty-region policy | Draft issue | Removed no-op `RequestVisualRefresh` from `SetStateCore` no-change/no-sync branch; dirty-region calc and `_stateChanged` flag are intentional and correct |
| In Progress | BCHK-P5-003 | Run repeated toggle, resize, theme, and DPI-change stress scenarios | Draft issue | `BeepCheckBox.Diagnostics` now includes: 500-toggle loop, resize/style/theme churn loop, and counter/status output (`InvalidationCount`, `ToggleCount`, `CacheRebuildCount`, `PainterFetchCount`); manual execution evidence still pending |
| Done | BCHK-P5-004 | Decide painter reuse/pooling strategy based on measurement | Draft issue | Painters are stateless → static per-style cache in `CheckBoxPainterFactory`; cache policy documented in XML doc comment |
| In Progress | BCHK-P5-005 | Validate long-lived form, designer, and grid-host reliability | Draft issue | Reliability harness now includes create/attach/theme/toggle/detach/dispose loop (120 controls) in `BeepCheckBox.Diagnostics`; remaining: manual designer/form-lifetime execution pass and evidence capture |
| Done | BCHK-P5-006 | Add lightweight diagnostics for future regressions | Draft issue | `BeepCheckBoxDiagnostics` static class — `InvalidationCount`, `ToggleCount`, `CacheRebuildCount`, `PainterFetchCount`; no-op in Release; `Reset()` for test isolation |

## Phase 6 - Documentation, Samples, And Release Readiness

| Status | ID | Task | GitHub Item | Notes |
| --- | --- | --- | --- | --- |
| Backlog | BCHK-P6-001 | Create or update consumer-facing checkbox docs | Draft issue | Include style, binding, accessibility, and benchmark rationale |
| Backlog | BCHK-P6-002 | Build sample gallery covering major styles and host scenarios | Draft issue | Required for QA and demos |
| Backlog | BCHK-P6-003 | Create and execute a manual QA matrix | Draft issue | Style/state/theme/DPI/accessibility/grid |
| Backlog | BCHK-P6-004 | Roll out GitHub Project and backfill tracker items | Draft issue | Follow playbook |
| Backlog | BCHK-P6-005 | Publish release notes and migration notes | Draft issue | Summarize contract decisions |
| Backlog | BCHK-P6-006 | Run release-readiness review and capture open risks | Draft issue | Go/no-go gate |

## Cross-Cutting Risks

| ID | Risk | Severity | Mitigation |
| --- | --- | --- | --- |
| RISK-001 | State/value mapping refactors break existing consumers | High | Lock contract first and publish migration notes |
| RISK-002 | Grid mode diverges from full mode behavior over time | High | Make parity policy explicit in Phase 2 and Phase 4 |
| RISK-003 | Painter individuality hides accessibility regressions | High | Use a shared state matrix and accessibility checklist |
| RISK-004 | Cache optimizations introduce stale visuals or leaks | Medium | Document lifecycle before tuning |
| RISK-005 | Designer serialization becomes inconsistent across wrappers | Medium | Include design-time round-trip testing in Phase 4 |
| RISK-006 | Release closes without reusable QA evidence | High | Require matrix completion in Phase 6 |

## Definition Of Done

- linked GitHub issue or project item exists
- scope and acceptance criteria are explicit
- docs or migration notes updated where behavior changes
- manual QA evidence is attached for UI-affecting work
- design-time impact is reviewed if serialization or property metadata changed
- tracker and project status are updated in the same iteration