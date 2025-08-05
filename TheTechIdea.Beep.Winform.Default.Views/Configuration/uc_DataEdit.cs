
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;


namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Data Edit", Name = "uc_DataEdit", ScopeCreateType = AddinScopeCreateType.Multiple, misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]

    public partial class uc_DataEdit : TemplateUserControl
    {
        public uc_DataEdit(IServiceProvider services): base(services)
        {
            InitializeComponent();
            
            Details.AddinName = "Data Edit";

        }
       
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);

        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.Count > 0)
            {
                string? name=parameters.First(parameters => parameters.Key == "CurrentEntity").Value.ToString();
                if (!string.IsNullOrEmpty(name))
                {
                    Details.AddinName = name;
                }
            }
            if (uow != null)
            {
                uow.Get();
                beepSimpleGrid1.DataSource = uow.Units;
            }


        }
    }
}
