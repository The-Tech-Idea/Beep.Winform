# 05 — Engine and operator coverage

## Scope

`FilterEngine.cs` (416), `FilterOperator.cs` (266), `FilterCriteria.cs` (221),
`FilterValidationHelper.cs` (355).

This is the part that decides whether a row matches — and the part with **no visual symptom when it
is wrong**. A filter that silently excludes the wrong rows looks exactly like a filter that works.
Every other phase in this program can be checked by looking; this one cannot.

## Work

- [ ] Enumerate every `FilterOperator` value and assert each is handled by `FilterEngine`. An
      unhandled operator that falls through to "no match" or "all match" is the highest-severity
      defect available in this folder, and the one least likely to be reported as a bug
- [ ] Type coverage: `string`, numeric, `DateTime`, `bool`, `null`, plus the enum and `Guid` cases the
      entity structures actually carry
- [ ] Null semantics: is `IsNull` distinct from `Equals(null)`, and does a null *field* match a
      non-null criterion?
- [ ] Case sensitivity: decided once, applied everywhere. A `Contains` that is case-insensitive while
      `Equals` is not will be read as a bug by users either way
- [ ] Culture: numeric and date parsing must not depend on the ambient culture, or the same filter
      returns different rows for a user in a different locale
- [ ] `FilterValidationHelper` — confirm every rule it enforces is one the engine relies on, and that
      the engine does not silently accept input that validation rejects. Divergence here means the UI
      blocks something the engine handles, or permits something it does not

## Verification

Table-driven, in [09](09-verification-harness.md): a fixed dataset, every operator, every supported
type, the expected matching row set asserted per case. This is the one phase whose verification
should be exhaustive rather than sampled, because the failure mode is invisible.

Include negative cases explicitly. An operator that matches *everything* passes any test that only
checks the right rows are present — the assertion has to be that the wrong rows are absent.

---

## Outcome

### The headline suspicion was not supported

`FilterOperator` declares **17** values and `FilterEngine` handles **all 17**. There is no unhandled
operator falling through to "no match" or "all match".

A first pass reported a gap — `And` and `Or` unhandled. They are in a **separate `FilterLogic` enum**
declared in the same file; the count had spanned both. This is the second time in this program that
counting across enum boundaries in one file produced a false finding, after `FilterStyle`. Files here
routinely declare several related enums together, and `grep` does not respect the boundary.

The engine's fallthrough is `_ => false`, so an operator added later without a handler excludes
rather than includes — the safer of the two failure modes.

### Three real findings

**1. `Regex` ignored `CaseSensitive`.** Every other string operator passes
`criterion.CaseSensitive` to its comparison. `Regex` did not: the dispatch called
`MatchesRegex(propertyValue, criterion.Value)` and the implementation hardcoded
`RegexOptions.IgnoreCase`. Setting `CaseSensitive = true` changed `Equals`, `Contains`,
`StartsWith`, `EndsWith` and `In`, and silently did nothing for `Regex`. Fixed — both directions are
now asserted.

**2. `CompareIn` silently matched nothing when handed a collection.** It parsed
`filterValue.ToString().Split(',')`, so a `List` or array became the single value
`"System.Object[]"`. A set filter returning no rows is indistinguishable from one correctly excluding
everything. It now accepts either shape.

*Not a production break:* `GridSortFilterHelper.FilterIn` keeps its own `_inFilters` set and never
uses `FilterEngine`, so only route A — `BeepFilter` criteria applied through
`FilterEngine<ExpandoObject>` — could reach it.

**3. `IsNull` means null-or-empty.** `IsNull => IsNullOrEmpty(propertyValue)`. Defensible for a grid,
where a blank cell and an absent value are the same thing to the user — but the operator's name does
not say so, and a caller cannot currently distinguish `null` from `""`. Left as-is and documented at
the dispatch site rather than changed silently; changing it would alter results for every existing
filter.

### Coverage

21 cases, every one asserting an **exact** expected row set rather than the presence of expected rows
— an operator matching everything passes the weaker form. 17/17 operators exercised across string,
`int`, `DateTime`, `bool`, null and set-membership.

### Three of the five initial failures were the test, not the engine

`Regex/string`, `In` and `NotIn` failed because the probe passed `object[]` where the engine wanted a
comma-joined string, and asserted case-sensitive regex behaviour that did not yet exist.
`IsNull`/`IsNotNull` failed because the probe assumed strict null. Each was checked against the
implementation before being called a defect — two turned out to be real, three were mine.
