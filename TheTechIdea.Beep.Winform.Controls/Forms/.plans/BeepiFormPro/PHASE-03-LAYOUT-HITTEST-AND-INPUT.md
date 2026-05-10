# Phase 3 - Layout, Hit-Testing, And Input

## Goal

Normalize caption geometry, hit-area registration, and pointer/keyboard interaction so all painters participate in one reliable interaction model.

## Evidence From Current Code

- `BeepiFormPro.Core.cs` has to normalize multiple hit-area spellings in `OnRegionClicked`, including both `style` and `Style` style variants.
- Several right-aligned painters share the same caption-button skeleton with only spacing and title or icon differences, while Metro and traffic-light families prove the contract still needs layout-family boundaries instead of one universal template.
- `MacOSFormPainter.cs` uses a left-side traffic-light model, which proves the contract needs to support layout families, not ad hoc per-file naming.
- `BeepiFormPro.Drawing.cs` routes interaction only outside `CurrentLayout.ContentRect`, which makes layout freshness critical for all pointer behavior.
- Search box, theme button, style button, custom action button, and profile button all rely on painter-provided rectangles but are not yet governed by one central naming and geometry contract.

## Scope

- Standardize hit-area names and button identities.
- Move repeated caption geometry into reusable layout helpers or patterns.
- Ensure `CurrentLayout` is valid before interaction decisions.
- Align keyboard, mouse, hover, and caption-drag behavior across styles.

## Deliverables

- Canonical hit-area constants.
- One documented layout contract for right-aligned caption buttons and one for left-side traffic-light painters.
- Reduced geometry duplication in painters that share the same layout family.
- Clear interaction rules for content, caption, search box, and optional action buttons.

## Current Progress

