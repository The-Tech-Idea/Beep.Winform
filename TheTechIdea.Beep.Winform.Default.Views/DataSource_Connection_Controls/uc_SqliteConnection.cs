using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    [AddinAttribute(Caption = "SQLite Connection", ScopeCreateType = AddinScopeCreateType.Multiple, DatasourceType = DataSourceType.SqlLite, Category = DatasourceCategory.RDBMS, Name = "uc_SqliteConnection", misc = "Config", menu = "Configuration", addinType = AddinType.ConnectionProperties, displayType = DisplayType.Popup, ObjectType = "Beep")]
    public partial class uc_SqliteConnection : uc_DataConnectionBase
    {
        private TabPage filesTab;
        private BeepTextBox filePathTextBox;
        private BeepTextBox fileNameTextBox;
        private BeepTextBox extTextBox;

        public uc_SqliteConnection()
        {
            InitializeComponent();
        }
        public uc_SqliteConnection(IServiceProvider services) : base(services)
        {
            InitializeComponent();
            Details.AddinName = "SQLite Connection";
        }
        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);

            // Hide server/auth fields not applicable to SQLite
            LoginIDbeepTextBox.Visible = false;
            PasswordbeepTextBox.Visible = false;
            HostbeepTextBox.Visible = false;
            PortbeepTextBox.Visible = false;
            sidbeepTextBox.Visible = false;

            // Ensure Files tab exists
            filesTab = FindTab("Files") ?? CreateFilesTab();
            if (!beepTabs1.TabPages.Contains(filesTab))
            {
                beepTabs1.TabPages.Add(filesTab);
            }
        }

        private TabPage FindTab(string text) => beepTabs1.TabPages.Cast<TabPage>().FirstOrDefault(t => t.Text == text);

        private TabPage CreateFilesTab()
        {
            var tab = new TabPage { Text = "Files" };
            filePathTextBox = CreateTextBox("File Path", 30);
            fileNameTextBox = CreateTextBox("File Name", 100);
            extTextBox = CreateTextBox("Extension", 170);

            // Bindings to ConnectionProperties
            filePathTextBox.DataBindings.Add(new Binding("Text", dataConnectionViewModelBindingSource, "FilePath", true, DataSourceUpdateMode.OnPropertyChanged));
            fileNameTextBox.DataBindings.Add(new Binding("Text", dataConnectionViewModelBindingSource, "FileName", true, DataSourceUpdateMode.OnPropertyChanged));
            extTextBox.DataBindings.Add(new Binding("Text", dataConnectionViewModelBindingSource, "Ext", true, DataSourceUpdateMode.OnPropertyChanged));

            tab.Controls.Add(filePathTextBox);
            tab.Controls.Add(fileNameTextBox);
            tab.Controls.Add(extTextBox);
            return tab;
        }

        private BeepTextBox CreateTextBox(string placeholder, int top)
        {
            return new BeepTextBox
            {
                PlaceholderText = placeholder,
                Location = new System.Drawing.Point(24, top),
                Size = new System.Drawing.Size(380, 40),
                Multiline = false,
                WordWrap = false,
                AcceptsTab = false
            };
        }
    }
}
