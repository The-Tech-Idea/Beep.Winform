# MDI Manager Modernization Plan

Goal: Implement a modern, theme-aware MDI (Multiple Document Interface) manager control similar in behavior to DevExpress DocumentManager (Tabbed View focus) using Beep UI infrastructure (themes, animations, helpers) and matching visual concepts used in `BeepDisplayContainer2` while remaining lightweight and extensible.

## 1. Core Objectives
1. Provide tabbed document hosting (primary scope).
2. Support themed rendering via `IBeepTheme` (colors, fonts, active/hover states).
3. Provide document lifecycle API: Add, Activate, Close, Enumerate.
4. Provide events mirroring DevExpress style: DocumentAdded, DocumentRemoved, ActiveDocumentChanging (cancelable), ActiveDocumentChanged.
5. Provide scrolling when tabs overflow (left/right arrows + new document button placeholder).
6. Provide extensibility points (helpers for layout, painting, animation) just like `BeepDisplayContainer2`.
7. Clean disposal of hosted controls.

## 2. Minimal Initial Feature Set (Phase 1)
- Tabbed view only (future: Tabbed, Windows, Accordion, Split, Docked).
- Mouse interactions: hover, activate, close button.
- Themed colors (inactive, active, hover, close hover).
- Animation placeholder (progress value updated – optional easing later).
- Keyboard stub (future: Ctrl+Tab cycling, close with Ctrl+F4, etc.).

## 3. Data Structures
- `MDIDocument` (analogous to `AddinTab`):
  - Id (Guid string)
  - Title
  - Content (Control)
  - CanClose
  - IsVisible
  - Rectangle TabBounds
  - bool IsCloseHovered
  - float AnimationProgress / TargetAnimationProgress

- `MDIDocumentEventArgs` : EventArgs (with Document, Cancel for Changing).

## 4. Helpers (in MDI folder)
- `MDILayoutHelper`
  - Method: `CalculateLayout(List<MDIDocument>, Rectangle headerArea, int minWidth, int maxWidth, int scrollOffset, Font font)`
  - Measures text using `TextRenderer.MeasureText`
  - Assigns `TabBounds`
  - Returns `MDILayoutResult` (NeedsScrolling, ScrollLeftRect, ScrollRightRect, NewDocRect)

- `MDIPaintHelper`
  - Accepts `IBeepTheme`
  - Method: `DrawTab(Graphics g, Rectangle bounds, string title, Font font, bool active, bool hovered, bool showClose, bool closeHovered, float anim)`
  - Visuals: rounded rect, gradient or flat depending on theme, subtle shadow under active tab.
  - Draw close glyph (x) with hover highlight.

- `MDIAnimationHelper` (Phase 1 stub)
  - Registers documents needing animation; tick method to approach target progress.
  - Optional internal timer triggered by manager; for now reused from manager's timer.

## 5. Manager Control (`MDIManager`)
- Inherit from `BeepControl` to reuse theme & painting pipeline.
- Properties:
  - `int TabHeaderHeight` (default 34)
  - `bool ShowCloseButtons` (default true)
  - `bool EnableAnimations`
  - `int TabMinWidth`, `int TabMaxWidth`
- API:
  - `MDIDocument AddDocument(Control control, string title, bool canClose = true)`
  - `bool CloseDocument(string id)` / overloads by Control / Title
  - `bool ActivateDocument(string id)`
  - `IReadOnlyList<MDIDocument> Documents`
  - `MDIDocument ActiveDocument`
- Events:
  - `EventHandler<MDIDocumentEventArgs> DocumentAdded`
  - `EventHandler<MDIDocumentEventArgs> DocumentRemoved`
  - `EventHandler<MDIDocumentEventArgs> ActiveDocumentChanging` (Cancel)
  - `EventHandler<MDIDocumentEventArgs> ActiveDocumentChanged`

## 6. Layout Flow
1. On resize / document list change -> recalc header & content rectangles.
2. Layout helper assigns tab bounds; compute scroll requirement & utility button rectangles.
3. Active document content Control is sized to content rectangle and brought to front.
4. Others hidden (Visible=false) to reduce flicker.

## 7. Painting Flow
1. Override `DrawContent(Graphics g)` from `BeepControl`.
2. Draw header background (theme panel color or gradient).
3. Iterate visible documents – paint each tab via `MDIPaintHelper`.
4. Draw scroll / new document buttons if required.
5. Optionally draw bottom border line.

## 8. Interaction Flow
- MouseMove: detect hovered tab & close glyph; update cursor; start animation (if enabled).
- MouseLeave: clear hover.
- MouseClick (Left):
  - If on close glyph -> CloseDocument
  - Else if on tab -> Activate
  - Else if on scroll buttons -> adjust scrollOffset & relayout
- Wheel (future): horizontal scroll of tabs when overflow.

## 9. Animation (Phase 1 Stub)
- Each hover sets TargetAnimationProgress = 1 else 0.
- Timer (16ms) interpolates progress with simple lerp.

## 10. Theme Integration
- On `ApplyTheme()` refresh `MDIPaintHelper` with new theme.
- Colors derived from theme: Active (ButtonBackColor), Hover (Lighten), Inactive (PanelBackColor), Text (ForeColor), Border.
- Font: theme TabFont if available else control Font.

## 11. Disposal
- Stop timer.
- Dispose each document's Control if owned (configurable later; for now always dispose).

## 12. Future Phases (Not implemented now – placeholders for extensibility)
- Multiple view styles (Tabbed, Accordion, DocumentsWindow overlay, Split, Dock Layout).
- Drag & drop tab reordering.
- Persist / restore layout.
- Context menu (Close, Close Others, Pin, Float, New Vertical Group, etc.).
- Document icons & dirty state indicators.
- Keyboard navigation (Ctrl+Tab switch, Ctrl+F4 close).
- Pinning & grouping.
- Scroll with mouse wheel and middle-click close.

## 13. Testing Strategy
- Add 10+ documents and verify scrolling and activation.
- Theme switch at runtime reflects visuals.
- Dispose manager removes child controls.
- Performance: ensure no leak by profiling activation cycles.

## 14. Implementation Order
1. Data classes (`MDIDocument`, `MDIDocumentEventArgs`).
2. Helpers (`MDILayoutHelper`, `MDILayoutResult`, `MDIPaintHelper`, `MDIAnimationHelper`).
3. Implement `MDIManager` core (fields, ctor, ApplyTheme, Add/Activate/Close, Layout, Paint, Mouse handlers, Dispose).
4. Add minimal animation.
5. Build & fix compile errors.
6. Future enhancements (separate commits).

## 15. Coding Conventions
- Keep helpers internal to assembly.
- Avoid premature optimization; clear separation of concerns.
- Lean on existing Beep theming objects; no external dependencies.

---
This plan establishes the foundation; implementation follows in accompanying helper classes and updated `MDIManager.cs`.
