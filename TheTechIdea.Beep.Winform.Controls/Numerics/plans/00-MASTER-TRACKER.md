# Numerics — review & enhancement plan (2026-08)

19 files, ~4,400 lines (sixteenth folder). `BeepNumericUpDown : BaseControl` (5 partials,
hosted TextBox + spinner buttons, mask presets, 6 painters behind
`INumericUpDownPainter` keyed by NumericStyle: Standard/Currency/Percentage/Phone/
CompactStepper/InlineStepper) + `BeepDualPercentageControl` (composed two-section
percentage display). Consumed by GridX editors and Filtering — a live editing control.
Census: 42 flags, 33 guards, 33 literals, 4 luminance, 0 catches, 0 reflection.

## Findings (static pass)

### F1 — NumericThemeHelpers: the anti-pattern over the right families

The control correctly borrows the TextBox* family (it IS a text input) but wraps it in
useThemeColors/Empty-guards/literals and derives button/pressed/hover fills with
`ShiftLuminance` when the Button* family carries exactly those slots. Rewrite slot-direct
flag-less: surface → `TextBoxBackColor`/`TextBoxHoverBackColor`/`TextBoxSelectedBackColor`,
ink → `TextBoxForeColor` (+Hover/Selected), border → `TextBoxBorderColor` (+Hover/
Selected, focus falls to the Selected slot), disabled → `DisabledBackColor`/
`DisabledForeColor`, spinner buttons → `ButtonBackColor`/`ButtonHoverBackColor`/
`ButtonSelectedBackColor` + `ButtonForeColor` variants, error → `ErrorColor`. HC per
paint. `GetNumericColors` 6-tuple kept (callers exist), flag-less.

### F2 — flag web: control property + painter interface + icon helpers

`UseThemeColors` local property on BeepNumericUpDown (ApplyTheme calls, 10 sites),
`useThemeColors` params threaded through StandardNumericPainter and NumericIconHelpers.
All deleted; ApplyTheme keeps its surface-stamping shape (the BaseControl norm) but
flag-less. Painter `context.Theme?.Slot ?? literal` guards → slot-direct (the context
always carries a theme).

### F3 — BeepDualPercentageControl literals

Hand-set `Color.White` backgrounds/label inks, `LightGray` borders/divider, and a
`?? Color.FromArgb(144,238,144)` fallback beside an already-correct Empty-passthrough
`LeftSectionColor`. Slots: surface → TextBoxBackColor/SurfaceColor, borders/divider →
BorderColor, section defaults → SuccessColor/ErrorColor families, label inks →
theme ink slots.

### F4 — literal sweep remainder + `_invalidInputColor` literal init (resolve from ErrorColor)

### F5 — no probe

Planned (NumProbe): renders with value text; spin up/down via button hit-areas changes
Value + raises ValueChanged; keyboard up/down; Min/Max clamping; SetValue/GetValue round
trip (the GridX editor contract); focused border differs from unfocused; live theme
change re-renders; 2–3 painter styles distinct; DualPercentage renders both sections.
Eyeball everything.

## Order

1. F1–F4 one batch — build + commit
2. F5 probe + eyeball — commit fixes

## Standing constraints

There is ALWAYS a theme — slot per role from the borrowed TextBox*/Button* families, no
flags/guards/luminance, HC per paint, customs as Empty-passthrough where they exist. A
check must be able to fail; renders get eyeballed. Commit to master only.
