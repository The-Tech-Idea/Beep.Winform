# Phase 8 — Enhance All Remaining Painters

**Priority**: Medium  
**Status**: Not Started  
**Depends on**: Phase 7 (base helpers `Scale()`, `DrawCircularAvatar()`, `GetInitials()`, tokens)

## Problem

Of 47 painters, only **10 are fully enhanced** (GOOD). The remaining **35 PARTIAL + 2 NEEDS_WORK** painters have a mix of:
- Hardcoded magic pixel values (no DPI scaling)
- Missing `ListBoxTokens` usage
- Missing `Scale()` helper usage
- Missing `DrawItemBackgroundEx()` calls
- Non-scaled `GetPreferredItemHeight()` returns
- Direct `BeepStyling.CurrentTheme` references instead of `_theme`
- One painter (`BorderlessListBoxPainter`) bypasses `BaseListBoxPainter` entirely

---

## Audit Summary

### GOOD — No changes needed (10)

| Painter | Notes |
|---------|-------|
| `ChatListBoxPainter` | Phase 7 — tokens, Scale(), avatar helper |
| `CheckboxListPainter` | Tokens, Scale(), DrawItemBackgroundEx |
| `CommandListBoxPainter` | Tokens, Scale(), DrawItemBackgroundEx |
| `ContactListBoxPainter` | Phase 7 — tokens, Scale(), avatar helper |
| `InfiniteScrollListBoxPainter` | Tokens, Scale() |
| `NavigationRailListBoxPainter` | Tokens, Scale(), DrawItemBackgroundEx |
| `NotificationListBoxPainter` | Phase 7 — tokens, Scale(), avatar helper |
| `ProfileCardListBoxPainter` | Phase 7 — tokens, Scale(), avatar helper |
| `StandardListBoxPainter` | Tokens, Scale(), DrawItemBackgroundEx |
| `ThreeLineListBoxPainter` | Phase 7 — tokens, Scale(), avatar helper |

### NEEDS_WORK — Structural problems (2)

| Painter | Issue |
|---------|-------|
| `BorderlessListBoxPainter` | Implements `IListBoxPainter` directly, bypasses all base infrastructure. Own Paint loop, own DrawItemText, hardcoded 36px rows. |
| `CategoryChipsPainter` | No `DrawItem` override; only overrides `DrawSearchArea` with hardcoded chip sizes. |

### PARTIAL — Enhancement needed (35)

Each needs a subset of the standard enhancement checklist (see below).

---

## Inheritance Hierarchy

Understanding the chain is critical — fixing a parent fixes behavior for all children.

```
IListBoxPainter
├── BaseListBoxPainter (abstract)
│   ├─ StandardListBoxPainter ✅ GOOD
│   │   ├─ OutlinedListBoxPainter ⚠ PARTIAL
│   │   │   ├─ CategoryChipsPainter ❌ NEEDS_WORK
│   │   │   ├─ SearchableListPainter ⚠ PARTIAL
│   │   │   ├─ MaterialOutlinedListBoxPainter ⚠ PARTIAL
│   │   │   └─ WithIconsListBoxPainter ⚠ PARTIAL
│   │   │       └─ LanguageSelectorPainter ⚠ PARTIAL
│   │   ├─ MinimalListBoxPainter ⚠ PARTIAL
│   │   │   └─ SimpleListPainter ⚠ PARTIAL
│   │   └─ CompactListPainter ⚠ PARTIAL
│   │
│   ├─ AvatarListBoxPainter ⚠ PARTIAL
│   ├─ CardListPainter ⚠ PARTIAL
│   ├─ ChakraUIListBoxPainter ⚠ PARTIAL
│   ├─ ChipStyleListBoxPainter ⚠ PARTIAL
│   ├─ ColoredSelectionPainter ⚠ PARTIAL
│   ├─ CustomListPainter ⚠ PARTIAL
│   ├─ ErrorStatesPainter ⚠ PARTIAL
│   ├─ FilledListBoxPainter ⚠ PARTIAL
│   ├─ FilledStylePainter ⚠ PARTIAL
│   ├─ FilterStatusPainter ⚠ PARTIAL
│   ├─ GlassmorphismListBoxPainter ⚠ PARTIAL
│   ├─ GradientCardListBoxPainter ⚠ PARTIAL
│   ├─ GroupedListPainter ⚠ PARTIAL
│   ├─ HeroUIListBoxPainter ⚠ PARTIAL
│   ├─ MultiSelectionTealPainter ⚠ PARTIAL
│   ├─ NeumorphicListBoxPainter ⚠ PARTIAL
│   ├─ OutlinedCheckboxesPainter ⚠ PARTIAL
│   ├─ RadioSelectionPainter ⚠ PARTIAL
│   ├─ RaisedCheckboxesPainter ⚠ PARTIAL
│   ├─ RekaUIListBoxPainter ⚠ PARTIAL
│   ├─ RoundedListBoxPainter ⚠ PARTIAL
│   ├─ TeamMembersPainter ⚠ PARTIAL
│   └─ TimelineListBoxPainter ⚠ PARTIAL
│
├── BorderlessListBoxPainter ❌ NEEDS_WORK (raw IListBoxPainter!)
```

