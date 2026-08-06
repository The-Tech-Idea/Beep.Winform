# Stage 06 — `BeepTaskCard`

**Kind:** refactor · **Files:** `Tasks/` (1,773 lines, 5 helper files)

8 public properties, 3 events, one `DrawContent` override.

## Composition

```
┌──────────────────────────────┐
│ [status] [title]      [more] │  status chip, title, overflow action
│          [description]       │
│ [avatar] [assignee]  [due]   │
└──────────────────────────────┘
```

`Chips/` already provides a status pill — check it before adding anything. `[more]` is a
`BeepButton`, not an image.

**Status: done.** 10 controls.

**`AvatarIndex` had a collection after all.** The stage said to find it before composing: it is
`AvatarImagePaths`, and `AvatarIndex` is a member of `AvatarClickEventArgs` rather than a property of
the card. Each path composes into a `BeepImage` that raises `AvatarClick` with its own captured index,
so the index comes from the control that was clicked instead of from hit-test arithmetic. Nothing had
to be deleted and nothing had to be invented.

**`MoreIcon` is a `BeepButton`** — clickable, focusable, keyboard-reachable, which is what the stage
argued a painted overflow affordance could never be.

**The hard-coded pink gradient is gone.** The constructor set `GradientStartColor`,
`GradientEndColor` and a white `ForeColor` to literals. That is style rather than meaning, and a card
that hard-codes its own surface cannot follow a theme.

**The progress bar is a `BeepProgressBar`**, not a filled rectangle.

## This card gains the most from composition

Two properties are declared and read by nothing:

| property | references anywhere |
|---|---|
| `AvatarIndex` | 0 |
| `MoreIcon` | 0 |

`MoreIcon` is the one that matters, and it is the clearest argument in the folder for this refactor.
An overflow affordance is only useful if it can be **clicked, focused and reached from the keyboard**.
Painted, it is none of those — which is why a painted `MoreIcon` was never going to work regardless of
whether it rendered. As a `BeepButton` it is all three by construction, and the `Click` it needs is an
event rather than hit-test arithmetic.

`AvatarIndex` implies a collection it indexes into. **Find that collection before composing.** An
index with nothing to index is a design that was started and abandoned, and deleting it may be the
honest fix; inventing a collection to justify it would be worse.

## Verification

1. **The overflow action is focusable, keyboard-activatable and hit-testable at its centre.** Three
   assertions, because a painted affordance fails all three and a composed one should pass all three.
2. **Clicking it raises an event.** If no handler exists anywhere, the property is decorative and
   should say so or go — assert whichever is decided.
3. **The avatar renders for a valid index**, and an out-of-range index adds no control rather than
   throwing or leaving a placeholder.
4. **The status chip uses the existing chip control**, asserted by type — not a label styled to look
   like one.
