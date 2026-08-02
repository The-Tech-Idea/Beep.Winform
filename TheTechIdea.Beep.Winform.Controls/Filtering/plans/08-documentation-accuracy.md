# 08 — Documentation accuracy

## Finding

`Filtering/README.md` (234 lines) documents the architecture well, and its central claim is **true**:
`BeepFilter` builds a `FilterConfiguration`, raises `FilterApplied`, and the consumer applies it.
That was checked against the code rather than assumed — `GridX/BeepGridPro.Filtering.cs` constructs
the filter at `:95`, subscribes at `:266`, and applies with `FilterEngine<ExpandoObject>` at `:552`.

One documented claim is not true.

## `BeepListBox` is not a consumer

The architecture diagram — the first thing a reader sees — shows:

```
┌─────────────────┐
│   BeepListBox   │  ← Another Consumer
│ (Filter Applier)│     - Subscribes to BeepFilter.FilterApplied
│                 │     - Uses FilterEngine<T> to apply filter
└─────────────────┘     - Updates visible items
```

Searching `ListBoxes/` for `BeepFilter` or `FilterEngine` returns nothing. `BeepGridPro` is the only
consumer in this repository.

This matters more than a stale line usually would: presenting a single-consumer component as a
multi-consumer one changes how someone reasons about altering its contract. A developer who believes
two controls depend on `FilterApplied` will preserve a signature that only one actually needs — or,
worse, will assume the generic design has been validated against a second case when it has not.

## Work

- [ ] Correct the diagram to the consumers that exist
- [ ] Either implement the `BeepListBox` integration or remove it. If it is intended-but-unbuilt,
      mark it explicitly as planned rather than describing it in the present tense
- [ ] Re-check the remaining claims against the code — in particular the **DOES NOT** list.
      `BeepFilter` claiming it does not apply filters is worth confirming against the ~480-line
      `Filter Management Methods` region identified in [03](03-beepfilter-decomposition.md)
- [ ] Fold in what [01](01-dead-configuration-surface.md) establishes: `Position` and three of five
      `DisplayMode` values do nothing, and the README should not imply otherwise

## Verification

Every architectural claim in the README traceable to a call site, or explicitly marked as planned.

---

## Outcome

### The diagram

`BeepListBox` removed from the consumer diagram. `ListBoxes/` contains no reference to `BeepFilter`
or `FilterEngine`; `BeepGridPro` is the only consumer. The README now says so, and notes that
`FilterEngine<T>` is generic so any control *can* adopt the pattern — which is the true and useful
statement, rather than implying one already has.

### The claims that held

Both checked against the code, not assumed:

- **"Does not apply filters to data"** — `BeepFilter` never references `FilterEngine`. Holds. Phase
  03 had already established that the large `Filter Management` region is UI interaction, not filter
  application, so this is consistent.
- **"Does not know about grid rows or cells"** — no row or cell access anywhere in the partials.
  Holds.

### A claim the code contradicted

**"Does not handle data source directly"** was true of the *behaviour* — and `BeepFilter` exposed a
`public object? DataSource { get; set; }` anyway, advertising the opposite.

It had **zero readers** in this repository and no consumer in `Beep.Winform.Data.Integrated` or
`Beep.Sample`. Removed. A property that contradicts its own documentation and does nothing is worse
than either alone: a developer who finds it will set it and wait for something to happen.

Fifth instance of declared-but-never-read in this folder, after `FilterPosition`, three
`FilterDisplayMode` values, the two painter capability flags, and `FocusedFilterIndex`.

### Also corrected

The file listing said `FilterStyle.cs` declares `FilterStyle, FilterDisplayMode, FilterPosition`.
`FilterPosition` is gone; the listing now matches. A new **Configuration surface** section states
what the two remaining enums actually offer, including that `Modal` and `SlideIn` were removed and
why — so the next reader does not re-add them.
