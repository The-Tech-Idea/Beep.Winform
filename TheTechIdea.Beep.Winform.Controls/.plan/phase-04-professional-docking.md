# Phase 4: Professional Docking Framework

> **Sprint:** 25-26 · **Priority:** High · **Complexity:** High
> **Dependency:** Phase 2 complete · **Estimated Effort:** 3-4 weeks

---

## Objective

Implement a Visual Studio-grade docking framework with 5-zone compass docking, translucent preview rectangles, cross-host drag with full document transfer, auto-hide strip enhancements, and dock state persistence. This phase transforms the basic docking into a professional-grade system.

---

## Current Limitations

| Limitation | Current State | Target State |
|------------|--------------|--------------|
| Dock zones | Basic 5-zone | 5-zone with visual compass |
| Dock preview | Translucent rect | Translucent rect with content preview |
| Dock guides | Basic adorner | Animated guide highlights |
| Cross-host drag | Basic transfer | Full transfer with preview |
| Auto-hide strips | Basic slide | Enhanced with pin, resize, multi-doc |
| Dock constraints | None | Configurable constraints |
| Dock persistence | Basic | Full dock state in layout |
| Dock-to-tab | No | Drop onto tab to create group |
| Dock-to-split | Basic | Drop onto edge with ratio preview |

---

## Tasks

### Task 4.1: Implement 5-Zone Dock Compass

**Files to Create:**

```
Docking/
├── BeepDockZone.cs                  ← Dock zone definition
├── BeepDockCompass.cs               ← Visual dock compass
├── BeepDockGuideAdorner.cs          ← Dock guide adorner
├── BeepDockPreview.cs               ← Dock preview rectangle
└── BeepDockDropTarget.cs            ← Drop target resolver
```

**`BeepDockCompass` Requirements:**

- Paint 5-zone compass overlay during drag:
  - Center (fill host)
  - Left (split vertical, dock left)
  - Right (split vertical, dock right)
  - Top (split horizontal, dock top)
  - Bottom (split horizontal, dock bottom)
- Each zone highlights on hover
- Zone sizes proportional to available space
- Theme-colored highlights
- Smooth fade-in/fade-out animation
- DPI-aware geometry

**Zone Detection Logic:**

```
┌─────────────────────────────────┐
│              Top                │
├─────────┬───────────┬───────────┤
│         │           │           │
│  Left   │  Center   │   Right   │
│         │           │           │
├─────────┴───────────┴───────────┤
│            Bottom               │
└─────────────────────────────────┘
```

- Cursor position determines active zone
- Zone boundaries scaled by DPI
- Zone highlights with `PrimaryColor` at 30% opacity
- Active zone border with `PrimaryColor` at 100%

### Task 4.2: Implement Dock Preview Rectangle

**`BeepDockPreview` Requirements:**

- Show translucent rectangle where document will dock
- Preview size matches expected result:
  - Center: full host area
  - Left/Right: split ratio preview (50/50 default)
  - Top/Bottom: split ratio preview (50/50 default)
- Preview shows document title and icon
- Preview fades in on zone enter, fades out on zone leave
- Preview updates in real-time as cursor moves
- Preview respects minimum group sizes

### Task 4.3: Implement Dock Guide Adorner

**`BeepDockGuideAdorner` Requirements:**

- Paint guide lines at split boundaries
- Guide lines highlight on hover
- Guide lines show drop target area
- Animated guide line appearance
- Theme-colored guide lines
- DPI-aware line thickness

### Task 4.4: Implement Drop Target Resolver

**`BeepDockDropTarget` Requirements:**

- Resolve cursor position to dock action:
  - `DockFill` — fill entire host
  - `DockLeft` — split vertical, dock left
  - `DockRight` — split vertical, dock right
  - `DockTop` — split horizontal, dock top
  - `DockBottom` — split horizontal, dock bottom
  - `DockTab` — add to existing tab group
  - `DockSplit` — create new split at position
- Calculate split ratio based on drop position
- Validate dock action (check constraints)
- Return dock action with all parameters

### Task 4.5: Enhance Cross-Host Drag

**Requirements:**

- Full document transfer between hosts
- Show drag preview (translucent tab following cursor)
- Show dock compass on target host
- Show dock preview on target host
- Transfer document content (not just metadata)
- Fire `DocumentDetaching` event on source
- Fire `DocumentAttaching` event on target
- Support cancel on source or target
- Support drag between any registered hosts
- Cross-process drag support (future)

