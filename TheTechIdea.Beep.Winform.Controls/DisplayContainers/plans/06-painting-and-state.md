# 06 — Painting: states, theme, and the shadow implementation

## Finding 1 — a second tab painter lives inside a `catch`

`Helpers/TabPaintHelper.cs:295-315`:

```csharp
// Fallback to simple rectangle drawing
try
{
    using (var brush = new SolidBrush(isActive ? ColorUtils.MapSystemColor(SystemColors.ControlLight)
                                               : ColorUtils.MapSystemColor(SystemColors.Control)))
        g.FillRectangle(brush, bounds);
    using (var pen = new Pen(ColorUtils.MapSystemColor(SystemColors.ControlDark)))
        g.DrawRectangle(pen, bounds);

    if (!string.IsNullOrEmpty(title) && bounds.Width > 20 && bounds.Height > 10)
    {
        var textRect = new Rectangle(bounds.X + 4, bounds.Y + 2, bounds.Width - 8, bounds.Height - 4);
        TextRenderer.DrawText(g, title, font, textRect, ColorUtils.MapSystemColor(SystemColors.ControlText),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
    }
}
```

This is a complete second tab renderer, reached only when the real one throws. It differs from the
live path in every respect that matters:

| | live path (`:695`) | fallback (`:311`) |
|---|---|---|
| Colours | theme (`colors.TextColor`, …) | hardcoded `SystemColors` |
| Alignment | `Left` | `HorizontalCenter` |
| Padding flag | `NoPadding` | *absent* — text measures wider than layout assumed |
| Slot geometry | `TabHeaderMetrics` | inline `+4 / +2 / -8 / -4` |

The user-visible consequence: when the painter fails, tabs do not disappear — they silently change
alignment, lose the theme, and start ellipsising early. That reads as a styling bug, not an error,
so nobody investigates. It is the "no fallback / no swallow" rule in one place.

Three `SystemColors` references remain in the painter; all are in this path.

**Work:** delete the fallback. If the real painter can throw, that is a defect to surface and fix
(see [07](07-exception-policy.md)), not to paper over with a differently-styled renderer.

## Finding 2 — state coverage is unverified

The strip must express: normal, hover, active, active+hover, pressed, dragging, pinned, badged,
closable-hover (close glyph only appears on hover), disabled, and focused-for-keyboard. Some are
implemented (`closeAlpha` at `:276` implies a hover fade). None are asserted anywhere.

The BeepTabs program found seven painters whose visually distinct *code* produced pixel-identical
*output*; reading an implementation is not evidence it renders differently. Every state above needs a
render assertion against a controlled baseline — specifically, each state must differ from `normal`,
and `active` must differ from `hover`.

## Finding 3 — no keyboard focus affordance

No focus ring or equivalent is drawn for the focused tab. A container navigable by keyboard with no
visible focus indicator fails WCAG 2.4.7. This pairs with the accessibility work already done for
`BeepTabs` (`BeepTabs.Accessibility.cs`), which v2 does not have an equivalent of — v2 paints
natively and therefore inherits none of it.

**Work:** draw a focus indicator distinct from both hover and active, and add an accessible object
tree for v2 (`Role = PageTabList`, children `Role = PageTab`) mirroring `BeepTabs.Accessibility.cs`.

## Work

- [ ] Delete the `catch`-hosted fallback renderer and its `SystemColors` uses
- [ ] Assert every visual state renders differently from `normal`
- [ ] Add a keyboard focus indicator
- [ ] Add `CreateAccessibilityInstance` for `BeepDisplayContainer2`

## Verification

- pairwise render comparison across all states — any two states rendering identically is a failure,
  the same check that exposed the duplicate tab painters
- focus indicator visible and distinct from hover and active
- accessible tree reports a tab list with one child per tab, each named by its caption
- high-contrast: captions and glyphs remain legible with `SystemInformation.HighContrast` on
