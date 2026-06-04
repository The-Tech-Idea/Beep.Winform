# WinForms Designer Guidelines for Agents

## Custom Control Designer Pattern for Auto-Creating Child Controls

To make a custom control automatically add another child control and have Visual Studio write that child into the host form's designer.cs file at design time, you **must** use a Custom Control Designer.

If you simply instantiate a sub-control inside your custom control's constructor, Visual Studio will treat it as internal and will **not** serialize it to the designer.cs file of the form. To force serialization, you must use the `IDesignerHost` service to formally create the child component.

## Step-by-Step Implementation

### 1. Add Required References

Your project needs access to design-time libraries:
- **.NET Framework**: Reference `System.Design`
- **.NET Core / .NET 5+**: NuGet package `System.Windows.Forms.Design`

### 2. Create the Custom Designer Class

This designer intercepts the initialization of your custom control at design time, requests the Visual Studio designer host to create the companion control, and links them together.

```csharp
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

public class MyCustomContainerDesigner : ControlDesigner
{
    // Initialize is called immediately when your control is dropped onto a form
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        // Get the designer host service from Visual Studio
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

        if (host != null && component is MyCustomContainer parentControl)
        {
            // Only add the child control if it doesn't exist yet 
            // This prevents duplicating the control every time the designer reloads
            if (parentControl.ChildButton == null)
            {
                // CRITICAL: Let Visual Studio host create the control so it tracks it
                Button newButton = (Button)host.CreateComponent(typeof(Button));

                // Inform the designer system that a property is changing
                PropertyDescriptor property = TypeDescriptor.GetProperties(parentControl)["ChildButton"];
                changeService?.OnComponentChanging(parentControl, property);

                // Configure your child control properties
                newButton.Text = "Auto Generated Button";
                newButton.Location = new System.Drawing.Point(10, 10);
                newButton.Size = new System.Drawing.Size(150, 30);

                // Add the child to the parent control's surface
                parentControl.Controls.Add(newButton);
                parentControl.ChildButton = newButton;

                // Inform the designer system that the change is finished
                changeService?.OnComponentChanged(parentControl, property, null, newButton);
            }
        }
    }
}
```

### 3. Bind the Designer to Your Custom Control

Attach the designer using the `[Designer]` attribute. Apply `[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]` to the child control's property so Visual Studio knows it needs to serialize its code into designer.cs.

```csharp
using System.ComponentModel;
using System.Windows.Forms;

[Designer(typeof(MyCustomContainerDesigner))]
public class MyCustomContainer : UserControl
{
    // Expose the child control via a public property
    // This attribute forces Visual Studio to save this child control's configuration to designer.cs
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Button ChildButton { get; set; }

    public MyCustomContainer()
    {
        // Keep the constructor empty or clear of the child instantiation 
        // because the Designer handles it now.
    }
}
```

### 4. Result in Form1.Designer.cs

Once you rebuild and drag `MyCustomContainer` from the toolbox onto a form, Visual Studio generates:

```csharp
// Inside InitializeComponent():
this.myCustomContainer1 = new MyCustomContainer();
this.button1 = new System.Windows.Forms.Button(); // Automatically tracked!
this.SuspendLayout();

// button1
this.button1.Location = new System.Drawing.Point(10, 10);
this.button1.Name = "button1";
this.button1.Size = new System.Drawing.Size(150, 30);
this.button1.Text = "Auto Generated Button";

// myCustomContainer1
this.myCustomContainer1.ChildButton = this.button1; // Linked together
this.myCustomContainer1.Controls.Add(this.button1);
```

## Troubleshooting

- **Clear/Rebuild**: If you modify designer code, always Rebuild your solution before opening the target Form layout window, otherwise Visual Studio will cache the old designer behavior.
- **Control Deletion**: If you delete `MyCustomContainer` from your form layout, Visual Studio will clean up both the container and the auto-generated button out of your designer.cs file cleanly.

## Architecture Rule: DocumentManager → DocumentHost → Documents

When implementing document management:
1. `DocumentManager` (component) should auto-create `DocumentHost` (view/control) via `IDesignerHost.CreateComponent`
2. `DocumentHost` should auto-create document panels via `IDesignerHost.CreateComponent`
3. All components must serialize to the form's `designer.cs`
4. Do NOT create intermediate wrapper components (e.g., `BeepTabbedView`) — the host IS the view
5. Use `[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]` on collection properties
6. Designer must call `host.CreateComponent(typeof(BeepDocumentHost))` and `host.CreateComponent(typeof(BeepDocumentPanel))`
7. Never instantiate child controls in the control constructor — let the designer create them

