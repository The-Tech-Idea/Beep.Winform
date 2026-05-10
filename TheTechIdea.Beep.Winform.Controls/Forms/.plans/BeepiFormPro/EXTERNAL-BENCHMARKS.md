# External Benchmarks For BeepiFormPro

This note captures implementation ideas and product practices observed in mature WinForms UI libraries and skins.
It is intended to feed the BeepiFormPro phase plan, not replace it.

## Reference Sources Reviewed

### Open Source

- Krypton Toolkit
  - Custom chrome base form, palette and renderer separation, explicit non-client redraw and recalc hooks, palette change events, and designer-visible palette storage.
- MaterialSkin
  - Central skin manager, managed-form registration, simple color scheme object, dedicated animation manager, consistent hover and press states, and form-level custom button painting.

### Commercial Suites

- DevExpress WinForms
  - Large theme catalog, runtime skin switching, skin/theme editor, DirectX rendering path, HTML and CSS templating, accessibility support, localization, and demo templates.
- Telerik UI for WinForms
  - Consistent API, theme tooling, high-DPI and touch emphasis, shaped and ribbon forms, visual style builder, control spy and demo apps, accessibility and UI automation support.
- Syncfusion WinForms
  - Theme Studio, accessibility and automation positioning, localization, strong designer integration, and broad demo coverage around themed forms.

## Best Practices Worth Adopting

### 1. Separate Theme Tokens From Renderers

Observed in Krypton and commercial suites.

What they do:

- Keep a theme or palette object as the source of truth.
- Resolve a renderer from that palette instead of letting each control own ad hoc theme state.
- Raise explicit events for repaint-only changes vs layout-affecting changes.

What BeepiFormPro can learn:

- Split color and metric data from painter implementation objects.
- Replace broad cache clears with targeted theme-version or palette-version invalidation.
- Add a clear event contract for repaint, relayout, and chrome recomposition.

### 2. Provide Explicit Non-Client Recalc And Redraw Hooks

Observed strongly in Krypton.

What they do:

- Expose separate methods for non-client redraw and non-client recalculation.
- Handle active-window changes and palette changes explicitly.
- Keep non-client invalidation disciplined instead of relying only on generic `Invalidate()`.

What BeepiFormPro can learn:

- Add explicit helpers for chrome redraw vs full geometry resync.
- Trigger those helpers from maximize, restore, DPI change, palette change, and active-state change.
- Stop coupling all repaint correctness to resize-end only.

### 3. Use A Managed Theme Registration Model

Observed in MaterialSkin and Krypton.

What they do:

- Register forms with a central theme manager.
- Apply theme changes through a managed list of live surfaces.
- Unhook and rehook theme events cleanly when the active palette changes.

What BeepiFormPro can learn:

- Move from loosely coupled global theme callbacks toward a clearer registration and disposal model.
- Track theme subscribers and child-style propagation centrally.
- Make event subscription and unsubscription part of the lifecycle contract.

### 4. Build A Dedicated Animation Layer

Observed in MaterialSkin and commercial suites.

What they do:

- Use a small animation manager with explicit timing, easing, and progress callbacks.
- Reuse the same animation primitives across controls.
- Keep painting code consuming animation state instead of owning animation logic directly.

What BeepiFormPro can learn:

- Introduce a shared caption and chrome animation manager for hover, press, focus glow, reveal, and backdrop transition effects.
- Keep painter animation opt-in, but drive it through a common timing system.
- Reduce per-painter animation duplication.

### 5. Distinguish Active, Inactive, Maximized, And High-Contrast States First-Class

Observed in Krypton and commercial suites.

What they do:

- Theme stores state-specific colors and metrics.
- Chrome changes when the window is active, inactive, or maximized.
- Accessibility states are handled explicitly, not as afterthoughts.

What BeepiFormPro can learn:

- Promote active, inactive, maximized, disabled, and high-contrast visual states into the form metrics contract.
- Avoid per-painter hidden logic for those state transitions.
- Add a mandatory fallback theme path for high-contrast mode.

### 6. Invest In Theme Authoring And Diagnostics Tools

Observed in DevExpress, Telerik, Syncfusion, and Krypton.

What they do:

- Provide theme editors, visual style builders, preview apps, palette designers, and control inspection tools.

What BeepiFormPro can learn:

- Add a lightweight Beep theme and painter preview sample.
- Add a diagnostics overlay or debug panel showing layout rectangles, hit areas, current painter, current theme, DPI scale, and window state.
- Treat authoring tools as part of maintainability, not optional extras.

### 7. Treat Accessibility, Automation, And Localization As Core Skinning Requirements

Observed across DevExpress, Telerik, and Syncfusion.

What they do:

- Validate themed controls against accessibility and automation expectations.
- Ensure localization and resource-driven text are supported cleanly.

What BeepiFormPro can learn:

- Audit caption buttons, search box, and custom regions for accessible names, keyboard access, focus cues, and high-contrast behavior.
- Add localization-safe metrics and ellipsis behavior for longer captions.
- Include UI automation and accessibility checks in validation.

### 8. Prefer Consistent Design-Time And Demo Experiences

Observed strongly in commercial products.

What they do:

- Offer predictable Visual Studio integration, demo apps, preview forms, and design-time resilience.

What BeepiFormPro can learn:

- Simplify the design-time code path.
- Add a sample host that can switch painters and themes live.
- Use that sample as the first regression surface for future form-style work.

## Candidate Features For BeepiFormPro

### High Value

1. Palette-style token object separate from painter instances.
2. Explicit `RecalcChrome`, `RedrawChrome`, and `ReapplyTheme` style operations.
3. Shared animation manager for caption and form chrome.
4. Active and inactive window visual state support.
5. Debug overlay for layout, hit areas, and DPI diagnostics.
6. Accessibility and keyboard audit for system and custom caption actions.
7. Live theme and painter preview sample.

### Medium Value

1. ToolStrip and menu renderer bridging so the form skin extends to common hosted menus.
2. Theme export and import format for painter and metric presets.
3. Touch-friendly caption button metrics and hit zones.
4. Localizable caption and search placeholder metrics.

### Lower Priority Or Later

1. Markup-based caption templating similar to HTML and CSS driven systems.
2. Hardware-accelerated rendering path experiments.
3. End-user theme editor.

## Plan Mapping

- Phase 2: palette and theme token model, registration lifecycle, invalidation events.
- Phase 3: state model, accessibility semantics, touch and keyboard interactions.
- Phase 4: renderer consolidation, shared animation infrastructure, ToolStrip renderer bridging.
- Phase 5: explicit chrome redraw and recalc hooks, active and inactive transitions, maximize and restore correctness.
- Phase 6: diagnostics overlay, demo host, accessibility checklist, localization, and regression matrix.

## Recommendation

Do not try to copy feature breadth from commercial suites.
The useful lesson is architectural discipline:

- one theme source of truth
- one renderer or painter activation path
- explicit chrome redraw vs recalc operations
- shared animation primitives
- strong design-time and accessibility validation

That subset is realistic for BeepiFormPro and aligns with the current debt profile.