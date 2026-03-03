# BeepiFormPro — MDI Strategy Plan

**Author**: Copilot  
**Date**: 2026-02-27  
**Status**: ACTIVE — Scenario C Selected, inheritance revised  
**Chosen Approach**: Simulated Document Host — lightweight non-BaseControl hierarchy

---

## Architecture Decision: Why NOT BaseControl

`BaseControl` inherits from `ContainerControl` and sets aggressive `ControlStyles`:

```csharp
// BaseControl constructor
ControlStyles.ContainerControl       = true
ControlStyles.AllPaintingInWmPaint   = true   // suppresses WM_ERASEBKGND
ControlStyles.OptimizedDoubleBuffer  = true
ControlStyles.Selectable | UserMouse = true
```

It also initialises multiple heavyweight helpers on every instance:
`ControlEffectHelper`, `ControlHitTestHelper`, `ControlInputHelper`,
`ControlDataBindingHelper`, `ControlExternalDrawingHelper`, `IBaseControlPainter`.

**Problems for the document host family:**

| Issue | Detail |
|---|---|
| `AllPaintingInWmPaint` suppresses `WM_ERASEBKGND` | Can cause flicker cascade when `BeepDocumentHost` is a child of `BeepiFormPro` which also suppresses erase |
| `ContainerControl` focus handling | `ContainerControl` routes `Tab` key focus through its children — bad for `BeepDocumentTabStrip` (no logical children) |
| Heavyweight helpers | `_effects`, `_hitTest`, `_input`, `_dataBinding` — overhead for controls that just paint tabs |
| Painter strategy | `ClassicBaseControlPainter` runs on every `OnPaint` — our controls paint themselves from scratch |
| Theme wiring | `BaseControl` wires to `BeepThemesManager.ThemeChanged` inside `ApplyTheme`; document controls need the same but shouldn't carry all of BaseControl's machinery |

**Solution**: use the minimum base class per control role:

| Control | Base Class | Reason |
|---|---|---|
| `BeepDocumentTabStrip` | `Control` | Pure self-painted, no children from user, no focus needed |
| `BeepDocumentPanel` | `Panel` | Container, `AutoScroll` built in, no WM_ fighting |
| `BeepDocumentHost` | `Panel` | Container, manages two child controls, no Win32 overrides |
| `BeepDocumentFloatWindow` | `BeepiFormPro` | Full themed window when document is floated |
| `BeepDocumentTab` | `class` | Data model only, never a control |

---

## Can `BeepiFormPro` Instances Be MDI Children?

**Short answer: yes, via Scenario A (Win32 MDI) — which is independent of Scenario C.**

Scenario C creates a *simulated* tab host using `Panel`-derived controls: each "document" is a `BeepDocumentPanel` (a Panel). Content goes inside the panel. This is like VS Code — not real Win32 MDI.

If you want actual `BeepiFormPro` windows (with their full painter chrome) living inside an MDI workspace as child windows, use **Scenario A**: add `IsMdiShell = true` to a `BeepiFormPro` host. Each child is another `BeepiFormPro` with `MdiParent = host`.

**Both scenarios can coexist**: you can use Scenario A (Win32 MDI, `BeepiFormPro` children with full chrome) AND Scenario C (tab strip simulation inside those MDI children for sub-documents).

---

## 1. Executive Summary

Win32 MDI conflicts with `BeepiFormPro`'s custom `WM_NCCALCSIZE` / `WM_NCPAINT` / `WM_NCHITTEST` overrides, incompatible rounded borders, and painter-owned layout model.

**Decision**: Implement **Scenario C — Simulated Document Host** as a set of new `BaseControl`-derived components that drop into any `BeepiFormPro`'s content area without touching any painter code.

---

## 2. Why Scenario C

| Requirement | Win32 MDI | Simulated Doc Host (Scenario C) |
|---|---|---|
| All 35 painters work unchanged | ⚠️ Need `MdiClient` reposition guard | ✅ Zero painter changes |
| Full Beep theme on document chrome | ❌ Child NC area owned by Windows | ✅ 100% custom painted |
| Tab strip with close / dirty indicator | ❌ Win32 limitation | ✅ Fully custom |
| Float document to separate window | ❌ Win32 quirks | ✅ Simple `BeepiFormPro` wrapper |
| DPI + per-monitor scaling | ⚠️ `MdiClient` DPI issues | ✅ Inherits `BaseControl` DPI |
| Cross-platform readiness | ❌ Win32 only | ✅ Pure GDI+ |
| Drag-to-reorder tabs | ❌ Impossible | ✅ Possible |

---

## 3. Component Overview

```
BeepDocumentHost            (BaseControl — Container)
  ├── BeepDocumentTabStrip  (BaseControl — painted tab bar)
  │     └── BeepDocumentTab (logical per-tab data model, not a control)
  └── BeepDocumentPanel     (BaseControl — active document surface)
        └── [User content added as child controls here]
```

**Interaction flow:**

```
User clicks tab in BeepDocumentTabStrip
  → BeepDocumentHost.SetActiveDocument(panel)
    → old panel.Visible = false
    → new panel.Bounds = ContentPanelRect
    → new panel.Visible = true
    → BeepDocumentTabStrip.Invalidate()
```

---

## 4. Detailed Class Specifications

---

### 4.1 `BeepDocumentTab` — Tab Data Model (not a control)

**Namespace**: `TheTechIdea.Beep.Winform.Controls.DocumentHost`  
**File**: `BeepDocumentTab.cs`  
**Base**: plain `class` (painted by `BeepDocumentTabStrip`)

```
Properties:
  string  Id              — unique identifier (GUID string)
  string  Title           — tab label text
  string  IconPath        — image path (Beep convention: string path, never Bitmap)
  bool    IsModified      — shows dirty-dot (●) before title
  bool    CanClose        — shows × button in tab
  bool    IsPinned        — pinned tabs cannot be reordered or closed
  bool    IsActive        — currently selected tab
  Color   AccentColor     — optional per-tab accent override (Color.Empty = use theme)
  string  TooltipText     — tooltip shown on hover
  object  Tag             — user data slot

Computed (set by BeepDocumentTabStrip during layout):
  Rectangle TabRect       — bounding rectangle in strip coords
  Rectangle CloseRect     — close button hot zone
  Rectangle DirtyRect     — dirty-dot location
  Rectangle IconRect      — icon bounds
  Rectangle TitleRect     — title text bounds
```

---

### 4.2 `BeepDocumentTabStrip` — Painted Tab Bar

**Namespace**: `TheTechIdea.Beep.Winform.Controls.DocumentHost`  
**File**: `BeepDocumentTabStrip.cs`  
**Base**: `Control` — minimal, no BaseControl overhead, no focus/container behavior

#### 4.2.1 Properties

