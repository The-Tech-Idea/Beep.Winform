using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    // Internal wizard step — not a standalone view; no [AddinAttribute] to prevent DI auto-discovery.
    public class uc_Nugget_Step_ReviewAndInstall : TemplateUserControl, IWizardStepContent
    {
        private readonly BeepTextBox _txtSummary = new();
        private readonly BeepCheckBoxBool _chkRunOnFinish = new();
        private readonly BeepLabel _status = new();

        public uc_Nugget_Step_ReviewAndInstall(IServiceProvider services)
            : base(services)
        {
            BuildUi();
        }

        public event EventHandler<StepValidationEventArgs>? ValidationStateChanged;
        public bool IsComplete => true;
        public string NextButtonText => "Finish";

        public void OnStepEnter(WizardContext context)
        {
            var request = context.GetValue<NuggetInstallRequest?>(NuggetsWizardKeys.InstallRequest, null) ?? new NuggetInstallRequest();
            _chkRunOnFinish.CurrentValue = context.GetValue(NuggetsWizardKeys.RunInstallOnFinish, true);
            _txtSummary.Text = BuildSummary(request);
            _status.Text = "Review request and finish the wizard to execute install.";
            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
            context.SetValue(NuggetsWizardKeys.RunInstallOnFinish, _chkRunOnFinish.CurrentValue);
        }

        WizardValidationResult IWizardStepContent.Validate()
        {
            return WizardValidationResult.Success();
        }

        public Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(WizardValidationResult.Success());
        }

        private void BuildUi()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            _chkRunOnFinish.Location = new System.Drawing.Point(10, 10);
            _chkRunOnFinish.Size = new System.Drawing.Size(220, 24);
            _chkRunOnFinish.Text = "Run Install On Wizard Finish";
            _chkRunOnFinish.CurrentValue = true;

            _txtSummary.Location = new System.Drawing.Point(10, 40);
            _txtSummary.Size = new System.Drawing.Size(720, 360);
            _txtSummary.Multiline = true;
            _txtSummary.ReadOnly = true;
            _txtSummary.ScrollBars = ScrollBars.Vertical;

            _status.Location = new System.Drawing.Point(10, 408);
            _status.Size = new System.Drawing.Size(720, 24);
            _status.Text = string.Empty;

            panel.Controls.Add(_chkRunOnFinish);
            panel.Controls.Add(_txtSummary);
            panel.Controls.Add(_status);
            Controls.Add(panel);
        }

        private static string BuildSummary(NuggetInstallRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Nugget Install Review");
            sb.AppendLine("---------------------");
            sb.AppendLine($"Package: {request.PackageId}");
            sb.AppendLine($"Version: {request.Version}");
            sb.AppendLine($"Source : {string.Join(", ", request.Sources)}");
            sb.AppendLine($"Load After Install: {(request.LoadAfterInstall ? "Yes" : "No")}");
            sb.AppendLine($"Use Shared Context: {(request.UseSingleSharedContext ? "Yes" : "No")}");
            sb.AppendLine($"Use Process Host  : {(request.UseProcessHost ? "Yes" : "No")}");
            sb.AppendLine($"App Install Path  : {request.AppInstallPath}");
            sb.AppendLine();
            sb.AppendLine("On finish, the launcher executes install through Editor.assemblyHandler.");
            return sb.ToString();
        }

        private void RaiseValidationState()
        {
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(true));
        }
    }
}
