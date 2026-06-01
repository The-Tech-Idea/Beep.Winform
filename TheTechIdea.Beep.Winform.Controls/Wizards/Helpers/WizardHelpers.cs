using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Helper utilities for wizard animations and transitions.
    /// Bitmap-based animation eliminates per-frame control hierarchy repaints.
    /// </summary>
    public static class WizardHelpers
    {
        public static Font GetFont(IBeepTheme? theme, TypographyStyle? style, float fallbackSize, FontStyle fallbackStyle)
        {
            if (style != null)
                return BeepThemesManager.ToFont(style)
                    ?? BeepFontManager.GetFont(BeepFontManager.DefaultFontName, fallbackSize, fallbackStyle);

            if (theme?.BodyStyle != null && fallbackStyle == FontStyle.Regular)
                return BeepThemesManager.ToFont(theme.BodyStyle)
                    ?? BeepFontManager.GetFont(BeepFontManager.DefaultFontName, fallbackSize, fallbackStyle);

            return BeepFontManager.GetFont(BeepFontManager.DefaultFontName, fallbackSize, fallbackStyle);
        }

        public static Color GetErrorColor(IBeepTheme? theme)
        {
            return theme?.ErrorColor ?? Color.FromArgb(200, 50, 50);
        }

        public static Color GetWarningBackColor(IBeepTheme? theme)
        {
            if (theme == null) return Color.FromArgb(255, 235, 235);
            return Color.FromArgb(40, theme.ErrorColor);
        }

        /// <summary>
        /// Animate a step transition by sliding control bitmaps.
        /// Captures snapshots of both controls, animates the bitmaps,
        /// then swaps in the live target control on completion.
        /// Eliminates per-frame control hierarchy repaints during animation.
        /// </summary>
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
            int height = container.ClientSize.Height;

            // Guard against zero-dimension container (form not yet shown)
            if (width <= 0 || height <= 0)
            {
                onComplete?.Invoke();
                return;
            }

            // Capture bitmaps. fromControl is already a visible child.
            // toControl may not be parented yet — parent it momentarily for bitmap capture.
            Bitmap fromBitmap = null, toBitmap = null;
            try
            {
                fromBitmap = new Bitmap(width, height);
                fromControl.DrawToBitmap(fromBitmap, new Rectangle(0, 0, width, height));

                bool toWasUnparented = toControl.Parent == null;
                if (toWasUnparented)
                {
                    // Minimally parent toControl so DrawToBitmap works (needs a handle)
                    container.Controls.Add(toControl);
                    toControl.Visible = false;
                }

                toBitmap = new Bitmap(width, height);
                toControl.DrawToBitmap(toBitmap, new Rectangle(0, 0, width, height));

                if (toWasUnparented)
                    container.Controls.Remove(toControl);
            }
            catch
            {
                fromBitmap?.Dispose();
                toBitmap?.Dispose();
                onComplete?.Invoke();
                return;
            }

            // Hide fromControl during bitmap-only animation
            fromControl.Visible = false;

            // Create a PictureBox for the animation overlay
            var overlay = new PictureBox
            {
                Size = new Size(width, height),
                Location = Point.Empty,
                BackColor = container.BackColor
            };
            container.Controls.Add(overlay);
            overlay.BringToFront();

            // Draw initial state
            using (var g = overlay.CreateGraphics())
            {
                g.DrawImageUnscaled(fromBitmap, 0, 0);
            }

            int fromStartX = 0;
            int fromEndX = forward ? -width : width;
            int toStartX = forward ? width : -width;
            int toEndX = 0;

            var stopwatch = Stopwatch.StartNew();
            const int durationMs = 300;
            var timer = new Timer { Interval = 16 };
            timerRegistry?.Add(timer);

            timer.Tick += (s, e) =>
            {
                long elapsed = stopwatch.ElapsedMilliseconds;
                float progress = Math.Min(1f, (float)elapsed / durationMs);

                if (progress >= 1f)
                {
                    timer.Stop();
                    timerRegistry?.Remove(timer);
                    timer.Dispose();
                    stopwatch.Stop();

                    overlay.Dispose();
                    fromBitmap?.Dispose();
                    toBitmap?.Dispose();

                    // Callback handles parenting and making toControl visible with Dock = Fill
                    onComplete?.Invoke();
                    return;
                }

                float eased = WizardAnimationEngine.EaseInOutCubic(progress);
                int fromX = (int)(fromStartX + (fromEndX - fromStartX) * eased);
                int toX = (int)(toStartX + (toEndX - toStartX) * eased);

                using (var g = overlay.CreateGraphics())
                {
                    g.Clear(container.BackColor);
                    g.DrawImageUnscaled(fromBitmap, fromX, 0);
                    g.DrawImageUnscaled(toBitmap, toX, 0);
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Animate a fade transition between controls.
        /// Simple swap — true opacity animation requires per-pixel alpha which is complex in WinForms.
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

            onComplete?.Invoke();
        }
    }
}
