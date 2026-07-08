using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Templates
{
    public abstract class WizardStepTemplateBase : UserControl, IWizardStepContent
    {
        protected WizardContext Context { get; private set; }
        public virtual bool IsComplete => true;
        public virtual string NextButtonText => null;
        public event EventHandler<StepValidationEventArgs> ValidationStateChanged;

        public virtual void OnStepEnter(WizardContext context) { Context = context; LoadData(); }
        public virtual void OnStepLeave(WizardContext context) { SaveData(); }
        public virtual WizardValidationResult Validate() => WizardValidationResult.Success();
        public virtual Task<WizardValidationResult> ValidateAsync() => Task.FromResult(Validate());
        protected virtual void LoadData() { }
        protected virtual void SaveData() { }

        protected void RaiseValidationStateChanged(bool isValid, string message = null)
            => ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(isValid, message));

        /// <summary>DPI-scale a logical pixel value.</summary>
        protected int S(int v) => DpiScalingHelper.ScaleValue(v, this);
    }

    public class WelcomeStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel, _messageLabel;
        private Panel _iconPanel;

        public string WelcomeTitle { get; set; } = "Welcome";
        public string WelcomeMessage { get; set; } = "This wizard will guide you through the setup process.";
        public Image WelcomeIcon { get; set; }

        public WelcomeStepTemplate() { InitializeControls(); }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(S(40));

            _iconPanel = new Panel { Dock = DockStyle.Top, Height = S(100), BackColor = Color.Transparent };
            _titleLabel = new BeepLabel { Dock = DockStyle.Top, Height = S(50), Text = WelcomeTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 24f, FontStyle.Bold) };
            _messageLabel = new BeepLabel { Dock = DockStyle.Top, Height = S(100), Text = WelcomeMessage,
                Font = WizardHelpers.GetFont(theme, theme?.BodyStyle, 12f, FontStyle.Regular) };

            Controls.Add(_messageLabel);
            Controls.Add(_titleLabel);
            Controls.Add(_iconPanel);
        }

        protected override void LoadData() { _titleLabel.Text = WelcomeTitle; _messageLabel.Text = WelcomeMessage; }

        // Fonts are managed by BeepThemesManager — do NOT dispose them.
        protected override void Dispose(bool disposing)
        {
            if (disposing) { _titleLabel = null; _messageLabel = null; }
            base.Dispose(disposing);
        }
    }

    public class SummaryStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel;
        private Panel _summaryPanel;

        public string SummaryTitle { get; set; } = "Summary";
        public Func<WizardContext, string> SummaryBuilder { get; set; }

        public SummaryStepTemplate() { InitializeControls(); }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(S(20));

            _titleLabel = new BeepLabel { Dock = DockStyle.Top, Height = S(40), Text = SummaryTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 16f, FontStyle.Bold) };
            _summaryPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.Transparent, Padding = new Padding(S(10)) };

            Controls.Add(_summaryPanel);
            Controls.Add(_titleLabel);
        }

        protected override void LoadData()
        {
            // Clear previous labels to prevent control leak
            for (int i = _summaryPanel.Controls.Count - 1; i >= 0; i--)
                _summaryPanel.Controls[i].Dispose();
            _summaryPanel.Controls.Clear();

            if (SummaryBuilder != null && Context != null)
            {
                var summary = SummaryBuilder(Context);
                var label = new BeepLabel { Dock = DockStyle.Top, AutoSize = true, Text = summary,
                    Font = WizardHelpers.GetFont(BeepThemesManager.CurrentTheme, BeepThemesManager.CurrentTheme?.BodyStyle, 10f, FontStyle.Regular) };
                _summaryPanel.Controls.Add(label);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _titleLabel = null;
            base.Dispose(disposing);
        }
    }

    public class CompletionStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel, _messageLabel;
        private Panel _iconPanel;

        public string CompletionTitle { get; set; } = "Setup Complete";
        public string CompletionMessage { get; set; } = "The wizard has completed successfully. Click Finish to close.";
        public Image CompletionIcon { get; set; }
        public override string NextButtonText => "Finish";

        public CompletionStepTemplate() { InitializeControls(); }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(S(40), S(60), S(40), S(40));

            _iconPanel = new Panel { Dock = DockStyle.Top, Height = S(80), BackColor = Color.Transparent };
            _iconPanel.Paint += IconPanel_Paint;
            _titleLabel = new BeepLabel { Dock = DockStyle.Top, Height = S(50), Text = CompletionTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 22f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            _messageLabel = new BeepLabel { Dock = DockStyle.Top, Height = S(60), Text = CompletionMessage,
                Font = WizardHelpers.GetFont(theme, theme?.BodyStyle, 11f, FontStyle.Regular), TextAlign = ContentAlignment.TopCenter };

            Controls.Add(_messageLabel);
            Controls.Add(_titleLabel);
            Controls.Add(_iconPanel);
        }

        private void IconPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int size = S(60), y = S(10);
            int x = (_iconPanel.Width - size) / 2;
            var rect = new Rectangle(x, y, size, size);
            var theme = BeepThemesManager.CurrentTheme;

            using (var brush = new SolidBrush(theme?.SuccessColor ?? Color.FromArgb(46, 125, 50)))
                g.FillEllipse(brush, rect);
            using (var pen = new Pen(Color.White, S(4)))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                int cx = x + size / 2, cy = y + size / 2;
                g.DrawLines(pen, new Point[] { new(cx - S(14), cy + S(2)), new(cx - S(4), cy + S(12)), new(cx + S(14), cy - S(10)) });
            }
        }

        protected override void LoadData() { _titleLabel.Text = CompletionTitle; _messageLabel.Text = CompletionMessage; }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _titleLabel = null; _messageLabel = null; }
            base.Dispose(disposing);
        }
    }

    public class ErrorStepTemplate : WizardStepTemplateBase
    {
        private BeepLabel _titleLabel, _messageLabel;
        private Panel _iconPanel;

        public string ErrorTitle { get; set; } = "An Error Occurred";
        public string ErrorMessage { get; set; } = "The wizard encountered an error and cannot continue.";
        public override string NextButtonText => "Close";

        public ErrorStepTemplate() { InitializeControls(); }

        private void InitializeControls()
        {
            var theme = BeepThemesManager.CurrentTheme;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            Padding = new Padding(S(40), S(60), S(40), S(40));

            _iconPanel = new Panel { Dock = DockStyle.Top, Height = S(80), BackColor = Color.Transparent };
            _iconPanel.Paint += IconPanel_Paint;
            _titleLabel = new BeepLabel { Dock = DockStyle.Top, Height = S(50), Text = ErrorTitle,
                Font = WizardHelpers.GetFont(theme, theme?.TitleStyle, 22f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = WizardHelpers.GetErrorColor(theme) };
            _messageLabel = new BeepLabel { Dock = DockStyle.Top, Height = S(80), Text = ErrorMessage,
                Font = WizardHelpers.GetFont(theme, theme?.BodyStyle, 11f, FontStyle.Regular), TextAlign = ContentAlignment.TopCenter };

            Controls.Add(_messageLabel);
            Controls.Add(_titleLabel);
            Controls.Add(_iconPanel);
        }

        private void IconPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int size = S(60), y = S(10);
            int x = (_iconPanel.Width - size) / 2;
            var rect = new Rectangle(x, y, size, size);
            var theme = BeepThemesManager.CurrentTheme;

            using (var brush = new SolidBrush(WizardHelpers.GetErrorColor(theme)))
                g.FillEllipse(brush, rect);
            using (var pen = new Pen(Color.White, S(4)))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                int cx = x + size / 2, cy = y + size / 2;
                g.DrawLine(pen, cx - S(12), cy - S(12), cx + S(12), cy + S(12));
                g.DrawLine(pen, cx + S(12), cy - S(12), cx - S(12), cy + S(12));
            }
        }

        protected override void LoadData() { _titleLabel.Text = ErrorTitle; _messageLabel.Text = ErrorMessage; }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _titleLabel = null; _messageLabel = null; }
            base.Dispose(disposing);
        }
    }
}
