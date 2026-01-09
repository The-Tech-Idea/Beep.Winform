using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Templates
{
    /// <summary>
    /// Template for form-based wizard steps
    /// Creates a step with form fields (text boxes, labels, etc.)
    /// </summary>
    public class FormStepTemplate : WizardStepTemplate
    {
        private readonly string _stepKey;
        private readonly string _stepTitle;
        private readonly string _stepDescription;
        private readonly List<FormField> _fields;

        public FormStepTemplate(string stepKey, string stepTitle, string stepDescription, List<FormField> fields)
        {
            _stepKey = stepKey;
            _stepTitle = stepTitle;
            _stepDescription = stepDescription;
            _fields = fields ?? new List<FormField>();
        }

        public override UserControl CreateStepControl()
        {
            var panel = new UserControl
            {
                AutoScroll = true,
                Padding = new Padding(20)
            };

            int y = 20;
            foreach (var field in _fields)
            {
                // Label
                var label = new BeepLabel
                {
                    Text = field.Label,
                    Location = new Point(20, y),
                    AutoSize = true
                };
                label.ApplyTheme();
                panel.Controls.Add(label);

                y += 25;

                // Input control
                Control inputControl = null;
                switch (field.FieldType)
                {
                    case FormFieldType.Text:
                        inputControl = new BeepTextBox
                        {
                            Location = new Point(20, y),
                            Size = new Size(panel.Width - 40, 32),
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                        };
                        ((BeepTextBox)inputControl).ApplyTheme();
                        break;
                    // Add more field types as needed
                }

                if (inputControl != null)
                {
                    inputControl.Tag = field.Key; // Store field key for data extraction
                    panel.Controls.Add(inputControl);
                    y += 50;
                }
            }

            return panel;
        }

        public override WizardStep GetStepConfig()
        {
            return new WizardStep
            {
                Key = _stepKey,
                Title = _stepTitle,
                Description = _stepDescription
            };
        }

        public override void ExtractData(UserControl control, WizardContext context)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl.Tag is string fieldKey)
                {
                    if (ctrl is BeepTextBox textBox)
                    {
                        context.SetValue(fieldKey, textBox.Text);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Form field definition
    /// </summary>
    public class FormField
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public FormFieldType FieldType { get; set; } = FormFieldType.Text;
        public bool IsRequired { get; set; } = false;
        public string DefaultValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Form field types
    /// </summary>
    public enum FormFieldType
    {
        Text,
        Number,
        Email,
        Password,
        Date,
        Time,
        CheckBox,
        ComboBox,
        TextArea
    }
}
