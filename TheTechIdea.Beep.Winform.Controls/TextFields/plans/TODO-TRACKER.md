# BeepTextBox TODO Tracker

Date: 2026-06-06 | Revised: 2026-06-06 (BaseControl audit)
Status: Active | Legend: P0=Critical P1=High P2=Medium P3=Low

## Strategy: Wire BeepTextBox to BaseControl, Don't Rebuild

BaseControl already has: error/validation, hover state, trailing/leading icons, clear button,
helper text, label text, context menu, external icon system, disabled colors, IsRequired.
BeepTextBox currently uses NONE of these. The tasks below wire BeepTextBox to what already exists.

---

## Phase 1: Wire to BaseControl Infrastructure

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 1.1 | Wire ValidateData() failure -> `ShowValidation(Error, message)` | P0 | TODO | 0.25d | Methods.cs: after ValidateData returns false |
| 1.2 | Wire text change clearing error -> `ClearValidation()` | P0 | TODO | 0.25d | Properties.cs: Text setter, when text is valid |
| 1.3 | In DrawContent: when `HasError`, draw error-colored border via `ErrorColor` | P0 | TODO | 0.25d | Drawing.cs: check HasError before normal border |
| 1.4 | Verify `ShowIndicatorLine` rendering via external drawing (no-op in DrawContent) | P0 | TODO | 0.25d | Drawing.cs: skip indicator line if BaseControl handles it |
| 1.5 | Enable `HelperTextOn = true` when `HelperText` is set | P1 | TODO | 0.25d | Core.cs: constructor or property wire |
| 1.6 | Wire `ErrorText` priority: shows instead of `HelperText` when error present | P1 | TODO | 0.25d | BaseControl already does this; verify |
| 1.7 | Verify BeepTextBox `LabelText` / `LabelTextOn` plumbing is functional | P1 | TODO | 0.25d | Properties.cs: may need to expose or just verify |
| 1.8 | Call `UpdateExternalDrawing()` after error/helper/label changes | P1 | TODO | 0.25d | Various setters |
| 1.9 | Verify `UseBaseMouseInputRouting` is true (or manually wire IsHovered) | P1 | TODO | 0.25d | Core.cs: check field; if false, override OnMouseEnter/Leave |
| 1.10 | In DrawContent: if `IsHovered && !Focused && !HasError`, use `HoverBackColor`/`HoverBorderColor` | P1 | TODO | 0.25d | Drawing.cs |
| 1.11 | Add smooth hover transition via existing `_animationTimer` | P1 | TODO | 0.25d | Drawing.cs: interpolate hover alpha |
| 1.12 | In DrawContent: when `ReadOnly`, fill with `DisabledBackColor` | P1 | TODO | 0.25d | Drawing.cs |
| 1.13 | Use `DisabledBorderColor` for border when ReadOnly | P1 | TODO | 0.25d | Drawing.cs: dashed or lighter border |
| 1.14 | Disable focus animation when ReadOnly | P1 | TODO | 0.25d | Events.cs: OnGotFocus guard |
| 1.15 | Disable typing indicator when ReadOnly | P1 | TODO | 0.25d | Drawing.cs: guard |
| 1.16 | When `IsRequired`: set `LabelText` suffix "*" or set `TrailingIconPath = Common.Star` | P1 | TODO | 0.25d | Properties.cs |
| 1.17 | When text empty + IsRequired: `ShowValidation(Warning, "Required")` | P1 | TODO | 0.25d | Methods.cs or Text setter |
| 1.18 | When text filled after required warning: `ClearValidation()` | P1 | TODO | 0.25d | Text setter |
| 1.19 | Override OnMouseUp: right-click -> hide autocomplete, build menu, `ShowContextMenu()` | P0 | TODO | 0.25d | Events.cs |
| 1.20 | Build context menu items via `CreateMenuItem(text, imagePath)` / `CreateMenuItemWithShortcut` | P0 | TODO | 0.25d | Events.cs: Undo,Redo,Cut,Copy,Paste,Del,SelectAll |
| 1.21 | Map image paths: Undo=`Common.Undo`, Redo=`Common.Redo`, Copy=`Common.Copy`, Paste=`Common.Paste`, Del=`Common.Delete` | P0 | TODO | 0.25d | Events.cs |
| 1.22 | Wire `ContextMenuItemSelected` to dispatch actions (tag-based switch) | P0 | TODO | 0.25d | Events.cs |
| 1.23 | Dynamic enabled/disabled: Undo/Redo per stack, Cut/Paste/Del per ReadOnly+selection | P1 | TODO | 0.25d | Events.cs: rebuild menu each right-click |

