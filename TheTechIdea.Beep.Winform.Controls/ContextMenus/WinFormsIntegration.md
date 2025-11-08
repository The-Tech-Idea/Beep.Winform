# BeepContextMenu WinForms Integration Guide

## Overview

BeepContextMenu now implements key patterns from `ToolStripDropDown` and `ContextMenuStrip` to behave like native WinForms context menus.

## Key WinForms Patterns Implemented

### 1. **Hidden Owner Window** (`BeepContextMenu.WinFormsIntegration.cs`)
- **Purpose**: Prevents menu from appearing in Windows taskbar
- **WinForms Pattern**: `ToolStripDropDown.DropDownOwnerWindow`
- **Implementation**: 
  - Static `NativeWindow` created lazily with WS_POPUP style
  - Menu reparented to this window via `SetWindowLong(GWL_HWNDPARENT)`
  - Called in `ReparentToDropDownOwnerWindow()` before showing

```csharp
internal static NativeWindow DropDownOwnerWindow
{
    get
    {
        if (_dropDownOwnerWindow == null || _dropDownOwnerWindow.Handle == IntPtr.Zero)
        {
            // Create hidden popup window
        }
        return _dropDownOwnerWindow;
    }
}
```

### 2. **WM_NCACTIVATE Handling**
- **Purpose**: Keeps owner window's title bar active when menu shows
- **Problem**: Without this, owner window appears inactive, causing desktop flicker
- **WinForms Pattern**: `ToolStripDropDown.WmNCActivate()`
- **Implementation**:
  - Override `WndProc` to intercept `WM_NCACTIVATE`
  - Send `WM_NCACTIVATE` to active window to keep it visually active
  - Redraw owner's frame with `RedrawWindow(RDW_FRAME)`

```csharp
private void WmNCActivate(ref Message m)
{
    if (m.WParam != IntPtr.Zero && !_sendingActivateMessage)
    {
        IntPtr activeWindow = GetActiveWindow();
        SendMessage(activeWindow, WM_NCACTIVATE, (IntPtr)1, (IntPtr)(-1));
        RedrawWindow(activeWindow, ...);
    }
    base.WndProc(ref m);
}
```

### 3. **MA_NOACTIVATE Mouse Activation**
- **Purpose**: Shows menu without stealing focus from owner
- **WinForms Pattern**: `ToolStripDropDown.ShowParams = SW_SHOWNOACTIVATE`
- **Implementation**:
  - Intercept `WM_MOUSEACTIVATE` in `WndProc`
  - Return `MA_NOACTIVATE` (3) to prevent window activation
  - Menu remains visible but doesn't take keyboard focus

```csharp
case WM_MOUSEACTIVATE:
    m.Result = (IntPtr)MA_NOACTIVATE;
    return;
```

### 4. **CloseReason Tracking**
- **Purpose**: Track why menu closed for proper event handling
- **WinForms Pattern**: `ToolStripDropDownCloseReason` enum
- **Implementation**:
  - `BeepContextMenuCloseReason` enum with values:
    - `AppFocusChange` - Window deactivated
    - `AppClicked` - Click outside menu
    - `ItemClicked` - Menu item selected
    - `Keyboard` - ESC/Alt key pressed
    - `CloseCalled` - Programmatic close
  - `SetCloseReason()` / `ResetCloseReason()` methods
  - Passed to `Closing` and `Closed` events

```csharp
internal void SetCloseReason(BeepContextMenuCloseReason reason)
{
    _closeReason = reason;
}

// Usage:
SetCloseReason(BeepContextMenuCloseReason.AppClicked);
Close();
```

### 5. **SetVisibleCore Override**
- **Purpose**: Centralize visibility logic with proper event sequencing
- **WinForms Pattern**: `ToolStripDropDown.SetVisibleCore()`
- **Event Sequence**:
  1. **Opening** → User can cancel (`e.Cancel = true`)
  2. Show window / Reparent to owner
  3. **Opened** → Menu is now visible
  4. (user interaction)
  5. **Closing** → User can cancel
  6. Hide window
  7. **Closed** → Menu is now hidden

```csharp
protected override void SetVisibleCore(bool visible)
{
    if (visible)
    {
        var openingArgs = new FormClosingEventArgs(...);
        OnMenuOpening(openingArgs);
        if (openingArgs.Cancel) return;
        
        ReparentToDropDownOwnerWindow();
        base.SetVisibleCore(true);
        OnMenuOpened(EventArgs.Empty);
    }
    else
    {
        var closingArgs = new BeepContextMenuClosingEventArgs(_closeReason, false);
        OnMenuClosing(closingArgs);
        if (closingArgs.Cancel) return;
        
        base.SetVisibleCore(false);
        OnMenuClosed(new BeepContextMenuClosedEventArgs(_closeReason));
        ResetCloseReason();
    }
}
```

