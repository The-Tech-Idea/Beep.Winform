using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        private BeepButton _prevButton;
        private BeepButton _nextButton;
        private bool _showNextPrevButtons;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        [Description("Shows themed Previous and Next BeepButtons under the stepper.")]
        public bool ShowNextPrevButtons
        {
            get => _showNextPrevButtons;
            set
            {
                if (_showNextPrevButtons == value)
                {
                    return;
                }

                _showNextPrevButtons = value;
                EnsureNavigationButtons();
                UpdateNavigationButtonsLayout();
                UpdateNavigationButtonsState();
                Invalidate();
            }
        }

        private void EnsureNavigationButtons()
        {
            if (_prevButton == null)
            {
                _prevButton = CreateNavigationButton("Previous", NavigationButtonRole.Previous);
                Controls.Add(_prevButton);
            }

            if (_nextButton == null)
            {
                _nextButton = CreateNavigationButton("Next", NavigationButtonRole.Next);
                Controls.Add(_nextButton);
            }

            _prevButton.Visible = _showNextPrevButtons;
            _nextButton.Visible = _showNextPrevButtons;
        }

        private BeepButton CreateNavigationButton(string text, NavigationButtonRole role)
        {
            var button = new BeepButton
            {
                IsChild = true,
                UseThemeColors = UseThemeColors,
                Text = text,
                Visible = false,
                TabStop = false
            };

            button.Click += (_, _) =>
            {
                if (role == NavigationButtonRole.Previous)
                {
                    PreviousStep();
                }
                else
                {
                    NextStep();
                }
            };

            return button;
        }

        private int GetNavigationButtonsReservedHeight()
        {
            if (!_showNextPrevButtons)
            {
                return 0;
            }

            int buttonHeight = DpiScalingHelper.ScaleValue(30, this);
            int spacing = DpiScalingHelper.ScaleValue(12, this);
            int verticalMargins = DpiScalingHelper.ScaleValue(16, this);
            return buttonHeight + spacing + verticalMargins;
        }

        private Rectangle GetStepperContentBounds()
        {
            int outerPadding = DpiScalingHelper.ScaleValue(6, this);
            return StepperLayoutHelper.ComputeContentBounds(
                ClientRectangle,
                _showNextPrevButtons,
                GetNavigationButtonsReservedHeight(),
                outerPadding);
        }

        private void UpdateNavigationButtonsLayout()
        {
            if (_prevButton == null || _nextButton == null)
            {
                return;
            }

            if (!_showNextPrevButtons)
            {
                _prevButton.Visible = false;
                _nextButton.Visible = false;
                return;
            }

            int buttonHeight = DpiScalingHelper.ScaleValue(30, this);
            int buttonWidth = DpiScalingHelper.ScaleValue(92, this);
            int spacing = DpiScalingHelper.ScaleValue(10, this);
            int bottomPadding = DpiScalingHelper.ScaleValue(8, this);
            int rightPadding = DpiScalingHelper.ScaleValue(8, this);
            int y = Height - buttonHeight - bottomPadding;
            int nextX = Width - buttonWidth - rightPadding;
            int prevX = nextX - buttonWidth - spacing;

            _prevButton.Bounds = new Rectangle(prevX, y, buttonWidth, buttonHeight);
            _nextButton.Bounds = new Rectangle(nextX, y, buttonWidth, buttonHeight);
            _prevButton.Visible = true;
            _nextButton.Visible = true;
        }

        private void UpdateNavigationButtonsState()
        {
            if (_prevButton == null || _nextButton == null)
            {
                return;
            }

            _prevButton.Enabled = _showNextPrevButtons && currentStep > 0;
            _nextButton.Enabled = _showNextPrevButtons && currentStep < (stepCount - 1);
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateNavigationButtonsLayout();
        }

        private enum NavigationButtonRole
        {
            Previous,
            Next
        }

        private void DisposeNavigationButtons()
        {
            if (_prevButton != null)
            {
                Controls.Remove(_prevButton);
                _prevButton.Dispose();
                _prevButton = null;
            }

            if (_nextButton != null)
            {
                Controls.Remove(_nextButton);
                _nextButton.Dispose();
                _nextButton = null;
            }
        }
    }
}
