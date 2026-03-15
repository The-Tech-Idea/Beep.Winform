# BeepChevronButton

Custom chevron-shaped button control for WinForms.

## Architecture contract

- `BeepChevronButton` owns its silhouette painting (chevron path fill/border/ripple clip).
- `BaseControl` provides shared infrastructure (theme/state plumbing, events, accessibility, DPI helpers).
- A canonical chevron path function is reused for:
  - drawing,
  - ripple clipping,
  - control `Region`.

## Direction support

- `Forward`
- `Backward`
- `Up`
- `Down`

## Key behavior properties

- `IsCheckable` — when true, click toggles `IsChecked`.
- `IsChecked` — current toggle state.
- `ShowRipple` — enables click ripple animation.
- `ReducedMotion` — shortens ripple timing and lowers motion.
- `AllowArrowDirectionCycle` — arrow keys can change direction while focused.
- `FocusVisibleOnly` — focus ring is shown only for keyboard focus.

## Accessibility defaults

- `AccessibleRole` is `PushButton` by default, and switches to `CheckButton` when checkable behavior is active.
- Minimum hit size is enforced with DPI scaling.
- Keyboard activation supports Enter/Space.

## Theming

Uses `IBeepTheme` button colors:

- `ButtonBackColor`, `ButtonHoverBackColor`, `ButtonPressedBackColor`
- `DisabledBackColor`, `DisabledForeColor`
- `PrimaryTextColor`, `ButtonSelectedForeColor` (focus ring)

## Notes

- Keep image rendering path-based and use `ImagePath` with `StyledImagePainter`.
- Avoid mixing BaseControl chrome painting with custom chevron chrome in the same control.
