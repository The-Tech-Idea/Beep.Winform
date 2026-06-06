# BeepTextBox Enhancement Plan

Priority: High
Status: Planning Approved
Date: 2026-06-06
Revised: 2026-06-06 (icon paths, autocomplete integration)

## Icon Sources

All trailing/leading icons reference `TheTechIdea.Beep.Icons.SvgsUIcons` from
`IconsManagement\SvgsUIcons.cs`. The `BeepImage` infrastructure supports embedded
SVG rendering with theme tinting via `ApplyThemeOnImage`.

Key icon paths used throughout this plan:

| Feature | Icon class | Resource |
|---------|-----------|----------|
| Search | `Common.Search` | `fi-tr-search-alt.svg` |
| Clear (X) | `Common.Close` | `fi-tr-x.svg` |
| Password show | `Common.Visible` | `fi-tr-eye-recognition.svg` |
| Password hide | `Common.Hidden` | `fi-tr-low-vision.svg` |
| Error indicator | `Common.Error` | `fi-tr-circle-xmark.svg` |
| Warning indicator | `Common.Warning` | `fi-tr-triangle-warning.svg` |
| Success check | `Common.Check` | `fi-tr-check-circle.svg` |
| Required (*) | `Common.Star` | `fi-tr-star.svg` |
| Read-only lock | `Common.Lock` | `fi-tr-lock-alt.svg` |
| Loading spinner | `Activity.Loading` | `fi-tr-loading.svg` |
| Dropdown caret | `Carets.Down` | `fi-tr-caret-down.svg` |
| Autocomplete list | `View.List` | `fi-tr-rectangle-list.svg` |
| Context copy | `Common.Copy` | `fi-tr-copy.svg` |
| Context paste | `Common.Paste` | `fi-tr-paste.svg` |
| Context undo | `Common.Undo` | `fi-tr-undo.svg` |
| Context redo | `Common.Redo` | `fi-tr-redo-alt.svg` |
| Spell check error | `Common.Warning` | `fi-tr-triangle-warning.svg` |

---

## Overview

Enhance BeepTextBox to achieve commercial parity with DevExpress/Telerik text
controls and modern web standards (Material Design 3, Fluent UI). Current
control has strong feature breadth but lacks accessibility, validation UI, and
modern adornment patterns.

## AutoComplete Integration Architecture

The current autocomplete system (`SmartAutoCompleteHelper` + `BeepPopupListForm`)
needs tighter integration. Key integration touchpoints:

```
User types char
  -> InsertText() [BeepSimpleTextBoxHelper]
     -> ShouldTriggerAutoComplete()
        -> SmartAutoCompleteHelper.TriggerSmartAutoComplete()
           -> 300ms debounce timer
           -> ShowAutoCompletePopup()
              -> BeepPopupListForm.ShowPopup() (borderless, fade-animated)

User context-clicks
  -> OnMouseUp() [new: ContextMenu integration]
     -> IF autocomplete popup is open: HideAutoCompletePopup()
     -> THEN show ContextMenuStrip

User presses Ctrl+F
  -> HandleSearchKeyDown() [BeepTextBox.Search.cs]
     -> IF autocomplete popup is open: HideAutoCompletePopup()
     -> THEN ShowFindDialog()

User presses Ctrl+V (paste)
  -> Paste() [BeepTextBox.Input.cs]
     -> IF paste adds new characters: TriggerSmartAutoComplete()
        -> Re-evaluate suggestions based on new content
```

### AutoComplete Integration Rules

1. **Context Menu**: Hide autocomplete popup before showing context menu.
2. **Search/F3**: Hide autocomplete popup when search is activated (Ctrl+F).
3. **Escape**: Priority: close autocomplete popup first, then clear search, then
   lose focus.
4. **Paste/Undo/Redo**: Re-trigger autocomplete after text mutations.
5. **Focus Lost**: Hide autocomplete popup immediately (already done via
   `OnGotFocus`/`OnLostFocus` tie-in).
6. **Incremental Search vs Autocomplete**: They share the `_incrementalSearchTimer`
   (300ms). When both are enabled, autocomplete wins and incremental search
   defers. When only search is enabled, incremental search fires.
