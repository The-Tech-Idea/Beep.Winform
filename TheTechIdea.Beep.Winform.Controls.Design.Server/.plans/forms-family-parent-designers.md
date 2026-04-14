# Forms-Family Parent Designers

## Scope

- `BeepFormsHostDesigner`
- `BeepFormsHeaderDesigner`
- `BeepFormsStatusStripDesigner`
- `BeepFormsQueryShelfDesigner`
- `BeepFormsCommandBarDesigner`
- `BeepFormsPersistenceShelfDesigner`
- `BeepFormsToolbarDesigner`

## Completed

- Migrated every forms-family parent designer to `BaseBeepParentControlDesigner`
- Removed duplicated `_changeService`, `SetProperty`, `GetProperty<T>`, and local `ActionLists` boilerplate
- Inherited the common Beep style/theme smart-tag surface across the integrated forms shell controls

## Follow-Up

- Keep any new forms-family parent designer on the shared parent base instead of reintroducing local property plumbing
- Add container-specific actions only if a forms shell surface starts supporting authored child-control composition beyond its current runtime-managed layout