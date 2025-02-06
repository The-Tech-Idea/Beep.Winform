using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A grid control that displays data using TableLayout.")]
    [DisplayName("Beep Table Layout Grid")]
    public class BeepTableLayoutGrid:BeepControl
    {

        public TableLayoutPanel tableLayoutPanel1;
        public BeepDataNavigator beepDataNavigator1;
        public VScrollBar vScrollBar1;
        public HScrollBar hScrollBar1;
        public BeepMultiSplitter GridtableLayoutPanel;
        public BeepLabel TitleLabel;
        public BeepLabel RecordCountLabel;
        public BeepLabel PageCountLabel;
        public BeepLabel PageNumberLabel;
        public int Gridheight { get; set; }
        public int GridWidth { get; set; }
        public ObservableCollection<object>  _dataSource;

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
            //if (GridtableLayoutPanel == null || vScrollBar1 == null || hScrollBar1 == null)
            //    throw new InvalidOperationException("GridtableLayoutPanel or scrollbars are not initialized.");

            beepGridRowPainter = new BeepGridRowPainterForTableLayout(
                GridtableLayoutPanel // Table Layout Panel for Grid

            );
            Console.WriteLine("BeepTableLayoutGrid InitializeBeepGridPainter");
            LoadSampleData();
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
        new { ID = 10, Name = "Jack", Age = 31, Country = "Brazil" },
         new { ID = 11, Name = "Alice", Age = 30, Country = "USA" },
        new { ID = 12, Name = "Bob", Age = 28, Country = "Canada" },
        new { ID = 13, Name = "Charlie", Age = 35, Country = "UK" },
        new { ID = 14, Name = "David", Age = 40, Country = "Germany" },
        new { ID = 15, Name = "Eva", Age = 25, Country = "France" },
        new { ID = 16, Name = "Frank", Age = 38, Country = "Australia" },
        new { ID = 17, Name = "Grace", Age = 29, Country = "Italy" },
        new { ID = 18, Name = "Henry", Age = 32, Country = "Spain" },
        new { ID = 19, Name = "Ivy", Age = 27, Country = "Japan" },
        new { ID = 20, Name = "Jack", Age = 31, Country = "Brazil" },
         new { ID = 21, Name = "Alice", Age = 30, Country = "USA" },
        new { ID = 22, Name = "Bob", Age = 28, Country = "Canada" },
        new { ID = 23, Name = "Charlie", Age = 35, Country = "UK" },
        new { ID = 24, Name = "David", Age = 40, Country = "Germany" },
        new { ID = 25, Name = "Eva", Age = 25, Country = "France" },
        new { ID = 26, Name = "Frank", Age = 38, Country = "Australia" },
        new { ID = 27, Name = "Grace", Age = 29, Country = "Italy" },
        new { ID = 28, Name = "Henry", Age = 32, Country = "Spain" },
        new { ID = 29, Name = "Ivy", Age = 27, Country = "Japan" },
        new { ID = 30, Name = "Jack", Age = 31, Country = "Brazil" }
    };

            if (beepGridRowPainter == null)
                throw new InvalidOperationException("BeepGridRowPainterForTableLayout is not initialized. Call InitializeBeepGridPainter() first.");
            Console.WriteLine("BeepTableLayoutGrid LoadSampleData");
            beepGridRowPainter.SetDataSource(sampleData);
        }

        public void initView()
        {
            // Clear existing controls
            Controls.Clear();

            // Initialize main layout panel
            tableLayoutPanel1 = new TableLayoutPanel
            {
                Dock = DockStyle.Fill
            };
            tableLayoutPanel1.ColumnCount = 1; // Single column layout
            tableLayoutPanel1.RowCount = 4;   // Title, Grid, Filter, Navigator

            // Define column style
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Full width

            // Define row styles
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Title
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Data Rows (fills available space)
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Filter Panel
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Data Navigator

            // Initialize Title Label
            TitleLabel = new BeepLabel
            {
                Text = "Grid Title",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14F, FontStyle.Bold),
                BackColor = Color.White
            };
            tableLayoutPanel1.Controls.Add(TitleLabel, 0, 0);
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray,
                Size = new System.Drawing.Size(GridWidth, Gridheight)

            };
            // Initialize Grid Table Layout
            GridtableLayoutPanel = new BeepMultiSplitter
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };
            panel.Controls.Add(GridtableLayoutPanel);
            tableLayoutPanel1.Controls.Add(panel, 0, 1);

            // Initialize Filter Panel
            var filterPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };
            var filterLabel = new BeepLabel
            {
                Text = "Filter String: show conditions to filter data",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                Font = new Font("Arial", 10F, FontStyle.Regular)
            };
            filterPanel.Controls.Add(filterLabel);
            tableLayoutPanel1.Controls.Add(filterPanel, 0, 2);

            // Initialize Data Navigator
            beepDataNavigator1 = new BeepDataNavigator
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };
            tableLayoutPanel1.Controls.Add(beepDataNavigator1, 0, 3);

            // Add the main layout panel to the control
            Controls.Add(tableLayoutPanel1);

            // Initialize BeepGridRowPainter
           InitializeBeepGridPainter();

            // Force layout updates
            ResumeLayout(true);
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
