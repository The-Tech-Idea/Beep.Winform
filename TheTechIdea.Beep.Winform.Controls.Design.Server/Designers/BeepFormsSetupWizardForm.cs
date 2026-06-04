using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormsSetupWizardForm : Form
    {
        private readonly BeepForms _owner;
        private readonly BeepFormsDefinition _working;
        private readonly List<BeepDataConnection> _availableConnections;

        private readonly TabControl _tabControl;
        private readonly Button _backButton;
        private readonly Button _nextButton;
        private readonly Button _finishButton;
        private readonly Button _cancelButton;

        private readonly TextBox _formIdTextBox;
        private readonly TextBox _formNameTextBox;
        private readonly TextBox _titleTextBox;
        private readonly CheckBox _autoCreateCheckBox;
        private readonly ComboBox _dataConnectionComboBox;
        private ListBox _blocksListBox;
        private readonly TextBox _triggerInfoTextBox;

        public BeepFormsDefinition Result => _working;

        public BeepFormsSetupWizardForm(BeepForms owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _working = owner.Definition?.Clone() ?? new BeepFormsDefinition();
            if (string.IsNullOrWhiteSpace(_working.FormName))
            {
                _working.FormName = owner.FormName ?? "BeepForm";
            }

            _availableConnections = new List<BeepDataConnection>();
            CollectAvailableConnections();

            Text = "BeepForms Setup Wizard";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Size = new Size(720, 480);
            MinimumSize = new Size(560, 360);

            _tabControl = new TabControl { Dock = DockStyle.Fill };
            _tabControl.TabPages.Add(BuildFormPropertiesTab(out _formIdTextBox, out _formNameTextBox, out _titleTextBox, out _autoCreateCheckBox));
            _tabControl.TabPages.Add(BuildDataSourceTab(out _dataConnectionComboBox));
            _tabControl.TabPages.Add(BuildBlocksTab());
            _tabControl.TabPages.Add(BuildReviewTab(out _triggerInfoTextBox));

            _backButton = new Button { Text = "< Back", Width = 90, Height = 30 };
            _nextButton = new Button { Text = "Next >", Width = 90, Height = 30 };
            _finishButton = new Button { Text = "Finish", DialogResult = DialogResult.OK, Width = 90, Height = 30 };
            _cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90, Height = 30 };

            _backButton.Click += OnBackClicked;
            _nextButton.Click += OnNextClicked;
            _finishButton.Click += OnFinishClicked;

            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(12, 6, 12, 6) };
            buttonPanel.Controls.Add(_backButton);
            buttonPanel.Controls.Add(_nextButton);
            buttonPanel.Controls.Add(_finishButton);
            buttonPanel.Controls.Add(_cancelButton);
            buttonPanel.Resize += (_, _) => LayoutButtons(buttonPanel);
            LayoutButtons(buttonPanel);

            Controls.Add(_tabControl);
            Controls.Add(buttonPanel);
            CancelButton = _cancelButton;

            Load += (_, _) => PopulateFromWorking();
        }

        private void CollectAvailableConnections()
        {
            try
            {
                var site = _owner.Site;
                if (site?.Container != null)
                {
                    foreach (var component in site.Container.Components)
                    {
                        if (component is BeepDataConnection connection)
                        {
                            _availableConnections.Add(connection);
                        }
                    }
                }
            }
            catch
            {
                // ignore: site enumeration is a best-effort at design time
            }
        }

        private TabPage BuildFormPropertiesTab(out TextBox idBox, out TextBox formNameBox, out TextBox titleBox, out CheckBox autoCreateBox)
        {
            var tab = new TabPage("Form Properties");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 4; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            }
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            idBox = new TextBox { Dock = DockStyle.Fill };
            formNameBox = new TextBox { Dock = DockStyle.Fill };
            titleBox = new TextBox { Dock = DockStyle.Fill };
            autoCreateBox = new CheckBox { Text = "Auto-create block controls from this definition", Checked = true, AutoSize = true };

            layout.Controls.Add(new Label { Text = "Form ID:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            layout.Controls.Add(idBox, 1, 0);
            layout.Controls.Add(new Label { Text = "Form Name:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            layout.Controls.Add(formNameBox, 1, 1);
            layout.Controls.Add(new Label { Text = "Title:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
            layout.Controls.Add(titleBox, 1, 2);
            layout.Controls.Add(new Label { Text = "Behavior:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 3);
            layout.Controls.Add(autoCreateBox, 1, 3);

            var description = new Label
            {
                Text = "Step 1 of 4: Set identifying properties for this form.",
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText,
                Padding = new Padding(0, 8, 0, 0)
            };
            layout.Controls.Add(description, 0, 4);
            layout.SetColumnSpan(description, 2);

            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage BuildDataSourceTab(out ComboBox dataConnectionComboBox)
        {
            var tab = new TabPage("Data Source");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var description = new Label
            {
                Text = "Step 2 of 4: Pick the BeepDataConnection used at runtime (optional).",
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText
            };
            layout.Controls.Add(description, 0, 0);

            dataConnectionComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            dataConnectionComboBox.Items.Add("(none)");
            foreach (var connection in _availableConnections)
            {
                dataConnectionComboBox.Items.Add(connection);
            }
            layout.Controls.Add(dataConnectionComboBox, 0, 1);

            var helpLabel = new Label
            {
                Text = "Blocks may override this default with their own ConnectionName.",
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText,
                Padding = new Padding(0, 8, 0, 0)
            };
            layout.Controls.Add(helpLabel, 0, 2);

            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage BuildBlocksTab()
        {
            var tab = new TabPage("Blocks");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

            var description = new Label
            {
                Text = "Step 3 of 4: Reorder blocks; the order here is the order they are created at runtime.",
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText
            };
            layout.Controls.Add(description, 0, 0);
            layout.SetColumnSpan(description, 2);

            var listBox = new ListBox { Dock = DockStyle.Fill };
            _blocksListBox = listBox;
            layout.Controls.Add(listBox, 0, 1);
            layout.SetRowSpan(listBox, 2);

            var moveUp = new Button { Text = "Move Up", Dock = DockStyle.Top, Height = 28 };
            var moveDown = new Button { Text = "Move Down", Dock = DockStyle.Top, Height = 28 };
            var addStarter = new Button { Text = "Add Starter", Dock = DockStyle.Top, Height = 28 };
            var remove = new Button { Text = "Remove", Dock = DockStyle.Top, Height = 28 };
            moveUp.Click += (_, _) => MoveBlock(listBox, -1);
            moveDown.Click += (_, _) => MoveBlock(listBox, +1);
            addStarter.Click += (_, _) => AddStarterBlock(listBox);
            remove.Click += (_, _) => RemoveBlock(listBox);

            var buttonStack = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            buttonStack.Controls.AddRange(new Control[] { addStarter, moveUp, moveDown, remove });
            layout.Controls.Add(buttonStack, 1, 1);
            layout.SetRowSpan(buttonStack, 2);

            var blocksHint = new Label
            {
                Text = "Use the BeepBlock smart-tag to add fields and entity metadata to each block.",
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText,
                Padding = new Padding(0, 8, 0, 0)
            };
            layout.Controls.Add(blocksHint, 0, 3);
            layout.SetColumnSpan(blocksHint, 2);

            tab.Controls.Add(layout);
            return tab;
        }

        private TabPage BuildReviewTab(out TextBox triggerInfoTextBox)
        {
            var tab = new TabPage("Review");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(12)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var description = new Label
            {
                Text = "Step 4 of 4: Review and finish. Triggers are managed from the BeepBlock smart-tag.",
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText
            };
            layout.Controls.Add(description, 0, 0);

            triggerInfoTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = SystemColors.Control
            };
            layout.Controls.Add(triggerInfoTextBox, 0, 1);

            tab.Controls.Add(layout);
            return tab;
        }

        private void LayoutButtons(Panel buttonPanel)
        {
            _cancelButton.Location = new Point(buttonPanel.Width - _cancelButton.Width - 6, 7);
            _finishButton.Location = new Point(_cancelButton.Location.X - _finishButton.Width - 6, 7);
            _nextButton.Location = new Point(_finishButton.Location.X - _nextButton.Width - 6, 7);
            _backButton.Location = new Point(_nextButton.Location.X - _backButton.Width - 6, 7);
        }

        private void PopulateFromWorking()
        {
            _formIdTextBox.Text = _working.Id ?? string.Empty;
            _formNameTextBox.Text = _working.FormName ?? string.Empty;
            _titleTextBox.Text = _working.Title ?? string.Empty;
            _autoCreateCheckBox.Checked = _owner.AutoCreateBlocksFromDefinition;
            _dataConnectionComboBox.SelectedIndex = 0;
            if (_owner.DataConnection != null)
            {
                int idx = _dataConnectionComboBox.Items.IndexOf(_owner.DataConnection);
                if (idx >= 0)
                {
                    _dataConnectionComboBox.SelectedIndex = idx;
                }
            }
            RefreshBlocksList(_blocksListBox);
            RefreshTriggerSummary();
        }

        private void RefreshBlocksList(ListBox listBox)
        {
            listBox.Items.Clear();
            foreach (var block in _working.Blocks)
            {
                if (block == null)
                {
                    continue;
                }
                listBox.Items.Add(block);
            }
        }

        private void MoveBlock(ListBox listBox, int direction)
        {
            int idx = listBox.SelectedIndex;
            if (idx < 0 || idx >= _working.Blocks.Count)
            {
                return;
            }
            int newIdx = idx + direction;
            if (newIdx < 0 || newIdx >= _working.Blocks.Count)
            {
                return;
            }
            var block = _working.Blocks[idx];
            _working.Blocks.RemoveAt(idx);
            _working.Blocks.Insert(newIdx, block);
            listBox.SelectedIndex = newIdx;
            RefreshBlocksList(listBox);
        }

        private void AddStarterBlock(ListBox listBox)
        {
            int n = _working.Blocks.Count + 1;
            _working.Blocks.Add(new BeepBlockDefinition
            {
                BlockName = $"Block{n}",
                Caption = $"Block {n}",
                PresentationMode = BeepBlockPresentationMode.Record,
                Entity = new BeepBlockEntityDefinition
                {
                    EntityName = string.Empty,
                    Caption = string.Empty
                }
            });
            RefreshBlocksList(listBox);
        }

        private void RemoveBlock(ListBox listBox)
        {
            int idx = listBox.SelectedIndex;
            if (idx < 0 || idx >= _working.Blocks.Count)
            {
                return;
            }
            _working.Blocks.RemoveAt(idx);
            RefreshBlocksList(listBox);
        }

        private void RefreshTriggerSummary()
        {
            var lines = new List<string>
            {
                $"Form: {_working.FormName}",
                $"Title: {(_working.Title ?? "(none)")}",
                $"Blocks: {_working.Blocks.Count}",
                "",
                "Blocks:"
            };
            foreach (var block in _working.Blocks)
            {
                if (block == null)
                {
                    continue;
                }
                lines.Add($"  - {block.BlockName} ({block.PresentationMode}) -> {block.Entity?.EntityName ?? "(no entity)"}");
            }
            lines.Add("");
            lines.Add("Triggers are configured per-block via the BeepBlock smart-tag.");
            _triggerInfoTextBox.Lines = lines.ToArray();
        }

        private void OnBackClicked(object? sender, EventArgs e)
        {
            if (_tabControl.SelectedIndex > 0)
            {
                _tabControl.SelectedIndex -= 1;
                UpdateNavigationButtons();
            }
        }

        private void OnNextClicked(object? sender, EventArgs e)
        {
            if (_tabControl.SelectedIndex < _tabControl.TabPages.Count - 1)
            {
                _tabControl.SelectedIndex += 1;
                UpdateNavigationButtons();
            }
        }

        private void OnFinishClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_formNameTextBox.Text))
            {
                MessageBox.Show(this, "Form Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _tabControl.SelectedIndex = 0;
                UpdateNavigationButtons();
                DialogResult = DialogResult.None;
                return;
            }

            _working.Id = _formIdTextBox.Text.Trim();
            _working.FormName = _formNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(_working.Title))
            {
                _working.Title = _working.FormName;
            }
            _owner.AutoCreateBlocksFromDefinition = _autoCreateCheckBox.Checked;
            _owner.DataConnection = _dataConnectionComboBox.SelectedItem as BeepDataConnection;
        }

        private void UpdateNavigationButtons()
        {
            bool onFirst = _tabControl.SelectedIndex == 0;
            bool onLast = _tabControl.SelectedIndex == _tabControl.TabPages.Count - 1;
            _backButton.Enabled = !onFirst;
            _nextButton.Enabled = !onLast;
            _finishButton.Enabled = onLast;
            if (onLast)
            {
                RefreshTriggerSummary();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateNavigationButtons();
        }
    }
}
