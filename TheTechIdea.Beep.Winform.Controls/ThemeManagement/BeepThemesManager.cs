using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;


namespace TheTechIdea.Beep.Winform.Controls.ThemeManagement
{
    public static class BeepThemesManager
    {
        public static readonly Guid DefaultThemeGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");
        private static string _currentThemeName = "DefaultTheme";
       
        public static event EventHandler<ThemeChangeEventArgs> ThemeChanged;
        public static event EventHandler <StyleChangeEventArgs> FormStyleChanged;
        // Static themes collection
        public static readonly List<IBeepTheme> _themes = new List<IBeepTheme>();

        // Static constructor to initialize themes
        static BeepThemesManager()
        {
            InitializeThemes();
        }

        // Method to initialize themes collection
        // Method to initialize themes collection
        public static void InitializeThemes()
        {
            // Clear existing themes if any
            _themes.Clear();

            // Get all relevant assemblies
            var assemblies = new List<Assembly>();

            // 1. Add the executing assembly
            assemblies.Add(Assembly.GetExecutingAssembly());

            // 2. Add the entry assembly if available
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null && !assemblies.Contains(entryAssembly))
                assemblies.Add(entryAssembly);

            // 3. Add currently loaded assemblies that might contain themes
            var relevantAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic &&
                           (a.FullName.Contains("TheTechIdea") ||
                            a.FullName.Contains("Beep")));

            foreach (var assembly in relevantAssemblies)
            {
                if (!assemblies.Contains(assembly))
                    assemblies.Add(assembly);
            }

            // 4. Try to load specific assemblies that might contain themes
            string[] potentialAssemblies = new[] {
        "TheTechIdea.Beep.Vis.Modules",
        "TheTechIdea.Beep.Vis.Modules2.0",
        "TheTechIdea.Beep.Winform.Controls"
    };

