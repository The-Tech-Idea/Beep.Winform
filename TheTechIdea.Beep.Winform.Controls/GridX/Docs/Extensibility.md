# Extensibility

Custom cell drawers
- `GridRenderHelper` uses `IBeepUIComponent` implementations to render each cell based on `BeepColumnConfig.CellEditor`.
- Add new editor types by implementing `IBeepUIComponent` and adding a case in `GetDrawerForColumn`.

Dialog editors
- `GridDialogHelper` shows a modeless overlay aligned to the cell with key-driven commit/cancel/navigation.
- Replace with a custom editor strategy if needed; call `BeepGridPro.OnCellValueChanged()` after commit.

External navigator
- Attach `BeepBindingNavigator` via `AttachNavigator(navigator, dataSource)`.
- Or rely on built-in owner-drawn navigator.

Excel-style filter integration
- Zero-internal-changes approach via `BeepSimpleGridLike` and `BeepGridProAdapter` with `ExcelFilterHelper.ShowForColumn()`.
- Or call `EnableExcelFilter()` extension to hook header clicks directly.

Unit of Work binder
- Use `GridUnitOfWorkBinder` to hook a UnitOfWork that exposes `Units` collection and emits events.
- Call `Attach(uow)` to bind; `Detach()` to unhook.

Theming
- Extend `BeepThemesManager` and theme definitions (`IBeepTheme`) to provide different typography and color sets.