```
Category "Document Tabs":
  IReadOnlyList<BeepDocumentTab> Tabs            — read-only; managed internally
  BeepDocumentTab ActiveTab                      — currently selected tab
  int TabHeight                                  — default 32; DPI-scaled
  int TabMinWidth                                — default 80
  int TabMaxWidth                                — default 200
  bool ShowAddButton                             — shows [+] at end of tab row
  bool AllowReorder                              — drag-to-reorder tabs
  bool AllowOverflow                             — show overflow ›› button when tabs don't fit
  TabCloseMode CloseMode                         — Always, OnHover, Never
  TabStripPosition Position                      — Top (default), Bottom

Category "Appearance":
  Color ActiveTabBackground                      — from theme.PrimaryColor if empty
  Color InactiveTabBackground                    — from theme.PanelColor if empty
  Color ActiveTabForeground                      — from theme.TextColor if empty
  Color InactiveTabForeground                    — from theme.SecondaryTextColor if empty
  Color StripBackground                          — from theme.PanelColor if empty
  Color TabBorderColor                           — from theme.BorderColor if empty
  int   TabCornerRadius                          — default 6
  bool  ShowTabSeparators                        — vertical dividers between inactive tabs
  Font  TabFont                                  — from theme.SmallStyle if null
  Font  ActiveTabFont                            — bold version / theme.TitleStyle if null
```

#### 4.2.2 Events

```
event EventHandler<TabEventArgs>   TabSelected         — user clicked a tab
event EventHandler<TabEventArgs>   TabCloseRequested   — user clicked ×
event EventHandler<TabEventArgs>   TabPinToggled       — context menu pin
event EventHandler<TabReorderArgs> TabReordered        — drag complete
event EventHandler                 AddButtonClicked    — [+] clicked
event EventHandler<TabEventArgs>   TabDoubleClicked    — float request
```

#### 4.2.3 Internal Fields

```csharp
private List<BeepDocumentTab> _tabs        = new();
private BeepDocumentTab _hoveredTab;       // tab under mouse
private BeepDocumentTab _pressedTab;       // mouse-down tab
private BeepDocumentTab _dragTab;          // tab being dragged
private int _dragStartX;
private int _scrollOffset;                 // horizontal px when tabs overflow
private bool _overflowVisible;
private Rectangle _addButtonRect;
private Rectangle _overflowButtonRect;
```

#### 4.2.4 Layout Calculation

```
Per-layout pass (triggered on Resize, tab add/remove, font change):

  availableWidth = ClientWidth - addButtonWidth - overflowButtonWidth
  tabWidth = Clamp( availableWidth / Count, TabMinWidth, TabMaxWidth )
  
  For each tab i:
    tab.TabRect   = new Rectangle(i * tabWidth - _scrollOffset, 0, tabWidth, TabHeight)
    tab.IconRect  = left zone of tab (iconSize × iconSize, centred vertically)
    tab.TitleRect = tab.TabRect inset by icon+padding left, close+padding right
    tab.CloseRect = right zone (closeSize × closeSize, centred vertically)
    tab.DirtyRect = small circle left of TitleText when IsModified
    
  _overflowVisible    = any tab.TabRect.Right > availableWidth
  _addButtonRect      = Rectangle after last visible tab
  _overflowButtonRect = far right when _overflowVisible
```

#### 4.2.5 OnPaint

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    if (Width <= 0 || Height <= 0) return;
    var g = e.Graphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;

    // 1. Strip background
    DrawStripBackground(g);

    // 2. Bottom separator line (1px)
    DrawBottomSeparator(g);

    // 3. Inactive tabs (back to front)
    foreach (var tab in _tabs.Where(t => !t.IsActive && IsTabVisible(t)))
        DrawTab(g, tab, false);

    // 4. Active tab last (paints over separator)
    if (ActiveTab != null && IsTabVisible(ActiveTab))
        DrawTab(g, ActiveTab, true);

    // 5. Overflow button
    if (_overflowVisible) DrawOverflowButton(g);

    // 6. Add button
    if (ShowAddButton) DrawAddButton(g);
}

