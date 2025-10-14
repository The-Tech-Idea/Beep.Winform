using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Helper methods for wizard rendering and layout
    /// </summary>
    public static class WizardHelpers
    {
        /// <summary>
        /// Get theme colors for wizard elements
        /// </summary>
        public static (Color backColor, Color foreColor, Color accentColor, Color borderColor) GetWizardColors(IBeepTheme theme)
        {
            if (theme == null)
            {
                return (Color.White, Color.Black, Color.FromArgb(0, 120, 215), Color.LightGray);
            }

            return (
                theme.BackColor,
                theme.ForeColor,
                theme.AccentColor,
                theme.BorderColor
            );
        }

        /// <summary>
        /// Draw progress bar
        /// </summary>
        public static void DrawProgressBar(Graphics g, Rectangle bounds, int currentStep, int totalSteps, Color accentColor)
        {
            if (totalSteps <= 0) return;

            var progress = (float)currentStep / totalSteps;

            // Background
            using (var backBrush = new SolidBrush(Color.FromArgb(40, accentColor)))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Progress fill
            var fillWidth = (int)(bounds.Width * progress);
            var fillRect = new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height);

            using (var fillBrush = new SolidBrush(accentColor))
            {
                g.FillRectangle(fillBrush, fillRect);
            }
        }

        /// <summary>
        /// Draw step indicator (numbered circle)
        /// </summary>
        public static void DrawStepIndicator(Graphics g, Point center, int radius, int stepNumber, bool isActive, bool isCompleted, IBeepTheme theme)
        {
            var (backColor, foreColor, accentColor, borderColor) = GetWizardColors(theme);

            var bounds = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            if (isCompleted)
            {
                // Completed: filled with success color
                using (var brush = new SolidBrush(theme?.SuccessColor ?? Color.Green))
                {
                    g.FillEllipse(brush, bounds);
                }

                // Checkmark
                DrawCheckmark(g, bounds, Color.White);
            }
            else if (isActive)
            {
                // Active: filled with accent color
                using (var brush = new SolidBrush(accentColor))
                {
                    g.FillEllipse(brush, bounds);
                }

                // Step number in white
                DrawStepNumber(g, bounds, stepNumber, Color.White);
            }
            else
            {
                // Inactive: border only
                using (var pen = new Pen(Color.FromArgb(100, foreColor), 2))
                {
                    g.DrawEllipse(pen, bounds);
                }

                // Step number
                DrawStepNumber(g, bounds, stepNumber, Color.FromArgb(150, foreColor));
            }
        }

        /// <summary>
        /// Draw checkmark icon
        /// </summary>
        private static void DrawCheckmark(Graphics g, Rectangle bounds, Color color)
        {
            using (var pen = new Pen(color, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                var centerX = bounds.X + bounds.Width / 2;
                var centerY = bounds.Y + bounds.Height / 2;
                var size = bounds.Width / 3;

                var points = new[]
                {
                    new Point(centerX - size / 2, centerY),
                    new Point(centerX - size / 6, centerY + size / 2),
                    new Point(centerX + size / 2, centerY - size / 3)
                };

                g.DrawLines(pen, points);
            }
        }

        /// <summary>
        /// Draw step number
        /// </summary>
        private static void DrawStepNumber(Graphics g, Rectangle bounds, int number, Color color)
        {
            using (var font = new Font("Segoe UI", 10f, FontStyle.Bold))
            using (var brush = new SolidBrush(color))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(number.ToString(), font, brush, bounds, sf);
            }
        }

        /// <summary>
        /// Draw connector line between steps
        /// </summary>
        public static void DrawConnectorLine(Graphics g, Point start, Point end, bool isCompleted, IBeepTheme theme)
        {
            var (_, foreColor, accentColor, _) = GetWizardColors(theme);

            var color = isCompleted ? (theme?.SuccessColor ?? Color.Green) : Color.FromArgb(100, foreColor);

            using (var pen = new Pen(color, 2))
            {
                g.DrawLine(pen, start, end);
            }
        }

        /// <summary>
        /// Create rounded rectangle path
        /// </summary>
        public static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Animate transition between steps
        /// </summary>
        public static void AnimateStepTransition(Control fromControl, Control toControl, bool forward, Action onComplete)
        {
            if (!WizardManager.EnableAnimations)
            {
                if (fromControl != null) fromControl.Visible = false;
                if (toControl != null) toControl.Visible = true;
                onComplete?.Invoke();
                return;
            }

            var timer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            var duration = 300; // ms
            var elapsed = 0;

            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                var progress = Math.Min(1.0, (double)elapsed / duration);

                // Easing
                progress = EaseOutCubic(progress);

                if (fromControl != null && fromControl.Parent != null)
                {
                    var offset = (int)(fromControl.Width * progress * (forward ? -1 : 1));
                    fromControl.Left = offset;
                    // Note: Control.Opacity doesn't exist in WinForms - opacity handled at Form level
                }

                if (toControl != null && toControl.Parent != null)
                {
                    var offset = (int)(toControl.Width * (1 - progress) * (forward ? 1 : -1));
                    toControl.Left = offset;
                    toControl.Visible = true;
                    // Note: Control.Opacity doesn't exist in WinForms - opacity handled at Form level
                }

                if (elapsed >= duration)
                {
                    timer.Stop();
                    timer.Dispose();

                    if (fromControl != null) fromControl.Visible = false;
                    if (toControl != null)
                    {
                        toControl.Left = 0;
                        // Note: Control.Opacity doesn't exist in WinForms - opacity handled at Form level
                    }

                    onComplete?.Invoke();
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Easing function for smooth animations
        /// </summary>
        private static double EaseOutCubic(double t)
        {
            return 1 - Math.Pow(1 - t, 3);
        }

        /// <summary>
        /// Calculate optimal button width based on text
        /// </summary>
        public static int CalculateButtonWidth(Graphics g, string text, Font font, int minWidth = 80, int padding = 32)
        {
            var size = TextUtils.MeasureText(g,text, font);
            return Math.Max(minWidth, (int)Math.Ceiling(size.Width) + padding);
        }

        /// <summary>
        /// Format step title with numbering
        /// </summary>
        public static string FormatStepTitle(int stepNumber, int totalSteps, string title)
        {
            return $"Step {stepNumber} of {totalSteps}: {title}";
        }

        /// <summary>
        /// Get completion percentage
        /// </summary>
        public static int GetCompletionPercentage(int currentStep, int totalSteps)
        {
            if (totalSteps == 0) return 0;
            return (int)((float)(currentStep + 1) / totalSteps * 100);
        }

        /// <summary>
        /// Validate all completed steps
        /// </summary>
        public static bool ValidateCompletedSteps(WizardContext context, System.Collections.Generic.List<WizardStep> steps)
        {
            for (int i = 0; i <= context.CurrentStepIndex; i++)
            {
                if (i >= steps.Count) continue;

                var step = steps[i];
                if (step.CanNavigateNext != null && !step.CanNavigateNext(context))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
