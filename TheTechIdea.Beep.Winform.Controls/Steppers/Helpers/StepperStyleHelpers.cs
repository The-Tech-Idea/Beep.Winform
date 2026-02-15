using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    /// <summary>
    /// Helper class for mapping stepper styles to styling properties
    /// Integrates with BeepStyling system for consistent styling
    /// </summary>
    public static class StepperStyleHelpers
    {
        /// <summary>
        /// Gets recommended button size for stepper based on control style
        /// </summary>
        public static System.Drawing.Size GetRecommendedButtonSize(BeepControlStyle controlStyle, Control ownerControl = null)
        {
            var size = controlStyle switch
            {
                BeepControlStyle.Material3 => new System.Drawing.Size(40, 40),
                BeepControlStyle.MaterialYou => new System.Drawing.Size(40, 40),
                BeepControlStyle.iOS15 => new System.Drawing.Size(44, 44),
                BeepControlStyle.MacOSBigSur => new System.Drawing.Size(40, 40),
                BeepControlStyle.Fluent2 => new System.Drawing.Size(36, 36),
                BeepControlStyle.Windows11Mica => new System.Drawing.Size(36, 36),
                BeepControlStyle.Minimal => new System.Drawing.Size(32, 32),
                BeepControlStyle.NotionMinimal => new System.Drawing.Size(32, 32),
                BeepControlStyle.VercelClean => new System.Drawing.Size(32, 32),
                BeepControlStyle.Bootstrap => new System.Drawing.Size(38, 38),
                BeepControlStyle.TailwindCard => new System.Drawing.Size(36, 36),
                BeepControlStyle.StripeDashboard => new System.Drawing.Size(36, 36),
                BeepControlStyle.FigmaCard => new System.Drawing.Size(38, 38),
                BeepControlStyle.DiscordStyle => new System.Drawing.Size(36, 36),
                BeepControlStyle.AntDesign => new System.Drawing.Size(32, 32),
                BeepControlStyle.ChakraUI => new System.Drawing.Size(36, 36),
                BeepControlStyle.PillRail => new System.Drawing.Size(40, 40),
                BeepControlStyle.Metro => new System.Drawing.Size(36, 36),
                BeepControlStyle.Office => new System.Drawing.Size(36, 36),
                BeepControlStyle.NeoBrutalist => new System.Drawing.Size(40, 40),
                BeepControlStyle.HighContrast => new System.Drawing.Size(44, 44), // Larger for accessibility
                _ => new System.Drawing.Size(36, 36)
            };
            return DpiScalingHelper.ScaleSize(size, ownerControl);
        }

        /// <summary>
        /// Gets recommended connector line width based on control style
        /// </summary>
        public static int GetRecommendedConnectorLineWidth(BeepControlStyle controlStyle, Control ownerControl = null)
        {
            int baseValue = controlStyle switch
            {
                BeepControlStyle.Material3 => 2,
                BeepControlStyle.MaterialYou => 2,
                BeepControlStyle.iOS15 => 2,
                BeepControlStyle.MacOSBigSur => 2,
                BeepControlStyle.Fluent2 => 2,
                BeepControlStyle.Windows11Mica => 2,
                BeepControlStyle.Minimal => 1,
                BeepControlStyle.NotionMinimal => 1,
                BeepControlStyle.VercelClean => 1,
                BeepControlStyle.Bootstrap => 2,
                BeepControlStyle.TailwindCard => 2,
                BeepControlStyle.StripeDashboard => 2,
                BeepControlStyle.FigmaCard => 2,
                BeepControlStyle.DiscordStyle => 2,
                BeepControlStyle.AntDesign => 2,
                BeepControlStyle.ChakraUI => 2,
                BeepControlStyle.PillRail => 2,
                BeepControlStyle.Metro => 2,
                BeepControlStyle.Office => 2,
                BeepControlStyle.NeoBrutalist => 3,
                BeepControlStyle.HighContrast => 3, // Thicker for accessibility
                _ => 2
            };
            return Math.Max(1, DpiScalingHelper.ScaleValue(baseValue, ownerControl));
        }

        /// <summary>
        /// Gets recommended spacing between steps based on control style
        /// </summary>
        public static int GetRecommendedStepSpacing(BeepControlStyle controlStyle, Control ownerControl = null)
        {
            int baseValue = controlStyle switch
            {
                BeepControlStyle.Material3 => 24,
                BeepControlStyle.MaterialYou => 24,
                BeepControlStyle.iOS15 => 28,
                BeepControlStyle.MacOSBigSur => 24,
                BeepControlStyle.Fluent2 => 20,
                BeepControlStyle.Windows11Mica => 20,
                BeepControlStyle.Minimal => 16,
                BeepControlStyle.NotionMinimal => 16,
                BeepControlStyle.VercelClean => 16,
                BeepControlStyle.Bootstrap => 24,
                BeepControlStyle.TailwindCard => 20,
                BeepControlStyle.StripeDashboard => 20,
                BeepControlStyle.FigmaCard => 24,
                BeepControlStyle.DiscordStyle => 20,
                BeepControlStyle.AntDesign => 24,
                BeepControlStyle.ChakraUI => 24,
                BeepControlStyle.PillRail => 24,
                BeepControlStyle.Metro => 20,
                BeepControlStyle.Office => 20,
                BeepControlStyle.NeoBrutalist => 24,
                BeepControlStyle.HighContrast => 28, // More spacing for accessibility
                _ => 20
            };
            return DpiScalingHelper.ScaleValue(baseValue, ownerControl);
        }

        /// <summary>
        /// Gets recommended border radius for step buttons based on control style
        /// </summary>
        public static int GetRecommendedBorderRadius(BeepControlStyle controlStyle, int buttonSize, Control ownerControl = null)
        {
            // For circular buttons, radius is half the size
            // But we can adjust based on style for rounded rectangles if needed
            int baseValue = controlStyle switch
            {
                BeepControlStyle.Material3 => buttonSize / 2, // Circular
                BeepControlStyle.MaterialYou => buttonSize / 2,
                BeepControlStyle.iOS15 => buttonSize / 2,
                BeepControlStyle.MacOSBigSur => buttonSize / 2,
                BeepControlStyle.Fluent2 => buttonSize / 2,
                BeepControlStyle.Windows11Mica => buttonSize / 2,
                BeepControlStyle.Minimal => buttonSize / 2,
                BeepControlStyle.NotionMinimal => buttonSize / 2,
                BeepControlStyle.VercelClean => buttonSize / 2,
                BeepControlStyle.Bootstrap => buttonSize / 2,
                BeepControlStyle.TailwindCard => buttonSize / 2,
                BeepControlStyle.StripeDashboard => buttonSize / 2,
                BeepControlStyle.FigmaCard => buttonSize / 2,
                BeepControlStyle.DiscordStyle => buttonSize / 2,
                BeepControlStyle.AntDesign => buttonSize / 2,
                BeepControlStyle.ChakraUI => buttonSize / 2,
                BeepControlStyle.PillRail => buttonSize / 2,
                BeepControlStyle.Metro => buttonSize / 2,
                BeepControlStyle.Office => buttonSize / 2,
                BeepControlStyle.NeoBrutalist => 4, // Square with slight rounding
                BeepControlStyle.HighContrast => buttonSize / 2,
                _ => buttonSize / 2
            };
            return DpiScalingHelper.ScaleValue(baseValue, ownerControl);
        }

        /// <summary>
        /// Gets recommended label spacing (distance from step button to label)
        /// </summary>
        public static int GetRecommendedLabelSpacing(BeepControlStyle controlStyle, Control ownerControl = null)
        {
            int baseValue = controlStyle switch
            {
                BeepControlStyle.Material3 => 8,
                BeepControlStyle.MaterialYou => 8,
                BeepControlStyle.iOS15 => 10,
                BeepControlStyle.MacOSBigSur => 8,
                BeepControlStyle.Fluent2 => 6,
                BeepControlStyle.Windows11Mica => 6,
                BeepControlStyle.Minimal => 6,
                BeepControlStyle.NotionMinimal => 6,
                BeepControlStyle.VercelClean => 6,
                BeepControlStyle.Bootstrap => 8,
                BeepControlStyle.TailwindCard => 6,
                BeepControlStyle.StripeDashboard => 6,
                BeepControlStyle.FigmaCard => 8,
                BeepControlStyle.DiscordStyle => 6,
                BeepControlStyle.AntDesign => 8,
                BeepControlStyle.ChakraUI => 8,
                BeepControlStyle.PillRail => 8,
                BeepControlStyle.Metro => 6,
                BeepControlStyle.Office => 6,
                BeepControlStyle.NeoBrutalist => 8,
                BeepControlStyle.HighContrast => 10, // More spacing for accessibility
                _ => 8
            };
            return DpiScalingHelper.ScaleValue(baseValue, ownerControl);
        }

        /// <summary>
        /// Gets recommended border width for step buttons based on control style
        /// </summary>
        public static int GetRecommendedBorderWidth(BeepControlStyle controlStyle, Control ownerControl = null)
        {
            int baseValue = controlStyle switch
            {
                BeepControlStyle.Material3 => 2,
                BeepControlStyle.MaterialYou => 2,
                BeepControlStyle.iOS15 => 2,
                BeepControlStyle.MacOSBigSur => 2,
                BeepControlStyle.Fluent2 => 2,
                BeepControlStyle.Windows11Mica => 2,
                BeepControlStyle.Minimal => 1,
                BeepControlStyle.NotionMinimal => 1,
                BeepControlStyle.VercelClean => 1,
                BeepControlStyle.Bootstrap => 2,
                BeepControlStyle.TailwindCard => 2,
                BeepControlStyle.StripeDashboard => 2,
                BeepControlStyle.FigmaCard => 2,
                BeepControlStyle.DiscordStyle => 2,
                BeepControlStyle.AntDesign => 2,
                BeepControlStyle.ChakraUI => 2,
                BeepControlStyle.PillRail => 2,
                BeepControlStyle.Metro => 2,
                BeepControlStyle.Office => 2,
                BeepControlStyle.NeoBrutalist => 3,
                BeepControlStyle.HighContrast => 3, // Thicker for accessibility
                _ => 2
            };
            return Math.Max(1, DpiScalingHelper.ScaleValue(baseValue, ownerControl));
        }

        /// <summary>
        /// Gets recommended padding for stepper control based on control style
        /// </summary>
        public static int GetRecommendedPadding(BeepControlStyle controlStyle, Control ownerControl = null)
        {
            int baseValue = controlStyle switch
            {
                BeepControlStyle.Material3 => 16,
                BeepControlStyle.MaterialYou => 16,
                BeepControlStyle.iOS15 => 20,
                BeepControlStyle.MacOSBigSur => 16,
                BeepControlStyle.Fluent2 => 12,
                BeepControlStyle.Windows11Mica => 12,
                BeepControlStyle.Minimal => 8,
                BeepControlStyle.NotionMinimal => 8,
                BeepControlStyle.VercelClean => 8,
                BeepControlStyle.Bootstrap => 16,
                BeepControlStyle.TailwindCard => 12,
                BeepControlStyle.StripeDashboard => 12,
                BeepControlStyle.FigmaCard => 16,
                BeepControlStyle.DiscordStyle => 12,
                BeepControlStyle.AntDesign => 16,
                BeepControlStyle.ChakraUI => 16,
                BeepControlStyle.PillRail => 16,
                BeepControlStyle.Metro => 12,
                BeepControlStyle.Office => 12,
                BeepControlStyle.NeoBrutalist => 16,
                BeepControlStyle.HighContrast => 20, // More padding for accessibility
                _ => 12
            };
            return DpiScalingHelper.ScaleValue(baseValue, ownerControl);
        }
    }
}