private void DrawTab(Graphics g, BeepDocumentTab tab, bool isActive)
{
    var theme   = _currentTheme;
    var bgColor = isActive
        ? (ActiveTabBackground   != Color.Empty ? ActiveTabBackground   : _theme.PrimaryColor)
        : (InactiveTabBackground != Color.Empty ? InactiveTabBackground : _theme.PanelColor);
    var fgColor = isActive
        ? (ActiveTabForeground   != Color.Empty ? ActiveTabForeground   : _theme.TextColor)
        : (InactiveTabForeground != Color.Empty ? InactiveTabForeground : _theme.SecondaryTextColor);
    var rect = tab.TabRect;

    // — Background with rounded top corners (bottom edge straight = connects to content) —
    using var path    = BuildTabPath(rect, TabCornerRadius, isActive);
    using var bgBrush = new SolidBrush(bgColor);
    g.FillPath(bgBrush, path);

    // — Top accent line on active tab —
    if (isActive)
    {
        var accentCol = tab.AccentColor != Color.Empty ? tab.AccentColor : _theme.PrimaryColor;
        using var accentPen = new Pen(accentCol, 2);
        g.DrawLine(accentPen,
            rect.Left  + TabCornerRadius, rect.Top + 1,
            rect.Right - TabCornerRadius, rect.Top + 1);
    }

    // — Icon —
    if (!string.IsNullOrEmpty(tab.IconPath))
        StyledImagePainter.DrawImage(g, tab.IconPath, tab.IconRect, _theme, isActive ? 1f : 0.6f);

    // — Dirty dot —
    if (tab.IsModified)
        DrawDirtyDot(g, tab.DirtyRect, _theme.WarningColor);

    // — Title text —
    var font = isActive
        ? (ActiveTabFont ?? BeepThemesManager.ToFont(_theme.TitleStyle))
        : (TabFont       ?? BeepThemesManager.ToFont(_theme.SmallStyle));
    TextRenderer.DrawText(g, tab.Title, font, tab.TitleRect, fgColor,
        TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

    // — Close button (hover-sensitive) —
    if (tab.CanClose && (CloseMode == TabCloseMode.Always || tab == _hoveredTab))
        DrawCloseButton(g, tab.CloseRect, tab == _hoveredTab, _theme);

    // — Tab separator —
    if (ShowTabSeparators && !isActive)
    {
        using var sepPen = new Pen(_theme.BorderColor, 1);
        g.DrawLine(sepPen, rect.Right - 1, rect.Top + 4, rect.Right - 1, rect.Bottom - 4);
    }
}
```

#### 4.2.6 Mouse Handling

```
OnMouseMove  → update _hoveredTab; Invalidate if changed; update drag ghost if dragging
OnMouseDown  → set _pressedTab; begin drag timer if AllowReorder
OnMouseUp    → if no drag + same tab as _pressedTab → fire TabSelected
               close zone → fire TabCloseRequested
               drag complete → rearrange _tabs, fire TabReordered
OnMouseLeave → clear _hoveredTab, Invalidate
OnMouseClick → AddButton → fire AddButtonClicked
               Overflow   → show BeepDocumentOverflow popup
```

---

### 4.3 `BeepDocumentPanel` — Document Content Surface

**Namespace**: `TheTechIdea.Beep.Winform.Controls.DocumentHost`  
**File**: `BeepDocumentPanel.cs`  
**Base**: `Panel` — simple container, `AutoScroll` built in, no Win32 paint overrides

#### 4.3.1 Properties

```
Category "Document":
  string  DocumentTitle       — displayed in tab; setter raises TitleChanged
  string  DocumentIconPath    — tab icon path
  bool    IsModified          — dirty indicator in tab; setter raises ModifiedChanged
  bool    CanClose            — controls tab × visibility
  bool    IsPinned            — forwarded to BeepDocumentTab
  Color   AccentColor         — per-document theme accent
  string  DocumentId          — read-only GUID string (set at construction)
  object  DocumentTag         — user data

Category "Appearance":
  bool    ShowDocumentBorder  — draws thin inset border inside panel
  bool    ShowStatusBar       — optional 24px bottom status strip

Category "Status Bar":
  string  StatusText          — text shown in bottom status bar
  string  StatusIconPath      — icon in status bar (string path)
  Color   StatusBarBackground — from theme.PanelColor if empty
```

#### 4.3.2 Events

```
event EventHandler TitleChanged
event EventHandler ModifiedChanged
event EventHandler CloseRequested
event EventHandler Activated     — raised when panel becomes active document
event EventHandler Deactivated   — raised when panel is hidden
```

#### 4.3.3 OnPaint

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    base.OnPaint(e); // Panel handles background (BackColor fill)
    if (ShowDocumentBorder) DrawDocumentBorder(e.Graphics);
    if (ShowStatusBar)      DrawStatusBar(e.Graphics);
}

private void DrawDocumentBorder(Graphics g)
{
    var edgeRect = new Rectangle(0, 0, Width - 1,
                          Height - (ShowStatusBar ? _statusBarHeight : 1));
    using var pen = new Pen(_theme?.BorderColor ?? Color.Gray, 1);
    g.DrawRectangle(pen, edgeRect);
}

private void DrawStatusBar(Graphics g)
{
    var sbRect = new Rectangle(0, Height - _statusBarHeight, Width, _statusBarHeight);
    using var bg  = new SolidBrush(_theme?.PanelColor ?? SystemColors.Control);
    g.FillRectangle(bg, sbRect);
    using var sep = new Pen(_theme?.BorderColor ?? Color.Gray);
    g.DrawLine(sep, sbRect.Left, sbRect.Top, sbRect.Right, sbRect.Top);

    int textX = sbRect.Left + 4;
    if (!string.IsNullOrEmpty(StatusIconPath))
    {
        var iconRect = new Rectangle(sbRect.Left + 4, sbRect.Top + 4, 16, 16);
        StyledImagePainter.DrawImage(g, StatusIconPath, iconRect, _theme);
        textX = iconRect.Right + 4;
    }
    TextRenderer.DrawText(g, StatusText,
        BeepThemesManager.ToFont(_theme?.SmallStyle),
        new Rectangle(textX, sbRect.Top, sbRect.Width - textX, sbRect.Height),
        _theme?.SecondaryTextColor ?? SystemColors.GrayText,
        TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
}
```

#### 4.3.4 Layout

- `AutoScroll = true` — default scrollable document area.
- When `ShowStatusBar = true`, subtract `_statusBarHeight` (24px, DPI-scaled) from scroll area via `AutoScrollMinSize` or override `GetScrollBarInfo`.
- `IsContainerControl` override returns `true` so `BaseControl` excludes child bounds from background painting.

---

### 4.4 `BeepDocumentHost` — Orchestrating Container

**Namespace**: `TheTechIdea.Beep.Winform.Controls.DocumentHost`  
**File**: `BeepDocumentHost.cs`  
**Base**: `Panel` — simple container, no Win32 message overrides

#### 4.4.1 Properties

```
Category "Document Host":
  IReadOnlyList<BeepDocumentPanel> Documents   — all open panels (read-only)
  BeepDocumentPanel ActiveDocument             — currently shown panel
  bool ShowEmptyState                          — paints empty-state when no docs open
  string EmptyStateMessage                     — default "No documents open"
  string EmptyStateIconPath                    — large center icon on empty state
  bool AllowFloating                           — double-click tab floats panel
  TabStripPosition TabPosition                 — Top (default), Bottom, Hidden

Category "Layout":
  Padding DocumentPadding                      — insets around document panels
```

#### 4.4.2 Events

```
event EventHandler<DocumentEventArgs>  DocumentAdded
event EventHandler<DocumentEventArgs>  DocumentRemoved
event EventHandler<DocumentEventArgs>  DocumentActivated
event EventHandler<DocumentEventArgs>  DocumentFloated
event EventHandler<DocumentEventArgs>  DocumentCloseRequested
event EventHandler                     AllDocumentsClosed
```

#### 4.4.3 Internal Layout

```
Fields:
  BeepDocumentTabStrip   _tabStrip
  List<BeepDocumentPanel> _panels
  BeepDocumentPanel       _activeDocument

RecalculateLayout():
  tabH = (TabPosition != Hidden) ? _tabStrip.TabHeight : 0
  tabY = (TabPosition == Bottom) ? ClientHeight - tabH : 0
  docY = (TabPosition == Top)    ? tabH               : 0
  docH = ClientHeight - tabH

  _tabStrip.Bounds = new Rectangle(0, tabY, ClientWidth, tabH)
  _tabStrip.Visible = (TabPosition != Hidden)

  var docBounds = new Rectangle(
      DocumentPadding.Left,
      docY + DocumentPadding.Top,
      ClientWidth  - DocumentPadding.Horizontal,
      docH - DocumentPadding.Vertical)

  foreach (var p in _panels)
  {
      p.Bounds  = docBounds;
      p.Visible = (p == _activeDocument);
  }
```

#### 4.4.4 Public API

```csharp
// Add a pre-built panel
void AddDocument(BeepDocumentPanel panel, bool activate = true);

// Create a blank panel, add to host, and return it
BeepDocumentPanel NewDocument(string title, string iconPath = null);

// Activate (bring to front)
void SetActiveDocument(BeepDocumentPanel panel);
void SetActiveDocument(string documentId);

// Close workflow
void RequestClose(BeepDocumentPanel panel);   // fires DocumentCloseRequested only
void Remove(BeepDocumentPanel panel);          // actually removes + disposes tab

// Float panel into its own BeepiFormPro window
BeepDocumentFloatWindow Float(BeepDocumentPanel panel);

// Re-dock a floating panel (called when BeepDocumentFloatWindow fires DockedBack)
void DockBack(BeepDocumentPanel panel);

// Internal wirings (called by BeepDocumentTabStrip callbacks)
private void OnTabSelected(object sender, TabEventArgs e);
private void OnTabCloseRequested(object sender, TabEventArgs e);
private void OnTabReordered(object sender, TabReorderArgs e);
private void OnTabDoubleClicked(object sender, TabEventArgs e); // → Float if AllowFloating
private void OnAddButtonClicked(object sender, EventArgs e);    // → NewDocument
```

#### 4.4.5 Tab Synchronisation

Each `BeepDocumentPanel` owns its data; `BeepDocumentHost` owns the corresponding `BeepDocumentTab` inside `_tabStrip`. The host wires these property-change events on every panel it manages:

```csharp
private void WirePanel(BeepDocumentPanel panel)
{
    panel.TitleChanged    += (s, e) => SyncTab(panel);
    panel.ModifiedChanged += (s, e) => SyncTab(panel);
}

private void SyncTab(BeepDocumentPanel panel)
{
    var tab = _tabStrip.Tabs.FirstOrDefault(t => t.Id == panel.DocumentId);
    if (tab == null) return;
    tab.Title      = panel.DocumentTitle;
    tab.IsModified = panel.IsModified;
    tab.IconPath   = panel.DocumentIconPath;
    _tabStrip.Invalidate();
}
```

#### 4.4.6 OnPaint — Empty State

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    base.OnPaint(e); // Panel fills BackColor
    if (_panels.Count == 0 && ShowEmptyState)
        DrawEmptyState(e.Graphics);
}

