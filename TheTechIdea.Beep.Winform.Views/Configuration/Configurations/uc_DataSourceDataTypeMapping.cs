
using TheTechIdea.Beep.Vis;
using TheTechIdea.Util;
using System.Data;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Logger;

using TheTechIdea.Beep.DataBase;
using DataManagementModels.DriversConfigurations;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Addin;
using DataManagementModels.Editor;
using TheTechIdea.Beep.Winform.Controls.Tree;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace Beep.Config.Winform.Configurations
{
    [AddinAttribute(Caption = "Field Mapping", Name = "uc_DBFieldType", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 2, RootNodeName = "Configuration", Order = 2, ID = 2, BranchText = "Field Types", BranchType = EnumPointType.Function, IconImageName = "fieldtypeconfig.png", BranchClass = "ADDIN", BranchDescription = "Data Sources Connection Drivers Setup Screen")]

    public partial class uc_DataSourceDataTypeMapping :uc_Addin, IAddinVisSchema
    {
        public uc_DataSourceDataTypeMapping()
        {
            InitializeComponent();
            AddinName  = "DataSource DataType Mapping Manager";
        }
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "DDL";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 2;
        public int ID { get; set; } = 2;
        public string BranchText { get; set; } = "Field Types";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 2;
        public string IconImageName { get; set; } = "fieldtype.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "ADDIN";
        #endregion "IAddinVisSchema"
      
    
      
        public UnitofWork<DatatypeMapping> DBWork { get; set; }
        public DatatypeMapping cn { get; set; }
        public string GuidID { get; set; }
        public int id { get; set; }
        public bool IsReady { get; set; }
        public bool IsRunning { get; set; }
        public bool IsNew { get; set; }
        string selectedCategoryValue;
        TreeControl tree;
        IBranch branch;
        DataTypeMappingViewModel viewModel;
       

        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs obj, IErrorsInfo per)
        {
           
            base.SetConfig(pDMEEditor,plogger,putil,args,obj,per);
            viewModel = new DataTypeMappingViewModel(DMEEditor, Visutil);

            // Create a new DataGridViewImageColumn
            DataGridViewImageColumn editButtonColumn = new DataGridViewImageColumn();
            editButtonColumn.Name = "EditButton";
            editButtonColumn.HeaderText = "Edit";
          //  editButtonColumn.Image = Properties.Resources._221_database_6; // Add your image from resources here
            editButtonColumn.ImageLayout = DataGridViewImageCellLayout.Normal;

            int columnIndex = 0;  // Choose the index where you want the button column to be.
            if (poisonDataGridView1.Columns["EditButton"] == null)  // Ensure this column is not already added.
            {
                poisonDataGridView1.Columns.Insert(columnIndex, editButtonColumn);
           
            }
            if (DMEEditor.ConfigEditor.DataTypesMap!=null && DMEEditor.ConfigEditor.DataTypesMap.Count==0)
            {
                viewModel.Read();
            }
           
            this.dataSourceNameDataGridViewTextBoxColumn.DataSource = viewModel.DataClasses;
            this.netDataTypeDataGridViewTextBoxColumn.DataSource = viewModel.DataTypes;


             dataTypesMapBindingSource.DataSource = viewModel.DBWork.Units;//DBWork.Units;

            this.dataTypesMapBindingSource.AddingNew += DataTypesMapBindingSource_AddingNew;
            //poisonDataGridView1.RowValidated += PoisonDataGridView1_RowValidated;
            this.poisonDataGridView1.DataError += MappingDataGridView_DataError;
            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, DMEEditor.Passedarguments, DMEEditor.ErrorObject);
            BeepbindingNavigator1.HightlightColor = Color.Yellow;
            BeepbindingNavigator1.bindingSource = dataTypesMapBindingSource;
            poisonDataGridView1.DataSource = dataTypesMapBindingSource;
            this.BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled;
        }

        private void BeepbindingNavigator1_SaveCalled(object? sender, BindingSource e)
        {
            Save();
        }

        private void MappingDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }


        private void DataTypesMapBindingSource_AddingNew(object? sender, System.ComponentModel.AddingNewEventArgs e)
        {
            
            DatatypeMapping mp = new DatatypeMapping();
           
            GuidID = mp.GuidID;
            id = mp.ID;
            e.NewObject = mp;
            //DBWork.Create(cn);
        }

        //private void PoisonDataGridView1_RowValidated(object? sender, DataGridViewCellEventArgs e)
        //{
        //    // Check if this is a new row
        //    if (poisonDataGridView1.Rows[e.RowIndex].IsNewRow)
        //    {
        //        // Get the data from the new row
        //        var newRowData = (DatatypeMapping)poisonDataGridView1.Rows[e.RowIndex].DataBoundItem;

        //        // Add the data to your list (replace 'yourList' with the name of your list)
        //        DatatypeMapping mp = new DatatypeMapping();
        //        GuidID = mp.GuidID;
        //        id = cn.ID;
        //        DBWork.Create(cn);
        //    }
        //}

        private void ExitCancelpoisonButton_Click(object? sender, EventArgs e)
        {
            Form f = (Form)this.Parent;
            if (f != null)
            {
                f.Close();
            }

        }
        private void setfilterdData()
        {
            List<AppFilter> filters = new List<AppFilter>();
            filters.Add(new AppFilter() { FieldName = "Category", FilterValue = selectedCategoryValue.ToString(), Operator = "=" });
            var retval = DBWork.Get(filters);
            retval.Wait();
            poisonDataGridView1.DataSource = retval.Result;
            poisonDataGridView1.Refresh();
        }
        private void Save()
        {
            DMEEditor.ConfigEditor.DataTypesMap = DBWork.Units.ToList();
            DMEEditor.ConfigEditor.WriteDataTypeFile();
            MessageBox.Show("Changes Saved", "Beep");
            Form f = (Form)this.Parent;
            if (f != null)
            {
                f.Close();
            }
        }
    }
}
