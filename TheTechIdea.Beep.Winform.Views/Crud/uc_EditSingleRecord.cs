using DataManagementModels.Editor;
using System.Data;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Logger;
using TheTechIdea.Util;


namespace TheTechIdea.Beep.Winform.Views.Crud
{
    [AddinAttribute(Caption = "Show/Edit Data", Name = "uc_EditSingleRecord", misc = "VIEW", addinType = AddinType.Control, ObjectType = "Beep")]

    public partial class uc_EditSingleRecord : uc_Addin
    {
        public uc_EditSingleRecord()
        {
            InitializeComponent();
        }
        CreateCrudViewViewModel CreateCrudView;
        EntityControlViewModel EntityControlView;
        Type EntityType;
        IDataSource dataSource;
        public BindingSource bindingSource { get; set; }
        bool IsPrimarykeyMissing = false;
        public override void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
            base.SetConfig(pDMEEditor, plogger, putil, args, e, per);
            CreateCrudView = new CreateCrudViewViewModel(pDMEEditor, Visutil);
            EntityControlView = new EntityControlViewModel(pDMEEditor, Visutil);
            EntityControlView.Container = splitContainer1.Panel1;
            AddinName = e.CurrentEntity;

            if (CreateCrudView.Init())
            {
                EntityControlView.DataSource = dataSource;
                EntityControlView.Structure = CreateCrudView.Structure;
                EntityControlView.Entityname = CreateCrudView.Entityname;
                EntityControlView.EntityType = EntityType;

                try
                {
                    if (CreateCrudView.Structure == null)
                    {
                        DMEEditor.AddLogMessage("Fail", $"Could not find entity {e.CurrentEntity}", DateTime.Now, 0, e.DatasourceName, Errors.Failed);
                        return;
                    }
                    CreateCrudView.GetData();
                    EntityControlView.CrudFilterPanel = splitContainer1.Panel1;
                    //   this.ResumeLayout();
                }
                catch (Exception ex)
                {
                    DMEEditor.AddLogMessage("Fail", $"Failed to create controls {ex.Message}", DateTime.Now, 0, e.DatasourceName, Errors.Failed);
                }

            }

            try
            {
                EntityControlView.CreateControlsForEntity(bindingSource);
                
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Failed to create controls {ex.Message}", DateTime.Now, 0, e.DatasourceName, Errors.Failed);
            }


        }

    }
}
