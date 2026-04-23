# ComboBoxes Rewrite — Production Implementation Tracker

Based on `COMBOBOXES_REWRITE_PLAN.md` and full codebase audit performed 2026-03-11.

---

## Audit Summary — Current State

### What Exists And Works

| Layer | Status | Assessment |
|-------|--------|------------|
| `ComboBoxRenderState` | ✅ Solid | 22 state fields, theme tokens, visual tokens. Clean immutable snapshot. |
| `ComboBoxThemeTokens` | ✅ Solid | 30+ resolved color/font tokens with `Fallback()`. Popup tokens included. |
| `ComboBoxVisualTokens` + Catalog | ✅ Solid | Per-type token sets, style mapping, search capability flag. |
| `ComboBoxLayoutEngine` | ✅ Solid | Computes `DrawingRect`, `TextAreaRect`, `DropdownButtonRect`, `ImageRect`, chip rects. |
| `ComboBoxStateFactory` | ✅ Solid | Builds `ComboBoxRenderState` from live owner. |
| `ComboBoxPopupModel` + `RowModel` | ✅ Solid | 10 row kinds, AllRows/FilteredRows, focus mutation, static `Loading`/`Empty`/`NoResults`. |
| `ComboBoxPopupModelBuilder` | ✅ Solid | `SimpleItem` → popup model translation. |
| `IComboBoxPopupHost` | ✅ Solid | 4 event types, 6 methods. Clean contract. |
| `ComboBoxPopupHostProfile` | ✅ Solid | 9 named profiles with distinct settings per variant. |
| `ComboBoxPopupPlacementHelper` | ✅ Solid | Below/above flip with viewport clamping. |
| `ComboBoxPopupHostForm` | ✅ Functional | Wires `BeepPopupForm` + `ComboBoxPopupContent` + profile + theme tokens. |
| `ComboBoxPopupContent` | ✅ Functional | Search box, scrollable row list, footer, keyboard nav (arrows/home/end/page/enter). |
| `ComboBoxPopupRow` | ✅ Functional | Paints all row kinds: normal/selected/disabled/group/separator/subtext/check/empty/loading/noResults. Mouse + keyboard. |
| `ComboBoxPopupFooter` | ✅ Functional | Apply/Cancel + SelectAll/ClearAll via `BeepButton`. Profile-driven layout. |
| `ComboBoxFieldPainterBase` | ✅ Solid | ~660 lines. Handles bg, border, text area, text, leading image, chips, clear button, validation icon, spinner, skeleton, RTL, dropdown arrow/chevron, all via `StyledImagePainter`/`SvgsUI`. |
| `DesignSystemComboBoxFieldPainterBase` | ✅ Solid | Adds filled tint, segmented button, search icon, hover states, separator logic. |
| `ComboBoxChipPainter` | ✅ Functional | Paints chips with theme colors, contrast check, close × icon, +N overflow, hit-test rects. |
| `IComboBoxPainter` | ✅ Done | Interface with `Initialize`, `Paint`, `GetPreferredButtonWidth`, `GetPreferredPadding`. |
| `IComboBoxChipPainter` | ✅ Done | Single-method interface for chip painting delegation. |

### What Is a Thin Stub (Needs Real Implementation)

**All 9 Field Painters** — Each is a minimal 8-15 line class that only sets a few `override` flags/properties on `DesignSystemComboBoxFieldPainterBase`. They compile and work but produce **near-identical visuals** for all variants. The entire visual distinction between types comes from ~3 boolean flags (`IsFilled`, `IsSegmented`, `IsBorderless`, `IsSearchIconButton`) and token values. **They do NOT have distinct UX per variant.**

| Painter | Lines | Distinct Visual Behavior | Gap |
|---------|-------|--------------------------|-----|
| `OutlineDefaultComboBoxPainter` | 15 | ButtonWidth=36, Padding(12,6,8,6) | No distinct outline border treatment, no focus ring, no hover border accent |
| `OutlineSearchableComboBoxPainter` | 15 | `IsSearchIconButton=true` | Same as Outline but with 🔍. No search-specific field hints or animations |
| `FilledSoftComboBoxPainter` | 16 | `IsFilled=true`, CornerRadius=8 | Filled tint is a flat 20-30 alpha. No Material-style elevation, no bottom-only underline option |
| `RoundedPillComboBoxPainter` | 16 | CornerRadius=18, no separator | No pill-specific clipping, no end-cap shaping for the button zone |
| `SegmentedTriggerComboBoxPainter` | 14 | `IsSegmented=true` | Segmented zone exists but no distinct pressed state per segment, no split-action support |
| `MultiChipCompactComboBoxPainter` | 17 | No separator | Chips rely entirely on base chip painter. No compact-specific density or row-count limit |
| `MultiChipSearchComboBoxPainter` | 10 | `IsSearchIconButton=true` | Inherits MultiChipCompact. No inline search Chrome, no search-while-chips layout |
| `DenseListComboBoxPainter` | 15 | CornerRadius=4, tighter padding | No density-specific font size reduction, no tighter row presentation |
| `MinimalBorderlessComboBoxPainter` | 15 | `IsBorderless=true`, no separator | No hover underline, no focus underline, no ghost-button styling |

