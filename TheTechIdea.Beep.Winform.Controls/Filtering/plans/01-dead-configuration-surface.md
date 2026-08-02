# 01 — Dead configuration surface

Two public enums are offered on `BeepFilter` as though they configure it. One is inert entirely; the
other honours two of its five values.

## Finding 1 — `FilterPosition` does nothing at all

`FilterStyle.cs:116` declares `FilterPosition { Top, Bottom, Left, Right, Floating }`.
`BeepFilter.Properties.cs:66-73` exposes it as a browsable property with `[DefaultValue(Top)]`.

Every reference to the backing field, across the whole solution:

```
BeepFilter.cs:36                 private FilterPosition _position = FilterPosition.Top;
BeepFilter.Properties.cs:69      get => _position;
BeepFilter.Properties.cs:72      if (_position != value)
```

The declaration, the getter, and the setter's own change-check. **Nothing reads it to place
anything.** Setting `Position = FilterPosition.Left` in the designer or in code changes nothing on
screen, and the property grid gives no hint of that.

## Finding 2 — `FilterDisplayMode` honours two of five values

`FilterStyle.cs:85` declares `{ AlwaysVisible, Collapsible, OnHover, Modal, SlideIn }`.

Counting real comparisons (`== FilterDisplayMode.X` or `case FilterDisplayMode.X`):

| value | comparisons | effect |
|---|---|---|
| `Collapsible` | 2 — `BeepFilter.cs:1279`, `BeepFilter.Layout.cs:45` | works |
| `AlwaysVisible` | 0 | works only as the default, i.e. "not Collapsible" |
| `OnHover` | 0 | **nothing** |
| `Modal` | 0 | **nothing** |
| `SlideIn` | 0 | **nothing** |

`OnHover`, `Modal` and `SlideIn` are indistinguishable from `AlwaysVisible` at runtime. A caller
selecting `Modal` gets an always-visible inline filter.

## Why this matters more than it looks

Both enums are `public` and both properties are `[Browsable]`. They are part of the control's
advertised configuration surface, they appear in the designer's property grid, and they are
serialised into `.Designer.cs` files. A developer who sets one has no way to discover it is inert
short of reading the source.

## Work

- [ ] Decide per value: **implement** or **remove**. There is no third option — leaving a
      browsable property that does nothing is the defect.
- [ ] `FilterPosition` — either place the filter surface accordingly (Top/Bottom/Left/Right dock the
      filter region; Floating hosts it in a popup) or delete the enum and the property.
- [ ] `FilterDisplayMode.OnHover` — reveal on pointer-enter, hide on leave, with the hover-persist
      rule that already exists in the notifications subsystem (a surface that vanishes while the
      pointer is inside it fails WCAG 1.4.13).
- [ ] `FilterDisplayMode.Modal` — present the filter in a dialog. `DialogsManagers` already provides
      the shell; this should host, not reimplement.
- [ ] `FilterDisplayMode.SlideIn` — either animate the reveal or drop it in favour of `Collapsible`.
- [ ] Sweep all sibling repos before deleting either enum: designer files in
      `Beep.Winform.Data.Integrated` may already serialise these properties, and a removed enum
      member is a compile break there, not here.

**Recommendation:** implement `OnHover` and `Modal`, delete `SlideIn` (it is `Collapsible` with an
animation, not a distinct mode), and decide `FilterPosition` on whether a docked filter region is
wanted at all — if it is not, deleting five enum values and a property is the honest outcome.

## Verification

- Set each `FilterDisplayMode` value in turn and assert the rendered surface differs from
  `AlwaysVisible`. Three of them currently cannot, which is the defect — so this check must be shown
  to fail before the fix and pass after.
- Set each `FilterPosition` value and assert the filter region's bounds change.
- No `[Browsable(true)]` property on `BeepFilter` whose value never reaches behaviour — worth
  enforcing mechanically in [09](09-verification-harness.md), since this is the second folder in a
  row where a complete feature was missing only its last wire.

---

## Outcome

Swept all consuming repositories first, as this document requires. `FilterPosition` had **zero**
references outside this folder; `FilterDisplayMode` had two, both in `GridX`, both setting
`AlwaysVisible` — the default. Nothing external depended on what was removed.

*(The first sweep returned zero for both, including for `FilterDisplayMode`, which was known to have
GridX consumers. A `timeout` was killing the traversal mid-way and reporting the partial result as
empty — a silent truncation indistinguishable from "no consumers". Re-run scoped per repository.)*

### Decisions

| value | decision | reason |
|---|---|---|
| `FilterPosition` (whole enum) | **deleted** | Entirely inert, no consumers. A docked filter region is a product decision, not a defect repair. |
| `FilterDisplayMode.OnHover` | **implemented** | Collapse machinery already existed for `Collapsible`; OnHover differs only in what expands it. |
| `FilterDisplayMode.Modal` | **deleted** | Hosting the filter in a dialog is the caller's concern, and `BeepGridPro.ShowAdvancedFilterDialog` already does it. A control-level flag would be a second way to do the same thing. |
| `FilterDisplayMode.SlideIn` | **deleted** | `Collapsible` with an animation, not a distinct mode. |

`FilterDisplayMode` now declares three values and honours all three.

### Implementation

`Collapsible` and `OnHover` share one predicate, `CollapsesWhenInactive`, so the layout cannot honour
one and forget the other. `OnHover` expands on `MouseEnter` and collapses on `MouseLeave` — but only
after confirming the pointer has genuinely left the client rectangle, because a suggestion popup or a
child editor takes the pointer outside the control while the user is still working, and collapsing
then would close the filter mid-edit.

The new handler was folded into `BeepFilter`'s **existing** `OnMouseLeave` rather than added beside
it; the compiler caught the duplicate. Worth noting because the previous program shipped a duplicate
focus ring for exactly this reason — a complete implementation already existed and was not looked for.

### Measured

| mode | collapsed height |
|---|---|
| `AlwaysVisible` | 200 (full) |
| `Collapsible` | 32 (header only) |
| `OnHover` | 32 (header only) — **was 200**, indistinguishable from the default |

Asserted in `scratchpad/FilterProbe`, with a baseline confirming the check reports `AlwaysVisible` as
*not* distinguishable from itself.