## Phase 2: Trailing Icon Integration via BaseControl

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 2.1 | Set `ShowClearButton = true` in constructor when BeepTextBox wants clear button | P1 | TODO | 0.25d | Core.cs: or expose as property |
| 2.2 | Subscribe to `TrailingIconClicked` -> clear text | P1 | TODO | 0.25d | Events.cs: check if clear button was clicked |
| 2.3 | Add `ShowPasswordReveal` property (bool, default false) | P1 | TODO | 0.25d | Properties.cs |
| 2.4 | Add `PasswordRevealed` field (bool, default false) | P1 | TODO | 0.25d | Core.cs |
| 2.5 | When show+masked: set `TrailingIconPath = SvgsUIcons.Common.Visible/Hidden` | P1 | TODO | 0.25d | Properties.cs setter |
| 2.6 | When `PasswordRevealed`: bypass `GetActualText()` masking | P1 | TODO | 0.25d | Methods.cs: GetActualText guard |
| 2.7 | Subscribe to `TrailingIconClicked` -> toggle PasswordRevealed, flip icon | P1 | TODO | 0.25d | Events.cs |
| 2.8 | Auto-re-mask on focus lost after `AutoReMaskDelayMs` (2s) | P1 | TODO | 0.25d | Events.cs: OnLostFocus + timer |
| 2.9 | Add `PasswordRevealChanged` event | P1 | TODO | 0.25d | Events.cs |
| 2.10 | When `AutoCompleteMode != None`, set `TrailingIconPath = Carets.Down` | P2 | TODO | 0.25d | Properties.cs or ApplyTheme |
| 2.11 | When autocomplete popup open, flip to `Carets.Up` | P2 | TODO | 0.25d | SmartAutoCompleteHelper.cs |
| 2.12 | Subscribe to `TrailingIconClicked` -> open autocomplete with all suggestions | P2 | TODO | 0.25d | Events.cs |
| 2.13 | Port `_beepImage` leading display to `LeadingIconPath` | P2 | TODO | 0.25d | Properties.cs: ImagePath -> LeadingIconPath |
| 2.14 | Deprecate `_beepImage` for leading (backward compat shim) | P2 | TODO | 0.25d | Core.cs |

## Phase 3: AutoComplete System Hardening

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 3.1 | Hide autocomplete popup before showing context menu | P1 | TODO | 0.25d | Events.cs: OnMouseUp |
| 3.2 | Re-trigger autocomplete after undo/redo/paste | P1 | TODO | 0.25d | Input.cs + SmartAutoCompleteHelper |
| 3.3 | Hide autocomplete popup when Ctrl+F search activates | P1 | TODO | 0.25d | Search.cs |
| 3.4 | Escape priority: close popup -> clear search -> lose focus | P1 | TODO | 0.25d | Input.cs: HandleSearchKeyDown |
| 3.5 | Pass `Theme` string to `BeepPopupListForm` for theme consistency | P2 | TODO | 0.25d | SmartAutoCompleteHelper.cs |
| 3.6 | UIA notification: announce "suggestions available" when popup opens | P2 | TODO | 0.25d | SmartAutoCompleteHelper.cs |
| 3.7 | UIA notification: announce selected item | P2 | TODO | 0.25d | BeepPopupListForm.cs |
| 3.8 | Arrow keys in popup navigate with screen reader announcement | P2 | TODO | 0.25d | BeepPopupListForm.cs |

