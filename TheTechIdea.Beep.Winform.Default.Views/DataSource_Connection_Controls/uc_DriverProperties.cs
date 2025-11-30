using System;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    /// <summary>
    /// Tab control for Driver region of IConnectionProperties
    /// Properties: DriverName, DriverVersion, Parameters
    /// </summary>
    public partial class uc_DriverProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_DriverProperties()
        {
            InitializeComponent();
        }

        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            Text = "Driver";
            if (conn == null) return;

            // Clear existing bindings
            Driver_DriverNamebeepTextBox.DataBindings.Clear();
            Driver_DriverVersionbeepTextBox.DataBindings.Clear();
            Driver_ParametersbeepTextBox.DataBindings.Clear();

            // Bindings for Driver region
            Driver_DriverNamebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.DriverName), true, DataSourceUpdateMode.OnPropertyChanged));
            Driver_DriverVersionbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.DriverVersion), true, DataSourceUpdateMode.OnPropertyChanged));
            Driver_ParametersbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Parameters), true, DataSourceUpdateMode.OnPropertyChanged));
        }
    }
}

