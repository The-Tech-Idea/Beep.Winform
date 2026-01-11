using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    /// <summary>
    /// Tab control for Driver region of IConnectionProperties
    /// Properties: DriverName, DriverVersion, Parameters (displayed from ParameterList)
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
            
            // Parameters textbox displays a formatted summary of ParameterList
            // Format: key1=value1;key2=value2;...
            UpdateParametersDisplay();
            
            // Handle text changed to update ParameterList
            Driver_ParametersbeepTextBox.TextChanged -= Driver_ParametersbeepTextBox_TextChanged;
            Driver_ParametersbeepTextBox.TextChanged += Driver_ParametersbeepTextBox_TextChanged;
        }
        
        /// <summary>
        /// Update the Parameters textbox to display a formatted summary of ParameterList
        /// </summary>
        private void UpdateParametersDisplay()
        {
            if (ConnectionProperties == null || ConnectionProperties.ParameterList == null)
            {
                Driver_ParametersbeepTextBox.Text = string.Empty;
                return;
            }
            
            // Format ParameterList as key=value pairs separated by semicolons
            var formattedParams = string.Join("; ", 
                ConnectionProperties.ParameterList
                    .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
                    .Select(kvp => $"{kvp.Key}={kvp.Value}"));
            
            Driver_ParametersbeepTextBox.Text = formattedParams;
        }
        
        /// <summary>
        /// Handle text changed event to parse and update ParameterList
        /// </summary>
        private void Driver_ParametersbeepTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ConnectionProperties == null || ConnectionProperties.ParameterList == null) return;
            
            try
            {
                // Parse the formatted string back into ParameterList
                // Format: key1=value1;key2=value2;...
                var text = Driver_ParametersbeepTextBox.Text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    ConnectionProperties.ParameterList.Clear();
                    return;
                }
                
                // Split by semicolon and parse each key=value pair
                var pairs = text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var tempParams = new Dictionary<string, string>();
                
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        if (!string.IsNullOrEmpty(key))
                        {
                            tempParams[key] = value;
                        }
                    }
                }
                
                // Update ParameterList with parsed values
                ConnectionProperties.ParameterList.Clear();
                foreach (var kvp in tempParams)
                {
                    ConnectionProperties.ParameterList[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                // On parse error, revert to last valid display
                System.Diagnostics.Debug.WriteLine($"Error parsing Parameters: {ex.Message}");
                UpdateParametersDisplay();
            }
        }
    }
}

