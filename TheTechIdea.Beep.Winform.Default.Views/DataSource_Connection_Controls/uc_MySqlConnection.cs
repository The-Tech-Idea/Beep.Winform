using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    [AddinAttribute(Caption = "MySQL Connection", ScopeCreateType = AddinScopeCreateType.Multiple, DatasourceType = DataSourceType.Mysql, Category = DatasourceCategory.RDBMS, Name = "uc_MySqlConnection", misc = "Config", menu = "Configuration", addinType = AddinType.ConnectionProperties, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class uc_MySqlConnection : uc_DataConnectionBase
    {
        public uc_MySqlConnection()
        {
            InitializeComponent();
        }
        public uc_MySqlConnection(IServiceProvider services) : base(services)
        {
            InitializeComponent();
            Details.AddinName = "MySQL Connection";
        }
        public override void OnNavigatedTo(System.Collections.Generic.Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
