# Phase 6: Advanced Navigation & Command System

> **Sprint:** 28 · **Priority:** Medium · **Complexity:** Medium
> **Dependency:** Phase 3 complete · **Estimated Effort:** 2 weeks

---

## Objective

Implement a command palette (fuzzy search all documents and commands), breadcrumb navigation, workspace switcher, and 50+ power-user keyboard shortcuts. This phase transforms the navigation experience from basic tab switching to a full command-driven interface.

---

## Current Limitations

| Limitation | Current State | Target State |
|------------|--------------|--------------|
| Document search | MRU quick-switch only | Fuzzy search all documents |
| Command access | Menu/context menu only | Command palette (Ctrl+Shift+P) |
| Breadcrumb | None | File path breadcrumb bar |
| Workspace switch | Manual load/save | Visual workspace switcher |
| Keyboard shortcuts | Basic (10 shortcuts) | 50+ power-user shortcuts |
| Keyboard chords | None | Ctrl+K, Ctrl+S pattern |
| Go to file | None | Fuzzy file search |
| Split navigation | Manual | Keyboard focus between splits |

---

## Tasks

### Task 6.1: Implement Command Palette

**Files to Create:**

```
Navigation/
├── BeepCommandPalette.cs            ← Command palette control
├── BeepCommandEntry.cs              ← Command definition
├── BeepCommandRegistry.cs           ← Command registry
└── BeepCommandPalettePopup.cs       ← Palette popup window
```

**`BeepCommandPalette` Requirements:**

- Fuzzy search across all commands and documents
- Command categories: Documents, Navigation, View, Layout, Tabs, Docking, Workspace
- Keyboard shortcuts shown next to commands
- Command execution on Enter
- Command filtering on typing
- Recent commands section at top
- Command icons for visual identification
- Theme-colored, DPI-aware
- Smooth open/close animation

**Command Entry Structure:**

```csharp
public class BeepCommandEntry
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public string? IconPath { get; set; }
    public string? Shortcut { get; set; }
    public Action Execute { get; set; }
    public Func<bool>? CanExecute { get; set; }
    public int UsageCount { get; set; }
    public DateTime? LastUsed { get; set; }
}
```

**Built-in Commands:**

