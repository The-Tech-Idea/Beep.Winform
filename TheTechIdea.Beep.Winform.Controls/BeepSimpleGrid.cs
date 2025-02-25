
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Design;
using TheTechIdea.Beep.Winform.Controls.Grid;
using TheTechIdea.Beep.Winform.Controls.Models;



namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A grid control that displays data in a simple table format with scrollbars.")]
    [DisplayName("Beep Simple Grid")]
    public class BeepSimpleGrid : BeepControl
    {
        #region Properties
        #region Title Properties
        private TextImageRelation textImageRelation = TextImageRelation.ImageAboveText;
        private string _titletext = "Simple BeepGrid";
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titletext;
            set
            {
                _titletext = value;
                if (titleLabel != null)
                {
                    titleLabel.Text = value;
                }
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Image alignment relative to text.")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                textImageRelation = value;
                if (titleLabel != null)
                {
                    titleLabel.TextImageRelation = value;
                }
                Invalidate();
            }
        }
        private Font _textFont;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TitleTextFont
        {
            get => _textFont ?? Font;
            set
            {
                _textFont = value;
                if (titleLabel != null)
                {
                    titleLabel.TextFont = _textFont;
                }
                Invalidate();
            }
        }
        private Font _columnHeadertextFont = new Font("Arial", 8);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for column headers.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font ColumnHeaderFont
        {
            get => _columnHeadertextFont;
            set
            {
                _columnHeadertextFont = value;
                Invalidate();
            }
        }
        #endregion "Title Properties"

        private int bottomPanelY;
        private int botomspacetaken = 0;
        private int topPanelY;
        private Rectangle gridRect;
        private int _rowHeight = 25;
        [Browsable(true)]
        [Category("Layout")]
        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                _rowHeight = value;
                InitializeRows();
                UpdateScrollBars();
                Invalidate();
            }
        }

        private bool _resizingColumn = false;
        private bool _resizingRow = false;
        private int _resizingIndex = -1;
        private int _resizeMargin = 2;
        private Point _lastMousePos;

        private BeepGridRow _hoveredRow = null;
        private BeepGridCell _hoveredCell = null;

        private List<Rectangle> sortIconBounds = new List<Rectangle>();
        private List<Rectangle> filterIconBounds = new List<Rectangle>();
        private int _defaultcolumnheaderheight = 25;
        private int _defaultcolumnheaderwidth = 50;

        private object _dataSource;
        private List<object> _fullData;
        private int _dataOffset = 0;
        public object DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                InitializeData();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        public string QueryFunctionName { get; set; }
        public string QueryFunction { get; set; }
        private int _xOffset = 0;
        private bool _showFilterButton = true;

        public BindingList<BeepGridRow> Rows { get; set; } = new BindingList<BeepGridRow>();
        public BeepGridRow BottomRow { get; set; }
        public BeepDataNavigator DataNavigator { get; set; }
        private List<BeepGridColumnConfig> _columns = new List<BeepGridColumnConfig>();
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(BeepGridColumnConfigCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<BeepGridColumnConfig> Columns
        {
            get => _columns;
            set
            {
                _columns = value ?? new List<BeepGridColumnConfig>();
                InitializeRows();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Layout")]
        public int DefaultColumnHeaderWidth
        {
            get => _defaultcolumnheaderwidth;
            set { _defaultcolumnheaderwidth = value; Invalidate(); }
        }

        private bool _showverticalgridlines = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowVerticalGridLines
        {
            get => _showverticalgridlines;
            set
            {
                _showverticalgridlines = value;
                Invalidate();
            }
        }
        private bool _showhorizontalgridlines = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHorizontalGridLines
        {
            get => _showhorizontalgridlines;
            set
            {
                _showhorizontalgridlines = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public bool ShowFilterButton
        {
            get => _showFilterButton;
            set
            {
                _showFilterButton = value;
                Invalidate();
            }
        }
        private bool _showSortIcons = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowSortIcons
        {
            get => _showSortIcons;
            set
            {
                _showSortIcons = value;
                Invalidate();
            }
        }
        private bool _showRowNumbers = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowRowNumbers
        {
            get => _showRowNumbers;
            set
            {
                _showRowNumbers = value;
                Invalidate();
            }
        }
        private bool _showColumnHeaders = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowColumnHeaders
        {
            get => _showColumnHeaders;
            set
            {
                _showColumnHeaders = value;
                Invalidate();
            }
        }
        private bool _showRowHeaders = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowRowHeaders
        {
            get => _showRowHeaders;
            set
            {
                _showRowHeaders = value;
                Invalidate();
            }
        }
        private bool _showNavigator = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set
            {
                _showNavigator = value;
                Invalidate();
            }
        }
        private bool _showBottomRow = false;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowBottomRow
        {
            get => _showBottomRow;
            set
            {
                _showBottomRow = value;
                Invalidate();
            }
        }
        private bool _showFooter = false;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowFooter
        {
            get => _showFooter;
            set
            {
                _showFooter = value;
                Invalidate();
            }
        }
        private bool _showHeaderPanel = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHeaderPanel
        {
            get => _showHeaderPanel;
            set
            {
                _showHeaderPanel = value;
                Invalidate();
            }
        }
        private bool _showHeaderPanelBorder = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHeaderPanelBorder
        {
            get => _showHeaderPanelBorder;
            set
            {
                _showHeaderPanelBorder = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public int ColumnHeight
        {
            get => _defaultcolumnheaderheight;
            set { _defaultcolumnheaderheight = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Layout")]
        [Description("Horizontal offset for drawing grid cells.")]
        public int XOffset
        {
            get => _xOffset;
            set
            {
                _xOffset = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        public GridDataSourceType DataSourceType { get; set; } = GridDataSourceType.Fixed;
        private string _title = "BeepSimpleGrid Title";
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        // Scrollbar Properties
        private BeepScrollBar _verticalScrollBar;
        private BeepScrollBar _horizontalScrollBar;
        private bool _showVerticalScrollBar = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowVerticalScrollBar
        {
            get => _showVerticalScrollBar;
            set
            {
                _showVerticalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        private bool _showHorizontalScrollBar = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHorizontalScrollBar
        {
            get => _showHorizontalScrollBar;
            set
            {
                _showHorizontalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        #endregion "Properties"

        #region Constructor
        public BeepSimpleGrid():base()
        {
            // This ensures child controls are painted properly
            this.SetStyle(ControlStyles.ContainerControl, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            Width = 200;
            Height = 200;
            // Attach event handlers for mouse actions
            this.MouseClick += BeepGrid_MouseClick;
            this.MouseDown += BeepGrid_MouseDown;
            this.MouseMove += BeepGrid_MouseMove;
            this.MouseUp += BeepGrid_MouseUp;
            Rows.ListChanged += Rows_ListChanged;
            ApplyThemeToChilds = false;
            // Initialize scrollbars immediately to ensure they’re never null
            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            Controls.Add(_horizontalScrollBar);
            DataNavigator = new BeepDataNavigator
            {
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ApplyThemeToChilds = true,
                Theme = Theme
            };
            Controls.Add(DataNavigator);

            titleLabel = new BeepLabel
            {
                Text = string.IsNullOrEmpty(Title) ? "Beep Grid" : Title,
                TextAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                MaxImageSize = new Size(20, 20),
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };
            titleLabel.BackColor = _currentTheme?.BackColor ?? Color.White;

            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            Controls.Add(_horizontalScrollBar);

            InitializeRows();
        }
        #endregion

        #region Initialization
        private void InitializeRows()
        {
            Rows.Clear();
            int visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
            int visibleRowCount = visibleHeight / _rowHeight;

            for (int i = 0; i < visibleRowCount; i++)
            {
                var row = new BeepGridRow();
                foreach (var col in Columns)
                {
                    // Create a new cell that will later be filled with data.
                    // Note: We no longer assign UIComponent here.
                    var cell = new BeepGridCell
                    {
                        // Do not default CellValue to string.Empty if FillVisibleRows will update it.
                        // You can leave it null or use a default if desired.
                        CellValue = null,
                        CellData = null,
                        IsEditable =true,
                        Index = col.Index,
                        IsVisible = col.Visible,
                        RowIdx = i

                    };
                    row.Cells.Add(cell);
                }
                Rows.Add(row);
            }

            UpdateScrollBars();
        }
        private void InitializeData()
        {
            _fullData = _dataSource is IEnumerable<object> enumerable ? enumerable.ToList() : new List<object>();
            _dataOffset = 0;

            if (_columns.Count == 0 && _fullData.Any())
            {
                var firstItem = _fullData.First();
                var properties = firstItem.GetType().GetProperties();
                int index = 0;
                foreach (var prop in properties)
                {
                    var columnType = MapPropertyTypeToDbFieldCategory(prop.PropertyType);
                    var cellEditor = MapPropertyTypeToCellEditor(prop.PropertyType);

                    var columnConfig = new BeepGridColumnConfig
                    {
                        ColumnCaption = prop.Name,
                        ColumnName = prop.Name,
                        Width = 100,
                        Index = index++,
                        Visible = true,
                        ColumnType = columnType,
                        CellEditor = cellEditor
                    };

                    _columns.Add(columnConfig);

                    
                }

                InitializeRows();
            }

            UpdateScrollBars();
        }
        private BeepControl CreateCellControlForEditing(BeepGridColumnConfig column)
        {
           // Console.WriteLine("YY"+column.CellEditor);
            //Debug.WriteLine("YY" + column.CellEditor);
            switch (column.CellEditor)
            {
               
                case BeepGridColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
                case BeepGridColumnType.CheckBox:
                    return new BeepCheckBox { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items) };
                case BeepGridColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true };
                case BeepGridColumnType.NumericUpDown:
                    return new BeepNumericUpDown { Theme = Theme, IsChild = true };
                case BeepGridColumnType.StarRating:
                    return new BeepStarRating { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Switch:
                    return new BeepSwitch { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ListBox:
                    return new BeepListofValuesBox { Theme = Theme, IsChild = true, ListItems = column.Items };
                default:
                    return new BeepTextBox { Theme = Theme, IsChild = true }; // Default fallback
            }
        }

        #endregion

        #region Data Filling and Navigation
        private void FillVisibleRows()
        {
            if (_fullData == null || !_fullData.Any()) return;

            for (int i = 0; i < Rows.Count; i++)
            {
                int dataIndex = _dataOffset + i;
                var row = Rows[i];
                if (dataIndex < _fullData.Count)
                {
                    var dataItem = _fullData[dataIndex];
                    for (int j = 0; j < row.Cells.Count && j < Columns.Count; j++)
                    {
                        var col = Columns[j];
                        var prop = dataItem.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                        var value = prop?.GetValue(dataItem)?.ToString() ?? string.Empty;
                        row.Cells[j].CellValue = value;
                        string colKey = col.ColumnName;
                        if(_editingCell != null && _editingCell.RowIdx == i && _editingCell.Index == j)
                        {
                            continue;
                        }
                        if (_columnEditors.TryGetValue(colKey, out IBeepUIComponent editor))
                        {
                            UpdateCellControl(editor, col, row.Cells[j].CellValue);
                        }

                    }
                }
                else
                {
                    foreach (var cell in row.Cells)
                    {
                        UpdateCellControl(cell.UIComponent, null, string.Empty);
                    }
                }
            }
            UpdateScrollBars();
        }

        public void MoveNext()
        {
            if (_dataOffset + Rows.Count < _fullData.Count)
            {
                _dataOffset++;
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }

        public void MovePrevious()
        {
            if (_dataOffset > 0)
            {
                _dataOffset--;
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        #endregion

        #region Control Creation and Updating
        private object CreateCellControl(BeepGridColumnConfig column)
        {
            switch (column.CellEditor)
            {
                case BeepGridColumnType.Text:
                    return new BeepTextBox { Theme = Theme };
                case BeepGridColumnType.CheckBox:
                    return new BeepCheckBox { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items) };
                case BeepGridColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Rating:
                case BeepGridColumnType.StarRating:
                    return new BeepStarRating { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Custom:
                    if (column.ColumnType == DbFieldCategory.Numeric)
                        return new BeepNumericUpDown { Theme = Theme, IsChild = true };
                    else if (column.ColumnType == DbFieldCategory.Boolean)
                        return new BeepSwitch { Theme = Theme, IsChild = true };
                    else if (column.Items != null && column.Items.Any())
                        return new BeepListofValuesBox { Theme = Theme, IsChild = true, ListItems = column.Items};
                    else
                        return new BeepTextBox { Theme = Theme, IsChild = true };
                default:
                    return new BeepLabel { Theme = Theme, IsChild = true };
            }
        }

        private void UpdateCellControl(IBeepUIComponent control, BeepGridColumnConfig column, object value)
        {
            if (control == null) return;
            string stringValue = value?.ToString() ?? ""; // Ensure it's always a string

            switch (control)
            {
                case BeepTextBox textBox:
                    textBox.Text = stringValue;
                    if (column != null)
                        textBox.MaskFormat = GetTextBoxMaskFormat(column.ColumnType);
                    break;

                case BeepCheckBox checkBox:
                    checkBox.CheckedValue = bool.TryParse(stringValue, out bool boolVal) && boolVal;
                    break;

                case BeepComboBox comboBox:
                    if (column?.Items != null)
                    {
                        var item = column.Items.FirstOrDefault(i => i.Value?.ToString() == stringValue || i.Text == stringValue);
                        if (item != null)
                        {
                            comboBox.SelectedItem = item;
                        }
                        else
                        {
                            comboBox.Text = stringValue;
                        }
                    }
                    else
                    {
                        comboBox.Text = stringValue;
                    }
                    break;

                case BeepDatePicker datePicker:
                    if (DateTime.TryParse(stringValue, out DateTime dateValue))
                    {
                        datePicker.SelectedDate = stringValue;
                    }
                    else
                    {
                        datePicker.SelectedDate = null; // Clear if invalid
                    }
                    break;

                case BeepImage image:
                    image.ImagePath = stringValue;
                    break;

                case BeepButton button:
                    button.Text = stringValue;
                    break;

                case BeepProgressBar progressBar:
                    progressBar.Value = int.TryParse(stringValue, out int progress) ? progress : 0;
                    break;

                case BeepStarRating starRating:
                    starRating.SelectedRating = int.TryParse(stringValue, out int rating) ? rating : 0;
                    break;

                case BeepNumericUpDown numericUpDown:
                    numericUpDown.Value = int.TryParse(stringValue, out int numVal) ? numVal : 0;
                    break;

                case BeepSwitch switchControl:
                    switchControl.Checked = bool.TryParse(stringValue, out bool switchVal) && switchVal;
                    break;

                case BeepListofValuesBox listBox:
                    listBox.SelectedKey = stringValue;
                    break;

                case BeepLabel label:
                    label.Text = stringValue;
                    break;
            }
        }

        private TextBoxMaskFormat GetTextBoxMaskFormat(DbFieldCategory columnType)
        {
            return columnType switch
            {
                DbFieldCategory.Numeric => TextBoxMaskFormat.Numeric,
                DbFieldCategory.Date => TextBoxMaskFormat.Date,
                DbFieldCategory.Timestamp => TextBoxMaskFormat.DateTime,
                DbFieldCategory.Currency => TextBoxMaskFormat.Currency,
                _ => TextBoxMaskFormat.None
            };
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
          //  base.OnPaint(e);
            UpdateDrawingRect();
            var g = e.Graphics;
            var drawingBounds = DrawingRect;

            bottomPanelY = drawingBounds.Bottom;
            botomspacetaken = 0;
            topPanelY = drawingBounds.Top;

            if (_showNavigator)
            {
                bottomPanelY -= navigatorPanelHeight;
                botomspacetaken += navigatorPanelHeight;
                navigatorPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), navigatorPanelHeight);
                DrawNavigationRow(g, navigatorPanelRect);
            }
            else
            {
                navigatorPanelRect = new Rectangle(-100, -100, drawingBounds.Width, navigatorPanelHeight);
                DrawNavigationRow(g, navigatorPanelRect);
            }

            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), footerPanelHeight);
                DrawFooterRow(g, footerPanelRect);
            }

            if (_showBottomRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), bottomagregationPanelHeight);
                DrawBottomAggregationRow(g, bottomagregationPanelRect);
            }

            if (_showHeaderPanel)
            {
                headerPanelHeight = titleLabel?.GetPreferredSize(Size.Empty).Height ?? headerPanelHeight;
                headerPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), headerPanelHeight);
                DrawHeaderPanel(g, headerPanelRect);
                topPanelY += headerPanelHeight;
                botomspacetaken += headerPanelHeight;
            }

            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), ColumnHeight);
                PaintColumnHeaders(g, columnsheaderPanelRect);
                topPanelY += ColumnHeight;
                botomspacetaken += ColumnHeight;
            }

            int availableHeight = drawingBounds.Height - botomspacetaken - (_horizontalScrollBar.Visible ? _horizontalScrollBar.Height : 0);
            int availableWidth = drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
            gridRect = new Rectangle(drawingBounds.Left, topPanelY, availableWidth, availableHeight);
           
            PaintRows(g, gridRect);

            if (_showverticalgridlines)
                DrawColumnBorders(g, gridRect);
            if (_showhorizontalgridlines)
                DrawRowsBorders(g, gridRect);

            PositionScrollBars();
        }
        private void DrawBottomAggregationRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.FillRectangle(brush, rect);
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                if (BottomRow != null)
                {
                    int xOffset = rect.Left + XOffset;
                    for (int i = 0; i < BottomRow.Cells.Count && i < Columns.Count; i++)
                    {
                        var cellRect = new Rectangle(xOffset, rect.Top, Columns[i].Width, rect.Height);
                        PaintCell(g, BottomRow.Cells[i], cellRect);
                        xOffset += Columns[i].Width;
                    }
                }
            }
        }
        private void DrawFooterRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.FillRectangle(brush, rect);
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                g.DrawString("Footer", Font, new SolidBrush(_currentTheme.PrimaryTextColor), rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }
        private void DrawNavigationRow(Graphics g, Rectangle rect)
        {
            DataNavigator.Location = new Point(rect.Left, rect.Top + 1);
            DataNavigator.Size = new Size(rect.Width, rect.Height);
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }
        }
        private void PaintColumnHeaders(Graphics g, Rectangle bounds)
        {
            int xOffset = bounds.Left - XOffset; // Ensure same offset as rows
            for (int i = 0; i < Columns.Count; i++)
            {
                var col = Columns[i];
                if (!col.Visible) continue;

                string headerText = col.ColumnCaption;
                var columnRect = new Rectangle(xOffset, bounds.Top, col.Width, bounds.Height);

                if (columnRect.Right >= bounds.Left && columnRect.Left < bounds.Right) // Only draw visible columns
                {
                    using (var textBrush = new SolidBrush(_currentTheme.ButtonForeColor))
                    {
                        g.DrawString(headerText, _columnHeadertextFont ?? Font, textBrush, columnRect,
                            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                }

                xOffset += col.Width;
                if (xOffset > bounds.Right) break; // Stop drawing when out of bounds
            }

            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
            }
        }
        private void PaintRows(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                var rowRect = new Rectangle(bounds.Left, yOffset, bounds.Width, RowHeight);
                PaintRow(g, row, rowRect);
                yOffset += _rowHeight;
                if (yOffset >= bounds.Bottom) break;
            }
        }
        private void PaintRow(Graphics g, BeepGridRow row, Rectangle rowRect)
        {
            int xOffset = rowRect.Left - XOffset; // Apply scrolling offset
            for (int i = 0; i < row.Cells.Count && i < Columns.Count; i++)
            {
                var cell = row.Cells[i];

                // 🔹 Update BeepGridCell with new calculated position
                cell.X = xOffset;
                cell.Y = rowRect.Top;
                cell.Width = Columns[i].Width;
                cell.Height = rowRect.Height;

                var cellRect = new Rectangle(cell.X, cell.Y, cell.Width, cell.Height);

                if (cellRect.Right >= gridRect.Left && cellRect.Left < gridRect.Right) // Draw only visible cells
                {
                    PaintCell(g, cell, cellRect);
                }

                xOffset += Columns[i].Width;
                if (xOffset > rowRect.Right) break; // Stop drawing if outside the visible area
            }
        }

        private void PaintCell(Graphics g, BeepGridCell cell, Rectangle cellRect)
        {
            // If this cell is being edited, skip drawing so that
            // the editor control remains visible.
            
            using (var cellBrush = new SolidBrush(_currentTheme.BackColor))
            {
                g.FillRectangle(cellBrush, cellRect);
            }

            if (cell.UIComponent == null)
            {
               // Console.WriteLine("Cell UIComponent is null");
                //Debug.WriteLine("Cell UIComponent is null");
                // Get the column editor if available
                if (_columnEditors.TryGetValue(Columns[cell.Index].ColumnName, out IBeepUIComponent columnEditor))
                {
                   // Console.WriteLine("Cell UIComponent is null 1");
                    //Debug.WriteLine("Cell UIComponent is null 1");
                    cell.UIComponent = columnEditor;
                }
                else
                {
                   // Console.WriteLine("Cell UIComponent is null 2");
                    //Debug.WriteLine("Cell UIComponent is null 2");
                    // Create a new control if it doesn't exist (failsafe)
                    cell.UIComponent = CreateCellControlForEditing(cell);
                    _columnEditors[Columns[cell.Index].ColumnName] = (IBeepUIComponent)cell.UIComponent;
                }
            }
           // Console.WriteLine("Cell UIComponent is null 3");
            //Debug.WriteLine("Cell UIComponent is null 3");
            if (cell.UIComponent != null)
            {
               // Console.WriteLine("Cell UIComponent is null 4");
                //Debug.WriteLine("Cell UIComponent is null 4");
                // 🔹 Correctly update control bounds
                var editor = (Control)cell.UIComponent;
                editor.Bounds = new Rectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
               // Console.WriteLine($"Cell {cell.CellValue}");
                //Debug.WriteLine($"Cell UIComponent is null 4");
                // 🔹 Update the control value to match the cell's data
                UpdateCellControl(cell.UIComponent, Columns[cell.Index], cell.CellValue);

                // 🔹 Draw the editor or its representation
                switch (cell.UIComponent)
                {
                    case BeepTextBox textBox:
                        textBox.Draw(g, cellRect);
                        break;
                    case BeepCheckBox checkBox:
                        checkBox.Draw(g, cellRect);
                        break;
                    case BeepComboBox comboBox:
                        comboBox.Draw(g, cellRect);
                        break;
                    case BeepDatePicker datePicker:
                        datePicker.Draw(g, cellRect);
                        break;
                    case BeepImage image:
                        image.DrawImage(g, cellRect);
                        break;
                    case BeepButton button:
                        button.Draw(g, cellRect);
                        break;
                    case BeepProgressBar progressBar:
                        progressBar.Draw(g, cellRect);
                        break;
                    case BeepStarRating starRating:
                        starRating.Draw(g, cellRect);
                        break;
                    case BeepNumericUpDown numericUpDown:
                        numericUpDown.Draw(g, cellRect);
                        break;
                    case BeepSwitch switchControl:
                        switchControl.Draw(g, cellRect);
                        break;
                    case BeepListofValuesBox listBox:
                        listBox.Draw(g, cellRect);
                        break;
                    case BeepLabel label:
                        label.Draw(g, cellRect);
                        break;
                    default:
                        g.DrawString(cell.UIComponent.ToString(), Font, new SolidBrush(_currentTheme.PrimaryTextColor),
                            cellRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                        break;
                }
            }
        }
        private void DrawColumnBorders(Graphics g, Rectangle bounds)
        {
            int xOffset = bounds.Left - XOffset; // Adjust for scrolling
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (!Columns[i].Visible) continue;
                    int columnRight = xOffset + Columns[i].Width;

                    if (columnRight >= bounds.Left && columnRight < bounds.Right) // Ensure only visible borders are drawn
                    {
                        g.DrawLine(pen, columnRight, bounds.Top, columnRight, bounds.Bottom);
                    }

                    xOffset += Columns[i].Width;
                    if (xOffset > bounds.Right) break; // Stop drawing when out of bounds
                }
            }
        }
        private void DrawRowsBorders(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    yOffset += _rowHeight;
                    if (yOffset < bounds.Bottom)
                        g.DrawLine(pen, bounds.Left, yOffset, bounds.Right, yOffset);
                }
            }
        }
        private void DrawHeaderPanel(Graphics g, Rectangle rect)
        {
            using (var borderPen = new Pen(_currentTheme.BorderColor))
            {
                if (ShowHeaderPanelBorder)
                {
                    g.DrawLine(borderPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                }
            }
            titleLabel.TextFont = _textFont;
            titleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            titleLabel.Size = new Size(rect.Width - 2, rect.Height - 2);
            titleLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            titleLabel.BackColor = _currentTheme.BackColor;
            titleLabel.Text = TitleText;
            titleLabel.Theme = Theme;
            titleLabel.DrawToGraphics(g, rect);
        }
        #endregion

        #region Scrollbar Management
        private void OnScroll(object sender, ScrollEventArgs e)
        {
             visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
             visibleRowCount = visibleHeight / RowHeight; // 🔹 Calculate dynamically

            _dataOffset = _verticalScrollBar.Value;
            Invalidate();
            //PositionControlsForVisibleCells(); // 🔹 Update visible controls dynamically
        }


        private void UpdateScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            if (_fullData == null || !_fullData.Any())
            {
                _verticalScrollBar.Visible = false;
                _horizontalScrollBar.Visible = false;
                return;
            }

            int totalRowHeight = _fullData.Count * _rowHeight;
            int totalColumnWidth = Columns.Sum(col => col.Width);
            int visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
            int visibleWidth = DrawingRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);

            // **Vertical ScrollBar Fix**
            if (_showVerticalScrollBar && totalRowHeight > visibleHeight)
            {
                int maxOffset = Math.Max(0, _fullData.Count - Rows.Count);
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = maxOffset;
                _verticalScrollBar.LargeChange = Rows.Count; // Number of visible rows
                _verticalScrollBar.SmallChange = 1;
                _verticalScrollBar.Value = Math.Min(_dataOffset, maxOffset);
                _verticalScrollBar.Visible = true;
            }
            else
            {
                _verticalScrollBar.Visible = false;
            }

            // **Horizontal ScrollBar Fix**
            if (_showHorizontalScrollBar && totalColumnWidth > visibleWidth)
            {
                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = Math.Max(0, totalColumnWidth - visibleWidth);
                _horizontalScrollBar.LargeChange = visibleWidth;
                _horizontalScrollBar.SmallChange = 10;
                _horizontalScrollBar.Value = Math.Max(0, _xOffset);
                _horizontalScrollBar.Visible = true;
            }
            else
            {
                _horizontalScrollBar.Visible = false;
                _xOffset = 0; // Reset offset when no scrollbar
            }

            PositionScrollBars();
        }


        private void PositionScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            int verticalScrollWidth = _verticalScrollBar.Width;
            int horizontalScrollHeight = _horizontalScrollBar.Height;
            int visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
            int visibleWidth = DrawingRect.Width;

            if (_verticalScrollBar.Visible)
            {
                _verticalScrollBar.Location = new Point(DrawingRect.Right - verticalScrollWidth, DrawingRect.Top + (ShowHeaderPanel ? headerPanelHeight : 0) + (ShowColumnHeaders ? ColumnHeight : 0));
                _verticalScrollBar.Height = visibleHeight - (_horizontalScrollBar.Visible ? horizontalScrollHeight : 0);
            }

            if (_horizontalScrollBar.Visible)
            {
                _horizontalScrollBar.Location = new Point(DrawingRect.Left, DrawingRect.Bottom - horizontalScrollHeight - (_showNavigator ? navigatorPanelHeight : 0));
                _horizontalScrollBar.Width = visibleWidth - (_verticalScrollBar.Visible ? verticalScrollWidth : 0);
            }
        }

        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (_verticalScrollBar != null)
            {
                int maxOffset = Math.Max(0, _fullData.Count - Rows.Count);
                _dataOffset = Math.Min(_verticalScrollBar.Value, maxOffset);
                FillVisibleRows();
                Invalidate();
            }
        }

        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (_horizontalScrollBar != null)
            {
                int totalColumnWidth = Columns.Sum(col => col.Width);
                int visibleWidth = DrawingRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
                _xOffset = Math.Min(_horizontalScrollBar.Value, Math.Max(0, totalColumnWidth - visibleWidth));
                Invalidate();
            }
        }

        #endregion

        #region Resizing Logic
        private void BeepGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsNearColumnBorder(e.Location, out _resizingIndex))
            {
                _resizingColumn = true;
                _lastMousePos = e.Location;
                this.Cursor = Cursors.SizeWE;
            }
            else if (IsNearRowBorder(e.Location, out _resizingIndex))
            {
                _resizingRow = true;
                _lastMousePos = e.Location;
                this.Cursor = Cursors.SizeNS;
            }
        }

        private void BeepGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizingColumn && _resizingIndex >= 0)
            {
                int deltaX = e.X - _lastMousePos.X;
                if (_resizingIndex < Columns.Count)
                {
                    Columns[_resizingIndex].Width = Math.Max(20, Columns[_resizingIndex].Width + deltaX); // Prevent negative width
                }
                _lastMousePos = e.Location;
                UpdateScrollBars();
                Invalidate();
            }
            else if (_resizingRow && _resizingIndex >= 0)
            {
                int deltaY = e.Y - _lastMousePos.Y;
                _rowHeight = Math.Max(10, _rowHeight + deltaY); // Prevent negative height
                _lastMousePos = e.Location;
                InitializeRows(); // Recreate rows with new height
                UpdateScrollBars();
                Invalidate();
            }
            else
            {
                if (IsNearColumnBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (IsNearRowBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }


        private void BeepGrid_MouseUp(object sender, MouseEventArgs e)
        {
            _resizingColumn = false;
            _resizingRow = false;
            this.Cursor = Cursors.Default;
            FillVisibleRows(); // Ensure data is redrawn after resizing
          //  Invalidate();
        }

        private bool IsNearColumnBorder(Point location, out int columnIndex)
        {
            // Calculate the X coordinate relative to the grid's content.
            // gridRect.Left is where the cells start; add XOffset to account for horizontal scrolling.
            int xRelative = location.X - gridRect.Left + XOffset;
            int currentX = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!Columns[i].Visible)
                    continue;
                currentX += Columns[i].Width;
                // Check if the point is within _resizeMargin pixels of this column’s right edge.
                if (Math.Abs(xRelative - currentX) <= _resizeMargin)
                {
                    columnIndex = i;
                    return true;
                }
            }
            columnIndex = -1;
            return false;
        }


        private bool IsNearRowBorder(Point location, out int rowIndex)
        {
            // Calculate the Y coordinate relative to the grid's drawing area (rows start at gridRect.Top).
            int yRelative = location.Y - gridRect.Top;
            int row = yRelative / RowHeight;
            if (row < 0 || row >= Rows.Count)
            {
                rowIndex = -1;
                return false;
            }
            // The bottom edge of this row (in relative coordinates) is at (row + 1) * RowHeight.
            int rowBottom = (row + 1) * RowHeight;
            if (Math.Abs(yRelative - rowBottom) <= _resizeMargin)
            {
                rowIndex = row;
                return true;
            }
            rowIndex = -1;
            return false;
        }




        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
           // InitializeRows();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }
        #endregion
        #region Editor
        // One editor control per column (keyed by column index)
        private Dictionary<string, IBeepUIComponent> _columnEditors = new Dictionary<string, IBeepUIComponent>();
        // The currently active editor and cell being edited
        private BeepControl _editingControl = null;
        private BeepGridCell _editingCell = null;
        private IBeepComponentForm _editorPopupForm;

        private void ShowCellEditor(BeepGridCell cell, Point location)
        {
            if (!cell.IsEditable)
                return;

            int colIndex = cell.Index;
            string columnName = Columns[colIndex].ColumnName;

            // Close any existing editor before opening a new one
            CloseCurrentEditor();

            _editingCell = cell;
            Size cellSize = new Size(cell.Width, cell.Height);
            // Create the popup form if it's null
            if (_editorPopupForm == null)
            {
                _editorPopupForm = new IBeepComponentForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.Manual,
                    ShowInTaskbar = false,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    TopMost = true,
                    MinimumSize = cellSize, // Force exact size
                    MaximumSize = cellSize
                };

            }
            // Apply size
            _editorPopupForm.Size = cellSize;
            _editorPopupForm.ClientSize = cellSize;
            // Retrieve or create editor control
            if (!_columnEditors.TryGetValue(columnName, out IBeepUIComponent Ieditor))
            {
                _editingControl = CreateCellControlForEditing(cell);
                _columnEditors[columnName] = _editingControl;
            }
            else
            {
                _editingControl = (BeepControl)Ieditor;
            }

            // Ensure the control is not disposed before using it
            if (_editingControl.IsDisposed)
            {
                _editingControl = CreateCellControlForEditing(cell);
                _columnEditors[columnName] = _editingControl;
            }

            // Ensure the control is only added once
            if (_editingControl.Parent != _editorPopupForm)
            {
                _editorPopupForm.AddComponent(_editingControl);
            }
            _editingControl.Theme = Theme;
         
            // **🔹 Force the popup form and editor control to match the exact cell size**
           // _editingControl.IsFrameless = true;
         //   _editingControl.ShowAllBorders = false;
            Debug.WriteLine($"Cell size: {cellSize}");
            _editorPopupForm.ClientSize = cellSize; // 👈 Ensures no extra space due to window frame
            _editingControl.Size = cellSize; // 👈 Matches cell exactly
            _editingControl.Dock = DockStyle.Fill; // 👈 Ensures it doesn't resize incorrectly
            Debug.WriteLine($"Editor size: {_editingControl.Size}");
            Debug.WriteLine($"Popup size: {_editorPopupForm.ClientSize}");
            // **🔹 Position popup exactly at the cell location (relative to BeepSimpleGrid)**
            Point screenLocation = this.PointToScreen(new Point(cell.X, cell.Y));
            _editorPopupForm.Location = screenLocation;
            _editingControl.Theme = Theme;
            // **Set initial text**
            //  _editingControl.Text = cell.CellValue;
            _editingControl.Focus();
            UpdateCellControl(_editingControl, Columns[colIndex], cell.CellValue);
            // 🔹 Confirm value in editor **after setting**
            Debug.WriteLine($"✅ Editor Text After Update: {_editingControl.Text}");
            // Handle when the popup loses focus
            _editorPopupForm.Deactivate += (s, e) => CloseCurrentEditor();
            //_editorPopupForm.Shown += (s, e) =>
            //{
            //    Debug.WriteLine($"🔄 Setting BeepTextBox text after popup is visible: {cell.CellValue}");
            //    
            //};
            //_editingControl.Invalidate();
            _editorPopupForm.Show();
            _editorPopupForm.SetValue(cell.CellValue);
            Debug.WriteLine($"✅ after show Editor Text After Update: {_editingControl.Text}");
        }
        private void CloseCurrentEditor()
        {
            if (_editingControl != null)
            {
                // Save the current value before closing
             
                Debug.WriteLine($"✅ before closing Editor Text After Update: {_editingControl.Text}");

                SaveEditedValue();
                // Remove editor control from popup (prevents premature disposal)
                if (_editorPopupForm != null && _editingControl.Parent == _editorPopupForm)
                {
                    _editorPopupForm.Controls.Remove(_editingControl);
                }

                // Close and dispose of the popup form safely
                if (_editorPopupForm != null)
                {
                    _editorPopupForm.Close();
                    _editorPopupForm.Dispose();
                    _editorPopupForm = null;
                }

                // Reset references (but don't dispose `_editingControl` yet)
                _editingCell = null;
                _editingControl = null;

                Debug.WriteLine("Popup editor closed successfully.");
            }
        }
        private BeepControl CreateCellControlForEditing(BeepGridCell cell)
        {
            // Get the column definition based on cell.Index.
            var column = Columns[cell.Index];

            switch (column.CellEditor)
            {
                case BeepGridColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
                case BeepGridColumnType.CheckBox:
                    return new BeepCheckBox { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items) };
                case BeepGridColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true };
                case BeepGridColumnType.NumericUpDown:
                    return new BeepNumericUpDown { Theme = Theme, IsChild = true };
                default:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
            }
        }
        private void EditingControl_LostFocus(object sender, EventArgs e)
        {
            // Optionally, check if the mouse is not over the editor before closing.
            CloseCurrentEditor();
        }
        private void EditingControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveEditedValue();
                CloseCurrentEditor();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CancelEditing();
                CloseCurrentEditor();
            }
        }
        private void SaveEditedValue()
        {
            if (_editingControl != null && _editingCell != null)
            {
                IBeepUIComponent component = (IBeepUIComponent)_editingControl;
                object newValue = component.GetValue();

                // Update cell's stored value
                _editingCell.OldValue = _editingCell.CellData;
                _editingCell.CellData = newValue;
                _editingCell.CellValue = newValue;
                _editingCell.IsDirty = true;

                // Fire cell validation event (if any)
                _editingCell.ValidateCell();

                Invalidate(); // Redraw grid if necessary
            }
        }
        private void CancelEditing()
        {
            // Optionally, revert the editor's value if needed.
            CloseCurrentEditor();
        }
        private void BeepGrid_MouseClick(object sender, MouseEventArgs e)
        {
            // If clicked cell is null → close editor
            var clickedCell = GetCellAtLocation(e.Location);
            if (_editingCell!=null)
            {
                CloseCurrentEditor();
                return;
            }

            // Or if the clicked cell is different from the editing cell, close the old one and open new
            if (clickedCell != _editingCell)
            {
                
                CloseCurrentEditor();
                _editingCell = clickedCell;
                if (clickedCell.IsEditable)
                    ShowCellEditor(clickedCell,e.Location);
            }
        }

        private BeepGridCell GetCellAtLocation(Point location)
        {
            // First, ensure the point is inside the grid's drawing area.
            if (!gridRect.Contains(location))
                return null;

            // Compute the Y coordinate relative to gridRect.
            int yRelative = location.Y - gridRect.Top;
            int rowIndex = yRelative / RowHeight;
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return null;
            var row = Rows[rowIndex];

            // Compute the X coordinate relative to gridRect.
            // Note: We add XOffset because our content is shifted to the right by the scroll offset.
            int xRelative = location.X - gridRect.Left + XOffset;
            int currentX = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!Columns[i].Visible)
                    continue;
                if (xRelative >= currentX && xRelative < currentX + Columns[i].Width)
                {
                    // Return the cell at column i (assuming 1-to-1 correspondence).
                    return row.Cells[i];
                }
                currentX += Columns[i].Width;
            }
            return null;
        }
        private Rectangle GetCellRectangle(BeepGridCell cell)
        {
            // Find which row and column this cell belongs to.
            int rowIndex = -1, colIndex = -1;
            for (int r = 0; r < Rows.Count; r++)
            {
                int c = Rows[r].Cells.IndexOf(cell);
                if (c != -1)
                {
                    rowIndex = r;
                    colIndex = c;
                    break;
                }
            }
            if (rowIndex == -1 || colIndex == -1)
                return Rectangle.Empty;

            // Calculate Y coordinate: gridRect.Top is the start of rows.
            int y = gridRect.Top + rowIndex * RowHeight;

            // Calculate X coordinate: start at gridRect.Left and add the widths of the previous visible columns.
            int x = gridRect.Left - XOffset;
            for (int i = 0; i < colIndex; i++)
            {
                if (Columns[i].Visible)
                    x += Columns[i].Width;
            }
            int width = Columns[colIndex].Width;
            int height = RowHeight;
            return new Rectangle(x, y, width, height);
        }

        #endregion Editor
        #region Header Layout
        protected int headerPanelHeight = 20;
        protected int bottomagregationPanelHeight = 12;
        protected int footerPanelHeight = 12;
        protected int navigatorPanelHeight = 20;

        private Rectangle footerPanelRect;
        private Rectangle headerPanelRect;
        private Rectangle columnsheaderPanelRect;
        private Rectangle bottomagregationPanelRect;
        private Rectangle navigatorPanelRect;
        private Rectangle filterButtonRect;
        private BeepLabel titleLabel;
        private BeepButton filterButton;
        private int visibleHeight;
        private int visibleRowCount;
        #endregion

        #region Events
        private void Rows_ListChanged(object sender, ListChangedEventArgs e) => Invalidate();
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            this.BackColor = _currentTheme.GridBackColor;
            this.ForeColor = _currentTheme.GridForeColor;
            if (titleLabel != null)
            {
                titleLabel.Theme = Theme;
            }
            if (DataNavigator != null)
            {
                _currentTheme.ButtonBackColor = _currentTheme.GridBackColor;
                _currentTheme.ButtonForeColor = _currentTheme.GridForeColor;
                DataNavigator.Theme = Theme;
            }
            if (_verticalScrollBar != null)
            {
                _verticalScrollBar.Theme = Theme;
            }
            if (_horizontalScrollBar != null)
            {
                _horizontalScrollBar.Theme = Theme;
            }
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (cell.UIComponent is BeepControl ctrl)
                    {
                        ctrl.Theme = Theme;
                    }
                }
            }
            Invalidate();
        }
        #endregion

        #region Helper Methods
        private DbFieldCategory MapPropertyTypeToDbFieldCategory(Type type)
        {
            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            // Default to string for unknown types
            return DbFieldCategory.String;
        }

        private BeepGridColumnType MapPropertyTypeToCellEditor(Type type)
        {
            if (type == typeof(string)) return BeepGridColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepGridColumnType.NumericUpDown; // Integer-based controls
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepGridColumnType.Text; // Use a formatted text field for decimal numbers
            if (type == typeof(DateTime)) return BeepGridColumnType.DateTime;
            if (type == typeof(bool)) return BeepGridColumnType.CheckBox;
            if (type == typeof(Guid)) return BeepGridColumnType.Text; // Could be a readonly label or a dropdown if referencing something

            // Fallback to text if the type is unknown
            return BeepGridColumnType.Text;
        }

        #endregion
    }


}