- A baseline repair has already been applied so hit-area names are normalized when painters register them, rather than being fixed up later in the click path.
- `GetHitArea` lookups and `OnRegionClicked` now consume the same canonical naming rules.
- Built-in hit-area names now have shared constants, so the default form chrome path no longer depends on ad hoc string literals for the core caption buttons and search interactions.
- The normalized click-dispatch path has been repaired so custom action, theme, style, search, and caption hits route to the intended behavior again.
- Painter hover lookups for the built-in caption actions have now been migrated to the shared constants as well, so the main lookup surface no longer reintroduces raw built-in key strings.
- Representative painter internals now use `FormHitAreaNames` in helper button-type flows and registration calls, which proves the constant-based contract works beyond hover lookups.
- Multiple additional painter families now use the same constant-based contract through helper arguments, switch labels, and registrations, so the migration is no longer limited to a representative sample.
- Several registration-only painters have also been cleaned up, which reduces the remaining contract drift to a smaller set of untouched families instead of the whole painter folder.
- The remaining painter families and the legacy painter-plan example have now been aligned to the same built-in naming contract, so the painter folder no longer depends on raw built-in hit-area strings for live code paths.
- The Forms and Painters readmes now document the canonical built-in hit-area naming rule for future painter updates.
- A dedicated `FormPainterLayoutHelper` now owns the standard right-aligned caption skeleton for compatible painters, and Modern, Material, Material You, NextJS, RadixUI, Shadcn, Linear, Minimal, Metro, and Metro2 now share that helper while still owning painter-specific title, icon, and safe-area calculations.
- The helper contract now includes explicit optional-button policy for the custom-action slot, which keeps Metro and Metro2 behavior intact without reopening duplication.
- A second `FormPainterLayoutHelper` path now owns the traffic-light caption skeleton for MacOS and iOS, including caption drag registration, left-side close/minimize/maximize placement, and the shared right-side optional button cluster.
- Ubuntu is not part of that traffic-light family in live code, so the remaining extraction boundary is now the true outlier painters rather than the primary caption families.
- The core layout cache now tracks the missing search and profile visibility flags, and replacing `FormPainterMetrics` now invalidates layout so geometry-driven painters do not keep stale hit areas until an unrelated resize or style change.
- A shared keyboard-caption focus model now sits on top of the painter geometry contract: `Ctrl+F` focuses the search box, `F6` and `Shift+F6` cycle visible caption actions, `Tab` continues traversal while search is active, and `Enter` or `Space` activates the focused caption action without depending on mouse hover state.
- Caption keyboard focus now renders through a central focus-ring overlay instead of painter-specific drawing paths, which keeps the interaction layer separate from painter-owned caption visuals.
- When `ScreenReaderSupport` is enabled, caption keyboard focus now updates the form accessibility surface with caption-action names and descriptions so the new keyboard path is not visual-only.
- `OnRegionClicked` has now been repaired to dispatch caption actions through canonical hit-area names again, which removes the previously corrupted merge state from the central click path.
- The shared layout helpers now reserve a profile slot, and the helper-based painters plus the remaining manual search-paint outliers now render that built-in profile button so profile activation is no longer limited to a dead event surface.
- `FormPainterMetrics` now owns `SearchBoxWidth` and `SearchBoxPadding`, those values flow through the DPI-aware metrics path, and the remaining painter layouts no longer carry local `200/8` search-box constants.
- A follow-up audit of hardcoded caption button widths found only one no-behavior-change normalization case so far: Glass now uses `metrics.ButtonWidth`, while the remaining local widths still differ from the current style metrics table and therefore stay painter-owned until reviewed deliberately.
- `CAPTION-BUTTON-WIDTH-AUDIT.md` now records the remaining outliers explicitly: Fluent, Cartoon, ChatBubble, and Custom still use manual right-side widths, while GNOME and the MacOS/iOS traffic-light family have been moved onto explicit shared metrics (`VisualButtonWidth` and `AuxiliaryButtonWidth`) instead of painter-local literals.
- `CAPTION-BUTTON-WIDTH-REFACTOR-PROPOSAL.md` now splits those remaining outliers into three decision buckets: Fluent as an independent font-derived contract, Cartoon and ChatBubble as one shared live-layout pair that still disagrees with the metrics table, and Custom as an intentional wide-cluster baseline that should not be collapsed into `ButtonWidth` by default.
- Caption accessibility focus now carries `AccessibleRole` and `AccessibleDefaultActionDescription` alongside name and description, the original accessibility metadata is restored and re-snapshotted cleanly when caption focus leaves, and runtime changes to `HighContrastMode`, `ScreenReaderSupport`, and `FocusIndicatorStyle` now update the live caption experience instead of waiting for unrelated repaint or focus cycles.
- The focus-ring renderer now automatically upgrades to the high-contrast variant when either `HighContrastMode` or the OS high-contrast setting is active, so caption keyboard focus is no longer dependent on a separate manual focus-style switch.
- Successful layout recalculation now also clears keyboard-caption focus when the previously focused caption target disappears from the visible caption target list, which closes the stale-focus gap for runtime changes to optional caption elements and caption-bar visibility.
- Disabling `ScreenReaderSupport` now force-restores the base accessibility metadata instead of leaving the last caption-action metadata active, and toggling `HighContrastMode` now invalidates the full form rather than only the caption because that mode changes the global graphics-quality setup used by the custom form paint path.
- `FormRegion` now has an opt-in interaction contract for custom regions, layout recalculation registers interactive regions into hit-testing automatically, caption-docked interactive custom regions now participate in keyboard-caption traversal, and their accessibility names, descriptions, roles, and default actions can be supplied directly on the region model.
- Caption-docked custom regions now also honor explicit bounds relative to `CurrentLayout.CaptionRect`; the older full-caption rectangle remains only as the fallback for built-in or legacy regions that leave `Bounds` empty.
- The remaining work in this phase is now narrower: validate the new custom-region contract in real usage paths and finish the deliberate review of painter-specific button-geometry ownership.

## Task Breakdown

- [ ] Introduce shared hit-area name constants and remove case-based normalization fallback logic.
- [ ] Extract reusable caption layout builders for common painter families.
- [x] Replace hardcoded repeated search box widths and padding with metrics-driven values.
- [ ] Use `CAPTION-BUTTON-WIDTH-REFACTOR-PROPOSAL.md` and the diagnostics host metrics/layout/hit-area view to decide whether any remaining manual right-side caption widths should move to shared metrics as part of a broader caption-height/layout cleanup.
- [ ] Ensure `EnsureLayoutCalculated()` and interaction code cannot operate on stale or partial layout.
- [ ] Add a clear policy for optional caption elements when they are hidden or disabled.
- [ ] Validate and polish the new custom-caption-region semantics, including caption-relative bounds, high-contrast expectations, and any remaining keyboard edge cases.
- [ ] Review hit targets for touch-friendly minimum sizes where the style permits it.

## Special Cases To Preserve

- macOS and iOS style traffic-light buttons on the left.
- Custom painter-specific spacing and icon treatment.
- Search box and profile button support for styles that opt in.
- Custom caption regions should stay opt-in: only `FormRegion` entries marked interactive participate in hit-testing, keyboard traversal, and accessibility metadata.

## Risks

- Over-centralizing layout can erase style-specific identity if painters lose control over spacing and safe areas.
- Changing hit-area identifiers without a migration pass can break existing click behavior and designer tooling.

## Exit Criteria

- `OnRegionClicked` no longer needs compatibility aliases for normal painter behavior.
- Painters that share a layout family stop duplicating the same geometry logic.
- Pointer and keyboard interaction works from one stable layout contract.