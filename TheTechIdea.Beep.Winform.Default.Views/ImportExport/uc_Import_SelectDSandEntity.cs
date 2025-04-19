using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    [AddinAttribute(Caption = "Select Datasource and Entity", Name = "uc_Import_SelectDSandEntity", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_Import_SelectDSandEntity : TemplateUserControl
    {
        public uc_Import_SelectDSandEntity(IBeepService service):base(service)
        {
            InitializeComponent();
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);



        }
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
        }
    }
}
