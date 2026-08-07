# Phase 2: Fix Popup Positioning

## Problem

The popup often appears in the wrong location or is clipped by screen edges:
- `ComboBoxPopupPlacementHelper.Calculate` computes proper screen-aware placement but the result is **never used**
- `ShowPopup` recalculates position from scratch ignoring screen bounds
- No horizontal alignment (popup should match combobox width)
- No auto-flip when not enough space below

## Root Cause

In `ComboBoxPopupHostForm.cs`:
```csharp
var placement = ComboBoxPopupPlacementHelper.Calculate(owner, desiredWidth, targetHeight, autoFlip);
_form.Size = new Size(desiredWidth, placement.Height);
// placement.Location is IGNORED!

_form.ShowPopup(owner, BeepPopupFormPosition.Bottom, _form.Width, _form.Height);
// ShowPopup recalculates: triggerScreenLocation.Y + triggerSize.Height
```

## Solution

### Step 2.1: Use Placement Helper Location

**File:** `ComboBoxes/Popup/ComboBoxPopupHostForm.cs`

Modify `ShowPopup`:
```csharp
public void ShowPopup(Control owner, ComboBoxPopupModel model, Rectangle triggerBounds)
{
    // ... existing setup code ...

    int targetHeight = CalculatePopupHeight(effectiveModel, _profile);
    int minWidthOverride = 0;
    bool autoFlip = true;
    if (owner is BeepComboBox comboOwner)
    {
        minWidthOverride = Math.Max(0, comboOwner.MinDropdownWidth);
        autoFlip = comboOwner.AutoFlip;
    }

    int desiredWidth = Math.Max(triggerBounds.Width, Math.Max(_profile.MinWidth, minWidthOverride));
    var placement = ComboBoxPopupPlacementHelper.Calculate(owner, desiredWidth, targetHeight, autoFlip);
    
    _form.Size = new Size(desiredWidth, placement.Height);
    _contentPanel.UpdateModel(effectiveModel);

    // Use the calculated location from placement helper!
    _form.Location = placement.Location;
    _form.Show();
    
    // ... rest of setup ...
}
```

### Step 2.2: Add Screen Edge Detection

**File:** `ComboBoxes/Popup/ComboBoxPopupPlacementHelper.cs`

Enhance `Calculate` method:
```csharp
public static (Point Location, int Height) Calculate(
    Control owner,
    int popupWidth,
    int preferredHeight,
    bool autoFlip = true)
{
    Rectangle screenBounds = owner.RectangleToScreen(owner.ClientRectangle);
    Rectangle workingArea = Screen.FromControl(owner).WorkingArea;

    int spaceBelow = workingArea.Bottom - screenBounds.Bottom;
    int spaceAbove = screenBounds.Top - workingArea.Top;

    int actualHeight = preferredHeight;
    Point location;

    bool placeBelow = !autoFlip || spaceBelow >= preferredHeight || spaceBelow >= spaceAbove;
    if (placeBelow)
    {
        actualHeight = Math.Min(preferredHeight, Math.Max(0, spaceBelow - 4));
        location = new Point(screenBounds.Left, screenBounds.Bottom);
    }
    else
    {
        actualHeight = Math.Min(preferredHeight, Math.Max(0, spaceAbove - 4));
        location = new Point(screenBounds.Left, screenBounds.Top - actualHeight);
    }

    // Ensure popup is fully within screen bounds horizontally
    if (location.X + popupWidth > workingArea.Right)
    {
        location.X = workingArea.Right - popupWidth - 4;
    }
    if (location.X < workingArea.Left)
    {
        location.X = workingArea.Left + 4;
    }

    // Ensure popup is fully within screen bounds vertically
    if (location.Y < workingArea.Top)
    {
        location.Y = workingArea.Top + 4;
    }
    if (location.Y + actualHeight > workingArea.Bottom)
    {
        location.Y = workingArea.Bottom - actualHeight - 4;
    }

    return (location, actualHeight);
}
```

## Patterns from Reference Implementations

### CheckBoxComboBox (sgissinger/CheckBoxComboBox)
```csharp
// Real implementation from GitHub - handles auto-flip and screen edges:
public void Show(Control control, Rectangle area)
{
    SetOwnerItem(control);
    resizableTop = resizableRight = false;
    Point location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
    Rectangle screen = Screen.FromControl(control).WorkingArea;
    
    // Auto-flip when not enough space below
    if (location.X + Size.Width > (screen.Left + screen.Width))
    {
        resizableRight = true;
        location.X = (screen.Left + screen.Width) - Size.Width;
    }
    if (location.Y + Size.Height > (screen.Top + screen.Height))
    {
        resizableTop = true;
        location.Y -= Size.Height + area.Height; // Flip to above
    }
    
    location = control.PointToClient(location);
    Show(control, location, ToolStripDropDownDirection.BelowRight);
}
```

### Key Insights from Research
- **CheckBoxComboBox** uses `ToolStripDropDown` which has built-in auto-flip via `ToolStripDropDownDirection`
- **EWSoftware** uses `Screen.FromControl(control).WorkingArea` for bounds
- **CodeProject** uses `ToolStripDropDown` with explicit `BelowRight` direction
- All implementations check `screen.Left + screen.Width` not just `Screen.PrimaryScreen`

### Step 2.3: Horizontal Alignment

**File:** `ComboBoxes/Popup/ComboBoxPopupPlacementHelper.cs`

Add horizontal alignment option:
```csharp
public enum PopupHorizontalAlignment
{
    Left,      // Align popup left with control left
    Right,     // Align popup right with control right
    Center     // Center popup over control
}

public static (Point Location, int Height) Calculate(
    Control owner,
    int popupWidth,
    int preferredHeight,
    bool autoFlip = true,
    PopupHorizontalAlignment hAlign = PopupHorizontalAlignment.Left)
{
    // ... existing code ...
    
    // Adjust horizontal position based on alignment
    switch (hAlign)
    {
        case PopupHorizontalAlignment.Right:
            location.X = screenBounds.Right - popupWidth;
            break;
        case PopupHorizontalAlignment.Center:
            location.X = screenBounds.Left + (screenBounds.Width - popupWidth) / 2;
            break;
        default:
            // Left alignment - already set
            break;
    }
    
    // ... screen edge checks ...
}
```

### Step 2.4: Update BeepPopupForm

**File:** `Forms/BeepPopupForm.cs`

Add overload that accepts location directly:
```csharp
public virtual void ShowPopup(Control triggerControl, Point location, int width, int height)
{
    this.Size = new Size(width, height);
    Location = location;
    
    TriggerControl = triggerControl;
    // ... rest of ShowPopup logic without recalculating location
}
```

## Expected Behavior

- Popup aligns left edge with combobox left edge
- Popup width matches combobox width (or content width, whichever is larger)
- If not enough space below, flips to show above
- Ensures popup is fully visible within screen bounds
- Handles multi-monitor scenarios

## Validation

- [ ] Popup aligns with combobox left edge
- [ ] Popup width matches combobox width
- [ ] When near bottom of screen, popup shows above combobox
- [ ] Popup never extends beyond screen edges
- [ ] Multi-monitor: popup appears on correct monitor
