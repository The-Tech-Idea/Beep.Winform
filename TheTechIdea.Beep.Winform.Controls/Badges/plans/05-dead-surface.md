# Stage 05 — Declared and does nothing

**Kind:** cleanup · **Files:** across `Badges/`, plus two members on `BaseControl`

Members a caller can set that have no effect. This is the defect class the Cards program found most of,
and the standard is the same: **a declared input with nothing on the other end of it is a bug, not a
gap.** A caller sets it, the getter agrees, and nothing happens.

**Status: done.**

- **`CustomShapeProvider`** gives `BadgeShape.Custom` the hook it named, mirroring
  `BadgeLocation.BoundsProvider`. With no provider it falls back to a rectangle rather than throwing.
  Both verified.
- **`BadgeFont`** reaches the glyphs — see [02](02-theming.md).
- **`IBeepBadge` gained `Role`, `Shape`, `BadgeBackColor`, `BadgeForeColor` and `ApplyTheme`**, so a
  badge from `BeepBadgeFactory.Create` is configurable without a cast. Verified by writing that call.
- **`IBeepTextBadge`** carries `DisplayText` for the two badges that have it, rather than forcing the
  other four to declare a member they ignore.

## `BadgeShape.Custom` renders as a plain rectangle

Measured: a badge with `Shape = BadgeShape.Custom` renders **pixel-identical** to one with
`Shape = BadgeShape.Rectangle`.

`GetShapePath` has cases for `Circle`, `RoundedSquare`, `Pill` and `Diamond`, and a `default:` that
returns a rectangle. `Rectangle` and `Custom` both fall into it. So the enum member that promises "I
will supply my own shape" silently gives you the default one.

**Two honest options:**

1. **Give it the hook it names.** A `Func<Rectangle, GraphicsPath>? CustomShapeProvider` property, used
   when `Shape == Custom`, falling back to rectangle when null. This mirrors `BadgeLocation`'s
   `BoundsProvider`, which is the same idea for position and is the nicest thing in the folder — so the
   shape equivalent is consistent rather than novel.
2. **Delete the member.** Fewer promises. But it is a public enum value, so removing it is a
   caller-visible break, and there is a clean design one property away.

**Recommendation: option 1.** `BoundsProvider` already proves the pattern works here.

Note the asymmetry that makes this worth doing: `BadgeAnchor.Custom` **does** work — `BadgeLocation`
honours it through `RelativePosition` and `BoundsProvider`. One `Custom` is real and the other is not,
which is exactly the kind of inconsistency that costs someone an afternoon.

## `BaseControl.BadgeFont` never reaches a badge

Written, disposed on teardown, never read. `SyncBadgeAppearance` copies back and fore colour to the
badge and not the font, and there is no font property on any badge to copy it to — `BeepTextBadge` and
`BeepCounterBadge` each construct `new Font("Segoe UI", …)` inline.

**Fix:** a font property on `BeepFloatingBadge` (or on the two text-bearing badges), used in place of
the inline construction, plus the missing line in `SyncBadgeAppearance`. See [02](02-theming.md), where
the same hard-coded font is also a theming problem.

## `BeepFloatingBadge.BadgeForeColor` on badges that draw nothing

`BeepDotBadge.DrawBadgeContent` is empty by design — a dot is just its background. It still inherits
`BadgeForeColor`, which does nothing on it. That is inheritance, not a defect, and needs no change;
recorded so a later census does not "find" it.

## What is NOT dead, and why the census stops here

`BeepDotBadge`, `BeepIconBadge`, `BeepTextBadge`, `BeepValidationBadge`, `BeepNotificationBadge`,
`BeepBadgeFactory`, `BeepBadgeManager`, `BadgeLocations`, `BadgeSide` and `BadgeAlignment` have **no
references outside `Badges/`**.

**None of that is evidence of dead code.** This is a control library and these are its public surface;
consumers of the package are the callers. The Cards program deleted 55 unreferenced painters because
they were an internal detail behind a public control — a different situation with the same grep result.

The census here is therefore restricted to **members that do nothing when called**, which is a property
of the code rather than of who happens to call it.

## `IBeepBadge` is narrow enough to be inconvenient

```csharp
Control? Target { get; }
BadgeLocation Location { get; set; }
bool ShowDropShadow { get; set; }
bool ShowBorder { get; set; }
Color BorderColor { get; set; }
void Attach(Control target); void Detach(); void Reposition();
```

It carries the *border* colour but not the badge's own back or fore colour, its shape, or its text. So
`BeepBadgeFactory.Create("Counter")` hands back an `IBeepBadge` that cannot be given a number to show
without a cast to the concrete type — which defeats most of the point of the factory.

This is a design gap rather than a bug, and it is worth fixing in the same pass as [02](02-theming.md)
since both touch the same property set. Adding `BadgeBackColor`, `BadgeForeColor` and `Shape` to the
interface is source-compatible for implementors inside this library and makes the factory usable.

Text is the harder one — `DisplayText` lives on two of the six and means nothing on a dot. A separate
`IBeepTextBadge` is more honest than putting text on the base interface.

## Verification

1. **`Shape = Custom` with a provider renders the provider's path** and differs from `Rectangle`.
   *Today they are pixel-identical* — that is the failing run.
2. **`Shape = Custom` with no provider falls back to rectangle** and does not throw.
3. **`BadgeFont` changes the rendered glyphs.** Set two clearly different sizes; the bitmaps differ.
4. **A badge from the factory can be fully configured through `IBeepBadge`** without a cast — assert by
   writing the call, since it either compiles or it does not.
