# BeepDialogManager — Enhancement Program (Master Tracker)

Target: a dialog layer measured against Radix UI / shadcn Dialog, Headless UI, Material 3 dialogs,
Fluent 2, Ant Design Modal, and the desktop bar set by DevExpress `XtraDialog`, Telerik `RadDialog`,
VS Code's modal + Quick Pick, and macOS/Windows platform sheets.

Written from a full read of `DialogsManagers/` — **9,829 lines across 42 files**. Every claim below
cites the code it came from. Where a claim is unverified, it says so.

## Ground rules

These are constraints, not aspirations. They are the same rules the `BeepTabs` program ran under.

1. **No stubs.** A method that exists must do something. An empty body is a lie about capability.
2. **No legacy.** No back-compat shims, no `[Obsolete]` alias kept beside its replacement. When
   something is replaced, the old one is deleted.
3. **No swallowed exceptions.** A bare `catch { return fallback; }` converts a bug into a silent
   wrong answer. Catch what you can genuinely handle, and surface the rest.
4. **No duplication.** One implementation per concept.
5. **Layout via `TableLayoutPanel`.** Dialog structure is a layout problem; it belongs in a layout
   manager, not in absolute coordinates or a stack of docked panels.
6. **Verify by measurement.** A claim about behaviour is backed by a probe run or a render.

## Phases

Ordered so that the things which make later work verifiable come first, and so that the two changes
that alter public API land before anything is built on top of them.

| Phase | Theme | Features |
|---|---|---|
| **1 — Ground truth** | Make the code honest and testable before changing behaviour | [12](12-verification-harness.md), [04](04-dead-scaffolding.md), [03](03-exception-policy.md) |
| **2 — One way to do each thing** | Collapse the duplicate API and model surface | [01](01-api-surface.md), [02](02-result-and-config-model.md) |
| **3 — Structure** | Layout manager and separation in the dialog forms | [05](05-layout-and-composition.md), [06](06-designer-serialization.md) |
| **4 — Pipeline** | One placement, motion and lifecycle path | [07](07-placement-and-motion.md), [09](09-progress-and-busy.md) |
| **5 — Product surface** | Parity with the reference products | [08](08-notifications.md), [10](10-command-palette.md) |
| **6 — Accessibility & design-time** | The parts a screen reader and the VS designer see | [11](11-accessibility.md) |

## Feature index

| # | Feature | Doc | State | Priority |
|---|---------|-----|-------|----------|
| 1 | API surface & duplicate entry points | [01](01-api-surface.md) | 6 alias pairs; one silently bypasses the pipeline | P0 |
| 2 | Result & config model | [02](02-result-and-config-model.md) | three result types for one concept | P0 |
| 3 | Exception policy | [03](03-exception-policy.md) | 4 bare catches | P0 |
| 4 | Dead scaffolding | [04](04-dead-scaffolding.md) | test form, orphan `.resx`, dead engine | P0 |
| 5 | Layout & composition | [05](05-layout-and-composition.md) | **no dialog uses `TableLayoutPanel`** | P1 |
| 6 | Designer serialization | [06](06-designer-serialization.md) | 30 leaked `GraphicsPath` per dialog | P1 |
| 7 | Placement & motion | [07](07-placement-and-motion.md) | engine has **zero callers**; 17 scattered sites | P1 |
| 8 | Notifications & toasts | [08](08-notifications.md) | unaudited | P2 |
| 9 | Progress & busy | [09](09-progress-and-busy.md) | unaudited | P2 |
| 10 | Command palette | [10](10-command-palette.md) | unaudited | P2 |
| 11 | Accessibility & keyboard | [11](11-accessibility.md) | per-form `ProcessCmdKey`, no `AcceptButton` | P2 |
| 12 | Verification harness | [12](12-verification-harness.md) | **build this first** | P0 |

## Headline findings

**`ShowInfo` is not the alias it looks like.** `Warning`/`ShowWarning`, `Error`/`ShowError` and
`Question`/`ShowQuestion` are character-identical pairs where the `Show*` form carries
`[Obsolete("Use X instead.")]`. `ShowInfo` carries no such attribute — and is the one that behaves
differently: it constructs `BeepMessageDialog` directly and **bypasses the pipeline**, so theming,
animation, placement, state persistence and the `DialogOpened`/`DialogConfirmed` events do not happen.
Its own XML comment says so. A caller following the deprecation guidance would assume
`ShowInfo` → `Info` is the same trivial rename the other three are; it is not.
See [01](01-api-surface.md).

**Three types mean "the result of a dialog".** `DialogReturn` (in
`TheTechIdea.Beep.Vis.Modules2.0/IDialogManager.cs`) is what every public method actually returns.
`DialogsManagers.Models.DialogResult` is a 243-line local class referenced by exactly one callback
signature and two doc comments — and it **shadows `System.Windows.Forms.DialogResult`**, which is why
44 call sites across 8 files have to write the framework type fully qualified.
See [02](02-result-and-config-model.md).

**`DialogPlacementEngine` has zero callers.** It exists, it is complete, and no code outside its own
file references it — while 17 sites set `StartPosition`/`CenterParent` by hand. This is the seventh
instance in this codebase of complete, plausible code that nothing calls; the `BeepTabs` program
found six. See [07](07-placement-and-motion.md).

**Every dialog leaks GDI+ handles at construction.** `BeepInputDialog.Designer.cs` allocates **30**
`GraphicsPath` objects in `InitializeComponent` and disposes none; `TestDialogForm` adds 6 more. They
are assigned to `BorderPath`, `ContentShape` and `InnerShape` — runtime-computed geometry properties
that are designer-serializable, so the designer has written out 30 *empty* paths and assigns them
over the computed geometry. This is also why that one designer file is 2,060 lines. The fix is
upstream on the control that exposes those properties. See [06](06-designer-serialization.md).

**No dialog form uses `TableLayoutPanel`.** Every form positions with absolute `Location` plus
`Dock`/`Anchor`. This is the structural change you asked for and it is also a standing rule for this
codebase. See [05](05-layout-and-composition.md).

**Four bare `catch` blocks.** Two in `DialogStateStore` mean a corrupt or unwritable state file
silently discards every remembered dialog position — one of them is commented *"Silently fail —
persistence is non-critical"*. One in `DialogResult.GetData<T>` uses `InvalidCastException` as a type
test where `value is T` is the correct construct. See [03](03-exception-policy.md).

**Shipped scaffolding.** `TestDialogForm` (455-line designer + code) has no references anywhere in
the solution, and `Forms/BeepDialogForm.resx` has no matching `.cs`. See [04](04-dead-scaffolding.md).

## Prior art in this repo

Four defect classes have recurred across `BeepGridPro`, `BeepTree`, `ToolTips` and `BeepTabs`. All
four are already visible here, which is why the harness in [12](12-verification-harness.md) tests for
them explicitly rather than waiting to be surprised:

- **Complete code that nothing calls** — six instances in `BeepTabs`; `DialogPlacementEngine` is the
  first one found here.
- **Two implementations of one concept** — three placement engines in `ToolTips`, two layout engines
  in `BeepTree`; here it is the result model and the show-a-dialog entry points.
- **Measure with one font, draw with another** — not yet checked in this directory.
- **A declared property nothing reads** — `ToolTips` had six; `DialogConfig` is 1,221 lines with 32
  static factories and has not been audited.
