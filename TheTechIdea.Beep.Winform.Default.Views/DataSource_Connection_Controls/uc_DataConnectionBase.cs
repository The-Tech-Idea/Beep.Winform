using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.ConfigUtil; // ensure LINQ available

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_DataConnectionBase : TemplateUserControl
    {
        // Main data object - ConnectionProperties that will be passed in and returned
        protected ConnectionProperties _connectionProperties;

        public ConnectionProperties ConnectionProperties
        {
            get => _connectionProperties;
            set
            {
                _connectionProperties = value;
                if (_connectionProperties != null)
                {
                    SetupBindings();
                }
            }
        }

        public DialogResult DialogResult { get; private set; }

        // Properties for dialog behavior
        DataSourceType SourceType;
        DatasourceCategory Category;
        string DataSourceName;
        string ConnectionID;
        List<SimpleItem> versions = new List<SimpleItem>();
        List<SimpleItem> drivers = new List<SimpleItem>();
        List<ConnectionDriversConfig> connectionDriversConfigs = new List<ConnectionDriversConfig>();

        public uc_DataConnectionBase()
        {
            InitializeComponent();
           
        }
        /// <summary>
        /// Initialize the dialog with ConnectionProperties
        /// </summary>
        public void InitializeDialog(ConnectionProperties connectionProperties)
        {
            ConnectionProperties = connectionProperties ?? new ConnectionProperties();

            // Set dialog title based on connection
            this.Text = string.IsNullOrEmpty(ConnectionProperties.ConnectionName)
                ? "New Connection"
                : $"Edit Connection: {ConnectionProperties.ConnectionName}";

            // Initialize driver/version lists
            InitializeDriverLists();

            // Setup event handlers
            SavebeepButton.Click += SavebeepButton_Click;
            CancelbeepButton.Click += CancelbeepButton_Click;

            DriverbeepComboBox.SelectedItemChanged -= DriverbeepComboBox_SelectedItemChanged;
            DriverbeepComboBox.SelectedItemChanged += DriverbeepComboBox_SelectedItemChanged;

            DriverVersionbeepComboBox.SelectedItemChanged -= DriverVersionbeepComboBox_SelectedItemChanged;
            DriverVersionbeepComboBox.SelectedItemChanged += DriverVersionbeepComboBox_SelectedItemChanged;
        }

        /// <summary>
        /// Get the updated ConnectionProperties after dialog interaction
        /// </summary>
        public ConnectionProperties GetUpdatedProperties()
        {
            // Ensure all bound values are committed
            this.Validate();

            return ConnectionProperties;
        }

        /// <summary>
        /// Add a tab to the dialog - called by inherited controls
        /// </summary>
        protected void AddTab(TabPage tab)
        {
            if (beepTabs1 != null && !beepTabs1.TabPages.Contains(tab))
            {
                beepTabs1.TabPages.Add(tab);
            }
        }

        /// <summary>
        /// Validate the connection before saving
        /// </summary>
        protected virtual bool ValidateConnection()
        {
            // Basic validation - can be overridden by specific connection types
            if (string.IsNullOrEmpty(ConnectionProperties?.ConnectionName))
            {
                MessageBox.Show("Connection name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void InitializeDriverLists()
        {
            // Initialize driver and version lists based on connection properties
            // This replaces the old navigation-based initialization
            if (ConnectionProperties?.Category != null && ConnectionProperties?.DatabaseType != null)
            {
                Category = ConnectionProperties.Category;
                SourceType = ConnectionProperties.DatabaseType;

                versions = new List<SimpleItem>();
                drivers = new List<SimpleItem>();

                // Get available drivers for this category/type
                // Note: This assumes beepService is available - may need to pass it in or make it injectable
                connectionDriversConfigs = new List<ConnectionDriversConfig>(); // TODO: Get from config

                foreach (var item in connectionDriversConfigs)
                {
                    SimpleItem driveritem = new SimpleItem();
                    driveritem.DisplayField = item.PackageName;
                    driveritem.Text = item.PackageName;
                    driveritem.Name = item.PackageName;
                    driveritem.Value = item.PackageName;

                    // Add versions for this driver
                    // TODO: Populate versions list

                    drivers.Add(driveritem);
                }

                DriverbeepComboBox.ListItems = drivers.ToBindingList();

                // Preselect current driver if available
                if (!string.IsNullOrEmpty(ConnectionProperties.DriverName))
                {
                    DriverbeepComboBox.SetValue(ConnectionProperties.DriverName);
                    // Filter versions for selected driver
                    DriverVersionbeepComboBox.ListItems = versions.Where(v => v.ParentValue?.ToString() == ConnectionProperties.DriverName).ToBindingList();
                }

                if (!string.IsNullOrEmpty(ConnectionProperties.DriverVersion))
                {
                    DriverVersionbeepComboBox.SetValue(ConnectionProperties.DriverVersion);
                }
            }
        }

        private void SetupBindings()
        {
            if (ConnectionProperties == null) return;

            // Clear existing bindings to avoid duplicates
            ConnectionNamebeepTextBox.DataBindings.Clear();
           
            ConnectionStringbeepTextBox.DataBindings.Clear();

            // Bind directly to ConnectionProperties
            ConnectionNamebeepTextBox.DataBindings.Add(new Binding("Text", ConnectionProperties, "UserID", true, DataSourceUpdateMode.OnPropertyChanged));
          
            ConnectionStringbeepTextBox.DataBindings.Add(new Binding("Text", ConnectionProperties, "ConnectionString", true, DataSourceUpdateMode.OnPropertyChanged));
        }

        private void SavebeepButton_Click(object sender, EventArgs e)
        {
            if (ValidateConnection())
            {
                this.DialogResult = DialogResult.OK;
               
            }
        }

        private void CancelbeepButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            
        }

        private void DriverVersionbeepComboBox_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is SimpleItem v && ConnectionProperties != null)
            {
                ConnectionProperties.DriverVersion = v.Value?.ToString();
            }
        }

        private void DriverbeepComboBox_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null && ConnectionProperties != null)
            {
                SimpleItem selectedItem = (SimpleItem)e.SelectedItem;
                // Filter versions for the selected driver
                DriverVersionbeepComboBox.ListItems = versions.Where(v => v.ParentValue == (string)selectedItem.Value).ToBindingList();

                // Update the connection driver name when selection changes
                ConnectionProperties.DriverName = selectedItem.Value?.ToString();
            }
        }
    }
}