7. **Dropdown Caret**: When `AutoCompleteMode != None`, render `Carets.Down` as
   trailing icon to indicate dropdown capability (like ComboBox).

---

## Phase Structure

### Phase 1: Foundation Hardening (P0)
Goal: Fix critical gaps that block production readiness. Status: PLANNING

#### P1-W1: Validation Error UI (3 days)
- Add `IsError` property (bool, default false)
- Add `ErrorBorderColor` property (Color, default `Color.FromArgb(239, 68, 68)`)
- Add `ErrorText` property (string, shown in helper-text area)
- Add `ErrorIconVisible` property (bool, default true)
- Render error icon via `Common.Error` (`fi-tr-circle-xmark.svg`) as trailing icon
- Modify `Draw()` to render solid error-state border (no animation, distinct from focus)
- Wire `ValidateData()` result -> `IsError` + `ErrorText`
- Add `ErrorStateChanged` event
- Error icon takes rightmost trailing slot (before clear button if both shown)

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Methods.cs`, `BeepTextBox.Events.cs`

#### P1-W2: Context Menu with Icon Integration (1.5 days)
- Override `OnMouseUp` to show `ContextMenuStrip` on right-click
- Menu items (with SvgsUIcons paths):
  - **Undo** (`Common.Undo`, `fi-tr-undo.svg`)
  - **Redo** (`Common.Redo`, `fi-tr-redo-alt.svg`)
  - Separator
  - **Cut** (`Common.Copy`, reuse -- or scissor icon if available)
  - **Copy** (`Common.Copy`, `fi-tr-copy.svg`)
  - **Paste** (`Common.Paste`, `fi-tr-paste.svg`)
  - **Delete**
  - Separator
  - **Select All**
- Disable items per `ReadOnly` state and selection state:
  - Undo: disabled when stack empty
  - Redo: disabled when stack empty
  - Cut: disabled when `ReadOnly` or no selection
  - Paste: disabled when `ReadOnly` or no clipboard text
  - Delete: disabled when `ReadOnly` or no selection
  - Select All: always enabled
- **AutoComplete integration**: Hide autocomplete popup before showing context menu
- On Escape while context menu open: close menu (not textbox focus)
- Load icons via `BeepImage` using `ApplyThemeOnImage = true` to ensure theme tinting

**Files**: `BeepTextBox.Events.cs`, `BeepTextBox.Input.cs`

#### P1-W3: Undo/Redo Hardening (2 days)
- Review `BeepTextBoxUndoRedoHelper` for multi-step support
- Implement unbounded undo/redo stack with 100-entry cap
- Memoize snapshots: (text, caret position, selection start, selection length) tuple
- Add `UndoAvailable` / `RedoAvailable` events (wire to context menu enabled states)
- Preserve caret position and selection on undo/redo
- Unit tests for undo/redo chain integrity
- **AutoComplete integration**: Re-trigger autocomplete after undo/redo

**Files**: `TextFields/Helpers/BeepTextBoxUndoRedoHelper.cs`, Tests

#### P1-W4: Font/DPI Robustness (1 day)
- Remove all `CreateGraphics()` calls from layout methods
- Cache text metrics at `HandleCreated` time or first paint
- Use `TextRenderer.MeasureText` everywhere for consistency
- Fix `UpdateDrawingRect()` to use cached metrics
- Ensure DPI changes invalidate all caches

**Files**: `BeepTextBox.Methods.cs`, `BeepTextBox.Core.cs`

**Exit Criteria:** Error state renders visually with icon. Context menu works with
icons. Undo/redo handles 50+ steps. No `CreateGraphics()` in layout path.

---

### Phase 2: UX Polish (P1)
Goal: Match modern web/material UX patterns. Status: PLANNING

#### P2-W1: Trailing Icon Zone Architecture (0.5 day)
Before adding specific icons, create a unified trailing icon zone that handles:
- Icon slot ordering: `error → password reveal → clear button → autocomplete caret → loading spinner`
- Hit-testing per icon slot
- Right-to-left awareness (swap to leading zone for RTL)
- Theme-aware rendering via `BeepImage` with `ApplyThemeOnImage`
- Each icon creates its own `BeepImage` instance (not SVGs loaded per paint)

**Files**: `BeepTextBox.Drawing.cs`, new helper `TrailingIconManager` (optional)

#### P2-W2: Clear Button (X) (1 day)
- Add `ShowClearButton` property (bool, default false)
- Render `Common.Close` (`fi-tr-x.svg`) in trailing zone when text is non-empty
- Clicking clears text, resets error state, raises `ClearButtonClicked` event
- Auto-hide when `ReadOnly` or text is empty
- Support theme tinting via `ApplyThemeOnImage`
- **AutoComplete integration**: Closing via clear button hides autocomplete popup
- **Compatibility**: `ShowClearButton` + `ImageVisible` + password reveal are
  mutually stackable in trailing zone

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Events.cs`

