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
    public partial class uc_TypeandStateFlagsProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_TypeandStateFlagsProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            ConnectionPropertytabPage.Text = "Type and State";
            // Bind controls to ConnectionProperties properties for Type and State.
            // 
        }
    }
}
