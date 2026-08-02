# 07 — Exception policy

## Finding

`Filtering/` contains **10** catch statements, **5** of them bare `catch { }`.

That is materially better than the folders this program follows: `DisplayContainers` opened with 32
(22 bare), `DialogsManagers` with 4 that between them hid two real defects. The work here is
correspondingly small.

**The count is stated as statements, not word occurrences.** Counting the word `catch` produced a
wrong figure twice in the previous program — once inflating it by including comments and prose (44
where the truth was 32), and once deflating it by anchoring on `^\s*catch`, which misses the inline
`try { … } catch { }` form and so reported no change across a commit that removed eight.

## Policy

1. **Delete** where nothing is expected to throw. Two guards in `DisplayContainers` were proven
   unreachable by feeding them the degenerate inputs they existed for — zero size, negative
   dimensions, a radius larger than the rectangle, a 1px sliver. All were handled without throwing.
2. **Narrow** where a specific failure is expected: a malformed user-entered value, a missing icon
   resource, a font family that is not installed.
3. **Report** where a failure genuinely must be absorbed. `BeepFilter` is a control; throwing from
   its paint path tears down the host form's paint cycle. `DisplayContainers` added `ContainerError`
   for exactly this shape, and the same applies here.

## Work

- [ ] Classify all 10 into delete / narrow / report
- [ ] Add a `FilterError` event if any prove to need route 3
- [ ] Prove each deletion by feeding that path the inputs the guard was guarding against

## Verification

- Zero bare `catch { }` in the folder, enforced mechanically
- Every remaining catch names a specific exception type or reports
- **Expect new failures.** If removing five guards surfaces nothing, they may not be tested hard
  enough — though at five, "nothing" is a more plausible honest outcome than it was at twenty-two.

---

## Outcome

**Bare `catch { }` in the folder: 5 → 0.**

| site | disposition | reason |
|---|---|---|
| `FilterEngine` — 5 value comparisons | narrowed | `IComparable.CompareTo` throws when runtime types disagree. That means "does not match"; a disposed source or out-of-memory does not, and now propagates. |
| `FilterEngine.GetPropertyValue` | narrowed | Indexed or inaccessible properties cannot be read — `TargetException` and friends. Anything else is a real fault. |
| `FilterEngine` regex | narrowed to `ArgumentException` | An invalid pattern is user input, not a fault. |
| `BeepFilter.InlineEditing` conversion | narrowed | The user is mid-type — `"12"` on the way to `"12.5"`. Keeping raw text is right for cast/format/overflow only. |
| `BeepFilter` OnPaint, Draw | **reported** | See below. |
| `BeepFilter.Keyboard` import/export | left alone | Already report through `NotifyImportFailure` / `NotifyExportFailure` — they tell the user, which is the point. |

### The one that mattered

The two paint guards logged through `System.Diagnostics.Debug.WriteLine`. **The compiler strips
`Debug` from Release builds**, so in a shipped application a painter failure produced a blank filter
and left no trace anywhere — not a log, not an event, nothing. It was a bare catch wearing a
diagnostic.

They now report through `FilterError` / `FilterErrorEventArgs` and `Trace`, which survives Release.
Absorbing is still correct — throwing from `OnPaint` tears down the host form's paint cycle — but
absorbing silently was not.

This is the same shape as `DisplayContainers`, where six per-step catches in `OnPaint` hid a failing
tab strip. Different symptom, same cause: a control that cannot afford to throw, and nobody deciding
what it should do instead.
