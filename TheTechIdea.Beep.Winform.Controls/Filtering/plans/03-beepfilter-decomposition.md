# 03 — `BeepFilter` decomposition

## Finding

`BeepFilter` is already a partial class split across five files:

| lines | file |
|---|---|
| **1,358** | `BeepFilter.cs` |
| 333 | `BeepFilter.Properties.cs` |
| 326 | `BeepFilter.Events.cs` |
| 214 | `BeepFilter.Layout.cs` |
| 201 | `BeepFilter.HitTest.cs` |

The split is sound in principle — properties, events, layout and hit-testing are separated. But
`BeepFilter.cs` still holds 1,358 lines across eleven regions, and the region offsets show what is
actually in there:

```
Painter Management           :125
Layout Management            :142
Overrides                    :179
Event Handlers               :363
Filter Management Methods    :467    <- ~480 lines
Event Raising Methods        :950
Phase 1: Keyboard Handling   :984
Phase 1: Public API Methods  :1005   <- ~300 lines
Dispose                      :1312
```

Two observations:

1. **`Filter Management Methods` is ~480 lines inside a control class.** Building, validating and
   serialising criteria is model work, not control work — and `FilterCriteria` (221 lines),
   `FilterValidationHelper` (355) and `FilterEngine` (416) already sit beside it. Either this region
   duplicates them or it bypasses them; both are worth knowing.
2. **`Phase 1:` region names are scaffolding left in place.** They record the increment that added
   the code, not what it does. `Phase 1: Public API Methods (Keyboard Handler Callbacks)` is ~300
   lines whose only stated identity is when it arrived.

## Work

- [ ] Establish what `Filter Management Methods` does that `FilterCriteria` /
      `FilterValidationHelper` / `FilterEngine` do not. Move what belongs to them; delete what
      duplicates them
- [ ] Move keyboard callbacks into `FilterKeyboardHandler` (353 lines, already exists) or a
      `BeepFilter.Keyboard.cs` partial, leaving `BeepFilter.cs` with construction, painting and
      overrides
- [ ] Rename regions to say what they contain rather than when they arrived
- [ ] Target: no partial much over 400 lines, and no region named for a phase number

## Verification

- The public surface of `BeepFilter` is unchanged — decomposition is not an API change. Assert by
  compiling every consumer, **including the sibling repositories**, not just this one
- `FilterProbe` drives build → validate → apply through the public API and gets an identical
  `FilterConfiguration` before and after
