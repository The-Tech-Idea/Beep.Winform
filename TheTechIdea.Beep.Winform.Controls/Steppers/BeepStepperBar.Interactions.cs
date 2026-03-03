using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        private int GetNextEnabledStepIndex(int startIndex, int direction)
        {
            if (stepCount <= 0)
            {
                return -1;
            }

            int index = startIndex;
            for (int i = 0; i < stepCount; i++)
            {
                index += direction;
                if (index < 0 || index >= stepCount)
                {
                    return -1;
                }

                return index;
            }

            return -1;
        }

        private void FocusStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepCount)
            {
                return;
            }

            _focusedStepIndex = stepIndex;
            Invalidate();
        }

        private void NavigateToStep(int stepIndex)
        {
            if (stepIndex >= 0 && stepIndex < stepCount)
            {
                var args = new StepValidatingEventArgs(currentStep, stepIndex);
                StepValidating?.Invoke(this, args);

                if (!args.Cancel)
                {
                    CurrentStep = stepIndex;

                    if (stepIndex < ListItems.Count)
                    {
                        SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(ListItems[stepIndex]));
                    }
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (stepCount <= 0)
            {
                return base.ProcessDialogKey(keyData);
            }

            int keyCode = (int)(keyData & Keys.KeyCode);
            int currentFocus = _focusedStepIndex >= 0 ? _focusedStepIndex : currentStep;
            switch ((Keys)keyCode)
            {
                case Keys.Left:
                case Keys.Up:
                {
                    int previous = GetNextEnabledStepIndex(currentFocus, -1);
                    if (previous >= 0)
                    {
                        FocusStep(previous);
                    }
                    return true;
                }
                case Keys.Right:
                case Keys.Down:
                {
                    int next = GetNextEnabledStepIndex(currentFocus, 1);
                    if (next >= 0)
                    {
                        FocusStep(next);
                    }
                    return true;
                }
                case Keys.Home:
                    FocusStep(0);
                    return true;
                case Keys.End:
                    FocusStep(stepCount - 1);
                    return true;
                case Keys.Enter:
                case Keys.Space:
                    if (allowStepNavigation)
                    {
                        int target = _focusedStepIndex >= 0 ? _focusedStepIndex : currentStep;
                        NavigateToStep(target);
                        StartStepRipple(target, buttonBounds.Count > target ? buttonBounds[target].Location : Point.Empty);
                    }
                    return true;
                default:
                    return base.ProcessDialogKey(keyData);
            }
        }
    }
}
