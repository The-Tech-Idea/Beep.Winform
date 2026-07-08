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
    /// Painter for minimal wizard style with simple progress indicator
    /// </summary>
    public class MinimalPainter : IWizardPainter
    {
        #region Fields

        private Control _host;
        private IBeepTheme _theme;
        private WizardInstance _instance;

        private Color _completedColor;
        private Color _currentColor;
        private Color _pendingColor;
        private Color _textColor;
        private Color _subtextColor;

        private Font _titleFont;
        private Font _stepFont;

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
                _pendingColor = Color.FromArgb(60, theme.ForeColor);
                _textColor = theme.ForeColor;
                _subtextColor = Color.FromArgb(128, theme.ForeColor);
            }
            else
            {
                _completedColor = ColorUtils.MapSystemColor(SystemColors.Highlight);
                _currentColor = ColorUtils.MapSystemColor(SystemColors.HotTrack);
                _pendingColor = Color.FromArgb(60, ColorUtils.MapSystemColor(SystemColors.GrayText));
                _textColor = ColorUtils.MapSystemColor(SystemColors.WindowText);
                _subtextColor = Color.FromArgb(128, ColorUtils.MapSystemColor(SystemColors.GrayText));
            }

            _titleFont?.Dispose();
            _titleFont = WizardHelpers.GetFont(theme, theme?.TitleStyle, 14f, FontStyle.Bold);
            _stepFont?.Dispose();
            _stepFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 10f, FontStyle.Regular);
        }

        #endregion

        #region IWizardPainter

        public Rectangle GetContentBounds(Rectangle formBounds)
        {
            int l = DpiScalingHelper.ScaleValue(40, _host);
            int t = DpiScalingHelper.ScaleValue(70, _host);
            int r = DpiScalingHelper.ScaleValue(80, _host);
            int b = DpiScalingHelper.ScaleValue(140, _host);
            return new Rectangle(formBounds.Left + l, formBounds.Top + t, formBounds.Width - r, formBounds.Height - b);
        }

        public Rectangle GetStepIndicatorBounds(Rectangle formBounds)
        {
            return new Rectangle(formBounds.Left, formBounds.Top, formBounds.Width, DpiScalingHelper.ScaleValue(60, _host));
        }

        public void PaintStepIndicators(Graphics g, Rectangle bounds, int currentIndex, IList<WizardStep> steps)
            => PaintMinimalProgress(g, bounds, currentIndex, steps.Count, steps);

        #endregion

        #region Paint Methods

        /// <summary>
        /// Paint minimal progress indicator
        /// </summary>
        public void PaintMinimalProgress(Graphics g, Rectangle bounds, int currentIndex, int totalSteps, IList<WizardStep> steps)
        {
            if (steps == null || steps.Count == 0) return;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int dotSize = DpiScalingHelper.ScaleValue(10, _host);
            int dotSpacing = DpiScalingHelper.ScaleValue(24, _host);
            int ringOffset = DpiScalingHelper.ScaleValue(3, _host);
            int ringExtra = DpiScalingHelper.ScaleValue(6, _host);
            float ringPenWidth = DpiScalingHelper.ScaleValue(2f, _host);
            int centerY = bounds.Top + DpiScalingHelper.ScaleValue(25, _host);
            int titleYOffset = DpiScalingHelper.ScaleValue(15, _host);
            int titleHeight = DpiScalingHelper.ScaleValue(25, _host);
            int countYOffset = DpiScalingHelper.ScaleValue(-5, _host);
            int countWidthPad = DpiScalingHelper.ScaleValue(20, _host);
            int countHeight = DpiScalingHelper.ScaleValue(20, _host);

            int totalWidth = (totalSteps * dotSize) + ((totalSteps - 1) * (dotSpacing - dotSize));
            int startX = bounds.Left + (bounds.Width - totalWidth) / 2;

            for (int i = 0; i < totalSteps; i++)
            {
                int x = startX + (i * dotSpacing);
                var rect = new Rectangle(x, centerY - dotSize / 2, dotSize, dotSize);
                Color dotColor = i < currentIndex ? _completedColor : i == currentIndex ? _currentColor : _pendingColor;
                using (var brush = new SolidBrush(dotColor))
                    g.FillEllipse(brush, rect);

                if (i == currentIndex)
                {
                    var ringRect = new Rectangle(x - ringOffset, centerY - dotSize / 2 - ringOffset, dotSize + ringExtra, dotSize + ringExtra);
                    using (var pen = new Pen(Color.FromArgb(60, _currentColor), ringPenWidth))
                        g.DrawEllipse(pen, ringRect);
                }
            }

            // Radial progress arc (when enabled)
            if (_instance?.Config?.ShowRadialProgress == true)
            {
                int arcSize = DpiScalingHelper.ScaleValue(60, _host);
                int arcX = bounds.Left + (bounds.Width - arcSize) / 2;
                int arcY = centerY - arcSize / 2;
                float sweep = totalSteps > 0 ? 360f * (currentIndex + 1) / totalSteps : 0f;
                using (var arcPen = new Pen(_completedColor, DpiScalingHelper.ScaleValue(4f, _host)))
                    g.DrawArc(arcPen, arcX, arcY, arcSize, arcSize, -90f, sweep);
                using (var trackPen = new Pen(_pendingColor, DpiScalingHelper.ScaleValue(4f, _host)))
                    g.DrawArc(trackPen, arcX, arcY, arcSize, arcSize, -90f + sweep, 360f - sweep);
            }

            var currentStep = currentIndex >= 0 && currentIndex < steps.Count ? steps[currentIndex] : null;
            var titleText = currentStep?.Title ?? $"Step {currentIndex + 1}";
            var countText = $"Step {currentIndex + 1} of {totalSteps}";

            TextUtils.DrawText(g, titleText, _titleFont,
                new Rectangle(bounds.Left, centerY + titleYOffset, bounds.Width, titleHeight), _textColor, TextFormatFlags.HorizontalCenter);

            TextUtils.DrawText(g, countText, _stepFont,
                new Rectangle(bounds.Left, centerY + countYOffset, bounds.Width - countWidthPad, countHeight), _subtextColor, TextFormatFlags.Right);
        }

        #endregion
    }
}
