---
name: winforms-control-designer-child-interaction
description: >
  Design or refactor Microsoft WinForms custom-control design-time interaction using
  ControlDesigner.GetHitTest, EnableDesignMode, designer-host transactions, and
  Designer.cs serialization. Use when implementing clickable child UI at design time,
  tab/header interaction, designable child surfaces, page/document creation, or removing
  message-filter and duplicated design-time mouse-hook approaches.
---

# WinForms ControlDesigner Child Interaction and Designer.cs Persistence

## Purpose

Use this skill when building or repairing custom WinForms controls whose child controls must respond to mouse input in the Visual Studio designer and whose design-authored children, pages, tabs, or layout values must persist into the owning `Form.Designer.cs` or `UserControl.Designer.cs`.

The required architecture is the Microsoft design-time model:

- Route design-time mouse interaction through the relevant `ControlDesigner.GetHitTest(Point)` override.
- Make true child design surfaces editable through `EnableDesignMode(child, name)`.
- Persist child contents only through a parent-exposed design model or designable content surface.
- Create design-authored components through `IDesignerHost.CreateComponent(...)`, within a `DesignerTransaction`.
- Let the WinForms serializer generate `.Designer.cs`; never manually patch generated designer files as the implementation strategy.

## Project conventions for this codebase

Apply these defaults unless source inspection proves otherwise:

- Runtime controls and public APIs belong in `TheTechIdea.Beep.Winform.Controls`.
- Designer-only infrastructure belongs in `TheTechIdea.Beep.Winform.Controls.Design.Server`.
- Do not put designer-service dependencies in the runtime control assembly.
- Do not introduce a new MDI-only theme interface. Visible runtime controls that require theming expose the project’s required `Theme` property and `ApplyTheme()` method directly or follow its existing minimal theming contract.
- Design-time authored layout and placeholder state must be serialized into the owning form/user control `.Designer.cs`. Runtime user/session layout changes must be persisted separately and must not alter `.Designer.cs`.
- For an MDI replacement, design toward a polished DevExpress-style user experience: tabbed/native documents, tool windows, commands, layout persistence, accessible interaction, and designer usability.

## Target-framework rule: check before coding

Before writing designer code, inspect the target framework and existing design-server references.

| Target | Designer API choice |
|---|---|
| Modern .NET WinForms (`net6.0-windows` or later) | Use the design server project and the `Microsoft.WinForms.Designer.SDK` API. Designer types such as `ControlDesigner` are in `Microsoft.DotNet.DesignTools.Designers`. |
| .NET Framework WinForms | Use `System.Windows.Forms.Design.ControlDesigner` / `ParentControlDesigner` and `System.Design`. |

Do not blindly copy .NET Framework designer namespaces into a modern .NET design-server project. When the solution already has a designer-server pattern, match its references and metadata-registration approach.

## Non-negotiable design rules

### 1. `GetHitTest` is the design-time mouse input gate

Use a designer attached to the smallest interactive child control, for example a tab strip or header bar. Override `GetHitTest(Point)` and return `true` only where design-time interaction is intentionally allowed.

Requirements:

- The input point is in screen coordinates; convert with `Control.PointToClient(point)`.
- Keep the method fast; it may be invoked as the cursor moves.
- Limit pass-through to tabs, close buttons, add buttons, dropdown buttons, resize grips, or other explicitly supported design-time hit regions.
- Return `base.GetHitTest(point)` outside supported regions.
- Do not install a global `IMessageFilter` to catch designer clicks.
- Do not duplicate the same click interception in both the host designer and the child/tab-strip designer.

Canonical designer:

