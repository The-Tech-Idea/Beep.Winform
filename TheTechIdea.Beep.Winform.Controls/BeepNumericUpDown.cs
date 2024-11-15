using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepNumericUpDown : BeepControl
    {
        private NumericUpDown numericUpDownControl;

        public BeepNumericUpDown()
        {
            InitializeNumericUpDown();
        }

        private void InitializeNumericUpDown()
        {
            numericUpDownControl = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Minimum = MinimumValue,
                Maximum = MaximumValue,
                Increment = IncrementValue,
                Value = DefaultValue
            };

            numericUpDownControl.ValueChanged += (s, e) => Text = numericUpDownControl.Value.ToString();
            Controls.Add(numericUpDownControl);
        }

        // Properties to expose NumericUpDown control settings
        [Browsable(true)]
        [Category("Numeric Settings")]
        public decimal MinimumValue
        {
            get => numericUpDownControl.Minimum;
            set => numericUpDownControl.Minimum = value;
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        public decimal MaximumValue
        {
            get => numericUpDownControl.Maximum;
            set => numericUpDownControl.Maximum = value;
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        public decimal IncrementValue
        {
            get => numericUpDownControl.Increment;
            set => numericUpDownControl.Increment = value;
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        public decimal DefaultValue
        {
            get => numericUpDownControl.Value;
            set => numericUpDownControl.Value = value;
        }

        // Override Text property to sync with numeric value
        [Browsable(true)]
        [Category("Appearance")]
        public override string Text
        {
            get => numericUpDownControl.Value.ToString();
            set
            {
                if (decimal.TryParse(value, out decimal result))
                {
                    numericUpDownControl.Value = result;
                }
            }
        }

        // Apply theme for customization in line with BeepControl’s theming
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            numericUpDownControl.ForeColor = _currentTheme.PrimaryTextColor;
            numericUpDownControl.BackColor = _currentTheme.TextBoxBackColor;
            numericUpDownControl.BorderStyle = BorderStyle.None;
        }
    }
}
