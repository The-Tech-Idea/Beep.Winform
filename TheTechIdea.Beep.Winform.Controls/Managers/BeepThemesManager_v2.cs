using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private static EnumBeepThemes _currentTheme = EnumBeepThemes.DefaultTheme;
        private static event EventHandler<ThemeChangeEventsArgs> _themeChangedEvent;

        // Static themes collection instead of instance property
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
                    _themes.Add(theme);
            }

            // Set default theme if available
            if (!_themes.Any(t => t.ThemeGuid == DefaultThemeGuid.ToString()))
            {
                // Create default theme if not found
                BeepTheme defaultTheme = new BeepTheme();
                defaultTheme.ThemeGuid = DefaultThemeGuid.ToString();
                _themes.Add(defaultTheme);
            }
        }

        // Method to add a theme
        public static void AddTheme(BeepTheme theme)
        {
            if (theme != null && !_themes.Any(t => t.ThemeGuid == theme.ThemeGuid))
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
        public static event EventHandler<ThemeChangeEventsArgs> ThemeChanged
        {
            add { _themeChangedEvent += value; }
            remove { _themeChangedEvent -= value; }
        }

        // Current theme property
        public static EnumBeepThemes CurrentTheme
        {
            get { return _currentTheme; }
            set
            {
                var oldTheme = _currentTheme;
                _currentTheme = value;

                // Notify subscribers about theme change
                _themeChangedEvent?.Invoke(null, new ThemeChangeEventsArgs
                {
                    OldTheme = oldTheme,
                    NewTheme = _currentTheme
                });
            }
        }

        // Get theme from enum
        public static BeepTheme GetTheme(EnumBeepThemes theme)
        {
            var themeName = Enum.GetName(typeof(EnumBeepThemes), theme);
            return _themes.FirstOrDefault(t => t.ThemeName == themeName) ??
                   _themes.FirstOrDefault(t => t.ThemeGuid == DefaultThemeGuid.ToString());
        }

        // Get theme by name
        public static BeepTheme GetTheme(string themeName)
        {
            return _themes.FirstOrDefault(t => t.ThemeName == themeName) ??
                   _themes.FirstOrDefault(t => t.ThemeGuid == DefaultThemeGuid.ToString());
        }

        // Get enum from theme object
        public static EnumBeepThemes GetThemeToEnum(BeepTheme theme)
        {
            if (theme == null) return EnumBeepThemes.DefaultTheme;

            if (Enum.TryParse(theme.ThemeName, out EnumBeepThemes result))
                return result;

            return EnumBeepThemes.DefaultTheme;
        }

        // Get enum from theme name
        public static EnumBeepThemes GetEnumFromTheme(string themeName)
        {
            if (string.IsNullOrEmpty(themeName)) return EnumBeepThemes.DefaultTheme;

            if (Enum.TryParse(themeName, out EnumBeepThemes result))
                return result;

            return EnumBeepThemes.DefaultTheme;
        }

        // Get theme name
        public static string GetTheme(BeepTheme theme)
        {
            return theme?.ThemeName ?? "DefaultTheme";
        }

        // Get list of theme names
        public static List<string> GetThemesNames()
        {
            return _themes.Select(t => t.ThemeName).ToList();
        }

        // Get all themes
        public static List<BeepTheme> GetThemes()
        {
            return _themes.ToList(); // Return a copy to prevent external modification
        }

        // Get theme name from enum
        public static string GetThemeName(EnumBeepThemes theme)
        {
            return Enum.GetName(typeof(EnumBeepThemes), theme) ?? "DefaultTheme";
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
                return (BeepTheme)serializer.Deserialize(stream);
            }
        }

        // Get current theme
        public static EnumBeepThemes GetCurrentTheme()
        {
            return CurrentTheme;
        }

        // Set current theme
        public static void SetCurrentTheme(EnumBeepThemes theme)
        {
            CurrentTheme = theme;
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
    }

    // This class should be kept the same as in BeepThemesManager
   
}