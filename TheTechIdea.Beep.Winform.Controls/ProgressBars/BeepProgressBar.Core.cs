using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars
{
    public partial class BeepProgressBar : BaseControl
    {
        private readonly Dictionary<ProgressPainterKind, IProgressPainter> _painters = new();
        private ProgressPainterKind _painterKind = ProgressPainterKind.Linear;
        private Dictionary<string, object> _parameters = new();

        [Category("Appearance")]
        public ProgressPainterKind PainterKind { get => _painterKind; set { _painterKind = value; Invalidate(); } }

        [Browsable(false)]
        public Dictionary<string, object> Parameters { get => _parameters; set { _parameters = value ?? new(); Invalidate(); } }

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
