using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing constructor and lifecycle management for BeepSimpleGrid
    /// </summary>
    public partial class BeepSimpleGrid
    {
        /// <summary>
        /// Initializes a new instance of the BeepSimpleGrid class
        /// </summary>
        public BeepSimpleGrid()
        {
            // Initialize default values
            InitializeDefaultValues();
            
            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.EnableNotifyMessage, true);

            // Initialize rows collection
            if (Rows == null)
            {
                Rows = new BindingList<BeepRowConfig>();
            }
            Rows.ListChanged += Rows_ListChanged;

            // Initialize all embedded controls
            InitializeControls();
            _isInitializing = false;
            // Set default size
            Size = new System.Drawing.Size(800, 400);
        }

        /// <summary>
        /// Initializes default values for all fields
        /// </summary>
        private void InitializeDefaultValues()
        {
            _rowHeight = 25;
            _rowHeights = new System.Collections.Generic.Dictionary<int, int>();
            _fullData = new System.Collections.Generic.List<object>();
            originalList = new System.Collections.Generic.List<object>();
            deletedList = new System.Collections.Generic.List<object>();
            _selectedRows = new System.Collections.Generic.List<int>();
            _selectedgridrows = new System.Collections.Generic.List<BeepRowConfig>();
            _persistentSelectedRows = new System.Collections.Generic.Dictionary<int, bool>();
            ChangedValues = new System.Collections.Generic.Dictionary<object, System.Collections.Generic.Dictionary<string, object>>();
            _columns = new System.Collections.Generic.List<BeepColumnConfig>();
            Trackings = new System.Collections.Generic.List<Tracking>();
            UpdateLog = new System.Collections.Generic.Dictionary<System.DateTime, EntityUpdateInsertLog>();
            buttons = new System.Collections.Generic.List<Control>();
            pagingButtons = new System.Collections.Generic.List<Control>();
            _columnDrawers = new System.Collections.Generic.Dictionary<string, IBeepUIComponent>();
            _columnEditors = new System.Collections.Generic.Dictionary<string, IBeepUIComponent>();
            _controlPool = new System.Collections.Generic.Dictionary<BeepColumnType, System.Collections.Generic.List<IBeepUIComponent>>();
            _cellBounds = new System.Collections.Generic.Dictionary<int, System.Drawing.Rectangle>();
            columnHeaderBounds = new System.Collections.Generic.List<System.Drawing.Rectangle>();
            rowHeaderBounds = new System.Collections.Generic.List<System.Drawing.Rectangle>();
            sortIconBounds = new System.Collections.Generic.List<System.Drawing.Rectangle>();
            filterIconBounds = new System.Collections.Generic.List<System.Drawing.Rectangle>();
            _columnTextSizes = new System.Collections.Generic.Dictionary<string, System.Drawing.Size>();
            _navigationButtonCache = new System.Collections.Generic.Dictionary<string, BeepButton>();
            _scaledColumnWidths = new System.Collections.Generic.Dictionary<int, int>();
            _hitAreas = new System.Collections.Generic.Dictionary<string, System.Drawing.Rectangle>();
        }

        /// <summary>
        /// Initializes the grid when the control is first loaded
        /// </summary>
        private void InitializeGridOnLoad()
        {
            // Initialize controls after the control handle is created
            InitializeControls();
            
            // Apply pending data source if any
            if (_pendingDataSource != null)
            {
                DataSource = _pendingDataSource;
                _pendingDataSource = null;
            }

            _isInitializing = false;
            
            // Initialize layout
            _layoutDirty = true;
            RecalculateGridRect();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        /// <summary>
        /// Cleans up resources when the control is disposed
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of timers
                if (_fillRowsTimer != null)
                {
                    _fillRowsTimer.Dispose();
                    _fillRowsTimer = null;
                }

                if (_scrollTimer != null)
                {
                    _scrollTimer.Dispose();
                    _scrollTimer = null;
                }

                if (_resizeTimer != null)
                {
                    _resizeTimer.Dispose();
                    _resizeTimer = null;
                }

                // Dispose of rendering resources
                _cellBrush?.Dispose();
                _selectedCellBrush?.Dispose();
                _borderPen?.Dispose();
                _selectedBorderPen?.Dispose();
                _columnHeadertextFont?.Dispose();
                _textFont?.Dispose();

                // Detach events
                if (_bindingSource != null)
                {
                    _bindingSource.ListChanged -= BindingSource_ListChanged;
                }

                if (Rows != null)
                {
                    Rows.ListChanged -= Rows_ListChanged;
                }

                if (_selectAllCheckBox != null)
                {
                    _selectAllCheckBox.StateChanged -= SelectAllCheckBox_StateChanged;
                }

                // Clear collections
                _fullData?.Clear();
                originalList?.Clear();
                deletedList?.Clear();
                _selectedRows?.Clear();
                _persistentSelectedRows?.Clear();
                ChangedValues?.Clear();
                buttons?.Clear();
                pagingButtons?.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
