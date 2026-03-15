# BeepListBox UI/UX Enhancement & Fix Plan

> Revised against **Material Design 3**, **Fluent UI 2**, **Figma Auto-Layout** standards, and **WCAG 2.2 AA** accessibility requirements.

## Summary

| Area | Severity | Current State |
|------|----------|---------------|
| **Search Box** | **Broken** | `ShowSearch=true` paints a static gray rectangle — no input control exists, users cannot type |
| **Item Alignment** | **Deficient** | Text/icons/checkboxes use hardcoded spacing (8px) instead of `ListBoxTokens` (12px), text not vertically centered |
| **Scrolling** | **Buggy** | Virtual size doesn't subtract search area, mouse wheel fires unnecessary invalidation, scroll not clamped on filter |
| **Hierarchical List** | **Missing** | `SimpleItem.Children` and `IsExpanded` exist but `GetVisibleItems()` never flattens hierarchy, no indent, no chevron |
| **Modern UX Gaps** | **Missing** | Tooltips, badge rendering, separator painting, disabled opacity, focus ring, empty-search state, trailing metadata, pinned items — all have model properties but no painter/layout support |

## Root Causes

1. **Search**: The old `TextBox` control (visible in `BeepListBox.cs.old`) was removed during the painter refactor. `BaseListBoxPainter.DrawSearchArea()` was introduced as a placeholder but only draws a gray rectangle — no real control was created to replace it.

2. **Alignment**: `BeepListBoxLayoutHelper.CalculateLayout()` uses `paddingX = ScaleValue(8)` while `ListBoxTokens.ItemPaddingH = 12`. Text rect starts at row top instead of vertically centered. `BaseListBoxPainter.DrawItem()` hardcodes image size to 32px regardless of `_owner.ImageSize`.

3. **Scrolling**: `_virtualSize` is set as full content height but compared against the full drawing rect — search area height isn't subtracted. `InvalidateLayoutCache()` never clamps `_yOffset`.

4. **Hierarchy**: `SimpleItem` already has `BindingList<SimpleItem> Children`, `IsExpanded`, `ParentItem`, `ParentID`, and `ComposedID`. But `BeepListBoxHelper.GetVisibleItems()` only iterates top-level `_owner.ListItems` — it never recurses into children. `BeepListBoxLayoutHelper` has no indentation logic. No expand/collapse chevron is drawn.

5. **Modern UX**: Several `SimpleItem` and `BeepListItem` properties (ToolTip, BadgeText, IsSeparator, ShortcutText, TrailingMeta, IsPinned, IsDisabled) are defined in the model but have no corresponding rendering in `BaseListBoxPainter`. Focus ring properties exist but aren't consistently drawn by painters.

## Phases

| Phase | Document | Scope | Priority |
|-------|----------|-------|----------|
| 1 | [Phase 1 — Search Box](./01-phase1-searchbox.md) | Replace painted placeholder with real `BeepTextBox` control | **Critical** |
| 2 | [Phase 2 — Item Alignment](./02-phase2-alignment.md) | Apply `ListBoxTokens` consistently, fix vertical centering | **High** |
| 3 | [Phase 3 — Scrolling](./03-phase3-scrolling.md) | Fix virtual size, mouse wheel, scroll clamping | **High** |
| 4 | [Phase 4 — Cleanup & Polish](./04-phase4-cleanup.md) | Dispose, bidirectional sync, keyboard redirect | **Medium** |
| 5 | [Phase 5 — Hierarchical List](./05-phase5-hierarchy.md) | Flatten Children tree, indent, expand/collapse chevron, ARIA tree role | **High** |
| 6 | [Phase 6 — Modern UX Polish](./06-phase6-modern-ux.md) | Tooltips, badges, separators, disabled opacity, focus ring, trailing meta, pinned items, empty-search state | **Medium** |
| 7 | [Phase 7 — Rich List Types](./07-phase7-rich-list-types.md) | ChatList, ContactList, ThreeLineList, NotificationList, ProfileCard painters using SimpleItem SubText/Description/ImagePath | **High** |

## Key Design Decisions

