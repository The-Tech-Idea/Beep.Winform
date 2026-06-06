# BeepTextBox TODO Tracker

Date: 2026-06-06 | Revised: 2026-06-06
Status: Active
Legend: P0=Critical P1=High P2=Medium P3=Low
Icon source: `TheTechIdea.Beep.Icons.SvgsUIcons`

---

## Phase 1: Foundation Hardening

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 1.1 | Add IsError property (bool) | P0 | TODO | 0.25d | Properties.cs |
| 1.2 | Add ErrorBorderColor property (Color) | P0 | TODO | 0.25d | Default #EF4444 red |
| 1.3 | Add ErrorText property (string) | P0 | TODO | 0.25d | Properties.cs |
| 1.4 | Add ErrorIconVisible property (bool) | P0 | TODO | 0.25d | Default true |
| 1.5 | Render error icon via `Common.Error` (fi-tr-circle-xmark.svg) | P0 | TODO | 0.5d | Trailing zone slot |
| 1.6 | Render error-state border in Draw() | P0 | TODO | 0.5d | Solid, no animation |
| 1.7 | Wire ValidateData() -> IsError + ErrorText | P0 | TODO | 0.25d | Methods.cs |
| 1.8 | Add ErrorStateChanged event | P0 | TODO | 0.25d | Events.cs |
| 1.9 | Override OnMouseUp for context menu trigger | P0 | TODO | 0.25d | Events.cs |
| 1.10 | Build ContextMenuStrip with icon items | P0 | TODO | 0.5d | Undo/Redo/Cut/Copy/Paste/Del/SelAll |
| 1.11 | Load menu icons via BeepImage (Copy=f-tr-copy, Paste=f-tr-paste, etc) | P0 | TODO | 0.25d | SvgsUIcons.Common.* |
| 1.12 | Wire menu enabled/disabled per ReadOnly + selection state | P0 | TODO | 0.25d | Events.cs |
| 1.13 | Hide autocomplete popup before showing context menu | P0 | TODO | 0.25d | Integration rule #1 |
| 1.14 | Review UndoRedoHelper for multi-step support | P0 | TODO | 0.5d | TextFields/Helpers |
| 1.15 | Implement 100-entry undo/redo stack with snapshot tuples | P0 | TODO | 1d | (text, caret, selStart, selLen) |
| 1.16 | Add UndoAvailable / RedoAvailable events | P0 | TODO | 0.25d | Events.cs |
| 1.17 | Re-trigger autocomplete after undo/redo | P0 | TODO | 0.25d | Integration rule |
| 1.18 | Remove CreateGraphics() from UpdateDrawingRect | P0 | TODO | 0.5d | Methods.cs |
| 1.19 | Cache text metrics at HandleCreated / first paint | P0 | TODO | 0.5d | Core.cs |
| 1.20 | Unit tests: error state set/clear/reset flow | P0 | TODO | 0.5d | Tests |
| 1.21 | Unit tests: context menu items state machine | P0 | TODO | 0.5d | Tests |
| 1.22 | Unit tests: undo/redo 50+ steps | P0 | TODO | 0.5d | Tests |

