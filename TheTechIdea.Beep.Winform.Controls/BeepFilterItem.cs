using System.ComponentModel;
using System.Diagnostics;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Controls")]
    [DisplayName("Beep Filter Item")]
    [Description("A single filter condition for the BeepFilter control")]
    public class BeepFilterItem : BaseControl
    {
        private FilterCondition condition;
        private List<string> fieldNames;
        private List<Type> fieldTypes;
        private FlowLayoutPanel visualPanel;
        private FlowLayoutPanel editPanel;
        private BeepButton removeButton;
        private BeepButton checkButton;

        // Optional: restrict operators by type
        [Browsable(false)]
        public List<FilterOperator>? AllowedOperators { get; set; }

        public event EventHandler<FilterChangedEventArgs> ConditionChanged;
        public event EventHandler RemoveRequested;

        public bool UseMenuForOperandsAndOperators { get; set; } = false;
        public bool ShowLogicalOperator { get; set; } = false; // groups handle logical ops by default

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
                Visible = true // Default to visual mode
            };
            editPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Visible = false, // Hidden by default
                MinimumSize = new Size(300, 0) // Ensure enough space for controls
            };
            Controls.Add(visualPanel);
            Controls.Add(editPanel);

            int textheight;
            // get texthieght using textbox control
            using (var tb = new TextBox())
            {
                tb.Text = "Test";
                textheight = tb.PreferredSize.Height;
            }
            // Initialize remove button
            removeButton = new BeepButton
            {
                HideText = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                ImagePath = ImageListHelper.GetImagePathFromName("close.svg"),
                Size = new Size(20, textheight),
                IsRounded = false,
                MaxImageSize = new Size(18, textheight - 2),
                Visible = true,
                Anchor = AnchorStyles.None,
                IsFrameless = true,
                IsRoundedAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
            };
            removeButton.Click += (s, e) => RemoveRequested?.Invoke(this, EventArgs.Empty);

            // Initialize check button
            checkButton = new BeepButton
            {
                HideText = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                ImagePath = ImageListHelper.GetImagePathFromName("checkround.svg"),

                IsRounded = false,
                Size = new Size(20, textheight),
                MaxImageSize = new Size(18, textheight - 2),
                Anchor = AnchorStyles.None,
                IsFrameless = true,
                Visible = false, // Hidden by default, shown in edit mode
                IsRoundedAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
            };
            checkButton.Click += (s, e) => ExitEditMode();

            // Handle Delete/Subtract key for removal
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Subtract)
                {
                    RemoveRequested?.Invoke(this, EventArgs.Empty);
                }
            };

            // Add click handler to switch to edit mode
            visualPanel.Click += (s, e) => SwitchToEditMode();
            foreach (Control control in visualPanel.Controls)
            {
                control.Click += (s, e) => SwitchToEditMode();
            }

            // Handle focus loss to exit edit mode
            editPanel.LostFocus += (s, e) =>
            {
                if (!editPanel.ContainsFocus && !checkButton.ContainsFocus)
                {
                    ExitEditMode();
                }
            };
            editPanel.ControlAdded += (s, e) =>
            {
                e.Control.LostFocus += (s2, e2) =>
                {
                    if (!editPanel.ContainsFocus && !checkButton.ContainsFocus)
                    {
                        ExitEditMode();
                    }
                };
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

            visualPanel.Visible = true;
            editPanel.Visible = false;
            checkButton.Visible = false;

            if (ShowLogicalOperator)
            {
                var logicalLabel = new Label
                {
                    Text = condition.LogicalOperator.ToString(),
                    BackColor = Color.FromArgb(255, 182, 193),
                    ForeColor = Color.Black,
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
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(2),
                Padding = new Padding(2),
                BorderStyle = BorderStyle.FixedSingle
            };
            fieldLabel.Click += (s, e) => SwitchToEditMode();
            visualPanel.Controls.Add(fieldLabel);

            var operatorLabel = new Label
            {
                Text = GetOperatorChipText(condition.Operator),
                BackColor = Color.FromArgb(224, 255, 224),
                ForeColor = Color.Black,
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
                    ForeColor = Color.Black,
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
                    ForeColor = Color.Black,
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
                    ForeColor = Color.Black,
                    AutoSize = true,
                    Margin = new Padding(2),
                    Padding = new Padding(2),
                    BorderStyle = BorderStyle.FixedSingle
                };
                valueLabel.Click += (s, e) => SwitchToEditMode();
                visualPanel.Controls.Add(valueLabel);
            }

            // Add removeButton to visualPanel
            visualPanel.Controls.Add(removeButton);
        }

        private static string GetOperatorChipText(FilterOperator op)
        {
            return op switch
            {
                FilterOperator.Equals => "=",
                FilterOperator.NotEquals => "!=",
                FilterOperator.GreaterThan => ">",
                FilterOperator.LessThan => "<",
                FilterOperator.Contains => "contains",
                FilterOperator.StartsWith => "begins with",
                FilterOperator.EndsWith => "ends with",
                FilterOperator.IsBetween => "between",
                _ => op.ToString()
            };
        }

        private void SwitchToEditMode()
        {
            // Initialize defaults when entering edit mode
            if (string.IsNullOrEmpty(condition.FieldName) && fieldNames.Count > 0)
            {
                condition.FieldName = fieldNames[0];
                var index = 0;
                condition.DataType = fieldTypes.Count > 0 ? fieldTypes[index] : typeof(string);
            }

            editPanel.Visible = true;
            visualPanel.Visible = false;
            checkButton.Visible = true;
            UpdateEditModeControls();
            editPanel.PerformLayout(); // Force layout update
            editPanel.Focus(); // Ensure focus is on edit panel
        }

        private void ExitEditMode()
        {
            visualPanel.Visible = true;
            editPanel.Visible = false;
            checkButton.Visible = false;
            UpdateDisplay();
            RaiseConditionChanged();
        }

        private void UpdateEditModeControls()
        {
            editPanel.Controls.Clear();

            if (ShowLogicalOperator)
            {
                var logicalControl = CreateOperandControl(Enum.GetNames(typeof(FilterLogicalOperator)), condition.LogicalOperator.ToString(), "Logical", c =>
                {
                    condition.LogicalOperator = (FilterLogicalOperator)Enum.Parse(typeof(FilterLogicalOperator), c);
                }, false);
                editPanel.Controls.Add(logicalControl);
            }

            var fieldControl = CreateOperandControl(fieldNames.ToArray(), condition.FieldName, "Field", c =>
            {
                condition.FieldName = c;
                var index = fieldNames.IndexOf(condition.FieldName);
                condition.DataType = index >= 0 ? fieldTypes[index] : typeof(string);
                UpdateEditModeControls();
            });
            editPanel.Controls.Add(fieldControl);

            // Choose operator list; if AllowedOperators provided, respect it
            string[] operatorItems = AllowedOperators != null && AllowedOperators.Count > 0
                ? AllowedOperators.Select(o => o.ToString()).ToArray()
                : Enum.GetNames(typeof(FilterOperator));

            // Keep current selection if present; otherwise pick first
            string currentOp = operatorItems.Contains(condition.Operator.ToString())
                ? condition.Operator.ToString()
                : operatorItems.FirstOrDefault() ?? FilterOperator.Equals.ToString();

            var operatorControl = CreateOperandControl(operatorItems, currentOp, "Operator", c =>
            {
                // Parse only valid enum names
                if (Enum.TryParse(typeof(FilterOperator), c, out var parsed))
                {
                    condition.Operator = (FilterOperator)parsed;
                }
                UpdateEditModeControls();
            });
            editPanel.Controls.Add(operatorControl);

            UpdateEditModeValueControls();

            // Add checkButton and removeButton to the end of editPanel
            editPanel.Controls.Add(checkButton);
            editPanel.Controls.Add(removeButton);
        }

        private void UpdateEditModeValueControls()
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
                    Width = 120,
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
                    Width = 120,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Tag = tag,
                    Enabled = !disabled,
                    DropDownHeight = 150
                };
                comboBox.Items.AddRange(items);
                comboBox.SelectedItem = selectedItem;
                comboBox.SelectedIndexChanged += (s, e) =>
                {
                    if (comboBox.SelectedItem != null)
                        onSelected(comboBox.SelectedItem.ToString());
                };
                return comboBox;
            }
        }

        private Control CreateValueControl(string tag, object value, Action<object> onValueChanged)
        {
            Control valueControl;

            // Booleans first
            if (condition.DataType == typeof(bool))
            {
                var chk = new CheckBox
                {
                    Width = 20,
                    Checked = value is bool b && b
                };
                chk.CheckedChanged += (s, e) => onValueChanged(chk.Checked);
                valueControl = chk;
            }
            // Numeric types
            else if (condition.DataType == typeof(int) || condition.DataType == typeof(decimal) ||
                     condition.DataType == typeof(long) || condition.DataType == typeof(double) || condition.DataType == typeof(float))
            {
                var numeric = new NumericUpDown
                {
                    Width = 120,
                    DecimalPlaces = (condition.DataType == typeof(decimal) || condition.DataType == typeof(double) || condition.DataType == typeof(float)) ? 2 : 0,
                    Value = value != null ? SafeToDecimal(value) : 0
                };
                numeric.ValueChanged += (s, e) => onValueChanged(numeric.Value);
                valueControl = numeric;
            }
            // Date/time
            else if (condition.DataType == typeof(DateTime))
            {
                var datePicker = new DateTimePicker
                {
                    Width = 140,
                    Value = value is DateTime dt ? dt : DateTime.Now
                };
                datePicker.ValueChanged += (s, e) => onValueChanged(datePicker.Value);
                valueControl = datePicker;
            }
            // Enum types
            else if (condition.DataType != null && condition.DataType.IsEnum)
            {
                var combo = new ComboBox
                {
                    Width = 140,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                var names = Enum.GetNames(condition.DataType);
                combo.Items.AddRange(names);
                combo.SelectedItem = value?.ToString() ?? names.FirstOrDefault();
                combo.SelectedIndexChanged += (s, e) => onValueChanged(combo.SelectedItem);
                valueControl = combo;
            }
            else
            {
                var textBox = new TextBox
                {
                    Width = 140,
                    Text = value?.ToString() ?? ""
                };
                textBox.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                        ExitEditMode();
                };
                textBox.LostFocus += (s, e) =>
                {
                    if (!editPanel.ContainsFocus && !checkButton.ContainsFocus)
                        ExitEditMode();
                };
                textBox.TextChanged += (s, e) => onValueChanged(textBox.Text);
                valueControl = textBox;
            }
            valueControl.Tag = tag;
            return valueControl;
        }

        private static decimal SafeToDecimal(object v)
        {
            try
            {
                return Convert.ToDecimal(v);
            }
            catch
            {
                return 0m;
            }
        }

        private void RaiseConditionChanged()
        {
            ConditionChanged?.Invoke(this, new FilterChangedEventArgs(new List<FilterCondition> { condition }));
        }
    }
}