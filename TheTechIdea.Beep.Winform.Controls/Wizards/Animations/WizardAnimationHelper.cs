using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Animations
{
    /// <summary>
    /// Helper class for wizard animations
    /// Provides slide, fade, and scale transition effects
    /// </summary>
    public static class WizardAnimationHelper
    {
        /// <summary>
        /// Animate slide transition between steps
        /// </summary>
        public static async Task AnimateSlide(Control fromControl, Control toControl, SlideDirection direction, int durationMs = 300)
        {
            if (fromControl == null || toControl == null) return;

            var parent = fromControl.Parent;
            if (parent == null) return;

            var bounds = parent.ClientRectangle;
            int stepCount = durationMs / 16; // ~60 FPS
            float stepSize = (float)bounds.Width / stepCount;

            toControl.Visible = true;
            toControl.BringToFront();

            // Set initial positions based on direction
            Point fromStart = fromControl.Location;
            Point toStart = toControl.Location;
            Point fromEnd = fromStart;
            Point toEnd = toStart;

            switch (direction)
            {
                case SlideDirection.Left:
                    toStart = new Point(bounds.Right, toStart.Y);
                    fromEnd = new Point(-bounds.Width, fromStart.Y);
                    break;
                case SlideDirection.Right:
                    toStart = new Point(-bounds.Width, toStart.Y);
                    fromEnd = new Point(bounds.Right, fromStart.Y);
                    break;
                case SlideDirection.Top:
                    toStart = new Point(toStart.X, bounds.Bottom);
                    fromEnd = new Point(fromStart.X, -bounds.Height);
                    break;
                case SlideDirection.Bottom:
                    toStart = new Point(toStart.X, -bounds.Height);
                    fromEnd = new Point(fromStart.X, bounds.Bottom);
                    break;
            }

            toControl.Location = toStart;

            for (int i = 0; i <= stepCount; i++)
            {
                float progress = (float)i / stepCount;
                progress = EaseInOut(progress); // Apply easing

                // Calculate positions
                int fromX = (int)(fromStart.X + (fromEnd.X - fromStart.X) * progress);
                int fromY = (int)(fromStart.Y + (fromEnd.Y - fromStart.Y) * progress);
                int toX = (int)(toStart.X + (toEnd.X - toStart.X) * progress);
                int toY = (int)(toStart.Y + (toEnd.Y - toStart.Y) * progress);

                fromControl.Location = new Point(fromX, fromY);
                toControl.Location = new Point(toX, toY);

                await Task.Delay(16); // ~60 FPS
            }

            // Final positions
            fromControl.Location = fromEnd;
            toControl.Location = toEnd;
            fromControl.Visible = false;
        }

        /// <summary>
        /// Animate fade transition between steps
        /// Note: WinForms controls don't support Opacity directly, so we use a simple cross-fade effect
        /// </summary>
        public static async Task AnimateFade(Control fromControl, Control toControl, int durationMs = 300)
        {
            if (fromControl == null || toControl == null) return;

            int stepCount = durationMs / 16; // ~60 FPS

            toControl.Visible = true;
            toControl.BringToFront();

            // Simple fade using visibility and alpha blending would require custom painting
            // For now, use a simple cross-fade by showing/hiding controls
            for (int i = 0; i <= stepCount; i++)
            {
                float progress = (float)i / stepCount;
                progress = EaseInOut(progress);

                // Simple visibility-based fade (can be enhanced with custom painting)
                if (progress > 0.5f)
                {
                    fromControl.Visible = false;
                }

                await Task.Delay(16);
            }

            fromControl.Visible = false;
            toControl.Visible = true;
        }

        /// <summary>
        /// Animate scale transition
        /// </summary>
        public static async Task AnimateScale(Control control, float fromScale, float toScale, int durationMs = 300)
        {
            if (control == null) return;

            int stepCount = durationMs / 16;
            var originalSize = control.Size;
            var originalLocation = control.Location;

            for (int i = 0; i <= stepCount; i++)
            {
                float progress = (float)i / stepCount;
                progress = EaseInOut(progress);

                float currentScale = fromScale + (toScale - fromScale) * progress;
                var newSize = new Size(
                    (int)(originalSize.Width * currentScale),
                    (int)(originalSize.Height * currentScale));

                control.Size = newSize;
                control.Location = new Point(
                    originalLocation.X + (originalSize.Width - newSize.Width) / 2,
                    originalLocation.Y + (originalSize.Height - newSize.Height) / 2);

                await Task.Delay(16);
            }
        }

        /// <summary>
        /// Ease-in-out easing function
        /// </summary>
        private static float EaseInOut(float t)
        {
            return t < 0.5f
                ? 2f * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;
        }
    }

    /// <summary>
    /// Slide direction for animations
    /// </summary>
    public enum SlideDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }
}
