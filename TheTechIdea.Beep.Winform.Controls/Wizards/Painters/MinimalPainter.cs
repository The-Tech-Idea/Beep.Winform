using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

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
                _completedColor = Color.FromArgb(46, 125, 50);
                _currentColor = Color.FromArgb(25, 118, 210);
                _pendingColor = Color.FromArgb(180, 180, 180);
                _textColor = Color.FromArgb(50, 50, 50);
                _subtextColor = Color.FromArgb(120, 120, 120);
            }

            // Dispose old fonts before creating new ones to prevent memory leaks
            _titleFont?.Dispose();
            _stepFont?.Dispose();

            _titleFont = new Font("Segoe UI Semibold", 14f);
            _stepFont = new Font("Segoe UI", 10f);
        }

        #endregion

        #region IWizardPainter

        public Rectangle GetContentBounds(Rectangle formBounds)
        {
            return new Rectangle(
                formBounds.Left + 40,
                formBounds.Top + 70,
                formBounds.Width - 80,
                formBounds.Height - 140
            );
        }

        public Rectangle GetStepIndicatorBounds(Rectangle formBounds)
        {
            return new Rectangle(
                formBounds.Left,
                formBounds.Top,
                formBounds.Width,
                60
            );
        }

        #endregion

        #region Paint Methods

        /// <summary>
        /// Paint minimal progress indicator
        /// </summary>
        public void PaintMinimalProgress(Graphics g, Rectangle bounds, int currentIndex, int totalSteps, IList<WizardStep> steps)
        {
            if (steps == null || steps.Count == 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int dotSize = 10;
            int dotSpacing = 24;
            int totalWidth = (totalSteps * dotSize) + ((totalSteps - 1) * (dotSpacing - dotSize));
            int startX = bounds.Left + (bounds.Width - totalWidth) / 2;
            int centerY = bounds.Top + 25;

            // Draw dots
            for (int i = 0; i < totalSteps; i++)
            {
                int x = startX + (i * dotSpacing);
                var rect = new Rectangle(x, centerY - dotSize / 2, dotSize, dotSize);

                Color dotColor;
                if (i < currentIndex)
                {
                    dotColor = _completedColor;
                }
                else if (i == currentIndex)
                {
                    dotColor = _currentColor;
                }
                else
                {
                    dotColor = _pendingColor;
                }

                using (var brush = new SolidBrush(dotColor))
                {
                    g.FillEllipse(brush, rect);
                }

                // Draw larger ring around current
                if (i == currentIndex)
                {
                    var ringRect = new Rectangle(x - 3, centerY - dotSize / 2 - 3, dotSize + 6, dotSize + 6);
                    using (var pen = new Pen(Color.FromArgb(60, _currentColor), 2f))
                    {
                        g.DrawEllipse(pen, ringRect);
                    }
                }
            }

            // Draw step title and count
            var currentStep = currentIndex >= 0 && currentIndex < steps.Count ? steps[currentIndex] : null;
            var titleText = currentStep?.Title ?? $"Step {currentIndex + 1}";
            var countText = $"Step {currentIndex + 1} of {totalSteps}";

            using (var brush = new SolidBrush(_textColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(titleText, _titleFont, brush, bounds.Left + bounds.Width / 2, centerY + 15, sf);
            }

            using (var brush = new SolidBrush(_subtextColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Far })
            {
                g.DrawString(countText, _stepFont, brush, bounds.Right - 20, centerY - 5, sf);
            }
        }

        #endregion
    }
}
