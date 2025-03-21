using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum FieldOrientation
    {
        Horizontal,
        Vertical
    }

    public enum RecordStatus
    {
        New,
        Updated,
        Unchanged
    }

    public class BeepDataRecord : BeepControl, IEditableObject
    {
        #region Properties

        private Type _currentRowType;
        private Panel _mainPanel; // Main container
        private TableLayoutPanel _tableLayout;
        private int _fieldCount;
        private object _originalDataRecord;
        public bool AutoSave { get; set; } = false;
        private bool _isdirty = false;

        public bool IsDirty
        {
            get { _isdirty = RecordStatus == RecordStatus.Updated; return _isdirty; }
            set { _isdirty = value; }
        }

        private Dictionary<string, object> _originalValues = new Dictionary<string, object>();

        [Category("Data")]
        public object DataRecord { get; private set; }

        [Category("Data")]
        public string DataRecordType { get; private set; }

        private RecordStatus _recordStatus = RecordStatus.Unchanged;

        [Category("Data Status")]
        public RecordStatus RecordStatus
        {
            get => _recordStatus;
            private set
            {
                if (_recordStatus != value)
                {
                    _recordStatus = value;
                    RecordStatusChanged?.Invoke(this, _recordStatus);
                }
            }
        }

        [Category("Data Status")]
        public bool IsNewRecord => RecordStatus == RecordStatus.New;

        [Category("Data Status")]
        public bool IsUpdatedRecord => RecordStatus == RecordStatus.Updated;

        [Category("Layout")]
        public int FieldCount
        {
            get => _fieldCount;
            set
            {
                if (_fieldCount != value)
                {
                    _fieldCount = value;
                    UpdateLayout();
                }
            }
        }

        private List<IBeepUIComponent> _fields;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<IBeepUIComponent> Fields
        {
            get => _fields;
            private set
            {
                _fields = value ?? new List<IBeepUIComponent>();
                UpdateLayout();
            }
        }

        [Category("Layout")]
        public FieldOrientation FieldOrientation { get; set; } = FieldOrientation.Horizontal;

        [Category("Labels")]
        public bool ShowFieldPrompts { get; set; } = false;

        [Browsable(false)]
        public Dictionary<string, string> FieldPrompts { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Events

        public event EventHandler<BeepComponentEventArgs> FieldValueChanged;
        public event EventHandler<BeepComponentEventArgs> FieldSelected;
        public event EventHandler<bool> RecordValidated;
        public event EventHandler<RecordStatus> RecordStatusChanged;
        public event EventHandler<IBeepUIComponent> OnFieldCreated;

        #endregion

        #region Constructor

        public BeepDataRecord() : base()
        {
            InitializeComponent();
            this.AutoScroll = false;
        }

        public BeepDataRecord(object row, bool isNew = false) : this()
        {
            SetDataRecord(row, isNew);
        }

        private void InitializeComponent()
        {
            _fields = new List<IBeepUIComponent>();

            _mainPanel = new Panel // Replace with BeepPanel if available
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = _currentTheme?.PanelBackColor ?? Color.White
            };
            Controls.Add(_mainPanel);

            _tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoScroll = false
            };
            _mainPanel.Controls.Add(_tableLayout);

            ApplyTheme();
        }

        #endregion

        #region Data Handling

        public void SetDataRecord(object row, bool isNew = false)
        {
            if (row == null) return;

            if (_currentRowType == row.GetType())
            {
                DataRecord = row;
                BindDataRecord();
            }
            else
            {
                _currentRowType = row.GetType();
                DataRecord = row;
                GenerateFieldsFromDataRecord();
            }

            _originalDataRecord = EntityHelper.DeepCopyUsingSerialize(row);
            _originalValues.Clear();
            foreach (var prop in row.GetType().GetProperties())
            {
                _originalValues[prop.Name] = prop.GetValue(row);
            }
            RecordStatus = isNew ? RecordStatus.New : RecordStatus.Unchanged;
        }

        public void UndoChanges()
        {
            if (_originalDataRecord != null)
            {
                SetDataRecord(EntityHelper.DeepCopyUsingSerialize(_originalDataRecord));
            }
        }

        public void BindDataRecord()
        {
            if (DataRecord == null || _fields == null) return;

            foreach (var field in _fields)
            {
                if (!string.IsNullOrEmpty(field.BoundProperty))
                {
                    var propInfo = DataRecord.GetType().GetProperty(field.BoundProperty);
                    if (propInfo != null)
                    {
                        var value = propInfo.GetValue(DataRecord);
                        field.SetValue(value);
                    }
                }
            }
        }

        public void UpdateDataRecord()
        {
            if (DataRecord == null || _fields == null) return;

            foreach (var field in _fields)
            {
                if (!string.IsNullOrEmpty(field.BoundProperty))
                {
                    var propInfo = DataRecord.GetType().GetProperty(field.BoundProperty);
                    if (propInfo != null)
                    {
                        var newValue = field.GetValue();
                        propInfo.SetValue(DataRecord, newValue);
                    }
                }
            }

            RecordStatus = RecordStatus.Updated;
        }

        #endregion

        #region Field Generation

        public void GenerateFieldsFromDataRecord()
        {
            if (DataRecord == null) return;

            _fields.Clear();
            var properties = DataRecord.GetType().GetProperties();

            foreach (var prop in properties)
            {
                IBeepUIComponent field = ControlExtensions.CreateFieldBasedOnCategory(prop.Name, prop.PropertyType);
                if (field != null)
                {
                    // Configure the control
                    field.ComponentName = prop.Name;
                    field.BoundProperty = prop.Name;
                  
                    if (field is BeepTextBox textBox)
                    {
                        textBox.ShowAllBorders = false;
                    }
                    else if (field is BeepComboBox comboBox && prop.PropertyType.IsEnum)
                    {
                        // Populate combo box with enum values if applicable
                        comboBox.ListItems = new BindingList<SimpleItem>(
                            Enum.GetValues(prop.PropertyType)
                                .Cast<object>()
                                .Select(e => new SimpleItem { Text = e.ToString(), Value = e })
                                .ToList()
                        );
                    }

                    field.OnValueChanged += Field_OnValueChanged;
                    field.OnValidate += Field_OnValidate;
                    field.OnSelected += Field_OnSelected;

                    field.Theme = Theme;
                    field.ApplyTheme();

                    OnFieldCreated?.Invoke(this, field);
                    _fields.Add(field);
                }
            }
            FieldCount = Fields.Count;
            UpdateLayout();
            BindDataRecord();
        }

        #endregion

        #region Layout Management

        public void UpdateLayout()
        {
            if (_tableLayout == null) return;

            _tableLayout.SuspendLayout();
            _tableLayout.Controls.Clear();
            _tableLayout.RowStyles.Clear();
            _tableLayout.ColumnStyles.Clear();

            if (_fields == null || _fields.Count == 0)
            {
                SetupEmptyLayout();
                return;
            }

            if (FieldOrientation == FieldOrientation.Horizontal)
                SetupHorizontalLayout();
            else
                SetupVerticalLayout();

            _tableLayout.ResumeLayout(true);
            _mainPanel.Invalidate();
        }

        private void SetupHorizontalLayout()
        {
            int columns = FieldCount;
            int rows = ShowFieldPrompts ? 2 : 1;

            _tableLayout.ColumnCount = columns;
            _tableLayout.RowCount = rows;

            for (int i = 0; i < columns; i++)
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));

            for (int i = 0; i < rows; i++)
                _tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            int fieldIndex = 0;
            for (int col = 0; col < columns && fieldIndex < _fields.Count; col++)
            {
                var field = _fields[fieldIndex];
                if (field is Control control)
                {
                    control.Dock = DockStyle.Fill;
                    if (ShowFieldPrompts)
                    {
                        var label = CreateFieldLabel(field);
                        _tableLayout.Controls.Add(label, col, 0);
                        _tableLayout.Controls.Add(control, col, 1);
                    }
                    else
                    {
                        _tableLayout.Controls.Add(control, col, 0);
                    }
                }
                fieldIndex++;
            }
        }

        private void SetupVerticalLayout()
        {
            int columns = ShowFieldPrompts ? 2 : 1;
            int rows = _fields.Count;

            _mainPanel.Controls.Clear();
            _tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoScroll = false
            };
            _mainPanel.Controls.Add(_tableLayout);

            _tableLayout.ColumnCount = columns;
            _tableLayout.RowCount = rows;

            if (ShowFieldPrompts)
            {
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150f));
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            }
            else
            {
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            }

            for (int i = 0; i < rows; i++)
                _tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            int row = 0;
            foreach (var field in _fields)
            {
                if (field is Control control)
                {
                    control.Dock = DockStyle.Fill;
                    if (ShowFieldPrompts)
                    {
                        var label = CreateFieldLabel(field);
                        _tableLayout.Controls.Add(label, 0, row);
                        _tableLayout.Controls.Add(control, 1, row);
                    }
                    else
                    {
                        _tableLayout.Controls.Add(control, 0, row);
                    }
                }
                row++;
            }
        }

        private void SetupEmptyLayout()
        {
            _tableLayout.RowCount = 1;
            _tableLayout.ColumnCount = 1;
            _tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            var placeholder = new BeepLabel
            {
                Text = "No fields available",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Theme = Theme
            };
            placeholder.ApplyTheme();
            _tableLayout.Controls.Add(placeholder, 0, 0);
        }

        private BeepLabel CreateFieldLabel(IBeepUIComponent field)
        {
            var label = new BeepLabel
            {
                Text = FieldPrompts.ContainsKey(field.ComponentName)
                    ? FieldPrompts[field.ComponentName]
                    : field.ComponentName.Replace("_", " "),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Theme = Theme
            };
            label.ApplyTheme();
            return label;
        }

        #endregion

        #region Event Handlers

        private void Field_OnValueChanged(object sender, BeepComponentEventArgs e)
        {
            FieldValueChanged?.Invoke(this, e);

            if (DataRecord != null)
            {
                var propInfo = DataRecord.GetType().GetProperty(e.PropertyName);
                if (propInfo != null)
                {
                    propInfo.SetValue(DataRecord, e.PropertyValue);
                }
            }

            RecordStatus = RecordStatus.Updated;
            ValidateRecord();
        }

        public void UndoField(string fieldName)
        {
            if (_originalValues.ContainsKey(fieldName) && DataRecord != null)
            {
                var propInfo = DataRecord.GetType().GetProperty(fieldName);
                if (propInfo != null)
                {
                    propInfo.SetValue(DataRecord, _originalValues[fieldName]);
                    BindDataRecord();
                }
            }
        }

        private void Field_OnValidate(object sender, BeepComponentEventArgs e)
        {
            ValidateRecord();
        }

        private void Field_OnSelected(object sender, BeepComponentEventArgs e)
        {
            FieldSelected?.Invoke(this, e);
        }

        private void ValidateRecord()
        {
            bool isValid = true;
            foreach (var field in _fields)
            {
                if (field.IsRequired && string.IsNullOrEmpty(field.GetValue()?.ToString()))
                {
                    isValid = false;
                    field.BorderColor = Color.Red;
                    field.ShowToolTip("This field is required.");
                }
                else
                {
                    field.BorderColor = Color.Gray;
                    field.HideToolTip();
                }
            }
            RecordValidated?.Invoke(this, isValid);
        }

        #endregion

        #region IEditableObject Implementation

        public void BeginEdit()
        {
            _originalDataRecord = EntityHelper.DeepCopyUsingSerialize(DataRecord);
        }

        public void CancelEdit()
        {
            if (_originalDataRecord != null)
            {
                SetDataRecord(EntityHelper.DeepCopyUsingSerialize(_originalDataRecord));
                _originalDataRecord = null;
            }
        }

        public void EndEdit()
        {
            _originalDataRecord = null;
            RecordStatus = RecordStatus.Updated;
        }

        #endregion

        #region Theme Application

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            _mainPanel.BackColor = _currentTheme.PanelBackColor;
            _tableLayout.BackColor = _currentTheme.PanelBackColor;
            foreach (var field in _fields)
            {
                field.Theme = Theme;
                field.ApplyTheme();
            }
        }

        #endregion

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tableLayout?.Dispose();
                _mainPanel?.Dispose();
                foreach (var field in _fields)
                {
                    if (field is Control control) control.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}