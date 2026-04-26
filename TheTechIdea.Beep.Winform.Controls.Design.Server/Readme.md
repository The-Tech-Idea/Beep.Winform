# BeepDocumentHost Design Server

Design-time extensions for Beep WinForms controls, including `BeepDocumentHost` and the fresh-start integrated forms stack. The design server ships custom designers, smart-tag action lists, collection editors, typed converters, and dialogs for the WinForms VS designer.

---

## Folder Structure

| Folder | Contents |
|---|---|
| `ActionLists/` | Shared smart-tag infrastructure such as `CommonBeepControlActionList`, `ImagePathDesignerActionList`, `ContainerControlActionList`, `DataControlActionList`, `DocumentHostActionList`, and `WizardConfigActionList` |
| `Designers/` | `BeepDocumentHostDesigner`, `LayoutPresetPickerDialog`, `ThemePickerDialog`, **`GroupTabPositionDialog`**, **`LayoutTreeDialog`** |
| `Editors/` | `DocumentDescriptorCollectionEditor`, `DesignTimeDocumentsEditor`, `IconPickerDialog`, `IntegratedFormsDefinitionEditors` |
| `Helpers/` | Internal utilities used by designers |

---

## Key Components

### `IntegratedFormsDefinitionEditors` (Editors/)
Focused modal editors and type converters for the integrated forms path.

Provides:
- `BeepFormsDefinitionEditor` and `BeepBlockDefinitionEditor` for modal definition editing
- Collection editors for `Blocks`, `Fields`, and field `Options`
- `BeepFormsBlockNameTypeConverter` so `BeepBlock.BlockName` can suggest nearby `BeepForms.Definition` block names
- `BeepFieldEditorKeyTypeConverter` so `BeepFieldDefinition.EditorKey` exposes the default presenter keys (`text`, `numeric`, `date`, `checkbox`, `combo`, `lov`, `option`)
- `BeepFieldControlTypeTypeConverter` and `BeepFieldBindingPropertyTypeConverter` so block fields and entity-field snapshots can persist an explicit control class plus binding property in `Designer.cs`, with suggestions hydrated from the shared `BeepFieldControlTypeRegistry`
- `BeepStringDictionaryEditor` for focused key/value editing of `BeepFormsDefinition.Metadata` and `BeepBlockDefinition.Metadata`

Shared field default policy:
- Integrated field generation now resolves defaults through `BeepFieldControlTypeRegistry`.
- Built-in defaults still cover text, numeric, date, checkbox, and combo/LOV presenters.
- Optional override rules can be stored in `%LocalAppData%\TheTechIdea\Beep.Winform\field-control-defaults.json` and matched by field category, data-type pattern, `IsCheck`, and editor key.
- `BeepBlock` design-time surfaces now expose a first-class policy editor through the smart tag, the field-property editor, and the setup wizard field step, so global default-control changes no longer require hand-editing the JSON file.
- `BeepBlockEntityFieldDefinition` now exposes `EditorKey`, `ControlType`, and `BindingProperty` so entity snapshots can preseed generated fields before those rows are materialized into `BeepFieldDefinition` entries.

Using the integrated editors:
1. Select `BeepForms` or `BeepBlock` in the designer.
2. Use the smart-tag or properties grid to open the modal definition editor.
3. Use the definition editor to add blocks, fields, and options.
4. Use the field editor to add, remove, or update generated field definitions, including editor key, explicit control type, and binding property.
5. For manually placed blocks, pick `BlockName` from the suggested host block list when a nearby `BeepForms` definition is present.
6. When you need global default changes instead of per-field overrides, use `Edit Field Default Policy...` from the `BeepBlock` smart tag or the `Default Policy...` button inside the field editor or setup wizard.
7. The policy editor persists back to `%LocalAppData%\TheTechIdea\Beep.Winform\field-control-defaults.json` and applies immediately to newly generated block fields.
8. `BeepBlock` smart tags now expose direct `Caption`, `Manager Block Name`, `Presentation Mode`, `Navigation Settings`, and `Field Controls Layout` editing, including a dedicated `Edit Navigation Settings...` action plus quick presets for `DesignerGenerated` with Stacked Vertical, Label Field Pairs, and Grid layout metadata.
9. The BeepBlock setup wizard now supports `DesignerGenerated` presentation mode, persists `FieldControlsLayoutMode` metadata when that mode is selected, and shows a live layout preview so authors can see the generated composition before finishing.
10. The field editor now preserves `DefaultValue`, loads rows in persisted field order, and supports explicit Move Up / Move Down row ordering without forcing manual order edits.

### `BeepBlockDesigner` (Designers/)
The integrated block smart tag now covers the current BeepBlock definition contract directly.

