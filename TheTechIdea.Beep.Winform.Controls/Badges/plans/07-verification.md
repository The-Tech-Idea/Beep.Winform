# Stage 07 — The harness

**Kind:** verification · **Status: done.** **27 checks, 0 failures.**

Every stage now has a check that fails without its fix, including the four that had only failing
assertions when this was written. The two items still marked unverified below — the `BringToFront`
paint amplification and the static dictionary's lifetime — remain unverified and are named as such.

There were no tests for this folder. The harness is a probe that constructs badges, attaches them to a
real parented control on a shown form, and asserts against measured facts.

## What it asserts

**27 checks across the six stages, all passing.** Each was seen to fail before its fix landed.

| stage | checks |
|---|---|
| 01 | identical render across 3 repaints with no shadow; a detached badge is collectable |
| 02 | colour changes across `DefaultTheme -> ArcLinuxTheme`; an explicit colour survives; error and success stay distinguishable; error stays warm; no literal colour assignments remain |
| 03 | a pill is wider than tall; text is not clipped; a long label is not capped at 48px; a circle stays square; a pill counter grows; `MiddleLeft` stays centred and overhangs the left edge |
| 04 | the pulse is visible; it does not resize the control; hiding stops the timer; showing resumes it; disposing stops it |
| 05 | the signature discriminates shapes; `Custom` falls back to `Rectangle`; `Custom` with a provider differs; `BadgeFont` changes the glyphs; a factory badge is configurable through `IBeepBadge` |
| 06 | a throwing `BadgeClick` surfaces; a bad icon path does not loop; a broken icon looks different from no icon |

Two are worth singling out because they exist to catch a *fix* going wrong rather than the original
defect: "an explicit colour survives a theme change" guards against `ApplyTheme` stomping a caller's
decision, and "a circle stays square" guards against the sizing fix making every shape grow.

## Every check is guarded against being blind

The shape comparison asserts that **`Rectangle` and `Circle` render differently** before concluding
anything from "`Custom` == `Rectangle`". Without that guard, a signature function that returned a
constant would report `Custom` as broken and every other shape as fine.

## Two checks were wrong before they were right

Worth recording, because both failed *silently* in the direction that looks like a result.

**Counting `Resize` subscribers by reflection reported 0 while the badge was attached.** The reflection
against `Control.EventResize` found nothing, so the check returned `0 → 0 → 0` and printed PASS. It
would have printed PASS whether or not the leak existed — it could not fail for the reason it was
written. Replaced with a reachability test.

**The reachability test then reported "leaked" after the fix.** The badge was created, attached,
detached and disposed inside a block in `Main`, and a Debug-build local stays rooted to the end of its
method — so the badge was reachable through the stack frame, not through the leak. Moving it into a
`[MethodImpl(NoInlining)]` helper that returns only the `WeakReference` fixed it.

**A third was wrong and got caught by its own guard.** The `BadgeShape.Custom` check first parented
both badges at their default `Location` of `(0,0)`, where they overlapped, and reported "differs" — the
answer that would have closed the finding as a non-issue. Rendering them standalone and unparented gave
the real answer: identical.

Three instrument failures in six checks. **The instrument is wrong at least as often as the code**, and
a result that closes a finding deserves the same suspicion as one that opens it.

## What is not measured yet

Named so a later reader knows the difference between "checked and fine" and "not checked".

- **`BeepBadgeManager` hooks `parent.Paint` and calls `BringToFront` on every badge on every paint.**
  `BringToFront` reorders the parent's child collection, which invalidates. Whether that re-enters
  painting, and at what cost with several badges on one parent, is **unverified** — an edit adding the
  check did not land and the result was never obtained. It is a plausible amplification and nothing
  more until measured. Do not repeat it as a finding.
- **`BeepBadgeManager` keys a `static` dictionary on `Control`.** Entries are removed on the parent's
  `Disposed`, so a parent that is never disposed stays in the map for the life of the process. Not
  measured; the shape of the risk is worth stating.
- **Thread safety of that static dictionary** — untested, and badges are UI-thread-only in practice.

## How to run it

The probe lives in the session scratchpad and references the control project directly. It exits
non-zero when any check fails, so it works as a gate. It uses `Control.DrawToBitmap` and never reads
the screen — screen capture returns whatever window is foreground, which cost the Cards program three
wrong answers.
