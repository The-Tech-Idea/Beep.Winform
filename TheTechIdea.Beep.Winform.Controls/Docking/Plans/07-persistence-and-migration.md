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
