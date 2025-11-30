using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters
{
    /// <summary>
    /// Factory for creating appropriate dialog painters
    /// Maps DialogPreset and BeepControlStyle to painter implementations
    /// </summary>
    public static class DialogPainterFactory
    {
        /// <summary>
        /// Create painter based on dialog configuration
        /// </summary>
        /// <param name="config">Dialog configuration</param>
        /// <returns>Appropriate IDialogPainter implementation</returns>
        public static IDialogPainter CreatePainter(DialogConfig config)
        {
            if (config == null)
                return new BeepStyledDialogPainter();

            // If preset is specified, use preset painter
            if (config.Preset != DialogPreset.None)
            {
                return CreatePresetPainter(config.Preset, config.Style);
            }

            // Create painter based on BeepControlStyle
            return CreatePainterForStyle(config.Style);
        }

        /// <summary>
        /// Create painter for specific BeepControlStyle
        /// </summary>
        /// <param name="style">BeepControlStyle to use</param>
        /// <returns>Appropriate IDialogPainter implementation</returns>
        public static IDialogPainter CreatePainterForStyle(BeepControlStyle style)
        {
            return style switch
            {
                // Material Design styles
                BeepControlStyle.Material3 => new Material3DialogPainter(),
                BeepControlStyle.Material => new Material3DialogPainter(),
                BeepControlStyle.MaterialYou => new Material3DialogPainter(),

                // Fluent Design styles
                BeepControlStyle.Fluent => new FluentDialogPainter(),
                BeepControlStyle.Fluent2 => new FluentDialogPainter(),
                BeepControlStyle.Windows11Mica => new FluentDialogPainter(),

                // Glassmorphism styles
                BeepControlStyle.Glassmorphism => new GlassmorphismDialogPainter(),
                BeepControlStyle.GlassAcrylic => new GlassmorphismDialogPainter(),
                BeepControlStyle.GradientModern => new GlassmorphismDialogPainter(),

                // Neumorphism styles
                BeepControlStyle.Neumorphism => new NeumorphismDialogPainter(),

                // Default to BeepStyledDialogPainter for other styles
                _ => new BeepStyledDialogPainter()
            };
        }

        /// <summary>
        /// Create painter for specific preset
        /// </summary>
        /// <param name="preset">Dialog preset</param>
        /// <param name="style">Optional BeepControlStyle</param>
        /// <returns>PresetDialogPainter configured for the preset</returns>
        public static IDialogPainter CreatePresetPainter(DialogPreset preset, BeepControlStyle style = BeepControlStyle.Material3)
        {
            return new PresetDialogPainter(preset, style);
        }

        /// <summary>
        /// Get default painter (BeepStyledDialogPainter)
        /// </summary>
        /// <returns>Default dialog painter</returns>
        public static IDialogPainter GetDefaultPainter()
        {
            return new BeepStyledDialogPainter();
        }

        /// <summary>
        /// Get Material 3 dialog painter
        /// </summary>
        public static IDialogPainter GetMaterial3Painter()
        {
            return new Material3DialogPainter();
        }

        /// <summary>
        /// Get Fluent Design dialog painter
        /// </summary>
        public static IDialogPainter GetFluentPainter()
        {
            return new FluentDialogPainter();
        }

        /// <summary>
        /// Get Glassmorphism dialog painter
        /// </summary>
        public static IDialogPainter GetGlassmorphismPainter()
        {
            return new GlassmorphismDialogPainter();
        }

        /// <summary>
        /// Get Neumorphism dialog painter
        /// </summary>
        public static IDialogPainter GetNeumorphismPainter()
        {
            return new NeumorphismDialogPainter();
        }

        /// <summary>
        /// Check if preset requires special handling
        /// </summary>
        /// <param name="preset">Dialog preset</param>
        /// <returns>True if preset needs custom painter</returns>
        public static bool RequiresCustomPainter(DialogPreset preset)
        {
            return preset != DialogPreset.None;
        }

        /// <summary>
        /// Get recommended painter for specific dialog type
        /// </summary>
        /// <param name="iconType">Dialog icon type (Information, Warning, Error, etc.)</param>
        /// <param name="usePreset">Whether to use preset styling</param>
        /// <returns>Appropriate painter</returns>
        public static IDialogPainter CreatePainterForType(Vis.Modules.BeepDialogIcon iconType, bool usePreset = false)
        {
            if (!usePreset)
                return new BeepStyledDialogPainter();

            // Map icon types to appropriate presets
            DialogPreset preset = iconType switch
            {
                Vis.Modules.BeepDialogIcon.Success => DialogPreset.Success,
                Vis.Modules.BeepDialogIcon.Error => DialogPreset.Danger,
                Vis.Modules.BeepDialogIcon.Warning => DialogPreset.Warning,
                Vis.Modules.BeepDialogIcon.Question => DialogPreset.Question,
                Vis.Modules.BeepDialogIcon.Information => DialogPreset.Information,
                _ => DialogPreset.None
            };

            return preset == DialogPreset.None ? 
                new BeepStyledDialogPainter() : 
                new PresetDialogPainter(preset);
        }

        /// <summary>
        /// Gets all available painter types
        /// </summary>
        public static string[] GetAvailablePainters()
        {
            return new[]
            {
                "BeepStyled",
                "Material3",
                "Fluent",
                "Glassmorphism",
                "Neumorphism",
                "Preset"
            };
        }
    }
}
