# Notifications — review & enhancement plan (2026-08)

10 files, ~5,100 lines (seventeenth folder). `BeepNotification : BeepiFormPro` (composed
toast form: icon/labels/progress/actions), `BeepNotificationGroup : BaseControl`
(expandable stack), `BeepNotificationHistory : BaseControl` (persisted list),
`BeepNotificationManager` (singleton queue/position/dedupe/schedule), sound player,
animator, focus detector. Pre-existing docs (MASTER_TODO/ENHANCEMENT_PLAN) superseded by
this tracker. Census: 21 flag mentions (child-panel noise + doc comments), 25 guards,
65 literals, 8 luminance, 16 catches (several are comment words), 8 Debug.WriteLine.

## Findings (static pass)

### F1 — the toast NEVER sees the theme: ApplyData passes null

`BeepNotification.ApplyData` calls `GetColorsForType(type, null, …)` — the literal
Tailwind-palette branch runs for every toast, in every app, under every theme. The
manager positions themed-looking cards that are actually hardcoded. Fix: pass
`BeepThemesManager.CurrentTheme` (toasts are short-lived; live re-theme mid-toast is
recorded as out of scope).

### F2 — NotificationThemeHelpers: derivation engine instead of slots

Guards + literal fallbacks + `Lighten`/`Darken` pastel derivations + a private HSL
engine + three `GetContrastColor` variants + `GetRelativeLuminance`/`ShiftLuminance` —
the contrast/HSL half has ZERO callers (rule 2). No Notification* slot family exists, so
the settled shape is the accepted idioms: card base = `SurfaceColor` for every type
(type identity carried by border/icon/veil), ink = `ForeColor`, border/icon = semantic
slots (Success/Warning/Error; Info → `AccentColor` — the Tabs lesson: PrimaryColor is
not always an accent; System → `BorderColor`/`SecondaryColor`), plus `GetTypeVeil` =
alpha veil of the type accent (the Group already hand-rolls exactly this at 12%). HC per
paint. Custom overrides Empty-passthrough.

### F3 — noise flags and inert child assignments

`UseThemeColors = true` stamped onto child BeepPanels (default is already true) and doc
comments referencing the flag. Swept.

### F4 — reporting: 8 Debug.WriteLine (History file ops, Sound failures) → BeepLog

### F5 — literal sweep remainder after F2 (audit per file)

### F6 — no probe

Planned (NoteProbe): a toast shown via the manager renders semantic types distinctly AND
themed (the null-theme proof: same type differs between ArcLinux and Zen — this check
fails before F1); dismiss removes it; Group renders + expands; History records shown
notifications. Composed forms: DrawToBitmap should capture children (no TransparencyKey
here); fall back to the ToolTips OnPaint capture if blank. Eyeball everything.

## Order

1. F1–F5 one batch — build + commit
2. F6 probe + eyeball — commit fixes

## Standing constraints

There is ALWAYS a theme — semantic slots direct, card surfaces from SurfaceColor, alpha
veils + WCAG picks are the only derivations, no flags/guards/HSL engines, HC per paint.
A check must be able to fail; renders get eyeballed. Commit to master only.

## Batch 2 — toast layout (sizing FIXED; nested chrome NOT fixed)

User report: "sizing and text and icon are all wrong; using borders in all controls is nonsense,
make some isframeless=true".

### Fixed: the message text was never drawn at all

`_messageLabel` was `Dock = Fill` inside an `AutoSize` form. A Fill-docked child contributes no
preferred height, so the form sat at `MinimumSize` (280x60) forever and the title's AutoSize height
consumed the entire text panel - the message was laid out at Y=33 inside a 33px-tall panel, i.e.
entirely outside it. Now both labels dock TOP with heights measured from their own fonts
(`TextRenderer.MeasureText` with `WordBreak`), `AutoSize` is off, and `RecomputeSize` sizes the card
from real content within Min/Max. The card grows 280x60 -> 280x88 and the message renders.

Also: children are `IsFrameless`/`IsChild`/`IsTransparentBackground`/`ControlStyle.None`, ink is
handed down from the resolved semantic colours, and the ctor's mojibake moon icon was replaced with
an escaped codepoint.

### NOT fixed: every inner label/panel still paints its own box, and the icon never renders

Five approaches were tried and all failed; the flags are all verifiably correct at runtime
(`IsFrameless=True IsTransparentBackground=True IsChild=True ControlStyle=None`, dumped from the
live toast), and every layer of the Beep paint chain honours them:
`ClearDrawingSurface` skips on `IsTransparentBackground`; `ClassicBaseControlPainter` computes
`shouldDrawBorders = !IsFrameless && ...`; `BeepStyling.PaintControl` skips the background painter
when transparent. A lone `BeepLabel` on a magenta panel DOES render transparent - so the mechanism
works in isolation and something in the nested/parent-cached context defeats it.

Diagnostics gathered (for whoever picks this up):
- theme slots differ per control type, which is why the boxes are visible at all:
  DefaultTheme `Surface`=245,245,245 but `LabelBackColor`=255,255,255; ZenTheme `Surface`=255 vs
  `PanelBackColor`=34,34,34. Assigning `child.BackColor` is futile - each child's own `ApplyTheme`
  re-resolves its slot, and the children subscribe to `ThemeChanged` independently of the toast.
- WinForms' own `OnPaintBackground` erases with `BackColor` before any Beep painting, which no
  Beep-level flag can suppress.

**Recommended fix (not attempted): draw the toast interior in ONE painter.** That is already the
pattern used by `BeepNotificationGroup` in this same folder, and by every painter in
`Widgets/`, `ProjectCards/` and `Wizards/`. Composing a 280x88 card out of five chrome-bearing
BaseControls and then trying to strip all their chrome from outside is the wrong shape; the
accessibility/tab-order benefit does not survive the visual cost. Keep the real `BeepButton` close
control, draw title/message/icon directly.

The icon is unverified in the same way - `_iconContainer` is 36px wide and `IconPicture_Paint`
calls `StyledImagePainter.PaintWithTint`, but nothing appears in any render. Not diagnosed.
