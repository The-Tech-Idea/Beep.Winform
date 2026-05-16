using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.NuGet;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Templates;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    /// <summary>
    /// Install wizard – Step 1: choose version and source.
    /// </summary>
    public partial class uc_NuggetsInstall_Step_VersionSource : WizardStepTemplateBase
    {
        private NuggetsManageService? _service;

        public uc_NuggetsInstall_Step_VersionSource()
        {
            InitializeComponent();
        }

        public override bool IsComplete
            => _cmbVersion.SelectedItem != null && _cmbSource.SelectedItem != null;

        public override void OnStepEnter(WizardContext context)
        {
            base.OnStepEnter(context);

            _service = context.GetValue<NuggetsManageService>(NuggetWizardKeys.Service, null!);
            var packageId = context.GetValue<string>(NuggetWizardKeys.PackageId, string.Empty);
            var prerelease = context.GetValue<bool>(NuggetWizardKeys.IncludePrerelease, false);

            _lblPackageId.Text = $"Package:  {packageId}";
            _chkPrerelease.CurrentValue = prerelease;

            LoadSources();
            _ = LoadVersionsAsync(packageId, prerelease);
        }

        public override void OnStepLeave(WizardContext context)
        {
            context.SetValue(NuggetWizardKeys.SelectedVersion, _cmbVersion.SelectedItem?.Text ?? string.Empty);
            context.SetValue(NuggetWizardKeys.SelectedSourceUrl, _cmbSource.SelectedItem?.Item?.ToString() ?? string.Empty);
            context.SetValue(NuggetWizardKeys.IncludePrerelease, _chkPrerelease.CurrentValue);
        }

        public override WizardValidationResult Validate()
        {
            if (_cmbVersion.SelectedItem == null)
                return WizardValidationResult.Error("Please select a version.");
            if (_cmbSource.SelectedItem == null)
                return WizardValidationResult.Error("Please select a source.");
            return WizardValidationResult.Success();
        }

        private void LoadSources()
        {
            if (_service == null) return;
            var sources = _service.GetAllSources().Where(s => s.IsEnabled).ToList();
            var items = new BindingList<SimpleItem>(
                sources.Select(s => new SimpleItem { Text = s.Name, Item = s.Url }).ToList());
            _cmbSource.ListItems = items;
            if (items.Count > 0) _cmbSource.SelectedItem = items[0];
        }

        private async Task LoadVersionsAsync(string packageId, bool prerelease)
        {
            if (_service == null || string.IsNullOrWhiteSpace(packageId)) return;
            try
            {
                var versions = await _service.GetVersionsAsync(packageId, prerelease);
                var items = new BindingList<SimpleItem>(
                    versions.Select(v => new SimpleItem { Text = v, Item = v }).ToList());
                _cmbVersion.ListItems = items;
                if (items.Count > 0) _cmbVersion.SelectedItem = items[0];
                RaiseValidationStateChanged(IsComplete);
            }
            catch (Exception ex)
            {
                RaiseValidationStateChanged(false, ex.Message);
            }
        }

        private void ChkPrerelease_StateChanged(object? sender, EventArgs e)
        {
            var packageId = Context?.GetValue<string>(NuggetWizardKeys.PackageId, string.Empty) ?? string.Empty;
            _ = LoadVersionsAsync(packageId, _chkPrerelease.CurrentValue);
        }
    }
}

