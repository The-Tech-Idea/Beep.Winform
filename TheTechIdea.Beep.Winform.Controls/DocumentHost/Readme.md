# Beep DocumentHost — Architecture Guide

**Question this document answers:** *Why three components — `BeepDocumentHost`, `BeepTabbedView`, `BeepDocumentManager` — and what is the relationship between them?*

The short answer: this is the **DevExpress / Telerik / Syncfusion MDI pattern** transplanted into the Beep ecosystem. One non-visual orchestrator drives an interchangeable rendering strategy, which in turn drives an interchangeable visual surface. Each layer has exactly one responsibility, and you can swap any layer without rewriting the others.

The long answer is below.

---

## 1. The three components at a glance

| Component | Kind | Lives in | Responsibility |
|---|---|---|---|
| **`BeepDocumentManager`** | Non-visual `Component` | Form's **component tray** | Public API surface for application code. Hosts cross-cutting services (command registry, workspace manager, design-time documents, layout persistence, theme/policy fan-out, `IDisplayContainer` contract). Never paints. Never knows about tabs, splitters, or MDI. |
| **`IBeepDocumentManagerView`** | Strategy interface | Implemented by view components | The rendering-strategy seam. Two implementations ship today (`BeepTabbedView`, `BeepNativeMdiView`); third-party views can be added without changing the manager. |
| **`BeepTabbedView`** | Non-visual `Component` | Component tray | Concrete view: delegates every document operation to an associated `BeepDocumentHost`. Lightweight adapter — holds a `Host` reference, wires events, forwards method calls. |
| **`BeepNativeMdiView`** | Non-visual `Component` | Component tray | Concrete view: uses standard WinForms MDI (`Form.IsMdiContainer = true`, child Forms with `MdiParent` set). No `BeepDocumentHost` involved. |
| **`BeepDocumentHost`** | Visual `BaseControl` | **On the form** (Dock=Fill) | Actually paints the tabs, splitters, dock rails, floating windows, and content panels. Renders everything you see when you choose tabbed mode. |

---

## 2. The composition diagram

### Tabbed mode (the default)

```
┌─ MainForm ─────────────────────────────────────────────┐
│ ┌────────────────────────────────────────────────────┐ │
│ │ BeepMenuBar             (Dock = Top)               │ │
│ ├────────────────────────────────────────────────────┤ │
│ │ ┌─ BeepDocumentHost  (Dock = Fill, SendToBack) ──┐ │ │
│ │ │  ┌─ BeepDocumentTabStrip  (Dock = Top) ─────┐  │ │ │
│ │ │  │ [ Doc1 ] [ Doc2 ] [ + ]   [Workspaces] │  │ │ │
│ │ │  └────────────────────────────────────────┘  │ │ │
│ │ │  ┌─ Content area (BeepDocumentPanel × N) ──┐ │ │ │
│ │ │  │           (splitter bars + dock rails)  │ │ │ │
│ │ │  └─────────────────────────────────────────┘ │ │ │
│ │ └──────────────────────────────────────────────┘ │ │
│ ├────────────────────────────────────────────────────┤ │
│ │ Status bar              (Dock = Bottom)            │ │
│ └────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────┘
   Component tray
   ├─ beepDocumentManager1   ── .View ───┐
   ├─ beepTabbedView1        ◄───────────┘
   │     └─ .Host ──► beepDocumentHost1  (the visual control above)
   └─ beepCommandRegistry1   (auto-created by the manager)
```

**Reading the wire-up:**

- `beepDocumentManager1.View` = `beepTabbedView1`
- `beepTabbedView1.Host` = `beepDocumentHost1`
- `beepDocumentHost1` is the visible control on the form

This is exactly the **DevExpress XtraDockManager → DockPanel** pattern. The manager is the API; the view is the strategy; the host is the canvas.

### Native MDI mode

```
┌─ MainForm  (IsMdiContainer = TRUE) ────────────────────┐
│ ┌────────────────────────────────────────────────────┐ │
│ │ BeepMenuBar             (Dock = Top)               │ │
│ ├────────────────────────────────────────────────────┤ │
│ │                                                    │ │
│ │  ┌─ MDI Child Form ────┐   ┌─ MDI Child Form ──┐  │ │
│ │  │ Document 1          │   │ Document 2        │  │ │
│ │  │                     │   │                   │  │ │
│ │  └─────────────────────┘   └───────────────────┘  │ │
│ │                                                    │ │
│ └────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────┘
   Component tray
   ├─ beepDocumentManager1   ── .View ───┐
   └─ beepNativeMdiView1     ◄───────────┘
         └─ .ParentForm ──► MainForm
```

