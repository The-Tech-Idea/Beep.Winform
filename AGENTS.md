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

## References
- [Microsoft: Creating a WF Control Design-Time Features](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/creating-a-wf-control-design-time-features)
- [Microsoft: Serializing Collections DesignerSerializationVisibilityAttribute](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/serializing-collections-designerserializationvisibilityattribute)
- [Stack Overflow: Add Child to Custom Control in Designer Mode](https://stackoverflow.com/questions/44144698/add-child-to-custom-control-in-designer-mode)
- [Reza Aghaei: Enable Designer of Child Panel in a UserControl](https://reza-aghaei.com/enable-designer-of-child-panel-in-a-usercontrol/)
