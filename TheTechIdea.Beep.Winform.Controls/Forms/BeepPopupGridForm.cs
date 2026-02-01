using System;
using System.Collections;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;

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
                _grid.IsInitializing = false;
                _grid.DataSource = value;
  //              AdjustSize();
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
            this.ShowCaptionBar= false;
            this.ShowIcon = false;
          
            this.ShowInTaskbar = false;
            this.Size = new Size(300, 200);
            this.Deactivate += (s, e) => this.CloseCascade();
            this.ResumeLayout(false);
            InitializeGrid();
        }

        public void Init(object dataSource, List<BeepColumnConfig> columns)
        {
            
            Columns = columns;
            DataSource = dataSource;
        }
        #endregion

        #region Initialization
      

        private void InitializeGrid()
        {
            _grid = new BeepSimpleGrid()
            {
                Dock = DockStyle.Fill,
                ShowColumnHeaders = false,
                ShowNavigator = false,
                Theme = Theme
            };
            _grid.CurrentRowChanged += Grid_SelectedRowChanged; // Updated to match BeepSimpleGrid
            this.Controls.Add(_grid);
        }
        #endregion

        #region Event Handlers
        private void Grid_SelectedRowChanged(object sender, BeepRowSelectedEventArgs e)
        {
            if (e.Row != null && e.Row.RowData != null)
            {
                SelectedRowData = e.Row.RowData;
                OnRowSelected(SelectedRowData);
              //  _grid.Dispose();
                this.Close();
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
            base.ShowPopup(anchorPoint, position);
            return SelectedRowData;
        }
        // New method to change column width
        public void SetColumnWidth(string columnName, int width)
        {
            var column = _grid.GetColumnByName(columnName);
            if (column != null)
            {
                column.Width = Math.Max(20, width); // Ensure minimum width
              //  AdjustSize();
                _grid.Invalidate(); // Redraw grid with updated column width
            }
            else
            {
               ////MiscFunctions.SendLog($"SetColumnWidth: Column '{columnName}' not found.");
            }
        }
        public object ShowPopupList(Control triggerControl, object griddata, string keycolumn, string valuecolumn, int keyColumnWidth, int valueColumnWidth)
        {
            if (griddata == null || !(griddata is IList || griddata is IEnumerable))
            {
               ////MiscFunctions.SendLog("ShowPopupList: Invalid griddata - must be IList or IEnumerable");
                return null;
            }
            _grid.ShowHeaderPanel = false;
            DataSource = griddata; // Let BeepSimpleGrid auto-generate columns

            foreach (var column in Columns)
            {
                if (column.ColumnName.Equals( keycolumn,StringComparison.InvariantCultureIgnoreCase))
                {
                    column.Width = keyColumnWidth;
                    column.Visible = true;
                    column.ReadOnly = true;
                }
                else if (column.ColumnName.Equals(valuecolumn, StringComparison.InvariantCultureIgnoreCase))
                {
                    column.Width = valueColumnWidth;
                    column.Visible = true;
                    column.ReadOnly = true;
                }
                else
                {
                    column.Visible = false;
                }
            }
            // rearrange columns to ensure key column is first
            Columns.Sort((a, b) => a.ColumnName.Equals(keycolumn, StringComparison.InvariantCultureIgnoreCase) ? -1 : 1);
            AdjustSize();
            base.ShowPopup(triggerControl, BeepPopupFormPosition.Bottom);
            return SelectedRowData;
        }

      
        #endregion

        #region Helper Methods
        private void AdjustSize()
        {
            if (_grid.Rows == null || _grid.Rows.Count == 0) return;

            int rowHeight = _grid.RowHeight;
            int visibleRowCount = Math.Min(_grid.Rows.Count, 10);
            int gridHeight = visibleRowCount * rowHeight + (_grid.BorderThickness * 2)+10;
            int gridWidth = Columns.Where(c => c.Visible).Sum(c => c.Width)+10;

            this.Size = new Size(gridWidth, gridHeight);
            _grid.Size = this.Size;
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
         //   base.ApplyTheme();
            if (_grid != null)
            {
                _grid.Theme = Theme;
            }
        }
        #endregion
    }
}