# Tutorial 02 — Migrate from Host-Only to Manager-Paired Form

> **Audience**: Developers who already have a form using `BeepDocumentHost` directly
> and want to gain the full `BeepDocumentManager` feature set without rewriting the form.
> **Effort**: 30–60 minutes per form depending on complexity.
> **Reference form**: `MainFrm_MDI.cs` (host-only baseline).

---

## Why migrate?

| Capability | Host-only | Manager-paired |
|------------|-----------|---------------|
| Window menu | manual | automatic |
| Status bar breadcrumb | manual | automatic |
| Workspace save/switch/delete | manual | `SaveWorkspace` / `SwitchWorkspace` |
| Layout auto-save on close | manual | `AutoSaveLayout = true` |
| Cloud sync | not available | `ConfigureCloudSync(...)` |
| Design-time document seeding | not available | `DesignTimeDocuments` collection |
| Keyboard shortcut customiser | not available | designer verb |
| View-switch safety (replace host) | manual re-wire | automatic |

---

## Before you start — read the existing form

1. Open `MainFrm_MDI.cs` (or your host-only form) in the Designer and in code.
2. Note every place `BeepDocumentHost` is referenced directly:
   - `AddDocument(...)` calls
   - `SaveLayout` / `RestoreLayout` calls
   - `WindowMenu` wiring
   - `StatusStrip` population
   - `DocumentClosing` / `ActiveDocumentChanged` event wiring
3. Note the current `SessionFile` path if any.

---

## Step 1 — Add `BeepDocumentManager` to the form

In Designer, drag a `BeepDocumentManager` from the Toolbox onto the component tray.
Name it `manager` (rename in Properties).

If the form has no designer (code-only), add to `InitializeComponent()` or `Form_Load`:

```csharp
// Add this alongside the existing host variable
manager = new BeepDocumentManager(components);
```

---

## Step 2 — Point the manager at the existing host

```csharp
// Assuming your host is already created as:
//   var host = new BeepDocumentHost { Dock = DockStyle.Fill };
// Or the tabbedView wraps it.
manager.View = beepTabbedView;   // or the IBeepDocumentView wrapper
```

If your form uses `BeepDocumentHost` directly without a `BeepTabbedView` wrapper,
wrap it now:

```csharp
// Wrap the existing host in a view adapter:
var view = new BeepTabbedView();
view.Host = existingHost;          // BeepTabbedView.Host property
Controls.Add(view);
manager.View = view;
```

---

## Step 3 — Remove manual Window-menu wiring

**Before** (host-only):
```csharp
host.WindowMenu = windowToolStripMenuItem;
host.DocumentAdded   += UpdateWindowMenu;
host.DocumentRemoved += UpdateWindowMenu;

private void UpdateWindowMenu(object? sender, EventArgs e)
{
    windowToolStripMenuItem.DropDownItems.Clear();
    foreach (var tab in host.GetAllTabs())
        windowToolStripMenuItem.DropDownItems.Add(tab.Title, null,
            (_, _) => host.SetActiveDocument(tab.Id));
}
```

**After** (manager):
```csharp
manager.WindowMenuOwner = menuStrip1;   // one line; manager maintains it
// Delete UpdateWindowMenu entirely.
```

---

## Step 4 — Remove manual StatusStrip population

**Before** (host-only):
```csharp
host.ActiveDocumentChanged += (_, e) =>
{
    statusLabelTitle.Text    = e.DocumentTitle;
    statusLabelModified.Text = "";
};
host.DocumentModifiedChanged += (_, e) =>
{
    statusLabelModified.Text = e.IsModified ? "●" : "";
};
```

**After** (manager):
```csharp
manager.StatusStripOwner         = statusStrip1;
manager.AutoPopulateStatusStrip  = true;
// Delete the manual handlers above.
```

The manager adds its own `ToolStripStatusLabel` items for title, modified flag,
cursor position (for text-box documents), and a workspace dropdown button.

---

## Step 5 — Replace AddDocument calls