---

## Standard Enhancement Checklist

Every painter should satisfy all of these:

- [ ] **E1 — `Scale()` for all pixel values**: Replace raw `8`, `12`, `16`, `20` etc. with `Scale(8)`, `Scale(12)`, etc.
- [ ] **E2 — Token constants**: Replace repeated magic numbers with `ListBoxTokens.*` where a matching token exists (padding, icon sizes, gaps, alphas).
- [ ] **E3 — Scaled height**: `GetPreferredItemHeight()` must return `Scale(N)` not raw `N`. Use a token constant where possible.
- [ ] **E4 — `DrawItemBackgroundEx()`**: `DrawItem()` must call `DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected)` as its first operation (handles clear, selection, hover, HC, disabled dimming).
- [ ] **E5 — `_theme` not `BeepStyling.CurrentTheme`**: All theme references must go through `_theme` field, not the static `BeepStyling.CurrentTheme`.
- [ ] **E6 — Required `using` directives**: Add `ListBoxs.Tokens`, `Helpers`, `Styling` as needed.
- [ ] **E7 — Inherit `BaseListBoxPainter`**: Must not implement `IListBoxPainter` directly (BorderlessListBoxPainter violation).

---

## Batched Implementation Plan

### Batch 1 — Structural Rewrites (2 painters)

High-risk, high-impact. Must be done first since one bypasses all base infrastructure.

#### 1.1 `BorderlessListBoxPainter.cs` — Full rewrite

**Current**: Implements `IListBoxPainter` directly. Own `Paint()` loop, own `DrawItemText()`, hardcoded 36px rows, no DPI scaling, no theme field from base.

**Target**: Inherit `BaseListBoxPainter`. Override `DrawItem()` + `DrawItemBackground()` + `GetPreferredItemHeight()`. Use `Scale()`, `DrawItemBackgroundEx()`, `DrawItemText()` from base.

**Changes**:
- Change `class BorderlessListBoxPainter : IListBoxPainter` → `class BorderlessListBoxPainter : BaseListBoxPainter`
- Remove manual `_owner`/`_theme` fields, `Initialize()`, `Paint()`, `DrawItemText()`
- Add `override DrawItem(...)` using base helpers
- Add `override GetPreferredItemHeight()` → `Scale(36)`
- Add `override DrawItemBackground(...)` with bottom-border selection style
- Apply E1–E6

#### 1.2 `CategoryChipsPainter.cs` — Add DrawItem, scale chips

**Current**: Only overrides `DrawSearchArea()`. No `DrawItem()`. Chip sizing all hardcoded (24px chips, 32px text padding, 8px gaps).

**Target**: Keep search-area chip rendering but scale all values. Verify inherits list items correctly from `OutlinedListBoxPainter` chain.

**Changes**:
- Replace all raw pixel values with `Scale()`
- Use `_theme` instead of `BeepStyling.CurrentTheme`
- Apply E1, E2, E5, E6

---

### Batch 2 — Inheritance Ancestors (3 painters)

Fix these before their children (Batch 3). Changes here cascade down.

#### 2.1 `OutlinedListBoxPainter.cs`

**Current**: `DrawItemBackground()` uses `BeepStyling.CurrentTheme` directly (3 places).

**Changes**:
- E5: Replace `Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor` → `_theme?.PrimaryColor`
- E5: Replace `BeepStyling.CurrentTheme?.AccentColor` → `_theme?.AccentColor`
- E5: Replace `BeepStyling.CurrentTheme?.BorderColor` → `_theme?.BorderColor`

#### 2.2 `MinimalListBoxPainter.cs`