            foreach (var assemblyName in potentialAssemblies)
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (!assemblies.Contains(assembly))
                        assemblies.Add(assembly);
                }
                catch (Exception)
                {
                    // Assembly couldn't be loaded, continue with others
                    System.Diagnostics.Debug.WriteLine($"Could not load assembly: {assemblyName}");
                }
            }

            // Find and instantiate all theme types from all assemblies
            var themeInstances = new List<IBeepTheme>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    // Find all classes that implement IBeepTheme in this assembly
                    var themeTypes = assembly.GetTypes()
                        .Where(t => t != null &&
                                   t.IsClass &&
                                   !t.IsAbstract &&
                                   typeof(IBeepTheme).IsAssignableFrom(t));

                    foreach (var type in themeTypes)
                    {
                        try
                        {
                            if (Activator.CreateInstance(type) is IBeepTheme theme)
                            {
                               

                                // Generate a GUID if not set
                                if (string.IsNullOrEmpty(theme.ThemeGuid))
                                {
                                    theme.ThemeGuid = Guid.NewGuid().ToString();
                                }

                                // Add to our temporary list if not already there
                                if (!themeInstances.Any(t => t.ThemeName == theme.ThemeName))
                                {
                                    themeInstances.Add(theme);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with other types
                            System.Diagnostics.Debug.WriteLine($"Error creating instance of {type.Name}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue with other assemblies
                    System.Diagnostics.Debug.WriteLine($"Error processing assembly {assembly.FullName}: {ex.Message}");
                }
            }

            // Add all discovered themes to our collection
            foreach (var theme in themeInstances)
            {
                _themes.Add(theme);
            }

          

            // After loading standard themes, call AddPredefinedThemes for specialized themes
            AddPredefinedThemes();

            // Ensure fallback (DefaultBeepTheme) exists if no themes or no default themes found
            EnsureFallbackTheme();

            // Log the discovered themes
            System.Diagnostics.Debug.WriteLine($"Initialized {_themes.Count} themes:");
            foreach (var theme in _themes)
            {
                System.Diagnostics.Debug.WriteLine($"- {theme.ThemeName} ({theme.GetType().FullName})");
            }
        }

        // Add or create fallback DefaultBeepTheme if needed
        private static void EnsureFallbackTheme()
        {
            // If we already have either a "DefaultTheme" or "DefaultBeepTheme" we are fine
            if (_themes.Any(t => t.ThemeName == "DefaultTheme" || t.ThemeName == "DefaultBeepTheme"))
                return;

            try
            {
                // Try to find the DefaultBeepTheme type (internal in Winform.Controls assembly) via reflection
                var fallbackType = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                    })
                    .FirstOrDefault(t => t.FullName == "TheTechIdea.Beep.Winform.Controls.Themes.DefaultBeepTheme");

                if (fallbackType != null && typeof(IBeepTheme).IsAssignableFrom(fallbackType))
                {
                    if (Activator.CreateInstance(fallbackType, true) is IBeepTheme fallbackTheme)
                    {
                        if (string.IsNullOrEmpty(fallbackTheme.ThemeGuid))
                            fallbackTheme.ThemeGuid = Guid.NewGuid().ToString();
                        _themes.Add(fallbackTheme);
                        // Point current theme name to it if current referenced default does not exist
                        if (!_themes.Any(t => t.ThemeName == _currentThemeName))
                            _currentThemeName = fallbackTheme.ThemeName;
                        System.Diagnostics.Debug.WriteLine("Fallback DefaultBeepTheme added to themes list.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to instantiate DefaultBeepTheme via reflection: {ex.Message}");
            }
        }

        // Method to add a theme
        public static void AddTheme(IBeepTheme theme)
        {
            if (theme == null)
                return;


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
        public static void RemoveTheme(IBeepTheme theme)
        {
            if (theme != null)
            {
                _themes.RemoveAll(t => t.ThemeGuid == theme.ThemeGuid);
            }
        }

        private static FormStyle _currentstyle = FormStyle.Modern;
        public static FormStyle CurrentStyle
        {
            get { return _currentstyle; }
            set
            {
                FormStyle oldstyle = _currentstyle;
                _currentstyle = value;

                StyleChangeEventArgs x = new()
                {
                    OldStyle = oldstyle,
                    NewStyle = _currentstyle
                };
                FormStyleChanged?.Invoke(null, x);
            }
        }

        // Current theme property
        public static string CurrentThemeName
        {
            get { return _currentThemeName; }
            set
            {
                string oldThemeName = _currentThemeName;
                _currentThemeName = value;

                ThemeChangeEventArgs x = new()
                {
                    OldThemeName = oldThemeName,
                    NewThemeName = _currentThemeName,
                    OldTheme = GetTheme(oldThemeName),
                    NewTheme = GetTheme(_currentThemeName)
                };
                ThemeChanged?.Invoke(null, x);
                
             
            }
        }

        // Get current theme object
        public static IBeepTheme CurrentTheme => GetTheme(_currentThemeName);

        public static IBeepTheme DefaultTheme { get; internal set; }
      

        // Get theme by name
        public static IBeepTheme GetTheme(string themeName)
        {
            return _themes.FirstOrDefault(t => t.ThemeName == themeName) ?? GetDefaultTheme();
        }

        // Get the default theme
        public static IBeepTheme GetDefaultTheme()
        {
            // Prefer explicit "DefaultTheme" if present
            var def = _themes.FirstOrDefault(t => t.ThemeName == "DefaultTheme");
            if (def != null) return def;

            // Then look for the provided fallback class name
            var fallback = _themes.FirstOrDefault(t => t.ThemeName == "DefaultBeepTheme");
            if (fallback != null) return fallback;

            // Ensure we attempted to load fallback if not already
            EnsureFallbackTheme();

            // Try again after ensuring
            fallback = _themes.FirstOrDefault(t => t.ThemeName == "DefaultBeepTheme");
            if (fallback != null) return fallback;

            // Last resort: any available theme
            return _themes.FirstOrDefault();
        }

        // Get theme name
        public static string GetThemeName(IBeepTheme theme)
        {
            return theme?.ThemeName ?? "DefaultBeepTheme";
        }

        // Get list of theme names
        public static List<string> GetThemeNames()
        {
            return _themes.Select(t => t.ThemeName).ToList();
        }

        // Get all themes
        public static List<IBeepTheme> GetThemes()
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
        public static void SaveTheme(IBeepTheme theme, string filePath)
        {
            var serializer = new XmlSerializer(typeof(IBeepTheme));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, theme);
            }
        }

        // Load theme from file
        public static IBeepTheme LoadTheme(string filePath)
        {
            var serializer = new XmlSerializer(typeof(IBeepTheme));
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var theme = (IBeepTheme)serializer.Deserialize(stream);

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
        public static void SetCurrentStyle(FormStyle style)
        {
            // Only set if we can find the style
            if (Enum.IsDefined(typeof(FormStyle), style))
            {
                CurrentStyle = style;
            }
        }
        // Set current theme by object
        public static void SetCurrentTheme(IBeepTheme theme)
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
        public static string GetGuidFromTheme(IBeepTheme theme)
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
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(IBeepTheme)))
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
                        if (themeType != null && themeType.IsSubclassOf(typeof(IBeepTheme)))
                        {
                            var theme = (IBeepTheme)Activator.CreateInstance(themeType);

                           

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

   

       
    }

   
}