```csharp
// Design.Server project.
// For modern .NET, use Microsoft.DotNet.DesignTools.Designers.
// For .NET Framework, use System.Windows.Forms.Design.
using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using Microsoft.DotNet.DesignTools.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server;

public sealed class BeepMdiTabStripDesigner : ControlDesigner
{
    private BeepMdiTabStrip TabStrip => (BeepMdiTabStrip)Control;

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);
        TabStrip.HeaderActionRequested += OnHeaderActionRequested;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && Control is BeepMdiTabStrip strip)
        {
            strip.HeaderActionRequested -= OnHeaderActionRequested;
        }

        base.Dispose(disposing);
    }

    protected override bool GetHitTest(Point point)
    {
        Point clientPoint = Control.PointToClient(point);

        // Hit-test visual UI only. Do not change model state here.
        return TabStrip.IsDesignInteractiveHit(clientPoint)
            || base.GetHitTest(point);
    }

    private void OnHeaderActionRequested(object? sender, BeepMdiHeaderActionEventArgs e)
    {
        // Execute model changes through designer services and transactions.
        // Do not let the runtime visual child write serialized design state directly.
        e.Handled = TryHandleDesignAction(e);
    }

    private bool TryHandleDesignAction(BeepMdiHeaderActionEventArgs e)
    {
        // Delegate to a transaction-aware design command/controller.
        // See the component creation pattern below.
        return false;
    }
}
```

The runtime child should expose hit-testing and an intent event without referencing the designer assembly:

```csharp
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls;

public sealed class BeepMdiTabStrip : Control
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler<BeepMdiHeaderActionEventArgs>? HeaderActionRequested;

    public bool IsDesignInteractiveHit(Point clientPoint)
    {
        // Return true only for real interactive header geometry.
        return TryHitTab(clientPoint, out _)
            || IsAddButtonHit(clientPoint)
            || IsDropDownButtonHit(clientPoint);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        var action = ResolveHeaderAction(e.Location);
        if (action is not null)
        {
            HeaderActionRequested?.Invoke(this, action);
            if (action.Handled)
            {
                return;
            }
        }

        // Runtime-only/default behavior remains inside the runtime control.
        base.OnMouseDown(e);
    }

    private bool TryHitTab(Point point, out int index)
    {
        index = -1;
        return false; // Replace with actual geometry calculation.
    }

    private bool IsAddButtonHit(Point point) => false;
    private bool IsDropDownButtonHit(Point point) => false;
    private BeepMdiHeaderActionEventArgs? ResolveHeaderAction(Point point) => null;
}

public sealed class BeepMdiHeaderActionEventArgs : EventArgs
{
    public BeepMdiHeaderActionEventArgs(BeepMdiHeaderAction action, int index = -1)
    {
        Action = action;
        Index = index;
    }

    public BeepMdiHeaderAction Action { get; }
    public int Index { get; }
    public bool Handled { get; set; }
}

public enum BeepMdiHeaderAction
{
    SelectDocument,
    AddDocument,
    CloseDocument,
    ShowDocumentMenu
}
```

Do not mutate persistent design state inside `GetHitTest`. Its job is only to declare whether the child receives mouse interaction.

### 2. Use `EnableDesignMode` for editable child content surfaces

Use `EnableDesignMode` when a child surface must behave as a nested design host, for example a content panel into which developers can drop controls.

A designer for a composite runtime control should expose and enable the content child:

```csharp
using System.ComponentModel;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server;

public sealed class BeepDocumentSurfaceDesigner : ParentControlDesigner
{
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        var surface = (BeepDocumentSurface)component;

        // Enables Visual Studio designer interaction inside the nested surface.
        EnableDesignMode(surface.ContentPanel, nameof(surface.ContentPanel));
    }
}
```

The runtime parent must expose any child surface whose design-authored contents need to persist:

```csharp
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls;

public sealed class BeepDocumentSurface : ContainerControl
{
    private readonly Panel _contentPanel = new() { Dock = DockStyle.Fill };

    public BeepDocumentSurface()
    {
        Controls.Add(_contentPanel);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Panel ContentPanel => _contentPanel;
}
```

Treat `EnableDesignMode` and persistence as separate responsibilities:

- `EnableDesignMode` makes the nested child interactive/designable.
- Exposing the child or the owning content model gives the serializer a path to persist design-authored state.
- A private or hidden internal child with no exposed serializable route is not a reliable persistence model for child contents.

### 3. Never write design-authored controls directly into `.Designer.cs`