## Phase 2: UX Polish

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 2.1 | Design unified trailing icon zone architecture | P1 | TODO | 0.5d | Drawing.cs, slot ordering |
| 2.2 | Implement icon slot hit-testing per slot | P1 | TODO | 0.25d | Drawing.cs |
| 2.3 | Add ShowClearButton property (bool) | P1 | TODO | 0.25d | Properties.cs |
| 2.4 | Render `Common.Close` (fi-tr-x.svg) as trailing icon | P1 | TODO | 0.25d | Drawing.cs |
| 2.5 | Handle clear button click: Clear() + hide popup | P1 | TODO | 0.25d | Events.cs |
| 2.6 | Add ClearButtonClicked event | P1 | TODO | 0.25d | Events.cs |
| 2.7 | Add ShowPasswordReveal property (bool) | P1 | TODO | 0.25d | Properties.cs |
| 2.8 | Add PasswordRevealed state (bool) | P1 | TODO | 0.25d | Properties.cs |
| 2.9 | Render `Common.Hidden` (fi-tr-low-vision) / `Common.Visible` (fi-tr-eye-recognition) toggle | P1 | TODO | 0.25d | Drawing.cs |
| 2.10 | Handle eye icon click: toggle PasswordRevealed | P1 | TODO | 0.25d | Events.cs |
| 2.11 | Auto-re-mask after AutoReMaskDelayMs (2s) on focus lost | P1 | TODO | 0.25d | Events.cs |
| 2.12 | Add PasswordRevealChanged event | P1 | TODO | 0.25d | Events.cs |
| 2.13 | Implement OnMouseEnter/OnMouseLeave hover | P1 | TODO | 0.25d | Events.cs |
| 2.14 | Apply HoverBackColor in Draw() (0.15s ease) | P1 | TODO | 0.25d | Drawing.cs |
| 2.15 | Hover border lighter than focus border | P1 | TODO | 0.25d | Drawing.cs |
| 2.16 | Add IsRequired property (bool) | P1 | TODO | 0.25d | Properties.cs |
| 2.17 | Render `Common.Star` (fi-tr-star.svg) as trailing required icon | P1 | TODO | 0.25d | Drawing.cs |
| 2.18 | Required indicator tooltip "Required field" | P1 | TODO | 0.25d | Events.cs |
| 2.19 | Set aria-required=true via UIA (defer to Phase 5) | P1 | TODO | 0.25d | Phase 5 integration |
| 2.20 | Add HelperText property (string) | P1 | TODO | 0.25d | Properties.cs |
| 2.21 | Add ShowHelperText property (bool) | P1 | TODO | 0.25d | Properties.cs |
| 2.22 | Render helper/error text below control in small font | P1 | TODO | 0.5d | Drawing.cs |
| 2.23 | Add ReadOnlyBackColor property | P1 | TODO | 0.25d | Properties.cs |
| 2.24 | Render read-only neutral background | P1 | TODO | 0.25d | Drawing.cs |
| 2.25 | Show `Common.Lock` (fi-tr-lock-alt.svg) when ReadOnly (optional) | P1 | TODO | 0.25d | Drawing.cs |
| 2.26 | Disable hover/focus animations in ReadOnly mode | P1 | TODO | 0.25d | Events.cs |
| 2.27 | Adjust layout/padding for helper text region | P1 | TODO | 0.25d | Methods.cs |
| 2.28 | Adjust DisplayRectangle for helper text | P1 | TODO | 0.25d | Methods.cs |

## Phase 3: Adornments & Modern Features

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 3.1 | Add PrefixText + PrefixForeColor properties | P2 | TODO | 0.25d | Properties.cs |
| 3.2 | Add SuffixText + SuffixForeColor properties | P2 | TODO | 0.25d | Properties.cs |
| 3.3 | Render prefix/suffix as static text in input area | P2 | TODO | 0.5d | Drawing.cs |
| 3.4 | Adjust _textRect for prefix/suffix width | P2 | TODO | 0.5d | Methods.cs |
| 3.5 | Add IsLoading property (bool) | P2 | TODO | 0.25d | Properties.cs |
| 3.6 | Render `Activity.Loading` (fi-tr-loading.svg) with rotation animation | P2 | TODO | 0.5d | Drawing.cs |
| 3.7 | Disable input + hide clear/reveal during loading | P2 | TODO | 0.25d | Input.cs |
| 3.8 | Hide autocomplete popup while loading | P2 | TODO | 0.25d | Integration |
| 3.9 | Render `Carets.Down` (fi-tr-caret-down.svg) when AutoCompleteMode != None | P2 | TODO | 0.25d | Drawing.cs |
| 3.10 | Animate caret rotation on popup open/close | P2 | TODO | 0.25d | Drawing.cs |
| 3.11 | Handle caret click: open autocomplete with all suggestions | P2 | TODO | 0.25d | Events.cs + SmartAutoCompleteHelper.cs |
| 3.12 | Add AutoSize property for multiline | P2 | TODO | 0.25d | Properties.cs |
| 3.13 | Calculate & update height from content line count | P2 | TODO | 0.5d | Methods.cs |
| 3.14 | Respect MaxHeight/MinHeight when auto-sizing | P2 | TODO | 0.25d | Methods.cs |
| 3.15 | Set AllowDrop = true | P2 | TODO | 0.25d | Core.cs |
| 3.16 | Implement OnDragEnter/Over/Drop (Text/UnicodeText) | P2 | TODO | 0.5d | Events.cs |
| 3.17 | Show drop target underline indicator | P2 | TODO | 0.25d | Drawing.cs |

