using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;

using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Container.Services;

using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Default.Views.Configuration
{
    [AddinAttribute(Caption = "Create Local DB", Name = "uc_CreateLocalDB", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    [AddinVisSchema(BranchID = 1, RootNodeName = "Configuration", Order = 1, ID = 1, BranchText = "Create Local DB", BranchType = EnumPointType.Function, IconImageName = "localconnections.svg", BranchClass = "ADDIN", BranchDescription = "Create Local DB Screen")]

    public partial class uc_CreateLocalDB : TemplateUserControl, IAddinVisSchema
    {
        public uc_CreateLocalDB(IBeepService service)
        {
            InitializeComponent();
            beepservice = service;
            Details.AddinName = "Create Local DB";

        }
        #region "IAddinVisSchema"
        public string RootNodeName { get; set; } = "Configuration";
        public string CatgoryName { get; set; }
        public int Order { get; set; } = 1;
        public int ID { get; set; } = 1;
        public string BranchText { get; set; } = "Create Local DB";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; } = 1;
        public string IconImageName { get; set; } = "localconnections.svg";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; } = "Create Local DB";
        public string BranchClass { get; set; } = "ADDIN";
        public string AddinName { get; set; }
        #endregion "IAddinVisSchema"

        private IBeepService beepservice;
        List<SimpleItem> Drivers = new List<SimpleItem>();
        List<SimpleItem> InstallationFolders = new List<SimpleItem>();
        private DataConnectionViewModel viewModel;

        public override void Configure(Dictionary<string, object> settings)
        {

           viewModel = new DataConnectionViewModel(beepservice.DMEEditor, beepservice.vis);


            foreach (var item in beepservice.Config_editor.DataConnections)
            {
                SimpleItem conn = new SimpleItem();
                conn.Display = item.ConnectionName;
                conn.Text = item.ConnectionName;
                conn.Name = item.ConnectionName;
                conn.Value = item.ConnectionName;
                conn.GuidId = item.GuidID;
                conn.ParentItem = null;
                conn.ContainerGuidID = item.GuidID;
               // DatasourcebeepComboBox.ListItems.Add(conn);
            }
            List<SimpleItem> versions = new List<SimpleItem>();
            foreach (var item in beepservice.Config_editor.DataDriversClasses.Where(i=>i.CreateLocal && !string.IsNullOrEmpty(i.classHandler)))
            {
                SimpleItem driveritem = new SimpleItem();
                driveritem.Display =item.classHandler +" - " +item.DriverClass +" - " + item.version;
                driveritem.Text = item.classHandler + " - " + item.DriverClass + " - " + item.version;
                driveritem.Name = item.PackageName;
                driveritem.Value = item.GuidID;
                foreach (var DriversClasse in beepservice.Config_editor.DataDriversClasses.Where(x => x.PackageName.Equals( item.PackageName)))
                {
                    SimpleItem itemversion = new SimpleItem();
                    itemversion.Display = DriversClasse.version;
                    itemversion.Value = DriversClasse.version;
                    itemversion.Text = DriversClasse.version;
                    itemversion.Name = DriversClasse.version;
                    itemversion.ParentItem = driveritem;
                    itemversion.ParentValue = item.PackageName;
                    versions.Add(itemversion);
                }
                Drivers.Add(driveritem);
            }
            // Get installation folders from config foders
            foreach (var item in beepservice.Config_editor.Config.Folders.Where(x => x.FolderFilesType == FolderFileTypes.DataFiles || x.FolderFilesType == FolderFileTypes.ProjectData).ToList())
            {
                string foldername=Path.GetFileName(item.FolderPath);
                SimpleItem folderitem = new SimpleItem();
                folderitem.Display = foldername;
                folderitem.Text = foldername;
                folderitem.Name = item.FolderPath;
                folderitem.Value = item.GuidID;
                InstallationFolders.Add(folderitem);
            }
            // Get System folders and documents folders (from .net environment)and others  add them to the list installation folders
            string programfilesfolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string commonapplicationdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string commonprogramfilesfolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
            string localapplicationdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string desktopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string personalfolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string documentsfolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string downloadsfolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // now add them to the list
            SimpleItem programfiles = new SimpleItem();
            programfiles.Display = "Program Files";
            programfiles.Text = "Program Files";
            programfiles.Name = programfilesfolder;
            programfiles.Value = programfilesfolder;
            InstallationFolders.Add(programfiles);
            SimpleItem commonapplicationdata = new SimpleItem();
            commonapplicationdata.Display = "Common Application Data";
            commonapplicationdata.Text = "Common Application Data";
            commonapplicationdata.Name = commonapplicationdatafolder;
            commonapplicationdata.Value = commonapplicationdatafolder;
            InstallationFolders.Add(commonapplicationdata);
            SimpleItem commonprogramfiles = new SimpleItem();
            commonprogramfiles.Display = "Common Program Files";
            commonprogramfiles.Text = "Common Program Files";
            commonprogramfiles.Name = commonprogramfilesfolder;
            commonprogramfiles.Value = commonprogramfilesfolder;
            InstallationFolders.Add(commonprogramfiles);
            SimpleItem localapplicationdata = new SimpleItem();
            localapplicationdata.Display = "Local Application Data";
            localapplicationdata.Text = "Local Application Data";
            localapplicationdata.Name = localapplicationdatafolder;
            localapplicationdata.Value = localapplicationdatafolder;
            InstallationFolders.Add(localapplicationdata);
            SimpleItem appdata = new SimpleItem();
            appdata.Display = "Application Data";
            appdata.Text = "Application Data";
            appdata.Name = appdatafolder;
            appdata.Value = appdatafolder;
            InstallationFolders.Add(appdata);
            SimpleItem desktop = new SimpleItem();
            desktop.Display = "Desktop";
            desktop.Text = "Desktop";
            desktop.Name = desktopfolder;
            desktop.Value = desktopfolder;
            InstallationFolders.Add(desktop);
            SimpleItem personal = new SimpleItem();
            personal.Display = "Personal";
            personal.Text = "Personal";
            personal.Name = personalfolder;
            personal.Value = personalfolder;
            InstallationFolders.Add(personal);
            SimpleItem documents = new SimpleItem();
            documents.Display = "Documents";
            documents.Text = "Documents";
            documents.Name = documentsfolder;
            documents.Value = documentsfolder;
            InstallationFolders.Add(documents);
            SimpleItem downloads = new SimpleItem();
            downloads.Display = "Downloads";
            downloads.Text = "Downloads";
            downloads.Name = downloadsfolder;
            downloads.Value = downloadsfolder;
            InstallationFolders.Add(downloads);
            // Add the drivers to the combo box



            LocalDbTypebeepComboBox.ListItems.AddRange( Drivers);
            SystemFolderbeepComboBox.ListItems.AddRange(InstallationFolders);
            LocalDbTypebeepComboBox.SelectedItemChanged += LocalDbTypebeepComboBox_SelectedItemChanged;
        }

        private void LocalDbTypebeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SimpleItem selectedItem = (SimpleItem)e.SelectedItem;
                // Get the selected driver
                //string selectedDriver = selectedItem.Value.ToString();
                //// Get the selected version
                //string selectedVersion = selectedItem.ParentValue.ToString();
                //// Get the selected driver class
                //string selectedDriverClass = selectedItem.Name.ToString();
                //// Get the selected driver name
                //string selectedClassDriverName = selectedItem.Display.ToString();
                // Set the driver name and version in the view model in the UI comboboxs
                

            }
        }

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);

        }
    }

}
