using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Built-in layout presets for <see cref="BeepMultiSplitter"/>, modelled on common
    /// product layouts (app shells, holy grail, master-detail, dashboards, IDE panes).
    /// Applying a preset reconfigures the grid (columns / rows + size styles); with
    /// placeholders enabled it also lays out named region tiles with the correct spans.
    /// </summary>
    public enum MultiSplitterLayout
    {
        /// <summary>No preset - configured manually / by the designer.</summary>
        Custom,

        // Basic splits
        SingleCell,
        TwoColumns,
        ThreeColumns,
        FourColumns,
        TwoRows,
        ThreeRows,

        // Ratio splits
        SplitTwoThirdsLeft,     // 66 / 33
        SplitTwoThirdsRight,    // 33 / 66
        SplitThreeQuarterLeft,  // 75 / 25
        SplitThreeQuarterRight, // 25 / 75
        SplitGoldenLeft,        // 62 / 38
        SplitGoldenRight,       // 38 / 62

        // Sidebars / master-detail
        SidebarLeft,
        SidebarRight,
        MasterDetail,           // list rail + detail
        DualSidebar,            // left + content + right
        ThreePaneColumns,       // rail + content + rail

        // App shells
        AppShell,               // header + sidebar + content
        AppShellWithInspector,  // header + sidebar + content + inspector
        HolyGrail,              // header + left + content + right + footer
        RailPanelContent,       // VS Code style: rail + panel + content
        IdeLayout,              // rail + explorer + editor + inspector + bottom terminal

        // Header / footer stacks
        HeaderContent,
        HeaderContentFooter,
        ContentWithBottomBar,
        TitleContentActions,    // header + content + action bar

        // Productivity
        EmailThreePane,         // folders + list + reading
        ChatLayout,             // conversations + messages
        EditorPreview,          // editor + live preview

        // Dashboards / grids
        Grid2x2,
        Grid3x3,
        Grid2x3,
        Grid3x2,
        KpiRowWithContent,      // 4 KPI tiles on top, content below
        DashboardMain           // KPI row + two charts
    }

    public partial class BeepMultiSplitter
    {
        /// <summary>A named cell/region within a preset, with optional column/row spans.</summary>
        private readonly struct Region
        {
            public readonly string Name;
            public readonly int Col, Row, ColSpan, RowSpan;
            public Region(string name, int col, int row, int colSpan = 1, int rowSpan = 1)
            {
                Name = name; Col = col; Row = row; ColSpan = colSpan; RowSpan = rowSpan;
            }
        }

        private MultiSplitterLayout _layout = MultiSplitterLayout.Custom;

        /// <summary>
        /// Gets or sets a built-in layout preset. Setting a value other than
        /// <see cref="MultiSplitterLayout.Custom"/> reconfigures the grid structure.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Apply a built-in layout preset. Reconfigures the grid columns/rows and their size styles.")]
        [DefaultValue(MultiSplitterLayout.Custom)]
        public MultiSplitterLayout Layout
        {
            get => _layout;
            set
            {
                _layout = value;
                if (value != MultiSplitterLayout.Custom)
                    ApplyLayout(value, addPlaceholders: false);
            }
        }

        /// <summary>
        /// Applies a built-in layout preset to the grid. Clears existing cell styles and
        /// (optionally) child controls, then configures columns/rows for the chosen preset.
        /// Fixed dimensions (rail/sidebar width, header/footer height) are DPI-scaled.
        /// </summary>
        /// <param name="layout">The preset to apply.</param>
        /// <param name="addPlaceholders">
        /// When true, named region tiles (BeepLabel) are placed with the correct spans so
        /// the layout is visible immediately. Existing controls are cleared first.
        /// </param>
        public void ApplyLayout(MultiSplitterLayout layout, bool addPlaceholders = false)
        {
            if (layout == MultiSplitterLayout.Custom) return;
            var tlp = _tableLayoutPanel;
            if (tlp == null) return;

            var spec = GetPreset(layout);
            if (spec.cols == null) return;

            tlp.SuspendLayout();
            try
            {
                if (addPlaceholders)
                {
                    for (int i = tlp.Controls.Count - 1; i >= 0; i--)
                    {
                        var c = tlp.Controls[i];
                        tlp.Controls.Remove(c);
                        c.Dispose();
                    }
                }

                tlp.ColumnStyles.Clear();
                tlp.RowStyles.Clear();
                tlp.ColumnCount = spec.cols.Length;
                tlp.RowCount = spec.rows.Length;
                foreach (var c in spec.cols) tlp.ColumnStyles.Add(new ColumnStyle(c.t, c.v));
                foreach (var r in spec.rows) tlp.RowStyles.Add(new RowStyle(r.t, r.v));

                if (addPlaceholders) PlaceRegions(spec.regions);
            }
            finally
            {
                tlp.ResumeLayout(true);
            }
        }

        /// <summary>Resolves a preset to its column specs, row specs, and named regions.</summary>
        private ((SizeType t, float v)[] cols, (SizeType t, float v)[] rows, Region[] regions) GetPreset(MultiSplitterLayout layout)
        {
            int rail      = DpiScalingHelper.ScaleValue(56, this);
            int sidebar   = DpiScalingHelper.ScaleValue(BeepLayoutMetrics.Sidebar.Width, this); // 240
            int panel     = DpiScalingHelper.ScaleValue(220, this);
            int inspector = DpiScalingHelper.ScaleValue(280, this);
            int bar       = DpiScalingHelper.ScaleValue(BeepLayoutMetrics.NavBar.Height, this);  // 56
            int bottom    = DpiScalingHelper.ScaleValue(160, this);
            int kpi       = DpiScalingHelper.ScaleValue(120, this);

            switch (layout)
            {
                case MultiSplitterLayout.SingleCell:
                    return (C(Pct(100)), C(Pct(100)), R(new Region("Content", 0, 0)));

                case MultiSplitterLayout.TwoColumns:
                    return (C(Pct(50), Pct(50)), C(Pct(100)), R(new Region("Left", 0, 0), new Region("Right", 1, 0)));
                case MultiSplitterLayout.ThreeColumns:
                    return (C(Eq(3)), C(Pct(100)), Cells(3, 1));
                case MultiSplitterLayout.FourColumns:
                    return (C(Eq(4)), C(Pct(100)), Cells(4, 1));
                case MultiSplitterLayout.TwoRows:
                    return (C(Pct(100)), C(Pct(50), Pct(50)), R(new Region("Top", 0, 0), new Region("Bottom", 0, 1)));
                case MultiSplitterLayout.ThreeRows:
                    return (C(Pct(100)), C(Eq(3)), Cells(1, 3));

                case MultiSplitterLayout.SplitTwoThirdsLeft:
                    return (C(Pct(66.66f), Pct(33.34f)), C(Pct(100)), R(new Region("Content", 0, 0), new Region("Aside", 1, 0)));
                case MultiSplitterLayout.SplitTwoThirdsRight:
                    return (C(Pct(33.34f), Pct(66.66f)), C(Pct(100)), R(new Region("Aside", 0, 0), new Region("Content", 1, 0)));
                case MultiSplitterLayout.SplitThreeQuarterLeft:
                    return (C(Pct(75f), Pct(25f)), C(Pct(100)), R(new Region("Content", 0, 0), new Region("Aside", 1, 0)));
                case MultiSplitterLayout.SplitThreeQuarterRight:
                    return (C(Pct(25f), Pct(75f)), C(Pct(100)), R(new Region("Aside", 0, 0), new Region("Content", 1, 0)));
                case MultiSplitterLayout.SplitGoldenLeft:
                    return (C(Pct(61.8f), Pct(38.2f)), C(Pct(100)), R(new Region("Content", 0, 0), new Region("Aside", 1, 0)));
                case MultiSplitterLayout.SplitGoldenRight:
                    return (C(Pct(38.2f), Pct(61.8f)), C(Pct(100)), R(new Region("Aside", 0, 0), new Region("Content", 1, 0)));

                case MultiSplitterLayout.SidebarLeft:
                    return (C(Abs(sidebar), Pct(100)), C(Pct(100)), R(new Region("Sidebar", 0, 0), new Region("Content", 1, 0)));
                case MultiSplitterLayout.SidebarRight:
                    return (C(Pct(100), Abs(sidebar)), C(Pct(100)), R(new Region("Content", 0, 0), new Region("Sidebar", 1, 0)));
                case MultiSplitterLayout.MasterDetail:
                    return (C(Abs(DpiScalingHelper.ScaleValue(320, this)), Pct(100)), C(Pct(100)), R(new Region("List", 0, 0), new Region("Detail", 1, 0)));
                case MultiSplitterLayout.DualSidebar:
                    return (C(Abs(sidebar), Pct(100), Abs(sidebar)), C(Pct(100)), R(new Region("Left", 0, 0), new Region("Content", 1, 0), new Region("Right", 2, 0)));
                case MultiSplitterLayout.ThreePaneColumns:
                    return (C(Abs(rail), Pct(100), Abs(rail)), C(Pct(100)), R(new Region("Rail", 0, 0), new Region("Content", 1, 0), new Region("Rail", 2, 0)));

                case MultiSplitterLayout.AppShell:
                    return (C(Abs(sidebar), Pct(100)), C(Abs(bar), Pct(100)),
                        R(new Region("Header", 0, 0, colSpan: 2), new Region("Sidebar", 0, 1), new Region("Content", 1, 1)));
                case MultiSplitterLayout.AppShellWithInspector:
                    return (C(Abs(sidebar), Pct(100), Abs(inspector)), C(Abs(bar), Pct(100)),
                        R(new Region("Header", 0, 0, colSpan: 3), new Region("Sidebar", 0, 1), new Region("Content", 1, 1), new Region("Inspector", 2, 1)));
                case MultiSplitterLayout.HolyGrail:
                    return (C(Abs(sidebar), Pct(100), Abs(sidebar)), C(Abs(bar), Pct(100), Abs(bar)),
                        R(new Region("Header", 0, 0, colSpan: 3), new Region("Left", 0, 1), new Region("Content", 1, 1),
                          new Region("Right", 2, 1), new Region("Footer", 0, 2, colSpan: 3)));
                case MultiSplitterLayout.RailPanelContent:
                    return (C(Abs(rail), Abs(panel), Pct(100)), C(Pct(100)),
                        R(new Region("Rail", 0, 0), new Region("Panel", 1, 0), new Region("Content", 2, 0)));
                case MultiSplitterLayout.IdeLayout:
                    return (C(Abs(rail), Abs(panel), Pct(100), Abs(inspector)), C(Pct(100), Abs(bottom)),
                        R(new Region("Rail", 0, 0, rowSpan: 2), new Region("Explorer", 1, 0), new Region("Editor", 2, 0),
                          new Region("Inspector", 3, 0), new Region("Terminal", 1, 1, colSpan: 3)));

                case MultiSplitterLayout.HeaderContent:
                    return (C(Pct(100)), C(Abs(bar), Pct(100)), R(new Region("Header", 0, 0), new Region("Content", 0, 1)));
                case MultiSplitterLayout.HeaderContentFooter:
                    return (C(Pct(100)), C(Abs(bar), Pct(100), Abs(bar)), R(new Region("Header", 0, 0), new Region("Content", 0, 1), new Region("Footer", 0, 2)));
                case MultiSplitterLayout.ContentWithBottomBar:
                    return (C(Pct(100)), C(Pct(100), Abs(bar)), R(new Region("Content", 0, 0), new Region("Bottom Bar", 0, 1)));
                case MultiSplitterLayout.TitleContentActions:
                    return (C(Pct(100)), C(Abs(bar), Pct(100), Abs(bar)), R(new Region("Title", 0, 0), new Region("Content", 0, 1), new Region("Actions", 0, 2)));

                case MultiSplitterLayout.EmailThreePane:
                    return (C(Abs(sidebar), Abs(DpiScalingHelper.ScaleValue(320, this)), Pct(100)), C(Pct(100)),
                        R(new Region("Folders", 0, 0), new Region("List", 1, 0), new Region("Reading", 2, 0)));
                case MultiSplitterLayout.ChatLayout:
                    return (C(Abs(DpiScalingHelper.ScaleValue(300, this)), Pct(100)), C(Pct(100)),
                        R(new Region("Conversations", 0, 0), new Region("Messages", 1, 0)));
                case MultiSplitterLayout.EditorPreview:
                    return (C(Pct(50), Pct(50)), C(Pct(100)), R(new Region("Editor", 0, 0), new Region("Preview", 1, 0)));

                case MultiSplitterLayout.Grid2x2:
                    return (C(Eq(2)), C(Eq(2)), Cells(2, 2));
                case MultiSplitterLayout.Grid3x3:
                    return (C(Eq(3)), C(Eq(3)), Cells(3, 3));
                case MultiSplitterLayout.Grid2x3:
                    return (C(Eq(2)), C(Eq(3)), Cells(2, 3));
                case MultiSplitterLayout.Grid3x2:
                    return (C(Eq(3)), C(Eq(2)), Cells(3, 2));
                case MultiSplitterLayout.KpiRowWithContent:
                    return (C(Eq(4)), C(Abs(kpi), Pct(100)),
                        R(new Region("KPI 1", 0, 0), new Region("KPI 2", 1, 0), new Region("KPI 3", 2, 0), new Region("KPI 4", 3, 0),
                          new Region("Content", 0, 1, colSpan: 4)));
                case MultiSplitterLayout.DashboardMain:
                    return (C(Pct(50), Pct(50)), C(Abs(kpi), Pct(100)),
                        R(new Region("KPI Row", 0, 0, colSpan: 2), new Region("Chart", 0, 1), new Region("Chart", 1, 1)));

                default:
                    return (null, null, null);
            }
        }

        #region "Preset builder helpers"

        private static (SizeType t, float v) Pct(float value) => (SizeType.Percent, value);
        private static (SizeType t, float v) Abs(int pixels) => (SizeType.Absolute, pixels);

        /// <summary>Column/row spec array shorthand.</summary>
        private static (SizeType t, float v)[] C(params (SizeType t, float v)[] specs) => specs;

        /// <summary>N equal percentage tracks.</summary>
        private static (SizeType t, float v)[] Eq(int count)
        {
            var arr = new (SizeType t, float v)[count];
            float share = 100f / count;
            for (int i = 0; i < count; i++) arr[i] = (SizeType.Percent, share);
            return arr;
        }

        /// <summary>Region array shorthand.</summary>
        private static Region[] R(params Region[] regions) => regions;

        /// <summary>Generates one region per cell of a cols x rows grid (labelled RrCc).</summary>
        private static Region[] Cells(int cols, int rows)
        {
            var list = new List<Region>(cols * rows);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    list.Add(new Region($"R{r + 1}C{c + 1}", c, r));
            return list.ToArray();
        }

        /// <summary>Places a labelled BeepLabel tile for each region, applying column/row spans.</summary>
        private void PlaceRegions(Region[] regions)
        {
            if (regions == null) return;
            foreach (var region in regions)
            {
                var lbl = new BeepLabel
                {
                    Text = region.Name,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                _tableLayoutPanel.Controls.Add(lbl, region.Col, region.Row);
                if (region.ColSpan > 1) _tableLayoutPanel.SetColumnSpan(lbl, region.ColSpan);
                if (region.RowSpan > 1) _tableLayoutPanel.SetRowSpan(lbl, region.RowSpan);
            }
        }

        #endregion
    }
}
