# ComboBox Overhaul QA Session Log

Use this log together with `manual-qa-matrix.md` to capture concrete runtime findings.

## Session Metadata
- Date:
- Tester:
- Branch/Commit:
- Build Configuration:
- Theme:
- DPI:
- Direction (`LTR`/`RTL`):

## Run Summary
- Total checks executed:
- Passed:
- Failed:
- Blocked:

## Findings

| ID | Area | Variant/Type | Scenario | Expected | Actual | Repro Steps | Severity | Status |
|----|------|---------------|----------|----------|--------|------------|----------|--------|
| F-001 | Example | OutlineDefault | Keyboard Enter commit | Row commits once | Double commit observed | 1) Open 2) ArrowDown 3) Enter | High | Open |

## Retest Results

| Finding ID | Fix Commit/PR | Retest Date | Retest Result | Notes |
|------------|----------------|-------------|---------------|-------|
| F-001 |  |  |  |  |

## Signoff
- [ ] Core field state matrix complete
- [ ] Popup behavior matrix complete
- [ ] Row-kind contract matrix complete
- [ ] Multi-select stress matrix complete
- [ ] Property contract matrix complete
- [ ] DPI/RTL matrix complete
- [ ] No open high-severity findings

Reviewer:
