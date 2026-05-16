using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Templates;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    /// <summary>
    /// Install wizard – Step 3: shows a summary then runs the install.
    /// </summary>
    public partial class uc_NuggetsInstall_Step_Run : WizardStepTemplateBase
    {
        private NuggetsManageService? _service;
        private bool _installSucceeded;

        public uc_NuggetsInstall_Step_Run()
        {
            InitializeComponent();
        }

        public override bool IsComplete => _installSucceeded;
        public override string NextButtonText => "Install";

        public override void OnStepEnter(WizardContext context)
        {
            base.OnStepEnter(context);
            _service = context.GetValue<NuggetsManageService>(NuggetWizardKeys.Service, null!);
            _installSucceeded = false;

            var packageId = context.GetValue(NuggetWizardKeys.PackageId,       string.Empty);
            var version   = context.GetValue(NuggetWizardKeys.SelectedVersion,  string.Empty);
            var source    = context.GetValue(NuggetWizardKeys.SelectedSourceUrl, string.Empty);
            var load      = context.GetValue(NuggetWizardKeys.LoadAfterInstall, true);
            var shared    = context.GetValue(NuggetWizardKeys.SharedContext,    true);
            var path      = context.GetValue(NuggetWizardKeys.InstallPath,      string.Empty);

            _lblSummary.Text =
                $"Package:   {packageId}\n" +
                $"Version:   {version}\n" +
                $"Source:    {source}\n" +
                $"Load now:  {(load ? "Yes" : "No")}\n" +
                $"Shared ctx:{(shared ? "Yes" : "No")}\n" +
                (string.IsNullOrWhiteSpace(path) ? string.Empty : $"Path:      {path}");

            _lblStatus.Text = "Click Install to proceed.";
        }

        public override async Task<WizardValidationResult> ValidateAsync()
        {
            if (_installSucceeded) return WizardValidationResult.Success();

            return await RunInstallAsync();
        }

        private async Task<WizardValidationResult> RunInstallAsync()
        {
            if (_service == null)
                return WizardValidationResult.Error("Service not available.");

            var context = Context!;
            var request = new NuggetInstallRequest
            {
                PackageId             = context.GetValue(NuggetWizardKeys.PackageId,        string.Empty),
                Version               = context.GetValue(NuggetWizardKeys.SelectedVersion,  string.Empty),
                Sources               = new List<string> { context.GetValue(NuggetWizardKeys.SelectedSourceUrl, string.Empty) },
                LoadAfterInstall      = context.GetValue(NuggetWizardKeys.LoadAfterInstall, true),
                UseSingleSharedContext = context.GetValue(NuggetWizardKeys.SharedContext,   true),
                UseProcessHost        = context.GetValue(NuggetWizardKeys.UseProcessHost,   false),
                AppInstallPath        = context.GetValue(NuggetWizardKeys.InstallPath,      string.Empty)
            };

            _progressBar.Visible = true;
            _lblStatus.Text = $"Installing {request.PackageId} {request.Version}…";

            var progress = new Progress<string>(msg => _lblStatus.Text = msg);
            try
            {
                var result = await _service.InstallAsync(request, progress);
                _installSucceeded = result.Success;
                _lblStatus.Text = result.Message;
                context.SetValue(NuggetWizardKeys.InstallResult, result);
                RaiseValidationStateChanged(result.Success, result.Message);
                return result.Success
                    ? WizardValidationResult.Success()
                    : WizardValidationResult.Error(result.Message);
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Install failed: {ex.Message}";
                RaiseValidationStateChanged(false, ex.Message);
                return WizardValidationResult.Error(ex.Message);
            }
            finally
            {
                _progressBar.Visible = false;
            }
        }
    }
}

