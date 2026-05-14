# Tutorial 01 — Build a VS-style IDE Shell in 10 Minutes

> **Prerequisites**: `BeepDocumentHost` NuGet package installed, WinForms .NET 8 project.
> **Time**: ~10 minutes
> **Sample**: `IdeShellDemoForm.cs` in `TheTechIdea.Beep.Winform.Default.Views`.

---

## What you will build

A desktop IDE shell with:
- A multi-tab code editor area
- File tree on the left (tool panel)
- Output console at the bottom (auto-hide)
- A themed Window menu, status bar, and workspace switcher

---

## Step 1 — Project and form setup

Create a new `.NET 8` WinForms application or open an existing one.
Ensure the project targets `net8.0-windows` and references `TheTechIdea.Beep.Winform.Controls`.

Open `MainForm.cs` in the Designer.

---

## Step 2 — Drop the controls onto the form

From the Toolbox:

1. Drag a **`MenuStrip`** to the top of the form. Name it `menuStrip`.
2. Drag a **`StatusStrip`** to the bottom. Name it `statusStrip`.
3. Drag a **`BeepTabbedView`** to fill the remaining area. Set `Dock = Fill`. Name it `tabbedView`.
4. Drag a **`BeepDocumentManager`** from the component category — it appears in the component tray below the form. Name it `manager`.

---

## Step 3 — Wire the manager in Properties

Select `manager` in the component tray. In the Properties grid:

| Property | Value |
|----------|-------|
| `View` | `tabbedView` |
| `WindowMenuOwner` | `menuStrip` |
| `StatusStripOwner` | `statusStrip` |
| `AutoSaveLayout` | `True` |
| `SessionFile` | `%AppData%\IdeShellDemo\layout.json` |
| `ThemeName` | `Dark` (or any installed Beep theme) |

The manager will add a `Window` item to your menu strip automatically and populate
the status bar with a document-title breadcrumb, dirty indicator, and a workspace
selector button.

---

## Step 4 — Seed the Welcome document

In the manager's Properties grid click the `DesignTimeDocuments` collection editor
(or use the **"Edit Documents…"** smart-tag action on the manager).

Add one entry:

```
Title   : Welcome
TypeKey : welcome
```

Alternatively, set it in code inside `Form_Load`:

```csharp
private void Form_Load(object sender, EventArgs e)
{
    var panel = manager.AddDocument("Welcome", iconPath: null, activate: true);
    if (panel is null) return;
    panel.Controls.Add(new Label
    {
        Text     = "👋  Welcome to IdeShellDemo!\n\nUse File → New to open a document.",
        Font     = new Font("Segoe UI", 14f),
        Dock     = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleCenter
    });
}
```

---

## Step 5 — Handle Document Closing (dirty guard)

Wire the `DocumentClosing` event. In the Designer, double-click `DocumentClosing`
in the Events panel of the manager, or add this in code:

```csharp
manager.DocumentClosing += (_, e) =>
{
    if (e.Panel?.IsModified == true)
    {
        var result = MessageBox.Show(
            $"'{e.DocumentTitle}' has unsaved changes. Close anyway?",
            "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        e.Cancel = result == DialogResult.No;
    }
};
```

---

## Step 6 — Wire File → New

Add a `File` > `New` menu item and handle it:

```csharp
private int _docCounter = 1;

private void newToolStripMenuItem_Click(object sender, EventArgs e)
{
    var name  = $"Untitled-{_docCounter++}";
    var panel = manager.AddDocument(name, activate: true);
    if (panel is null) return;

    var editor = new RichTextBox
    {
        Dock         = DockStyle.Fill,
        Font         = new Font("Cascadia Code", 10f),
        AcceptsTab   = true,
        BackColor    = Color.FromArgb(30, 30, 30),
        ForeColor    = Color.White
    };
    editor.TextChanged += (_, _) => panel.IsModified = true;
    panel.Controls.Add(editor);
}
```

---

## Step 7 — Workspaces

Save and switch named workspace layouts so users can quickly swap between
"Coding", "Review", and "Debug" views:

```csharp
// Save current layout as a workspace
manager.SaveWorkspace("Coding", "Main coding view");

// Switch to a saved workspace
manager.SwitchWorkspace("Debug");

// Enumerate all saved workspaces
foreach (var ws in manager.GetAllWorkspaces())
    Console.WriteLine($"{ws.Name}: {ws.Description}");
```

The status-strip workspace selector button (added automatically by the manager)
lets users switch workspaces at runtime without writing code.

---

## Step 8 — Add a File Tree (tool panel)

To add a left-side file tree, use a `BeepDocumentHost` with auto-hide enabled, or
simply add a standard `Panel` with a `TreeView` docked to the left and let the
manager handle the editor area:

```csharp
// Place a TreeView in a split container to the left of tabbedView
var splitContainer         = new SplitContainer { Dock = DockStyle.Fill };
splitContainer.Panel1MinSize = 150;
Controls.Add(splitContainer);

var fileTree = new TreeView { Dock = DockStyle.Fill };
splitContainer.Panel1.Controls.Add(fileTree);
splitContainer.Panel2.Controls.Add(tabbedView);
```

For a full auto-hide tool-panel implementation see
[`BeepDocumentHost.AutoHide.cs`](../BeepDocumentHost.AutoHide.cs).

---

## Step 9 — Layout persistence across sessions

Because `AutoSaveLayout = true` and `SessionFile` is set, the manager will:

- Automatically **save** the layout JSON when the form closes.
- Automatically **restore** the layout from that file on the next run.

To perform a manual save/restore (e.g. on File → Save Session):

```csharp
// Trigger an explicit save
manager.SaveLayout();

// Read back and restore from a string
string json = File.ReadAllText(expandedPath);
manager.RestoreLayout(json);
```

---

## Step 10 — Keyboard shortcuts

All shortcuts are enabled by default:

| Chord | Effect |
|-------|--------|
| `Ctrl+Tab` | MRU quick-switch popup |
| `Ctrl+Shift+Tab` | Cycle backward through MRU |
| `Ctrl+W` / `Ctrl+F4` | Close active document |
| `Ctrl+Shift+T` | Reopen last closed tab |
| `Ctrl+1–9` | Jump to tab by position |

Disable or remap shortcuts in the **Customize Shortcuts…** designer verb or via:

```csharp
tabbedView.Host.KeyboardShortcutsEnabled = true;
```

---

## Result

At this point you have a fully functional IDE shell:

- Tabbed documents with MRU switching and dirty-guard on close
- Auto-save / restore layout across sessions
- Window menu maintained automatically by the manager
- Status bar showing active document title, modified state, and workspace selector
- Named workspaces your users can create and switch without restarting

For the full `IdeShellDemoForm` source, open:
`TheTechIdea.Beep.Winform.Default.Views/IdeShellDemoForm.cs`

---

## Next steps

- [Tutorial 02 — Migrate from host-only form](02-Migrate-from-host-only.md)
- [BeepDocumentManager reference](../BeepDocumentManager.Readme.md)
- [Cloud sync](../Cloud/Readme.md)
