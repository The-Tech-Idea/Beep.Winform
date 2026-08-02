# 04 — Competing filter systems

## Finding

`GridX/CLAUDE.md` states plainly, as a high-signal invariant:

> **Two filter systems exist**
> - `BeepGridPro.Filtering.cs` owns `ActiveFilter`, `ApplyQuickFilter`, `ShowAdvancedFilterDialog`,
>   `FilterApplied`, `FilterCleared`.
> - `GridSortFilterHelper` owns the `SortFilter` pipeline used by header popup filtering and
>   binding-source sort/filter attempts.
> - They are related in the UI but not the same internal state. Do not document them as one unified
>   engine.

`Filtering/` is a third participant in the same outcome:

- `GridX/BeepGridPro.Filtering.cs:95` constructs a `BeepFilter`
- `:266` subscribes to its `FilterApplied`
- `:552` applies the result with `FilterEngine<ExpandoObject>`

And two further subscribers exist on *different* popups:

- `GridX/Helpers/ExcelFilterHelper.cs:35`
- `GridX/Helpers/BeepGridProFilterExtensions.cs:75`

So a grid can reach a filtered state by several routes that do not share internal state.

## Why this phase belongs to the Filtering program

`Filtering/` is the generic, reusable component — the one with a documented contract and an engine
that works on `T`. If any of these is the intended single path, it is this one.

This phase does **not** propose collapsing the grid's internals; that is `GridX` work with its own
constraints, and `GridX/CLAUDE.md` explicitly warns against documenting the two as unified. What it
proposes is establishing which system owns the truth, and whether `Filtering/` can serve the other
callers so they become adapters rather than parallel implementations.

## Work

- [ ] Map the routes: what state each owns, which events each raises, and what happens when two are
      used in one session
- [ ] Determine whether `FilterEngine<T>` can serve the header-popup and quick-search paths
- [ ] If it can, reduce the others to callers of it and delete the duplicated application logic
- [ ] If it cannot, record precisely why in `Filtering/README.md`. A deliberate split is defensible;
      an undocumented one is what produces the next bug

## Verification

- Apply a filter through each route in turn; assert `ActiveFilter` and the visible row set agree
- Apply through one route then another; assert the second **replaces** rather than silently
  compounding
- Clear through one route; assert every route reports cleared

---

## Outcome

### The map

Two systems, no shared state, **the same output field**:

| | route A | route B |
|---|---|---|
| owner | `BeepGridPro.Filtering.cs` | `GridSortFilterHelper` |
| state | `ActiveFilter`, `_isFiltered`, `_filteredRowIndices` | `_containsFilters`, `_inFilters`, `column.Filter` |
| driven by | `BeepFilter` via `FilterApplied` | header popups, quick search |
| writes | `Data.Rows[i].IsVisible` | `row.IsVisible` |
| reads the other | **no** | **no** |

Route B's `SetAllRowsVisible()` recomputes visibility from its own criteria alone. Route A's
`UpdateFilteredDisplay()` does the same from its own. Whichever ran last decided what the user saw,
and the other system's state stayed behind claiming otherwise.

### Demonstrated, not asserted

A grid bound to five rows:

| step | visible | `IsFiltered` |
|---|---|---|
| unfiltered | 5 | — |
| route A quick-filter `"Norway"` | 2 | `True` |
| route B `ClearFilters()` | **5** | **`True`** |

The grid showed every row while reporting an active filter, with `ActiveFilter` still holding the
Norway criterion. Anything reading `IsFiltered` — a clear-filter chip, the toolbar's active-filter
badge — advertised a filter that was not applied.

The mirror was equally broken: route A's `ClearFilter()` left route B's per-column filters and
`column.Filter` strings in force.

### The fix

Each route's clear now clears the other, guarded by `_isClearingAllFilters` against the mutual call.
This does **not** collapse the two systems — `GridX/CLAUDE.md` documents them as deliberately
separate and that stands. It makes them agree on the one thing they share: whether the grid is
filtered.

| step | visible | `IsFiltered` |
|---|---|---|
| route A filter, route B clear | 5 | `False` |
| route B filter, route A clear | 5 | — |

### Not done

Unifying application (as opposed to clearing) is left alone. Route A and route B still compute
visibility independently, so applying through both in one session still means the last one wins.
That is a larger change inside `GridX` and belongs to a `GridX` program, not this one — but it is now
a known, bounded gap rather than an invisible one.
