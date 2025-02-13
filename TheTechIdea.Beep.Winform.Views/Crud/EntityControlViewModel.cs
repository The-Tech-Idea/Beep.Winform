using TheTechIdea.Beep.Vis.Modules;
using CommunityToolkit.Mvvm.ComponentModel;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.MVVM;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Utilities;

using System.ComponentModel;
using System.Diagnostics;

using System.Data;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Views.Crud
{
    public partial class EntityControlViewModel: BaseViewModel
    {
        [ObservableProperty]
        string entityname;
        [ObservableProperty]
        Type entityType;
        [ObservableProperty]
        string datasourceName;
        [ObservableProperty]
        EntityStructure structure;
        [ObservableProperty]
        Control container;
        [ObservableProperty]
        Control crudFilterPanel;
        [ObservableProperty]
        ObservableBindingList<AppFilter> filters;
        [ObservableProperty]
        IDataSource dataSource;
        [ObservableProperty]
        IBindingListView ts;
        [ObservableProperty]
        string primaryKey;
        [ObservableProperty]
        bool isPrimarykeyMissing;
        [ObservableProperty]
        int fieldheight;
        public EntityControlViewModel(IDMEEditor Editor, Vis.Modules.IAppManager visManager) : base(Editor, visManager)
        {
            Fieldheight = 25;
        }
        public object GetObjectValue(object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName).GetValue(instance, null);
        }
        public object SetObjectValue(object instance, string propertyName, object setvalue)
        {
            instance.GetType().GetProperty(propertyName).SetValue(instance, setvalue);
            return instance;

        }
        public void CreateControlsForEntity(IBindingListView pTs)
        {
            Ts = pTs;
            if (Structure == null || Container == null || Ts == null) return;

            Container.Controls.Clear();
            int yPosition = 10;
            int maxWidth = 0; // To track the maximum width required

            // var entity = Ts[0]; // Assuming binding to the first element in the list
            if (string.IsNullOrEmpty(DatasourceName))
            {
                DatasourceName = Structure.DataSourceID;
            }
            DataSource = Editor.GetDataSource(DatasourceName);
            if (DataSource != null)
            {
                if (DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
                {
                    Structure = DataSource.GetEntityStructure(Entityname, false);
                }
                else
                {
                    Editor.AddLogMessage("Beep", "Datasource is not connected and Entity Structure not set", DateTime.Now, 0, null, Errors.Failed);
                    return;
                }
            }
            if (Structure.Fields.Count == 0)
            {
                Structure = DataSource.GetEntityStructure(Entityname, false);
            }
            if (string.IsNullOrEmpty(DatasourceName))
            {
                Editor.AddLogMessage("Beep", "Datasource is not set", DateTime.Now, 0, null, Errors.Failed);
                return;
            }
            if (Structure.Fields.Count == 0)
            {

            }
            if (Structure.Fields.Count == 0)
            {
                Editor.AddLogMessage("Beep", "Entity Structure not set", DateTime.Now, 0, null, Errors.Failed);
                return;
            }
            // Getting Defaults for Data
            List<DefaultValue> defaults = Editor.ConfigEditor.DataConnections[Editor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == DatasourceName)].DatasourceDefaults;

            // Getting Size of Labels
            int maxlabelsize = 0;
            int maxDatasize = 0;
            foreach (EntityField col in Structure.Fields.OrderBy(p => p.fieldname))
            {
                int x = getTextSize(col.fieldname);
                if (maxlabelsize < x)
                    maxlabelsize = x;
            }
            maxDatasize = Container.Width - maxlabelsize - 20;

            // Looping through the fields
            foreach (var field in Structure.Fields.OrderBy(p => p.fieldname))
            {
                DefaultValue coldefaults = defaults.Where(o => o.PropertyName == field.fieldname).FirstOrDefault();
                if (coldefaults == null)
                {
                    coldefaults = defaults.Where(o => field.fieldname.Contains(o.PropertyName)).FirstOrDefault();
                }
                else
                    coldefaults = new DefaultValue();
                string coltype = field.fieldtype;
                RelationShipKeys FK = Structure.Relations.Where(f => f.EntityColumnID == field.fieldname).FirstOrDefault();

                var label = new Label
                {
                    Text = field.fieldname,
                    Location = new Point(10, yPosition),
                    AutoSize = false,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    ForeColor = Color.Red
                };
                label.Size = TextRenderer.MeasureText(field.fieldname, label.Font);
                label.Height += 5;
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                label.Width = maxlabelsize;
                Control inputControl = null;
                if (FK != null)
                {
                    ComboBox cb = new ComboBox();

                    cb.DataSource = GetDisplayLookup(DatasourceName, FK.RelatedEntityID, FK.RelatedEntityColumnID, FK.EntityColumnID);
                    cb.DisplayMember = "DisplayField";
                    cb.ValueMember = FK.RelatedEntityColumnID;
                    cb.Width = maxDatasize;
                    cb.Height = label.Height;
                    cb.DataBindings.Add(new Binding("SelectedValue", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                    // cb.SelectedValueChanged += Cb_SelectedValueChanged;
                    cb.Anchor = AnchorStyles.Top;
                    inputControl = cb;

                }
                else
                {
                    switch (Type.GetType(field.fieldtype))
                    {
                        case Type type when type == typeof(System.TimeSpan):
                            inputControl = new TextBox();
                            inputControl.DataBindings.Add(new Binding("Text", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            break;

                        case Type type when type == typeof(string):
                            if (field.Size1 > 1)
                            {
                                inputControl = new TextBox();
                                inputControl.DataBindings.Add(new Binding("Text", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));

                            }
                            else
                            {
                                BeepCheckBox<Char> ch2 = new BeepCheckBox<Char>();

                                ch2.DataBindings.Add(new Binding("Checked", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                                ch2.Text = "";
                                ch2.Width = maxDatasize;
                                ch2.Height = label.Height;
                                if (coldefaults != null)
                                {
                                    if (coldefaults.PropertyValue.Contains(","))
                                    {
                                        string[] v = coldefaults.PropertyValue.Split(',');
                                        v = coldefaults.PropertyValue.Split(',');
                                        ch2.CheckedValue = v[0].ToCharArray()[0];
                                        ch2.UncheckedValue = v[1].ToCharArray()[0];
                                    }

                                }
                                //      ch2.CheckStateChanged += Ch1_CheckStateChanged; ;
                                inputControl = ch2;
                            }
                            break;
                        case Type type when type == typeof(int) || type == typeof(long) || type == typeof(short):
                            inputControl = new NumericUpDown();
                            ((NumericUpDown)inputControl).Maximum = decimal.MaxValue;
                            inputControl.DataBindings.Add(new Binding("Value", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            break;
                        case Type type when type == typeof(decimal) || type == typeof(double) || type == typeof(float):
                            inputControl = new NumericUpDown();
                            ((NumericUpDown)inputControl).DecimalPlaces = 2;
                            ((NumericUpDown)inputControl).Maximum = decimal.MaxValue;
                            inputControl.DataBindings.Add(new Binding("Value", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            break;
                        case Type type when type == typeof(bool):
                            inputControl = new CheckBox();
                            inputControl.DataBindings.Add(new Binding("Checked", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            break;
                        case Type type when type == typeof(DateTime):
                            inputControl = new DateTimePicker();
                            inputControl.DataBindings.Add(new Binding("Value", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            break;
                        case Type type when type.IsEnum:
                            inputControl = new ComboBox();
                            ((ComboBox)inputControl).DataSource = Enum.GetValues(type);
                            ((ComboBox)inputControl).DataBindings.Add(new Binding("SelectedMenuItem", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            break;
                        case Type type when type == typeof(Guid):
                            inputControl = new TextBox();
                            inputControl.DataBindings.Add(new Binding("Text", Ts, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            break;
                            // Add more specific cases as needed
                    }
                }
                if (Structure.PrimaryKeys.Any(x => x.fieldname == field.fieldname))
                {
                    if (Structure.Relations.Any(x => x.EntityColumnID == field.fieldname))
                    {
                        inputControl.Enabled = false;
                    }
                    if (field.IsAutoIncrement)
                    {
                        inputControl.Enabled = false;
                    }
                }
                if (inputControl != null)
                {
                    inputControl.Anchor = AnchorStyles.Top;
                    inputControl.Location = new Point(label.Width + 5, yPosition); // 20 is the space between label and control
                    inputControl.Width = 250; // Or set based on the control type or field type
                    maxWidth = Math.Max(maxWidth, label.Width + inputControl.Width + 30); // 30 is the total margin (20 between + 10 right)

                    Container.Controls.Add(label);
                    Container.Controls.Add(inputControl);
                    yPosition += inputControl.Height + 5; // Adjust vertical position for the next control, 10 is the margin between controls
                }
                // yPosition += 5;
            }
            // Adjust the container's size based on the number of controls and maximum width
            Container.Height = yPosition + 20; // 20 is the bottom margin
            Container.Width = maxWidth + 10; // 10 is the right margin
        }
        public void CreateControlsForEntity(BindingSource bindingSource)
        {
            if (Structure == null || Container == null || bindingSource == null) return;

            Container.Controls.Clear();
            int yPosition = 10;
            int maxWidth = 0; // To track the maximum width required

            if (string.IsNullOrEmpty(DatasourceName))
            {
                DatasourceName = Structure.DataSourceID;
            }
            DataSource = Editor.GetDataSource(DatasourceName);
            if (DataSource != null)
            {
                if (DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
                {
                    Structure = DataSource.GetEntityStructure(Entityname, false);
                }
                else
                {
                    Editor.AddLogMessage("Beep", "Datasource is not connected and Entity Structure not set", DateTime.Now, 0, null, Errors.Failed);
                    return;
                }
            }
            if (Structure.Fields.Count == 0)
            {
                Structure = DataSource.GetEntityStructure(Entityname, false);
            }
            List<DefaultValue> defaults = Editor.ConfigEditor.DataConnections[Editor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == DatasourceName)].DatasourceDefaults;

            // Getting Size of Labels
            int maxlabelsize = 0;
            int maxDatasize = 0;
            foreach (EntityField col in Structure.Fields.OrderBy(p => p.fieldname))
            {
                int x = getTextSize(col.fieldname);
                if (maxlabelsize < x)
                    maxlabelsize = x;
            }
            int fieldheight = 25;
            maxDatasize = Container.Width - maxlabelsize - 20;
            // Looping through the fields and creating controls
            foreach (var field in Structure.Fields.OrderBy(p => p.fieldname))
            {

                // Create and configure the label for the field
                var label = new Label
                {
                    Text = field.fieldname,
                    Location = new Point(10, yPosition),
                    AutoSize = false,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    ForeColor = Color.Red
                };
                label.Size = TextRenderer.MeasureText(field.fieldname, label.Font);
                label.Height = fieldheight;
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                label.Width = maxlabelsize;
                Control inputControl = null;
                try
                {
                    // Determine the appropriate control for the field type
                    inputControl = CreateControlForField(field, bindingSource, defaults, yPosition, maxDatasize, maxlabelsize);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($" Error while Removing Tab : {ex.Message}");
                }

                if (inputControl != null)
                {
                    inputControl.Anchor = AnchorStyles.Top;
                    inputControl.Location = new Point(label.Width + 5, yPosition); // 20 is the space between label and control
                    inputControl.Width = 250; // Or set based on the control type or field type
                    maxWidth = Math.Max(maxWidth, label.Width + inputControl.Width + 30); // 30 is the total margin (20 between + 10 right)

                    Container.Controls.Add(label);
                    Container.Controls.Add(inputControl);
                    yPosition += inputControl.Height + 5; // Adjust vertical position for the next control, 10 is the margin between controls
                }
            }

            // Adjust the container's size based on the maximum width and current yPosition
            Container.Width = maxWidth + 5;
            Container.Height = yPosition + 5;
        }
        public Control CreateControlForField(EntityField field, object bindingSource, List<DefaultValue> defaults, int yPosition, int maxDatasize, int maxlabelsize)
        {
            Control inputControl = null;
            // Create and configure the label for the field

            DefaultValue coldefaults = defaults.Where(o => o.PropertyName == field.fieldname).FirstOrDefault();
            if (coldefaults == null)
            {
                coldefaults = defaults.Where(o => field.fieldname.Contains(o.PropertyName)).FirstOrDefault();
            }
            else
                coldefaults = new DefaultValue();
            string coltype = field.fieldtype;
            RelationShipKeys FK = Structure.Relations.Where(f => f.EntityColumnID == field.fieldname).FirstOrDefault();
            if (FK != null)
            {
                ComboBox cb = new ComboBox();

                cb.DataSource = GetDisplayLookup(DatasourceName, FK.RelatedEntityID, FK.RelatedEntityColumnID, FK.EntityColumnID);
                cb.DisplayMember = "DisplayField";
                cb.ValueMember = FK.RelatedEntityColumnID;
                cb.Width = maxDatasize;
                cb.Height = Fieldheight;
                cb.DataBindings.Add(new Binding("SelectedValue", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                // cb.SelectedValueChanged += Cb_SelectedValueChanged;
                cb.Anchor = AnchorStyles.Top;
                inputControl = cb;

            }
            else
            {
                Debug.WriteLine($"Field Type : {field.fieldname}-{field.fieldtype}-{field.Size1}");
                switch (Type.GetType(field.fieldtype))
                {
                    case Type type when type == typeof(System.TimeSpan):
                        inputControl = new TextBox();
                        inputControl.DataBindings.Add(new Binding("Text", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                        break;

                    case Type type when type == typeof(string):
                        if (field.Size1 > 1)
                        {
                            inputControl = new TextBox();
                            inputControl.DataBindings.Add(new Binding("Text", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged, ""));

                        }
                        else
                        {
                            BeepCheckBox<Char> ch2 = new BeepCheckBox<Char>();

                            ch2.DataBindings.Add(new Binding("Checked", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                            ch2.Text = "";
                            ch2.Width = maxDatasize;
                            ch2.Height = fieldheight;
                            if (coldefaults != null)
                            {
                                if (coldefaults.PropertyValue.Contains(","))
                                {
                                    string[] v = coldefaults.PropertyValue.Split(',');
                                    v = coldefaults.PropertyValue.Split(',');
                                    ch2.CheckedValue = v[0].ToCharArray()[0];
                                    ch2.UncheckedValue = v[1].ToCharArray()[0];
                                }

                            }
                            //      ch2.CheckStateChanged += Ch1_CheckStateChanged; ;
                            inputControl = ch2;
                        }
                        break;
                    case Type type when type == typeof(int) || type == typeof(long) || type == typeof(short):
                        //inputControl = new NumericUpDown();
                        //((NumericUpDown)inputControl).Maximum = decimal.MaxValue;
                        //inputControl.DataBindings.Add(new Binding("Value", BindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                        inputControl = new TextBox();
                        inputControl.DataBindings.Add(new Binding("Text", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged, 0));

                        break;
                    case Type type when type == typeof(decimal) || type == typeof(double) || type == typeof(float):
                        //inputControl = new NumericUpDown();
                        //((NumericUpDown)inputControl).DecimalPlaces = 2;
                        //((NumericUpDown)inputControl).Maximum = decimal.MaxValue;

                        //// Create a binding with proper formatting and parsing
                        //Binding binding = new Binding("Value", BindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged);
                        //binding.Format += (s, e) => {
                        //    if (e.DesiredType == typeof(decimal))
                        //    {
                        //        e.Value = Convert.ToDecimal(e.Value);
                        //    }
                        //};
                        //binding.Parse += (s, e) => {
                        //    if (e.DesiredType == typeof(float) && e.Value is decimal)
                        //    {
                        //        e.Value = Convert.ToSingle((decimal)e.Value);
                        //    }
                        //    else if (e.DesiredType == typeof(double) && e.Value is decimal)
                        //    {
                        //        e.Value = Convert.ToDouble((decimal)e.Value);
                        //    }
                        //};
                        //inputControl.DataBindings.Add(binding);
                        inputControl = new TextBox();
                        inputControl.DataBindings.Add(new Binding("Text", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged, 0));
                        break;

                    case Type type when type == typeof(bool):
                        inputControl = new CheckBox();
                        inputControl.DataBindings.Add(new Binding("Checked", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                        break;
                    case Type type when type == typeof(DateTime):
                        inputControl = new DateTimePicker();
                        inputControl.DataBindings.Add(new Binding("Value", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                        break;
                    case Type type when type.IsEnum:
                        inputControl = new ComboBox();
                        ((ComboBox)inputControl).DataSource = Enum.GetValues(type);
                        ((ComboBox)inputControl).DataBindings.Add(new Binding("SelectedMenuItem", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                        break;
                    case Type type when type == typeof(Guid):
                        inputControl = new TextBox();
                        inputControl.DataBindings.Add(new Binding("Text", bindingSource, field.fieldname, true, DataSourceUpdateMode.OnPropertyChanged));
                        break;
                        // Add more specific cases as needed
                }
            }
            if (Structure.PrimaryKeys.Any(x => x.fieldname == field.fieldname))
            {
                if (Structure.Relations.Any(x => x.EntityColumnID == field.fieldname))
                {
                    inputControl.Enabled = false;
                }
                if (field.IsAutoIncrement)
                {
                    inputControl.Enabled = false;
                }
            }
            return inputControl;
        }
        public void CreateColumnsForEntity(DataGridView dataGridView, EntityStructure structure)
        {
            dataGridView.SuspendLayout();
            dataGridView.Columns.Clear();
            if (dataGridView == null || structure == null || structure.Fields == null)
            {
                dataGridView.ResumeLayout();
                return;
            }

            try
            {
                foreach (var field in structure.Fields)
                {
                    DataGridViewColumn column = null;

                    switch (Type.GetType(field.fieldtype))
                    {
                        case Type type when type == typeof(string):
                            column = new DataGridViewTextBoxColumn();
                            break;
                        case Type type when type == typeof(int) || type == typeof(long) || type == typeof(short):
                            column = new DataGridViewTextBoxColumn();
                            column.ValueType = type;
                            break;
                        case Type type when type == typeof(decimal) || type == typeof(double) || type == typeof(float):
                            column = new DataGridViewTextBoxColumn();
                            column.ValueType = type;
                            break;
                        case Type type when type == typeof(bool):
                            column = new DataGridViewCheckBoxColumn();
                            break;
                        //case Type type when type == typeof(DateTime):
                        //    column = new DataGridViewDateTimePickerColumn(); // Custom column for DateTime
                        //                                                     //column = new DataGridViewTextBoxColumn();
                        //    column.ValueType = type;
                        //    column.DefaultCellStyle.Format = "g"; // General date-time pattern.
                        //    break;
                        case Type type when type.IsEnum:
                            column = new DataGridViewComboBoxColumn()
                            {
                                DataSource = Enum.GetValues(type),
                                ValueType = type
                            };
                            break;
                        case Type type when type == typeof(Guid):
                            column = new DataGridViewTextBoxColumn();
                            break;
                            // Add more cases as needed for other data types
                    }

                    if (column != null)
                    {
                        column.DataPropertyName = field.fieldname;
                        column.Name = field.fieldname;
                        column.HeaderText = field.fieldname;
                        //       column.ReadOnly = field.IsAutoIncrement || field.IsKey; // Set read-only for auto-increment or key fields
                        dataGridView.Columns.Add(column);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error adding columns in _targetGrid for Entity {structure.EntityName}: " + ex.Message);
            }

            dataGridView.ResumeLayout();

        }
        public EntityStructure CreateFilterFields(EntityStructure entityStructure)
        {

            CrudFilterPanel.Controls.Clear();
            if (string.IsNullOrEmpty(DatasourceName))
            {
                DatasourceName = Structure.DataSourceID;
            }
            DataSource = Editor.GetDataSource(DatasourceName);
            if (DataSource != null)
            {
                if (DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
                {
                    Structure = DataSource.GetEntityStructure(Entityname, false);
                }
                else
                {
                    Editor.AddLogMessage("Beep", "Datasource is not connected and Entity Structure not set", DateTime.Now, 0, null, Errors.Failed);
                    return null;
                }
            }
            if (Structure.Fields.Count == 0)
            {
                Structure = DataSource.GetEntityStructure(Entityname, false);
            }
            List<DefaultValue> defaults = Editor.ConfigEditor.DataConnections[Editor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == DatasourceName)].DatasourceDefaults;

            //  BindingSource[] BindingData = new BindingSource[entityStructure.Fields.Count];
            int maxlabelsize = 0;
            int maxDatasize = 0;
            foreach (EntityField col in entityStructure.Fields)
            {
                int x = getTextSize(col.fieldname.ToUpper());
                if (maxlabelsize < x)
                    maxlabelsize = x;
            }
            maxDatasize = CrudFilterPanel.Width - maxlabelsize - 20;
            if (entityStructure.Filters != null)
            {
                entityStructure.Filters.Clear();
            }
            else
                entityStructure.Filters = new List<AppFilter>();

            List<string> FieldNames = new List<string>();
            Filters = new ObservableBindingList<AppFilter>();
            var starth = 25;
            int startleft = maxlabelsize + 90;
            int valuewidth = 100;
            int maxWidth = 0;

            for (int i = 0; i <= entityStructure.Fields.Count - 1; i++)
            {
                AppFilter filter = new AppFilter();
                filter.FieldName = entityStructure.Fields[i].fieldname;
                filter.Operator = null;
                filter.FilterValue = null;
                filter.FilterValue1 = null;
                filter.valueType = entityStructure.Fields[i].fieldtype;
                entityStructure.Filters.Add(filter);
                Filters.Add(filter);
                //  BindingData[i] = new BindingSource();
                //   BindingData[i].Data = filter;
                FieldNames.Add(entityStructure.Fields[i].fieldname);
                EntityField col = entityStructure.Fields[i];
                int fromwidth = 0;
                int towidth = 0;
                try
                {
                    DefaultValue coldefaults = defaults.Where(o => o.PropertyName == col.fieldname).FirstOrDefault();
                    if (coldefaults == null)
                    {
                        coldefaults = defaults.Where(o => col.fieldname.Contains(o.PropertyName)).FirstOrDefault();
                    }
                    string coltype = col.fieldtype;
                    RelationShipKeys FK = entityStructure.Relations.Where(f => f.EntityColumnID == col.fieldname).FirstOrDefault();
                    //----------------------
                    Label l = new Label
                    {
                        Top = starth,
                        Left = 10,
                        AutoSize = false,
                        BorderStyle = BorderStyle.FixedSingle,
                        Text = col.fieldname.ToUpper(),
                        BackColor = Color.White,
                        ForeColor = Color.Red

                    };
                    l.Size = TextRenderer.MeasureText(col.fieldname.ToUpper(), l.Font);
                    l.Height += 10;
                    l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    l.Width = maxlabelsize;
                    //---------------------
                    ComboBox cbcondition = new ComboBox
                    {
                        Left = l.Left + l.Width + 10,
                        Top = starth
                    };

                    //cbcondition.Data = GetDisplayLookup(entityStructure.DataSourceID, FK.ParentEntityID, FK.ParentEntityColumnID, FK.EntityColumnID);
                    //cbcondition.DisplayMember = DisplayField;
                    //cbcondition.ValueMember = FK.ParentEntityColumnID;
                    cbcondition.DataSource = AddFilterTypes();
                    cbcondition.DisplayMember = "FilterDisplay";
                    cbcondition.ValueMember = "FilterValue";
                    // cbcondition.SelectedValueChanged += Cb_SelectedValueChanged;
                    cbcondition.Name = col.fieldname + "." + "Operator";
                    cbcondition.Width = 50;
                    cbcondition.Height = l.Height;
                    // cbcondition.SelectedText
                    cbcondition.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", Filters[i], "Operator", true, DataSourceUpdateMode.OnPropertyChanged));
                    //  cbcondition.Anchor = AnchorStyles.Top;
                    CrudFilterPanel.Controls.Add(cbcondition);

                    if (FK != null)
                    {
                        ComboBox cb = new ComboBox
                        {
                            Left = startleft,
                            Top = starth
                        };

                        cb.DataSource = GetDisplayLookup(entityStructure.DataSourceID, FK.RelatedEntityID, FK.RelatedEntityColumnID, FK.EntityColumnID);
                        cb.DisplayMember = "DisplayField";
                        cb.ValueMember = FK.RelatedEntityColumnID;
                        cb.Width = valuewidth;
                        cb.Height = l.Height;
                        cb.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", Filters[i], "FilterValue", true));
                        //  cb.SelectedValueChanged += Cb_SelectedValueChanged;
                        //cb.Anchor = AnchorStyles.Top;
                        CrudFilterPanel.Controls.Add(cb);
                        cb.Name = col.fieldname;
                        starth = l.Bottom + 1;
                    }
                    else
                    {
                        switch (coltype)
                        {
                            case "System.DateTime":
                                DateTimePicker dt = new DateTimePicker
                                {
                                    Left = startleft,
                                    Top = starth
                                };
                                dt.DataBindings.Add(new System.Windows.Forms.Binding("Value", Filters[i], "FilterValue", true));

                                dt.Width = valuewidth;
                                dt.Height = l.Height;
                                //    dt.ValueChanged += Dt_ValueChanged;
                                //     dt.Anchor = AnchorStyles.Top;
                                dt.Tag = i;
                                dt.Format = DateTimePickerFormat.Short;
                                dt.Name = "From" + "." + col.fieldname;
                                CrudFilterPanel.Controls.Add(dt);
                                DateTimePicker dt1 = new DateTimePicker
                                {
                                    Left = dt.Left + 10 + dt.Width,
                                    Top = starth
                                };

                                dt1.DataBindings.Add(new System.Windows.Forms.Binding("Value", Filters[i], "FilterValue1", true));

                                dt1.Width = valuewidth;
                                dt1.Height = l.Height;
                                dt1.Format = DateTimePickerFormat.Short;
                                //   dt1.ValueChanged += Dt_ValueChanged;
                                //     dt.Anchor = AnchorStyles.Top;
                                dt1.Tag = i;
                                dt1.Name = "From" + "." + col.fieldname;
                                towidth = (valuewidth * 2) + 10;
                                CrudFilterPanel.Controls.Add(dt1);
                                break;
                            case "System.TimeSpan":
                                TextBox t1 = new TextBox
                                {
                                    Left = startleft,
                                    Top = starth
                                };

                                t1.DataBindings.Add(new System.Windows.Forms.Binding("Text", Filters[i], "FilterValue", true));
                                t1.TextAlign = HorizontalAlignment.Left;
                                t1.Width = valuewidth;
                                t1.Height = l.Height;

                                t1.Tag = i;
                                // t1.TextChanged += T_TextChanged;
                                //  t1.KeyPress += T_KeyPress;
                                //     t1.Anchor = AnchorStyles.Top;
                                t1.Name = col.fieldname;
                                CrudFilterPanel.Controls.Add(t1);
                                break;
                            case "System.Boolean":
                                CheckBox ch1 = new CheckBox
                                {
                                    Left = startleft,
                                    Top = starth
                                };

                                ch1.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", Filters[i], "FilterValue", true));
                                ch1.Text = "";
                                ch1.Width = valuewidth;
                                ch1.Height = l.Height;
                                // ch1.CheckStateChanged += Ch1_CheckStateChanged; ;
                                //       ch1.Anchor = AnchorStyles.Top;
                                ch1.Tag = i;
                                ch1.Name = col.fieldname;
                                CrudFilterPanel.Controls.Add(ch1);


                                break;
                            case "System.Char":
                                BeepCheckBox<Char> ch2 = new BeepCheckBox<Char>
                                {
                                    Left = startleft,
                                    Top = starth
                                };

                                ch2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", Filters[i], "FilterValue", true));
                                ch2.Text = "";
                                ch2.Width = valuewidth;
                                ch2.Height = l.Height;
                                string[] v = coldefaults.PropertyValue.Split(',');

                                if (coldefaults != null)
                                {
                                    ch2.CheckedValue = v[0].ToCharArray()[0];
                                    ch2.UncheckedValue = v[1].ToCharArray()[0];
                                }
                                //  ch2.CheckStateChanged += Ch1_CheckStateChanged; ;
                                //   ch2.Anchor = AnchorStyles.Top;
                                ch2.Tag = i;
                                ch2.Name = col.fieldname;
                                CrudFilterPanel.Controls.Add(ch2);

                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Decimal":
                            case "System.Double":
                            case "System.Single":
                                NumericUpDown t3 = new NumericUpDown();

                                t3.Left = startleft;
                                t3.Top = starth;
                                t3.DataBindings.Add(new System.Windows.Forms.Binding("Value", Filters[i], "FilterValue", true));
                                t3.TextAlign = HorizontalAlignment.Left;
                                t3.Width = valuewidth;
                                t3.Height = l.Height;
                                t3.Tag = i;
                                t3.Minimum = 0;
                                //  t3.TextChanged += T_TextChanged;
                                //t.KeyPress += T_KeyPress;
                                //if (entityStructure.PrimaryKeys.Where(x => x.fieldname == col.fieldname).FirstOrDefault() != null)
                                //{
                                //    t3.Enabled = false;
                                //}
                                CrudFilterPanel.Controls.Add(t3);

                                NumericUpDown t2 = new NumericUpDown();
                                t2.Left = t3.Left + t3.Width + 10;
                                t2.Top = starth;
                                t2.DataBindings.Add(new System.Windows.Forms.Binding("Value", Filters[i], "FilterValue1", true));
                                t2.TextAlign = HorizontalAlignment.Left;
                                t2.Width = valuewidth;
                                t2.Height = l.Height;
                                t2.Tag = i;
                                t2.Maximum = 99999;
                                //   t2.TextChanged += T_TextChanged;
                                //t.KeyPress += T_KeyPress;
                                //if (entityStructure.PrimaryKeys.Where(x => x.fieldname == col.fieldname).FirstOrDefault() != null)
                                //{
                                //    t2.Enabled = false;
                                //}
                                //   t.Anchor = AnchorStyles.Top;
                                towidth = (valuewidth * 2) + 10;
                                CrudFilterPanel.Controls.Add(t2);
                                //   t.Anchor = AnchorStyles.Top;
                                break;

                            case "System.String":
                                if (entityStructure.Fields.Where(p => p.fieldname == col.fieldname).FirstOrDefault().Size1 != 1)
                                {
                                    t1 = new TextBox
                                    {
                                        Left = startleft,
                                        Top = starth
                                    };

                                    t1.DataBindings.Add(new System.Windows.Forms.Binding("Text", Filters[i], "FilterValue", true));
                                    t1.TextAlign = HorizontalAlignment.Left;
                                    t1.Width = valuewidth;
                                    t1.Height = l.Height;

                                    //      t1.TextChanged += T_TextChanged;
                                    //  t1.KeyPress += T_KeyPress;
                                    //if (entityStructure.PrimaryKeys.Any(x => x.fieldname == col.fieldname))
                                    //{
                                    //    if (entityStructure.Relations.Any(x => x.EntityColumnID == col.fieldname))
                                    //    {
                                    //        t1.Enabled = false;
                                    //    }

                                    //}
                                    CrudFilterPanel.Controls.Add(t1);
                                    t1.Tag = i;
                                    //      t1.Anchor = AnchorStyles.Top;
                                }
                                else
                                {
                                    ch2 = new BeepCheckBox<Char>
                                    {
                                        Left = startleft,
                                        Top = starth
                                    };

                                    ch2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", Filters[i], "FilterValue", true));
                                    ch2.Text = "";
                                    ch2.Width = valuewidth;
                                    ch2.Height = l.Height;
                                    ch2.Tag = i;


                                    if (coldefaults != null)
                                    {
                                        v = coldefaults.PropertyValue.Split(',');
                                        ch2.CheckedValue = v[0].ToCharArray()[0];
                                        ch2.UncheckedValue = v[1].ToCharArray()[0];
                                    }
                                    // ch2.CheckStateChanged += Ch1_CheckStateChanged; ;

                                    CrudFilterPanel.Controls.Add(ch2);

                                    //     ch2.Anchor = AnchorStyles.Top;
                                }
                                break;
                            default:
                                TextBox t = new TextBox();

                                t.Left = startleft;
                                t.Top = starth;
                                t.DataBindings.Add(new System.Windows.Forms.Binding("Text", Filters[i], "FilterValue", true));
                                t.TextAlign = HorizontalAlignment.Left;
                                t.Width = valuewidth;
                                t.Height = l.Height;
                                t.Tag = i;
                                //   t.TextChanged += T_TextChanged;
                                //t.KeyPress += T_KeyPress;
                                //if (entityStructure.PrimaryKeys.Where(x => x.fieldname == col.fieldname).FirstOrDefault() != null)
                                //{
                                //    t.Enabled = false;
                                //}
                                //   t.Anchor = AnchorStyles.Top;

                                CrudFilterPanel.Controls.Add(t);

                                break;

                        }
                        if (towidth > 0)
                        {
                            maxWidth = Math.Max(maxWidth, l.Width + towidth + 30); // 30 is the total margin (20 between + 10 right)
                        }
                        else
                        {
                            maxWidth = Math.Max(maxWidth, l.Width + valuewidth + 30); // 30 is the total margin (20 between + 10 right)
                        }

                    }


                    CrudFilterPanel.Controls.Add(l);
                    starth = l.Bottom + 1;

                }

                catch (Exception ex)
                {
                    return null;
                    // Logger.WriteLog($"Error in Loading View ({ex.Message}) ");
                }
                CrudFilterPanel.Width = maxWidth + 20;
                CrudFilterPanel.Height = starth + 5; // Adjust verti
            }

            return entityStructure;
        }
        #region "Utility Functions"
        public virtual List<FilterType> AddFilterTypes()
        {
            //{ null, "=", ">=", "<=", ">", "<", "Like", "Between" }
            List<FilterType> lsop = new List<FilterType>();
            FilterType filterType = new FilterType("");
            lsop.Add(filterType);

            filterType = new FilterType("=");
            lsop.Add(filterType);

            filterType = new FilterType(">=");
            lsop.Add(filterType);

            filterType = new FilterType("<=");
            lsop.Add(filterType);
            filterType = new FilterType(">");
            lsop.Add(filterType);

            filterType = new FilterType("<");
            lsop.Add(filterType);

            filterType = new FilterType("like");
            lsop.Add(filterType);

            filterType = new FilterType("Between");
            lsop.Add(filterType);
            return lsop;

        }
        public virtual int getTextSize(string text)
        {
            Font font = new Font("Courier New", 10.0F);
            Image fakeImage = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF size = graphics.MeasureString(text, font);
            return Convert.ToInt32(size.Width);
        }
        public virtual object GetDisplayLookup(string datasourceid, string Parententityname, string ParentKeyField, string EntityField)
        {
            object retval = null;
            string DisplayField = string.Empty;
            try
            {
                if (Parententityname != null)
                {
                    IDataSource ds = Editor.GetDataSource(datasourceid);
                    EntityStructure ent = ds.GetEntityStructure(Parententityname, false);

                    // bool found = true;
                    // int i = 0;
                    List<DefaultValue> defaults = ds.Dataconnection.ConnectionProp.DatasourceDefaults.Where(o => o.propertyType == DefaultValueType.DisplayLookup).ToList();
                    List<string> fields = ent.Fields.Select(u => u.EntityName).ToList();
                    if (defaults != null)
                    {
                        DefaultValue defaultValue = defaults.Where(p => p.PropertyName.Equals(EntityField, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (defaultValue != null)
                        {
                            DisplayField = defaultValue.PropertyValue;
                        }

                    }
                    if (string.IsNullOrWhiteSpace(DisplayField) || string.IsNullOrEmpty(DisplayField))
                    {
                        DisplayField = ent.Fields.Where(r => r.fieldname.Contains("NAME")).Select(u => u.fieldname).FirstOrDefault();
                    }
                    if (string.IsNullOrWhiteSpace(DisplayField) || string.IsNullOrEmpty(DisplayField))
                    {
                        DisplayField = ParentKeyField;
                    }
                    string qrystr = "select " + DisplayField + " as DisplayField," + ParentKeyField + "  from " + Parententityname;


                    retval = ds.RunQuery(qrystr);
                }


                return retval;
            }
            catch (Exception)
            {

                return null;
            }
        }



        #endregion
    }
}
