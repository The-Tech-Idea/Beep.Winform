using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using TheTechIdea.Beep.Winform.Controls.Managers;
using TheTechIdea.Beep.Winform.Controls.Models;

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
        // Method to add all theme classes from the ThemeTypes folder in TheTechIdea.Beep.Vis.Modules2.0 project
        public static void AddPredefinedThemes()
        {
            try
            {
                // List of known theme types based on the EnumBeepThemes enum
                var themeTypeNames = new List<string>
        {
            "DefaultTheme",
            "WinterTheme",
            "CandyTheme",
            "ZenTheme",
            "RetroTheme",
            "RoyalTheme",
            "HighlightTheme",
            "DarkTheme",
            "OceanTheme",
            "LightTheme",
            "PastelTheme",
            "MidnightTheme",
            "SpringTheme",
            "ForestTheme",
            "NeonTheme",
            "RusticTheme",
            "GalaxyTheme",
            "DesertTheme",
            "VintageTheme",
            "ModernDarkTheme",
            "MaterialDesignTheme",
            "NeumorphismTheme",
            "GlassmorphismTheme",
            "FlatDesignTheme",
            "CyberpunkNeonTheme",
            "GradientBurstTheme",
            "HighContrastTheme",
            "MonochromeTheme",
            "LuxuryGoldTheme",
            "SunsetTheme",
            "AutumnTheme",
            "EarthyTheme"
        };

                // Get the assembly that contains the theme types
                Assembly themeAssembly = null;
                try
                {
                    // Try direct load first
                    themeAssembly = Assembly.Load("TheTechIdea.Beep.Vis.Modules2.0");
                }
                catch
                {
                    // If direct load fails, try to find it among loaded assemblies
                    themeAssembly = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.FullName.Contains("TheTechIdea.Beep.Vis.Modules2.0") ||
                                            a.FullName.Contains("TheTechIdea.Beep.Vis.Modules"));
                }

                if (themeAssembly == null)
                {
                    // If still not found, try one more approach with GetExecutingAssembly
                    var executingAssembly = Assembly.GetExecutingAssembly();
                    var referencedAssemblies = executingAssembly.GetReferencedAssemblies();
                    var modulesAssemblyName = referencedAssemblies
                        .FirstOrDefault(a => a.Name.Contains("TheTechIdea.Beep.Vis.Modules"));

                    if (modulesAssemblyName != null)
                    {
                        themeAssembly = Assembly.Load(modulesAssemblyName);
                    }
                }

                if (themeAssembly == null)
                {
                    // Unable to find the assembly containing themes
                    return;
                }

                // Try to use reflection to find all theme types in the assembly
                var allPossibleThemeTypes = themeAssembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BeepTheme)))
                    .ToList();

                // Add any discovered theme types to our list if they aren't already there
                foreach (var type in allPossibleThemeTypes)
                {
                    if (!themeTypeNames.Contains(type.Name))
                    {
                        themeTypeNames.Add(type.Name);
                    }
                }

                // Go through each theme type name and try to create an instance
                foreach (var themeTypeName in themeTypeNames)
                {
                    try
                    {
                        // Try several possible namespace patterns
                        Type themeType = null;
                        string[] possibleNamespaces = new[]
                        {
                    $"TheTechIdea.Beep.Vis.Modules.{themeTypeName}",
                    $"TheTechIdea.Beep.Vis.Modules",
                    $"TheTechIdea.Beep.Vis.Modules.ThemeTypes.{themeTypeName}",
                    $"TheTechIdea.Beep.Vis.Modules.ThemeTypes.{themeTypeName}.{themeTypeName}",
                    $"TheTechIdea.Beep.Vis.Modules2.0.ThemeTypes.{themeTypeName}"
                };

                        // Try to find the type in each possible namespace
                        foreach (var ns in possibleNamespaces)
                        {
                            themeType = themeAssembly.GetType($"{ns}.{themeTypeName}");
                            if (themeType != null)
                                break;

                            // Special case for namespace matching the class name directly
                            if (ns.EndsWith(themeTypeName))
                            {
                                themeType = themeAssembly.GetType(ns);
                                if (themeType != null)
                                    break;
                            }
                        }

                        // Try again with just the type name (some themes might be in the root namespace)
                        if (themeType == null)
                        {
                            themeType = themeAssembly.GetType(themeTypeName);
                        }

                        // As a final attempt, search by name in all types
                        if (themeType == null)
                        {
                            themeType = allPossibleThemeTypes.FirstOrDefault(t => t.Name == themeTypeName);
                        }

                        // If we found the type, create an instance and add it to our themes
                        if (themeType != null && themeType.IsSubclassOf(typeof(BeepTheme)))
                        {
                            var theme = (BeepTheme)Activator.CreateInstance(themeType);

                            // Ensure theme has a name
                            if (string.IsNullOrEmpty(theme.ThemeName))
                            {
                                theme.ThemeName = themeTypeName;
                            }

                            AddTheme(theme);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception for this specific theme type
                        // Continue trying to load other themes
                        System.Diagnostics.Debug.WriteLine($"Error loading theme {themeTypeName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the general exception
                System.Diagnostics.Debug.WriteLine($"Error loading predefined themes: {ex.Message}");
            }
        }

        // Helper method to add the AddPredefinedThemes call to the InitializeThemes method
        private static void InitializeThemesWithPredefined()
        {
            // Call the original initialization
            InitializeThemes();

            // Add predefined themes from the TheTechIdea.Beep.Vis.Modules2.0 project
            AddPredefinedThemes();
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

   
}