- **BeepTextBox over raw TextBox**: Uses existing themed control, gets consistent look, avoids Win32 cue banner API hacks
- **Fixed header search box**: Pinned at top (doesn't scroll with items) — matches Material Design / VS Code / Figma patterns
- **Painters still reserve layout space**: `SupportsSearch()` gates the Y offset in layout, but painting of search UI is handled by the real control
- **ListBoxTokens as single source of truth**: All spacing/sizing references go through tokens — no more magic numbers
- **Hierarchy via flattening**: Children are recursively flattened into the visible list with depth metadata; indentation computed as `depth × indentStep`. This avoids a separate TreeView component and keeps all 34 painters working.
- **Material-3 / Figma alignment**: Item row composition follows the MD3 "Leading + Content + Trailing" three-zone model:
  - **Leading zone**: checkbox → icon/avatar (left-aligned, fixed width)
  - **Content zone**: title + sub-text (flex, vertically centered)
  - **Trailing zone**: trailing metadata / badge / shortcut text / chevron (right-aligned, fixed width)

## Files Affected

| File | Phase(s) | Description |
|------|----------|-------------|
| `BeepListBox.Core.cs` | 1, 3, 4, 5 | Add BeepTextBox field/init, fix UpdateLayout, fix scroll clamping, dispose, hierarchy mode |
| `BeepListBox.Properties.cs` | 1, 4, 5, 6 | Wire ShowSearch/SearchText to BeepTextBox, add ShowHierarchy, add ShowTooltips |
| `BeepListBox.Events.cs` | 3, 5 | Fix OnMouseWheel, handle expand/collapse click |
| `BeepListBox.Keyboard.cs` | 4, 5 | Redirect type-ahead, ← / → for collapse/expand |
| `Helpers/BeepListBoxHelper.cs` | 5 | Add `FlattenHierarchy()`, modify `GetVisibleItems()` |
| `Helpers/BeepListBoxLayoutHelper.cs` | 2, 3, 5 | Replace hardcoded spacing, add indent calculation |
| `Models/ListItemInfo.cs` | 5 | Add `Depth` and `HasChildren` fields |
| `Painters/BaseListBoxPainter.cs` | 1, 2, 5, 6, 7 | Remove painted search, fix image sizes, add chevron, badges, separators, disabled, trailing meta, shared avatar helpers |
| `Tokens/ListBoxTokens.cs` | 5, 6, 7 | Add hierarchy tokens, tooltip tokens, rich list type tokens |
| `Painters/ChatListBoxPainter.cs` | 7 | **Create** — 72px row, 52px avatar, name + message + time + badge |
| `Painters/ContactListBoxPainter.cs` | 7 | **Create** — 72px row, 48px avatar, name + role + email |
| `Painters/ThreeLineListBoxPainter.cs` | 7 | **Create** — 88px row, 48px thumb, title + 3 sub-text lines |
| `Painters/NotificationListBoxPainter.cs` | 7 | **Create** — 80px row, 40px icon, title + description + time + badge |
| `Painters/ProfileCardListBoxPainter.cs` | 7 | **Create** — 120px row, 64px centered avatar, name + title + bio |

## Modern UI/UX Standards Applied

### Material Design 3 Compliance
- **Three-zone row layout**: Leading (56dp) → Content (flex) → Trailing (48dp)
- **Density tokens**: Comfortable (52dp), Compact (40dp), Dense (28dp)
- **Touch target minimum**: 44px (WCAG 2.5.5)
- **Focus indicators**: 2px ring with offset, visible on keyboard navigation
- **State layers**: hover 8% opacity, pressed 12%, selected 12%, dragged 16%

### Figma Auto-Layout Alignment
- **Consistent spacing via tokens**: all gaps use token constants, no magic numbers
- **Vertical center alignment** within rows for all configurations
- **Responsive padding**: horizontal/vertical padding scales with DPI

### Accessibility (WCAG 2.2 AA)
- `role="listbox"` / `role="tree"` depending on hierarchy mode
- `aria-expanded` on parent items in hierarchy
- Focus ring visible on keyboard navigation
- Disabled items excluded from tab order and shown at reduced opacity
- Minimum 4.5:1 contrast ratio for text (inherited from theme)

## Verification Checklist

### Phases 1–4 (Core Fixes)
- [ ] `ShowSearch = true` → real BeepTextBox appears at top, typing filters items live
- [ ] Switch themes → search box colors/fonts update correctly
- [ ] Mixed items (text-only, +icon, +checkbox, +subtext) → all elements vertically centered, consistent spacing
- [ ] Switch Comfortable/Compact/Dense → row heights change, items remain centered
- [ ] 50+ items → scrollbar appears, mouse wheel smooth, PgUp/PgDn works
- [ ] With search visible → no blank over-scroll at bottom
- [ ] Scroll to bottom, then filter → no blank space (offset resets)
- [ ] With `ShowSearch=true`, press letter key → focus jumps to search box

### Phase 5 (Hierarchy)
- [ ] Items with Children → expand/collapse chevron visible on leading edge
- [ ] Click chevron → children toggled, `IsExpanded` updated
- [ ] Nested 3+ levels deep → consistent indentation per level
- [ ] Keyboard → / ← keys expand/collapse current node
- [ ] Search filters hierarchy — matching children make parents visible
- [ ] `ShowGroups=true` and `ShowHierarchy=true` coexist without crash

### Phase 6 (Modern UX)
- [ ] `ToolTip` set on item → tooltip appears on hover after delay
- [ ] `BadgeText` set → colored pill rendered in trailing zone
- [ ] `IsSeparator = true` → horizontal rule drawn, item not selectable
- [ ] `IsDisabled = true` → item at reduced opacity, not selectable
- [ ] `TrailingMeta` set → right-aligned text in trailing zone
- [ ] `ShortcutText` set → rendered right-aligned (dimmed)
- [ ] Focus ring visible on keyboard nav, hidden on mouse click
- [ ] Search returning 0 results → "No matches found" empty state shown
- [ ] `IsPinned = true` → item stays at top of list

### Phase 7 (Rich List Types)
- [ ] `ChatList` → 72px rows, circular avatar, bold name, message preview, time, unread badge
- [ ] `ContactList` → 72px rows, avatar, name + role + email on 3 lines
- [ ] `ThreeLineList` → 88px rows, rounded thumbnail, title + 3 sub-text lines
- [ ] `NotificationList` → 80px rows, icon, title + description body, time + badge
- [ ] `ProfileCard` → 120px rows, large centered avatar, centered name + title + bio
- [ ] All 5 types: DPI scaling at 100%, 125%, 150%
- [ ] All 5 types: theme change updates colors
- [ ] Missing `ImagePath` → initials fallback renders correctly
- [ ] Empty sub-text lines collapse gracefully (no blank gaps)
- [ ] Existing 34 painters unaffected
