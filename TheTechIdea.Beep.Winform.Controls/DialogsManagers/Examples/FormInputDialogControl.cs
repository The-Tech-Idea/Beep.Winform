using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Examples
{
    /// <summary>
    /// Generic form input control for dialogs
    /// Supports multiple input types (text, number, date, dropdown)
    /// </summary>
    public class FormInputDialogControl : UserControl
    {
        private FlowLayoutPanel flowPanel;
        private Dictionary<string, Control> inputControls = new Dictionary<string, Control>();
        private Dictionary<string, Label> labels = new Dictionary<string, Label>();
        private Label lblError;

        #region Configuration

        /// <summary>
        /// Input field definition
        /// </summary>
        public class InputField
        {
            public string Name { get; set; }
            public string Label { get; set; }
            public InputType Type { get; set; } = InputType.Text;
            public bool Required { get; set; } = false;
            public object DefaultValue { get; set; }
            public List<object> DropdownItems { get; set; }
            public int? MinLength { get; set; }
            public int? MaxLength { get; set; }
            public string ValidationPattern { get; set; }
            public string ValidationMessage { get; set; }
        }

        public enum InputType
        {
            Text,
            Number,
            Date,
            Dropdown,
            MultilineText,
            Checkbox
        }

        #endregion

        public FormInputDialogControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(10)
            };

            lblError = new Label
            {
                Text = "",
                Size = new Size(280, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 53, 69),
                Visible = false,
                Margin = new Padding(0, 0, 0, 10)
            };

            flowPanel.Controls.Add(lblError);

            Controls.Add(flowPanel);
            Size = new Size(320, 400);
            BackColor = Color.Transparent;
        }

        /// <summary>
        /// Add input fields to the form
        /// </summary>
        public void AddFields(params InputField[] fields)
        {
            foreach (var field in fields)
            {
                AddField(field);
            }
        }

        /// <summary>
        /// Add a single input field
        /// </summary>
        private void AddField(InputField field)
        {
            // Label
            var label = new Label
            {
                Text = field.Label + (field.Required ? " *" : ""),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 5, 0, 5)
            };
            labels[field.Name] = label;
            flowPanel.Controls.Add(label);

            // Input control based on type
            Control inputControl = field.Type switch
            {
                InputType.Text => CreateTextBox(field),
                InputType.Number => CreateNumericUpDown(field),
                InputType.Date => CreateDatePicker(field),
                InputType.Dropdown => CreateDropdown(field),
                InputType.MultilineText => CreateMultilineTextBox(field),
                InputType.Checkbox => CreateCheckBox(field),
                _ => CreateTextBox(field)
            };

            inputControls[field.Name] = inputControl;
            flowPanel.Controls.Add(inputControl);
        }

        private Control CreateTextBox(InputField field)
        {
            var txt = new TextBox
            {
                Size = new Size(280, 24),
                Font = new Font("Segoe UI", 10F),
                Text = field.DefaultValue?.ToString() ?? "",
                MaxLength = field.MaxLength ?? 255,
                Margin = new Padding(0, 0, 0, 10)
            };
            txt.Tag = field;
            return txt;
        }

        private Control CreateNumericUpDown(InputField field)
        {
            var num = new NumericUpDown
            {
                Size = new Size(280, 24),
                Font = new Font("Segoe UI", 10F),
                Value = field.DefaultValue != null ? Convert.ToDecimal(field.DefaultValue) : 0,
                Margin = new Padding(0, 0, 0, 10)
            };
            num.Tag = field;
            return num;
        }

        private Control CreateDatePicker(InputField field)
        {
            var date = new DateTimePicker
            {
                Size = new Size(280, 24),
                Font = new Font("Segoe UI", 10F),
                Value = field.DefaultValue is DateTime dt ? dt : DateTime.Now,
                Margin = new Padding(0, 0, 0, 10)
            };
            date.Tag = field;
            return date;
        }

        private Control CreateDropdown(InputField field)
        {
            var cmb = new ComboBox
            {
                Size = new Size(280, 24),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 0, 10)
            };
            
            if (field.DropdownItems != null)
            {
                foreach (var item in field.DropdownItems)
                {
                    cmb.Items.Add(item);
                }
            }
            
            if (field.DefaultValue != null)
            {
                cmb.SelectedItem = field.DefaultValue;
            }
            
            cmb.Tag = field;
            return cmb;
        }

        private Control CreateMultilineTextBox(InputField field)
        {
            var txt = new TextBox
            {
                Size = new Size(280, 80),
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                Text = field.DefaultValue?.ToString() ?? "",
                MaxLength = field.MaxLength ?? 1000,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 0, 0, 10)
            };
            txt.Tag = field;
            return txt;
        }

        private Control CreateCheckBox(InputField field)
        {
            var chk = new CheckBox
            {
                Text = field.Label,
                Size = new Size(280, 24),
                Font = new Font("Segoe UI", 9F),
                Checked = field.DefaultValue is bool b && b,
                Margin = new Padding(0, 0, 0, 10)
            };
            chk.Tag = field;
            
            // Hide the label since checkbox has its own text
            if (labels.TryGetValue(field.Name, out var lbl))
            {
                lbl.Visible = false;
            }
            
            return chk;
        }

        /// <summary>
        /// Get value of a field
        /// </summary>
        public object GetValue(string fieldName)
        {
            if (!inputControls.TryGetValue(fieldName, out var control))
                return null;

            return control switch
            {
                TextBox txt => txt.Text,
                NumericUpDown num => num.Value,
                DateTimePicker date => date.Value,
                ComboBox cmb => cmb.SelectedItem,
                CheckBox chk => chk.Checked,
                _ => null
            };
        }

        /// <summary>
        /// Get all field values
        /// </summary>
        public Dictionary<string, object> GetAllValues()
        {
            var values = new Dictionary<string, object>();
            foreach (var kvp in inputControls)
            {
                values[kvp.Key] = GetValue(kvp.Key);
            }
            return values;
        }

        /// <summary>
        /// Validate all fields
        /// </summary>
        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            foreach (var kvp in inputControls)
            {
                var control = kvp.Value;
                if (control.Tag is InputField field)
                {
                    var value = GetValue(kvp.Key);

                    // Required validation
                    if (field.Required)
                    {
                        if (value == null || 
                            (value is string str && string.IsNullOrWhiteSpace(str)))
                        {
                            errors.Add($"{field.Label} is required.");
                            continue;
                        }
                    }

                    // Length validation for text
                    if (field.Type == InputType.Text && value is string textValue)
                    {
                        if (field.MinLength.HasValue && textValue.Length < field.MinLength.Value)
                        {
                            errors.Add($"{field.Label} must be at least {field.MinLength.Value} characters.");
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                ShowError(string.Join("\n", errors));
                return false;
            }

            HideError();
            return true;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        private void HideError()
        {
            lblError.Visible = false;
        }
    }
}