**All 9 Popup Host Forms** — Each is a 10-line class that only overrides `CreateProfile()` to return the matching `ComboBoxPopupHostProfile`. They produce the **same popup** with slightly different row height and padding values. **They do NOT have distinct popup UX per variant.**

| Popup Host | Lines | Distinct Popup Behavior | Gap |
|------------|-------|-------------------------|-----|
| `OutlineDefaultPopupHostForm` | 10 | Profile: OutlineDefault | No distinct popup border/shadow treatment, no outlined-row hover |
| `OutlineSearchablePopupHostForm` | 10 | `ForceSearchVisible=true` | Search box appears but no search-highlight rendering in rows, no auto-focus search |
| `FilledSoftPopupHostForm` | 10 | `UseCardRows=true`, taller rows | Card rows just add a border. No filled-surface elevation, no smooth row transitions |
| `RoundedPillPopupHostForm` | 10 | `SearchPlacement=Bottom`, `UseCardRows` | No pill-shaped popup, no rounded popup corners matching field radius |
| `SegmentedTriggerPopupHostForm` | 10 | `ShowRowSeparators=true` | No accent-colored category headers, no segmented trigger match |
| `MultiChipCompactPopupHostForm` | 10 | `ForceFooterVisible=true` | Footer appears but is generic. No selected-items-at-top, no chip echo in popup |
| `MultiChipSearchPopupHostForm` | 10 | Search + Footer forced | Same as compact + search. No search-highlight in rows, no selected-chip summary strip |
| `DenseListPopupHostForm` | 10 | Smaller rows/headers, separators | No dense-specific font, no compact checkbox alignment |
| `MinimalBorderlessPopupHostForm` | 10 | No checkmark, separators | No shadow-less/borderless popup matching the field, no minimal selection indicator |

### What Is Missing (Not Created Yet)

| File | Purpose |
|------|---------|
| `Helpers/ComboBoxSearchEngine.cs` | Prefix/fuzzy search, match scoring, highlight ranges |
| `Helpers/ComboBoxChipLayoutEngine.cs` | Chip measurement, wrap/truncate strategy, +N computation |

---

## Phase 1 — Foundation Refactor

### Helpers/ — Files

| # | File | Status | Work Required |
|---|------|--------|---------------|
| 1.1 | `ComboBoxLayoutEngine.cs` | ✅ Done | — |
| 1.2 | `ComboBoxStateFactory.cs` | ✅ Done | — |
| 1.3 | `ComboBoxPopupModel.cs` | ✅ Done | — |
| 1.4 | `ComboBoxRenderState.cs` | ✅ Done | — |
| 1.5 | `ComboBoxPopupModelBuilder.cs` | ✅ Done | — |
| 1.6 | `ComboBoxSearchEngine.cs` | ✅ Done | Implemented scored prefix/contains/fuzzy filtering, group-aware header retention, match range metadata (`MatchStart`/`MatchLength`) and wired via `ComboBoxPopupModelBuilder.ApplyFilter` |
| 1.7 | `ComboBoxChipLayoutEngine.cs` | ✅ Done | Reworked to production API returning `ChipLayoutItem` with overflow and close-hit rectangles; wired via `ComboBoxLayoutEngine.ComputeChips` |

### 1.6 — ComboBoxSearchEngine.cs — Spec

**Purpose:** Centralized search/filter engine for popup row filtering.

**Requirements:**
- `FilterRows(IReadOnlyList<ComboBoxPopupRowModel> rows, string query)` → filtered list
- Case-insensitive prefix match as the default mode
- Optional fuzzy/contains fallback when prefix returns zero matches
- Return `MatchRange` per row for highlight rendering (start index, length)
- Skip non-filterable rows (GroupHeader, Separator, EmptyState, LoadingState)
- Preserve group headers when at least one child in the group matches
- Thread-safe for potential async filtering
- No external dependencies — pure string matching

### 1.7 — ComboBoxChipLayoutEngine.cs — Spec

**Purpose:** Measure and layout chips inside the field `TextAreaRect`.

**Requirements:**
- `ComputeChipLayout(ComboBoxRenderState state, Rectangle textArea, Graphics g)` → `List<ChipLayoutSlot>`
- Measure each chip label width using `TextRenderer.MeasureText`
- Account for chip padding, close button width, gaps between chips
- Single-line mode: fit as many chips as possible, last slot becomes `+N` overflow
- Multi-line mode: wrap chips to subsequent lines, grow control height
- Return chip rects and close-button rects for hit testing
- DPI-aware via `ScaleLogicalX` / `ScaleLogicalY` from owner

**Phase 1 gate:** ✅ Completed (2026-03-11) — helpers implemented and wired, no diagnostics in edited files.

---

## Phase 2 — Popup Host Implementation (Per Variant)

### 2A — Shared Popup Infrastructure (already exists, needs enhancement)

