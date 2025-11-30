using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Models
{
    /// <summary>
    /// Text effect mode for BeepTextBox
    /// </summary>
    public enum TextEffectMode
    {
        /// <summary>No special effect</summary>
        None,
        
        /// <summary>Characters appear one by one like typing</summary>
        Typewriter,
        
        /// <summary>Terminal/console style appearance</summary>
        Terminal,
        
        /// <summary>Matrix-style falling characters</summary>
        Matrix,
        
        /// <summary>Random character glitch effect</summary>
        Glitch,
        
        /// <summary>Text fades in character by character</summary>
        FadeIn,
        
        /// <summary>Random characters resolve to final text</summary>
        Scramble,
        
        /// <summary>Text slides in from direction</summary>
        SlideIn,
        
        /// <summary>Retro computer boot sequence</summary>
        RetroComputer
    }

    /// <summary>
    /// Terminal cursor style
    /// </summary>
    public enum TerminalCursorStyle
    {
        /// <summary>Blinking block cursor █</summary>
        Block,
        
        /// <summary>Blinking underline cursor _</summary>
        Underline,
        
        /// <summary>Blinking vertical line cursor |</summary>
        Line,
        
        /// <summary>No visible cursor</summary>
        Hidden
    }

    /// <summary>
    /// Preset terminal style themes
    /// </summary>
    public enum TerminalStylePreset
    {
        /// <summary>Classic green on black</summary>
        Classic,
        
        /// <summary>Modern terminal with subtle colors</summary>
        Modern,
        
        /// <summary>Matrix green rain style</summary>
        Matrix,
        
        /// <summary>Amber/orange on black (vintage)</summary>
        Amber,
        
        /// <summary>Cyberpunk neon style</summary>
        Cyberpunk,
        
        /// <summary>Retro blue on white</summary>
        RetroBlue,
        
        /// <summary>Hacker dark theme</summary>
        Hacker,
        
        /// <summary>Solarized dark</summary>
        SolarizedDark,
        
        /// <summary>Dracula theme</summary>
        Dracula,
        
        /// <summary>Nord theme</summary>
        Nord,
        
        /// <summary>Custom (use custom colors)</summary>
        Custom
    }

    /// <summary>
    /// Configuration for terminal style appearance
    /// </summary>
    public class TerminalStyleConfig
    {
        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.Black;

        /// <summary>
        /// Primary text color
        /// </summary>
        public Color TextColor { get; set; } = Color.FromArgb(0, 255, 0);

        /// <summary>
        /// Secondary/dim text color
        /// </summary>
        public Color DimTextColor { get; set; } = Color.FromArgb(0, 128, 0);

        /// <summary>
        /// Cursor color
        /// </summary>
        public Color CursorColor { get; set; } = Color.FromArgb(0, 255, 0);

        /// <summary>
        /// Selection highlight color
        /// </summary>
        public Color SelectionColor { get; set; } = Color.FromArgb(0, 100, 0);

        /// <summary>
        /// Error/warning text color
        /// </summary>
        public Color ErrorColor { get; set; } = Color.Red;

        /// <summary>
        /// Success text color
        /// </summary>
        public Color SuccessColor { get; set; } = Color.Lime;

        /// <summary>
        /// Prompt text color
        /// </summary>
        public Color PromptColor { get; set; } = Color.Cyan;

        /// <summary>
        /// Font family (monospace recommended)
        /// </summary>
        public string FontFamily { get; set; } = "Consolas";

        /// <summary>
        /// Font size
        /// </summary>
        public float FontSize { get; set; } = 12f;

        /// <summary>
        /// Enable scanline effect
        /// </summary>
        public bool EnableScanlines { get; set; } = false;

        /// <summary>
        /// Scanline opacity (0-1)
        /// </summary>
        public float ScanlineOpacity { get; set; } = 0.1f;

        /// <summary>
        /// Enable CRT screen curvature effect
        /// </summary>
        public bool EnableCRTCurvature { get; set; } = false;

        /// <summary>
        /// Enable text glow effect
        /// </summary>
        public bool EnableGlow { get; set; } = true;

        /// <summary>
        /// Glow intensity (0-1)
        /// </summary>
        public float GlowIntensity { get; set; } = 0.3f;

        /// <summary>
        /// Enable flicker effect
        /// </summary>
        public bool EnableFlicker { get; set; } = false;

        /// <summary>
        /// Flicker intensity (0-1)
        /// </summary>
        public float FlickerIntensity { get; set; } = 0.05f;

        /// <summary>
        /// Command prompt prefix
        /// </summary>
        public string Prompt { get; set; } = "> ";

        /// <summary>
        /// Cursor style
        /// </summary>
        public TerminalCursorStyle CursorStyle { get; set; } = TerminalCursorStyle.Block;

        /// <summary>
        /// Cursor blink rate in milliseconds
        /// </summary>
        public int CursorBlinkRate { get; set; } = 530;

        /// <summary>
        /// Creates a config from a preset
        /// </summary>
        public static TerminalStyleConfig FromPreset(TerminalStylePreset preset)
        {
            return preset switch
            {
                TerminalStylePreset.Classic => new TerminalStyleConfig
                {
                    BackgroundColor = Color.Black,
                    TextColor = Color.FromArgb(0, 255, 0),
                    DimTextColor = Color.FromArgb(0, 128, 0),
                    CursorColor = Color.FromArgb(0, 255, 0),
                    SelectionColor = Color.FromArgb(0, 100, 0),
                    PromptColor = Color.FromArgb(0, 255, 0),
                    EnableGlow = true,
                    GlowIntensity = 0.2f
                },
                
                TerminalStylePreset.Modern => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(30, 30, 30),
                    TextColor = Color.FromArgb(204, 204, 204),
                    DimTextColor = Color.FromArgb(128, 128, 128),
                    CursorColor = Color.White,
                    SelectionColor = Color.FromArgb(38, 79, 120),
                    PromptColor = Color.FromArgb(86, 156, 214),
                    EnableGlow = false,
                    FontFamily = "Cascadia Code"
                },
                
                TerminalStylePreset.Matrix => new TerminalStyleConfig
                {
                    BackgroundColor = Color.Black,
                    TextColor = Color.FromArgb(0, 255, 65),
                    DimTextColor = Color.FromArgb(0, 100, 0),
                    CursorColor = Color.FromArgb(0, 255, 65),
                    SelectionColor = Color.FromArgb(0, 80, 0),
                    PromptColor = Color.FromArgb(0, 255, 65),
                    EnableGlow = true,
                    GlowIntensity = 0.4f,
                    EnableScanlines = true,
                    ScanlineOpacity = 0.08f
                },
                
                TerminalStylePreset.Amber => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(20, 15, 5),
                    TextColor = Color.FromArgb(255, 176, 0),
                    DimTextColor = Color.FromArgb(180, 120, 0),
                    CursorColor = Color.FromArgb(255, 176, 0),
                    SelectionColor = Color.FromArgb(100, 70, 0),
                    PromptColor = Color.FromArgb(255, 200, 50),
                    EnableGlow = true,
                    GlowIntensity = 0.25f,
                    EnableScanlines = true,
                    ScanlineOpacity = 0.1f,
                    EnableCRTCurvature = true
                },
                
                TerminalStylePreset.Cyberpunk => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(13, 2, 33),
                    TextColor = Color.FromArgb(0, 255, 255),
                    DimTextColor = Color.FromArgb(0, 128, 128),
                    CursorColor = Color.FromArgb(255, 0, 255),
                    SelectionColor = Color.FromArgb(80, 0, 80),
                    PromptColor = Color.FromArgb(255, 0, 128),
                    ErrorColor = Color.FromArgb(255, 0, 64),
                    SuccessColor = Color.FromArgb(0, 255, 128),
                    EnableGlow = true,
                    GlowIntensity = 0.5f
                },
                
                TerminalStylePreset.RetroBlue => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(0, 0, 170),
                    TextColor = Color.FromArgb(170, 170, 170),
                    DimTextColor = Color.FromArgb(85, 85, 85),
                    CursorColor = Color.White,
                    SelectionColor = Color.FromArgb(0, 0, 100),
                    PromptColor = Color.Yellow,
                    EnableGlow = false
                },
                
                TerminalStylePreset.Hacker => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(10, 10, 10),
                    TextColor = Color.FromArgb(0, 200, 0),
                    DimTextColor = Color.FromArgb(0, 100, 0),
                    CursorColor = Color.FromArgb(0, 255, 0),
                    SelectionColor = Color.FromArgb(0, 60, 0),
                    PromptColor = Color.FromArgb(0, 255, 0),
                    EnableGlow = true,
                    GlowIntensity = 0.35f,
                    EnableFlicker = true,
                    FlickerIntensity = 0.02f
                },
                
                TerminalStylePreset.SolarizedDark => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(0, 43, 54),
                    TextColor = Color.FromArgb(131, 148, 150),
                    DimTextColor = Color.FromArgb(88, 110, 117),
                    CursorColor = Color.FromArgb(147, 161, 161),
                    SelectionColor = Color.FromArgb(7, 54, 66),
                    PromptColor = Color.FromArgb(38, 139, 210),
                    ErrorColor = Color.FromArgb(220, 50, 47),
                    SuccessColor = Color.FromArgb(133, 153, 0),
                    EnableGlow = false
                },
                
                TerminalStylePreset.Dracula => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(40, 42, 54),
                    TextColor = Color.FromArgb(248, 248, 242),
                    DimTextColor = Color.FromArgb(98, 114, 164),
                    CursorColor = Color.FromArgb(248, 248, 242),
                    SelectionColor = Color.FromArgb(68, 71, 90),
                    PromptColor = Color.FromArgb(189, 147, 249),
                    ErrorColor = Color.FromArgb(255, 85, 85),
                    SuccessColor = Color.FromArgb(80, 250, 123),
                    EnableGlow = false,
                    FontFamily = "Fira Code"
                },
                
                TerminalStylePreset.Nord => new TerminalStyleConfig
                {
                    BackgroundColor = Color.FromArgb(46, 52, 64),
                    TextColor = Color.FromArgb(216, 222, 233),
                    DimTextColor = Color.FromArgb(76, 86, 106),
                    CursorColor = Color.FromArgb(236, 239, 244),
                    SelectionColor = Color.FromArgb(67, 76, 94),
                    PromptColor = Color.FromArgb(136, 192, 208),
                    ErrorColor = Color.FromArgb(191, 97, 106),
                    SuccessColor = Color.FromArgb(163, 190, 140),
                    EnableGlow = false
                },
                
                _ => new TerminalStyleConfig()
            };
        }

        /// <summary>
        /// Creates a font based on config
        /// </summary>
        public Font CreateFont()
        {
            try
            {
                return new Font(FontFamily, FontSize, FontStyle.Regular);
            }
            catch
            {
                return new Font("Consolas", FontSize, FontStyle.Regular);
            }
        }
    }

    /// <summary>
    /// Configuration for text effects
    /// </summary>
    public class TextEffectConfig
    {
        /// <summary>
        /// Effect mode
        /// </summary>
        public TextEffectMode Mode { get; set; } = TextEffectMode.None;

        /// <summary>
        /// Characters per second for typewriter effect
        /// </summary>
        public int TypewriterSpeed { get; set; } = 50;

        /// <summary>
        /// Enable typing sound for typewriter effect
        /// </summary>
        public bool TypewriterSoundEnabled { get; set; } = false;

        /// <summary>
        /// Duration of scramble effect in milliseconds
        /// </summary>
        public int ScrambleDuration { get; set; } = 1000;

        /// <summary>
        /// Characters used for scramble effect
        /// </summary>
        public string ScrambleCharacters { get; set; } = "!@#$%^&*()_+-=[]{}|;':\",./<>?0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Glitch frequency (0-1)
        /// </summary>
        public float GlitchFrequency { get; set; } = 0.1f;

        /// <summary>
        /// Glitch duration in milliseconds
        /// </summary>
        public int GlitchDuration { get; set; } = 100;

        /// <summary>
        /// Fade in duration in milliseconds
        /// </summary>
        public int FadeInDuration { get; set; } = 500;

        /// <summary>
        /// Slide direction for slide-in effect
        /// </summary>
        public SlideDirection SlideDirection { get; set; } = SlideDirection.Left;

        /// <summary>
        /// Slide duration in milliseconds
        /// </summary>
        public int SlideDuration { get; set; } = 300;

        /// <summary>
        /// Loop the effect continuously
        /// </summary>
        public bool Loop { get; set; } = false;

        /// <summary>
        /// Delay before effect starts (milliseconds)
        /// </summary>
        public int StartDelay { get; set; } = 0;
    }

    /// <summary>
    /// Slide direction for effects
    /// </summary>
    public enum SlideDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    /// <summary>
    /// Event arguments for effect events
    /// </summary>
    public class EffectEventArgs : EventArgs
    {
        public TextEffectMode EffectMode { get; set; }
        public float Progress { get; set; }
        public bool IsComplete { get; set; }
        public string CurrentText { get; set; }

        public EffectEventArgs(TextEffectMode mode, float progress, string currentText)
        {
            EffectMode = mode;
            Progress = progress;
            CurrentText = currentText;
            IsComplete = progress >= 1.0f;
        }
    }

    /// <summary>
    /// Event arguments for typewriter effect
    /// </summary>
    public class TypewriterEventArgs : EventArgs
    {
        public char Character { get; set; }
        public int Position { get; set; }
        public int TotalLength { get; set; }
        public float Progress => TotalLength > 0 ? (float)Position / TotalLength : 0;

        public TypewriterEventArgs(char character, int position, int totalLength)
        {
            Character = character;
            Position = position;
            TotalLength = totalLength;
        }
    }

    /// <summary>
    /// Event arguments for terminal command
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        public string Command { get; set; }
        public string[] Arguments { get; set; }
        public bool Handled { get; set; }
        public string Response { get; set; }

        public CommandEventArgs(string command)
        {
            Command = command;
            Arguments = command?.Split(' ') ?? Array.Empty<string>();
        }
    }
}

