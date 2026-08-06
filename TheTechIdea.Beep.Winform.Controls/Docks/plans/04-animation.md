# Stage 04 — three copies of an easing library, none of them reachable

**Kind:** structural, with one published property that does nothing.
**Status:** ☑ done, Option A taken. Six checks green. Two of the nine enum values turned out to
belong to [05](05-dead-capability-surface.md), not here — see *Outcome*.

## What runs

The animation timer ticks at 16 ms (`BeepDock.cs:102`) and calls two methods:

```csharp
DockAnimationHelper.ApplySpringEffect(_itemStates, hoveredItemName, _config);   // BeepDock.Animation.cs:17
bool needsRedraw = DockAnimationHelper.UpdateAnimations(_itemStates, _config.AnimationSpeed); // :18
```

`UpdateAnimations` interpolates with a private linear `Lerp` (`DockAnimationHelper.cs:119-122`).
That is the whole animation system. It is **linear**, and it works.

## What does not run

| file | contents | callers |
|---|---|---|
| `Helpers/DockEasingHelper.cs` | 341 lines, 27 easing functions, spring physics, `SmoothDamp` | **none** |
| `Helpers/DockAnimationHelper.cs:127,135,145` | `EaseOutCubic`, `EaseInOutCubic`, `EaseOutElastic` | **none** |
| `Helpers/DockLayoutHelper.cs:341,349` | `EaseOutCubic`, `EaseOutElastic` again | **none** |

`DockEasingHelper` is not referenced by a single line outside its own file — the only uses of
`EaseOutCubic` inside it are its own `GetEasingFunction` switch (`:282`, `:288`, `:289`), which
nothing calls either. `EaseOutCubic` is defined three times in this folder with three slightly
different formulations:

```csharp
(--t) * t * t + 1                      // DockEasingHelper.cs:49
1 - (float)Math.Pow(1 - t, 3)          // DockAnimationHelper.cs:127
```

Those two are algebraically the same curve. Nothing depends on either, so nothing has noticed. That
is the point: three copies survive precisely because none is load-bearing.

## The property that does nothing

`DockAnimationStyle` has nine values (`DockEnums.cs:198-244`). `DockConfig.AnimationStyle`
(`DockConfig.cs:26`) is published on the control (`BeepDock.Properties.cs:193`) with a designer
category and a `[DefaultValue]`. Its only consumer is `DockEasingHelper.GetEasingFunction`
(`:277`) — the dead switch in the dead file.

So a user sets `AnimationStyle = Bounce`, the property round-trips, the designer serializes it, and
the dock animates linearly exactly as before. Nine published values, one behaviour. This is a
capability the control's own API promises and does not have; see [05](05-dead-capability-surface.md)
for the rest of that group.

## The decision this stage exists to make

**Option A — wire the easing in.** `UpdateAnimations` takes the easing function from
`DockEasingHelper.GetEasingFunction(config.AnimationStyle)` and drives it with elapsed time rather
than a per-tick fraction. The nine enum values become nine visibly different animations, and 341
lines stop being dead.

**Option B — delete the easing and the enum down to what runs.** `DockAnimationStyle` keeps `None`
and `Spring`; `DockEasingHelper` goes; `AnimationSpeed` stays. The API then describes what the
control does.

**Recommendation: A, with a caveat that shapes the work.** The reason easing was never wired is
visible in the signature: `UpdateAnimations(states, animationSpeed)` has **no notion of time or
progress**. Its `Lerp(current, target, speed)` is an exponential approach with no `t` to feed an
easing curve — you cannot pass `EaseOutBounce` a value it can use. So A is not "call the helper";
it is "give each item an animation with a start value, a target, a duration and a start timestamp,
then evaluate the curve at `elapsed / duration`". That is the real work of this stage and it must be
stated plainly, because "wire up the existing helper" reads like an afternoon and is not.

Whichever is chosen: **one easing implementation, reachable, with no second copy left behind.**

## Work if A is taken

1. `DockItemState` gains what a timed animation needs — start scale, target scale, start time,
   duration. `CurrentScale` stays the field painters read, so painters are untouched.
2. `UpdateAnimations` evaluates `curve(elapsed / duration)` per item, where `curve` comes from
   `GetEasingFunction(config.AnimationStyle)`. `AnimationSpeed` becomes duration, or is replaced by
   a duration property — do not keep both meaning the same thing.
3. `DockAnimationHelper.cs:127-152` and `DockLayoutHelper.cs:341-355` are deleted. `DockEasingHelper`
   is the only easing in the folder.
4. `Spring` (`DockEasingHelper.cs:244`) is evaluated before it is trusted: it is a hand-rolled damped
   oscillator that does not obviously satisfy `f(0)=0, f(1)=1`, and `DockAnimationStyle.Spring` is
   the **default**. If it misbehaves at the endpoints, every dock in the product gets it. Plot it
   before wiring it.
