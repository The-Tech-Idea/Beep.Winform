# BeepDocumentHost Gap Matrix (Phase 1 Baseline)

## Baseline
This matrix compares current `DocumentHost` capabilities with expected commercial-grade behavior for WinForms document/docking controls used at design time.

Scoring:
- `Present`: implemented and usable now
- `Partial`: implemented but missing reliability/design-time parity
- `Missing`: not implemented

## Feature Matrix
| Area | Current | Status | Gap to Commercial Parity | Priority |
|---|---|---|---|---|
| Core tabbed documents | Add/activate/close/pin/reorder/overflow exist | Present | Need strict lifecycle contracts and tests | High |
| Split groups | Horizontal/vertical split + group move exist | Partial | Limited topology (not full nested layout tree), max-groups model is flat | High |
| Docking guides overlay | Overlay exists for floated docs | Partial | Only center docking finalized; side docking to split is marked future | High |
| Floating windows | Float + dock back implemented | Partial | Close semantics for floated docs can emit close event without explicit close intent | High |
| Auto-hide | Auto-hide strips/overlay exist | Partial | Integration with layout persistence and designer commands is limited | Medium |
| Cross-host drag | Transfer between hosts supported | Partial | Needs stronger transaction/event consistency and design-time safeguards | Medium |
| MRU / quick switch / reopen | Implemented | Present | Needs formal ordering contract and state restoration hooks tests | Medium |
| Tab visual styles | Multiple styles implemented | Present | Needs per-theme QA baseline for all styles/states | Medium |
| Accessibility | Custom accessible objects exist | Partial | Needs verification for screen reader workflow and keyboard narration | Medium |
| Serialization schema | JSON save/restore exists (`SchemaVersion = 1`) | Partial | Captures tabs + active + scroll only; no groups/split/float/auto-hide geometry | High |
| Layout migration | No explicit migrator | Missing | No forward/backward compatibility framework | High |
| Error handling in restore | basic `try/catch` + skip invalid | Partial | No partial-recovery report, diagnostics, or conflict policy | Medium |
| Designer action list | Rich smart-tag actions exist | Present | Many actions mutate runtime state directly without designer transactions | High |
| Designer verbs | Add/Close active verbs exist | Partial | No verbs for split/group/layout persistence or guarded destructive actions | Medium |
| Designer undo/redo integrity | Property changes use change service | Partial | Method actions (add/split/close/float) are not transaction-wrapped | High |
| Designer serialization to `InitializeComponent` | Implicit via runtime API usage | Partial | No explicit design-time collection model/component serializer strategy | High |
| Designer glyphs/adorners | Snap lines implemented | Partial | No docking glyphs, no split-drop target adorners | High |
| Property grid experience | Pre-filter hides panel plumbing | Present | No advanced editors (layout picker, theme picker, icon picker) | Medium |
| Runtime/design-time isolation | Basic checks by behavior | Partial | Some operations can spawn windows/popups in design-time context | High |
| Performance at scale | No benchmark artifacts | Missing | No measured targets for 100+ docs, drag latency, allocation profile | Medium |
| Automated tests | Not evident for DocumentHost feature surface | Missing | Need lifecycle/layout/serialization/design-time test suites | High |

## Key Risks Found in Current Baseline
1. Layout model is not yet expressive enough for commercial parity (flat group list vs hierarchical docking tree).
2. Designer operations are not consistently transaction-safe, so undo/redo and code serialization reliability can drift.
3. Serialization does not include split groups, float geometry, or auto-hide state, which blocks true workspace persistence.
4. Float and close pathways need explicit, deterministic event ordering contracts to avoid accidental data-loss behavior.

## Phase 1 Decisions
1. Freeze current public API behavior as `v1 compatibility mode`.
2. Define `v2 contract` before adding features.
3. Build layout-tree + serializer redesign first, then expand designer UX on top.
