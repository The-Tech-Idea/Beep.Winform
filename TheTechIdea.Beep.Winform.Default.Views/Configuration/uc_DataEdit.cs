
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;


namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Data Edit", Name = "uc_DataEdit", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]

    public partial class uc_DataEdit : TemplateUserControl
    {
        public uc_DataEdit(IBeepService service) : base(service)
        {
            InitializeComponent();
            
            Details.AddinName = "Data Edit";

        }
        IDataSource ds;
        string DataSourceName;
        string EntityName;
        UnitOfWorkWrapper uow;
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
       
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters != null)
            {
                if (parameters.ContainsKey("DatasourceName"))
                {
                    DataSourceName = (string)parameters["DatasourceName"];
                    if (DataSourceName != null)
                    {
                        if(parameters.ContainsKey("CurrentEntity"))
                        {
                            EntityName = (string)parameters["CurrentEntity"];
                            if (EntityName != null)
                            {
                               Details.ObjectName  = EntityName;
                            }
                        }
                    }
                }
            }
            if(DataSourceName != null)
            {
                ds = beepService.DMEEditor.GetDataSource(DataSourceName);
                if (ds != null)
                {
                   if(EntityName != null)
                    {
                        EntityStructure entityStructure = ds.GetEntityStructure(EntityName,true);
                        if (entityStructure != null)
                        {
                           Type type= ds.GetEntityType(EntityName);
                            if(type != null)
                            {
                                var u = UnitOfWorkFactory.CreateUnitOfWork(type,beepService.DMEEditor,DataSourceName,EntityName);
                                uow=new UnitOfWorkWrapper(u);
                                if (uow != null)
                                {
                                    uow.Get();
                                    beepSimpleGrid1.DataSource = uow.Units;
                                }
                            }
                           
                        }
                       
                    }
                }
            }
        }
    

    }
}
