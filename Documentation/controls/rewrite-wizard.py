"""
rewrite-wizard.py  — rewrites beep-wizard.html with full Wizard System documentation.
Run once, then run update-navs.py to inject the standard sidebar nav.
"""
import os, re

DIR = os.path.dirname(os.path.abspath(__file__))
TARGET = os.path.join(DIR, "beep-wizard.html")

HEAD = """<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Beep Wizard System - Beep Controls Documentation</title>
    <link rel="stylesheet" href="../sphinx-style.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/themes/prism-tomorrow.min.css">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
</head>
<body>
    <button class="mobile-menu-toggle" onclick="toggleSidebar()"><i class="bi bi-list"></i></button>
    <button class="theme-toggle" onclick="toggleTheme()" title="Toggle theme"><i class="bi bi-sun-fill" id="theme-icon"></i></button>
    <div class="container">
        <!-- sidebar placeholder — will be replaced by update-navs.py -->
        <aside class="sidebar" id="sidebar">
            <div class="logo">
                <img src="../assets/beep-logo.svg" alt="Beep Controls Logo">
                <div class="logo-text"><h2>Beep Controls</h2><span class="version">v1.0.164</span></div>
            </div>
        </aside>"""

MAIN = """
        <main class="content">
            <div class="content-wrapper">
                <nav class="breadcrumb-nav">
                    <a href="../index.html">Home</a>
                    <span>›</span>
                    <a href="#">Specialized Controls</a>
                    <span>›</span>
                    <span>Beep Wizard System</span>
                </nav>

                <!-- ═══════════ PAGE HEADER ═══════════ -->
                <div class="page-header">
                    <div class="header-content">
                        <h1>Beep Wizard System</h1>
                        <p class="subtitle">A complete, fully decoupled wizard framework with four visual styles, async step navigation, pluggable validation, an inter-step data bus, and smooth slide/fade animations.</p>
                        <div class="header-badges">
                            <span class="badge badge-stable">Stable</span>
                            <span class="badge badge-version">v1.0.164</span>
                            <span class="badge badge-namespace">TheTechIdea.Beep.Winform.Controls.Wizards</span>
                        </div>
                    </div>
                </div>

                <!-- ═══════════ OVERVIEW ═══════════ -->
                <section id="overview" class="section">
                    <h2>Overview</h2>
                    <p>The Beep Wizard System is architected as a <strong>decoupled core-engine / UI-host</strong> pair:</p>
                    <ul>
                        <li><strong>WizardManager</strong> — static façade; creates and shows wizards in one call.</li>
                        <li><strong>WizardInstance</strong> — the runtime engine; owns navigation, validation, and state. <em>Contains no UI code.</em></li>
                        <li><strong>WizardFormFactory</strong> — instantiates the correct form host for the chosen <code>WizardStyle</code>.</li>
                        <li><strong>IWizardFormHost</strong> — interface that all four form types implement; keeps the engine UI-agnostic.</li>
                        <li><strong>WizardContext</strong> — typed key/value data bus shared across all steps.</li>
                    </ul>
                    <div class="info-box">
                        <strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Wizards</code><br>
                        <strong>Form types:</strong> HorizontalStepper · VerticalStepper · Minimal · Cards<br>
                        <strong>Entry point:</strong> <code>WizardManager.ShowWizard(config)</code> or <code>WizardManager.CreateWizard(config)</code>
                    </div>
                </section>

                <!-- ═══════════ ARCHITECTURE ═══════════ -->
                <section id="architecture" class="section">
                    <h2>Architecture</h2>
<pre><code class="language-csharp">// Layer overview
WizardManager            // static façade — create / show / close wizards
  └─ WizardInstance      // core engine: navigation, validation, state, events
       ├─ WizardConfig   // immutable construction config + step list
       ├─ WizardContext  // typed key/value data shared between steps
       └─ IWizardFormHost─┬─ HorizontalStepperWizardForm
                          ├─ VerticalStepperWizardForm
                          ├─ MinimalWizardForm
                          └─ CardsWizardForm</code></pre>
                    <h3>Decoupling principle</h3>
                    <p><code>WizardInstance</code> calls only <code>IWizardFormHost</code> methods (<code>UpdateUI</code>, <code>ShowValidationError</code>, <code>GetContentPanel</code>, etc.). Swapping a visual style at creation time is the <em>only</em> change needed — no engine logic changes.</p>
                </section>

                <!-- ═══════════ WIZARD STYLES ═══════════ -->
                <section id="styles" class="section">
                    <h2>Wizard Styles (WizardStyle enum)</h2>
                    <table class="props-table">
                        <thead><tr><th>Value</th><th>Form Class</th><th>Description</th></tr></thead>
                        <tbody>
                            <tr><td><code>HorizontalStepper</code></td><td>HorizontalStepperWizardForm</td><td>Numbered step pills across the top, animated slide transition between steps.</td></tr>
                            <tr><td><code>VerticalStepper</code></td><td>VerticalStepperWizardForm</td><td>Timeline-style vertical step list on the left panel, content area on the right.</td></tr>
                            <tr><td><code>Minimal</code></td><td>MinimalWizardForm</td><td>Clean single-line progress indicator, maximises content area, no step sidebar.</td></tr>
                            <tr><td><code>Cards</code></td><td>CardsWizardForm</td><td>Clickable step-card deck on the left; selecting a card jumps to that step.</td></tr>
                        </tbody>
                    </table>
                </section>

                <!-- ═══════════ KEY CLASSES ═══════════ -->
                <section id="key-classes" class="section">
                    <h2>Key Classes &amp; Models</h2>

                    <h3>WizardConfig</h3>
                    <p>All wizard settings passed to <code>WizardManager.CreateWizard()</code>. Immutable after creation.</p>
                    <table class="props-table">
                        <thead><tr><th>Property</th><th>Type</th><th>Default</th><th>Description</th></tr></thead>
                        <tbody>
                            <tr><td><code>Key</code></td><td>string</td><td>(auto GUID)</td><td>Unique key for the wizard instance. Used by <code>WizardManager.GetWizard()</code>.</td></tr>
                            <tr><td><code>Title</code></td><td>string</td><td>"Wizard"</td><td>Title displayed in the wizard heading.</td></tr>
                            <tr><td><code>Description</code></td><td>string</td><td>""</td><td>Optional sub-heading or description text.</td></tr>
                            <tr><td><code>Size</code></td><td>Size</td><td>900×650</td><td>Initial form size.</td></tr>
                            <tr><td><code>Style</code></td><td>WizardStyle</td><td>HorizontalStepper</td><td>Visual layout style (see above).</td></tr>
                            <tr><td><code>ShowProgressBar</code></td><td>bool</td><td>true</td><td>Show or hide the progress bar.</td></tr>
                            <tr><td><code>ShowStepList</code></td><td>bool</td><td>true</td><td>Show or hide the step list / sidebar.</td></tr>
                            <tr><td><code>AllowCancel</code></td><td>bool</td><td>true</td><td>Enable the Cancel button.</td></tr>
                            <tr><td><code>AllowBack</code></td><td>bool</td><td>true</td><td>Enable the Back button.</td></tr>
                            <tr><td><code>AllowSkip</code></td><td>bool</td><td>false</td><td>Show Skip button for optional steps.</td></tr>
                            <tr><td><code>ShowHelp</code></td><td>bool</td><td>false</td><td>Show a Help button.</td></tr>
                            <tr><td><code>HelpUrl</code></td><td>string</td><td>""</td><td>URL opened when Help is clicked.</td></tr>
                            <tr><td><code>NextButtonText</code></td><td>string</td><td>"Next"</td><td>Default Next button label.</td></tr>
                            <tr><td><code>BackButtonText</code></td><td>string</td><td>"Back"</td><td>Default Back button label.</td></tr>
                            <tr><td><code>FinishButtonText</code></td><td>string</td><td>"Finish"</td><td>Finish button label (shown on last step).</td></tr>
                            <tr><td><code>CancelButtonText</code></td><td>string</td><td>"Cancel"</td><td>Cancel button label.</td></tr>
                            <tr><td><code>SkipButtonText</code></td><td>string</td><td>"Skip"</td><td>Skip button label.</td></tr>
                            <tr><td><code>ShowInlineErrors</code></td><td>bool</td><td>true</td><td>Show validation errors inline instead of MessageBox.</td></tr>
                            <tr><td><code>AutoHideErrors</code></td><td>bool</td><td>true</td><td>Hide validation error panel when navigating to the next step.</td></tr>
                            <tr><td><code>Steps</code></td><td>List&lt;WizardStep&gt;</td><td>[]</td><td>Ordered list of wizard steps.</td></tr>
                            <tr><td><code>OnComplete</code></td><td>Action&lt;WizardContext&gt;</td><td>null</td><td>Callback fired when the wizard finishes successfully.</td></tr>
                            <tr><td><code>OnCancel</code></td><td>Action&lt;WizardContext&gt;</td><td>null</td><td>Callback fired when the wizard is cancelled.</td></tr>
                            <tr><td><code>OnStepChanging</code></td><td>Func&lt;int,WizardContext,bool&gt;</td><td>null</td><td>Called before each step transition; return false to cancel.</td></tr>
                            <tr><td><code>OnStepChanged</code></td><td>Action&lt;int,WizardContext&gt;</td><td>null</td><td>Called after each step transition.</td></tr>
                            <tr><td><code>OnProgress</code></td><td>Action&lt;int,int,string&gt;</td><td>null</td><td>Progress callback for long async operations (current, total, message).</td></tr>
                        </tbody>
                    </table>

                    <h3>WizardStep</h3>
                    <p>Each step in the <code>Config.Steps</code> list.</p>
                    <table class="props-table">
                        <thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead>
                        <tbody>
                            <tr><td><code>Key</code></td><td>string</td><td>Unique key for this step (used in WizardContext).</td></tr>
                            <tr><td><code>Title</code></td><td>string</td><td>Step title shown in the stepper / card UI.</td></tr>
                            <tr><td><code>Description</code></td><td>string</td><td>Sub-title or hint shown under the step title.</td></tr>
                            <tr><td><code>Icon</code></td><td>string</td><td>SVG/image icon path shown in the step indicator.</td></tr>
                            <tr><td><code>Content</code></td><td>Control</td><td>The UserControl or Control displayed in the content panel for this step.</td></tr>
                            <tr><td><code>State</code></td><td>StepState</td><td>Current state: Pending · Current · Completed · Error · Skipped.</td></tr>
                            <tr><td><code>IsOptional</code></td><td>bool</td><td>Marks the step as optional (Skip button shows when AllowSkip=true).</td></tr>
                            <tr><td><code>NextButtonText</code></td><td>string</td><td>Overrides Config.NextButtonText for this step only.</td></tr>
                            <tr><td><code>BackButtonText</code></td><td>string</td><td>Overrides Config.BackButtonText for this step only.</td></tr>
                            <tr><td><code>Validators</code></td><td>List&lt;IWizardValidator&gt;</td><td>Validators run automatically before navigating away.</td></tr>
                            <tr><td><code>CanNavigateNext</code></td><td>Func&lt;WizardContext,bool&gt;</td><td>Delegate gate for next navigation. False = block with no error message.</td></tr>
                            <tr><td><code>CanNavigateBack</code></td><td>Func&lt;WizardContext,bool&gt;</td><td>Delegate gate for back navigation.</td></tr>
                            <tr><td><code>ShouldSkip</code></td><td>Func&lt;WizardContext,bool&gt;</td><td>If returns true at runtime, this step is automatically skipped.</td></tr>
                            <tr><td><code>OnEnter</code></td><td>Action&lt;WizardContext&gt;</td><td>Sync callback when the step becomes active.</td></tr>
                            <tr><td><code>OnLeave</code></td><td>Action&lt;WizardContext&gt;</td><td>Sync callback when the step is navigated away from.</td></tr>
                            <tr><td><code>OnEnterAsync</code></td><td>Func&lt;WizardContext,Task&gt;</td><td>Async enter callback (e.g. load remote data).</td></tr>
                            <tr><td><code>OnLeaveAsync</code></td><td>Func&lt;WizardContext,Task&gt;</td><td>Async leave callback (e.g. submit step data).</td></tr>
                            <tr><td><code>Tag</code></td><td>object</td><td>Arbitrary attached data.</td></tr>
                        </tbody>
                    </table>

                    <h3>WizardContext (Data Bus)</h3>
                    <p>A typed key/value dictionary shared across all steps. The engine sets <code>CurrentStepIndex</code> and <code>TotalSteps</code>; your code reads/writes all other keys.</p>
                    <table class="props-table">
                        <thead><tr><th>Method / Property</th><th>Description</th></tr></thead>
                        <tbody>
                            <tr><td><code>SetValue(key, value)</code></td><td>Store any value under a string key.</td></tr>
                            <tr><td><code>GetValue&lt;T&gt;(key, default)</code></td><td>Retrieve strongly-typed value; returns default if missing or unconvertable.</td></tr>
                            <tr><td><code>ContainsKey(key)</code></td><td>Test whether a key has been set.</td></tr>
                            <tr><td><code>Remove(key)</code></td><td>Delete a key.</td></tr>
                            <tr><td><code>GetStepData(stepKey)</code></td><td>Per-step WizardStepData bag (isolated namespace per step).</td></tr>
                            <tr><td><code>CurrentStepIndex</code></td><td>int — set by the engine during navigation.</td></tr>
                            <tr><td><code>TotalSteps</code></td><td>int — set at construction time.</td></tr>
                            <tr><td><code>StepValidation</code></td><td>Dictionary&lt;int,bool&gt; — engine writes true after each step validates.</td></tr>
                            <tr><td><code>NavigationHistory</code></td><td>Stack&lt;int&gt; — push/pop order of visited step indices.</td></tr>
                        </tbody>
                    </table>
                </section>

                <!-- ═══════════ ENUMS ═══════════ -->
                <section id="enums" class="section">
                    <h2>Enumerations</h2>

                    <h3>StepState</h3>
                    <table class="props-table">
                        <thead><tr><th>Value</th><th>Meaning</th></tr></thead>
                        <tbody>
                            <tr><td><code>Pending</code></td><td>Step has not been visited yet.</td></tr>
                            <tr><td><code>Current</code></td><td>Step is currently active.</td></tr>
                            <tr><td><code>Completed</code></td><td>Step was successfully passed through.</td></tr>
                            <tr><td><code>Error</code></td><td>Step has unsatisfied validation errors.</td></tr>
                            <tr><td><code>Skipped</code></td><td>Step was automatically bypassed via <code>ShouldSkip</code>.</td></tr>
                        </tbody>
                    </table>

                    <h3>WizardResult</h3>
                    <table class="props-table">
                        <thead><tr><th>Value</th><th>Meaning</th></tr></thead>
                        <tbody>
                            <tr><td><code>None</code></td><td>Wizard is still running.</td></tr>
                            <tr><td><code>Completed</code></td><td>User clicked Finish and all validation passed.</td></tr>
                            <tr><td><code>Cancelled</code></td><td>User clicked Cancel.</td></tr>
                            <tr><td><code>Failed</code></td><td>An unhandled error occurred during completion.</td></tr>
                        </tbody>
                    </table>
                </section>

                <!-- ═══════════ WIZARDMANAGER ═══════════ -->
                <section id="wizard-manager" class="section">
                    <h2>WizardManager (Entry Point)</h2>
                    <p><code>WizardManager</code> is a static class that acts as the single entry point for creation, display, and lifecycle management.</p>
                    <table class="props-table">
                        <thead><tr><th>Member</th><th>Type</th><th>Description</th></tr></thead>
                        <tbody>
                            <tr><td><code>DefaultStyle</code></td><td>WizardStyle</td><td>Global default style used when config doesn't specify one. Default: HorizontalStepper.</td></tr>
                            <tr><td><code>EnableAnimations</code></td><td>bool</td><td>Enable/disable step-transition animations globally. Default: true.</td></tr>
                            <tr><td><code>CreateWizard(config)</code></td><td>WizardInstance</td><td>Create a wizard without showing it. Use to hook events before display.</td></tr>
                            <tr><td><code>ShowWizard(config)</code></td><td>DialogResult</td><td>Create and show as modal dialog; returns OK (Completed) or Cancel.</td></tr>
                            <tr><td><code>ShowWizard(config, owner)</code></td><td>DialogResult</td><td>Same but with explicit owner window for correct centering.</td></tr>
                            <tr><td><code>GetWizard(key)</code></td><td>WizardInstance</td><td>Retrieve a running wizard by its Key.</td></tr>
                            <tr><td><code>CloseWizard(key)</code></td><td>void</td><td>Close a wizard by Key.</td></tr>
                            <tr><td><code>CloseAllWizards()</code></td><td>void</td><td>Close all currently open wizard instances.</td></tr>
                        </tbody>
                    </table>
                </section>

                <!-- ═══════════ WIZARDINSTANCE EVENTS ═══════════ -->
                <section id="events" class="section">
                    <h2>WizardInstance Events</h2>
                    <table class="props-table">
                        <thead><tr><th>Event</th><th>Args Type</th><th>Description</th></tr></thead>
                        <tbody>
                            <tr><td><code>StepChanging</code></td><td>StepChangingEventArgs</td><td>Raised before navigating away from the current step. Set <code>e.Cancel = true</code> to block navigation.</td></tr>
                            <tr><td><code>StepChanged</code></td><td>StepChangedEventArgs</td><td>Raised after successfully navigating to a new step. Contains old and new step indices.</td></tr>
                            <tr><td><code>Completed</code></td><td>WizardCompletedEventArgs</td><td>Raised when the user clicks Finish and all final validation passes.</td></tr>
                            <tr><td><code>Cancelled</code></td><td>WizardCancelledEventArgs</td><td>Raised when the user clicks Cancel.</td></tr>
                        </tbody>
                    </table>
                    <h3>WizardInstance navigation methods</h3>
                    <table class="props-table">
                        <thead><tr><th>Method</th><th>Returns</th><th>Description</th></tr></thead>
                        <tbody>
                            <tr><td><code>NavigateNextAsync()</code></td><td>Task&lt;bool&gt;</td><td>Validate current step, fire events, advance to next step (skipping ShouldSkip steps). Returns false if blocked.</td></tr>
                            <tr><td><code>NavigateBackAsync()</code></td><td>Task&lt;bool&gt;</td><td>Navigate to the previous step. Returns false if on first step or AllowBack=false.</td></tr>
                            <tr><td><code>CompleteAsync()</code></td><td>Task&lt;bool&gt;</td><td>Validate final step, fire Completed event, fire OnComplete callback.</td></tr>
                            <tr><td><code>CancelAsync()</code></td><td>Task</td><td>Fire Cancelled event and OnCancel callback, then close the form.</td></tr>
                            <tr><td><code>CurrentStep</code></td><td>WizardStep</td><td>The currently active step.</td></tr>
                            <tr><td><code>CurrentStepIndex</code></td><td>int</td><td>0-based index of the active step.</td></tr>
                            <tr><td><code>IsFirstStep</code></td><td>bool</td><td>True when on step 0.</td></tr>
                            <tr><td><code>IsLastStep</code></td><td>bool</td><td>True when on the last step (Next becomes Finish).</td></tr>
                            <tr><td><code>Result</code></td><td>WizardResult</td><td>Final result after the wizard closes.</td></tr>
                            <tr><td><code>Context</code></td><td>WizardContext</td><td>The shared data bus.</td></tr>
                            <tr><td><code>Config</code></td><td>WizardConfig</td><td>The construction configuration.</td></tr>
                        </tbody>
                    </table>
                </section>

                <!-- ═══════════ VALIDATION ═══════════ -->
                <section id="validation" class="section">
                    <h2>Validation</h2>
                    <p>Validators are attached to a <code>WizardStep.Validators</code> list. They implement <code>IWizardValidator</code> and are called automatically by the engine when Next / Finish is clicked. The first failing validator blocks navigation and shows the error inline.</p>

                    <h3>Built-in Validators</h3>
                    <table class="props-table">
                        <thead><tr><th>Class</th><th>What it checks</th></tr></thead>
                        <tbody>
                            <tr><td><code>RequiredFieldValidator(keys…)</code></td><td>Ensures the listed context keys exist and are non-empty.</td></tr>
                            <tr><td><code>CustomValidator(func)</code></td><td>Full control — supply a <code>Func&lt;WizardContext, WizardStep, WizardValidationResult&gt;</code>.</td></tr>
                            <tr><td><code>PredicateValidator(pred, msg)</code></td><td>Simple bool predicate. Fails with <code>msg</code> when predicate returns false.</td></tr>
                            <tr><td><code>RegexValidator(key, pattern, msg)</code></td><td>Tests a context string value against a regex pattern.</td></tr>
                            <tr><td><code>EmailValidator(key)</code></td><td>Validates that the context key contains a valid email address.</td></tr>
                            <tr><td><code>RangeValidator(key, min, max)</code></td><td>Validates a numeric context value is within [min, max].</td></tr>
                            <tr><td><code>StringLengthValidator(key, min, max)</code></td><td>Validates string length.</td></tr>
                        </tbody>
                    </table>

                    <h3>ValidationPatterns constants</h3>
<pre><code class="language-csharp">ValidationPatterns.Email    // ^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$
ValidationPatterns.Phone    // ^\+?[\d\s\-()]{10,}$
ValidationPatterns.Url      // ^https?://[\w\-]+(\.[\w\-]+)+[/#?]?.*$
ValidationPatterns.ZipCode  // ^\d{5}(-\d{4})?$
ValidationPatterns.Integer  // ^\-?\d+$
ValidationPatterns.Decimal  // ^\-?\d+(\.\d+)?$</code></pre>

                    <h3>WizardValidationResult</h3>
<pre><code class="language-csharp">// Factory methods — use these in custom validators
WizardValidationResult.Success()
WizardValidationResult.Error("Error message")
WizardValidationResult.Error("Error message", "contextKey")  // focuses specific field</code></pre>

                    <h3>Delegate-based quick validation (no validator class needed)</h3>
<pre><code class="language-csharp">new WizardStep
{
    Key = "terms",
    Title = "Terms",
    CanNavigateNext = ctx =>
        ctx.GetValue&lt;bool&gt;("termsAccepted", false),
}</code></pre>
                </section>

                <!-- ═══════════ STEP TEMPLATES ═══════════ -->
                <section id="step-templates" class="section">
                    <h2>Step Templates</h2>
                    <p>Pre-built step <code>UserControl</code> classes that implement <code>IWizardStepContent</code>. Derive from <code>WizardStepTemplateBase</code> to get automatic <code>OnStepEnter</code> / <code>OnStepLeave</code> lifecycle and <code>ValidationStateChanged</code> event.</p>

                    <table class="props-table">
                        <thead><tr><th>Template Class</th><th>Purpose</th><th>Key Properties</th></tr></thead>
                        <tbody>
                            <tr><td><code>WelcomeStepTemplate</code></td><td>Introductory step with icon, title, and message text.</td><td>WelcomeTitle, WelcomeMessage, WelcomeIcon</td></tr>
                            <tr><td><code>SummaryStepTemplate</code></td><td>Final review screen displaying collected data before Finish.</td><td>SummaryTitle, SummaryBuilder (Func&lt;WizardContext, string&gt;)</td></tr>
                        </tbody>
                    </table>

                    <h3>Creating a custom step template</h3>
<pre><code class="language-csharp">public class AccountStep : WizardStepTemplateBase
{
    private TextBox _emailBox;

    public AccountStep()
    {
        InitializeComponent();   // your designer code
    }

    protected override void LoadData()
    {
        // called on enter — populate UI from context
        _emailBox.Text = Context.GetValue&lt;string&gt;("email", "");
    }

    protected override void SaveData()
    {
        // called on leave — persist UI back to context
        Context.SetValue("email", _emailBox.Text.Trim());
    }

    public override WizardValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(_emailBox.Text))
            return WizardValidationResult.Error("Email is required", "email");
        return WizardValidationResult.Success();
    }
}</code></pre>
                </section>

                <!-- ═══════════ WIZARDFORMFACTORY ═══════════ -->
                <section id="form-factory" class="section">
                    <h2>WizardFormFactory — Custom Styles</h2>
                    <p>Register a factory function to provide a completely custom form for any <code>WizardStyle</code> value:</p>
<pre><code class="language-csharp">// Register a branded corporate wizard form
WizardFormFactory.RegisterFactory(
    WizardStyle.Cards,
    instance => new MyCorporateWizardForm(instance)
);

// Now all Cards style wizards use your custom form
WizardManager.ShowWizard(new WizardConfig
{
    Style = WizardStyle.Cards,
    // ...
});</code></pre>
                </section>

                <!-- ═══════════ ANIMATIONS ═══════════ -->
                <section id="animations" class="section">
                    <h2>Animations</h2>
                    <p><code>WizardHelpers</code> provides two built-in transitions used by the form hosts:</p>
                    <table class="props-table">
                        <thead><tr><th>Method</th><th>Effect</th><th>Details</th></tr></thead>
                        <tbody>
                            <tr><td><code>AnimateStepTransition(from, to, forward, onComplete)</code></td><td>Slide left/right</td><td>12 frames at ~60 fps with ease-out cubic deceleration. Forward=true slides left (next), false slides right (back).</td></tr>
                            <tr><td><code>AnimateFadeTransition(from, to, onComplete)</code></td><td>Cross-fade</td><td>Opacity blend between the old and new step controls.</td></tr>
                        </tbody>
                    </table>
                    <p>Disable all animations globally: <code>WizardManager.EnableAnimations = false;</code></p>
                </section>

                <!-- ═══════════ QUICK START ═══════════ -->
                <section id="quick-start" class="section">
                    <h2>Quick Start</h2>
<pre><code class="language-csharp">// Minimal 3-step wizard shown as modal dialog
var result = WizardManager.ShowWizard(new WizardConfig
{
    Title       = "New Connection",
    Style       = WizardStyle.HorizontalStepper,
    AllowCancel = true,

    Steps = new List&lt;WizardStep&gt;
    {
        new WizardStep
        {
            Key     = "welcome",
            Title   = "Welcome",
            Content = new WelcomeStepTemplate
            {
                WelcomeTitle   = "New Connection Wizard",
                WelcomeMessage = "This wizard will guide you through setting up a data connection."
            }
        },
        new WizardStep
        {
            Key     = "connection",
            Title   = "Connection Details",
            Content = new ConnectionDetailsStep(),      // your UserControl
            Validators = new List&lt;IWizardValidator&gt;
            {
                new RequiredFieldValidator("serverName", "database"),
                new RequiredFieldValidator("username")
            }
        },
        new WizardStep
        {
            Key          = "summary",
            Title        = "Summary",
            Content      = new SummaryStepTemplate
            {
                SummaryTitle   = "Review Connection",
                SummaryBuilder = ctx =>
                    $"Server: {ctx.GetValue&lt;string&gt;("serverName")}\\n" +
                    $"Database: {ctx.GetValue&lt;string&gt;("database")}"
            },
            NextButtonText = "Finish"
        }
    },

    OnComplete = ctx =>
    {
        var server = ctx.GetValue&lt;string&gt;("serverName");
        var db     = ctx.GetValue&lt;string&gt;("database");
        CreateConnection(server, db);
    }
}, this);   // owner window

if (result == DialogResult.OK)
    MessageBox.Show("Connection created!");
</code></pre>
                </section>

                <!-- ═══════════ ADVANCED EXAMPLES ═══════════ -->
                <section id="advanced" class="section">
                    <h2>Advanced Examples</h2>

                    <h3>Hooking events before display</h3>
<pre><code class="language-csharp">var instance = WizardManager.CreateWizard(new WizardConfig { /* ... */ });

instance.StepChanging += (s, e) =>
{
    if (e.ToIndex == 2 && !PreFlightCheck())
    {
        e.Cancel = true;
        MessageBox.Show("Please complete the pre-flight check first.");
    }
};

instance.StepChanged += (s, e) =>
    statusBar.Text = $"Step {e.ToIndex + 1} of {instance.Config.Steps.Count}";

instance.Completed += (s, e) =>
    SaveWizardData(e.Context);

instance.ShowDialog(this);
</code></pre>

                    <h3>Async step callbacks</h3>
<pre><code class="language-csharp">new WizardStep
{
    Key   = "validate",
    Title = "Validate Connection",

    OnEnterAsync = async ctx =>
    {
        // runs when step becomes active — e.g. load defaults from API
        var defaults = await ApiClient.GetConnectionDefaultsAsync();
        ctx.SetValue("defaultServer", defaults.Server);
    },

    OnLeaveAsync = async ctx =>
    {
        // runs when navigating away — e.g. persist/validate server-side
        var server = ctx.GetValue&lt;string&gt;("serverName");
        bool ok = await ApiClient.TestConnectionAsync(server);
        if (!ok)
            throw new InvalidOperationException("Cannot reach server");  // blocks navigation
    }
}
</code></pre>

                    <h3>Conditional step skipping</h3>
<pre><code class="language-csharp">new WizardStep
{
    Key        = "advancedOptions",
    Title      = "Advanced Options",
    IsOptional = true,
    ShouldSkip = ctx => !ctx.GetValue&lt;bool&gt;("showAdvanced", false),
    Content    = new AdvancedOptionsStep()
}
</code></pre>

                    <h3>Custom CardStyle wizard with branded header</h3>
<pre><code class="language-csharp">WizardFormFactory.RegisterFactory(WizardStyle.Cards, inst => new BrandedWizardForm(inst));

WizardManager.ShowWizard(new WizardConfig
{
    Style       = WizardStyle.Cards,
    Title       = "Onboarding",
    Size        = new Size(1000, 700),
    AllowSkip   = true,
    ShowHelp    = true,
    HelpUrl     = "https://docs.example.com/onboarding",
    Steps       = /* ... */
});
</code></pre>

                    <h3>Reading collected data after completion</h3>
<pre><code class="language-csharp">var instance = WizardManager.CreateWizard(config);
instance.ShowDialog(this);

if (instance.Result == WizardResult.Completed)
{
    var ctx = instance.Context;
    var name     = ctx.GetValue&lt;string&gt;("firstName")  + " " + ctx.GetValue&lt;string&gt;("lastName");
    var plan     = ctx.GetValue&lt;string&gt;("selectedPlan", "Free");
    var accepted = ctx.GetValue&lt;bool&gt;("termsAccepted", false);
    // process...
}
</code></pre>
                </section>

                <!-- ═══════════ BEST PRACTICES ═══════════ -->
                <section id="best-practices" class="section">
                    <h2>Best Practices</h2>
                    <ul>
                        <li><strong>Use WizardManager.ShowWizard() for fire-and-forget wizards.</strong> Use <code>CreateWizard()</code> only when you need to hook events before display.</li>
                        <li><strong>Store all cross-step data in WizardContext</strong> — do not use static fields or class-level variables. Context data is available in the <code>OnComplete</code> callback and in <code>WizardInstance.Context</code> after the dialog closes.</li>
                        <li><strong>Prefer Step.Validators over CanNavigateNext delegates</strong> when you want inline error messages. <code>CanNavigateNext</code> silently blocks navigation without a user-facing message.</li>
                        <li><strong>Use WizardStepTemplateBase</strong> for custom step controls — the <code>LoadData()</code> / <code>SaveData()</code> pattern keeps concerns clean.</li>
                        <li><strong>Mark optional steps with IsOptional=true</strong> and enable <code>AllowSkip</code> in WizardConfig to give users an explicit Skip button.</li>
                        <li><strong>Use ShouldSkip for programmatic conditional steps</strong> (e.g. skip billing step for Free plans). This is automatic and invisible to the user.</li>
                        <li><strong>Prefer OnEnterAsync / OnLeaveAsync for I/O</strong> (API calls, DB queries). The engine awaits the task before updating the UI.</li>
                        <li><strong>Disable animations during automated tests:</strong> <code>WizardManager.EnableAnimations = false;</code></li>
                        <li><strong>Always pass an owner window</strong> to <code>ShowWizard(config, this)</code> so the dialog centres correctly.</li>
                        <li><strong>Register a custom factory once at startup</strong> in <code>WizardFormFactory.RegisterFactory()</code> to globally change the UI for a given style.</li>
                    </ul>
                </section>

            </div>
        </main>"""

