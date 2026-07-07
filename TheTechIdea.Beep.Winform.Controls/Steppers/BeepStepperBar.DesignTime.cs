using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        /// <summary>
        /// Populate sample steps when the control is in design mode
        /// so the designer preview shows realistic content.
        /// </summary>
        private void EnsureDesignTimeSampleData()
        {
            if (!DesignMode && !LicenseManager.UsageMode.HasFlag(LicenseUsageMode.Designtime))
                return;

            if (_stepModels != null && _stepModels.Count > 0)
                return;
            if (ListItems != null && ListItems.Count > 0)
                return;
            if (stepCount > 1 && stepLabels.Count > 0)
                return;

            // Populate with 4 sample workflow steps for the designer preview
            stepCount = 4;
            currentStep = 1;

            stepLabels.Clear();
            stepStates.Clear();

            stepLabels[0] = "Select";
            stepLabels[1] = "Configure";
            stepLabels[2] = "Review";
            stepLabels[3] = "Complete";

            stepStates[0] = StepState.Completed;
            stepStates[1] = StepState.Active;
            stepStates[2] = StepState.Pending;
            stepStates[3] = StepState.Pending;

            SyncStepsWithListItems();
        }

        /// <summary>
        /// Populate StepModels with sample data for design-time preview.
        /// </summary>
        public void LoadDesignTimeSampleSteps()
        {
            if (_stepModels == null)
                _stepModels = new BindingList<StepModel>();

            if (_stepModels.Count > 0)
                return;

            _stepModels.Add(new StepModel
            {
                Text = "Select",
                Subtitle = "Choose your plan",
                State = StepState.Completed,
                Tooltip = "Step 1: Select your plan",
                ImagePath = "check.svg"
            });
            _stepModels.Add(new StepModel
            {
                Text = "Configure",
                Subtitle = "Customize settings",
                State = StepState.Active,
                Tooltip = "Step 2: Configure your settings"
            });
            _stepModels.Add(new StepModel
            {
                Text = "Review",
                Subtitle = "Verify details",
                State = StepState.Pending,
                Tooltip = "Step 3: Review your order"
            });
            _stepModels.Add(new StepModel
            {
                Text = "Complete",
                Subtitle = "Confirmation",
                State = StepState.Pending,
                Tooltip = "Step 4: Order confirmed"
            });

            stepCount = _stepModels.Count;
            currentStep = 1;
            SyncStepModelsWithSteps();
            InitializeSteps();
        }
    }
}
