using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.MVVM.ViewModels;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using System.Linq;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig; // ensure LINQ available

namespace TheTechIdea.Beep.Winform.Connections.ConnectionControls
{
    public partial class uc_DataConnectionBase : TemplateUserControl
    {
        // create a new cancel and save  events to tell parent form to close
        public event EventHandler SaveClicked;
        public event EventHandler CancelClicked;


        RecordStatus recordstatus = RecordStatus.Nothing;
        protected DataConnectionViewModel viewModel;
        DataSourceType SourceType;
        DatasourceCategory Category;
        string DataSourceName;
        string ConnectionID;
        List<SimpleItem> versions = new List<SimpleItem>();
        List<SimpleItem> drivers = new List<SimpleItem>();
        List<ConnectionDriversConfig> connectionDriversConfigs = new List<ConnectionDriversConfig>();
        public uc_DataConnectionBase(IServiceProvider services) : base(services)
        {
            InitializeComponent();
            Details.AddinName = "Oracle Connection";
        }
        public uc_DataConnectionBase()
        {
            InitializeComponent();
        }
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            if (viewModel == null)
            {
                viewModel = new DataConnectionViewModel(beepService.DMEEditor, beepService.vis);
            }
            // Setup binding source to point to Connection object
            InitializeBindingSource();

            SavebeepButton.Click += SavebeepButton_Click;
            CancelbeepButton.Click += CancelbeepButton_Click;
        }

        private void InitializeBindingSource()
        {
            try
            {
                // Ensure the BindingSource points to the Connection object so child bindings are valid
                dataConnectionViewModelBindingSource.DataSource = viewModel;
                dataConnectionViewModelBindingSource.DataMember = "Connection";

                // Setup/refresh control bindings
                SetupBindings();
            }
            catch { /* ignore in design-time */ }
        }

        private void SetupBindings()
        {
            // Clear existing bindings to avoid duplicates
            LoginIDbeepTextBox.DataBindings.Clear();
            PasswordbeepTextBox.DataBindings.Clear();
            ConnectionStringbeepTextBox.DataBindings.Clear();
            // Note: BeepComboBox doesn't expose DisplayMember/ValueMember/SelectedValue.
            // We'll handle driver mapping via events + SetValue instead of WinForms binding.

            // Bind to Connection properties through the binding source
            LoginIDbeepTextBox.DataBindings.Add(new Binding("Text", dataConnectionViewModelBindingSource, "UserID", true, DataSourceUpdateMode.OnPropertyChanged));
            PasswordbeepTextBox.DataBindings.Add(new Binding("Text", dataConnectionViewModelBindingSource, "Password", true, DataSourceUpdateMode.OnPropertyChanged));
            ConnectionStringbeepTextBox.DataBindings.Add(new Binding("Text", dataConnectionViewModelBindingSource, "ConnectionString", true, DataSourceUpdateMode.OnPropertyChanged));
        }

        public virtual void CancelbeepButton_Click(object? sender, EventArgs e)
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SavebeepButton_Click(object? sender, EventArgs e)
        {
            // Persist current connection via ViewModel then bubble the event
            try { viewModel?.Save(); } catch { }
            SaveClicked?.Invoke(this, EventArgs.Empty);
        }

        public  override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
          // check if there is parameters DataSourceType and Category
            if (parameters.ContainsKey("DataSourceType"))
            {
                DataSourceType dst = (DataSourceType)parameters["DataSourceType"];
               SourceType = dst;
            }
            if (parameters.ContainsKey("Category"))
            {
                DatasourceCategory cat = (DatasourceCategory)parameters["Category"];
                Category = cat;
            }
            if(parameters.ContainsKey("AddinName"))
            {
                string addinname = parameters["AddinName"].ToString();
                Details.AddinName = addinname;
            }
            if(parameters.ContainsKey("DataSourceName"))
            {
                DataSourceName = parameters["DataSourceName"].ToString();
                viewModel.GetConnectionByName(DataSourceName, SourceType, Category);

            } else if (parameters.ContainsKey("ConnectionID"))
            {
                ConnectionID = parameters["ConnectionID"].ToString();
                viewModel.GetConnection(ConnectionID);

            }
            if(Category!=null && SourceType != null)
            {
                versions = new List<SimpleItem>();
                drivers = new List<SimpleItem>();
                connectionDriversConfigs = beepService.Config_editor.DataDriversClasses.Where(c => c.DatasourceCategory == Category && c.DatasourceType == SourceType).ToList();
                foreach (var item in connectionDriversConfigs)
                {
                    SimpleItem driveritem = new SimpleItem();
                    driveritem.DisplayField = item.PackageName;
                    driveritem.Text = item.PackageName;
                    driveritem.Name = item.PackageName;
                    driveritem.Value = item.PackageName;
                    foreach (var DriversClasse in beepService.Config_editor.DataDriversClasses.Where(x => x.PackageName == item.PackageName))
                    {
                        SimpleItem itemversion = new SimpleItem();
                        itemversion.DisplayField = DriversClasse.version;
                        itemversion.Value = DriversClasse.version;
                        itemversion.Text = DriversClasse.version;
                        itemversion.Name = DriversClasse.version;
                        itemversion.ParentItem = driveritem;
                        itemversion.ParentValue = item.PackageName;
                        versions.Add(itemversion);
                    }
                    drivers.Add(driveritem);
                }
            }
            DriverbeepComboBox.ListItems = drivers.ToBindingList();
            DriverbeepComboBox.SelectedItemChanged -= DriverbeepComboBox_SelectedItemChanged;
            DriverbeepComboBox.SelectedItemChanged += DriverbeepComboBox_SelectedItemChanged;

            // Also wire version selection to update the connection
            DriverVersionbeepComboBox.SelectedItemChanged -= DriverVersionbeepComboBox_SelectedItemChanged;
            DriverVersionbeepComboBox.SelectedItemChanged += DriverVersionbeepComboBox_SelectedItemChanged;

            // Initialize binding source for current connection
            InitializeBindingSource();

            // Preselect current driver and version if available
            if (!string.IsNullOrEmpty(viewModel?.Connection?.DriverName))
            {
                DriverbeepComboBox.SetValue(viewModel.Connection.DriverName);
                // Populate versions list for the selected driver
                var selectedDriver = viewModel.Connection.DriverName;
                DriverVersionbeepComboBox.ListItems = versions.Where(v => v.ParentValue?.ToString() == selectedDriver).ToBindingList();
            }
            if (!string.IsNullOrEmpty(viewModel?.Connection?.DriverVersion))
            {
                DriverVersionbeepComboBox.SetValue(viewModel.Connection.DriverVersion);
            }

            this.DataContext = viewModel.Connection; // set to current connection if framework uses it
        }

        private void DriverVersionbeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is SimpleItem v && viewModel?.Connection != null)
            {
                viewModel.Connection.DriverVersion = v.Value?.ToString();
            }
        }

        private void DriverbeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SimpleItem selectedItem = (SimpleItem)e.SelectedItem;
                // Filter versions for the selected driver
                DriverVersionbeepComboBox.ListItems = versions.Where(v => v.ParentValue == (string)selectedItem.Value).ToBindingList();

                // Update the connection driver name when selection changes
                if (viewModel?.Connection != null)
                {
                    viewModel.Connection.DriverName = selectedItem.Value?.ToString();
                }
            }
        }
    }
    public enum RecordStatus { New,Edit, Delete, Nothing };
}
