using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;
using TheTechIdea.Beep.Winform.Controls.Steppers.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBreadCrumb
    {
        private IStepperPainter _stepperPainter;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("ChevronBreadcrumb")]
        [TypeConverter(typeof(StepperPainterNameConverter))]
        [Description("Stepper painter name resolved from StepperPainterRegistry.")]
        public string PainterName { get; set; } = "ChevronBreadcrumb";

        private void InitializePainter()
        {
            _stepperPainter = StepperPainterRegistry.GetPainter(PainterName);
            _stepperPainter?.Initialize(this, _currentTheme, _textFont, _textFont, _textFont);
        }

        private bool TryPaintWithRegisteredPainter(Graphics graphics)
        {
            if (_stepperPainter == null || _stepperPainter is NoOpStepperPainter || ListItems == null || ListItems.Count == 0)
            {
                return false;
            }

            var steps = BuildPainterSteps();
            var styleConfig = new StepperStyleConfig
            {
                ControlStyle = ControlStyle,
                RecommendedButtonSize = new Size(32, 32),
                RecommendedStepSpacing = 0,
                RecommendedConnectorLineWidth = System.Math.Max(1, BorderThickness)
            };

            var layout = _stepperPainter.ComputeLayout(DrawingRect, steps, orientation, styleConfig);
            if (layout?.StepRects == null || layout.StepRects.Count == 0)
            {
                return false;
            }

            chevronBounds.Clear();
            chevronPaths.Clear();
            foreach (var rect in layout.StepRects)
            {
                chevronBounds.Add(rect);
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddRectangle(rect);
                chevronPaths.Add(path);
            }

            var context = new StepPainterContext
            {
                Graphics = graphics,
                DrawingRect = DrawingRect,
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                Steps = steps,
                StepRects = layout.StepRects,
                ConnectorRects = layout.ConnectorRects,
                AnimationStates = Enumerable.Range(0, steps.Count).Select(_ => new StepAnimationState()).ToList(),
                SelectedIndex = selectedIndex,
                HoveredIndex = _hoveredStepIndex,
                FocusedIndex = _focusedStepIndex >= 0 ? _focusedStepIndex : selectedIndex,
                Orientation = orientation,
                StyleConfig = styleConfig,
                StepFont = _textFont,
                LabelFont = _textFont,
                NumberFont = _textFont
            };

            _stepperPainter.Paint(graphics, context);
            RegisterChevronHitAreas();
            return true;
        }

        private List<StepModel> BuildPainterSteps()
        {
            var steps = new List<StepModel>(ListItems.Count);
            for (int i = 0; i < ListItems.Count; i++)
            {
                StepState state = i < selectedIndex ? StepState.Completed : i == selectedIndex ? StepState.Active : StepState.Pending;
                steps.Add(new StepModel
                {
                    Text = ListItems[i].Name ?? string.Empty,
                    Subtitle = ListItems[i].Text ?? string.Empty,
                    Tooltip = GetStepTooltip(i),
                    State = state,
                    IsEnabled = true
                });
            }

            return steps;
        }

        private void DrawLegacyChevronStep(Graphics graphics, int stepIndex, int count, int x, int y, int stepLen, int crossLen, int arrowSize)
        {
            Point[] points = GetChevronPolygonPoints(stepIndex, count, x, y, stepLen, crossLen, arrowSize);
            using var path = new GraphicsPath();
            path.AddPolygon(points);
            chevronPaths.Add((GraphicsPath)path.Clone());
            chevronBounds.Add(Rectangle.Round(path.GetBounds()));

            Color fill = ResolveLegacyChevronFill(stepIndex);
            using (var fillBrush = new SolidBrush(fill))
            {
                graphics.FillPolygon(fillBrush, points);
            }

            int borderWidth = StepperAccessibilityHelpers.GetAccessibleBorderWidth(BorderThickness);
            using (var pen = new Pen(_currentTheme?.ShadowColor ?? Color.Gray, borderWidth))
            {
                graphics.DrawPolygon(pen, points);
            }

            DrawLegacyChevronText(graphics, stepIndex, x, y, stepLen, crossLen);
        }

        private Color ResolveLegacyChevronFill(int stepIndex)
        {
            if (animatedColors.ContainsKey(stepIndex))
            {
                return animatedColors[stepIndex];
            }

            StepState state = stepIndex < selectedIndex
                ? StepState.Completed
                : stepIndex == selectedIndex
                    ? StepState.Active
                    : StepState.Pending;

            return state switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(_currentTheme, UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(_currentTheme, UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(_currentTheme, UseThemeColors)
            };
        }
    }
}
