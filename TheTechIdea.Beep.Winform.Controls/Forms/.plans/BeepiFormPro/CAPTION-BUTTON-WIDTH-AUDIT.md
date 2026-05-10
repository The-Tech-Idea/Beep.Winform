# Caption Button Width Audit

Last updated: 2026-05-10

## Goal

Document the remaining caption button-width outliers so width changes are driven by style intent and layout contracts, not by a blind replacement of local constants with `FormPainterMetrics.ButtonWidth`.

Use `CAPTION-BUTTON-WIDTH-REFACTOR-PROPOSAL.md` as the next-step design record once the outliers have been identified.

## Baseline Rule

- Safe normalization has already been applied where the local layout matched the shared metric exactly.
- Remaining mismatches stay painter-owned until the metrics model or the painter contract changes deliberately.

## Audited Outliers

| Style | File | Local Width | Metrics Width | Current Decision | Reason |
|---|---|---:|---:|---|---|
| Fluent | `Painters/FluentFormPainter.cs` | 32 | 36 | Keep local for now | The manual layout also derives caption height from `owner.Font.Height + 16`, so the width mismatch is part of a broader local caption contract rather than a standalone literal. |
| Cartoon | `Painters/CartoonFormPainter.cs` | 32 | 34 | Keep local for now | The painter still uses a font-derived `owner.Font.Height + 20` caption formula, so the width mismatch is tied to a broader local caption-height contract rather than one isolated literal. |
| ChatBubble | `Painters/ChatBubbleFormPainter.cs` | 32 | 30 | Keep local for now | The painter also still uses `owner.Font.Height + 20`, which means the width review belongs with the local caption-height formula and with Cartoon's still-shared live layout path. |
| Custom | `Painters/CustomFormPainter.cs` | 46 | 32 | Keep local | The customizable painter intentionally reserves a wider control cluster than the shared metric, so replacing it would change the painter's public visual baseline. |
| GNOME | `Painters/GNOMEFormPainter.cs` | 28 paint width | 32 | Resolved in metrics | GNOME now uses `FormPainterMetrics.VisualButtonWidth` for the narrower painted pill while keeping the wider hit target contract. |
| MacOS | `Painters/MacOSFormPainter.cs` | 32 right-side auxiliary width | 28 | Resolved in metrics | The traffic-light family now uses `FormPainterMetrics.AuxiliaryButtonWidth` for the right-side optional-button cluster instead of a painter-local literal. |
| iOS | `Painters/iOSFormPainter.cs` | 32 right-side auxiliary width | 30 | Resolved in metrics | Same traffic-light-family fix as macOS: the wider right-side auxiliary controls now come from `AuxiliaryButtonWidth`. |

## Already Normalized

| Style | Result |
|---|---|
| Glass | Normalized to `FormPainterMetrics.ButtonWidth` because the local width already matched the shared metric. |

## Audit Conclusion

- The remaining width mismatches are not one uniform cleanup category.
- The shared metrics model now separates three concepts: shared hit-target width (`ButtonWidth`), traffic-light auxiliary width (`AuxiliaryButtonWidth`), and GNOME-style painted visual width (`VisualButtonWidth`).
- The next safe step is still a targeted metrics-model decision, not a broad search-and-replace.
- The follow-up proposal now splits the four unresolved right-side painters into three buckets instead of treating them as one cleanup pass.

## Follow-Up Questions

1. Should `FormPainterMetrics` distinguish hit-target width from painted visual width for styles like GNOME?
2. Should manual right-side painters such as Fluent, Cartoon, and ChatBubble move to metrics only after their local caption-height formulas are reviewed at the same time?
3. Should any non-GNOME painter also expose a distinct painted visual width, or should that remain a GNOME-only style token until another concrete user shows up?