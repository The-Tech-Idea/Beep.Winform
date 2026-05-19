using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Original direct control position animation.
    /// Both controls are in the container and their Left properties are animated.
    /// Container double-buffering prevents flicker.
    /// </summary>
    public static class WizardAnimationEngine
    {
        private const int DefaultDurationMs = 400;

        private static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;
        }

        public static void SlideTransition(
            Control fromControl,
            Control toControl,
            bool forward,
            Action onComplete,
            List<Timer> timerRegistry = null,
            int durationMs = DefaultDurationMs)
        {
            if (fromControl == null || toControl == null || fromControl.Parent == null)
            {
                onComplete?.Invoke();
                return;
            }

            var container = fromControl.Parent;
            int width = container.ClientSize.Width;

            // Ensure both controls are in the container
            if (!container.Controls.Contains(fromControl))
                container.Controls.Add(fromControl);
            if (!container.Controls.Contains(toControl))
                container.Controls.Add(toControl);

            container.SuspendLayout();

            fromControl.Dock = DockStyle.None;
            toControl.Dock = DockStyle.None;
            fromControl.Size = container.ClientSize;
            toControl.Size = container.ClientSize;

            fromControl.Location = Point.Empty;
            toControl.Location = new Point(forward ? width : -width, 0);

            fromControl.Visible = true;
            toControl.Visible = true;

            fromControl.SendToBack();
            toControl.BringToFront();

            container.ResumeLayout(false);

            // Animation parameters
            int fromStartX = 0;
            int toStartX = forward ? width : -width;
            int fromEndX = forward ? -width : width;
            int toEndX = 0;

            var stopwatch = Stopwatch.StartNew();
            bool complete = false;
            var timer = new Timer { Interval = 16 };
            timerRegistry?.Add(timer);

            timer.Tick += (s, e) =>
            {
                if (complete) return;

                long elapsed = stopwatch.ElapsedMilliseconds;
                float progress = Math.Min(1f, (float)elapsed / durationMs);

                if (progress >= 1f)
                {
                    complete = true;
                    timer.Stop();
                    timerRegistry?.Remove(timer);
                    timer.Dispose();
                    stopwatch.Stop();

                    // Final positions
                    container.SuspendLayout();
                    fromControl.Location = new Point(fromEndX, 0);
                    toControl.Location = Point.Empty;
                    container.ResumeLayout(false);

                    // Hide old, show new
                    fromControl.Visible = false;
                    toControl.Dock = DockStyle.Fill;

                    onComplete?.Invoke();
                    return;
                }

                // Ease-in-out cubic easing
                float eased = EaseInOutCubic(progress);

                int fromX = (int)(fromStartX + (fromEndX - fromStartX) * eased);
                int toX = (int)(toStartX + (toEndX - toStartX) * eased);

                // Direct position change
                fromControl.Left = fromX;
                toControl.Left = toX;
            };

            timer.Start();
        }
    }
}
