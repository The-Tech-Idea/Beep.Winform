using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    /// <summary>
    /// Static manager class for application fonts and typography.
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

        // Font instances for various UI elements. These are shared cached fonts from FontListHelper.
        // Do not dispose them here; FontListHelper owns the lifecycle of its cached fonts.
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
        private static bool _registryEventSubscribed = false;
        private static readonly SemaphoreSlim _initializeLock = new SemaphoreSlim(1, 1);

        // Pixel-unit fonts are created here for custom painters, so this manager owns and disposes them.
        private static readonly object _pixelFontCacheLock = new object();
        private static readonly Dictionary<string, Font> _pixelFontCache =
            new Dictionary<string, Font>(StringComparer.OrdinalIgnoreCase);

        #region "Properties"
        public static string DefaultFontName
        {
            get => _defaultFontName;
            set { if (_defaultFontName != value) { _defaultFontName = value; ResetFonts(); } }
        }
        public static float DefaultFontSize
        {
            get => _defaultFontSize;
            set { if (Math.Abs(_defaultFontSize - value) > 0.001f) { _defaultFontSize = value; ResetFonts(); } }
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
            set { if (Math.Abs(_appFontSize - value) > 0.001f) { _appFontSize = value; ResetFonts(); } }
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
        public static Font TooltipFont => _tooltipFont ??= GetOrCreateFont(_appFontName, Math.Max(_appFontSize - 1, 6f), _appFontStyle);
        public static Font StatusBarFont => _statusBarFont ??= GetOrCreateFont(_appFontName, Math.Max(_appFontSize - 1, 6f), _appFontStyle);
        public static Font DialogFont => _dialogFont ??= GetOrCreateFont(_appFontName, _appFontSize, _appFontStyle);
        public static Font MonospaceFont => _monospaceFont ??= GetOrCreateFont("Consolas", _appFontSize, _appFontStyle);

        public static List<FontConfiguration> FontConfigurations => FontListHelper.FontConfigurations;

        // NOTE: Renamed to avoid conflict with System.Drawing.SystemFonts class
        public static List<string> SystemFontNames => FontListHelper.GetSystemFontNames();
        public static List<string> CustomFontNames => FontListHelper.GetPrivateFontNames();
        public static List<string> AllFontNames => FontListHelper.GetFontNames();
        public static List<string> EmbeddedFontFamilies => BeepFontPaths.GetFontFamilyNames();
        #endregion

        #region "Initialization"
        public static async Task Initialize()
        {
            if (_isInitialized)
                return;

            await _initializeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_isInitialized)
                    return;

                // Merge registry-registered namespaces and directories into scan options.
                var embNamespaces = new List<string>
                {
                    "TheTechIdea.Beep.Winform.Controls.Fonts",
                    "TheTechIdea.Beep.Fonts"
                };
                embNamespaces.AddRange(BeepFontRegistry.GetRegisteredNamespaces());

                var allDirs = new List<string>
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts")
                };
                allDirs.AddRange(BeepFontRegistry.GetRegisteredDirectories());

                var opt = new FontListHelper.FontScanOptions
                {
                    ScanSystemFonts = true,
                    Directories = allDirs,
                    EmbeddedNamespaces = embNamespaces.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                    AdditionalAssemblies = BeepFontRegistry.GetRegisteredAssemblies().ToList(),
                    IncludeFrameworkAssemblies = false,
                    IncludeReferencedAssemblies = true,
                    MaxReferenceDepth = 2,
                    UseConventionDiscovery = true
                };

                await Task.Run(() => FontListHelper.EnsureFontsLoaded(opt, false)).ConfigureAwait(false);

                // Create default fonts.
                CreateDefaultFonts();

                if (!_registryEventSubscribed)
                {
                    BeepFontRegistry.Changed += OnRegistryChanged;
                    _registryEventSubscribed = true;
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepFontManager.Initialize failed: {ex.Message}");
                // Consumers still fall back to generic fonts through FontListHelper.
            }
            finally
            {
                _initializeLock.Release();
            }
        }

        private static void OnRegistryChanged(object sender, EventArgs e)
        {
            if (!_isInitialized)
                return;

            try
            {
                // Incrementally load fonts from newly registered assemblies.
                foreach (var asm in BeepFontRegistry.GetRegisteredAssemblies())
                {
                    FontListHelper.RegisterAndLoad(asm);
                    BeepFontPaths.RegisterFamilyFromAssembly(asm);
                }

                // Incrementally scan registered font directories too.
                foreach (var dir in BeepFontRegistry.GetRegisteredDirectories())
                {
                    try { FontListHelper.GetFontFilesLocations(dir); } catch { }
                }

                ResetFonts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepFontManager registry update failed: {ex.Message}");
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
            _tooltipFont = GetOrCreateFont(_appFontName, Math.Max(_appFontSize - 1, 6f), _appFontStyle);
            _statusBarFont = GetOrCreateFont(_appFontName, Math.Max(_appFontSize - 1, 6f), _appFontStyle);
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

            ClearPixelFontCache();
            CreateDefaultFonts();
        }

        private static Font GetOrCreateFont(string fontName, float size, FontStyle style)
        {
            var font = FontListHelper.GetFont(fontName, size, style);
            if (font != null) return font;

            // Resilient fallbacks. These are only used if FontListHelper itself failed unexpectedly.
            try { return new Font(FontFamily.GenericSansSerif, size, style); }
            catch { }
            try { return new Font("Arial", size, style); }
            catch { return System.Drawing.SystemFonts.DefaultFont; }
        }
        #endregion

        #region "Font Management"

        #region "Assembly Registration"
        /// <summary>
        /// Registers an external assembly so its embedded fonts are discovered by all Beep
        /// font look-up methods. If <see cref="Initialize"/> has already run an incremental scan
        /// is triggered immediately; otherwise the assembly is included on the next call.
        /// </summary>
        /// <example>
        /// // In your app startup, before showing any Beep controls:
        /// BeepFontManager.Register(Assembly.GetExecutingAssembly());
        /// BeepFontManager.Register("MyCompany.MyProject.Fonts");
        /// </example>
        public static void Register(Assembly assembly)
        {
            if (assembly == null) return;
            BeepFontRegistry.Register(assembly);
            if (_isInitialized)
            {
                FontListHelper.RegisterAndLoad(assembly);
                BeepFontPaths.RegisterFamilyFromAssembly(assembly);
                ResetFonts();
            }
        }

        /// <summary>Registers multiple assemblies at once. Null entries are skipped.</summary>
        public static void Register(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) return;
            foreach (var asm in assemblies) Register(asm);
        }

        /// <summary>
        /// Registers a namespace prefix so that any embedded resource containing this string is
        /// treated as a font resource during scanning.
        /// Example: <c>"MyCompany.MyTheme.Fonts"</c>
        /// </summary>
        public static void Register(string namespacePrefix)
        {
            if (string.IsNullOrWhiteSpace(namespacePrefix)) return;
            BeepFontRegistry.Register(namespacePrefix);
        }

        /// <summary>
        /// Registers a file-system directory for .ttf/.otf/.ttc scanning.
        /// If already initialized, scanning runs immediately.
        /// </summary>
        public static void RegisterDirectory(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return;
            BeepFontRegistry.RegisterFromDirectory(directoryPath);
            if (_isInitialized)
            {
                try
                {
                    FontListHelper.GetFontFilesLocations(directoryPath);
                    ResetFonts();
                }
                catch { }
            }
        }

        /// <summary>
        /// Walks the current AppDomain for assemblies matching the convention: at least one
        /// embedded resource whose namespace prefix ends with <c>.Fonts</c>.
        /// Those assemblies are registered and, if already initialized, loaded immediately.
        /// </summary>
        public static void RegisterFromAppDomain()
        {
            BeepFontRegistry.RegisterFromAppDomain();
            if (_isInitialized)
            {
                foreach (var asm in BeepFontRegistry.GetRegisteredAssemblies())
                {
                    FontListHelper.RegisterAndLoad(asm);
                    BeepFontPaths.RegisterFamilyFromAssembly(asm);
                }
                ResetFonts();
            }
        }
        #endregion

        /// <summary>
        /// Gets a font using GraphicsUnit.Point (default for standard WinForms controls).
        /// Point fonts are DPI-independent and normally scale with WinForms.
        /// </summary>
        public static Font GetFont(string fontName, float sizeInPoints, FontStyle style = FontStyle.Regular)
        {
            return FontListHelper.GetFont(fontName, sizeInPoints, style);
        }

        /// <summary>
        /// Gets a DPI-aware font for a control using GraphicsUnit.Point.
        /// For custom-painted controls, use GetFontForPainter() instead.
        /// </summary>
        public static Font GetFontForControl(string fontName, float sizeInPoints, Control control, FontStyle style = FontStyle.Regular)
        {
            if (control == null || !control.IsHandleCreated)
                return GetFont(fontName, sizeInPoints, style);

            // Point fonts are the normal unit for WinForms controls. Avoid manual scaling here.
            return GetFont(fontName, sizeInPoints, style);
        }

        /// <summary>
        /// Gets a DPI-scaled font using GraphicsUnit.Pixel for custom painters.
        /// Use this in OnPaint methods when you need a pixel-unit font for Graphics.DrawString.
        /// Returned fonts are cached and owned by BeepFontManager; callers must not dispose them.
        /// </summary>
        public static Font GetFontForPainter(string fontName, float sizeInPoints, Control control, FontStyle style = FontStyle.Regular)
        {
            if (control == null || !control.IsHandleCreated)
                return GetFont(fontName, sizeInPoints, style);

            string resolvedName = string.IsNullOrWhiteSpace(fontName) ? _appFontName : fontName.Trim();

            // pixels = points × (DPI ÷ 72)
            float pixelSize = Math.Max(sizeInPoints * (control.DeviceDpi / 72.0f), 6.0f);
            string key = BuildPixelFontKey(resolvedName, pixelSize, style);

            lock (_pixelFontCacheLock)
            {
                if (_pixelFontCache.TryGetValue(key, out var cached))
                {
                    try
                    {
                        var _ = cached.Height;
                        return cached;
                    }
                    catch
                    {
                        _pixelFontCache.Remove(key);
                    }
                }

                try
                {
                    var font = new Font(resolvedName, pixelSize, style, GraphicsUnit.Pixel);
                    var _ = font.Height;
                    _pixelFontCache[key] = font;
                    return font;
                }
                catch
                {
                    // Fallback to the shared point-based font cache.
                    return GetFont(resolvedName, sizeInPoints, style);
                }
            }
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
            if (string.IsNullOrWhiteSpace(fontName)) return false;
            return FontListHelper.GetFontIndex(fontName) != -1 ||
                   EmbeddedFontFamilies.Contains(fontName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Safely scales a control's font during DPI changes.
        /// Only scales explicitly-set fonts, not inherited fonts, to reduce double-scaling risk.
        /// </summary>
        /// <param name="control">Control to scale</param>
        /// <param name="oldDpi">Previous DPI (e.g., 96 for 100%)</param>
        /// <param name="newDpi">New DPI (e.g., 144 for 150%)</param>
        /// <returns>True if font was scaled; false if skipped</returns>
        public static bool TryScaleControlFont(Control control, int oldDpi, int newDpi)
        {
            if (control == null || control.IsDisposed || !control.IsHandleCreated)
                return false;

            if (oldDpi <= 0 || newDpi <= 0 || oldDpi == newDpi)
                return false;

            if (DpiScalingHelper.IsFontInherited(control, control.Font))
                return false;

            float oldScale = oldDpi / 96.0f;
            float newScale = newDpi / 96.0f;
            Font currentFont = control.Font;

            try
            {
                float newSize = currentFont.Unit == GraphicsUnit.Pixel
                    ? DpiScalingHelper.PixelsToPoints(currentFont.Size, oldScale) * newScale
                    : currentFont.SizeInPoints;

                Font newFont = new Font(currentFont.FontFamily, newSize, currentFont.Style,
                    currentFont.Unit, currentFont.GdiCharSet, currentFont.GdiVerticalFont);

                Font oldFont = control.Font;
                control.Font = newFont;

                if (!ReferenceEquals(oldFont, Control.DefaultFont) &&
                    !ReferenceEquals(oldFont, System.Drawing.SystemFonts.DefaultFont))
                {
                    try { oldFont.Dispose(); } catch { }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Font ToFont(TypographyStyle style)
        {
            if (style == null) return DefaultFont;
            return FontListHelper.CreateFontFromTypography(style) ?? DefaultFont;
        }

        /// <summary>
        /// Gets a cached font to avoid per-paint allocations in painters.
        /// Use this in OnPaint methods instead of 'new Font(...)'.
        /// Returned fonts are owned by FontListHelper; callers must not dispose them.
        /// </summary>
        public static Font GetCachedFont(string fontName, float size, FontStyle style = FontStyle.Regular)
        {
            return FontListHelper.GetCachedFont(fontName, size, style);
        }

        /// <summary>
        /// Clears the font cache (call on theme change or cleanup).
        /// </summary>
        public static void ClearFontCache()
        {
            ClearPixelFontCache();
            FontListHelper.ClearFontCache();
            ResetFontReferencesOnly();
        }

        public static float GetFontWeightMultiplier(FontWeight weight)
        {
            return (float)weight / 1000f;
        }

        /// <summary>
        /// Gets a font from embedded resources using the BeepFontPaths system.
        /// </summary>
        /// <param name="familyName">Font family name (e.g., "Roboto", "Cairo", "SourceSansPro")</param>
        /// <param name="size">Font size in points</param>
        /// <param name="style">Font Style (Regular, Bold, Italic, etc.)</param>
        /// <returns>Font object or null if not found</returns>
        public static Font GetEmbeddedFont(string familyName, float size, FontStyle style = FontStyle.Regular)
        {
            if (string.IsNullOrWhiteSpace(familyName))
                return null;

            try
            {
                string styleString = MapFontStyleToString(style);
                string fontPath = BeepFontPaths.GetFontPath(familyName, styleString);

                if (string.IsNullOrEmpty(fontPath))
                    fontPath = BeepFontPaths.GetFontPath(familyName, "Regular");

                if (!string.IsNullOrEmpty(fontPath))
                {
                    var font = GetEmbeddedFontFromResourcePath(fontPath, size, style);
                    if (font != null)
                        return font;
                }

                return GetFont(familyName, size, style);
            }
            catch
            {
                return GetFont(familyName, size, style);
            }
        }

        /// <summary>
        /// Gets an embedded font scaled for a control's current DPI.
        /// </summary>
        public static Font GetEmbeddedFont(string familyName, float size, Control control, FontStyle style = FontStyle.Regular)
        {
            float dpiScale = control != null ? DpiScalingHelper.GetDpiScaleFactor(control) : 1.0f;
            float scaledSize = Math.Max(size * dpiScale, 6.0f);
            return GetEmbeddedFont(familyName, scaledSize, style);
        }

        /// <summary>
        /// Gets a font from embedded resources using the BeepFontPaths system with a specific font path.
        /// </summary>
        /// <param name="fontResourcePath">The full resource path from BeepFontPaths (e.g., BeepFontPaths.RobotoRegular)</param>
        /// <param name="size">Font size in points</param>
        /// <returns>Font object or null if not found</returns>
        public static Font GetEmbeddedFont(string fontResourcePath, float size)
        {
            return GetEmbeddedFontFromResourcePath(fontResourcePath, size, FontStyle.Regular);
        }

        /// <summary>
        /// Gets an embedded font by resource path scaled for a control's current DPI.
        /// </summary>
        public static Font GetEmbeddedFont(string fontResourcePath, float size, Control control)
        {
            float dpiScale = control != null ? DpiScalingHelper.GetDpiScaleFactor(control) : 1.0f;
            float scaledSize = Math.Max(size * dpiScale, 6.0f);
            return GetEmbeddedFont(fontResourcePath, scaledSize);
        }

        /// <summary>
        /// Maps System.Drawing.FontStyle to BeepFontPaths Style string.
        /// </summary>
        private static string MapFontStyleToString(FontStyle style)
        {
            return style switch
            {
                FontStyle.Bold => "Bold",
                FontStyle.Italic => "Italic",
                FontStyle.Bold | FontStyle.Italic => "BoldItalic",
                _ when style.HasFlag(FontStyle.Bold) && style.HasFlag(FontStyle.Italic) => "BoldItalic",
                _ when style.HasFlag(FontStyle.Bold) => "Bold",
                _ when style.HasFlag(FontStyle.Italic) => "Italic",
                _ => "Regular"
            };
        }

        /// <summary>
        /// Checks if an embedded font is available.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <returns>True if the embedded font exists</returns>
        public static bool IsEmbeddedFontAvailable(string familyName)
        {
            if (string.IsNullOrWhiteSpace(familyName))
                return false;

            return BeepFontPaths.GetFontFamilyNames()
                .Contains(familyName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets all available embedded font family names.
        /// </summary>
        /// <returns>List of embedded font family names</returns>
        public static List<string> GetEmbeddedFontFamilies()
        {
            return BeepFontPaths.GetFontFamilyNames();
        }

        /// <summary>
        /// Gets all available styles for a specific embedded font family.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <returns>List of available styles</returns>
        public static List<string> GetEmbeddedFontStyles(string familyName)
        {
            return BeepFontPaths.GetFontFamilyStyles(familyName);
        }
        #endregion

        #region "Cleanup"
        public static void Cleanup()
        {
            if (_registryEventSubscribed)
            {
                BeepFontRegistry.Changed -= OnRegistryChanged;
                _registryEventSubscribed = false;
            }

            ResetFontReferencesOnly();
            ClearPixelFontCache();
            _isInitialized = false;
        }

        public static FontFamily? GetFontFamily(BeepControlStyle controlStyle)
        {
            try
            {
                IBeepTheme theme = BeepThemesManager.CurrentTheme;
                if (theme == null || string.IsNullOrEmpty(theme.FontFamily))
                    return null;
                return FontListHelper.GetFontFamily(theme.FontFamily);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Private Helpers

        private static Font GetEmbeddedFontFromResourcePath(string fontResourcePath, float size, FontStyle style)
        {
            if (string.IsNullOrWhiteSpace(fontResourcePath))
                return null;

            string familyName = TryExtractFamilyNameFromResourcePath(fontResourcePath);
            if (!string.IsNullOrWhiteSpace(familyName))
            {
                var font = FontListHelper.GetFont(familyName, size, style);
                if (font != null)
                    return font;
            }

            // Last resort for older BeepFontPaths implementations. This may allocate, but only if the
            // font has not already been loaded into FontListHelper's private collection.
            try
            {
                return BeepFontPathsExtensions.CreateFontFromResource(fontResourcePath, size, style) ?? DefaultFont;
            }
            catch
            {
                return DefaultFont;
            }
        }

        private static string TryExtractFamilyNameFromResourcePath(string fontResourcePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fontResourcePath))
                    return null;

                // Prefer the official extractor if available in this project.
                string name = BeepFontPaths.ExtractFontNameFromPath(fontResourcePath);
                return string.IsNullOrWhiteSpace(name) ? null : name;
            }
            catch
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(fontResourcePath);
                    return string.IsNullOrWhiteSpace(fileName) ? null : fileName;
                }
                catch
                {
                    return null;
                }
            }
        }

        private static string BuildPixelFontKey(string fontName, float pixelSize, FontStyle style)
        {
            string resolvedName = string.IsNullOrWhiteSpace(fontName) ? _appFontName : fontName.Trim();
            return $"{resolvedName}|{Math.Round(pixelSize, 2)}|{(int)style}|Pixel";
        }

        private static void ResetFontReferencesOnly()
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
        }

        private static void ClearPixelFontCache()
        {
            lock (_pixelFontCacheLock)
            {
                foreach (var font in _pixelFontCache.Values.Distinct())
                {
                    try { font?.Dispose(); } catch { }
                }
                _pixelFontCache.Clear();
            }
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
