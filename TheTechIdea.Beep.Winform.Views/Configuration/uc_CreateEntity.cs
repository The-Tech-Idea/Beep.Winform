using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.DriversConfigurations;

namespace TheTechIdea.Beep.Winform.Views.Configuration
{
    [AddinAttribute(Caption = "Entity Creator", Name = "uc_CreateEntity", misc = "Config", menu = "null", addinType = AddinType.Control, displayType = DisplayType.Popup, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 2, RootNodeName = "DDL", Order = 2, ID = 2, BranchText = "Entity Creator Drivers", BranchType = EnumPointType.Function, IconImageName = "createentity.ico", BranchClass = "DDL", BranchDescription = "Data Sources Connection Drivers Setup Screen")]
    public partial class uc_CreateEntity : uc_Addin, IAddinVisSchema
    {
        public uc_CreateEntity()
        {
            InitializeComponent();
            AddinName  = "Entity Creator";
        }




   
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "DDL";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 2;
        public int ID { get; set; } = 2;
        public string BranchText { get; set; } = "Entity Creator";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 2;
        public string IconImageName { get; set; } = "createentity.ico";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "";
        public string BranchClass { get; set; } = "DDL";
        #endregion "IAddinVisSchema"
        private EntityStructure tb = new EntityStructure();
        EntityManagerViewModel viewModel;
        // public event EventHandler<PassedArgs> OnObjectSelected;
        string datasourcename = null;
      

        public override void SetConfig(IDMEEditor pbl, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pbl,plogger,putil,args,e,per);
            viewModel = new EntityManagerViewModel(pbl, Visutil);
            mappingBindingSource.DataSource = DMEEditor.ConfigEditor.DataTypesMap.Distinct();
            List<EntityStructure> entityStructures = new List<EntityStructure>();
            
            entitiesBindingSource.DataSource = entityStructures;
           
            entitiesBindingSource.AddNew();
            fieldsBindingSource.DataSource = entitiesBindingSource;
            fieldsBindingSource.DataMember = "Fields";
            fieldtypeDataGridViewTextBoxColumn.DataSource = mappingBindingSource;
            fieldsDataGridView.DataSource = fieldsBindingSource;
           
            this.CreateinDBbutton.Click += CreateinDBbutton_Click1;
            this.fieldsDataGridView.DataError += FieldsDataGridView_DataError;
          
            this.fieldsDataGridView.RowValidating += FieldsDataGridView_RowValidating;
            this.fieldsDataGridView.CellEndEdit += FieldsDataGridView_CellEndEdit;

            BeepbindingNavigator1.SetConfig(DMEEditor, DMEEditor.Logger, DMEEditor.Utilfunction, new string[] { }, e, DMEEditor.ErrorObject);
            BeepbindingNavigator1.bindingSource = fieldsBindingSource;
            BeepbindingNavigator1.SaveCalled += BeepbindingNavigator1_SaveCalled1;
            BeepbindingNavigator1.HightlightColor = Color.Yellow;
            if (!string.IsNullOrEmpty(e.DatasourceName))
            {
                datasourcename = e.DatasourceName;
                this.DataSourceName.Text = datasourcename;
                UpdateFieldTypes();
            }
            else
                return;

        }

        private void BeepbindingNavigator1_SaveCalled1(object sender, BindingSource e)
        {
            save();
        }

        private void FieldsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (string.IsNullOrEmpty(datasourcename))
            {
                return;
            }
            DataGridViewRow row = fieldsDataGridView.Rows[e.RowIndex];
            DataGridViewCell fieldtype = row.Cells[2];
            DataGridViewCell size1 = row.Cells[3];
            DataGridViewCell nperc = row.Cells[4];
            DataGridViewCell nscale = row.Cells[5];
            if (e.ColumnIndex == 2)
            {
                if (fieldtype.Value.ToString().Contains("N"))
                {
                    size1.ReadOnly = false;
                    nperc.ReadOnly = true;
                    nscale.ReadOnly = true;
                }
                if (fieldtype.Value.ToString().Contains("P,S"))
                {
                    size1.ReadOnly = true;
                    nperc.ReadOnly = false;
                    nscale.ReadOnly = false;
                }
            }
        }

        private void FieldsDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (string.IsNullOrEmpty(datasourcename))
            {
                return;
            }
            DataGridViewRow row = fieldsDataGridView.Rows[e.RowIndex];
            DataGridViewCell id = row.Cells[0];
            DataGridViewCell fieldname = row.Cells[1];
            DataGridViewCell fieldtype = row.Cells[2];
            DataGridViewCell size1 = row.Cells[3];
            DataGridViewCell nperc = row.Cells[4];

            DataGridViewCell nscale = row.Cells[5];
            DataGridViewCell Autoinc = row.Cells[6];
            DataGridViewCell isdbnull = row.Cells[7];
            DataGridViewCell ischeck = row.Cells[8];
            DataGridViewCell isunique = row.Cells[9];
            DataGridViewCell iskey = row.Cells[10];

            //   e.Cancel = !(IsDoc(Docnamecell) && IsGender(Gendercell) && IsAddress(Addresscell) && IsContactno(Contactnocell) && IsDate(Datecell));
        }
        private void UpdateFieldTypes()
        {
            if (!string.IsNullOrEmpty(datasourcename) && string.IsNullOrEmpty(this.EntityName))
            {
                ConnectionProperties connection = DMEEditor.ConfigEditor.DataConnections.Where(o => o.ConnectionName.Equals(datasourcename, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                ConnectionDriversConfig conf = DMEEditor.Utilfunction.LinkConnection2Drivers(connection);
                if (conf != null)
                {
                    mappingBindingSource.DataSource = DMEEditor.ConfigEditor.DataTypesMap.Where(p => p.DataSourceName.Equals(conf.classHandler, StringComparison.InvariantCultureIgnoreCase) ).Distinct();
                }
            }
        }
       

        //private void FieldsBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(datasourcename) && string.IsNullOrEmpty(this.EntityName))
        //    {
        //        return;
        //    }
          

        //}

        //private void EntitiesBindingSource_AddingNew(object sender, AddingNewEventArgs e)
        //{
        //    //EntityStructure entityStructure = new EntityStructure();
        //    //entityStructure.Drawn = false;
        //    //entityStructure.Editable = true;
        //    //e.NewObject = entityStructure;
        //}

        private void save()
        {
            try
            {
                fieldsBindingSource.EndEdit();
                entitiesBindingSource.EndEdit();
                viewModel.SaveEntity();
                MessageBox.Show("Table Creation is Saved", "Beep");
            }
            catch (Exception ex)
            {

                DMEEditor.AddLogMessage("Fail", $"Could not Save Entity Creation Script {ex.Message}", DateTime.Now, -1, "", Errors.Failed);
            }

        }
        private void CreateinDBbutton_Click1(object sender, EventArgs e)
        {

            try
            {
                viewModel.ApplyChanges();

            }
            catch (Exception ex)
            {
                string mes = "Entity Creation Failed";
                MessageBox.Show(mes, "Beep");
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };

        }


        private void FieldsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void SaveTableConfigbutton_Click(object sender, EventArgs e)
        {
            save();
        }

        private void NewTablebutton_Click(object sender, EventArgs e)
        {
            entitiesBindingSource.AddNew();
        }
    }
}