Provides:
- Block-level smart-tag properties for `BlockName`, `Caption`, `ManagerBlockName`, `PresentationMode`, `Navigation`, and full `Definition`
- Quick actions to create starter definitions, capture entity metadata from FormsManager, rebuild fields from the entity snapshot, open the setup wizard or field editor, and launch a dedicated navigation editor dialog without opening the full definition object
- Dedicated `Designer Generated` actions that switch the block into `BeepBlockPresentationMode.DesignerGenerated` and persist the selected generated field-controls layout in `Definition.Metadata["FieldControlsLayoutMode"]`
- Smart-tag `BlockName` edits now keep the component property and `Definition.BlockName` synchronized, and the standalone `Field Controls Layout` property only appears while the block is already running in `DesignerGenerated` mode
- Baseline image-backed controls now share the same Beep style/theme smart tags as the rest of the suite: `BeepButtonDesigner` and `BeepLabelDesigner` now inherit the common designer base, `BeepPanelDesigner` now uses a shared parent-control base plus container/header smart tags, and the panel header image/title presets are available from the smart-tag surface instead of only the designer verb menu
- Data-entry designer gaps were tightened for the common pickers: `BeepComboBoxDesigner` now maps multi-select to the real `AllowMultipleSelection` runtime property and exposes searchable / chip / command-menu presets, while `BeepListBoxDesigner` now maps search to `ShowSearch` and surfaces selection, grouping, density, loading, and data-binding properties that actually exist on the runtime control
- The integrated forms shell family now shares `BaseBeepParentControlDesigner`: `BeepFormsHostDesigner`, `BeepFormsHeaderDesigner`, `BeepFormsStatusStripDesigner`, `BeepFormsQueryShelfDesigner`, `BeepFormsCommandBarDesigner`, `BeepFormsPersistenceShelfDesigner`, and `BeepFormsToolbarDesigner` no longer duplicate local change-service and property plumbing just to expose control-specific presets
- Container designers were brought closer to the `BeepDocumentHost` experience: `BeepTabsDesigner` now exposes selected-tab caption editing plus add / remove / clear / reorder / select page actions from both smart tags and designer verbs, and it now syncs the VS designer selection service to the active `TabPage` after add / remove / navigation actions so focus stays on the authored page; `BeepLayoutControlDesigner` now exposes restore / clear generated-child actions and workspace-oriented 2 x 2 grid and split presets in addition to raw template switching
- Thin widget designers now expose the runtime switches that make presets usable instead of only `Style` and `Title`: calendar, chart, dashboard, finance, form, map, metric, navigation, control, list, media, notification, and social widgets now surface key behavior toggles directly from the smart-tag panel
- Reusable smart-tag helpers now live under `ActionLists/` instead of being split across designer files: `CommonBeepControlActionList` and `ImagePathDesignerActionList` were moved alongside the other shared action-list types, while control-specific action lists still stay next to their designer when they only serve one control surface
- Designers that expose custom verb collections now route those commands through explicit designer helpers and override `Verbs` so smart-tag actions do not depend on fragile `Verbs[index]` assumptions; this tightened the icon-picking surfaces on `BeepToggleDesigner`, `BeepSwitchDesigner`, and `BeepExtendedButtonDesigner`
- Toggle-family icon surfaces now target the real runtime properties (`OnIconPath` / `OffIconPath` for `BeepToggle`, `OnIconName` / `OffIconName` for `BeepSwitch`) instead of placeholder names, and `BeepCheckBoxDesigner` style presets now apply the helper-backed recommended checkbox size and spacing together with the selected visual style
- `BeepToggleDesigner` icon presets now also switch the control to `ToggleStyle.IconCustom`, matching the runtime examples where custom icon paths are only meaningful when the toggle is in a custom-icon rendering mode
- `BeepDockDesigner` style presets now defer to the runtime `DockStyleType` setter for recommended item size, dock height, spacing, padding, and scale instead of hardcoding duplicate values that can drift from `DockStyleHelpers`; the dock smart tag also now exposes runtime-backed `AnimationStyle`, `IconMode`, `ShowTooltips`, and `ShowBadges` properties
- `BeepCardDesigner` now reuses the shared image smart-tag surface and exposes the card-specific runtime fields that its painters actually depend on (`SubtitleText`, secondary button text/visibility, badge text, status text/visibility, rating, and accent color); the built-in card presets were also tightened so metric, testimonial, profile, pricing, product, and stat presets populate those painter-backed fields instead of only header/paragraph/button text
- `BeepGridProDesigner` no longer exposes a dead sample-data action: `Generate Sample Data` now binds a non-serialized design-time preview row set through the grid's existing binder and pairs it with `Clear Sample Preview`, so the smart tag can show a reversible header/row preview without claiming unsupported behavior
- Shared data-binding smart tags were narrowed to the real common surface: `DataControlActionList` now only exposes binding properties plus `Clear Data Binding`, while placeholder `Configure Data Source` and `Generate Sample Data` actions were removed so sample/config flows stay on the control-specific designers that actually implement them
- Shared container smart tags were narrowed the same way: `ContainerControlActionList` now only exposes implemented `Arrange Children` and `Clear All Children` actions, while the old dock/flow layout entries were removed because `BeepPanelDesigner` never had a real shared implementation behind them