| Command | Shortcut | Description |
|---------|----------|-------------|
| New Document | Ctrl+N | Create new document |
| Close Document | Ctrl+W | Close active document |
| Close All | Ctrl+K Ctrl+W | Close all documents |
| Reopen Closed | Ctrl+Shift+T | Reopen last closed document |
| Quick Switch | Ctrl+Tab | Switch between documents |
| Command Palette | Ctrl+Shift+P | Open command palette |
| Go to File | Ctrl+P | Fuzzy file search |
| Split Horizontal | Ctrl+K Ctrl+\ | Split active document horizontally |
| Split Vertical | Ctrl+K Ctrl+| | Split active document vertically |
| Merge All Groups | Ctrl+K Ctrl+M | Merge all split groups |
| Toggle Auto-Hide | Ctrl+K Ctrl+H | Toggle auto-hide for active document |
| Float Document | Ctrl+K Ctrl+F | Float active document |
| Pin Document | Ctrl+K Ctrl+P | Pin/unpin active document |
| Next Tab | Ctrl+PgDn | Activate next tab |
| Previous Tab | Ctrl+PgUp | Activate previous tab |
| Jump to Tab 1-9 | Ctrl+1-9 | Jump to nth tab |
| Focus Left Split | Ctrl+K Ctrl+Left | Move focus to left split |
| Focus Right Split | Ctrl+K Ctrl+Right | Move focus to right split |
| Focus Top Split | Ctrl+K Ctrl+Up | Move focus to top split |
| Focus Bottom Split | Ctrl+K Ctrl+Down | Move focus to bottom split |
| Toggle Tab Position | Ctrl+K Ctrl+T | Cycle tab position (top/bottom/left/right) |
| Toggle Tab Style | Ctrl+K Ctrl+S | Cycle tab style |
| Toggle Density | Ctrl+K Ctrl+D | Cycle tab density |
| Save Layout | Ctrl+K Ctrl+L | Save current layout |
| Load Layout | Ctrl+K Ctrl+O | Load saved layout |
| Toggle Minimap | Ctrl+K Ctrl+M | Toggle minimap visibility |
| Toggle Breadcrumb | Ctrl+K Ctrl+B | Toggle breadcrumb bar |
| Toggle Terminal | Ctrl+` | Toggle terminal panel |
| Toggle Fullscreen | F11 | Toggle fullscreen mode |
| Show Keyboard Shortcuts | Ctrl+K Ctrl+H | Show all keyboard shortcuts |

### Task 6.2: Implement Breadcrumb Navigation

**Files to Create:**

```
Navigation/
├── BeepDocumentBreadcrumb.cs        ← Breadcrumb bar control
└── BeepBreadcrumbItem.cs            ← Breadcrumb item definition
```

**`BeepDocumentBreadcrumb` Requirements:**

- Show file path as clickable breadcrumb
- Each segment is clickable (navigate to parent)
- Overflow handling (collapse middle segments)
- Theme-colored, DPI-aware
- Position above content area
- Configurable visibility
- Keyboard navigation (Left/Right arrows between segments)
- Context menu on each segment (open in explorer, copy path, etc.)

### Task 6.3: Implement Workspace Switcher

**Files to Create:**

```
Navigation/
├── BeepWorkspaceSwitcher.cs         ← Workspace switcher control
└── BeepWorkspaceSwitcherPopup.cs    ← Switcher popup window
```

**`BeepWorkspaceSwitcher` Requirements:**

- Show list of saved workspaces
- Workspace preview (thumbnail of layout)
- Switch workspace on selection
- Create new workspace
- Delete workspace
- Rename workspace
- Set default workspace
- Keyboard navigation (Up/Down arrows, Enter to select)
- Fuzzy search workspace names
- Theme-colored, DPI-aware

### Task 6.4: Implement Keyboard Chord System

**Requirements:**

- Support chord patterns: Ctrl+K then Ctrl+S
- Chord timeout (cancel chord after 2 seconds)
- Visual chord hint (show pending chord)
- Chord conflict detection
- Configurable chord bindings
- Chord help (show all chords starting with prefix)

### Task 6.5: Implement Go to File

**Requirements:**

- Fuzzy file search across all open documents
- Show file path, icon, and modified indicator
- Navigate results with Up/Down arrows
- Open file on Enter
- Open file in new split on Ctrl+Enter
- Open file in new row on Ctrl+Shift+Enter
- Recent files section at top
- Theme-colored, DPI-aware

### Task 6.6: Implement Split Navigation

**Requirements:**

- Move focus between splits with keyboard
- Visual focus indicator on active split
- Move document between splits with keyboard
- Resize splits with keyboard (Ctrl+Alt+Arrows)
- Close split with keyboard
- Merge split with keyboard

### Task 6.7: Implement Keyboard Shortcut Help

**Requirements:**

- Show all keyboard shortcuts in searchable list
- Group shortcuts by category
- Filter shortcuts by search text
- Show chord patterns clearly
- Print-friendly layout
- Export shortcuts to JSON/CSV

---

## Acceptance Criteria

- [ ] Command palette with fuzzy search
- [ ] 30+ built-in commands
- [ ] Command execution on Enter
- [ ] Breadcrumb navigation with clickable segments
- [ ] Workspace switcher with preview
- [ ] Keyboard chord system (Ctrl+K pattern)
- [ ] Go to file with fuzzy search
- [ ] Split navigation with keyboard
- [ ] 50+ keyboard shortcuts
- [ ] Keyboard shortcut help panel
- [ ] All navigation controls DPI-aware
- [ ] All navigation controls theme-aware
- [ ] Smooth animations for all popups
- [ ] High contrast mode support

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Command palette performance with many commands | Virtual rendering for command list |
| Keyboard chords conflict with system shortcuts | Use configurable chord prefix (default Ctrl+K) |
| Breadcrumb overflow on long paths | Collapse middle segments with ellipsis |
| Workspace switcher loses unsaved changes | Prompt to save before switching |
| Go to file search is slow | Index files on background thread |

---

## Files Modified

| File | Change Type |
|------|-------------|
| `BeepDocumentHost.Keyboard.cs` | Enhance (chord system, more shortcuts) |
| `BeepDocumentQuickSwitch.cs` | Enhance (fuzzy search, file navigation) |
| `BeepDocumentHost.cs` | Enhance (command registry) |
| `Tokens/DocumentHostTokens.cs` | Update (navigation tokens) |

## Files Created

| File | Purpose |
|------|---------|
| `Navigation/BeepCommandPalette.cs` | Command palette |
| `Navigation/BeepCommandEntry.cs` | Command definition |
| `Navigation/BeepCommandRegistry.cs` | Command registry |
| `Navigation/BeepCommandPalettePopup.cs` | Palette popup |
| `Navigation/BeepDocumentBreadcrumb.cs` | Breadcrumb bar |
| `Navigation/BeepBreadcrumbItem.cs` | Breadcrumb item |
| `Navigation/BeepWorkspaceSwitcher.cs` | Workspace switcher |
| `Navigation/BeepWorkspaceSwitcherPopup.cs` | Switcher popup |
