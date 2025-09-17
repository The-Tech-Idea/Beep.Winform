using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.Examples
{
    /// <summary>
    /// Example form demonstrating the BeepRadioGroupAdvanced control
    /// </summary>
    public partial class RadioGroupExampleForm : Form
    {
        private BeepRadioGroup _materialRadioGroup;
        private BeepRadioGroup _cardRadioGroup;
        private BeepRadioGroup _chipRadioGroup;
        private BeepRadioGroup _circularRadioGroup;
        private Label _selectionLabel;
        private ComboBox _styleComboBox;
        private ComboBox _orientationComboBox;
        private CheckBox _multipleSelectionCheckBox;

        public RadioGroupExampleForm()
        {
            InitializeComponent();
            SetupControls();
            PopulateData();
        }

        private void InitializeComponent()
        {
            this.Text = "BeepRadioGroupAdvanced Examples";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);
        }

        private void SetupControls()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(20)
            };

            // Column styles
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Row styles
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));

            // Create radio groups
            _materialRadioGroup = CreateMaterialRadioGroup();
            _cardRadioGroup = CreateCardRadioGroup();
            _chipRadioGroup = CreateChipRadioGroup();
            _circularRadioGroup = CreateCircularRadioGroup();

            // Create control panels
            var materialPanel = CreateGroupPanel("Material Design Style", _materialRadioGroup);
            var cardPanel = CreateGroupPanel("Card Style", _cardRadioGroup);
            var chipPanel = CreateGroupPanel("Chip Style", _chipRadioGroup);
            var circularPanel = CreateGroupPanel("Traditional Circular", _circularRadioGroup);

            // Add to layout
            mainPanel.Controls.Add(materialPanel, 0, 0);
            mainPanel.Controls.Add(cardPanel, 1, 0);
            mainPanel.Controls.Add(chipPanel, 0, 1);
            mainPanel.Controls.Add(circularPanel, 1, 1);

            // Create controls panel
            var controlsPanel = CreateControlsPanel();
            mainPanel.Controls.Add(controlsPanel, 0, 2);
            mainPanel.SetColumnSpan(controlsPanel, 2);

            // Add button to show hierarchical example
            var hierarchicalButton = new Button
            {
                Text = "Show Hierarchical Example",
                Location = new Point(10, 10),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            hierarchicalButton.Click += (s, e) => {
                var hierarchicalForm = new HierarchicalRadioGroupExampleForm();
                hierarchicalForm.Show();
            };
            controlsPanel.Controls.Add(hierarchicalButton);

            this.Controls.Add(mainPanel);
        }

        private BeepRadioGroup CreateMaterialRadioGroup()
        {
            var radioGroup = new BeepRadioGroup
            {
                RenderStyle = RadioGroupRenderStyle.Material,
                Orientation = RadioGroupOrientation.Vertical,
                AllowMultipleSelection = false,
                AutoSizeItems = true,
                ItemSpacing = 12,
                Dock = DockStyle.Fill
            };

            radioGroup.SelectionChanged += OnSelectionChanged;
            return radioGroup;
        }

        private BeepRadioGroup CreateCardRadioGroup()
        {
            var radioGroup = new BeepRadioGroup
            {
                RenderStyle = RadioGroupRenderStyle.Card,
                Orientation = RadioGroupOrientation.Vertical,
                AllowMultipleSelection = true,
                AutoSizeItems = true,
                ItemSpacing = 8,
                Dock = DockStyle.Fill
            };

            radioGroup.SelectionChanged += OnSelectionChanged;
            return radioGroup;
        }

        private BeepRadioGroup CreateChipRadioGroup()
        {
            var radioGroup = new BeepRadioGroup
            {
                RenderStyle = RadioGroupRenderStyle.Chip,
                Orientation = RadioGroupOrientation.Flow,
                AllowMultipleSelection = true,
                AutoSizeItems = true,
                ItemSpacing = 6,
                Dock = DockStyle.Fill
            };

            radioGroup.SelectionChanged += OnSelectionChanged;
            return radioGroup;
        }

        private BeepRadioGroup CreateCircularRadioGroup()
        {
            var radioGroup = new BeepRadioGroup
            {
                RenderStyle = RadioGroupRenderStyle.Circular,
                Orientation = RadioGroupOrientation.Vertical,
                AllowMultipleSelection = false,
                AutoSizeItems = true,
                ItemSpacing = 8,
                Dock = DockStyle.Fill
            };

            radioGroup.SelectionChanged += OnSelectionChanged;
            return radioGroup;
        }

        private Panel CreateGroupPanel(string title, BeepRadioGroup radioGroup)
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
                Height = 60,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(249, 250, 251),
                Padding = new Padding(10)
            };

            // Style selector
            var styleLabel = new Label
            {
                Text = "Demo Style:",
                Location = new Point(10, 20),
                AutoSize = true
            };

            _styleComboBox = new ComboBox
            {
                Location = new Point(85, 17),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _styleComboBox.Items.AddRange(new[] { "Material", "Card", "Chip", "Circular", "Flat" });
            _styleComboBox.SelectedIndex = 0;
            _styleComboBox.SelectedIndexChanged += OnStyleChanged;

            // Orientation selector
            var orientationLabel = new Label
            {
                Text = "Orientation:",
                Location = new Point(220, 20),
                AutoSize = true
            };

            _orientationComboBox = new ComboBox
            {
                Location = new Point(295, 17),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _orientationComboBox.Items.AddRange(new[] { "Vertical", "Horizontal", "Grid", "Flow" });
            _orientationComboBox.SelectedIndex = 0;
            _orientationComboBox.SelectedIndexChanged += OnOrientationChanged;

            // Multiple selection checkbox
            _multipleSelectionCheckBox = new CheckBox
            {
                Text = "Multiple Selection",
                Location = new Point(410, 18),
                AutoSize = true
            };
            _multipleSelectionCheckBox.CheckedChanged += OnMultipleSelectionChanged;

            // Selection display
            _selectionLabel = new Label
            {
                Text = "Selection: None",
                Location = new Point(540, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(75, 85, 99)
            };

            panel.Controls.AddRange(new Control[] {
                styleLabel, _styleComboBox,
                orientationLabel, _orientationComboBox,
                _multipleSelectionCheckBox,
                _selectionLabel
            });

            return panel;
        }

        private void PopulateData()
        {
            // Create sample items
            var items = new[]
            {
                new SimpleItem { Text = "Option 1", SubText = "First choice with description" },
                new SimpleItem { Text = "Option 2", SubText = "Second choice available" },
                new SimpleItem { Text = "Option 3", SubText = "Third alternative option" },
                new SimpleItem { Text = "Option 4", SubText = "Fourth and final choice" }
            };

            // Add items to all radio groups
            foreach (var item in items)
            {
                _materialRadioGroup.AddItem(new SimpleItem { Text = item.Text, SubText = item.SubText });
                _cardRadioGroup.AddItem(new SimpleItem { Text = item.Text, SubText = item.SubText });
                _chipRadioGroup.AddItem(new SimpleItem { Text = item.Text });
                _circularRadioGroup.AddItem(new SimpleItem { Text = item.Text });
            }

            // Set initial selections
            _materialRadioGroup.SelectItem("Option 1");
            _cardRadioGroup.SelectItem("Option 1");
            _circularRadioGroup.SelectItem("Option 1");
        }

        #region Event Handlers
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var radioGroup = sender as BeepRadioGroup;
            if (radioGroup != null)
            {
                var selectedText = string.Join(", ", e.SelectedItems.Select(i => i.Text));
                _selectionLabel.Text = $"Selection: {(string.IsNullOrEmpty(selectedText) ? "None" : selectedText)}";
            }
        }

        private void OnStyleChanged(object sender, EventArgs e)
        {
            if (_styleComboBox.SelectedItem == null) return;

            var style = _styleComboBox.SelectedItem.ToString();
            var renderStyle = (RadioGroupRenderStyle)Enum.Parse(typeof(RadioGroupRenderStyle), style);

            // Update the current demo radio group
            var currentGroup = GetCurrentDemoGroup();
            if (currentGroup != null)
            {
                currentGroup.RenderStyle = renderStyle;
            }
        }

        private void OnOrientationChanged(object sender, EventArgs e)
        {
            if (_orientationComboBox.SelectedItem == null) return;

            var orientation = _orientationComboBox.SelectedItem.ToString();
            var radioOrientation = (RadioGroupOrientation)Enum.Parse(typeof(RadioGroupOrientation), orientation);

            // Update the current demo radio group
            var currentGroup = GetCurrentDemoGroup();
            if (currentGroup != null)
            {
                currentGroup.Orientation = radioOrientation;
            }
        }

        private void OnMultipleSelectionChanged(object sender, EventArgs e)
        {
            var currentGroup = GetCurrentDemoGroup();
            if (currentGroup != null)
            {
                currentGroup.AllowMultipleSelection = _multipleSelectionCheckBox.Checked;
            }
        }

        private BeepRadioGroup GetCurrentDemoGroup()
        {
            // Return the first radio group for demo purposes
            return _materialRadioGroup;
        }
        #endregion
    }
}