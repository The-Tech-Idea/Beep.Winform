using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public class ControlManager : IControlManager
    {
        private IBeepService beepservices;
        public IDMEEditor DMEEditor { get; set; }
        public Control DisplayPanel { get; set; }
        public Control CrudFilterPanel { get; set; }
        public BindingSource EntityBindingSource { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; }
        string DisplayField;

        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;

        public ControlManager(IBeepService services)
        {
            beepservices = services;
            DMEEditor = beepservices.DMEEditor;
        }

        #region "Dialog Methods"
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
        #endregion

        #region "File and Folder Dialogs"
        public string SelectFile(string filter)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = filter;
                return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : null;
            }
        }

        public string LoadFileDialog(string exts, string dir, string filter)
        {
            return DialogHelper.LoadFileDialog(exts, dir, filter);
        }

        public List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            return DialogHelper.LoadFilesDialog(exts, dir, filter);
        }

        public string SaveFileDialog(string exts, string dir, string filter)
        {
            return DialogHelper.SaveFileDialog(exts, dir, filter);
        }

        public string ShowSpecialDirectoriesComboBox()
        {
            // If needed, implement a synchronous version here.
            // For now, return null or a default value.
            return null;
        }

        public string SelectFolderDialog()
        {
            return DialogHelper.SelectFolderDialog();
        }
        #endregion

        #region "Utility Methods"
        public List<FilterType> AddFilterTypes()
        {
            var lsop = new List<FilterType>
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
            return lsop;
        }

     

        public void ShowMessege(string title, string message, string icon)
        {
            // For a simple synchronous message display, use MessageBox.
            MessageBox.Show(message, title);
        }
        #endregion

        #region "CRUD and Other Synchronous Methods"
        public object GenerateEntityonControl(string entityname, object record, int starttop, string datasourceid, TransActionType TranType, IPassedArgs passedArgs = null)
        {
            Control control = new Control();

            if (passedArgs.Objects.Any(c => c.Name == "BindingSource"))
            {
                EntityBindingSource = (BindingSource)passedArgs.Objects.First(c => c.Name == "BindingSource").obj;
            }
            if (passedArgs.Objects.Any(c => c.Name == "Control"))
            {
                control = (Control)passedArgs.Objects.First(c => c.Name == "Control").obj;
            }

            Dictionary<string, Control> controls = new Dictionary<string, Control>();
            ErrorsandMesseges = new ErrorsInfo { Flag = Errors.Ok };
            TextBox t1 = new TextBox();
            EntityStructure TableCurrentEntity = new EntityStructure();
            DisplayField = null;
            if (record == null)
            {
                MessageBox.Show("Error", " record has no Data");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Message = "Error Table has no Data";
                return DMEEditor.ErrorObject;
            }

            IDataSource ds = DMEEditor.GetDataSource(datasourceid);
            List<DefaultValue> defaults = DMEEditor.ConfigEditor.DataConnections[
                DMEEditor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == ds.DatasourceName)]
                .DatasourceDefaults;
            TableCurrentEntity = ds.GetEntityStructure(entityname, false);
            Type enttype = ds.GetEntityType(entityname);
            var ti = Activator.CreateInstance(enttype);

            if (record.GetType().GetInterfaces().Contains(typeof(ICustomTypeDescriptor)))
            {
                DataRowView dv = (DataRowView)record;
                DataRow dr = dv.Row;
                foreach (EntityField col in TableCurrentEntity.Fields)
                {
                    var prop = enttype.GetProperty(col.fieldname);
                    if (dr[col.fieldname] != DBNull.Value)
                    {
                        prop?.SetValue(ti, dr[col.fieldname], null);
                    }
                    else
                    {
                        prop?.SetValue(ti, null, null);
                    }
                }
            }
            else
            {
                ti = record;
            }

            // --- Generate Controls for Each Field ---
            int maxlabelsize = 0;
            int maxDatasize = 0;
            foreach (EntityField col in TableCurrentEntity.Fields)
            {
                int x = getTextSize(col.fieldname);
                if (maxlabelsize < x)
                    maxlabelsize = x;
            }
            maxDatasize = control.Width - maxlabelsize - 20;
            try
            {
                int starth = starttop;
                TableCurrentEntity.Filters = new List<AppFilter>();
                Control CurCTL = new Control();
                foreach (EntityField col in TableCurrentEntity.Fields)
                {
                    DefaultValue coldefaults = defaults.FirstOrDefault(o => o.PropertyName == col.fieldname)
                        ?? defaults.FirstOrDefault(o => col.fieldname.Contains(o.PropertyName));
                    string coltype = col.fieldtype;
                    RelationShipKeys FK = TableCurrentEntity.Relations.FirstOrDefault(f => f.EntityColumnID == col.fieldname);

                    Label l = new Label
                    {
                        Top = starth,
                        Left = 10,
                        AutoSize = false,
                        BorderStyle = BorderStyle.FixedSingle,
                        Text = col.fieldname,
                        BackColor = Color.White,
                        ForeColor = Color.Red
                    };
                    l.Size = TextRenderer.MeasureText(col.fieldname, l.Font);
                    l.Height += 10;
                    l.TextAlign = ContentAlignment.MiddleCenter;
                    l.Width = maxlabelsize;

                    if (FK != null)
                    {
                        ComboBox cb = new ComboBox
                        {
                            Left = l.Left + l.Width + 10,
                            Top = starth,
                            Width = maxDatasize,
                            Height = l.Height,
                            Anchor = AnchorStyles.Top
                        };

                        cb.DataSource = GetDisplayLookup(datasourceid, FK.RelatedEntityID, FK.RelatedEntityColumnID, FK.EntityColumnID);
                        cb.DisplayMember = "DisplayField";
                        cb.ValueMember = FK.RelatedEntityColumnID;
                        cb.DataBindings.Add(new Binding("SelectedDisplayValue", EntityBindingSource, col.fieldname, true));
                        control.Controls.Add(cb);
                        CurCTL = cb;
                        starth = l.Bottom + 1;
                    }
                    else
                    {
                        // Create different control types based on field type
                        switch (coltype)
                        {
                            case "System.DateTime":
                                DateTimePicker dt = new DateTimePicker
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth,
                                    Width = maxDatasize,
                                    Height = l.Height,
                                    Anchor = AnchorStyles.Top
                                };
                                dt.DataBindings.Add(new Binding("Value", EntityBindingSource, col.fieldname, true));
                                control.Controls.Add(dt);
                                CurCTL = dt;
                                break;
                            case "System.TimeSpan":
                                t1 = new TextBox
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth,
                                    Width = maxDatasize,
                                    Height = l.Height,
                                    Anchor = AnchorStyles.Top,
                                    TextAlign = HorizontalAlignment.Left
                                };
                                t1.DataBindings.Add(new Binding("Text", EntityBindingSource, col.fieldname, true));
                                control.Controls.Add(t1);
                                CurCTL = t1;
                                break;
                            case "System.Boolean":
                                CheckBox ch1 = new CheckBox
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth,
                                    Width = maxDatasize,
                                    Height = l.Height,
                                    Anchor = AnchorStyles.Top,
                                    Text = ""
                                };
                                ch1.DataBindings.Add(new Binding("CheckState", EntityBindingSource, col.fieldname, true));
                                control.Controls.Add(ch1);
                                CurCTL = ch1;
                                break;
                            default:
                                TextBox t = new TextBox
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth,
                                    Width = maxDatasize,
                                    Height = l.Height,
                                    Anchor = AnchorStyles.Top,
                                    TextAlign = HorizontalAlignment.Left
                                };
                                t.DataBindings.Add(new Binding("Text", EntityBindingSource, col.fieldname, true));
                                control.Controls.Add(t);
                                CurCTL = t;
                                break;
                        }
                    }

                    // Disable primary keys if needed
                    if (TableCurrentEntity.PrimaryKeys.Any(x => x.fieldname == col.fieldname))
                    {
                        if (TableCurrentEntity.Relations.Any(x => x.EntityColumnID == col.fieldname) && TranType != TransActionType.Insert)
                        {
                            CurCTL.Enabled = false;
                        }
                        if (col.IsAutoIncrement)
                        {
                            CurCTL.Enabled = false;
                        }
                    }

                    l.Anchor = AnchorStyles.Top;
                    control.Controls.Add(l);
                    starth = l.Bottom + 1;
                }
            }
            catch (Exception ex)
            {
                ErrorsandMesseges.Flag = Errors.Failed;
                ErrorsandMesseges.Ex = ex;
                DMEEditor.Logger.WriteLog($"Error in Loading View ({ex.Message}) ");
            }
            return control;
        }

        public List<AppFilter> CreateEntityFilterControls(EntityStructure entityStructure, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
            List<AppFilter> Filters = new List<AppFilter>();
            Filters.Clear();
            CreateFilterFields(entityStructure, dsdefaults, passedArgs);
            return Filters;
        }

        public List<AppFilter> CreateFieldsFilterControls(List<EntityField> Fields, List<AppFilter> Filters, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
            if (Filters != null)
            {
                Filters.Clear();
            }
            else
            {
                Filters = new List<AppFilter>();
            }

            EntityStructure str = new EntityStructure() { Fields = Fields, Filters = Filters };
            CreateFilterFields(str, dsdefaults, passedArgs);
            return Filters;
        }

        private void CreateFilterFields(EntityStructure entityStructure, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
            if (passedArgs.Objects.Any(i => i.Name == "FilterPanel"))
            {
                CrudFilterPanel = (Control)passedArgs.Objects.First(c => c.Name == "FilterPanel").obj;
            }
            CrudFilterPanel.Controls.Clear();
            BindingSource[] BindingData = new BindingSource[entityStructure.Fields.Count];
            int maxlabelsize = 0;
            int maxDatasize = 0;
            foreach (EntityField col in entityStructure.Fields)
            {
                int x = getTextSize(col.fieldname.ToUpper());
                if (maxlabelsize < x)
                    maxlabelsize = x;
            }
            maxDatasize = DisplayPanel.Width - maxlabelsize - 20;
            if (entityStructure.Filters != null)
                entityStructure.Filters.Clear();
            else
                entityStructure.Filters = new List<AppFilter>();

            int starth = 25;
            int startleft = maxlabelsize + 90;
            int valuewidth = 100;
            for (int i = 0; i < entityStructure.Fields.Count; i++)
            {
                AppFilter r = new AppFilter
                {
                    FieldName = entityStructure.Fields[i].fieldname,
                    Operator = null,
                    FilterValue = null,
                    FilterValue1 = null,
                    valueType = entityStructure.Fields[i].fieldtype
                };
                entityStructure.Filters.Add(r);
                BindingData[i] = new BindingSource { DataSource = r };

                EntityField col = entityStructure.Fields[i];
                try
                {
                    DefaultValue coldefaults = dsdefaults.FirstOrDefault(o => o.PropertyName == col.fieldname)
                        ?? dsdefaults.FirstOrDefault(o => col.fieldname.Contains(o.PropertyName));
                    string coltype = col.fieldtype;
                    RelationShipKeys FK = entityStructure.Relations.FirstOrDefault(f => f.EntityColumnID == col.fieldname);

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
                    l.TextAlign = ContentAlignment.MiddleCenter;
                    l.Width = maxlabelsize;

                    ComboBox cbcondition = new ComboBox
                    {
                        Left = l.Left + l.Width + 10,
                        Top = starth,
                        DataSource = AddFilterTypes(),
                        DisplayMember = "FilterDisplay",
                        ValueMember = "FilterValue",
                        Name = col.fieldname + ".Operator",
                        Width = 50,
                        Height = l.Height
                    };
                    cbcondition.DataBindings.Add(new Binding("SelectedDisplayValue", BindingData[i], "Operator", true, DataSourceUpdateMode.OnPropertyChanged));
                    CrudFilterPanel.Controls.Add(cbcondition);

                    if (FK != null)
                    {
                        ComboBox cb = new ComboBox
                        {
                            Left = startleft,
                            Top = starth,
                            DataSource = GetDisplayLookup(entityStructure.DataSourceID, FK.RelatedEntityID, FK.RelatedEntityColumnID, FK.EntityColumnID),
                            DisplayMember = "DisplayField",
                            ValueMember = FK.RelatedEntityColumnID,
                            Width = valuewidth,
                            Height = l.Height
                        };
                        cb.DataBindings.Add(new Binding("SelectedDisplayValue", BindingData[i], "FilterValue", true));
                        CrudFilterPanel.Controls.Add(cb);
                        cb.Name = col.fieldname;
                        starth = l.Bottom + 1;
                    }
                    else
                    {
                        // Additional cases for filter controls (DateTime, NumericUpDown, etc.) can be added here.
                    }
                    CrudFilterPanel.Controls.Add(l);
                    starth = l.Bottom + 1;
                }
                catch (Exception ex)
                {
                    // Handle exceptions as needed.
                }
            }
        }
        #endregion

        #region "Utility Methods"
        public int getTextSize(string text)
        {
            using (Font font = new Font("Courier New", 10.0F))
            using (Image fakeImage = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(fakeImage))
            {
                SizeF size = graphics.MeasureString(text, font);
                return Convert.ToInt32(size.Width);
            }
        }

        public object GetDisplayLookup(string datasourceid, string Parententityname, string ParentKeyField, string EntityField)
        {
            object retval = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(Parententityname))
                {
                    IDataSource ds = DMEEditor.GetDataSource(datasourceid);
                    EntityStructure ent = ds.GetEntityStructure(Parententityname, false);
                    DisplayField = null;
                    List<DefaultValue> defaults = ds.Dataconnection.ConnectionProp.DatasourceDefaults
                        .Where(o => o.propertyType == DefaultValueType.DisplayLookup)
                        .ToList();

                    if (defaults != null)
                    {
                        DefaultValue defaultValue = defaults.FirstOrDefault(p => p.PropertyName.Equals(EntityField, StringComparison.InvariantCultureIgnoreCase));
                        if (defaultValue != null)
                        {
                            DisplayField = defaultValue.PropertyValue;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(DisplayField))
                    {
                        DisplayField = ent.Fields.FirstOrDefault(r => r.fieldname.Contains("NAME"))?.fieldname;
                    }
                    if (string.IsNullOrWhiteSpace(DisplayField))
                    {
                        DisplayField = ParentKeyField;
                    }
                    string qrystr = $"select {DisplayField} as DisplayField, {ParentKeyField} from {Parententityname}";
                    retval = ds.RunQuery(qrystr);
                }
                return retval;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ShowAlert(string title, string message, string iconPath = null)
        {
            try
            {
                NotifyIcon notifyIcon = new NotifyIcon
                {
                    Visible = true,
                    BalloonTipTitle = title,
                    BalloonTipText = message,
                    Icon = string.IsNullOrEmpty(iconPath) ? SystemIcons.Exclamation : new Icon(iconPath)
                };

                notifyIcon.ShowBalloonTip(3000);
                Task.Delay(3100).ContinueWith(_ => notifyIcon.Dispose());
                return true;
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, -1, "ShowAlert", Errors.Failed);
                return false;
            }
        }
        #endregion
    }
}
