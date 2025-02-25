
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
           // Console.WriteLine("BeepDataRecord Constructor");
            SetDataRecord(row, isNew);
        }

        private void InitializeComponent()
        {
            _fields = new List<IBeepUIComponent>();

            _tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                AutoScroll = false
            };

            Controls.Add(_tableLayout);
        }

        #endregion

        #region Data Handling

        public void SetDataRecord(object row, bool isNew = false)
        {
            if (row == null) return;
           // Console.WriteLine("SetDataRecord 1");
            if (_currentRowType == row.GetType())
            {
               // Console.WriteLine("SetDataRecord 2");
                DataRecord = row;
                BindDataRecord();
            }
            else
            {
               // Console.WriteLine("SetDataRecord 3");
                _currentRowType = row.GetType();
                DataRecord = row;
                GenerateFieldsFromDataRecord();
            }
           // Console.WriteLine("SetDataRecord 4");
            _originalDataRecord = EntityHelper.DeepCopyUsingSerialize(row); // Store the original copy
           // Console.WriteLine("SetDataRecord 5");
            // Store original field values
            _originalValues.Clear();
            foreach (var prop in row.GetType().GetProperties())
            {
                _originalValues[prop.Name] = prop.GetValue(row);
            }
           // Console.WriteLine("SetDataRecord 6");
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
               // Console.WriteLine("GenerateFieldsFromDataRecord 1");
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
            FieldCount=Fields.Count;
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
        }

        /// <summary>
        /// Sets up a layout where labels are above fields (if ShowFieldPrompts = true).
        /// </summary>
        private void SetupHorizontalLayout()
        {
            int columns = FieldCount;
            int rows = ShowFieldPrompts ? 2 : 1;

            _tableLayout.ColumnCount = columns;
            _tableLayout.RowCount = rows;

            // Define column styles (equal distribution)
            for (int i = 0; i < columns; i++)
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));

            // Define row styles (label row + field row)
            for (int i = 0; i < rows; i++)
                _tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            int fieldIndex = 0;
            for (int col = 0; col < columns; col++)
            {
                if (fieldIndex >= _fields.Count)
                    break;

                var field = _fields[fieldIndex];

                if (field is Control control)
                {
                    if (ShowFieldPrompts)
                    {
                        var label = CreateFieldLabel(field);
                        _tableLayout.Controls.Add(label, col, 0);  // Add label to row 0
                        _tableLayout.Controls.Add(control, col, 1); // Add field to row 1
                    }
                    else
                    {
                        _tableLayout.Controls.Add(control, col, 0); // Add field directly if no labels
                    }
                }

                fieldIndex++;
            }
        }

        /// <summary>
        /// Sets up a layout where labels are to the left of fields (if ShowFieldPrompts = true).
        /// </summary>
        private void SetupVerticalLayout()
        {
            int columns = ShowFieldPrompts ? 2 : 1;
            int rows = _fields.Count;

            _tableLayout.ColumnCount = columns;
            _tableLayout.RowCount = rows;

            // Define column styles
            if (ShowFieldPrompts)
            {
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150f)); // Fixed width for labels
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Fields take remaining space
            }
            else
            {
                _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Fields take full width
            }

            // Define row styles (one per field)
            for (int i = 0; i < rows; i++)
                _tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            int row = 0;
            foreach (var field in _fields)
            {
                if (field is Control control)
                {
                    if (ShowFieldPrompts)
                    {
                        var label = CreateFieldLabel(field);
                        _tableLayout.Controls.Add(label, 0, row);  // Add label to column 0
                        _tableLayout.Controls.Add(control, 1, row); // Add field to column 1
                    }
                    else
                    {
                        _tableLayout.Controls.Add(control, 0, row); // Add field directly if no labels
                    }
                }
                row++;
            }
        }

        /// <summary>
        /// Handles cases where there are no fields to display.
        /// </summary>
        private void SetupEmptyLayout()
        {
            _tableLayout.RowCount = 1;
            _tableLayout.ColumnCount = 1;
            _tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            var placeholder = new Label
            {
                Text = "No fields available",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            _tableLayout.Controls.Add(placeholder, 0, 0);
        }

        /// <summary>
        /// Creates a label for a field, using the ComponentName or FieldPrompts.
        /// </summary>
        private Label CreateFieldLabel(IBeepUIComponent field)
        {
            return new Label
            {
                Text = FieldPrompts.ContainsKey(field.ComponentName)
                    ? FieldPrompts[field.ComponentName]
                    : field.ComponentName.Replace("_", " "),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
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
