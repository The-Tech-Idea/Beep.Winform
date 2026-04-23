# Phase 16: Accessibility

**Priority:** P3 | **Track:** Polish & Quality | **Status:** Pending

## Objective

Add UI Automation support and advanced keyboard navigation for WCAG compliance.

## Implementation Steps

### Step 1: Create UIA Provider

```csharp
// Accessibility/GridAccessibilityProvider.cs
public class GridAccessibilityProvider : AutomationProvider
{
    // Implement IRawElementProviderSimple
    // Expose grid as a DataGrid control type
    // Expose cells as DataItem elements
    // Expose headers as Header elements
    // Expose navigator as ToolBar element
    // Expose filter panel as Group element
}
```

### Step 2: Implement Cell UIA

Each visible cell should expose:
- `Name`: cell content as string
- `ControlType`: DataItem
- `Row` and `Column` properties
- `Value` pattern for editable cells
- `SelectionItem` pattern for selected cells

### Step 3: Create Keyboard Navigator

```csharp
// Accessibility/GridKeyboardNavigator.cs
public class GridKeyboardNavigator
{
    // Tab: move focus between grid regions (headers → cells → navigator → filter)
    // Arrow keys: navigate within current region
    // Home/End: first/last in current row/column
    // Page Up/Down: scroll one page
    // Ctrl+Home/End: first/last cell in grid
    // Space: toggle checkbox
    // Enter: begin edit / activate
    // Escape: cancel edit / close popup
    // F2: begin edit
    // Delete: delete row
    // Ctrl+C/V/X: clipboard
}
```

### Step 4: Create Focus Manager

```csharp
// Accessibility/GridFocusManager.cs
public class GridFocusManager
{
    // Track focus region (header, cell, navigator, filter)
    // Manage focus rectangle rendering
    // Coordinate with selection strategy
    // Announce focus changes to UIA
}
```

### Step 5: Wire to BeepGridPro

```csharp
// In BeepGridPro constructor or initialization:
AccessibilityProvider = new GridAccessibilityProvider(this);
KeyboardNavigator = new GridKeyboardNavigator(this);
FocusManager = new GridFocusManager(this);
```

## Acceptance Criteria

- [ ] Screen reader announces grid content
- [ ] Screen reader announces cell position (row X, column Y)
- [ ] Screen reader announces selection changes
- [ ] Full keyboard navigation without mouse
- [ ] Focus indicator visible in all grid regions
- [ ] All interactive elements are keyboard accessible
- [ ] Keyboard shortcuts documented

## Files to Create

- `Accessibility/GridAccessibilityProvider.cs`
- `Accessibility/GridKeyboardNavigator.cs`
- `Accessibility/GridFocusManager.cs`

## Files to Modify

- `BeepGridPro.cs` (wire accessibility components)
- `Helpers/GridInputHelper.cs` (coordinate with keyboard navigator)
