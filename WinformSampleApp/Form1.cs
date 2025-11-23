using TheTechIdea.Beep.Winform.Controls;
using System.Text.Json;
using TheTechIdea.Beep.Winform.Controls.SideBar;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.BottomNavBars;

namespace WinformSampleApp
{
    public partial class Form1 : BeepiForm
    {
        private ComboBox _styleCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox _sideBarStyleCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox _backdropCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private CheckBox _acrylicChk = new CheckBox { Text = "Acrylic (Glass)" };
        private CheckBox _micaChk = new CheckBox { Text = "Mica" };
        private Button _addPageBtn = new Button { Text = "Add Ribbon Page" };
        private Button _addCtxGroupBtn = new Button { Text = "Add Contextual Group" };
        private Button _toggleCtxBtn = new Button { Text = "Toggle Group" };
        private Button _saveQA = new Button { Text = "Save Quick Access" };
        private Button _loadQA = new Button { Text = "Load Quick Access" };
        private Button _loadPreset = new Button { Text = "Load Preset JSON" };
        private CheckBox _darkRibbon = new CheckBox { Text = "Ribbon Dark" };

        private int _ctxGroupId = -1;

        public Form1()
        {
            InitializeComponent();
            Text = "Beep WinForms UI Demo";
            Size = new Size(1100, 750);
            ShowCaptionBar = true;
            ShowRibbonPlaceholder = true;
            RibbonHeight = 120;

            _styleCombo.Items.AddRange(Enum.GetNames(typeof(BeepFormStyle)));
            _styleCombo.SelectedIndexChanged += (_, __) =>
            {
                if (Enum.TryParse<BeepFormStyle>(_styleCombo.SelectedItem?.ToString(), out var s))
                {
                    FormStyle = s;
                    ApplyTheme();
                    Ribbon?.ApplyThemeFromBeep(_currentTheme);
                }
            };
            _styleCombo.SelectedItem = BeepFormStyle.Modern.ToString();

            _backdropCombo.Items.AddRange(Enum.GetNames(typeof(BackdropType)));
            _backdropCombo.SelectedIndexChanged += (_, __) =>
            {
                if (Enum.TryParse<BackdropType>(_backdropCombo.SelectedItem?.ToString(), out var b))
                {
                    Backdrop = b;
                }
            };
            _backdropCombo.SelectedItem = BackdropType.None.ToString();

            _acrylicChk.CheckedChanged += (_, __) => EnableAcrylicForGlass = _acrylicChk.Checked;
            _micaChk.CheckedChanged += (_, __) => EnableMicaBackdrop = _micaChk.Checked;
            _darkRibbon.CheckedChanged += (_, __) => { if (Ribbon != null) { Ribbon.DarkMode = _darkRibbon.Checked; Ribbon.ApplyThemeFromBeep(_currentTheme); } };

            _addPageBtn.Click += (_, __) =>
            {
                var page = Ribbon?.AddPage($"Page {Ribbon?.Tabs.TabPages.Count + 1}");
                if (page != null)
                {
                    var grp = Ribbon?.AddGroup(page, "Group");
                    grp?.AddLargeButton("Large");
                    grp?.AddSmallButton("Small");
                }
            };

            _addCtxGroupBtn.Click += (_, __) =>
            {
                if (Ribbon == null) return;
                _ctxGroupId = Ribbon.AddContextualGroup("Design", Color.MediumPurple);
                var ctxTab = Ribbon.AddContextualTab(_ctxGroupId, "Format");
                var grp = Ribbon.AddGroup(ctxTab, "Arrange");
                grp.AddLargeButton("Align");
                grp.AddSmallButton("Spacing");
            };

            _toggleCtxBtn.Click += (_, __) => { if (Ribbon != null && _ctxGroupId >= 0) Ribbon.SetContextualGroupVisible(_ctxGroupId, true ^ false); };

            _saveQA.Click += (_, __) => Ribbon?.SaveQuickAccessTo("qa.txt");
            _loadQA.Click += (_, __) => Ribbon?.LoadQuickAccessFrom("qa.txt");

            _loadPreset.Click += (_, __) =>
            {
                using var ofd = new OpenFileDialog { Filter = "JSON (*.json)|*.json" };
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        var json = File.ReadAllText(ofd.FileName);
                        StylePresets.LoadFromJson(json);
                        if (StylePresets.Presets.Keys.FirstOrDefault() is string key)
                        {
                            ApplyPreset(key);
                            ApplyTheme();
                        }
                    }
                    catch { }
                }
            };

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
            // Quick demo: Add a BeepSideBar to preview painters and default icon fallback
            var sideBar = new BeepSideBar
            {
                Dock = DockStyle.Left,
                ExpandedWidth = 220,
                CollapsedWidth = 64,
                DefaultItemImagePath = Svgs.Menu,
                Style = TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.FinSet
            };

