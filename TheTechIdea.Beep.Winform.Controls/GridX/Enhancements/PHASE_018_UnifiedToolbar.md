# Phase 18: Unified Grid Toolbar — Actions + Search + Filter

**Priority:** P1 | **Track:** Bug Fixes & Feature Additions | **Status:** Pending

## Objective

Replace the separate filter panel (`BeepFilterRow`, `BeepQuickFilterBar`) and add a new unified toolbar that combines action buttons (export, import, print), search box, and filter controls in a single horizontal bar — all owner-drawn with SVG icons.

## Problem

Current state has two separate UI regions with overlapping concerns:
- `BeepQuickFilterBar` — search + filter (uses real child controls, fixed widths, emoji)
- `BeepFilterRow` — advanced filter rows (uses real child controls, fixed widths)
- No toolbar for action buttons (export, import, print)

This causes:
- **Z-order fights**: Child controls paint over grid rendering
- **DPI/font scaling breaks**: Fixed pixel widths don't adapt
- **Clipping issues**: Controls overflow their allocated region
- **Performance**: Each child control has its own WndProc and message loop
- **Alignment drift**: Manual layout with hardcoded margins breaks on resize
- **Fragmented UX**: Actions and filters are in separate bars

## What Commercial Products Do

| Product | Toolbar Layout |
|---|---|
| **DevExpress XtraGrid** | Single bar: [New] [Edit] [Delete] | [Search____] [Filter] [Advanced] | [Export] [Print] |
| **Telerik RadGridView** | Single bar: [Add] [Delete] | [Quick Filter____] | [Export] [Print] |
| **Infragistics UltraGrid** | Single bar: [Actions] | [Search] | [Filter] | [Export] |
| **AG-Grid (web)** | Developer-provided single toolbar above grid |

**Common pattern**: One toolbar, left-to-right: Actions → Search → Filter → Export.

## Unified Toolbar Layout

```
┌─────────────────────────────────────────────────────────────────────────┐
│ [➕] [✏️] [🗑️] │ [🔍 Search____________] │ [⚙ Filter] [▼] │ [📥] [📤] [🖨️] │
│  Actions         Search Box           Filter Controls      Export/Print  │
└─────────────────────────────────────────────────────────────────────────┘
```

### Section Layout (left to right)

| Section | Elements | Width Strategy |
|---|---|---|
| **Actions** | Add, Edit, Delete buttons | Fixed icon size |
| **Separator** | Vertical divider line | 1px |
| **Search** | Search icon + textbox | Flexible (takes remaining space) |
| **Filter** | Filter icon + dropdown + advanced | Fixed icon size |
| **Separator** | Vertical divider line | 1px |
| **Export** | Import, Export, Print buttons | Fixed icon size |

## Implementation Steps

### Step 1: Create Toolbar State Model

```csharp
// Toolbar/BeepGridToolbarState.cs
public class BeepGridToolbarState
{
    public string SearchText { get; set; } = string.Empty;
    public bool SearchHasFocus { get; set; }
    public int ActiveFilterCount { get; set; }
    public bool IsFilterActive { get; set; }
    public float DpiScale { get; set; } = 1f;
    
    // Layout rects (calculated)
    public Rectangle ActionsSectionRect { get; private set; }
    public Rectangle SearchSectionRect { get; private set; }
    public Rectangle FilterSectionRect { get; private set; }
    public Rectangle ExportSectionRect { get; private set; }
    public Rectangle SearchIconRect { get; private set; }
    public Rectangle SearchBoxRect { get; private set; }
    public Rectangle FilterButtonRect { get; private set; }
    public Rectangle AdvancedButtonRect { get; private set; }
    public Rectangle ClearFilterRect { get; private set; }
    public Rectangle BadgeRect { get; private set; }
    
    // Button rects
    public Rectangle AddButtonRect { get; private set; }
    public Rectangle EditButtonRect { get; private set; }
    public Rectangle DeleteButtonRect { get; private set; }
    public Rectangle ImportButtonRect { get; private set; }
    public Rectangle ExportButtonRect { get; private set; }
    public Rectangle PrintButtonRect { get; private set; }
    
    // Hover/pressed state
    public string? HoveredButtonKey { get; set; }
    public string? PressedButtonKey { get; set; }
    
    public void CalculateLayout(Rectangle bounds);
    
    // Hit testing
    public string? HitTest(Point p);
    public bool HitTestSearchBox(Point p) => SearchBoxRect.Contains(p);
    public bool HitTestFilterButton(Point p) => FilterButtonRect.Contains(p);
    public bool HitTestAdvancedButton(Point p) => AdvancedButtonRect.Contains(p);
    public bool HitTestClearFilter(Point p) => ClearFilterRect.Contains(p);
}
```

