using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Painters
{
    /// <summary>
    /// Interface for wizard painters
    /// </summary>
    public interface IWizardPainter
    {
        void Initialize(Control host, IBeepTheme theme, WizardInstance instance);
        Rectangle GetContentBounds(Rectangle formBounds);
        Rectangle GetStepIndicatorBounds(Rectangle formBounds);
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
                _completedColor = Color.FromArgb(46, 125, 50);
                _currentColor = Color.FromArgb(25, 118, 210);
                _pendingColor = Color.FromArgb(150, 150, 150);
                _lineColor = Color.FromArgb(200, 200, 200);
                _textColor = Color.FromArgb(50, 50, 50);
                _subtextColor = Color.FromArgb(120, 120, 120);
            }

            _titleFont = WizardHelpers.GetFont(theme, theme?.TitleStyle, 10f, FontStyle.Bold);
            _labelFont = WizardHelpers.GetFont(theme, theme?.BodyStyle, 9f, FontStyle.Regular);
        }

        #endregion

        #region IWizardPainter

        public Rectangle GetContentBounds(Rectangle formBounds)
        {
            return new Rectangle(
                formBounds.Left + 30,
                formBounds.Top + 110,
                formBounds.Width - 60,
                formBounds.Height - 180
            );
        }

        public Rectangle GetStepIndicatorBounds(Rectangle formBounds)
        {
            return new Rectangle(
                formBounds.Left,
                formBounds.Top,
                formBounds.Width,
                100
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

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int stepCount = steps.Count;
            int circleSize = 36;
            int padding = 50;
            int availableWidth = bounds.Width - (padding * 2);
            int stepSpacing = stepCount > 1 ? availableWidth / (stepCount - 1) : 0;
            int startX = bounds.Left + padding;
            int centerY = bounds.Top + bounds.Height / 2 - 5;

            // Draw connecting lines first
            if (stepCount > 1)
            {
                for (int i = 0; i < stepCount - 1; i++)
                {
                    int x1 = startX + (i * stepSpacing) + circleSize / 2;
                    int x2 = startX + ((i + 1) * stepSpacing) - circleSize / 2;
                    int y = centerY;

                    var lineColor = i < currentIndex ? _completedColor : _lineColor;
                    using (var pen = new Pen(lineColor, 3f))
                    {
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

                // Draw circle background
                using (var path = CreateRoundedRectangle(circleRect, circleSize / 2))
                {
                    if (circleColor != Color.Transparent)
                    {
                        using (var brush = new SolidBrush(circleColor))
                        {
                            g.FillPath(brush, path);
                        }
                    }

                    using (var pen = new Pen(circleBorderColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Draw content (checkmark or number)
                if (state == StepState.Completed || i < currentIndex)
                {
                    // Draw checkmark
                    DrawCheckmark(g, circleRect, innerColor);
                }
                else if (state == StepState.Error)
                {
                    // Draw X for error
                    DrawErrorX(g, circleRect, WizardHelpers.GetErrorColor(_theme));
                }
                else
                {
                    // Draw step number
                    var numText = (i + 1).ToString();
                    using (var brush = new SolidBrush(innerColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(numText, _titleFont, brush, circleRect, sf);
                    }
                }

                // Draw step title below
                var labelRect = new Rectangle(x - 60, centerY + circleSize / 2 + 8, 120, 24);
                var labelColor = i == currentIndex ? _textColor : _subtextColor;
                using (var brush = new SolidBrush(labelColor))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                {
                    var font = i == currentIndex ? _titleFont : _labelFont;
                    g.DrawString(step.Title ?? $"Step {i + 1}", font, brush, labelRect, sf);
                }
            }
        }

        #endregion

        #region Helpers

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;
                int size = 8;

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
            using (var pen = new Pen(color, 2.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;
                int size = 6;

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