When an action inside your designer creates a control, page, or placeholder intended to persist with the form, obtain the designer host and create the component through it. The serializer will generate `.Designer.cs` when the model is correctly connected to the design surface.

Do **not**:

```csharp
// Wrong: generated code must not be constructed or edited manually.
File.AppendAllText("Form1.Designer.cs", "this.button1 = new Button();");
```

Do:

```csharp
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

internal static class DesignedControlFactory
{
    public static T AddChildControl<T>(
        IServiceProvider services,
        Control designSurface,
        string transactionDescription,
        Action<T>? configure = null)
        where T : Control
    {
        var host = (IDesignerHost?)services.GetService(typeof(IDesignerHost))
            ?? throw new InvalidOperationException("IDesignerHost is unavailable.");

        var changes = (IComponentChangeService?)services.GetService(typeof(IComponentChangeService));

        using DesignerTransaction transaction = host.CreateTransaction(transactionDescription);

        var child = (T)host.CreateComponent(typeof(T));

        changes?.OnComponentChanging(designSurface, null);
        designSurface.Controls.Add(child);
        configure?.Invoke(child);
        changes?.OnComponentChanged(designSurface, null, null, null);

        transaction.Commit();
        return child;
    }

    public static void SetProperty<TValue>(object component, string propertyName, TValue value)
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(component)[propertyName]
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");

        property.SetValue(component, value);
    }
}
```

Use this from a designer command, verb, action list, or transaction-aware handler:

```csharp
private void AddLabelToDocumentSurface(BeepDocumentSurface surface)
{
    DesignedControlFactory.AddChildControl<BeepLabel>(
        this,
        surface.ContentPanel,
        "Add Beep label",
        label =>
        {
            DesignedControlFactory.SetProperty(label, nameof(label.Name), "beepLabel1");
            DesignedControlFactory.SetProperty(label, nameof(label.Text), "New label");
            DesignedControlFactory.SetProperty(label, nameof(label.Location), new Point(24, 24));
        });
}
```

Expected generated form code is produced by Visual Studio, not handwritten by the designer:

```csharp
private TheTechIdea.Beep.Winform.Controls.BeepLabel beepLabel1;

private void InitializeComponent()
{
    this.beepLabel1 = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
    // ...
    this.beepLabel1.Location = new System.Drawing.Point(24, 24);
    this.beepLabel1.Name = "beepLabel1";
    this.beepLabel1.Text = "New label";
    this.beepDocumentSurface1.ContentPanel.Controls.Add(this.beepLabel1);
}
```

Exact generated shape may vary with the serializer and exposed content surface. Validate by saving, reopening the designer, and inspecting generated code.

### 4. Persist custom pages/documents through a public serializable model

If the MDI control owns non-visual document metadata or page placeholder items rather than ordinary child controls, expose a collection property that the designer serializer can traverse.

Runtime example:

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls;

public sealed class BeepMdiManager : Component
{
    private readonly Collection<BeepMdiDocument> _documents = [];

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Collection<BeepMdiDocument> Documents => _documents;
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class BeepMdiDocument : Component
{
    [DefaultValue("")]
    public string Title { get; set; } = string.Empty;

    [DefaultValue(false)]
    public bool IsPinned { get; set; }

    public override string ToString() =>
        string.IsNullOrWhiteSpace(Title) ? "(Document)" : Title;
}
```

Designer-side document creation pattern:

```csharp
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server;

internal sealed class BeepMdiDocumentDesignCommands
{
    private readonly IServiceProvider _services;
    private readonly BeepMdiManager _manager;

    public BeepMdiDocumentDesignCommands(IServiceProvider services, BeepMdiManager manager)
    {
        _services = services;
        _manager = manager;
    }

