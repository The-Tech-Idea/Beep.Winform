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
- Projects target .NET 8 or .NET 9. Prefer C# 12. Match each project�s TFM when adding code.
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
- Implement both `Paint` and `UpdateHitAreas` and register in the owner�s registry.
- Reuse existing parameter keys where possible; document any new keys here.
- Provide sample defaults in the owner constructor so it previews at design-time.

---

## BeepSideBar Painter Architecture (CRITICAL REQUIREMENTS)

**Painter Pattern for SideBar:**
- Interface: `ISideBarPainter` with methods: `Paint()`, `PaintToggleButton()`, `PaintSelection()`, `PaintHover()`, `PaintMenuItem()`, `PaintChildItem()`
- Context: `ISideBarPainterContext` provides all state (Graphics, Bounds, Theme, UseThemeColors, Items, ExpandedState, etc.)
- Base class: `BaseSideBarPainter` provides helper methods (`CreateRoundedPath`, `GetEffectiveColor`)

**MANDATORY REQUIREMENTS FOR ALL SIDEBAR PAINTERS:**
1. **ImagePainter Usage**: MUST use `static readonly ImagePainter _imagePainter = new ImagePainter();` for ALL icons
   ```csharp
   _imagePainter.ImagePath = item.ImagePath;
   if (context.Theme != null && context.UseThemeColors) {
       _imagePainter.CurrentTheme = context.Theme;
       _imagePainter.ApplyThemeOnImage = true;
       _imagePainter.ImageEmbededin = ImageEmbededin.SideBar;
   }
   _imagePainter.DrawImage(g, iconRect);
   ```

2. **UseThemeColors Check**: MUST check `context.UseThemeColors` for EVERY color decision
   ```csharp
   Color someColor = context.UseThemeColors && context.Theme != null
       ? context.Theme.ThemeColor      // Use theme
       : Color.FromArgb(...);           // Design system fallback
   ```

3. **Custom Drawing**: Each painter MUST draw everything itself with custom code
   - NO calls to `base.PaintMenuItem()` or `base.PaintChildItem()`
   - Draw icons using `_imagePainter.DrawImage()` (NOT base methods)
   - Draw text with custom fonts and colors
   - Draw selection indicators, hover effects, expand/collapse icons
   - Draw connector lines for child items

4. **Distinct Visual Appearance**: Each painter MUST have unique visual style
   - Custom colors matching design system (Material, Fluent, iOS, etc.)
   - Custom fonts (Segoe UI, Roboto, SF Pro, etc.)
   - Custom border radius (0px to 28px)
   - Custom selection style (background, left accent, pill, etc.)
   - Custom hover effects
   - Custom spacing/padding

**16 Design Systems:**
iOS15, Material3, Fluent2, Minimal, AntDesign, MaterialYou, Windows11Mica, MacOSBigSur, ChakraUI, TailwindCard, NotionMinimal, VercelClean, StripeDashboard, DarkGlow, DiscordStyle, GradientModern

**Reference Implementation:**
- `iOS15SideBarPainter.cs` is the DEFINITIVE reference showing correct pattern
- ~100-150 lines per painter
- Structure: Paint() → PaintMenuItems() → PaintChildItems() (recursive)

**Partial Class Structure:**
- `BeepSideBar.cs` - Main file (properties only)
- `BeepSideBar.Painters.cs` - Style property, InitializePainter() with 16-case switch
- `BeepSideBar.Drawing.cs` - OnPaint(), layout, hit testing
- `BeepSideBar.Events.cs` - Mouse/keyboard handlers
- `BeepSideBar.Helpers.cs` - Accordion state management
- `BeepSideBar.Animation.cs` - Smooth collapse/expand animation
- `BeepSideBar.Accordion.cs` - Menu expansion logic

**Testing Checklist Per Painter:**
- [ ] Uses static readonly ImagePainter
- [ ] Checks UseThemeColors for ALL colors (background, text, icons, borders, hover, selection, connectors)
- [ ] NO calls to base.PaintMenuItem/PaintChildItem
- [ ] Visually distinct from other painters
- [ ] Icons render with ImagePainter.DrawImage()
- [ ] Text renders with custom fonts
- [ ] Selection indicator works
- [ ] Hover effect works
- [ ] Expand/collapse icons work
- [ ] Child items with indentation and connector lines
