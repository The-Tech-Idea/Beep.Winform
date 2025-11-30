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
    /// <summary>
    /// Base class for connection property tab controls.
    /// Each child control represents one tab/region of IConnectionProperties.
    /// </summary>
    public partial class uc_DataConnectionPropertiesBaseControl : UserControl
    {
        // Main data object - ConnectionProperties that will be passed in and returned
        protected ConnectionProperties _connectionProperties;
        
        /// <summary>
        /// Extra parameters for specific connection types
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
            // Override in child classes to bind controls to ConnectionProperties
        }

        public uc_DataConnectionPropertiesBaseControl()
        {
            InitializeComponent();
        }

        protected virtual void InitializeComponent()
        {
            SuspendLayout();
            // 
            // uc_DataConnectionPropertiesBaseControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "uc_DataConnectionPropertiesBaseControl";
            Size = new Size(726, 694);
            ResumeLayout(false);
        }
    }
}
