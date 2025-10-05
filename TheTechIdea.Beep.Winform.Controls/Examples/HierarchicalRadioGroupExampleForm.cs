using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.Examples
{
    /// <summary>
    /// Example form demonstrating the BeepHierarchicalRadioGroup control
    /// </summary>
    public partial class HierarchicalRadioGroupExampleForm : Form
    {
        private BeepHierarchicalRadioGroup _ktloExample;
        private BeepHierarchicalRadioGroup _fruitsExample;
        private Label _selectionLabel;
        private ComboBox _styleComboBox;
        private CheckBox _multipleSelectionCheckBox;
        private CheckBox _showExpandersCheckBox;
        private NumericUpDown _imageSizeNumeric;

        public HierarchicalRadioGroupExampleForm()
        {
            InitializeComponent();
            SetupControls();
            PopulateData();
        }

        private void InitializeComponent()
        {
            this.Text = "Beep Hierarchical Radio Group Examples";
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
        }

        private void SetupControls()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(20)
            };

            // Column styles
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Row styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));

            // Create hierarchical radio groups
            _ktloExample = CreateKTLOExample();
            _fruitsExample = CreateFruitsExample();

            // Create group panels
            var ktloPanel = CreateGroupPanel("KTLO Assessment Categories", _ktloExample);
            var fruitsPanel = CreateGroupPanel("Fruits & Berries Selection", _fruitsExample);

            // Add to layout
            mainPanel.Controls.Add(ktloPanel, 0, 0);
            mainPanel.Controls.Add(fruitsPanel, 1, 0);

            // Create controls panel
            var controlsPanel = CreateControlsPanel();
            mainPanel.Controls.Add(controlsPanel, 0, 1);
            mainPanel.SetColumnSpan(controlsPanel, 2);

            this.Controls.Add(mainPanel);
        }

        private BeepHierarchicalRadioGroup CreateKTLOExample()
        {
            var radioGroup = new BeepHierarchicalRadioGroup
            {
                RenderStyle = RadioGroupRenderStyle.Flat,
                AllowMultipleSelection = true,
                AutoSizeItems = true,
                ItemSpacing = 6,
                IndentSize = 25,
                ShowExpanderButtons = true,
                Dock = DockStyle.Fill
            };

            radioGroup.SelectionChanged += OnSelectionChanged;
            radioGroup.ItemExpandedChanged += OnItemExpandedChanged;
            return radioGroup;
        }

        private BeepHierarchicalRadioGroup CreateFruitsExample()
        {
            var radioGroup = new BeepHierarchicalRadioGroup
            {
                RenderStyle = RadioGroupRenderStyle.Circular,
                AllowMultipleSelection = false,
                AutoSizeItems = true,
                ItemSpacing = 8,
                IndentSize = 30,
                ShowExpanderButtons = true,
                Dock = DockStyle.Fill
            };

            radioGroup.SelectionChanged += OnSelectionChanged;
            radioGroup.ItemExpandedChanged += OnItemExpandedChanged;
            return radioGroup;
        }

        private Panel CreateGroupPanel(string title, BeepHierarchicalRadioGroup radioGroup)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // Add border
            panel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240)))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            radioGroup.Location = new Point(15, 45);
            radioGroup.Size = new Size(panel.Width - 30, panel.Height - 60);
            radioGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(radioGroup);

            return panel;
        }

        private Panel CreateControlsPanel()
        {
            var panel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(249, 250, 251),
                Padding = new Padding(10)
            };

            // ProgressBarStyle selector
            var styleLabel = new Label
            {
                Text = "ProgressBarStyle:",
                Location = new Point(10, 20),
                AutoSize = true
            };

            _styleComboBox = new ComboBox
            {
                Location = new Point(55, 17),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _styleComboBox.Items.AddRange(new[] { "Flat", "Material", "Card", "Circular", "Chip" });
            _styleComboBox.SelectedIndex = 0;
            _styleComboBox.SelectedIndexChanged += OnStyleChanged;

            // Multiple selection checkbox
            _multipleSelectionCheckBox = new CheckBox
            {
                Text = "Multiple Selection",
                Location = new Point(190, 18),
                AutoSize = true,
                Checked = true
            };
            _multipleSelectionCheckBox.CheckedChanged += OnMultipleSelectionChanged;

            // Show expanders checkbox
            _showExpandersCheckBox = new CheckBox
            {
                Text = "Show Expanders",
                Location = new Point(320, 18),
                AutoSize = true,
                Checked = true
            };
            _showExpandersCheckBox.CheckedChanged += OnShowExpandersChanged;

            // Image size control
            var imageSizeLabel = new Label
            {
                Text = "Image Size:",
                Location = new Point(450, 20),
                AutoSize = true
            };

            _imageSizeNumeric = new NumericUpDown
            {
                Location = new Point(520, 17),
                Width = 60,
                Minimum = 16,
                Maximum = 64,
                Value = 24
            };
            _imageSizeNumeric.ValueChanged += OnImageSizeChanged;

            // Action buttons
            var expandAllButton = new Button
            {
                Text = "Expand All",
                Location = new Point(590, 15),
                Size = new Size(80, 25)
            };
            expandAllButton.Click += (s, e) => {
                _ktloExample.ExpandAll();
                _fruitsExample.ExpandAll();
            };

            var collapseAllButton = new Button
            {
                Text = "Collapse All",
                Location = new Point(680, 15),
                Size = new Size(80, 25)
            };
            collapseAllButton.Click += (s, e) => {
                _ktloExample.CollapseAll();
                _fruitsExample.CollapseAll();
            };

            // Selection display
            _selectionLabel = new Label
            {
                Text = "Selection: None",
                Location = new Point(10, 50),
                AutoSize = true,
                ForeColor = Color.FromArgb(75, 85, 99)
            };

            panel.Controls.AddRange(new Control[] {
                styleLabel, _styleComboBox,
                _multipleSelectionCheckBox,
                _showExpandersCheckBox,
                imageSizeLabel, _imageSizeNumeric,
                expandAllButton, collapseAllButton,
                _selectionLabel
            });

            return panel;
        }

        private void PopulateData()
        {
            PopulateKTLOData();
            PopulateFruitsData();
        }

        private void PopulateKTLOData()
        {
            // Create KTLO (Keep The Lights On) hierarchy
            var ktlo = new SimpleItem
            {
                Text = "KTLO",
                SubText = "Keep The Lights On",
                ImagePath = "folder", // This will be rendered if the image exists
                IsExpanded = true,
                IsSelected = true
            };

            // Main categories
            var maintenance = new SimpleItem
            {
                Text = "Maint / Upgrade",
                SubText = "Maintenance and Upgrades",
                ImagePath = "settings", // Example icon
                IsExpanded = true
            };

            var security = new SimpleItem
            {
                Text = "Security / Compliance", 
                SubText = "Security and Compliance",
                ImagePath = "shield", // Example icon
                IsExpanded = true,
                IsSelected = true
            };

            var reliability = new SimpleItem
            {
                Text = "Reliability / Performance",
                SubText = "System Reliability and Performance",
                ImagePath = "trending-up", // Example icon
                IsExpanded = true,
                IsSelected = true
            };

            var monitoring = new SimpleItem
            {
                Text = "Monitoring",
                SubText = "System Monitoring and Alerting",
                ImagePath = "monitor", // Example icon
                IsSelected = true
            };

            var support = new SimpleItem
            {
                Text = "End User Services & Support",
                SubText = "User Support and Services",
                ImagePath = "users", // Example icon
                IsSelected = true
            };

            // Other categories (not selected)
            var enhancements = new SimpleItem 
            { 
                Text = "Enhancements", 
                SubText = "Feature Enhancements",
                ImagePath = "plus-circle" // Example icon
            };
            var retirements = new SimpleItem 
            { 
                Text = "Retirements", 
                SubText = "System Retirements",
                ImagePath = "archive" // Example icon
            };
            var tlm = new SimpleItem 
            { 
                Text = "TLM", 
                SubText = "Technology Lifecycle Management",
                ImagePath = "refresh-cw" // Example icon
            };
            var other = new SimpleItem 
            { 
                Text = "Other", 
                SubText = "Other Activities",
                ImagePath = "more-horizontal" // Example icon
            };

            // Add children to KTLO
            ktlo.Children.Add(maintenance);
            ktlo.Children.Add(security);
            ktlo.Children.Add(reliability);
            ktlo.Children.Add(monitoring);
            ktlo.Children.Add(support);
            ktlo.Children.Add(enhancements);
            ktlo.Children.Add(retirements);
            ktlo.Children.Add(tlm);
            ktlo.Children.Add(other);

            // Add root item
            _ktloExample.AddRootItem(ktlo);
        }

        private void PopulateFruitsData()
        {
            // Create fruits hierarchy
            var directions = new SimpleItem
            {
                Text = "Directions",
                SubText = "Cardinal Directions",
                ImagePath = "compass", // Example icon
                IsExpanded = true
            };

            // Add direction children
            var north = new SimpleItem 
            { 
                Text = "North", 
                SubText = "Side of the world",
                ImagePath = "arrow-up" // Example icon
            };
            var south = new SimpleItem 
            { 
                Text = "South", 
                SubText = "Side of the world",
                ImagePath = "arrow-down" // Example icon
            };
            var east = new SimpleItem 
            { 
                Text = "East", 
                SubText = "Side of the world",
                ImagePath = "arrow-right" // Example icon
            };
            var west = new SimpleItem 
            { 
                Text = "West", 
                SubText = "Side of the world",
                ImagePath = "arrow-left" // Example icon
            };

            directions.Children.Add(north);
            directions.Children.Add(south);
            directions.Children.Add(east);
            directions.Children.Add(west);

            // Create berries hierarchy
            var berries = new SimpleItem
            {
                Text = "Berries",
                SubText = "Various Types of Berries",
                ImagePath = "git-branch", // Example icon (represents berries on branch)
                IsExpanded = true
            };

            var strawberry = new SimpleItem 
            { 
                Text = "Strawberry", 
                SubText = "Delicious berry",
                ImagePath = "star", // Example icon
                IsSelected = true 
            };
            var raspberries = new SimpleItem 
            { 
                Text = "Raspberries", 
                SubText = "Bears like it",
                ImagePath = "circle" // Example icon
            };
            var cherry = new SimpleItem 
            { 
                Text = "Cherry", 
                SubText = "Just beatiful",
                ImagePath = "heart" // Example icon
            };
            var cranberry = new SimpleItem 
            { 
                Text = "Cranberry", 
                SubText = "Russian sour berry",
                ImagePath = "disc" // Example icon
            };

            berries.Children.Add(strawberry);
            berries.Children.Add(raspberries);
            berries.Children.Add(cherry);
            berries.Children.Add(cranberry);

            _fruitsExample.AddRootItem(directions);
            _fruitsExample.AddRootItem(berries);
        }

        #region Event Handlers
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var radioGroup = sender as BeepHierarchicalRadioGroup;
            if (radioGroup != null)
            {
                var selectedText = string.Join(", ", e.SelectedItems.Select(i => i.Text));
                _selectionLabel.Text = $"Selection: {(string.IsNullOrEmpty(selectedText) ? "None" : selectedText)}";
            }
        }

        private void OnItemExpandedChanged(object sender, ItemExpandedEventArgs e)
        {
            Console.WriteLine($"Item '{e.Item.Text}' {(e.IsExpanded ? "expanded" : "collapsed")}");
        }

        private void OnStyleChanged(object sender, EventArgs e)
        {
            if (_styleComboBox.SelectedItem == null) return;

            var style = _styleComboBox.SelectedItem.ToString();
            var renderStyle = (RadioGroupRenderStyle)Enum.Parse(typeof(RadioGroupRenderStyle), style);

            _ktloExample.RenderStyle = renderStyle;
            _fruitsExample.RenderStyle = renderStyle;
        }

        private void OnMultipleSelectionChanged(object sender, EventArgs e)
        {
            _ktloExample.AllowMultipleSelection = _multipleSelectionCheckBox.Checked;
            if (!_multipleSelectionCheckBox.Checked)
            {
                _fruitsExample.AllowMultipleSelection = false;
            }
        }

        private void OnShowExpandersChanged(object sender, EventArgs e)
        {
            _ktloExample.ShowExpanderButtons = _showExpandersCheckBox.Checked;
            _fruitsExample.ShowExpanderButtons = _showExpandersCheckBox.Checked;
        }

        private void OnImageSizeChanged(object sender, EventArgs e)
        {
            var newSize = new Size((int)_imageSizeNumeric.Value, (int)_imageSizeNumeric.Value);
            _ktloExample.MaxImageSize = newSize;
            _fruitsExample.MaxImageSize = newSize;
        }
        #endregion
    }
}