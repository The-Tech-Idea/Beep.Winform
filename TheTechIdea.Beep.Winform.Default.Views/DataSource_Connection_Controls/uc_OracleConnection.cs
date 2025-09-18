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
    [AddinAttribute(Caption = "Oracle Connection", ScopeCreateType = AddinScopeCreateType.Multiple, DatasourceType = DataSourceType.Oracle,Category = DatasourceCategory.RDBMS , Name = "uc_OracleConnection", misc = "Config", menu = "Configuration", addinType = AddinType.ConnectionProperties, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class uc_OracleConnection : uc_DataConnectionBase
    {
        public uc_OracleConnection(IServiceProvider services) : base(services)
        {
            InitializeComponent();
            Details.AddinName = "Oracle Connection";
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            
        }

    }
}
