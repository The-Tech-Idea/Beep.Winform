# BeepDocumentHost Enhancement Plan — Modern MDI/Tabbed Document System

> **Version:** 3.0 · **Created:** 2026-04-10
> **Goal:** Transform BeepDocumentHost into a world-class MDI/tabbed document system rivaling VS Code, Visual Studio, JetBrains Rider, and Chrome
> **Compliance:** All phases MUST adhere to `.cursor/rules/mycontrolsonly.mdc` and `.cursor/skills/beep-winform/`

---

## Current State Analysis

The existing `BeepDocumentHost` (v2.0, Sprint 18) provides a solid foundation:
- 8 tab visual styles, split-view, auto-hide, float/dock
- JSON layout persistence with schema migration
- MVVM support, data binding, cross-host drag
- Designer integration, touch support, accessibility
- MRU navigation, closed-tab history, rich tooltips

### Critical Gaps vs `.cursor/rules/mycontrolsonly.mdc`

| Rule | Current Status | Action Required |
|------|---------------|-----------------|
| Inherit from `BaseControl` | Inherits `Panel` / `Control` | Phase 1 |
| Use `StyledImagePainter` | Direct `g.DrawImage` | Phase 1 |
| Use `BeepFontManager` | Inline `new Font(...)` | Phase 1 |
| Use `BackgroundPainterFactory` | Manual background painting | Phase 1 |
| Use `BorderPainterFactory` | Manual border painting | Phase 1 |
| Use `ShadowPainterFactory` | No shadow support | Phase 1 |
| Use `ControlHitTestHelper` | Manual hit-testing | Phase 1 |
| Use `BeepStyling` | Manual theme color resolution | Phase 1 |
| Painter/Partial/Helper pattern | Monolithic partials | Phase 1 |
| DPI scaling via `DpiScalingHelper` | Partial | Phase 1 |

### Feature Gaps vs Professional Products

| Feature | VS Code | Visual Studio | JetBrains | BeepDocumentHost |
|---------|---------|---------------|-----------|------------------|
| Nested split layouts | Yes | Yes | Yes | No (flat only) |
| Tab groups/rows | Yes | Yes | Yes | No |
| Vertical tab orientation | Yes | Yes | Yes | Yes |
| Pinned tabs (persistent left) | Yes | Yes | Yes | Basic |
| Multi-editor per tab | Yes | Yes | Yes | No |
| Breadcrumb navigation | Yes | Yes | Yes | No |
| Command palette | Yes | No | Yes | No |
| Minimap | Yes | No | No | No |
| Workspace management | Yes | Yes | Yes | No |
| Layout templates/presets | No | Yes | Yes | Basic |
| Cloud sync layouts | No | No | Yes | No |
| Virtual tab rendering | Yes | Yes | Yes | No |
| Tab preview on hover | Yes | Yes | Yes | Yes |
| Drag-to-split docking | Yes | Yes | Yes | Basic |
| Multi-window support | Yes | Yes | Yes | Basic |
| Keyboard power-user mode | Yes | Yes | Yes | Basic |
| Context-aware toolbars | No | Yes | Yes | No |
| Document comparison | No | Yes | Yes | No |
| Terminal integration | Yes | No | Yes | No |
| Git integration badges | Yes | No | Yes | No |

---

## Phase Overview

| Phase | Title | Priority | Complexity | Sprint |
|-------|-------|----------|------------|--------|
| [Phase 1](./phase-01-architecture-refactoring.md) | Architecture Refactoring & Beep Compliance | Critical | High | 19-20 |
| [Phase 2](./phase-02-core-mdi-enhancements.md) | Core MDI & Layout Engine Overhaul | Critical | High | 21-22 |
| [Phase 3](./phase-03-advanced-tab-system.md) | Advanced Tab System & Workspace Model | High | Medium | 23-24 |
| [Phase 4](./phase-04-professional-docking.md) | Professional Docking Framework | High | High | 25-26 |
| [Phase 5](./phase-05-performance-scalability.md) | Performance, Virtualization & Scalability | High | Medium | 27 |
| [Phase 6](./phase-06-advanced-navigation.md) | Advanced Navigation & Command System | Medium | Medium | 28 |
| [Phase 7](./phase-07-collaboration-sharing.md) | Collaboration, Templates & Cloud Sync | Medium | Low | 29 |
| [Phase 8](./phase-08-polish-accessibility.md) | Polish, Accessibility & Power User Features | Medium | Medium | 30 |

---

## Implementation Order

```
Phase 1 → Phase 2 → Phase 3 → Phase 4
                      ↓
                  Phase 5 (can parallel with 4)
                      ↓
                  Phase 6 → Phase 7 → Phase 8
```

**Critical Path:** Phases 1-2-3-4 must be sequential (architecture → layout → tabs → docking)
**Parallelizable:** Phase 5 can start after Phase 3 completes
**Independent:** Phases 6-8 depend on Phase 3+ being complete

---

## Key Design Principles

