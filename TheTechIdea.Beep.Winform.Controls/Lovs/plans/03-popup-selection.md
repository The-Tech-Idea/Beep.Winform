# Stage 03 — The popup accepts a row nobody chose

**Kind:** bug · **Files:** `BeepLovPopup.cs` · **Status: done.**

```csharp
// Fall back to first item if nothing was explicitly clicked
var item = _pendingSelection ?? (_filteredItems.Count > 0 ? _filteredItems[0] : null);
```

Pressing Enter in the search box with no row highlighted committed **whatever happened to be at the top
of the list**. On a control whose job is to put a foreign key into a record, silently choosing a row the
user never looked at is the worst thing it can do — the record saves, and nothing looks wrong.

`FilterItems` sets `_pendingSelection = null` on every keystroke, so this was not an edge case: typing
anything and hitting Enter went through the fallback.

## The fix

`Accept` commits the highlighted row, **or the only row left after filtering**. Narrowing a search until
one candidate remains and pressing Enter is how a LOV is meant to be driven and there is nothing
ambiguous about it; anything else needs an explicit choice.

With more than one candidate and nothing highlighted, focus moves into the grid instead of guessing.

**Down from the search box now moves into the list** and highlights the first row. Without it, taking
away the guess would have left the mouse as the only way to choose — Enter no longer picks for you, so
something has to get you into the list.

## Verification

| check | why |
|---|---|
| Enter with nothing highlighted commits nothing | the defect |
| Enter with one candidate accepts it | the deliberate exception |
| **a highlighted row is committed** | the guard — without it, an inert `Accept` would pass the first check |

The guard is the important one. "Commits nothing" is exactly what a broken `Accept` would also report.
