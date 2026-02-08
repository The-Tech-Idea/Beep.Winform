using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Helper utilities for wizard animations and transitions
    /// </summary>
    public static class WizardHelpers
    {
        /// <summary>
        /// Animate a step transition by sliding controls in/out
        /// </summary>
        /// <param name="fromControl">The control being navigated away from</param>
        /// <param name="toControl">The control being navigated to</param>
        /// <param name="forward">True for forward (slide left), false for back (slide right)</param>
        /// <param name="onComplete">Callback when animation completes</param>
        /// <param name="timerRegistry">Optional timer list for disposal tracking</param>
        public static void AnimateStepTransition(
            Control fromControl,
            Control toControl,
            bool forward,
            Action onComplete,
            List<Timer> timerRegistry = null)
        {
            if (fromControl == null || toControl == null || fromControl.Parent == null)
            {
                onComplete?.Invoke();
                return;
            }

            var container = fromControl.Parent;
            int width = container.ClientSize.Width;

            // Position the new control off-screen
            toControl.Location = new Point(forward ? width : -width, 0);
            toControl.Size = fromControl.Size;

            if (!container.Controls.Contains(toControl))
            {
                container.Controls.Add(toControl);
            }

            // Animation parameters
            const int totalFrames = 12;
            int currentFrame = 0;
            int fromStartX = fromControl.Left;
            int toStartX = toControl.Left;
            int fromEndX = forward ? -width : width;
            int toEndX = 0;

            var timer = new Timer { Interval = 16 }; // ~60fps
            timerRegistry?.Add(timer);

            timer.Tick += (s, e) =>
            {
                currentFrame++;
                float progress = (float)currentFrame / totalFrames;

                // Ease-out cubic for smooth deceleration
                float eased = 1f - (1f - progress) * (1f - progress) * (1f - progress);

                int fromX = (int)(fromStartX + (fromEndX - fromStartX) * eased);
                int toX = (int)(toStartX + (toEndX - toStartX) * eased);

                fromControl.Left = fromX;
                toControl.Left = toX;

                if (currentFrame >= totalFrames)
                {
                    timer.Stop();
                    timerRegistry?.Remove(timer);
                    timer.Dispose();

                    // Final positions
                    toControl.Left = 0;
                    toControl.Dock = DockStyle.Fill;

                    onComplete?.Invoke();
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Animate a fade transition between controls
        /// </summary>
        public static void AnimateFadeTransition(
            Control fromControl,
            Control toControl,
            Action onComplete,
            List<Timer> timerRegistry = null)
        {
            if (fromControl == null || toControl == null)
            {
                onComplete?.Invoke();
                return;
            }

            // Simple approach: just invoke complete immediately
            // True opacity animation requires per-pixel alpha which is complex in WinForms
            onComplete?.Invoke();
        }
    }
}
