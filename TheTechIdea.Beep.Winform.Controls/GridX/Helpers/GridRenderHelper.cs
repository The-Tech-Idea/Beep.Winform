using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
 
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
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
        public float FocusedCellBorderWidth { get; set; } = 2f;

        internal IBeepTheme Theme => _grid.Theme != null ? BeepThemesManager.GetTheme(_grid.Theme) : BeepThemesManager.GetDefaultTheme();
    }
}

