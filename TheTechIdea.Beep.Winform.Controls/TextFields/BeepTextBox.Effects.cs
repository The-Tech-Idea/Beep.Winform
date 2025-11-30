using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;
using TheTechIdea.Beep.Winform.Controls.TextFields.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Text effects and terminal mode functionality for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region Effects Fields

        private TextBoxEffectsHelper _effectsHelper;
        private TextEffectMode _effectMode = TextEffectMode.None;
        private TerminalStylePreset _terminalStylePreset = TerminalStylePreset.Classic;

        #endregion

        #region Effects Properties

        /// <summary>
        /// Gets the effects helper for advanced effect operations
        /// </summary>
        [Browsable(false)]
        public TextBoxEffectsHelper EffectsHelper
        {
            get
            {
                if (_effectsHelper == null)
                {
                    _effectsHelper = new TextBoxEffectsHelper(this);
                    _effectsHelper.EffectStarted += EffectsHelper_EffectStarted;
                    _effectsHelper.EffectCompleted += EffectsHelper_EffectCompleted;
                    _effectsHelper.CharacterTyped += EffectsHelper_CharacterTyped;
                    _effectsHelper.CommandEntered += EffectsHelper_CommandEntered;
                }
                return _effectsHelper;
            }
        }

        /// <summary>
        /// Text effect mode
        /// </summary>
        [Browsable(true)]
        [Category("Effects")]
        [DefaultValue(TextEffectMode.None)]
        [Description("Text effect mode for displaying text.")]
        public TextEffectMode EffectMode
        {
            get => _effectMode;
            set
            {
                _effectMode = value;
                EffectsHelper.EffectConfig.Mode = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Typewriter effect speed (characters per second)
        /// </summary>
        [Browsable(true)]
        [Category("Effects")]
        [DefaultValue(50)]
        [Description("Characters per second for typewriter effect.")]
        public int TypewriterSpeed
        {
            get => EffectsHelper.EffectConfig.TypewriterSpeed;
            set
            {
                EffectsHelper.EffectConfig.TypewriterSpeed = Math.Max(1, value);
            }
        }

        /// <summary>
        /// Enable typewriter sound effect
        /// </summary>
        [Browsable(true)]
        [Category("Effects")]
        [DefaultValue(false)]
        [Description("Enable typing sound for typewriter effect.")]
        public bool TypewriterSoundEnabled
        {
            get => EffectsHelper.EffectConfig.TypewriterSoundEnabled;
            set => EffectsHelper.EffectConfig.TypewriterSoundEnabled = value;
        }

        /// <summary>
        /// Duration of scramble effect in milliseconds
        /// </summary>
        [Browsable(true)]
        [Category("Effects")]
        [DefaultValue(1000)]
        [Description("Duration of scramble effect in milliseconds.")]
        public int ScrambleDuration
        {
            get => EffectsHelper.EffectConfig.ScrambleDuration;
            set => EffectsHelper.EffectConfig.ScrambleDuration = Math.Max(100, value);
        }

        /// <summary>
        /// Loop effects continuously
        /// </summary>
        [Browsable(true)]
        [Category("Effects")]
        [DefaultValue(false)]
        [Description("Loop effects continuously.")]
        public bool LoopEffect
        {
            get => EffectsHelper.EffectConfig.Loop;
            set => EffectsHelper.EffectConfig.Loop = value;
        }

        /// <summary>
        /// Whether an effect is currently running
        /// </summary>
        [Browsable(false)]
        public bool IsEffectRunning => EffectsHelper.IsEffectRunning;

        /// <summary>
        /// Current effect progress (0-1)
        /// </summary>
        [Browsable(false)]
        public float EffectProgress => EffectsHelper.EffectProgress;

        #endregion

        #region Terminal Mode Properties

        /// <summary>
        /// Enable terminal mode
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(false)]
        [Description("Enable terminal/console mode.")]
        public bool TerminalModeEnabled
        {
            get => EffectsHelper.TerminalModeEnabled;
            set
            {
                if (value)
                    EffectsHelper.EnableTerminalMode(_terminalStylePreset);
                else
                    EffectsHelper.DisableTerminalMode();
                Invalidate();
            }
        }

        /// <summary>
        /// Terminal style preset
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(TerminalStylePreset.Classic)]
        [Description("Terminal style preset theme.")]
        public TerminalStylePreset TerminalStyle
        {
            get => _terminalStylePreset;
            set
            {
                _terminalStylePreset = value;
                EffectsHelper.TerminalConfig = TerminalStyleConfig.FromPreset(value);
                if (TerminalModeEnabled)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Terminal command prompt prefix
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue("> ")]
        [Description("Terminal command prompt prefix.")]
        public string TerminalPrompt
        {
            get => EffectsHelper.TerminalConfig.Prompt;
            set
            {
                EffectsHelper.TerminalConfig.Prompt = value ?? "> ";
                Invalidate();
            }
        }

        /// <summary>
        /// Terminal cursor style
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(TerminalCursorStyle.Block)]
        [Description("Terminal cursor style.")]
        public TerminalCursorStyle TerminalCursorStyle
        {
            get => EffectsHelper.TerminalConfig.CursorStyle;
            set
            {
                EffectsHelper.TerminalConfig.CursorStyle = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Enable scanlines effect
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(false)]
        [Description("Enable CRT scanlines effect.")]
        public bool EnableScanlines
        {
            get => EffectsHelper.TerminalConfig.EnableScanlines;
            set
            {
                EffectsHelper.TerminalConfig.EnableScanlines = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Enable CRT curvature effect
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(false)]
        [Description("Enable CRT screen curvature effect.")]
        public bool EnableCRTEffect
        {
            get => EffectsHelper.TerminalConfig.EnableCRTCurvature;
            set
            {
                EffectsHelper.TerminalConfig.EnableCRTCurvature = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Enable text glow effect
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(true)]
        [Description("Enable text glow effect in terminal mode.")]
        public bool EnableGlow
        {
            get => EffectsHelper.TerminalConfig.EnableGlow;
            set
            {
                EffectsHelper.TerminalConfig.EnableGlow = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Text glow intensity (0-1)
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(0.3f)]
        [Description("Text glow intensity (0-1).")]
        public float GlowIntensity
        {
            get => EffectsHelper.TerminalConfig.GlowIntensity;
            set
            {
                EffectsHelper.TerminalConfig.GlowIntensity = Math.Max(0, Math.Min(1, value));
                Invalidate();
            }
        }

        /// <summary>
        /// Enable flicker effect
        /// </summary>
        [Browsable(true)]
        [Category("Terminal")]
        [DefaultValue(false)]
        [Description("Enable CRT flicker effect.")]
        public bool EnableFlicker
        {
            get => EffectsHelper.TerminalConfig.EnableFlicker;
            set
            {
                EffectsHelper.TerminalConfig.EnableFlicker = value;
            }
        }

        #endregion

        #region Effects Events

        /// <summary>
        /// Fired when an effect starts
        /// </summary>
        public event EventHandler<EffectEventArgs> EffectStarted;

        /// <summary>
        /// Fired when an effect completes
        /// </summary>
        public event EventHandler<EffectEventArgs> EffectCompleted;

        /// <summary>
        /// Fired for each character in typewriter effect
        /// </summary>
        public event EventHandler<TypewriterEventArgs> CharacterTyped;

        /// <summary>
        /// Fired when a command is entered in terminal mode
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandEntered;

        private void EffectsHelper_EffectStarted(object sender, EffectEventArgs e)
        {
            EffectStarted?.Invoke(this, e);
        }

        private void EffectsHelper_EffectCompleted(object sender, EffectEventArgs e)
        {
            EffectCompleted?.Invoke(this, e);
        }

        private void EffectsHelper_CharacterTyped(object sender, TypewriterEventArgs e)
        {
            CharacterTyped?.Invoke(this, e);
        }

        private void EffectsHelper_CommandEntered(object sender, CommandEventArgs e)
        {
            CommandEntered?.Invoke(this, e);
        }

        #endregion

        #region Effects Methods

        /// <summary>
        /// Set text with typewriter effect
        /// </summary>
        public void TypewriterText(string text)
        {
            EffectsHelper.StartTypewriterEffect(text);
        }

        /// <summary>
        /// Set text with scramble effect
        /// </summary>
        public void ScrambleText(string text)
        {
            EffectsHelper.StartScrambleEffect(text);
        }

        /// <summary>
        /// Apply glitch effect to current text
        /// </summary>
        public void GlitchText()
        {
            EffectsHelper.StartGlitchEffect();
        }

        /// <summary>
        /// Set text with fade-in effect
        /// </summary>
        public void FadeInText(string text)
        {
            EffectsHelper.StartFadeInEffect(text);
        }

        /// <summary>
        /// Set text with the configured effect mode
        /// </summary>
        public void SetTextWithEffect(string text)
        {
            EffectsHelper.SetTextWithEffect(text, _effectMode);
        }

        /// <summary>
        /// Stop any running effect
        /// </summary>
        public void StopEffect()
        {
            EffectsHelper.StopEffect();
        }

        #endregion

        #region Terminal Methods

        /// <summary>
        /// Output text to terminal with animation
        /// </summary>
        public void TerminalOutput(string text, bool animate = true)
        {
            if (!TerminalModeEnabled)
            {
                TerminalModeEnabled = true;
            }
            EffectsHelper.TerminalOutput(text, animate);
        }

        /// <summary>
        /// Clear terminal and show prompt
        /// </summary>
        public void TerminalClear()
        {
            EffectsHelper.TerminalClear();
        }

        /// <summary>
        /// Write a line to terminal
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="animate">Whether to use typewriter animation (default: true)</param>
        public void TerminalWriteLine(string text, bool animate = true)
        {
            TerminalOutput(text + Environment.NewLine, animate);
        }

        /// <summary>
        /// Write text to terminal without newline
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="animate">Whether to use typewriter animation (default: true)</param>
        public void TerminalWrite(string text, bool animate = true)
        {
            TerminalOutput(text, animate);
        }
        
        /// <summary>
        /// Cancel any running effect and show final text immediately
        /// </summary>
        public void CancelEffectAndShowFinal()
        {
            if (IsEffectRunning)
            {
                EffectsHelper.StopEffect();
                // Force show the final text
                Invalidate();
            }
        }
        
        /// <summary>
        /// Wait for any running effect to complete
        /// </summary>
        /// <param name="timeout">Maximum time to wait in milliseconds</param>
        /// <returns>True if effect completed, false if timed out</returns>
        public bool WaitForEffectCompletion(int timeout = 5000)
        {
            if (!IsEffectRunning) return true;
            
            var startTime = DateTime.Now;
            while (IsEffectRunning && (DateTime.Now - startTime).TotalMilliseconds < timeout)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
            }
            
            return !IsEffectRunning;
        }

        /// <summary>
        /// Simulate command execution
        /// </summary>
        public void ExecuteCommand(string command)
        {
            EffectsHelper.ProcessCommand(command);
        }

        #endregion

        #region Effects Drawing

        /// <summary>
        /// Draw effects and terminal styling
        /// </summary>
        private void DrawEffects(Graphics g, Rectangle textRect)
        {
            if (TerminalModeEnabled)
            {
                string displayText = IsEffectRunning 
                    ? EffectsHelper.CurrentDisplayText 
                    : GetDisplayText();

                EffectsHelper.DrawTerminalText(g, textRect, displayText);
            }
            else if (IsEffectRunning && _effectMode == TextEffectMode.FadeIn)
            {
                EffectsHelper.DrawFadeText(g, textRect, EffectsHelper.CurrentDisplayText, 
                    _textFont, _currentTheme?.TextBoxForeColor ?? ForeColor);
            }
        }

        #endregion
    }
}

