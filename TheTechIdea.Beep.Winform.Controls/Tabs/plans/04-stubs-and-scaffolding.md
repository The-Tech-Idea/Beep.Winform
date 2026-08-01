# 04 — Stubs & Empty Scaffolding

**Priority P0.** No stubs is a stated rule for this control.

## Current behaviour

### A public no-op

```csharp
/// <summary>Applies font theme — no-op (fonts are resolved at paint time).</summary>
public static void ApplyFontTheme(BeepControlStyle controlStyle) { }
```

`TabFontHelpers.ApplyFontTheme` has an empty body. It is public, it is named as though it does
something, and its summary admits it does not. Any caller reading the signature reasonably concludes
fonts are being themed; nothing happens.

Call sites must be checked before deleting — if something calls it, that call site believes it is
applying a theme and is therefore also wrong.

### Empty folders

- **`Tabs/Adapters/`** contained no files. The deleted plan described adapters as "temporary
  internal seams, not long-term premium API"; the seam was removed and the folder outlived it.
  Deleted while writing this plan.
- **`Tabs/plans/`** existed but was empty, while the real documents lived in a hidden `.plans/`
  folder. Both are now resolved: `.plans/` is deleted and this folder holds the program.

### Nine stale documents

`.plans/` held six documents plus `Readme.md`, `TABS_ENHANCEMENT_SUMMARY.md` and
`TABS_FLICKER_FIX_PLAN.md`. The `.plans/README.md` described itself as "the active commercialization
plan as of May 2026" and stated the premium cutover was "still in progress" — with no way to tell
which parts had since been done. All nine are deleted; this program replaces them.

## Why this matters beyond tidiness

A stub is worse than a missing method. A missing method is a compile error at the call site; a stub
compiles, runs, does nothing, and pushes the failure somewhere distant and hard to attribute. The
same reasoning applies to the six never-read configuration properties found in `ToolTips` — declared
capability that does not exist is a defect, not incompleteness.

## Work

1. **Resolve `ApplyFontTheme`.** Find its callers. If none, delete it. If some, either implement it
   or delete it *and* fix those call sites — they are currently relying on nothing.
2. **Sweep for other no-ops.** Every method whose body is empty or which only returns a constant,
   audited: intentional (an interface default, an event hook) or a stub.
3. **Sweep for capability that does not exist.** Public properties on `BeepTabs`, `BeepTabItem`,
   `BeepTabPage` and the models that nothing reads — the same reflection check that found six in
   `ToolTips` applies unchanged here.
4. **No new empty folders.** If a folder is created for planned work, the work lands in the same
   change or the folder does not exist yet.

## Verification

- Harness fails if any method under `Tabs/` has an empty body without an explicit
  `// intentionally empty: <reason>` justification.
- Harness fails if a public property on the tab models is never read anywhere in the assembly.
- No empty directories under `Tabs/`.
