using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public partial class uc_NuggetsInstall_Wizard : Form
    {
        private readonly List<IWizardStepContent> _steps;
        private readonly WizardContext _context;
        private int _stepIdx;
        private bool _hasEntered;

        public event Action<WizardContext>? Complete;

        public uc_NuggetsInstall_Wizard(string title, WizardContext context, params IWizardStepContent[] steps)
        {
            _steps = steps.ToList();
            _context = context ?? new WizardContext();

            InitializeComponent();
            LayoutButtons();
            Text = title;
            Size = new Size(700, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;
            KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Cancel(); };

            UpdateIndicator();
            Go(0);
        }

        public DialogResult ShowWizard(IWin32Window owner) => ShowDialog(owner);

        private void Go(int i)
        {
            if (i < 0 || i >= _steps.Count) return;

            if (_hasEntered)
            {
                _steps[_stepIdx].OnStepLeave(_context);
                _steps[_stepIdx].ValidationStateChanged -= OnValidChanged;
            }

            _stepIdx = i;
            _hasEntered = true;

            _pnlContent.Controls.Clear();
            var step = _steps[_stepIdx];
            step.ValidationStateChanged += OnValidChanged;
            if (step is Control c) { c.Dock = DockStyle.Fill; _pnlContent.Controls.Add(c); }
            step.OnStepEnter(_context);

            _btnBack.Enabled = i > 0;
            var last = i == _steps.Count - 1;
            _btnNext.Text = !string.IsNullOrEmpty(step.NextButtonText) ? step.NextButtonText : last ? "Finish" : "Next";
            _btnNext.Enabled = last || step.IsComplete;
            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            _lblStep1.ForeColor = _stepIdx == 0 ? SystemColors.Highlight : SystemColors.ControlText;
            _lblStep2.ForeColor = _stepIdx == 1 ? SystemColors.Highlight : SystemColors.ControlText;
            _lblStep3.ForeColor = _stepIdx == 2 ? SystemColors.Highlight : SystemColors.ControlText;
        }

        private async void BtnNext_Click(object? s, EventArgs e)
        {
            var step = _steps[_stepIdx];
            var vr = await step.ValidateAsync();
            if (!vr.IsValid) { MessageBox.Show(this, vr.ErrorMessage, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (_stepIdx == _steps.Count - 1)
            {
                step.OnStepLeave(_context);
                Complete?.Invoke(_context);
                DialogResult = DialogResult.OK;
                Close();
            }
            else Go(_stepIdx + 1);
        }

        private void BtnBack_Click(object? s, EventArgs e) { if (_stepIdx > 0) Go(_stepIdx - 1); }
        private void BtnCancel_Click(object? s, EventArgs e) => Cancel();

        private void Cancel()
        {
            if (MessageBox.Show(this, "Cancel installation?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            { DialogResult = DialogResult.Cancel; Close(); }
        }

        private void OnValidChanged(object? s, StepValidationEventArgs e)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnValidChanged(s, e))); return; }
            _btnNext.Enabled = _stepIdx == _steps.Count - 1 || e.IsValid;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (var st in _steps) st.ValidationStateChanged -= OnValidChanged;
            base.OnFormClosing(e);
        }
    }
}
