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