    public BeepMdiDocument AddDocument()
    {
        var host = (IDesignerHost?)_services.GetService(typeof(IDesignerHost))
            ?? throw new InvalidOperationException("IDesignerHost is unavailable.");
        var changes = (IComponentChangeService?)_services.GetService(typeof(IComponentChangeService));

        PropertyDescriptor documentsProperty =
            TypeDescriptor.GetProperties(_manager)[nameof(BeepMdiManager.Documents)]
            ?? throw new InvalidOperationException("Documents property is unavailable.");

        using DesignerTransaction transaction = host.CreateTransaction("Add MDI document");

        var document = (BeepMdiDocument)host.CreateComponent(typeof(BeepMdiDocument));

        changes?.OnComponentChanging(_manager, documentsProperty);
        document.Title = $"Document {_manager.Documents.Count + 1}";
        _manager.Documents.Add(document);
        changes?.OnComponentChanged(_manager, documentsProperty, null, null);

        transaction.Commit();
        return document;
    }
}
```

The desired serializer output for the owning form should be conceptually similar to:

```csharp
private TheTechIdea.Beep.Winform.Controls.BeepMdiManager beepMdiManager1;
private TheTechIdea.Beep.Winform.Controls.BeepMdiDocument beepMdiDocument1;

private void InitializeComponent()
{
    this.beepMdiManager1 = new TheTechIdea.Beep.Winform.Controls.BeepMdiManager();
    this.beepMdiDocument1 = new TheTechIdea.Beep.Winform.Controls.BeepMdiDocument();
    this.beepMdiDocument1.Title = "Document 1";
    this.beepMdiManager1.Documents.Add(this.beepMdiDocument1);
}
```

If Visual Studio does not serialize the collection as expected, stop and fix the design model/serializer metadata. Do not compensate by editing `.Designer.cs`.

## Choosing the correct designer arrangement

| Need | Correct design-time implementation |
|---|---|
| Click an MDI tab/header button at design time | Dedicated tab-strip/header `ControlDesigner` with `GetHitTest`. |
| Drop controls into a document surface | Parent/container designer calls `EnableDesignMode(ContentPanel, ...)`; parent publicly exposes the content child. |
| Add a persistent visual child from a smart tag or verb | `IDesignerHost.CreateComponent`, designer transaction, attach to exposed designable container. |
| Add a persistent non-visual document/page item | `IDesignerHost.CreateComponent`, transaction, public collection with `DesignerSerializationVisibility.Content`. |
| Simple configuration value | Public property with matching default value or paired `Reset<Property>` / `ShouldSerialize<Property>`. |
| Runtime user layout | Separate runtime persistence service; never update the designer-authored collection or `.Designer.cs` during normal app use. |

## Anti-patterns to remove or reject

Reject patches that introduce any of the following unless a proven framework limitation is documented:

- Global message filters to capture clicks on a designer surface.
- Host designer and child/tab-strip designer both subscribing to the same mouse workflow.
- Mouse coordinate assumptions that treat `GetHitTest` input as client coordinates.
- State mutation inside `GetHitTest`.
- Creation of persistent components with `new` only, bypassing `IDesignerHost.CreateComponent`.
- File/string manipulation of `.Designer.cs`.
- Child surfaces enabled for design interaction but not exposed through a public serializable parent model when their contents must persist.
- Runtime session layout being written back as design-time state.
- Designer-specific assembly references added to the runtime control library.

## Required implementation workflow

Follow this sequence for every design-time child interaction task.

### Step 1: Inspect before editing

Determine:

1. Runtime target framework and designer target framework.
2. Existing designer-server NuGet/package/reference pattern.
3. Runtime control namespace and design-server namespace.
4. Whether the child is merely clickable, is a container for dropped controls, or represents a persistent page/document model.
5. Current serialization contract: public content panel, collection, component fields, or none.
6. Any current global filters, duplicate event hooks, or hand-edited designer-code strategy to remove.

### Step 2: Define ownership

Write down one owner for each responsibility:

- Runtime control: rendering, hit geometry, normal runtime behavior, intent events.
- Dedicated child designer: `GetHitTest` and design-time command routing for its own visible UI.
- Parent/container designer: `EnableDesignMode` for designable child surfaces.
- Design command/factory: transactions, `IDesignerHost.CreateComponent`, change notifications, selection updates.
- Runtime parent model: exposed serializable child or collection.
- Visual Studio serializer: output into `.Designer.cs`.

Do not allow two designers to own the same click behavior.

### Step 3: Implement design-time interaction

For header/tab mouse behavior:

1. Attach a designer to the child tab strip/header.
2. Override `GetHitTest` and convert screen to client coordinates.
3. Return `true` only for supported interactive regions.
4. Raise intent from the runtime visual child.
5. Let the designer execute any persistent action using design services.

### Step 4: Implement persistence

For child-control hosting:

1. Expose a read-only content panel or equivalent on the runtime parent.
2. Mark it with appropriate designer serialization visibility.
3. Call `EnableDesignMode` for it from its parent designer.
4. Use the host to create any child created by a verb or header command.

For document/page collections:

1. Expose the collection publicly from the owner.
2. Mark it `DesignerSerializationVisibility.Content`.
3. Use components or serializable page items suited to the intended generated code.
4. Create host-managed items and add them under a transaction/change notification.

### Step 5: Check undo, serialization, and reopen behavior

At minimum, test:

- Design-time tab selection or supported header command works only in intended regions.
- Non-interactive regions still select/move the owning control normally.
- Adding a page/control can be undone and redone in Visual Studio.
- Saving creates the expected owner/item/child statements in `.Designer.cs`.
- Closing and reopening the designer reconstructs the same visual/model state.
- Reset/default property behavior removes unneeded assignments from generated code.
- Runtime layout changes do not modify design-time generated state.

## Serialization property guidance

For simple public properties:

```csharp
[DefaultValue(TabStripPlacement.Top)]
public TabStripPlacement TabStripPlacement { get; set; } = TabStripPlacement.Top;
```

For non-trivial defaults, use both methods and no `DefaultValue` attribute:

```csharp
public Padding DocumentPadding { get; set; } = new Padding(8);

private void ResetDocumentPadding() => DocumentPadding = new Padding(8);

private bool ShouldSerializeDocumentPadding() => DocumentPadding != new Padding(8);
```

For public design content:

```csharp
[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
public Collection<BeepMdiDocument> Documents => _documents;
```

For runtime-only/transient state:

```csharp
[Browsable(false)]
[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
public int HotTabIndex { get; private set; } = -1;
```

## Response requirements when applying this skill

When asked to review or implement design-time behavior, produce:

1. A brief diagnosis identifying whether the existing code follows or violates the model.
2. A file/class responsibility map separating runtime from designer-server work.
3. Concrete code changes or full class implementations.
4. The expected `.Designer.cs` serialization shape for design-authored state.
5. A Visual Studio validation checklist covering interaction, persistence, reopen, undo/redo, and runtime separation.

If a proposed implementation cannot guarantee serializer output without testing in Visual Studio, say so and specify the expected generated code to verify.

## Official basis

The rules in this skill are based on Microsoft documentation:

- `ControlDesigner.GetHitTest(Point)`: permits the control to receive selected design-time mouse clicks; its point is in screen coordinates and should be converted with `PointToClient` where needed.
  - https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.design.controldesigner.gethittest
- `ControlDesigner.EnableDesignMode(Control, String)`: enables design-time functionality for a child control; Microsoft states that child contents persist only when the primary control exposes the relevant child through its public model.
  - https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.design.controldesigner.enabledesignmode
- `IDesignerHost`: creates components in the current design document and supports transactions used for designer operations.
  - https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.design.idesignerhost
- Design-time properties for custom controls: explains designer serialization, defaults, `Reset<Property>` / `ShouldSerialize<Property>`, and hidden transient state.
  - https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls-design/designer-properties-overview
- Serialize collections with `DesignerSerializationVisibilityAttribute`: establishes `DesignerSerializationVisibility.Content` for collection content persistence, with the warning to account for newer .NET designer changes.
  - https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/serializing-collections-designerserializationvisibilityattribute
- Designer differences from .NET Framework: explains the modern .NET out-of-process designer and the `Microsoft.WinForms.Designer.SDK` / `Microsoft.DotNet.DesignTools.Designers` namespace migration.
  - https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls-design/designer-differences-framework
