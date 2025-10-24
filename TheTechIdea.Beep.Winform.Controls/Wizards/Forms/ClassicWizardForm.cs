using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Classic wizard form with left sidebar (Image 1 Style)
    /// Traditional installation wizard design
    /// </summary>
    public class ClassicWizardForm : BaseWizardForm
    {
        private Panel _sidebarPanel;
        private ListBox _lstSteps;

        public ClassicWizardForm(WizardInstance instance) : base(instance)
        {
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Sidebar with steps list
            _sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(0, 150, 215), // Blue sidebar
                Padding = new Padding(20)
            };

            _lstSteps = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(0, 150, 215),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10f),
                SelectionMode = SelectionMode.None
            };

            _sidebarPanel.Controls.Add(_lstSteps);

            // Adjust content panel to account for sidebar
            _contentPanel.Padding = new Padding(30);
        }

        protected override void LayoutControls()
        {
            base.LayoutControls();
            Controls.Add(_sidebarPanel);
            _sidebarPanel.BringToFront();
        }

        protected override void UpdateHeader()
        {
            // Update steps list
            _lstSteps.Items.Clear();

            for (int i = 0; i < _instance.Config.Steps.Count; i++)
            {
                var step = _instance.Config.Steps[i];
                var prefix = i < _instance.CurrentStepIndex ? "✓ " :
                             i == _instance.CurrentStepIndex ? "▶ " : "  ";
                _lstSteps.Items.Add($"{prefix}{step.Title}");
            }

            _lstSteps.SelectedIndex = _instance.CurrentStepIndex;
        }
    }
}