### Step 2: Calculate Layout Proportionally

```csharp
public void CalculateLayout(Rectangle bounds)
{
    int margin = (int)(6 * DpiScale);
    int iconSize = (int)(18 * DpiScale);
    int buttonGap = (int)(4 * DpiScale);
    int height = (int)(32 * DpiScale);
    int y = bounds.Top + (bounds.Height - height) / 2;
    
    // === ACTIONS SECTION (left) ===
    int x = bounds.Left + margin;
    AddButtonRect = new Rectangle(x, y, iconSize, iconSize);
    x += iconSize + buttonGap;
    EditButtonRect = new Rectangle(x, y, iconSize, iconSize);
    x += iconSize + buttonGap;
    DeleteButtonRect = new Rectangle(x, y, iconSize, iconSize);
    x += iconSize + margin;
    ActionsSectionRect = new Rectangle(bounds.Left, y - (height - iconSize) / 2, x - bounds.Left - margin, height);
    
    // === SEARCH SECTION (flexible) ===
    int searchIconX = x;
    SearchIconRect = new Rectangle(searchIconX, y, iconSize, iconSize);
    x += iconSize + (int)(4 * DpiScale);
    
    // Reserve space for filter + export sections
    int filterSectionWidth = iconSize * 2 + buttonGap + margin * 2; // filter + advanced
    int exportSectionWidth = iconSize * 3 + buttonGap * 2 + margin * 2; // import + export + print
    int separatorWidth = (int)(2 * DpiScale);
    int reservedRight = filterSectionWidth + exportSectionWidth + separatorWidth * 2 + margin;
    
    int searchWidth = bounds.Right - x - reservedRight;
    searchWidth = Math.Max(100, searchWidth); // minimum 100px
    
    SearchBoxRect = new Rectangle(x, y, searchWidth, height);
    x = SearchBoxRect.Right + margin;
    SearchSectionRect = new Rectangle(ActionsSectionRect.Right, y - (height - iconSize) / 2, x - ActionsSectionRect.Right, height);
    
    // === FILTER SECTION ===
    x += separatorWidth; // separator line
    FilterButtonRect = new Rectangle(x, y, iconSize, iconSize);
    x += iconSize + buttonGap;
    AdvancedButtonRect = new Rectangle(x, y, iconSize, iconSize);
    x += iconSize + margin;
    
    // Badge position
    BadgeRect = new Rectangle(FilterButtonRect.Right - 4, y - 6, (int)(16 * DpiScale), (int)(16 * DpiScale));
    
    // Clear filter (only visible when filter active)
    ClearFilterRect = new Rectangle(AdvancedButtonRect.Right + buttonGap, y, iconSize, iconSize);
    
    FilterSectionRect = new Rectangle(SearchSectionRect.Right, y - (height - iconSize) / 2, x - SearchSectionRect.Right, height);
    
    // === EXPORT SECTION (right) ===
    x += separatorWidth; // separator line
    ImportButtonRect = new Rectangle(x, y, iconSize, iconSize);
    x += iconSize + buttonGap;
    ExportButtonRect = new Rectangle(x, y, iconSize, iconSize);
    x += iconSize + buttonGap;
    PrintButtonRect = new Rectangle(x, y, iconSize, iconSize);
    ExportSectionRect = new Rectangle(FilterSectionRect.Right, y - (height - iconSize) / 2, bounds.Right - FilterSectionRect.Right, height);
}
```

### Step 3: Create Toolbar Painter

