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

        private string _painterName = "CircularNode";

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("CircularNode")]
        [TypeConverter(typeof(StepperPainterNameConverter))]
        [Description("Stepper painter name resolved from StepperPainterRegistry.")]
        public string PainterName
        {
            get => _painterName;
            set
            {
                if (_painterName == value) return;
                // An auto-property here left the painter stale: assigning PainterName after
                // construction painted the OLD style until something else re-initialized.
                _painterName = value;
                InitializePainter();
                Invalidate();
            }
        }

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

        /// <summary>The first of the candidates that actually carries text, or null.</summary>
        private static string FirstNonBlank(params string[] candidates)
        {
            if (candidates == null) return null;
            foreach (var c in candidates)
                if (!string.IsNullOrWhiteSpace(c)) return c;
            return null;
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
                        model.SubText = string.Empty;
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
                // Text is the title. Name and SubText are the fallbacks, not the other way round.
                //
                // This read `item.Name` for the title and `item.Text` for the subtitle, which is
                // backwards from every other Beep control - Text is the display text everywhere
                // else. A caller filling ListItems the ordinary way (Text set, Name left null)
                // therefore got a generic "Step 1" heading with their real caption demoted to the
                // subtitle line, which reads as "the stepper shows no titles".
                string label = showLabel ? (item != null
                    ? (FirstNonBlank(item.Text, item.Name) ?? $"Step {i + 1}")
                    : GetStepLabel(i)) : string.Empty;
                string subtitle = showLabel ? FirstNonBlank(item?.SubText, item?.Description) : string.Empty;

                steps.Add(new StepModel
                {
                    Text = label,
                    SubText = subtitle,
                    Tooltip = item != null ? GetStepTooltip(i) : label,
                    State = GetStepState(i),
                    IsEnabled = true
                });
            }

            return steps;
        }
    }
}
