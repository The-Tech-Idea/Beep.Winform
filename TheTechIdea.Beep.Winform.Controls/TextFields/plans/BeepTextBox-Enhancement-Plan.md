# BeepTextBox Enhancement Plan

Priority: High | Status: Planning Approved | Date: 2026-06-06 | Revised: 2026-06-06

## Executive Summary

BeepTextBox already has great feature breadth (search, terminal effects, masking, autocomplete).
However, it does **not** leverage the extensive infrastructure that `BaseControl` provides:

| BaseControl System | Already Exists? | BeepTextBox Uses It? |
|---|---|---|
| `ShowValidation(state, message)` + `ErrorText` + `ErrorColor` + `ValidationIcon` + indicator line | Yes | **No** |
| `HelperText` / `HelperTextOn` + external drawing pipeline | Yes | **No** |
| `LabelText` / `LabelTextOn` / `FloatingLabelOn` + MD3 label animation | Yes | **No** |
| `IsHovered` (auto-set by `OnMouseEnter/Leave`) + `HoverBackColor`/`HoverForeColor`/`HoverBorderColor` | Yes | **No (no rendering)** |
| `TrailingIconPath` / `LeadingIconPath` + `IconSize`/`IconPadding` + click events | Yes | **No** |
| `ShowClearButton` built-in property | Yes | **No** |
| `DisabledBackColor` / `DisabledBorderColor` / `DisabledForeColor` for read-only state | Yes | **No** |
| `ShowBeepContextMenu()` / `CreateMenuItem()` / `CreateMenuItemWithShortcut()` / `CreateMenuSeparator()` | Yes | **No** |
| `ShowExternalIcon(svg)` / `ClearExternalIcon()` | Yes | **No** |
| `IsRequired` property (from `IBeepUIComponent`) | Yes | **No (no rendering)** |
| `FocusBackColor` / `FocusBorderColor` / `FocusForeColor` | Yes | **Partial** (only FocusBorderColor used in animation) |

**This plan is NOT about building new infrastructure. It is about WIRING BeepTextBox to the infrastructure that BaseControl already provides.**

## Icon Sources

All icons from `TheTechIdea.Beep.Icons.SvgsUIcons` (`IconsManagement\SvgsUIcons.cs`):

| Purpose | Class | Resource |
|---------|-------|----------|
| Search | `Common.Search` | `fi-tr-search-alt.svg` |
| Clear (X) | `Common.Close` | `fi-tr-x.svg` |
| Password show | `Common.Visible` | `fi-tr-eye-recognition.svg` |
| Password hide | `Common.Hidden` | `fi-tr-low-vision.svg` |
| Error icon | `Common.Error` | `fi-tr-circle-xmark.svg` |
| Warning | `Common.Warning` | `fi-tr-triangle-warning.svg` |
| Success | `Common.Success` | `fi-tr-check-circle.svg` |
| Info | `Common.Info` | `fi-tr-info.svg` |
| Required star | `Common.Star` | `fi-tr-star.svg` |
| Read-only lock | `Common.Lock` | `fi-tr-lock-alt.svg` |
| Loading spinner | `Activity.Loading` | `fi-tr-loading.svg` |
| Dropdown caret | `Carets.Down` | `fi-tr-caret-down.svg` |
| Dropdown caret up | `Carets.Up` | `fi-tr-caret-up.svg` |
| Copy | `Common.Copy` | `fi-tr-copy.svg` |
| Paste | `Common.Paste` | `fi-tr-paste.svg` |
| Undo | `Common.Undo` | `fi-tr-undo.svg` |
| Redo | `Common.Redo` | `fi-tr-redo-alt.svg` |
| Delete | `Common.Delete` | `fi-tr-trash-empty.svg` |
| Edit | `Common.Edit` | `fi-tr-pen.svg` |
| Download | `Common.Download` | `fi-tr-file-download.svg` |
| Upload | `Common.Upload` | `fi-tr-file-upload.svg` |

---

## Phase 1: Wire to BaseControl (P0-P1)
Goal: Adopt BaseControl's existing infrastructure. Status: PLANNING

