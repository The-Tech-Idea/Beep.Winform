using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Tools;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    // Internal wizard step — not a standalone view; no [AddinAttribute] to prevent DI auto-discovery.
    public class uc_Nugget_Step_SelectVersionAndOptions : TemplateUserControl, IWizardStepContent
    {
        private readonly BeepLabel _lblPackage = new();
        private readonly BeepComboBox _cmbVersion = new();
        private readonly BeepCheckBoxBool _chkLoadNow = new();
        private readonly BeepCheckBoxBool _chkSharedContext = new();
        private readonly BeepCheckBoxBool _chkUseProcessHost = new();
        private readonly BeepTextBox _txtInstallPath = new();
        private readonly BeepButton _btnBrowseInstall = new();
        private readonly BeepLabel _status = new();

        public uc_Nugget_Step_SelectVersionAndOptions(IServiceProvider services)
            : base(services)
        {
            BuildUi();
        }

        public event EventHandler<StepValidationEventArgs>? ValidationStateChanged;
        public bool IsComplete => !string.IsNullOrWhiteSpace(_cmbVersion.SelectedItem?.Text);
        public string NextButtonText => string.Empty;

        public async void OnStepEnter(WizardContext context)
        {
            var packageId = context.GetValue(NuggetsWizardKeys.SelectedPackageId, string.Empty);
            _lblPackage.Text = string.IsNullOrWhiteSpace(packageId) ? "Package: (none selected)" : $"Package: {packageId}";

            var request = context.GetValue<NuggetInstallRequest?>(NuggetsWizardKeys.InstallRequest, null) ?? new NuggetInstallRequest();
            _chkLoadNow.CurrentValue = request.LoadAfterInstall;
            _chkSharedContext.CurrentValue = request.UseSingleSharedContext;
            _chkUseProcessHost.CurrentValue = request.UseProcessHost;
            _txtInstallPath.Text = request.AppInstallPath ?? string.Empty;

            await LoadVersionsAsync(packageId);
            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
            var request = context.GetValue<NuggetInstallRequest?>(NuggetsWizardKeys.InstallRequest, null) ?? new NuggetInstallRequest();
            request.Version = _cmbVersion.SelectedItem?.Text ?? string.Empty;
            request.LoadAfterInstall = _chkLoadNow.CurrentValue;
            request.UseSingleSharedContext = _chkSharedContext.CurrentValue;
            request.UseProcessHost = _chkUseProcessHost.CurrentValue;
            request.AppInstallPath = _txtInstallPath.Text?.Trim() ?? string.Empty;

            context.SetValue(NuggetsWizardKeys.SelectedPackageVersion, request.Version);
            context.SetValue(NuggetsWizardKeys.InstallRequest, request);
        }

        WizardValidationResult IWizardStepContent.Validate()
        {
            return IsComplete
                ? WizardValidationResult.Success()
                : WizardValidationResult.Error("Select a package version.");
        }

        public Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(((IWizardStepContent)this).Validate());
        }

        private void BuildUi()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            _lblPackage.Location = new System.Drawing.Point(10, 10);
            _lblPackage.Size = new System.Drawing.Size(700, 24);
            _lblPackage.Text = "Package:";

            var lblVersion = new BeepLabel { Location = new System.Drawing.Point(10, 42), Size = new System.Drawing.Size(58, 23), Text = "Version:" };
            _cmbVersion.Location = new System.Drawing.Point(74, 39);
            _cmbVersion.Size = new System.Drawing.Size(220, 27);
            _cmbVersion.SelectedItemChanged += (_, _) => RaiseValidationState();

            _chkLoadNow.Location = new System.Drawing.Point(10, 74);
            _chkLoadNow.Size = new System.Drawing.Size(140, 24);
            _chkLoadNow.Text = "Load After Install";
            _chkLoadNow.CurrentValue = true;

            _chkSharedContext.Location = new System.Drawing.Point(156, 74);
            _chkSharedContext.Size = new System.Drawing.Size(165, 24);
            _chkSharedContext.Text = "Use Shared Context";
            _chkSharedContext.CurrentValue = true;

            _chkUseProcessHost.Location = new System.Drawing.Point(327, 74);
            _chkUseProcessHost.Size = new System.Drawing.Size(150, 24);
            _chkUseProcessHost.Text = "Use Process Host";

            var lblInstallPath = new BeepLabel { Location = new System.Drawing.Point(10, 106), Size = new System.Drawing.Size(100, 23), Text = "Install Path:" };
            _txtInstallPath.Location = new System.Drawing.Point(110, 103);
            _txtInstallPath.Size = new System.Drawing.Size(510, 27);
            _txtInstallPath.PlaceholderText = "optional app install path";
            _btnBrowseInstall.Location = new System.Drawing.Point(626, 103);
            _btnBrowseInstall.Size = new System.Drawing.Size(84, 27);
            _btnBrowseInstall.Text = "Browse";
            _btnBrowseInstall.Click += (_, _) => BrowseInstallPath();

            _status.Location = new System.Drawing.Point(10, 140);
            _status.Size = new System.Drawing.Size(700, 24);
            _status.Text = "Configure install behavior.";

            panel.Controls.Add(_lblPackage);
            panel.Controls.Add(lblVersion);
            panel.Controls.Add(_cmbVersion);
            panel.Controls.Add(_chkLoadNow);
            panel.Controls.Add(_chkSharedContext);
            panel.Controls.Add(_chkUseProcessHost);
            panel.Controls.Add(lblInstallPath);
            panel.Controls.Add(_txtInstallPath);
            panel.Controls.Add(_btnBrowseInstall);
            panel.Controls.Add(_status);
            Controls.Add(panel);
        }

        private async Task LoadVersionsAsync(string packageId)
        {
            _cmbVersion.ListItems = new BindingList<SimpleItem>();
            if (string.IsNullOrWhiteSpace(packageId))
            {
                _status.Text = "Select a package in the previous step.";
                return;
            }

            _status.Text = "Loading versions...";
            try
            {
                var versions = await Editor.assemblyHandler.GetNuGetPackageVersionsAsync(packageId, includePrerelease: true);
                var items = new BindingList<SimpleItem>();
                foreach (var version in versions)
                {
                    items.Add(new SimpleItem { Text = version, Item = version });
                }

                _cmbVersion.ListItems = items;
                if (items.Count > 0)
                {
                    _cmbVersion.SelectedItem = items[0];
                }

                _status.Text = $"Loaded {items.Count} versions.";
            }
            catch (Exception ex)
            {
                _status.Text = $"Version lookup failed: {ex.Message}";
            }
        }

        private void BrowseInstallPath()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select application install folder"
            };
            if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
            {
                _txtInstallPath.Text = dialog.SelectedPath;
            }
        }

        private void RaiseValidationState()
        {
            var result = ((IWizardStepContent)this).Validate();
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(result.IsValid, result.ErrorMessage));
        }
    }
}
