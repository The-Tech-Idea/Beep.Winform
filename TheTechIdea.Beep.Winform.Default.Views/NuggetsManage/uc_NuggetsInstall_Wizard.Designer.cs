using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Buttons;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    partial class uc_NuggetsInstall_Wizard
    {
        private Panel _pnlSteps;
        private Label _lblStep1;
        private Label _lblStep2;
        private Label _lblStep3;
        private Panel _pnlContent;
        private Panel _pnlButtons;
        private BeepButton _btnBack;
        private BeepButton _btnNext;
        private BeepButton _btnCancel;

        private void InitializeComponent()
        {
            // Step indicator
            _pnlSteps = new Panel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(24, 12, 24, 0) };
            _lblStep1 = new Label { Text = "1. Version & Source", AutoSize = true, Location = new Point(24, 16) };
            _lblStep2 = new Label { Text = "2. Options", AutoSize = true, Location = new Point(260, 16) };
            _lblStep3 = new Label { Text = "3. Install", AutoSize = true, Location = new Point(480, 16) };
            _pnlSteps.Controls.Add(_lblStep1);
            _pnlSteps.Controls.Add(new Label { Text = "→", AutoSize = true, Location = new Point(212, 14), Font = new Font(Font.FontFamily, 12f, FontStyle.Bold) });
            _pnlSteps.Controls.Add(_lblStep2);
            _pnlSteps.Controls.Add(new Label { Text = "→", AutoSize = true, Location = new Point(444, 14), Font = new Font(Font.FontFamily, 12f, FontStyle.Bold) });
            _pnlSteps.Controls.Add(_lblStep3);

            // Content
            _pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };

            // Buttons
            _pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(16, 10, 16, 10) };
            _btnBack = new BeepButton { Text = "Back", Size = new Size(90, 32), Anchor = AnchorStyles.Right };
            _btnBack.Click += BtnBack_Click;
            _btnNext = new BeepButton { Text = "Next", Size = new Size(90, 32), Anchor = AnchorStyles.Right };
            _btnNext.Click += BtnNext_Click;
            _btnCancel = new BeepButton { Text = "Cancel", Size = new Size(90, 32), Anchor = AnchorStyles.Left };
            _btnCancel.Click += BtnCancel_Click;
            _pnlButtons.Controls.Add(_btnNext);
            _pnlButtons.Controls.Add(_btnBack);
            _pnlButtons.Controls.Add(_btnCancel);

            Controls.Add(_pnlContent);
            Controls.Add(_pnlButtons);
            Controls.Add(_pnlSteps);
        }

        private void LayoutButtons()
        {
            var right = _pnlButtons.ClientSize.Width - _pnlButtons.Padding.Right;
            var cy = (_pnlButtons.Height - _btnNext.Height) / 2;
            _btnNext.Location = new Point(right - _btnNext.Width, cy);
            _btnBack.Location = new Point(_btnNext.Left - _btnBack.Width - 8, cy);
            _btnCancel.Location = new Point(_pnlButtons.Padding.Left, cy);
        }
    }
}
