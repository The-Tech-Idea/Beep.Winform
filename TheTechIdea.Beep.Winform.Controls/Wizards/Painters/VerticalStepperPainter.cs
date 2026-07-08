using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Painters
{
    /// <summary>
    /// Painter for vertical stepper wizard style with left-side timeline
    /// </summary>
    public class VerticalStepperPainter : IWizardPainter
    {
        #region Fields

        private Control _host;
        private IBeepTheme _theme;
        private WizardInstance _instance;

        private Color _completedColor;
        private Color _currentColor;
        private Color _pendingColor;
        private Color _lineColor;
        private Color _textColor;
        private Color _subtextColor;
        private Color _sidePanelColor;

        private Font _titleFont;
        private Font _descFont;
        private Font _numberFont;

        private float _connectorAnimProgress = 1f;
        private System.Windows.Forms.Timer _animTimer;
        private System.Diagnostics.Stopwatch _animStopwatch;
        public void StartConnectorAnimation(int targetIndex, int durationMs = 300)
        {
            if (_host == null || WizardManager.ReducedMotion) { _connectorAnimProgress = 1f; _host?.Invalidate(); return; }
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
                _sidePanelColor = ColorUtils.ShiftLuminance(theme.BackColor, -0.05f);
            }
            else
            {
                _completedColor = ColorUtils.MapSystemColor(SystemColors.Highlight);
                _currentColor = ColorUtils.MapSystemColor(SystemColors.HotTrack);
                _pendingColor = Color.FromArgb(80, ColorUtils.MapSystemColor(SystemColors.GrayText));
                _lineColor = Color.FromArgb(40, ColorUtils.MapSystemColor(SystemColors.ControlDark));
                _textColor = ColorUtils.MapSystemColor(SystemColors.WindowText);
                _subtextColor = Color.FromArgb(128, ColorUtils.MapSystemColor(SystemColors.GrayText));
                _sidePanelColor = ColorUtils.ShiftLuminance(ColorUtils.MapSystemColor(SystemColors.Window), -0.03f);
            }

            _titleFont?.Dispose();
            _titleFont = WizardHelpers.GetFont(theme, theme?.TitleStyle, 11f, FontStyle.Bold);
            _descFont?.Dispose();
            _descFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 9f, FontStyle.Regular);
            _numberFont?.Dispose();
            _numberFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 10f, FontStyle.Bold);
        }

        #endregion

        #region IWizardPainter

        public Rectangle GetContentBounds(Rectangle formBounds)
        {
            int left = DpiScalingHelper.ScaleValue(290, _host);
            int top = DpiScalingHelper.ScaleValue(20, _host);
            int right = DpiScalingHelper.ScaleValue(320, _host);
            int bottom = DpiScalingHelper.ScaleValue(100, _host);
            return new Rectangle(formBounds.Left + left, formBounds.Top + top,
                formBounds.Width - right, formBounds.Height - bottom);
        }

        public Rectangle GetStepIndicatorBounds(Rectangle formBounds)
        {
            int width = DpiScalingHelper.ScaleValue(280, _host);
            return new Rectangle(formBounds.Left, formBounds.Top, width, formBounds.Height);
        }

        public void PaintStepIndicators(Graphics g, Rectangle bounds, int currentIndex, IList<WizardStep> steps)
            => PaintStepTimeline(g, bounds, currentIndex, steps);

        #endregion

        #region Paint Methods

        /// <summary>
        /// Paint vertical step timeline
        /// </summary>
        public void PaintStepTimeline(Graphics g, Rectangle bounds, int currentIndex, IList<WizardStep> steps)
        {
            if (steps == null || steps.Count == 0) return;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw side panel background
            using (var brush = new SolidBrush(_sidePanelColor))
            {
                g.FillRectangle(brush, bounds);
            }

            int stepCount = steps.Count;
            int circleSize = DpiScalingHelper.ScaleValue(32, _host);
            int leftMargin = DpiScalingHelper.ScaleValue(30, _host);
            int topMargin = DpiScalingHelper.ScaleValue(40, _host);
            int maxItemHeight = DpiScalingHelper.ScaleValue(80, _host);
            int itemHeight = Math.Min(maxItemHeight, (bounds.Height - topMargin * 2) / stepCount);
            float penWidth = DpiScalingHelper.ScaleValue(2f, _host);
            int textOffset = DpiScalingHelper.ScaleValue(15, _host);
            int titleYOffset = DpiScalingHelper.ScaleValue(4, _host);
            int descYOffset = DpiScalingHelper.ScaleValue(24, _host);
            int descHeight = DpiScalingHelper.ScaleValue(20, _host);

            // Draw title
            int titleY = DpiScalingHelper.ScaleValue(15, _host);
            TextUtils.DrawText(g, _instance?.Config?.Title ?? "Wizard", _titleFont,
                new Point(leftMargin, titleY), _textColor);

            // Draw connecting lines first
            for (int i = 0; i < stepCount - 1; i++)
            {
                int y1 = topMargin + (i * itemHeight) + circleSize;
                int y2 = topMargin + ((i + 1) * itemHeight);
                int x = leftMargin + circleSize / 2;

                var lineColor = i < currentIndex ? _completedColor : _lineColor;
                using (var pen = new Pen(lineColor, penWidth))
                    g.DrawLine(pen, x, y1, x, y2);
            }

            // Draw step items
            for (int i = 0; i < stepCount; i++)
            {
                int y = topMargin + (i * itemHeight);
                var step = steps[i];
                var state = step.State;

                Color circleColor, circleBorderColor, innerColor;
                if (i < currentIndex || state == StepState.Completed)
                { circleColor = _completedColor; circleBorderColor = _completedColor; innerColor = Color.White; }
                else if (i == currentIndex)
                { circleColor = _currentColor; circleBorderColor = _currentColor; innerColor = Color.White; }
                else
                { circleColor = Color.Transparent; circleBorderColor = _pendingColor; innerColor = _pendingColor; }

                var circleRect = new Rectangle(leftMargin, y, circleSize, circleSize);

                using (var path = CreateCirclePath(circleRect))
                {
                    if (circleColor != Color.Transparent)
                        using (var brush = new SolidBrush(circleColor))
                            g.FillPath(brush, path);
                    using (var pen = new Pen(circleBorderColor, penWidth))
                        g.DrawPath(pen, path);
                }

                if (state == StepState.Completed || i < currentIndex)
                    DrawCheckmark(g, circleRect, innerColor);
                else if (state == StepState.Error)
                    DrawErrorX(g, circleRect, WizardHelpers.GetErrorColor(_theme));
                else if (state == StepState.Skipped)
                    DrawSkipIcon(g, circleRect, innerColor);
                else
                {
                    var numText = (i + 1).ToString();
                    TextUtils.DrawText(g, numText, _numberFont, circleRect, innerColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                int textX = leftMargin + circleSize + textOffset;
                int textWidth = bounds.Width - textX - textOffset;
                var titleColor = i == currentIndex ? _textColor : _subtextColor;
                var titleAlpha = i == currentIndex ? 255 : 180;
                var font = i == currentIndex ? _titleFont : _descFont;
                TextUtils.DrawText(g, step.Title ?? $"Step {i + 1}", font,
                    new Point(textX, y + titleYOffset), Color.FromArgb(titleAlpha, titleColor));

                if (!string.IsNullOrEmpty(step.Description) && i == currentIndex)
                {
                    var descRect = new Rectangle(textX, y + descYOffset, textWidth, descHeight);
                    TextUtils.DrawText(g, step.Description, _descFont, descRect, _subtextColor);
                }
            }
        }

        #endregion

        #region Helpers

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            float pw = DpiScalingHelper.ScaleValue(2f, _host);
            int size = DpiScalingHelper.ScaleValue(6, _host);
            using (var pen = new Pen(color, pw))
            {
                pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round; pen.LineJoin = LineJoin.Round;
                int cx = rect.X + rect.Width / 2, cy = rect.Y + rect.Height / 2;
                g.DrawLines(pen, new Point[] { new(cx - size, cy), new(cx - size / 3, cy + size / 2), new(cx + size, cy - size / 2) });
            }
        }

        private void DrawErrorX(Graphics g, Rectangle rect, Color color)
        {
            float pw = DpiScalingHelper.ScaleValue(2f, _host);
            int size = DpiScalingHelper.ScaleValue(5, _host);
            using (var pen = new Pen(color, pw))
            {
                pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                int cx = rect.X + rect.Width / 2, cy = rect.Y + rect.Height / 2;
                g.DrawLine(pen, cx - size, cy - size, cx + size, cy + size);
                g.DrawLine(pen, cx + size, cy - size, cx - size, cy + size);
            }
        }

        private void DrawSkipIcon(Graphics g, Rectangle rect, Color color)
        {
            float pw = DpiScalingHelper.ScaleValue(2f, _host);
            int size = DpiScalingHelper.ScaleValue(4, _host);
            using (var pen = new Pen(color, pw))
            {
                pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                int cx = rect.X + rect.Width / 2, cy = rect.Y + rect.Height / 2;
                g.DrawLine(pen, cx - size, cy, cx + size, cy);
            }
        }

        private GraphicsPath CreateCirclePath(Rectangle rect)
        {
            var path = new GraphicsPath();
            path.AddEllipse(rect);
            return path;
        }

        #endregion
    }
}
