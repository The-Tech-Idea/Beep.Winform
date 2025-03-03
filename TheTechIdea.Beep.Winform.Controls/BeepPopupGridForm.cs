using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Grid;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Popup Grid Form")]
    [Description("A popup form displaying a BeepSimpleGrid for selecting a row, using DataSource.")]
    public partial class BeepPopupGridForm : BeepPopupForm
    {
        #region Fields
        private BeepSimpleGrid _grid;
        private object _selectedRowData;
        #endregion

        #region Properties
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public object DataSource
        {
            get => _grid.DataSource;
            set
            {
                _grid.DataSource = value;
                // Grid fills itself when DataSource is set
                AdjustSize();
            }
        }

        [Browsable(false)]
        public object SelectedRowData
        {
            get => _selectedRowData;
            private set => _selectedRowData = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("List of columns to display in the grid.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<BeepColumnConfig> Columns
        {
            get => _grid.Columns;
            set => _grid.Columns = value ?? new List<BeepColumnConfig>();
        }
        #endregion

        #region Events
        public event EventHandler<object> RowSelected;
        protected virtual void OnRowSelected(object selectedRow)
        {
            RowSelected?.Invoke(this, selectedRow);
        }
        #endregion

        #region Constructor
        public BeepPopupGridForm()
        {
            InitializeComponent();
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.Size = new Size(300, 200); // Default size, adjusted later
            this.Deactivate += (s, e) => this.Hide();
            this.ResumeLayout(false);
            InitializeGrid();
        }

        public BeepPopupGridForm(object dataSource, List<BeepColumnConfig> columns) : this()
        {
            Columns = columns;
            DataSource = dataSource; // Grid will fill itself
        }
        #endregion

        #region Initialization
       

        private void InitializeGrid()
        {
            _grid = new BeepSimpleGrid
            {
                Dock = DockStyle.Fill,
                ShowColumnHeaders = false,
                ShowNavigator = false,
                Theme = Theme
            };
            _grid.SelectedRowChanged += _grid_SelectedRowChanged;
            this.Controls.Add(_grid);
        }


        #endregion

        #region Event Handlers
        private void _grid_SelectedRowChanged(object? sender, BeepGridRowSelectedEventArgs e)
        {
            if (e.Row != null && e.Row.RowData != null)
            {
                SelectedRowData = e.Row.RowData;
                OnRowSelected(SelectedRowData);
                this.Hide();
            }
        }
        #endregion

        #region Public Methods
        public object ShowPopup(Control triggerControl, BeepPopupFormPosition position)
        {
            AdjustSize();
            base.ShowPopup(triggerControl, position);
            return SelectedRowData;
        }

        public object ShowPopup(Control triggerControl, BeepPopupFormPosition position, Point adjustment)
        {
            AdjustSize();
            base.ShowPopup(triggerControl, position, adjustment);
            return SelectedRowData;
        }

        public object ShowPopup(Control triggerControl, Point location)
        {
            AdjustSize();
            base.ShowPopup(triggerControl, location);
            return SelectedRowData;
        }

        public object ShowPopup(Point anchorPoint, BeepPopupFormPosition position)
        {
            AdjustSize();
            base.ShowPopup(anchorPoint, position); // Now matches new base overload
            return SelectedRowData;
        }
        #endregion
        #region Helper Methods
        private void AdjustSize()
        {
            int rowHeight = _grid.RowHeight;
            int visibleRowCount = Math.Min(_grid.Rows.Count, 10); // Cap at 10 rows, adjust as needed
            int gridHeight = visibleRowCount * rowHeight + (_grid.BorderThickness * 2);
            int gridWidth = Math.Max(300, _grid.Columns.Where(c => c.Visible).Sum(c => c.Width));

            this.Size = new Size(gridWidth, gridHeight);
            _grid.Size = this.Size;
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_grid != null)
            {
                _grid.Theme = Theme;
            }
        }
        #endregion
    }

    // Assuming RowClickedEventArgs exists in BeepSimpleGrid
    public class RowClickedEventArgs : EventArgs
    {
        public BeepGridRow Row { get; }

        public RowClickedEventArgs(BeepGridRow row)
        {
            Row = row;
        }
    }
}