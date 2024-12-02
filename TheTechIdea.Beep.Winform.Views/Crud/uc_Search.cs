//using DataManagementModels.Editor;
using System.Data;
using System.Windows.Forms.VisualStyles;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Winform.Views.Crud
{
    [AddinAttribute(Caption = "Filter", Name = "uc_Search", misc = "VIEW", addinType = AddinType.Control, ObjectType = "Beep")]

    public partial class uc_Search : uc_Addin
    {
        public uc_Search()
        {
            InitializeComponent();
        }
        CreateCrudViewViewModel CreateCrudView;
        EntityControlViewModel EntityControlView;
        Type EntityType;
        IDataSource dataSource;
        public List<AppFilter> AppFilters { get; set; }
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


            EntityControlView.CrudFilterPanel=splitContainer1.Panel1;
            this.Submitbutton.Click += new System.EventHandler(this.Submitbutton_Click);
            this.Cancelbutton.Click += Cancelbutton_Click;

            FillPanel();


        }
        public void FillPanel()
        {
            try
            {

                EntityControlView.CrudFilterPanel = splitContainer1.Panel1;
                EntityControlView.CreateFilterFields(EntityStructure);
                this.Width = EntityControlView.CrudFilterPanel.Width+15;


            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Failed to create controls {ex.Message}", DateTime.Now, 0,null, Errors.Failed);
            }

        }
        private void Cancelbutton_Click(object? sender, EventArgs e)
        {
           
        }

        private void Submitbutton_Click(object? sender, EventArgs e)
        {
            
        }
    }
}
