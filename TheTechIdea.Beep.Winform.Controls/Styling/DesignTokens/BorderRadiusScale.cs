using System;

namespace TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens
{
    /// <summary>
    /// Standardized border radius scale for consistent UI design
    /// Provides predefined radius values following modern design system principles
    /// </summary>
    public static class BorderRadiusScale
    {
        /// <summary>
        /// Small radius - 4px - For small controls, badges, tags
        /// </summary>
        public const int Small = 4;

        /// <summary>
        /// Medium radius - 8px - Standard for buttons, inputs, cards (most common)
        /// </summary>
        public const int Medium = 8;

        /// <summary>
        /// Large radius - 12px - For larger cards, panels, modals
        /// </summary>
        public const int Large = 12;

        /// <summary>
        /// Extra large radius - 16px - For prominent cards, dialogs
        /// </summary>
        public const int XLarge = 16;

        /// <summary>
        /// Round radius - 24px - For pill-shaped controls, avatars
        /// </summary>
        public const int Round = 24;

        /// <summary>
        /// Get responsive radius that scales with control size
        /// </summary>
        /// <param name="controlSize">Size of the control (width or height)</param>
        /// <param name="baseRadius">Base radius size (Small, Medium, Large, etc.)</param>
        /// <returns>Scaled radius value</returns>
        public static int GetResponsiveRadius(int controlSize, int baseRadius = Medium)
        {
            // Scale radius proportionally with control size
            // For very small controls (< 40px), use smaller radius
            // For large controls (> 200px), use larger radius
            if (controlSize < 40)
            {
                return Math.Max(2, baseRadius / 2);
            }
            else if (controlSize > 200)
            {
                return (int)(baseRadius * 1.5f);
            }
            return baseRadius;
        }

        /// <summary>
        /// Get radius for a specific control type
        /// </summary>
        /// <param name="controlType">Type of control</param>
        /// <returns>Recommended radius</returns>
        public static int GetRadiusForControlType(ControlType controlType)
        {
            return controlType switch
            {
                ControlType.Button => Medium,
                ControlType.Input => Medium,
                ControlType.Card => Large,
                ControlType.Panel => Large,
                ControlType.Modal => XLarge,
                ControlType.Badge => Small,
                ControlType.Tag => Small,
                ControlType.Avatar => Round,
                ControlType.Pill => Round,
                ControlType.Tab => Small,
                _ => Medium
            };
        }
    }

    /// <summary>
    /// Control type enumeration for radius recommendations
    /// </summary>
    public enum ControlType
    {
        Button,
        Input,
        Card,
        Panel,
        Modal,
        Badge,
        Tag,
        Avatar,
        Pill,
        Tab
    }
}
