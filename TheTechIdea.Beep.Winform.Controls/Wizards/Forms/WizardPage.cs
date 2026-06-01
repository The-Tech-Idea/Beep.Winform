using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Forms
{
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.WizardPageDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    [ToolboxItem(true)]
    [Description("A container page for a wizard step. Drop child controls here at design time.")]
    public class WizardPage : BufferedPanel, IWizardStepContent
    {
        private string _title;
        private string _description;
        private string _nextButtonText;
        private bool _isComplete;

        protected WizardContext Context { get; private set; }

        [Category("Wizard")]
        [Description("Step title shown in the wizard stepper")]
        [DefaultValue("")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Wizard")]
        [Description("Step description shown beneath the title")]
        [DefaultValue("")]
        public string Description
        {
            get => _description;
            set { _description = value; Invalidate(); }
        }

        [Category("Wizard")]
        [Description("Custom Next button text for this step (leave empty for default)")]
        [DefaultValue("")]
        public string NextButtonText
        {
            get => _nextButtonText;
            set => _nextButtonText = value;
        }

        [Category("Wizard")]
        [Description("Whether this step is currently in a complete state")]
        [DefaultValue(false)]
        public bool IsComplete
        {
            get => _isComplete;
            set
            {
                if (_isComplete == value) return;
                _isComplete = value;
                OnValidationStateChanged(new StepValidationEventArgs(value));
            }
        }

        public WizardPage()
        {
            Text = "";
            TabIndex = 0;
        }

        public virtual void OnStepEnter(WizardContext context)
        {
            Context = context;
            LoadData();
        }

        public virtual void OnStepLeave(WizardContext context)
        {
            SaveData();
        }

        public virtual WizardValidationResult Validate()
        {
            return WizardValidationResult.Success();
        }

        public virtual Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(Validate());
        }

        public event System.EventHandler<StepValidationEventArgs> ValidationStateChanged;

        protected virtual void OnValidationStateChanged(StepValidationEventArgs e)
        {
            ValidationStateChanged?.Invoke(this, e);
        }

        protected void RaiseValidationStateChanged(bool isValid, string? message = null)
        {
            OnValidationStateChanged(new StepValidationEventArgs(isValid, message));
        }

        protected virtual void LoadData() { }
        protected virtual void SaveData() { }
    }
}
