# BeepTextBox External Benchmark And UX Standards

Priority: High
Status: Planning Approved
Date: 2026-06-06

## 1. Codebase Summary (9 partial files, ~3,500 LOC)

| File | Lines | Responsibility |
|------|-------|----------------|
| BeepTextBox.cs | 29 | Main class, partial routing |
| BeepTextBox.Core.cs | 298 | Fields, constructor, init, DPI, Dispose |
| BeepTextBox.Properties.cs | 978 | ~60 public properties |
| BeepTextBox.Events.cs | 234 | Timer, focus, mouse, paint, resize |
| BeepTextBox.Input.cs | 461 | Key handling, text ops, clipboard |
| BeepTextBox.Methods.cs | 433 | Layout, wrapping, validation |
| BeepTextBox.Drawing.cs | 124 | Focus animation, char count, Draw |
| BeepTextBox.Theme.cs | 111 | ApplyTheme, theme colors |
| BeepTextBox.Effects.cs | 524 | Typewriter, scramble, CRT terminal |
| BeepTextBox.Search.cs | 454 | Find/Replace with highlighting |
| BeepTextBoxDesigner.cs | 263 | Design-time smart tags |

Architecture: BaseControl owner-draw (no inner TextBox). Helper pattern with BeepSimpleTextBoxHelper.

## 2. Commercial Product Benchmark

### vs Microsoft WinForms TextBox (System.Windows.Forms)
| Feature | Stock | BeepTextBox | Gap |
|---------|-------|-------------|-----|
| Custom theming | No | Yes | STRENGTH |
| Placeholder text | No | Yes | STRENGTH |
| Character count | No | Yes | STRENGTH |
| Search/find/replace | No | Yes | STRENGTH |
| Masked input | No | Yes (10+ formats) | STRENGTH |
| Image/icon inside | No | Yes | STRENGTH |
| Line numbers | No | Yes | STRENGTH |
| Text effects (typewriter/CRT) | No | Yes | STRENGTH |
| AutoComplete dropdown | Native | Custom | NEUTRAL |
| Undo/Redo | OS-level | Custom | GAP |
| IME support | Full OS-level | None | CRITICAL GAP |
| Right-to-Left | Yes | No | GAP |
| Spell checking | OS-level | Stub only | GAP |
| Accessibility (UIA) | Full MSAA/UIA | None | CRITICAL GAP |
| Drag/drop text | Yes | No | GAP |
| Context menu | Yes | No | GAP |

### vs DevExpress / Telerik / Syncfusion
| Feature | Commercial | BeepTextBox | Gap |
|---------|------------|-------------|-----|
| Validation error UI (icon + tooltip) | Yes | No (code only) | GAP |
| Clear button (X) | Yes | No | GAP |
| Password reveal toggle (eye) | Yes | No | GAP |
| Prefix/Suffix adornments | Yes | No (image only) | GAP |
| Loading/spinner state | Yes | No | GAP |
| Multi-style text (rich) | Yes | No | GAP |
| Validation error state styling | Yes | No | GAP |
| Read-only visual distinction | Yes | Theme-only | GAP |

### vs Material Design 3 (Google)
| Pattern | Present | Notes |
|---------|---------|-------|
| Filled variant | Partial | No MD3 filled style |
| Outlined variant | Yes | Border-based |
| Label animation (shrink to top) | No | Placeholder is static |
| Supporting/Helper text | No | No helper text slot |
| Error text below | No | ValidateData returns string only |
| Leading icon | Yes | ImagePath |
| Trailing icon | Yes | Image on right |
| Prefix text | No | No prefix property |
| Suffix text | No | No suffix property |
| Density variants | No | Fixed padding |
| Required indicator (*) | No | |

### vs Fluent UI (Microsoft)
| Pattern | Present | Notes |
|---------|---------|-------|
| Underlined variant | No | |
| Clear button | No | |
| Error state styling | No | |
| Required indicator | No | |
| Description text | No | |

### vs Ant Design (Alibaba)
| Pattern | Present | Notes |
|---------|---------|-------|
| Prefix/Suffix slots | No | Image only |
| Addon before/after | No | |
| AllowClear (X button) | No | |
| Search with loading | No | Search exists but no spinner |
| Auto-size TextArea | No | Fixed height |

### vs VS Code Editor Patterns
| Pattern | Present | Notes |
|---------|---------|-------|
| Find widget | No | Search via API only |
| Multi-cursor | No | Single caret |
| Code folding | No | |
| Syntax highlighting | No | Plain text |
| Indent guides | No | |

## 3. Key Findings

### Strengths
1. Feature breadth: search/find/replace, terminal mode, CRT effects, masking, autocomplete
2. Clean partial-class architecture with helper pattern
3. DPI-aware font handling via BeepFontManager
4. Designer smart tags with quick presets
5. Terminal mode: CRT scanlines, glow, flicker, cursor styles
6. 10+ mask formats (Phone, Email, SSN, CreditCard, IP, etc.)

### Critical Gaps
1. NO ACCESSIBILITY: Zero UIA/automation. WCAG/Section 508 non-compliant.
2. NO IME SUPPORT: Cannot input CJK or composed characters.
3. NO CONTEXT MENU: Right-click copy/paste/select all missing.
4. FRAGILE UNDO/REDO: Custom impl vs OS-level EM_UNDO.
5. NO VALIDATION ERROR UI: Message string exists but no visual error state.
6. SPELL CHECK IS STUB: Property exists, no implementation.

### Moderate Gaps
1. No clear (X) button
2. No password reveal toggle
3. No prefix/suffix text slots
4. Read-only no visual distinction
5. No hover state rendering (colors exist, not used)
6. No loading/spinner state
7. No drag-drop text
8. No RTL support
9. No rich/multi-style text
10. AutoComplete UX is dated (InputBox pattern)

### Performance Concerns
1. CreateGraphics() calls in UpdateDrawingRect (Microsoft anti-pattern)
2. Text measurement per-keystroke via cascading updates
3. 4 always-on timers (delayedUpdate, animation, typing, search)
4. WordWrap called in paint path could cause flicker

## 4. UX Priorities (Figma/MD3-aligned)

| Priority | Feature | Effort | Impact |
|----------|---------|--------|--------|
| P0 | Error state visual styling | Medium | CRITICAL |
| P0 | Accessibility (UIA patterns) | High | CRITICAL |
| P1 | Clear button (X) inside control | Low | HIGH |
| P1 | Password reveal toggle | Low | HIGH |
| P1 | Hover state rendering | Low | MEDIUM |
| P1 | Context menu | Medium | HIGH |
| P1 | Read-only visual distinction | Low | MEDIUM |
| P2 | Helper/Supporting text | Medium | MEDIUM |
| P2 | Prefix/Suffix text slots | Medium | MEDIUM |
| P2 | IME composition | High | HIGH |
| P2 | Drag-drop text | Low | LOW |
| P2 | Spell check impl | High | MEDIUM |
| P3 | RTL support | Medium | LOW |
| P3 | Rich text | Very High | LOW |
| P3 | Auto-size multiline | Low | MEDIUM |
| P3 | Required indicator (*) | Low | LOW |
| P3 | Loading/skeleton state | Low | LOW |
