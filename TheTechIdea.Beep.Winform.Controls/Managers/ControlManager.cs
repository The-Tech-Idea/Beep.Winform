﻿
using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public   class ControlManager : IControlManager
    {
        public   ControlManager(IDMEEditor pdmeeditor, Vis.Modules.IAppManager pVismanager)
        {
            DMEEditor = pdmeeditor;
            Vismanager = pVismanager;
           // vismanager = (VisManager)pVismanager;
           // DisplayPanel = (Control)vismanager.Container;
        }
        string DisplayField;
        public virtual  event EventHandler<IPassedArgs> PreCallModule;
        public virtual  event EventHandler<IPassedArgs> PreShowItem;
        public virtual  IDMEEditor DMEEditor { get; set; }
        public virtual Vis.Modules.IAppManager Vismanager { get; set; }
        //private VisManager vismanager { get; set; }
        public virtual  Control DisplayPanel { get; set; }
        public virtual  Control CrudFilterPanel { get; set; }
        public virtual  BindingSource EntityBindingSource { get; set; }
        public virtual  ErrorsInfo ErrorsandMesseges { get; set; }
        #region "MessegeBox and Dialogs"
        public virtual BeepDialogResult InputBoxYesNo(string title, string promptText)
        {
            // Use DialogHelper for InputBoxYesNo
            return DialogHelper.InputBoxYesNoAsync(title, promptText).GetAwaiter().GetResult();
        }

        public virtual BeepDialogResult InputBox(string title, string promptText, ref string value)
        {
            // Use DialogHelper for InputBox
            var (result, updatedValue) = DialogHelper.InputBoxAsync(title, promptText, value).GetAwaiter().GetResult();
            if (result == BeepDialogResult.OK)
            {
                value = updatedValue;
            }
            return result;
        }

        public virtual BeepDialogResult InputLargeBox(string title, string promptText, ref string value)
        {
            // Use DialogHelper for InputLargeBox
            var (result, updatedValue) = DialogHelper.InputLargeBoxAsync(title, promptText, value).GetAwaiter().GetResult();
            if (result == BeepDialogResult.OK)
            {
                value = updatedValue;
            }
            return result;
        }

        public virtual void MsgBox(string title, string promptText)
        {
            try
            {
                // Use DialogHelper for MsgBox
                DialogHelper.ShowMessageBoxAsync(title, promptText).GetAwaiter().GetResult();
                DMEEditor?.AddLogMessage("Success", "Displayed MsgBox", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string message = "Could not display MsgBox";
                DMEEditor?.AddLogMessage(ex.Message, message, DateTime.Now, -1, message, Errors.Failed);
            }
        }

        public virtual BeepDialogResult InputComboBox(string title, string promptText, List<string> itvalues, ref string value)
        {
            try
            {
                // Use DialogHelper for InputComboBox
                var (result, updatedValue) = DialogHelper.InputComboBoxAsync(title, promptText, itvalues, value).GetAwaiter().GetResult();
                if (result == BeepDialogResult.OK)
                {
                    value = updatedValue;
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = "Could not display InputComboBox";
                DMEEditor?.AddLogMessage(ex.Message, message, DateTime.Now, -1, message, Errors.Failed);
                return BeepDialogResult.Cancel;
            }
        }

        public virtual string DialogCombo(string text, List<object> comboSource, string displayMember, string valueMember)
        {
            // Use DialogHelper for DialogCombo
            var (result, selectedValue) = DialogHelper.DialogComboAsync(text, comboSource, displayMember, valueMember).GetAwaiter().GetResult();
            return result == BeepDialogResult.OK ? selectedValue : null;
        }
        public virtual  List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog()
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = dir


                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };

            openFileDialog1.Multiselect = true;
            BeepDialogResult result = MapDialogResult(openFileDialog1.ShowDialog());

            return openFileDialog1.FileNames.ToList();
        }
        public virtual  string SelectFolderDialog()
        {
            string folderPath = string.Empty;
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            string specialFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            folderBrowser.SelectedPath = specialFolderPath;
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = folderBrowser.SelectedPath;
                // ...
            }
            return folderPath;
        }
        public virtual  string LoadFileDialog(string exts, string dir,string filter)
        {
            OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog()
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = dir


                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };
            openFileDialog1.InitialDirectory = dir;

            BeepDialogResult result = MapDialogResult(openFileDialog1.ShowDialog());

            return openFileDialog1.FileName;
        }
        public virtual  string SaveFileDialog(string exts, string dir, string filter)
        {
            SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "Save File",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = dir


                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };
          

            BeepDialogResult result = MapDialogResult(saveFileDialog1.ShowDialog());

            return saveFileDialog1.FileName;
        }
        public virtual string ShowSpecialDirectoriesComboBox()
        {
            // Prepare the list of special directories
            var specialDirectories = Enum.GetValues(typeof(Environment.SpecialFolder))
                                         .Cast<Environment.SpecialFolder>()
                                         .Select(folder => folder.ToString())
                                         .ToList();

            // Use DialogHelper to display the ComboBox
            var (result, selectedValue) = DialogHelper.InputComboBoxAsync(
                title: "Special Directories",
                promptText: "Select a special directory:",
                items: specialDirectories,
                selectedValue: specialDirectories.First()
            ).GetAwaiter().GetResult();

            // Return the selected directory path if confirmed
            if (result == BeepDialogResult.OK && Enum.TryParse(selectedValue, out Environment.SpecialFolder selectedFolder))
            {
                return Environment.GetFolderPath(selectedFolder);
            }

            // If canceled, return null
            return null;
        }

        public virtual string SelectFile(string filter)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set the filter for the file extension and default file extension 
                openFileDialog.Filter = filter;

                // Display the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the path of the selected file
                    string selectedFilePath = openFileDialog.FileName;
                    return selectedFilePath;
                }
            }

            return null; // or return an empty string, depending on how you want to handle the cancellation
        }
        public virtual void ShowMessege(string title, string message, string iconPath = null)
        {
            try
            {
                // Create and configure the NotifyIcon
                NotifyIcon notifyIcon = new NotifyIcon
                {
                    Icon = string.IsNullOrEmpty(iconPath)
                        ? SystemIcons.Information // Default icon
                        : new Icon(iconPath),     // Custom icon if provided
                    Visible = true,
                    BalloonTipIcon = ToolTipIcon.Info, // Default ToolTip icon
                    BalloonTipTitle = title,
                    BalloonTipText = message
                };

                // Config the balloon tip
                notifyIcon.ShowBalloonTip(3000);

                // Dispose of the NotifyIcon after the balloon tip is displayed
                Task.Delay(3100).ContinueWith(_ => notifyIcon.Dispose());
            }
            catch (Exception ex)
            {
                // Log the error (assuming you have a logging mechanism)
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, -1, "ShowMessage", Errors.Failed);
            }
        }

        #endregion
        #region "CRUD"
        public virtual  object GenerateEntityonControl(string entityname, object record, int starttop, string datasourceid, TransActionType TranType, IPassedArgs passedArgs=null)
        {
            Control control = new Control();
            
            if (passedArgs.Objects.Where(c => c.Name == "BindingSource").Any())
            {
                EntityBindingSource = (BindingSource)passedArgs.Objects.Where(c => c.Name == "BindingSource").FirstOrDefault().obj;
            }
            if (passedArgs.Objects.Where(c => c.Name == "Control").Any())
            {
                control = (Control)passedArgs.Objects.Where(c => c.Name == "Control").FirstOrDefault().obj;
            }
            
            Dictionary<string, Control> controls = new Dictionary<string, Control>();
            ErrorsandMesseges = new ErrorsInfo();
            ErrorsandMesseges.Flag = Errors.Ok;
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
            List<DefaultValue> defaults = DMEEditor.ConfigEditor.DataConnections[DMEEditor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == ds.DatasourceName)].DatasourceDefaults;
            TableCurrentEntity = ds.GetEntityStructure(entityname, false);
            Type enttype = ds.GetEntityType(entityname);
            var ti = Activator.CreateInstance(enttype);
            // ICustomTypeDescriptor, IEditableObject, IDataErrorInfo, INotifyPropertyChanged
            if (record.GetType().GetInterfaces().Contains(typeof(ICustomTypeDescriptor)))
            {
                DataRowView dv = (DataRowView)record;
                DataRow dr = dv.Row;
                foreach (EntityField col in TableCurrentEntity.Fields)
                {
                    // TrySetProperty<enttype>(ti, dr[col.fieldname], null);
                    if (dr[col.fieldname] != System.DBNull.Value)
                    {
                        System.Reflection.PropertyInfo PropAInfo = enttype.GetProperty(col.fieldname);
                        PropAInfo.SetValue(ti, dr[col.fieldname], null);
                    }else
                    {
                       
                        System.Reflection.PropertyInfo PropAInfo = enttype.GetProperty(col.fieldname);
                        PropAInfo.SetValue(ti, null, null);
                    }

                }


            }
            else
            {
                ti = record;
            }
          //  EntityBindingSource.Add(ti);
            //bindingsource.CurrentChanged += Bindingsource_CurrentChanged;
            //bindingsource.PositionChanged += Bindingsource_PositionChanged;
            //bindingsource.ListChanged += Bindingsource_ListChanged;
            // Create Filter Control
            // CreateFilterQueryGrid(entityname, TableCurrentEntity.Fields, null);
            //--- Get Max label size
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
                var starth = starttop;

                TableCurrentEntity.Filters = new List<TheTechIdea.Beep.Report.AppFilter>();
                Control CurCTL=new Control();
                foreach (EntityField col in TableCurrentEntity.Fields)
                {
                    DefaultValue coldefaults = defaults.Where(o => o.PropertyName == col.fieldname).FirstOrDefault();
                    if (coldefaults == null)
                    {
                        coldefaults = defaults.Where(o => col.fieldname.Contains(o.PropertyName)).FirstOrDefault();
                    }
                    string coltype = col.fieldtype;
                    RelationShipKeys FK = TableCurrentEntity.Relations.Where(f => f.EntityColumnID == col.fieldname).FirstOrDefault();
                    //----------------------
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
                    l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    l.Width = maxlabelsize;
                    //---------------------

                    if (FK != null)
                    {
                        ComboBox cb = new ComboBox
                        {
                            Left = l.Left + l.Width + 10,
                            Top = starth
                        };
                       
                        cb.DataSource = GetDisplayLookup(datasourceid, FK.RelatedEntityID, FK.RelatedEntityColumnID, FK.EntityColumnID);
                        cb.DisplayMember = "DisplayField";
                        cb.ValueMember = FK.RelatedEntityColumnID;
                        cb.Width = maxDatasize;
                        cb.Height = l.Height;
                        cb.DataBindings.Add(new System.Windows.Forms.Binding("SelectedDisplayValue", EntityBindingSource, col.fieldname, true));
                        // cb.SelectedValueChanged += Cb_SelectedValueChanged;
                        cb.Anchor = AnchorStyles.Top;
                        control.Controls.Add(cb);
                        CurCTL = cb;
                        controls.Add(col.fieldname, cb);
                        starth = l.Bottom + 1;
                    }
                    else
                    {
                        switch (coltype)
                        {
                            case "System.DateTime":
                                DateTimePicker dt = new DateTimePicker
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth
                                };
                                 dt.DataBindings.Add(new System.Windows.Forms.Binding("Value", EntityBindingSource, col.fieldname, true));
                                dt.Width = maxDatasize;
                                dt.Height = l.Height;
                                //  dt.ValueChanged += Dt_ValueChanged;
                                dt.Anchor = AnchorStyles.Top;
                                control.Controls.Add(dt);
                                CurCTL = dt;
                                controls.Add(col.fieldname, dt);
                                break;
                            case "System.TimeSpan":
                                t1 = new TextBox
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth
                                };

                               t1.DataBindings.Add(new System.Windows.Forms.Binding("Text", EntityBindingSource, col.fieldname, true));
                                t1.TextAlign = HorizontalAlignment.Left;
                                t1.Width = maxDatasize;
                                t1.Height = l.Height;
                                control.Controls.Add(t1);
                                controls.Add(col.fieldname, t1);
                                //   t1.TextChanged += T_TextChanged;
                                //   t1.KeyPress += T_KeyPress;
                                CurCTL = t1;
                                t1.Anchor = AnchorStyles.Top;
                                break;
                            case "System.Boolean":
                                CheckBox ch1 = new CheckBox
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth
                                };

                                ch1.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", EntityBindingSource, col.fieldname, true));
                                ch1.Text = "";
                                ch1.Width = maxDatasize;
                                ch1.Height = l.Height;
                               // ch1.CheckStateChanged += Ch1_CheckStateChanged; ;
                                ch1.Anchor = AnchorStyles.Top;
                                control.Controls.Add(ch1);
                                CurCTL = ch1;
                                controls.Add(col.fieldname, ch1);

                                break;
                            case "System.Char":
                                BeepCheckBox<Char> ch2 = new BeepCheckBox<Char>
                                {
                                    Left = l.Left + l.Width + 10,
                                    Top = starth
                                };

                                ch2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", EntityBindingSource, col.fieldname, true));
                                ch2.Text = "";
                                ch2.Width = maxDatasize;
                                ch2.Height = l.Height;
                                string[] v = coldefaults.PropertyValue.Split(',');

                                if (coldefaults != null)
                                {
                                    ch2.CheckedValue = v[0].ToCharArray()[0];
                                    ch2.UncheckedValue = v[1].ToCharArray()[0];
                                }
                                //ch2.CheckStateChanged += Ch1_CheckStateChanged; ;
                                ch2.Anchor = AnchorStyles.Top;
                                control.Controls.Add(ch2);
                                CurCTL = ch2;
                                controls.Add(col.fieldname, ch2);
                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Decimal":
                            case "System.Double":
                            case "System.Single":
                            case "System.String":
                                if (TableCurrentEntity.Fields.Where(p => p.fieldname == col.fieldname).FirstOrDefault().Size1 > 1)
                                {
                                    t1 = new TextBox
                                    {
                                        Left = l.Left + l.Width + 10,
                                        Top = starth
                                    };

                                    t1.DataBindings.Add(new System.Windows.Forms.Binding("Text", EntityBindingSource, col.fieldname, true));
                                    t1.TextAlign = HorizontalAlignment.Left;
                                    t1.Width = maxDatasize;
                                    t1.Height = l.Height;

                                    //      t1.TextChanged += T_TextChanged;
                                    //      t1.KeyPress += T_KeyPress;
                                    CurCTL = t1;
                                    control.Controls.Add(t1);
                                    controls.Add(col.fieldname, t1);
                                    t1.Anchor = AnchorStyles.Top;
                                }
                                else
                                {
                                    ch2 = new BeepCheckBox<Char>
                                    {
                                        Left = l.Left + l.Width + 10,
                                        Top = starth
                                    };

                                    ch2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", EntityBindingSource, col.fieldname, true));
                                    ch2.Text = "";
                                    ch2.Width = maxDatasize;
                                    ch2.Height = l.Height;



                                    if (coldefaults != null)
                                    {
                                        v = coldefaults.PropertyValue.Split(',');
                                        ch2.CheckedValue = v[0].ToCharArray()[0];
                                        ch2.UncheckedValue = v[1].ToCharArray()[0];
                                    }
                                    //      ch2.CheckStateChanged += Ch1_CheckStateChanged; ;
                                    CurCTL = ch2;
                                    control.Controls.Add(ch2);
                                    controls.Add(col.fieldname, ch2);
                                    ch2.Anchor = AnchorStyles.Top;
                                }
                                break;
                            default:
                                TextBox t = new TextBox();

                                t.Left = l.Left + l.Width + 10;
                                t.Top = starth;
                                t.DataBindings.Add(new System.Windows.Forms.Binding("Text", EntityBindingSource, col.fieldname, true));
                                t.TextAlign = HorizontalAlignment.Left;
                                t.Width = maxDatasize;
                                t.Height = l.Height;

                                //  t.TextChanged += T_TextChanged;
                                //   t.KeyPress += T_KeyPress;
                              
                                t.Anchor = AnchorStyles.Top;
                                CurCTL = t;
                                control.Controls.Add(t);
                                controls.Add(col.fieldname, t);
                                break;

                        }
                    }
                   
                    if (TableCurrentEntity.PrimaryKeys.Any(x => x.fieldname == col.fieldname))
                    {
                        if (TableCurrentEntity.Relations.Any(x => x.EntityColumnID == col.fieldname) && TranType != TransActionType.Insert)
                        {
                            CurCTL.Enabled = false;
                        }
                        if (col.IsAutoIncrement )
                        {
                            CurCTL.Enabled = false;
                        }

                    }
                   

                    l.Anchor = AnchorStyles.Top;
                    control.Controls.Add(l);

                    starth = l.Bottom + 1;
                    //this.databaseTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dataConnectionsEntityBindingSource, "Database", true));

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
        public virtual List<AppFilter> CreateEntityFilterControls( EntityStructure entityStructure, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
            List<AppFilter> Filters = new List<AppFilter>();
            if (Filters != null)
            {
                Filters.Clear();
            }
            else
                Filters = new List<AppFilter>();
             CreateFilterFields(entityStructure,dsdefaults,passedArgs);
            return Filters;

        }
        public  virtual List<AppFilter> CreateFieldsFilterControls(List<EntityField> Fields, List<AppFilter> Filters, List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
          
            if (Filters != null)
            {
                Filters.Clear();
            }
            else
                Filters = new List<AppFilter>();

            List<string> FieldNames = new List<string>();
            EntityStructure str=new EntityStructure() { Fields=Fields, Filters=Filters};
            CreateFilterFields(str,  dsdefaults, passedArgs);
            return Filters;


        }
        private void CreateFilterFields(EntityStructure entityStructure,  List<DefaultValue> dsdefaults, IPassedArgs passedArgs = null)
        {
            if (passedArgs.Objects.Where(i => i.Name == "FilterPanel").Any())
            {
                CrudFilterPanel = (Control)passedArgs.Objects.Where(c => c.Name == "FilterPanel").FirstOrDefault().obj;
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
            {
                entityStructure.Filters.Clear();
            }
            else
                entityStructure.Filters = new List<AppFilter>();

            List<string> FieldNames = new List<string>();
            var starth = 25;
            int startleft = maxlabelsize + 90;
            int valuewidth = 100;
            for (int i = 0; i <= entityStructure.Fields.Count - 1; i++)
            {
                AppFilter r = new AppFilter();
                r.FieldName = entityStructure.Fields[i].fieldname;
                r.Operator = null;
                r.FilterValue = null;
                r.FilterValue1 = null;
                r.valueType = entityStructure.Fields[i].fieldtype;
                entityStructure.Filters.Add(r);
                BindingData[i] = new BindingSource();
                BindingData[i].DataSource = r;
                FieldNames.Add(entityStructure.Fields[i].fieldname);
                EntityField col = entityStructure.Fields[i];
                try
                {
                    DefaultValue coldefaults = dsdefaults.Where(o => o.PropertyName == col.fieldname).FirstOrDefault();
                    if (coldefaults == null)
                    {
                        coldefaults = dsdefaults.Where(o => col.fieldname.Contains(o.PropertyName)).FirstOrDefault();
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

                    //cbcondition.DataBindingSource = GetDisplayLookup(entityStructure.DataSourceID, FK.ParentEntityID, FK.ParentEntityColumnID, FK.EntityColumnID);
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
                    cbcondition.DataBindings.Add(new System.Windows.Forms.Binding("SelectedDisplayValue", BindingData[i], "Operator", true, DataSourceUpdateMode.OnPropertyChanged));
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
                        cb.DataBindings.Add(new System.Windows.Forms.Binding("SelectedDisplayValue", BindingData[i], "FilterValue", true));
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
                                dt.DataBindings.Add(new System.Windows.Forms.Binding("Value", BindingData[i], "FilterValue", true));

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

                                dt1.DataBindings.Add(new System.Windows.Forms.Binding("Value", BindingData[i], "FilterValue1", true));

                                dt1.Width = valuewidth;
                                dt1.Height = l.Height;
                                dt1.Format = DateTimePickerFormat.Short;
                                //   dt1.ValueChanged += Dt_ValueChanged;
                                //     dt.Anchor = AnchorStyles.Top;
                                dt1.Tag = i;
                                dt1.Name = "From" + "." + col.fieldname;
                                CrudFilterPanel.Controls.Add(dt1);
                                break;
                            case "System.TimeSpan":
                                TextBox t1 = new TextBox
                                {
                                    Left = startleft,
                                    Top = starth
                                };

                                t1.DataBindings.Add(new System.Windows.Forms.Binding("Text", BindingData[i], "FilterValue", true));
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

                                ch1.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", BindingData[i], "FilterValue", true));
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

                                ch2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", BindingData[i], "FilterValue", true));
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
                                t3.DataBindings.Add(new System.Windows.Forms.Binding("Value", BindingData[i], "FilterValue", true));
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
                                t2.DataBindings.Add(new System.Windows.Forms.Binding("Value", BindingData[i], "FilterValue1", true));
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

                                    t1.DataBindings.Add(new System.Windows.Forms.Binding("Text", BindingData[i], "FilterValue", true));
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

                                    ch2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", BindingData[i], "FilterValue", true));
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
                                t.DataBindings.Add(new System.Windows.Forms.Binding("Text", BindingData[i], "FilterValue", true));
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
                    }
                    // l.Anchor = AnchorStyles.Top;
                    CrudFilterPanel.Controls.Add(l);
                    starth = l.Bottom + 1;

                }
                catch (Exception ex)
                {

                    // Logger.WriteLog($"Error in Loading View ({ex.Message}) ");
                }

            }
        }
        #endregion
        #region "util"
        public virtual  List<FilterType> AddFilterTypes()
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
        private System.Windows.Forms.DialogResult MapDialogResult(BeepDialogResult dialogResult)
        {
            System.Windows.Forms.DialogResult retval = System.Windows.Forms.DialogResult.None;
            switch (dialogResult)
            {

                case BeepDialogResult.None:
                    retval = System.Windows.Forms.DialogResult.None;
                    break;
                case BeepDialogResult.OK:
                    retval = System.Windows.Forms.DialogResult.OK;
                    break;
                case BeepDialogResult.Cancel:
                    retval = System.Windows.Forms.DialogResult.Cancel;
                    break;
                case BeepDialogResult.Abort:
                    retval = System.Windows.Forms.DialogResult.Abort;
                    break;
                case BeepDialogResult.Retry:
                    retval = System.Windows.Forms.DialogResult.Retry;
                    break;
                case BeepDialogResult.Ignore:
                    retval = System.Windows.Forms.DialogResult.Ignore;
                    break;
                case BeepDialogResult.Yes:
                    retval = System.Windows.Forms.DialogResult.Yes;
                    break;
                case BeepDialogResult.No:
                    retval = System.Windows.Forms.DialogResult.No;
                    break;
                default:
                    retval = System.Windows.Forms.DialogResult.None;
                    break;
            }
            return retval;
        }
        private BeepDialogResult MapDialogResult(System.Windows.Forms.DialogResult dialogResult)
        {
            BeepDialogResult retval = BeepDialogResult.None;
            switch (dialogResult)
            {

                case System.Windows.Forms.DialogResult.None:
                    retval = BeepDialogResult.None;
                    break;
                case System.Windows.Forms.DialogResult.OK:
                    retval = BeepDialogResult.OK;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                    retval = BeepDialogResult.Cancel;
                    break;
                case System.Windows.Forms.DialogResult.Abort:
                    retval = BeepDialogResult.Abort;
                    break;
                case System.Windows.Forms.DialogResult.Retry:
                    retval = BeepDialogResult.Retry;
                    break;
                case System.Windows.Forms.DialogResult.Ignore:
                    retval = BeepDialogResult.Ignore;
                    break;
                case System.Windows.Forms.DialogResult.Yes:
                    retval = BeepDialogResult.Yes;
                    break;
                case System.Windows.Forms.DialogResult.No:
                    retval = BeepDialogResult.No;
                    break;
                default:
                    retval = BeepDialogResult.None;
                    break;
            }
            return retval;
        }
        public virtual  int getTextSize(string text)
        {
            Font font = new Font("Courier New", 10.0F);
            Image fakeImage = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF size = graphics.MeasureString(text, font);
            return Convert.ToInt32(size.Width);
        }
        public virtual  object GetDisplayLookup(string datasourceid, string Parententityname, string ParentKeyField,string EntityField)
        {
            object retval=null;
            try
            {
                if (Parententityname != null)
                {
                    IDataSource ds = DMEEditor.GetDataSource(datasourceid);
                    EntityStructure ent = ds.GetEntityStructure(Parententityname, false);
                    DisplayField = null;
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
        public virtual bool ShowAlert(string title, string message, string iconPath = null)
        {
            try
            {
                // Create a NotifyIcon
                NotifyIcon notifyIcon = new NotifyIcon
                {
                    Visible = true,
                    BalloonTipTitle = title,
                    BalloonTipText = message,
                    Icon = string.IsNullOrEmpty(iconPath)
                        ? SystemIcons.Exclamation
                        : new Icon(iconPath) // Use custom icon if provided
                };

                // Display the balloon tip for 3 seconds
                notifyIcon.ShowBalloonTip(3000);

                // Dispose of the NotifyIcon after the balloon tip is displayed
                Task.Delay(3100).ContinueWith(_ => notifyIcon.Dispose());

                return true;
            }
            catch (Exception ex)
            {
                // Log the error (assuming you have a logging mechanism)
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, -1, "ShowAlert", Errors.Failed);
                return false;
            }
        }
        #endregion
    }
}
