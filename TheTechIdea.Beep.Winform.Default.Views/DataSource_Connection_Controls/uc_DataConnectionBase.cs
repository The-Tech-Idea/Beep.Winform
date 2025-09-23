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
    /// User control for managing data connection properties will embedded opn a dialog form 
    /// to receive and return ConnectionProperties object
    /// its suppose to return updated ConnectionProperties object after user interaction
    /// with the dialog and user clicks OK/Save button and form closes with DialogResult.OK
    /// </summary>
    public partial class uc_DataConnectionBase : TemplateUserControl
    {
        // Main data object - ConnectionProperties that will be passed in and returned
        protected ConnectionProperties _connectionProperties;
        private readonly List<uc_DataConnectionPropertiesBaseControl> _childPropertyControls = new();
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

            BuildPropertyTabs();
            // Set default parameters if any
            // use ConnectionHelper.GetAllParametersForDataSourceTypeNotInConnectionProperties

            SetDefaultParameters();
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
            if (beepTabs1 != null && tab != null && !beepTabs1.TabPages.Contains(tab))
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
        #region Dialog Events
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
        #endregion

        #region Driver and Version Initialization
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

                // Get available drivers for this category/type
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
            // Clear existing extra tabs except the first base tab (tabPage1)
            for (int i = beepTabs1.TabPages.Count - 1; i >= 0; i--)
            {
                var tp = beepTabs1.TabPages[i];
                if (tp != tabPage1)
                {
                    beepTabs1.TabPages.Remove(tp);
                }
            }
            _childPropertyControls.Clear();

            // Always add General and Provider/Categorization and Credentials
            AddChildTab(new uc_GeneralProperties());
            AddChildTab(new uc_ProviderandCategorizationProperties());
            AddChildTab(new uc_CredentialsandConnectionStringProperties());

            // Add according to category/type flags
            var cat = ConnectionProperties.Category;
            bool isDb = ConnectionProperties.IsDatabase || cat == DatasourceCategory.RDBMS || cat == DatasourceCategory.NOSQL || cat == DatasourceCategory.DocumentDB || cat == DatasourceCategory.KeyValueDB || cat == DatasourceCategory.ColumnarDB || cat == DatasourceCategory.GraphDB || cat == DatasourceCategory.TimeSeriesDB || cat == DatasourceCategory.BigData;
            bool isFile = ConnectionProperties.IsFile || cat == DatasourceCategory.FILE;
            bool isWeb = ConnectionProperties.IsWebApi || cat == DatasourceCategory.WEBAPI;

            if (isDb)
            {
                AddChildTab(new uc_DatabaseProperties());
                AddChildTab(new uc_NetwrokandRemoteProperties());
                AddChildTab(new uc_AuthenticationandSecurityProperties());
                AddChildTab(new uc_CertificatesandSSLProperties());
            }
            if (isFile)
            {
                AddChildTab(new uc_FileProperties());
            }
            if (isWeb)
            {
                AddChildTab(new uc_WebApiProperties());
                AddChildTab(new uc_webapiAuthenticationProperties());
                AddChildTab(new uc_HttpCompositionProperties());
                AddChildTab(new uc_NetwrokandRemoteProperties());
            }

            // Additional tabs
            AddChildTab(new uc_TypeandStateFlagsProperties());
            AddChildTab(new uc_RequstandBehaviorProperties());
            // uc_MetaDataProperties is dynamic; it already exists in project if needed, add as last
            AddChildTab(new uc_MetaDataProperties());
        }

        private void AddChildTab(uc_DataConnectionPropertiesBaseControl child)
        {
            if (child == null) return;
            child.ConnectionProperties = ConnectionProperties;
            child.SetupBindings(ConnectionProperties);
            _childPropertyControls.Add(child);
            AddTab(child.ConnectionPropertytabPage);
        }
        #endregion

        #region ParameterList Handling
        public void SetParameterValue(string parameterName, string value)
        {
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
            var defaultParams = ConnectionHelper.GetAllParametersForDataSourceTypeNotInConnectionProperties(ConnectionProperties.DatabaseType);
            foreach (parameterinfo param in defaultParams.Values)
            {
                SetParameterValue(param.Name, param.Value);
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