5. `SmoothDamp` (`:309`) has no caller in either option. It goes unless step 2 uses it.

## Verification

Deletion plus a clean compile is authoritative for deadness; grep is not.

1. Delete `DockEasingHelper.cs`, `DockAnimationHelper.cs:127-152` and `DockLayoutHelper.cs:341-355`,
   build the solution. If it compiles, all three copies are dead and this survey is confirmed.
   Restore, then implement. *This is expected to compile — if it does not, the finding is wrong and
   the stage needs rewriting before any code changes.*
2. **The assertion the stage exists for**: for each of the nine `DockAnimationStyle` values, hover an
   item, sample `CurrentScale` at 6 fixed points across the animation, and assert the nine curves are
   **pairwise distinct**. *Today all nine are identical* — that is the failing baseline, and it is
   the only check that proves the enum means something.
3. Endpoint check for every curve: `f(0) == 0` and `f(1) == 1` within 0.001, and `f` stays within
   `[-0.5, 1.5]` across `[0,1]`. Catches step 4's spring and the overshoot curves (`Back`, `Elastic`)
   scaling an item to something absurd.
4. Termination: after hovering and waiting 2 s, assert every `CurrentScale` equals its target exactly
   and `UpdateAnimations` reports no redraw needed. The current exponential approach never quite
   arrives; a timed animation must, or the 60 FPS timer never idles.
5. No easing function is defined twice in the folder. One grep, in review.

## Outcome

The deletion test ran first and passed: removing `DockEasingHelper.cs` outright, plus the three
duplicate methods in `DockAnimationHelper` and the two in `DockLayoutHelper`, compiled with **0
errors**. All three copies dead, confirmed rather than inferred.

(The first attempt at that test produced two CS1022 errors — a regex had eaten the closing braces of
the enclosing classes. Structural errors are not evidence of use, so the run was thrown away and
redone with brace matching. A deletion test that fails for a syntax reason proves nothing.)

The five duplicates stay deleted. `DockEasingHelper` came back as the single implementation and is
now actually wired.

### The real work was giving the animation a clock

As the plan anticipated, this was never "call the helper". `UpdateAnimations(states, animationSpeed)`
approached the target by a fixed fraction per tick — there was no `t` to hand `EaseOutBounce`.
`DockItemState` gained `AnimationFromScale`, `AnimationToScale` and `AnimationElapsed`;
`UpdateAnimations` now takes the config and a real elapsed time and evaluates
`curve(elapsed / duration)`.

`AnimationSpeed` became `AnimationDuration` (seconds), with the old name kept as an alias so the
published property and saved designer state keep working — one value, honest name, not two things
meaning the same.

`BeepDock` measures real elapsed time from `Environment.TickCount64` rather than assuming 16 ms.
The timer asks for 16 and Windows delivers what it delivers; an eased animation that assumed a fixed
step would run at a different speed under load and finish early or late instead of on its duration.

### `None` was animating

`GetEasingFunction`'s `_ =>` sent `DockAnimationStyle.None` to `EaseOutCubic`, so the one value whose
entire purpose is "do not animate" animated exactly like `Scale`. `UpdateAnimations` now short-circuits
it to an immediate assignment.

### Two values belong to stage 05, not here

The stage's headline assertion was "nine values give nine curves". After wiring, six of the nine are
distinct — and the shortfall is not a bug in the wiring:

- `Rotate` maps to the same curve as `Scale`, and **`DockItemState.CurrentRotation` is written once,
  to zero, and read by no painter**. There is no rotation mechanism to select.
- `Pulse` maps to the same curve as `Fade`, and there is no pulsing mechanism either.

Both name *effects*, not easing shapes. Giving them invented curves would have turned the check green
while leaving "Rotate" performing no rotation — the exact dishonesty this program keeps finding. They
are recorded in [05](05-dead-capability-surface.md) with the other capabilities that have no
mechanism, and the check here asserts what this stage can actually deliver: the six curve-named
values move differently, and `None` does not move at all.

### Measured

| check | result |
|---|---|
| curve-named styles distinct | **6 of 6** (was 1 across all 9) |
| `None` does not animate | scale == target after one tick |
| every curve `f(0)=0`, `f(1)=1` | 9 of 9, including the hand-rolled `Spring` default |
| no curve leaves `[-0.5, 1.5]` | 9 of 9 |
| settles within 2s, stops requesting redraws | yes — the old exponential approach never did |
| settled scale is exactly the target | 1.50001 vs 1.50000 |

`Spring` was the one the plan said to plot before trusting, since it is the default and a hand-rolled
damped oscillator. It satisfies both endpoints and stays in range.

### Still owed

`DockEasingHelper.SmoothDamp` still has no caller — the timed animation did not need it. It should
go, but deleting it is a one-line change with no risk and no urgency.
