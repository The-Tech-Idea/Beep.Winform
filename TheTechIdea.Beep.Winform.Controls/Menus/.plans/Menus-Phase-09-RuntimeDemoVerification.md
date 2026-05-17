# Menus — Phase 09: Runtime Demo Form + Verification Matrix

> Status: **Planned**
> Owner: Menus & ContextMenus program
> Tracker entry: `MASTER-TODO-TRACKER.md` → `MENU-P09`
> Predecessors: all prior phases shipped
> Estimated risk: **Low** (sample-only; no framework code)

---

## Goal

Ship a runtime acceptance form in `WinFormsApp.UI.Test` that demonstrates and verifies every behaviour the program introduced. Modelled on `DocumentHostMdiDemoForm` (DocumentHost MDI Phase 09): one form, one CLI flag, every scenario reproducible by a human in under two minutes.

This is the *final gate* on the menu program — when this demo shows all matrix rows green, the program ships.

---

## Demo Form: `MenusShowcaseForm`

Layout:
```
┌──────────────────────────────────────────────────────────────┐
│ BeepMenuBar (top) — File | Edit | View | Tools | Help        │
├──────────────────────────────────────────────────────────────┤
│ Toolbar:  [Open Right-Click Demo] [Switch ControlStyle ▼]    │
│           [Toggle High Contrast] [Reset Activation State]    │
├──────────────────────────────────────────────────────────────┤
│ Status panel — live state readouts:                          │
│   • Activation: Inactive / ActiveNoPopup / ActiveWithPopup   │
│   • Last MenuDismissed reason: ItemClicked                   │
│   • Last action invoked: File→Open                           │
│   • Cool-down swallowed count: 0                             │
│                                                              │
│ Live event log (scrolling textbox):                          │
│   12:34:56 OnMouseClick(File)                                │
│   12:34:56 MenuDismissed reason=ItemClicked                  │
│   …                                                          │
└──────────────────────────────────────────────────────────────┘
```

CLI launch:
```
dotnet run --project WinFormsApp.UI.Test -- --demo menus
```

---

## Design Decisions

