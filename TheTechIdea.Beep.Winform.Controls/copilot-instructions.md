@Copilot instructions

[]: # Use the following instructions to complete the Winform control and its look and feel:
[]: # 1. Ensure the control is compatible with the latest WinForms framework.
[]: # 2. Implement a modern look and feel using appropriate styles and themes.
[]: # 3. Use custom rendering techniques to enhance the visual appearance of the control.
[]: # 4. Maintain consistency with modern UI design principles.
[]: # 5. Include properties for customization, such as colors, fonts, and sizes.
[]: # 6. Ensure the control is responsive and adapts to different screen resolutions.
[]: # 7. Test the control in various scenarios to ensure it behaves as expected.
[]: # 8. Document any specific design choices or features implemented in the control.
[]: # 9. Follow best practices for WinForms development to ensure performance and maintainability.
[]: # 10. Consider accessibility features to make the control usable for all users.
[]: # 11. Use event handlers to provide interactivity and respond to user actions.
[]: # 12. Ensure the control integrates well with other components in the application.
[]: # 13. Provide examples or documentation on how to use the control effectively.
[]: # 14. Review the control for any potential improvements or refactoring opportunities.

---

## Additional instructions (append-only; keep existing content intact)

Purpose
- Extend the house rules for Beep.Winform controls so future contributions follow the same architecture and UX patterns.

Workspace rules
- Do not run a solution-wide build unless explicitly requested. Implement and validate file-level changes only.
- Projects target .NET 8 or .NET 9. Prefer C# 12. Match each project’s TFM when adding code.
- Use partial classes to separate Core/Registry/Painters/Helpers.

BaseControl painting pipeline
- Derive from `BaseControl`. Use `DrawingRect` as the content area; background, borders, shadows are handled by `ControlPaintHelper`.
- Implement visuals in `DrawContent(Graphics g)` only. Do not add child WinForms controls for layout; paint text/shapes directly.
- Apply theming via `IBeepTheme` tokens (e.g., `CardBackColor`, `CardTextForeColor`, `CardHeaderStyle`, `CardparagraphStyle`, `SmallText`). Use `BeepThemesManager.ToFont(style)` for fonts.

Interaction (Widgets pattern)
- Register interactive regions using the built-in hit-test wrappers from `BaseControl`:
  - `ClearHitList()` before re-adding.
  - `AddHitArea(name, Rectangle rect, this, action)` for each clickable region.
- Provide hover/pressed feedback using the external drawing helper:
  - `AddChildExternalDrawing(this, DrawHoverPressedOverlay, DrawingLayer.AfterAll)",
  - Track `_hoverArea`/`_pressedArea` and invalidate overlays on state changes,
  - Use theme hover colors (`ProgressBarHoverBackColor`, `ProgressBarHoverForeColor`, `ProgressBarHoverBorderColor`) with safe fallbacks.

Painter pattern (cards/widgets)
- Define an interface with both paint and hit-area collection:
  - `void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, <Owner> owner, IReadOnlyDictionary<string, object> parameters);`
  - `void UpdateHitAreas(<Owner> owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, Action<string, Rectangle> register);`
- Maintain a registry in the owner mapping enum kind -> painter instance. Provide `PainterKind` property and a `Parameters` bag (`Dictionary<string,object>`).
- Keep owner convenience properties (e.g., `Title`, `Progress`) and sync them into `Parameters` before painting.

BeepStatCard specifics
- Painters: `SimpleKpi`, `HeartRate`, `EnergyActivity`, `Performance`.
- Parameter keys: `Header`, `Value`, `Delta`, `Info`, `Labels`, `Series`, `Series2`, `Spark`, `Min`, `Max`, `Unit`, `PrimaryColor`, `SecondaryColor`.
- Default painter: `SimpleKpi`.

BeepProjectCard specifics
- Enum kinds: `CompactProgress`, `DarkTile`, `RichCourse`, `ListKanban`, `AvatarTile`, `TeamAvatars`, `OutlineMeta`, `CalendarStripe`, `PillBadges`.
- Parameter keys: `Title`, `Subtitle`, `Description`, `Progress`, `Tags`, `Assignees`, `Status`, `DaysLeft`, `Icon`, `AccentColor`.
- Events exposed by the owner for named areas:
  - `TitleClicked`, `ProgressClicked`.
  - `PillClicked(string tag)` via `Pill:<tag>` area names.
  - `AvatarClicked(int index)` via `Avatar:<index>` area names.
  - Painter-specific: `StripeClicked`, `ContentClicked`, `OutlineClicked`, `CardBodyClicked`.
- Painters must register intuitive areas in `UpdateHitAreas`:
  - CompactProgress: `Title`, `ProgressBar`.
  - DarkTile: `Title`, `ProgressLine`.
  - RichCourse: `ProgressChip`, `ProgressBar`.
  - ListKanban: `CardBody`, `ProgressBar`.
  - AvatarTile: `Avatar`, `Title`.
  - TeamAvatars: `Title`, `ProgressBar`.
  - OutlineMeta: `Outline`, `ProgressBar`.
  - CalendarStripe: `Stripe`, `Content`, `ProgressBar`.
  - PillBadges: `Pill:<tag>` for each tag, `ProgressBar`.

Coding guidelines
- Dispose GDI objects (`using` for `Pen`, `Brush`, `Font`). Prefer `TextRenderer` for sharp text.
- Avoid allocations in the paint hot path; precompute where feasible.
- Keep APIs additive and backward compatible. Provide design-time defaults for preview.

Testing checklist
- Visual: light/dark themes, resize behavior, text truncation, progress bars alignment.
- Interaction: hover/pressed overlays appear on the correct areas; events fire as expected.
- Theming: `ApplyTheme()` updates colors/fonts consistently.

When adding a new painter
- Implement both `Paint` and `UpdateHitAreas` and register in the owner’s registry.
- Reuse existing parameter keys where possible; document any new keys here.
- Provide sample defaults in the owner constructor so it previews at design-time.
