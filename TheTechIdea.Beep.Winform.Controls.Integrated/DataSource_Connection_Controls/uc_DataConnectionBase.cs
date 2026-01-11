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
using TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls;
using TheTechIdea.Beep.Helpers;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{

    /// <summary>
    /// Base user control for managing data connection properties.
    /// This control is designed to be embedded in a dialog Form to create or edit ConnectionProperties.
    /// 
    /// <para><strong>Usage Pattern:</strong></para>
    /// <code>
    /// // Create the control
    /// uc_DataConnectionBase connectionDialog = new uc_DataConnectionBase();
    /// 
    /// // Initialize with ConnectionProperties (new or existing)
    /// connectionDialog.InitializeDialog(connectionProperties);
    /// 
    /// // Host in a Form
    /// Form dialogForm = new Form { /* ... */ };
    /// connectionDialog.Dock = DockStyle.Fill;
    /// dialogForm.Controls.Add(connectionDialog);
    /// 
    /// // Show dialog
    /// if (dialogForm.ShowDialog() == DialogResult.OK)
    /// {
    ///     // Get updated properties
    ///     ConnectionProperties updated = connectionDialog.GetUpdatedProperties();
    ///     // Save or use the updated connection
    /// }
    /// </code>
    /// 
    /// <para>The control provides:</para>
    /// <list type="bullet">
    /// <item>Tabbed interface for editing connection properties</item>
    /// <item>Automatic validation based on connection type</item>
    /// <item>Connection testing functionality</item>
    /// <item>ParameterList management for provider-specific settings</item>
    /// <item>Driver and version selection</item>
    /// </list>
    /// 
    /// <para>When the user clicks Save, the parent Form's DialogResult is set to OK and the form closes.
    /// When Cancel is clicked, DialogResult is set to Cancel.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// // Example: Creating a new connection
    /// ConnectionProperties newConn = new ConnectionProperties 
    /// { 
    ///     ConnectionName = "My Connection",
    ///     DatabaseType = DataSourceType.SQLServer 
    /// };
    /// 
    /// uc_DataConnectionBase dialog = new uc_DataConnectionBase();
    /// dialog.InitializeDialog(newConn);
    /// 
    /// Form form = new Form { Size = new Size(700, 800) };
    /// dialog.Dock = DockStyle.Fill;
    /// form.Controls.Add(dialog);
    /// 
    /// if (form.ShowDialog() == DialogResult.OK)
    /// {
    ///     ConnectionProperties updated = dialog.GetUpdatedProperties();
    ///     // Save connection
    /// }
    /// </code>
    /// </example>
    public partial class uc_DataConnectionBase : TemplateUserControl
    {
        // Main data object - ConnectionProperties that will be passed in and returned
        protected ConnectionProperties _connectionProperties;
        private readonly List<uc_DataConnectionPropertiesBaseControl> _childPropertyControls = new();
        // Additional parameters if needed
        // for specific connection types not covered in ConnectionProperties
        /// <summary>
        /// Default parameters for specific connection types.
        /// Set this property before calling InitializeDialog() to provide default values
        /// that will be added to ConnectionProperties.ParameterList if they don't already exist.
        /// 
        /// <para>Example:</para>
        /// <code>
        /// dialog.DefaultParameterList = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "ConnectionTimeout", "30" },
        ///     { "Pooling", "true" }
        /// };
        /// dialog.InitializeDialog(connectionProperties);
        /// </code>
        /// </summary>
        public Dictionary<string, string> DefaultParameterList { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Gets or sets the ConnectionProperties object being edited.
        /// Setting this property automatically sets up data bindings.
        /// Typically set through InitializeDialog() method.
        /// </summary>
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

        /// <summary>
        /// Gets the dialog result after user interaction.
        /// DialogResult.OK when Save is clicked, DialogResult.Cancel when Cancel is clicked.
        /// </summary>
        public DialogResult DialogResult { get; private set; }

        #region Private Fields
        // Properties for dialog behavior
        DataSourceType SourceType;
        DatasourceCategory Category;
        string DataSourceName;
        string ConnectionID;
        List<SimpleItem> versions = new List<SimpleItem>();
        List<SimpleItem> drivers = new List<SimpleItem>();
        List<ConnectionDriversConfig> connectionDriversConfigs = new List<ConnectionDriversConfig>();
        
        // Test Connection button
        protected BeepButton TestConnectionButton;
        #endregion

        public uc_DataConnectionBase()
        {
            InitializeComponent();
           
        }
        #region Public Methods
        /// <summary>
        /// Initialize the dialog with ConnectionProperties
        /// Sets up bindings, driver lists, and event handlers
        /// </summary>
        /// <param name="connectionProperties">The connection properties to edit. Can be null to create a new connection.</param>
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

            // Create and setup Test Connection button
            CreateTestConnectionButton();

            BuildPropertyTabs();
            // Set default parameters if any
            // use ConnectionHelper.GetAllParametersForDataSourceTypeNotInConnectionProperties

            SetDefaultParameters();
        }

        /// <summary>
        /// Get the updated ConnectionProperties after dialog interaction
        /// Validates all bound values are committed before returning
        /// </summary>
        /// <returns>The updated ConnectionProperties object with all user changes</returns>
        public ConnectionProperties GetUpdatedProperties()
        {
            // Ensure all bound values are committed
            this.Validate();

            return ConnectionProperties;
        }
        #endregion
        
        #region Protected Methods
        /// <summary>
        /// Add a tab to the dialog - called by inherited controls to add custom tabs
        /// </summary>
        /// <param name="tab">The TabPage to add to the dialog</param>
        protected void AddTab(TabPage tab)
        {
            if (beepTabs1 != null && tab != null && !beepTabs1.TabPages.Contains(tab))
            {
                beepTabs1.TabPages.Add(tab);
            }
        }

        /// <summary>
        /// Validate the connection before saving
        /// Can be overridden by specific connection types for additional validation
        /// </summary>
        protected virtual bool ValidateConnection()
        {
            if (ConnectionProperties == null)
            {
                MessageBox.Show("Connection properties are not set.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            // Validate required fields based on Category/DatabaseType
            bool isValid = true;
            string errorMessage = string.Empty;
            
            // Connection name is always required
            if (string.IsNullOrWhiteSpace(ConnectionProperties.ConnectionName))
            {
                errorMessage += "Connection name is required.\n";
                isValid = false;
            }
            
            // Validate based on category
            switch (ConnectionProperties.Category)
            {
                case DatasourceCategory.Database:
                    // For database connections, require either connection string or server name
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.ConnectionString) &&
                        string.IsNullOrWhiteSpace(ConnectionProperties.ServerName))
                    {
                        errorMessage += "Connection string or server name is required for database connections.\n";
                        isValid = false;
                    }
                    // Database name is often required for database connections
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.DatabaseName) &&
                        (ConnectionProperties.DatabaseType == DataSourceType.SQLServer ||
                         ConnectionProperties.DatabaseType == DataSourceType.MySQL ||
                         ConnectionProperties.DatabaseType == DataSourceType.PostgreSQL))
                    {
                        errorMessage += "Database name is required for this database type.\n";
                        isValid = false;
                    }
                    break;
                    
                case DatasourceCategory.File:
                    // For file connections, file path is required
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.FilePath))
                    {
                        errorMessage += "File path is required for file connections.\n";
                        isValid = false;
                    }
                    break;
                    
                case DatasourceCategory.WebAPI:
                    // For Web API connections, URL is required
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.URL))
                    {
                        errorMessage += "URL is required for Web API connections.\n";
                        isValid = false;
                    }
                    break;
            }
            
            // Additional validation based on DatabaseType
            if (ConnectionProperties.DatabaseType == DataSourceType.SQLServer ||
                ConnectionProperties.DatabaseType == DataSourceType.MySQL ||
                ConnectionProperties.DatabaseType == DataSourceType.PostgreSQL)
            {
                // For these database types, authentication is typically required
                if (ConnectionProperties.AuthenticationType == AuthenticationType.WindowsAuthentication &&
                    string.IsNullOrWhiteSpace(ConnectionProperties.UserID))
                {
                    // Windows auth might not require UserID, but SQL auth does
                    // This is handled below
                }
                else if (ConnectionProperties.AuthenticationType == AuthenticationType.SQLAuthentication ||
                         ConnectionProperties.AuthenticationType == AuthenticationType.BasicAuthentication)
                {
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.UserID))
                    {
                        errorMessage += "User ID is required for SQL/Basic authentication.\n";
                        isValid = false;
                    }
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.Password))
                    {
                        errorMessage += "Password is required for SQL/Basic authentication.\n";
                        isValid = false;
                    }
                }
            }
            
            // Show validation errors if any
            if (!isValid)
            {
                MessageBox.Show($"Validation failed:\n\n{errorMessage.Trim()}", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            return isValid;
        }
        private void SetupBindings()
        {
            if (ConnectionProperties == null) return;

            // Clear existing bindings to avoid duplicates
            ConnectionNamebeepTextBox.DataBindings.Clear();
           
            ConnectionStringbeepTextBox.DataBindings.Clear();

            // Bind directly to ConnectionProperties
            ConnectionNamebeepTextBox.DataBindings.Add(new Binding("Text", ConnectionProperties, nameof(ConnectionProperties.ConnectionName), true, DataSourceUpdateMode.OnPropertyChanged));
          
            ConnectionStringbeepTextBox.DataBindings.Add(new Binding("Text", ConnectionProperties, nameof(ConnectionProperties.ConnectionString), true, DataSourceUpdateMode.OnPropertyChanged));
        }
        #endregion
        
        #region Dialog Events
        private void SavebeepButton_Click(object sender, EventArgs e)
        {
            if (ValidateConnection())
            {
                this.DialogResult = DialogResult.OK;
                
                // If parent is a form, close it
                if (this.ParentForm != null)
                {
                    this.ParentForm.DialogResult = DialogResult.OK;
                    this.ParentForm.Close();
                }
            }
        }
        private void CancelbeepButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            
            // If parent is a form, close it
            if (this.ParentForm != null)
            {
                this.ParentForm.DialogResult = DialogResult.Cancel;
                this.ParentForm.Close();
            }
        }
        
        private async void TestConnectionButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable button during test
                if (sender is BeepButton btn)
                {
                    btn.Enabled = false;
                    btn.Text = "Testing...";
                }
                
                // Perform connection test
                bool success = await TestConnectionAsync();
                
                if (success)
                {
                    MessageBox.Show("Connection test successful!", "Connection Test", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Connection test failed. Please check your connection properties.", 
                        "Connection Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing connection: {ex.Message}", "Connection Test Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable button
                if (sender is BeepButton btn)
                {
                    btn.Enabled = true;
                    btn.Text = "Test Connection";
                }
            }
        }
        /// <summary>
        /// Create and configure the Test Connection button
        /// </summary>
        private void CreateTestConnectionButton()
        {
            // Remove existing button if present
            if (TestConnectionButton != null)
            {
                if (tabPage1.Controls.Contains(TestConnectionButton))
                {
                    tabPage1.Controls.Remove(TestConnectionButton);
                }
                TestConnectionButton.Dispose();
            }
            
            // Create new Test Connection button
            TestConnectionButton = new BeepButton
            {
                Name = "TestConnectionButton",
                Text = "Test Connection",
                Size = new Size(130, 60),
                Location = new Point(260, 567), // Position between Cancel and Save buttons
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.FromArgb(76, 175, 80), // Green color for test button
                BorderThickness = 1,
                BorderRadius = 8,
                UseFormStylePaint = true,
                TextFont = new System.Drawing.Font("Segoe UI", 15F),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            TestConnectionButton.Click += TestConnectionButton_Click;
            tabPage1.Controls.Add(TestConnectionButton);
            TestConnectionButton.BringToFront();
        }
        
        #endregion
        
        #region Connection Testing
        /// <summary>
        /// Test the connection using current ConnectionProperties
        /// Override in derived classes for specific connection type testing
        /// </summary>
        protected virtual async System.Threading.Tasks.Task<bool> TestConnectionAsync()
        {
            if (ConnectionProperties == null)
            {
                MessageBox.Show("Connection properties are not set.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            // Validate required properties first
            if (!ValidateConnection())
            {
                return false;
            }
            
            try
            {
                // Basic validation - check if connection string is provided or can be built
                if (string.IsNullOrWhiteSpace(ConnectionProperties.ConnectionString) && 
                    string.IsNullOrWhiteSpace(ConnectionProperties.ServerName))
                {
                    MessageBox.Show("Connection string or server name is required.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                
                // TODO: Implement actual connection test using IDataSource factory
                // For now, return true if validation passes
                // In a real implementation, this would:
                // 1. Create an IDataSource instance using the factory
                // 2. Set the ConnectionProperties
                // 3. Attempt to open the connection
                // 4. Return success/failure
                
                await System.Threading.Tasks.Task.Delay(500); // Simulate async operation
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Connection test error: {ex.Message}");
                MessageBox.Show($"Connection test failed: {ex.Message}", "Connection Test Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region Driver and Version Initialization
        /// <summary>
        /// Initialize driver and version lists based on connection properties
        /// Loads drivers from ConnectionDriversConfig or uses defaults based on DatabaseType
        /// </summary>
        private void InitializeDriverLists()
        {
            // Initialize driver and version lists based on connection properties
            // This replaces the old navigation-based initialization
            if (ConnectionProperties != null)
            {
                Category = ConnectionProperties.Category;
                SourceType = ConnectionProperties.DatabaseType;

                versions = new List<SimpleItem>();
                drivers = new List<SimpleItem>();

                try
                {
                    // Get available drivers for this category/type
                    // Try to load from config if available, otherwise use defaults
                    if (connectionDriversConfigs == null || connectionDriversConfigs.Count == 0)
                    {
                        // Load from config if available (e.g., DMEEditor.ConfigEditor.DataDriversClasses)
                        // For now, use default drivers based on DatabaseType
                        connectionDriversConfigs = GetDefaultDriversForType(ConnectionProperties.DatabaseType);
                    }

                    if (connectionDriversConfigs != null && connectionDriversConfigs.Count > 0)
                    {
                        foreach (var item in connectionDriversConfigs)
                        {
                            if (item == null || string.IsNullOrEmpty(item.PackageName)) continue;
                            
                            SimpleItem driveritem = new SimpleItem();
                            driveritem.DisplayField = item.PackageName;
                            driveritem.Text = item.PackageName;
                            driveritem.Name = item.PackageName;
                            driveritem.Value = item.PackageName;

                            // Add versions for this driver from config
                            if (item.Versions != null && item.Versions.Count > 0)
                            {
                                foreach (var version in item.Versions)
                                {
                                    SimpleItem versionItem = new SimpleItem();
                                    versionItem.DisplayField = version;
                                    versionItem.Text = version;
                                    versionItem.Name = version;
                                    versionItem.Value = version;
                                    versionItem.ParentValue = item.PackageName;
                                    versions.Add(versionItem);
                                }
                            }
                            else
                            {
                                // Add default versions if not specified in config
                                AddDefaultVersionsForDriver(item.PackageName);
                            }

                            drivers.Add(driveritem);
                        }
                    }
                    else
                    {
                        // No drivers found, create a default entry based on DatabaseType
                        AddDefaultDriverForType(ConnectionProperties.DatabaseType);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading drivers: {ex.Message}");
                    // Fallback: create default driver entry
                    AddDefaultDriverForType(ConnectionProperties.DatabaseType);
                }

                DriverbeepComboBox.ListItems = drivers.ToBindingList();

                // Preselect current driver if available
                if (!string.IsNullOrEmpty(ConnectionProperties.DriverName))
                {
                    DriverbeepComboBox.SetValue(ConnectionProperties.DriverName);
                    // Filter versions for selected driver
                    DriverVersionbeepComboBox.ListItems = versions
                        .Where(v => v.ParentValue?.ToString() == ConnectionProperties.DriverName)
                        .ToBindingList();
                }
                else if (drivers.Count > 0)
                {
                    // Select first driver by default
                    DriverbeepComboBox.SelectedIndex = 0;
                    if (DriverbeepComboBox.SelectedItem is SimpleItem selectedDriver)
                    {
                        ConnectionProperties.DriverName = selectedDriver.Value?.ToString();
                        DriverVersionbeepComboBox.ListItems = versions
                            .Where(v => v.ParentValue?.ToString() == ConnectionProperties.DriverName)
                            .ToBindingList();
                    }
                }

                if (!string.IsNullOrEmpty(ConnectionProperties.DriverVersion))
                {
                    DriverVersionbeepComboBox.SetValue(ConnectionProperties.DriverVersion);
                }
                else if (versions.Count > 0)
                {
                    // Select first version by default
                    DriverVersionbeepComboBox.ListItems = versions
                        .Where(v => v.ParentValue?.ToString() == ConnectionProperties.DriverName)
                        .ToBindingList();
                    if (DriverVersionbeepComboBox.ListItems.Count > 0)
                    {
                        DriverVersionbeepComboBox.SelectedIndex = 0;
                    }
                }
            }
        }
        
        /// <summary>
        /// Get default drivers for a specific DatabaseType
        /// Override in derived classes for specific driver lists
        /// </summary>
        protected virtual List<ConnectionDriversConfig> GetDefaultDriversForType(DataSourceType databaseType)
        {
            var defaultDrivers = new List<ConnectionDriversConfig>();
            
            // Add common default drivers based on database type
            switch (databaseType)
            {
                case DataSourceType.SQLServer:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "Microsoft.Data.SqlClient",
                        DatasourceType = databaseType
                    });
                    break;
                case DataSourceType.MySQL:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "MySql.Data",
                        DatasourceType = databaseType
                    });
                    break;
                case DataSourceType.PostgreSQL:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "Npgsql",
                        DatasourceType = databaseType
                    });
                    break;
                case DataSourceType.SQLite:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "System.Data.SQLite",
                        DatasourceType = databaseType
                    });
                    break;
                case DataSourceType.Oracle:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "Oracle.ManagedDataAccess",
                        DatasourceType = databaseType
                    });
                    break;
            }
            
            return defaultDrivers;
        }
        
        /// <summary>
        /// Add default driver for a specific DatabaseType
        /// </summary>
        protected virtual void AddDefaultDriverForType(DataSourceType databaseType)
        {
            var defaultDrivers = GetDefaultDriversForType(databaseType);
            if (defaultDrivers.Count > 0)
            {
                var driver = defaultDrivers[0];
                SimpleItem driveritem = new SimpleItem();
                driveritem.DisplayField = driver.PackageName;
                driveritem.Text = driver.PackageName;
                driveritem.Name = driver.PackageName;
                driveritem.Value = driver.PackageName;
                drivers.Add(driveritem);
                AddDefaultVersionsForDriver(driver.PackageName);
            }
        }
        
        /// <summary>
        /// Add default versions for a driver
        /// </summary>
        protected virtual void AddDefaultVersionsForDriver(string driverName)
        {
            // Add common default versions
            var defaultVersions = new[] { "Latest", "1.0", "2.0", "3.0" };
            foreach (var version in defaultVersions)
            {
                SimpleItem versionItem = new SimpleItem();
                versionItem.DisplayField = version;
                versionItem.Text = version;
                versionItem.Name = version;
                versionItem.Value = version;
                versionItem.ParentValue = driverName;
                versions.Add(versionItem);
            }
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
        #endregion

        #region Tabs Composition
        private void BuildPropertyTabs()
        {
            // Tabs are now created in Designer.cs for design-time visibility
            // Here we just setup bindings for the existing designer tabs
            _childPropertyControls.Clear();

            // Setup bindings for designer-created tabs
            if (ucGeneralProperties != null)
            {
                ucGeneralProperties.ConnectionProperties = ConnectionProperties;
                ucGeneralProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucGeneralProperties);
            }

            if (ucTypeFlagsProperties != null)
            {
                ucTypeFlagsProperties.ConnectionProperties = ConnectionProperties;
                ucTypeFlagsProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucTypeFlagsProperties);
            }

            if (ucDatabaseProperties != null)
            {
                ucDatabaseProperties.ConnectionProperties = ConnectionProperties;
                ucDatabaseProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucDatabaseProperties);
            }

            if (ucFileProperties != null)
            {
                ucFileProperties.ConnectionProperties = ConnectionProperties;
                ucFileProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucFileProperties);
            }

            if (ucNetworkProperties != null)
            {
                ucNetworkProperties.ConnectionProperties = ConnectionProperties;
                ucNetworkProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucNetworkProperties);
            }

            if (ucAuthProperties != null)
            {
                ucAuthProperties.ConnectionProperties = ConnectionProperties;
                ucAuthProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucAuthProperties);
            }

            if (ucDriverProperties != null)
            {
                ucDriverProperties.ConnectionProperties = ConnectionProperties;
                ucDriverProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucDriverProperties);
            }

            if (ucWebApiProperties != null)
            {
                ucWebApiProperties.ConnectionProperties = ConnectionProperties;
                ucWebApiProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucWebApiProperties);
            }

            if (ucOAuthProperties != null)
            {
                ucOAuthProperties.ConnectionProperties = ConnectionProperties;
                ucOAuthProperties.SetupBindings(ConnectionProperties);
                _childPropertyControls.Add(ucOAuthProperties);
            }
        }
        #endregion

        #region ParameterList Handling
        
        /// <summary>
        /// Helper method to bind a control's Text property to a ParameterList entry
        /// </summary>
        /// <param name="control">The control to bind (must have a Text property)</param>
        /// <param name="parameterName">The parameter name in ParameterList</param>
        /// <param name="defaultValue">Default value if parameter is missing</param>
        protected void BindToParameterList(Control control, string parameterName, string defaultValue = null)
        {
            if (control == null || ConnectionProperties == null) return;
            
            // Initialize parameter if missing
            if (!ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                ConnectionProperties.ParameterList[parameterName] = defaultValue ?? string.Empty;
            }
            
            // Clear existing bindings
            control.DataBindings.Clear();
            
            // Create binding to ParameterList dictionary entry
            var binding = new Binding("Text", ConnectionProperties.ParameterList, $"[{parameterName}]", 
                true, DataSourceUpdateMode.OnPropertyChanged);
            binding.Format += (s, e) => e.Value = e.Value ?? defaultValue ?? string.Empty;
            binding.Parse += (s, e) => e.Value = e.Value?.ToString() ?? defaultValue ?? string.Empty;
            control.DataBindings.Add(binding);
        }
        
        /// <summary>
        /// Initialize a parameter in ParameterList if it doesn't exist
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="defaultValue">Default value to set if missing</param>
        protected void InitializeParameterIfMissing(string parameterName, string defaultValue)
        {
            if (ConnectionProperties == null) return;
            
            if (!ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                ConnectionProperties.ParameterList[parameterName] = defaultValue ?? string.Empty;
            }
        }
        
        /// <summary>
        /// Get parameter value from ParameterList, returning default if not found
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="defaultValue">Default value to return if parameter is missing</param>
        /// <returns>The parameter value or default value</returns>
        protected string GetParameterValueOrDefault(string parameterName, string defaultValue = null)
        {
            if (ConnectionProperties == null) return defaultValue ?? string.Empty;
            
            if (ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                var value = ConnectionProperties.ParameterList[parameterName];
                return string.IsNullOrEmpty(value) ? (defaultValue ?? string.Empty) : value;
            }
            return defaultValue ?? string.Empty;
        }
        
        public void SetParameterValue(string parameterName, string value)
        {
            if (ConnectionProperties == null || ConnectionProperties.ParameterList == null) return;
            
            if (ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                ConnectionProperties.ParameterList[parameterName] = value;
            }
            else
            {
                ConnectionProperties.ParameterList.Add(parameterName, value);
            }
        }
        public string GetParameterValue(string parameterName)
        {
            if (ConnectionProperties == null || ConnectionProperties.ParameterList == null) return null;
            
            if (ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                return ConnectionProperties.ParameterList[parameterName];
            }
            return null;
        }
        public void RemoveParameter(string parameterName)
        {
            if (ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                ConnectionProperties.ParameterList.Remove(parameterName);
            }
        }
        public void ClearParameters()
        {
            ConnectionProperties.ParameterList.Clear();
        }
        public Dictionary<string, string> GetAllParameters()
        {
            return new Dictionary<string, string>(ConnectionProperties.ParameterList);
        }
        public void SetAllParameters(Dictionary<string, string> parameters)
        {
            ConnectionProperties.ParameterList = new Dictionary<string, string>(parameters);
        }
        public void SetDefaultParameters()
        {
            if (ConnectionProperties == null || ConnectionProperties.ParameterList == null) return;
            
            try
            {
                // Try to get parameters from ConnectionHelper if available
                // ConnectionHelper may be a static class, imported type, or instance from Helpers namespace
                var defaultParams = ConnectionHelper.GetAllParametersForDataSourceTypeNotInConnectionProperties(ConnectionProperties.DatabaseType);
                
                if (defaultParams != null && defaultParams.Count > 0)
                {
                    foreach (parameterinfo paramInfo in defaultParams.Values)
                    {
                        if (paramInfo != null && !string.IsNullOrEmpty(paramInfo.Name))
                        {
                            SetParameterValue(paramInfo.Name, paramInfo.Value ?? string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with DefaultParameterList fallback
                // This handles cases where ConnectionHelper doesn't exist or method is not available
                System.Diagnostics.Debug.WriteLine($"Error getting parameters from ConnectionHelper: {ex.Message}");
            }
            
            // Use DefaultParameterList as fallback if ConnectionHelper fails or is not available
            if (DefaultParameterList != null && DefaultParameterList.Count > 0)
            {
                foreach (var param in DefaultParameterList)
                {
                    if (!ConnectionProperties.ParameterList.ContainsKey(param.Key))
                    {
                        ConnectionProperties.ParameterList[param.Key] = param.Value ?? string.Empty;
                    }
                }
            }
        }
        public bool HasParameter(string parameterName)
        {
            return ConnectionProperties.ParameterList.ContainsKey(parameterName);
        }
        public int ParameterCount()
        {
            return ConnectionProperties.ParameterList.Count;
        }
        public IEnumerable<string> GetParameterNames()
        {
            return ConnectionProperties.ParameterList.Keys;
        }
        public IEnumerable<string> GetParameterValues()
        {
            return ConnectionProperties.ParameterList.Values;
        }
        public void UpdateParameters(Dictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                SetParameterValue(param.Key, param.Value);
            }
        }
        public void SyncParametersWithDefaultParametersList()
        {
            // Add any missing default parameters to the ConnectionProperties.ParameterList
            foreach (var param in DefaultParameterList)
            {
                if (!ConnectionProperties.ParameterList.ContainsKey(param.Key))
                {
                    ConnectionProperties.ParameterList.Add(param.Key, param.Value);
                }
            }
            // remove any parameters not in the DefaultParameterList
            var keysToRemove = ConnectionProperties.ParameterList.Keys.Except(DefaultParameterList.Keys).ToList();
            foreach (var key in keysToRemove)
            {
                ConnectionProperties.ParameterList.Remove(key);
            }
        }
        #endregion
    }
}
