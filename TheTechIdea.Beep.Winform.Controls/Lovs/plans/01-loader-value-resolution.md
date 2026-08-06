# Stage 01 — The async loader cannot hold a value

**Kind:** bug · **Files:** `BeepListofValuesBox.cs`, `BeepLovPopup.cs` · **Status: done.**

`ValidateKey` and `UpdateDisplayValue` consulted `ListItems` and nothing else. `ItemsLoader` filled the
popup's own list and never wrote back — above a comment saying it should.

So with a loader set, which is the query-backed case an Oracle LOV exists for:

- assigning `SelectedKey` was refused and reverted
- **picking a row in the popup reverted the selection the user had just made**, because
  `SetSelectedItem` assigns `_keyTextBox.Text`, which re-enters `TextChanged`, which validated against
  the empty `ListItems`

## The fix

**One source of truth.** A `_known` dictionary records every item the control has seen — from
`ListItems`, from the loader's results, or from a single accepted item — and validation and display
lookup both read it.

- `SetSelectedItem` records the item **before** assigning the text box, since that assignment
  synchronously re-enters validation.
- `LoadItemsAsync` now **returns** its results instead of `Task`, and the box records them.

## A key can arrive before the list does

The first fix was not enough, and the check caught it: a bound form loads a record and assigns
`SelectedKey` long before anyone opens the LOV. The loader has not run, so the key is unknown, so it
was refused — throwing away valid data because the control had not looked it up yet.

Now an unknown key is **accepted provisionally** when a lookup is still possible, and resolved in the
background:

- **`KeyResolver`** — `Func<string, CancellationToken, Task<SimpleItem?>>` — is Oracle's
  validate-from-list: one row, one query. A LOV over ten thousand rows must not load them all to check
  one foreign key.
- With no resolver, it falls back to running `ItemsLoader` once, so a caller who configured only the
  bulk loader still gets a display value rather than a bare key.
- A resolver that returns `null` is authoritative and the key is then refused.

A lookup that *throws* is reported through `BeepLog` and the key is left alone. Failing to reach the
database is not evidence the key is wrong, and clearing a user's input on a network blip is worse than
showing an unresolved key.

## Verification

`sync: a valid key is accepted` and `sync: the display value resolves` pass throughout — that is the
guard. Without it, the async assertions failing would only show that the checks were broken for
everything.

| check | before |
|---|---|
| async: a key from the loader is accepted | **reverted to `''`** |
| async: the display value resolves | **empty** |
| resolver: the display value fills in | (no resolver existed) |
| resolver: an unknown key is eventually refused | (no resolver existed) |