## Phase 4: Internationalization & Stubs

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 4.1 | Research: IMM32/TSF for IME composition [SPIKE] | P2 | TODO | 0.5d | |
| 4.2 | Override WndProc for WM_IME_* messages | P2 | TODO | 1d | Core.cs |
| 4.3 | Render composition underline during IME input | P2 | TODO | 0.5d | Drawing.cs |
| 4.4 | Handle IME candidate window positioning | P2 | TODO | 0.5d | Core.cs |
| 4.5 | Support full-width/half-width mode toggle | P2 | TODO | 0.5d | Core.cs |
| 4.6 | Integrate NHunspell (NuGet) for spell check | P3 | TODO | 0.5d | New SpellCheckHelper.cs |
| 4.7 | Add DictionaryPath + CustomDictionaryWords properties | P3 | TODO | 0.25d | Properties.cs |
| 4.8 | Render squiggly red underline on misspelled words | P3 | TODO | 1d | Drawing.cs |
| 4.9 | Add spell suggestions to context menu (right-click on word) | P3 | TODO | 0.5d | Events.cs |
| 4.10 | Add "Add to dictionary" / "Ignore all" context items | P3 | TODO | 0.25d | Events.cs + SpellCheckHelper |
| 4.11 | Viewport-aware spell checking (visible lines only) | P3 | TODO | 0.25d | SpellCheckHelper.cs |
| 4.12 | Spell check does NOT trigger autocomplete | P3 | TODO | 0.25d | Integration rule |
| 4.13 | Add RightToLeft property | P3 | TODO | 0.25d | Properties.cs |
| 4.14 | Mirror text alignment & caret for RTL | P3 | TODO | 0.5d | Drawing.cs + Methods.cs |
| 4.15 | Swap leading/trailing icon zones for RTL | P3 | TODO | 0.25d | Drawing.cs |
| 4.16 | Flip scrollbar to left for RTL | P3 | TODO | 0.25d | Scrolling helper |
| 4.17 | Use TextFormatFlags.RightToLeft for text measurement | P3 | TODO | 0.25d | Methods.cs |

## Phase 5: Accessibility

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 5.1 | Research: UIA provider pattern for custom Control [SPIKE] | P0 | TODO | 0.5d | |
| 5.2 | Override CreateAccessibilityInstance() | P0 | TODO | 0.25d | Core.cs |
| 5.3 | Implement custom BeepTextBoxAccessibleObject | P0 | TODO | 1d | New file |
| 5.4 | Expose ControlType.Edit | P0 | TODO | 0.25d | AccessibleObject |
| 5.5 | Implement IValueProvider (Value, IsReadOnly, SetValue) | P0 | TODO | 1d | AccessibleObject |
| 5.6 | Implement ITextProvider for multiline (text ranges) | P0 | TODO | 1d | AccessibleObject |
| 5.7 | Raise UIA TextChanged notifications | P0 | TODO | 0.5d | AccessibleObject |
| 5.8 | Set AccessibleName from associated Label | P1 | TODO | 0.25d | AccessibleObject |
| 5.9 | Include placeholder/helper/error in AccessibleDescription | P1 | TODO | 0.25d | AccessibleObject |
| 5.10 | Set aria-required=true when IsRequired | P1 | TODO | 0.25d | AccessibleObject (deferred from 2.19) |
| 5.11 | Make trailing icons keyboard-accessible (Tab, Enter/Space) | P1 | TODO | 0.5d | Input.cs |
| 5.12 | Add Alt+P (reveal), Alt+C (autocomplete), Escape (close) shortcuts | P1 | TODO | 0.25d | Input.cs |
| 5.13 | Visually hidden accessible names for icon buttons | P1 | TODO | 0.25d | AccessibleObject |
| 5.14 | Support Windows High Contrast theme colors | P1 | TODO | 0.5d | Core.cs + Drawing.cs |
| 5.15 | Ensure focus indicator meets 3:1 contrast ratio | P1 | TODO | 0.25d | Drawing.cs |
| 5.16 | Test with Windows Narrator (all states) | P0 | TODO | 0.5d | QA |
| 5.17 | Test with Accessibility Insights for Windows | P0 | TODO | 0.5d | QA |
| 5.18 | Document WCAG 2.1 AA conformance | P1 | TODO | 0.25d | Docs |

