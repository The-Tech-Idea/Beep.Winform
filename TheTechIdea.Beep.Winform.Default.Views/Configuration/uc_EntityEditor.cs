using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Container.Services;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Entity Editor", Name = "uc_EntityEditor", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 1, ID = 1, BranchText = "Entity Editor", BranchType = EnumPointType.Function, IconImageName = "entityeditor.svg", BranchClass = "ADDIN", BranchDescription = "Local DB Connections Setup Screen")]

    public partial class uc_EntityEditor : TemplateUserControl, IAddinVisSchema
    {
        public uc_EntityEditor(IServiceProvider services): base(services)
        {
            InitializeComponent();
          
            Details.AddinName = "Entity Editor";

        }
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 1;
        public int ID { get; set; } = 1;
        public string BranchText { get; set; } = "Entity Editor";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Function;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "entityeditor.svg";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Entity Editor Screen";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; }
        #endregion "IAddinVisSchema"
        EntityManagerViewModel viewModel;

        List<SimpleItem> Drivers = new List<SimpleItem>();
    
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            if (beepService == null || appManager == null)
            {
                return;
            }

            viewModel ??= new EntityManagerViewModel(beepService.DMEEditor, appManager);
            entityManagerViewModelBindingSource.DataSource = viewModel;

            DatasourcebeepComboBox.SelectedItemChanged -= DatasourcebeepComboBox_SelectedItemChanged;
            EntitiesbeepComboBox.SelectedItemChanged -= EntitiesbeepComboBox_SelectedItemChanged;
            ApplybeepButton.Click -= ApplybeepButton_Click;
            DatasourcebeepComboBox.SelectedItemChanged += DatasourcebeepComboBox_SelectedItemChanged;
            EntitiesbeepComboBox.SelectedItemChanged += EntitiesbeepComboBox_SelectedItemChanged;
            ApplybeepButton.Click += ApplybeepButton_Click;

            ApplyLayoutDefaults();
            DatasourcebeepComboBox.ListItems = new BindingList<SimpleItem>();
            EntitiesbeepComboBox.ListItems = new BindingList<SimpleItem>();
            DatasourcebeepComboBox.Text = string.Empty;
            EntitiesbeepComboBox.Text = string.Empty;
            Drivers.Clear();

            foreach (var item in beepService.Config_editor.DataConnections)
            {
                SimpleItem conn = new SimpleItem();
                conn.DisplayField = item.ConnectionName;
                conn.Text = item.ConnectionName;
                conn.Name = item.ConnectionName;
                conn.Value = item.ConnectionName;
                conn.GuidId = item.GuidID;
                conn.ParentItem = null;
                conn.ContainerGuidID = item.GuidID;
                DatasourcebeepComboBox.ListItems.Add(conn);
            }
            List<SimpleItem> versions = new List<SimpleItem>();
            foreach (var item in beepService.Config_editor.DataDriversClasses.Select(o=>o.PackageName))
            {
                SimpleItem driveritem = new SimpleItem();
                driveritem.DisplayField = item;
                driveritem.Text = item;
                driveritem.Name = item;
                driveritem.Value = item;
                foreach (var DriversClasse in beepService.Config_editor.DataDriversClasses.Where(x => x.PackageName == item))
                {
                    SimpleItem itemversion = new SimpleItem();
                    itemversion.DisplayField = DriversClasse.version;
                    itemversion.Value = DriversClasse.version;
                    itemversion.Text = DriversClasse.version;
                    itemversion.Name = DriversClasse.version;
                    itemversion.ParentItem = driveritem;
                    itemversion.ParentValue = item;
                    versions.Add(itemversion);
                }
                 Drivers.Add(driveritem);
            }
            SyncBindings();


            // idx = 0;
            //foreach (var item in viewModel.PackageVersions)
            //{
            //    SimpleItem driveritem = new SimpleItem();
            //    driveritem.IsDisplayField = item;
            //    driveritem.Value = idx++;
            //    driveritem.Text = item;
            //    driveritem.Name = item;
            //    driverversion.Items.Add(driveritem);
            //}
        }

        private void ApplyLayoutDefaults()
        {
            //beepWebHeaderAppBar1.Text = "Entity Editor";
            //beepButton1.Text = "Save";
            //beepButton2.Text = "Delete";

            DatasourcebeepComboBox.PlaceholderText = "Datasource";
            EntitiesbeepComboBox.PlaceholderText = "Entity";
            ApplybeepButton.Text = "Create";

            DatasourcebeepComboBox.Location = new System.Drawing.Point(16, 76);
            DatasourcebeepComboBox.Size = new System.Drawing.Size(260, 44);
            EntitiesbeepComboBox.Location = new System.Drawing.Point(292, 76);
            EntitiesbeepComboBox.Size = new System.Drawing.Size(220, 44);
            //beepButton1.Location = new System.Drawing.Point(528, 76);
            //beepButton1.Size = new System.Drawing.Size(96, 44);
            //beepButton2.Location = new System.Drawing.Point(636, 76);
            //beepButton2.Size = new System.Drawing.Size(96, 44);

            //beepSimpleGrid1.Location = new System.Drawing.Point(16, 140);
            //beepSimpleGrid1.Size = new System.Drawing.Size(908, 590);
        }

        private void DatasourcebeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            // set entiies from selected datasource
            if (e.SelectedItem != null)
            {
                string datasource = e.SelectedItem.Text;
                viewModel.Datasourcename = datasource;
                viewModel.SourceConnection = beepService.DMEEditor.GetDataSource(datasource);
                viewModel.EntityName = string.Empty;
                if (viewModel.SourceConnection != null)
                {
                    viewModel.UpdateFieldTypes();
                    ConfigureFieldTypeColumn();
                }
                if(viewModel.SourceConnection != null)
                {
                    if(viewModel.SourceConnection.ConnectionStatus != ConnectionState.Open)
                    {
                        viewModel.SourceConnection.Openconnection();
                        if(viewModel.SourceConnection.ConnectionStatus != ConnectionState.Open)
                        {
                            beepService.DMEEditor.AddLogMessage("Beep", "Datasource not Found", DateTime.Now, 0, null, Errors.Failed);
                            return;
                        }
                    }
                }else
                {
                    beepService.DMEEditor.AddLogMessage("Beep", "Datasource not Found", DateTime.Now, 0, null, Errors.Failed);
                    return;
                }

                viewModel.Structure = null;
                viewModel.DBWork = null;
                viewModel.Fields = null;
                viewModel.EntityName = null;
                SyncBindings();
                LoadEntitiesList();

            }
        }

        private void EntitiesbeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null || viewModel == null)
            {
                return;
            }

            LoadOrCreateEntity(e.SelectedItem.Text);
        }

        private void BeepButton1_Click(object? sender, EventArgs e)
        {
            if (viewModel == null)
            {
                return;
            }

            viewModel.IsChanged = true;
            viewModel.SaveEntity();
            LoadEntitiesList();
        }

        private void BeepButton2_Click(object? sender, EventArgs e)
        {
            if (viewModel == null)
            {
                return;
            }

            viewModel.DeleteEntity();
            LoadEntitiesList();
        }

        private void LoadEntitiesList()
        {
            EntitiesbeepComboBox.ListItems = new BindingList<SimpleItem>();
            if (viewModel?.SourceConnection == null)
            {
                return;
            }

            foreach (var item in viewModel.SourceConnection.GetEntitesList())
            {
                SimpleItem entityitem = new SimpleItem();
                entityitem.DisplayField = item;
                entityitem.Text = item;
                entityitem.Name = item;
                entityitem.Value = item;
                EntitiesbeepComboBox.ListItems.Add(entityitem);
            }

            if (!string.IsNullOrWhiteSpace(viewModel.EntityName))
            {
                SelectEntity(viewModel.EntityName);
            }
        }

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            if (viewModel == null || beepService == null)
            {
                return;
            }
            if (parameters.TryGetValue("Datasource", out var datasourceObj))
            {
                string datasource = datasourceObj?.ToString() ?? string.Empty;
                viewModel.Datasourcename = datasource;
                viewModel.SourceConnection = beepService.DMEEditor.GetDataSource(datasource);
                viewModel.EntityName = string.Empty;
                if (viewModel.SourceConnection != null)
                {
                    viewModel.UpdateFieldTypes();
                    ConfigureFieldTypeColumn();
                }
                LoadEntitiesList();
            }
            if (parameters.TryGetValue("EntityName", out var entityNameObj))
            {
                string entityname = entityNameObj?.ToString() ?? string.Empty;
                viewModel.EntityName = entityname;
                if(viewModel.SourceConnection == null)
                {
                    viewModel.SourceConnection = beepService.DMEEditor.GetDataSource(viewModel.Datasourcename);
                }
                viewModel.LoadOrCreateEntityStructure(entityname);
                viewModel.IsNew = false;
                viewModel.IsChanged = false;
                
            }
            else
            {
                viewModel.IsNew = true;
            }
            //HeaderbeepPanel.TitleText = viewModel.EntityName?? "Entity Editor";
          //  beepSimpleGrid1.TitleText =  "Field Structure";
            SyncBindings();
        }

        private void ApplybeepButton_Click(object? sender, EventArgs e)
        {
            if (viewModel == null || beepService == null)
            {
                return;
            }

            if (viewModel.SourceConnection == null)
            {
                beepService.DMEEditor.AddLogMessage("Beep", "Select a datasource first", DateTime.Now, 0, null, Errors.Failed);
                return;
            }

            string entityName = GetEntityNameFromUi();
            if (string.IsNullOrWhiteSpace(entityName))
            {
                beepService.DMEEditor.AddLogMessage("Beep", "Select or type an entity name", DateTime.Now, 0, null, Errors.Failed);
                return;
            }

            if (viewModel.Structure == null || !string.Equals(viewModel.EntityName, entityName, StringComparison.OrdinalIgnoreCase))
            {
                if (!LoadOrCreateEntity(entityName))
                {
                    return;
                }
            }

            fieldsBindingSource.EndEdit();
            if (BindingContext[fieldsBindingSource] is CurrencyManager cm)
            {
                cm.EndCurrentEdit();
            }

            viewModel.IsChanged = true;
            viewModel.SaveEntity();

            SyncBindings();
            LoadEntitiesList();
        }

        private bool LoadOrCreateEntity(string? entityName)
        {
            if (viewModel == null || string.IsNullOrWhiteSpace(entityName))
            {
                return false;
            }

            viewModel.LoadOrCreateEntityStructure(entityName.Trim());
            SyncBindings();
            return true;
        }

        private string? GetEntityNameFromUi()
        {
            if (EntitiesbeepComboBox.SelectedItem is SimpleItem selected && !string.IsNullOrWhiteSpace(selected.Text))
            {
                return selected.Text.Trim();
            }

            if (!string.IsNullOrWhiteSpace(EntitiesbeepComboBox.Text))
            {
                return EntitiesbeepComboBox.Text.Trim();
            }

            return viewModel?.EntityName;
        }

        private void SelectEntity(string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName) || EntitiesbeepComboBox.ListItems == null)
            {
                return;
            }

            var existing = EntitiesbeepComboBox.ListItems.FirstOrDefault(i =>
                string.Equals(i.Text, entityName, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                EntitiesbeepComboBox.SelectedItem = existing;
            }

            EntitiesbeepComboBox.Text = entityName;
        }

        private void SyncBindings()
        {
            if (viewModel == null)
            {
                return;
            }

            viewModel.Fields = viewModel.DBWork?.Units;
            entityManagerViewModelBindingSource.DataSource = viewModel;
            entityManagerViewModelBindingSource.ResetBindings(false);
            fieldsBindingSource.ResetBindings(false);

            // BeepGridPro binding modes are mutually exclusive:
            // if Uow is set, it becomes the authoritative source and DataSource is ignored.
            if (viewModel.DBWork != null)
            {
                EntityFieldsbeepGridPro.DataSource = null;
                EntityFieldsbeepGridPro.Uow = viewModel.DBWork;
            }
            else
            {
                EntityFieldsbeepGridPro.Uow = null;
                EntityFieldsbeepGridPro.DataSource = viewModel.Fields;
            }
            ConfigureEditorsFromEntityFieldProperties();
            ConfigureFieldTypeColumn();
            ApplybeepButton.Text = viewModel.IsNew ? "Create" : "Update";
        }

        private void ConfigureEditorsFromEntityFieldProperties()
        {
            if (EntityFieldsbeepGridPro?.Columns == null)
            {
                return;
            }

            foreach (var column in EntityFieldsbeepGridPro.Columns)
            {
                if (column == null || string.IsNullOrWhiteSpace(column.ColumnName))
                {
                    continue;
                }

                if (string.Equals(column.ColumnName, "Fieldtype", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var propertyType = ResolveEntityFieldPropertyType(column);
                if (propertyType == null)
                {
                    continue;
                }

                var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                if (type == typeof(bool))
                {
                    column.CellEditor = BeepColumnType.CheckBoxBool;
                }
                else if (type == typeof(char))
                {
                    column.CellEditor = BeepColumnType.CheckBoxChar;
                }
            }
        }

        private static Type? ResolveEntityFieldPropertyType(BeepColumnConfig column)
        {
            if (!string.IsNullOrWhiteSpace(column.PropertyTypeName))
            {
                var resolved = Type.GetType(column.PropertyTypeName, throwOnError: false);
                if (resolved != null)
                {
                    return resolved;
                }
            }

            var prop = typeof(EntityField).GetProperty(
                column.ColumnName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            return prop?.PropertyType;
        }

        private void ConfigureFieldTypeColumn()
        {
            if (EntityFieldsbeepGridPro?.Columns == null || viewModel == null)
            {
                return;
            }

            var fieldTypeColumn = EntityFieldsbeepGridPro.Columns.FirstOrDefault(c =>
                string.Equals(c.ColumnName, "Fieldtype", StringComparison.OrdinalIgnoreCase));

            if (fieldTypeColumn == null)
            {
                return;
            }

            var typeItems = (viewModel.DatatypeMappings ?? Enumerable.Empty<DatatypeMapping>())
                .Where(m => !string.IsNullOrWhiteSpace(m.NetDataType))
                .GroupBy(m => m.NetDataType, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.OrderByDescending(m => m.Fav).First())
                .OrderByDescending(m => m.Fav)
                .ThenBy(m => m.NetDataType)
                .Select(m => new SimpleItem
                {
                    DisplayField = m.NetDataType,
                    Text = m.NetDataType,
                    Name = m.NetDataType,
                    Value = m.NetDataType
                })
                .ToList();

            fieldTypeColumn.CellEditor = BeepColumnType.ComboBox;
            fieldTypeColumn.Items = typeItems;
            fieldTypeColumn.EnumSourceType = null;
            fieldTypeColumn.QueryToGetValues = null;
        }
       
    }
}
