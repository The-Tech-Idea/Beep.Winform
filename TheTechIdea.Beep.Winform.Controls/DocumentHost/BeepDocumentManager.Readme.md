# BeepDocumentManager — Reference

`BeepDocumentManager` is the non-visual orchestrator component for the `BeepDocumentHost`
MDI/docking system. It sits in the form's component tray, bridges the visible
`BeepDocumentHost` view with the form's `MenuStrip`, `StatusStrip`, workspace persistence,
cloud sync, and shortcut customiser.

---

## Quick Comparison: DevExpress vs Beep

| Concept | DevExpress | Beep |
|---------|-----------|------|
| Non-visual orchestrator | `DocumentManager` | `BeepDocumentManager` |
| Visible host | `DocumentGroup` / `TabbedView` | `BeepTabbedView` / `BeepDocumentHost` |
| Document content area | `DocumentPanel` | `BeepDocumentPanel` |
| Named layout sets | `WorkspaceManager` add-in | `WorkspaceManager` (built-in) |
| Auto Window menu | `MainMenu` property on manager | `WindowMenuOwner` property |
| Status-strip integration | manual | `StatusStripOwner` + `AutoPopulateStatusStrip` |
| Cloud sync | external | `BeepCloudSyncManager` extension |

---

## Properties

### View / connection

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `View` | `IBeepDocumentView?` | `null` | The visible host control (set this first). |
| `AutoWireForm` | `bool` | `true` | When `true`, `EndInit()` pushes all settings onto `View` automatically. |

### Appearance

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ThemeName` | `string` | `""` | Beep theme propagated to the view and all panels. |

### Persistence

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AutoSaveLayout` | `bool` | `false` | Save layout on close, restore on next open. |
| `SessionFile` | `string` | `""` | File path for the saved layout JSON. Supports `%AppData%`, `%LocalAppData%`, `%Profile%`. |

### Shell integration

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `WindowMenuOwner` | `MenuStrip?` | `null` | MenuStrip that receives the auto-maintained `Window` sub-menu. Changing the owner detaches the old view's menu wiring before attaching the new one. |
| `WindowMenuText` | `string` | `"&Window"` | Text of the Window menu item. |
| `StatusStripOwner` | `StatusStrip?` | `null` | StatusStrip that receives breadcrumb and dirty-indicator items. |
| `AutoPopulateStatusStrip` | `bool` | `true` | When `true`, populates `StatusStripOwner` with document title, modified flag, cursor position, and workspace selector. |

### Policy

| Property | Type | Description |
|----------|------|-------------|
| `DefaultPolicy` | `BeepDocumentPolicy` | Float / pin / split policy pushed to the view. `AllowFloat`, `AllowPin`, `AllowSplit`, `MaxSplitDepth`. |

### Design-time seeding

| Property | Type | Description |
|----------|------|-------------|
| `DesignTimeDocuments` | `Collection<DocumentDescriptor>` | Documents opened when the view is first attached. Set at design time via the **Edit Documents…** smart-tag action. |

### Cloud sync

| Property | Type | Description |
|----------|------|-------------|
| `CloudSyncSettings` | `BeepCloudSyncSettings?` | Cloud-sync settings applied to the view. Hidden from designer; set at runtime. |

### Extender properties (per-control, via IExtenderProvider)

| Attached property | Description |
|-------------------|-------------|
| `DocumentTitle` | Tab title used when a `Control` is opened as a document. |
| `DocumentIconPath` | Tab icon path used when a `Control` is opened as a document. |

---

## Events

| Event | Args | When |
|-------|------|------|
| `DocumentAdded` | `DocumentAddedEventArgs` | After a document is added to the view. |
| `DocumentRemoved` | `DocumentEventArgs` | After a document is closed. |
| `ActiveDocumentChanged` | `DocumentEventArgs` | The foreground document changes. |
| `DocumentClosing` | `TabClosingEventArgs` | Before close — set `Cancel = true` to abort. |
| `LayoutChanged` | `ManagerLayoutChangedEventArgs` | After `SaveLayout()` completes. |
| `WorkspaceSaved` | `WorkspaceEventArgs` | After the active view saves a workspace. |
| `WorkspaceDeleted` | `WorkspaceEventArgs` | After the active view deletes a workspace. |
| `WorkspaceSwitched` | `WorkspaceEventArgs` | After the active view switches workspace. |

---

## Methods

### Document lifecycle

| Method | Signature | Description |
|--------|-----------|-------------|
| `AddDocument` | `(string title, string? iconPath, bool activate) → BeepDocumentPanel?` | Opens a new document tab. |
| `RemoveDocument` | `(string id, bool force = false) → bool` | Closes the document with the given ID. |
| `ActivateDocument` | `(string id)` | Brings the document to the foreground. |
| `CloseAllDocuments` | `() → bool` | Closes every open document. |
| `BeginBatchAddDocuments` | `()` | Suspends repaints for bulk adds. |
| `EndBatchAddDocuments` | `()` | Flushes after a bulk add. |
| `DocumentCount` | `int` (property) | Number of open documents in the active view (tabbed host or native MDI). |