            var sideBarRail = new BeepSideBar
            {
                Dock = DockStyle.Left,
                ExpandedWidth = 78,
                CollapsedWidth = 64,
                DefaultItemImagePath = Svgs.Menu,
                Style = TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.PillRail
            };

            sideBar.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Dashboard", ImagePath = Svgs.NavDashboard });
            sideBar.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Inbox" }); // no ImagePath -> should use DefaultItemImagePath
            sideBar.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Settings", ImagePath = Svgs.Settings });
            sideBarRail.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Home", ImagePath = Svgs.NavDashboard });
            sideBarRail.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Inbox" });
            sideBarRail.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Search", ImagePath = Svgs.Search });
            Controls.Add(sideBar);
            Controls.Add(sideBarRail);
            panel.Controls.AddRange(new Control[] { new Label{ Text = "Style:" }, _styleCombo, new Label{ Text = "Backdrop:" }, _backdropCombo, _acrylicChk, _micaChk, _darkRibbon, _addPageBtn, _addCtxGroupBtn, _toggleCtxBtn, _saveQA, _loadQA, _loadPreset });

            // Sidebar style selector
            _sideBarStyleCombo.Items.AddRange(Enum.GetNames(typeof(TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle)));
            _sideBarStyleCombo.SelectedIndexChanged += (_, __) =>
            {
                if (Enum.TryParse<TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle>(_sideBarStyleCombo.SelectedItem?.ToString(), out var s))
                {
                    sideBar.Style = s;
                    sideBarRail.Style = s;
                }
            };
            _sideBarStyleCombo.SelectedItem = TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.FinSet.ToString();

            panel.Controls.Add(new Label{ Text = "SideBar Style:" });
            panel.Controls.Add(_sideBarStyleCombo);
            var _bottomBarStyleCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _bottomBarStyleCombo.Items.AddRange(Enum.GetNames(typeof(BottomBarStyle)));
            _bottomBarStyleCombo.SelectedIndexChanged += (_, __) =>
            {
                if (Enum.TryParse<BottomBarStyle>(_bottomBarStyleCombo.SelectedItem?.ToString(), out var s))
                {
                    bottomBar.BarStyle = s;
                }
            };
            _bottomBarStyleCombo.SelectedItem = BottomBarStyle.Classic.ToString();
            panel.Controls.Add(new Label{ Text = "BottomBar Style:" });
            panel.Controls.Add(_bottomBarStyleCombo);
            
            // BottomBar demo
            var bottomBar = new BottomBar
            {
                Dock = DockStyle.Bottom,
                BarHeight = 72,
                DefaultItemImagePath = Svgs.Menu,
                BarStyle = BottomBarStyle.Classic
            };
            bottomBar.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Home", ImagePath = Svgs.NavDashboard });
            bottomBar.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Search", ImagePath = Svgs.Search });
            bottomBar.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Add", ImagePath = Svgs.Add });
            bottomBar.Items.Add(new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = "Profile", ImagePath = Svgs.User });
            bottomBar.ItemClicked += item => MessageBox.Show($"Clicked {item.Text}");
            Controls.Add(bottomBar);
            Controls.Add(panel);
        }
    }
}
