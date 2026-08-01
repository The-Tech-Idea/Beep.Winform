# DisplayContainers — enhancement program

Tab-hosting containers: the document-host surface that presents addins as tabs, with a painted tab
strip (header), content area, and the input that binds them.

## Scope

`DisplayContainers/` — both container implementations, the four helpers under `Helpers/`, and the
tab-strip painting, sizing and alignment that the header depends on.

## Ground rules

Carried from the Tabs, ToolTips and DialogsManagers programs, and restated because every one of them
is currently violated somewhere in this folder:

- **No stubs.** An empty body is a claim of capability that isn't there.
- **No legacy, no fallback.** One implementation per concept. A fallback path that draws differently
  from the live path is a second implementation that nobody tests.
- **No swallowed exceptions.** There are **44** `catch` blocks in this folder. A caught-and-ignored
  exception during layout or paint produces a silently wrong container.
- **No duplication.** Two containers, two text-draw paths, two sources of tab geometry.
- **Measure what you claim.** Every visual assertion needs a controlled baseline, or it measures the
  wrong thing. This was learned the expensive way: eight harness checks in the preceding programs
  returned confident false verdicts, and the baseline caught all eight.

## Where this program starts

Everything below was read from the code in this folder, not assumed. Two prior documents existed
(`enhancmentplan.md` and `BeepDisplayContainer2_COMPLETED.md`, both deleted in `a8901646` and
recoverable from `HEAD~2`). That work — zero-tab lifecycle reset, keeping the header live when empty,
the `NewTabRequested` contract, and a first metrics pass — **is done and is not re-proposed here.**

## Phases

| # | Feature | Why it leads |
|---|---|---|
| [01](01-container-consolidation.md) | Two containers, one used | Structural. Blocks everything else. |
| [02](02-header-metrics-and-alignment.md) | Close button, badge, hit targets | The alignment defects you can see |
| [03](03-measure-draw-contract.md) | One geometry source | Badges are drawn but never measured |
| [04](04-tab-strip-layout.md) | Overflow, scroll, pinned | Sizing under pressure |
| [05](05-vertical-tab-positions.md) | Left/Right tab text | No rotation exists |
| [06](06-painting-and-state.md) | States, theme, the fallback path | Hardcoded `SystemColors` |
| [07](07-exception-policy.md) | 44 catch blocks | Ground rule |
| [08](08-verification-harness.md) | Render harness | Proves the rest |

## Verification

A render harness (`scratchpad/ContainerProbe`) is built in phase 08 and every earlier phase's claim
is asserted through it. The pattern is established: render, compare against a controlled baseline,
and treat a passing check as suspect until the baseline proves it can fail.
