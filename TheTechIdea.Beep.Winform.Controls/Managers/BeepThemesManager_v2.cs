using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Vis.Modules
{
    public static class BeepThemesManager_v2
    {
        public static readonly Guid DefaultThemeGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
        private static string _currentThemeName = "DefaultTheme";
        private static event EventHandler<ThemeChangeEventArgs> _themeChangedEvent;

        // Static themes collection
        private static readonly List<BeepTheme> _themes = new List<BeepTheme>();

        // Static constructor to initialize themes
        static BeepThemesManager_v2()
        {
            InitializeThemes();
        }

        // Method to initialize themes collection
        private static void InitializeThemes()
        {
            // Clear existing themes if any
            _themes.Clear();

            // Dynamically load all BeepTheme subclasses
            var themeTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BeepTheme)) && !t.IsAbstract);

            foreach (var type in themeTypes)
            {
                if (Activator.CreateInstance(type) is BeepTheme theme)
                {
                    // Use the theme's class name if ThemeName is not set
                    if (string.IsNullOrEmpty(theme.ThemeName))
                    {
                        theme.ThemeName = type.Name;
                    }

                    // Generate a GUID if not set
                    if (string.IsNullOrEmpty(theme.ThemeGuid))
                    {
                        theme.ThemeGuid = Guid.NewGuid().ToString();
                    }

                    _themes.Add(theme);
                }
            }

            // Set default theme if available
            if (!_themes.Any(t => t.ThemeName == "DefaultTheme"))
            {
                // Create default theme if not found
                BeepTheme defaultTheme = new BeepTheme();
                defaultTheme.ThemeGuid = DefaultThemeGuid.ToString();
                defaultTheme.ThemeName = "DefaultTheme";
                _themes.Add(defaultTheme);
            }
        }

        // Method to add a theme
        public static void AddTheme(BeepTheme theme)
        {
            if (theme == null)
                return;

            // Make sure theme has a name
            if (string.IsNullOrEmpty(theme.ThemeName))
            {
                theme.ThemeName = theme.GetType().Name;
            }

            // Make sure theme has a GUID
            if (string.IsNullOrEmpty(theme.ThemeGuid))
            {
                theme.ThemeGuid = Guid.NewGuid().ToString();
            }

            // Check if it already exists
            if (!_themes.Any(t => t.ThemeGuid == theme.ThemeGuid))
            {
                _themes.Add(theme);
            }
        }

        // Method to remove a theme
        public static void RemoveTheme(BeepTheme theme)
        {
            if (theme != null)
            {
                _themes.RemoveAll(t => t.ThemeGuid == theme.ThemeGuid);
            }
        }

        // Event for theme changes
        public static event EventHandler<ThemeChangeEventArgs> ThemeChanged
        {
            add { _themeChangedEvent += value; }
            remove { _themeChangedEvent -= value; }
        }

        // Current theme property
        public static string CurrentThemeName
        {
            get { return _currentThemeName; }
            set
            {
                string oldThemeName = _currentThemeName;
                _currentThemeName = value;

                // Notify subscribers about theme change
                _themeChangedEvent?.Invoke(null, new ThemeChangeEventArgs
                {
                    OldThemeName = oldThemeName,
                    NewThemeName = _currentThemeName,
                    OldTheme = GetTheme(oldThemeName),
                    NewTheme = GetTheme(_currentThemeName)
                });
            }
        }

        // Get current theme object
        public static BeepTheme CurrentTheme => GetTheme(_currentThemeName);

        // Get theme by name
        public static BeepTheme GetTheme(string themeName)
        {
            return _themes.FirstOrDefault(t => t.ThemeName == themeName) ?? GetDefaultTheme();
        }

        // Get the default theme
        public static BeepTheme GetDefaultTheme()
        {
            return _themes.FirstOrDefault(t => t.ThemeName == "DefaultTheme") ?? _themes.FirstOrDefault();
        }

        // Get theme name
        public static string GetThemeName(BeepTheme theme)
        {
            return theme?.ThemeName ?? "DefaultTheme";
        }

        // Get list of theme names
        public static List<string> GetThemeNames()
        {
            return _themes.Select(t => t.ThemeName).ToList();
        }

        // Get all themes
        public static List<BeepTheme> GetThemes()
        {
            return _themes.ToList(); // Return a copy to prevent external modification
        }

        // Helper for font creation - uses BeepFontManager
        public static Font ToFont(TypographyStyle style)
        {
            return BeepFontManager.ToFont(style);
        }

        // Create font from parameters - uses BeepFontManager
        public static Font ToFont(string fontFamily, float fontSize, FontWeight fontWeight, FontStyle fontStyle)
        {
            // Check if font is available via BeepFontManager
            if (!BeepFontManager.IsFontAvailable(fontFamily))
                fontFamily = "Arial";

            return BeepFontManager.GetFont(fontFamily, fontSize, fontStyle);
        }

        // Save theme to file
        public static void SaveTheme(BeepTheme theme, string filePath)
        {
            var serializer = new XmlSerializer(typeof(BeepTheme));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, theme);
            }
        }

        // Load theme from file
        public static BeepTheme LoadTheme(string filePath)
        {
            var serializer = new XmlSerializer(typeof(BeepTheme));
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var theme = (BeepTheme)serializer.Deserialize(stream);

                // Add the loaded theme to our collection if it's not already there
                if (!ThemeExists(theme.ThemeName))
                {
                    AddTheme(theme);
                }

                return theme;
            }
        }

        // Set current theme by name
        public static void SetCurrentTheme(string themeName)
        {
            // Only set if we can find the theme
            if (ThemeExists(themeName))
            {
                CurrentThemeName = themeName;
            }
        }

        // Set current theme by object
        public static void SetCurrentTheme(BeepTheme theme)
        {
            if (theme != null)
            {
                // Make sure theme is in our collection
                if (!ThemeExists(theme.ThemeName))
                {
                    AddTheme(theme);
                }

                CurrentThemeName = theme.ThemeName;
            }
        }

        // Check if theme exists by name
        public static bool ThemeExists(string themeName)
        {
            return _themes.Any(t => t.ThemeName == themeName);
        }

        // Get GUID from theme
        public static string GetGuidFromTheme(BeepTheme theme)
        {
            return theme?.ThemeGuid ?? DefaultThemeGuid.ToString();
        }

        // Reset theme collection (useful for testing or reloading)
        public static void ResetThemes()
        {
            InitializeThemes();
        }

        // Configure basic properties for a new theme based on its name
        public static void ConfigureDefaultTheme(BeepTheme theme)
        {
            // Basic configuration based on theme name
            if (theme.ThemeName.Contains("Dark", StringComparison.OrdinalIgnoreCase))
            {
                theme.IsDarkTheme = true;
                theme.BackColor = Color.FromArgb(30, 30, 30);
                theme.ForeColor = Color.White;
            }
            else
            {
                theme.IsDarkTheme = false;
                theme.BackColor = Color.White;
                theme.ForeColor = Color.Black;
            }

            // Set sensible defaults for other properties
            theme.BorderRadius = 4;
            theme.PaddingSmall = 4;
            theme.PaddingMedium = 8;
            theme.PaddingLarge = 16;
        }

        #region Legacy Support Adapters

        // This inner class provides adapters for code that still uses EnumBeepThemes
        public static class Legacy
        {
            // For backward compatibility with existing code that uses EnumBeepThemes
            public static BeepTheme GetTheme(EnumBeepThemes theme)
            {
                string themeName = Enum.GetName(typeof(EnumBeepThemes), theme) ?? "DefaultTheme";
                return BeepThemesManager_v2.GetTheme(themeName);
            }

            public static string GetThemeName(EnumBeepThemes theme)
            {
                return Enum.GetName(typeof(EnumBeepThemes), theme) ?? "DefaultTheme";
            }

            public static EnumBeepThemes GetThemeToEnum(BeepTheme theme)
            {
                if (theme == null) return EnumBeepThemes.DefaultTheme;

                if (Enum.TryParse(theme.ThemeName, out EnumBeepThemes result))
                    return result;

                return EnumBeepThemes.DefaultTheme;
            }

            public static EnumBeepThemes GetEnumFromTheme(string themeName)
            {
                if (string.IsNullOrEmpty(themeName)) return EnumBeepThemes.DefaultTheme;

                if (Enum.TryParse(themeName, out EnumBeepThemes result))
                    return result;

                return EnumBeepThemes.DefaultTheme;
            }

            // Enum-based current theme property for legacy code
            public static EnumBeepThemes CurrentTheme
            {
                get => GetEnumFromTheme(BeepThemesManager_v2._currentThemeName);
                set => BeepThemesManager_v2.CurrentThemeName = GetThemeName(value);
            }

            public static EnumBeepThemes GetCurrentTheme()
            {
                return GetEnumFromTheme(BeepThemesManager_v2._currentThemeName);
            }

            public static void SetCurrentTheme(EnumBeepThemes theme)
            {
                BeepThemesManager_v2.CurrentThemeName = GetThemeName(theme);
            }
        }

        #endregion
    }

    // New event args class that doesn't depend on EnumBeepThemes
    public class ThemeChangeEventArgs : EventArgs
    {
        public string OldThemeName { get; set; }
        public string NewThemeName { get; set; }
        public BeepTheme OldTheme { get; set; }
        public BeepTheme NewTheme { get; set; }
    }

    // Adapter for the old event args for backward compatibility
    public class ThemeChangeEventsArgs : EventArgs
    {
        public EnumBeepThemes OldTheme { get; set; }
        public EnumBeepThemes NewTheme { get; set; }

        // Conversion constructor for easier interop
        public ThemeChangeEventsArgs(ThemeChangeEventArgs args)
        {
            OldTheme = BeepThemesManager_v2.Legacy.GetEnumFromTheme(args.OldThemeName);
            NewTheme = BeepThemesManager_v2.Legacy.GetEnumFromTheme(args.NewThemeName);
        }

        // Default constructor for direct use
        public ThemeChangeEventsArgs() { }
    }
}