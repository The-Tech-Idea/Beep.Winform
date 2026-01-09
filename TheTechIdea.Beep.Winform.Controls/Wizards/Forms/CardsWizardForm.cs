using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Card-based wizard form (Image 6 Style)
    /// Shows selectable cards for each step
    /// </summary>
    public class CardsWizardForm : BaseWizardForm
    {
        public CardsWizardForm(WizardInstance instance) : base(instance)
        {
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Cards wizard doesn't need standard header
            _headerPanel.Height = 60;
            _headerPanel.Controls.Clear();

            var lblTitle = new BeepLabel
            {
                Text = _instance.Config.Title,
                Location = new Point(20, 20),
                AutoSize = true
            };
            lblTitle.ApplyTheme();

            _headerPanel.Controls.Add(lblTitle);
        }

        protected override void UpdateHeader()
        {
            // Card Style doesn't need header updates
        }
    }
}
