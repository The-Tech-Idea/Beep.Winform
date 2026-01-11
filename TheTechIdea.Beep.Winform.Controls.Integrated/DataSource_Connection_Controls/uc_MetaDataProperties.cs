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
    public partial class uc_MetaDataProperties : uc_DataConnectionPropertiesBaseControl
    {
        // Main data object - ConnectionProperties that will be passed in and returned from ParameterList
        public uc_MetaDataProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            Text = "Meta Data";
            // Bind controls to ConnectionProperties properties
            // Fill Values from ParameterList if exists
        }
    }
}
