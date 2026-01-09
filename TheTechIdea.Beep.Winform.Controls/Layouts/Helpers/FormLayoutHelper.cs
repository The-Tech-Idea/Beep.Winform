using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating form layouts.
    /// Creates a standard form layout with labels and input fields arranged in a table.
    /// </summary>
    public static class FormLayoutHelper
    {
        /// <summary>
        /// Builds a form layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="fieldCount">Number of form fields to create. Default is 5.</param>
        /// <returns>The TableLayoutPanel containing the form layout.</returns>
        public static Control Build(Control parent, int fieldCount = 5)
        {
            return Build(parent, fieldCount, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a form layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="fieldCount">Number of form fields to create. Must be greater than 0.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The TableLayoutPanel containing the form layout.</returns>
        public static Control Build(Control parent, int fieldCount, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (fieldCount <= 0)
                throw new ArgumentException("FieldCount must be greater than 0", nameof(fieldCount));

            options = options ?? LayoutOptions.Default;

            var table = new TableLayoutPanel 
            { 
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = fieldCount + 1, // +1 for header row
                BackColor = BaseLayoutHelper.GetBackgroundColor(options)
            };

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f)); // Label column
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70f)); // Input column

            // Header row
            var headerLabel = BaseLayoutHelper.CreateStyledLabel("Form Fields", options, ContentAlignment.MiddleCenter);
            headerLabel.Font = BaseLayoutHelper.GetFont(options, 14f);
            headerLabel.Font = new Font(headerLabel.Font, FontStyle.Bold);
            headerLabel.Dock = DockStyle.Fill;
            table.Controls.Add(headerLabel, 0, 0);
            table.SetColumnSpan(headerLabel, 2);
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));

            // Form fields
            string[] fieldLabels = { "Name", "Email", "Phone", "Address", "City" };
            for (int i = 0; i < fieldCount; i++)
            {
                string labelText = i < fieldLabels.Length ? fieldLabels[i] : $"Field {i + 1}";
                
                var label = BaseLayoutHelper.CreateStyledLabel($"{labelText}:", options, ContentAlignment.MiddleRight);
                label.Dock = DockStyle.Fill;
                label.Padding = new Padding(0, 0, 10, 0);

                var textBox = BaseLayoutHelper.CreateBeepTextBox("", options);
                textBox.Dock = DockStyle.Fill;
                textBox.Margin = new Padding(0, options.Spacing, 0, options.Spacing);

                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 35f));
                table.Controls.Add(label, 0, i + 1);
                table.Controls.Add(textBox, 1, i + 1);
            }

            // Button row
            table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));

            var buttonPanel = new Panel { Dock = DockStyle.Fill };
            buttonPanel.Controls.Add(new Panel { Dock = DockStyle.Fill }); // Spacer

            var submitButton = BaseLayoutHelper.CreateBeepButton("Submit", options);
            submitButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            submitButton.Margin = new Padding(0, 10, 10, 10);
            buttonPanel.Controls.Add(submitButton);

            table.Controls.Add(buttonPanel, 0, fieldCount + 1);
            table.SetColumnSpan(buttonPanel, 2);

            parent.Controls.Add(table);
            return table;
        }
    }
}
