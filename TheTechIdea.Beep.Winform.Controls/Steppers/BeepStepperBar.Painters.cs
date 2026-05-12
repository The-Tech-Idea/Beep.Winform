using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;
using TheTechIdea.Beep.Winform.Controls.Steppers.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        private IStepperPainter _stepperPainter;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("CircularNode")]
        [TypeConverter(typeof(StepperPainterNameConverter))]
        [Description("Stepper painter name resolved from StepperPainterRegistry.")]
        public string PainterName { get; set; } = "CircularNode";

        private void InitializePainter()
        {
            _stepperPainter = StepperPainterRegistry.GetPainter(PainterName);
            _stepperPainter?.Initialize(this, _currentTheme, _textFont, _textFont, _textFont);
        }

        private bool TryPaintWithRegisteredPainter(Graphics graphics)
        {
            if (_stepperPainter == null || _stepperPainter is NoOpStepperPainter || stepCount <= 0)
            {
                return false;
            }

            var steps = BuildPainterSteps();
            var drawingRect = GetStepperContentBounds();
            var styleConfig = new StepperStyleConfig
            {
                ControlStyle = ControlStyle,
                RecommendedButtonSize = buttonSize,
                RecommendedStepSpacing = GetScaledStepSpacing(),
                RecommendedConnectorLineWidth = connectorLineWidth
            };

            var layout = _stepperPainter.ComputeLayout(drawingRect, steps, orientation, styleConfig);
            if (layout?.StepRects == null || layout.StepRects.Count == 0)
            {
                return false;
            }

            buttonBounds.Clear();
            buttonBounds.AddRange(layout.StepRects);
            RegisterStepHitAreas();

            var context = new StepPainterContext
            {
                Graphics = graphics,
                DrawingRect = drawingRect,
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                Steps = steps,
                StepRects = layout.StepRects,
                ConnectorRects = layout.ConnectorRects,
                AnimationStates = GetPainterAnimationStatesSnapshot(),
                SelectedIndex = currentStep,
                HoveredIndex = _hoveredStepIndex,
                PressedIndex = _pressedStepIndex,
                FocusedIndex = _focusedStepIndex,
                Orientation = orientation,
                StyleConfig = styleConfig,
                StepFont = _textFont,
                LabelFont = _textFont,
                NumberFont = _textFont
            };

            _stepperPainter.Paint(graphics, context);
            return true;
        }

        private List<StepModel> BuildPainterSteps()
        {
            if (stepCount <= 0)
            {
                return new List<StepModel>();
            }

            if (_stepModels != null && _stepModels.Count > 0)
            {
                return _stepModels.Take(stepCount).Select((m, i) =>
                {
                    var model = m ?? new StepModel();
                    bool showLabel = ShouldShowStepLabel(i);
                    if (!showLabel)
                    {
                        model.Text = string.Empty;
                        model.Subtitle = string.Empty;
                    }
                    return model;
                }).ToList();
            }

            ListItems ??= new BindingList<SimpleItem>();
            var steps = new List<StepModel>(stepCount);
            for (int i = 0; i < stepCount; i++)
            {
                var item = i < ListItems.Count ? ListItems[i] : null;
                bool showLabel = ShouldShowStepLabel(i);
                string label = showLabel ? (item != null
                    ? (item.Name ?? $"Step {i + 1}")
                    : GetStepLabel(i)) : string.Empty;
                string subtitle = showLabel ? item?.Text : string.Empty;

                steps.Add(new StepModel
                {
                    Text = label,
                    Subtitle = subtitle,
                    Tooltip = item != null ? GetStepTooltip(i) : label,
                    State = GetStepState(i),
                    IsEnabled = true
                });
            }

            return steps;
        }
    }
}
