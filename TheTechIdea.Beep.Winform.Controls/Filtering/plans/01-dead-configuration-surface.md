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