**Current**: `GetPreferredItemHeight()` returns raw `28`. No `Scale()`.

**Changes**:
- E3: `return 28;` → `return Scale(28);`
- E1: hardcoded `2` in accent bar → `Scale(2)`

#### 2.3 `CompactListPainter.cs`

**Current**: Only overrides `GetPreferredItemHeight()`.

**Changes**:
- E3: Ensure height is `Scale(N)`.
- Check for any hardcoded values.

---

### Batch 3 — Thin Wrappers / Children (6 painters)

These inherit from Batch 2 painters. Many only override `DrawItemBackground` or `GetPreferredItemHeight`. Quick fixes.

| Painter | Parent | Changes |
|---------|--------|---------|
| `SimpleListPainter` | MinimalListBoxPainter | E1: `Scale(4)` for indicator width, accent bar |
| `SearchableListPainter` | OutlinedListBoxPainter | E1: Scale all search area pixel values (40, 8, 16, 12, 20, 50). E5: use `_theme` |
| `MaterialOutlinedListBoxPainter` | OutlinedListBoxPainter | E3: `return 48;` → `return Scale(48);`. E5: replace `BeepStyling.CurrentTheme` |
| `WithIconsListBoxPainter` | OutlinedListBoxPainter | E3: `return 40;` → `return Scale(40);`. E1: Scale padding (16,6,12,6) |
| `LanguageSelectorPainter` | WithIconsListBoxPainter | E5: replace `BeepStyling.CurrentTheme` (in DrawItemBackground) |
| `CategoryChipsPainter` | OutlinedListBoxPainter | (Batch 1.2 — already planned) |

---

### Batch 4 — Direct BaseListBoxPainter Children with Own DrawItem (23 painters)

These each have full `DrawItem()` overrides with hardcoded pixel values. They all need the same pattern:
1. Add `using ...Tokens;` if missing
2. Replace raw pixel literals with `Scale(N)`
3. Replace `GetPreferredItemHeight()` return with `Scale(N)`
4. Ensure `DrawItemBackgroundEx()` is called
5. Replace any `BeepStyling.CurrentTheme` with `_theme`

Ordered by complexity (simplest first):

#### Tier A — Simple (mostly just need Scale + token usings)

| # | Painter | Key Issues |
|---|---------|------------|
| 1 | `ColoredSelectionPainter` | Hardcoded 4px indicator, 8px offsets |
| 2 | `FilterStatusPainter` | Hardcoded 6px, 12px, status dot sizes |
| 3 | `ErrorStatesPainter` | Hardcoded icon indent, margins |
| 4 | `SimpleListPainter` | (Batch 3 — already planned) |
| 5 | `MultiSelectionTealPainter` | Hardcoded 8px, checkbox sizes |
| 6 | `OutlinedCheckboxesPainter` | Hardcoded 8px, 3px corner radius |
| 7 | `RaisedCheckboxesPainter` | Hardcoded 8px, 4px radius, shadows |

#### Tier B — Medium (own DrawItem + custom background painting)

| # | Painter | Key Issues |
|---|---------|------------|
| 8 | `CardListPainter` | Hardcoded 44px image, 48px offset, 8px margin, 12px radius |
| 9 | `RoundedListBoxPainter` | Hardcoded 4/2px deflation, 8px radius. Uses layout cache correctly. |
| 10 | `FilledListBoxPainter` | Hardcoded font offsets, margins |
| 11 | `FilledStylePainter` | Hardcoded avatar 40px, 12px margins, 8px radius |
| 12 | `GroupedListPainter` | Hardcoded group header heights, 8px padding, 4px radius |
| 13 | `RadioSelectionPainter` | Hardcoded radio circle sizes, 8px gaps |
| 14 | `RekaUIListBoxPainter` | Hardcoded 10px paddings, 8px radius, 6px corners |
| 15 | `CustomListPainter` | Hardcoded padding, image sizes |

#### Tier C — Complex (significant custom painting logic)

