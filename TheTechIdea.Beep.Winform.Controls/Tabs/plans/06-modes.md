# 06 — Modes

## Done — one resolution point

The mode was re-decided at **20 call sites across 11 files**, every one of them spelled
`TabMode == BeepTabMode.Navigation` or its negation. Nothing named what any individual guard
protected, so the contract could only be reconstructed by reading all twenty.

`BeepTabModeCapabilities` now states it once, with a member per feature it governs —
`SupportsPinning`, `SupportsMruOrdering`, `SupportsClosedTabHistory`, `SupportsPreviewTabs`,
`SupportsDirtyCloseGuard`, `SupportsDragReorder`, `SupportsTabContextMenu`. Each call site now reads
as the thing it is protecting rather than as a mode comparison. The harness fails if any file other
than the enum and the capability type names a mode directly.

**`BeepTabInputPolicy` deleted.** 115 lines whose own header said consumers should call it "instead
of scattering if/else guards across keyboard, mouse, and command handlers" — with **zero callers**,
while those scattered guards existed. It was a second implementation of both per-item permissions
(already computed inline in `Metadata` and `WorkspaceCommands`) and close-key handling (already in
`BeepTabs.Keyboard`, which handles `Delete` and `Ctrl+W`). Proven dead by deleting it and compiling.

## The open question: Documents and Workspace are the same mode

All twenty guards tested Navigation against not-Navigation. **The enum declares three modes and the
control implements two** — there is no code anywhere that distinguishes `Documents` from
`Workspace`. Setting one or the other today changes nothing.

This was *not* silently collapsed, unlike the overflow policies, and the difference is worth stating:
the three unimplemented overflow values were strictly harmful (they dropped tabs and suppressed the
menu that would have reached them), so removing them fixed a defect. Here, which behaviours *ought*
to differ — preview tabs, pinning, MRU, split groups — is a product decision that cannot be inferred
from the absence of code. Deleting `Workspace` would discard intent; inventing a split would invent
design.

`BeepTabModeCapabilities` documents the truth in one place, so whichever way it is decided, the
change is a handful of expressions in one type rather than an archaeology exercise across 11 files.

## Original findings
: Navigation / Documents / Workspace

**Priority P1.**

## Current behaviour

`Models/BeepTabMode.cs` declares three modes:

```csharp
public enum BeepTabMode { Navigation, Documents, Workspace }
```

Supporting machinery exists and is substantial:

- `BeepTabs.ClosedTabHistory.cs` + `Models/BeepTabClosedRecord.cs` — reopen-last-closed
  (`TryReopenLastClosedTab`, Ctrl+Shift+T), with a `TabReopenRequested` event
- `BeepTabs.WorkspaceCommands.cs` (313 lines) and `BeepTabs.WorkspaceMru.cs` (184 lines)
- `Helpers/BeepTabWorkspaceMruTracker.cs` (248 lines)
- `Models/BeepTabWorkspaceState.cs`
- `TabCloseRequested` with cancellation, documented as firing "before a dirty (unsaved) tab is closed
  in Documents or Workspace mode"
- `BeepTabQuickSwitch.cs` (315 lines) — a Ctrl+Tab style switcher with a filter box

That is a real feature set, not scaffolding. What is missing is any evidence of **what each mode
actually changes**. The enum is declared; the behavioural differences between Navigation, Documents
and Workspace are spread across several files and have never been stated in one place or tested.

This is the same shape as `ToolTipLayoutVariant` in the tooltip program: seven declared values, four
of which resolved to identical behaviour because nothing branched on them. That was only discovered
by rendering all seven side by side.

## What the reference products do

| Mode | Expected behaviour |
|---|---|
| Navigation | fixed set of views; no close, no reorder, no MRU; selection is the only state |
| Documents | dynamic set; close buttons, dirty markers, close-others/close-right, reopen-closed, preview (italic) tabs, MRU Ctrl+Tab ordering |
| Workspace | Documents plus grouping, pinning, split/dock targets, persisted layout across sessions |

Visual Studio and VS Code differ from a plain navigation tab strip in exactly these ways, and each
difference is observable.

## Work

1. **Write down the mode contract** — a table of every behaviour and which modes enable it. Until
   that exists, "does Workspace work?" has no answer.
2. **Make the mode authoritative**, the way `LayoutVariant` was made authoritative for tooltips:
   one resolution point that maps mode → enabled behaviours, consulted by close buttons, reorder,
   MRU, context menu, keyboard and persistence. Not `if (mode == ...)` scattered per feature.
3. **Verify each mode differs.** Set each mode with identical tabs and compare: affordances present,
   context-menu entries, keyboard bindings, persisted state.
4. **Confirm `TabCloseRequested` fires only where documented** — "Documents or Workspace mode" — and
   that Navigation genuinely has no close path.
5. **Confirm the workspace state actually round-trips**: save, restart, restore, and compare tab
   order, selection, pinned and group state.

## Verification

- Probe: for each of the three modes, dump the enabled-behaviour set and assert the three differ in
  the documented ways.
- Probe: reopen-last-closed restores order and selection.
- Probe: MRU ordering after a Ctrl+Tab cycle matches most-recently-used, not positional, order.
- Probe: workspace state survives a save/load round trip byte-for-byte on the fields that matter.
