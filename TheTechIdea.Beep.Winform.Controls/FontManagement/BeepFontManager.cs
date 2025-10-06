using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Static manager class for application fonts and typography
    /// </summary>
    public static class BeepFontManager
    {
        // Default font settings
        private static string _defaultFontName = "Segoe UI";
        private static float _defaultFontSize = 9.0f;
        private static FontStyle _defaultFontStyle = FontStyle.Regular;

        // Application font settings
        private static string _appFontName = "Segoe UI";
        private static float _appFontSize = 9.0f;
        private static FontStyle _appFontStyle = FontStyle.Regular;

        // Font instances for various UI elements
        private static Font _defaultFont;
        private static Font _buttonFont;
        private static Font _labelFont;
        private static Font _headerFont;
        private static Font _titleFont;
        private static Font _menuFont;
        private static Font _tooltipFont;
        private static Font _statusBarFont;
        private static Font _dialogFont;
        private static Font _monospaceFont;

        // Flag to track initialization
        private static bool _isInitialized = false;

        #region "Properties"
        public static string DefaultFontName
        {
            get => _defaultFontName;
            set { if (_defaultFontName != value) { _defaultFontName = value; ResetFonts(); } }
        }
        public static float DefaultFontSize
        {
            get => _defaultFontSize;
            set { if (_defaultFontSize != value) { _defaultFontSize = value; ResetFonts(); } }
        }
        public static FontStyle DefaultFontStyle
        {
            get => _defaultFontStyle;
            set { if (_defaultFontStyle != value) { _defaultFontStyle = value; ResetFonts(); } }
        }
        public static string AppFontName
        {
            get => _appFontName;
            set { if (_appFontName != value) { _appFontName = value; ResetFonts(); } }
        }
        public static float AppFontSize
        {
            get => _appFontSize;
            set { if (_appFontSize != value) { _appFontSize = value; ResetFonts(); } }
        }
        public static FontStyle AppFontStyle
        {
            get => _appFontStyle;
            set { if (_appFontStyle != value) { _appFontStyle = value; ResetFonts(); } }
        }

        public static Font DefaultFont => _defaultFont ??= GetOrCreateFont(_defaultFontName, _defaultFontSize, _defaultFontStyle);
        public static Font ButtonFont => _buttonFont ??= GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
        public static Font LabelFont => _labelFont ??= GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
        public static Font HeaderFont => _headerFont ??= GetOrCreateFont(_appFontName, _appFontSize + 2, FontStyle.Bold);
        public static Font TitleFont => _titleFont ??= GetOrCreateFont(_appFontName, _appFontSize + 4, FontStyle.Bold);
        public static Font MenuFont => _menuFont ??= GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
        public static Font TooltipFont => _tooltipFont ??= GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle);
        public static Font StatusBarFont => _statusBarFont ??= GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle);
        public static Font DialogFont => _dialogFont ??= GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
        public static Font MonospaceFont => _monospaceFont ??= GetOrCreateFont("Consolas", _appFontSize, _appFontStyle);

        public static List<FontConfiguration> FontConfigurations => FontListHelper.FontConfigurations;
        public static List<string> SystemFonts => FontListHelper.GetSystemFontNames();
        public static List<string> CustomFonts => FontListHelper.GetPrivateFontNames();
        public static List<string> AllFonts => FontListHelper.GetFontNames();
        #endregion

        #region "Initialization"
        public static async Task Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                // Orchestrated scan
                var opt = new FontListHelper.FontScanOptions
                {
                    ScanSystemFonts = true,
                    Directories = new List<string>
                    {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts")
                    },
                    EmbeddedNamespaces = new[]
                    {
                        "TheTechIdea.Beep.Winform.Controls.Fonts",
                        "TheTechIdea.Beep.Fonts"
                    },
                    IncludeFrameworkAssemblies = false,
                    IncludeReferencedAssemblies = true,
                    MaxReferenceDepth = 2
                };

                await Task.Run(() => FontListHelper.LoadAllFonts(opt));

                // Create default fonts
                CreateDefaultFonts();

                _isInitialized = true;
            }
            catch
            {
                // swallow, consumers will fallback to generic fonts
            }
        }

        private static void CreateDefaultFonts()
        {
            _defaultFont = GetOrCreateFont(_defaultFontName, _defaultFontSize, _defaultFontStyle);
            _buttonFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
            _labelFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
            _headerFont = GetOrCreateFont(_appFontName, _appFontSize + 2, FontStyle.Bold);
            _titleFont = GetOrCreateFont(_appFontName, _appFontSize + 4, FontStyle.Bold);
            _menuFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
            _tooltipFont = GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle);
            _statusBarFont = GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle);
            _dialogFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
            _monospaceFont = GetOrCreateFont("Consolas", _appFontSize, _appFontStyle);
        }

        private static void ResetFonts()
        {
            _defaultFont = null;
            _buttonFont = null;
            _labelFont = null;
            _headerFont = null;
            _titleFont = null;
            _menuFont = null;
            _tooltipFont = null;
            _statusBarFont = null;
            _dialogFont = null;
            _monospaceFont = null;

            CreateDefaultFonts();
        }

        private static Font GetOrCreateFont(string fontName, float size, FontStyle style)
        {
            var font = FontListHelper.GetFont(fontName, size, style);
            if (font != null) return font;

            // resilient fallbacks
            try { return new Font(FontFamily.GenericSansSerif, size, style); }
            catch { }
            try { return new Font("Arial", size, style); }
            catch { return null; }
        }
        #endregion

        #region "Font Management"
        public static Font GetFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            return FontListHelper.GetFont(fontName, size, style);
        }

        public static Font GetFontForElement(UIElementType elementType)
        {
            return elementType switch
            {
                UIElementType.Button => ButtonFont,
                UIElementType.Label => LabelFont,
                UIElementType.Header => HeaderFont,
                UIElementType.Title => TitleFont,
                UIElementType.Menu => MenuFont,
                UIElementType.Tooltip => TooltipFont,
                UIElementType.StatusBar => StatusBarFont,
                UIElementType.Dialog => DialogFont,
                UIElementType.Code => MonospaceFont,
                _ => DefaultFont
            };
        }

        public static List<FontConfiguration> LoadFontsFromFolder(string folderPath)
        {
            return FontListHelper.GetFontFilesLocations(folderPath);
        }

        public static void SetDefaultFont(string fontName, float fontSize)
        {
            _defaultFontName = fontName;
            _defaultFontSize = fontSize;
            _appFontName = fontName;
            _appFontSize = fontSize;
            ResetFonts();
        }

        public static void ScaleFonts(float scaleFactor)
        {
            if (scaleFactor <= 0) return;
            _defaultFontSize *= scaleFactor;
            _appFontSize *= scaleFactor;
            ResetFonts();
        }

        public static bool IsFontAvailable(string fontName)
        {
            return FontListHelper.GetFontIndex(fontName) != -1;
        }

        public static Font ToFont(TypographyStyle style)
        {
            if (style == null) return DefaultFont;
            return GetFont(style.FontFamily, style.FontSize,
                style.FontWeight >= FontWeight.Bold ? FontStyle.Bold : FontStyle.Regular);
        }

        public static float GetFontWeightMultiplier(FontWeight weight)
        {
            return (float)weight / 1000f;
        }
        #endregion

        #region "Cleanup"
        public static void Cleanup()
        {
            _defaultFont = null;
            _buttonFont = null;
            _labelFont = null;
            _headerFont = null;
            _titleFont = null;
            _menuFont = null;
            _tooltipFont = null;
            _statusBarFont = null;
            _dialogFont = null;
            _monospaceFont = null;
            _isInitialized = false;
        }
        #endregion
    }

    public enum UIElementType
    {
        Default,
        Button,
        Label,
        Header,
        Title,
        Menu,
        Tooltip,
        StatusBar,
        Dialog,
        Code
    }
}