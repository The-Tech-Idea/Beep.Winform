# Stage 02 — `BeepCard`: 56 styles as compositions

**Kind:** refactor · **Files:** `BeepCard.cs` + 6 partials (2,496 lines), `Painters/` (10,727 lines)
**Do this last of the card stages.** 56 styles is the bulk of the work; let the pattern survive five
smaller controls first.

**Status: done.** All 56 styles compose; `BlankCard` is the only empty one, which is what its name
promises.

## 56 styles became a table, not 56 methods

Every style is a selection of the same parts in an order. That is data, so it lives in
`Helpers/CardStyleLayouts.cs` as a `CardStyle → CardPart[]` map, and `Compose()` is one loop over it.
A new style is an entry rather than a class.

The parts: `Avatar`, `Hero`, `Header`, `Subtitle`, `Paragraph`, `Emphasis`, `Badges`, `Tags`,
`Rating`, `Status`, `Actions`, `PrimaryAction`, `SecondaryAction`. A part whose property is empty adds
no control, so a style listing `Badges` with no badge text set does not hold an empty cell open.

## `PriceText` had a backing field and no property

`_priceText` was declared, commented *"For product cards"*, and had **no public property** — nothing
outside the class could ever set it. `PricingCard`, `ProductCard`, `OfferCard`, `ServiceCard`,
`CartItemCard`, `StatCard` and `MetricCard` all exist to show one prominent number, and none of them
could be given one. It has a property now, and it is what the `Emphasis` part reads.

This is the same class of defect as `TrendText` on the stat card and the four icon paths on the
feature card: a declared input with nothing on the other end of it.

## What else went with the painters

- **`AccessibleName` is no longer the literal `"Card"` for all 56 styles.** Each label carries the
  text it displays, so the name is the content and there is no second string to keep in step.
- **`BeepCard.Drawing.cs` is deleted** — 520 lines including `PaintEnhancedButton`,
  `DrawLoadingSkeleton`, `DrawAccentBar`, `DrawFocusRing` and `DrawRippleOverlay`.
- **The `_layoutContext` hit-testing in `BeepCard.Events.cs` is gone.** It read rectangles a painter
  computed to decide whether the mouse was over the button, image, header or paragraph. Each is a
  control now. `ProcessDialogKey` asks the composed button whether it exists instead of asking a
  rectangle whether it is empty.
- **Both swallowed exceptions are gone**, including the one the tracker recorded at `BeepCard.cs:238`.
  The constructor no longer wraps its whole body in a `catch` that writes to `Debug`, and
  `SafeInvalidate` no longer has a bare `catch`.
- **Collapsed height is measured from the composed header**, so it stays right when the header wraps.

## Alignment properties still mean something, but less

`HeaderAlignment` and `TextAlignment` were written for text painted inside a computed rectangle, where
the vertical half mattered. A label occupies a row the layout engine sized to it, so only
left/centre/right survives — and it is read, which is what keeps a caller's centred header centred.


## What changes

`CardStyle` keeps all 56 public values — no caller changes. The switch at `BeepCard.cs:250-320` stops
returning a painter and starts selecting a composition. The 55 painter classes go in stage
[08](08-removals.md), once nothing selects them.

## The 56 styles are not 56 layouts

Read them before building. Grouped by the scaffold they need, most collapse:

- **Single-part styles** — `BadgeOnly`, `ButtonOnly`, `HeaderOnly`, `ImageOnly`, `ParagraphOnly`,
  `RatingOnly`, `SecondaryButtonOnly`, `StatusOnly`, `VideoOnly`, `Blank`. One control in one cell,
  or none. These are the cheapest and should be built first as the pattern's smoke test.
- **Icon + text** — `FeatureCard`, `ServiceCard`, `IconCard`, `BenefitCard`. The scaffold's default
  shape.
- **Media + text + actions** — `ContentCard`, `BlogCard`, `NewsCard`, `MediaCard`, `ProductCard`.
  Media row, text column, action row.
- **Avatar + text** — `ProfileCard`, `CompactProfile`, `UserCard`, `TeamMemberCard`,
  `TestimonialCard`, `SocialMediaCard`.
- **Data-bearing** — `ChartCard`, `DataCard`, `StatCard`-alikes. These host an existing chart or
  progress control; they do not draw one.

**A style that turns out to be an exact duplicate of another is a finding, not a layout to copy.**
Three already are — see below.

## Three defects that dissolve, and one that does not

The survey found four defects in this control. Composition removes three by construction:

| defect | after |
|---|---|
| `AccessibleName = "Card"` for all 56 styles (`BeepCard.cs:200`) | the title label carries the title; there is no separate string to be wrong |
| `AccessibleDescription = $"Card: {style}"` (`:201`, `.Properties.cs:23`) | the body label carries the body |
| 13 painters never scaling for DPI, 24 literal colours | no painters |

**The one that does not dissolve** is the style/painter arithmetic, because it is a question about
intent rather than about rendering:

```
ImageCard    → renders as MediaCardPainter
DownloadCard → renders as InteractiveCardPainter
ContactCard  → renders as InteractiveCardPainter
CommunicationCardPainter    exists, never constructed
ProductCompactCardPainter   exists, never constructed
```

Composition forces the question rather than answering it: each of the three styles needs a
composition, and whoever writes it must decide whether `ContactCard` really is `InteractiveCard` or
whether `CommunicationCardPainter` was written for it and never wired. **Read the two unused painters
before deleting them** — they are the only surviving record of what those styles were meant to look
like.

## The swallowed exception

```csharp
private void SafeInvalidate()
{
    try { if (!IsDisposed && IsHandleCreated) Invalidate(); }
    catch { }        // BeepCard.cs:238
}
```

The guard already covers the two cases `Invalidate` throws for. The catch hides everything else, and
`SafeInvalidate` exists to drive painter repaints — so it should go with them rather than be carried
into the composed control.

## Verification

1. **Every style renders**, and the corpus render for each is compared against the pre-refactor
   capture. Differences are expected; **unexplained** differences are the finding.
2. **Every style produces controls.** Assert each composition contains at least one child control —
   catches a style whose switch arm was never written and silently renders an empty card.
3. **The accessible name carries content**, asserted from the control tree rather than the card's own
   property. *Today it is the literal `"Card"` for all 56.*
4. **The three aliased styles are decided.** Either they compose differently, or the enum documents
   that they are intentionally identical. Assert whichever is chosen.
5. **No painter is referenced** from `BeepCard` after migration — the check that lets stage 08 delete
   safely.
