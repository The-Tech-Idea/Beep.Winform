namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        private bool _animateMaximizeRestore = true;
        private bool _animateStyleChange = true;
        private double _animationOpacityFrom = 1.0;

        public bool AnimateMaximizeRestore
        {
            get => _animateMaximizeRestore;
            set => _animateMaximizeRestore = value;
        }

        public bool AnimateStyleChange
        {
            get => _animateStyleChange;
            set => _animateStyleChange = value;
        }

        partial void InitializeAnimationsFeature()
        {
            // nothing
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            _animationOpacityFrom = Opacity;
        }

        private async Task AnimateOpacityAsync(double from, double to, int durationMs)
        {
            try
            {
                double start = from, end = to;
                int steps = 10;
                double delta = (end - start) / steps;
                int delay = Math.Max(8, durationMs / steps);
                Opacity = Math.Max(0, Math.Min(1, start));
                for (int i = 0; i < steps; i++)
                {
                    await Task.Delay(delay);
                    Opacity = Math.Max(0, Math.Min(1, Opacity + delta));
                }
                Opacity = Math.Max(0, Math.Min(1, end));
            }
            catch { }
        }

        partial void OnApplyFormStyleAnimated()
        {
            if (IsHandleCreated && _animateStyleChange)
            {
                _ = AnimateOpacityAsync(0.0, 1.0, 140);
            }
        }
    }
}
