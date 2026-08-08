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
