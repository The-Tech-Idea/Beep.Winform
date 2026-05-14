# Phase 06 — Commands, Shortcuts, Window Menu, Ribbon

> **Owner:** _unassigned_  · **Status:** 🟧 Partial (A1–A4, B2, B3, C1–C2 done; B1 already in Phase 02; D, E, F deferred) · **Predecessor:** Phase 02

## Why This Phase Exists

Beep already ships:

- ✅ `BeepCommandRegistry` (runtime command map)
- ✅ Shortcut editor dialog
- ✅ Command palette (`Ctrl+Shift+P`)
- ✅ Quick-switch (`Ctrl+Tab`)
- ✅ Auto-populate "Window" menu via `AttachWindowMenu`
- ✅ Customise-shortcuts verb on the host designer

But these are **wired through the host directly**. Once the manager
(Phase 02) lands, every form-level concern should flow through the manager so
developers configure them once, in the property grid.

## Tasks

### A. Manager-level command surface

- [x] **A1** `BeepDocumentManager.CommandRegistry` property surface (read-only):
  exposes the host's `BeepCommandRegistry` via `(View as BeepTabbedView)?.Host?.CommandRegistry`.
  Returns `null` when the active view is not `BeepTabbedView`.
- [x] **A2** `BeepDocumentManager.RegisterCommand(string id, string title, Action callback, Keys shortcut)`
  forwarding to registry.
- [x] **A3** Designer verb **"Customize Keyboard Shortcuts…"** at manager level
  (opens `DocumentHostShortcutEditorDialog`; shows info prompt when view is not `BeepTabbedView`).
  Also available in the smart-tag "Commands" group.
- [x] **A4** `[Browsable(false)]` on the host-level verb when a manager is bound:
  `BeepDocumentHostDesigner.Verbs` checks `HasBoundManager()` and sets
  `_shortcutsVerb.Enabled = false` so the host entry is greyed out, eliminating
  duplication in the right-click context menu.

### B. Window menu — declarative

- [ ] **B1** Manager prop `WindowMenuOwner` already exists (Phase 02 B1).
- [x] **B2** Auto-populate items at runtime:
  - [x] One item per open document (max 9 with `&1…&9` mnemonics, then plain text)
  - [x] Separator
  - [x] _Close All_, _Close All But This_, _Close All to the Right_
  - [x] Separator — _Save Layout As…_ / _Load Layout…_ / _Reset Layout_
  - [ ] _Tile Horizontal_ / _Tile Vertical_ / _Cascade_ (NativeMdi only — deferred to Phase 05)
- [x] **B3** Honour `AllowSplit` policy — split group actions already check `_allowSplit`; tile/cascade deferred to Phase 05 where NativeMdi gets its own menu section.

### C. Status-strip integration

- [x] **C1** `StatusStripOwner` populates 3 default items (see Phase 05 G).
- [x] **C2** Opt-out flag `AutoPopulateStatusStrip`.

Current status: document title + dirty indicator + cursor position are
auto-populated by the manager, and the workspace dropdown is layered on when a
tabbed host is bound. Cursor reporting is automatic for `TextBoxBase` /
`RichTextBox`; custom content can implement `IDocumentStatusInfoProvider`.

### D. Command palette polish

- [x] **D1** Global `IMessageFilter` (`HostKeyFilter` nested class in `Keyboard.cs`)
  intercepts `Ctrl+Shift+P` / `Ctrl+P` even when focus is on a MenuStrip,
  ToolStrip, floating window, or any control outside the host hierarchy.
  Installed in `OnHandleCreated` (runtime only); removed in `Dispose`.
- [x] **D2** Fuzzy match — verified. `BeepCommandRegistry.FuzzySearch` applies
  fuzzy scoring; results ordered by relevance.
- [x] **D3** Recently-used commands at top — verified. `FuzzySearch("")` returns
  commands sorted by `UsageCount desc, LastUsed desc`. `RecordUsage` is called in
  both `ExecuteCommandById` and `BeepCommandPalettePopup` on commit.

### E. Quick-switch polish

- [x] **E1** `Ctrl+Tab` / `Ctrl+Shift+Tab` MRU cycling. `BeepDocumentHost.ProcessCmdKey`
  intercepts the chord before the tab strip sees it (when ≥ 2 docs are open) and calls
  `ShowQuickSwitchMru(reverse)`. The MRU-sorted list is built from `_mruList`; the
  second-most-recent entry is pre-selected on first press. Inside the popup,
  `ProcessCmdKey` maps `Ctrl+Tab` / `Ctrl+Shift+Tab` to list-cycle so holding Ctrl and
  repeatedly pressing Tab steps through the MRU list.  `BeepDocumentQuickSwitch` also
  accepts an `initialIndex` constructor parameter so the pre-selection is set by index
  rather than document ID.
- [x] **E2** Live preview pane — verified implemented. `BeepDocumentQuickSwitch`
  has `_preview`, `_previewIcon`, `_previewTitle`, `_previewPath`, `_previewMeta`
  panels; updated on selection change.
- [x] **E3** Quick-switch verify pass — done.

### F. Ribbon-style command bar (optional)

- [ ] **F1** If the host is sited under a `BeepRibbon` control, auto-discover
  manager commands and surface them as ribbon buttons in a "Documents" tab.
- [ ] **F2** Hide if no ribbon present.
- [ ] **F3** Defer if behind schedule — it's a polish item, not a blocker.

## Acceptance Criteria

- ✅ Manager exposes all commands through one property surface.
- ✅ Setting `WindowMenuOwner` on the manager auto-builds the Window menu.
- ✅ Shortcut editor works whether opened from manager or host designer.
- ✅ Command palette works without any code in `MainFrm_MDI.cs`.

## Out of Scope

- Replacing `BeepCommandRegistry` internals (it's fine as-is).
- New keyboard shortcuts beyond what's already wired in `BeepDocumentHost.Events.cs`.

## Notes

- Verify that `Ctrl+Alt+Left/Right`, `Ctrl+Shift+W`, `Ctrl+Shift+M` (added in the
  previous polish session) are visible in the shortcut editor and can be remapped.