Runtime documents opened through `AddDocument(...)` are now owned by the manager and replayed when a new view attaches, preserving the same descriptor metadata across tabbed/native-MDI view changes. The manager detaches those same documents from the old view during the transfer, so a swap does not leave duplicate manager-owned documents behind or route the move through normal close semantics.

Existing hosted controls now follow the manager too: extender-backed controls and rehostable `IDisplayContainer` addins are detached from the old view and attached to the new one during a view change, instead of staying parented to stale panels or MDI child hosts.

Those internal transfer removals are not forwarded through the public close/remove event bridge, so a view switch does not look like a user-initiated document close sequence.

Live child controls inside manager-owned documents now move with the document as well. The manager captures those controls from the old tab panel or MDI child before teardown and reattaches them after the destination view recreates the logical document shell.

Form-backed addins now transition between host styles as well. The manager restores `TopLevel` / `FormBorderStyle` for MDI hosting and normalizes those values for embedded panel hosting, so the same addin form can be moved between the two modes.

When `StatusStripOwner` is active, the manager now resolves the current document title from the active view itself, so native MDI updates the status line correctly even though it hosts child `Form` instances instead of `BeepDocumentPanel` controls.

`IDisplayContainer.AddControl(...)` uses `TitleText` as a unique key. Duplicate titles are rejected so an existing tracked addin/document pair cannot be overwritten by a second entry with the same title.

`IDisplayContainer` addins are not part of the manager's runtime replay set. They continue to use their specialized hosting pipeline so a view switch does not blindly duplicate embedded controls or external forms.

When the active view is native MDI, the manager now retargets its internal `DocumentFormCreated` hook as views change, so later `AddControl(...)` calls keep attaching addins to the current MDI view instance.

`BeepNativeMdiView` now also detaches reusable handlers from external forms before a transfer, so a migrated MDI form does not accumulate duplicate close/activate/text-change subscriptions.

### Layout persistence

| Method | Signature | Description |
|--------|-----------|-------------|
| `SaveLayout` | `()` | Saves the current layout to `SessionFile` (if set). |
| `RestoreLayout` | `(string json) → bool` | Restores from a JSON string. Accepts v1, v2, v3 payloads. |

### Workspaces

| Method | Signature | Description |
|--------|-----------|-------------|
| `SaveWorkspace` | `(string name, string description = "") → WorkspaceDefinition?` | Saves the current layout as a named workspace. |
| `SwitchWorkspace` | `(string name) → LayoutRestoreReport?` | Switches to the named workspace. |
| `DeleteWorkspace` | `(string name) → bool` | Deletes the named workspace. |
| `GetAllWorkspaces` | `() → IReadOnlyList<WorkspaceDefinition>` | Returns all saved workspaces. |
| `ActiveWorkspaceName` | `string?` (property) | Name of the currently active workspace. |

### Document templates

| Method | Signature | Description |
|--------|-----------|-------------|
| `RegisterDocumentTemplate` | `(string typeKey, Func<DocumentDescriptor, Control> factory)` | Registers a factory used when restoring documents of a given type. |

### Cloud sync

| Method | Signature | Description |
|--------|-----------|-------------|
| `ConfigureCloudSync` | `(BeepCloudSyncSettings? settings)` | Applies cloud-sync settings to the current view. See [`Cloud/Readme.md`](Cloud/Readme.md). |

---

## Designer Verbs (right-click in Designer)

| Verb | Action |
|------|--------|
| **Edit Documents…** | Opens the `DocumentDescriptor` collection editor to seed design-time documents. |
| **Manage Workspaces…** | Opens the workspace manager dialog to create, rename, and delete named workspaces. |
| **Customize Shortcuts…** | Opens the keyboard shortcut customiser. |
| **Show Help** | Opens this readme in a browser or the VS Help Viewer. |

---

## Typical Form Setup

```csharp
// Designer-generated InitializeComponent equivalent:
manager               = new BeepDocumentManager(components);
tabbedView            = new BeepTabbedView { Dock = DockStyle.Fill };

manager.View                  = tabbedView;
manager.WindowMenuOwner       = menuStrip1;
manager.StatusStripOwner      = statusStrip1;
manager.AutoSaveLayout        = true;
manager.SessionFile           = @"%AppData%\MyApp\layout.json";
manager.ThemeName             = "Dark";
manager.DefaultPolicy.AllowFloat = true;

// Seed tabbed design-time documents as real panels
host = new BeepDocumentHost { Dock = DockStyle.Fill };
tabbedView.Host = host;
Controls.Add(host);

welcomePanel = new BeepDocumentPanel
{
    DocumentId = "welcome",
    DocumentTitle = "Welcome"
};
host.DocumentPanels.Add(welcomePanel);

// Event wiring
manager.DocumentClosing       += OnDocumentClosing;
manager.ActiveDocumentChanged += OnActiveDocumentChanged;
manager.WorkspaceSwitched     += OnWorkspaceSwitched;
```

---

## See Also

- [Readme.md](Readme.md) — `BeepDocumentHost` overview and class reference
- [Tutorials/01-IdeShell.md](Tutorials/01-IdeShell.md) — 10-minute IDE shell tutorial
- [Tutorials/02-Migrate-from-host-only.md](Tutorials/02-Migrate-from-host-only.md) — migrate from host-only forms
- [Cloud/Readme.md](Cloud/Readme.md) — cloud sync extension contract
