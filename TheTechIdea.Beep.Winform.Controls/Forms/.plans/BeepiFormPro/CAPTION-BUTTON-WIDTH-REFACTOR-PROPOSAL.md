# Caption Button Width Refactor Proposal

Last updated: 2026-05-10

## Goal

Turn the remaining manual right-side caption button-width review into explicit design decisions instead of another open-ended audit pass.

## Scope

- Fluent
- Cartoon
- ChatBubble
- Custom

This proposal excludes the styles already normalized into explicit metrics:

- GNOME uses `FormPainterMetrics.VisualButtonWidth` for its narrower painted pill width.
- MacOS and iOS use `FormPainterMetrics.AuxiliaryButtonWidth` for the wider right-side auxiliary cluster.

## Evidence Snapshot

| Style | Live Layout Contract | Live Width | Metrics Defaults | Proposed Bucket | Why |
|---|---|---:|---|---|---|
| Fluent | `captionHeight = owner.Font.Height + 16` | 32 | `CaptionHeight = 40`, `ButtonWidth = 36` | Independent font-derived contract | Both height and width differ from the metrics table, so width-only normalization would hide a broader local caption strategy. |
| Cartoon | `captionHeight = owner.Font.Height + 20` | 32 | `CaptionHeight = 38`, `ButtonWidth = 34` | Shared live-layout pair | Its live layout still matches ChatBubble structurally, but the metrics table no longer describes that shared code path. |
| ChatBubble | `captionHeight = owner.Font.Height + 20` | 32 | `CaptionHeight = 34`, `ButtonWidth = 30` | Shared live-layout pair | Same live caption formula and width as Cartoon, but different metrics intent. The next change must decide whether code or metrics is the source of truth. |
| Custom | `captionHeight = metrics.CaptionHeight` | 46 | `CaptionHeight = 32`, `ButtonWidth = 32` | Intentional wide cluster | Caption height already comes from metrics, but the wider button cluster is a visible painter baseline rather than accidental drift. |

## Recommended Decision Model

### Fluent

- Keep the painter-owned width and caption-height formula for now.
- Only revisit this painter if a second right-side style needs the same font-derived caption strategy or if `FormPainterMetrics` gains an explicit font-derived caption concept.
- Do not replace `32` with `metrics.ButtonWidth` in isolation.

### Cartoon and ChatBubble

- Review these two styles together because their current live layout code still follows the same caption-height and button-width formula.
- Choose one source of truth before editing either painter:
  - Option A: `FormPainterMetrics` becomes authoritative. Move both painters to `metrics.CaptionHeight` and `metrics.ButtonWidth`, then validate visuals against the diagnostics host.
  - Option B: the live font-derived layout remains authoritative. Add an explicit helper or metric concept for font-derived caption families, such as `FontDerivedCaptionPadding`, and keep the width decision attached to that family contract.
- Do not normalize only one of these painters. That would split a shared live layout path without resolving the underlying contract mismatch.

### Custom

- Keep the wider `46` width painter-owned for now.
- If another style later needs the same wider control cluster, add a new explicit metric token such as `WideButtonWidth` or `PrimaryClusterButtonWidth` instead of overloading `ButtonWidth`.
- Do not fold this painter back to `32` unless the customizable painter's public visual baseline is intentionally changed.

## Diagnostics-Driven Validation

Use `WinFormsApp.UI.Test --demo diagnostics` as the validation surface before changing any of the remaining manual widths.

Check all three live surfaces together:

1. `FormPainterMetrics` values.
2. `CurrentLayout` rectangles.
3. Registered hit areas from `GetRegisteredHitAreasSnapshot()`.

Recommended manual checks:

1. Compare Fluent, Cartoon, ChatBubble, and Custom at default DPI.
2. Repeat at higher DPI and after maximize or restore.
3. Toggle search, profile, theme, and style buttons to confirm the width relationship still holds when the right-side cluster changes.
4. For Cartoon and ChatBubble, also compare at least one larger font size because their live caption height is still font-derived.

## Suggested Next Implementation Order

1. Capture a diagnostics-host pass for Fluent, Cartoon, ChatBubble, and Custom.
2. Decide whether Cartoon and ChatBubble should become metrics-driven or should stay a font-derived family with an explicit helper or metric concept.
3. Revisit Fluent only after that family decision exists.
4. Leave Custom alone unless a second painter actually needs the same wide-cluster baseline.

## Non-Goals

- No broad search-and-replace of caption button widths.
- No helper extraction before the family boundary is explicit.
- No metrics expansion for one isolated painter without a second concrete adopter.