using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public class DialogManager : IDialogManager
    {
        private readonly IBeepService _beepServices;
        public IDMEEditor DMEEditor { get; set; }
        public Control DisplayPanel { get; set; }
        public Control CrudFilterPanel { get; set; }
        public BindingSource EntityBindingSource { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; }
        private string DisplayField;

        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;

        public DialogManager(IBeepService services)
        {
            _beepServices = services ?? throw new ArgumentNullException(nameof(services));
            DMEEditor = _beepServices.DMEEditor;
            ErrorsandMesseges = new ErrorsInfo { Flag = Errors.Ok };
        }

        #region Dialog Methods
        public BeepDialogResult InputBoxYesNo(string title, string promptText)
        {
            return DialogHelper.InputBoxYesNo(title, promptText);
        }

        public BeepDialogResult InputBox(string title, string promptText, ref string value)
        {
            var result = DialogHelper.InputBox(title, promptText, value);
            if (result.Result == BeepDialogResult.OK)
            {
                value = result.Value;
            }
            return result.Result;
        }

        public BeepDialogResult InputLargeBox(string title, string promptText, ref string value)
        {
            var result = DialogHelper.InputLargeBox(title, promptText, value);
            if (result.Result == BeepDialogResult.OK)
            {
                value = result.Value;
            }
            return result.Result;
        }

        public BeepDialogResult InputComboBox(string title, string promptText, List<string> itvalues, ref string value)
        {
            var result = DialogHelper.InputComboBox(title, promptText, itvalues, value);
            if (result.Result == BeepDialogResult.OK)
            {
                value = result.Value;
            }
            return result.Result;
        }

        public string DialogCombo(string text, List<object> comboSource, string displayMember, string valueMember)
        {
            var result = DialogHelper.DialogCombo(text, comboSource, displayMember, valueMember);
            return result.Result == BeepDialogResult.OK ? result.Value : null;
        }

        public void MsgBox(string title, string promptText)
        {
            DialogHelper.ShowMessageBox(title, promptText);
        }

        public async Task ShowProgressDialog(string title, string initialMessage, Func<IProgress<(int Progress, string Message)>, Task> operation)
        {
            await DialogHelper.ShowProgressDialog(title, initialMessage, operation);
        }

        public (BeepDialogResult Result, Dictionary<string, string> Values) MultiInputDialog(
            string title, Dictionary<string, (string DefaultValue, Func<string, bool> Validator)> fields)
        {
            return DialogHelper.MultiInputDialog(title, fields);
        }
        #endregion

        #region File and Folder Dialogs
        public string SelectFile(string filter)
        {
            return DialogHelper.LoadFileDialog(null, null, filter);
        }

        public string LoadFileDialog(string exts, string dir, string filter)
        {
            return DialogHelper.LoadFileDialog(exts, dir, filter);
        }

        public List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            return DialogHelper.LoadFilesDialog(exts, dir, filter).ToList();
        }

        public string SaveFileDialog(string exts, string dir, string filter)
        {
            return DialogHelper.SaveFileDialog(exts, dir, filter);
        }

        public string ShowSpecialDirectoriesComboBox()
        {
            var specialDirs = Enum.GetValues(typeof(Environment.SpecialFolder))
                .Cast<Environment.SpecialFolder>()
                .Select(f => new { Name = f.ToString(), Path = Environment.GetFolderPath(f) })
                .Where(d => !string.IsNullOrEmpty(d.Path))
                .ToList();

            var result = DialogHelper.DialogCombo("Select Special Directory", specialDirs, "Name", "Path");
            return result.Result == BeepDialogResult.OK ? result.Value : null;
        }

        public string SelectFolderDialog()
        {
            return DialogHelper.SelectFolderDialog();
        }
        #endregion

        #region Utility Methods
        public List<FilterType> AddFilterTypes()
        {
            return new List<FilterType>
            {
                new FilterType(""),
                new FilterType("="),
                new FilterType(">="),
                new FilterType("<="),
                new FilterType(">"),
                new FilterType("<"),
                new FilterType("like"),
                new FilterType("Between")
            };
        }

        public void ShowMessege(string title, string message, string icon)
        {
            MsgBox(title, message); // Using existing MsgBox for simplicity
        }
        #endregion

        #region CRUD and Other Synchronous Methods
        public object GenerateEntityonControl(string entityname, object record, int starttop, string datasourceid,
            TransActionType TranType, IPassedArgs passedArgs = null)
        {
            Control control = new Control();
            try
            {
                if (passedArgs?.Objects != null)
                {
                    if (passedArgs.Objects.Any(c => c.Name == "BindingSource"))
                        EntityBindingSource = (BindingSource)passedArgs.Objects.First(c => c.Name == "BindingSource").obj;
                    if (passedArgs.Objects.Any(c => c.Name == "Control"))
                        control = (Control)passedArgs.Objects.First(c => c.Name == "Control").obj;
                }

                if (record == null)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Message = "Record has no data";
                    MsgBox("Error", "Record has no data");
                    return DMEEditor.ErrorObject;
                }

                IDataSource ds = DMEEditor.GetDataSource(datasourceid);
                if (ds == null)
                {
                    DMEEditor.ErrorObject.Flag = Errors.Failed;
                    DMEEditor.ErrorObject.Message = $"Data source '{datasourceid}' not found";
                    return DMEEditor.ErrorObject;
                }

                var defaults = DMEEditor.ConfigEditor.DataConnections
                    .FirstOrDefault(i => i.ConnectionName == ds.DatasourceName)?.DatasourceDefaults ?? new List<DefaultValue>();
                var entityStructure = ds.GetEntityStructure(entityname, false);
                Type entityType = ds.GetEntityType(entityname);
                object entityInstance = Activator.CreateInstance(entityType);

                // Populate entity instance from record
                if (record is ICustomTypeDescriptor descriptor)
                {
                    DataRowView dv = (DataRowView)record;
                    DataRow dr = dv.Row;
                    foreach (var col in entityStructure.Fields)
                    {
                        var prop = entityType.GetProperty(col.fieldname);
                        prop?.SetValue(entityInstance, dr[col.fieldname] == DBNull.Value ? null : dr[col.fieldname]);
                    }
                }
                else
                {
                    entityInstance = record;
                }

                int maxLabelSize = entityStructure.Fields.Max(col => GetTextSize(col.fieldname));
                int maxDataSize = control.Width - maxLabelSize - 20;

                int currentTop = starttop;
                foreach (var col in entityStructure.Fields)
                {
                    var colDefault = defaults.FirstOrDefault(o => o.PropertyName == col.fieldname) ??
                                     defaults.FirstOrDefault(o => col.fieldname.Contains(o.PropertyName));
                    var fk = entityStructure.Relations.FirstOrDefault(f => f.EntityColumnID == col.fieldname);

                    var label = new Label
                    {
                        Top = currentTop,
                        Left = 10,
                        AutoSize = false,
                        BorderStyle = BorderStyle.FixedSingle,
                        Text = col.fieldname,
                        BackColor = Color.White,
                        ForeColor = Color.Red,
                        Width = maxLabelSize,
                        Height = 20,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top
                    };

                    Control dataControl;
                    if (fk != null)
                    {
                        dataControl = new ComboBox
                        {
                            Left = label.Right + 10,
                            Top = currentTop,
                            Width = maxDataSize,
                            Height = label.Height,
                            Anchor = AnchorStyles.Top
                        };
                        var combo = (ComboBox)dataControl;
                        combo.DataSource = GetDisplayLookup(datasourceid, fk.RelatedEntityID, fk.RelatedEntityColumnID, fk.EntityColumnID);
                        combo.DisplayMember = "DisplayField";
                        combo.ValueMember = fk.RelatedEntityColumnID;
                        combo.DataBindings.Add(new Binding("SelectedValue", EntityBindingSource, col.fieldname, true));
                    }
                    else
                    {
                        switch (col.fieldtype)
                        {
                            case "System.DateTime":
                                dataControl = new DateTimePicker
                                {
                                    Left = label.Right + 10,
                                    Top = currentTop,
                                    Width = maxDataSize,
                                    Height = label.Height,
                                    Anchor = AnchorStyles.Top
                                };
                                dataControl.DataBindings.Add(new Binding("Value", EntityBindingSource, col.fieldname, true));
                                break;
                            case "System.TimeSpan":
                            case "System.String":
                            default:
                                dataControl = new TextBox
                                {
                                    Left = label.Right + 10,
                                    Top = currentTop,
                                    Width = maxDataSize,
                                    Height = label.Height,
                                    Anchor = AnchorStyles.Top,
                                    TextAlign = HorizontalAlignment.Left
                                };
                                dataControl.DataBindings.Add(new Binding("Text", EntityBindingSource, col.fieldname, true));
                                break;
                            case "System.Boolean":
                                dataControl = new CheckBox
                                {
                                    Left = label.Right + 10,
                                    Top = currentTop,
                                    Width = maxDataSize,
                                    Height = label.Height,
                                    Anchor = AnchorStyles.Top,
                                    Text = ""
                                };
                                dataControl.DataBindings.Add(new Binding("Checked", EntityBindingSource, col.fieldname, true));
                                break;
                        }
                    }

                    if (entityStructure.PrimaryKeys.Any(x => x.fieldname == col.fieldname))
                    {
                        if (entityStructure.Relations.Any(x => x.EntityColumnID == col.fieldname) && TranType != TransActionType.Insert)
                            dataControl.Enabled = false;
                        if (col.IsAutoIncrement)
                            dataControl.Enabled = false;
                    }

                    control.Controls.AddRange([label, dataControl]);
                    currentTop = label.Bottom + 1;
                }

                return control;
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Ex = ex;
                DMEEditor.Logger.WriteLog($"Error in Loading View ({ex.Message})");
                return ErrorsandMesseges;
            }
        }

        public List<AppFilter> CreateEntityFilterControls(EntityStructure entityStructure, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
            List<AppFilter> filters = new List<AppFilter>();
            CreateFilterFields(entityStructure, dsdefaults, filters, passedArgs);
            return filters;
        }

        public List<AppFilter> CreateFieldsFilterControls(List<EntityField> fields, List<AppFilter> filters,
            List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
            filters ??= new List<AppFilter>();
            filters.Clear();
            EntityStructure structure = new EntityStructure { Fields = fields, Filters = filters };
            CreateFilterFields(structure, dsdefaults, filters, passedArgs);
            return filters;
        }

        private void CreateFilterFields(EntityStructure entityStructure, List<DefaultValue> dsdefaults,
            List<AppFilter> filters, IPassedArgs passedArgs = null)
        {
            if (passedArgs?.Objects.Any(i => i.Name == "FilterPanel") == true)
            {
                CrudFilterPanel = (Control)passedArgs.Objects.First(c => c.Name == "FilterPanel").obj;
            }

            CrudFilterPanel?.Controls.Clear();
            if (CrudFilterPanel == null) return;

            int maxLabelSize = entityStructure.Fields.Max(col => GetTextSize(col.fieldname.ToUpper()));
            int maxDataSize = DisplayPanel?.Width - maxLabelSize - 20 ?? 200;
            int startLeft = maxLabelSize + 90;
            int valueWidth = 100;
            int startHeight = 25;

            var bindingData = new BindingSource[entityStructure.Fields.Count];
            for (int i = 0; i < entityStructure.Fields.Count; i++)
            {
                var filter = new AppFilter
                {
                    FieldName = entityStructure.Fields[i].fieldname,
                    Operator = null,
                    FilterValue = null,
                    FilterValue1 = null,
                    valueType = entityStructure.Fields[i].fieldtype
                };
                filters.Add(filter);
                bindingData[i] = new BindingSource { DataSource = filter };

                var col = entityStructure.Fields[i];
                try
                {
                    var fk = entityStructure.Relations.FirstOrDefault(f => f.EntityColumnID == col.fieldname);
                    var label = new Label
                    {
                        Top = startHeight,
                        Left = 10,
                        AutoSize = false,
                        BorderStyle = BorderStyle.FixedSingle,
                        Text = col.fieldname.ToUpper(),
                        BackColor = Color.White,
                        ForeColor = Color.Red,
                        Width = maxLabelSize,
                        Height = 20,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    var conditionCombo = new ComboBox
                    {
                        Left = label.Right + 10,
                        Top = startHeight,
                        DataSource = AddFilterTypes(),
                        DisplayMember = "FilterDisplay",
                        ValueMember = "FilterValue",
                        Name = $"{col.fieldname}.Operator",
                        Width = 50,
                        Height = label.Height
                    };
                    conditionCombo.DataBindings.Add(new Binding("SelectedValue", bindingData[i], "Operator", true, DataSourceUpdateMode.OnPropertyChanged));

                    Control valueControl = fk != null
                        ? new ComboBox
                        {
                            Left = startLeft,
                            Top = startHeight,
                            DataSource = GetDisplayLookup(entityStructure.DataSourceID, fk.RelatedEntityID, fk.RelatedEntityColumnID, fk.EntityColumnID),
                            DisplayMember = "DisplayField",
                            ValueMember = fk.RelatedEntityColumnID,
                            Width = valueWidth,
                            Height = label.Height,
                            Name = col.fieldname
                        }
                        : new TextBox
                        {
                            Left = startLeft,
                            Top = startHeight,
                            Width = valueWidth,
                            Height = label.Height,
                            Name = col.fieldname
                        };
                    valueControl.DataBindings.Add(new Binding(fk != null ? "SelectedValue" : "Text", bindingData[i], "FilterValue", true));

                    CrudFilterPanel.Controls.AddRange([label, conditionCombo, valueControl]);
                    startHeight = label.Bottom + 1;
                }
                catch (Exception ex)
                {
                    DMEEditor.Logger.WriteLog($"Error creating filter for {col.fieldname}: {ex.Message}");
                }
            }
        }
        #endregion

        #region Utility Methods
        public int GetTextSize(string text)
        {
            using (var font = new Font("Courier New", 10.0F))
            using (var fakeImage = new Bitmap(1, 1))
            using (var graphics = Graphics.FromImage(fakeImage))
            {
                SizeF size = graphics.MeasureString(text ?? string.Empty, font);
                return (int)size.Width;
            }
        }

        public object GetDisplayLookup(string datasourceid, string parentEntityName, string parentKeyField, string entityField)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parentEntityName))
                    return null;

                var ds = DMEEditor.GetDataSource(datasourceid);
                var entity = ds.GetEntityStructure(parentEntityName, false);
                var defaults = ds.Dataconnection.ConnectionProp.DatasourceDefaults
                    ?.Where(o => o.propertyType == DefaultValueType.DisplayLookup)
                    .ToList() ?? new List<DefaultValue>();

                DisplayField = defaults.FirstOrDefault(p => p.PropertyName.Equals(entityField, StringComparison.OrdinalIgnoreCase))?.PropertyValue ??
                               entity.Fields.FirstOrDefault(r => r.fieldname.Contains("NAME", StringComparison.OrdinalIgnoreCase))?.fieldname ??
                               parentKeyField;

                string query = $"SELECT {DisplayField} AS DisplayField, {parentKeyField} FROM {parentEntityName}";
                return ds.RunQuery(query);
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in GetDisplayLookup: {ex.Message}");
                return null;
            }
        }

        public bool ShowAlert(string title, string message, string iconPath = null)
        {
            try
            {
                using var notifyIcon = new NotifyIcon
                {
                    Visible = true,
                    BalloonTipTitle = title,
                    BalloonTipText = message,
                    Icon = string.IsNullOrEmpty(iconPath) ? SystemIcons.Exclamation : new Icon(iconPath)
                };
                notifyIcon.ShowBalloonTip(3000);
                return true;
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"ShowAlert failed: {ex.Message}", DateTime.Now, -1, "ShowAlert", Errors.Failed);
                return false;
            }
        }
        #endregion
    }

  
}