```csharp
// Toolbar/BeepGridToolbarPainter.cs
public class BeepGridToolbarPainter
{
    private readonly BeepGridPro _grid;
    
    public BeepGridToolbarPainter(BeepGridPro grid) { _grid = grid; }
    
    public void Paint(Graphics g, Rectangle bounds, BeepGridToolbarState state)
    {
        // Background
        using var bgBrush = new SolidBrush(_grid.ToolbarBackColor);
        g.FillRectangle(bgBrush, bounds);
        
        // Paint sections
        PaintActionButtons(g, state);
        PaintSearchSection(g, state);
        PaintFilterSection(g, state);
        PaintExportButtons(g, state);
        PaintSeparators(g, state);
        
        // Bottom border
        using var borderPen = new Pen(_grid.GridLineColor, 1);
        g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
    }
    
    private void PaintActionButtons(Graphics g, BeepGridToolbarState state)
    {
        var style = _grid.GridStyle.ToControlStyle();
        
        PaintIconButton(g, state.AddButtonRect, SvgsUI.Plus, "Add",
            state.HoveredButtonKey == "add", state.PressedButtonKey == "add");
        PaintIconButton(g, state.EditButtonRect, SvgsUI.Edit, "Edit",
            state.HoveredButtonKey == "edit", state.PressedButtonKey == "edit");
        PaintIconButton(g, state.DeleteButtonRect, SvgsUI.Trash, "Delete",
            state.HoveredButtonKey == "delete", state.PressedButtonKey == "delete");
    }
    
    private void PaintSearchSection(Graphics g, BeepGridToolbarState state)
    {
        // Search icon (tinted)
        var iconColor = state.SearchHasFocus ? _grid.AccentColor : _grid.ToolbarForeColor;
        StyledImagePainter.PaintWithTint(g, state.SearchIconRect, Svgs.Search, iconColor, 0.7f);
        
        // Search box (painted fake textbox)
        PaintSearchBox(g, state.SearchBoxRect, state.SearchText, state.SearchHasFocus);
    }
    
    private void PaintSearchBox(Graphics g, Rectangle bounds, string text, bool hasFocus)
    {
        // Background
        var bgColor = hasFocus ? _grid.ToolbarSearchFocusBackColor : _grid.ToolbarSearchBackColor;
        using var bgBrush = new SolidBrush(bgColor);
        g.FillRectangle(bgBrush, bounds);
        
        // Border
        using var borderPen = new Pen(hasFocus ? _grid.AccentColor : _grid.ToolbarBorderColor, 1);
        g.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        
        // Text
        if (!string.IsNullOrEmpty(text))
        {
            TextRenderer.DrawText(g, text, _grid.Font, bounds, _grid.ToolbarForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
        else
        {
            // Placeholder
            TextRenderer.DrawText(g, "Search...", _grid.Font, bounds, _grid.ToolbarPlaceholderColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
    
    private void PaintFilterSection(Graphics g, BeepGridToolbarState state)
    {
        var filterColor = state.IsFilterActive ? _grid.AccentColor : _grid.ToolbarForeColor;
        
        // Filter button
        StyledImagePainter.PaintWithTint(g, state.FilterButtonRect, SvgsUI.Filter, filterColor,
            state.IsFilterActive ? 1f : 0.6f);
        
        // Advanced button (sliders icon)
        StyledImagePainter.PaintWithTint(g, state.AdvancedButtonRect, SvgsUI.AdjustmentsHorizontal,
            _grid.ToolbarForeColor, 0.6f);
        
        // Clear filter (only when active)
        if (state.IsFilterActive)
        {
            PaintIconButton(g, state.ClearFilterRect, SvgsUI.X, "Clear Filter",
                state.HoveredButtonKey == "clearfilter", state.PressedButtonKey == "clearfilter");
        }
        
        // Badge
        if (state.ActiveFilterCount > 0)
        {
            PaintBadge(g, state.BadgeRect, state.ActiveFilterCount);
        }
    }
    
    private void PaintExportButtons(Graphics g, BeepGridToolbarState state)
    {
        PaintIconButton(g, state.ImportButtonRect, Svgs.FileUpload, "Import",
            state.HoveredButtonKey == "import", state.PressedButtonKey == "import");
        PaintIconButton(g, state.ExportButtonRect, Svgs.Export, "Export",
            state.HoveredButtonKey == "export", state.PressedButtonKey == "export");
        PaintIconButton(g, state.PrintButtonRect, Svgs.Print, "Print",
            state.HoveredButtonKey == "print", state.PressedButtonKey == "print");
    }
    
    private void PaintIconButton(Graphics g, Rectangle bounds, string iconPath, string tooltip,
        bool isHovered, bool isPressed)
    {
        // Hover background
        if (isHovered || isPressed)
        {
            var hoverColor = isPressed ? _grid.ToolbarButtonPressedBackColor : _grid.ToolbarButtonHoverBackColor;
            using var hoverBrush = new SolidBrush(hoverColor);
            g.FillRectangle(hoverBrush, bounds);
        }
        
        // Icon
        var iconColor = _grid.ToolbarForeColor;
        StyledImagePainter.PaintWithTint(g, bounds, iconPath, iconColor, 0.8f);
    }
    
    private void PaintSeparators(Graphics g, BeepGridToolbarState state)
    {
        using var pen = new Pen(_grid.ToolbarSeparatorColor, 1);
        
        // Between actions and search
        int sep1X = state.ActionsSectionRect.Right;
        g.DrawLine(pen, sep1X, state.ActionsSectionRect.Top + 4, sep1X, state.ActionsSectionRect.Bottom - 4);
        
        // Between filter and export
        int sep2X = state.FilterSectionRect.Right;
        g.DrawLine(pen, sep2X, state.FilterSectionRect.Top + 4, sep2X, state.FilterSectionRect.Bottom - 4);
    }
    
    private void PaintBadge(Graphics g, Rectangle bounds, int count)
    {
        // Circle background
        using var brush = new SolidBrush(_grid.AccentColor);
        g.FillEllipse(brush, bounds);
        
        // Count text
        var text = count > 9 ? "9+" : count.ToString();
        using var font = new Font(_grid.Font.FontFamily, 7f);
        TextRenderer.DrawText(g, text, font, bounds, Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
```

