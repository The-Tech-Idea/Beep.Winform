# DialogManager Implementation Plan

## Overview
This document outlines the implementation plan for a comprehensive `DialogManager` using **Beep.Winform.Controls**. The architecture is centered around the **Helper Pattern** and the versatile **`BeepPopupForm`** to create a maintainable, scalable, and stylistically consistent dialog system.

## Core Architecture Strategy

### 1. The Helper Pattern
The `DialogManager` will act as a facade, delegating calls to specialized helper classes. This separates concerns, making the system easier to test, maintain, and extend.

**Core Helper Structure:**
```
DialogsManagers/
├── DialogManager.cs        // Main facade implementing IDialogManager
└── Helpers/
    ├── DialogHelperBase.cs     // Base class with common logic (theming, positioning)
    ├── InputDialogHelper.cs    // Handles all text, numeric, and date/time inputs
    ├── ListDialogHelper.cs     // Manages selections (ComboBox, ListBox, Radio, CheckList)
    ├── FileDialogHelper.cs     // Wraps system file/folder dialogs and custom file lists
    ├── NotifyDialogHelper.cs   // Manages confirmations, alerts, messages, and exceptions
    └── ProgressDialogHelper.cs // Handles progress indicators and toast notifications
```

### 2. The Dynamic Dialog Form Strategy
Instead of creating numerous specific form files (e.g., `BeepInputDialog`, `BeepConfirmDialog`), we will use a single, powerful `BeepDialogForm`. This form will be dynamically configured at runtime by the helpers.

**Key Benefits:**
- **Reduced Boilerplate**: Eliminates the need for dozens of nearly identical form classes.
- **Maximum Flexibility**: New dialog variations can be created without new UI files.
- **Consistency**: All dialogs will share the same base behavior and styling from `BeepDialogForm`.

**`BeepDialogForm.cs` (The Generic Dialog Host):**
- Inherits from `BeepPopupForm` to leverage its features (theming, positioning, cascading).
- Contains a `Panel` or `FlowLayoutPanel` to host dynamically added controls.
- Exposes methods to configure its title, icon, content controls, and action buttons.

### 3. Component and Dialog Strategy

| Dialog Type | Implementation Strategy | Key Beep Components |
|---|---|---|
| **Input/Numeric/Date** | Custom `BeepDialogForm` | `BeepTextField`, `BeepCalendar`, `BeepButton` |
| **List/Combo/Radio/Check** | Custom `BeepDialogForm` | `BeepComboBox`, `BeepListBox`, `BeepRadioButton`, `BeepCheckBox` |
| **Confirm/Alert/Message** | Custom `BeepDialogForm` | `BeepLabel`, `BeepImage`, `BeepButton` |
| **File/Folder Selection** | **Wrap System Dialogs** | `OpenFileDialog`, `SaveFileDialog`, `FolderBrowserDialog` |
| **Custom File List** | Custom `BeepDialogForm` | `BeepListBox` with filtering |
| **Progress/Toast** | Custom `BeepPopupForm` (non-modal) | `BeepProgressBar`, `BeepLabel` |
| **Color/Font Picker** | Wrap System Dialogs or Custom `BeepDialogForm` | `ColorDialog`, `FontDialog` or custom controls |

---

## Implementation Phases

### Phase 1: Foundation
1.  **`DialogHelperBase.cs`**:
    -   Create the base class for all helpers.
    -   Implement shared logic for creating and positioning a `BeepDialogForm`.
    -   Integrate theme handling from the `Beep` framework.
2.  **`BeepDialogForm.cs`**:
    -   Create the generic dialog form inheriting from `BeepPopupForm`.
    -   Add a title bar area (using `BeepLabel` and `BeepImage`) and a content `Panel`.
    -   Add a bottom panel for action buttons (`BeepButton`).
    -   Implement public methods like `SetTitle(string, Image)`, `AddControl(Control)`, and `AddButton(BeepDialogButtons)`.

### Phase 2: Notification and Confirmation Dialogs
1.  **`NotifyDialogHelper.cs`**:
    -   Implement `Confirm`, `MsgBox`, and `ShowAlert`.
    -   These methods will instantiate `BeepDialogForm`, add a `BeepLabel` for the message, a `BeepImage` for the icon, and dynamically generate the required `BeepButton`s (`OkCancel`, `YesNo`, etc.).
    -   The helper will show the form modally (`ShowDialog()`) and translate the `DialogResult` into a `DialogReturn`.

### Phase 3: Core Input Dialogs
1.  **`InputDialogHelper.cs`**:
    -   Implement `InputBox`, `InputPassword`, `InputInt`, `InputDouble`, `InputDateTime`.
    -   Each method will configure a `BeepDialogForm` with a `BeepLabel` (prompt) and the appropriate input control (`BeepTextField`, `BeepCalendar`).
    -   Implement basic validation within the helper before closing the dialog.

### Phase 4: List Selection Dialogs
1.  **`ListDialogHelper.cs`**:
    -   Implement `InputComboBox`, `InputListBox`, `InputRadioGroupBox`, `MultiSelect`.
    -   The helper will add the corresponding `Beep` selection control to a `BeepDialogForm`.
    -   It will be responsible for populating the control with `SimpleItem` data.

### Phase 5: System Dialog Wrappers
1.  **`FileDialogHelper.cs`**:
    -   Implement `LoadFileDialog`, `SaveFileDialog`, and `SelectFolderDialog`.
    -   These methods will instantiate and configure the standard .NET `OpenFileDialog`, `SaveFileDialog`, etc.
    -   The helper's role is to abstract these system dialogs and return a consistent `DialogReturn` object.

### Phase 6: Asynchronous and Non-Modal UI
1.  **`ProgressDialogHelper.cs`**:
    -   Implement `ShowProgress`, `UpdateProgress`, and `CloseProgress`. This will use a non-modal `BeepPopupForm` instance managed via a token.
    -   Implement `ShowToast`, which will show a short-lived, non-modal `BeepPopupForm`.

---

## Technical Implementation Example

```csharp
// In NotifyDialogHelper.cs
public DialogReturn ShowConfirm(string title, string message, BeepDialogButtonSchema schema, BeepDialogIcon icon)
{
    // 1. Create the dynamic dialog form
    using var dialog = new BeepDialogForm();

    // 2. Configure it
    dialog.SetTitle(title, GetIconFor(icon));
    dialog.AddControl(new BeepLabel { Text = message, Dock = DockStyle.Fill });
    dialog.AddButtons(schema); // This method adds the correct BeepButtons

    // 3. Show modally and get result
    var result = dialog.ShowDialog();

    // 4. Translate to a standard DialogReturn object
    return new DialogReturn
    {
        Result = ToBeepDialogResult(result),
        // ... other properties
    };
}

// In DialogManager.cs
public DialogReturn Confirm(string title, string message, BeepDialogButtonSchema schema, BeepDialogIcon icon)
{
    // Delegate the call to the appropriate helper
    return _notifyHelper.ShowConfirm(title, message, schema, icon);
}
```

This revised plan provides a clear, modern, and highly reusable architecture for the `DialogManager`, placing `BeepPopupForm` at the center of the custom dialog experience while pragmatically using system dialogs where appropriate.