# Phase 1 — Smooth Transitions & Animation Engine

**Status:** `[ ]` | **Tasks:** 8 | **Complexity:** Medium | **Depends on:** None

---

## Objective

Replace the single-function `WizardAnimationEngine` (only `EaseInOutCubic`) with a rich animation engine supporting 12 easing functions, 4+ transition types, configurable durations, and reduced-motion accessibility. Fix the `AnimateFadeTransition` stub (currently a no-op).

## Background

- `WizardAnimationEngine.cs` — 22 lines, single `EaseInOutCubic(float t)` function
- `WizardHelpers.AnimateFadeTransition()` — no-op stub, just calls `onComplete()`
- `WizardHelpers.AnimateStepTransition()` — solid bitmap-slide approach, but duration hardcoded to 300ms and easing is fixed to EaseInOutCubic
- All 4 forms call `WizardHelpers.AnimateStepTransition` identically with `bool forward` direction only
- **Research:** Actipro WPF offers 10 animation types; MaterialDesignInXAML offers 7 transitions with forward/backward wipe directions

## Tasks

### WZ-01 — Expand WizardAnimationEngine (Low)
Add 12 easing functions as static methods:
- `Linear(t)`, `EaseInQuad(t)`, `EaseOutQuad(t)`, `EaseInOutQuad(t)`
- `EaseInCubic(t)`, `EaseOutCubic(t)`, `EaseInOutCubic(t)` (already exists)
- `EaseOutBack(t)` — overshoot at end
- `EaseOutBounce(t)` — bouncing at end
- `EaseOutElastic(t)` — spring oscillation
- `EaseOutQuint(t)` — strong deceleration
- `Spring(t, damping=0.6f)` — configurable spring

**File:** `Helpers/WizardAnimationEngine.cs`

### WZ-02 — Add TransitionType Enum (Low)
Add to `WizardEnums.cs`:
```csharp
public enum TransitionType { Slide, Fade, Zoom, Flip, None }
public enum TransitionEasing { EaseOutCubic, EaseInOutCubic, EaseOutBack, Spring, Bounce, Linear }
```
Add to `WizardConfig`:
- `TransitionType TransitionType { get; set; } = TransitionType.Slide;`
- `TransitionEasing TransitionEasing { get; set; } = TransitionEasing.EaseOutCubic;`
- `int TransitionDurationMs { get; set; } = 300;` (range 100-800)

**Files:** `Core/WizardEnums.cs`, `Core/WizardModels.cs`

### WZ-03 — Implement Fade Transition (Medium)
Replace the `AnimateFadeTransition` stub. Use the same bitmap-capture approach as slide but apply alpha compositing via `ColorMatrix`:
- Capture `fromBitmap` and `toBitmap`
- Each tick: draw `fromBitmap` with decreasing opacity, `toBitmap` with increasing opacity
- Use `ImageAttributes` with `ColorMatrix` for alpha blending
- Duration: respect `TransitionDurationMs`

**File:** `Helpers/WizardHelpers.cs`

### WZ-04 — Implement Zoom Transition (Medium)
Scale from/to bitmaps:
- Outgoing control zooms out (scale 1.0 → 0.8 + fade out)
- Incoming control zooms in (scale 1.2 → 1.0 + fade in)
- Use `Graphics.DrawImage(destRect, srcRect, Unit)` for scaled rendering
- Center the zoom origin at the content panel center

**File:** `Helpers/WizardHelpers.cs`

### WZ-05 — Implement Flip/Wipe Transitions (Medium)
- **Flip:** compress outgoing width to 0, then expand incoming from 0 to full (simulate 3D Y-axis rotation)
- **Wipe/None:** instant swap (duration=0)
- Use shrinking/expanding `Rectangle` calculations

**File:** `Helpers/WizardHelpers.cs`

### WZ-06 — Route Transition Config Through Forms (Low)
In each form's `UpdateUI()` animation block:
- Read `Config.TransitionType`, `Config.TransitionDurationMs`, `Config.TransitionEasing`  
- Pass to the transition dispatcher

**Files:** All 4 `Forms/*WizardForm.cs`

### WZ-07 — Reduced Motion Support (Low)
- Add `WizardManager.ReducedMotion` static property, default reads from `SystemParametersInfo(SPI_GETCLIENTAREAANIMATION)` or `SystemInformation.ClientRectangle` check
- When true: all transitions instant (duration=0, no animation timer)
- `WizardConfig.EnableAnimations` per-instance override (currently only global on WizardManager)

**Files:** `Core/WizardManager.cs`, `Core/WizardModels.cs`, `Helpers/WizardHelpers.cs`

### WZ-08 — Refactor into WizardTransitionEngine (Medium)
Create `Helpers/WizardTransitionEngine.cs`:
- `static void AnimateTransition(Control from, Control to, TransitionType type, TransitionEasing easing, int durationMs, bool forward, Action onComplete, List<Timer> registry)`
- Internal dispatch to slide/fade/zoom/flip implementations
- Move bitmap capture logic from `WizardHelpers` to this class
- Update all 4 forms to call through this engine

**Files:** Create `Helpers/WizardTransitionEngine.cs`, modify `Helpers/WizardHelpers.cs`, all 4 forms

## Acceptance Criteria
- [ ] All 4 transition types produce 60fps smooth motion
- [ ] Duration configurable per-wizard, easing selectable
- [ ] ReducedMotion=true produces instant (0ms) transitions
- [ ] Fade transition works (no longer a no-op)
- [ ] Zero GDI handle leaks after 50+ transitions
- [ ] `onComplete` always fires even on edge cases (zero-size, disposed control)
- [ ] Build with 0 errors