## Architecture Rule: Docking — `BeepDockspace` is a Persistent Runtime Control

The control hierarchy is: `Form` → `BeepDockingManager` (component) → `BeepDockspace[]` (Panels) → `DockPanel[]` (children of dockspace).

**Runtime must not reparent anything.** The designer (and `*.Designer.cs`) places `DockPanel`s inside `BeepDockspace` containers. The dockspace IS the view, not a transparent shell. At runtime:
- Each `BeepDockspace` sits directly in `hostForm.Controls` and gets a `DockStyle` (`Top`/`Bottom`/`Left`/`Right`/`Fill`) derived from its `DockPosition` (set by the manager in `SyncDockspaceDockStyles`).
- The WinForms layout engine sizes each dockspace to its assigned edge of the host form.
- Each dockspace arranges its own child `DockPanel`s via its own `LayoutPanels` / `OnLayout` (panels fill the content area below the header).
- The manager's `ApplyLayout` positions dockspaces via `DockStyle` and skips per-panel bounds for panels whose parent is a dockspace.

**Forbidden patterns** (these break the design-time contract):
- `panel.Parent?.Controls.Remove(panel); hostForm.Controls.Add(panel);` — never reparent a `DockPanel` out of its dockspace.
- Removing now-empty `BeepDockspace` controls from the form — they stay.
- `DockingLayoutController` setting `panel.Bounds` for panels in dockspaces — the dockspace owns their layout.

**Allowed patterns**:
- The `AddPanel` runtime API still adds panels directly to `hostForm.Controls` as a legacy path; the layout controller handles their bounds.
- The `AutoHidePanel` flow removes a panel from its dockspace, gives it to the `AutoHideStrip`'s slide panel while peeked, then returns it to the dockspace.