**Before** (host-only):
```csharp
var panel = host.AddDocument("MyDoc", "icons/doc.png");
```

**After** (manager):
```csharp
var panel = manager.AddDocument("MyDoc", iconPath: "icons/doc.png", activate: true);
```

The signatures are identical in intent. `manager.AddDocument` delegates to the active
view, so the code change is mechanical (find-and-replace `host.AddDocument` →
`manager.AddDocument`).

---

## Step 6 — Replace layout persistence calls

**Before** (host-only):
```csharp
private void MainFrm_FormClosing(object? sender, FormClosingEventArgs e)
{
    string json = host.SaveLayout();
    File.WriteAllText(_sessionPath, json);
}

private void MainFrm_Load(object? sender, EventArgs e)
{
    if (File.Exists(_sessionPath))
        host.RestoreLayout(File.ReadAllText(_sessionPath));
}
```

**After** (manager with `AutoSaveLayout`):
```csharp
manager.AutoSaveLayout = true;
manager.SessionFile    = @"%AppData%\MyApp\layout.json";
// Delete the manual FormClosing save and Form_Load restore entirely.
```

Or keep manual control via `manager.SaveLayout()` / `manager.RestoreLayout(json)`.

---

## Step 7 — Move DocumentClosing / ActiveDocumentChanged wiring

**Before** (host-only):
```csharp
host.DocumentClosing       += OnDocumentClosing;
host.ActiveDocumentChanged += OnActiveDocumentChanged;
```

**After** (manager):
```csharp
manager.DocumentClosing       += OnDocumentClosing;
manager.ActiveDocumentChanged += OnActiveDocumentChanged;
// Remove the host subscriptions.
```

The manager re-raises these events from the current view, so they survive view
replacements.

---

## Step 8 — Add workspace support (optional but recommended)

```csharp
// Save current layout as a named workspace
manager.SaveWorkspace("Default", "Default editor layout");

// Wire to a menu item or button to let users switch workspaces
private void workspaceToolStripMenuItem_Click(object sender, EventArgs e)
{
    // Simple example — replace with a proper picker dialog
    manager.SwitchWorkspace("Debug");
}
```

---

## Step 9 — Remove the direct `host` variable (if possible)

Once the manager is the single entry point, remove or demote the direct `host`
reference to a private local used only for operations the manager does not yet expose:

```csharp
// Was: public BeepDocumentHost host = new();
// Now: private BeepDocumentHost host => (manager.View as BeepTabbedView)?.Host!;
// Or:  remove entirely if all calls went through the manager.
```

---

## Step 10 — Test the migrated form

1. Build without errors.
2. Run the form — verify documents open/close/activate correctly.
3. Close the form — verify `layout.json` is created.
4. Reopen — verify tabs are restored.
5. Check the Window menu is populated.
6. Check the status bar shows the active document title and dirty dot.
7. Press `Ctrl+Tab` — verify the MRU quick-switch popup appears.

---

## `MainFrm_MDI` migration checklist

| Item | Done |
|------|------|
| `BeepDocumentManager` added to component tray | ☐ |
| `manager.View` set to existing `BeepTabbedView` | ☐ |
| `WindowMenuOwner` replaces manual menu wiring | ☐ |
| `StatusStripOwner` replaces manual status wiring | ☐ |
| `AddDocument` calls use `manager.AddDocument` | ☐ |
| Layout persistence replaced with `AutoSaveLayout + SessionFile` | ☐ |
| `DocumentClosing` / `ActiveDocumentChanged` re-wired to manager | ☐ |
| Direct `host` references reviewed / removed | ☐ |
| Build passes | ☐ |
| Manual test pass (open/close/restore/MRU) | ☐ |

---

## See Also

- [Tutorial 01 — Build a VS-style IDE shell from scratch](01-IdeShell.md)
- [BeepDocumentManager reference](../BeepDocumentManager.Readme.md)
- [BeepDocumentHost overview](../Readme.md)
