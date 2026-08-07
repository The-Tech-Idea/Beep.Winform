# Stage 05 — The ticker ran forever, and every label allocated a font

**Kind:** perf · **Status: done.**

## A 50ms timer for the life of the control

The constructor started a 50ms `Timer` unconditionally and never stopped it. Its tick invalidated the
whole control, so **every** bar repainted about twenty times a second - hidden, empty, or drawn in a
style with nothing to animate.

Only four of the ten styles read `AnimationPhase`: Bubble, Pill, NotionMinimal and FloatingCTA. The
painters declare it now:

```csharp
bool WantsContinuousAnimation { get; }   // false on the base
```

and `UpdateTickerState` runs the timer only while it is worth running - visible, items present, a
style that animates, and not switched off. It is re-evaluated on style change, visibility change,
items change and construction.

**`AnimateContinuously`** was added because the motion had no off switch at all. A navigation bar that
never stops moving is a distraction on a desktop and a cost on a laptop, and a reduced-motion setting
had nothing to turn off. The default preserves the existing look.

## A Font per label per paint

`ResolveItemFont` returned `new Font(...)` on every call, once per label per paint. With the ticker
running that is a hundred GDI font handles a second on a five-item bar, all going to the finalizer.
It is cached against the theme's family/size/style now and disposed with the painter.

## What was not changed

The perpetual *design* - a selection that breathes forever - is left as it is. Whether a nav bar
should animate at rest is a design decision rather than a defect, and `AnimateContinuously` makes it
answerable either way without changing the default.
