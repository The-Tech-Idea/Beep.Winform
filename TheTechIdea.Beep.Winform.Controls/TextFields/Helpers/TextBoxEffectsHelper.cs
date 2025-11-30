using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields.Models;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Helper class for text effects and terminal-style rendering in BeepTextBox
    /// </summary>
    public class TextBoxEffectsHelper : IDisposable
    {
        #region Fields

        private readonly BeepTextBox _textBox;
        private TextEffectConfig _effectConfig;
        private TerminalStyleConfig _terminalConfig;
        
        // Effect state
        private Timer _effectTimer;
        private string _targetText = string.Empty;
        private string _currentDisplayText = string.Empty;
        private int _effectPosition = 0;
        private float _effectProgress = 0f;
        private DateTime _effectStartTime;
        private bool _isEffectRunning = false;
        private Random _random = new Random();

        // Terminal state
        private Timer _cursorBlinkTimer;
        private bool _cursorVisible = true;
        private Timer _flickerTimer;
        private float _currentFlickerAlpha = 1f;

        // Cached resources
        private Font _terminalFont;
        private SolidBrush _textBrush;
        private SolidBrush _dimBrush;
        private SolidBrush _cursorBrush;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Current effect configuration
        /// </summary>
        public TextEffectConfig EffectConfig
        {
            get => _effectConfig;
            set => _effectConfig = value ?? new TextEffectConfig();
        }

        /// <summary>
        /// Current terminal style configuration
        /// </summary>
        public TerminalStyleConfig TerminalConfig
        {
            get => _terminalConfig;
            set
            {
                _terminalConfig = value ?? new TerminalStyleConfig();
                UpdateTerminalResources();
            }
        }

        /// <summary>
        /// Whether an effect is currently running
        /// </summary>
        public bool IsEffectRunning => _isEffectRunning;

        /// <summary>
        /// Current effect progress (0-1)
        /// </summary>
        public float EffectProgress => _effectProgress;

        /// <summary>
        /// Text currently being displayed (may differ from target during effects)
        /// </summary>
        public string CurrentDisplayText => _currentDisplayText;

        /// <summary>
        /// Whether cursor is currently visible (for blinking)
        /// </summary>
        public bool IsCursorVisible => _cursorVisible;

        /// <summary>
        /// Terminal mode enabled
        /// </summary>
        public bool TerminalModeEnabled { get; set; } = false;

        #endregion

        #region Events

        /// <summary>
        /// Fired when effect starts
        /// </summary>
        public event EventHandler<EffectEventArgs> EffectStarted;

        /// <summary>
        /// Fired during effect progress
        /// </summary>
        public event EventHandler<EffectEventArgs> EffectProgressChanged;

        /// <summary>
        /// Fired when effect completes
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

        #endregion

        #region Constructor

        public TextBoxEffectsHelper(BeepTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            _effectConfig = new TextEffectConfig();
            _terminalConfig = TerminalStyleConfig.FromPreset(TerminalStylePreset.Classic);

            InitializeTimers();
            UpdateTerminalResources();
        }

        private void InitializeTimers()
        {
            _effectTimer = new Timer { Interval = 16 }; // ~60 FPS
            _effectTimer.Tick += EffectTimer_Tick;

            _cursorBlinkTimer = new Timer { Interval = 530 };
            _cursorBlinkTimer.Tick += CursorBlinkTimer_Tick;

            _flickerTimer = new Timer { Interval = 50 };
            _flickerTimer.Tick += FlickerTimer_Tick;
        }

        private void UpdateTerminalResources()
        {
            _terminalFont?.Dispose();
            _textBrush?.Dispose();
            _dimBrush?.Dispose();
            _cursorBrush?.Dispose();

            _terminalFont = _terminalConfig.CreateFont();
            _textBrush = new SolidBrush(_terminalConfig.TextColor);
            _dimBrush = new SolidBrush(_terminalConfig.DimTextColor);
            _cursorBrush = new SolidBrush(_terminalConfig.CursorColor);

            _cursorBlinkTimer.Interval = _terminalConfig.CursorBlinkRate;
        }

        #endregion

        #region Effect Methods

        /// <summary>
        /// Start a typewriter effect with the given text
        /// </summary>
        public void StartTypewriterEffect(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            StopEffect();
            
            _targetText = text;
            _currentDisplayText = string.Empty;
            _effectPosition = 0;
            _effectProgress = 0f;
            _effectStartTime = DateTime.Now;
            _isEffectRunning = true;

            int interval = 1000 / Math.Max(1, _effectConfig.TypewriterSpeed);
            _effectTimer.Interval = interval;
            _effectTimer.Start();

            EffectStarted?.Invoke(this, new EffectEventArgs(TextEffectMode.Typewriter, 0, _currentDisplayText));
        }

        /// <summary>
        /// Start a scramble effect that resolves to the given text
        /// </summary>
        public void StartScrambleEffect(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            StopEffect();

            _targetText = text;
            _currentDisplayText = GenerateScrambledText(text.Length);
            _effectPosition = 0;
            _effectProgress = 0f;
            _effectStartTime = DateTime.Now;
            _isEffectRunning = true;

            _effectTimer.Interval = 16;
            _effectTimer.Start();

            EffectStarted?.Invoke(this, new EffectEventArgs(TextEffectMode.Scramble, 0, _currentDisplayText));
        }

        /// <summary>
        /// Start a glitch effect on current text
        /// </summary>
        public void StartGlitchEffect()
        {
            if (string.IsNullOrEmpty(_textBox.Text)) return;

            StopEffect();

            _targetText = _textBox.Text;
            _currentDisplayText = _targetText;
            _effectStartTime = DateTime.Now;
            _isEffectRunning = true;

            _effectTimer.Interval = 16;
            _effectTimer.Start();

            EffectStarted?.Invoke(this, new EffectEventArgs(TextEffectMode.Glitch, 0, _currentDisplayText));
        }

        /// <summary>
        /// Start a fade-in effect
        /// </summary>
        public void StartFadeInEffect(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            StopEffect();

            _targetText = text;
            _currentDisplayText = text;
            _effectProgress = 0f;
            _effectStartTime = DateTime.Now;
            _isEffectRunning = true;

            _effectTimer.Interval = 16;
            _effectTimer.Start();

            EffectStarted?.Invoke(this, new EffectEventArgs(TextEffectMode.FadeIn, 0, _currentDisplayText));
        }

        /// <summary>
        /// Stop current effect
        /// </summary>
        public void StopEffect()
        {
            _effectTimer.Stop();
            _isEffectRunning = false;
            
            if (!string.IsNullOrEmpty(_targetText))
            {
                _currentDisplayText = _targetText;
            }
            
            _textBox.Invalidate();
        }

        /// <summary>
        /// Set text with effect
        /// </summary>
        public void SetTextWithEffect(string text, TextEffectMode mode)
        {
            switch (mode)
            {
                case TextEffectMode.Typewriter:
                    StartTypewriterEffect(text);
                    break;
                case TextEffectMode.Scramble:
                    StartScrambleEffect(text);
                    break;
                case TextEffectMode.Glitch:
                    _textBox.Text = text;
                    StartGlitchEffect();
                    break;
                case TextEffectMode.FadeIn:
                    StartFadeInEffect(text);
                    break;
                default:
                    _textBox.Text = text;
                    _currentDisplayText = text;
                    break;
            }
        }

        #endregion

        #region Timer Handlers

        private void EffectTimer_Tick(object sender, EventArgs e)
        {
            switch (_effectConfig.Mode)
            {
                case TextEffectMode.Typewriter:
                    UpdateTypewriterEffect();
                    break;
                case TextEffectMode.Scramble:
                    UpdateScrambleEffect();
                    break;
                case TextEffectMode.Glitch:
                    UpdateGlitchEffect();
                    break;
                case TextEffectMode.FadeIn:
                    UpdateFadeInEffect();
                    break;
            }

            _textBox.Invalidate();
        }

        private void UpdateTypewriterEffect()
        {
            if (_effectPosition < _targetText.Length)
            {
                char c = _targetText[_effectPosition];
                _currentDisplayText += c;
                _effectPosition++;
                _effectProgress = (float)_effectPosition / _targetText.Length;

                CharacterTyped?.Invoke(this, new TypewriterEventArgs(c, _effectPosition, _targetText.Length));
                EffectProgressChanged?.Invoke(this, new EffectEventArgs(TextEffectMode.Typewriter, _effectProgress, _currentDisplayText));
            }
            else
            {
                CompleteEffect(TextEffectMode.Typewriter);
            }
        }

        private void UpdateScrambleEffect()
        {
            var elapsed = (DateTime.Now - _effectStartTime).TotalMilliseconds;
            _effectProgress = Math.Min(1f, (float)(elapsed / _effectConfig.ScrambleDuration));

            if (_effectProgress >= 1f)
            {
                _currentDisplayText = _targetText;
                CompleteEffect(TextEffectMode.Scramble);
                return;
            }

            // Gradually reveal characters
            int revealedCount = (int)(_targetText.Length * _effectProgress);
            char[] display = new char[_targetText.Length];

            for (int i = 0; i < _targetText.Length; i++)
            {
                if (i < revealedCount)
                {
                    display[i] = _targetText[i];
                }
                else
                {
                    display[i] = _effectConfig.ScrambleCharacters[_random.Next(_effectConfig.ScrambleCharacters.Length)];
                }
            }

            _currentDisplayText = new string(display);
            EffectProgressChanged?.Invoke(this, new EffectEventArgs(TextEffectMode.Scramble, _effectProgress, _currentDisplayText));
        }

        private void UpdateGlitchEffect()
        {
            var elapsed = (DateTime.Now - _effectStartTime).TotalMilliseconds;

            if (elapsed > _effectConfig.GlitchDuration)
            {
                _currentDisplayText = _targetText;
                CompleteEffect(TextEffectMode.Glitch);
                return;
            }

            // Random glitch characters
            if (_random.NextDouble() < _effectConfig.GlitchFrequency)
            {
                char[] display = _targetText.ToCharArray();
                int glitchCount = Math.Max(1, (int)(display.Length * 0.1));

                for (int i = 0; i < glitchCount; i++)
                {
                    int pos = _random.Next(display.Length);
                    display[pos] = _effectConfig.ScrambleCharacters[_random.Next(_effectConfig.ScrambleCharacters.Length)];
                }

                _currentDisplayText = new string(display);
            }
            else
            {
                _currentDisplayText = _targetText;
            }
        }

        private void UpdateFadeInEffect()
        {
            var elapsed = (DateTime.Now - _effectStartTime).TotalMilliseconds;
            _effectProgress = Math.Min(1f, (float)(elapsed / _effectConfig.FadeInDuration));

            if (_effectProgress >= 1f)
            {
                CompleteEffect(TextEffectMode.FadeIn);
            }

            EffectProgressChanged?.Invoke(this, new EffectEventArgs(TextEffectMode.FadeIn, _effectProgress, _currentDisplayText));
        }

        private void CompleteEffect(TextEffectMode mode)
        {
            _effectTimer.Stop();
            _isEffectRunning = false;
            _effectProgress = 1f;

            EffectCompleted?.Invoke(this, new EffectEventArgs(mode, 1f, _currentDisplayText));

            if (_effectConfig.Loop)
            {
                SetTextWithEffect(_targetText, mode);
            }
        }

        private void CursorBlinkTimer_Tick(object sender, EventArgs e)
        {
            _cursorVisible = !_cursorVisible;
            _textBox.Invalidate();
        }

        private void FlickerTimer_Tick(object sender, EventArgs e)
        {
            if (_terminalConfig.EnableFlicker)
            {
                _currentFlickerAlpha = 1f - ((float)_random.NextDouble() * _terminalConfig.FlickerIntensity);
                _textBox.Invalidate();
            }
        }

        private string GenerateScrambledText(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _effectConfig.ScrambleCharacters[_random.Next(_effectConfig.ScrambleCharacters.Length)];
            }
            return new string(chars);
        }

        #endregion

        #region Terminal Mode

        /// <summary>
        /// Enable terminal mode
        /// </summary>
        public void EnableTerminalMode(TerminalStylePreset preset = TerminalStylePreset.Classic)
        {
            TerminalModeEnabled = true;
            TerminalConfig = TerminalStyleConfig.FromPreset(preset);
            _cursorBlinkTimer.Start();

            if (_terminalConfig.EnableFlicker)
            {
                _flickerTimer.Start();
            }
        }

        /// <summary>
        /// Disable terminal mode
        /// </summary>
        public void DisableTerminalMode()
        {
            TerminalModeEnabled = false;
            _cursorBlinkTimer.Stop();
            _flickerTimer.Stop();
            _cursorVisible = true;
        }

        /// <summary>
        /// Append text with terminal-style output animation
        /// </summary>
        public void TerminalOutput(string text, bool animate = true)
        {
            if (animate)
            {
                StartTypewriterEffect(_textBox.Text + text);
            }
            else
            {
                _textBox.AppendText(text);
            }
        }

        /// <summary>
        /// Clear terminal and show prompt
        /// </summary>
        public void TerminalClear()
        {
            _textBox.Clear();
            _textBox.Text = _terminalConfig.Prompt;
        }

        /// <summary>
        /// Process command entered in terminal mode
        /// </summary>
        public void ProcessCommand(string input)
        {
            string command = input;
            
            if (command.StartsWith(_terminalConfig.Prompt))
            {
                command = command.Substring(_terminalConfig.Prompt.Length);
            }

            var args = new CommandEventArgs(command.Trim());
            CommandEntered?.Invoke(this, args);

            if (!args.Handled && !string.IsNullOrEmpty(args.Response))
            {
                TerminalOutput("\n" + args.Response + "\n" + _terminalConfig.Prompt);
            }
        }

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Draw text with terminal styling
        /// </summary>
        public void DrawTerminalText(Graphics g, Rectangle textRect, string text)
        {
            if (!TerminalModeEnabled) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Apply flicker
            float alpha = _currentFlickerAlpha;

            // Draw background
            using (var bgBrush = new SolidBrush(_terminalConfig.BackgroundColor))
            {
                g.FillRectangle(bgBrush, textRect);
            }

            // Draw scanlines
            if (_terminalConfig.EnableScanlines)
            {
                DrawScanlines(g, textRect);
            }

            // Draw text with glow
            if (_terminalConfig.EnableGlow)
            {
                DrawGlowText(g, textRect, text, alpha);
            }
            else
            {
                using (var brush = new SolidBrush(Color.FromArgb((int)(255 * alpha), _terminalConfig.TextColor)))
                {
                    g.DrawString(text, _terminalFont, brush, textRect);
                }
            }

            // Draw cursor
            if (_cursorVisible && _textBox.Focused)
            {
                DrawTerminalCursor(g, textRect, text);
            }

            // Draw CRT curvature effect
            if (_terminalConfig.EnableCRTCurvature)
            {
                DrawCRTEffect(g, textRect);
            }
        }

        private void DrawScanlines(Graphics g, Rectangle rect)
        {
            using (var pen = new Pen(Color.FromArgb((int)(255 * _terminalConfig.ScanlineOpacity), Color.Black), 1))
            {
                for (int y = rect.Top; y < rect.Bottom; y += 2)
                {
                    g.DrawLine(pen, rect.Left, y, rect.Right, y);
                }
            }
        }

        private void DrawGlowText(Graphics g, Rectangle rect, string text, float alpha)
        {
            // Draw glow layers
            int glowLayers = 3;
            float intensity = _terminalConfig.GlowIntensity;

            for (int i = glowLayers; i > 0; i--)
            {
                int glowAlpha = (int)(30 * intensity * (1f - (float)i / glowLayers) * alpha);
                using (var glowBrush = new SolidBrush(Color.FromArgb(glowAlpha, _terminalConfig.TextColor)))
                {
                    var glowRect = new RectangleF(rect.X - i, rect.Y - i, rect.Width, rect.Height);
                    g.DrawString(text, _terminalFont, glowBrush, glowRect);
                }
            }

            // Draw main text
            using (var brush = new SolidBrush(Color.FromArgb((int)(255 * alpha), _terminalConfig.TextColor)))
            {
                g.DrawString(text, _terminalFont, brush, rect);
            }
        }

        private void DrawTerminalCursor(Graphics g, Rectangle textRect, string text)
        {
            // Calculate cursor position
            string textBeforeCursor = text.Substring(0, Math.Min(_textBox.SelectionStart, text.Length));
            var textSize = TextRenderer.MeasureText(g, textBeforeCursor, _terminalFont, 
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

            int cursorX = textRect.X + textSize.Width;
            int cursorY = textRect.Y;
            int cursorWidth = 10;
            int cursorHeight = textSize.Height;

            switch (_terminalConfig.CursorStyle)
            {
                case TerminalCursorStyle.Block:
                    using (var brush = new SolidBrush(_terminalConfig.CursorColor))
                    {
                        g.FillRectangle(brush, cursorX, cursorY, cursorWidth, cursorHeight);
                    }
                    break;

                case TerminalCursorStyle.Underline:
                    using (var pen = new Pen(_terminalConfig.CursorColor, 2))
                    {
                        g.DrawLine(pen, cursorX, cursorY + cursorHeight - 2, cursorX + cursorWidth, cursorY + cursorHeight - 2);
                    }
                    break;

                case TerminalCursorStyle.Line:
                    using (var pen = new Pen(_terminalConfig.CursorColor, 2))
                    {
                        g.DrawLine(pen, cursorX, cursorY, cursorX, cursorY + cursorHeight);
                    }
                    break;
            }
        }

        private void DrawCRTEffect(Graphics g, Rectangle rect)
        {
            // Simple vignette effect for CRT simulation
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect.X - rect.Width / 4, rect.Y - rect.Height / 4, 
                    rect.Width * 1.5f, rect.Height * 1.5f);

                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.Transparent;
                    brush.SurroundColors = new[] { Color.FromArgb(60, 0, 0, 0) };
                    g.FillRectangle(brush, rect);
                }
            }
        }

        /// <summary>
        /// Draw text with fade effect
        /// </summary>
        public void DrawFadeText(Graphics g, Rectangle rect, string text, Font font, Color color)
        {
            int alpha = (int)(255 * _effectProgress);
            using (var brush = new SolidBrush(Color.FromArgb(alpha, color)))
            {
                g.DrawString(text, font, brush, rect);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            _effectTimer?.Stop();
            _effectTimer?.Dispose();
            _cursorBlinkTimer?.Stop();
            _cursorBlinkTimer?.Dispose();
            _flickerTimer?.Stop();
            _flickerTimer?.Dispose();

            _terminalFont?.Dispose();
            _textBrush?.Dispose();
            _dimBrush?.Dispose();
            _cursorBrush?.Dispose();
        }

        #endregion
    }
}