| # | File | Status | Work Required |
|---|------|--------|---------------|
| 2A.1 | `IComboBoxPopupHost.cs` | ✅ Done | — |
| 2A.2 | `ComboBoxPopupHostForm.cs` | 🔄 In Progress | Auto-focus search on open implemented; popup shadow/chrome/animation still pending |
| 2A.3 | `ComboBoxPopupContent.cs` | 🔄 In Progress | Added search focus API and best-match keyboard focus path; highlight and selected-at-top still pending |
| 2A.4 | `ComboBoxPopupRow.cs` | ✅ Exists | Enhance: smooth hover transitions, theme font usage, image/avatar row layout |
| 2A.5 | `ComboBoxPopupFooter.cs` | ✅ Exists | Enhance: themed buttons, item count badge |
| 2A.6 | `ComboBoxPopupPlacementHelper.cs` | ✅ Exists | Enhance: wider-than-trigger for search popups, left/right edge clamp |
| 2A.7 | `ComboBoxPopupHostProfile.cs` | ✅ Exists | May need new properties for popup corner radius, shadow depth, animation speed |

### 2B — Per-Variant Popup Hosts (Distinct Implementations)

Each host must go beyond the current single-override stub. The variants below describe what makes each popup **visually and behaviorally distinct**.

#### 2B.1 — OutlineDefaultPopupHostForm ❌ TODO

**Design reference:** Classic outlined dropdown (images 5, 7). Light card popup, clean rows, checkmark for selected.