## References
- [Microsoft: Creating a WF Control Design-Time Features](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/creating-a-wf-control-design-time-features)
- [Microsoft: Serializing Collections DesignerSerializationVisibilityAttribute](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/serializing-collections-designerserialization-visibilityattribute)
- [Stack Overflow: Add Child to Custom Control in Designer Mode](https://stackoverflow.com/questions/44144698/add-child-to-custom-control-in-designer-mode)
- [Reza Aghaei: Enable Designer of Child Panel in a UserControl](https://reza-aghaei.com/enable-designer-of-child-panel-in-a-usercontrol/)

## Custom Control Smart-Tag Pattern (DesignerActionList + Verb + Dialog)

The BeepForms / BeepDataConnection / BeepBlock smart-tags follow a single recipe. New verbs should follow this recipe rather than ad-hoc `DesignerVerb` collections.

### Anatomy

1. **Designer** (`Designers/*Designer.cs`) — extends `ComponentDesigner` (for `Component`s) or `BaseBeepParentControlDesigner` (for `Control`s) and exposes a cached `ActionLists` collection.
2. **ActionList** (`Designers/*ActionList.cs`) — a `DesignerActionList` subclass:
   - Wraps the designer.
   - Exposes typed property getters/setters that call `designer.GetProperty<T>()` and `designer.SetProperty()`.
   - Exposes verb methods that open WinForms dialogs and write the result back through `SetProperty()` so `IComponentChangeService` fires and the property grid updates.
   - Implements `GetSortedActionItems()` and groups items with `DesignerActionHeaderItem` / `DesignerActionPropertyItem` / `DesignerActionMethodItem`.
3. **Dialogs** (`Designers/*Form.cs`) — modal WinForms `Form` subclasses. They follow the **DataGridView / TableLayoutPanel** pattern from `BeepBlockFieldEditorForm` and `BeepConnectionEditorForm` rather than designer-generated `.Designer.cs` files.
4. **Reuse `ConflictPolicyDialog`** (verified, `Designers/ConflictPolicyDialog.cs`) for any promote/demote/import/export verb that needs a `ConnectionConflictPolicy` + `ImportWhenEmptyOnly` choice.

### Base-class choice for designer

| Component type | Designer base | Why |
|----------------|---------------|-----|
| `Component` (no UI, e.g. `BeepDataConnection`) | `ComponentDesigner` | No surface to draw; the `IDesignerHost` services are enough for verbs. |
| `Control` / `Panel` (e.g. `BeepForms`) | `BaseBeepParentControlDesigner` (which itself extends `Microsoft.DotNet.DesignTools.Designers.ControlDesigner`) | Inherits the common Beep style smart-tag + change-service plumbing. |
| `Control` with inner panels and child composition (e.g. `BeepBlock`) | `ParentControlDesigner` (the out-of-process equivalent, see `BeepBlockDesigner`) | Lets the designer offer parent/child surface verbs. |

### Setting and reading properties

Always go through `designer.SetProperty(name, value)` / `designer.GetProperty<T>(name)` so the component change service fires:

```csharp
_designer.SetProperty(nameof(BeepForms.FormName), "NewForm");
string current = _designer.GetProperty<string>(nameof(BeepForms.FormName)) ?? string.Empty;
```

Direct property writes (e.g. `forms.Definition = newDefinition`) bypass the change service and the property grid won't refresh.

### ActionLists plumbing

For a `Component` designer, add the field, override `ActionLists`, and return a cached collection. The `ComponentDesigner` base class does **not** add the field for you.

```csharp
public override DesignerActionListCollection ActionLists
    => _actionLists ??= new DesignerActionListCollection
    {
        new BeepDataConnectionActionList(this)
    };
```

For a `Control` designer that extends `BaseBeepParentControlDesigner`, the `ActionLists` collection already contains the common Beep control action list. Override `GetControlSpecificActionLists()` instead of `ActionLists` itself.

### Editor-dialog files

| Control | Smart-tag editor form(s) |
|---------|--------------------------|
| `BeepDataConnection` | `BeepConnectionEditorForm`, `BeepConnectionListEditorForm`, `BeepConnectionTestReportForm` (all in `Designers/`) |
| `BeepForms` | `BeepFormsSetupWizardForm`, `BeepFormPropertiesEditorForm`, `BeepFormsValidationReportForm` |
| `BeepBlock` | `BeepBlockSetupWizardForm` (existing), `BeepBlockFieldEditorForm` (existing), `BeepBlockEntityEditorForm`, `BeepFieldControlTypePolicyEditorForm` (existing) |

When you need a new editor:

1. Add a WinForms `Form` subclass in `Designers/`. **Do not** hand-write a `.Designer.cs` and `.resx`; build the layout in code with `TableLayoutPanel` / `SplitContainer` / `DataGridView`.
2. Add a `DesignerActionMethodItem` in the action list's `GetSortedActionItems()` and a verb method that opens the form and writes the result through `SetProperty()`.
3. Reuse `ConflictPolicyDialog` for any policy-picker verb.
4. Reuse `DefinitionObjectEditorForm<T>` (`Editors/IntegratedFormsDefinitionEditors.cs`, `internal`) for full-object property-grid editing of any `*Definition` model. It's `internal` so the action list must live in the same assembly (`Design.Server`).
5. Add an XUnit test in `Beep.Winform/.../Beep.Winform.Controls.Tests/` that opens a `DesignSurface`, drops the control, and asserts the new verb is present in the smart-tag.

### Test connection at design time

`IDMEEditor` is not available through the designer's `GetService`. Get it from a live `BeepDataConnection.BeepService?.DMEEditor`. The action list helper that handles test should look like this:

```csharp
private static IDMEEditor? ResolveEditor(BeepDataConnection? connection)
    => connection?.BeepService?.DMEEditor;
```

If the connection is null or the editor is null, the smart-tag verb should still work (just show "Editor not available" in the report). Do not throw.

### Out-of-process designer crash recipe: `BackColor = Color.Transparent`

The WinForms out-of-process designer refuses to instantiate any `Control` that sets `BackColor = Color.Transparent` without first enabling `ControlStyles.SupportsTransparentBackColor`. Two safe options:

1. **Derive from `Panel`** instead of `Control`. `Panel` enables `SupportsTransparentBackColor` in its constructor. Use this for any control that needs to be transparent at design time.
2. **Set the style flag in `InitializeComponent`**: `SetStyle(ControlStyles.SupportsTransparentBackColor, true); BackColor = Color.Transparent;`. Use this only when the class must stay a `Control` for some reason.

`BeepBlock.cs` uses option 2. `BeepForms.cs` was failing with the `Control does not support transparent background colors` error and was changed to derive from `Panel` (option 1).

### Designer-time service lease

`BeepDataConnectionDesigner` and `BeepBlockDesigner` both call `DesignTimeBeepServiceManager.Acquire(...)` in their `Initialize` to attach a shared `IBeepService` while the form is open in Visual Studio. The lease is reference-counted and disposed in the designer's `Dispose`. Verb methods that need the live `IDMEEditor` should resolve it through this lease, not through `GetService(typeof(IDMEEditor))`.
