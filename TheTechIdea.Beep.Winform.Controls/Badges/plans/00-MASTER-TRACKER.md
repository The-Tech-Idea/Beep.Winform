# Badges — review and enhancement

Master tracker for `TheTechIdea.Beep.Winform.Controls/Badges/`.
**19 C# files, 1,501 lines, one base control, six built-in badges, a manager, a factory.**

## What this folder is

`BeepFloatingBadge` is a `UserControl` that attaches itself to *another* control, adds itself to that
control's **parent**, and repositions to a corner whenever the target moves. Six built-ins derive from
it: dot, text, counter, icon, validation, notification. `BaseControl.BadgeText` is the live consumer —
setting it auto-creates a `BeepCounterBadge`.

The design is sound. The badge-on-the-parent trick is the right one for WinForms, where a child cannot
paint outside its parent's bounds, and `BadgeLocation` with its `BoundsProvider` escape hatch is a
genuinely good abstraction. **The problems are in the implementation, not the shape of it.**

## Two bugs were fixed during the review

Both are in `BeepFloatingBadge`, both confirmed by a probe that fails without the fix.

| | |
|---|---|
| **A crash** | `OnPaint` handed its *cached* `GraphicsPath` to `using`, disposing the object the cache still held. The next cache hit returned the dead path and `FillPath` threw `ArgumentException: Parameter is not valid`. |
| **A leak** | `Attach` subscribed to `parent.Resize` but never assigned `_badgeParent`, so `Detach`'s `if (_badgeParent is not null)` guard never ran. Every attached-then-detached badge left a live handler on the parent, which kept the badge alive and repositioned a detached one. |

**The crash needed no unusual input — only `ShowDropShadow = false`.** With a shadow, the shadow and
content rectangles differ by one pixel, so the cache missed on every paint and rebuilt; that is the
only reason this was not constant. See [01](01-crash-and-leak.md).

## Stages

| # | Stage | Kind | Status |
|---|---|---|---|
| [01](01-crash-and-leak.md) | The disposed path and the leaked handler | **bug** | ☑ done |
| [02](02-theming.md) | Badges cannot follow a theme | enhancement | ☑ done |
| [03](03-sizing.md) | A badge cannot be wider than tall | **bug** | ☑ done |
| [04](04-pulse.md) | The notification pulse is clipped | **bug** | ☑ done |
| [05](05-dead-surface.md) | Declared and does nothing | cleanup | ☑ done |
| [06](06-swallows.md) | Five swallowed exceptions | cleanup | ☑ done |
| [07](07-verification.md) | The harness | verification | ☑ done |

Status marks: ☐ open · ◐ in progress · ☑ done

## Every stage is done

**27 checks, 0 failures.** Each stage's fix has a check that fails without it.

| stage | outcome |
|---|---|
| 01 | crash and leak fixed — the shape cache is gone, `Attach` records its parent |
| 02 | badges follow the theme by subscribing, **not** by re-parenting onto `BaseControl` |
| 03 | `BeepTextBadge("NEW")` is 50×18; `"IN PROGRESS"` is 113px; middle anchors fixed |
| 04 | the pulse animates **colour, not size**, and stops while hidden |
| 05 | `CustomShapeProvider`, a working `BadgeFont`, a usable `IBeepBadge` |
| 06 | no bare `catch` remains; a broken icon renders as broken |

### Two decisions, made and recorded

**Theming: subscribe rather than re-parent.** The defect was "badges do not follow the theme", not
"badges do not derive from `BaseControl`". Subscribing to `BeepThemesManager.ThemeChanged` buys the
whole benefit without pulling a large base control's painting, hit-testing, hover and focus machinery
into a 10-24px decoration that is `TabStop = false`.

**Pulse: animate colour rather than grow the control.** Growing it was truer to the original intent and
would have had to interact with `CornerOverlap`, which anchors on the control's own bounds — so the
badge's anchor point would move on every frame. Lightening the fill cannot clip by construction.

### The one that only a check could have caught

Removing the two paint `catch { }` blocks would have changed nothing. `StyledImagePainter.Paint`
**writes a `Debug` line and returns** when it cannot resolve an image — it never throws, so the catch
had never caught anything and an unresolvable icon was already failing silently one layer down.

The check "a broken icon looks different from no icon" **failed after the first fix**, which is how this
surfaced. The badge now asks `ImagePainter.HasImage` whether the path resolves and draws a
missing-glyph cross. Catching an exception that is never raised is not error handling.

## The defects, and how each was established

| finding | how it was established |
|---|---|
| **Cached `GraphicsPath` disposed by its caller → crash** | A live stack trace: `ArgumentException` at `Graphics.FillPath`, `BeepFloatingBadge.cs:335` |
| **`parent.Resize` handler leaked on `Detach`** | Badge unreachable after `Detach`+`Dispose`+GC only once fixed |
| **A pill badge is forced square** | `BeepTextBadge("NEW")` measures **18×18** with `Shape = Pill` |
| **Badges cannot follow a theme** | Derives from `UserControl`, not `BaseControl`; back colour is the literal `Red` |
| **`BadgeShape.Custom` is a silent no-op** | Renders **pixel-identical** to `Rectangle` |
| **The notification pulse is clipped** | At 1.2× scale the control is still 20px wide — the outer 20% is cut off |
| **`BaseControl.BadgeFont` never reaches a badge** | No font property on any badge; each hard-codes `new Font("Segoe UI", …)` |

Every check was confirmed to discriminate before being trusted. The shape check asserts that
`Rectangle` and `Circle` render *differently* first — otherwise "Custom == Rectangle" would prove
nothing but a blind instrument.

### Two checks were wrong before they were right

- **Counting `Resize` subscribers by reflection reported 0 even while attached.** It could not have
  failed for the reason it was written. Replaced with a reachability test.
- **The reachability test then reported "leaked" even after the fix**, because a Debug-build local
  stays rooted to the end of its method. Moved into a non-inlined frame, it reports correctly.

Same lesson the Cards program kept hitting: **the instrument is wrong at least as often as the code.**

## What is deliberately not a finding

Most of this folder's public surface has **no callers inside the solution** — `BeepDotBadge`,
`BeepIconBadge`, `BeepTextBadge`, `BeepValidationBadge`, `BeepNotificationBadge`, `BeepBadgeFactory`,
`BeepBadgeManager` and `BadgeLocations` are all unreferenced outside `Badges/`.

**That is not evidence of dead code here.** This is a control library; these types exist for consumers
of the package, and "unused internally" is their normal state. The Cards program deleted unreferenced
painters because they were an *internal* implementation detail behind a public control. The distinction
matters, and nothing in [05](05-dead-surface.md) is proposed for deletion on reference count alone —
only where the member does nothing when called.

## Standing constraints

- **Nothing assigns colours.** Controls resolve their own from `BeepThemesManager`. The exception is a
  colour that carries meaning — a validation state, an alert.
- **`BeepImage` for every icon.** It is what renders and themes SVGs.
- Never swallow an exception — the five that existed are gone, see [06](06-swallows.md).
- Do not modify `BaseControl` beyond what stage [02](02-theming.md) records; use it.

## The rule every stage is verified against

**A check must be able to fail for the reason it was written.** Every stage states what a failing run
prints today.