| # | Painter | Key Issues |
|---|---------|------------|
| 16 | `AvatarListBoxPainter` | Hardcoded 40px avatar, 12px gap, 10/30px text Y offsets, 12px dot. Already uses StyledImagePainter.PaintInCircle but has own DrawInitialsAvatar + GetInitials (duplicates base). |
| 17 | `TeamMembersPainter` | Hardcoded 36px avatar, 16px margin, 12px gap. Uses StyledImagePainter.PaintInCircle. |
| 18 | `ChipStyleListBoxPainter` | Complex chip layout with wrapping. Hardcoded 28px height, 12px radius, 6px padding. Uses StyledImagePainter.PaintInCircle. |
| 19 | `GlassmorphismListBoxPainter` | Complex glass effects, custom checkboxes. Hardcoded 20/44px heights, 12/8px radius. Missing DrawItemBackgroundEx. |
| 20 | `GradientCardListBoxPainter` | Gradient cards with circular icons. Hardcoded 80px height, 16px margin, 8px gap, 36px icon. Missing DrawItemBackgroundEx. |
| 21 | `NeumorphicListBoxPainter` | Complex neumorphic shadow effects. Hardcoded margins, radii. Missing DrawItemBackgroundEx. |
| 22 | `HeroUIListBoxPainter` | Complex layout with hero sections. Hardcoded spacing throughout. |
| 23 | `ChakraUIListBoxPainter` | Complex Chakra-style layout. Hardcoded 8px margins, 28px icons, 6px radius. |
| 24 | `TimelineListBoxPainter` | Timeline vertical line + nodes. Hardcoded node sizes, line positions. Missing DrawItemBackgroundEx. |

---

## Per-Painter Change Specification

### Batch 4 Tier A: Detailed

**`ColoredSelectionPainter`** — E1, E3
```
- GetPreferredItemHeight(): return N; → return Scale(N);
- DrawItem: new Rectangle(..., 4, ...) → Scale(4) for indicator width
- All hardcoded 8 → Scale(8)
```

**`FilterStatusPainter`** — E1, E3
```
- Scale all pixel values in status dot drawing
- Height: return Scale(N)
```

**`ErrorStatesPainter`** — E1, E3
```
- Scale error icon indent, margins
- Height: return Scale(N)
```

**`MultiSelectionTealPainter`** — E1, E3
```
- Scale checkbox sizes, 8px gaps
- Height: return Scale(N)
```

**`OutlinedCheckboxesPainter`** — E1, E3
```
- Scale 8px offsets, 3px corner radius
- Height: return Scale(N)
```

**`RaisedCheckboxesPainter`** — E1, E3
```
- Scale shadow offsets, checkbox sizes, 4px radius
- Height: return Scale(N)
```

### Batch 4 Tier B: Detailed

**`CardListPainter`** — E1, E2, E3
```
- 44px image → Scale(44), 48px offset → Scale(48)
- 8px margin → Scale(8), 12px radius → Scale(12)
- Height: return Scale(N)
```

**`RoundedListBoxPainter`** — E1, E3
```
- Deflation: -4, -2 → -Scale(4), -Scale(2)
- ItemRadius 8 → Scale(8)
- Height: (inherits base, check if overridden)
```

**`FilledListBoxPainter`** — E1, E3
```
- Scale all text Y offsets, padding values
- Height: return Scale(N)
```

**`FilledStylePainter`** — E1, E3
```
- 40px avatar → Scale(40), 12px margins → Scale(12)
- 8px radius → Scale(8)
- Height: return Scale(N)
```

**`GroupedListPainter`** — E1, E3, E2
```
- 8px padding → Scale(8), group header 28px → Scale(28)
- 4px radius → Scale(4)
```

**`RadioSelectionPainter`** — E1, E3
```
- Scale radio circle outer/inner sizes, gap values
- Height: return Scale(N)
```

**`RekaUIListBoxPainter`** — E1, E3
```
- 10px padding → Scale(10), 8px radius → Scale(8)
- Height: return Scale(N)
```

**`CustomListPainter`** — E1, E3
```
- Scale padding, image sizes
- Height: return Scale(N)
```

### Batch 4 Tier C: Detailed

**`AvatarListBoxPainter`** — E1, E2, E3, dedup
```
- 40px avatar → Scale(ListBoxTokens.AvatarSize) or Scale(40)
- 12px gap → Scale(12), 10/30px text Y → Scale(10)/Scale(30)
- 12px status dot → Scale(12)
- Remove private GetInitials() (now in base)
- Height: return Scale(56) (currently Math.Max with font height)
```

**`TeamMembersPainter`** — E1, E3
```
- 36px avatar → Scale(36), 16px margin → Scale(16), 12px gap → Scale(12)
- Height: return Scale(N)
```

