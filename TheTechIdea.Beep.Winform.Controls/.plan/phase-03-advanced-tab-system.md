# Phase 3: Advanced Tab System & Workspace Model

> **Sprint:** 23-24 · **Priority:** High · **Complexity:** Medium
> **Dependency:** Phase 2 complete · **Estimated Effort:** 3-4 weeks

---

## Objective

Implement VS Code-style tab rows, pinned tabs, tab groups, multi-row support, and a workspace model that allows saving and switching between different document arrangements. This phase transforms the tab system from a single strip into a flexible, multi-row, workspace-aware system.

---

## Current Limitations

| Limitation | Current State | Target State |
|------------|--------------|--------------|
| Tab rows | Single strip per group | Multiple rows per group |
| Pinned tabs | Basic icon-only | Persistent left section with dedicated area |
| Tab overflow | Dropdown popup | Multi-row + overflow + search |
| Tab grouping | By category only | Visual groups with separators |
| Tab tear-out | Float window only | Create new row or group |
| Tab density | 3 modes | 3 modes + responsive breakpoints |
| Tab styles | 8 styles | 12 styles (4 new) |
| Workspace model | None | Save/switch named workspaces |

---

## Tasks

### Task 3.1: Implement Tab Row System

**Files to Create:**

```
Tabs/
├── BeepTabRow.cs                    ← Single row of tabs
├── BeepTabRowContainer.cs           ← Container for multiple rows
└── BeepTabRowLayout.cs              ← Row layout calculations
```

**`BeepTabRow` Requirements:**

- Renders a single horizontal row of tabs
- Calculates tab widths based on available space and sizing mode
- Handles overflow within the row
- Supports pinned tabs at the left edge
- Supports active tab indicator animation
- Supports close button rendering per close mode
- DPI-aware, theme-driven

**`BeepTabRowContainer` Requirements:**

- Manages multiple `BeepTabRow` instances
- Automatically creates new row when tabs exceed single row capacity
- Distributes tabs across rows based on priority/pinned status
- Handles row height calculations
- Supports row collapse/expand

### Task 3.2: Implement Pinned Tab Section

**Requirements:**

- Dedicated pinned tab area at the left of each tab row
- Pinned tabs show icon-only by default
- Pinned tabs are always visible (never overflow)
- Pinned tabs can be expanded to show title on hover
- Pin/unpin via context menu or keyboard shortcut
- Pinned state persists in layout save/restore
- Visual separator between pinned and unpinned tabs

### Task 3.3: Enhance Tab Overflow System

**Changes to `BeepTabOverflowPopup`:**

- Add fuzzy search filtering
- Add category grouping in popup
- Add pinned section at top of popup
- Keyboard navigation: Up/Down arrows, Enter to select, Esc to close
- Show tab icon, title, and modified indicator in popup
- Support multi-column layout for large tab counts
- Show keyboard shortcut hints (Ctrl+1-9)

### Task 3.4: Implement Tab Grouping

**Requirements:**

- Visual separators between tab groups
- Group headers (optional, collapsible)
- Group by: category, file type, project, custom
- Drag tabs between groups
- Group colors (per-group accent)
- Collapse/expand groups
- Group context menu (close all in group, pin all, etc.)

### Task 3.5: Implement Tab Tear-Out

**Requirements:**

- Drag tab vertically beyond threshold to tear out
- Show tear-out preview (translucent tab following cursor)
- Drop to create new tab row
- Drop to create new group
- Drop to create float window
- Drop to transfer to another host
- Smooth animation on tear-out

### Task 3.6: Add 4 New Tab Styles

**New Styles:**

1. **Neumorphic** — Soft raised/inset effect with subtle shadows
2. **Glass** — Translucent background with blur effect
3. **Segmented** — iOS-style segmented control appearance
4. **Minimal** — Ultra-thin, no background, text-only with underline

**Requirements per Style:**

- Dedicated painter class (Phase 1 painter architecture)
- Support all states: inactive, hover, active, drag
- Theme-aware colors
- DPI-aware geometry
- High contrast mode support

### Task 3.7: Enhance Tab Density System

**Changes:**

- Comfortable: 36px height, 12pt font, full icons
- Compact: 28px height, 11pt font, small icons
- Dense: 22px height, 10pt font, icon-only
- Responsive breakpoints:
  - Width > 480px: Normal (full tabs)
  - Width 240-480px: Compact (title-only)
  - Width 120-240px: IconOnly
  - Width < 120px: ActiveOnly (only active tab visible)

### Task 3.8: Implement Workspace Model

**Files to Create:**