There is **no `BeepDocumentHost` in native MDI mode** — the form's MDI client area replaces it. Each `AddDocument(...)` call creates a child `Form` whose `MdiParent` is set.

---

## 3. Why three components and not one giant control?

Combining all three concerns into a single class would lock you into a single rendering strategy and a single API surface. The split is deliberate:

### a) Single Responsibility

- **`BeepDocumentManager`** answers the question *"what is the application doing?"* — adding documents, switching workspaces, saving layout, applying themes. It has no opinion on how documents are drawn.
- **`IBeepDocumentManagerView`** answers the question *"how should this be drawn?"* — tabs and splitters? Native MDI children? A custom Windows-UI-style view? Each is a complete strategy.
- **`BeepDocumentHost`** answers the question *"what does a tabbed canvas look like and how does it behave?"* — paint chrome, hit-test tabs, handle drags, manage splits.

### b) Pluggable rendering strategy

Today the project ships two views. Tomorrow you can add a `BeepWindowsUIView` (Windows 11 tab groups), a `BeepStackedView` (single-document-at-a-time), or a custom corporate view, without touching `BeepDocumentManager` or any code that consumes it.

### c) DevExpress / Telerik / Syncfusion parity

Every commercial WinForms MDI suite uses this exact split:

| Vendor | "Manager" (tray) | "View / Strategy" | "Host / Surface" |
|---|---|---|---|
| **Beep** | `BeepDocumentManager` | `IBeepDocumentManagerView` (`BeepTabbedView` / `BeepNativeMdiView`) | `BeepDocumentHost` |
| **DevExpress** | `XtraDockManager` / `DocumentManager` | `BaseView` (`TabbedView` / `NativeMdiView` / `WindowsUIView`) | Dock panels / Document hosts |
| **Telerik** | `RadDock` | `DockingStrategy` (auto-hide / tabbed / floating) | `RadPaneGroup` / `RadSplitContainer` |
| **Syncfusion** | `DockingManager` | view-mode enum | `DockingClientPanel` |

A developer arriving from any of those suites already knows the mental model.

### d) Designer ergonomics

Non-visual components live in the **tray**, so the form designer surface stays clean. Properties like `View` show as a dropdown in the property grid; the smart-tag on `BeepDocumentManager` offers a wizard. A monolithic control would clutter the form with every conceivable property at once.

### e) Code-behind ergonomics

Application code only touches `BeepDocumentManager`:

```csharp
var panel = beepDocumentManager1.AddDocument("Report.pdf", iconPath: "pdf.svg", activate: true);
panel?.Controls.Add(myReportViewer);

beepDocumentManager1.SaveLayout();
beepDocumentManager1.ActiveDocumentChanged += OnActiveDocumentChanged;
```

You never write `beepTabbedView1.Host.…` or talk to the host directly. Swap to native MDI later and the same code still works.

---

## 4. The dependency chain

```
[Your Code] ──API──► BeepDocumentManager
                        │
                        ▼  IBeepDocumentManagerView
                  ┌──────────────────────────────────┐
                  │                                  │
            BeepTabbedView                  BeepNativeMdiView
                  │                                  │
                  ▼ Host                             ▼ ParentForm
          BeepDocumentHost                   WinForms Form
        (renders tabs+splits)            (IsMdiContainer = true)
```

**Top to bottom:**

1. Your code calls `manager.AddDocument(...)`.
2. The manager calls `View.AddDocument(...)` on the assigned `IBeepDocumentManagerView`.
3. The view does whatever its strategy says — `BeepTabbedView` forwards to `_host.AddDocument(...)`; `BeepNativeMdiView` constructs a child `Form` and sets `MdiParent`.
4. `BeepDocumentHost` (only in the tabbed path) paints the new tab + panel and raises `DocumentAdded`.

**Bottom to top (events):**

1. User clicks a tab close button on `BeepDocumentHost`.
2. Host raises `DocumentClosing` → forwarded by `BeepTabbedView` → forwarded by `BeepDocumentManager`.
3. Your event handler runs and may set `e.Cancel = true`.

