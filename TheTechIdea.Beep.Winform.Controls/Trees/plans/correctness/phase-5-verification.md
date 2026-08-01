# Phase 5 — Permanent Verification Harness

**Goal:** the alignment invariant is machine-checked, and every painter is rendered before a layout
change is called done.

---

## Why

The reported misalignment shipped because nothing asserted it. It is invisible in a build, invisible
in a unit test that never measures geometry, and easy to miss by eye in a tree whose nodes all
happen to have children. It took a probe that prints per-node X positions to make it obvious in one
line:

```
level 2: distinct text X = [32, 50]   <-- 2 different alignments on the same level
```

That check is cheap and permanent. It should not live in a scratchpad.

## Work items

### 5.1 Promote the probe

`scratchpad/TreeProbe` currently reaches `_visibleNodes` and `GetScaled*` through reflection. Move
it into the repo as a small harness. Either expose a debug-only accessor for the layout snapshot, or
keep reflection and accept it — but the probe should live next to the control, not in a temp folder
that disappears.

### 5.2 Assertions, not output

Today the probe prints a table a human reads. Make the invariants fail loudly:

- **Per-level alignment** — one distinct `TextRectContent.X` per `Level`. The core assertion.
- **Monotonic indent** — level N's text X is strictly greater than level N-1's.
- **Slot reservation** — a leaf and a parent on the same level produce the same text X.
- **No overlap** — within a row, toggle / checkbox / icon / text rects do not intersect.
- **Row heights positive** and `Y` strictly increasing down the visible list.

### 5.3 Matrix

Run the assertions across the combinations that actually vary geometry:

```
{leaf, parent} × {icon, no icon} × {ShowCheckBox on, off}
              × {100%, 150%, 200% DPI} × {single-column, multi-column}
```

DPI matters: every metric goes through `GetScaled*`, and rounding at 150% is where off-by-one
indent errors appear.

### 5.4 Painter sweep

There are **28 painters** in `Trees/Painters/`. They consume the layout rather than computing indent
themselves (verified — none of them multiply by `Level` or read `IndentWidth`), so a layout fix
applies to all of them. That also means one bad layout breaks all 28 at once, which is exactly why
the sweep is worth automating: render each painter to PNG and diff against a stored baseline.

Start by rendering all 28 and **looking at them once** — the grid work turned up four style groups
that rendered identically, which is only visible in a contact sheet.

### 5.5 Wire into the tracker

Record the probe's numbers in `MASTER-TODO-TRACKER.md` when a phase closes, the way the grid phases
record theirs. A phase is done when the numbers say so, not when it compiles.

## Exit criteria

- [ ] Probe lives in the repo and runs from a single command
- [ ] Invariants assert and fail; they do not merely print
- [ ] Matrix covers leaf/parent × icon × checkbox × DPI × column mode
- [ ] All 28 painters render to a contact sheet that has been reviewed at least once
- [ ] Baseline images stored so future layout changes diff rather than re-eyeball
