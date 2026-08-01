# 11 — Theming & Style Parity

**Priority P2.**

## Current behaviour

Theming is wired more carefully here than in some other areas:

- `ToolTipManager` subscribes to `BeepThemesManager.ThemeChanged` and repaints live tooltips, with a
  comment noting a previous placeholder `try/catch` silently swallowed this.
- `ToolTipConfig.UseBeepThemeColors` (default `true`) selects theme colours over explicit ones.
- `ToolTipConfig.Style` is a `BeepControlStyle` (Material3, iOS15, Fluent2, …), separate from
  `ToolTipType`, which is semantic (Success, Warning, Error…).
- `ToolTipStyleHelpers` supplies per-style recommended arrow size and offset.

Gaps:

1. **`ToolTipType` has 21 values.** Default, Primary, Secondary, Accent, Success, Warning, Error,
   Info, Help, Validation, Interactive, Descriptive, Notification, Tutorial, Shortcut, Badge,
   Preview, ContextMenu, Status, Hint, Custom. Confirm each resolves to a distinct, theme-derived
   colour pair — an enum this wide is usually aspirational, and the same audit on `BeepTree` found
   four style groups rendering identically.
2. **`Style` defaults to `Material3` in three places** (`ToolTipConfig.Style`,
   `ToolTipManager.DefaultStyle`, `DefaultControlStyle`) rather than following the active Beep theme.
   A Fluent-themed application gets Material tooltips unless every call site overrides.
3. **Contrast is not enforced per type.** A `Warning` tooltip on a light theme can end up with amber
   text on a near-white surface. `ColorUtils.EnsureReadable` exists in this repo for exactly this.
4. **Glass painter and theme colours.** `GlassToolTipPainter` uses translucency; verify it derives
   from theme colours rather than hard-coded whites, and that it degrades in high contrast
   (see [09](09-accessibility.md)).

## What the reference systems do

- Semantic *intent* (info/success/warning/error) and visual *style* (Material/Fluent/iOS) are
  orthogonal — which this design already gets right, and is worth preserving.
- Colour pairs are derived from theme tokens, not literals, so a new theme automatically styles
  every tooltip type.
- The default style follows the app theme rather than being pinned to one design language.

## Work

1. **Audit all 21 `ToolTipType` values** for distinct, theme-derived colour resolution. Render a
   contact sheet of all 21 across at least three themes (a light, a dark, and a hostile one such as
   `MaterialYouTheme`, whose near-identical colour pairs exposed real bugs in the grid).
2. **Default `Style` from the active theme** instead of the `Material3` literal, with the literal
   remaining an explicit opt-in.
3. **Run resolved colours through `ColorUtils.EnsureReadable`** so a theme that pairs a foreground
   too closely with its surface still yields legible tooltips.
4. **Verify live theme switching** actually repaints — the subscription exists, but confirm the
   painters re-resolve colours rather than caching them from first paint. Cached brushes are exactly
   how the grid toolbar ended up with a stale-palette bug.
5. **Per-style arrow/offset**: confirm `ToolTipStyleHelpers` covers every `BeepControlStyle` and does
   not silently fall back to one default for most of them.

## Verification

- Contact sheet: 21 types × 3 themes, rendered and reviewed. Assert no two types in the same theme
  produce identical colour pairs unless intended.
- Assert every resolved pair meets `MinContrastRatio`.
- Switch theme while five tooltips are visible; assert all five repaint with the new palette.
- Render each `BeepControlStyle` and confirm arrow size/offset visibly differ per design language.
