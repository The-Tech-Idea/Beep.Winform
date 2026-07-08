using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Dispatches wizard step transitions with configurable type, easing, and duration.
    /// Uses bitmap-based pre-rendering for flicker-free 60fps animations.
    /// </summary>
    public static class WizardTransitionEngine
    {
        /// <summary>
        /// Animate a transition between two controls.
        /// </summary>
        public static void AnimateTransition(
            Control fromControl,
            Control toControl,
            TransitionType type,
            TransitionEasing easing,
            int durationMs,
            bool forward,
            Action onComplete,
            List<Timer> timerRegistry = null)
        {
            if (fromControl == null || toControl == null || fromControl.Parent == null)
            {
                onComplete?.Invoke();
                return;
            }

            // Respect global reduced motion
            if (WizardManager.ReducedMotion || type == TransitionType.None || durationMs <= 0)
            {
                onComplete?.Invoke();
                return;
            }

            switch (type)
            {
                case TransitionType.Fade:
                    AnimateFade(fromControl, toControl, easing, durationMs, onComplete, timerRegistry);
                    break;
                case TransitionType.Zoom:
                    AnimateZoom(fromControl, toControl, easing, durationMs, onComplete, timerRegistry);
                    break;
                case TransitionType.Flip:
                    AnimateFlip(fromControl, toControl, easing, durationMs, onComplete, timerRegistry);
                    break;
                case TransitionType.Slide:
                default:
                    AnimateSlide(fromControl, toControl, easing, durationMs, forward, onComplete, timerRegistry);
                    break;
            }
        }

        // ── Slide ──────────────────────────────────────────────────────────

        private static void AnimateSlide(
            Control fromControl, Control toControl,
            TransitionEasing easing, int durationMs, bool forward,
            Action onComplete, List<Timer> timerRegistry)
        {
            var container = fromControl.Parent;
            int width = container.ClientSize.Width;
            int height = container.ClientSize.Height;
            if (width <= 0 || height <= 0) { onComplete?.Invoke(); return; }

            Bitmap fromBitmap = null, toBitmap = null;
            try
            {
                (fromBitmap, toBitmap) = CaptureBitmaps(container, fromControl, toControl, width, height);
            }
            catch
            {
                fromBitmap?.Dispose();
                toBitmap?.Dispose();
                onComplete?.Invoke();
                return;
            }

            fromControl.Visible = false;
            var overlay = CreateOverlay(container, width, height);
            var stopwatch = Stopwatch.StartNew();
            var timer = new Timer { Interval = 16 };
            timerRegistry?.Add(timer);
            var easingFn = WizardAnimationEngine.Resolve(easing);

            int fromEndX = forward ? -width : width;
            int toStartX = forward ? width : -width;

            timer.Tick += (s, e) =>
            {
                float progress = Math.Min(1f, (float)stopwatch.ElapsedMilliseconds / durationMs);
                float eased = easingFn(progress);

                if (progress >= 1f)
                {
                    CleanupAnimation(timer, timerRegistry, stopwatch, overlay, fromBitmap, toBitmap);
                    onComplete?.Invoke();
                    return;
                }

                int fromX = (int)(fromEndX * eased);
                int toX = (int)(toStartX * (1f - eased));

                using (var g = overlay.CreateGraphics())
                {
                    g.Clear(container.BackColor);
                    g.DrawImageUnscaled(fromBitmap, fromX, 0);
                    g.DrawImageUnscaled(toBitmap, toX, 0);
                }
            };

            timer.Start();
        }

        // ── Fade ────────────────────────────────────────────────────────────

        private static void AnimateFade(
            Control fromControl, Control toControl,
            TransitionEasing easing, int durationMs,
            Action onComplete, List<Timer> timerRegistry)
        {
            var container = fromControl.Parent;
            int width = container.ClientSize.Width;
            int height = container.ClientSize.Height;
            if (width <= 0 || height <= 0) { onComplete?.Invoke(); return; }

            Bitmap fromBitmap = null, toBitmap = null;
            try
            {
                (fromBitmap, toBitmap) = CaptureBitmaps(container, fromControl, toControl, width, height);
            }
            catch
            {
                fromBitmap?.Dispose();
                toBitmap?.Dispose();
                onComplete?.Invoke();
                return;
            }

            fromControl.Visible = false;
            var overlay = CreateOverlay(container, width, height);
            var stopwatch = Stopwatch.StartNew();
            var timer = new Timer { Interval = 16 };
            timerRegistry?.Add(timer);
            var easingFn = WizardAnimationEngine.Resolve(easing);

            // Color matrices for alpha blending
            var fromMatrix = new ColorMatrix { Matrix33 = 1f };  // full opacity (will be reduced)
            var toMatrix = new ColorMatrix { Matrix33 = 0f };    // starts transparent

            timer.Tick += (s, e2) =>
            {
                float progress = Math.Min(1f, (float)stopwatch.ElapsedMilliseconds / durationMs);
                float eased = easingFn(progress);

                if (progress >= 1f)
                {
                    CleanupAnimation(timer, timerRegistry, stopwatch, overlay, fromBitmap, toBitmap);
                    onComplete?.Invoke();
                    return;
                }

                fromMatrix.Matrix33 = 1f - eased;  // from fades out
                toMatrix.Matrix33 = eased;          // to fades in

                using (var g = overlay.CreateGraphics())
                {
                    g.Clear(container.BackColor);
                    var fromAttrs = new ImageAttributes();
                    fromAttrs.SetColorMatrix(fromMatrix);
                    g.DrawImage(fromBitmap, new Rectangle(0, 0, width, height),
                        0, 0, width, height, GraphicsUnit.Pixel, fromAttrs);

                    var toAttrs = new ImageAttributes();
                    toAttrs.SetColorMatrix(toMatrix);
                    g.DrawImage(toBitmap, new Rectangle(0, 0, width, height),
                        0, 0, width, height, GraphicsUnit.Pixel, toAttrs);
                }
            };

            timer.Start();
        }

        // ── Zoom ────────────────────────────────────────────────────────────

        private static void AnimateZoom(
            Control fromControl, Control toControl,
            TransitionEasing easing, int durationMs,
            Action onComplete, List<Timer> timerRegistry)
        {
            var container = fromControl.Parent;
            int width = container.ClientSize.Width;
            int height = container.ClientSize.Height;
            if (width <= 0 || height <= 0) { onComplete?.Invoke(); return; }

            Bitmap fromBitmap = null, toBitmap = null;
            try
            {
                (fromBitmap, toBitmap) = CaptureBitmaps(container, fromControl, toControl, width, height);
            }
            catch
            {
                fromBitmap?.Dispose();
                toBitmap?.Dispose();
                onComplete?.Invoke();
                return;
            }

            fromControl.Visible = false;
            var overlay = CreateOverlay(container, width, height);
            var stopwatch = Stopwatch.StartNew();
            var timer = new Timer { Interval = 16 };
            timerRegistry?.Add(timer);
            var easingFn = WizardAnimationEngine.Resolve(easing);
            float cx = width / 2f, cy = height / 2f;

            timer.Tick += (s, e2) =>
            {
                float progress = Math.Min(1f, (float)stopwatch.ElapsedMilliseconds / durationMs);
                float eased = easingFn(progress);

                if (progress >= 1f)
                {
                    CleanupAnimation(timer, timerRegistry, stopwatch, overlay, fromBitmap, toBitmap);
                    onComplete?.Invoke();
                    return;
                }

                // from: scale 1.0 → 0.8 + fade
                float fromScale = 1f - 0.2f * eased;
                int fromW = (int)(width * fromScale);
                int fromH = (int)(height * fromScale);
                int fromX = (int)(cx - fromW / 2f);
                int fromY = (int)(cy - fromH / 2f);
                float fromAlpha = 1f - eased;

                // to: scale 1.2 → 1.0 + fade
                float toScale = 1.2f - 0.2f * eased;
                int toW = (int)(width * toScale);
                int toH = (int)(height * toScale);
                int toX = (int)(cx - toW / 2f);
                int toY = (int)(cy - toH / 2f);
                float toAlpha = eased;

                using (var g = overlay.CreateGraphics())
                {
                    g.Clear(container.BackColor);
                    // From (zooming out, fading out)
                    var fromAttrs = new ImageAttributes();
                    fromAttrs.SetColorMatrix(new ColorMatrix { Matrix33 = fromAlpha });
                    g.DrawImage(fromBitmap, new Rectangle(fromX, fromY, fromW, fromH),
                        0, 0, width, height, GraphicsUnit.Pixel, fromAttrs);
                    // To (zooming in, fading in)
                    var toAttrs = new ImageAttributes();
                    toAttrs.SetColorMatrix(new ColorMatrix { Matrix33 = toAlpha });
                    g.DrawImage(toBitmap, new Rectangle(toX, toY, toW, toH),
                        0, 0, width, height, GraphicsUnit.Pixel, toAttrs);
                }
            };

            timer.Start();
        }

        // ── Flip ────────────────────────────────────────────────────────────

        private static void AnimateFlip(
            Control fromControl, Control toControl,
            TransitionEasing easing, int durationMs,
            Action onComplete, List<Timer> timerRegistry)
        {
            var container = fromControl.Parent;
            int width = container.ClientSize.Width;
            int height = container.ClientSize.Height;
            if (width <= 0 || height <= 0) { onComplete?.Invoke(); return; }

            Bitmap fromBitmap = null, toBitmap = null;
            try
            {
                (fromBitmap, toBitmap) = CaptureBitmaps(container, fromControl, toControl, width, height);
            }
            catch
            {
                fromBitmap?.Dispose();
                toBitmap?.Dispose();
                onComplete?.Invoke();
                return;
            }

            fromControl.Visible = false;
            var overlay = CreateOverlay(container, width, height);
            var stopwatch = Stopwatch.StartNew();
            var timer = new Timer { Interval = 16 };
            timerRegistry?.Add(timer);
            var easingFn = WizardAnimationEngine.Resolve(easing);
            float cx = width / 2f;

            timer.Tick += (s, e2) =>
            {
                float progress = Math.Min(1f, (float)stopwatch.ElapsedMilliseconds / durationMs);
                float eased = easingFn(progress);

                if (progress >= 1f)
                {
                    CleanupAnimation(timer, timerRegistry, stopwatch, overlay, fromBitmap, toBitmap);
                    onComplete?.Invoke();
                    return;
                }

                // First half: compress from width → 0. Second half: expand to from 0 → width.
                float halfProgress = eased < 0.5f ? eased * 2f : (1f - eased) * 2f;
                float compressWidth = width * (1f - halfProgress);
                int compX = (int)(cx - compressWidth / 2f);

                using (var g = overlay.CreateGraphics())
                {
                    g.Clear(container.BackColor);
                    if (eased < 0.5f)
                    {
                        // Show from, compressing toward center
                        g.DrawImage(fromBitmap,
                            new Rectangle(compX, 0, (int)compressWidth, height),
                            new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        // Show to, expanding from center
                        g.DrawImage(toBitmap,
                            new Rectangle(compX, 0, (int)compressWidth, height),
                            new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                    }
                }
            };

            timer.Start();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static (Bitmap from, Bitmap to) CaptureBitmaps(
            Control container, Control fromControl, Control toControl, int width, int height)
        {
            // Phase A optimization: toControl is already parented and laid out by UpdateUI.
            // No need for the expensive parent/unparent cycle here.
            var fromBitmap = new Bitmap(width, height);
            fromControl.DrawToBitmap(fromBitmap, new Rectangle(0, 0, width, height));

            var toBitmap = new Bitmap(width, height);
            toControl.DrawToBitmap(toBitmap, new Rectangle(0, 0, width, height));

            return (fromBitmap, toBitmap);
        }

        private static PictureBox CreateOverlay(Control container, int width, int height)
        {
            var overlay = new PictureBox
            {
                Size = new Size(width, height),
                Location = Point.Empty,
                BackColor = container.BackColor
            };
            container.Controls.Add(overlay);
            overlay.BringToFront();
            return overlay;
        }

        private static void CleanupAnimation(
            Timer timer, List<Timer> registry,
            Stopwatch stopwatch, PictureBox overlay,
            Bitmap fromBitmap, Bitmap toBitmap)
        {
            timer.Stop();
            registry?.Remove(timer);
            timer.Dispose();
            stopwatch.Stop();
            overlay.Dispose();
            fromBitmap?.Dispose();
            toBitmap?.Dispose();
        }
    }
}
