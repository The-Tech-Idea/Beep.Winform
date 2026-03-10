using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars
{
    public partial class BeepProgressBar : BaseControl
    {
        private readonly Dictionary<ProgressPainterKind, IProgressPainter> _painters = new();
        private ProgressPainterKind _painterKind = ProgressPainterKind.Linear;
        private Dictionary<string, object> _parameters = new();
        private ProgressPainterContext _cachedPainterContext;
        private IReadOnlyDictionary<string, object> _cachedPainterParameters;

        [Category("Appearance")]
        public ProgressPainterKind PainterKind
        {
            get => _painterKind;
            set
            {
                if (_painterKind == value)
                {
                    return;
                }

                _painterKind = value;
                _hoverArea = null;
                _pressedArea = null;
                EnsurePreferredHeightForPainter();
                ApplyAccessibilitySettings();
                RequestVisualRefresh(resetLayoutCache: true);
            }
        }

        [Browsable(false)]
        public Dictionary<string, object> Parameters
        {
            get => _parameters;
            set
            {
                _parameters = value ?? new();
                ApplyAccessibilitySettings();
                RequestVisualRefresh(resetLayoutCache: true);
            }
        }

        [Browsable(false)]
        internal ProgressPainterContext ActivePainterContext
            => _cachedPainterContext ?? BuildPainterContext(ProgressBarLayoutHelper.GetPaintBounds(this, DrawingRect));

        [Browsable(false)]
        internal IReadOnlyDictionary<string, object> ActivePainterParameters
            => _cachedPainterParameters ?? BuildPainterParameters(ActivePainterContext);

        // Events
        public event System.EventHandler StepClicked; // generic
        public event System.EventHandler<ProgressStepEventArgs> StepIndexClicked; // Step:i
        public event System.EventHandler<ProgressStepEventArgs> ChevronStepClicked; // Step:i when ChevronSteps painter
        public event System.EventHandler<ProgressDotEventArgs> DotClicked; // Dot:i
        public event System.EventHandler RingClicked; // Ring or RingDots

        private void EnsureDefaultPainters()
        {
            if (_painters.Count > 0) return;
            RegisterPainter(ProgressPainterKind.Linear, new Painters.LinearProgressPainter());
            RegisterPainter(ProgressPainterKind.StepperCircles, new Painters.StepperCirclesPainter());
            RegisterPainter(ProgressPainterKind.ChevronSteps, new Painters.ChevronStepsPainter());
            RegisterPainter(ProgressPainterKind.DotsLoader, new Painters.DotsLoaderPainter());
            RegisterPainter(ProgressPainterKind.Segmented, new Painters.SegmentedLinePainter());
            RegisterPainter(ProgressPainterKind.Ring, new Painters.RingProgressPainter());
            RegisterPainter(ProgressPainterKind.DottedRing, new Painters.DottedRingProgressPainter());
            // new mockup variants
            RegisterPainter(ProgressPainterKind.LinearBadge, new Painters.LinearBadgePainter());
            RegisterPainter(ProgressPainterKind.LinearTrackerIcon, new Painters.LinearTrackerIconPainter());
            RegisterPainter(ProgressPainterKind.ArrowStripe, new Painters.ArrowStripePainter());
            RegisterPainter(ProgressPainterKind.RadialSegmented, new Painters.RadialSegmentedPainter());
            RegisterPainter(ProgressPainterKind.RingCenterImage, new Painters.RingCenterImagePainter());
            RegisterPainter(ProgressPainterKind.ArrowHeadAnimated, new Painters.ArrowHeadAnimatedPainter());
        }

        private IProgressPainter GetActivePainter()
        {
            return _painters.TryGetValue(_painterKind, out var p) ? p : null;
        }

        public void RegisterPainter(ProgressPainterKind kind, IProgressPainter painter)
        {
            if (painter == null) return; _painters[kind] = painter;
        }

        private void EnsurePreferredHeightForPainter()
        {
            int preferredHeight = ProgressPainterRegistry.GetPreferredMinimumHeight(this, _painterKind);
            if (preferredHeight <= 0 || Height >= preferredHeight)
            {
                return;
            }

            Height = preferredHeight;
        }

        internal ProgressPainterContext BuildPainterContext(Rectangle paintBounds)
        {
            _cachedPainterContext = new ProgressPainterContext
            {
                PainterKind = _painterKind,
                Bounds = paintBounds,
                Theme = _currentTheme,
                ControlStyle = ControlStyle,
                Parameters = _parameters,
                State = new ProgressPainterState
                {
                    Value = _value,
                    Minimum = _minimum,
                    Maximum = _maximum,
                    Step = _step,
                    IsEnabled = Enabled,
                    IsFocused = Focused && _keyboardFocusVisible,
                    IsHovered = !string.IsNullOrEmpty(_hoverArea),
                    IsPressed = !string.IsNullOrEmpty(_pressedArea),
                    Progress01 = ProgressPercentage,
                    DisplayProgress01 = DisplayProgressPercentage
                }
            };

            _cachedPainterParameters = BuildPainterParameters(_cachedPainterContext);
            return _cachedPainterContext;
        }

        private IReadOnlyDictionary<string, object> BuildPainterParameters(ProgressPainterContext context)
        {
            var merged = new Dictionary<string, object>(_parameters);
            merged["State"] = context.State;
            merged["Context"] = context;
            merged["DpiScale"] = DeviceDpi / 96f;
            merged["ScaledBorderThickness"] = ProgressBarDpiHelpers.Scale(this, BorderThickness);
            merged["ScaledCornerRadius"] = ProgressBarLayoutHelper.GetScaledCornerRadius(this, context.Bounds);
            merged["ScaledStripeWidth"] = ProgressBarDpiHelpers.Scale(this, StripeWidth);
            merged["ScaledSegments"] = Segments;
            merged["SupportsKeyboard"] = ProgressPainterRegistry.GetMetadata(_painterKind).SupportsKeyboard;
            return merged;
        }
    }

    public sealed class ProgressStepEventArgs : System.EventArgs
    {
        public ProgressStepEventArgs(int index) { Index = index; }
        public int Index { get; }
    }

    public sealed class ProgressDotEventArgs : System.EventArgs
    {
        public ProgressDotEventArgs(int index) { Index = index; }
        public int Index { get; }
    }
}
