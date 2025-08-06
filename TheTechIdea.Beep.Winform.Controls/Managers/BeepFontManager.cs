using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Managers
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
        /// <summary>
        /// Gets or sets the default font name
        /// </summary>
        public static string DefaultFontName
        {
            get => _defaultFontName;
            set
            {
                if (_defaultFontName != value)
                {
                    _defaultFontName = value;
                    ResetFonts();
                }
            }
        }

        /// <summary>
        /// Gets or sets the default font size
        /// </summary>
        public static float DefaultFontSize
        {
            get => _defaultFontSize;
            set
            {
                if (_defaultFontSize != value)
                {
                    _defaultFontSize = value;
                    ResetFonts();
                }
            }
        }

        /// <summary>
        /// Gets or sets the default font style
        /// </summary>
        public static FontStyle DefaultFontStyle
        {
            get => _defaultFontStyle;
            set
            {
                if (_defaultFontStyle != value)
                {
                    _defaultFontStyle = value;
                    ResetFonts();
                }
            }
        }

        /// <summary>
        /// Gets or sets the application font name
        /// </summary>
        public static string AppFontName
        {
            get => _appFontName;
            set
            {
                if (_appFontName != value)
                {
                    _appFontName = value;
                    ResetFonts();
                }
            }
        }

        /// <summary>
        /// Gets or sets the application font size
        /// </summary>
        public static float AppFontSize
        {
            get => _appFontSize;
            set
            {
                if (_appFontSize != value)
                {
                    _appFontSize = value;
                    ResetFonts();
                }
            }
        }

        /// <summary>
        /// Gets or sets the application font style
        /// </summary>
        public static FontStyle AppFontStyle
        {
            get => _appFontStyle;
            set
            {
                if (_appFontStyle != value)
                {
                    _appFontStyle = value;
                    ResetFonts();
                }
            }
        }

        /// <summary>
        /// Gets the default font for the application
        /// </summary>
        public static Font DefaultFont => _defaultFont ?? (_defaultFont = GetOrCreateFont(_defaultFontName, _defaultFontSize, _defaultFontStyle));

        /// <summary>
        /// Gets the font for buttons
        /// </summary>
        public static Font ButtonFont => _buttonFont ?? (_buttonFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the font for labels
        /// </summary>
        public static Font LabelFont => _labelFont ?? (_labelFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the font for headers
        /// </summary>
        public static Font HeaderFont => _headerFont ?? (_headerFont = GetOrCreateFont(_appFontName, _appFontSize + 2, FontStyle.Bold));

        /// <summary>
        /// Gets the font for titles
        /// </summary>
        public static Font TitleFont => _titleFont ?? (_titleFont = GetOrCreateFont(_appFontName, _appFontSize + 4, FontStyle.Bold));

        /// <summary>
        /// Gets the font for menus
        /// </summary>
        public static Font MenuFont => _menuFont ?? (_menuFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the font for tooltips
        /// </summary>
        public static Font TooltipFont => _tooltipFont ?? (_tooltipFont = GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle));

        /// <summary>
        /// Gets the font for status bar
        /// </summary>
        public static Font StatusBarFont => _statusBarFont ?? (_statusBarFont = GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle));

        /// <summary>
        /// Gets the font for dialogs
        /// </summary>
        public static Font DialogFont => _dialogFont ?? (_dialogFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the monospace font for code or fixed-width text
        /// </summary>
        public static Font MonospaceFont => _monospaceFont ?? (_monospaceFont = GetOrCreateFont("Consolas", _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the list of all font configurations
        /// </summary>
        public static List<FontConfiguration> FontConfigurations => FontListHelper.FontConfigurations;

        /// <summary>
        /// Gets the list of system font names
        /// </summary>
        public static List<string> SystemFonts => FontListHelper.GetSystemFontNames();

        /// <summary>
        /// Gets the list of private/custom font names
        /// </summary>
        public static List<string> CustomFonts => FontListHelper.GetPrivateFontNames();

        /// <summary>
        /// Gets all available font names
        /// </summary>
        public static List<string> AllFonts => FontListHelper.GetFontNames();
        #endregion

        #region "Initialization"
        /// <summary>
        /// Initializes the font manager by discovering fonts
        /// </summary>
        public static async Task Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                // Load system fonts
                FontListHelper.GetSystemFonts();

                // Load application fonts
                string appFontPath = AppDomain.CurrentDomain.BaseDirectory;
                string fontsFolderPath = System.IO.Path.Combine(appFontPath, "Fonts");

                // If we have a dedicated fonts folder, load from there
                if (System.IO.Directory.Exists(fontsFolderPath))
                {
                    FontListHelper.GetFontFilesLocations(fontsFolderPath);
                }

                // Load embedded fonts
                await Task.Run(() => FontListHelper.GetFontResourcesFromEmbedded(new[] { "TheTechIdea" }));

                // Create default fonts
                CreateDefaultFonts();

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                ////MiscFunctions.SendLog("Error in Getting Font Resources");
            }
        }

        /// <summary>
        /// Creates the default font instances
        /// </summary>
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

        /// <summary>
        /// Resets all font instances when preferences change
        /// </summary>
        private static void ResetFonts()
        {
            // Clear existing fonts
            ClearFont(ref _defaultFont);
            ClearFont(ref _buttonFont);
            ClearFont(ref _labelFont);
            ClearFont(ref _headerFont);
            ClearFont(ref _titleFont);
            ClearFont(ref _menuFont);
            ClearFont(ref _tooltipFont);
            ClearFont(ref _statusBarFont);
            ClearFont(ref _dialogFont);
            ClearFont(ref _monospaceFont);

            // Recreate fonts
            CreateDefaultFonts();
        }

        /// <summary>
        /// Gets or creates a font with fallback
        /// </summary>
        private static Font GetOrCreateFont(string fontName, float size, FontStyle style)
        {
            // Try to get the font from the helper
            Font font = FontListHelper.GetFont(fontName, size, style);

            // If not found, use a fallback
            if (font == null)
            {
                font = FontListHelper.GetFontWithFallback("Segoe UI", "Arial", size, style);

                // If still null, create a generic font
                if (font == null)
                {
                    try
                    {
                        font = new Font(FontFamily.GenericSansSerif, size, style);
                    }
                    catch
                    {
                        // Last resort: try to create an Arial font
                        try
                        {
                            font = new Font("Arial", size, style);
                        }
                        catch
                        {
                            // If all else fails, return null and let the caller handle it
                            return null;
                        }
                    }
                }
            }

            return font;
        }
        #endregion

        #region "Font Management"
        /// <summary>
        /// Gets a custom font by name with a specified size and style
        /// </summary>
        /// <param name="fontName">Name of the font</param>
        /// <param name="size">Font size</param>
        /// <param name="style">Font style</param>
        /// <returns>Font instance or null if not found</returns>
        public static Font GetFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            return FontListHelper.GetFont(fontName, size, style);
        }

        /// <summary>
        /// Gets a font suitable for the given UI element type
        /// </summary>
        /// <param name="elementType">Type of UI element</param>
        /// <returns>Font instance appropriate for the element type</returns>
        public static Font GetFontForElement(UIElementType elementType)
        {
            switch (elementType)
            {
                case UIElementType.Button:
                    return ButtonFont;
                case UIElementType.Label:
                    return LabelFont;
                case UIElementType.Header:
                    return HeaderFont;
                case UIElementType.Title:
                    return TitleFont;
                case UIElementType.Menu:
                    return MenuFont;
                case UIElementType.Tooltip:
                    return TooltipFont;
                case UIElementType.StatusBar:
                    return StatusBarFont;
                case UIElementType.Dialog:
                    return DialogFont;
                case UIElementType.Code:
                    return MonospaceFont;
                default:
                    return DefaultFont;
            }
        }

        /// <summary>
        /// Loads fonts from a specific folder
        /// </summary>
        /// <param name="folderPath">Path to the folder containing font files</param>
        /// <returns>List of loaded font configurations</returns>
        public static List<FontConfiguration> LoadFontsFromFolder(string folderPath)
        {
            return FontListHelper.GetFontFilesLocations(folderPath);
        }

        /// <summary>
        /// Sets default font preferences based on a theme or user settings
        /// </summary>
        /// <param name="fontName">Font name to use</param>
        /// <param name="fontSize">Font size to use</param>
        public static void SetDefaultFont(string fontName, float fontSize)
        {
            // Update the default font properties
            _defaultFontName = fontName;
            _defaultFontSize = fontSize;

            // Also update app font to match
            _appFontName = fontName;
            _appFontSize = fontSize;

            // Reset all fonts to apply the changes
            ResetFonts();
        }

        /// <summary>
        /// Scales all fonts by a given factor
        /// </summary>
        /// <param name="scaleFactor">Scale factor (e.g. 1.25 for 125%)</param>
        public static void ScaleFonts(float scaleFactor)
        {
            if (scaleFactor <= 0)
                return;

            _defaultFontSize *= scaleFactor;
            _appFontSize *= scaleFactor;

            ResetFonts();
        }

        /// <summary>
        /// Checks if a specified font is available in the system
        /// </summary>
        /// <param name="fontName">Name of the font to check</param>
        /// <returns>True if the font is available</returns>
        public static bool IsFontAvailable(string fontName)
        {
            return FontListHelper.GetFontIndex(fontName) != -1;
        }

        /// <summary>
        /// Convert a BeepTheme TypographyStyle to a Font
        /// </summary>
        /// <param name="style">Typography style from theme</param>
        /// <returns>Font instance</returns>
        public static Font ToFont(TypographyStyle style)
        {
            if (style == null) return DefaultFont;

            return GetFont(
                style.FontFamily,
                style.FontSize,
                ConvertFontWeight(style.FontWeight)
            );
        }

        /// <summary>
        /// Convert FontWeight enum to FontStyle
        /// </summary>
        /// <param name="weight">Font weight value</param>
        /// <returns>Appropriate FontStyle</returns>
        public static FontStyle ConvertFontWeight(FontWeight weight)
        {
            return weight >= FontWeight.Bold ? FontStyle.Bold : FontStyle.Regular;
        }

        /// <summary>
        /// Get font weight multiplier for styling
        /// </summary>
        /// <param name="weight">Font weight</param>
        /// <returns>Multiplier value between 0.1 and 0.9</returns>
        public static float GetFontWeightMultiplier(FontWeight weight)
        {
            return (float)weight / 1000f;
        }
        #endregion

        #region "Cleanup"
        /// <summary>
        /// Clears a font reference
        /// </summary>
        private static void ClearFont(ref Font font)
        {
            // Fonts from FontListHelper are cached, so we don't dispose them
            // Just set the reference to null
            font = null;
        }

        /// <summary>
        /// Clean up all font references (call before application exit)
        /// </summary>
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

    /// <summary>
    /// Enum defining types of UI elements for font assignment
    /// </summary>
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