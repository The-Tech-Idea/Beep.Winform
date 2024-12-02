using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;


namespace TheTechIdea.Beep.Winform.Controls
{

    public class BeepQueryandFilter : BeepControl
    {
        private TableLayoutPanel tableLayoutPanel;
        private List<AppFilter> Filters = new List<AppFilter>();
        public event EventHandler SubmitClicked;

        public BeepQueryandFilter()
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            // Initialize TableLayoutPanel
            tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3, // Label, Input, (Optional: Second Input for ranges)
                AutoSize = true,
                AutoScroll = true
            };
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // For Labels
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // For Inputs
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // For Ranges (optional)
            Controls.Add(tableLayoutPanel);

            // Add Submit Button
            var submitButton = new Button
            {
                Text = "Submit",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            submitButton.Click += (s, e) => SubmitClicked?.Invoke(this, EventArgs.Empty);

            tableLayoutPanel.Controls.Add(submitButton, 1, 0); // Add Submit Button to layout
            tableLayoutPanel.SetColumnSpan(submitButton, 2);  // Span across input columns
        }

        public void CreateQueryControls(EntityStructure entityStructure)
        {
            Filters.Clear();
            tableLayoutPanel.Controls.Clear(); // Clear previous controls

            foreach (var field in entityStructure.Fields)
            {
                // Create Label
                var label = new Label
                {
                    Text = field.fieldname,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left
                };
                tableLayoutPanel.Controls.Add(label);

                // Create Input Control(s)
                Control controlFrom = CreateInputControl(field, true);
                tableLayoutPanel.Controls.Add(controlFrom);

                Control controlTo = null;
                if (IsRangeSupported(field))
                {
                    controlTo = CreateInputControl(field, false);
                    tableLayoutPanel.Controls.Add(controlTo);
                }
                else
                {
                    tableLayoutPanel.SetColumnSpan(controlFrom, 2); // Span if no range
                }

                // Create Filter object and tag controls
                var filter = new AppFilter
                {
                    FieldName = field.fieldname,
                    valueType = field.fieldtype
                };
                Filters.Add(filter);
                if (controlFrom != null) controlFrom.Tag = (filter, true); // "From" input
                if (controlTo != null) controlTo.Tag = (filter, false);   // "To" input
            }

            // Add Submit Button
            var submitButton = new Button
            {
                Text = "Submit",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            submitButton.Click += (s, e) => SubmitClicked?.Invoke(this, EventArgs.Empty);

            tableLayoutPanel.Controls.Add(submitButton, 1, tableLayoutPanel.RowCount); // Add Submit Button to layout
            tableLayoutPanel.SetColumnSpan(submitButton, 2);  // Span across input columns
        }

        private Control CreateInputControl(EntityField field, bool isFrom)
        {
            switch (Type.GetType(field.fieldtype))
            {
                case Type type when type == typeof(DateTime):
                    return new DateTimePicker
                    {
                        Format = DateTimePickerFormat.Short,
                        Width = 100,
                        Anchor = AnchorStyles.Left
                    };

                case Type type when type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(float):
                    return new NumericUpDown
                    {
                        Minimum = decimal.MinValue,
                        Maximum = decimal.MaxValue,
                        DecimalPlaces = (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) ? 2 : 0,
                        Width = 100,
                        Anchor = AnchorStyles.Left
                    };

                case Type type when type == typeof(bool):
                    return new CheckBox
                    {
                        Text = "",
                        Width = 50,
                        Anchor = AnchorStyles.Left
                    };

                default:
                    return new TextBox
                    {
                        Width = 150,
                        Anchor = AnchorStyles.Left
                    };
            }
        }

        private bool IsRangeSupported(EntityField field)
        {
            // Allow ranges for numeric and date fields
            var type = Type.GetType(field.fieldtype);
            return type == typeof(DateTime) || type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }

        public List<AppFilter> GetFilters()
        {
            foreach (Control control in tableLayoutPanel.Controls)
            {
                if (control.Tag is ValueTuple<AppFilter, bool> tagInfo)
                {
                    var (filter, isFrom) = tagInfo;

                    if (control is TextBox textBox)
                    {
                        if (isFrom)
                            filter.FilterValue = textBox.Text;
                        else
                            filter.FilterValue1 = textBox.Text;
                    }
                    else if (control is NumericUpDown numericUpDown)
                    {
                        if (isFrom)
                            filter.FilterValue = numericUpDown.Value.ToString(); // Convert decimal to string
                        else
                            filter.FilterValue1 = numericUpDown.Value.ToString(); // Convert decimal to string
                    }
                    else if (control is DateTimePicker dateTimePicker)
                    {
                        if (isFrom)
                            filter.FilterValue = dateTimePicker.Value.ToString("yyyy-MM-dd"); // Convert DateTime to string
                        else
                            filter.FilterValue1 = dateTimePicker.Value.ToString("yyyy-MM-dd"); // Convert DateTime to string
                    }
                }
            }

            return Filters;
        }
    }


}