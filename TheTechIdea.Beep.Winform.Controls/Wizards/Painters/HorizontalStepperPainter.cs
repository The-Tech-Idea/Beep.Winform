using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Painters
{
    /// <summary>
    /// Interface for wizard painters
    /// </summary>
    public interface IWizardPainter
    {
        /// <summary>Initialize the painter with host, theme, and wizard instance.</summary>
        void Initialize(Control host, IBeepTheme theme, WizardInstance instance);

        /// <summary>Compute content panel bounds from the form client rectangle.</summary>
        Rectangle GetContentBounds(Rectangle formBounds);

        /// <summary>Compute step indicator panel bounds from the form client rectangle.</summary>
        Rectangle GetStepIndicatorBounds(Rectangle formBounds);

        /// <summary>Paint the step indicators (circles, dots, timeline, cards).</summary>
        void PaintStepIndicators(Graphics g, Rectangle bounds, int currentIndex, IList<WizardStep> steps);
    }

    /// <summary>
    /// Painter for horizontal stepper wizard style
    /// </summary>
    public class HorizontalStepperPainter : IWizardPainter
    {
        #region Fields

        private Control _host;
        private IBeepTheme _theme;
        private WizardInstance _instance;

        // Cached colors
        private Color _completedColor;
        private Color _currentColor;
        private Color _pendingColor;
        private Color _lineColor;
        private Color _textColor;
        private Color _subtextColor;

        // Cached fonts
        private Font _titleFont;
        private Font _labelFont;

        // Connector animation
        private float _connectorAnimProgress = 1f; // 0→1 as connector fills
        private System.Windows.Forms.Timer _animTimer;
        private int _animTargetIndex;
        private System.Diagnostics.Stopwatch _animStopwatch;

        /// <summary>Start the connector bar animation toward the target step index.</summary>
        public void StartConnectorAnimation(int targetIndex, int durationMs = 300)
        {
            if (_host == null || WizardManager.ReducedMotion) { _connectorAnimProgress = 1f; _host?.Invalidate(); return; }
            _animTargetIndex = targetIndex;
            _connectorAnimProgress = 0f;
            _animStopwatch = System.Diagnostics.Stopwatch.StartNew();
            if (_animTimer == null)
            {
                _animTimer = new System.Windows.Forms.Timer { Interval = 16 };
                _animTimer.Tick += (s, e) =>
                {
                    float t = Math.Min(1f, (float)_animStopwatch.ElapsedMilliseconds / durationMs);
                    _connectorAnimProgress = WizardAnimationEngine.EaseOutCubic(t);
                    _host?.Invalidate();
                    if (t >= 1f) { _animTimer.Stop(); _animStopwatch.Stop(); }
                };
            }
            _animTimer.Start();
        }

        #endregion

        #region Initialization

        public void Initialize(Control host, IBeepTheme theme, WizardInstance instance)
        {
            _host = host;
            _theme = theme;
            _instance = instance;

            if (theme != null)
            {
                _completedColor = theme.SuccessColor;
                _currentColor = theme.PrimaryColor;
                _pendingColor = Color.FromArgb(80, theme.ForeColor);
                _lineColor = Color.FromArgb(40, theme.ForeColor);
                _textColor = theme.ForeColor;
                _subtextColor = Color.FromArgb(128, theme.ForeColor);
            }
            else
            {
                _completedColor = ColorUtils.MapSystemColor(SystemColors.Highlight);
                _currentColor = ColorUtils.MapSystemColor(SystemColors.HotTrack);
                _pendingColor = Color.FromArgb(80, ColorUtils.MapSystemColor(SystemColors.GrayText));
                _lineColor = Color.FromArgb(40, ColorUtils.MapSystemColor(SystemColors.ControlDark));
                _textColor = ColorUtils.MapSystemColor(SystemColors.WindowText);
                _subtextColor = Color.FromArgb(128, ColorUtils.MapSystemColor(SystemColors.GrayText));
            }

            _titleFont?.Dispose();
            _titleFont = WizardHelpers.GetFont(theme, theme?.TitleStyle, 10f, FontStyle.Bold);
            _labelFont?.Dispose();
            _labelFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 9f, FontStyle.Regular);
        }

        #endregion

        #region IWizardPainter

        void IWizardPainter.PaintStepIndicators(Graphics g, Rectangle bounds, int currentIndex, IList<WizardStep> steps)
            => PaintStepIndicators(g, bounds, currentIndex, steps?.Count ?? 0, steps);

        public Rectangle GetContentBounds(Rectangle formBounds)
        {
            int marginX = DpiScalingHelper.ScaleValue(30, _host);
            int top = DpiScalingHelper.ScaleValue(110, _host);
            int bottom = DpiScalingHelper.ScaleValue(180, _host);
            return new Rectangle(
                formBounds.Left + marginX,
                formBounds.Top + top,
                formBounds.Width - (marginX * 2),
                formBounds.Height - bottom
            );
        }

        public Rectangle GetStepIndicatorBounds(Rectangle formBounds)
        {
            int height = DpiScalingHelper.ScaleValue(100, _host);
            return new Rectangle(
                formBounds.Left,
                formBounds.Top,
                formBounds.Width,
                height
            );
        }

        #endregion

        #region Paint Methods

        /// <summary>
        /// Paint horizontal step indicators with icons
        /// </summary>
        public void PaintStepIndicators(Graphics g, Rectangle bounds, int currentIndex, int totalSteps, IList<WizardStep> steps)
        {
            if (steps == null || steps.Count == 0) return;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // DPI-scale all layout values
            int stepCount = steps.Count;
            int circleSize = DpiScalingHelper.ScaleValue(36, _host);
            int padding = DpiScalingHelper.ScaleValue(50, _host);
            int labelOffset = DpiScalingHelper.ScaleValue(8, _host);
            int labelWidth = DpiScalingHelper.ScaleValue(120, _host);
            int labelHeight = DpiScalingHelper.ScaleValue(24, _host);
            float lineWidth = DpiScalingHelper.ScaleValue(3f, _host);

            int availableWidth = bounds.Width - (padding * 2);
            int stepSpacing = stepCount > 1 ? availableWidth / (stepCount - 1) : 0;
            int startX = bounds.Left + padding;
            int centerY = bounds.Top + bounds.Height / 2 - DpiScalingHelper.ScaleValue(5, _host);

            // Draw connecting lines with animated fill
            if (stepCount > 1)
            {
                for (int i = 0; i < stepCount - 1; i++)
                {
                    int x1 = startX + (i * stepSpacing) + circleSize / 2;
                    int x2 = startX + ((i + 1) * stepSpacing) - circleSize / 2;
                    int y = centerY;
                    bool isComplete = i < currentIndex;
                    int lineLen = x2 - x1;

                    // Base line (always grey)
                    using (var pen = new Pen(_lineColor, lineWidth))
                        g.DrawLine(pen, x1, y, x2, y);

                    // Completed overlay — animated fill from left
                    if (isComplete && _connectorAnimProgress < 1f)
                    {
                        int fillX = x1 + (int)(lineLen * _connectorAnimProgress);
                        using (var pen = new Pen(_completedColor, lineWidth))
                            g.DrawLine(pen, x1, y, fillX, y);
                    }
                    else if (isComplete)
                    {
                        using (var pen = new Pen(_completedColor, lineWidth))
                            g.DrawLine(pen, x1, y, x2, y);
                    }
                }
            }

            // Draw step circles and labels
            for (int i = 0; i < stepCount; i++)
            {
                int x = startX + (i * stepSpacing);
                var step = steps[i];
                var state = step.State;

                // Determine colors
                Color circleColor;
                Color circleBorderColor;
                Color innerColor;

                if (i < currentIndex || state == StepState.Completed)
                {
                    circleColor = _completedColor;
                    circleBorderColor = _completedColor;
                    innerColor = Color.White;
                }
                else if (i == currentIndex)
                {
                    circleColor = _currentColor;
                    circleBorderColor = _currentColor;
                    innerColor = Color.White;
                }
                else
                {
                    circleColor = Color.Transparent;
                    circleBorderColor = _pendingColor;
                    innerColor = _pendingColor;
                }

                var circleRect = new Rectangle(x - circleSize / 2, centerY - circleSize / 2, circleSize, circleSize);

                // Drop shadow for active step
                if (i == currentIndex)
                {
                    int so = DpiScalingHelper.ScaleValue(2, _host);
                    var sr = new Rectangle(circleRect.X + so, circleRect.Y + so, circleRect.Width, circleRect.Height);
                    using (var sp = CreateRoundedRectangle(sr, circleSize / 2))
                    using (var sb = new SolidBrush(Color.FromArgb(40, Color.Black)))
                        g.FillPath(sb, sp);
                }

                // Draw circle background
                using (var path = CreateRoundedRectangle(circleRect, circleSize / 2))
                {
                    if (circleColor != Color.Transparent)
                    {
                        if (i == currentIndex)
                        {
                            var lc = Color.FromArgb(220, Color.White);
                            using (var gb = new System.Drawing.Drawing2D.LinearGradientBrush(circleRect, circleColor, lc, 315f))
                                g.FillPath(gb, path);
                        }
                        else
                        {
                            using (var brush = new SolidBrush(circleColor))
                                g.FillPath(brush, path);
                        }
                    }

                    using (var pen = new Pen(circleBorderColor, 2f))
                        g.DrawPath(pen, path);
                }

                // Draw content (SVG icon, checkmark, or number)
                if (!string.IsNullOrEmpty(step.Icon))
                {
                    int iconPad = DpiScalingHelper.ScaleValue(6, _host);
                    var iconRect = new Rectangle(circleRect.X + iconPad, circleRect.Y + iconPad,
                        circleRect.Width - iconPad * 2, circleRect.Height - iconPad * 2);
                    StyledImagePainter.PaintWithTint(g, iconRect, step.Icon, innerColor, 1f);
                }
                else if (state == StepState.Completed || i < currentIndex)
                {
                    DrawCheckmark(g, circleRect, innerColor);
                }
                else if (state == StepState.Error)
                {
                    DrawErrorX(g, circleRect, WizardHelpers.GetErrorColor(_theme));
                }
                else
                {
                    var numText = (i + 1).ToString();
                    TextUtils.DrawText(g, numText, _titleFont, circleRect, innerColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                // Draw step title below
                int halfLabelW = labelWidth / 2;
                var labelRect = new Rectangle(x - halfLabelW, centerY + circleSize / 2 + labelOffset,
                    labelWidth, labelHeight);
                var labelColor = i == currentIndex ? _textColor : _subtextColor;
                var font = i == currentIndex ? _titleFont : _labelFont;
                TextUtils.DrawText(g, step.Title ?? $"Step {i + 1}", font, labelRect, labelColor,
                    TextFormatFlags.HorizontalCenter);

                // Draw description for current step
                if (i == currentIndex && !string.IsNullOrEmpty(step.Description))
                {
                    int descHeight = DpiScalingHelper.ScaleValue(16, _host);
                    var descRect = new Rectangle(x - halfLabelW, labelRect.Bottom, labelWidth, descHeight);
                    TextUtils.DrawText(g, step.Description, _labelFont, descRect, _subtextColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
                }
            }
        }

        #endregion

        #region Helpers

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            float penWidth = DpiScalingHelper.ScaleValue(2.5f, _host);
            int size = DpiScalingHelper.ScaleValue(8, _host);
            using (var pen = new Pen(color, penWidth))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;

                g.DrawLines(pen, new Point[]
                {
                    new Point(cx - size, cy),
                    new Point(cx - size / 3, cy + size / 2),
                    new Point(cx + size, cy - size / 2)
                });
            }
        }

        private void DrawErrorX(Graphics g, Rectangle rect, Color color)
        {
            float penWidth = DpiScalingHelper.ScaleValue(2.5f, _host);
            int size = DpiScalingHelper.ScaleValue(6, _host);
            using (var pen = new Pen(color, penWidth))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;

                g.DrawLine(pen, cx - size, cy - size, cx + size, cy + size);
                g.DrawLine(pen, cx + size, cy - size, cx - size, cy + size);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
