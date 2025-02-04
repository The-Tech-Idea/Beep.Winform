using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A grid control that displays data using TableLayout.")]
    [DisplayName("Beep Table Layout Grid")]
    public class BeepTableLayoutGrid:BeepControl
    {

        public BeepMultiSplitter tableLayoutPanel1;
        public BeepDataNavigator beepDataNavigator1;
        public VScrollBar vScrollBar1;
        public HScrollBar hScrollBar1;
        public BeepMultiSplitter GridtableLayoutPanel;
        public BeepLabel TitleLabel;
        public BeepLabel RecordCountLabel;
        public BeepLabel PageCountLabel;
        public BeepLabel PageNumberLabel;

        private BeepGridRowPainterForTableLayout beepGridRowPainter;
        public BeepTableLayoutGrid()
        {
            Console.WriteLine("BeepTableLayoutGrid Constructor");
            initView();
            Console.WriteLine("BeepTableLayoutGrid Constructor End");
        //    InitializeBeepGridPainter();
            Console.WriteLine("BeepTableLayoutGrid Constructor End 2");
            // Load sample data for testing
           
            Console.WriteLine("BeepTableLayoutGrid Constructor End 3");
        }
        private void InitializeBeepGridPainter()
        {
            if (GridtableLayoutPanel == null || vScrollBar1 == null || hScrollBar1 == null)
                throw new InvalidOperationException("GridtableLayoutPanel or scrollbars are not initialized.");

            beepGridRowPainter = new BeepGridRowPainterForTableLayout(
                GridtableLayoutPanel, // Table Layout Panel for Grid
                vScrollBar1, // Vertical Scroll Bar
                hScrollBar1  // Horizontal Scroll Bar
            );
        }

        public void LoadSampleData()
        {
            var sampleData = new List<object>
    {
        new { ID = 1, Name = "Alice", Age = 30, Country = "USA" },
        new { ID = 2, Name = "Bob", Age = 28, Country = "Canada" },
        new { ID = 3, Name = "Charlie", Age = 35, Country = "UK" },
        new { ID = 4, Name = "David", Age = 40, Country = "Germany" },
        new { ID = 5, Name = "Eva", Age = 25, Country = "France" },
        new { ID = 6, Name = "Frank", Age = 38, Country = "Australia" },
        new { ID = 7, Name = "Grace", Age = 29, Country = "Italy" },
        new { ID = 8, Name = "Henry", Age = 32, Country = "Spain" },
        new { ID = 9, Name = "Ivy", Age = 27, Country = "Japan" },
        new { ID = 10, Name = "Jack", Age = 31, Country = "Brazil" }
    };

            if (beepGridRowPainter == null)
                throw new InvalidOperationException("BeepGridRowPainterForTableLayout is not initialized. Call InitializeBeepGridPainter() first.");

            beepGridRowPainter.SetDataSource(sampleData);
        }

        public void initView()
        {
            // Clear existing controls
            this.Controls.Clear();

            // Initialize main layout panel
            tableLayoutPanel1 = new BeepMultiSplitter();
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel1.tableLayoutPanel.RowCount = 5;

            // Define column styles
            tableLayoutPanel1.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50)); // Checkbox/Pointer
            tableLayoutPanel1.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Data Rows
            tableLayoutPanel1.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20)); // Vertical Scrollbar

            // Define row styles
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Title
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Data Rows
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20)); // Horizontal Scrollbar
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Filter String
            tableLayoutPanel1.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Data Navigator

            // Initialize Title Label
            TitleLabel = new BeepLabel
            {
                Text = "Grid Title",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14F, FontStyle.Bold),
                BackColor = Color.White
            };
            tableLayoutPanel1.tableLayoutPanel.SetColumnSpan(TitleLabel, 3);
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(TitleLabel, 0, 0);

            // Initialize Grid Table Layout
            GridtableLayoutPanel = new BeepMultiSplitter
            {
                Dock = DockStyle.Fill
            };
            tableLayoutPanel1.tableLayoutPanel.SetColumnSpan(GridtableLayoutPanel, 2); // Spans Checkbox and Data Columns
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(GridtableLayoutPanel, 0, 1);

            // Initialize Vertical Scrollbar
            vScrollBar1 = new VScrollBar
            {
                Dock = DockStyle.Fill,
                SmallChange = 1,
                LargeChange = 5
            };
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(vScrollBar1, 2, 1);

            // Initialize Horizontal Scrollbar
            hScrollBar1 = new HScrollBar
            {
                Dock = DockStyle.Fill,
                SmallChange = 1,
                LargeChange = 5
            };
            tableLayoutPanel1.tableLayoutPanel.SetColumnSpan(hScrollBar1, 2); // Spans Checkbox and Data Columns
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(hScrollBar1, 0, 2);

            // Initialize Filter String Label
            var filterStringLabel = new BeepLabel
            {
                Text = "Filter String: show conditions to filter data",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10F, FontStyle.Regular)
            };
            tableLayoutPanel1.tableLayoutPanel.SetColumnSpan(filterStringLabel, 3);
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(filterStringLabel, 0, 3);

            // Initialize Data Navigator
            beepDataNavigator1 = new BeepDataNavigator
            {
                Dock = DockStyle.Fill
            };
            tableLayoutPanel1.tableLayoutPanel.SetColumnSpan(beepDataNavigator1, 3);
            tableLayoutPanel1.tableLayoutPanel.Controls.Add(beepDataNavigator1, 0, 4);

            // Add the main layout panel to the control
            this.Controls.Add(tableLayoutPanel1);

            // Attach Resize Event
            

            // Initialize BeepGridRowPainter
            InitializeBeepGridPainter();

            // Delay sample data loading to ensure proper initialization
           
        }
    
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Resize the Grid Table Layout Panel
            UpdateDrawingRect();
           tableLayoutPanel1.Left = BorderThickness;
            tableLayoutPanel1.Top = BorderThickness;
            tableLayoutPanel1.Width = DrawingRect.Width- (2*BorderThickness);
            tableLayoutPanel1.Height = DrawingRect.Height- (2*BorderThickness);
        }




    }
}