private void DrawEmptyState(Graphics g)
{
    var center = new Point(ClientWidth / 2, ClientHeight / 2
        + (TabPosition == TabStripPosition.Hidden ? 0 : _tabStrip.Height / 2));
    if (!string.IsNullOrEmpty(EmptyStateIconPath))
    {
        var iconRect = new Rectangle(center.X - 32, center.Y - 48, 64, 64);
        StyledImagePainter.DrawImage(g, EmptyStateIconPath, iconRect, _theme, 0.4f);
    }
    TextRenderer.DrawText(g, EmptyStateMessage,
        BeepThemesManager.ToFont(_theme?.SubTitleStyle),
        new Rectangle(0, center.Y + 24, ClientWidth, 32),
        _theme?.SecondaryTextColor ?? SystemColors.GrayText,
        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
}
```

#### 4.4.7 ApplyTheme Cascade

```csharp
// No BaseControl.ApplyTheme() to call — do it directly
public void ApplyTheme(string themeName)
{
    ThemeName = themeName; // also calls OnThemeChanged → Invalidate
    _tabStrip?.ApplyTheme(themeName);
    foreach (var p in _panels)
    {
        p.ApplyTheme(themeName);
        // Cascade to any IBeepUIComponent children inside the panel
        foreach (Control c in p.Controls)
            if (c is IBeepUIComponent ui) ui.Theme = themeName;
    }
}
```

---

### 4.5 `BeepDocumentFloatWindow` — Float Wrapper

**Namespace**: `TheTechIdea.Beep.Winform.Controls.DocumentHost`  
**File**: Nested class in `BeepDocumentHost.cs`  
**Base**: `BeepiFormPro`

```csharp
public class BeepDocumentFloatWindow : BeepiFormPro
{
    public BeepDocumentPanel Document { get; }
    public event EventHandler DockedBack;

    public BeepDocumentFloatWindow(BeepDocumentPanel panel)
    {
        Document  = panel;
        Text      = panel.DocumentTitle;
        Size      = new Size(800, 600);
        panel.Dock = DockStyle.Fill;
        Controls.Add(panel);

        panel.TitleChanged += (s, e) => Text = panel.DocumentTitle;
        FormClosed         += (s, e) => DockedBack?.Invoke(this, EventArgs.Empty);
    }
}
```

`BeepDocumentHost.Float(panel)`:
1. Remove panel from `_panels`, remove its tab from `_tabStrip`.
2. Create `BeepDocumentFloatWindow(panel)`, wire `DockedBack` → `DockBack(panel)`.
3. Fire `DocumentFloated`.
4. `Show()` the window.

`DockBack(panel)` reverses the operation: re-adds panel + syncs tab + activates.

---

### 4.6 `BeepDocumentOverflow` — Overflow Popup

**Namespace**: `TheTechIdea.Beep.Winform.Controls.DocumentHost`  
**File**: Nested class in `BeepDocumentHost.cs`  
**Base**: `BaseControl` displayed inside a lightweight `Form` (no border, `TopMost = true`)

Layout: vertical list of rows. Each row = icon (16px) + dirty-dot (if modified) + title text.  
Colors match `_currentTheme`. Uses same chip/row painting as LOV recent items.  
Click → fires callback to `BeepDocumentTabStrip` which activates the tab, closes popup.

---

## 5. Enumerations and Event Args

**File**: top of `BeepDocumentHost.cs`

```csharp
public enum TabStripPosition { Top, Bottom, Hidden }
public enum TabCloseMode     { Always, OnHover, Never }

public class TabEventArgs : EventArgs
{
    public BeepDocumentTab   Tab   { get; }
    public BeepDocumentPanel Panel { get; }
    public TabEventArgs(BeepDocumentTab tab, BeepDocumentPanel panel)
        => (Tab, Panel) = (tab, panel);
}

public class TabReorderArgs : EventArgs
{
    public BeepDocumentTab Tab      { get; }
    public int             OldIndex { get; }
    public int             NewIndex { get; }
    public TabReorderArgs(BeepDocumentTab tab, int oldIndex, int newIndex)
        => (Tab, OldIndex, NewIndex) = (tab, oldIndex, newIndex);
}

public class DocumentEventArgs : EventArgs
{
    public BeepDocumentPanel Document { get; }
    public DocumentEventArgs(BeepDocumentPanel doc) => Document = doc;
}
```

---

## 6. Theme Integration

`BaseControl` is NOT the base, so there is no inherited `_currentTheme`. Each control holds its own theme field
and subscribes to `BeepThemesManager.ThemeChanged`.

**Pattern used in every document host control:**

```csharp
// In BeepDocumentTabStrip / BeepDocumentPanel / BeepDocumentHost
private IBeepTheme _theme = BeepThemesManager.GetDefaultTheme();

public string ThemeName
{
    get => _theme?.ThemeName ?? BeepThemesManager.CurrentThemeName;
    set
    {
        var t = BeepThemesManager.GetTheme(value);
        if (t == null) return;
        _theme = t;
        OnThemeChanged();
    }
}

public void ApplyTheme(string themeName) => ThemeName = themeName;

protected virtual void OnThemeChanged() => Invalidate();

// Constructor wires event:
public BeepDocumentTabStrip()
{
    SetStyle(ControlStyles.AllPaintingInWmPaint |
             ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.UserPaint |
             ControlStyles.ResizeRedraw, true);
    _theme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
             ?? BeepThemesManager.GetDefaultTheme();
    BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
}

private void OnGlobalThemeChanged(object sender, string themeName)
    => ThemeName = themeName;

// Must unsubscribe in Dispose:
protected override void Dispose(bool disposing)
{
    if (disposing) BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
    base.Dispose(disposing);
}
```

**BeepDocumentPanel / BeepDocumentHost** use the same pattern but with `Panel` as base.
They also cascade theme to child controls that implement `IBeepUIComponent`.

Default color mappings:

| UI Element | Theme Property |
|---|---|
| Strip background | `theme.PanelColor` |
| Active tab background | `theme.PrimaryColor` |
| Inactive tab background | `theme.PanelColor` |
| Active tab text | `theme.TextColor` |
| Inactive tab text | `theme.SecondaryTextColor` |
| Active tab top accent line | `theme.PrimaryColor` (or `tab.AccentColor`) |
| Tab border | `theme.BorderColor` |
| Dirty dot | `theme.WarningColor` |
| Close button (normal) | `theme.SecondaryTextColor` |
| Close button (hover) | `theme.DangerColor` with circular fill |
| Document panel background | `theme.BackgroundColor` |
| Empty-state text | `theme.SecondaryTextColor` |
| Status bar | `theme.PanelColor` |
| Float window painter | `BeepiFormPro` default (inherits host theme) |

---

## 7. DPI Handling

All size constants are logical pixels scaled via `BeepThemesManager.ScaleToDpi`:

```csharp
// In BeepDocumentTabStrip:
private int _dpi = 96;
private int ScaledTabHeight   => BeepThemesManager.ScaleToDpi(32,  _dpi);
private int ScaledTabMinWidth => BeepThemesManager.ScaleToDpi(80,  _dpi);
private int ScaledTabMaxWidth => BeepThemesManager.ScaleToDpi(200, _dpi);
private int ScaledCloseSize   => BeepThemesManager.ScaleToDpi(16,  _dpi);
private int ScaledIconSize    => BeepThemesManager.ScaleToDpi(16,  _dpi);

protected override void OnDpiChanged(DpiChangedEventArgs e)
{
    base.OnDpiChanged(e);
    _dpi = e.DeviceDpiNew;
    CalculateTabLayout();
    Invalidate();
}
```

Same pattern in `BeepDocumentHost`: override `OnDpiChanged` → `RecalculateLayout()`.

---

## 8. Painter Impact on `BeepiFormPro`

**Zero changes to any of the 35 painters.**

`BeepDocumentHost` is added as a normal child control:

```csharp
// Minimal usage — just dock fill
var host = new BeepDocumentHost { Dock = DockStyle.Fill };
myBeepiFormPro.Controls.Add(host);
```

Painters compute `CurrentLayout.ContentRect` as always. `BeepDocumentHost` fills that area. No painter callbacks or overrides are needed.

---

## 9. File Structure

```
TheTechIdea.Beep.Winform.Controls/
  DocumentHost/
    BeepDocumentTab.cs          ← data model (no control)
    BeepDocumentTabStrip.cs     ← painted tab bar (BaseControl)
    BeepDocumentPanel.cs        ← document surface (BaseControl)
    BeepDocumentHost.cs         ← orchestrator + FloatWindow + Overflow
                                   + enumerations + event args
    Readme.md
  plans/
    mdiformplan.md              ← THIS FILE
```

---

## 10. Implementation Phases

### Phase 1 — Data Model (`BeepDocumentTab.cs`)
- All `BeepDocumentTab` properties including computed layout fields.
- `TabStripPosition`, `TabCloseMode` enums.
- `TabEventArgs`, `TabReorderArgs`, `DocumentEventArgs`.
- **No UI, no painting.**

### Phase 2 — `BeepDocumentTabStrip.cs`
- Inherit **`Control`** (NOT BaseControl).
- Constructor: `SetStyle(AllPaintingInWmPaint | OptimizedDoubleBuffer | UserPaint | ResizeRedraw, true)`.
- Subscribe `BeepThemesManager.ThemeChanged` → `ThemeName = name`.
- Unsubscribe in `Dispose(bool)`.
- `AddTab(BeepDocumentTab)`, `RemoveTab(id)`, `MoveTab(oldIndex, newIndex)`.
- `CalculateTabLayout()` — width distribution + overflow detection.
- `OnPaint` — full pipeline: background → separator → inactive tabs → active tab → overflow → add button.
- `BuildTabPath(rect, radius, isActive)` — `GraphicsPath` with rounded top corners.
- `DrawTab`, `DrawCloseButton`, `DrawDirtyDot`, `DrawAddButton`, `DrawOverflowButton`.
- Mouse: `GetTabAtPoint`, `GetCloseAtPoint`, hover tracking, click → fire events.
- `ApplyTheme` → invalidate.
- DPI: `OnDpiChanged` → recalculate.

### Phase 3 — `BeepDocumentPanel.cs`
- Inherit **`Panel`** (NOT BaseControl), set `IsContainerControl = false` (Panel is not ContainerControl).
- Constructor: subscribe `BeepThemesManager.ThemeChanged`; `BackColor = theme.BackgroundColor`; `DoubleBuffered = true`.
- Unsubscribe in `Dispose(bool)`.
- `DocumentTitle`, `DocumentIconPath`, `IsModified`, `CanClose`, `IsPinned`, `AccentColor`, `DocumentId`, `DocumentTag`.
- `ShowDocumentBorder`, `ShowStatusBar`, `StatusText`, `StatusIconPath`.
- `OnPaint`: border + status bar.
- `AutoScroll = true`.
- Events: `TitleChanged`, `ModifiedChanged`, `CloseRequested`, `Activated`, `Deactivated`.
- `ApplyTheme` cascade to child controls.

### Phase 4 — `BeepDocumentHost.cs` (core orchestration)
- Inherit **`Panel`** (NOT BaseControl); `DoubleBuffered = true`.
- Constructor: subscribe `BeepThemesManager.ThemeChanged`; create `_tabStrip`, wire events, `Controls.Add(_tabStrip)`.
- Unsubscribe + dispose tabStrip in `Dispose(bool)`.
- `AddDocument`, `NewDocument`, `SetActiveDocument`, `Remove`, `RequestClose`.
- `RecalculateLayout()` with `SuspendLayout` / `ResumeLayout`.
- `WirePanel` / `SyncTab` for property-change sync.
- Empty-state `OnPaint`.
- `OnResize` → `RecalculateLayout`.
- `ApplyTheme` cascade.
- `Float` / `DockBack`.

### Phase 5 — `BeepDocumentFloatWindow` (nested in `BeepDocumentHost.cs`)
- Inherit `BeepiFormPro`.
- Constructor: dock panel fill, set title, wire `TitleChanged` + `FormClosed`.
- `DockedBack` event.

### Phase 6 — `BeepDocumentOverflow` (nested in `BeepDocumentHost.cs`)
- Popup `Form` with `FormBorderStyle.None`, `TopMost = true`.
- Painted row list of overflowed tabs.
- Click → activate tab callback + close popup.
- Auto-close on `Deactivate`.

### Phase 7 — Integration & Sample
- Add `BeepDocumentHost` + `BeepDocumentPanel` + `BeepDocumentTabStrip` to `DesignRegistration.cs` toolbox category `"Beep Document"`.
- Demo form in `Beep.Sample.Winform`: 3 sample panels, theme switcher, float demo, dirty-state toggle.

### Phase 8 — Drag-to-Reorder (Extension)
- Full drag reorder in `BeepDocumentTabStrip`.
- Draw ghost tab during drag (semi-transparent copy at cursor position).
- On drop: call `MoveTab(oldIndex, newIndex)`, fire `TabReordered`.

---

## 11. Usage Example

```csharp
// Attach to any BeepiFormPro
var host = new BeepDocumentHost
{
    Dock             = DockStyle.Fill,
    ShowAddButton    = true,
    AllowFloating    = true,
    EmptyStateMessage = "Open a file to begin",
    EmptyStateIconPath = SvgsUI.FolderOpen
};
mainForm.Controls.Add(host);

// Add a document
var doc = new BeepDocumentPanel
{
    DocumentTitle   = "Customer List",
    DocumentIconPath = SvgsUI.People,
    CanClose        = true
};
var grid = new BeepGridPro { Dock = DockStyle.Fill };
doc.Controls.Add(grid);
host.AddDocument(doc);

// Mark document dirty
doc.IsModified = true;   // ● dot appears in tab automatically

// Handle close
host.DocumentCloseRequested += (s, e) =>
{
    if (e.Document.IsModified)
    {
        var r = MessageBox.Show("Save changes?", "Close", MessageBoxButtons.YesNoCancel);
        if (r == DialogResult.Cancel) return;
        if (r == DialogResult.Yes) Save(e.Document);
    }
    host.Remove(e.Document);
};
```

---

## 12. Risks and Mitigations

| Risk | Mitigation |
|---|---|
| Tab strip performance with many tabs | Clip each `DrawTab` call to `e.ClipRectangle`; skip tabs fully outside clip |
| Panel `Visible` toggle flickering | `SuspendLayout/ResumeLayout` in `SetActiveDocument`; set `BackColor` on panels to match host |
| Float window DPI differs from host | `BeepDocumentFloatWindow` inherits `BeepiFormPro` DPI handling; panel auto-scales |
| Theme cascade O(n) panels | Guard: skip if `themeName == _currentThemeName` |
| Drag ghost overdraw | Render ghost onto a 1-px-transparent overlay; invalidate only the ghost band |
| `BeepDocumentHost` inside `BeepSplitContainer` | `Dock = DockStyle.Fill` in each pane — no extra work needed |
| Empty-state flashing during fast add | Only paint empty-state when `_panels.Count == 0`; `AddDocument` calls `Invalidate` after `RecalcLayout` |

---

## 13. Deliverables Summary

| File | Action | Phase |
|---|---|---|
| `DocumentHost/BeepDocumentTab.cs` | CREATE | 1 |
| `DocumentHost/BeepDocumentTabStrip.cs` | CREATE | 2 |
| `DocumentHost/BeepDocumentPanel.cs` | CREATE | 3 |
| `DocumentHost/BeepDocumentHost.cs` | CREATE (includes FloatWindow + Overflow + types) | 4–6 |
| `DocumentHost/Readme.md` | CREATE | 4 |
| `Beep.Sample.Winform` demo form | EDIT | 7 |
| `DesignRegistration.cs` | EDIT — register new controls | 7 |
| **All 35 painter files** | **UNCHANGED** | — |
| **All BeepiFormPro.*.cs files** | **UNCHANGED** | — |

<!-- ============================================================
     END OF ACTIVE PLAN  (Scenario C)
     Everything below this line is superseded reference material
     from the original pre-decision draft. Safe to delete.
     ============================================================ -->
---

## 3. Scenario Analysis

### Scenario A — Extend BeepiFormPro as MDI Host (Win32 MDI)

**Concept**: Add an `IsMdiShell` property to `BeepiFormPro`. When true, automatically locate and reposition/resize the injected `MdiClient` control to sit exactly at `CurrentLayout.ContentRect`, leaving the custom-painted caption, toolbar and borders wholly owned by BeepiFormPro.

```
+------------------------------------------+
|  BeepiFormPro (Custom Caption — painters)|
|  [Icon] [Title]     [Theme][Style][Min][X]|
+------------------------------------------+
|  MdiClient (repositioned to ContentRect) |
|  +--------+  +--------+                  |
|  | Child1 |  | Child2 |                  |
|  +--------+  +--------+                  |
+------------------------------------------+
```

**How the painter conflict is resolved:**

- `MdiClient` is a child control. BeepiFormPro's NC-area custom painting (caption, border) lives in the **expanded client area** NOT the actual Win32 NC zone. By repositioning MdiClient to `CurrentLayout.ContentRect` the painters retain full ownership of the caption band.
- MDI children inside MdiClient get the **system-drawn** frame (not BeepiFormPro painters). They will look like standard Windows child windows.
- Rounded corners work for the outer form (BeepiFormPro still clips itself). The MdiClient rectangle sits fully inside `ContentRect` so it never intersects corner radii.

**Painter impact** (of the 35+ painters):

| Painter Element | Impact |
|---|---|
| `PaintBackground` | **None** — MdiClient sits on top of ContentRect; background only needs to fill caption + borders area |
| `PaintCaption` / `PaintBuiltInCaptionElements` | **None** — fully unaffected |
| `PaintBorders` | **None** |
| `CalculateLayoutAndHitAreas` | **None** — ContentRect computation unchanged; BeepiFormPro just repositions MdiClient to match |
| `GetCornerRadius` | **Minor** — corners still applied to outer form shape; MdiClient is rectangular inside |

**Changes required in BeepiFormPro:**

| File | Change |
|---|---|
| `BeepiFormPro.cs` | Add `bool IsMdiShell` property; when set, call `SetupMdiShell()` |
| `BeepiFormPro.Core.cs` | Override `OnLayout` + subscribe to `CurrentLayoutChanged` → call `PositionMdiClient()` |
| New: `BeepiFormPro.MdiShell.cs` | `SetupMdiShell()`: locate MdiClient, style it, call first `PositionMdiClient()`; `PositionMdiClient()`: sets MdiClient `Bounds = CurrentLayout.ContentRect` |
| `BeepiFormPro.Win32.cs` | Guard `WM_NCCALCSIZE` when `IsMdiShell` — keep same expansion logic; guard `WM_CHILDACTIVATE` to forward to MdiClient |
| NO painter changes | Painters are untouched |

**MDI Child forms for Scenario A:**
- Use plain `BeepiFormPro` instances with `FormStyle.Flat` or `FormStyle.None` as MDI children — they get the BeepiFormPro-painted caption inside MdiClient.
- OR use plain `Form` with `FormBorderStyle.Sizable` for classic MDI children.
- Custom-painted BeepiFormPro children inside MdiClient work because MdiClient clips them but does NOT intercept their `WM_NCPAINT` unless maximized.

**Limitations:**
- Maximized child title merge (Windows merging child title into host menu bar) will look visually odd. Disable with `MaximizeBox = false` on children, or intercept `WM_MDIMAXIMIZE`.
- Rounded corner form cannot be resized to the exact MDI snap behaviour.
- `CornerRadius` on outer form should be disabled when `IsMdiShell = true` (or set to 0) for best MDI visual coherence.

---

### Scenario B — Create Dedicated `BeepMdiHostForm` + `BeepMdiChildForm`

**Concept**: Separate classes, no modification to BeepiFormPro.

- `BeepMdiHostForm : BeepiFormPro` — sets `IsMdiContainer = true` in its constructor, positions MdiClient at `CurrentLayout.ContentRect`, disables rounded corners.
- `BeepMdiChildForm : Form` — lightweight form with client-side caption painted as a UserControl (no NC override) to avoid Win32 MDI NC conflicts.

**Painter impact**: None on existing painters. BeepMdiChildForm has its own simplified paint path.

**Trade-off**: More files, more maintenance, but cleanest isolation.

---

### Scenario C — Simulated MDI (Recommended Modern Approach)

**Concept**: No Win32 MDI at all. Add a `BeepDocumentHost` control (a panel with overflow tab bar) that sits at `ContentRect` as a child control of BeepiFormPro. Each "document" is a `BeepDocumentPanel` UserControl that is shown/hidden inside `BeepDocumentHost`.

```
+------------------------------------------+
|  BeepiFormPro (any painter)              |
|  [Icon] [Title]     [Theme][Style]       |
+------------------------------------------+
|  BeepDocumentHost (UserControl)          |
|  [Tab1] [Tab2×] [Tab3] [+]  ← tab strip |
|  +------------------------------------+  |
|  |  BeepDocumentPanel (active doc)    |  |
|  +------------------------------------+  |
+------------------------------------------+
```

**Painter impact**: **Zero** — `BeepiFormPro` is used as-is. All 35 painters work unchanged. `BeepDocumentHost` is a normal child control at `ContentRect`.

**Advantages:**
- Full theme control on tab strip and document chrome.
- No Win32 MDI quirks (no title merge, no system frame).
- Works on all DPI, all Windows themes, all painter styles.
- Can implement floating/docking windows by moving `BeepDocumentPanel` into a `BeepPopupForm` wrapper.
- VS Code, JetBrains, and DevExpress all use this pattern.

**What needs to be created:**

| New Class | Description |
|---|---|
| `BeepDocumentHost` | Panel with tab strip at top; manages `BeepDocumentPanel` instances; `ActiveDocument` property |
| `BeepDocumentPanel` | UserControl acting as a document window; has `Title`, `Icon`, `CanClose`, `IsModified` |
| `BeepDocumentTabStrip` | Painted tab bar (reuses BeepGridPro row/chip painters) showing open docs |
| `BeepDocumentOverflowMenu` | Popup menu when tabs overflow |

**No painter registration needed** — these are regular `UserControl` / `Panel` subclasses.

---

## 4. Decision Matrix

| Criterion | Scenario A (BeepiFormPro extend) | Scenario B (New classes) | Scenario C (Simulated MDI) |
|---|---|---|---|
| Painter compatibility | ✅ Full (no changes) | ✅ Full (no changes) | ✅ Full (no changes) |
| Win32 MDI compatible | ✅ Yes | ✅ Yes | ❌ No (not Win32 MDI) |
| Code isolation risk | ⚠️ Medium — touches BeepiFormPro | ✅ Low — isolated | ✅ Low — new controls |
| Maximized child title merge | ⚠️ Requires guard | ✅ Disable in BeepMdiChildForm | ✅ N/A |
| DPI / HiDPI safety | ✅ Inherits BeepiFormPro DPI | ✅ Inherits | ✅ Inherits |
| Theme / Beep styling | ✅ Host is fully themed | ⚠️ Child needs manual | ✅ Full control |
| Modern UX (tabs, float) | ❌ Limited by Win32 | ❌ Limited by Win32 | ✅ Unlimited |
| Implementation effort | Low-Medium | Medium | Medium-High |
| Recommended for… | Legacy MDI app migration | Clean new MDI | New Beep apps |

---

## 5. Chosen Path

**Primary: Scenario C (Simulated MDI)** for all new Beep applications.  
**Secondary: Scenario A (BeepiFormPro.MdiShell extension)** for apps that must interop with Win32 MDI (e.g., migrating an existing MDI app to Beep).

---

## 6. Implementation Plan — Scenario A: `BeepiFormPro.MdiShell`

### Phase A-1 — New partial file `BeepiFormPro.MdiShell.cs`

```csharp
// Fields
private bool _isMdiShell;
private Control _mdiClient;

// Property
public bool IsMdiShell { get; set; }  // calls SetupMdiShell() on change

private void SetupMdiShell()
{
    if (!_isMdiShell) return;
    IsMdiContainer = true;           // WinForms creates MdiClient
    _currentCornerRadius = 0;        // flat corners for MDI host
    PositionMdiClient();
}

private Control FindMdiClient()
{
    foreach (Control c in Controls)
        if (c.GetType().Name == "MdiClient") return c;
    return null;
}

private void PositionMdiClient()
{
    _mdiClient ??= FindMdiClient();
    if (_mdiClient == null) return;
    EnsureLayoutCalculated();
    var cr = CurrentLayout.ContentRect;
    _mdiClient.Bounds = cr;
    _mdiClient.BackColor = CurrentTheme?.PanelColor ?? SystemColors.AppWorkspace;
}
```

### Phase A-2 — Wire into layout pipeline (`BeepiFormPro.Core.cs`)

```csharp
protected override void OnLayout(LayoutEventArgs e)
{
    base.OnLayout(e);
    if (_isMdiShell) PositionMdiClient();
}
```

Subscribe inside `ApplyTheme` / `OnThemeChanged`:
```csharp
if (_isMdiShell) PositionMdiClient(); // sync MdiClient colour to theme
```

### Phase A-3 — Win32 guards (`BeepiFormPro.Win32.cs`)

The existing `WM_NCCALCSIZE` logic is unchanged (MDI host's client area still expands to cover the caption — `MdiClient` is then repositioned by `PositionMdiClient` to sit below the caption).

Add a guard in `WM_CHILDACTIVATE` handling:
```csharp
case WM_CHILDACTIVATE:
    base.WndProc(ref m);
    if (_isMdiShell) PositionMdiClient(); // keep MdiClient bound after child activation
    return;
```

### Phase A-4 — Disable rounded corners when `IsMdiShell`

In `GetEffectiveCornerRadius()` or equivalent:
```csharp
if (_isMdiShell) return new CornerRadius(0);
```

### Phase A-5 — Designer registration

Add `IsMdiShell` to `BeepiFormProDesigner` smart tag if designer exists.

### Deliverables (Scenario A)

| File | Action |
|---|---|
| `Forms/ModernForm/BeepiFormPro.MdiShell.cs` | **CREATE** |
| `Forms/ModernForm/BeepiFormPro.Core.cs` | **EDIT** — hook `OnLayout` |
| `Forms/ModernForm/BeepiFormPro.Win32.cs` | **EDIT** — `WM_CHILDACTIVATE` guard |
| `Forms/ModernForm/BeepiFormPro.cs` | **EDIT** — expose `IsMdiShell` property |
| `plans/mdiformplan.md` | This file |

---

## 7. Implementation Plan — Scenario C: Simulated MDI Controls

### Phase C-1 — `BeepDocumentPanel` (UserControl)

```
TheTechIdea.Beep.Winform.Controls/
  DocumentHost/
    BeepDocumentPanel.cs      // document content container
    BeepDocumentHost.cs       // manages panels + tab strip
    BeepDocumentTabStrip.cs   // painted tab bar
    BeepDocumentOverflow.cs   // overflow popup
    Readme.md
```

**BeepDocumentPanel** key members:
- `string DocumentTitle` — tab label
- `string DocumentIconPath` — tab icon (image string path, Beep convention)
- `bool IsModified` — shows dot in tab
- `bool CanClose` — shows × in tab
- `event EventHandler CloseRequested`
- `event EventHandler Activated`

**BeepDocumentHost** key members:
- `IReadOnlyList<BeepDocumentPanel> Documents`
- `BeepDocumentPanel ActiveDocument { get; set; }`
- `void AddDocument(BeepDocumentPanel doc)`
- `void CloseDocument(BeepDocumentPanel doc)`
- `void FloatDocument(BeepDocumentPanel doc)` — moves to a `BeepPopupForm`
- Uses `BeepDocumentTabStrip` at top, remaining area hosts active `BeepDocumentPanel`

### Phase C-2 — `BeepDocumentTabStrip` (custom painted Control)

- Inherits `BaseControl`
- Uses `BeepStyling` + `BeepThemesManager`
- Paints tab pills using existing `BeepTheme` colors (same chip style as LOV recent-items)
- Supports overflow `»` button → `BeepDocumentOverflow` popup
- Drag-to-reorder tabs (Phase C-3 extension)

### Phase C-3 — Extension: Float + Dock

A `BeepDocumentPanel` can be extracted into a `BeepiFormPro`-hosted floating window when user double-clicks the tab. Dragging it back docks it.

### Phase C-4 — Usage in BeepiFormPro

```csharp
var host = new BeepDocumentHost();
// BeepiFormPro exposes a ContentControl registration method
mainForm.SetContentControl(host);
```

OR simply dock `BeepDocumentHost` to `DockStyle.Fill` inside the form's client area.

### Deliverables (Scenario C)

| File | Action |
|---|---|
| `DocumentHost/BeepDocumentPanel.cs` | **CREATE** |
| `DocumentHost/BeepDocumentHost.cs` | **CREATE** |
| `DocumentHost/BeepDocumentTabStrip.cs` | **CREATE** |
| `DocumentHost/BeepDocumentOverflow.cs` | **CREATE** |
| `DocumentHost/Readme.md` | **CREATE** |

---

## 8. Painter Impact Summary (Final)

All 35+ painters are **unaffected** by both scenarios. Neither scenario modifies `IFormPainter`, `PainterLayoutInfo`, `CalculateLayoutAndHitAreas`, `PaintCaption`, `PaintBackground`, or `PaintBorders`.

**Scenario A** adds one call `PositionMdiClient()` triggered from `OnLayout` — this is isolated in the new partial file and does not touch any painter code path.

**Scenario C** adds new `UserControl`-derived classes — zero painter interaction.

---

## 9. Risks and Mitigations

| Risk | Mitigation |
|---|---|
| MdiClient z-order fights with custom painted caption | `PositionMdiClient` sets `Bounds` to `ContentRect` — MdiClient never overlaps caption band |
| Maximized MDI child steals title bar | Set `MdiChildrenMinimizedAnchorBottom = false`; intercept `WM_MDIMAXIMIZE` to cancel merge; or disallow child maximize via `MaximizeBox = false` |
| `IsMdiShell` + rounded corners = visual glitch | `GetEffectiveCornerRadius` returns 0 when `_isMdiShell` |
| `BeepDocumentHost` resizing performance | Defer panel layout changes with `SuspendLayout/ResumeLayout`; only active panel is shown |
| DPI change while documents are open | Subscribe `DpiChanged` in `BeepDocumentHost` → iterate and reposition |

---

## 10. File Structure Reference

```
TheTechIdea.Beep.Winform.Controls/
  Forms/
    ModernForm/
      BeepiFormPro.cs
      BeepiFormPro.Core.cs
      BeepiFormPro.Drawing.cs
      BeepiFormPro.Win32.cs
      BeepiFormPro.MdiShell.cs        ← NEW (Scenario A)
      Painters/
        IFormPainter.cs               (unchanged)
        MaterialFormPainter.cs        (unchanged)
        ... 35 painters unchanged ...
  DocumentHost/                       ← NEW (Scenario C)
    BeepDocumentPanel.cs
    BeepDocumentHost.cs
    BeepDocumentTabStrip.cs
    BeepDocumentOverflow.cs
    Readme.md
  plans/
    mdiformplan.md                    ← THIS FILE
```

---

## 11. Recommended Next Steps

1. **Confirm scenario choice** — A (Win32 MDI) or C (Simulated) or both.
2. **Scenario A**: Implement `BeepiFormPro.MdiShell.cs` (Phase A-1 through A-4) — estimated ~150 lines.
3. **Scenario C**: Implement `BeepDocumentHost` + `BeepDocumentPanel` (Phases C-1/C-2) — estimated ~400 lines split across 2 files.
4. Add sample usage to `Beep.Sample.Winform` demo form.