## Phase 6: Performance & Testing

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| 6.1 | Profile text measurement perf (Stopwatch, 1000+ lines) | P1 | TODO | 0.25d | Benchmark |
| 6.2 | Implement viewport-culling for multiline text rendering | P2 | TODO | 0.5d | Drawing.cs |
| 6.3 | Cache line heights (same font = same height) | P2 | TODO | 0.25d | Helpers |
| 6.4 | Merge delayedUpdate + incrementalSearch timers | P2 | TODO | 0.25d | Core.cs |
| 6.5 | Batch Invalidate with SuspendLayout/ResumeLayout | P2 | TODO | 0.25d | Methods.cs |
| 6.6 | Reuse BeepImage instances for trailing icons | P2 | TODO | 0.25d | Drawing.cs |
| 6.7 | Unit: text input/delete/selection/caret nav | P1 | TODO | 0.5d | Tests |
| 6.8 | Unit: Ctrl+A/C/V/X/Z/Y, Home, End, arrows | P1 | TODO | 0.5d | Tests |
| 6.9 | Unit: mask format validation (10+ formats) | P1 | TODO | 0.5d | Tests |
| 6.10 | Unit: multiline + word wrap + scroll | P1 | TODO | 0.25d | Tests |
| 6.11 | Unit: undo/redo chain 50+ steps | P1 | TODO | Reserved (1.22) | Tests |
| 6.12 | Unit: search/find/replace | P1 | TODO | 0.25d | Tests |
| 6.13 | Unit: password masking + reveal toggle | P1 | TODO | 0.25d | Tests |
| 6.14 | Unit: clear button click + keyboard | P1 | TODO | 0.25d | Tests |
| 6.15 | Unit: required indicator visibility + UIA | P1 | TODO | 0.25d | Tests |
| 6.16 | Unit: error state set/clear/reset flow | P1 | TODO | Reserved (1.20) | Tests |
| 6.17 | Unit: trailing icon slot ordering | P1 | TODO | 0.25d | Tests |
| 6.18 | Unit: trailing icon hit-testing matrix | P1 | TODO | 0.25d | Tests |
| 6.19 | Unit: AutoComplete popup lifecycle (open/close/select) | P1 | TODO | 0.25d | Tests |
| 6.20 | Unit: AutoComplete caret open-trigger | P1 | TODO | 0.25d | Tests |
| 6.21 | Unit: DPI scaling (96/144/192) | P2 | TODO | 0.25d | Tests |
| 6.22 | Integration: all themes switch while text present | P1 | TODO | 0.25d | Tests |
| 6.23 | Integration: designer serialization round-trip (all new props) | P1 | TODO | 0.25d | Tests |
| 6.24 | Integration: data binding (BoundProperty=Text) + error | P1 | TODO | 0.25d | Tests |
| 6.25 | Integration: form load/unload lifecycle (timers stop, no leaks) | P1 | TODO | 0.25d | Tests |
| 6.26 | Integration: AutoComplete + context menu co-existence | P1 | TODO | 0.25d | Tests |
| 6.27 | Integration: AutoComplete + search co-existence | P1 | TODO | 0.25d | Tests |
| 6.28 | Integration: IME + autocomplete co-existence | P2 | TODO | 0.25d | Tests |

