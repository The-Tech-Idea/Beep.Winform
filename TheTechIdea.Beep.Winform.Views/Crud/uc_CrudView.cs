//using DataManagementModels.Editor;
using System.Data;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;
using TheTechIdea.Beep.Winform.Controls.Template;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.Composite;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Winform.Views.Crud
{
    [AddinAttribute(Caption = "Show/Edit Table Data", Name = "uc_crudView", misc = "VIEW", addinType = AddinType.Control, ObjectType = "Beep" )]
    public partial class uc_crudView : uc_Addin
    {
        public uc_crudView()
        {
            InitializeComponent();
        }

        CreateCrudViewViewModel CreateCrudView;
        EntityControlViewModel EntityControlView;
        Type EntityType;
        IDataSource dataSource;
      //  BindingSource bindingSource;
        bool IsPrimarykeyMissing = false;
        uc_Search uc_Search;
        frm_Addin frm_Addin;
        List<AppFilter> AppFilters=new List<AppFilter>  ();
        
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            Passedarg = e;
            uc_Search = new uc_Search();
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);
          
            CreateCrudView =new CreateCrudViewViewModel(pDMEEditor,Visutil);
            EntityControlView=new EntityControlViewModel(pDMEEditor, Visutil);
            AddinName = e.CurrentEntity;
           // bindingSource = new BindingSource();
            CreateCrudView.Entityname = e.CurrentEntity;
            CreateCrudView.DatasourceName = e.DatasourceName;
            CreateCrudView.DataSource=DMEEditor.GetDataSource(e.DatasourceName);
            dataSource = CreateCrudView.DataSource;
            if (dataSource.Category== DatasourceCategory.FILE)
            {

               CreateCrudView.IsCrudSupported = false;
            }
            else
            {
                CreateCrudView.IsCrudSupported = true;
            }
          
            if (CreateCrudView.Init())
            {
                dataSource=CreateCrudView.DataSource;
                EntityControlView.DataSource = dataSource;
                EntityControlView.Structure = CreateCrudView.Structure;
                EntityControlView.Entityname = CreateCrudView.Entityname;
                EntityControlView.EntityType = EntityType;
                this.label1.Text = e.CurrentEntity;
                if(CreateCrudView.IsCrudSupported)
                {
                    PrimaryKeycomboBox.DataSource = CreateCrudView.Structure.Fields;
                    PrimaryKeycomboBox.DisplayMember = "fieldname";
                    PrimaryKeycomboBox.ValueMember = "fieldname";
                    PrimaryKeycomboBox.SelectedValue = CreateCrudView.PrimaryKey;
                    PrimaryKeycomboBox.Enabled = true;
                    if (!CreateCrudView.IsPrimarykeyMissing)
                    {
                        PrimaryKeycomboBox.Enabled = false;
                    }
                }
                else
                {
                    PrimaryKeycomboBox.Enabled = false;
                }
         
              
                try
                {
                    if (CreateCrudView.Structure == null)
                    {
                        DMEEditor.AddLogMessage("Fail", $"Could not find entity {e.CurrentEntity}", DateTime.Now, 0, e.DatasourceName, Errors.Failed);
                        return;
                    }
                    CreateCrudView.GetData();
                    //this.beepGrid1.EntityStructure = CreateCrudView.Structure;
                    this.beepGrid1.SetConfig(pDMEEditor, plogger, putil, args, e, per);
                    this.beepGrid1.ResetData(CreateCrudView.Ts, CreateCrudView.Structure);
                    //this.beepGrid1.DataSource = CreateCrudView.Ts; // dataSource.GetEntity(e.CurrentEntity, null); ;
                    beepGrid1.BindingNavigator.SaveCalled += BeepbindingNavigator1_SaveCalled;
                    beepGrid1.BindingNavigator.ShowSearch += BeepbindingNavigator1_ShowSearch;
                    beepGrid1.BindingNavigator.NewRecordCreated += BeepbindingNavigator1_NewRecordCreated;
                    beepGrid1.BindingNavigator.EditCalled += BeepbindingNavigator1_EditCalled;
                    //   this.ResumeLayout();
                }
                catch (Exception ex)
                {
                    DMEEditor.AddLogMessage("Fail", $"Failed to create controls {ex.Message}", DateTime.Now, 0, e.DatasourceName, Errors.Failed);
                }

            }
            beepGrid1.Dock = DockStyle.Fill;
        }

        private void BeepbindingNavigator1_EditCalled(object? sender, BindingSource e)
        {
            
        }

        private void BeepbindingNavigator1_NewRecordCreated(object? sender, BindingSource e)
        {
            
        }
        private void SetPrimarKey()
        {
            PrimaryKeycomboBox.Enabled = true;
            dataSource.Entities[dataSource.GetEntityIdx(CreateCrudView.Entityname)].PrimaryKeys = new List<DataBase.EntityField>
                    {
                        new DataBase.EntityField(){fieldname=PrimaryKeycomboBox.SelectedText}
                    };
            CreateCrudView.Structure.PrimaryKeys = new List<DataBase.EntityField>
                {
                        new DataBase.EntityField(){fieldname=PrimaryKeycomboBox.SelectedText}
                    };
        }
        private void BeepbindingNavigator1_ShowSearch(object? sender, BindingSource e)
        {
          
           
            CreateFilter();
            if (AppFilters.Count > 0)
            {
                uc_Search.AppFilters = AppFilters;
            }
            if (frm_Addin.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Access the AppFilters property here after the form is closed with DialogResult.OK
                AppFilters = uc_Search.AppFilters;

                // Do something with filters...
            }
        }
        private void CreateFilter()
        {
             frm_Addin = new frm_Addin();
            frm_Addin.StartPosition = FormStartPosition.CenterParent;
            frm_Addin.FormBorderStyle= FormBorderStyle.None;
            frm_Addin.AddinName = $"Filter for Entity Data {CreateCrudView.Entityname}";
            frm_Addin.Text = $"Filter for Entity Data {CreateCrudView.Entityname}";
            frm_Addin.DMEEditor = DMEEditor;
            uc_Search.Location = new Point(0, 0);
            uc_Search = new uc_Search();
            uc_Search.SetConfig(DMEEditor, DMEEditor.Logger,DMEEditor.Utilfunction,new string[] { },Passedarg,DMEEditor.ErrorObject);
            frm_Addin.Controls.Add(uc_Search);
            frm_Addin.Width=uc_Search.Width;
            frm_Addin.Height=uc_Search.Height;
           
        }
        private void BeepbindingNavigator1_SaveCalled(object? sender, BindingSource e)
        {
            try
            {
                if (dataSource.Category == DatasourceCategory.FILE)
                {
                    MessageBox.Show("Cannot Save to a File", "Beep", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                beepGrid1.GridView.Invalidate();
                if(dataSource.Entities[dataSource.GetEntityIdx(CreateCrudView.Entityname)].PrimaryKeys.Count==0)
                {
                    IsPrimarykeyMissing = true;
                }
                if(IsPrimarykeyMissing)
                {
                    dataSource.Entities[dataSource.GetEntityIdx(CreateCrudView.Entityname)].PrimaryKeys = new List<DataBase.EntityField>
                    {
                        new DataBase.EntityField(){fieldname=PrimaryKeycomboBox.SelectedText}
                    };
                    CreateCrudView.Structure.PrimaryKeys = new List<DataBase.EntityField>
                {
                        new DataBase.EntityField(){fieldname=PrimaryKeycomboBox.SelectedText}
                    };
                }
                //Run(() =>
                //{
                if (CreateCrudView.Save())
                {
                    MessageBox.Show("Saved", "Beep", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(DMEEditor.ErrorObject.Message,"Beep",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                // dataSource.UpdateEntities(CreateCrudView.Entityname, dataTable, Progress);
                //});

            }
            catch (Exception ex)
            {
                
                DMEEditor.AddLogMessage("Beep",$"Failed Save {ex.Message}", DateTime.Now, 0, "", Errors.Failed);
                MessageBox.Show(DMEEditor.ErrorObject.Message, "Beep", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
           
        }
    }
}
