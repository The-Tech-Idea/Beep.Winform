# Beep ToolTips Framework — Comprehensive Enhancement Plan

**Status:** Plan (not yet implemented) — **corrected after code audit on 2026-06-05**
**Last audit:** 2026-06-05 (corrections applied)
**Scope:** `TheTechIdea.Beep.Winform.Controls/ToolTips/` + `BaseControl.Tooltip.cs` + `BaseControl/BaseControl.cs` (legacy `_toolTip` removal)
**Total estimated effort:** ~137 hours across 4 phases (revised from 130h after adding B1–B5 + D1)
**Target outcome:** Production-grade, accessible, theme-aware, async-friendly tooltip framework that supersedes the legacy `System.Windows.Forms.ToolTip` and unifies the rich `ToolTipManager` system.

> **Important corrections to previous draft:**
> 1. The animation `Timer _animationTimer` **already exists** in `CustomToolTip.Core.cs:59-61` and has an `OnAnimationTick` handler at `CustomToolTip.Animation.cs:202-208`. The bug is that the `AnimateInAsync`/`AnimateOutAsync` loops use `Task.Delay(16)` instead of the existing timer. The fix must migrate to the existing timer — **do not create a new one**.
> 2. `OnPrimaryClick` / `OnSecondaryClick` only exist on `PopoverConfig` (`PopoverConfig.cs:48,53`), **not** on `ToolTipConfig`. The tour uses `ToolTipConfig`, so the callbacks literally cannot be wired today. The fix must first add the fields to `ToolTipConfig` (Phase 0 C7), then wire them in `BeepTourManager.ShowCurrentStepAsync`.
> 3. WCAG contrast enforcement **already exists** in `CustomToolTip.Methods.cs` (`EnforceContrastIfNeeded`) and `ToolTipAccessibilityHelpers.EnsureContrastRatio` (`Helpers/ToolTipAccessibilityHelpers.cs:86`). This is no longer a gap.
> 4. `ToolTipStyleConfig` is dead (only definition + plan references), but `ToolTipStyleAdapter` is **live** — used by all 3 painters and `ToolTipLayoutHelpers`. Only the model file is dead, not the adapter.
> 5. `ContentItems` rendering **already exists** in `BeepStyledToolTipPainter.PaintContentItems()` (Header/Body/Divider/Footer with bold/italic/code/links). It is only missing in `GlassToolTipPainter` and `PreviewToolTipPainter` — that is the real gap (now tracked as bug B1 in Phase 0).
> 6. The plan previously said "5 new bugs found" — the actual audit found **5 new bugs** (B1–B5) plus the original 8 (C1–C8), so Phase 0 has 13 items now.

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Current State Audit](#2-current-state-audit)
3. [Critical Bugs — Phase 0 (Fix First)](#3-critical-bugs--phase-0-fix-first)
4. [Performance & Architecture — Phase 1](#4-performance--architecture--phase-1)
5. [API & Developer Experience — Phase 2](#5-api--developer-experience--phase-2)
6. [New Features — Phase 3](#6-new-features--phase-3)
7. [Testing Strategy — Phase 1 (parallel)](#7-testing-strategy--phase-1-parallel)
8. [Documentation — Phase 1 (parallel)](#8-documentation--phase-1-parallel)
9. [Migration Path](#9-migration-path)
10. [Phasing & Priorities](#10-phasing--priorities)
11. [Appendix: File-by-File Inventory](#11-appendix-file-by-file-inventory)

---

## 1. Executive Summary

The tooltip framework is **architecturally solid** (Manager / Config / Painter / Instance / Host pattern) but suffers from:

- **8 verified critical bugs** in current code (transparent corners show black, animation opacity not reset, tour keys mismatched, etc.) — `C1`–`C8`
- **5 newly discovered bugs** in `GlassToolTipPainter`, `VirtualToolTipHost`, painter pooling, `_lastPaintBounds` thread-safety, and shadow caching — `B1`–`B5`
- **Two positioning systems** (`ToolTipPositioningHelpers` + `ToolTipPositionResolver`) that duplicate logic
- **One dead model** (`ToolTipStyleConfig.cs`) — confirmed via `rg`, only the definition + plan docs reference it
- **`Task.Delay` animation loops** that allocate per-frame and run on the threadpool; an unused `Timer` already exists
- **Missing theme-change subscription** — `BeepThemesManager.ThemeChanged` is documented as a comment placeholder only (`ToolTipManager.cs:44-49`)
- **`VirtualToolTipHost.ShowAsync` bypasses positioning** — sets `_tip.Location` directly with no collision check
- **API surface bloat** — 47+ properties on `ToolTipConfig` makes discovery hard
- **Rich content support shallow in 2 of 3 painters** — only `BeepStyledToolTipPainter` handles `ContentItems`
- **No async content loading** — can't fetch data before display
- **Popover dismiss logic is broken** — `OnDeactivate` fires on internal button clicks
- **No headless test path** — the `IToolTipHost` boundary exists but `ToolTipManager` doesn't use it

The plan below is split into **4 phases** (0–3) prioritized by impact vs. risk. Phase 0 has 13 bug fixes (8 original + 5 new). Phases 1–3 can run in parallel after Phase 0 ships.

---

## 2. Current State Audit

### 2.1 What Exists (Strengths)

| Area | Strength | Evidence |
|------|----------|----------|
| **Architecture** | Manager / Config / Painter / Instance / Host — well-separated concerns | `ToolTipManager.cs`, `ToolTipInstance.cs`, `Painters/`, `Helpers/IToolTipHost.cs` |
| **Animations** | Fade, Scale, Slide, Bounce implemented | `CustomToolTip.Animation.cs:18-170` |
| **Placements** | 13 enum values covering all major compass positions + Auto | `ToolTipEnums.cs` |
| **Painter pattern** | Extensible — Glass, Preview, Tour are drop-in | `Painters/`, `ToolTipPainterFactory.cs` |
| **Theming** | `ToolTipThemeHelpers.ApplyThemeColors` resolves theme colors per `ToolTipType` | `Helpers/ToolTipThemeHelpers.cs` |
| **Accessibility** | WCAG contrast + high-contrast + screen reader + reduced-motion | `Helpers/ToolTipAccessibilityHelpers.cs:86-187`, `CustomToolTip.Accessibility.cs` |
| **Positioning** | `ToolTipPositionResolver` is multi-monitor and collision-aware | `Helpers/ToolTipPositionResolver.cs` |
| **Tours** | `BeepTourManager` + `BeepTourBuilder` + `BeepTourStep` for guided flows | `BeepTourManager.cs`, `BeepTourBuilder.cs` |
| **Popovers** | `BeepPopover` extends `CustomToolTip` with action buttons | `BeepPopover.cs` |
| **Pinning** | `BeepPinnedTooltip` for sticky tooltips | `BeepPinnedTooltip.cs` |
| **BaseControl integration** | All 17 Tooltip* properties wired through `UpdateTooltip()` | `BaseControl.Tooltip.cs` |
| **Rich content (1 of 3 painters)** | `BeepStyledToolTipPainter.PaintContentItems` renders Header/Body/Divider/Footer with bold/italic/code/links | `Painters/BeepStyledToolTipPainter.cs` |
| **Animation timer field** | `_animationTimer` already created at 60 FPS | `CustomToolTip.Core.cs:59-61` |
| **Live styling adapter** | `ToolTipStyleAdapter.GetBeepControlStyle` used by 3 painters + `ToolTipLayoutHelpers` | `Helpers/ToolTipStyleAdapter.cs` |
| **WCAG contrast** | `EnsureContrastRatio` + `EnforceContrastIfNeeded` | `Helpers/ToolTipAccessibilityHelpers.cs:86,128`, `CustomToolTip.Methods.cs` |

### 2.2 What's Broken or Missing (Gaps)

| # | Gap | Severity | Bug ID |
|---|-----|----------|--------|
| 1 | **TransparencyKey never set** — `OnPaintBackground` reads it but no one assigns it (Core.cs:43-68) | Critical | **C1** |
| 2 | **Opacity = 0 after AnimateOut** (`Animation.cs:169`); first re-show displays invisible | Critical | **C2** |
| 3 | **Tour key mismatch** — `ShowCurrentStepAsync` (`BeepTourManager.cs:161-174`) doesn't set `cfg.Key`, but `HideCurrentTooltip` (line 202) hides by `TourKey()` | Critical | **C3** |
| 4 | **4 placement cases fall through to default** (`Positioning.cs:46-81`) — LeftStart/LeftEnd/RightStart/RightEnd | Critical | **C4** |
| 5 | **BeepPopover.OnDeactivate dismisses on internal button clicks** (`BeepPopover.cs:92-97`) | Critical | **C5** |
| 6 | **Layout += PositionButtons re-subscribes on every config apply** (`BeepPopover.cs:138`) | Critical | **C6** |
| 7 | **Tour nav buttons can't navigate** — `OnPrimaryClick`/`OnSecondaryClick` only exist on `PopoverConfig`, not `ToolTipConfig`; tour uses `ToolTipConfig` | Critical | **C7** |
| 8 | **ThemeChanged not subscribed** — `ToolTipManager.cs:44-49` is a comment placeholder | Critical | **C8** |
| 9 | **`Task.Delay` animation loop** (`Animation.cs:99, 164`) — should migrate to the existing `_animationTimer` | High | **C9** *(renamed from C2 sub-task)* |
| 10 | **`GlassToolTipPainter` doesn't support `ContentItems`** — only Title+Text (`GlassToolTipPainter.cs:146-169`) | High | **B1** *(new)* |
| 11 | **`VirtualToolTipHost.ShowAsync` bypasses positioning** — sets `_tip.Location` directly, no collision check (`VirtualToolTipHost.cs:56`) | High | **B2** *(new)* |
| 12 | **No painter pooling in `ToolTipPainterFactory`** — new painter per show | Medium | **B3** *(new)* |
| 13 | **`_lastPaintBounds` is a non-volatile field** — mutated in UI-thread `Paint` (`BeepStyledToolTipPainter.cs:33`) but read in `GetArrowPath` (could be on any thread, line 272) | Medium | **B4** *(new)* |
| 14 | **Shadow cache never invalidated** — painters cache `GraphicsPath` keyed by size, but DPI/theme changes never clear cache | Low | **B5** *(new)* |
| 15 | **Two positioning systems** — `ToolTipPositioningHelpers` + `ToolTipPositionResolver` duplicate logic | High | P1.3 |
| 16 | **`IToolTipHost` unused by `ToolTipManager`** — hardcodes Form-based hosting | High | P1.2 |
| 17 | **`ToolTipStyleConfig` model is dead** — `rg` shows only the definition + plan docs reference it (`Models/ToolTipStyleConfig.cs`) | Low | P1.5 |
| 18 | **No markdown / HTML rendering** in painters (only inline markup `**bold**` etc. in `ToolTipMarkupParser`) | High (feature gap) | P3.1 |
| 19 | **No async content loading** (`Func<Task<ToolTipConfig>>`) | Medium (feature gap) | P3.2 |
| 20 | **No action buttons on plain tooltips** — only popovers have them today | Medium (feature gap) | P3.3 |
| 21 | **47+ properties on `ToolTipConfig`** — hard to discover, no presets | Medium (DX) | P2.3 |
| 22 | **`ShowDelay`/`HideDelay` partially wired** (HideDelay is write-only) | Medium | P1.4 |
| 23 | **No "smart dismissal"** — tooltip hides immediately when cursor exits target, even if cursor re-enters | Medium | P3.4 |
| 24 | **No RTL support** | Low | P3.8 |
| 25 | **No tooltip stacking** — second tooltip replaces first | Low | P3.5 |
| 26 | **No tests at all** — zero test coverage in `ToolTips/` | High | P1.12 |
| 27 | **No XML doc on most public members** | Medium | P2.9 |
| 28 | **No sample code** in `/samples` for tooltip system | Low | P2.14 |
| 29 | **No migration guide** from `System.Windows.Forms.ToolTip` | Medium | P2.13 |
| 30 | **`Readme.md` examples use `Message = "..."`** — but the actual field on `ToolTipConfig` is `Text` (`ToolTipConfig.cs:35`) | Low (doc bug) | **D1** *(new)* |

---

## 3. Critical Bugs — Phase 0 (Fix First)

**Effort:** ~12h | **Risk:** Low–Medium | **Impact:** High

These are **13 verified bugs** that affect users today. The original 8 (`C1`–`C8`) plus 5 newly discovered (`B1`–`B5`). Fix in 5 batches for easy review/revert.

### 3.1 Batch 1 — 1-line fixes (1.5h)

#### C1. `TransparencyKey` never set
- **File:** `CustomToolTip.Core.cs:43-68` (constructor) + `CustomToolTip.Drawing.cs:41-48` (OnPaintBackground)
- **Bug:** `OnPaintBackground` (Drawing.cs:44) reads `TransparencyKey`, but no one ever assigns it. The constructor never sets it.
- **Fix:** Set `TransparencyKey = Color.Magenta;` and `BackColor = Color.Magenta;` in the constructor (`CustomToolTip.Core.cs:43-68`). **Keep** the existing `OnPaintBackground` (Drawing.cs:41-48) — it already does the correct thing (fills the form with the transparency key so the OS compositor makes those pixels transparent).
- **DO NOT** remove `OnPaintBackground` as the original plan suggested — the current code is correct, it just needs the key to be set first.
- **Impact:** Rounded corners render correctly; transparency is honored.

#### C2. Opacity left at 0 after AnimateOut
- **File:** `CustomToolTip.Animation.cs:113-170` (line 169: `Opacity = 0;`)
- **Fix:** In `AnimateInAsync` (line 22), set `Opacity = 1.0` before the animation loop starts (or skip the fade-in start at line 40/47/55/59 by setting `Opacity = 1` first).
- **Impact:** Tooltip is visible on every show, not just the first.

#### C3. Tour key mismatch
- **File:** `BeepTourManager.cs:153-186` (see lines 161-174 — `cfg.Key` is NOT set)
- **Fix:** Set `cfg.Key = TourKey();` (where `TourKey()` returns `$"__tour_{_currentIndex}"`) before `await ToolTipManager.Instance.ShowTooltipAsync(cfg)` on line 183.
- **Impact:** `NextAsync`/`PreviousAsync`/`Skip` now visually change the tooltip (they currently hide a tooltip that was never registered with that key).

#### C4. LeftStart/LeftEnd/RightStart/RightEnd fall through
- **File:** `CustomToolTip.Positioning.cs:46-81`
- **Fix:** Add 4 explicit cases before the `_ =>` default:
  ```csharp
  ToolTipPlacement.LeftStart => new Point(
      targetPosition.X - Width - arrowSize - offset,
      targetPosition.Y),
  ToolTipPlacement.LeftEnd => new Point(
      targetPosition.X - Width - arrowSize - offset,
      targetPosition.Y - Height),
  ToolTipPlacement.RightStart => new Point(
      targetPosition.X + arrowSize + offset,
      targetPosition.Y),
  ToolTipPlacement.RightEnd => new Point(
      targetPosition.X + arrowSize + offset,
      targetPosition.Y - Height),
  ```
- **Impact:** All 13 placement enum values now position correctly.

### 3.2 Batch 2 — Popover fix (3h)

#### C5. `BeepPopover.OnDeactivate` dismisses on internal button clicks
- **File:** `BeepPopover.cs:92-97`
- **Fix:** Replace `OnDeactivate` with an `Application.AddMessageFilter` based on `IMessageFilter` that watches for `WM_LBUTTONDOWN` outside the popover's bounds. Or replace `OnDeactivate` with `OnLostFocus` (only dismiss if focus moved to a control **outside** `this.Controls`).
- **Impact:** Primary/secondary buttons inside popovers actually fire their actions before the popover dismisses.

#### C6. Layout += PositionButtons duplicate subscription
- **File:** `BeepPopover.cs:138` (inside `MountActionButtons()`)
- **Fix:** Unsubscribe first: `Layout -= PositionButtons; Layout += PositionButtons;`
- **Impact:** Buttons don't multiply-position after multiple `ApplyPopoverConfig` calls.

### 3.3 Batch 3 — Tour nav + theme wiring (3.5h)

#### C7. Tour nav buttons can't navigate (callback fields missing on `ToolTipConfig`)
- **Files:** `ToolTipConfig.cs` (add fields) + `BeepTourManager.cs:161-174` (wire them) + `CustomToolTip.Methods.cs` (plumb to painter) + `TourToolTipPainter.cs` (invoke on click)
- **Bug:** `OnPrimaryClick` / `OnSecondaryClick` only exist on `PopoverConfig` (`PopoverConfig.cs:48,53`), not `ToolTipConfig`. The tour uses `ToolTipConfig` (BeepTourManager.cs:161-174), so the callbacks literally don't exist for the tour case.
- **Fix:**
  1. Add to `ToolTipConfig`:
     ```csharp
     public Action OnPrimaryClick { get; set; }
     public Action OnSecondaryClick { get; set; }
     ```
  2. In `BeepTourManager.ShowCurrentStepAsync` (line 161-174), set:
     ```csharp
     cfg.OnPrimaryClick   = () => _ = NextAsync();
     cfg.OnSecondaryClick = Skip;
     ```
  3. In `CustomToolTip.Methods.cs`, ensure the painter receives these delegates and the `TourToolTipPainter` invokes them on button click.
- **Impact:** Tour "Next"/"Skip" buttons actually advance the tour.

#### C8. `BeepThemesManager.ThemeChanged` not subscribed
- **File:** `ToolTipManager.cs:44-49` (comment placeholder) and constructor
- **Fix:** Subscribe `BeepThemesManager.ThemeChanged += OnThemeChanged;` in the constructor. `OnThemeChanged` walks all `_instances`, calls `ApplyTheme(theme)` on each, then `Invalidate()`. Unsubscribe in `Dispose`. Use `BeepThemesManager.DefaultTheme` if the event arg doesn't carry a theme.
- **Impact:** Theme changes propagate to all active tooltips without restart.

### 3.4 Batch 4 — Migrate animation to existing Timer (1h)

#### C9. Animation uses `Task.Delay` instead of the existing `_animationTimer`
- **Files:** `CustomToolTip.Animation.cs:99, 164` (the two `await Task.Delay(16)` calls)
- **Important:** The timer **already exists** at `CustomToolTip.Core.cs:59-61`:
  ```csharp
  _animationTimer = new Timer();
  _animationTimer.Interval = 16; // ~60 FPS
  _animationTimer.Tick += OnAnimationTick;
  ```
  And `OnAnimationTick` (Animation.cs:202-208) just calls `Invalidate()`. The bug is that `AnimateInAsync`/`AnimateOutAsync` use `Task.Delay(16)` loops and never start the timer.
- **Fix:** Replace the two `await Task.Delay(16)` loops with a `TaskCompletionSource<bool>` that the timer completes when `_animationProgress >= 1.0`. In `OnAnimationTick`, advance `_animationProgress`, apply easing, call `Invalidate()`, and set the TCS when done. Migrate the per-frame logic from the loops into `OnAnimationTick`. Use `await _tcs.Task` instead of the loop. The timer is already disposed in `Dispose` (Drawing.cs:58-60).
- **Impact:** Animation no longer allocates a `Task` per frame; runs on the UI thread; cancellable.

### 3.5 Batch 5 — Newly discovered bugs (3h)

#### B1. `GlassToolTipPainter` doesn't support `ContentItems`
- **File:** `Painters/GlassToolTipPainter.cs:146-169`
- **Bug:** Only renders `config.Title` and `config.Text`. `BeepStyledToolTipPainter` already has `PaintContentItems` that handles Header/Body/Divider/Footer with bold/italic/code/links (`BeepStyledToolTipPainter.cs`).
- **Fix:** Refactor `PaintContentItems` from `BeepStyledToolTipPainter` into a static helper in `Helpers/` (e.g. `ToolTipRichContentPainter`). Both `BeepStyledToolTipPainter` and `GlassToolTipPainter` (and ideally `PreviewToolTipPainter`) call the helper.
- **Impact:** Consistent rich content across all 3 painters; users get markup support in Glass variant.

#### B2. `VirtualToolTipHost.ShowAsync` bypasses positioning
- **File:** `Helpers/VirtualToolTipHost.cs:52-60`
- **Bug:** Line 56: `_tip.Location = screenLocation;` — sets the location directly with no collision check, no `CalculatePlacement`, no `ConstrainToScreen`.
- **Fix:** Use the same positioning pipeline as `CustomToolTip.Positioning.cs`:
  ```csharp
  _tip.ApplyConfig(config);
  var bounds = new Rectangle(screenLocation, new Size(1, 1));
  var placement = ToolTipPositioningHelpers.CalculateOptimalPlacement(
      bounds, _tip.Size, config.Placement, config.Offset);
  _tip.Location = ToolTipPositioningHelpers.AdjustForScreenEdges(
      new Rectangle(Point.Empty, _tip.Size),
      _tip.CalculatePlacement(placement));
  _tip.Show();
  ```
- **Impact:** Virtual host positions tooltips correctly (used by tests + custom hosts).

#### B3. No painter pooling in `ToolTipPainterFactory`
- **File:** `Painters/ToolTipPainterFactory.cs`
- **Bug:** A new painter instance is allocated per show. Allocations + GC pressure on rapid tooltip cycling.
- **Fix:** Add a small pool (one instance per `LayoutVariant`) and return the pooled painter. Call `Reset()` between uses.
- **Impact:** Reduced allocations on repeated tooltip shows.

#### B4. `_lastPaintBounds` is a non-volatile, non-locked field
- **File:** `Painters/BeepStyledToolTipPainter.cs:33, 272, 285`
- **Bug:** Mutated in `Paint` (UI thread, line 33) and read in `GetArrowPath` (line 272). If `GetArrowPath` is called from a non-UI thread (e.g. layout calculation, async measure), torn reads possible.
- **Fix:** Either:
  - Mark `_lastPaintBounds` as `volatile`, or
  - Add a lock object (`private readonly object _boundsLock = new();`) and lock both writers and readers, or
  - Make `_lastPaintBounds` thread-local via `[ThreadStatic]`.
- **Impact:** No torn reads; safe if `MeasureSize` is ever called off-thread.

#### B5. Shadow cache never invalidated
- **File:** `Painters/*Painter.cs` (any painter that caches a `GraphicsPath` for shadow)
- **Bug:** If painters cache a shadow `GraphicsPath` keyed by `(width, height, radius)`, the cache survives DPI/theme changes and tooltip repositioning — old shadows render at the wrong location/size.
- **Fix:** Add a `Reset()` or `InvalidateCache()` method to `IToolTipPainter` (and `ToolTipPainterBase`). Call it from `CustomToolTip.Painter` setter (Core.cs:138-146) and from `OnDpiChanged` (Phase 1 P1.10).
- **Impact:** No stale shadow paths after DPI/theme change.

---

## 4. Performance & Architecture — Phase 1

**Effort:** ~30h | **Risk:** Medium | **Impact:** High

### 4.1 Replace `Task.Delay` animation with the existing `_animationTimer`

> **See C9 in Phase 0** — the timer already exists. Phase 1 wraps C9 with broader work.

**File:** `CustomToolTip.Animation.cs`, `Helpers/ToolTipAnimator.cs`
**Effort:** 6h
**Why:** Current loop allocates a `Task` per frame, runs on the threadpool, and desyncs when the UI thread is busy. The existing `_animationTimer` (Core.cs:59-61) ticks on the UI thread, doesn't allocate per frame, and is naturally cancellable.

**Tasks:**
- (C9) Migrate `AnimateInAsync` / `AnimateOutAsync` to use the existing `_animationTimer` + `TaskCompletionSource<bool>`
- Introduce `AnimationFrame` struct: `{ Progress, EasedValue, ElapsedMs }`
- `OnTimerTick` advances `_animationProgress`, calls `ApplyEasing`, `Invalidate`
- `Start()` and `Stop()` methods; `Start` is idempotent
- Keep public API (`AnimateInAsync`, `AnimateOutAsync`) returning `Task` for back-compat
- Cancel animations on `Dispose`

**Acceptance:** CPU usage during animation drops by 60%+; `Task.Delay` calls removed from profiler.

### 4.2 Adopt `IToolTipHost` boundary in `ToolTipManager`

**Files:** `ToolTipManager.cs`, `Helpers/VirtualToolTipHost.cs`, `IToolTipHost.cs`
**Effort:** 5h
**Why:** Decouples tooltip management from WinForms `Form`. Enables headless tests, WPF/Avalonia interop, and snapshot tests.

**Tasks:**
- `ToolTipManager` accepts `IToolTipHost` (default: `FormToolTipHost`)
- `ShowTooltipAsync` calls `host.ShowTooltip(config, position)`
- `HideTooltipAsync` calls `host.HideTooltip(key)`
- `VirtualToolTipHost` records all show/hide calls in-memory for tests
- `FormToolTipHost` is the existing Form-based implementation
- **Pre-fix B2 first** — VirtualToolTipHost currently bypasses positioning (see Phase 0 B2)

### 4.3 Consolidate positioning systems

**Files:** `Helpers/ToolTipPositioningHelpers.cs`, `Helpers/ToolTipPositionResolver.cs`, `CustomToolTip.Positioning.cs`
**Effort:** 5h
**Why:** Two systems, one should win. The resolver is more capable (multi-monitor, priority cascade) but positioning helpers is what `CustomToolTip` actually calls.

**Tasks:**
- Pick one — recommend `ToolTipPositionResolver` (Sprint 8 work, more capable)
- Replace all `ToolTipPositioningHelpers.FindBestPlacement` calls with `ToolTipPositionResolver`
- Delete `ToolTipPositioningHelpers` or move its public methods into the resolver
- Update unit tests to cover the consolidated path

### 4.4 Wire `ShowDelay`/`HideDelay` properly

**File:** `ToolTipConfig.cs`, `ToolTipManager.cs:OnControlMouseEnter/Leave`
**Effort:** 2h
**Why:** `HideDelay` is currently write-only (no read; never applied).

**Tasks:**
- Add `HideDelay` parameter to `OnControlMouseLeave` (use `config.HideDelay ?? 200`)
- Make `ShowDelay` and `HideDelay` both nullable int with default fallback to `DefaultShowDelay` / `DefaultHideDelay`
- Document the two as a "grace period" pair

### 4.5 Remove dead code — `ToolTipStyleConfig` model only

**Files:** `Models/ToolTipStyleConfig.cs`
**Effort:** 0.5h

> **Correction:** The original plan listed both `ToolTipStyleConfig` and `ToolTipStyleAdapter` as dead. Audit (2026-06-05) shows `ToolTipStyleAdapter` is **live** — used by all 3 painters and `ToolTipLayoutHelpers` (`rg ToolTipStyleAdapter` returns 6+ callers). Only the `ToolTipStyleConfig` model is dead.

**Tasks:**
- `rg ToolTipStyleConfig` shows only `Models/ToolTipStyleConfig.cs` and the two plan docs reference it — safe to delete
- Do **not** delete `ToolTipStyleAdapter.cs`
- Use `rg --files-without-match` to verify zero production callers

### 4.6 Fix `BeepPopover` re-subscription and click-outside detection

See C5/C6 above — counted in Phase 0.

### 4.7 Centralize painter registration

**File:** `Painters/ToolTipPainterFactory.cs`
**Effort:** 2h

**Tasks:**
- Add `Register(string styleName, Func<IToolTipPainter>)` API
- Add `RegisterByAttribute` discovery via reflection
- Expose `AvailablePainters` for UI dropdowns
- Migrate hard-coded `switch` statements in `ApplyConfig` to factory lookups
- Combine with **B3 (Phase 0)** — pool painter instances instead of allocating per show

### 4.8 Cache `Graphics` and shape paths

**Files:** All painters in `Painters/`
**Effort:** 3h

**Tasks:**
- Each painter caches its shape `GraphicsPath` keyed by `(width, height, radius)`
- `Invalidate()` clears cache on size change
- Reduces per-frame `Path.GetRoundedRectPath` allocations
- **B5 (Phase 0)** — wire `InvalidateCache()` into `OnDpiChanged` and `Painter` setter

### 4.9 Centralized color resolution

**File:** `Helpers/ToolTipThemeHelpers.cs`
**Effort:** 2h

**Tasks:**
- Add `ResolveColors(config, theme)` returning a `ToolTipColors` struct
- All painters consume the struct (no direct theme access)
- One place to add light/dark/high-contrast variations

### 4.10 Subscribe to DPI changes

**File:** `CustomToolTip.Core.cs`
**Effort:** 1h

**Tasks:**
- Listen for `SystemEvents.UserPreferenceChanged` (or `DpiChanged`) on the parent control
- Recalculate font size, paddings, and `MaxSize` on DPI change
- Repaint all active tooltips
- Also call `Painter.InvalidateCache()` (B5) on DPI change

---

## 5. API & Developer Experience — Phase 2

**Effort:** ~25h | **Risk:** Low | **Impact:** Medium

### 5.1 Fluent builder completion

**File:** `ToolTipExtensions.cs` → new `ToolTipBuilder.cs`
**Effort:** 4h

**Tasks:**
- `ToolTipBuilder` already exists — promote it to first-class API
- Method-chaining for all 47+ `ToolTipConfig` properties
- Example:
  ```csharp
  c.SetTooltip(b => b
      .WithText("Hello")
      .WithTitle("Greeting")
      .AsInfo()
      .Place(ToolTipPlacement.Top)
      .WithDelay(500)
      .WithDuration(3000)
      .WithArrow(true)
      .WithShadow(true)
      .Animated(ToolTipAnimation.Fade));
  ```
- IntelliSense-friendly; discoverable
- Existing positional `SetTooltip(text, ...)` overloads remain for back-compat

### 5.2 Named presets

**File:** `ToolTipExtensions.cs` → new `ToolTipPresets.cs`
**Effort:** 2h

**Tasks:**
- Static factories: `ToolTipPresets.Info()`, `.Error()`, `.Success()`, `.Warning()`, `.Notification()`, `.Preview()`, `.Help()`
- Encapsulate "good defaults" so users don't have to set 10 properties for a common case

### 5.3 Property categories

**File:** `ToolTipConfig.cs`
**Effort:** 2h

**Tasks:**
- Group properties into nested config objects:
  ```csharp
  public class ToolTipConfig {
      public TextConfig Text { get; set; } = new();
      public StyleConfig Style { get; set; } = new();
      public AnimationConfig Animation { get; set; } = new();
      public PlacementConfig Placement { get; set; } = new();
      public AccessibilityConfig Accessibility { get; set; } = new();
  }
  ```
- Mark old flat properties `[Obsolete]` with redirects to nested
- Reduces `ToolTipConfig` from 47 properties to 5 grouped config objects

### 5.4 Static `Tooltip.For(control)` factory

**File:** new `TooltipFactory.cs`
**Effort:** 2h

**Tasks:**
- `Tooltip.For(control).Show("text")` — single static entry point
- Returns disposable handle for hide/cancel
- Reduces dependency on `ToolTipManager.Instance` everywhere

### 5.5 Better extension discoverability

**File:** `ToolTipExtensions.cs`
**Effort:** 2h

**Tasks:**
- All `Control` extension methods in one namespace: `Beep.Winform.Controls.ToolTips.Extensions`
- Single `using` line gets you everything
- Add `[ToolboxItem]` friendly design-time attributes

### 5.6 Async/await throughout

**Files:** `ToolTipManager.cs`, `BeepTourManager.cs`
**Effort:** 3h

**Tasks:**
- `SetTooltip(...)` → `SetTooltipAsync(...)` (with sync wrapper that calls `.GetAwaiter().GetResult()` for back-compat)
- `RemoveTooltip(...)` → `RemoveTooltipAsync(...)`
- `ShowTooltipAsync(config)` already exists; ensure all call sites are async

### 5.7 Smart naming (no magic strings)

**File:** `ToolTipManager.cs:SetTooltip` (line 417)
**Effort:** 0.5h

**Tasks:**
- Replace `"control_{hash}_{ticks}"` with a deterministic `control.Name` + counter
- Allows external code to reference tooltips by key (e.g., for analytics)

### 5.8 Throw helpful exceptions

**Files:** All public API
**Effort:** 2h

**Tasks:**
- Custom exception types: `ToolTipDisposedException`, `ToolTipNotFoundException`, `ToolTipConfigException`
- Include config snapshot in exception for debugging

### 5.9 XML doc on all public API

**Files:** All public types
**Effort:** 6h

**Tasks:**
- Add `<summary>`, `<param>`, `<returns>`, `<exception>`, `<example>` on every public member
- Enable `GenerateDocumentationFile` in `.csproj` (already on for some)
- CI check: build with `-warnaserror:1591` (missing XML doc) fails build

### 5.10 Designer integration

**File:** `Painters/`, `ToolTipConfig.cs`
**Effort:** 3h

**Tasks:**
- Add `[TypeConverter]` for `ToolTipConfig` so Visual Studio property grid shows grouped sections
- Add `UITypeEditor` for placement/animation enums
- Drop a `ToolTip` smart-tag verb on `BaseControl` ("Edit Tooltip...")

---

## 6. New Features — Phase 3

**Effort:** ~50h | **Risk:** Medium | **Impact:** High

### 6.1 Rich content (markdown / HTML / images)

**File:** new `Helpers/ToolTipRichContentParser.cs`, painters
**Effort:** 8h

**Tasks:**
- Lightweight markdown subset: `**bold**`, `*italic*`, `` `code` ``, links, lists
- Inline images via markdown: `![alt](path)` and `![svg](svg://path)`
- Image rendering via existing `StyledImagePainter`
- Hyperlinks open in default browser on click

### 6.2 Async content loading

**File:** `ToolTipConfig.cs`, `ToolTipManager.cs`
**Effort:** 4h

**Tasks:**
- `public Func<Task<string>>? AsyncTextLoader`
- `public Func<Task<Image>>? AsyncImageLoader`
- `ShowTooltipAsync` awaits the loader, then displays
- Cancellable via `CancellationToken`

### 6.3 Action buttons (general)

**File:** `ToolTipConfig.cs`, `CustomToolTip.Methods.cs`, `Painters/`
**Effort:** 6h

**Tasks:**
- Promote popover's `OnPrimaryClick`/`OnSecondaryClick` to `ToolTipConfig`
- `ShowButtons` flag — not popover-only
- Painter renders primary/secondary buttons; clicking invokes delegates
- Escape closes tooltip if `Closable = true`

### 6.4 Smart dismissal

**File:** `ToolTipManager.cs:OnControlMouseLeave`
**Effort:** 3h

**Tasks:**
- Don't hide immediately when cursor exits target
- Check if cursor is over the tooltip itself (within `TooltipBounds + grace pixels`)
- If yes, defer hide until cursor leaves both regions or grace period expires
- Adds "tooltip pinning" UX without explicit pin

### 6.5 Multi-tooltip stacking

**File:** `ToolTipManager.cs`
**Effort:** 3h

**Tasks:**
- Allow multiple active tooltips simultaneously
- Stack vertically/horizontally if they overlap
- "Zebra" stacking for related items
- API: `ShowTooltipAsync(config, groupId)` for grouping

### 6.6 Hover-card / preview variant

**File:** `Painters/PreviewToolTipPainter.cs`
**Effort:** 3h

**Tasks:**
- Thumbnail + title + description layout
- Optional progress bar (e.g., "Loading 45%")
- Already exists; enhance with size variants and animations

### 6.7 Spotlight tour

**File:** `BeepTourManager.cs`, `Painters/TourToolTipPainter.cs`
**Effort:** 6h

**Tasks:**
- Dark overlay over entire app except current target
- Tooltip positions at target with arrow
- Cutout hole over target via `ControlPaint.DrawLocked`
- Auto-advance on target click (optional)
- Skip / Back / Next / Done buttons

### 6.8 RTL / bidirectional support

**File:** `CustomToolTip.cs`, all painters
**Effort:** 3h

**Tasks:**
- Detect `Control.RightToLeft` of parent
- Mirror `Left*` and `Right*` placements
- Mirror arrow direction
- Test with Arabic/Hebrew text

### 6.9 Theme auto-detection (light/dark)

**File:** `Helpers/ToolTipThemeHelpers.cs`
**Effort:** 2h

**Tasks:**
- When `theme == null`, sample parent background luminance
- Pick light or dark variant automatically
- Add `Auto` to `BeepControlStyle` for tooltip painters

### 6.10 Interactive content (textbox, button, link inside tooltip)

**File:** new `ToolTipInteractiveContent.cs`, painters
**Effort:** 6h

**Tasks:**
- Embed a `UserControl` inside tooltip via `Config.InteractiveContent`
- Auto-size painter to control
- Forward click events back via `Config.InteractionRequested` callback

### 6.11 Persistence / serialization

**File:** `ToolTipConfig.cs`
**Effort:** 2h

**Tasks:**
- `[Serializable]` on `ToolTipConfig`
- `Serialize(BinaryWriter)` / `Deserialize(BinaryReader)` for round-trip
- Store user-customized tooltips across sessions

### 6.12 A11y deep pass

**Files:** `CustomToolTip.Accessibility.cs`, `Helpers/ToolTipAccessibilityHelpers.cs`
**Effort:** 4h

**Tasks:**
- `IAccessible` + `IAccessible2` implementation
- `role="tooltip"`, `aria-describedby` on parent
- Live region announcement (`aria-live="polite"`)
- High-contrast mode detection (Windows accessibility setting)
- Reduced motion: disable animations

---

## 7. Testing Strategy — Phase 1 (parallel)

**Effort:** ~12h | **Goal:** Zero untested code

### 7.1 Test project

**Location:** `TheTechIdea.Beep.Winform.Controls.Tests/ToolTips/`

### 7.2 Unit tests

- **Positioning** — `ToolTipPositionResolver` test cases:
  - 13 placement values × 4 screen quadrants × 3 multi-monitor layouts
  - Verify position is within `WorkingArea`
  - Verify no overlap with target
- **Animation** — `CustomToolTip.Animation` test cases:
  - Fade ease curve
  - Scale ease curve
  - Slide offset correctness
  - Cancellation mid-animation
- **Painter** — each painter:
  - Generates shape with given bounds
  - Uses theme colors
  - Renders arrow in correct direction
  - Disposes brushes
- **Config** — `ToolTipConfig`:
  - Defaults
  - Clone / Copy
  - Serialization round-trip
- **Manager** — `ToolTipManager` with `VirtualToolTipHost`:
  - Set/Show/Hide
  - Key collisions
  - Event handler leak (the fix from earlier session)
  - Theme change propagation

### 7.3 Integration tests

- Drag a `BeepButton` onto a form, set `TooltipText`, show form, simulate hover, assert tooltip shown
- Multi-monitor: move target to secondary display, assert position
- Theme change: trigger `BeepThemesManager.ThemeChanged`, assert repaint
- Tour: `StartAsync` → simulate click Next → assert advance

### 7.4 Visual regression tests

- Snapshot painter output to PNG
- Compare to baseline on each test run
- Tool: `Verify` or custom `ImageComparator`
- Baselines stored in `tests/baselines/ToolTips/`

### 7.5 Accessibility tests

- Use `AccessibilityChecker` library
- Assert WCAG AA contrast on all painter outputs
- Assert `IAccessible` role on active tooltips
- Assert keyboard navigation: Tab → tooltip, Esc → hide

### 7.6 Performance benchmarks

- `BenchmarkDotNet` for animation FPS
- Memory profiler for handler leaks
- Target: <1% CPU during 5s animation, 0 leaked handlers

---

## 8. Documentation — Phase 1 (parallel)

**Effort:** ~8h

### 8.1 XML doc

See 5.9 above.

### 8.2 Sample code

**Location:** `samples/Beep.Sample.ToolTips/`

- **Sample 1:** Basic tooltip on a button
- **Sample 2:** Error/success/warning semantic tooltips
- **Sample 3:** Rich content with markdown
- **Sample 4:** Async-loaded content
- **Sample 5:** Popover with action buttons
- **Sample 6:** Pinned tooltip
- **Sample 7:** Tour with spotlight
- **Sample 8:** Theme auto-detection
- **Sample 9:** Custom painter (drag-drop painter registration)
- **Sample 10:** Migrating from `System.Windows.Forms.ToolTip`

### 8.3 Migration guide

**File:** `docs/ToolTips/MIGRATION.md`

- From `System.Windows.Forms.ToolTip` → `ToolTipManager`
- From legacy `BaseControl._toolTip` → `TooltipText` property
- From `RibbonSuperTooltip` → `BeepPinnedTooltip` (if applicable)
- Designer file migration (`UseRichToolTip` → no-op)

### 8.4 Best practices

**File:** `docs/ToolTips/BEST_PRACTICES.md`

- When to use popover vs tooltip
- Accessibility checklist
- Performance tips
- Theme integration
- Multi-monitor considerations
- Tour design

### 8.5 Architecture diagram

**File:** `docs/ToolTips/ARCHITECTURE.md` (replaces `ARCHITECTURE_ANALYSIS.md`)

- Manager / Config / Painter / Instance / Host diagram
- Sequence diagrams for: Show, Hide, Animate, ThemeChange
- Extension points and how to add custom painter

### 8.6 API reference

Auto-generated from XML doc via DocFX or similar.

---

## 9. Migration Path

**Effort:** ~6h | **Risk:** Low | **Impact:** Critical for adoption

### 9.1 From `System.Windows.Forms.ToolTip`

Already done in the previous session — `BaseControl._toolTip` removed. Provide adapter:

```csharp
// Optional: drop-in shim for legacy code
public class BeepToolTipAdapter : System.Windows.Forms.ToolTip
{
    public new void SetToolTip(Control control, string text) {
        ToolTipManager.Instance.SetTooltip(control, text);
    }
    // override Show/Hide/etc to forward
}
```

Place in `ToolTips/Compatibility/`.

### 9.2 From `BaseControl._toolTip` references

Already removed. Audit any remaining `using System.Windows.Forms;` lines for `ToolTip` references.

### 9.3 From `BaseControl.ToolTipText` (PascalCase)

- Keep as forwarder to `TooltipText` (already done)
- Mark `[Obsolete("Use TooltipText instead. ToolTipText is kept for IBeepUIComponent compatibility.")]`
- Tooltip message directs to lowercase property

### 9.4 From `RibbonSuperTooltip` / `BeepDockTooltip`

- These are independent systems for Ribbon/Dock — keep them
- Document in migration guide that they're separate from the unified framework
- Future: deprecate in favor of `BeepPinnedTooltip` (lower priority)

---

## 10. Phasing & Priorities

| Phase | Focus | Effort | Risk | Output |
|-------|-------|--------|------|--------|
| **0** | Critical bug fixes (13 items: C1–C9, B1–B5, D1) | 12h | Low | Stable framework, no crashes |
| **1** | Performance + architecture + tests + docs | 50h | Medium | Production-grade foundation |
| **2** | API + DX | 25h | Low | Developer-friendly |
| **3** | New features | 50h | Medium | Modern UX parity with web |
| **Total** | | **~137h** | | |

### Recommended sequencing

1. **Phase 0** (1.5 weeks) — unblock users
2. **Phase 1 + 2 in parallel** (3 weeks) — foundation + DX
3. **Phase 3** (2 weeks) — new features

### Per-PR breakdown

**Phase 0 PRs (5 PRs):**
- PR 1: C1, C2, C3, C4 (1.5h, 1-line fixes) + D1 (0.5h, doc fix to Readme.md)
- PR 2: C5, C6 (3h, popover fix)
- PR 3: C7, C8 (3.5h, tour + theme)
- PR 4: C9 (1h, migrate to existing animation timer)
- PR 5: B1, B2, B3, B4, B5 (3h, painter quality + positioning)

**Phase 1 PRs (8 PRs):**
- PR 6: Animation timer follow-up (easing registry, `AnimationFrame` struct)
- PR 7: IToolTipHost adoption
- PR 8: Positioning consolidation
- PR 9: ShowDelay/HideDelay wiring
- PR 10: Dead code removal (ToolTipStyleConfig only)
- PR 11: Painter registration factory + pooling (B3)
- PR 12: Color/path caching + InvalidateCache (B5)
- PR 13: DPI change subscription
- PR 14: Test project (all unit tests)
- PR 15: XML doc on all public API
- PR 16: Sample code

**Phase 2 PRs (6 PRs):**
- PR 17-19: Builder, presets, grouping
- PR 20-22: Async, naming, exceptions, designer

**Phase 3 PRs (12 PRs):**
- PR 23: Rich content (markdown via the new helper from B1)
- PR 24: Async loaders
- PR 25: Action buttons
- PR 26-34: One per feature

### Success metrics

| Metric | Before | Target |
|--------|--------|--------|
| Critical bugs | 13 (8 + 5) | 0 |
| Test coverage | 0% | 80%+ |
| `Task.Delay` calls in animation | 60+/sec | 0 |
| Event handler leaks | Yes | 0 |
| Public API with XML doc | 10% | 100% |
| Sample projects | 0 | 10+ |
| Time to "Hello World" tooltip | ~5min (find right overload) | <30s (Tooltip.For(c).Show("text")) |
| Theme propagation lag | Until restart | <100ms |
| Painters supporting ContentItems | 1 of 3 | 3 of 3 |

---

## 11. Appendix: File-by-File Inventory

### Core framework

| File | Lines | Phase | Action |
|------|-------|-------|--------|
| `CustomToolTip.cs` | 27 | 1 | Add `ApplyTheme()` public |
| `CustomToolTip.Core.cs` | 151 | 0 (C1) | Fix `TransparencyKey` (set in constructor; keep `OnPaintBackground`); refactor config |
| `CustomToolTip.Methods.cs` | 260 | 0 (C7) | Wire button callbacks; async API |
| `CustomToolTip.Drawing.cs` | 68 | 0 (B4) | Fix `_lastPaintBounds` thread-safety if touched there |
| `CustomToolTip.Positioning.cs` | 97 | 0 (C4) | Add 4 placement cases; use resolver |
| `CustomToolTip.Animation.cs` | 213 | 0 (C2, C9) | Fix opacity; migrate `Task.Delay` to existing `_animationTimer` |
| `CustomToolTip.Accessibility.cs` | 115 | 3 | Improve; add IAccessible2 |

### Variants

| File | Lines | Phase | Action |
|------|-------|-------|--------|
| `BeepPopover.cs` | 203 | 0 (C5, C6) | Fix OnDeactivate; unsubscribe Layout |
| `BeepPinnedTooltip.cs` | ~150 | 2 | XML doc, samples |
| `BeepTourBuilder.cs` | ~100 | 0, 1, 3 | Fix key; spotlight; XML doc |
| `BeepTourManager.cs` | 232 | 0 (C3, C7) | Fix key; wire callbacks; theme subscribe |
| `BeepTourStep.cs` | ~50 | 1 | XML doc |

### Manager + Config

| File | Lines | Phase | Action |
|------|-------|-------|--------|
| `ToolTipManager.cs` | ~953 | 0 (C8) | ThemeChanged subscribe; IToolTipHost; smart naming |
| `ToolTipConfig.cs` | ~470 | 0 (C7), 1, 2, 3 | Add `OnPrimaryClick`/`OnSecondaryClick`; group properties; async loaders |
| `ToolTipInstance.cs` | 361 | 1 | Cleanup |
| `ToolTipEnums.cs` | 327 | 1 | Add `RtlMode`, `DismissMode` |
| `ToolTipExtensions.cs` | 242 | 2 | Promote builder; add presets |
| `IToolTipHost.cs` | 56 | 1 | Implementations |
| `PopoverConfig.cs` | 62 | 0 (C7) | Source of `OnPrimaryClick`/`OnSecondaryClick` (existing) |

### Painters (5)

| File | Phase | Action |
|------|-------|--------|
| `IToolTipPainter.cs` | 0 (B3, B5) | Add `InvalidateCache()`; pool support |
| `ToolTipPainterBase.cs` | 0 (B3, B5), 1 | Cache paths; pool management |
| `BeepStyledToolTipPainter.cs` | 0 (B4) | Fix `_lastPaintBounds` thread-safety (lock or volatile) |
| `GlassToolTipPainter.cs` | 0 (B1) | Add `ContentItems` rendering (extract helper from `BeepStyledToolTipPainter`) |
| `PreviewToolTipPainter.cs` | 1, 3 | Performance + variants; optionally support `ContentItems` |
| `TourToolTipPainter.cs` | 0 (C7) | Wire nav buttons to `OnPrimaryClick`/`OnSecondaryClick` |
| `ToolTipPainterFactory.cs` | 0 (B3) | Public registration + pool |

### Helpers (15)

| File | Phase | Action |
|------|-------|--------|
| `ToolTipThemeHelpers.cs` | 1, 3 | Resolve all colors in one struct |
| `ToolTipPositionResolver.cs` | 1 | Make canonical |
| `ToolTipPositioningHelpers.cs` | 1 | Delete (consolidated) |
| `ToolTipStyleHelpers.cs` | 1 | Migrate to factory |
| `ToolTipLayoutHelpers.cs` | 1 | Cache measurements |
| `ToolTipAnimationHelpers.cs` | 1 | Easings registry |
| `ToolTipAccessibilityHelpers.cs` | 3 | High contrast, IAccessible2 *(WCAG already done — no Phase 0 work here)* |
| `ToolTipArrowPainter.cs` | 1 | Cache paths |
| `ToolTipHelpers.cs` | 1 | Tidy |
| `ToolTipMarkupParser.cs` | 0 (B1), 3 | Extract `PaintContentItems` from `BeepStyledToolTipPainter` into a shared helper; markdown |
| `ToolTipStyleAdapter.cs` | — | **KEEP — it is live, not dead** |
| `ToolTipAnimator.cs` | 1 | Use Timer (subsumed by C9 — the timer is already in `CustomToolTip.Core`) |
| `ShortcutBadgePainter.cs` | 1, 3 | XML doc |
| `VirtualToolTipHost.cs` | 0 (B2) | Use `CalculatePlacement` + `AdjustForScreenEdges` instead of raw `Location` |

### Models (1)

| File | Phase | Action |
|------|-------|--------|
| `ToolTipStyleConfig.cs` | 1 (P1.5) | Delete (only definition + plan docs reference it) |

### Tests (new)

| Path | Phase | Action |
|------|-------|--------|
| `Beep.Winform.Controls.Tests/ToolTips/Unit/*.cs` | 1 | Create |
| `Beep.Winform.Controls.Tests/ToolTips/Integration/*.cs` | 1 | Create |
| `Beep.Winform.Controls.Tests/ToolTips/Accessibility/*.cs` | 1 | Create |
| `Beep.Winform.Controls.Tests/ToolTips/Visual/*.cs` | 1 | Create |

### Samples (new)

| Path | Phase | Action |
|------|-------|--------|
| `samples/Beep.Sample.ToolTips/01_Basic/` | 2 | Create |
| `samples/Beep.Sample.ToolTips/02_Semantic/` | 2 | Create |
| `samples/Beep.Sample.ToolTips/03_RichContent/` | 3 | Create |
| `samples/Beep.Sample.ToolTips/04_AsyncContent/` | 3 | Create |
| `samples/Beep.Sample.ToolTips/05_Popover/` | 2 | Create |
| `samples/Beep.Sample.ToolTips/06_Pinned/` | 2 | Create |
| `samples/Beep.Sample.ToolTips/07_Tour/` | 3 | Create |
| `samples/Beep.Sample.ToolTips/08_ThemeAuto/` | 3 | Create |
| `samples/Beep.Sample.ToolTips/09_CustomPainter/` | 2 | Create |
| `samples/Beep.Sample.ToolTips/10_Migration/` | 2 | Create |

### Docs (new + updated)

| File | Phase | Action |
|------|-------|--------|
| `ToolTips/Readme.md` | 0 (D1) | Fix `Message = "..."` → `Text = "..."` in 4 examples |
| `docs/ToolTips/MIGRATION.md` | 2 | Create |
| `docs/ToolTips/BEST_PRACTICES.md` | 2 | Create |
| `docs/ToolTips/ARCHITECTURE.md` | 1 | Replace existing |
| `docs/ToolTips/API_REFERENCE.md` | 2 | Auto-generate |
| `ToolTips/TOOLTIP_ENHANCEMENT_PLAN.md` | 0 | Replace with this doc |
| `ToolTips/ARCHITECTURE_ANALYSIS.md` | 1 | Update to mention `IToolTipHost` adoption |
| `ToolTips/IMPLEMENTATION_SUMMARY.md` | 1 | Update to reflect Phase 0 changes |
| `ToolTips/TOOLTIP_ENHANCEMENT_SUMMARY.md` | 1 | Update to reflect Phase 0 changes |

---

## Appendix: Quick Reference — File-to-Phase Map (corrected)

```
Phase 0 (12h, 13 bugs in 5 PRs):
  PR 1: CustomToolTip.Core.cs          (C1: TransparencyKey)
        CustomToolTip.Animation.cs     (C2: Opacity 0 bug)
        BeepTourManager.cs             (C3: cfg.Key)
        CustomToolTip.Positioning.cs   (C4: 4 placement cases)
        ToolTips/Readme.md             (D1: Message → Text)
  PR 2: BeepPopover.cs                 (C5: OnDeactivate; C6: Layout unsubscribe)
  PR 3: ToolTipConfig.cs               (C7: add OnPrimaryClick/OnSecondaryClick)
        BeepTourManager.cs             (C7: wire callbacks)
        TourToolTipPainter.cs          (C7: invoke on click)
        ToolTipManager.cs              (C8: subscribe ThemeChanged)
  PR 4: CustomToolTip.Animation.cs     (C9: migrate to _animationTimer)
  PR 5: BeepStyledToolTipPainter.cs    (B4: _lastPaintBounds lock)
        Painters/GlassToolTipPainter.cs(B1: ContentItems)
        Helpers/ToolTipMarkupParser.cs (B1: extract shared helper)
        Helpers/VirtualToolTipHost.cs  (B2: use positioning)
        Painters/ToolTipPainterFactory.cs (B3: pool painters)
        Painters/IToolTipPainter.cs    (B5: InvalidateCache)

Phase 1 (50h):
  CustomToolTip.Animation.cs     (easing registry, AnimationFrame struct)
  ToolTipManager.cs              (IToolTipHost, smart dismiss)
  ToolTipConfig.cs               (delay wiring)
  Helpers/ToolTipPositionResolver.cs (canonical)
  Helpers/ToolTipPositioningHelpers.cs (delete)
  Helpers/ToolTipThemeHelpers.cs (centralize)
  Models/ToolTipStyleConfig.cs   (delete — only model is dead; adapter is live)
  Painters/ToolTipPainterFactory.cs (registration API)
  Painters/*.cs                  (caching, path invalidation)
  tests/                         (new project)

Phase 2 (25h):
  ToolTipConfig.cs               (group properties)
  ToolTipExtensions.cs           (builder, presets)
  Helpers/                       (XML doc)
  new TooltipFactory.cs          (static entry)
  samples/                       (10 projects)

Phase 3 (50h):
  Helpers/ToolTipMarkupParser.cs (markdown, building on B1 helper)
  ToolTipConfig.cs               (async, buttons)
  ToolTipManager.cs              (stacking, smart dismiss)
  Painters/                      (RTL, spotlight)
  CustomToolTip.Accessibility.cs (IAccessible2)
```

---

## Appendix: Open Questions

1. **Animation engine** — Should the timer live in `CustomToolTip` or in `Helpers/ToolTipAnimator`? The timer already lives in `CustomToolTip.Core.cs:59-61`. The helper `ToolTipAnimator.cs` is currently unused (Phase 0 C9 obsoletes it). **Recommendation: keep the timer in `CustomToolTip`; delete `ToolTipAnimator.cs`.**
2. **Tour key format** — Should the tour key be `__tour_{index}` or a GUID? (Current: index; recommend GUID for multi-tour support)
3. **Markdown subset** — How rich? Just `**bold**` + links, or full GFM? (Recommendation: start minimal, expand later)
4. **Sample project type** — WinForms (.NET 8+) only, or also .NET Framework? (Recommendation: .NET 8+ only; .NET Framework is in maintenance)
5. **Deprecation policy** — How long to keep `[Obsolete]` APIs? (Recommendation: 2 minor versions)
6. **Designer file migration** — How to handle the ~17 designer files setting `UseRichToolTip = true`? (Recommendation: keep the no-op property; document as obsolete-but-tolerated)
7. **Where to put `PaintContentItems` helper?** — `Helpers/ToolTipMarkupParser.cs` already has the `ToolTipMarkupParser` (analyzes text). The new helper renders layout. **Recommendation: new file `Helpers/ToolTipRichContentPainter.cs`** (or move the parser there too).

---

**Document version:** 1.1
**Author:** Tooltip framework audit (corrected)
**Next review:** After Phase 0 ships

