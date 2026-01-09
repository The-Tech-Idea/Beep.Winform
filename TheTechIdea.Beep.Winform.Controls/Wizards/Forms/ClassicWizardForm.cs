using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Classic wizard form with left sidebar (Image 1 Style)
    /// Traditional installation wizard design
    /// </summary>
    public class ClassicWizardForm : BaseWizardForm
    {
        private BeepPanel _sidebarPanel;
        private BeepListBox _lstSteps;

        public ClassicWizardForm(WizardInstance instance) : base(instance)
        {
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Sidebar with steps list
            _sidebarPanel = new BeepPanel
            {
                Dock = DockStyle.Left,
                Width = 250,
                Padding = new Padding(20),
                ShowTitle = false,  // No title for sidebar panel
                ShowTitleLine = false
            };
            _sidebarPanel.ApplyTheme();

            _lstSteps = new BeepListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None
            };
            _lstSteps.ApplyTheme();

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

            if (_instance.CurrentStepIndex >= 0 && _instance.CurrentStepIndex < _lstSteps.Items.Count)
            {
                _lstSteps.SelectedIndex = _instance.CurrentStepIndex;
            }
        }
    }
}
