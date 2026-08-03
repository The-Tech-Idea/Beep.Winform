# 07 — Layout persistence and migration

## What exists

`BeepDockingManager.Persistence.cs` (264 lines), `Models/DockLayoutTree.cs` (230 lines), and
`LoadLayout(DockLayoutDefinition)` at `:252`. A layout can be saved and restored.

`grep` finds **2** references to `SaveLayout|LoadLayout` in the whole folder — so the round trip
exists but is barely exercised, and nothing in the folder appears to test it.

## Why this feature grows once 01–04 land

Every feature ahead of it adds state that must survive a restart:

| feature | new persisted state |
|---|---|
| [01](01-split-editor-groups.md) split groups | the split tree, orientations, ratios, per-group active tab |
| [02](02-layout-perspectives.md) perspectives | several named layouts, one default |
| [03](03-multi-monitor-floating.md) multi-monitor | monitor device name and working area per float |
| [04](04-maximise-and-zen.md) maximise | whether a maximise was active, and of what |

Persisting more state makes the failure modes worse, not better. A layout file that cannot be read is
one thing; one that **can** be read and produces a subtly wrong arrangement is what users report as
"it forgot my windows again".

## Design

**Versioned schema.** Stamp a version into the definition. On load, migrate forward through known
versions; on an unknown future version, fall back to defaults and say so rather than
half-interpreting it.

**Degrade, do not fail.** A layout referencing a panel that no longer exists — a removed tool, a
plugin not loaded — must place everything else and drop the missing one. Today's behaviour needs
establishing before it can be judged.

**Never lose the user's arrangement on a failed load.** If restore fails partway, restore the
built-in default rather than leaving a half-materialised tree.

**Separate "layout" from "session".** Which panels exist is layout; which was active and where the
caret sat is session. Products that conflated them are the ones where opening a second window
disturbs the first.

## Work

- [ ] Establish current behaviour: unknown panel id, truncated file, missing file, future version.
      Four inputs, four defined outcomes — several are probably undefined today
- [ ] Version stamp and a forward-migration path
- [ ] Degradation for missing panels, shared with [02](02-layout-perspectives.md)
- [ ] Atomic write — write to a temporary file and move, so a crash mid-save does not corrupt the
      previous good layout
- [ ] Round-trip fidelity for everything features 01–04 add

## Verification

- Save → load → save; assert the two serialised forms are **byte-identical**. Round-trip fidelity is
  the one property that catches silent field loss, and it costs nothing to assert
- Load a definition naming a panel that does not exist; assert the rest materialises
- Load a truncated file; assert the default layout, and an error reported rather than swallowed
- Load a version from the future; assert defaults and a clear report
- Kill the process mid-save (simulated by failing the write); assert the previous layout still loads

---

## Outcome

### The file-based half of this document does not apply

Three work items — truncated file, missing file, atomic write — presuppose a layout file. There
isn't one. `SaveLayout()` returns a `DockLayoutDefinition` **object** and `LoadLayout(def)`
materialises it; the designer serialises that object into the host's `*.Designer.cs` as property
assignments. There is nothing to truncate and nothing to write atomically.

That leaves the items that do apply, and they were where the defects were.

### Measured before judged, as this document required

| input | before | after |
|---|---|---|
| save → load → save | **byte-identical** ✔ | unchanged |
| unknown panel id | **skipped, rest placed** ✔ | unchanged |
| version from the future | **accepted silently** | reported, arrangement kept |
| panel absent from the definition | **detached group, no bounds** | rejoins a group |
| hidden panel round-trip | **lost entirely** | survives |
| definition that places nothing | **every panel stranded, 2 → 0** | 3 → 3 |

Two of six were already right. Four were not, and none of the four threw — every one of them
produced a plausible-looking layout that was quietly wrong, which is precisely the failure this
document opens by naming.

### Schema version 2

`SchemaVersion` was **written on save and never read on load** — the same written-but-never-read
shape as the five dead context flags in [09](09-dead-surface.md). A definition from a future build
was materialised as though it were current: the recognised parts applied, the rest silently dropped.

Now `DockLayoutDefinition.CurrentSchemaVersion` is the single authority, and a newer definition is
refused **before anything is torn down**, reported through `DockingError`, and the user's existing
arrangement is kept.

This document says to "fall back to defaults" here. Keeping the current arrangement is the faithful
reading of that in a manager that is already live — there is no startup default to fall back to, and
"never lose the user's arrangement on a failed load" is the stronger of the two instructions.

Version 2 adds `Hidden`. A version 1 definition loads unchanged, because an absent hidden list and
an empty one mean the same thing — so the forward migration is the identity. That is **asserted**,
not assumed: a v1 definition loads with 3/3 panels placed, 0 hidden, nothing reported.

### Hidden panels are members

[10](10-verification-harness.md) left this open: `PanelKeys` captured only `Docked` panels, so a
hidden panel vanished from the layout and did not come back when shown.

`PanelKeys` is now **membership** — floating, auto-hiding and closing all detach the panel from its
group, so `group.Panels` is already exactly the right set — and `Hidden` records which members are
hidden. With that, `GroupHasPersistableContent` and `GroupHasMembers` became the same question and
collapsed back into one predicate. The split introduced in [10](10-verification-harness.md) was
correct while the schema could not express a hidden panel, and stopped being correct the moment it
could.

### Teardown no longer strands panels

`MaterializeFromDefinition` removed and unregistered every group **before** rebuilding, leaving the
panels inside them pointing at groups no longer in the tree. Any panel the incoming definition did
not mention stayed `Docked`, unplaced and unreachable — the identical stranding shape as the
hide/show defect, from an identical cause: a group left the tree while a panel still referenced it.

Two changes: teardown clears `panel.Group`, and `ReHomeUnplacedPanels` gives any still-groupless
docked panel the group for its own `DockPosition`. A definition need not mention every registered
panel — it may predate one, or a plugin may have added panels since — and those panels are still
expected on screen.

### A correction worth recording

The future-version check reported "accepted silently" **after** the fix was in place. The fix was
fine; the check only watched for an exception, and the manager reports through `DockingError` — the
right choice for a restore, and invisible to a `try`/`catch`. It now asserts both halves of a
defined outcome: the caller is told, *and* the arrangement is intact. A baseline confirms a
current-version definition is still accepted, so the guard is not simply refusing everything.

`DockProbe`: **110 passed, 0 failed**. Solution builds with 0 errors.

### Remaining

- [ ] Round-trip fidelity for state features 02 and 03 will add — named perspectives, and a
      monitor device name per float
- [ ] Whether a maximise was active is deliberately **not** persisted: [04](04-maximise-and-zen.md)
      makes it transient controller state, and writing it would reintroduce the coupling that
      design exists to avoid. Restoring always lands unmaximised, which is a defined outcome
- [ ] Separate "layout" from "session" — this document's fourth design point, untouched
