# BeepListBox Variant Compatibility Matrix

This matrix keeps all `ListBoxType` values distinct while documenting shared behavior guarantees after the modernization pass.

## Core Compatibility Guarantees

- No `ListBoxType` values removed or merged.
- Existing painter mapping remains one-to-one.
- Existing designer/runtime surfaces continue to work:
  - `BeepPopupListForm`
  - `BeepChipListBox`
  - `BeepRadioListBox`
  - `BeepCommandPaletteDialog`

## Behavior Matrix

| Variant Group | Types | Search | Checkbox | Grouping | Row Preset |
|---|---|---|---|---|---|
| Base | `Standard`, `Outlined`, `Rounded`, `Filled`, `Borderless` | Yes | Yes | Yes | `TitleOnly` |
| Data dense | `Compact`, `SimpleList`, `FilterStatus` | Yes | No | Conditional | `LeadingIconTrailingMeta` |
| Multi-select | `CheckboxList`, `OutlinedCheckboxes`, `RaisedCheckboxes`, `MultiSelectionTeal`, `CategoryChips`, `ChipStyle` | Yes | Yes | Optional | `CheckboxDescription` |
| Rich profile | `TeamMembers`, `AvatarList`, `Timeline`, `CardList`, `GradientCard`, `Glassmorphism`, `Neumorphic` | Yes | Optional | Yes | `AvatarSecondaryAction` / `TitleSubtext` |
| Command and nav | `CommandList`, `NavigationRail` | Command: Yes, Rail: No | No | Optional | `LeadingIconTrailingMeta` / rail-specific |
| Specialized | `LanguageSelector`, `RadioSelection`, `ErrorStates`, `InfiniteScroll`, `Custom` | Yes | Optional | Optional | Variant-defined |

## Regression Checklist

- Keyboard:
  - Arrow/Home/End/Page keys move focus correctly.
  - `SelectionFollowsFocus` behavior is correct in single-select mode.
  - `SpaceTogglesSelectionInMulti` and `EnterInvokesPrimaryAction` toggles behave as configured.
- Search:
  - `Contains`, `StartsWith`, `Fuzzy` filtering modes return expected results.
  - Highlighting appears only when `HighlightSearchMatches` is enabled and query is non-empty.
- Grouping:
  - Group headers render and toggle collapse/expand.
  - `GroupCollapsed` and `GroupExpanded` events are raised correctly.
  - Collapsed groups stay collapsed when `PersistCollapsedGroups` is enabled.
- Virtualization and scrolling:
  - Very large lists scroll smoothly.
  - Scrollbar ranges reflect full content height.
  - Visible rows are clipped to viewport without drawing artifacts.
- Visual and compatibility:
  - Trailing metadata and subtext row presets render consistently.
  - Existing wrappers and forms continue to function without API changes.
