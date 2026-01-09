using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Templates
{
    /// <summary>
    /// Template for summary/review wizard steps
    /// Displays a summary of all entered data for review before completion
    /// </summary>
    public class SummaryStepTemplate : WizardStepTemplate
    {
        private readonly string _stepKey;
        private readonly string _stepTitle;
        private readonly List<SummaryItem> _items;

        public SummaryStepTemplate(string stepKey, string stepTitle, List<SummaryItem> items)
        {
            _stepKey = stepKey;
            _stepTitle = stepTitle;
            _items = items ?? new List<SummaryItem>();
        }

        public override UserControl CreateStepControl()
        {
            var panel = new UserControl
            {
                AutoScroll = true,
                Padding = new Padding(20)
            };

            int y = 20;
            foreach (var item in _items)
            {
                // Label
                var label = new BeepLabel
                {
                    Text = item.Label + ":",
                    Location = new Point(20, y),
                    Size = new Size(200, 25),
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                };
                label.ApplyTheme();
                panel.Controls.Add(label);

                // Value
                var valueLabel = new BeepLabel
                {
                    Text = item.Value ?? "",
                    Location = new Point(230, y),
                    Size = new Size(panel.Width - 250, 25),
                    Font = new Font("Segoe UI", 9f)
                };
                valueLabel.ApplyTheme();
                panel.Controls.Add(valueLabel);

                y += 35;
            }

            return panel;
        }

        public override WizardStep GetStepConfig()
        {
            return new WizardStep
            {
                Key = _stepKey,
                Title = _stepTitle,
                Description = "Review your information before completing"
            };
        }

        /// <summary>
        /// Populate summary from wizard context
        /// </summary>
        public void PopulateFromContext(UserControl control, WizardContext context)
        {
            int y = 20;
            foreach (var item in _items)
            {
                var value = context.GetValue<object>(item.DataKey);
                var valueText = value?.ToString() ?? "";

                // Find and update value label
                foreach (Control ctrl in control.Controls)
                {
                    if (ctrl is BeepLabel label && label.Text.StartsWith(item.Label + ":"))
                    {
                        // Find the value label next to it
                        var valueLabel = control.Controls.Find($"Value_{item.DataKey}", false);
                        if (valueLabel.Length > 0 && valueLabel[0] is BeepLabel valLabel)
                        {
                            valLabel.Text = valueText;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Summary item definition
    /// </summary>
    public class SummaryItem
    {
        public string Label { get; set; } = string.Empty;
        public string DataKey { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
