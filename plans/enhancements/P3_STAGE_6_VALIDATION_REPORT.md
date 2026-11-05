# Phase 3 — Stage 6: Validation report

## Status

**Partial.** Automated build and the linked GridX suite pass. The full
visual, DPI, high-contrast, screen-reader, and performance matrix has not been
executed in a live window.

## Environment

- Repository: `The-Tech-Idea/Beep.Winform`
- Working tree: uncommitted changes present (feature work in progress)
- Host: Windows, .NET 10 (`net10.0-windows`)
- Date: 2026-10-22

## Checks run

| Check | Command | Result |
|---|---|---|
| Controls build | `dotnet build TheTechIdea.Beep.Winform.Controls\TheTechIdea.Beep.Winform.Controls.csproj -c Debug` | passed |
| Linked GridX suite | `dotnet test <temp>/gridx-tests/GridX.Tests.csproj -p:GeneratePackageOnBuild=false` | **95 passed, 0 failed** |

The linked suite is used because the repository test project still cannot build
because of unrelated `Beep.Winform.Data.Integrated.Controls` interface errors.

## Covered by the suite

- Chrome metrics: density values, breakpoints, state colors.
- Header geometry: Standard, Material, AG Grid, Compact painter cell layouts.
- Header focus state.
- Toolbar search layout: count, previous/next, clear, scope placeholder.
- Search: scope, match count, next-match navigation, clear-preserves-column-
  filters, virtual search, no-results state, cell highlighting, accessible
  description.
- Filters: operator/predicate behavior, saved-filter versioning, unknown-column
  cleanup, chip overflow, editable chip hit paths.
- Pagination: page size, page count, page navigation, page reset.
- Navigator status: total/filtered/selected text and compact form.
- Export: snapshot, async, cancel, progress, no control references.
- Conditional formatting and footer aggregate/pinning.
- Rendering smoke: `DrawToBitmap` with chrome, search, footer, and no-results.

## Not verified

| Area | Missing evidence |
|---|---|
| Visual matrix | All 12 header and 12 navigation painters rendered side by side |
| DPI | 125 % / 150 % rendering and hit-target measurements |
| High contrast | Chrome state and all header/navigation painters support system high contrast; live visual verification remains |
| Screen reader | Real Narrator/Inspector session for header/search/chips/pager |
| Performance | Before/after timings for typing, filtering, sorting, paging |
| Live UI | Modeless export progress and go-to-page dialog behavior in a running form |
| Filter unification | Both pipelines still have separate internal state |
| Search live region | Accessible description is updated, but not announced as a live region |

## Follow-up actions

1. Run the visual fixture matrix with light/dark/high-contrast themes at 100/125/150 % DPI.
2. Perform a keyboard-only and screen-reader walkthrough of the header, search, chips, and pager.
3. Capture performance numbers for typing/filter/sort/page-size changes.
4. Unify `ActiveFilter` and `GridSortFilterHelper` state behind one `GridQuery`.
5. Repair the repository test project reference or split GridX tests into a buildable project.
6. Verify the export progress and go-to-page dialogs in a live WinForms host.