### Step 4: On-Demand Control Activation

Only create real controls when user interacts with the painted search box:

```csharp
// Filtering/FilterEditorHelper.cs
public class FilterEditorHelper
{
    private BeepTextBox? _searchEditor;
    private readonly BeepGridPro _grid;
    
    public void ActivateSearchEditor(Rectangle bounds)
    {
        if (_searchEditor == null)
        {
            _searchEditor = new BeepTextBox
            {
                IsChild = true,
                IsFrameless = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = _grid.Font
            };
            _searchEditor.LostFocus += OnSearchEditorLostFocus;
            _searchEditor.KeyDown += OnSearchEditorKeyDown;
            _grid.Controls.Add(_searchEditor);
        }
        
        _searchEditor.Bounds = bounds;
        _searchEditor.Text = _grid.ToolbarState.SearchText;
        _searchEditor.Visible = true;
        _searchEditor.Focus();
        _searchEditor.SelectAll();
    }
    
    private void OnSearchEditorKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            _grid.ApplyQuickFilter(_searchEditor.Text);
            _searchEditor.Visible = false;
        }
        else if (e.KeyCode == Keys.Escape)
        {
            _searchEditor.Visible = false;
        }
    }
    
    private void OnSearchEditorLostFocus(object sender, EventArgs e)
    {
        _grid.ApplyQuickFilter(_searchEditor.Text);
        _searchEditor.Visible = false;
    }
}
```

### Step 5: Input Handling

```csharp
// In GridInputHelper.HandleMouseDown():
if (Layout.ToolbarRect.Contains(e.Location))
{
    var state = _grid.ToolbarState;
    var hitKey = state.HitTest(e.Location);
    
    if (state.HitTestSearchBox(e.Location))
    {
        _filterEditor.ActivateSearchEditor(state.SearchBoxRect);
        return;
    }
    
    if (state.HitTestFilterButton(e.Location))
    {
        _grid.ShowFilterPopup();
        return;
    }
    
    if (state.HitTestAdvancedButton(e.Location))
    {
        _grid.ShowAdvancedFilterDialog();
        return;
    }
    
    if (state.HitTestClearFilter(e.Location) && state.IsFilterActive)
    {
        _grid.ClearFilter();
        return;
    }
    
    // Button clicks
    switch (hitKey)
    {
        case "add": _grid.InsertNew(); break;
        case "edit": _grid.BeginEditCurrent(); break;
        case "delete": _grid.DeleteCurrent(); break;
        case "import": _grid.ImportData(); break;
        case "export": _grid.ShowExportDialog(); break;
        case "print": _grid.Print(); break;
    }
    
    state.PressedButtonKey = hitKey;
    _grid.SafeInvalidate(Layout.ToolbarRect);
    return;
}
```

### Step 6: Add to BeepGridPro

```csharp
// BeepGridPro.Properties.cs
public bool ShowToolbar { get; set; } = true;
public BeepGridToolbarState ToolbarState { get; } = new();
public BeepGridToolbarPainter ToolbarPainter { get; }

// Constructor:
ToolbarPainter = new BeepGridToolbarPainter(this);

// In Recalculate():
if (ShowToolbar)
{
    ToolbarRect = new Rectangle(r.Left, top, r.Width, (int)(36 * CurrentDpiScale));
    top += ToolbarRect.Height;
    ToolbarState.DpiScale = CurrentDpiScale;
    ToolbarState.CalculateLayout(ToolbarRect);
}
```