#### P2-W3: Password Reveal Toggle (1 day)
- Add `ShowPasswordReveal` property (bool, default false)
- Add `PasswordRevealed` state (bool, default false)
- Render `Common.Hidden` (`fi-tr-low-vision.svg`) when masked, `Common.Visible`
  (`fi-tr-eye-recognition.svg`) when revealed
- Toggle `PasswordRevealed` on click; fires `PasswordRevealChanged` event
- When revealed: show actual text (bypass `GetActualText()` masking)
- When re-masked or focus lost: auto-re-mask after `AutoReMaskDelayMs` (default 2000ms)
- **Accessibility**: Narrator reads "Show password" / "Hide password" based on state

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Methods.cs`

#### P2-W4: Hover State Rendering (1 day)
- Activate existing `HoverBackColor`/`HoverForeColor` properties (already set by theme)
- Override `OnMouseEnter` to set `_isHovered = true` + `Invalidate()`
- Override `OnMouseLeave` to set `_isHovered = false` + `Invalidate()`
- Modify `DrawContent()` to apply hover colors when `_isHovered && !Focused`
- Add smooth hover transition (0.15s ease) reusing animation timer
- Hover border color is lighter than focus border color

**Files**: `BeepTextBox.Events.cs`, `BeepTextBox.Drawing.cs`

#### P2-W5: Required Indicator (0.5 day)
- Add `IsRequired` property (bool, default false)
- Render `Common.Star` (`fi-tr-star.svg`) as trailing icon in the required slot
- Required indicator color: `_currentTheme.ErrorColor` or red
- Show as trailing icon (right side) -- follows MD3 and Fluent UI patterns
- Tooltip on hover: "Required field"
- Render before clear button, after error icon in trailing zone order
- **Accessibility**: `aria-required=true` via UIA provider

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`

#### P2-W6: Helper Text / Supporting Text (1 day)
- Add `HelperText` property (string, shown below control)
- Add `ShowHelperText` property (bool, default false)
- Render helper text in DPI-aware small font below the control bottom border
- Switch font color to `ErrorBorderColor` when `IsError && !string.IsNullOrEmpty(ErrorText)`
- Match Material Design 3 supporting text pattern (40px below border, 12px font)
- Adjust `DisplayRectangle` to leave space for helper text (affects parent layout)

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Methods.cs`

#### P2-W7: Read-Only Visual Distinction (0.5 day)
- Add `ReadOnlyBackColor` property (default: subtle gray from theme)
- Draw control background in neutral tone when `ReadOnly`
- Show `Common.Lock` (`fi-tr-lock-alt.svg`) as leading icon when `ReadOnly` (optional, `ShowReadOnlyIcon` property)
- Reduce border visibility in read-only mode (dashed or lighter)
- Disable hover and focus animations in read-only mode
- **Context Menu**: Show only Copy when ReadOnly

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Events.cs`

**Exit Criteria:** Clear button, password reveal, required star, hover, helper text,
and read-only distinction all render correctly with theme-aware icons.

---

### Phase 3: Adornments & Modern Features (P2)
Goal: Add prefix/suffix slots and async states. Status: PLANNING

