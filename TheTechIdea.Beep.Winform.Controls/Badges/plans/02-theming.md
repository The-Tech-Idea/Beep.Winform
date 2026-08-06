# Stage 02 — Badges cannot follow a theme

**Kind:** enhancement · **Files:** `BeepFloatingBadge.cs`, `Builtin/*.cs`

`BeepFloatingBadge` derives from **`UserControl`**, not `BaseControl`. Every other control in this
library derives from `BaseControl`, which subscribes to `BeepThemesManager.ThemeChanged` and re-applies
itself. A badge does not, so **a theme change leaves every badge on the form at its old colour.**

Measured: `new BeepDotBadge().BadgeBackColor` is the literal `Color.Red`.

**Status: done.** Verified: `ff2196f3 -> ff5294e2` across `DefaultTheme -> ArcLinuxTheme`.

**The decision: subscribe, do not re-parent.** `BeepFloatingBadge` stays a `UserControl` and subscribes
to `BeepThemesManager.ThemeChanged` directly. The defect was "badges do not follow the theme", not
"badges do not derive from `BaseControl`", and the subscription buys the whole benefit without pulling
a large base control's painting, hit-testing, hover and focus machinery into a 10-24px decoration that
is `TabStop = false`. Re-parenting stays available if something later needs it.

**A new `BadgeRole` decides which theme slot a badge takes** — `Default`, `Accent`, `Surface`, and the
four states. The built-ins now declare a role instead of assigning a colour.

**An explicit colour still wins.** Four `…Explicit` flags record when a caller set a colour, so
`ApplyTheme` never overwrites a deliberate choice. Verified: a badge pinned to `Magenta` is still
`Magenta` after a theme change.

**The validation colours moved to semantic slots** — `theme.ErrorColor`, `SuccessColor`, `WarningColor`
— rather than the literal ARGB they used to hold, so they follow a dark or high-contrast palette while
keeping their meaning. Verified error and success stay distinguishable, and error stays warm
(`R=211 B=47`) rather than by asserting an exact value that any palette change would break.

**`BadgeFont` now exists on the badge and is honoured**, and `SyncBadgeAppearance` passes it across —
cloned, since the badge disposes what it is given. `"Segoe UI"` is gone; the fallback is
`SystemFonts.DefaultFont.FontFamily`, which is installed everywhere and correct per locale.

**The colour fields have no initialisers at all now.** Literal defaults were dead on arrival —
`ApplyTheme` runs from the constructor — and they misrepresented where a badge's colours come from. If
no theme is registered, the fallback is `SystemColors`, which follows the OS's own light/dark and
high-contrast settings rather than being a brand literal.

## The literal colours

Nineteen across the folder:

| colour | where |
|---|---|
| `Color.White` ×7 | default `BorderColor`, dot/text/counter fore colours, icon badge back |
| `Color.Red` ×4 | dot, counter, notification defaults |
| `Color.DodgerBlue` | text badge default |
| `Color.Gray` | validation `None` |
| `Color.Black` | icon badge fore |
| `Color.FromArgb(80,0,0,0)` | shadow |
| `Color.FromArgb(220,60,60)` / `(40,167,69)` / `(255,152,0)` / `(33,150,243)` | validation error / success / warning / info |

**The four validation colours are the exception this library allows and should stay expressible.** An
error badge is red because red means error, not because red is the current accent. The right form is
for them to resolve from the theme's semantic slots — `ErrorColor`, `SuccessColor`, `WarningColor`,
`InfoColor` — rather than from literals, so they follow a high-contrast or dark theme while keeping
their meaning. `BaseControl` already exposes `ErrorColor`; `BaseControl.ShowValidation` already uses
`ErrorColor` for the error case and then **hard-codes `Color.FromArgb(40,167,69)` for success** — the
same split, in the consumer.

Everything else — the dot's red, the text badge's blue, the border's white, the shadow — is style, and
should come from the theme by not being assigned.

## The decision this stage has to make first

**Re-parenting `BeepFloatingBadge` onto `BaseControl` is the obvious move and it is not free.** Read
before choosing:

- `BaseControl` is a large base with its own painting, hit-testing, hover and focus machinery. A badge
  is a 10–24px decoration that is `TabStop = false` and mostly not interactive. Inheriting all of it to
  get `ThemeChanged` may cost more than it returns — the Cards measurement put a themed Beep control at
  0.11 ms to construct, which is fine for one badge and worth knowing for a grid of fifty.
- `BeepFloatingBadge` sets `SupportsTransparentBackColor` and `BackColor = Color.Transparent`, and
  relies on being z-ordered *behind* its target on the parent's surface. Whatever `BaseControl` does
  with background painting has to not fight that.

**The cheaper alternative is to subscribe to `BeepThemesManager.ThemeChanged` directly** and resolve
the badge's colours from the current theme, without changing the base class. That gets the actual
benefit — badges follow the theme — at a fraction of the risk.

**Recommendation: do the cheap one, and prove the benefit before considering the re-parent.** The
defect is "badges do not follow the theme", not "badges do not derive from `BaseControl`". If the
subscription version passes the verification below, the re-parent buys nothing this stage needs.

Whichever is chosen, record which and why — a later reader will ask.

## `BadgeFont` is the same defect in a different place

`BaseControl.BadgeFont` is a public property, written, disposed on teardown, and **never read**.
`SyncBadgeAppearance` copies `BadgeBackColor` and `BadgeForeColor` to the badge and not the font, and
no badge exposes a font property to copy it to: `BeepTextBadge` and `BeepCounterBadge` each construct
`new Font("Segoe UI", fontSize, FontStyle.Bold)` inline.

So a caller can set `BadgeFont`, see it stick on the getter, and never see it render. Fixing this means
a font property on the badge and a line in `SyncBadgeAppearance` — see [05](05-dead-surface.md), which
tracks it as dead surface.

Hard-coding `"Segoe UI"` is also a DPI and localisation problem: a font family that may not exist, at a
size derived from pixel height rather than from the theme's type scale.

## Verification

1. **Change the theme with a badge on screen; its rendered pixels change.** Capture the badge with
   `DrawToBitmap` before and after `BeepThemesManager` switches theme. *Today the two are identical* —
   that is the failing run.
2. **A validation badge keeps its meaning across themes.** Error stays red-ish, success stays green-ish
   in both a light and a dark theme; assert hue, not an exact ARGB, or the check breaks on any palette
   change.
3. **No literal colour remains outside the semantic four.** A grep over `Badges/` is a legitimate check
   here because the population is small and fully enumerated in the table above.
4. **`BadgeFont` reaches the glyphs.** Set it to a distinctly different size; assert the rendered
   bitmap changes. *Today it cannot* — there is nothing to set.
