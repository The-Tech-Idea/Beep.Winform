using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    [AddinAttribute(Caption = "PostgreSQL Connection", ScopeCreateType = AddinScopeCreateType.Multiple, DatasourceType = DataSourceType.Postgre, Category = DatasourceCategory.RDBMS, Name = "uc_PostgreConnection", misc = "Config", menu = "Configuration", addinType = AddinType.ConnectionProperties, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class uc_PostgreConnection : uc_DataConnectionBase
    {
        public uc_PostgreConnection()
        {
            InitializeComponent();
        }
        public uc_PostgreConnection(IServiceProvider services) : base(services)
        {
            InitializeComponent();
            Details.AddinName = "PostgreSQL Connection";
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