This is the [GoF Strategy pattern](https://refactoring.guru/design-patterns/strategy) plus a [Mediator](https://refactoring.guru/design-patterns/mediator).

---

## 5. Quick start (designer)

The designer does all this automatically — drop **one** `BeepDocumentManager` and the wizard handles the rest. For completeness, here's what the wizard wires up under the hood:

1. Drag `BeepDocumentManager` from the toolbox onto your form.
2. The **Setup Wizard** auto-opens. Pick **Tabbed Documents** (or Browser Tabs, or Native MDI).
3. Click **Finish**.
4. The designer:
   - creates `beepTabbedView1` in the tray
   - sets `beepDocumentManager1.View = beepTabbedView1`
   - creates `beepDocumentHost1` on the root form with `Dock = Fill` (sent to back so docked siblings claim their edges)
   - sets `beepTabbedView1.Host = beepDocumentHost1`
   - optionally seeds N sample documents

Re-running the wizard **reuses** all of these components — it never creates `beepTabbedView2`, `beepDocumentHost2`, etc.

---

## 6. Quick start (code-behind)

```csharp
// Manager owns everything. No need to touch view or host once configured.
beepDocumentManager1.AddDocument(
    title:    "Sales Q3",
    iconPath: "chart.svg",
    activate: true);

beepDocumentManager1.ActiveDocumentChanged += (s, e) =>
    statusLabel.Text = $"Active: {e.DocumentId}";

beepDocumentManager1.DocumentClosing += (s, e) =>
{
    if (HasUnsavedChanges(e.DocumentId))
        e.Cancel = !PromptToSave(e.DocumentId);
};

beepDocumentManager1.SaveLayout();                      // writes to SessionFile
beepDocumentManager1.LoadLayout();                      // restores on next launch
```

To switch rendering strategy at runtime:

```csharp
var native = beepDocumentManager1.ChangeView<BeepNativeMdiView>();
native.ParentForm = this;            // 'this' will become an MDI parent
this.IsMdiContainer = true;
```

The manager's public API is identical regardless of view; only the visual outcome changes.

---

## 7. Component reference

### `BeepDocumentManager` — the orchestrator

Primary public API surface. Drop one per form. Owns:

- **Document lifecycle** — `AddDocument`, `CloseDocument`, `ActivateDocument`, `CloseAllDocuments`, `GetPanel`.
- **Layout persistence** — `SaveLayout`, `LoadLayout`, `SessionFile`, `AutoSaveLayout`.
- **Theme + policy fan-out** — `PushTheme(themeName)`, `DefaultPolicy` (Float / Pin / Split flags pushed into the view).
- **Workspaces** — named layout snapshots, surfaced through `WorkspaceManager`.
- **Command registry** — `CommandRegistry` (used by `BeepDocumentHost` for shortcut routing and the keyboard-shortcut editor dialog).
- **Window menu** — `AttachWindowMenu(menuStrip, "&Window")` populates a "Window" menu with open documents.
- **`IDisplayContainer` parity** — `SetTabStylePreset(style)`, `ApplyBrowserPreset()`, `ApplyIdePreset()`. Lets the manager be a drop-in replacement for the legacy `BeepDisplayContainer`.
- **Events** — `DocumentAdded`, `DocumentRemoved`, `ActiveDocumentChanged`, `DocumentClosing`, `LayoutChanged`.

What it does *not* do: paint. It has no `OnPaint`, no `Controls`, no canvas — it's a `System.ComponentModel.Component`.

### `IBeepDocumentManagerView` — the strategy

Tiny interface. 14 methods + 4 events. Defines exactly what the manager needs from a view:

- `Attach(manager)` / `Detach()` — lifecycle hooks.
- `AddDocument`, `RemoveDocument`, `ActivateDocument`, `GetPanel`, `CloseAllDocuments`, `BeginBatchAddDocuments` / `EndBatchAddDocuments`.
- `SaveLayout`, `SaveLayoutToFile`, `TryRestoreLayout`.
- `PushPolicy`, `PushTheme`, `PushPersistence`, `AttachWindowMenu`.
- Events: `DocumentAdded`, `DocumentRemoved`, `ActiveDocumentChanged`, `DocumentClosing`.

To add a custom view, implement this interface. The manager will route everything through it.

### `BeepTabbedView` — the default view

~217 LOC. Pure adapter — every method forwards to its `Host`. The interesting state is just the `Host` reference and event re-routing. Add this to the tray when you want the tabbed-docking experience.

### `BeepNativeMdiView` — the native-MDI view

~437 LOC. Manages a dictionary of `Form` instances keyed by document id. Each `AddDocument` creates a new `Form`, sets `MdiParent = ParentForm`, fires `DocumentFormCreated` so the caller can add the actual content. No `BeepDocumentHost` is involved — the form's MDI client area replaces it. `LayoutMdi(Cascade / Tile / ArrangeIcons)` is exposed for the Window-menu integration.

### `BeepDocumentHost` — the visual surface

The big one (60+ partials, ~6000 LOC total across them). Only relevant in the tabbed path. Responsible for:

- **Tab strip** (`BeepDocumentTabStrip` + its partials: painting, keyboard, mouse, touch, drag, animations, badges, overflow, context menu, accessibility, high contrast).
- **Content panels** (`BeepDocumentPanel`).
- **Splits + dock tree** (`BeepDocumentHost.Layout.cs`, `BeepDocumentGroup`, `BeepDocumentSplitterBar`).
- **Dock rails + auto-hide** (`BeepDocumentHost.AutoHide.cs`, `BeepDockRail`).
- **Dock overlay during drag** (`BeepDocumentDockOverlay`).
- **Floating windows** (`BeepDocumentFloatWindow`).
- **Layout serialization** (`BeepDocumentHost.Serialisation.cs`).
- **Templates** (`BeepDocumentHost.Templates.cs` — Visual Studio / two-pane / browser presets).
- **Keyboard / command service** (`BeepDocumentHost.Keyboard.cs`, `BeepCommandRegistry`).
- **Data binding + MVVM** (`BeepDocumentHost.DataBinding.cs`, `BeepDocumentHost.MVVM.cs`).
- **Workspaces** (`BeepDocumentHost.Workspace.cs`).
- **Preview** (`BeepDocumentHost.Preview.cs`).

End users **never instantiate `BeepDocumentHost` directly in code-behind**. The designer auto-creates it; runtime code talks to `BeepDocumentManager`. Direct host API access is reserved for advanced customization (e.g. registering a template, calling `ApplyVisualStudioLayout()`).

---

## 8. Decision tree — which mode to pick?

```
Do you need standard WinForms MDI child Forms
(separate top-level windows nested in the parent form)?
├── YES → Native MDI mode
│         └── BeepDocumentManager + BeepNativeMdiView
│             (Form.IsMdiContainer = true required)
│
└── NO  → Tabbed / dockable documents
          ├── Want IDE-style (splits, floats, pinning, themes)?
          │      └── BeepDocumentManager + BeepTabbedView + BeepDocumentHost
          │          (the wizard's default)
          │
          └── Want browser-style (Chrome tabs, + button, always-close)?
                 └── BeepDocumentManager + BeepTabbedView + BeepDocumentHost
                     with TabStyle=Chrome, ShowAddButton=true, CloseMode=Always
                     (the wizard's "Browser Tabs" preset)
```

| Want | Pick |
|---|---|
| Visual Studio / Office-style document area | Tabbed Documents |
| Chrome / Edge tabbed browser feel | Browser Tabs |
| Classic multi-window MDI (separate child windows) | Native MDI |
| Single document at a time | Either tabbed mode with `MaxOpenDocuments = 1`, or skip the manager entirely |

---

## 9. What lives in this folder

This is one of the largest sub-systems in `TheTechIdea.Beep.Winform.Controls`. Files break into themes:

| Prefix | Theme |
|---|---|
| `BeepDocumentManager.*` | Manager partials (`.cs`, `.Presets.cs`, `.DisplayContainer.cs`) |
| `BeepTabbedView.cs`, `BeepNativeMdiView.cs` | View strategies |
| `BeepDocumentHost.*` | Visual host partials — Layout / Documents / Events / Keyboard / DataBinding / MVVM / Workspace / Serialisation / Templates / Preview / DockRails / AutoHide |
| `BeepDocumentTabStrip.*` | Tab strip partials — Properties / Layout / Painting / Mouse / Keyboard / Touch / Animations / Accessibility / HighContrast / Badges / Overflow / ContextMenu |
| `BeepDocument*.cs` | Internal pieces — `BeepDocumentPanel`, `BeepDocumentTab`, `BeepDocumentGroup`, `BeepDocumentSplitterBar`, `BeepDockRail`, `BeepDockManager`, etc. |
| `BeepCommand*.cs` | Command registry + palette popup |
| `BeepWorkspace*.cs` | Workspace switcher / manager |
| `DocumentDescriptor.cs`, `DocumentOpenOptions.cs`, `DockPanelDescriptor.cs`, `WorkspaceDefinition.cs` | DTOs |
| `IBeepDocumentManagerView.cs`, `IDocumentHostCommandService.cs`, `IDocumentViewModel.cs`, `IDocumentStatusInfoProvider.cs` | Contracts |
| `*EventArgs.cs` | Event payload types |

Detailed phase-by-phase development history lives in `.plans/` at the repository root (`.plans/DocumentHost-MDI-Phase-*`) and in the Beep MASTER tracker.

---

## 10. Anti-patterns (what to avoid)

- ❌ **Don't instantiate `BeepDocumentHost` from code and parent it to the form manually.** The designer's auto-create flow handles this and produces proper `IComponentChangeService` notifications. Hand-parenting bypasses undo and serialization.
- ❌ **Don't talk to `BeepDocumentHost` directly from app code.** Talk to `BeepDocumentManager`. The host is the rendering surface; the manager is the API.
- ❌ **Don't create more than one `BeepDocumentManager` per form.** The manager owns global state (command registry, session file, active document). Multiple managers on one form will fight.
- ❌ **Don't set `BeepDocumentManager.View` to a view that's already assigned to another manager.** Each view has a single manager parent.
- ❌ **Don't put `BeepDocumentHost` inside another `Dock=Fill` control without thinking about layout.** Drop it directly on the root form (or a designed container) so the docked menubar / status bar can claim their edges.

---

## 11. Design-time tab selection (Phase 11)

Clicking a tab header in the designer behaves exactly like clicking a tab at runtime:

1. The runtime `BeepDocumentTabStrip` receives the mouse message (the host designer's `GetHitTest` override claims pass-through for any point landing on a tab body, the add/scroll/overflow buttons, or a group header).
2. The strip raises `TabSelected` → the host raises `ActiveDocumentChanged`.
3. The host designer subscribes to `ActiveDocumentChanged` and forwards the new active `BeepDocumentPanel` into `ISelectionService.SetSelectedComponents`.
4. The WinForms property grid swaps to the panel's filtered `BeepDocumentPanelDesigner` properties (`DocumentTitle`, `IconPath`, `CanClose`, `DocumentCategory`, `ShowStatusBar`).
5. The designer selection rectangle is drawn around the active panel, and any subsequent toolbox drop targets that panel via the existing `OnDragDrop` routing in `BeepDocumentHostDesigner.DragDrop.cs`.

When a form is first opened, the host designer also runs a one-shot `SyncInitialActiveDocumentSelection()` so the property grid lands on the visible document tab instead of the host shell — but only if the user has not already selected something else.

If a hosting environment swallows the click before `GetHitTest` is consulted, two escape hatches are wired:

- **Right-click → "Select Tab Under Cursor"** (added to the host designer verbs).
- **Smart-tag → Documents → "Select Tab Under Cursor"** (added to `DocumentHostActionList`).

Both delegate to `BeepDocumentHostDesigner.SelectDocumentAt(Cursor.Position)`, which hit-tests every group's `TabStrip.Tabs[i].TabRect` and calls `host.SetActiveDocument(tabId)` directly. From there the same `ActiveDocumentChanged` pipeline promotes the panel into the selection service.

See `.plans/DocumentHost-MDI-Phase-11-DesignTimeTabSelection.md` for the full breakdown, decision rationale, and verification matrix.

---

## 12. Related reading

- Beep ecosystem rules: `Beep.Winform/.cursor/rules/mycontrolsonly.mdc`
- Phase plans + master tracker: `Beep.Winform/.plans/MASTER-TODO-TRACKER.md`
- IDisplayContainer parity: `BeepDocumentManager.DisplayContainer.cs` (drop-in replacement for legacy `BeepDisplayContainer`)
- Setup wizard: `TheTechIdea.Beep.Winform.Controls.Design.Server/Dialogs/DocumentSetupWizardDialog.cs`
- Designer hooks: `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepDocumentManagerDesigner*.cs`
- Tab-selection plumbing: `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepDocumentHostDesigner.TabSelection.cs`

---

*Last updated: 2026-05-17 — corresponds to DocumentHost MDI program through Phase 11 (design-time tab selection).*