### Step 7: Toolbar Color Properties

```csharp
// BeepGridPro.Properties.cs
public Color ToolbarBackColor { get; set; } = Color.FromArgb(248, 249, 250);
public Color ToolbarForeColor { get; set; } = Color.FromArgb(33, 37, 41);
public Color ToolbarPlaceholderColor { get; set; } = Color.FromArgb(150, 150, 150);
public Color ToolbarSearchBackColor { get; set; } = Color.White;
public Color ToolbarSearchFocusBackColor { get; set; } = Color.FromArgb(240, 245, 255);
public Color ToolbarBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
public Color ToolbarButtonHoverBackColor { get; set; } = Color.FromArgb(230, 235, 240);
public Color ToolbarButtonPressedBackColor { get; set; } = Color.FromArgb(210, 220, 230);
public Color ToolbarSeparatorColor { get; set; } = Color.FromArgb(220, 220, 220);
```

### Step 8: Deprecate Old Controls

```csharp
[Obsolete("Use the unified toolbar instead. This control will be removed in a future version.")]
public class BeepFilterRow { ... }

[Obsolete("Use the unified toolbar instead. This control will be removed in a future version.")]
public class BeepQuickFilterBar { ... }
```

## Icon Mapping

| Element | Icon Path | Source | Tint |
|---|---|---|---|
| Add | `SvgsUI.Plus` | Tabler | ForeColor |
| Edit | `SvgsUI.Edit` | Tabler | ForeColor |
| Delete | `SvgsUI.Trash` | Tabler | ForeColor |
| Search icon | `Svgs.Search` | General SVG | Focus: Accent, else: ForeColor |
| Filter | `SvgsUI.Filter` | Tabler | Active: Accent, else: ForeColor (60%) |
| Advanced | `SvgsUI.AdjustmentsHorizontal` | Tabler | ForeColor (60%) |
| Clear filter | `SvgsUI.X` | Tabler | ForeColor |
| Import | `Svgs.FileUpload` | General SVG | ForeColor |
| Export | `Svgs.Export` | General SVG | ForeColor |
| Print | `Svgs.Print` | General SVG | ForeColor |

## Acceptance Criteria

- [ ] Single toolbar renders above header
- [ ] All sections visible: Actions, Search, Filter, Export
- [ ] SVG icons render with correct tinting
- [ ] Search box activates on click, commits on Enter, cancels on Escape
- [ ] Filter button opens Excel-style popup
- [ ] Advanced button opens advanced filter dialog
- [ ] Clear filter button visible only when filter active
- [ ] Badge shows active filter count
- [ ] Action buttons (Add, Edit, Delete) trigger grid operations
- [ ] Export buttons (Import, Export, Print) trigger operations
- [ ] Hover and pressed states render on all buttons
- [ ] Separator lines divide sections correctly
- [ ] Toolbar resizes correctly when grid resizes
- [ ] Search box takes flexible width (fills available space)
- [ ] Works at 100%, 125%, 150%, 200% DPI
- [ ] No child controls visible at rest (only search editor on activation)
- [ ] No z-order or clipping issues
- [ ] `BeepFilterRow` and `BeepQuickFilterBar` marked as `[Obsolete]`

## Rollback Plan

Revert to using `BeepQuickFilterBar` as child control. The painted toolbar is additive — old code remains.

## Files to Create

- `Toolbar/BeepGridToolbarState.cs`
- `Toolbar/BeepGridToolbarPainter.cs`
- `Filtering/FilterEditorHelper.cs`

## Files to Modify

- `BeepGridPro.cs` (add toolbar initialization)
- `BeepGridPro.Properties.cs` (add ShowToolbar, ToolbarState, color properties)
- `Helpers/GridLayoutHelper.cs` (add ToolbarRect, update TopFilterRect)
- `Helpers/GridRenderHelper.Rendering.cs` (draw toolbar instead of filter panel)
- `Helpers/GridInputHelper.cs` (handle toolbar hit testing)
- `Filtering/BeepFilterRow.cs` (mark as [Obsolete])
- `Filtering/BeepQuickFilterBar.cs` (mark as [Obsolete])