### 6. **Deactivate Handling**
- **Purpose**: Close menu when focus changes (if AutoClose enabled)
- **WinForms Pattern**: `ToolStripDropDown` respects `AutoClose` property
- **Implementation**:
  - Check `_closeOnFocusLost` (mirrors `AutoClose`)
  - Set `CloseReason.AppFocusChange` before closing
  - Allow user to cancel via Closing event

```csharp
private void BeepContextMenu_Deactivate(object sender, EventArgs e)
{
    if (_closeOnFocusLost && Visible)
    {
        SetCloseReason(BeepContextMenuCloseReason.AppFocusChange);
        Close();
    }
}
```

### 7. **Hide-on-Close Behavior**
- **Purpose**: Reuse menu instances instead of disposing
- **WinForms Pattern**: `ToolStripDropDown` hides by default
- **Implementation**:
  - Override `OnFormClosing`
  - Cancel close (`e.Cancel = true`)
  - Call `Hide()` instead
  - Fire `Closed` event before hiding
  - Reset hover state for next show

```csharp
protected override void OnFormClosing(FormClosingEventArgs e)
{
    if (!_destroyOnClose && e.CloseReason != CloseReason.ApplicationExitCall)
    {
        e.Cancel = true;  // Prevent dispose
        
        // Clean state
        _hoveredItem = null;
        _submenuTimer.Stop();
        
        // Fire event
        OnMenuClosed(new BeepContextMenuClosedEventArgs(_closeReason));
        
        Hide();  // Just hide, don't dispose
        return;
    }
    base.OnFormClosing(e);
}
```

## Event Architecture

### New Events (matching WinForms):
- `MenuOpening` - Before menu shows (cancellable)
- `MenuOpened` - After menu shows
- `MenuClosing` - Before menu hides (cancellable, includes CloseReason)
- `MenuClosed` - After menu hides (includes CloseReason)

### Existing Events:
- `ItemClicked` - When menu item clicked
- `ItemHovered` - When mouse hovers over item
- `SubmenuOpening` - Before submenu shows
- `SelectedItemChanged` - When selection changes

## ContextMenuManager Integration

The manager should now:
1. Set `CloseReason` before closing menus
2. Respect `AutoClose` property
3. Handle activation changes properly
4. Coordinate submenu close reasons

```csharp
// Example: Click outside detection
SetCloseReason(BeepContextMenuCloseReason.AppClicked);
menu.Close();

// Example: Item click
SetCloseReason(BeepContextMenuCloseReason.ItemClicked);
menu.Close();
```

## Testing Checklist

- [ ] Menu doesn't appear in taskbar
- [ ] Owner window title bar stays active when menu shows
- [ ] Menu doesn't steal focus from owner
- [ ] ESC key closes menu with Keyboard reason
- [ ] Click outside closes menu with AppClicked reason
- [ ] Item click closes menu with ItemClicked reason  
- [ ] Focus change closes menu with AppFocusChange reason
- [ ] Closing event can cancel close
- [ ] Menu can be shown multiple times without recreating
- [ ] Submenu coordination works correctly

## Differences from WinForms

1. **Styling**: BeepContextMenu uses BeepStyling system, not ToolStripRenderer
2. **Layout**: Custom layout helpers instead of ToolStripLayoutEngine
3. **Items**: Uses SimpleItem instead of ToolStripItem
4. **Manager**: ContextMenuManager provides centralized coordination

## Migration Guide for Existing Code

### Before:
```csharp
var menu = new BeepContextMenu();
menu.Show(location);
// Menu would appear in taskbar, steal focus
```

### After:
```csharp
var menu = new BeepContextMenu();
menu.AutoClose = true;  // Close on focus change
menu.MenuClosing += (s, e) => {
    if (e.MenuCloseReason == BeepContextMenuCloseReason.AppFocusChange)
    {
        // Handle focus change close
    }
};
menu.Show(location);
// Menu now behaves like WinForms: no taskbar, no focus steal
```

## Performance Notes

- Owner window is created once and reused across all menus
- `WM_NCACTIVATE` redraw is minimal - only owner frame
- Hide-on-close eliminates repeated form creation
- Message filter is lightweight (only checks on mouse/keyboard msgs)

## Future Enhancements

1. **ModalMenuFilter Integration**: Full `ToolStripManager.ModalMenuFilter` clone for global menu mode
2. **Keyboard Navigation**: Arrow key navigation with visual feedback
3. **DPI Awareness**: Per-monitor V2 DPI scaling (like `ContextMenuStrip`)
4. **Accessibility**: UIA automation events on Open/Close

## References

- [ToolStripDropDown.cs](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms/src/System/Windows/Forms/Controls/ToolStrips/ToolStripDropDown.cs)
- [ContextMenuStrip.cs](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms/src/System/Windows/Forms/ContextMenuStrip.cs)
- [ToolStripManager.ModalMenuFilter.cs](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms/src/System/Windows/Forms/Controls/ToolStrips/ToolStripManager.ModalMenuFilter.cs)
