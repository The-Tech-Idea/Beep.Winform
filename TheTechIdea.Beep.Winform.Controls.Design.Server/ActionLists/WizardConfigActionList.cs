using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Action list for WizardConfig smart tags
    /// Provides quick actions for configuring wizard steps and behavior
    /// </summary>
    public class WizardConfigActionList : DesignerActionList
    {
        private readonly WizardConfig _config;
        private readonly IDesignerHost? _designerHost;
        private readonly IComponent _component;

        public WizardConfigActionList(IComponent component, WizardConfig config, IDesignerHost? designerHost = null)
            : base(component)
        {
            _component = component ?? throw new ArgumentNullException(nameof(component));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _designerHost = designerHost;
        }

        #region Properties (for smart tags)

        [Category("Wizard")]
        [Description("Title of the wizard")]
        public string Title
        {
            get => _config.Title;
            set
            {
                SetProperty(nameof(Title), value);
            }
        }

        [Category("Wizard")]
        [Description("Description of the wizard")]
        public string Description
        {
            get => _config.Description;
            set
            {
                SetProperty(nameof(Description), value);
            }
        }

        [Category("Wizard")]
        [Description("Visual style of the wizard")]
        public WizardStyle Style
        {
            get => _config.Style;
            set
            {
                SetProperty(nameof(Style), value);
            }
        }

        [Category("Wizard")]
        [Description("Show progress bar")]
        public bool ShowProgressBar
        {
            get => _config.ShowProgressBar;
            set
            {
                SetProperty(nameof(ShowProgressBar), value);
            }
        }

        [Category("Wizard")]
        [Description("Allow cancel")]
        public bool AllowCancel
        {
            get => _config.AllowCancel;
            set
            {
                SetProperty(nameof(AllowCancel), value);
            }
        }

        [Category("Wizard")]
        [Description("Allow back navigation")]
        public bool AllowBack
        {
            get => _config.AllowBack;
            set
            {
                SetProperty(nameof(AllowBack), value);
            }
        }

        [Category("Wizard")]
        [Description("Number of steps")]
        [ReadOnly(true)]
        public int StepCount => _config.Steps?.Count ?? 0;

        #endregion

        #region Actions

        /// <summary>
        /// Add a new step to the wizard
        /// </summary>
        public void AddStep()
        {
            var step = new WizardStep
            {
                Key = Guid.NewGuid().ToString(),
                Title = $"Step {(_config.Steps.Count + 1)}",
                Description = "New wizard step",
                IsOptional = false,
                IsCompleted = false
            };

            _config.Steps.Add(step);
            OnComponentChanged();
        }

        /// <summary>
        /// Remove the last step from the wizard
        /// </summary>
        public void RemoveLastStep()
        {
            if (_config.Steps.Count > 0)
            {
                _config.Steps.RemoveAt(_config.Steps.Count - 1);
                OnComponentChanged();
            }
        }

        /// <summary>
        /// Clear all steps
        /// </summary>
        public void ClearSteps()
        {
            if (_config.Steps.Count > 0)
            {
                _config.Steps.Clear();
                OnComponentChanged();
            }
        }

        /// <summary>
        /// Preview the wizard (opens in a test dialog)
        /// </summary>
        public void PreviewWizard()
        {
            try
            {
                var result = WizardManager.ShowWizard(_config);
                MessageBox.Show($"Wizard completed with result: {result}", "Wizard Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error previewing wizard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helper Methods

        private void SetProperty(string propertyName, object value)
        {
            var property = TypeDescriptor.GetProperties(_config)[propertyName];
            if (property != null && !property.IsReadOnly)
            {
                var oldValue = property.GetValue(_config);
                if (!Equals(oldValue, value))
                {
                    property.SetValue(_config, value);
                    OnComponentChanged();
                }
            }
        }

        private void OnComponentChanged()
        {
            // Notify the designer that the component has changed
            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            changeService?.OnComponentChanged(_config, null, null, null);
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Header
            items.Add(new DesignerActionHeaderItem("Wizard Configuration"));
            
            // Properties
            items.Add(new DesignerActionPropertyItem(nameof(Title), "Title:", "Wizard"));
            items.Add(new DesignerActionPropertyItem(nameof(Description), "Description:", "Wizard"));
            items.Add(new DesignerActionPropertyItem(nameof(Style), "Style:", "Wizard"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowProgressBar), "Show Progress Bar:", "Wizard"));
            items.Add(new DesignerActionPropertyItem(nameof(AllowCancel), "Allow Cancel:", "Wizard"));
            items.Add(new DesignerActionPropertyItem(nameof(AllowBack), "Allow Back:", "Wizard"));
            items.Add(new DesignerActionPropertyItem(nameof(StepCount), "Step Count:", "Wizard"));

            // Separator
            items.Add(new DesignerActionTextItem("Step Management", "Actions"));
            
            // Actions
            items.Add(new DesignerActionMethodItem(this, nameof(AddStep), "Add Step", "Actions", "Add a new step to the wizard", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveLastStep), "Remove Last Step", "Actions", "Remove the last step from the wizard", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearSteps), "Clear All Steps", "Actions", "Remove all steps from the wizard", true));
            
            // Separator
            items.Add(new DesignerActionTextItem("Testing", "Actions"));
            
            // Preview
            items.Add(new DesignerActionMethodItem(this, nameof(PreviewWizard), "Preview Wizard", "Actions", "Preview the wizard in a test dialog", true));

            return items;
        }

        #endregion
    }
}