Wizard / editor support:
- Setup wizard step 4 now offers Record, Grid, and Designer Generated presentation modes
- Designer Generated mode includes layout selection for `StackedVertical`, `LabelFieldPairs`, and `GridLayout`, plus a live preview panel that visualizes the selected composition alongside Record and Grid previews
- Retargeting the setup wizard to a different entity now defaults that entity's fields selected, while finishing the wizard on the same entity preserves explicitly removed baseline fields unless the author reselects them; intentionally removing every field now persists as an explicit empty state instead of silently regenerating the entity defaults on the next round-trip
- Entity snapshots captured through the wizard now preserve the lossless field flags used by runtime scaffolding: `IsIdentity`, `IsHidden`, `IsLong`, `IsRowVersion`, and `DefaultValue`
- Field editor rows now round-trip `DefaultValue`, honor persisted ordering on load, and expose move-up / move-down ordering helpers, while the generic collection editors also renormalize `Order` after add/remove/move so property-grid edits stay aligned with the dedicated field editor

---

### `BeepDocumentHostDesigner` (Designers/)
Extends `ParentControlDesigner`. Provides:
- Locked child controls (tab strip, content panel cannot be moved/deleted)
- 9 designer verbs (Add, Close, Split H/V, Merge, Edit Docs, Apply Preset, **Set Group Tab Position**, **View Layout Tree**)
- `OnPaintAdornments` — empty-state hint + **Sprint 17 docking compass** during drag
- `SnapLines` — content area edges for sibling alignment
- `PreFilterProperties` / `PreFilterEvents` — hides irrelevant base properties/events
- `DoDefaultAction` — double-click adds a document

#### Sprint 19 — Nested Split Design-Time Support
- `GroupTabPositionDialog` — per-group tab strip position editor for asymmetric nested layouts
- `LayoutTreeDialog` — read-only tree viewer showing the current `ILayoutNode` structure
- Smart-tag **Nested Splits** group with `GroupTabPositions` (editable) and `LayoutTreeInfo` (read-only)
- 3 new layout presets: **Three-Way Nested**, **Three Column**, **Five-Way**

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
`DesignerActionList` with 13 groups:
- **Documents** — Add, Close, Close All, Reopen, Quick Switch, Float, Pin/Unpin
- **Design-Time** — Edit Design-Time Documents…
- **Split View** — Split H/V, Merge, Apply Layout Preset…, MaxGroups, SplitHorizontal, SplitRatio
- **Nested Splits** — GroupTabPositions (editable), LayoutTreeInfo (read-only tree view)
- **Tabs** — TabStyle, TabPosition, CloseMode, ShowAddButton, TabColorMode
- **Tab Sizing** — TabSizeMode, FixedTabWidth
- **Interaction** — TabTooltipMode, AllowDragFloat
- **Style Presets** — Chrome / VS Code / Flat / Office / Pill / Underline one-click
- **Appearance** — ControlStyle, Theme, **Choose Theme… (Sprint 17.4)**
- **Preview** — TabPreviewEnabled
- **History** — MaxRecent/MaxClosed
- **Cross-Host Drag** — AllowDragBetweenHosts
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

### `LayoutPresetPickerDialog` (Designers/) — Sprint 12/17.3/19
8 visual preset tiles:

| Preset | Description |
|---|---|
| Single | One tab group, full area |
| Side-by-Side | Two groups, horizontal split |
| Stacked | Two groups, vertical split |
| Three-Way | Three groups in L-pattern |
| **Three-Way Nested** | **Three groups: left | right (nested top/bottom)** |
| Four-Up | Four groups in 2×2 grid |
| **Three Column** | **Three equal columns** |
| **Five-Way** | **Five groups: left | 2×2 right grid** |

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
| **19** | **Nested split design-time: `GroupTabPositionDialog`, `LayoutTreeDialog`, 3 new presets, Nested Splits smart-tag group, 2 new designer verbs** |

---

*Last updated: Sprint 19*
