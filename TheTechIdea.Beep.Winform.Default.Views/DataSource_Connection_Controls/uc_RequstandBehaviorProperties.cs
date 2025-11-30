using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_RequstandBehaviorProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_RequstandBehaviorProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            Text = "Request and Behavior";
            // Bind controls to ConnectionProperties properties for Providers.
            // 
        }
    }
}
