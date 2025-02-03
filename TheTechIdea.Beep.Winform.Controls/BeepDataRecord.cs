
using System.ComponentModel;
using TheTechIdea.Beep.Shared;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

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
        private TableLayoutPanel _tableLayout;
        private int _fieldCount;
        private object _originalDataRecord;
        public bool AutoSave { get; set; } = false;

        public bool IsDirty => RecordStatus == RecordStatus.Updated;

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
        public bool ShowFieldPrompts { get; set; } = true;

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
        }

        public BeepDataRecord(object row, bool isNew = false) : this()
        {
            SetDataRecord(row, isNew);
        }

        private void InitializeComponent()
        {
            _fields = new List<IBeepUIComponent>();

            _tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoScroll = true
            };

            Controls.Add(_tableLayout);
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

            _originalDataRecord = EntityHelper.DeepCopyUsingSerialize(row); // Store the original copy

            // Store original field values
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
                SetDataRecord(EntityHelper.DeepCopyUsingSerialize(_originalDataRecord)); // Restore original
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
                    field.OnValueChanged += Field_OnValueChanged;
                    field.OnValidate += Field_OnValidate;
                    field.OnSelected += Field_OnSelected;

                    field.Theme = Theme;
                    field.ApplyTheme();

                    OnFieldCreated?.Invoke(this, field); // 🔹 Allows external customization

                    _fields.Add(field);
                }
            }

            UpdateLayout();
            BindDataRecord();
        }

        #endregion

        #region Layout Management

        public void UpdateLayout()
        {
            _tableLayout.SuspendLayout();
            _tableLayout.Controls.Clear();
            _tableLayout.RowStyles.Clear();
            _tableLayout.ColumnStyles.Clear();

            if (_fields == null || _fields.Count == 0) return;

            int columns = FieldOrientation == FieldOrientation.Horizontal ? FieldCount : 2;
            int rows = FieldOrientation == FieldOrientation.Horizontal
                ? (int)Math.Ceiling((double)_fields.Count / columns)
                : _fields.Count;

            _tableLayout.ColumnCount = columns;
            _tableLayout.RowCount = rows;

            for (int i = 0; i < columns; i++)
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));

            for (int i = 0; i < rows; i++)
                _tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            int row = 0, col = 0;
            foreach (var field in _fields)
            {
                if (field is Control control)
                {
                    if (ShowFieldPrompts)
                    {
                        var label = new Label
                        {
                            Text = FieldPrompts.ContainsKey(field.ComponentName)
                                ? FieldPrompts[field.ComponentName]
                                : field.ComponentName.Replace("_", " "),
                            AutoSize = true
                        };

                        _tableLayout.Controls.Add(label, 0, row);
                        _tableLayout.Controls.Add(control, 1, row);
                    }
                    else
                    {
                        _tableLayout.Controls.Add(control, col, row);
                    }
                }

                col++;
                if (col >= columns)
                {
                    col = 0;
                    row++;
                }
            }

            _tableLayout.ResumeLayout();
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
                    BindDataRecord(); // Refresh UI
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
                if (field is IBeepUIComponent component)
                {
                    if (component.IsRequired && string.IsNullOrEmpty(component.GetValue()?.ToString()))
                    {
                        isValid = false;
                        field.BorderColor = Color.Red;  // Highlight required field
                        field.ShowToolTip("This field is required.");
                    }
                    else
                    {
                        field.BorderColor = Color.Gray;
                        field.HideToolTip();
                    }
                }
            }

            RecordValidated?.Invoke(this, isValid);
        }


        #endregion
        #region IEditableObject Implementation

        public void BeginEdit()
        {
            _originalDataRecord = EntityHelper.DeepCopyUsingSerialize(DataRecord); // Store the current state
        }

        public void CancelEdit()
        {
            if (_originalDataRecord != null)
            {
                SetDataRecord(EntityHelper.DeepCopyUsingSerialize(_originalDataRecord)); // Restore the backup
                _originalDataRecord = null;
            }
        }

        public void EndEdit()
        {
            _originalDataRecord = null;
            RecordStatus = RecordStatus.Updated;
        }

        #endregion

    }
}
