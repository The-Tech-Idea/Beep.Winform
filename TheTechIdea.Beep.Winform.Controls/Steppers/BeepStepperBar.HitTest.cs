using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ToolTips;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        private const string StepHitAreaPrefix = "Stepper.Step.";

        private void RegisterStepHitAreas()
        {
            _hitTest?.ClearHitList();
            if (_hitTest == null || buttonBounds.Count == 0)
            {
                return;
            }

            for (int i = 0; i < buttonBounds.Count; i++)
            {
                _hitTest.AddHitArea(GetStepHitAreaName(i), buttonBounds[i], null, null);
            }
        }

        private static string GetStepHitAreaName(int index) => $"{StepHitAreaPrefix}{index}";

        private static bool TryGetStepIndexFromHitArea(ControlHitTest hitTest, out int stepIndex)
        {
            stepIndex = -1;
            if (hitTest == null || string.IsNullOrWhiteSpace(hitTest.Name) || !hitTest.Name.StartsWith(StepHitAreaPrefix))
            {
                return false;
            }

            return int.TryParse(hitTest.Name.Substring(StepHitAreaPrefix.Length), out stepIndex);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _hitTest?.HandleMouseMove(e.Location);
            int hoveredIndex = -1;
            if (_hitTest != null && _hitTest.HitTest(e.Location, out var hit) && TryGetStepIndexFromHitArea(hit, out int idx))
            {
                hoveredIndex = idx;
            }

            if (hoveredIndex != _hoveredStepIndex)
            {
                _hoveredStepIndex = hoveredIndex;
                UpdateTooltipForHoveredStep();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hitTest?.HandleMouseLeave();

            _hoveredStepIndex = -1;
            if (!string.IsNullOrEmpty(_currentTooltipKey))
            {
                _ = ToolTipManager.Instance.HideTooltipAsync(_currentTooltipKey);
                _currentTooltipKey = null;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!allowStepNavigation || _hitTest == null)
            {
                return;
            }

            if (_hitTest.HitTest(e.Location, out var hit) && TryGetStepIndexFromHitArea(hit, out int stepIndex))
            {
                NavigateToStep(stepIndex);
                StartStepRipple(stepIndex, e.Location);
                ShowStepNotification(stepIndex);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_hitTest == null)
            {
                return;
            }

            _hitTest.HandleMouseDown(e.Location, e);
            _pressedStepIndex = -1;
            if (_hitTest.HitTest(e.Location, out var hit) && TryGetStepIndexFromHitArea(hit, out int stepIndex))
            {
                _pressedStepIndex = stepIndex;
            }
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _hitTest?.HandleMouseUp(e.Location, e);
            _pressedStepIndex = -1;
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _focusedStepIndex = currentStep >= 0 ? currentStep : 0;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _focusedStepIndex = -1;
            Invalidate();
        }
    }
}