FOOT = """
    </div>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/prism.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-csharp.min.js"></script>
    <script>
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'dark') {
            document.body.setAttribute('data-theme', 'dark');
            document.getElementById('theme-icon').className = 'bi bi-moon-fill';
        }
        function toggleTheme() {
            const body = document.body;
            const icon = document.getElementById('theme-icon');
            if (body.getAttribute('data-theme') === 'dark') {
                body.removeAttribute('data-theme');
                icon.className = 'bi bi-sun-fill';
                localStorage.setItem('theme', 'light');
            } else {
                body.setAttribute('data-theme', 'dark');
                icon.className = 'bi bi-moon-fill';
                localStorage.setItem('theme', 'dark');
            }
        }
        function toggleSidebar() { document.getElementById('sidebar').classList.toggle('open'); }
        function searchDocs(q) {
            document.querySelectorAll('.nav-menu a').forEach(a => {
                a.closest('li').style.display = a.textContent.toLowerCase().includes(q.toLowerCase()) || q === '' ? '' : 'none';
            });
        }
    </script>
</body>
</html>"""

with open(TARGET, "w", encoding="utf-8") as f:
    f.write(HEAD + MAIN + FOOT)

print(f"Written: {TARGET}")
print(f"Lines: {(HEAD+MAIN+FOOT).count(chr(10))}")