**`ChipStyleListBoxPainter`** — E1, E3
```
- 28px chip height → Scale(28), 12px radius → Scale(12)
- 6px padding → Scale(6)
- Height: return Scale(N)
```

**`GlassmorphismListBoxPainter`** — E1, E3, E4
```
- Add DrawItemBackgroundEx call in DrawItem (currently calls DrawGlassBackground directly)
- 44px height → Scale(44), 12/8px radius → Scale values
- Height: Scale the return value
```

**`GradientCardListBoxPainter`** — E1, E3, E4
```
- Add DrawItemBackgroundEx (currently no background clearing)
- 80px/16px/8px/36px → Scale all
- Height: return Scale(80)
```

**`NeumorphicListBoxPainter`** — E1, E3, E4
```
- Add DrawItemBackgroundEx
- Scale all neumorphic shadow offsets and radii
- Height: Scale return
```

**`HeroUIListBoxPainter`** — E1, E3
```
- Scale all spacing, icon sizes, margins
- Height: Scale return
```

**`ChakraUIListBoxPainter`** — E1, E3
```
- 8px margins → Scale(8), 28px icons → Scale(28), 6px radius → Scale(6)
- Height: Scale return (currently inherits base)
```

**`TimelineListBoxPainter`** — E1, E3, E4
```
- Add DrawItemBackgroundEx
- Scale timeline node radius, vertical line positions, text offsets
- Height: Scale return
```

---

## Implementation Order

```
Batch 1 — Structural rewrites         (2 painters)  ← do first
Batch 2 — Inheritance ancestors         (3 painters)  ← fixes cascade to Batch 3
Batch 3 — Thin wrappers                (5 painters)  ← quick after Batch 2
Batch 4A — Simple Scale fixes           (6 painters)  ← low risk, fast
Batch 4B — Medium (own DrawItem)        (8 painters)  ← moderate effort
Batch 4C — Complex (custom painting)    (9 painters)  ← highest effort/risk
                                        ─────────────
                                TOTAL:  33 painters
```

Note: `SimpleListPainter` appears in both Batch 3 and Tier A — count it once (Batch 3).
`CategoryChipsPainter` appears in both Batch 1.2 and Batch 3 — count it once (Batch 1.2).

---

## Verification Checklist (per painter)

- [ ] No raw pixel literals remain (except 0, 1 for pen widths, or alpha values 0-255)
- [ ] `GetPreferredItemHeight()` returns `Scale(N)`
- [ ] `DrawItem()` calls `DrawItemBackgroundEx()` as first operation
- [ ] All theme references use `_theme` not `BeepStyling.CurrentTheme`
- [ ] `using ...Tokens` present if tokens are referenced
- [ ] 0 compile errors
- [ ] Visual appearance preserved (only DPI scaling and consistency improved)

---

## Files Modified

| Batch | Files | Action |
|-------|-------|--------|
| 1 | `BorderlessListBoxPainter.cs`, `CategoryChipsPainter.cs` | Rewrite / major edit |
| 2 | `OutlinedListBoxPainter.cs`, `MinimalListBoxPainter.cs`, `CompactListPainter.cs` | Edit |
| 3 | `SimpleListPainter.cs`, `SearchableListPainter.cs`, `MaterialOutlinedListBoxPainter.cs`, `WithIconsListBoxPainter.cs`, `LanguageSelectorPainter.cs` | Edit |
| 4A | `ColoredSelectionPainter.cs`, `FilterStatusPainter.cs`, `ErrorStatesPainter.cs`, `MultiSelectionTealPainter.cs`, `OutlinedCheckboxesPainter.cs`, `RaisedCheckboxesPainter.cs` | Edit |
| 4B | `CardListPainter.cs`, `RoundedListBoxPainter.cs`, `FilledListBoxPainter.cs`, `FilledStylePainter.cs`, `GroupedListPainter.cs`, `RadioSelectionPainter.cs`, `RekaUIListBoxPainter.cs`, `CustomListPainter.cs` | Edit |
| 4C | `AvatarListBoxPainter.cs`, `TeamMembersPainter.cs`, `ChipStyleListBoxPainter.cs`, `GlassmorphismListBoxPainter.cs`, `GradientCardListBoxPainter.cs`, `NeumorphicListBoxPainter.cs`, `HeroUIListBoxPainter.cs`, `ChakraUIListBoxPainter.cs`, `TimelineListBoxPainter.cs` | Edit |

**Total: 33 painter files to modify**
