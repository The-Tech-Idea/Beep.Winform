using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_DataConnectionBase : TemplateUserControl
    {
        // create a new cancel and save  events to tell parent form to close
        public event EventHandler SaveClicked;
        public event EventHandler CancelClicked;


        RecordStatus recordstatus = RecordStatus.Nothing;
        DataConnectionViewModel viewModel;
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
      
        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            viewModel = new DataConnectionViewModel(beepService.DMEEditor, beepService.vis);
            SavebeepButton.Click += SavebeepButton_Click;
            CancelbeepButton.Click += CancelbeepButton_Click;
        }

        public virtual void CancelbeepButton_Click(object? sender, EventArgs e)
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SavebeepButton_Click(object? sender, EventArgs e)
        {
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
            this.DataContext = viewModel.DataConnections;


        }

        private void DriverbeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SimpleItem selectedItem = (SimpleItem)e.SelectedItem;
                if (selectedItem.Item != null)
                {
                    //viewModel.InstallFolderPath = (string)selectedItem.Value;
                   DriverVersionbeepComboBox.ListItems = versions.Where(v => v.ParentValue == (string)selectedItem.Value).ToBindingList();
                }
                else
                {
                   // viewModel.InstallFolderPath = null;
                }
            }
        }
    }
    public enum RecordStatus { New,Edit, Delete, Nothing };
}
