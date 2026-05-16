using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Templates;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    /// <summary>
    /// Install wizard – Step 2: installation options.
    /// </summary>
    public partial class uc_NuggetsInstall_Step_Options : WizardStepTemplateBase
    {
        public uc_NuggetsInstall_Step_Options()
        {
            InitializeComponent();
        }

        public override void OnStepEnter(WizardContext context)
        {
            base.OnStepEnter(context);
            _chkLoadAfterInstall.CurrentValue = context.GetValue(NuggetWizardKeys.LoadAfterInstall, true);
            _chkSharedContext.CurrentValue    = context.GetValue(NuggetWizardKeys.SharedContext, true);
            _chkUseProcessHost.CurrentValue   = context.GetValue(NuggetWizardKeys.UseProcessHost, false);
            _txtInstallPath.Text              = context.GetValue(NuggetWizardKeys.InstallPath, string.Empty);
        }

        public override void OnStepLeave(WizardContext context)
        {
            context.SetValue(NuggetWizardKeys.LoadAfterInstall, _chkLoadAfterInstall.CurrentValue);
            context.SetValue(NuggetWizardKeys.SharedContext,    _chkSharedContext.CurrentValue);
            context.SetValue(NuggetWizardKeys.UseProcessHost,   _chkUseProcessHost.CurrentValue);
            context.SetValue(NuggetWizardKeys.InstallPath,      _txtInstallPath.Text.Trim());
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog { Description = "Select install folder" };
            if (dialog.ShowDialog(this) == DialogResult.OK)
                _txtInstallPath.Text = dialog.SelectedPath;
        }
    }
}
