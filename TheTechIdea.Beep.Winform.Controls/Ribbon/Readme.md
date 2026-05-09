# BeepRibbonControl

`BeepRibbonControl` is organized as a partial class so feature areas stay isolated and the main control file does not become a monolith.

## Partial File Map

- `BeepRibbonControl.cs`: remaining ribbon composition, command/build helpers, quick access helpers, and shared control internals.
- `BeepRibbonControl.Properties.cs`: public property surface and ribbon events.
- `BeepRibbonControl.Lifecycle.cs`: lifecycle wiring, minimize behavior, and tab-header interaction logic.
- `BeepRibbonControl.Backstage.cs`: backstage content layout, navigation, recent/pinned items, and footer actions.
- `BeepRibbonControl.Search.cs`: search initialization, provider/local search execution, search history, and search result UI behavior.
- `BeepRibbonControl.KeyTips.cs`: key-tip generation, display, and keyboard invocation behavior.
- `BeepRibbonControl.Context.cs`: contextual tab groups and ribbon/backstage transition behavior.
- `BeepRibbonControl.Customization.cs`: runtime customization, layout persistence, and theme-token persistence helpers.
- `BeepRibbonControl.Rendering.cs`: disposal and the ribbon-specific `ToolStrip` renderer.
- `RibbonSupportTypes.cs`: ribbon event args plus customization/theme token state models.

## Notes

- Keep feature moves mechanical: copy contiguous methods into the matching partial and remove them from the main file without redesigning behavior.
- Prefer adding new ribbon behavior to the feature partial that already owns that slice instead of growing `BeepRibbonControl.cs` again.