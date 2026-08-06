# Stage 04 — The notification pulse is clipped

**Kind:** bug · **Files:** `Builtin/BeepNotificationBadge.cs`

Measured: **at `_pulseScale = 1.2f` the control is still 20px wide.** The animation scales the drawing
and not the control, so the outer 20% of every pulse is clipped away by the control's own bounds.

**Status: done.** **Option 2 was taken: the pulse animates colour, not size.**

The fill lightens toward white by up to 35% at the peak. It needs no extra layout footprint, cannot
clip by construction, and does not have to interact with `CornerOverlap` — which centres the badge on
the target's corner from the control's own bounds, so growing the control would have moved that anchor
point on every frame. Option 1 was truer to the original intent and would have cost that interaction.

A `protected virtual Color EffectiveBackColor` on the base carries it, so the subclass varies the fill
per frame without writing to `BadgeBackColor` — which would have marked the colour caller-chosen and
pinned it against the next theme change.

**The timer now runs only while the badge is visible.** It used to start in the constructor and run
until disposal regardless, invalidating every 40ms for the life of the form. `PulseEnabled` (what the
caller asked for) is now separate from `IsPulsing` (whether the timer is ticking), which the single
`_pulseActive` field could not express.

Verified: rest and peak render differently, the control is 20px at both, a hidden badge stops, showing
it again resumes, and disposing stops it.

## Why

```csharp
int scaledW = (int)(Width * _pulseScale);
...
g.TranslateTransform(offsetX, offsetY);
g.ScaleTransform(_pulseScale, _pulseScale);
base.OnPaint(e);
```

`offsetX` is `(Width - scaledW) / 2`, which is **negative** when `_pulseScale > 1` — so the transform
correctly moves the origin up and left to keep the pulse centred. But a WinForms control cannot paint
outside its client rectangle. Everything the transform pushes past the edge is discarded.

The visible result is not "no animation". It is a badge whose *edges* get squared off on every pulse
peak and round out again at rest — a shimmer artifact rather than the intended grow-and-shrink.

`_pulseScale` runs 1.0 → 1.2 in 0.04 steps on a 40ms timer, so roughly a 200ms half-cycle, continuously,
for as long as the badge exists.

## The choice

**Either the control grows with the pulse, or the pulse animates something that is not size.**

1. **Grow the control.** Size the badge to its maximum pulse extent — `BadgeDiameter * 1.2` — and draw
   the resting badge inset within it. The control is then always big enough for the peak, the transform
   never clips, and `Reposition` keeps the *visual* centre where the corner is. Costs a few pixels of
   layout footprint and needs `ApplyCornerOverlap` to centre on the drawn circle, not the control box.
2. **Animate opacity or the border instead.** A pulsing ring or a fading halo reads as "new" just as
   well and needs no extra room. Cheaper, and it sidesteps the interaction with corner overlap.

Option 1 is truer to what the code was trying to do. Option 2 is less work and less risk. Either is
defensible; **pick one and write down which**, because the current state is the one that is not.

## While here

- **The timer runs forever.** `PulseEnabled` is set `true` in the constructor, so every
  `BeepNotificationBadge` starts a 40ms `System.Windows.Forms.Timer` on creation and keeps invalidating
  for its whole life. For a badge meaning "you have unread items" that is arguably intended — but it is
  a repaint every 40ms per badge, and nothing stops it when the badge is not visible.
  **Stop the timer when `Visible` is false or the badge is detached**, and restart on show. A hidden
  badge animating is pure cost.
- **`PulseEnabled`'s getter returns `_pulseActive`, which is also the backing field for the setter's
  intent.** That works, but the name says "active" while the value means "enabled" — they diverge the
  moment you stop the timer for visibility. Separate "enabled by the caller" from "currently running".

## Verification

1. **At peak scale, nothing is clipped.** Render at `_pulseScale = 1.2` and assert the badge's drawn
   extent is inside the control bounds — or, under option 2, that scale is no longer used for size.
   *Today the control is 20px at both rest and peak*, which is the failing run.
2. **The pulse is visible at all.** Two captures at different phases must differ. Guards against a fix
   that stops the clipping by stopping the animation.
3. **A hidden badge does not repaint.** Count `OnPaint` calls over a second with `Visible = false`;
   assert zero.
4. **Disposing a pulsing badge stops the timer.** Already handled by `Dispose` calling `StopPulse` —
   assert it, because it is the kind of thing a refactor silently drops.
