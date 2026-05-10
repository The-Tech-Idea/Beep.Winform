# GitHub Project Playbook - BeepCheckBox Commercialization

## Goal

Manage the `CheckBoxes` roadmap like a commercial control program, not a loose list of tasks. The GitHub Project should make phase ownership, release readiness, risks, and QA status visible without reading source diffs.

## Benchmark Baseline

Use `07-github-source-benchmark-findings.md` as the benchmark reference for P1 through P4 work.

Rules:

- if an issue changes contract, layout, input, or designer behavior, cite the benchmark pattern it follows or document why Beep intentionally diverges
- do not create benchmark-comparison issues that stop at repository discovery; the comparison must name a source-backed behavior
- when a benchmark finding becomes a committed product rule, update the relevant phase doc and tracker note in the same iteration

## Recommended Project Setup

Project name: `BeepCheckBox Commercialization`  
Project type: GitHub Projects (table + board views)  
Scope: issues, draft issues, pull requests, and release-readiness tasks related to `TheTechIdea.Beep.Winform.Controls/CheckBoxes`

## Recommended Hierarchy

- One epic issue per phase
- Child issues for every tracker item in `todo-tracker.md`
- Separate bug issues for regressions found during QA or design-time validation
- Pull requests linked directly to the child issue they close

## Tracking IDs

Use the plan IDs in issue titles and PR descriptions:

- `BCHK-P1-001` through `BCHK-P6-999`
- include the tracking ID at the start of the issue title
- include the same ID in the PR title or PR body

Example issue title:

`BCHK-P2-003 Define painter capability matrix across all checkbox styles`

## Recommended Custom Fields

| Field | Type | Purpose |
| --- | --- | --- |
| Phase | Single select | P1, P2, P3, P4, P5, P6 |
| Area | Single select | API, Rendering, Accessibility, Binding, Designer, Performance, Docs, QA |
| Status | Single select | Backlog, Ready, In Progress, Review, Done, Blocked |
| Priority | Single select | P0, P1, P2, P3 |
| Risk | Single select | Low, Medium, High |
| Target Release | Single select | vNext-P1, vNext-P2, vNext-RC, vNext-GA |
| Demo Required | Single select | No, Internal Demo, QA Demo |
| QA State | Single select | Not Started, In Progress, Passed, Failed |
| Accessibility Impact | Single select | None, Minor, Significant |
| Design Review | Single select | Not Needed, Needed, Approved |

## Recommended Labels

- `area:checkbox`
- `phase:1-contracts`
- `phase:2-rendering`
- `phase:3-accessibility`
- `phase:4-binding-designer`
- `phase:5-performance`
- `phase:6-release`
- `type:enhancement`
- `type:bug`
- `type:docs`
- `type:qa`
- `priority:p0`
- `priority:p1`
- `risk:high`
- `needs:design-review`
- `needs:qa`

## Recommended Views

### 1. Phase Board

Board grouped by `Status`, filtered to `area:checkbox`, with `Phase`, `Priority`, and `Risk` shown on cards.

Use this view for daily execution.

### 2. Release Table

Table grouped by `Target Release`, sorted by `Priority` then `Risk`.

Use this view for release planning and cut decisions.

### 3. Risk And Blockers

Table filtered to `Risk = High` or `Status = Blocked`.

Use this view for leadership review and unblock meetings.

### 4. QA Readiness

Table filtered to `Demo Required != No` or `QA State != Passed`.

Use this view for sign-off preparation.

### 5. Accessibility Review

Table filtered to `Accessibility Impact = Significant`.

Use this view to keep accessibility work from being folded into generic polish.

## Workflow Rules

1. Do not move an item to `In Progress` unless acceptance criteria are written into the issue body.
2. Do not move an item to `Review` unless docs and QA implications are called out.
3. Do not move an item to `Done` unless the related PR is merged and the tracker is updated.
4. Any bug found during phase validation becomes its own issue, even if fixed inside the same PR.
5. If a task changes public behavior, add or update migration notes before closing the issue.
6. For P1 through P4 work, issue bodies should name the benchmark source or the intentional divergence rationale.

## Issue Template Guidance

Each issue should include:

- tracking ID
- problem statement
- scope boundaries
- acceptance criteria
- benchmark reference or explicit divergence note when the issue affects contract, layout, input, or designer behavior
- affected files or subsystems
- QA notes
- accessibility or designer impact

## Pull Request Expectations

- link the issue in the PR body
- state which phase doc the PR belongs to
- summarize behavioral impact, not just file changes
- include screenshots or sample notes for UI changes
- include QA notes for theme, DPI, accessibility, or designer impact when applicable

## Definition Of Done For Commercial UI Work

- implementation merged
- linked issue closed
- documentation impact handled
- QA evidence attached
- design-time and runtime impact reviewed
- tracker item and project item both updated

## Phase Epics To Create First

- `Phase 1 - Contracts, State, and API`
- `Phase 2 - Layout, Rendering, and Painter Contract`
- `Phase 3 - Input, Accessibility, and UX Reliability`
- `Phase 4 - Binding, Grid Mode, and Designer Workflows`
- `Phase 5 - Performance, Reliability, and Diagnostics`
- `Phase 6 - Documentation, Samples, and Release Readiness`

## Minimal Startup Sequence

1. Review `07-github-source-benchmark-findings.md` and decide which findings are adopted versus intentionally rejected.
2. Create the project.
3. Create the six phase epic issues.
4. Backfill draft issues from `todo-tracker.md`.
5. Add fields, labels, and views.
6. Link the project URL back into this folder when the board is live.