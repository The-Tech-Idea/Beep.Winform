# BeepDocumentHost Design Server

Design-time extensions for `BeepDocumentHost` — custom designers, smart-tag action lists,
collection editors, and dialogs for the WinForms VS designer.

---

## Folder Structure

| Folder | Contents |
|---|---|
| `ActionLists/` | `DocumentHostActionList` — smart-tag property bindings + method actions |
| `Designers/` | `BeepDocumentHostDesigner`, `LayoutPresetPickerDialog`, `ThemePickerDialog` |
| `Editors/` | `DocumentDescriptorCollectionEditor`, `DesignTimeDocumentsEditor`, `IconPickerDialog` |
| `Helpers/` | Internal utilities used by designers |

---

## Key Components

### `BeepDocumentHostDesigner` (Designers/)
Extends `ParentControlDesigner`. Provides:
- Locked child controls (tab strip, content panel cannot be moved/deleted)
- 7 designer verbs (Add, Close, Split H/V, Merge, Edit Docs, Apply Preset)
- `OnPaintAdornments` — empty-state hint + **Sprint 17 docking compass** during drag
- `SnapLines` — content area edges for sibling alignment
- `PreFilterProperties` / `PreFilterEvents` — hides irrelevant base properties/events
- `DoDefaultAction` — double-click adds a document

#### Sprint 17.1 — Docking Guide Adorner
When a control is dragged onto the host, `OnPaintAdornments` draws a 5-point compass:

| Zone | Result |
|---|---|
| Center | Drop into existing tab group |
| Left / Right | `SplitDocumentHorizontal` |
| Top / Bottom | `SplitDocumentVertical` |

Zone hit-testing uses the host's client-space coordinates (`HitTestCompass`).
The split is wrapped in a `DesignerTransaction` for full undo/redo support.

---

### `DocumentHostActionList` (ActionLists/)
`DesignerActionList` with 11 groups:
- **Documents** — Add, Close, Close All, Reopen, Quick Switch, Float, Pin/Unpin
- **Design-Time** — Edit Design-Time Documents…
- **Split View** — Split H/V, Merge, Apply Layout Preset…, MaxGroups, SplitRatio
- **Tabs** — TabStyle, TabPosition, CloseMode, ShowAddButton, TabColorMode
- **Tab Sizing** — TabSizeMode, FixedTabWidth
- **Interaction** — TabTooltipMode, AllowDragFloat
- **Style Presets** — Chrome / VS Code / Flat / Office / Pill / Underline one-click
- **Appearance** — ControlStyle, Theme, **Choose Theme… (Sprint 17.4)**
- **Preview** — TabPreviewEnabled
- **History** — MaxRecent/MaxClosed
- **Session** — AutoSaveLayout, SessionFile

---

### `DocumentDescriptorCollectionEditor` (Editors/) — Sprint 17.2
Full custom `CollectionEditor` for `DesignTimeDocuments`.

**`DocumentDescriptorEditorForm`** — custom `Form` with:
- `DataGridView`: Id / Title / IconPath / IsPinned / CanClose / InitialContent / AccentColor
- Icon picker column → opens `IconPickerDialog` (full Beep icon library browser)
- Accent colour column → opens `ColorDialog`
- **▲ Up / ▼ Down** — in-list reorder
- **+ Add / − Remove** buttons
- **Mini tab-strip preview** — live GDI+ rendering of configured tabs at the bottom

Using the editor:
1. Select the `BeepDocumentHost` in the designer
2. Smart-tag (▶) → **Edit Design-Time Documents…**  
   — or — Properties grid → `DesignTimeDocuments` → click `…`

---

### `ThemePickerDialog` (Designers/) — Sprint 17.4
Visual theme picker with:
- Scrollable `FlowLayoutPanel` tile grid (150 × 80 px tiles)
- Each tile shows: theme name, primary-colour accent stripe, 3 colour dots, dark/light badge
- Real-time search filter (`TextBox`)
- Loads from `BeepThemesManager.GetThemes()` via reflection; 15-theme fallback when unavailable
- Double-click a tile → immediate OK

---

### `LayoutPresetPickerDialog` (Designers/) — Sprint 12/17.3
5 visual preset tiles:

| Preset | Description |
|---|---|
| Single | One tab group, full area |
| Side-by-Side | Two groups, horizontal split |
| Stacked | Two groups, vertical split |
| Three-Way | Three groups in L-pattern |
| Four-Up | Four groups in 2×2 grid |

---

## Sprint History

| Sprint | Deliverable |
|---|---|
| 11 | Designer transaction safety for all verbs |
| 12 | 7 designer verbs, smart-tag 12 groups, `LayoutPresetPickerDialog`, `DocumentDescriptorCollectionEditor` v1 |
| 13 | Accessibility, high-contrast, arrow key nav |
| 14 | Fluent tab style |
| 15–16 | Status bar, MVVM, command service, options |
| **17** | **Docking guide compass (OnPaintAdornments), `DocumentDescriptorCollectionEditor` v2 (grid+icon+color+reorder+preview), `ThemePickerDialog`, "Choose Theme…" smart-tag action** |

---

*Last updated: Sprint 17*