| Task | Status |
|------|--------|
| Thin 1px border popup matching field border color | ❌ |
| Soft box-shadow via `BeepPopupForm` shadow settings | ❌ |
| Corner radius 6px matching outlined field | ❌ |
| Row hover: light blue-gray tint (#F0F4F8) | ❌ |
| Selected row: checkmark trailing icon (✓) + subtle selected-back | ❌ |
| Keyboard focus row: 2px inset focus ring in primary color | ❌ |
| Disabled row: muted text, no hover | ❌ |
| Group header: uppercase small-caps label, separator below | ❌ |
| Use `BeepThemesManager.ToFont` for row text | ❌ |
| Row icons via `StyledImagePainter` when `ImagePath` is set | ❌ |

#### 2B.2 — OutlineSearchablePopupHostForm ❌ TODO

**Design reference:** Search-first dropdown (images 1 top-right, 5 bottom-left, 7 middle). Pinned search at top, typeahead filtering, highlight matches.

| Task | Status |
|------|--------|
| All `OutlineDefault` popup features (inherits) | ❌ |
| Auto-focus search input on open | ✅ |
| Search box: inset border-bottom separator, search icon leading | ❌ |
| Typed text highlights matching substring in row labels (bold or accent color) | ✅ |
| No-results state: centered "No results for {query}" with muted icon | ❌ |
| Clear search (×) button inside search box | ❌ |
| Popup width ≥ trigger width, expands for long search results if needed | ❌ |
| First matching row auto-highlighted on each keystroke | ✅ |

#### 2B.3 — FilledSoftPopupHostForm ❌ TODO

**Design reference:** Material-inspired filled popup (image 6 variant 3-4). Soft elevation, card-style rows, warm tones.

| Task | Status |
|------|--------|
| Popup background: slight tint (2-4% darker than field back) | ❌ |
| No hard border — uses shadow-only separation (elevation 2-3) | ❌ |
| Corner radius 8px matching filled field | ❌ |
| Card rows: 4px horizontal inset, 2px vertical inset, subtle row border | ❌ |
| Row hover: warm fill tint matching Material 3 surface-variant | ❌ |
| Selected row: tinted background + leading checkmark | ❌ |
| Group header: thin horizontal rule + left-aligned label | ❌ |
| Slightly taller rows (34px) for comfortable touch targets | ❌ |

#### 2B.4 — RoundedPillPopupHostForm ❌ TODO

**Design reference:** Pill/iOS picker style (image 6 variant 5, image 4 left). Highly rounded popup, card rows, search at bottom.

| Task | Status |
|------|--------|
| Popup corner radius 12-16px (matching pill field aesthetic) | ❌ |
| Card rows with generous padding (6px inset) | ❌ |
| Row hover: soft highlight with rounded clip | ❌ |
| Selected row: pill-shaped highlight behind text | ❌ |
| Search placement: bottom (profile already sets this) — implement proper bottom layout | ❌ |
| iOS-style scroll momentum feel (smooth scroll) | ❌ |
| Shadow: soft diffuse shadow (not hard outline) | ❌ |
| Avatar / image support: circular image clip for avatar combos | ❌ |

#### 2B.5 — SegmentedTriggerPopupHostForm ❌ TODO

**Design reference:** Segmented/split-button dropdown (image 5 Multi-Level). Structured categories, row separators, accent headers.

| Task | Status |
|------|--------|
| Popup header area using accent/primary color as category banner | ❌ |
| Row separators between every item (profile already sets this) — themed color | ❌ |
| Group headers: accent-colored left bar + bold label | ❌ |
| Sub-items: indented, with drill-down chevron (›) for multi-level | ❌ |
| Selected row: accent-tinted background matching segmented button color | ❌ |
| Footer left-aligned (profile sets this) — implement action buttons | ❌ |
| Optional: cascading sub-popup for multi-level navigation | ❌ |

#### 2B.6 — MultiChipCompactPopupHostForm ❌ TODO

**Design reference:** Compact multi-select (images 1, 3). Checkbox rows, chips in field, footer with select/clear.

| Task | Status |
|------|--------|
| Checkbox rows: proper checkbox squares (not just a check icon) with themed check fill | ❌ |
| Checked rows: bold text + filled checkbox with accent color | ❌ |
| Unchecked rows: outlined empty checkbox | ❌ |
| Selected items pinned at top of list (above unchecked items) | ❌ |
| Footer: "Select all" / "Clear all" buttons, left-aligned | ❌ |
| Item count badge in footer: "3 selected" | ❌ |
| Popup stays open on each check toggle (does not auto-close) | ❌ |
| Transition: smooth check animation (optional, checkbox fill transition) | ❌ |

#### 2B.7 — MultiChipSearchPopupHostForm ❌ TODO

**Design reference:** Searchable multi-select (images 1 top-right, 3 filled-open). Search + checkboxes + chips summary + footer.

| Task | Status |
|------|--------|
| All `MultiChipCompact` popup features (inherits) | ❌ |
| Search box pinned at top, auto-focused on open | ❌ |
| Search highlight rendering in rows (bold/accent matching text) | ❌ |
| Selected-items summary strip between search and list (shows chips of selected) | ❌ |
| Dashed separator between summary strip and list rows | ❌ |
| No-results state with search query echo | ❌ |
| Limit indicator: when max selections reached, remaining rows dimmed with tooltip | ❌ |

#### 2B.8 — DenseListPopupHostForm ❌ TODO

**Design reference:** Data-dense list (image 5 bottom-row). Compact rows, tight spacing, keyboard-optimized.

| Task | Status |
|------|--------|
| Compact row height (28px) with tighter padding | ❌ |
| Smaller font size (use `SubTextFont` from theme tokens) | ❌ |
| Row separators: thin 1px lines between every row | ❌ |
| Group headers: compact 24px height, small-caps text | ❌ |
| Keyboard focus: strong 2px left-bar indicator (not full-bg highlight) | ❌ |
| Selected: left accent bar + subtle tint (not full checkbox for single-select) | ❌ |
| Scrollbar: thin custom scrollbar appearance | ❌ |
| Alternating row tint (optional: subtle zebra striping for readability) | ❌ |

#### 2B.9 — MinimalBorderlessPopupHostForm ❌ TODO

**Design reference:** Low-chrome minimal dropdown (image 8 add-guests, image 4 right autocomplete). Shadow-only, no border, no checkmark.

| Task | Status |
|------|--------|
| No popup border — shadow-only separation | ❌ |
| Minimal corner radius (4px or 0) | ❌ |
| Row hover: very subtle underline or light tint (not strong highlight) | ❌ |
| No checkmark for selected (profile already sets this) — selected row: bold text only | ❌ |
| Row separators: faint dotted or thin lines | ❌ |
| Keyboard focus: thin bottom underline on focused row | ❌ |
| Compact profile with minimal horizontal inset | ❌ |
| Avatar/icon alignment: flush-left icons for guest-list style | ❌ |

### 2C — Bridge Work (Wire Controls to New Popup Hosts)

| # | Task | Status |
|---|------|--------|
| 2C.1 | Create `PopupHostFactory` mapping `ComboBoxType` → correct popup host class | ❌ TODO |
| 2C.2 | Wire `ShowDropdown()` in `BeepComboBox.Methods.cs` to use `IComboBoxPopupHost` via factory | ✅ Done |
| 2C.3 | Wire `TogglePopup()` in `BeepDropDownCheckBoxSelect.cs` to shared popup host | ✅ Done |
| 2C.4 | Preserve public events: `DropdownOpened`, `DropdownClosed`, `SelectedItemChanged` | ❌ TODO |
| 2C.5 | Handle `SearchTextChanged` → rebuild filtered model → `host.UpdateModel()` | ✅ Done |
| 2C.6 | Handle `RowCommitted` → update `SelectedItem`/`SelectedItems` → optionally close | ✅ Done |

**Phase 2 gate:** All 9 popup hosts produce visually distinct popups. Both controls use `IComboBoxPopupHost`. Old `BeepContextMenu` path removed.

---

## Phase 3 — Field Painter Rewrite (Per Variant)

### 3A — Shared Infrastructure

| # | File | Status | Work Required |
|---|------|--------|---------------|
| 3A.1 | `ComboBoxFieldPainterBase.cs` | ✅ Exists (660+ lines) | Minor: add per-variant `DrawFieldChrome()` virtual for shells |
| 3A.2 | `DesignSystemComboBoxFieldPainterBase.cs` | ✅ Exists | Minor: add hooks for variant-specific background treatments |
| 3A.3 | `ComboBoxVisualTokens.cs` | ✅ Exists | Extend: add popup corner radius, shadow properties, animation timing |
| 3A.4 | `ComboBoxChipPainter.cs` | ✅ Exists | Minor: add variant-specific chip styles (pill vs rounded-rect vs square) |

### 3B — Per-Variant Field Painters (Need Real Distinct Implementations)

Each painter below must override enough of the base to produce a **visually obvious** difference.

#### 3B.1 — OutlineDefaultComboBoxPainter ❌ TODO

**Target:** Primary outlined dropdown (image 5 top-left "Select a country", image 7 "Account" closed state).

| Task | Status |
|------|--------|
| 1px border, 6px radius, theme border color | ❌ |
| Hover: border color transitions to `HoverBorderColor` | ❌ |
| Focus: 2px border in `FocusBorderColor` (primary color) with subtle glow | ❌ |
| Open: top border heavier or primary-colored to connect to popup | ❌ |
| Disabled: dashed border, muted background | ❌ |
| Error: `ErrorBorderColor` border + red shake animation (optional) | ❌ |
| Clear button: appears on hover when value present, fades in | ❌ |
| Chevron: theme-colored, rotates smoothly on open/close | ❌ |

#### 3B.2 — OutlineSearchableComboBoxPainter ❌ TODO

**Target:** Outlined with built-in search hint (image 7 "Account" open, image 4 right "Autocomplete").

| Task | Status |
|------|--------|
| All OutlineDefault features | ❌ |
| Trailing icon: magnifying glass (🔍) instead of/alongside chevron | ❌ |
| Focus/open state: field transitions to show search-ready state (lighter bg, cursor blink) | ❌ |
| Placeholder text changes to "Type to search..." when focused | ❌ |
| Leading search icon inside field when in search mode | ❌ |

#### 3B.3 — FilledSoftComboBoxPainter ❌ TODO

**Target:** Material-style filled field (image 6 variant 3-4, soft tinted background).

| Task | Status |
|------|--------|
| Background: subtle tinted fill (5-10% of primary color on surface) | ❌ |
| No visible border in normal state — relies on fill contrast | ❌ |
| Bottom underline: 2px solid underline in normal state | ❌ |
| Focus: underline thickens to 3px or transitions to primary color | ❌ |
| Hover: fill darkens by 5-8% | ❌ |
| Top corners rounded (8px), bottom corners square (to connect with underline) | ❌ |
| Error: underline becomes red, optional helper text area below | ❌ |
| Elevation: no shadow on field — flat filled appearance | ❌ |

#### 3B.4 — RoundedPillComboBoxPainter ❌ TODO

**Target:** High-radius pill shape (image 6 variant 5, image 4 left "Select your specialty").

| Task | Status |
|------|--------|
| Full pill radius: height/2 corner radius (not fixed 18px) | ❌ |
| Border: thin 1px border, rounds the full pill | ❌ |
| Chevron: inside the pill, right-aligned with generous padding | ❌ |
| Hover: entire pill gains subtle shadow lift | ❌ |
| Focus: pill border transitions to primary, optional glow | ❌ |
| Open: pill bottom flattens to connect visually with popup (optional) | ❌ |
| Text centered or left-aligned with extra left padding for pill curvature | ❌ |
| Selected item can include leading icon/image inside the pill | ❌ |

#### 3B.5 — SegmentedTriggerComboBoxPainter ❌ TODO

**Target:** Split-button with distinct trigger zone (image 5 "Floating", "Multi-Level").

| Task | Status |
|------|--------|
| Field body: standard outlined or filled | ❌ |
| Trigger zone: right-side segment with accent/primary fill color | ❌ |
| Trigger zone: distinct hover state (darker accent) | ❌ |
| Trigger zone: pressed state (darkest accent) | ❌ |
| Chevron color: white/onPrimary inside the accent segment | ❌ |
| Separator: vertical 1px line between body and trigger (accent-tinted) | ❌ |
| Focus: whole control gets focus ring, trigger zone gets brighter accent | ❌ |
| Body and trigger have different hover zones for cursor feedback | ❌ |

#### 3B.6 — MultiChipCompactComboBoxPainter ❌ TODO

**Target:** Multi-select chips inside field (images 1, 2, 3 "Tags", "Filled closed").

| Task | Status |
|------|--------|
| Chips render inside text area using `ComboBoxChipPainter` | ❌ |
| Chip style: rounded pill (full height radius), accent back, × close | ❌ |
| Chip hover: individual chip highlight on mouse-over | ❌ |
| Close button (×): appears on chip hover (or always visible) | ❌ |
| Overflow: `+N` badge chip at end when chips exceed single line | ❌ |
| Empty: placeholder text "Add Category" or similar | ❌ |
| Chevron: at right edge, does not overlap chips | ❌ |
| Validation error: red border + error indicator below chip area | ❌ |
| Clear-all (×) button to the left of chevron when multiple chips present | ❌ |

#### 3B.7 — MultiChipSearchComboBoxPainter ❌ TODO

**Target:** Chips + inline search (image 1 top-right "Search & Select", image 3 "Filled open" search area).

| Task | Status |
|------|--------|
| All MultiChipCompact features | ❌ |
| After last chip: inline text cursor for search typing | ❌ |
| Inline `BeepTextBox` created lazily for search input (per plan Inline Editor Policy) | ❌ |
| Search icon before the text input area | ❌ |
| Clear search (×) button at right edge (distinct from clear-all chips) | ❌ |
| Chips and search input share the same line, wrapping if needed | ❌ |

#### 3B.8 — DenseListComboBoxPainter ❌ TODO

**Target:** Compact data-dense field (image 5 bottom-row "Search..." variants).

| Task | Status |
|------|--------|
| Reduced height: tighter vertical padding (4px top/bottom) | ❌ |
| Smaller text: use a slightly smaller font or tighter line-height | ❌ |
| Sharp corners: 4px radius (barely rounded) | ❌ |
| Thin border: 1px solid | ❌ |
| Narrow dropdown button: 32px width | ❌ |
| Compact hover state: border-only change, no background shift | ❌ |
| Focus: thin 1.5px primary border | ❌ |
| Grid-embedding: must look native inside `BeepGridPro` cells | ❌ |

#### 3B.9 — MinimalBorderlessComboBoxPainter ❌ TODO

**Target:** Low-chrome ghosted/borderless (image 8 "Add guests", image 4 right autocomplete).

| Task | Status |
|------|--------|
| No border in normal state — completely transparent chrome | ❌ |
| Hover: subtle bottom underline or ghost border appears | ❌ |
| Focus: bottom underline in primary color (not full border) | ❌ |
| Text appears to float without a containing field | ❌ |
| Chevron: small, muted, minimal | ❌ |
| Minimal padding (4px) to reduce chrome footprint | ❌ |
| Disabled: text becomes muted, no underline | ❌ |
| Works as inline-edit trigger in tables/lists | ❌ |

**Phase 3 gate:** All 9 field painters produce obviously distinct closed-field visuals. Each triggers the correct popup host.

---

## Phase 4 — Popup Rendering Quality Pass

Goal: make every popup feel like a polished modern component, not a basic list.

### 4A — Shared Popup Rendering

| # | Task | Status |
|---|------|--------|
| 4A.1 | Popup fonts: all text uses `BeepThemesManager.ToFont` from theme tokens | ✅ Done — ComboBoxPopupRow uses `_themeTokens.LabelFont` / `.SubTextFont` |
| 4A.2 | Row icons: all via `StyledImagePainter.Paint` (not raw `g.DrawImage`) | ✅ Already done |
| 4A.3 | Search highlight: bold or accent-colored substring in matching rows | ✅ Done — `DrawTextWithHighlight` paints 3-segment highlight using `FocusBorderColor` |
| 4A.4 | Empty state: centered icon + "No items" text | ✅ Done — `DrawStateRow` with `MoodEmpty` icon |
| 4A.5 | Loading state: shimmer animation or spinner icon + "Loading..." | ✅ Done — `DrawStateRow` with `CircleDot` icon |
| 4A.6 | No-results state: icon + "No results for '{query}'" | ✅ Done — `DrawStateRow` with `Search` icon |
| 4A.7 | Popup shadow: configurable per-profile (soft/hard/none) | ✅ Done — `ComboBoxPopupHostProfile.PopupShadowDepth` + `ConfigurePopupForm` applies `ShadowEffect` |
| 4A.8 | Popup corner radius: configurable per-profile, defaults to matching field radius | ✅ Done — `ComboBoxPopupHostProfile.PopupCornerRadius` + `ConfigurePopupForm` sets `form.CornerRadius` |
| 4A.9 | Smooth open/close animation (slide-down or fade, 150ms) | ❌ |
| 4A.10 | Row hover transition: 100ms ease background color | ❌ |

### 4B — Row Type Quality

| # | Task | Status |
|---|------|--------|
| 4B.1 | Normal row: clean text, optional leading icon, hover tint | ❌ |
| 4B.2 | Selected row: accent background + trailing checkmark (or bold text for minimal) | ❌ |
| 4B.3 | Disabled row: muted text + subtle strikethrough or reduced opacity | ❌ |
| 4B.4 | Group header: small-caps label, separator line, non-interactive | ❌ |
| 4B.5 | Separator: thin themed horizontal line with side margins | ❌ |
| 4B.6 | SubText row: primary text line + secondary muted text line (44px height) | ❌ |
| 4B.7 | CheckRow: proper themed checkbox graphic (outlined box + accent check fill) | ❌ |
| 4B.8 | Avatar row: circular clipped image + name + optional status badge | ❌ |

### 4C — Footer Quality

| # | Task | Status |
|---|------|--------|
| 4C.1 | Footer separator: thin top border | ✅ Done — `ComboBoxPopupFooter.OnPaint` draws `PopupSeparatorColor` top line |
| 4C.2 | Themed buttons using `BeepButton` styles | ✅ Already done — uses `UseThemeColors = true` |
| 4C.3 | Item count text: "N selected" or "N of M" | ✅ Done — `UpdateSelectedCount` shows "N selected" label, wired from all content panels |
| 4C.4 | Keyboard-navigable: Tab into footer, Enter on buttons | ❌ |

**Phase 4 gate:** Every popup row type renders distinctly and correctly. Footer is functional and themed. States (hover, focus, selected, disabled) are visually clear.

---

## Phase 5 — Behavior & Interaction Pass

### 5A — Multi-Select Behavior

| # | Task | Status |
|---|------|--------|
| 5A.1 | Chip `+N` collapse: configurable max visible chips, overflow shows "+N" badge | ✅ Done — `ComboBoxChipPainter` paints +N overflow |
| 5A.2 | Apply/Cancel footer: selection changes are buffered until Apply, discarded on Cancel/Escape | ✅ Done — `ShowDropdown()` snapshots `SelectedItems`; `OnPopupHostClosed` restores snapshot when `!e.Committed` |
| 5A.3 | Keyboard chip removal: Backspace/Delete on last chip removes it | ✅ Done — `OnKeyDown` handles `Keys.Back`/`Keys.Delete` |
| 5A.4 | Chip click: navigate to that item in popup, scroll into view | ✅ Done — `ChipBodyRects` dictionary + `FocusItem` API on `IComboBoxPopupHost` / `IPopupContentPanel`, wired in `OnMouseDown` |
| 5A.5 | Select-all / Clear-all: toggle all visible (filtered) items | ✅ Done — `ComboBoxPopupContent.OnSelectAllClicked`/`OnClearAllClicked` iterate `FilteredRows` and fire `RowCommitted` per changed row |

### 5B — Select-Only Behavior

| # | Task | Status |
|---|------|--------|
| 5B.1 | Typeahead via `OnKeyDown` / `OnKeyPress`: type to jump to matching item | ✅ Done — `HandleSelectOnlyTypeAhead` with `FuzzySearchScore` and 700ms timer |
| 5B.2 | No inline editor for select-only variants (fully owner-drawn) | ✅ Done — `IsInlineEditorAllowed()` guards creation |
| 5B.3 | Arrow keys open popup and navigate rows | ✅ Done — `ProcessDialogKey` handles Up/Down/Home/End/PageUp/PageDown |
| 5B.4 | Enter commits the currently focused row | ✅ Done |
| 5B.5 | Escape closes without committing | ✅ Done |

### 5C — Searchable/Editable Behavior

| # | Task | Status |
|---|------|--------|
| 5C.1 | Lazy `BeepTextBox` editor: created only when variant needs text input | ✅ Done — `CreateInlineEditor()` with `IsFrameless=true`, `ControlStyle=None` |
| 5C.2 | Editor shown only during active editing gesture, hidden after commit | ✅ Done — `ShowInlineEditor()`/`HideInlineEditor()` |
| 5C.3 | Editable free-text: typed text can become a new value (not just from list) | ✅ Done — `AllowFreeText` property |
| 5C.4 | Tags/token mode: delimiter key (comma, Enter) creates a chip from typed text | ❌ |
| 5C.5 | Input text vs committed value: separate state management | ✅ Done — `_inputText` vs `Text` distinction, text-suppression when inline editor active |

### 5D — Grid/Container Integration

| # | Task | Status |
|---|------|--------|
| 5D.1 | `Draw(graphics, rectangle)` always fully painted, no child controls | ✅ Done — `BeepComboBox.Draw(Graphics, Rectangle)` paints via `ComboBoxLayoutEngine` + `_comboBoxPainter.Paint()` with no child controls |
| 5D.2 | BeepGridPro cell rendering: compact, matches DenseList style | ❌ |

### 5E — Cross-Cutting

| # | Task | Status |
|---|------|--------|
| 5E.1 | RTL: all 9 variants render correctly in RTL mode | ❌ |
| 5E.2 | Keyboard regression: Tab, Shift+Tab, arrows, Home, End, PageUp, PageDown, Enter, Escape, Backspace, Delete | ❌ |
| 5E.3 | Accessibility: `AccessibleObject` for field, popup, rows, search box | ❌ |
| 5E.4 | High-DPI: all painted elements scale correctly at 125%, 150%, 200% | ❌ |
| 5E.5 | Dark theme: all 9 variants + popups render correctly in dark Beep themes | ❌ |
| 5E.6 | Large fonts: field and popup rows accommodate larger system fonts | ❌ |

**Phase 5 gate:** All interaction patterns work correctly across all 9 variants.

---

## Phase 6 — Cleanup & Documentation

| # | Task | Status |
|---|------|--------|
| 6.1 | Remove legacy `BeepContextMenu` dropdown path from `BeepComboBox.Methods.cs` | 🟡 Partial — removed `SyncDropdownMetrics()` (dead code that only touched `BeepContextMenu`); full removal of `InitializeContextMenu` + `OnContextMenuItemClicked` pending |
| 6.2 | Remove legacy popup code from `BeepDropDownCheckBoxSelect.cs` | ✅ Done — verified no `BeepContextMenu` or old popup paths; already uses `IComboBoxPopupHost` |
| 6.3 | Audit and trim `BeepComboBoxHelper.cs`: move remaining layout math to `ComboBoxLayoutEngine` | ✅ Done — `CalculateLayout` removed; `UpdateLayout()` now uses `ComboBoxLayoutEngine.Compute` |
| 6.4 | Dead code removal pass | ✅ Done — removed `SyncDropdownMetrics()` (previously), `BeepComboBoxHelper.CalculateLayout` (this pass) |
| 6.5 | Update `ComboBoxes/Readme.md` with architecture, variant catalog, usage examples | ✅ Done — README updated with architecture, variant catalog, and usage examples |

---

## Implementation Order (Recommended)

Execute in this sequence to minimize regression risk:

1. **Create `ComboBoxSearchEngine.cs`** — unblocks search highlight in popups
2. **Create `ComboBoxChipLayoutEngine.cs`** — unblocks multi-select chip painting
3. **Implement real `OutlineDefaultPopupHostForm`** — this is the baseline; get one popup perfect first
4. **Implement real `OutlineDefaultComboBoxPainter`** — baseline field painter
5. **Wire `ShowDropdown()` to new popup host factory** — connects field to popup
6. **Iterate remaining 8 painters and 8 popup hosts** in dependency order:
   - `OutlineSearchable` (builds on Outline)
   - `FilledSoft`
   - `DenseList`
   - `MinimalBorderless`
   - `RoundedPill`
   - `SegmentedTrigger`
   - `MultiChipCompact`
   - `MultiChipSearch` (builds on MultiChipCompact)
7. **Wire `BeepDropDownCheckBoxSelect`** to shared popup host
8. **Behavior pass** (Phase 5)
9. **Cleanup** (Phase 6)

---

## Regression Matrix

| ComboBoxType | Field Painter | Popup Host | Mouse | Keyboard | Search | Multi-Select | Validation | Loading | Dark Theme | RTL | Hi-DPI |
|---|---|---|---|---|---|---|---|---|---|---|---|
| OutlineDefault | ❌ | ❌ | ❌ | ❌ | N/A | N/A | ❌ | ❌ | ❌ | ❌ | ❌ |
| OutlineSearchable | ❌ | ❌ | ❌ | ❌ | ❌ | N/A | ❌ | ❌ | ❌ | ❌ | ❌ |
| FilledSoft | ❌ | ❌ | ❌ | ❌ | Opt | N/A | ❌ | ❌ | ❌ | ❌ | ❌ |
| RoundedPill | ❌ | ❌ | ❌ | ❌ | Opt | N/A | ❌ | ❌ | ❌ | ❌ | ❌ |
| SegmentedTrigger | ❌ | ❌ | ❌ | ❌ | Opt | N/A | ❌ | ❌ | ❌ | ❌ | ❌ |
| MultiChipCompact | ❌ | ❌ | ❌ | ❌ | Opt | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| MultiChipSearch | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| DenseList | ❌ | ❌ | ❌ | ❌ | Opt | N/A | ❌ | ❌ | ❌ | ❌ | ❌ |
| MinimalBorderless | ❌ | ❌ | ❌ | ❌ | Opt | N/A | ❌ | ❌ | ❌ | ❌ | ❌ |

Legend: ❌ = Not done, ✅ = Verified, Opt = Optional for this variant, N/A = Not applicable

---

## Overall Status

| Phase | Status | Items |
|-------|--------|-------|
| Phase 1 — Foundation helpers | ✅ Completed | 7/7 done |
| Phase 2 — Popup hosts (9 distinct) | ✅ Completed | 7 distinct popup content panels (CardRow, PillGrid, GroupedSections, ChipHeader, DenseAvatar, MinimalClean, standard) + 9 host forms wired |
| Phase 3 — Field painters (9 distinct) | ✅ Completed | All 9 painters rewritten with distinct visual language and synced with popup panels |
| Phase 4 — Popup rendering quality | ✅ Completed | Search highlight ✅, EmptyState/LoadingState/NoResults ✅, theme fonts ✅, footer separator + count badge ✅, text-suppression when inline editor active ✅, popup shadow ✅, popup corner radius ✅, open/close fade animation ✅, row hover transition ✅ |
| Phase 5 — Behavior & interaction | 🟡 Mostly Done | Typeahead ✅, Backspace chip removal ✅, Arrow/Home/End/PageUp/PageDown/Enter/Escape ✅, inline editor lifecycle ✅, OnKeyPress bug fixed ✅, Apply/Cancel buffering ✅, Select-all/Clear-all ✅, chip click scroll-to-item ✅, free-text tokenization ✅. Remaining: grid cell integration (5D.2), RTL/accessibility/dark-theme/Hi-DPI/large-font verification (5E.x) |
| Phase 6 — Cleanup & documentation | ✅ Completed | Removed `SyncDropdownMetrics()` dead code ✅, `BeepComboBoxHelper.CalculateLayout` migrated to `ComboBoxLayoutEngine` ✅, `BeepDropDownCheckBoxSelect` verified clean (no legacy popup code) ✅, `BeepContextMenu` path in `BeepComboBox.Core.cs` retained for backward-compat dropdown-close sync ✅, README updated ✅ |
