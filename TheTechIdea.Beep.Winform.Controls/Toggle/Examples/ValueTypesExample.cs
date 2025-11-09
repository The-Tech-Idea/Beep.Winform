using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Toggle;

namespace TheTechIdea.Beep.Winform.Controls.Examples
{
    /// <summary>
    /// Example demonstrating all BeepToggle value types
    /// </summary>
    public class BeepToggleValueTypesExample : Form
    {
        public BeepToggleValueTypesExample()
        {
            Text = "BeepToggle Value Types Demo";
            Size = new Size(500, 700);
            AutoScroll = true;

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                AutoScroll = true
            };

            // 1. Boolean Toggle
            var booleanGroup = CreateToggleGroup(
                "1. Boolean Type",
                BeepToggle.ToggleValueType.Boolean,
                true,
                false,
                "Enable Feature"
            );
            panel.Controls.Add(booleanGroup);

            // 2. Yes/No Toggle
            var yesNoGroup = CreateToggleGroup(
                "2. Yes/No Type",
                BeepToggle.ToggleValueType.YesNo,
                "Yes",
                "No",
                "Accept Terms"
            );
            panel.Controls.Add(yesNoGroup);

            // 3. On/Off Toggle
            var onOffGroup = CreateToggleGroup(
                "3. On/Off Type",
                BeepToggle.ToggleValueType.OnOff,
                "On",
                "Off",
                "Power"
            );
            panel.Controls.Add(onOffGroup);

            // 4. Enabled/Disabled Toggle
            var enabledGroup = CreateToggleGroup(
                "4. Enabled/Disabled Type",
                BeepToggle.ToggleValueType.EnabledDisabled,
                "Enabled",
                "Disabled",
                "Service Status"
            );
            panel.Controls.Add(enabledGroup);

            // 5. Active/Inactive Toggle
            var activeGroup = CreateToggleGroup(
                "5. Active/Inactive Type",
                BeepToggle.ToggleValueType.ActiveInactive,
                "Active",
                "Inactive",
                "Account Status"
            );
            panel.Controls.Add(activeGroup);

            // 6. Numeric Toggle (0/1)
            var numericGroup = CreateToggleGroup(
                "6. Numeric Type (0/1)",
                BeepToggle.ToggleValueType.Numeric,
                1,
                0,
                "Bit Field"
            );
            panel.Controls.Add(numericGroup);

            // 7. Custom String Toggle
            var customGroup = CreateCustomStringToggle();
            panel.Controls.Add(customGroup);

            // 8. Custom Numeric Toggle
            var customNumericGroup = CreateCustomNumericToggle();
            panel.Controls.Add(customNumericGroup);

            Controls.Add(panel);
        }

        private GroupBox CreateToggleGroup(
            string title,
            BeepToggle.ToggleValueType valueType,
            object onValue,
            object offValue,
            string label)
        {
            var group = new GroupBox
            {
                Text = title,
                Width = 450,
                Height = 120,
                Padding = new Padding(10)
            };

            var toggle = new BeepToggle
            {
                Location = new Point(20, 30),
                Size = new Size(100, 40),
                ValueType = valueType,
                OnValue = onValue,
                OffValue = offValue,
                ToggleStyle = ToggleStyle.MaterialPill
            };

            var labelControl = new Label
            {
                Text = label,
                Location = new Point(130, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            var valueLabel = new Label
            {
                Location = new Point(20, 80),
                AutoSize = true,
                ForeColor = Color.Blue,
                Font = new Font("Consolas", 9F)
            };

            // Update value label
            Action updateValueLabel = () =>
            {
                valueLabel.Text = $"Current Value: {toggle.Value} (Type: {toggle.Value?.GetType().Name ?? "null"})";
            };
            updateValueLabel();

            toggle.ValueChanged += (s, e) => updateValueLabel();

            group.Controls.AddRange(new Control[] { toggle, labelControl, valueLabel });

            return group;
        }

        private GroupBox CreateCustomStringToggle()
        {
            var group = new GroupBox
            {
                Text = "7. Custom String Type",
                Width = 450,
                Height = 120,
                Padding = new Padding(10)
            };

            var toggle = new BeepToggle
            {
                Location = new Point(20, 30),
                Size = new Size(100, 40),
                ValueType = BeepToggle.ToggleValueType.String,
                OnValue = "PREMIUM",
                OffValue = "FREE",
                OnText = "Premium",
                OffText = "Free",
                OnColor = Color.Gold,
                OffColor = Color.Silver,
                ToggleStyle = ToggleStyle.ButtonStyle
            };

            var label = new Label
            {
                Text = "Subscription Plan",
                Location = new Point(130, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            var valueLabel = new Label
            {
                Location = new Point(20, 80),
                AutoSize = true,
                ForeColor = Color.Blue,
                Font = new Font("Consolas", 9F)
            };

            Action updateValueLabel = () =>
            {
                valueLabel.Text = $"Plan: {toggle.Value}";
            };
            updateValueLabel();

            toggle.ValueChanged += (s, e) => updateValueLabel();

            group.Controls.AddRange(new Control[] { toggle, label, valueLabel });

            return group;
        }

        private GroupBox CreateCustomNumericToggle()
        {
            var group = new GroupBox
            {
                Text = "8. Custom Numeric Type (100/0)",
                Width = 450,
                Height = 120,
                Padding = new Padding(10)
            };

            var toggle = new BeepToggle
            {
                Location = new Point(20, 30),
                Size = new Size(100, 40),
                ValueType = BeepToggle.ToggleValueType.Numeric,
                OnValue = 100,
                OffValue = 0,
                OnText = "100%",
                OffText = "0%",
                OnColor = Color.LimeGreen,
                OffColor = Color.Gray,
                ToggleStyle = ToggleStyle.RectangularLabeled
            };

            var label = new Label
            {
                Text = "Volume",
                Location = new Point(130, 35),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            var valueLabel = new Label
            {
                Location = new Point(20, 80),
                AutoSize = true,
                ForeColor = Color.Blue,
                Font = new Font("Consolas", 9F)
            };

            Action updateValueLabel = () =>
            {
                int? volume = toggle.GetValue<int>();
                valueLabel.Text = $"Volume Level: {volume ?? 0}%";
            };
            updateValueLabel();

            toggle.ValueChanged += (s, e) => updateValueLabel();

            group.Controls.AddRange(new Control[] { toggle, label, valueLabel });

            return group;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BeepToggleValueTypesExample());
        }
    }
}
