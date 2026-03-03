using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        private readonly List<StepAnimationState> _painterAnimationStates = new();
        private int _focusedStepIndex = -1;
        private int _pressedStepIndex = -1;
        private int _nodePulseStepIndex = -1;
        private System.DateTime _nodePulseStartTime;
        private const int NodePulseDurationMs = 100;

        private void EnsurePainterAnimationStates()
        {
            while (_painterAnimationStates.Count < stepCount)
            {
                int newIndex = _painterAnimationStates.Count;
                _painterAnimationStates.Add(new StepAnimationState
                {
                    NodeScale = 1f,
                    ConnectorFillProgress = newIndex < currentStep ? 1f : 0f
                });
            }

            if (_painterAnimationStates.Count > stepCount)
            {
                _painterAnimationStates.RemoveRange(stepCount, _painterAnimationStates.Count - stepCount);
            }
        }

        private IReadOnlyList<StepAnimationState> GetPainterAnimationStatesSnapshot()
        {
            EnsurePainterAnimationStates();
            return _painterAnimationStates.Select(state => new StepAnimationState
            {
                HoverProgress = state.HoverProgress,
                PressProgress = state.PressProgress,
                NodeScale = state.NodeScale,
                ConnectorFillProgress = state.ConnectorFillProgress,
                RippleActive = state.RippleActive,
                RippleCenter = state.RippleCenter,
                RippleRadius = state.RippleRadius,
                RippleAlpha = state.RippleAlpha
            }).ToList();
        }

        private void StartStepRipple(int stepIndex, Point location)
        {
            if (stepIndex < 0 || stepIndex >= stepCount || StepperAccessibilityHelpers.IsReducedMotionEnabled())
            {
                return;
            }

            EnsurePainterAnimationStates();
            var anim = _painterAnimationStates[stepIndex];
            anim.RippleActive = true;
            anim.RippleCenter = location;
            anim.RippleRadius = DpiScalingHelper.ScaleValue(4, this);
            anim.RippleAlpha = 110;
            if (!animationTimer.Enabled)
            {
                animationStartTime = System.DateTime.Now;
                animationTimer.Start();
            }
        }

        private void StartNodePulse(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepCount || StepperAccessibilityHelpers.IsReducedMotionEnabled())
            {
                return;
            }

            EnsurePainterAnimationStates();
            _nodePulseStepIndex = stepIndex;
            _nodePulseStartTime = System.DateTime.Now;
            _painterAnimationStates[stepIndex].NodeScale = 1f;
            if (!animationTimer.Enabled)
            {
                animationStartTime = System.DateTime.Now;
                animationTimer.Start();
            }
        }

        private void StartConnectorTransition(int targetStepIndex)
        {
            if (StepperAccessibilityHelpers.IsReducedMotionEnabled())
            {
                EnsurePainterAnimationStates();
                for (int i = 0; i < _painterAnimationStates.Count; i++)
                {
                    _painterAnimationStates[i].ConnectorFillProgress = i < targetStepIndex ? 1f : 0f;
                }
                return;
            }

            EnsurePainterAnimationStates();
            if (!animationTimer.Enabled)
            {
                animationStartTime = System.DateTime.Now;
                animationTimer.Start();
            }
        }

        private bool AdvancePainterAnimations()
        {
            EnsurePainterAnimationStates();
            bool hasActive = false;
            float connectorStep = animationDuration > 0
                ? 16f / animationDuration
                : 0.08f;

            for (int i = 0; i < _painterAnimationStates.Count; i++)
            {
                var anim = _painterAnimationStates[i];
                float connectorTarget = i < currentStep ? 1f : 0f;
                if (System.Math.Abs(anim.ConnectorFillProgress - connectorTarget) > 0.001f)
                {
                    if (anim.ConnectorFillProgress < connectorTarget)
                    {
                        anim.ConnectorFillProgress = StepperAnimationEasing.Clamp01(anim.ConnectorFillProgress + connectorStep);
                    }
                    else
                    {
                        anim.ConnectorFillProgress = StepperAnimationEasing.Clamp01(anim.ConnectorFillProgress - connectorStep);
                    }

                    hasActive = true;
                }

                if (i == _nodePulseStepIndex)
                {
                    double pulseElapsed = (System.DateTime.Now - _nodePulseStartTime).TotalMilliseconds;
                    if (pulseElapsed < NodePulseDurationMs)
                    {
                        float t = StepperAnimationEasing.Clamp01((float)(pulseElapsed / NodePulseDurationMs));
                        anim.NodeScale = 1f + (0.08f * (float)System.Math.Sin(t * System.Math.PI));
                        hasActive = true;
                    }
                    else
                    {
                        anim.NodeScale = 1f;
                        _nodePulseStepIndex = -1;
                    }
                }
                else if (anim.NodeScale != 1f)
                {
                    anim.NodeScale = 1f;
                }

                if (!anim.RippleActive)
                {
                    continue;
                }

                anim.RippleRadius += DpiScalingHelper.ScaleValue(3, this);
                anim.RippleAlpha = System.Math.Max(0, anim.RippleAlpha - 12);
                if (anim.RippleAlpha <= 0)
                {
                    anim.RippleActive = false;
                    anim.RippleRadius = 0f;
                }
                else
                {
                    hasActive = true;
                }
            }

            return hasActive;
        }
    }
}
