# Stage 01 — The disposed path and the leaked handler

**Kind:** bug · **Files:** `BeepFloatingBadge.cs` · **Status: done.**

Two defects in the base class every badge derives from. Both are fixed; both have a check that fails
without the fix.

## The crash

`OnPaint` did this:

```csharp
using var shapePath = GetOrCreateShapePath(contentBounds);   // <- disposes the CACHED object
g.FillPath(backBrush, shapePath);
```

`GetOrCreateShapePath` returns a `GraphicsPath` that the instance keeps in `_cachedShapePath`. Handing
it to `using` disposes it at the end of the paint while the field still points at it. The next paint
hits the cache, gets the dead path back, and GDI+ throws:

```
System.ArgumentException: Parameter is not valid.
   at System.Drawing.Graphics.FillPath(Brush brush, GraphicsPath path)
   at ...Badges.BeepFloatingBadge.OnPaint(PaintEventArgs e) in BeepFloatingBadge.cs:line 335
```

**A drop shadow was the only thing hiding it.** The shadow rectangle is `(cbX+1, cbY+1, cbW, cbH)` and
the content rectangle is `(cbX, cbY, cbW, cbH)` — different, so two calls per paint with different
bounds meant the cache missed and rebuilt every time. `ShowDropShadow` defaults to `true`, so the
common path never crashed. Set it to `false` — as `BeepDotBadge` users reasonably would — and the
**second paint of the badge throws**.

### The fix

The cache is gone. Both call sites build their path and dispose it:

```csharp
using var shapePath = GetShapePath(contentBounds);
```

`GetShapePath` was already there, already correct, and already used un-cached for the border. Building
an ellipse or a rounded rectangle is cheap — cheap enough that the cache was not paying for itself even
in the case where it worked, since it missed on every paint that drew a shadow. `_cachedShapeRect`,
`_cachedShapePath`, `_cachedShape`, `_cachedDiameter` and `InvalidateCachedPaths` are removed with it.

The brush and pen caches are untouched. Those are keyed on colour, hit reliably, and are never handed
to `using`.

## The leak

`Attach` subscribed to the parent's `Resize`:

```csharp
if (target.Parent is not null)
    target.Parent.Resize += OnParentResize;
```

`Detach` unsubscribed only under a guard on a field `Attach` never set:

```csharp
if (_badgeParent is not null)          // null unless OnTargetParentChanged had fired
{
    _badgeParent.Resize -= OnParentResize;
    _badgeParent = null;
}
```

`_badgeParent` was assigned in exactly one place — `OnTargetParentChanged` — which only runs if the
target is *reparented*. In the ordinary lifecycle it never fires, so `_badgeParent` stays null and the
handler is never removed.

**Consequences, in order of how much they matter:** the parent's event holds a delegate to the badge,
so a detached and disposed badge is never collected; and the handler still runs, calling `Reposition`
on a badge that is no longer attached.

### The fix

`Attach` records the parent it subscribes to:

```csharp
_badgeParent = target.Parent;
_badgeParent.Resize += OnParentResize;
```

## Verification

Both are checked by observable behaviour, not by reading fields.

1. **Render the badge three times with `ShowDropShadow = false` and compare the bitmaps.** Before the
   fix, paint 2 throws. After, all three are byte-identical.
2. **Attach, detach, dispose, collect, and ask whether the badge is still reachable.** Before the fix
   it is; after, it is collected.

Both checks were wrong before they were right, and both failures are worth recording:

- The leak was first checked by counting `Resize` subscribers through reflection on `EventResize`. It
  reported **0 subscribers even while the badge was attached** — the check could not have failed for
  the reason it was written.
- The reachability test replacing it reported "still reachable" *after* the fix, because a Debug-build
  local stays rooted to the end of its method. Moved into a `[MethodImpl(NoInlining)]` frame so the
  local dies before the collect, it reports correctly.
