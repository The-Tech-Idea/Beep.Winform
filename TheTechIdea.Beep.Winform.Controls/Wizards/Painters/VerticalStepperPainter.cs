using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

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
                _sidePanelColor = ControlPaint.Dark(theme.BackColor, 0.05f);
            }
            else
            {
                _completedColor = Color.FromArgb(46, 125, 50);
                _currentColor = Color.FromArgb(25, 118, 210);
                _pendingColor = Color.FromArgb(150, 150, 150);
                _lineColor = Color.FromArgb(220, 220, 220);
                _textColor = Color.FromArgb(50, 50, 50);
                _subtextColor = Color.FromArgb(120, 120, 120);
                _sidePanelColor = Color.FromArgb(245, 245, 250);
            }

            _titleFont = new Font("Segoe UI Semibold", 11f);
            _descFont = new Font("Segoe UI", 9f);
            _numberFont = new Font("Segoe UI Semibold", 10f);
        }

        #endregion

        #region IWizardPainter

        public Rectangle GetContentBounds(Rectangle formBounds)
        {
            return new Rectangle(
                formBounds.Left + 290,
                formBounds.Top + 20,
                formBounds.Width - 320,
                formBounds.Height - 100
            );
        }

        public Rectangle GetStepIndicatorBounds(Rectangle formBounds)
        {
            return new Rectangle(
                formBounds.Left,
                formBounds.Top,
                280,
                formBounds.Height
            );
        }

        #endregion

        #region Paint Methods

        /// <summary>
        /// Paint vertical step timeline
        /// </summary>
        public void PaintStepTimeline(Graphics g, Rectangle bounds, int currentIndex, IList<WizardStep> steps)
        {
            if (steps == null || steps.Count == 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw side panel background
            using (var brush = new SolidBrush(_sidePanelColor))
            {
                g.FillRectangle(brush, bounds);
            }

            int stepCount = steps.Count;
            int circleSize = 32;
            int leftMargin = 30;
            int topMargin = 40;
            int itemHeight = Math.Min(80, (bounds.Height - topMargin * 2) / stepCount);

            // Draw title
            using (var titleFont = new Font("Segoe UI Semibold", 14f))
            using (var brush = new SolidBrush(_textColor))
            {
                g.DrawString(_instance?.Config?.Title ?? "Wizard", titleFont, brush, leftMargin, 15);
            }

            // Draw connecting lines first
            for (int i = 0; i < stepCount - 1; i++)
            {
                int y1 = topMargin + (i * itemHeight) + circleSize;
                int y2 = topMargin + ((i + 1) * itemHeight);
                int x = leftMargin + circleSize / 2;

                var lineColor = i < currentIndex ? _completedColor : _lineColor;
                using (var pen = new Pen(lineColor, 2f))
                {
                    g.DrawLine(pen, x, y1, x, y2);
                }
            }

            // Draw step items
            for (int i = 0; i < stepCount; i++)
            {
                int y = topMargin + (i * itemHeight);
                var step = steps[i];
                var state = step.State;

                // Determine colors
                Color circleColor, circleBorderColor, innerColor;

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
                else if (state == StepState.Skipped)
                {
                    circleColor = Color.Transparent;
                    circleBorderColor = _pendingColor;
                    innerColor = _pendingColor;
                }
                else
                {
                    circleColor = Color.Transparent;
                    circleBorderColor = _pendingColor;
                    innerColor = _pendingColor;
                }

                var circleRect = new Rectangle(leftMargin, y, circleSize, circleSize);

                // Draw circle
                using (var path = CreateCirclePath(circleRect))
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

                // Draw content in circle
                if (state == StepState.Completed || i < currentIndex)
                {
                    DrawCheckmark(g, circleRect, innerColor);
                }
                else if (state == StepState.Error)
                {
                    DrawErrorX(g, circleRect, Color.FromArgb(200, 50, 50));
                }
                else if (state == StepState.Skipped)
                {
                    DrawSkipIcon(g, circleRect, innerColor);
                }
                else
                {
                    var numText = (i + 1).ToString();
                    using (var brush = new SolidBrush(innerColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(numText, _numberFont, brush, circleRect, sf);
                    }
                }

                // Draw title and description
                int textX = leftMargin + circleSize + 15;
                int textWidth = bounds.Width - textX - 15;

                var titleColor = i == currentIndex ? _textColor : _subtextColor;
                var titleAlpha = i == currentIndex ? 255 : 180;

                using (var brush = new SolidBrush(Color.FromArgb(titleAlpha, titleColor)))
                {
                    var font = i == currentIndex ? _titleFont : _descFont;
                    g.DrawString(step.Title ?? $"Step {i + 1}", font, brush, textX, y + 4);
                }

                if (!string.IsNullOrEmpty(step.Description) && i == currentIndex)
                {
                    using (var brush = new SolidBrush(_subtextColor))
                    {
                        var descRect = new Rectangle(textX, y + 24, textWidth, 20);
                        g.DrawString(step.Description, _descFont, brush, descRect);
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;
                int size = 6;

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
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;
                int size = 5;

                g.DrawLine(pen, cx - size, cy - size, cx + size, cy + size);
                g.DrawLine(pen, cx + size, cy - size, cx - size, cy + size);
            }
        }

        private void DrawSkipIcon(Graphics g, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int cx = rect.X + rect.Width / 2;
                int cy = rect.Y + rect.Height / 2;

                g.DrawLine(pen, cx - 4, cy, cx + 4, cy);
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
