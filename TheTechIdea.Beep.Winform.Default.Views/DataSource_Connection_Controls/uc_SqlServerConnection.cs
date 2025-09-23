using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    [AddinAttribute(Caption = "SQL Server Connection", ScopeCreateType = AddinScopeCreateType.Multiple, DatasourceType = DataSourceType.SqlServer, Category = DatasourceCategory.RDBMS, Name = "uc_SqlServerConnection", misc = "Config", menu = "Configuration", addinType = AddinType.ConnectionProperties, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class uc_SqlServerConnection : uc_DataConnectionBase
    {
        public uc_SqlServerConnection()
        {
            InitializeComponent();
        }
        public uc_SqlServerConnection(IServiceProvider services) : base(services)
        {
            InitializeComponent();
            Details.AddinName = "SQL Server Connection";
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