### 1.1 Error/Validation State Integration (1 day)
**What exists in BaseControl:**
- `ShowValidation(ValidationState, string message)` — sets icon, indicator line, error text, raises event
- `ClearValidation()` — clears all indicators
- `ErrorText` property — auto-sets `HasError`
- `ErrorColor` property — from theme
- `ValidationIcon` property — picks SvgsUIcons icon automatically
- `ShowIndicatorLine` — colored bar below control
- `ValidationStateChanged` event
- External drawing pipeline renders label + helper/error text + icon + indicator line

**What BeepTextBox needs to do:**
1. After `ValidateData(out message)` returns false: call `ShowValidation(ValidationState.Error, message)`
2. When text changes and clears error: call `ClearValidation()`
3. In `DrawContent()`: when `HasError`, draw error-colored border using `ErrorColor`
4. In `DrawContent()`: when `ShowIndicatorLine` is true, skip drawing indicator line (BaseControl's external painter handles it)
5. Remove `_beepImage`-based image validation approach in favor of BaseControl's system

**Files:** `BeepTextBox.Methods.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Events.cs`

### 1.2 Helper Text & Label Integration (0.5 day)
**What exists in BaseControl:**
- `HelperText` property — supporting text below control
- `HelperTextOn` property — toggle
- `LabelText` / `LabelTextOn` — label above/beside control
- `FloatingLabelOn` — MD3-style floating label
- External drawing pipeline (does all rendering to parent)

**What BeepTextBox needs to do:**
1. When `HelperText` is set: enable `HelperTextOn = true` (set defaults)
2. Wire `ErrorText` priority: `IsError` shows `ErrorText` instead of `HelperText` (BaseControl already does this)
3. Use `LabelText` for the field label (if set by user)
4. Call `UpdateExternalDrawing()` after property changes
5. Consolidate: BeepTextBox's `PlaceholderText` serves as the "inside" hint; BaseControl's `HelperText` serves as "below" supporting text

**Files:** `BeepTextBox.Properties.cs`, `BeepTextBox.Core.cs`

### 1.3 Hover State Rendering (0.5 day)
**What exists in BaseControl:**
- `IsHovered` is auto-set by `OnMouseEnter`/`OnMouseLeave` (already called)
- `HoverBackColor`, `HoverForeColor`, `HoverBorderColor` are already set by `ApplyTheme()`
- `UseBaseMouseInputRouting` (default false for BeepTextBox—needs to check this!)

**What BeepTextBox needs to do:**
1. Check if `UseBaseMouseInputRouting` is true (if not, the hover state isn't being tracked)
2. In `DrawContent()`: if `IsHovered && !Focused && !HasError`, fill background with `HoverBackColor` and draw border with `HoverBorderColor`
3. Optionally add smooth hover transition via existing `_animationTimer`

**Files:** `BeepTextBox.Drawing.cs`, `BeepTextBox.Core.cs`

### 1.4 Read-Only / Disabled Visual Distinction (0.5 day)
**What exists in BaseControl:**
- `DisabledBackColor`, `DisabledBorderColor`, `DisabledForeColor` from theme
- `IsEditable` property from `IBeepUIComponent`

**What BeepTextBox needs to do:**
1. In `DrawContent()`: when `ReadOnly`, fill background with `DisabledBackColor`
2. Use `DisabledBorderColor` for border (dashed or lighter)
3. Disable focus animation when ReadOnly
4. Disable typing indicator when ReadOnly
5. Optionally set `LeadingIconPath = SvgsUIcons.Common.Lock` when ReadOnly (via `ShowReadOnlyIcon` property)

**Files:** `BeepTextBox.Drawing.cs`, `BeepTextBox.Events.cs`

### 1.5 Required Indicator (0.5 day)
**What exists in BaseControl:**
- `IsRequired` property from `IBeepUIComponent`

**What BeepTextBox needs to do:**
1. When `IsRequired`: set `TrailingIconPath = SvgsUIcons.Common.Star` (or render "*" in label)
2. When text is empty and required: show error via `ShowValidation(Warning, "Required")`
3. When text is filled and was showing required warning: clear it
4. `LabelText` auto-appends "*" when `IsRequired` (BaseControl behavior, check if implemented)

**Files:** `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`

### 1.6 Context Menu Wiring (1 day)
**What exists in BaseControl:**
- `ShowBeepContextMenu()` / `ShowContextMenu(List<SimpleItem>)`
- `ShowContextMenuOnRightClick(items, e)`
- `CreateMenuItem(text, imagePath, tag)` — `public static`
- `CreateMenuItemWithShortcut(text, shortcut, imagePath, tag)` — `public static`
- `CreateMenuSeparator()` — `public static`

**What BeepTextBox needs to do:**
1. Override `OnMouseUp` (right-click): hide autocomplete popup, build menu items, call `ShowContextMenu()`
2. Menu structure:
   ```
   Undo (Ctrl+Z)  [fi-tr-undo]          — disabled if no undo stack
   Redo (Ctrl+Y)  [fi-tr-redo-alt]      — disabled if no redo stack
   ─────────────
   Cut (Ctrl+X)   [fi-tr-copy]           — disabled if no selection or ReadOnly
   Copy (Ctrl+C)  [fi-tr-copy]           — disabled if no selection
   Paste (Ctrl+V) [fi-tr-paste]          — disabled if ReadOnly or no clipboard
   Delete         [fi-tr-trash-empty]    — disabled if no selection or ReadOnly
   ─────────────
   Select All (Ctrl+A)                   — always enabled
   ```
3. Wire `ContextMenuItemSelected` event to dispatch actions:
   - "Undo" tag → `_helper.UndoRedo.Undo()`
   - "Redo" tag → `_helper.UndoRedo.Redo()`
   - "Cut" tag → `Cut()`
   - etc.
4. Autocomplete integration: hide popup before showing context menu

**Files:** `BeepTextBox.Events.cs`

---

## Phase 2: Trailing Icon Integration (P1-P2)
Goal: Use BaseControl's icon slots rather than building custom zones. Status: PLANNING

### 2.1 Clear Button Wiring (0.5 day)
**What exists in BaseControl:**
- `ShowClearButton` property (bool, default false)
- When enabled, BaseControl renders X icon and handles click internally

**What BeepTextBox needs to do:**
1. The BaseControl painter renders the clear button in the trailing slot automatically
2. Subscribe to `TrailingIconClicked` event to clear text
3. Wire `ClearButtonClicked` event (custom event for consumers)
4. When text is empty, the clear button auto-hides (BaseControl behavior—verify)
5. If BaseControl's auto-hide doesn't work for textboxes, override with manual check in DrawContent

**Files:** `BeepTextBox.Events.cs`

### 2.2 Password Reveal Toggle (1 day)
**What exists in BaseControl:**
- `TrailingIconPath` — can be toggled between eye-open/eye-closed
- `TrailingIconClicked` event

**What BeepTextBox needs to do:**
1. Add `ShowPasswordReveal` property (bool, default false): toggles visibility of reveal icon
2. Add `PasswordRevealed` field (bool, default false): current reveal state
3. When `ShowPasswordReveal && (UseSystemPasswordChar || PasswordChar != '\0')`:
   - Set `TrailingIconPath = PasswordRevealed ? SvgsUIcons.Common.Visible : SvgsUIcons.Common.Hidden`
   - When `PasswordRevealed`: bypass `GetActualText()` masking to show real text
4. Subscribe to `TrailingIconClicked`: toggle `PasswordRevealed`, flip icon
5. Auto-re-mask on focus lost after `AutoReMaskDelayMs` (default 2s)
6. Add `PasswordRevealChanged` event
7. **Accessibility**: Set accessible name on trailing icon

**Files:** `BeepTextBox.Properties.cs`, `BeepTextBox.Methods.cs`, `BeepTextBox.Events.cs`

### 2.3 Autocomplete Dropdown Caret (0.5 day)
**What exists in BaseControl:**
- `TrailingIconPath` — can toggle dropdown arrow

**What BeepTextBox needs to do:**
1. When `AutoCompleteMode != None` and text is empty: set `TrailingIconPath = SvgsUIcons.Carets.Down`
2. When autocomplete popup is open: set `TrailingIconPath = SvgsUIcons.Carets.Up`
3. Subscribe to `TrailingIconClicked`: trigger autocomplete with all suggestions (like ComboBox dropdown)
4. Icon auto-hides when `ShowClearButton` or `ShowPasswordReveal` is active and they occupy trailing slot

**Files:** `BeepTextBox.Events.cs`, `SmartAutoCompleteHelper.cs`

### 2.4 Leading Image Port (0.5 day)
**What exists in BeepTextBox:**
- `_beepImage` private instance with `ImagePath`, `ImageVisible`, etc.

**What exists in BaseControl:**
- `LeadingIconPath` / `LeadingImagePath` for left-side icon

**What BeepTextBox needs to do:**
1. When `ImagePath` is set and `ImageVisible`: set `LeadingIconPath = ImagePath`
2. Deprecate `_beepImage` for leading display; keep it for backward compatibility
3. Map `ImageAlign`, `TextImageRelation` to BaseControl's icon layout params

**Files:** `BeepTextBox.Properties.cs`, `BeepTextBox.Core.cs`

---

## Phase 3: AutoComplete System Integration (P1-P2)
Goal: Integrate autocomplete popup with BaseControl context/system. Status: PLANNING

### 3.1 Context Menu Coexistence (0.25 day)
- Hide autocomplete popup via `SmartAutoCompleteHelper.HideAutoCompletePopup()` before showing context menu
- Re-trigger autocomplete after context menu actions (undo/redo/paste change text)

### 3.2 Search Coexistence (0.25 day)
- When Ctrl+F triggers search dialog: hide autocomplete popup first
- When Escape is pressed: priority order = close autocomplete → clear search → lose focus
- Incremental search and autocomplete share 300ms timer: autocomplete wins when both enabled

### 3.3 Autocomplete Popup Accessibility (0.5 day)
- When popup opens: Narrator announces "suggestions available"
- UIA notification on popup show/hide
- Arrow keys in popup: navigate items with announcement
- Enter on popup item: select and announce

### 3.4 Popup Theme Consistency (0.25 day)
- Ensure `BeepPopupListForm` reads theme from parent textbox
- Pass `Theme` string to `ContextMenuManager.Show()` or `BeepPopupListForm`

**Files:** `BeepTextBox.Events.cs`, `SmartAutoCompleteHelper.cs`, `BeepPopupListForm.cs`

---

## Phase 4: IME & Spell Check (P2-P3)
Goal: International input and spell check. Status: PLANNING

### 4.1 IME Composition Support (3 days)
- Override `WndProc` for `WM_IME_STARTCOMPOSITION`, `WM_IME_COMPOSITION`, `WM_IME_ENDCOMPOSITION`, `WM_IME_CHAR`
- Render composition string with underline
- Handle IME candidate window positioning near caret

**Files:** `BeepTextBox.Core.cs`, `BeepTextBox.Drawing.cs`

### 4.2 Spell Check (2 days)
- Integrate NHunspell NuGet package
- Add `DictionaryPath`, `CustomDictionaryWords` properties
- Render squiggly underline on misspelled words (viewport-culled)
- Add right-click suggestions to context menu (append items)
- "Add to dictionary", "Ignore all" context items
- Spell check does NOT trigger autocomplete

**Files:** New `SpellCheckHelper.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Events.cs`

---

## Phase 5: Accessibility (P0, parallel)
Goal: WCAG 2.1 AA. Status: PLANNING

### 5.1 UIA Provider (4 days)
- Override `CreateAccessibilityInstance()` → custom `BeepTextBoxAccessibleObject`
- Implement `IValueProvider` (Value, IsReadOnly, SetValue)
- Implement `ITextProvider` for multiline (text ranges)
- Expose `ControlType.Edit`
- Raise UIA TextChanged notifications

### 5.2 Keyboard & Screen Reader (1 day)
- All trailing icons keyboard-accessible (Tab to icon, Enter/Space to activate)
- Accessible names for icons: "Clear text", "Show/Hide password", "Required field"
- Test with Narrator + Accessibility Insights

**Files:** New `BeepTextBoxAccessibleObject.cs`, `BeepTextBox.Core.cs`, `BeepTextBox.Input.cs`

---

## Phase 6: Performance & Testing (P2)
Goal: Production-hardened. Status: PLANNING

### 6.1 Performance (1 day)
- Remove `CreateGraphics()` from `UpdateDrawingRect()` — use cached metrics
- Viewport-culling for multiline text (only paint visible lines)
- Cache line heights (same font = same height for all lines)
- Merge timer instances where possible

### 6.2 Testing (2 days)
- Unit: text input/delete/selection/caret, keyboard shortcuts, masks, multiline, undo/redo, search, password, clear/reveal, error state flow, trailing icon state machine, AutoComplete lifecycle, DPI scaling
- Integration: all themes, designer serialization, data binding + error state, form lifecycle, AutoComplete+context menu+search co-existence, IME+autocomplete co-existence

**Files:** Tests project

---

## What Was REMOVED From Previous Plan

| Removed Item | Why |
|---|---|
| New `IsError` / `ErrorBorderColor` / `ErrorText` properties | Already exist in BaseControl |
| Custom error icon rendering | BaseControl creates `ValidationIcon` automatically |
| `HelperText` / `ShowHelperText` properties | Already exist in BaseControl |
| `LabelText` / `LabelTextOn` properties | Already exist in BaseControl |
| `IsHovered` wiring | Already handled by BaseControl's `OnMouseEnter/Leave` |
| `HoverBackColor` property | Already exists in BaseControl |
| `ReadOnlyBackColor` property | Use existing `DisabledBackColor` |
| Custom trailing icon zone architecture | BaseControl has `TrailingIconPath` + painter |
| `IconSize` / `IconPadding` properties | Already exist in BaseControl |
| Custom context menu infrastructure | `ShowContextMenu()` + `CreateMenuItem()` exist |
| `ShowClearButton` property | Already exists in BaseControl |
| `TrailingIconPath` / `LeadingIconPath` | Already exist in BaseControl |
| `IsRequired` property | Already exists in BaseControl (IBeepUIComponent) |
| `ShowExternalIcon()` / `ClearExternalIcon()` | Already exist in BaseControl |

---

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| BeepTextBox `DrawContent` override conflicts with BaseControl painter | Medium | High | Careful composition: call base.DrawContent first, then overlay BeepTextBox-specific |
| `UseBaseMouseInputRouting` false means hover isn't tracked | Low | Medium | Verify; if false, manually override OnMouseEnter/Leave |
| `ShowClearButton` behavior differs for textboxes vs buttons | Medium | Medium | May need to tweak BaseControl painter for text-field use case |
| `TrailingIconPath` slot already used by `ShowClearButton` | Medium | Medium | Priority ordering needed (clear > reveal > caret) |
| IME complexity | High | High | 2-day spike first; fallback to basic WM_IME_CHAR |
| BaseControl external drawing may not render inside BeepTextBox's control area | Low | Medium | Verify parent supports IExternalDrawingProvider |

## Timeline Estimate

| Phase | Weeks | Dependency |
|-------|-------|------------|
| Phase 1: Wire to BaseControl | 1.0 | None |
| Phase 2: Trailing Icon Integration | 0.5 | Phase 1 |
| Phase 3: AutoComplete Integration | 0.5 | Phase 1 |
| Phase 4: IME + Spell Check | 1.0 | Phase 1 |
| Phase 5: Accessibility | 1.0 | Phase 1 (parallel) |
| Phase 6: Perf/Testing | 0.5 | Phase 2-5 |
| **Total** | **4.5 weeks** | |

Reduced from 7.5 weeks (building from scratch) to 4.5 weeks (wiring + small new features).