## Phase 4: IME & Spell Check

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 4.1 | SPIKE: IMM32/TSF for IME composition | P2 | TODO | 0.5d | Research viability |
| 4.2 | Override WndProc for WM_IME_STARTCOMPOSITION/COMPOSITION/ENDCOMPOSITION/CHAR | P2 | TODO | 1d | Core.cs |
| 4.3 | Render composition underline during IME input | P2 | TODO | 0.5d | Drawing.cs |
| 4.4 | Handle IME candidate window positioning near caret | P2 | TODO | 0.5d | Core.cs |
| 4.5 | Integrate NHunspell NuGet + DictionaryPath property | P3 | TODO | 0.5d | New SpellCheckHelper.cs |
| 4.6 | Render red squiggly underline on misspelled words (viewport-culled) | P3 | TODO | 1d | Drawing.cs |
| 4.7 | Add spell suggestions to context menu (append items on right-click word) | P3 | TODO | 0.5d | Events.cs: detect word under caret, add items |
| 4.8 | "Add to dictionary" / "Ignore all" context items | P3 | TODO | 0.25d | SpellCheckHelper.cs |
| 4.9 | Spell check does NOT trigger autocomplete | P3 | TODO | 0.25d | Integration rule |

## Phase 5: Accessibility

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 5.1 | SPIKE: Custom AccessibleObject for UIA Edit pattern | P0 | TODO | 0.5d | Research |
| 5.2 | Override `CreateAccessibilityInstance()` -> `BeepTextBoxAccessibleObject` | P0 | TODO | 0.25d | Core.cs |
| 5.3 | Implement IValueProvider (Value, IsReadOnly, SetValue) | P0 | TODO | 1d | New BeepTextBoxAccessibleObject.cs |
| 5.4 | Implement ITextProvider for multiline (text ranges) | P0 | TODO | 1d | New BeepTextBoxAccessibleObject.cs |
| 5.5 | Expose ControlType.Edit | P0 | TODO | 0.25d | AccessibleObject |
| 5.6 | Raise UIA TextChanged notifications | P0 | TODO | 0.5d | AccessibleObject |
| 5.7 | Set AccessibleName from associated Label control | P1 | TODO | 0.25d | AccessibleObject |
| 5.8 | AccessibleDescription: include placeholder, helper text, error text | P1 | TODO | 0.25d | AccessibleObject |
| 5.9 | AccessibleName for trailing icons: "Clear text", "Show password" etc. | P1 | TODO | 0.25d | AccessibleObject |
| 5.10 | Make trailing icons keyboard-accessible (Tab, Enter/Space) | P1 | TODO | 0.5d | Input.cs |
| 5.11 | Support Windows High Contrast theme | P1 | TODO | 0.5d | Core.cs + Drawing.cs |
| 5.12 | Focus indicator meets 3:1 contrast ratio | P1 | TODO | 0.25d | Drawing.cs |
| 5.13 | Test with Windows Narrator (all states) | P0 | TODO | 0.5d | QA |
| 5.14 | Test with Accessibility Insights for Windows | P0 | TODO | 0.5d | QA |
| 5.15 | Document WCAG 2.1 AA conformance | P1 | TODO | 0.25d | Docs |

