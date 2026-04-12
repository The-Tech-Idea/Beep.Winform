# Phase 7: Collaboration, Templates & Cloud Sync

> **Sprint:** 29 · **Priority:** Medium · **Complexity:** Low
> **Dependency:** Phase 6 complete · **Estimated Effort:** 2 weeks

---

## Objective

Implement layout templates, workspace sharing, cloud sync for layouts, and team-shared workspace templates. This phase enables users to save, share, and sync their workspace configurations across machines and team members.

---

## Current Limitations

| Limitation | Current State | Target State |
|------------|--------------|--------------|
| Layout templates | Basic presets | Named, customizable templates |
| Workspace sharing | None | Export/import, team sharing |
| Cloud sync | None | Azure Blob Storage sync |
| Template marketplace | None | Share templates with team |
| Layout versioning | Schema migration only | Full version history |
| Layout comparison | None | Diff between layouts |
| Layout undo/redo | None | Multi-level undo/redo |

---

## Tasks

### Task 7.1: Implement Layout Template System

**Files to Create:**

```
Templates/
├── LayoutTemplate.cs                ← Template definition
├── LayoutTemplateManager.cs         ← Template CRUD operations
├── LayoutTemplatePicker.cs          ← Visual template picker
└── LayoutTemplateCategory.cs        ← Template categories
```

**`LayoutTemplate` Structure:**

```csharp
public class LayoutTemplate
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? IconPath { get; set; }
    public string Category { get; set; }
    public ILayoutNode LayoutTree { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    public DateTime Created { get; set; }
    public string Author { get; set; }
    public string Version { get; set; }
}
```

**Built-in Templates:**

| Template | Description |
|----------|-------------|
| Single | One tab group fills the area |
| Side-by-Side | Two groups split left/right |
| Stacked | Two groups split top/bottom |
| Three-Way | L-pattern (horizontal + vertical sub-split) |
| Four-Up | 2x2 grid |
| Code Review | Left: code, Right: diff viewer |
| Debug | Top: code, Bottom: output + watch |
| Data Explorer | Left: tree, Right: grid, Bottom: query |
| Designer | Center: canvas, Left: toolbox, Right: properties, Bottom: output |
| Terminal Focus | Top: code, Bottom: terminal (70/30) |

### Task 7.2: Implement Workspace Export/Import

**Requirements:**

- Export workspace to JSON file
- Export workspace to ZIP (includes layout + metadata)
- Import workspace from JSON file
- Import workspace from ZIP
- Validate imported workspace before applying
- Merge imported workspace with current (optional)
- Export/import workspace templates
- Share workspace via clipboard (base64 JSON)

### Task 7.3: Implement Cloud Sync

**Files to Create:**

```
Cloud/
├── CloudSyncProvider.cs             ← Cloud sync provider interface
├── AzureBlobSyncProvider.cs         ← Azure Blob Storage implementation
├── CloudSyncManager.cs              ← Sync orchestration
└── CloudSyncSettings.cs             ← Sync configuration
```

**`CloudSyncManager` Requirements:**

- Sync workspaces to cloud storage
- Sync layout templates to cloud storage
- Resolve sync conflicts (last-write-wins, manual merge)
- Sync on save, sync on load, sync on schedule
- Offline support (queue changes, sync when online)
- Sync status indicator
- Sync conflict resolution UI
- Configurable sync frequency

### Task 7.4: Implement Template Marketplace

**Requirements:**

- Browse shared templates from team
- Download and apply templates
- Rate and review templates
- Search templates by name, category, tags
- Template preview (thumbnail of layout)
- Template metadata (author, description, usage count)
- Template versioning
- Template categories

### Task 7.5: Implement Layout Versioning

**Requirements:**

- Save layout versions automatically
- Configurable version history depth (default: 20)
- Browse layout versions
- Compare layout versions
- Restore layout version
- Delete layout versions
- Layout version metadata (timestamp, document count, split count)

### Task 7.6: Implement Layout Comparison

**Requirements:**

- Diff between two layouts
- Visual diff view (side-by-side or unified)
- Show added/removed/changed documents
- Show added/removed/changed splits
- Show ratio changes
- Export diff report
- Merge changes from one layout to another

### Task 7.7: Implement Layout Undo/Redo

**Requirements:**

- Multi-level undo/redo for layout changes
- Track: document add/remove, split create/merge, document move, ratio change
- Configurable undo stack depth (default: 50)
- Undo/redo via keyboard (Ctrl+Z, Ctrl+Y)
- Undo/redo via command palette
- Undo/redo history viewer
- Clear undo history

---

## Acceptance Criteria

- [ ] 10+ built-in layout templates
- [ ] Custom template creation
- [ ] Template picker with preview
- [ ] Workspace export to JSON/ZIP
- [ ] Workspace import from JSON/ZIP
- [ ] Cloud sync with Azure Blob Storage
- [ ] Offline sync support
- [ ] Sync conflict resolution
- [ ] Template marketplace (browse, download, apply)
- [ ] Layout versioning (20 versions)
- [ ] Layout comparison (diff view)
- [ ] Layout undo/redo (50 levels)
- [ ] All operations DPI-aware
- [ ] All operations theme-aware

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Cloud sync conflicts | Last-write-wins with manual merge option |
| Template compatibility across versions | Schema validation on import |
| Large layout files | Compression for ZIP export |
| Undo/redo memory usage | Bounded stack with configurable depth |
| Template marketplace security | Validate templates before applying |

---

## Files Modified

| File | Change Type |
|------|-------------|
| `BeepDocumentHost.Serialisation.cs` | Enhance (versioning, export/import) |
| `BeepDocumentHost.Layout.cs` | Enhance (undo/redo tracking) |
| `Workspace/WorkspaceManager.cs` | Enhance (cloud sync) |
| `Tokens/DocumentHostTokens.cs` | Update (template tokens) |

## Files Created

| File | Purpose |
|------|---------|
| `Templates/LayoutTemplate.cs` | Template definition |
| `Templates/LayoutTemplateManager.cs` | Template CRUD |
| `Templates/LayoutTemplatePicker.cs` | Template picker UI |
| `Templates/LayoutTemplateCategory.cs` | Template categories |
| `Cloud/CloudSyncProvider.cs` | Cloud sync interface |
| `Cloud/AzureBlobSyncProvider.cs` | Azure Blob implementation |
| `Cloud/CloudSyncManager.cs` | Sync orchestration |
| `Cloud/CloudSyncSettings.cs` | Sync configuration |