## Quick Wins

| ID | Task | Pri | Status | Est | Notes |
|----|------|-----|--------|-----|-------|
| Q1 | Add ControlStyle enum (Filled/Outlined/Standard) for MD3 variant swap | P2 | TODO | 0.25d | Properties + Drawing |
| Q2 | Double-click word selection (select word under cursor) | P2 | TODO | 0.25d | Events.cs |
| Q3 | Triple-click line selection (select entire line) | P2 | TODO | 0.25d | Events.cs |
| Q4 | Placeholder text fade-out on focus (animate to transparent) | P3 | TODO | 0.25d | Drawing.cs |
| Q5 | Add MaxLines property for multiline (cap visible lines) | P2 | TODO | 0.25d | Properties + Methods |
| Q6 | Improve AutoComplete popup theme consistency (match textbox theme) | P2 | TODO | 0.25d | SmartAutoCompleteHelper |
| Q7 | AutoComplete popup show match count header ("3 matches") | P2 | TODO | 0.25d | BeepPopupListForm |
| Q8 | Preserve input text history (last N values) via Ctrl+Up/Down | P3 | TODO | 0.25d | New HistoryHelper |

---

## AutoComplete Integration Checklist

These are cross-cutting concerns spanning multiple tasks above:

| Check | Task Refs | Description |
|-------|-----------|-------------|
| AC-1 | 1.13 | Hide popup before context menu opens |
| AC-2 | 1.17 | Re-trigger after undo/redo |
| AC-3 | 3.8 | Hide popup during loading state |
| AC-4 | 3.9-3.11 | Caret dropdown trigger + rotation animation |
| AC-5 | 4.12 | Spell check does not trigger autocomplete |
| AC-6 | 6.26 | AutoComplete + context menu co-existence test |
| AC-7 | 6.27 | AutoComplete + search co-existence test |
| AC-8 | 6.28 | IME + autocomplete co-existence test |
| AC-9 | Q6 | Theme consistency between popup and textbox |
| AC-10 | Q7 | Show match count in popup header |
| AC-11 | 5.16 | Narrator reads autocomplete suggestions |

---

## Trailing Icon Zone Slot Ordering

```
Input text |--(leading image)--| | editable text | |-- star  error  eye  X  caret  spinner --| | border
```

Icon visibility rules table:
| Icon | Slot | Visible when |
|------|------|-------------|
| Required star | 1st | IsRequired && !IsError && !IsLoading |
| Error circle-x | 2nd | IsError && ErrorIconVisible && !IsLoading |
| Password eye | 3rd | ShowPasswordReveal && (UseSystemPasswordChar or PasswordChar != '\0') && !IsLoading |
| Clear X | 4th | ShowClearButton && text.Length > 0 && !ReadOnly && !IsLoading |
| Dropdown caret | 5th | AutoCompleteMode != None && !IsLoading |
| Loading spinner | 6th | IsLoading |

Leading icons (left side): `ImageVisible` + `ImagePath` (existing).

---

## Dependency Graph

```
Phase 1 (Foundation) ──┬── Phase 2 (UX Polish) —— Phase 3 (Adornments) ── Phase 6 (Perf/Test)
                       │         (depends on P1-W4 only)
                       ├── Phase 4 (I18n) ─────────────────────────────── Phase 6
                       │
                       └── Phase 5 (Accessibility) ────────────────────── Phase 6
```

---

## Progress Summary

| Phase | Tasks | Done | In Progress | Blocked |
|-------|-------|------|-------------|---------|
| Phase 1 | 22 | 0 | 0 | 0 |
| Phase 2 | 28 | 0 | 0 | 0 |
| Phase 3 | 17 | 0 | 0 | 0 |
| Phase 4 | 17 | 0 | 0 | 0 |
| Phase 5 | 18 | 0 | 0 | 0 |
| Phase 6 | 28 | 0 | 0 | 0 |
| Quick Wins | 8 | 0 | 0 | 0 |
| **Total** | **138** | **0** | **0** | **0** |

Last Updated: 2026-06-06
