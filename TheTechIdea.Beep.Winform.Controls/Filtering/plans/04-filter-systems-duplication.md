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
