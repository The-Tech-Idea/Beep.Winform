using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepFieldFilter : BeepControl
    {
        private ComboBox cmbOperator;
        private Control inputFrom;
        private Control inputTo;
        private Label lblFieldName;

        public AppFilter Filter { get; private set; }

        public BeepFieldFilter(string fieldName, string fieldType)
        {
            InitializeControls(fieldName, fieldType);
        }

        private void InitializeControls(string fieldName, string fieldType)
        {
            Filter = new AppFilter
            {
                FieldName = fieldName,
                valueType = fieldType
            };

            lblFieldName = new Label
            {
                Text = fieldName,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            cmbOperator = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(lblFieldName.Right + 10, lblFieldName.Top),
                Width = 100
            };

            cmbOperator.Items.AddRange(new[] { "=", ">", "<", ">=", "<=", "LIKE", "BETWEEN" });
            cmbOperator.SelectedIndexChanged += CmbOperator_SelectedIndexChanged;

            Controls.Add(lblFieldName);
            Controls.Add(cmbOperator);

            CreateInputControls(fieldType);
        }

        private void CreateInputControls(string fieldType)
        {
            // Remove previous input controls
            if (inputFrom != null)
            {
                Controls.Remove(inputFrom);
            }
            if (inputTo != null)
            {
                Controls.Remove(inputTo);
            }

            // Default positions for input controls
            int inputX = cmbOperator.Right + 10;

            switch (fieldType)
            {
                case "System.DateTime":
                    inputFrom = new DateTimePicker
                    {
                        Location = new Point(inputX, lblFieldName.Top),
                        Width = 150
                    };
                    break;

                case "System.Int32":
                case "System.Decimal":
                case "System.Double":
                    inputFrom = new NumericUpDown
                    {
                        Location = new Point(inputX, lblFieldName.Top),
                        Width = 100
                    };
                    break;

                default:
                    inputFrom = new TextBox
                    {
                        Location = new Point(inputX, lblFieldName.Top),
                        Width = 200
                    };
                    break;
            }

            Controls.Add(inputFrom);

            if (cmbOperator.SelectedItem != null && cmbOperator.SelectedItem.ToString() == "BETWEEN")
            {
                inputTo = CreateSecondInput(inputFrom);
                Controls.Add(inputTo);
            }
        }

        private Control CreateSecondInput(Control inputFrom)
        {
            return inputFrom switch
            {
                DateTimePicker => new DateTimePicker
                {
                    Location = new Point(inputFrom.Right + 10, inputFrom.Top),
                    Width = 150
                },
                NumericUpDown => new NumericUpDown
                {
                    Location = new Point(inputFrom.Right + 10, inputFrom.Top),
                    Width = 100
                },
                _ => new TextBox
                {
                    Location = new Point(inputFrom.Right + 10, inputFrom.Top),
                    Width = 200
                }
            };
        }

        private void CmbOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbOperator.SelectedItem != null)
            {
                string selectedOperator = cmbOperator.SelectedItem.ToString();

                if (selectedOperator == "BETWEEN")
                {
                    if (inputTo == null)
                    {
                        inputTo = CreateSecondInput(inputFrom);
                        Controls.Add(inputTo);
                    }
                }
                else if (inputTo != null)
                {
                    Controls.Remove(inputTo);
                    inputTo = null;
                }

                Invalidate();
            }
        }

        public AppFilter GetFilter()
        {
            Filter.Operator = cmbOperator.SelectedItem?.ToString();
            Filter.FilterValue = GetValueFromControl(inputFrom)?.ToString();
            Filter.FilterValue1 = inputTo != null ? GetValueFromControl(inputTo)?.ToString() : null;

            return Filter;
        }

        private object GetValueFromControl(Control control)
        {
            return control switch
            {
                TextBox textBox => textBox.Text,
                NumericUpDown numericUpDown => numericUpDown.Value,
                DateTimePicker dateTimePicker => dateTimePicker.Value,
                _ => null
            };
        }
    }
}