## Phase 6: Performance & Testing

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 6.1 | Remove `CreateGraphics()` from `UpdateDrawingRect()` | P1 | TODO | 0.5d | Methods.cs: use TextRenderer/cached metrics |
| 6.2 | Implement viewport-culling for multiline (only paint visible lines) | P2 | TODO | 0.5d | Drawing.cs or helper |
| 6.3 | Cache line heights (same font = same height all lines) | P2 | TODO | 0.25d | TextFields/Helpers |
| 6.4 | Merge timer instances (delayedUpdate + incrementalSearch?) | P2 | TODO | 0.25d | Core.cs |
| 6.5 | Unit: text input/delete/selection/caret nav | P1 | TODO | 0.5d | Tests |
| 6.6 | Unit: keyboard shortcuts (Ctrl+A/C/V/X/Z/Y, Home, End, arrows) | P1 | TODO | 0.5d | Tests |
| 6.7 | Unit: mask format validation (10+ formats) | P1 | TODO | 0.5d | Tests |
| 6.8 | Unit: multiline + word wrap + scroll | P1 | TODO | 0.25d | Tests |
| 6.9 | Unit: undo/redo chain 50+ steps | P1 | TODO | 0.5d | Tests |
| 6.10 | Unit: search/find/replace | P1 | TODO | 0.25d | Tests |
| 6.11 | Unit: password masking + reveal toggle | P1 | TODO | 0.25d | Tests |
| 6.12 | Unit: clear button click + keyboard activation | P1 | TODO | 0.25d | Tests |
| 6.13 | Unit: validation error state flow (set/clear/reset) | P1 | TODO | 0.5d | Tests |
| 6.14 | Unit: trailing icon state machine (combinations) | P1 | TODO | 0.25d | Tests |
| 6.15 | Unit: AutoComplete popup lifecycle (open/select/close/caret trigger) | P1 | TODO | 0.5d | Tests |
| 6.16 | Unit: DPI scaling (96/144/192) | P2 | TODO | 0.25d | Tests |
| 6.17 | Integration: all themes + text present | P1 | TODO | 0.25d | Tests |
| 6.18 | Integration: designer serialization round-trip (all new props) | P1 | TODO | 0.25d | Tests |
| 6.19 | Integration: data binding + error state | P1 | TODO | 0.25d | Tests |
| 6.20 | Integration: form load/unload lifecycle (no timer leaks) | P1 | TODO | 0.25d | Tests |
| 6.21 | Integration: AutoComplete + context menu + search co-existence | P1 | TODO | 0.25d | Tests |
| 6.22 | Integration: IME + autocomplete co-existence | P2 | TODO | 0.25d | Tests |

---

## Quick Wins (Low Effort)

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| Q1 | Double-click word selection (select word under cursor) | P2 | TODO | 0.25d | Events.cs: OnMouseDown |
| Q2 | Triple-click line selection (select entire line) | P2 | TODO | 0.25d | Events.cs |
| Q3 | Placeholder fade-out on focus (animate alpha) | P3 | TODO | 0.25d | Drawing.cs |
| Q4 | Check ShowClearButton auto-hide when text empty works for textboxes | P2 | TODO | 0.25d | Drawing.cs verify |
| Q5 | Show match count header in autocomplete popup ("3 matches") | P2 | TODO | 0.25d | BeepPopupListForm.cs |
| Q6 | Verify DisabledBackColor renders correctly in all theme modes | P1 | TODO | 0.25d | QA across themes |

---

## Dependency Graph

```
Phase 1 (Wire to BaseControl) ──┬── Phase 2 (Trailing Icons)
                                ├── Phase 3 (AutoComplete) ──── Phase 6 (Perf/Test)
                                ├── Phase 4 (IME + Spell)
                                └── Phase 5 (Accessibility) ── Phase 6
```

Phases 2-5 can run largely in parallel after Phase 1 completes.

---

## Progress Summary

| Phase | Tasks | Done | In Progress | Blocked |
|-------|-------|------|-------------|---------|
| Phase 1: Wire | 23 | 0 | 0 | 0 |
| Phase 2: Trailing Icons | 14 | 0 | 0 | 0 |
| Phase 3: AutoComplete | 8 | 0 | 0 | 0 |
| Phase 4: IME + Spell | 9 | 0 | 0 | 0 |
| Phase 5: Accessibility | 15 | 0 | 0 | 0 |
| Phase 6: Perf/Test | 22 | 0 | 0 | 0 |
| Quick Wins | 6 | 0 | 0 | 0 |
| **Total** | **97** | **0** | **0** | **0** |

(vs 138 tasks in previous plan -- 41 tasks removed as BaseControl already provides the functionality)

Last Updated: 2026-06-06
