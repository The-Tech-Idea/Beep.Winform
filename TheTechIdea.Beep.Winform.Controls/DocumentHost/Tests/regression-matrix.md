# BeepDocumentHost — Manual Smoke-Test Regression Matrix

> Run this checklist after any non-trivial change to `BeepDocumentHost`,
> `BeepDocumentManager`, `BeepDocumentTabStrip`, or layout persistence.
> Mark each row **Pass / Fail / N/A**.

---

## 1 — Core document lifecycle

| # | Test | Expected | Result |
|---|------|----------|--------|
| 1.1 | `manager.AddDocument("A")` | Tab "A" appears, document is active | |
| 1.2 | `manager.AddDocument("B", activate: false)` | Tab "B" appears but is **not** activated | |
| 1.3 | `manager.ActivateDocument(id)` | Target tab becomes active | |
| 1.4 | Click × on an unmodified tab | Tab closes, next-in-MRU becomes active | |
| 1.5 | Click × on a modified tab — handler sets `e.Cancel = true` | Tab stays open | |
| 1.6 | `manager.RemoveDocument(id, force: true)` | Closed without cancel | |
| 1.7 | `manager.CloseAllDocuments()` | All tabs removed | |

---

## 2 — Duplicate-safe document open

| # | Test | Expected | Result |
|---|------|----------|--------|
| 2.1 | Call `AddDocument("A")` twice with same title | Second call activates first; no duplicate | |
| 2.2 | Call `AddDocument` with same explicit ID twice | Second call activates first | |

---

## 3 — Keyboard shortcuts

| # | Shortcut | Expected | Result |
|---|----------|----------|--------|
| 3.1 | `Ctrl+Tab` with ≥ 2 tabs | MRU quick-switch popup appears | |
| 3.2 | Hold `Ctrl`, press `Tab` twice | Second-MRU item is selected | |
| 3.3 | `Ctrl+Shift+Tab` | Popup appears, last-MRU item selected | |
| 3.4 | `Ctrl+W` | Active tab closes | |
| 3.5 | `Ctrl+F4` | Active tab closes | |
| 3.6 | `Ctrl+1` | First tab activates | |
| 3.7 | `Ctrl+9` | Last tab activates | |
| 3.8 | `Ctrl+Shift+T` | Last closed tab reopens | |
| 3.9 | `Ctrl+P` | Command palette opens | |

---

## 4 — Split-view and groups

| # | Test | Expected | Result |
|---|------|----------|--------|
| 4.1 | Drag tab to right drop zone | View splits horizontally; both groups visible | |
| 4.2 | Drag tab to bottom drop zone | View splits vertically | |
| 4.3 | Move splitter bar | Both group areas resize correctly | |
| 4.4 | Close all tabs in one group | Group collapses; remaining group fills space | |
| 4.5 | Split 4 times (depth = 4) | `MaxSplitDepth` prevents further splitting | |

---

## 5 — Float and dock-back

| # | Test | Expected | Result |
|---|------|----------|--------|
| 5.1 | Drag tab vertically out of strip | Float window appears | |
| 5.2 | Drop float window onto drop zone | Tab docks back into host | |
| 5.3 | Close float window via × | Tab removed from `_panels` | |
| 5.4 | Cross-host drag (two `BeepDocumentHost` instances) | Tab transfers to target host | |

---

## 6 — Layout persistence

| # | Test | Expected | Result |
|---|------|----------|--------|
| 6.1 | `manager.SaveLayout()` | JSON file created at `SessionFile` path | |
| 6.2 | `manager.RestoreLayout(json)` from v3 | Tabs, split ratio, and active doc restored | |
| 6.3 | `manager.RestoreLayout(json)` from v2 payload | Migration runs; layout restored | |
| 6.4 | `manager.RestoreLayout(json)` from v1 payload | Migration chain v1→v2→v3; layout restored | |
| 6.5 | `AutoSaveLayout = true` + form close | `SessionFile` updated | |
| 6.6 | Reopen form with `AutoSaveLayout = true` + existing file | Layout restored from file | |

---

## 7 — Workspaces

| # | Test | Expected | Result |
|---|------|----------|--------|
| 7.1 | `manager.SaveWorkspace("Coding")` | Entry in workspace list | |
| 7.2 | `manager.SwitchWorkspace("Coding")` | Layout matches saved snapshot | |
| 7.3 | `manager.DeleteWorkspace("Coding")` | Removed from list | |
| 7.4 | `manager.GetAllWorkspaces()` | Returns saved entries | |
| 7.5 | Status-bar workspace button → click | Workspace picker popup appears | |

---

## 8 — Shell integration (manager)

| # | Test | Expected | Result |
|---|------|----------|--------|
| 8.1 | `manager.WindowMenuOwner = menuStrip` | `Window` menu item appears and populates | |
| 8.2 | Add/close documents | Window menu items update | |
| 8.3 | `manager.StatusStripOwner = statusStrip` | Document title label appears | |
| 8.4 | Switch active document | Status-bar title updates | |
| 8.5 | Mark document as modified | Dirty `●` appears in status bar | |

---

## 9 — Theme propagation

| # | Test | Expected | Result |
|---|------|----------|--------|
| 9.1 | `manager.ThemeName = "Dark"` at runtime | All tabs and panels repaint with dark colours | |
| 9.2 | Switch theme to `"Light"` | Repaint with light colours | |
| 9.3 | Add new document after theme change | New tab appears in current theme | |

---

## 10 — Accessibility

| # | Test | Expected | Result |
|---|------|----------|--------|
| 10.1 | Tab with screen reader enabled | Tab title is narrated on activation | |
| 10.2 | Keyboard-only navigation (Tab key within host) | Focus cycles predictably | |

---

## 11 — Designer safety

| # | Test | Expected | Result |
|---|------|----------|--------|
| 11.1 | Open form in VS Designer | No designer crash | |
| 11.2 | Drop `BeepDocumentManager` onto component tray | Properties grid shows all properties | |
| 11.3 | Set `View` in Properties grid | No designer exception | |
| 11.4 | "Edit Documents…" smart tag | Collection editor opens | |
| 11.5 | Close and reopen Designer | Settings persisted correctly | |

---

## 12 — Disposal and leak check

| # | Test | Expected | Result |
|---|------|----------|--------|
| 12.1 | Close form after adding 10 documents | No `ObjectDisposedException` in logs | |
| 12.2 | `manager.Dispose()` explicit | All event subscriptions unhooked | |
| 12.3 | Float windows closed before form close | Float windows disposed | |

---

## Sign-off

| Tester | Date | Build | OS | Notes |
|--------|------|-------|----|-------|
| | | | | |
