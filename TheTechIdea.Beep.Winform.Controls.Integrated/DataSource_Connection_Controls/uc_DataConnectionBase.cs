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

using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Common;

using TheTechIdea.Beep.ConfigUtil; // ensure LINQ available
using TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls;
using TheTechIdea.Beep.Helpers;
using TheTechIdea.Beep.Services;

using System.Reflection;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Services;

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
    public partial class uc_DataConnectionBase : UserControl
    {
        private IBeepService beepService;
        private bool _lastConnectionTestSucceeded;
        private bool _requireSuccessfulTestBeforeSave;

        [Browsable(true)]
        [Category("Validation")]
        [Description("Require successful test connection before allowing Save.")]
        [DefaultValue(false)]
        public bool RequireSuccessfulTestBeforeSave
        {
            get => _requireSuccessfulTestBeforeSave;
            set => _requireSuccessfulTestBeforeSave = value;
        }

        [Browsable(false)]
        public bool LastConnectionTestSucceeded => _lastConnectionTestSucceeded;
        public IBeepService BeepService
        {
            get => beepService;
            set
            {
                beepService = value;
                if (beepService != null)
                {
                    // If the editor is already available, we can initialize driver lists
                    if (beepService.DMEEditor != null)
                    {
                        TryHydrateDriversFromService();
                        InitializeDriverLists();
                    }
                }
            }
        }
        private IDMEEditor Editor => beepService?.DMEEditor;
        // Main data object - ConnectionProperties that will be passed in and returned
        protected ConnectionProperties _connectionProperties;
        private readonly List<uc_DataConnectionPropertiesBaseControl> _childPropertyControls = new();
        // Additional parameters if needed
        // for specific connection types not covered in ConnectionProperties
        private static readonly HashSet<string> StronglyTypedConnectionPropertyNames =
            new HashSet<string>(
                typeof(ConnectionProperties)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                    .Select(p => p.Name),
                StringComparer.OrdinalIgnoreCase);

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
            EnsureParameterListInitialized();
            SetDefaultParameters();

            // Set dialog title based on connection
            this.Text = string.IsNullOrEmpty(ConnectionProperties.ConnectionName)
                ? "New Connection"
                : $"Edit Connection: {ConnectionProperties.ConnectionName}";

            // Initialize driver/version lists
            InitializeDriverLists();

            // Setup event handlers
            SavebeepButton.Click -= SavebeepButton_Click;
            CancelbeepButton.Click -= CancelbeepButton_Click;
            SavebeepButton.Click += SavebeepButton_Click;
            CancelbeepButton.Click += CancelbeepButton_Click;

            DriverbeepComboBox.SelectedItemChanged -= DriverbeepComboBox_SelectedItemChanged;
            DriverbeepComboBox.SelectedItemChanged += DriverbeepComboBox_SelectedItemChanged;

            DriverVersionbeepComboBox.SelectedItemChanged -= DriverVersionbeepComboBox_SelectedItemChanged;
            DriverVersionbeepComboBox.SelectedItemChanged += DriverVersionbeepComboBox_SelectedItemChanged;

            // Create and setup Test Connection button
            CreateTestConnectionButton();

            BuildPropertyTabs();
            MoveDriverTabToEnd();
            HookDatabaseTypeRefresh();
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
                case DatasourceCategory.RDBMS:
                    // For database connections, require either connection string or server name
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.ConnectionString) &&
                        string.IsNullOrWhiteSpace(ConnectionProperties.Host))
                    {
                        errorMessage += "Connection string or server name is required for database connections.\n";
                        isValid = false;
                    }
                    // Database name is often required for database connections
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.Database) &&
                        (ConnectionProperties.DatabaseType == DataSourceType.SqlServer ||
                         ConnectionProperties.DatabaseType == DataSourceType.Mysql ||
                         ConnectionProperties.DatabaseType == DataSourceType.Postgre))
                    {
                        errorMessage += "Database name is required for this database type.\n";
                        isValid = false;
                    }
                    break;
                    
                case DatasourceCategory.FILE:
                    // For file connections, file path is required
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.FilePath))
                    {
                        errorMessage += "File path is required for file connections.\n";
                        isValid = false;
                    }
                    break;
                    
                case DatasourceCategory.WEBAPI:
                    // For Web API connections, URL is required
                    if (string.IsNullOrWhiteSpace(ConnectionProperties.Url))
                    {
                        errorMessage += "URL is required for Web API connections.\n";
                        isValid = false;
                    }
                    break;
            }
            
            // Additional validation based on DatabaseType
            if (ConnectionProperties.DatabaseType == DataSourceType.SqlServer ||
                ConnectionProperties.DatabaseType == DataSourceType.Mysql ||
                ConnectionProperties.DatabaseType == DataSourceType.Postgre)
            {
                // For these database types, authentication is typically required
                if (ConnectionProperties.AuthenticationTypeEnum == AuthTypeEnum.Windows &&
                    string.IsNullOrWhiteSpace(ConnectionProperties.UserID))
                {
                    // Windows auth might not require UserID, but SQL auth does
                    // This is handled below
                }
                else if (ConnectionProperties.AuthenticationTypeEnum == AuthTypeEnum.UserPassword ||
                         ConnectionProperties.AuthenticationTypeEnum == AuthTypeEnum.Basic)
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
            EnsureParameterListInitialized();

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
            if (!ValidateConnection())
            {
                return;
            }

            if (RequireSuccessfulTestBeforeSave && !_lastConnectionTestSucceeded)
            {
                var runTest = MessageBox.Show(
                    "A successful connection test is required before saving.\n\nRun test now?",
                    "Connection Test Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (runTest != DialogResult.Yes)
                {
                    return;
                }

                var testOk = TestConnectionAsync().GetAwaiter().GetResult();
                _lastConnectionTestSucceeded = testOk;
                if (!testOk)
                {
                    MessageBox.Show(
                        "Save blocked because the connection test did not succeed.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

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
                _lastConnectionTestSucceeded = success;
                
                if (success)
                {
                    MessageBox.Show("Connection test successful!", "Connection Test", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _lastConnectionTestSucceeded = false;
                    MessageBox.Show("Connection test failed. Please check your connection properties.", 
                        "Connection Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _lastConnectionTestSucceeded = false;
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
                    string.IsNullOrWhiteSpace(ConnectionProperties.Host))
                {
                    MessageBox.Show("Connection string or server name is required.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var editor = ResolveEditorForConnectionTest();
                if (editor == null)
                {
                    MessageBox.Show(
                        "Connection test requires an initialized BeepService/IDMEEditor instance. " +
                        "Host this control in a DI-enabled runtime container.",
                        "Connection Test Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }

                var testConnection = CreateTestConnectionProperties(ConnectionProperties);
                IDataSource testDataSource = null;

                try
                {
                    testDataSource = await System.Threading.Tasks.Task.Run(
                        () => editor.CreateNewDataSourceConnection(testConnection, testConnection.ConnectionName));

                    if (testDataSource == null)
                    {
                        return false;
                    }

                    var state = await System.Threading.Tasks.Task.Run(() => testDataSource.Openconnection());
                    return state == ConnectionState.Open || testDataSource.ConnectionStatus == ConnectionState.Open;
                }
                finally
                {
                    CleanupTestDataSource(editor, testDataSource);
                }
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

        private IDMEEditor ResolveEditorForConnectionTest()
        {
            if (beepService?.DMEEditor != null)
            {
                return beepService.DMEEditor;
            }

            return Editor;
        }

        private void TryHydrateDriversFromService()
        {
            var editor = Editor ?? beepService?.DMEEditor;
            var configEditor = editor?.ConfigEditor ?? beepService?.Config_editor;
            if (editor == null || configEditor == null)
            {
                return;
            }

            try
            {
                editor.AddAllConnectionConfigurations();
            }
            catch
            {
                // Best-effort hydration only.
            }

            try
            {
                configEditor.LoadConnectionDriversConfigValues();
            }
            catch
            {
                // Best-effort hydration only.
            }
        }

        private static ConnectionProperties CreateTestConnectionProperties(ConnectionProperties source)
        {
            var clone = CloneConnectionProperties(source);
            clone.ConnectionName = BuildTemporaryConnectionName(source.ConnectionName);
            clone.GuidID = Guid.NewGuid().ToString();
            clone.ID = 0;
            clone.ParameterList ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            RemoveTypedKeysFromParameterList(clone.ParameterList);
            return clone;
        }

        private static string BuildTemporaryConnectionName(string sourceName)
        {
            var baseName = string.IsNullOrWhiteSpace(sourceName) ? "BeepConnection" : sourceName.Trim();
            return $"{baseName}__ConnTest__{Guid.NewGuid():N}";
        }

        private static ConnectionProperties CloneConnectionProperties(ConnectionProperties source)
        {
            var clone = new ConnectionProperties();
            var properties = typeof(ConnectionProperties)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var property in properties)
            {
                object value;
                try
                {
                    value = property.GetValue(source);
                }
                catch
                {
                    continue;
                }

                try
                {
                    property.SetValue(clone, value);
                }
                catch
                {
                    // Skip non-assignable properties for clone safety.
                }
            }

            clone.ParameterList = source.ParameterList != null
                ? new Dictionary<string, string>(source.ParameterList, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            RemoveTypedKeysFromParameterList(clone.ParameterList);

            return clone;
        }

        private static void CleanupTestDataSource(IDMEEditor editor, IDataSource testDataSource)
        {
            if (testDataSource == null)
            {
                return;
            }

            try
            {
                if (testDataSource.ConnectionStatus == ConnectionState.Open)
                {
                    testDataSource.Closeconnection();
                }
            }
            catch
            {
                // Ignore cleanup close failures.
            }

            try
            {
                if (editor?.DataSources != null)
                {
                    var existing = editor.DataSources.FirstOrDefault(ds => ReferenceEquals(ds, testDataSource));
                    if (existing != null)
                    {
                        editor.DataSources.Remove(existing);
                    }
                }
            }
            catch
            {
                // Ignore cleanup list failures.
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(testDataSource.DatasourceName))
                {
                    DataSourceLifecycleHelper.UnregisterDataSource(testDataSource.DatasourceName);
                }
            }
            catch
            {
                // Ignore cache cleanup failures.
            }

            try
            {
                testDataSource.Dispose();
            }
            catch
            {
                // Ignore dispose failures.
            }
        }

        #region Driver and Version Initialization
        /// <summary>
        /// Initialize driver and version lists based on connection properties
        /// Loads drivers from ConnectionDriversConfig or uses defaults based on DatabaseType
        /// </summary>
        private void InitializeDriverLists(string? preferredDriverName = null, string? preferredDriverVersion = null)
        {
            if (ConnectionProperties == null)
            {
                return;
            }

            Category = ConnectionProperties.Category;
            SourceType = ConnectionProperties.DatabaseType;

            versions = new List<SimpleItem>();
            drivers = new List<SimpleItem>();
            connectionDriversConfigs = new List<ConnectionDriversConfig>();

            try
            {
                connectionDriversConfigs = GetCatalogDriversForType(ConnectionProperties.DatabaseType);
                if (connectionDriversConfigs.Count == 0)
                {
                    connectionDriversConfigs = GetDefaultDriversForType(ConnectionProperties.DatabaseType);
                }

                if (connectionDriversConfigs.Count > 0)
                {
                    var knownDriverNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    var knownVersionKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in connectionDriversConfigs)
                    {
                        if (item == null || string.IsNullOrWhiteSpace(item.PackageName))
                        {
                            continue;
                        }

                        if (!knownDriverNames.Add(item.PackageName))
                        {
                            continue;
                        }

                        drivers.Add(new SimpleItem
                        {
                            DisplayField = item.PackageName,
                            Text = item.PackageName,
                            Name = item.PackageName,
                            Value = item.PackageName
                        });

                        var configVersions = item.Versions ?? new List<string>();
                        if (configVersions.Count > 0)
                        {
                            foreach (var version in configVersions.Where(v => !string.IsNullOrWhiteSpace(v)))
                            {
                                var versionKey = $"{item.PackageName}|{version}";
                                if (!knownVersionKeys.Add(versionKey))
                                {
                                    continue;
                                }

                                versions.Add(new SimpleItem
                                {
                                    DisplayField = version,
                                    Text = version,
                                    Name = version,
                                    Value = version,
                                    ParentValue = item.PackageName
                                });
                            }
                        }
                    }

                    drivers = drivers
                        .OrderBy(item => item.Text, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }

                if (drivers.Count == 0)
                {
                    AddDefaultDriverForType(ConnectionProperties.DatabaseType);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading drivers: {ex.Message}");
                AddDefaultDriverForType(ConnectionProperties.DatabaseType);
            }

            DriverbeepComboBox.ListItems = drivers.ToBindingList();

            var selectedDriverName = preferredDriverName;
            if (string.IsNullOrWhiteSpace(selectedDriverName))
            {
                selectedDriverName = ConnectionProperties.DriverName;
            }

            if (!string.IsNullOrWhiteSpace(selectedDriverName) &&
                drivers.Any(item => string.Equals(item.Value?.ToString(), selectedDriverName, StringComparison.OrdinalIgnoreCase)))
            {
                DriverbeepComboBox.SetValue(selectedDriverName);
            }
            else if (drivers.Count > 0)
            {
                DriverbeepComboBox.SelectedIndex = 0;
            }

            if (DriverbeepComboBox.SelectedItem is SimpleItem selectedDriver)
            {
                ConnectionProperties.DriverName = selectedDriver.Value?.ToString();
            }

            var selectedVersion = preferredDriverVersion;
            if (string.IsNullOrWhiteSpace(selectedVersion))
            {
                selectedVersion = ConnectionProperties.DriverVersion;
            }

            BindVersionsForDriver(ConnectionProperties.DriverName, selectedVersion);
        }

        private List<ConnectionDriversConfig> GetCatalogDriversForType(DataSourceType databaseType)
        {
            var editor = Editor ?? beepService?.DMEEditor;
            var configEditor = editor?.ConfigEditor ?? beepService?.Config_editor;
            if (configEditor == null)
            {
                return new List<ConnectionDriversConfig>();
            }

            try
            {
                configEditor.LoadConnectionDriversConfigValues();
            }
            catch
            {
                // Best effort only.
            }

            var catalog = configEditor.DataDriversClasses ?? new List<ConnectionDriversConfig>();
            if (catalog.Count == 0)
            {
                return new List<ConnectionDriversConfig>();
            }

            var filtered = databaseType == DataSourceType.Unknown
                ? catalog
                : catalog.Where(driver => driver.DatasourceType == databaseType).ToList();

            return filtered
                .Where(driver => driver != null && !string.IsNullOrWhiteSpace(driver.PackageName))
                .GroupBy(driver => $"{driver.PackageName}|{driver.DatasourceType}", StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(driver => driver.PackageName, StringComparer.OrdinalIgnoreCase)
                .ToList();
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
                case DataSourceType.SqlServer:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "Microsoft.Data.SqlClient",
                        DatasourceType = databaseType
                    });
                    break;
                case DataSourceType.Mysql:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "MySql.Data",
                        DatasourceType = databaseType
                    });
                    break;
                case DataSourceType.Postgre:
                    defaultDrivers.Add(new ConnectionDriversConfig 
                    { 
                        PackageName = "Npgsql",
                        DatasourceType = databaseType
                    });
                    break;
                case DataSourceType.SqlLite:
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
                ConnectionProperties.DriverName = selectedItem.Value?.ToString();
                BindVersionsForDriver(ConnectionProperties.DriverName, ConnectionProperties.DriverVersion);
            }
        }

        private void BindVersionsForDriver(string? driverName, string? preferredVersion = null)
        {
            var driverKey = driverName ?? string.Empty;
            var availableVersions = versions
                .Where(v => string.Equals(v.ParentValue?.ToString(), driverKey, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (availableVersions.Count == 0 && !string.IsNullOrWhiteSpace(driverKey))
            {
                AddDefaultVersionsForDriver(driverKey);
                availableVersions = versions
                    .Where(v => string.Equals(v.ParentValue?.ToString(), driverKey, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            availableVersions = availableVersions
                .GroupBy(v => v.Value?.ToString() ?? v.Text ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(v => v.Text, StringComparer.OrdinalIgnoreCase)
                .ToList();

            DriverVersionbeepComboBox.ListItems = availableVersions.ToBindingList();
            if (availableVersions.Count == 0)
            {
                if (ConnectionProperties != null)
                {
                    ConnectionProperties.DriverVersion = string.Empty;
                }
                return;
            }

            if (!string.IsNullOrWhiteSpace(preferredVersion) &&
                availableVersions.Any(v => string.Equals(v.Value?.ToString(), preferredVersion, StringComparison.OrdinalIgnoreCase)))
            {
                DriverVersionbeepComboBox.SetValue(preferredVersion);
                if (ConnectionProperties != null)
                {
                    ConnectionProperties.DriverVersion = preferredVersion;
                }
                return;
            }

            DriverVersionbeepComboBox.SelectedIndex = 0;
            if (ConnectionProperties != null && DriverVersionbeepComboBox.SelectedItem is SimpleItem selectedVersion)
            {
                ConnectionProperties.DriverVersion = selectedVersion.Value?.ToString() ?? selectedVersion.Text;
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
                ucDatabaseProperties.DatabaseTypeChanged -= UcDatabaseProperties_DatabaseTypeChanged;
                ucDatabaseProperties.DatabaseTypeChanged += UcDatabaseProperties_DatabaseTypeChanged;
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

        private void MoveDriverTabToEnd()
        {
            if (beepTabs1 == null || tabPageDriver == null)
            {
                return;
            }

            if (beepTabs1.TabPages.Contains(tabPageDriver))
            {
                beepTabs1.TabPages.Remove(tabPageDriver);
                beepTabs1.TabPages.Add(tabPageDriver);
            }
        }

        private void HookDatabaseTypeRefresh()
        {
            if (ucDatabaseProperties == null)
            {
                return;
            }

            ucDatabaseProperties.DatabaseTypeChanged -= UcDatabaseProperties_DatabaseTypeChanged;
            ucDatabaseProperties.DatabaseTypeChanged += UcDatabaseProperties_DatabaseTypeChanged;
        }

        private void UcDatabaseProperties_DatabaseTypeChanged(object? sender, EventArgs e)
        {
            if (ConnectionProperties == null)
            {
                return;
            }

            var currentDriver = ConnectionProperties.DriverName;
            var currentVersion = ConnectionProperties.DriverVersion;
            InitializeDriverLists(currentDriver, currentVersion);
        }
        #endregion

        #region ParameterList Handling
        private void EnsureParameterListInitialized()
        {
            if (ConnectionProperties != null && ConnectionProperties.ParameterList == null)
            {
                ConnectionProperties.ParameterList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            if (ConnectionProperties?.ParameterList != null)
            {
                RemoveTypedKeysFromParameterList(ConnectionProperties.ParameterList);
            }
        }
        
        /// <summary>
        /// Helper method to bind a control's Text property to a ParameterList entry
        /// </summary>
        /// <param name="control">The control to bind (must have a Text property)</param>
        /// <param name="parameterName">The parameter name in ParameterList</param>
        /// <param name="defaultValue">Default value if parameter is missing</param>
        protected void BindToParameterList(Control control, string parameterName, string defaultValue = null)
        {
            if (control == null || ConnectionProperties == null) return;
            if (IsTypedConnectionProperty(parameterName)) return;
            EnsureParameterListInitialized();
             
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
            if (IsTypedConnectionProperty(parameterName)) return;
            EnsureParameterListInitialized();
             
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
            if (IsTypedConnectionProperty(parameterName)) return defaultValue ?? string.Empty;
            EnsureParameterListInitialized();
             
            if (ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                var value = ConnectionProperties.ParameterList[parameterName];
                return string.IsNullOrEmpty(value) ? (defaultValue ?? string.Empty) : value;
            }
            return defaultValue ?? string.Empty;
        }
        
        public void SetParameterValue(string parameterName, string value)
        {
            if (ConnectionProperties == null || string.IsNullOrWhiteSpace(parameterName)) return;
            if (IsTypedConnectionProperty(parameterName)) return;
            EnsureParameterListInitialized();
              
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
            if (ConnectionProperties == null || string.IsNullOrWhiteSpace(parameterName)) return null;
            if (IsTypedConnectionProperty(parameterName)) return null;
            EnsureParameterListInitialized();
             
            if (ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                return ConnectionProperties.ParameterList[parameterName];
            }
            return null;
        }
        public void RemoveParameter(string parameterName)
        {
            if (ConnectionProperties == null || string.IsNullOrWhiteSpace(parameterName)) return;
            if (IsTypedConnectionProperty(parameterName)) return;
            EnsureParameterListInitialized();

            if (ConnectionProperties.ParameterList.ContainsKey(parameterName))
            {
                ConnectionProperties.ParameterList.Remove(parameterName);
            }
        }
        public void ClearParameters()
        {
            if (ConnectionProperties == null) return;
            EnsureParameterListInitialized();
            ConnectionProperties.ParameterList.Clear();
        }
        public Dictionary<string, string> GetAllParameters()
        {
            if (ConnectionProperties == null) return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            EnsureParameterListInitialized();
            return new Dictionary<string, string>(ConnectionProperties.ParameterList);
        }
        public void SetAllParameters(Dictionary<string, string> parameters)
        {
            if (ConnectionProperties == null) return;
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    if (string.IsNullOrWhiteSpace(kvp.Key) || IsTypedConnectionProperty(kvp.Key))
                    {
                        continue;
                    }

                    result[kvp.Key] = kvp.Value ?? string.Empty;
                }
            }

            ConnectionProperties.ParameterList = result;
        }
        public void SetDefaultParameters()
        {
            if (ConnectionProperties == null) return;
            EnsureParameterListInitialized();
             
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
                            if (IsTypedConnectionProperty(paramInfo.Name))
                            {
                                continue;
                            }
                            if (!ConnectionProperties.ParameterList.ContainsKey(paramInfo.Name))
                            {
                                SetParameterValue(paramInfo.Name, paramInfo.Value ?? string.Empty);
                            }
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
                    if (IsTypedConnectionProperty(param.Key))
                    {
                        continue;
                    }
                    if (!ConnectionProperties.ParameterList.ContainsKey(param.Key))
                    {
                        ConnectionProperties.ParameterList[param.Key] = param.Value ?? string.Empty;
                    }
                }
            }
        }
        public bool HasParameter(string parameterName)
        {
            if (ConnectionProperties == null || string.IsNullOrWhiteSpace(parameterName)) return false;
            EnsureParameterListInitialized();
            return ConnectionProperties.ParameterList.ContainsKey(parameterName);
        }
        public int ParameterCount()
        {
            if (ConnectionProperties == null) return 0;
            EnsureParameterListInitialized();
            return ConnectionProperties.ParameterList.Count;
        }
        public IEnumerable<string> GetParameterNames()
        {
            if (ConnectionProperties == null) return Enumerable.Empty<string>();
            EnsureParameterListInitialized();
            return ConnectionProperties.ParameterList.Keys;
        }
        public IEnumerable<string> GetParameterValues()
        {
            if (ConnectionProperties == null) return Enumerable.Empty<string>();
            EnsureParameterListInitialized();
            return ConnectionProperties.ParameterList.Values;
        }
        public void UpdateParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return;
            foreach (var param in parameters)
            {
                SetParameterValue(param.Key, param.Value);
            }
        }
        public void SyncParametersWithDefaultParametersList()
        {
            if (ConnectionProperties == null || DefaultParameterList == null) return;
            EnsureParameterListInitialized();

            // Add any missing default parameters to the ConnectionProperties.ParameterList
            foreach (var param in DefaultParameterList)
            {
                if (IsTypedConnectionProperty(param.Key))
                {
                    continue;
                }
                if (!ConnectionProperties.ParameterList.ContainsKey(param.Key))
                {
                    ConnectionProperties.ParameterList.Add(param.Key, param.Value);
                }
            }
        }

        private static bool IsTypedConnectionProperty(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && StronglyTypedConnectionPropertyNames.Contains(key);
        }

        private static void RemoveTypedKeysFromParameterList(IDictionary<string, string> parameterList)
        {
            if (parameterList == null || parameterList.Count == 0)
            {
                return;
            }

            var typedKeys = parameterList.Keys
                .Where(IsTypedConnectionProperty)
                .ToList();

            foreach (var key in typedKeys)
            {
                parameterList.Remove(key);
            }
        }
        #endregion
    }
}
