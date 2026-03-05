using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Beep-themed progress dialog built entirely with Beep controls:
    /// <see cref="BeepPanel"/>, <see cref="BeepLabel"/>, <see cref="BeepProgressBar"/>,
    /// and <see cref="BeepButton"/>. Inherits <see cref="BeepiFormPro"/> for automatic
    /// theming and borderless rendering.
    /// </summary>
    internal class BeepProgressDialog : BeepiFormPro
    {
        // ── Layout panels ──────────────────────────────────────────────────
        private readonly BeepPanel _headerPanel;
        private readonly BeepPanel _bodyPanel;

        // ── Header ─────────────────────────────────────────────────────────
        private readonly BeepLabel _titleLabel;

        // ── Body (accessible by ProgressHandle for live updates) ──────────
        /// <summary>Status / message label.</summary>
        public BeepLabel MessageLabel { get; }

        /// <summary>The progress bar control.</summary>
        public BeepProgressBar ProgressBarControl { get; }

        /// <summary>Percentage text; null in indeterminate mode.</summary>
        public BeepLabel? PercentLabel { get; }

        /// <summary>Cancel button; null when not cancellable.</summary>
        public BeepButton? CancelButton { get; }

        // ──────────────────────────────────────────────────────────────────
        public BeepProgressDialog(string title, string? message, bool cancellable, bool indeterminate)
        {
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Text = title;
            ClientSize = new Size(400, cancellable ? 172 : 128);
            StartPosition = FormStartPosition.CenterParent;

            // ── Header panel ─────────────────────────────────────────────
            _headerPanel = new BeepPanel
            {
                Location = new Point(1, 1),
                Size = new Size(398, 42),
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
            };

            _titleLabel = new BeepLabel
            {
                Text = title,
                Location = new Point(12, 11),
                Size = new Size(374, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                UseThemeColors = true,
            };
            _headerPanel.Controls.Add(_titleLabel);

            // ── Body panel ───────────────────────────────────────────────
            int bodyHeight = cancellable ? 128 : 84;
            _bodyPanel = new BeepPanel
            {
                Location = new Point(1, 43),
                Size = new Size(398, bodyHeight),
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
            };

            // Message label (narrower when percent label is alongside)
            int msgWidth = indeterminate ? 374 : 308;
            MessageLabel = new BeepLabel
            {
                Text = message ?? "Please wait...",
                Location = new Point(12, 12),
                Size = new Size(msgWidth, 18),
                TextAlign = ContentAlignment.MiddleLeft,
                UseThemeColors = true,
            };
            _bodyPanel.Controls.Add(MessageLabel);

            // Percent label — only in determinate mode
            if (!indeterminate)
            {
                PercentLabel = new BeepLabel
                {
                    Text = "0%",
                    Location = new Point(328, 12),
                    Size = new Size(58, 18),
                    TextAlign = ContentAlignment.MiddleRight,
                    UseThemeColors = true,
                };
                _bodyPanel.Controls.Add(PercentLabel);
            }

            // Progress bar: DotsLoader for indeterminate, LinearBadge for determinate
            ProgressBarControl = new BeepProgressBar
            {
                Location = new Point(12, 38),
                Size = new Size(374, 26),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                PainterKind = indeterminate
                    ? ProgressPainterKind.DotsLoader
                    : ProgressPainterKind.LinearBadge,
                ProgressBarStyle = indeterminate
                    ? ProgressBars.ProgressBarStyle.Animated
                    : ProgressBars.ProgressBarStyle.Gradient,
                UseThemeColors = true,
                VisualMode = ProgressBars.ProgressBarDisplayMode.NoText,
            };
            _bodyPanel.Controls.Add(ProgressBarControl);

            // Optional cancel button
            if (cancellable)
            {
                CancelButton = new BeepButton
                {
                    Text = "Cancel",
                    Location = new Point(290, 80),
                    Size = new Size(96, 32),
                    UseThemeColors = true,
                };
                _bodyPanel.Controls.Add(CancelButton);
            }

            Controls.Add(_headerPanel);
            Controls.Add(_bodyPanel);
        }

        // ── Theme propagation ──────────────────────────────────────────────
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_headerPanel == null) return;

            _headerPanel.Theme = Theme;
            _titleLabel.Theme = Theme;

            _bodyPanel.Theme = Theme;
            MessageLabel.Theme = Theme;
            ProgressBarControl.Theme = Theme;
            if (PercentLabel != null) PercentLabel.Theme = Theme;
            if (CancelButton != null) CancelButton.Theme = Theme;
        }
    }
}