1. **Beep Framework First** — Every control inherits `BaseControl`, uses painters, DPI-aware, theme-driven
2. **Painter Architecture** — Separate rendering from logic; distinct painters per style
3. **Token-Driven Design** — All geometry, timing, colors via `DocumentHostTokens`
4. **Composition over Inheritance** — Small, focused controls composed into larger systems
5. **Virtual by Default** — Only render what's visible; lazy-load everything else
6. **Keyboard-First** — Every action accessible via keyboard; mouse is enhancement
7. **Extensible** — Plugin points for custom tab styles, docking zones, navigation modes
8. **Backward Compatible** — Existing layouts and APIs preserved with migration paths

---

## Success Metrics

| Metric | Target |
|--------|--------|
| Beep rules compliance | 100% |
| Tab styles supported | 12+ (8 existing + 4 new) |
| Split nesting depth | Unlimited (tree-based) |
| Tab rendering performance | 60 FPS with 500+ tabs |
| Layout save/restore time | <500ms for 100 documents |
| Keyboard shortcuts | 50+ power user shortcuts |
| Accessibility score | WCAG 2.1 AA compliant |
| Designer support | Full design-time experience |
| Test coverage | 80%+ unit tests |

---

## Risk Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| Phase 1 refactoring breaks existing functionality | High | Comprehensive test suite before refactoring; feature flags |
| Performance regression with BaseControl | Medium | Benchmark before/after; optimize hot paths |
| Complex nested splits cause layout bugs | High | Tree-based layout engine with validation |
| Migration from v2 to v3 layouts fails | Medium | Robust migration service with rollback |
| Designer integration complexity | Medium | Incremental designer feature rollout |

---

## File Structure (Post-Implementation)

```
DocumentHost/
├── .plan/
│   ├── overview.md
│   ├── phase-01-architecture-refactoring.md
│   ├── phase-02-core-mdi-enhancements.md
│   ├── phase-03-advanced-tab-system.md
│   ├── phase-04-professional-docking.md
│   ├── phase-05-performance-scalability.md
│   ├── phase-06-advanced-navigation.md
│   ├── phase-07-collaboration-sharing.md
│   ├── phase-08-polish-accessibility.md
│   └── todo-tracker.md
├── Core/
│   ├── BeepDocumentHost.cs
│   ├── BeepDocumentHost.*.cs
│   ├── BeepDocumentGroup.cs
│   ├── BeepDocumentPanel.cs
│   └── BeepDocumentFloatWindow.cs
├── Tabs/
│   ├── BeepDocumentTabStrip.cs
│   ├── BeepDocumentTabStrip.*.cs
│   ├── BeepDocumentTab.cs
│   ├── BeepTabRow.cs
│   └── BeepDocumentTab.*.cs
├── Layout/
│   ├── ILayoutNode.cs
│   ├── SplitLayoutNode.cs
│   ├── GroupLayoutNode.cs
│   ├── LayoutTreeBuilder.cs
│   ├── LayoutTreeApplier.cs
│   └── LayoutMigrationService.cs
├── Docking/
│   ├── BeepDocumentDockOverlay.cs
│   ├── BeepDockZone.cs
│   ├── BeepDockGuideAdorner.cs
│   └── BeepDocumentDragData.cs
├── Navigation/
│   ├── BeepDocumentQuickSwitch.cs
│   ├── BeepDocumentBreadcrumb.cs
│   ├── BeepCommandPalette.cs
│   └── BeepWorkspaceSwitcher.cs
├── Painters/
│   ├── ITabStripPainter.cs
│   ├── ChromeTabPainter.cs
│   ├── VSCodeTabPainter.cs
│   ├── FluentTabPainter.cs
│   ├── DockOverlayPainter.cs
│   └── EmptyStatePainter.cs
├── Helpers/
│   ├── DocumentHitTestHelper.cs
│   ├── DocumentLayoutManager.cs
│   ├── DocumentAnimationHelper.cs
│   └── DocumentDragHelper.cs
├── Tokens/
│   └── DocumentHostTokens.cs
├── MVVM/
│   ├── IDocumentViewModel.cs
│   └── DocumentViewModelBase.cs
├── Serialisation/
│   ├── DocumentLayoutSerializer.cs
│   └── DocumentLayoutMigrator.cs
└── Readme.md
```

---

## Notes for Implementation Agents

1. **Read `.cursor/rules/mycontrolsonly.mdc`** before writing any code
2. **Read `.cursor/skills/beep-winform/SKILL.md`** for mandatory patterns
3. **Read `.cursor/skills/beep-dpi-fonts/SKILL.md`** for font/DPI rules
4. **Read `.cursor/skills/beep-controls-usage/SKILL.md`** for control catalog
5. **Each phase is independent** — agents can work on different phases simultaneously after dependencies are met
6. **Update `todo-tracker.md`** as you complete tasks
7. **Preserve backward compatibility** — existing APIs must work or have migration paths
8. **Test thoroughly** — each phase should have unit tests before marking complete
