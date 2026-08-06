# Stage 04 — `BeepFeatureCard`

**Kind:** refactor · **Files:** `Features/` (1,754 lines, 5 helper files)

10 public properties, 4 events, one `DrawContent` override.

## Composition

```
┌──────────────────────────────┐
│ [logo]                       │  optional
│ [icon]   [title]             │
│          [description]       │
│          [• bullet]          │  one row per bullet, each icon + text
│          [• bullet]          │
│          [action1] [action2] │
└──────────────────────────────┘
```

Bullets are the one part needing care: a variable number of rows, each a `BeepImage` and a
`BeepLabel`. They are added in the `.cs` file rather than the designer, because the count depends on
data — that is the exception the designer-file rule allows, and it should be the only one here.

**Status: done.** 10 controls, and the four dead icon properties have controls to be assigned to.

**The bullets stayed a `BeepListBox`.** The plan called for one composed row per bullet; the card
already hosted a list box that renders an icon and a label per item *and* already honours `ListStyle`.
Hand-rolling rows would have wired `BulletIconPath` and silently lost the other presentation modes —
the exact failure this stage warned about. Using the control that exists is the composition rule, not
an exception to it, so `ComposeBullets` places the list box in the row that absorbs leftover height.

**The two actions are `BeepButton`s**, so they focus, take the keyboard and raise their own `Click`.
The rectangle arithmetic in `OnMouseClick`/`OnMouseMove` that decided which painted icon was under the
mouse is gone with them.

**An unset icon adds no control** — verified: with `LogoPath` empty the tree has no logo `BeepImage`
rather than an empty one holding a cell open.

## Four icon properties finally do something

Measured across 4,822 files, excluding the declaring file:

| property | references anywhere |
|---|---|
| `CardIconPath` | 0 |
| `BulletIconPath` | 0 |
| `ActionIcon1Path` | 0 |
| `ActionIcon2Path` | 0 |

A feature card is *"icon-based feature highlight with title and description"* — `BeepCard.cs`'s own
enum comment. Every icon path it accepts is declared and unread, so the icon a caller sets never
appears. This is the largest single build item across the card stages and the card's defining feature.

In the composed card each becomes a `BeepImage.ImagePath` — the property has a control to be assigned
to, which is why the wiring stops being something anyone has to remember.

## `ListStyle`

`ListStyle` selects how bullets present. Read it before composing: if it distinguishes bullets from
numbers from checkmarks, that is three bullet glyph sources and `BulletIconPath` is only one of them.
Composing it as "always `BulletIconPath`" would wire one property and silently break the other modes.

## Verification

1. **A card icon renders.** Set `CardIconPath`; assert a `BeepImage` in the tree carries it. *Today
   nothing does.*
2. **Bullets render one row each**, with the count matching the data, and each row's icon and text on
   one baseline.
3. **Both action icons render and are distinct**, and both actions are focusable and hit-testable —
   which painted icons were not.
4. **An unset icon adds no control**, rather than an empty `BeepImage` occupying a cell.
5. **Every `ListStyle` mode renders its own glyph kind**, asserted per mode. Catches the
   one-property-wired-three-modes-broken failure above.
