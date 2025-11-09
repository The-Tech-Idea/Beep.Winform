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
                return CreatePresetPainter(config.Preset);
            }

            // Otherwise use BeepStyledDialogPainter with current style
            return new BeepStyledDialogPainter();
        }

        /// <summary>
        /// Create painter for specific preset
        /// </summary>
        /// <param name="preset">Dialog preset</param>
        /// <returns>PresetDialogPainter configured for the preset</returns>
        public static IDialogPainter CreatePresetPainter(DialogPreset preset)
        {
            return new PresetDialogPainter(preset);
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
    }
}
