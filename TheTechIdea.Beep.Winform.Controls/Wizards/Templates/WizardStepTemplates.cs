using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;


namespace TheTechIdea.Beep.Winform.Controls.Wizards.Templates
{
    /// <summary>
    /// Base class for wizard step templates
    /// Implements IWizardStepContent with default behaviors
    /// </summary>
    public abstract class WizardStepTemplateBase : UserControl, IWizardStepContent
    {
        protected WizardContext Context { get; private set; }

        public virtual bool IsComplete => true;
        public virtual string NextButtonText => null;

        public event EventHandler<StepValidationEventArgs> ValidationStateChanged;

        public virtual void OnStepEnter(WizardContext context)
        {
            Context = context;
            LoadData();
        }

        public virtual void OnStepLeave(WizardContext context)
        {
            SaveData();
        }

        public virtual WizardValidationResult Validate()
        {
            return WizardValidationResult.Success();
        }

        public virtual Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(Validate());
        }

        protected virtual void LoadData() { }
        protected virtual void SaveData() { }

        protected void RaiseValidationStateChanged(bool isValid, string message = null)
        {
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(isValid, message));
        }
    }

    /// <summary>
    /// Welcome step template - introductory step with icon and message
    /// </summary>
    public class WelcomeStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel;
        private BeepLabel _messageLabel;
        private Panel _iconPanel;

        public string WelcomeTitle { get; set; } = "Welcome";
        public string WelcomeMessage { get; set; } = "This wizard will guide you through the setup process.";
        public Image WelcomeIcon { get; set; }

        public WelcomeStepTemplate()
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(40, 40, 40, 40);

            // Icon panel
            _iconPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent
            };

            // Title
            _titleLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Text = WelcomeTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 24f, FontStyle.Bold)
            };

            // Message
            _messageLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Text = WelcomeMessage,
                Font = WizardHelpers.GetFont(theme, theme?.BodyStyle, 12f, FontStyle.Regular)
            };

            Controls.Add(_messageLabel);
            Controls.Add(_titleLabel);
            Controls.Add(_iconPanel);
        }

        protected override void LoadData()
        {
            _titleLabel.Text = WelcomeTitle;
            _messageLabel.Text = WelcomeMessage;
        }
    }

    /// <summary>
    /// Summary step template - displays collected data before completion
    /// </summary>
    public class SummaryStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel;
        private Panel _summaryPanel;

        public string SummaryTitle { get; set; } = "Summary";
        public Func<WizardContext, string> SummaryBuilder { get; set; }

        public SummaryStepTemplate()
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(20);

            _titleLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Text = SummaryTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 16f, FontStyle.Bold)
            };

            _summaryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(10)
            };

            Controls.Add(_summaryPanel);
            Controls.Add(_titleLabel);
        }

        protected override void LoadData()
        {
            _summaryPanel.Controls.Clear();

            if (SummaryBuilder != null && Context != null)
            {
                var summary = SummaryBuilder(Context);
                var label = new BeepLabel
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    Text = summary,
                    Font = WizardHelpers.GetFont(BeepThemesManager.CurrentTheme, BeepThemesManager.CurrentTheme?.BodyStyle, 10f, FontStyle.Regular)
                };
                _summaryPanel.Controls.Add(label);
            }
        }
    }

    /// <summary>
    /// Completion step template - success message after wizard completes
    /// </summary>
    public class CompletionStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel;
        private BeepLabel _messageLabel;
        private Panel _iconPanel;

        public string CompletionTitle { get; set; } = "Setup Complete";
        public string CompletionMessage { get; set; } = "The wizard has completed successfully. Click Finish to close.";
        public Image CompletionIcon { get; set; }

        public override string NextButtonText => "Finish";

        public CompletionStepTemplate()
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(40, 60, 40, 40);

            _iconPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.Transparent
            };
            _iconPanel.Paint += IconPanel_Paint;

            _titleLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Text = CompletionTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 22f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _messageLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Text = CompletionMessage,
                Font = WizardHelpers.GetFont(theme, theme?.BodyStyle, 11f, FontStyle.Regular),
                TextAlign = ContentAlignment.TopCenter
            };

            Controls.Add(_messageLabel);
            Controls.Add(_titleLabel);
            Controls.Add(_iconPanel);
        }

        private void IconPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw success checkmark
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int size = 60;
            int x = (_iconPanel.Width - size) / 2;
            int y = 10;
            var rect = new Rectangle(x, y, size, size);
            var theme = BeepThemesManager.CurrentTheme;

            using (var brush = new SolidBrush(theme?.SuccessColor ?? Color.FromArgb(46, 125, 50)))
            {
                g.FillEllipse(brush, rect);
            }

            using (var pen = new Pen(Color.White, 4f))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

                int cx = x + size / 2;
                int cy = y + size / 2;

                g.DrawLines(pen, new Point[]
                {
                    new Point(cx - 14, cy + 2),
                    new Point(cx - 4, cy + 12),
                    new Point(cx + 14, cy - 10)
                });
            }
        }

        protected override void LoadData()
        {
            _titleLabel.Text = CompletionTitle;
            _messageLabel.Text = CompletionMessage;
        }
    }

    /// <summary>
    /// Error step template - displays error when wizard fails
    /// </summary>
    public class ErrorStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel;
        private BeepLabel _messageLabel;
        private Panel _iconPanel;

        public string ErrorTitle { get; set; } = "An Error Occurred";
        public string ErrorMessage { get; set; } = "The wizard encountered an error and cannot continue.";

        public override string NextButtonText => "Close";

        public ErrorStepTemplate()
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(40, 60, 40, 40);

            _iconPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.Transparent
            };
            _iconPanel.Paint += IconPanel_Paint;

            _titleLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Text = ErrorTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 22f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = WizardHelpers.GetErrorColor(theme)
            };

            _messageLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Text = ErrorMessage,
                Font = WizardHelpers.GetFont(theme, theme?.BodyStyle, 11f, FontStyle.Regular),
                TextAlign = ContentAlignment.TopCenter
            };

            Controls.Add(_messageLabel);
            Controls.Add(_titleLabel);
            Controls.Add(_iconPanel);
        }

        private void IconPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int size = 60;
            int x = (_iconPanel.Width - size) / 2;
            int y = 10;
            var rect = new Rectangle(x, y, size, size);
            var theme = BeepThemesManager.CurrentTheme;

            using (var brush = new SolidBrush(WizardHelpers.GetErrorColor(theme)))
            {
                g.FillEllipse(brush, rect);
            }

            using (var pen = new Pen(Color.White, 4f))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                int cx = x + size / 2;
                int cy = y + size / 2;

                g.DrawLine(pen, cx - 12, cy - 12, cx + 12, cy + 12);
                g.DrawLine(pen, cx + 12, cy - 12, cx - 12, cy + 12);
            }
        }

        protected override void LoadData()
        {
            _titleLabel.Text = ErrorTitle;
            _messageLabel.Text = ErrorMessage;
        }
    }
}
