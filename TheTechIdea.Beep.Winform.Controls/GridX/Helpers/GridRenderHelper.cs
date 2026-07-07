using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
 
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers; // Svgs
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using ContentAlignment = System.Drawing.ContentAlignment;
using navigationStyle = TheTechIdea.Beep.Winform.Controls.GridX.Painters.navigationStyle;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridRenderHelper
    {
        private readonly BeepGridPro _grid;
        private BeepCheckBoxBool _rowCheck;

        // Cache drawers per column (like BeepSimpleGrid)
        private readonly Dictionary<string, IBeepUIComponent> _columnDrawerCache = new();

        // Store filter icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _headerFilterIconRects = new();
        public Dictionary<int, Rectangle> HeaderFilterIconRects => _headerFilterIconRects;

        // Store sort icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _headerSortIconRects = new();
        public Dictionary<int, Rectangle> HeaderSortIconRects => _headerSortIconRects;

        // Store top filter panel cell/icon rectangles for hit-testing
        private readonly Dictionary<int, Rectangle> _topFilterCellRects = new();
        public Dictionary<int, Rectangle> TopFilterCellRects => _topFilterCellRects;

        private readonly Dictionary<int, Rectangle> _topFilterClearIconRects = new();
        public Dictionary<int, Rectangle> TopFilterClearIconRects => _topFilterClearIconRects;

        // Group header rectangles for hit-testing (key = group key hash)
        private readonly Dictionary<string, Rectangle> _groupHeaderRects = new();
        public Dictionary<string, Rectangle> GroupHeaderRects => _groupHeaderRects;
        private string? _hoveredGroupHeaderKey;

        private IGridFilterPanelPainter? _filterPanelPainter;
        private BeepGridStyle _filterPanelGridStyle = BeepGridStyle.Default;

        public GridRenderHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        // Grid Style properties
        public bool ShowGridLines { get; set; } = true;
        public bool ShowRowStripes { get; set; } = false;
        public System.Drawing.Drawing2D.DashStyle GridLineStyle { get; set; } = System.Drawing.Drawing2D.DashStyle.Solid;
        public bool UseElevation { get; set; } = false;
        public bool CardStyle { get; set; } = false;

        // Advanced header styling properties
        public bool UseHeaderGradient { get; set; } = false;
        public bool ShowSortIndicators { get; set; } = true;
        public HeaderIconVisibility SortIconVisibility { get; set; } = HeaderIconVisibility.Always;
        public HeaderIconVisibility FilterIconVisibility { get; set; } = HeaderIconVisibility.Hidden;
        public bool UseHeaderHoverEffects { get; set; } = true;
        public bool UseBoldHeaderText { get; set; } = false;
        public int HeaderCellPadding { get; set; } = 2;

        // Focus styling
        public bool UseDedicatedFocusedRowStyle { get; set; } = true;
        public Color FocusedRowBackColor { get; set; } = Color.Empty;
        public bool ShowFocusedCellFill { get; set; } = true;
        public Color FocusedCellFillColor { get; set; } = Color.Empty;
        public int FocusedCellFillOpacity { get; set; } = 36;
        public bool ShowFocusedCellBorder { get; set; } = true;
        public Color FocusedCellBorderColor { get; set; } = Color.Empty;
        public float FocusedCellBorderWidth { get; set; } = BeepLayoutMetrics.GridFocusBorderW;

        internal IBeepTheme Theme => _grid.Theme != null ? BeepThemesManager.GetTheme(_grid.Theme) : BeepThemesManager.GetDefaultTheme();

        /// <summary>Cached bold version of the header font. Created once per font change;
        /// reused across all header-cell draws to avoid the per-draw <c>new Font(...)</c>
        /// allocation that was visible in the toolbar paint profile.</summary>
        private Font? _cachedBoldHeaderFont;
        private Font? _cachedBoldBaseFont; // tracks which base font the bold variant was built from

        /// <summary>
        /// Returns the bold header font, creating it once per base-font change. The previous
        /// instance is disposed when the base font changes. Do not dispose the returned font
        /// — it is owned by this helper.
        /// </summary>
        internal Font GetBoldHeaderFont(Font baseFont)
        {
            if (baseFont == null) return SystemFonts.DefaultFont;
            if (!ReferenceEquals(baseFont, _cachedBoldBaseFont) || _cachedBoldHeaderFont == null)
            {
                _cachedBoldHeaderFont?.Dispose();
                _cachedBoldHeaderFont = new Font(baseFont.FontFamily, baseFont.Size, FontStyle.Bold);
                _cachedBoldBaseFont = baseFont;
            }
            return _cachedBoldHeaderFont;
        }

        internal void DisposeFontCache()
        {
            _cachedBoldHeaderFont?.Dispose();
            _cachedBoldHeaderFont = null;
            _cachedBoldBaseFont = null;
        }
    }
}

