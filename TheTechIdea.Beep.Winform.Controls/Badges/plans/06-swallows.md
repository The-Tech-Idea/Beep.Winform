# Stage 06 — Five swallowed exceptions

**Kind:** cleanup · **Files:** `BeepFloatingBadge.cs` ×3, `Builtin/BeepIconBadge.cs`, `Builtin/BeepValidationBadge.cs`

Five bare `catch { }` blocks. All five discard the exception entirely — no log, no rethrow, no `Debug`
line. The library's standing rule is that an exception is never swallowed.

| file | line | what it swallows |
|---|---|---|
| `BeepFloatingBadge.cs` | 223 | anything a `BadgeOpened` handler throws |
| `BeepFloatingBadge.cs` | 252 | anything a `BadgeClosed` handler throws |
| `BeepFloatingBadge.cs` | 437 | anything a `BadgeClick` handler throws |
| `Builtin/BeepIconBadge.cs` | 62 | `StyledImagePainter.Paint` failing to render the SVG |
| `Builtin/BeepValidationBadge.cs` | 104 | the same, for the validation glyph |

**Status: done.** No bare `catch` remains in `Badges/`.

**The three event swallows are gone.** `BadgeOpened`, `BadgeClosed` and `BadgeClick` raise without a
catch: the exception belongs to the subscriber and is the one person who can fix it. Verified — a
throwing `BadgeClick` handler now reaches the caller.

**The two paint swallows needed more than deleting the catch, and the reason is the interesting part.**
`StyledImagePainter.Paint` **writes a `Debug` line and returns** when it cannot resolve an image — it
never throws. So the bare `catch { }` could not have caught anything: an unresolvable icon path was
already failing silently one layer down, and the badge rendered as a plain coloured shape.

The check written for this caught it. "A broken icon looks different from no icon" **failed** after the
first fix, because catching an exception that is never raised changes nothing.

The badge now asks `ImagePainter.HasImage` whether the path resolves, reports once per path, and draws
a visible missing-glyph cross. The `catch` stays for the case where resolution succeeds and rendering
still fails — letting that escape would be worse than swallowing, since a throwing paint handler leaves
the region invalid and the next `WM_PAINT` throws again.

## The three event swallows are the more defensible ones, and still wrong

```csharp
try { BadgeOpened?.Invoke(this, EventArgs.Empty); }
catch { }
```

The intent is guessable: a badge should not bring down the form because a consumer's `BadgeClick`
handler threw. But swallowing means the consumer's own bug is invisible — their click handler throws,
nothing happens, and there is no way to find out why. **The exception belongs to the subscriber, and
they are the one person who can fix it.**

Let them propagate. This is a UI event raised on the UI thread, exactly like `Control.Click`, which
does not catch either. A consumer who wants to be defensive can be, in their own handler.

## The two paint swallows hide a real failure

```csharp
try { StyledImagePainter.Paint(g, iconRect, _svgPath); }
catch { }
```

If the SVG path is wrong, missing, or malformed, the badge renders as a coloured shape with **no icon
and no indication anything went wrong**. For `BeepValidationBadge` that is worse than cosmetic: an
error badge that silently loses its glyph looks like a success badge in the wrong colour, and the
control's entire job is to communicate state at a glance.

These are inside `OnPaint`, which is the one place where letting an exception escape genuinely is
dangerous — a throwing paint handler in WinForms can loop, since the failed paint leaves the region
invalid and the next `WM_PAINT` throws again.

**So the fix here is not "remove the catch".** It is:

1. Catch the *specific* failure the painter can produce, not everything.
2. Report it once — through the library's existing reporting path, or at minimum a `Debug.WriteLine`
   that names the path that failed — and set a flag so it is reported once rather than every 16ms.
3. Render something that reads as broken, so the failure is visible without a debugger. A "missing
   glyph" mark beats a blank badge that looks deliberate.

This is the same conclusion the GridX work reached for painters: report rather than log, and never let
a paint failure look like a successful paint of nothing.

## Verification

1. **A throwing `BadgeClick` handler surfaces.** Subscribe a handler that throws, click, and assert the
   exception reaches the caller. *Today it vanishes.*
2. **A bad SVG path is reported.** Set `SvgPath` to something that cannot resolve; assert the failure is
   reported once — and that it is reported **once**, not once per paint.
3. **A bad SVG path does not loop.** Paint the badge repeatedly with an unresolvable path and assert it
   terminates. This is the check that stops the fix from being worse than the bug.
4. **Nothing in `Badges/` matches a bare `catch`.** A grep is a legitimate check here: the population is
   five, fully enumerated above.