| Concern | Decision | Rationale |
|---|---|---|
| **Single form, two surfaces** | Hosts `BeepMenuBar` (top) and a button to launch a right-click `BeepContextMenu` (mid-body). | Both surfaces share the same dismissal / lifecycle infrastructure. |
| **Live state readouts** | Subscribes to `BeepMenuBar.SelectedItemChanged`, `ContextMenuManager.MenuDismissed` (Phase 01), and the menubar's activation transitions to update labels in real time. | Lets the user *see* the fix working, not just believe it. |
| **Style cycle button** | Cycles through every `BeepControlStyle` value so the user can confirm rendering parity across styles. | Closes the "did the styling consolidation regress anything?" question. |
| **HC toggle button** | Toggles a synthetic HC mode (overrides `BeepMenuBar.HighContrast` partial's `_simulateHc` flag) to verify Phase 07 HC handling without needing to flip OS settings. | Demoable without admin / system settings. |
| **Verification matrix lives in this doc** | One canonical place. Each row is a manual test step with the expected observable result. | Manual checklist beats brittle UI tests for menu interactions. |
| **No reflection** | Uses only the public surfaces shipped by phases 01–08. | Validates that the public API is sufficient for real consumers. |

---

## TODO Checklist

### A — Demo form
- [ ] `MENU-P09-001` Create `WinFormsApp.UI.Test/MenusShowcaseForm.cs`. Compose the layout from the diagram above.
- [ ] `MENU-P09-002` Wire `BeepMenuBar.LoadSampleMenuItems()` plus three nested items under File (Open, Save, Save As…).
- [ ] `MENU-P09-003` Subscribe to `BeepMenuBar.SelectedItemChanged` → append to log + update "Last action invoked".
- [ ] `MENU-P09-004` Subscribe to `ContextMenuManager.MenuDismissed` (from Phase 01) → append to log + update "Last MenuDismissed reason" and "Cool-down swallowed count".
- [ ] `MENU-P09-005` Subscribe to the menubar's activation event from Phase 04A (or read it on a 100 ms timer if no public event exposed) → update "Activation" label.

### B — Right-click context menu surface
- [ ] `MENU-P09-006` Add a "Right-click here" panel. On right-click, call `ShowContextMenu(...)` with a representative item tree including a submenu.

### C — Style + HC toggles
- [ ] `MENU-P09-007` "Switch ControlStyle ▼" button cycles `_menuBar.ControlStyle` through `Material → Fluent → Tokyo → GNOME → KDE → Neumorphism → iOS15 → Classic → Material …`.
- [ ] `MENU-P09-008` "Toggle High Contrast" button flips an in-app simulated HC mode (Phase 07 exposes either an OS-level toggle or a `_simulateHc` toggle; use whichever shipped).
- [ ] `MENU-P09-009` "Reset Activation State" button calls `ContextMenuManager.CloseRootMenus()` and resets the local readouts to defaults.

### D — Program.cs route
- [ ] `MENU-P09-010` In `WinFormsApp.UI.Test/Program.cs`, add a `--demo menus` branch that does `ApplicationConfiguration.Initialize(); Application.Run(new MenusShowcaseForm());`.

### E — Verification (manual, against the matrix below)
- [ ] `MENU-P09-011` Walk each row of the matrix manually with the demo; check off each one.
- [ ] `MENU-P09-012` Build all three projects — 0 errors.

### F — Doc + tracker
- [ ] `MENU-P09-013` Update `MASTER-TODO-TRACKER.md`: `MENU-P09` Shipped; declare the program **complete**.

---

## End-to-End Verification Matrix

(The single source of truth for "is this program done?")

### Bug fix (Phase 01)
| # | Step | Expected | Status |
|---|---|---|---|
| 1 | Click "File" → click "Edit" | Edit popup opens; no flicker | ☐ |
| 2 | Click "File" → click empty area | File popup closes; no re-open | ☐ |
| 3 | Click "File" → click outside form | File popup closes; no re-open | ☐ |
| 4 | Click "File" → click "File" again | File popup closes (toggle) | ☐ |
| 5 | "Cool-down swallowed count" increments on test 1 | Counter increments by 1 | ☐ |

### Partial split + styling stack (Phases 02 + 03)
| 6 | Designer opens menubar | No errors | ☐ |
| 7 | Cycle every `BeepControlStyle` | Each renders cleanly; no missing border/shadow | ☐ |
| 8 | `Menus/Helpers/` no longer contains `IMenuBarPainter*` files | `Glob` confirms 0 | ☐ |

### Commercial UX (Phase 04)
| 9 | Alt → mnemonic underlines visible | ✅ | ☐ |
| 10 | Alt+F → File popup opens | ✅ | ☐ |
| 11 | Escape → popup closes; activation → Inactive | ✅ | ☐ |
| 12 | Click File → drag (no click) to Edit | Hover-swap; Edit popup replaces File | ☐ |
| 13 | Open Edit → Find submenu → drag diagonally to a Find item | Submenu stays open | ☐ |

### Lifecycle (Phase 05)
| 14 | `ShowNonBlocking` from a button | Returns instantly; handle disposable | ☐ |
| 15 | No `Application.DoEvents()` in `Show` path | Verified by inspection (and demo doesn't freeze) | ☐ |
| 16 | Right-click near secondary-monitor edge | Popup clipped to monitor | ☐ |

### BeepListBox substrate (Phase 06)
| 17 | Type-ahead "Op" in File popup → jumps to "Open" | ✅ | ☐ |
| 18 | Up/Down arrows navigate popup items | ✅ | ☐ |
| 19 | Item icons render via list-box image pipeline | ✅ | ☐ |

### Accessibility (Phase 07)
| 20 | Accessibility Insights shows `MenuBar` role with `MenuItem` children | ✅ | ☐ |
| 21 | NVDA announces "File menu" on Alt+F | ✅ | ☐ |
| 22 | Focus rectangle on keyboard-focused top-level | ✅ | ☐ |
| 23 | HC toggle changes hover/select colours | ✅ | ☐ |

### Designer (Phase 08)
| 24 | Smart-tag verbs visible | 5 verbs | ☐ |
| 25 | Tree collection editor supports nesting | ✅ | ☐ |
| 26 | VS Undo reverses tree edits | ✅ | ☐ |

---

## Files Touched

| File | Change |
|---|---|
| `WinFormsApp.UI.Test/MenusShowcaseForm.cs` | **New.** Demo form. |
| `WinFormsApp.UI.Test/Program.cs` | `--demo menus` branch. |
| `Menus/.plans/MASTER-TODO-TRACKER.md` | Mark program Shipped. |

---

## Deferred (out-of-program)
- Automated UI tests via WinAppDriver / FlaUI — separate test-infrastructure program.
- `BeepDropDownMenu` / `BeepFlyoutMenu` / `BeepSideMenu` parity demos — separate follow-up program if needed.

---

## On Program Completion

Once row 26 is green:
1. Mark every `MENU-P##` row Shipped in `MASTER-TODO-TRACKER.md`.
2. Update the tracker `## Shipped` section with a single-paragraph program summary noting:
   - The bug fix and how it was solved (dismissal swallow + cool-down).
   - Decomposition stats (`BeepMenuBar.cs` 971 LOC → 9 partials ≈ 80–270 LOC each).
   - Stale-framework removal stats (5 files deleted, 4 docs archived, 1 ADR added).
   - Commercial UX wins (toggle, hover-swap, mnemonics, submenu triangle, Alt activation).
   - Accessibility upgrade (UIA roles, focus rectangle, HC handling, screen-reader announcements).
   - Designer wins (5 verbs, tree editor, undo-aware mutations, popup preview).
3. Author a one-page program retrospective in `Menus/.plans/PROGRAM-RETROSPECTIVE.md` capturing learnings, surprises, and future ideas (e.g. `BeepDropDownMenu` rev-2 audit).