```
Workspace/
├── Workspace.cs                     ← Workspace definition
├── WorkspaceManager.cs              ← Workspace CRUD operations
├── WorkspaceSwitcher.cs             ← UI for switching workspaces
└── IWorkspaceProvider.cs            ← Interface for workspace sources
```

**`Workspace` Structure:**

```csharp
public class Workspace
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? IconPath { get; set; }
    public ILayoutNode LayoutTree { get; set; }
    public List<WorkspaceDocument> Documents { get; set; }
    public Dictionary<string, string> CustomData { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public class WorkspaceDocument
{
    public string DocumentId { get; set; }
    public string Title { get; set; }
    public string? FilePath { get; set; }
    public string? IconPath { get; set; }
    public bool IsPinned { get; set; }
    public bool IsModified { get; set; }
    public Dictionary<string, string> CustomData { get; set; }
}
```

**`WorkspaceManager` Requirements:**

- Create, read, update, delete workspaces
- Save workspaces to JSON files
- Load workspaces from JSON files
- List available workspaces
- Switch between workspaces (save current, load target)
- Auto-save current workspace on close
- Workspace templates (predefined layouts)

### Task 3.9: Enhance Tab Context Menu

**New Menu Items:**

- Close All But This
- Close All to the Right
- Close All in Group
- Pin/Unpin
- Float
- Copy Path
- Copy File Name
- Reveal in Explorer
- Open Containing Folder
- Move to New Row
- Move to New Group
- Move to Group > [group list]
- Tab Style > [style list]
- Tab Density > [density list]

### Task 3.10: Implement Tab Drag Reorder Enhancements

**Requirements:**

- Smooth drag preview (tab follows cursor)
- Drop indicator line between tabs
- Animated reposition on drop
- Drag between rows
- Drag between groups
- Drag between hosts
- Cancel drag on Escape
- Visual feedback during drag (highlight drop target)

---

## Acceptance Criteria

- [ ] Multiple tab rows per group
- [ ] Pinned tab section with icon-only display
- [ ] Fuzzy search in overflow popup
- [ ] Visual tab group separators
- [ ] Tab tear-out creates new row/group/float
- [ ] 4 new tab styles (Neumorphic, Glass, Segmented, Minimal)
- [ ] 3 density modes + responsive breakpoints
- [ ] Workspace save/switch/load
- [ ] Enhanced context menu with all items
- [ ] Smooth drag reorder with preview
- [ ] All tab operations DPI-aware
- [ ] All tab operations theme-aware
- [ ] Layout save/restore includes workspace state
- [ ] Keyboard shortcuts for all tab operations
- [ ] High contrast mode for all tab styles

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Multi-row layout performance | Virtual rendering for off-screen rows |
| Workspace switch loses unsaved changes | Prompt to save before switching |
| Tab tear-out creates invalid layout | Validate layout after tear-out |
| Pinned tabs overflow on narrow widths | Responsive breakpoints handle narrow widths |
| Complex tab grouping confuses users | Clear visual separators and group labels |

---

## Files Modified

| File | Change Type |
|------|-------------|
| `BeepDocumentTabStrip.cs` | Refactor (multi-row support) |
| `BeepDocumentTabStrip.Mouse.cs` | Enhance (tear-out, drag between rows) |
| `BeepDocumentTabStrip.Keyboard.cs` | Enhance (workspace shortcuts) |
| `BeepDocumentTabStrip.ContextMenu.cs` | Enhance (new menu items) |
| `BeepDocumentTabStrip.Overflow.cs` | Enhance (fuzzy search) |
| `BeepDocumentHost.Serialisation.cs` | Update (workspace serialization) |
| `Tokens/DocumentHostTokens.cs` | Update (new tokens for rows, pinned, workspace) |

## Files Created

| File | Purpose |
|------|---------|
| `Tabs/BeepTabRow.cs` | Single tab row |
| `Tabs/BeepTabRowContainer.cs` | Multi-row container |
| `Tabs/BeepTabRowLayout.cs` | Row layout calculations |
| `Workspace/Workspace.cs` | Workspace definition |
| `Workspace/WorkspaceManager.cs` | Workspace CRUD |
| `Workspace/WorkspaceSwitcher.cs` | Workspace switcher UI |
| `Workspace/IWorkspaceProvider.cs` | Workspace source interface |
| `Painters/NeumorphicTabPainter.cs` | Neumorphic style |
| `Painters/GlassTabPainter.cs` | Glass style |
| `Painters/SegmentedTabPainter.cs` | Segmented style |
| `Painters/MinimalTabPainter.cs` | Minimal style |
