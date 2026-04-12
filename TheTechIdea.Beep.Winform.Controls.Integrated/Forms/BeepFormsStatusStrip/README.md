# BeepFormsStatusStrip

`BeepFormsStatusStrip` is the shared-state reader for the fresh-start integrated forms path. It renders `BeepForms.ViewState` without turning `BeepForms` back into a visual shell.

## Responsibilities

- Bind to a `BeepForms` host through `FormsHost`
- Auto-discover a nearby host when `AutoBindFormsHost` is enabled
- Render status, current message, coordination, savepoint, and alert lines from shared view state
- Expose designer-configurable line visibility so the strip can be shaped without custom code
- Keep workflow execution out of the strip; it is read-only UI over manager-owned state
- Prefer the active block's current queued message or the manager status-area message when those are available, so the strip stays aligned with `FormsManager` outcomes

## Notes

- `BeepForms` remains the non-visual coordinator/host
- `BeepFormsHeader` owns title and active-context rendering
- `BeepFormsCommandBar` owns block switching and sync
- `BeepFormsQueryShelf` owns query-mode entry, execution, and caption variants
- `BeepFormsPersistenceShelf` owns commit and rollback
- `BeepFormsToolbar` remains the action surface for savepoints and alerts
- `BeepFormsStatusStrip` is the companion state surface for those workflows and no longer falls back to form title text for default status copy