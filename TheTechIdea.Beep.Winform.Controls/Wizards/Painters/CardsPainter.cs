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
    /// Painter for card-based wizard style with clickable step cards.
    /// </summary>
    public class CardsPainter : IWizardPainter
    {
        private Control _host;
        private IBeepTheme _theme;

        // Cached colors
        private Color _completedColor;
        private Color _currentColor;
        private Color _pendingColor;
        private Color _textColor;
        private Color _subtextColor;

        // Cached fonts
        private Font _cardTitleFont;
        private Font _cardDescFont;
        private Font _cardNumberFont;

        public void Initialize(Control host, IBeepTheme theme, WizardInstance instance)
        {
            _host = host;
            _theme = theme;

            if (theme != null)
            {
                _completedColor = theme.SuccessColor;
                _currentColor = theme.PrimaryColor;
                _pendingColor = Color.FromArgb(80, theme.ForeColor);
                _textColor = theme.ForeColor;
                _subtextColor = Color.FromArgb(128, theme.ForeColor);
            }
            else
            {
                _completedColor = ColorUtils.MapSystemColor(SystemColors.Highlight);
                _currentColor = ColorUtils.MapSystemColor(SystemColors.HotTrack);
                _pendingColor = Color.FromArgb(80, ColorUtils.MapSystemColor(SystemColors.GrayText));
                _textColor = ColorUtils.MapSystemColor(SystemColors.WindowText);
                _subtextColor = Color.FromArgb(128, ColorUtils.MapSystemColor(SystemColors.GrayText));
            }

            _cardTitleFont?.Dispose();
            _cardTitleFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 10f, FontStyle.Bold);
            _cardDescFont?.Dispose();
            _cardDescFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 8.5f, FontStyle.Regular);
            _cardNumberFont?.Dispose();
            _cardNumberFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 12f, FontStyle.Bold);
        }

        public Rectangle GetContentBounds(Rectangle formBounds)
        {
            int cardW = DpiScalingHelper.ScaleValue(220, _host);
            int pad = DpiScalingHelper.ScaleValue(30, _host);
            int btnH = DpiScalingHelper.ScaleValue(70, _host);
            return new Rectangle(formBounds.Left + cardW + pad, formBounds.Top + pad,
                formBounds.Width - cardW - pad * 2, formBounds.Height - btnH - pad);
        }

        public Rectangle GetStepIndicatorBounds(Rectangle formBounds)
        {
            int cardW = DpiScalingHelper.ScaleValue(220, _host);
            return new Rectangle(formBounds.Left, formBounds.Top, cardW, formBounds.Height);
        }

        public void PaintStepIndicators(Graphics g, Rectangle bounds, int currentIndex, IList<WizardStep> steps)
        {
            // CardsPainter is unique — it paints individual cards, not a single indicator area.
            // The per-card painting is handled by CardsWizardForm.StepCard_Paint.
            // This method provides the overall card panel background.
            if (_theme != null)
            {
                using var brush = new SolidBrush(ColorUtils.ShiftLuminance(_theme.BackColor, -0.03f));
                g.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Paint a single step card. Called from CardsWizardForm.StepCard_Paint.
        /// </summary>
        public void PaintStepCard(Graphics g, Rectangle bounds, WizardStep step, int stepIndex,
            int currentIndex, Color? cardBgColor = null)
        {
            bool isCurrent = stepIndex == currentIndex;
            bool isCompleted = step.State == StepState.Completed || stepIndex < currentIndex;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Card background
            Color bg = isCurrent ? Color.FromArgb(15, _currentColor) : Color.Transparent;
            if (cardBgColor.HasValue && !isCurrent) bg = cardBgColor.Value;
            using (var bgBrush = new SolidBrush(bg))
                g.FillRectangle(bgBrush, bounds);

            // DPI-scaled values
            int circleSize = DpiScalingHelper.ScaleValue(30, _host);
            int circleX = bounds.Left + DpiScalingHelper.ScaleValue(14, _host);
            int circleY = bounds.Top + (bounds.Height - circleSize) / 2;
            var circleRect = new Rectangle(circleX, circleY, circleSize, circleSize);
            int textX = circleX + circleSize + DpiScalingHelper.ScaleValue(12, _host);
            int textWidth = bounds.Width - textX - DpiScalingHelper.ScaleValue(10, _host);
            float penW = DpiScalingHelper.ScaleValue(2f, _host);

            // Accent bar for current
            if (isCurrent)
            {
                int aTop = bounds.Top + DpiScalingHelper.ScaleValue(8, _host);
                int aH = bounds.Height - DpiScalingHelper.ScaleValue(16, _host);
                using var accentBrush = new SolidBrush(_currentColor);
                g.FillRectangle(accentBrush, bounds.Left, aTop, DpiScalingHelper.ScaleValue(4, _host), aH);
            }

            // Circle
            Color circleColor, innerColor;
            if (isCompleted) { circleColor = _completedColor; innerColor = ColorUtils.GetContrastColor(_completedColor); }
            else if (isCurrent) { circleColor = _currentColor; innerColor = ColorUtils.GetContrastColor(_currentColor); }
            else { circleColor = _pendingColor; innerColor = _pendingColor; }

            if (isCompleted || isCurrent)
            { using var b = new SolidBrush(circleColor); g.FillEllipse(b, circleRect); }
            else
            { using var p = new Pen(circleColor, penW); g.DrawEllipse(p, circleRect); }

            // Content: checkmark or number
            if (isCompleted)
            {
                using var pen = new Pen(innerColor, penW);
                pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round; pen.LineJoin = LineJoin.Round;
                int cx = circleRect.X + circleRect.Width / 2, cy = circleRect.Y + circleRect.Height / 2;
                int cs = DpiScalingHelper.ScaleValue(5, _host);
                g.DrawLines(pen, new Point[] { new(cx - cs, cy), new(cx - cs * 2 / 5, cy + cs * 4 / 5), new(cx + cs * 6 / 5, cy - cs * 4 / 5) });
            }
            else
            {
                TextUtils.DrawText(g, (stepIndex + 1).ToString(), _cardNumberFont, circleRect, innerColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            // Title
            int titleH = DpiScalingHelper.ScaleValue(18, _host);
            var titleColor = isCurrent ? _textColor : _subtextColor;
            var titleFont = isCurrent ? _cardTitleFont : _cardDescFont;
            TextUtils.DrawText(g, step.Title ?? $"Step {stepIndex + 1}", titleFont,
                new Rectangle(textX, circleY, textWidth, titleH), titleColor);

            // Description
            if (!string.IsNullOrEmpty(step.Description))
            {
                int descTop = circleY + DpiScalingHelper.ScaleValue(20, _host);
                int descH = DpiScalingHelper.ScaleValue(16, _host);
                TextUtils.DrawText(g, step.Description, _cardDescFont,
                    new Rectangle(textX, descTop, textWidth, descH), _subtextColor);
            }

            // Optional badge
            if (step.IsOptional && !isCompleted)
            {
                int badgeTop = circleY + DpiScalingHelper.ScaleValue(36, _host);
                int badgeH = DpiScalingHelper.ScaleValue(14, _host);
                using var optFont = WizardHelpers.GetFont(_theme, _theme?.CaptionStyle, 7f, FontStyle.Italic);
                TextUtils.DrawText(g, "Optional", optFont,
                    new Rectangle(textX, badgeTop, textWidth, badgeH), Color.FromArgb(60, _pendingColor));
            }
        }

        /// <summary>Gets the completed color (for card hover effects).</summary>
        public Color CompletedColor => _completedColor;
        /// <summary>Gets the current color (for card hover effects).</summary>
        public Color CurrentColor => _currentColor;
    }
}
