# PHASE 04 — Auto-Hide, Float & Animations

**Goal:** Finish the auto-hide experience (hover-peek, slide-back, pin/unpin) on top of the
**already-animating** slide panel, and make floating windows themed, snappable, and
redockable. Add a small shared animator only where reuse pays off.

**Depends on:** 01, 02, 03 · **Blocks:** —

---

## 4.1 Existing-code disposition (this phase)

| File | Disposition | What changes |
|------|-------------|--------------|
| `Runtime/AutoHideSlidePanel.cs` | **Reuse** | Already slides (100 ms / 10-tick timer, per-edge grow). Add: stay-open while hovered, slide-back on mouse-leave with a small delay, and a "pinned" path that converts back to a docked group. |
| `Runtime/AutoHideStrip.cs` | **Refactor** | Paint via `AutoHideStripRenderer` (Phase 01). Hover over a strip tab triggers peek (via `AutoHideLayoutManager`). |
| `Runtime/FloatWindow.cs` | **Refactor** | Themed caption (Phase 01 renderer); double-click & drag-to-redock (Phase 02 guides); edge/snap. |
| `BeepDockingManager.AutoHidePanel/FloatPanel` | **Refactor → `BeepDockingManager.Lifecycle.cs`** | Keep API; route through engine + animator. |

> Correction to earlier assumptions: auto-hide is **not** instant today — the slide panel
> animates. The missing pieces are **hover-peek, slide-back timing, and pin/unpin**, not the
> animation itself.

---

## 4.2 New components (small)

```
Docking/Runtime/Animation/
├── DockAnimator.cs   // optional shared timer-based tween runner (reused by float fade / peek delay)
├── Easing.cs         // easing functions (linear, easeOutQuad…)
└── AnimationTrack.cs // one running tween (from→to, duration, onTick, onComplete)
```

`AutoHideSlidePanel` may keep its own internal timer (it works) or be migrated onto
`AnimationTrack` for consistency — implementer's choice; do not regress current behavior.

---

## 4.3 Behavior

**Auto-hide:**
- Click pin on a docked group → collapse to an `AutoHideStrip` tab on the nearest edge.
- Hover a strip tab → `AutoHideSlidePanel.Show(panel)` (slide in), panel stays while the
  cursor is over the strip or the slide panel.
- Mouse-leaves both for ~300 ms → slide out (`Hide`).
- Click the strip tab (or pin again) → restore to a real docked group (engine re-layout).

**Float:**
- Float via drag (Phase 02) or caption button → `FloatWindow` with themed caption.
- Drag a float over the dock-site → guides appear; drop redocks (Phase 02 commit).
- Optional snap to screen/dock-site edges; double-click caption → redock to last location.

---

## 4.4 Implementation steps

1. `AutoHideLayoutManager`: strip tab hit-test + hover → raise peek/hide intents.
2. Hover-peek + slide-back delay in `AutoHideStrip`/`AutoHideSlidePanel`.
3. Pin/unpin converts between docked group and auto-hide strip (engine `Invalidate`+`ApplyLayout`).
4. `FloatWindow`: themed caption renderer; drag-to-redock via Phase 02; snap.
5. (Optional) `DockAnimator`/`Easing`/`AnimationTrack`; use for float fade-in and peek delay.

---

## 4.5 TODO checklist

- [x] Hover/peek detection — done via `AutoHideStripLayoutManager.HitTest` driven from `AutoHideStrip.OnMouseMove` (no separate manager needed; reuses Phase 01 layout manager).
- [x] Hover-peek + delayed slide-back; keep existing slide animation. `AutoHideStrip.PeekPanel` + a 150 ms poll timer with a ~300 ms grace (`HideGraceTicks`); slide animation in `AutoHideSlidePanel` unchanged.
- [x] Pin/unpin ↔ docked group via engine. `BeepDockingManager.RestoreAutoHiddenPanel` (clicking a strip tab raises `AutoHideStrip.PanelRestoreRequested`); `AutoHidePanel`/`FloatPanel`/`DockFloatingPanel` now call `RecalculateLayout()`.
- [x] Themed `FloatWindow` caption + drag-to-redock + snap. `FloatWindow` is now borderless, paints its caption via the Phase 01 `CaptionRenderer`, suppresses the hosted panel's own caption (`DockPanel.ShowCaption=false`), supports native caption move, WM_NCHITTEST resize borders, double-click/close redock, and owner-edge snapping.
- [x] Shared `DockAnimator`/`Easing`/`AnimationTrack` added under `Runtime/Animation/` (reusable tween runner).

## 4.7 Status — COMPLETE

- Both `TheTechIdea.Beep.Winform.Controls` and `TheTechIdea.Beep.Winform.Controls.Design.Server` build with 0 errors.
- New files: `Runtime/Animation/Easing.cs`, `Runtime/Animation/AnimationTrack.cs`, `Runtime/Animation/DockAnimator.cs`.
- Changed: `Runtime/AutoHideStrip.cs` (peek/poll/restore), `Runtime/FloatWindow.cs` (borderless themed rewrite), `Models/DockPanel.cs` (`ShowCaption`/`EffectiveCaptionHeight`), `BeepDockingManager.cs` (lifecycle routed through engine + `RestoreAutoHiddenPanel`).
- **Runtime validation pending:** the borderless `FloatWindow` chrome (NC hit-testing, native move, snap) and the auto-hide hover-peek timing need an interactive run to confirm feel at 100–200% DPI.

---

## 4.6 Verification criteria

- Pinning collapses to an edge strip; hovering peeks; leaving slides back after the delay.
- Unpin restores the panel to a docked group at the correct position/size.
- Float windows are themed and redock via the Phase 02 guides; snapping works.
- No visual stutter at 100/125/150/200% DPI.
