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
    /// Manager class for application fonts and typography
    /// </summary>
    public class BeepFontManager : IDisposable
    {
        private IDMEEditor _dmeEditor;
        private IAppManager _visManager;
        
        // Default font settings
        private string _defaultFontName = "Segoe UI";
        private float _defaultFontSize = 9.0f;
        private FontStyle _defaultFontStyle = FontStyle.Regular;
        
        // Application font settings
        private string _appFontName = "Segoe UI";
        private float _appFontSize = 9.0f;
        private FontStyle _appFontStyle = FontStyle.Regular;

        // Font instances for various UI elements
        private Font _defaultFont;
        private Font _buttonFont;
        private Font _labelFont;
        private Font _headerFont;
        private Font _titleFont;
        private Font _menuFont;
        private Font _tooltipFont;
        private Font _statusBarFont;
        private Font _dialogFont;
        private Font _monospaceFont;

        // Flag to track initialization
        private bool _isInitialized = false;
        private bool _disposedValue = false;

        /// <summary>
        /// Constructor for BeepFontManager
        /// </summary>
        /// <param name="dmeEditor">DME Editor instance</param>
        /// <param name="visManager">Visual Manager instance</param>
        public BeepFontManager(IDMEEditor dmeEditor, IAppManager visManager)
        {
            _dmeEditor = dmeEditor;
            _visManager = visManager;
            
            // Pass references to the FontListHelper
            FontListHelper.DMEEditor = dmeEditor;
            FontListHelper.Vismanager = visManager;
        }

        #region "Properties"
        /// <summary>
        /// Gets or sets the default font name
        /// </summary>
        public string DefaultFontName
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
        public float DefaultFontSize
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
        public FontStyle DefaultFontStyle
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
        public string AppFontName
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
        public float AppFontSize
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
        public FontStyle AppFontStyle
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
        public Font DefaultFont => _defaultFont ?? (_defaultFont = GetOrCreateFont(_defaultFontName, _defaultFontSize, _defaultFontStyle));

        /// <summary>
        /// Gets the font for buttons
        /// </summary>
        public Font ButtonFont => _buttonFont ?? (_buttonFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the font for labels
        /// </summary>
        public Font LabelFont => _labelFont ?? (_labelFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the font for headers
        /// </summary>
        public Font HeaderFont => _headerFont ?? (_headerFont = GetOrCreateFont(_appFontName, _appFontSize + 2, FontStyle.Bold));

        /// <summary>
        /// Gets the font for titles
        /// </summary>
        public Font TitleFont => _titleFont ?? (_titleFont = GetOrCreateFont(_appFontName, _appFontSize + 4, FontStyle.Bold));

        /// <summary>
        /// Gets the font for menus
        /// </summary>
        public Font MenuFont => _menuFont ?? (_menuFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the font for tooltips
        /// </summary>
        public Font TooltipFont => _tooltipFont ?? (_tooltipFont = GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle));

        /// <summary>
        /// Gets the font for status bar
        /// </summary>
        public Font StatusBarFont => _statusBarFont ?? (_statusBarFont = GetOrCreateFont(_appFontName, _appFontSize - 1, _appFontStyle));

        /// <summary>
        /// Gets the font for dialogs
        /// </summary>
        public Font DialogFont => _dialogFont ?? (_dialogFont = GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the monospace font for code or fixed-width text
        /// </summary>
        public Font MonospaceFont => _monospaceFont ?? (_monospaceFont = GetOrCreateFont("Consolas", _appFontSize, _appFontStyle));

        /// <summary>
        /// Gets the list of all font configurations
        /// </summary>
        public List<FontConfiguration> FontConfigurations => FontListHelper.FontConfigurations;

        /// <summary>
        /// Gets the list of system font names
        /// </summary>
        public List<string> SystemFonts => FontListHelper.GetSystemFontNames();

        /// <summary>
        /// Gets the list of private/custom font names
        /// </summary>
        public List<string> CustomFonts => FontListHelper.GetPrivateFontNames();

        /// <summary>
        /// Gets all available font names
        /// </summary>
        public List<string> AllFonts => FontListHelper.GetFontNames();
        #endregion

        #region "Initialization"
        /// <summary>
        /// Initializes the font manager by discovering fonts
        /// </summary>
        public async Task Initialize()
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
                _dmeEditor?.AddLogMessage("Error", $"Failed to initialize font manager: {ex.Message}", DateTime.Now, -1, null, ConfigUtil.Errors.Failed);
            }
        }

        /// <summary>
        /// Creates the default font instances
        /// </summary>
        private void CreateDefaultFonts()
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
        private void ResetFonts()
        {
            // Dispose existing fonts
            DisposeFont(ref _defaultFont);
            DisposeFont(ref _buttonFont);
            DisposeFont(ref _labelFont);
            DisposeFont(ref _headerFont);
            DisposeFont(ref _titleFont);
            DisposeFont(ref _menuFont);
            DisposeFont(ref _tooltipFont);
            DisposeFont(ref _statusBarFont);
            DisposeFont(ref _dialogFont);
            DisposeFont(ref _monospaceFont);
            
            // Recreate fonts
            CreateDefaultFonts();
        }

        /// <summary>
        /// Gets or creates a font with fallback
        /// </summary>
        private Font GetOrCreateFont(string fontName, float size, FontStyle style)
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
        public Font GetFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            return FontListHelper.GetFont(fontName, size, style);
        }

        /// <summary>
        /// Gets a font suitable for the given UI element type
        /// </summary>
        /// <param name="elementType">Type of UI element</param>
        /// <returns>Font instance appropriate for the element type</returns>
        public Font GetFontForElement(UIElementType elementType)
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
        public List<FontConfiguration> LoadFontsFromFolder(string folderPath)
        {
            return FontListHelper.GetFontFilesLocations(folderPath);
        }

        /// <summary>
        /// Sets default font preferences based on a theme or user settings
        /// </summary>
        /// <param name="fontName">Font name to use</param>
        /// <param name="fontSize">Font size to use</param>
        public void SetDefaultFont(string fontName, float fontSize)
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
        public void ScaleFonts(float scaleFactor)
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
        public bool IsFontAvailable(string fontName)
        {
            return FontListHelper.GetFontIndex(fontName) != -1;
        }
        #endregion

        #region "Cleanup"
        /// <summary>
        /// Disposes a font reference
        /// </summary>
        private void DisposeFont(ref Font font)
        {
            // Fonts from FontListHelper are cached, so we don't dispose them
            // Just set the reference to null
            font = null;
        }

        /// <summary>
        /// Disposes all managed resources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // We don't dispose fonts from FontListHelper as they are cached
                    // Just set our references to null
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
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Implements IDisposable Pattern
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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
