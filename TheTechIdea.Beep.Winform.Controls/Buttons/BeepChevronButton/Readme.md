# BeepChevronButton

Custom chevron-shaped button control for WinForms.

## Architecture contract

- `BeepChevronButton` owns its silhouette painting (chevron path fill/border/ripple clip).
- `BaseControl` provides shared infrastructure (theme/state plumbing, events, accessibility, DPI helpers).
- A canonical chevron path function is reused for:
  - drawing,
  - ripple clipping,
  - control `Region`.
- Paint flow is split into focused helpers:
  - `DrawChevronSurface`
  - `DrawStateOverlay`
  - `DrawFocusRing`
  - `DrawRipple`
  - `DrawImageContent`
  - `DrawTextContent`

## Direction support

- `Forward`
- `Backward`
- `Up`
- `Down`

## Variant and size support

- `Variant` (`ChevronButtonVariant`)
  - `Filled` — normal filled chevron
  - `Outlined` — transparent fill, emphasized border
  - `Tonal` — blended/tinted surface
  - `Text` — transparent fill and no border

- `ButtonSize` (`ChevronButtonSize`)
  - `Small`
  - `Medium`
  - `Large`

- Size presets are DPI-scaled and applied to minimum width/height.

## Key behavior properties

- `IsCheckable` — when true, click toggles `IsChecked`.
- `IsChecked` — current toggle state.
- `ShowRipple` — enables click ripple animation.
- `ReducedMotion` — shortens ripple timing and lowers motion.
- `AllowArrowDirectionCycle` — arrow keys can change direction while focused.
- `FocusVisibleOnly` — focus ring is shown only for keyboard focus.
- `EnforceMinimumHitSize` — enforces minimum touch target (48dp equivalent).

## Rendering APIs

- `Variant` (`ChevronButtonVariant`)
  - `Filled`
  - `Outlined`
  - `Tonal`
  - `Text`

- `ButtonSize` (`ChevronButtonSize`)
  - `Small`
  - `Medium`
  - `Large`

## Accessibility defaults

- `AccessibleRole` is `PushButton` by default, and switches to `CheckButton` when checkable behavior is active.
- Minimum hit size is enforced with DPI scaling.
- Keyboard activation supports Enter/Space.

## Theming

Uses `IBeepTheme` button colors:

- `ButtonBackColor`, `ButtonHoverBackColor`, `ButtonPressedBackColor`
- `DisabledBackColor`, `DisabledForeColor`
- `PrimaryTextColor`, `ButtonSelectedForeColor` (focus ring)

Variant color behavior is resolved from these theme colors at paint time.

## Notes

- Keep image rendering path-based and use `ImagePath` with `StyledImagePainter`.
- Avoid mixing BaseControl chrome painting with custom chevron chrome in the same control.