#### P3-W1: Prefix/Suffix Text Slots (2 days)
- Add `PrefixText` property (string, shown before editable text, left side)
- Add `SuffixText` property (string, shown after editable text, right side)
- Add `PrefixForeColor`, `SuffixForeColor` properties (default: subdued theme color)
- Render as non-editable, non-selectable text within the text boundary
- Adjust input text rect (`_textRect`) to accommodate prefix/suffix width
- Prefix example: `$` for currency, `@` for username, `https://` for URL
- Suffix example: `.00` for decimals, `kg` for weight, `@domain.com` for email
- Support in theme `ApplyTheme()`
- **Affects**: `_textRect` calculation, caret position offset, selection offsets

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Methods.cs`

#### P3-W2: Loading/Spinner State (1 day)
- Add `IsLoading` property (bool, default false)
- When `IsLoading = true`:
  - Render `Activity.Loading` (`fi-tr-loading.svg`) as trailing icon with rotation animation
  - Disable input (`ReadOnly` temporarily but visually distinct — gray + spinner)
  - Show placeholder "Loading..."
  - Disable clear button, password reveal while loading
- When `IsLoading = false`: restore previous state
- Loading spinner color: theme accent color
- **AutoComplete integration**: Hide popup while loading

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Input.cs`

#### P3-W3: AutoComplete Dropdown Caret (0.5 day)
- When `AutoCompleteMode != None`, render `Carets.Down` (`fi-tr-caret-down.svg`)
  as trailing icon (in the dropdown caret slot)
- Clicking the caret opens autocomplete popup with all suggestions (like ComboBox dropdown)
- Caret rotates 180° when popup is open (animate rotation)
- **Integration**: This is the explicit open-trigger; typing still triggers implicitly

**Files**: `BeepTextBox.Drawing.cs`, `BeepTextBox.Events.cs`, `SmartAutoCompleteHelper.cs`

