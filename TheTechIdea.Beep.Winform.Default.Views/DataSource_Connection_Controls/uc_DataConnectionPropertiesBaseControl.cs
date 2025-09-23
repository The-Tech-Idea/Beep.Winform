using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Printing.Interop;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_DataConnectionPropertiesBaseControl : TemplateUserControl
    {
        // Main data object - ConnectionProperties that will be passed in and returned
        protected ConnectionProperties _connectionProperties;
        // Additional parameters if needed
        // for specific connection types not covered in ConnectionProperties
        /// <summary>
        /// Extra parameters for specific connection types
        /// These will be the default and properties need to be set in the dialog before showing
        /// and added to the ConnectionProperties.ParameterList on save
        /// </summary>
        public Dictionary<string, string> DefaultParameterList { get; set; } = new Dictionary<string, string>();
        public ConnectionProperties ConnectionProperties
        {
            get => _connectionProperties;
            set
            {
                _connectionProperties = value;
                if (_connectionProperties != null)
                {
                    SetupBindings(_connectionProperties);
                }
            }
        }

        public virtual void SetupBindings(ConnectionProperties conn)
        {
            ConnectionProperties = conn;
            // Clear existing bindings
            // Set BeepTabs Tab Title

            // Bind controls to ConnectionProperties properties

        }

        public uc_DataConnectionPropertiesBaseControl()
        {
            InitializeComponent();
        }
    }
}
