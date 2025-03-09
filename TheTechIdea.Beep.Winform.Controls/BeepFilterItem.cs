using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Filter Item")]
    [Description("A single filter condition for the BeepFilter control")]
        public class BeepFilterItem : BeepControl
        {
            private FilterCondition condition;
            private List<string> fieldNames;
            private List<Type> fieldTypes;
            private FlowLayoutPanel visualPanel;
            private FlowLayoutPanel editPanel;
            private Button removeButton; // New remove button

            public event EventHandler<FilterChangedEventArgs> ConditionChanged;
            public event EventHandler RemoveRequested; // New event for removal

            public FilterCriteriaDisplayStyle DisplayStyle { get; set; } = FilterCriteriaDisplayStyle.Visual;
            public bool UseMenuForOperandsAndOperators { get; set; } = false;
            public bool ShowLogicalOperator { get; set; } = true;

            public BeepFilterItem(List<string> fieldNames, List<Type> fieldTypes)
            {
                this.fieldNames = fieldNames;
                this.fieldTypes = fieldTypes;
                condition = new FilterCondition();

                visualPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    Visible = DisplayStyle == FilterCriteriaDisplayStyle.Visual
                };
                editPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    Visible = DisplayStyle == FilterCriteriaDisplayStyle.Text
                };
                Controls.Add(visualPanel);
                Controls.Add(editPanel);

                // Add remove button
                removeButton = new Button
                {
                    Text = "x",
                    Width = 20,
                    Height = 20,
                    Visible = true
                };
                removeButton.Click += (s, e) => RemoveRequested?.Invoke(this, EventArgs.Empty);
                Controls.Add(removeButton);

                // Handle Delete/Subtract key for removal
                KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Subtract)
                    {
                        RemoveRequested?.Invoke(this, EventArgs.Empty);
                    }
                };

                UpdateDisplay();
            }

            public FilterCondition Condition
            {
                get => condition;
                set
                {
                    condition = value;
                    UpdateDisplay();
                }
            }

            public void UpdateDisplay()
            {
                visualPanel.Controls.Clear();
                editPanel.Controls.Clear();

                if (DisplayStyle == FilterCriteriaDisplayStyle.Visual)
                {
                    visualPanel.Visible = true;
                    editPanel.Visible = false;

                    if (ShowLogicalOperator)
                    {
                        var logicalLabel = new Label
                        {
                            Text = condition.LogicalOperator.ToString(),
                            BackColor = Color.FromArgb(255, 182, 193),
                            AutoSize = true,
                            Margin = new Padding(2),
                            Padding = new Padding(2),
                            BorderStyle = BorderStyle.FixedSingle
                        };
                        logicalLabel.Click += (s, e) => SwitchToEditMode();
                        visualPanel.Controls.Add(logicalLabel);
                    }

                    var fieldLabel = new Label
                    {
                        Text = condition.FieldName ?? "Select Field",
                        BackColor = Color.FromArgb(255, 255, 224),
                        AutoSize = true,
                        Margin = new Padding(2),
                        Padding = new Padding(2),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    fieldLabel.Click += (s, e) => SwitchToEditMode();
                    visualPanel.Controls.Add(fieldLabel);

                    var operatorLabel = new Label
                    {
                        Text = condition.Operator.ToString().Replace("Is", "is ").ToLower(),
                        BackColor = Color.FromArgb(224, 255, 224),
                        AutoSize = true,
                        Margin = new Padding(2),
                        Padding = new Padding(2),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    operatorLabel.Click += (s, e) => SwitchToEditMode();
                    visualPanel.Controls.Add(operatorLabel);

                    if (condition.Operator == FilterOperator.IsBetween)
                    {
                        var value1Label = new Label
                        {
                            Text = condition.Value?.ToString() ?? "Value",
                            BackColor = Color.FromArgb(224, 238, 255),
                            AutoSize = true,
                            Margin = new Padding(2),
                            Padding = new Padding(2),
                            BorderStyle = BorderStyle.FixedSingle
                        };
                        value1Label.Click += (s, e) => SwitchToEditMode();
                        visualPanel.Controls.Add(value1Label);

                        var andLabel = new Label { Text = "and", AutoSize = true, Margin = new Padding(2) };
                        andLabel.Click += (s, e) => SwitchToEditMode();
                        visualPanel.Controls.Add(andLabel);

                        var value2Label = new Label
                        {
                            Text = condition.Value2?.ToString() ?? "Value",
                            BackColor = Color.FromArgb(224, 238, 255),
                            AutoSize = true,
                            Margin = new Padding(2),
                            Padding = new Padding(2),
                            BorderStyle = BorderStyle.FixedSingle
                        };
                        value2Label.Click += (s, e) => SwitchToEditMode();
                        visualPanel.Controls.Add(value2Label);
                    }
                    else
                    {
                        var valueLabel = new Label
                        {
                            Text = condition.Value?.ToString() ?? "Value",
                            BackColor = Color.FromArgb(224, 238, 255),
                            AutoSize = true,
                            Margin = new Padding(2),
                            Padding = new Padding(2),
                            BorderStyle = BorderStyle.FixedSingle
                        };
                        valueLabel.Click += (s, e) => SwitchToEditMode();
                        visualPanel.Controls.Add(valueLabel);
                    }

                    // Position the remove button at the end
                    removeButton.Location = new Point(visualPanel.PreferredSize.Width + 5, 5);
                }
                else
                {
                    visualPanel.Visible = false;
                    editPanel.Visible = true;

                    if (ShowLogicalOperator)
                    {
                        var logicalControl = CreateOperandControl(Enum.GetNames(typeof(FilterLogicalOperator)), condition.LogicalOperator.ToString(), "Logical", c =>
                        {
                            condition.LogicalOperator = (FilterLogicalOperator)Enum.Parse(typeof(FilterLogicalOperator), c);
                            RaiseConditionChanged();
                        }, false);
                        editPanel.Controls.Add(logicalControl);
                    }

                    var fieldControl = CreateOperandControl(fieldNames.ToArray(), condition.FieldName, "Field", c =>
                    {
                        condition.FieldName = c;
                        var index = fieldNames.IndexOf(condition.FieldName);
                        condition.DataType = index >= 0 ? fieldTypes[index] : typeof(string);
                        UpdateEditModeControls();
                        RaiseConditionChanged();
                    });
                    editPanel.Controls.Add(fieldControl);

                    var operatorControl = CreateOperandControl(Enum.GetNames(typeof(FilterOperator)), condition.Operator.ToString(), "Operator", c =>
                    {
                        condition.Operator = (FilterOperator)Enum.Parse(typeof(FilterOperator), c);
                        UpdateEditModeControls();
                        RaiseConditionChanged();
                    });
                    editPanel.Controls.Add(operatorControl);

                    UpdateEditModeControls();

                    // Position the remove button at the end
                    removeButton.Location = new Point(editPanel.PreferredSize.Width + 5, 5);
                }
            }

            // Rest of the BeepFilterItem implementation remains unchanged...
            private void UpdateEditModeControls()
            {
                var valueControls = editPanel.Controls.OfType<Control>().Where(c => c.Tag?.ToString() == "Value" || c.Tag?.ToString() == "Value2").ToList();
                foreach (var control in valueControls)
                    editPanel.Controls.Remove(control);

                if (condition.Operator == FilterOperator.IsBetween)
                {
                    var value1Control = CreateValueControl("Value", condition.Value, v => condition.Value = v);
                    editPanel.Controls.Add(value1Control);

                    var andLabel = new Label { Text = "and", AutoSize = true, Margin = new Padding(2) };
                    editPanel.Controls.Add(andLabel);

                    var value2Control = CreateValueControl("Value2", condition.Value2, v => condition.Value2 = v);
                    editPanel.Controls.Add(value2Control);
                }
                else
                {
                    var valueControl = CreateValueControl("Value", condition.Value, v => condition.Value = v);
                    editPanel.Controls.Add(valueControl);
                }
            }

        private Control CreateOperandControl(string[] items, string selectedItem, string tag, Action<string> onSelected, bool disabled = false)
        {
            if (UseMenuForOperandsAndOperators)
            {
                var contextMenu = new ContextMenuStrip
                {
                    AutoSize = true,
                    ShowCheckMargin = false,
                    ShowImageMargin = false
                };
                foreach (var item in items)
                {
                    var menuItem = new ToolStripMenuItem(item)
                    {
                        Checked = item == selectedItem,
                        CheckOnClick = true
                    };
                    contextMenu.Items.Add(menuItem);
                }
                contextMenu.ItemClicked += (s, e) =>
                {
                    foreach (ToolStripMenuItem item in contextMenu.Items)
                        item.Checked = false;
                    ((ToolStripMenuItem)e.ClickedItem).Checked = true;
                    var label = ((Control)e.ClickedItem.Owner.Tag);
                    label.Text = e.ClickedItem.Text;
                    onSelected(e.ClickedItem.Text);
                };
                var label = new Label
                {
                    Text = selectedItem ?? items.FirstOrDefault() ?? "Select...",
                    Width = 100,
                    Tag = tag,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.LightGray,
                    Enabled = !disabled
                };
                contextMenu.Tag = label;
                label.ContextMenuStrip = contextMenu;
                label.Click += (s, e) =>
                {
                    if (!disabled)
                        label.ContextMenuStrip.Show(label, new Point(0, label.Height));
                };
                return label;
            }
            else
            {
                var comboBox = new ComboBox
                {
                    Width = 100,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Tag = tag,
                    Enabled = !disabled,
                    AutoCompleteMode = AutoCompleteMode.None, // Changed to None for DropDownList
                                                              // Removed AutoCompleteSource since it's not needed with AutoCompleteMode.None
                    DropDownHeight = 150
                };
                comboBox.Items.AddRange(items);
                comboBox.SelectedItem = selectedItem;
                comboBox.SelectedIndexChanged += (s, e) => onSelected(comboBox.SelectedItem?.ToString());
                return comboBox;
            }
        }
        private Control CreateValueControl(string tag, object value, Action<object> onValueChanged)
            {
                Control valueControl;
                if (condition.DataType == typeof(int) || condition.DataType == typeof(decimal))
                {
                    var numeric = new NumericUpDown
                    {
                        Width = 100,
                        Value = value != null ? Convert.ToDecimal(value) : 0
                    };
                    numeric.ValueChanged += (s, e) => onValueChanged(numeric.Value);
                    valueControl = numeric;
                }
                else if (condition.DataType == typeof(DateTime))
                {
                    var datePicker = new DateTimePicker
                    {
                        Width = 100,
                        Value = value != null ? (DateTime)value : DateTime.Now
                    };
                    datePicker.ValueChanged += (s, e) => onValueChanged(datePicker.Value);
                    valueControl = datePicker;
                }
                else
                {
                    var textBox = new TextBox
                    {
                        Width = 100,
                        Text = value?.ToString() ?? ""
                    };
                    textBox.TextChanged += (s, e) => onValueChanged(textBox.Text);
                    valueControl = textBox;
                }
                valueControl.Tag = tag;
                return valueControl;
            }

            private void SwitchToEditMode()
            {
                DisplayStyle = FilterCriteriaDisplayStyle.Text;
                UpdateDisplay();
            }

            private void RaiseConditionChanged()
            {
                ConditionChanged?.Invoke(this, new FilterChangedEventArgs(new List<FilterCondition>() { condition }));
                Debug.WriteLine($"Condition changed: {condition}");
            }
        }
}