#### P3-W4: Auto-Size Multiline Support (1 day)
- Add `AutoSize` property for multiline mode (bool, default false)
- Calculate required height from content lines * line height + padding + helper text
- Update control height on text change when `AutoSize = true`
- Respect `MaxHeight`/`MinHeight` constraints
- Only grow downward; never shrink below initial height

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Methods.cs`

#### P3-W5: Drag-Drop Text Support (0.5 day)
- Set `AllowDrop = true`
- Override `OnDragEnter`, `OnDragOver`, `OnDragDrop`
- Accept `Text`, `UnicodeText`, `StringFormat` data formats
- Insert at drop position (use caret positioning via hit-test)
- Respect `ReadOnly` state
- Highlight drop target with subtle underline indicator

**Files**: `BeepTextBox.Events.cs`

**Exit Criteria:** Prefix/suffix render. Loading spinner works. Autocomplete caret
opens suggestions. Multiline auto-grows. Text drag-droppable.

---

### Phase 4: Internationalization & Stubs (P2-P3)
Goal: Enable global usage and complete stub implementations. Status: PLANNING

#### P4-W1: IME Composition Support (2-3 days)
- Implement IME integration via `WndProc` override
- Handle messages: `WM_IME_STARTCOMPOSITION`, `WM_IME_COMPOSITION`,
  `WM_IME_ENDCOMPOSITION`, `WM_IME_CHAR`
- Render composition string with underline during IME input
- Handle IME candidate list positioning near caret
- Support full-width/half-width mode toggle
- **Risks**: IMM32 interop complexity. Spike first.

**Files**: `BeepTextBox.Core.cs` (WndProc), `BeepTextBox.Drawing.cs`

#### P4-W2: Spell Check Implementation (2 days)
- Integrate with NHunspell (NuGet: `NHunspell`)
- Add `DictionaryPath` property (string, path to .dic/.aff files)
- Add `CustomDictionaryWords` (string array, user-added words)
- Render red squiggly underline (`DrawCurve` zigzag) on misspelled words
- Word highlighting via `WrapLine`-style word boundary detection
- Add right-click suggestions to context menu:
  - For each suggestion: render as menu item, click replaces word
  - "Add to dictionary" item
  - "Ignore all" item
- **Performance**: Only check visible lines + lines scrolled into view
- **AutoComplete integration**: Spell check does NOT trigger autocomplete
- **Icon**: `Common.Warning` (`fi-tr-triangle-warning.svg`) for spell suggestions header

**Files**: New `SpellCheckHelper.cs`, `BeepTextBox.Drawing.cs`, `BeepTextBox.Events.cs`

#### P4-W3: Right-to-Left Support (1 day)
- Add `RightToLeft` property (RightToLeft enum: No/Yes/Inherit)
- Mirror text rendering alignment (Left -> Right, Right -> Left)
- Mirror caret position and movement
- Swap leading/trailing icon zones:
  - Trailing zone becomes leading zone (on left side)
  - Leading image moves to right side
- Flip scrollbar to left side
- Handle RTL text measurement via `TextRenderer` with `TextFormatFlags.RightToLeft`
- **Impact**: All drawing code, all icon zone calculations

**Files**: `BeepTextBox.Properties.cs`, `BeepTextBox.Drawing.cs`,
`BeepTextBox.Methods.cs`, `BeepTextBox.Core.cs`

**Exit Criteria:** CJK input works via IME. Spell check underlines errors with
context menu suggestions. RTL text renders correctly.

---

### Phase 5: Accessibility (P0 but long-running)
Goal: WCAG 2.1 AA compliance for text input. Status: PLANNING

#### P5-W1: UIA Provider Implementation (3-4 days)
- Override `CreateAccessibilityInstance()` to return custom `AccessibleObject`
- Implement `IRawElementProviderSimple` pattern
- Expose `ControlType.Edit`
- Implement `IValueProvider`:
  - `Value` = text content (or masked for password)
  - `IsReadOnly` = ReadOnly state
  - `SetValue(string)` for programmatic input
- Implement `ITextProvider` for multiline mode (expose text ranges)
- Raise `AutomationNotificationKind.TextChanged` via `provider.RaiseAutomationEvent()`
- **Accessible name**: Use associated `Label` control text or `AccessibleName` property
- **Accessible description**: Include placeholder, helper text, error text
- `aria-required=true` when `IsRequired`

**Files**: New `BeepTextBoxAccessibleObject.cs`, `BeepTextBox.Core.cs`

#### P5-W2: Keyboard Accessibility (1 day)
- All trailing icons reachable via keyboard:
  - Tab to clear button, press Enter/Space to activate
  - Alt+P for password reveal toggle
  - Alt+C for autocomplete dropdown (when caret shown)
  - Escape closes any open popup/autocomplete
- Focus indicator meets 3:1 contrast ratio
- Visually hidden accessible name for icon-only buttons

**Files**: `BeepTextBox.Input.cs`, `BeepTextBox.Events.cs`

#### P5-W3: High Contrast & Narrator Testing (1 day)
- Support Windows High Contrast themes:
  - Detect `SystemInformation.HighContrast`
  - Override colors with system high-contrast palette
  - Ensure borders are visible (minimum 1px)
- Verify Narrator reads:
  - Text content (or "password, hidden" when masked)
  - Placeholder, helper text, error text
  - Character count / remaining characters
  - Required indicator as "required"
  - Read-only as "read-only"
  - Autocomplete suggestions as they appear
- Test with Accessibility Insights for Windows tool
- Document WCAG conformance level

**Files**: `BeepTextBox.Core.cs`, documentation

**Exit Criteria:** UIA tree shows BeepTextBox as Edit control. Narrator reads
values and states. Accessibility Insights passes core checks.

---

### Phase 6: Performance & Testing
Goal: Ship production-hardened control. Status: PLANNING

#### P6-W1: Performance Optimization (2 days)
- Profile text measurement in multi-line mode (1000+ lines)
- Lazy-measure: only measure visible lines (viewport culling via scroll offset)
- Cache line heights (same font = same height for all lines)
- Batch `Invalidate()` calls: use `SuspendLayout()`/`ResumeLayout()` pattern
- Reduce timer count: merge `_delayedUpdateTimer` and `_incrementalSearchTimer`
- Reuse `BeepImage` instances for trailing icons (don't create per-paint)
- **Benchmark target**: <5ms per keystroke paint, <16ms for 1000-line multiline

**Files**: `BeepTextBox.Methods.cs`, `BeepTextBox.Core.cs`, `BeepTextBox.Drawing.cs`

#### P6-W2: Unit Test Coverage (2 days)
- Text input, deletion, selection, caret navigation
- All keyboard shortcuts (Ctrl+A/C/V/X/Z/Y, Home, End, arrows)
- Mask format validation (all 10+ formats)
- Multiline text operations + word wrap + scroll
- Undo/redo stack integrity (50+ steps)
- Search/find/replace functional tests
- Password masking/unmasking + reveal toggle
- Clear button click and keyboard activation
- Required indicator visibility and accessibility
- Error state: set/clear/reset flow
- Trailing icon zone: ordering, hit-testing, visibility
- AutoComplete: popup open/close/select, caret open-trigger
- DPI scaling tests (96, 144, 192 DPI)

**Files**: Tests project

#### P6-W3: Integration / Regression Testing (1 day)
- Theme switching while text present (all themes)
- Designer serialization round-trip (all new properties)
- Data binding (`BoundProperty = "Text"`) with error state
- Form load/unload lifecycle (timers stop, no leaks)
- Control disposal without hanging timers
- AutoComplete + context menu co-existence
- AutoComplete + search co-existence
- IME + auto-complete co-existence (shouldn't deadlock)

**Files**: Tests project

**Exit Criteria:** No performance regression >5ms per keystroke. Unit test
coverage >80%. All integration tests pass.

---

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| IME integration complexity | High | High | 2-day spike first; fallback to basic WM_IME_CHAR |
| UIA provider Win32 interop complexity | Medium | Medium | Use managed `AccessibleObject` base; incremental approach |
| Undo/redo memory for 10MB+ text | Medium | Medium | Cap at 100 snapshots; snapshot only on 500ms idle |
| Trailing icon zone hit-testing edge cases | Medium | Medium | Full unit test matrix for all icon slot permutations |
| Icon SVG rendering performance per-paint | Low | Medium | Cache BeepImage instances; measure before commit |
| Theme breakage with new properties | Low | High | Backward-compatible defaults; theme integration tests |
| Spell check perf on large documents | Medium | Low | Only check visible viewport; async dictionary load |
| AutoComplete popup flicker with fast typing | Medium | Medium | 300ms debounce already in place; verify adequacy |
| RTL + trailing zone mirroring bugs | Medium | High | Dedicated RTL test suite; visual diff testing |

## Icon Slot Ordering (Trailing Zone)

```
Input text area  |  required-star  error-icon  password-reveal  clear-X  autocomplete-caret  loading-spinner  |  border
```

Priority and visibility rules:
1. `loading-spinner`: visible only when `IsLoading`
2. `autocomplete-caret`: visible when `AutoCompleteMode != None` and not loading
3. `clear-X`: visible when `ShowClearButton && text.Length > 0 && !ReadOnly && !IsLoading`
4. `password-reveal`: visible when `ShowPasswordReveal && UseSystemPasswordChar` or `PasswordChar != '\0'`
5. `error-icon`: visible when `IsError && ErrorIconVisible && !IsLoading`
6. `required-star`: visible when `IsRequired && !IsError && !IsLoading`

Leading icon: `ImageVisible` uses `ImagePath` on left side (existing `BeepImage`).

## Timeline Estimate

| Phase | Weeks | Dependency |
|-------|-------|------------|
| Phase 1: Foundation | 1.5 | None |
| Phase 2: UX Polish | 1.5 | Phase 1 (W4 only) |
| Phase 3: Adornments | 1.0 | Phase 2 (W1 trailing zone) |
| Phase 4: I18n | 1.5 | Phase 1 |
| Phase 5: Accessibility | 1.5 | Phase 1 |
| Phase 6: Perf/Testing | 1.0 | Phase 2-5 |
| **Total** | **8.0 weeks** | |

Phase 2 can largely overlap with Phase 1 after P1-W4 (Font robustness).
Phase 5 (Accessibility) runs in parallel with Phases 2-4.
