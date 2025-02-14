using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    public static class BeepGridMiscUI
    {
        // Panels
        public static Panel TopPanel { get; private set; }
        public static Panel BottomPanel { get; private set; }
        public static Panel CenterPanel { get; private set; }

        // Sub-panels inside the top
        public static Panel HeaderPanel { get; private set; }
        public static Panel ColumnHeaderPanel { get; private set; }
        public static Panel FilterColumnHeaderPanel { get; private set; }

        // Sub-panels inside the bottom
        public static Panel FooterPanel { get; private set; }
        public static Panel TotalsPanel { get; private set; }
        public static Panel DataNavigatorPanel { get; private set; }

        // The main DataGridView container
        public static Panel DataGridViewPanel { get; private set; }
        public static Panel InnerGridPanel { get; private set; }
        public static Panel ScrollVerticalPanel { get; private set; }
        public static Panel ScrollHorizontalPanel { get; private set; }
        public static BeepScrollBar beepScrollBar1 { get; private set; }
        public static BeepScrollBar beepScrollBar2 { get; private set; }

        // The actual DataGridView passed in
        // Assume "control" is your Form or UserControl
        public static void InitializeLayout(Control control, DataGridView grid)
        {
            
            // The parent is where we put DataGridViewPanel, e.g. your main center area
            DataGridViewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Name = "DataGridViewPanel"
            };
            // 1. Create top/bottom/center panels
            TopPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            BottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            CenterPanel = new Panel
            {
                Dock = DockStyle.Fill,BorderStyle=  BorderStyle.None,
                
            };
            // 1) The vertical scroll panel (Dock=Right)
            ScrollVerticalPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 36, // or whatever width you want
                Name = "ScrollVerticalPanel"
            };
            beepScrollBar1 = new BeepScrollBar
            {
                Dock = DockStyle.Fill,
                Name = "beepScrollBar1"
            };
            ScrollVerticalPanel.Controls.Add(beepScrollBar1);

            // 2) The horizontal scroll panel (Dock=Bottom)
            ScrollHorizontalPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 24, // or whatever height you want
                Name = "ScrollHorizontalPanel"
            };
            beepScrollBar2 = new BeepScrollBar
            {
                Dock = DockStyle.Fill,
                Name = "beepScrollBar2"
            };
            ScrollHorizontalPanel.Controls.Add(beepScrollBar2);

            // 2. Create sub-panels in TopPanel
            HeaderPanel = new Panel { Dock = DockStyle.Top, Height = 30 };
            ColumnHeaderPanel = new Panel { Dock = DockStyle.Top, Height = 30 };
            FilterColumnHeaderPanel = new Panel { Dock = DockStyle.Top, Height = 30 };

            // Add them to TopPanel (reverse order if each is dock=top)
            TopPanel.Controls.Add(FilterColumnHeaderPanel);
            TopPanel.Controls.Add(ColumnHeaderPanel);
            TopPanel.Controls.Add(HeaderPanel);

            // 3. Create sub-panels in BottomPanel
            TotalsPanel = new Panel { Dock = DockStyle.Top, Height = 35 };
            DataNavigatorPanel = new Panel { Dock = DockStyle.Top, Height = 35 };
            FooterPanel = new Panel { Dock = DockStyle.Fill }; // or top, depends on your design

            BottomPanel.Controls.Add(FooterPanel);
            BottomPanel.Controls.Add(DataNavigatorPanel);
            BottomPanel.Controls.Add(TotalsPanel);

            // 3) The panel that actually holds the DataGridView (Dock=Fill)
            InnerGridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Name = "InnerGridPanel"
            };
            grid.Dock = DockStyle.Fill; // Fill the InnerGridPanel
            InnerGridPanel.Controls.Add(grid);

            // Put them into DataGridViewPanel, ensuring "fill" is added last
            DataGridViewPanel.Controls.Add(InnerGridPanel);       // fill leftover space
            DataGridViewPanel.Controls.Add(ScrollHorizontalPanel); // bottom
            DataGridViewPanel.Controls.Add(ScrollVerticalPanel);   // right
            CenterPanel.Controls.Add(DataGridViewPanel);

            // 5. Add them to the main control
            control.Controls.Add(CenterPanel);
            control.Controls.Add(BottomPanel);
            control.Controls.Add(TopPanel);
            grid.ScrollBars = ScrollBars.None;
            beepScrollBar1.ScrollOrientation = Orientation.Vertical; // or use property if needed
            beepScrollBar1.Minimum = 0;
            beepScrollBar1.Maximum = Math.Max(0, grid.RowCount - 1);
            beepScrollBar1.Value = 0;
            beepScrollBar1.ValueChanged += (s, e) =>
            {
                // Make sure Value doesn't exceed RowCount
                if (beepScrollBar1.Value >= 0 && beepScrollBar1.Value < grid.RowCount)
                {
                    grid.FirstDisplayedScrollingRowIndex = beepScrollBar1.Value;
                }
            };

            ShowHorizontalScrollBar = false;
            ShowVerticalScrollBar = false;
            // Done! Now, if you hide FilterColumnHeaderPanel.Visible = false,
            // the top panel automatically shrinks, and center expands.
            grid.Scroll += (s, e) =>
            {
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    // Keep beepScrollBar1 in sync
                    beepScrollBar1.Value = grid.FirstDisplayedScrollingRowIndex;
                }
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    beepScrollBar2.Value = grid.FirstDisplayedScrollingColumnIndex;
                }
            };


            beepScrollBar2.ScrollOrientation = Orientation.Horizontal;
            beepScrollBar2.Minimum = 0;
            beepScrollBar2.Maximum = Math.Max(0, grid.ColumnCount - 1);
            beepScrollBar2.Value = 0;
            beepScrollBar2.ValueChanged += (s, e) =>
            {
                if (beepScrollBar2.Value >= 0 && beepScrollBar2.Value < grid.ColumnCount)
                {
                    grid.FirstDisplayedScrollingColumnIndex = beepScrollBar2.Value;
                }
            };

            grid.RowsAdded += (s, e) =>
            {
                beepScrollBar1.Maximum = Math.Max(0, grid.RowCount - 1);
                UpdateVerticalScrollbarPanelVisibility(grid);
            };

            grid.RowsRemoved += (s, e) =>
            {
                beepScrollBar1.Maximum = Math.Max(0, grid.RowCount - 1);
                UpdateVerticalScrollbarPanelVisibility(grid );
            };

            grid.ColumnAdded += (s, e) =>
            {
                beepScrollBar2.Maximum = Math.Max(0, grid.ColumnCount - 1);
                UpdateHorizontalScrollbarPanelVisibility(grid);
            };

            grid.ColumnRemoved += (s, e) =>
            {
                beepScrollBar2.Maximum = Math.Max(0, grid.ColumnCount - 1);
                UpdateHorizontalScrollbarPanelVisibility(grid);
            };

            grid.ColumnWidthChanged += (s, e) =>
            {
                UpdateHorizontalScrollbarPanelVisibility(grid);
            };
            grid.BorderStyle = BorderStyle.None;
            InnerGridPanel.BorderStyle = BorderStyle.None;
            DataGridViewPanel.BorderStyle = BorderStyle.None;
            ScrollHorizontalPanel.BorderStyle = BorderStyle.None;
            ScrollVerticalPanel.BorderStyle = BorderStyle.None;
            CenterPanel.BorderStyle = BorderStyle.None;
            TopPanel.BorderStyle = BorderStyle.None;
            BottomPanel.BorderStyle = BorderStyle.None;
        }
        private static void UpdateVerticalScrollbarPanelVisibility(DataGridView grid)
        {
            // If row-based scrolling, show the panel only if rowCount > displayed
            int displayedCount = grid.DisplayedRowCount(true);
            bool needScroll = (grid.RowCount > displayedCount);

            // Suppose your vertical scrollbar is in a panel named 'ScrollVerticalPanel'
            // Hide or show the entire panel
            ScrollVerticalPanel.Visible = needScroll;
        }
        private static void UpdateHorizontalScrollbarPanelVisibility(DataGridView grid)
        {
            // Typically column-based
            // If you do column-based scroll => if (grid.ColumnCount > displayedColumns)
            // Or if pixel-based => compare total columns width to grid.ClientSize.Width
            int displayedCols = grid.DisplayedColumnCount(true);
            bool needScroll = (grid.ColumnCount > displayedCols);

            ScrollHorizontalPanel.Visible = needScroll;
        }
        // Example Hide property
        public static bool ShowFilter
        {
            get => FilterColumnHeaderPanel.Visible;
            set => FilterColumnHeaderPanel.Visible = value;
        }
        // Example Hide property for Totals
        public static bool ShowTotals
        {
            get => TotalsPanel.Visible;
            set { if (TotalsPanel != null) TotalsPanel.Visible = value; }
        }
        public static bool ShowHorizontalScrollBar
        {
            get => ScrollHorizontalPanel.Visible;
            set => ScrollHorizontalPanel.Visible = value;
        }
        public static bool ShowVerticalScrollBar
        {
            get => ScrollVerticalPanel.Visible;
            set => ScrollVerticalPanel.Visible = value;
        }
    }

}