### Task 4.6: Enhance Auto-Hide Strips

**Requirements:**

- Multi-document auto-hide strips (stack multiple docs on one side)
- Auto-hide strip shows document icons/titles vertically
- Click icon to slide out document
- Hover to peek (partial slide-out)
- Pin button on slide-out panel
- Auto-hide strip resize (drag edge to resize)
- Auto-hide strip position (left, right, top, bottom)
- Auto-hide state persists in layout
- Smooth slide animation (60 FPS)
- Auto-hide strip theme colors

### Task 4.7: Implement Dock Constraints

**Requirements:**

- Per-document dock constraints:
  - `AllowDockLeft`, `AllowDockRight`, `AllowDockTop`, `AllowDockBottom`, `AllowDockCenter`
  - `AllowFloat`, `AllowAutoHide`, `AllowClose`
  - `MinimumWidth`, `MinimumHeight`, `MaximumWidth`, `MaximumHeight`
- Per-host dock constraints:
  - `MaxSplitDepth` — maximum nesting depth
  - `MaxGroups` — maximum total groups
  - `AllowNestedSplits` — allow nested split nodes
  - `AllowCrossHostDrag` — allow drag from other hosts
- Constraint validation during drag
- Visual feedback when dock is not allowed (red X cursor)

### Task 4.8: Implement Dock-to-Tab

**Requirements:**

- Drop onto existing tab to add to that group
- Show tab highlight on hover
- Drop inserts tab at position
- Animated tab insertion
- Support drop between tabs (insert at position)
- Tab group expands to accommodate new tab

### Task 4.9: Implement Dock-to-Split

**Requirements:**

- Drop onto edge of existing group to create split
- Show split preview line
- Calculate split ratio based on drop position
- Create new split node in layout tree
- Move document to new group
- Animate split creation
- Support nested splits

### Task 4.10: Update Dock Serialization

**Changes to `BeepDocumentHost.Serialisation.cs`:**

- Serialize full dock state:
  - Layout tree with dock zones
  - Auto-hide strips per side
  - Auto-hide strip document order
  - Float window positions and states
  - Dock constraints per document
  - Cross-host registration state
- Schema version bump (include in v3)
- Migration from older schemas

---

## Acceptance Criteria

- [ ] 5-zone dock compass with visual highlights
- [ ] Translucent dock preview rectangle
- [ ] Animated dock guide adorner
- [ ] Drop target resolver with all dock actions
- [ ] Cross-host drag with full document transfer
- [ ] Multi-document auto-hide strips
- [ ] Auto-hide strip resize
- [ ] Auto-hide slide animation (60 FPS)
- [ ] Dock constraints (per-document and per-host)
- [ ] Dock-to-tab (add to existing group)
- [ ] Dock-to-split (create new split at position)
- [ ] Full dock state serialization
- [ ] Visual feedback for invalid docks
- [ ] All dock operations DPI-aware
- [ ] All dock operations theme-aware
- [ ] Keyboard shortcuts for dock operations

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Dock compass obscures content | Semi-transparent overlay with fade |
| Cross-host drag loses document content | Transfer content before removing from source |
| Auto-hide strip conflicts with dock zones | Auto-hide strips are separate from dock layout |
| Dock constraints prevent valid operations | Clear visual feedback when dock is blocked |
| Complex dock layouts cause performance issues | Virtual rendering for dock previews |

---

## Files Modified

| File | Change Type |
|------|-------------|
| `BeepDocumentDockOverlay.cs` | Rewrite (compass, preview, guides) |
| `BeepDocumentHost.Events.cs` | Enhance (dock event handling) |
| `BeepDocumentHost.AutoHide.cs` | Rewrite (multi-doc strips) |
| `BeepDocumentHost.Serialisation.cs` | Update (dock state) |
| `BeepDocumentDragData.cs` | Enhance (dock action data) |
| `Tokens/DocumentHostTokens.cs` | Update (dock tokens) |

## Files Created

| File | Purpose |
|------|---------|
| `Docking/BeepDockZone.cs` | Dock zone definition |
| `Docking/BeepDockCompass.cs` | Visual dock compass |
| `Docking/BeepDockGuideAdorner.cs` | Dock guide adorner |
| `Docking/BeepDockPreview.cs` | Dock preview rectangle |
| `Docking/BeepDockDropTarget.cs` | Drop target resolver |
