// This file now contains only a reference to the refactored partial classes.
// The actual implementation is split across multiple partial class files organized by font categories:
//
// - BeepFontPaths.Core.cs - Core functionality, constants, and utility methods
// - BeepFontPaths.WebFonts.cs - Modern web fonts (Roboto, Open Sans, Montserrat, Inter, etc.)
// - BeepFontPaths.MonospaceFonts.cs - Code/monospace fonts (Fira Code, JetBrains Mono, Cascadia Code, etc.)
// - BeepFontPaths.DisplayFonts.cs - Display/decorative fonts (Bebas Neue, Orbitron, Exo 2, Whitney, etc.)
// - BeepFontPaths.AccessibilityFonts.cs - Accessibility-focused fonts (Atkinson Hyperlegible, OpenDyslexic, Lexend)
// - BeepFontPaths.SystemFonts.cs - System and classic fonts (Source Sans Pro, Comic Neue, etc.)
// - BeepFontPaths.Families.cs - Nested Families class with organized font collections and helper methods
// - BeepFontPaths.Extensions.cs - Extension methods and helper functions for font management integration
//
// This refactoring improves maintainability by organizing fonts into logical categories
// and makes the codebase more manageable as new fonts are added.
//
// For backward compatibility, all existing public APIs remain unchanged.

namespace TheTechIdea.Beep.Winform.Controls.FontManagement
{
    // The BeepFontPaths class is now implemented as partial classes.
    // See the other partial class files for the actual implementation.
    // 
    // All existing functionality remains available:
    // - BeepFontPaths.GetAllFamilies()
    // - BeepFontPaths.GetFontPath(familyName, style)
    // - BeepFontPaths.Families.Roboto.Regular
    // - BeepFontPaths.ResourceExists(path)
    // - BeepFontPathsExtensions.GetBeepFont(family, style, size)
    // 
    // Total font families now supported: 25+
    // Total font files referenced: 200+
}