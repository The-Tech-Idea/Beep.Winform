# BeepLabel

A rich label control (`BeepLabel : BaseControl`) supporting text with an optional
leading/trailing image, title + sub-header, a range of visual shapes (rounded,
pill, circle, badge, chip, code, blockquote) and effects (accent bar, glow,
raised, sunken, gradient, shimmer), severity tinting, auto-contrast text, and
accessibility snapshots.

## Structure

| File | Responsibility |
|---|---|
| `BeepLabel.cs` | Control shell: fields, image resolution, theming, accessibility |
| `BeepLabel.Styling.cs` | Shape/effect rendering, variant overlays, auto-contrast |
| `Painters/BeepLabelPainter.cs` | Stateless paint orchestration + shared brush cache |
| `Helpers/BeepLabelLayoutHelper.cs` | Text layout/format flags |
| `Helpers/BeepLabelFontHelpers.cs` | Title/sub-header fonts |
| `Helpers/BeepLabelAccessibilityHelpers.cs` | UIA/accessibility snapshot |
| `Models/*` | `BeepLabelState`, `BeepLabelLayoutContext`, `LabelStyleConfig`, `LabelColorConfig` |

Rendering flows through `BeepLabelPainter.Paint`, which composes the shape
background, effect overlay, image (via `StyledImagePainter`), and text. Fonts come
from `BeepThemesManager.ToFont`; colours from the theme's `Label*` slots.

## Recent Fixes (2026-08)

- **Sizing now measures with the same fonts painting uses** — `GetPreferredSize` and
  `UpdateMinimumSize` measured the subheader with the raw `SubHeaderFont` property (and the
  header with `_textFont ?? Font`) instead of the resolved fonts the paint path uses
  (`BeepLabelFontHelpers.GetHeaderFont` / `GetSubHeaderFont`). A label with a themed
  subheader therefore measured at one font and drew at another, so auto-sized labels
  clipped or reserved the wrong height. Both measure and paint now resolve fonts identically.
- **Image-text gap added** — `BeepLabelLayoutHelper.CalculateLayout` placed the image and text
  flush (0 px gap) for `ImageBeforeText` / `TextBeforeImage` / `ImageAboveText` /
  `TextAboveImage`. There is now a DPI-scaled 4 px gap, and `GetPreferredSize` accounts for it
  via `Rectangle.Union`.
- **Swallowed image-resolution failure now reports** — the bare `catch` around
  `ImagePainter` in the image-size resolver (which silently fell back to the max
  size) now reports through `BeepLog.Fallback` with the image path.
- **GDI leak in the accent-bar effect** — `DrawStyleEffect`'s `AccentBar` case
  passed a `new SolidBrush(accent)` straight to `FillRectangle` without disposing
  it; it is now wrapped in `using`.

### Known minor items (not changed)

- `Color.FromArgb(33, 150, 243)` (Material Blue 500) is repeated three times in
  `BeepLabel.Styling.cs` as the fallback for `_currentTheme?.PrimaryColor` /
  `_currentTheme?.AccentColor`. It only fires during the brief pre-theme window;
  resolving it through `BeepThemesManager.CurrentTheme` (the "there is always a
  theme" pattern used elsewhere) would remove the literals entirely.
