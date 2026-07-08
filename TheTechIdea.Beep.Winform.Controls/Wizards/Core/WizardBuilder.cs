using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Fluent builder for programmatic wizard creation.
    /// Use WizardBuilder.Create("key").WithTitle("...").AddStep(...).Build().
    /// </summary>
    public class WizardBuilder
    {
        private readonly WizardConfig _config = new();

        private WizardBuilder(string key) { _config.Key = key; }

        /// <summary>Create a new builder with the given key.</summary>
        public static WizardBuilder Create(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key is required.", nameof(key));
            return new WizardBuilder(key);
        }

        // ── Wizard config ──────────────────────────────────────────────────

        public WizardBuilder WithTitle(string title) { _config.Title = title; return this; }
        public WizardBuilder WithDescription(string desc) { _config.Description = desc; return this; }
        public WizardBuilder WithSize(int w, int h) { _config.Size = new Size(w, h); return this; }
        public WizardBuilder WithStyle(WizardStyle style) { _config.Style = style; return this; }
        public WizardBuilder WithTheme(IBeepTheme theme) { _config.Theme = theme; return this; }
        public WizardBuilder WithoutCancel() { _config.AllowCancel = false; return this; }
        public WizardBuilder WithoutBack() { _config.AllowBack = false; return this; }
        public WizardBuilder WithHelp(string url = null) { _config.ShowHelp = true; _config.HelpUrl = url; return this; }
        public WizardBuilder WithProgressBar(bool show = true) { _config.ShowProgressBar = show; return this; }
        public WizardBuilder WithAnimation(TransitionType type, int durationMs = 300)
        { _config.TransitionType = type; _config.TransitionDurationMs = durationMs; return this; }
        public WizardBuilder WithUndo(bool enable = true) { _config.EnableUndo = enable; return this; }
        public WizardBuilder WithConfirmOnCancel(string message = null)
        { _config.ConfirmOnCancel = true; if (message != null) _config.CancelConfirmationMessage = message; return this; }

        // ── Button text ────────────────────────────────────────────────────

        public WizardBuilder WithNextText(string text) { _config.NextButtonText = text; return this; }
        public WizardBuilder WithBackText(string text) { _config.BackButtonText = text; return this; }
        public WizardBuilder WithFinishText(string text) { _config.FinishButtonText = text; return this; }
        public WizardBuilder WithCancelText(string text) { _config.CancelButtonText = text; return this; }

        // ── Steps ──────────────────────────────────────────────────────────

        public WizardBuilder AddStep(string key, string title, Control content = null, string description = null, bool optional = false)
        {
            _config.Steps.Add(new WizardStep
            {
                Key = key, Title = title, Content = content, Description = description, IsOptional = optional
            });
            return this;
        }

        public WizardBuilder AddStep(WizardStep step)
        {
            _config.Steps.Add(step ?? throw new ArgumentNullException(nameof(step)));
            return this;
        }

        // ── Callbacks ──────────────────────────────────────────────────────

        public WizardBuilder OnComplete(Action<WizardContext> callback) { _config.OnComplete = callback; return this; }
        public WizardBuilder OnCancel(Action<WizardContext> callback) { _config.OnCancel = callback; return this; }
        public WizardBuilder OnStepChanging(Func<int, WizardContext, bool> callback) { _config.OnStepChanging = callback; return this; }
        public WizardBuilder OnStepChanged(Action<int, WizardContext> callback) { _config.OnStepChanged = callback; return this; }
        public WizardBuilder OnProgress(Action<int, int, string> callback) { _config.OnProgress = callback; return this; }

        // ── Build & Show ──────────────────────────────────────────────────

        public WizardConfig Build() => _config;

        public DialogResult Show() => WizardManager.ShowWizard(_config);
        public DialogResult Show(IWin32Window owner) => WizardManager.ShowWizard(_config, owner);
    }
}
