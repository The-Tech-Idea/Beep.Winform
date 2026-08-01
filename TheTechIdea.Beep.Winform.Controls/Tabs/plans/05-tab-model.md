# 05 — Tab Model

## Done — each representation owns one thing

`BeepTabItem` carried duplicates of both of the other two representations, and both are removed.

**`Bounds` was a second copy of the tab geometry, and the copies disagreed.**
`BeepTabLayoutHelper` wrote the same rectangle into `item.Bounds` *and* `itemLayout.Bounds`, but
`BeepTabRtlLayoutHelper.MirrorSnapshot` only mirrors the layout copy — so under right-to-left the two
were actively inconsistent, and any future reader of `BeepTabItem.Bounds` would have got un-mirrored
geometry. Nothing read it; proven by deleting it and compiling clean.

**`Content` put a live `Control` inside a render snapshot.** That duplicates what `BeepTabPage`
owns, and it is a lifetime hazard: a disposed page left the snapshot holding a dangling reference.
It was load-bearing in exactly two ways, neither of which needed a control reference:

- `TryGetHostedSourceHeaderBounds` allocated a **whole snapshot** and scanned it comparing
  `item.Content` to the page, purely to recover an index — while `_hostedPages.IndexOf(page)` already
  knows it;
- `GetOrCreateHostedTabMetadata` used `Content == null` as an initialised/not-initialised sentinel,
  now the honestly-typed `IsPageBacked`.

A reflection check now fails if `BeepTabItem` reacquires any geometry-typed property or any
`Control`-derived property, so ownership cannot drift back.

| Type | Owns |
|---|---|
| `BeepTabPage` | content and document state |
| `BeepTabItem` | render and interaction state |
| `BeepTabHeaderItemLayout` | geometry |

**Remaining:** `Index` on a type described as a snapshot — it is load-bearing across the overflow,
layout and hit-test paths, so it stays until there is a reason beyond tidiness to move it.

## Original findings
: Item, Page and Snapshots

**Priority P1.**

## Current behaviour

Three representations of "a tab" coexist:

| Type | Kind | Role |
|---|---|---|
| `BeepTabPage : BaseControl` (461 lines) | a real `Control` | owns content; carries ~14 `Tab*` properties (`TabIconPath`, `TabBadgeText`, `TabIsPinned`, `TabIsDirty`, `TabIsPreview`, `TabCanClose`, …) |
| `BeepTabItem` (sealed) | plain data | render/header snapshot: `Index`, `Title`, `Content`, and eight interaction flags (`IsSelected`, `IsHovered`, `IsPressed`, `IsCloseButtonHovered`, …) |
| `BeepTabHeaderLayoutSnapshot` / `BeepTabHeaderItemLayout` | resolved geometry | what the host paints |

`BeepTabs.HostedContent.cs` (1,048 lines — the largest file in the folder) is the projection layer
between them: `GetHostedSourceItemsSnapshot`, `GetHostedSourcePagesSnapshot`,
`GetHostedSourceItemTitle`, `GetHostedSourceSelectedPage`, and so on.

This is a defensible architecture — a page owns content, an item is an immutable render snapshot,
a layout is resolved geometry. The concern is not that three types exist; it is that **the same
facts are spelled differently in each**, and the projection is hand-written across a thousand lines.

Specifically worth resolving:

- `BeepTabPage.TabIsPinned` / `TabIsDirty` / `TabIsPreview` versus `BeepTabItem`'s flag set — the
  item carries interaction state (`IsHovered`, `IsPressed`) while the page carries document state.
  Which type owns "pinned" is not obvious from either.
- `BeepTabItem.Content` duplicates the page's own content ownership.
- `BeepTabItem.Index` embeds position in what is described as an immutable snapshot; positions
  change on reorder, so an item's `Index` is only valid for the snapshot it came from.

## What the reference products do

DevExpress separates `XtraTabPage` (content owner) from view-info objects (resolved per paint) and
does not keep a third mutable mirror. VS Code models a tab as a document reference plus computed
presentation state. The consistent pattern is **two** levels — the durable model and the per-paint
resolved layout — not three.

## Work

1. **Establish which type owns each fact**, and write it down: document state (pinned, dirty,
   preview, can-close) on the page; interaction state (hover, press, drag) on the item; geometry on
   the layout. Then make each type carry *only* its own category.
2. **Generate the projection rather than hand-writing it** where possible, or at minimum reduce
   `HostedContent`'s surface — 1,048 lines of projection is where drift between the three models
   will hide.
3. **Remove `BeepTabItem.Content`** if the page is the content owner; a snapshot holding a live
   control reference is a lifetime hazard as well as a duplicate.
4. **Reconsider `BeepTabItem.Index`** — either the snapshot is positional (and says so) or items
   are identified by a stable id.
5. **Audit for properties nothing reads** across all three types (see [04](04-stubs-and-scaffolding.md)).

## Verification

- Probe: mutate each `BeepTabPage.Tab*` property and assert the change is visible in the rendered
  header — any property that does not reach the render is either unused or the projection is missing it.
- Probe: reorder tabs and assert every `BeepTabItem.Index` matches its position in the snapshot it
  came from.
- Probe: close a page and assert no `BeepTabItem` retains a reference to its disposed content.
