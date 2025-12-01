using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in toggle controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class ToggleFontHelpers
    {
        /// <summary>
        /// Gets the font for toggle labels (ON/OFF text)
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetLabelFont(
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            BeepControlStyle controlStyle,
            bool isOn)
        {
            if (toggle == null)
                return BeepFontManager.DefaultFont;

            // Base size from ControlStyle
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Adjust size based on toggle style
            float labelSize = toggleStyle switch
            {
                ToggleStyle.LabeledTrack => baseSize - 2f,      // Smaller for track labels
                ToggleStyle.RectangularLabeled => baseSize - 1f, // Slightly smaller
                ToggleStyle.ButtonStyle => baseSize,             // Standard size
                ToggleStyle.MaterialPill => baseSize - 1f,      // Material style
                ToggleStyle.MaterialSquare => baseSize - 1f,
                _ => baseSize - 2f                               // Default smaller
            };

            // Ensure minimum readable size
            labelSize = Math.Max(8f, labelSize);

            // Font style: Bold for active state, Regular for inactive
            FontStyle fontStyle = isOn ? FontStyle.Bold : FontStyle.Regular;

            // Get font family from ControlStyle
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            
            // Parse font family string (may contain fallbacks like "Roboto, Segoe UI")
            string primaryFont = fontFamily.Split(',')[0].Trim();

            // Use BeepFontManager to get the font
            return BeepFontManager.GetFont(primaryFont, labelSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for toggle button text (when using ButtonStyle)
        /// </summary>
        public static Font GetButtonFont(
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            BeepControlStyle controlStyle,
            bool isActive)
        {
            if (toggle == null)
                return BeepFontManager.ButtonFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = isActive ? FontStyle.Bold : FontStyle.Regular;

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, fontStyle);
        }

        /// <summary>
        /// Gets the default font for the toggle control
        /// Uses ControlStyle to determine appropriate font
        /// </summary>
        public static Font GetToggleFont(
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            BeepControlStyle controlStyle)
        {
            if (toggle == null)
                return BeepFontManager.DefaultFont;

            // Use ControlStyle-based font
            float fontSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = StyleTypography.GetFontStyle(controlStyle);
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, fontSize, fontStyle);
        }

        /// <summary>
        /// Gets a font for icon labels (when icons are used with text)
        /// Slightly smaller than regular labels
        /// </summary>
        public static Font GetIconLabelFont(
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            BeepControlStyle controlStyle)
        {
            if (toggle == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float iconLabelSize = Math.Max(7f, baseSize - 3f); // Smaller for icon labels

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, iconLabelSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets a compact font for small toggle controls
        /// Used when toggle size is constrained
        /// </summary>
        public static Font GetCompactFont(
            BeepToggle toggle,
            BeepControlStyle controlStyle)
        {
            if (toggle == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float compactSize = Math.Max(7f, baseSize - 3f); // Smaller for compact

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, compactSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets a bold font for emphasized toggle text
        /// </summary>
        public static Font GetBoldFont(
            BeepToggle toggle,
            BeepControlStyle controlStyle)
        {
            if (toggle == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, FontStyle.Bold);
        }

        /// <summary>
        /// Gets font size for a specific toggle element
        /// Returns size in points
        /// </summary>
        public static float GetFontSizeForElement(
            ToggleStyle toggleStyle,
            BeepControlStyle controlStyle,
            ToggleFontElement element)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            return element switch
            {
                ToggleFontElement.Label => toggleStyle switch
                {
                    ToggleStyle.LabeledTrack => baseSize - 2f,
                    ToggleStyle.RectangularLabeled => baseSize - 1f,
                    ToggleStyle.ButtonStyle => baseSize,
                    _ => baseSize - 2f
                },
                ToggleFontElement.Button => baseSize,
                ToggleFontElement.IconLabel => Math.Max(7f, baseSize - 3f),
                ToggleFontElement.Compact => Math.Max(7f, baseSize - 3f),
                _ => baseSize
            };
        }

        /// <summary>
        /// Gets font style for a specific toggle element
        /// </summary>
        public static FontStyle GetFontStyleForElement(
            ToggleStyle toggleStyle,
            bool isActive,
            ToggleFontElement element)
        {
            return element switch
            {
                ToggleFontElement.Label => isActive ? FontStyle.Bold : FontStyle.Regular,
                ToggleFontElement.Button => isActive ? FontStyle.Bold : FontStyle.Regular,
                ToggleFontElement.IconLabel => FontStyle.Regular,
                ToggleFontElement.Compact => FontStyle.Regular,
                _ => FontStyle.Regular
            };
        }

        /// <summary>
        /// Applies font theme to toggle control
        /// Updates the control's Font property based on ControlStyle
        /// </summary>
        public static void ApplyFontTheme(
            BeepToggle toggle,
            BeepControlStyle controlStyle)
        {
            if (toggle == null)
                return;

            // Get appropriate font for the control
            Font newFont = GetToggleFont(toggle, toggle.ToggleStyle, controlStyle);

            // Update control font if different
            if (toggle.Font != newFont && newFont != null)
            {
                // Dispose old font if it was created by us
                // Note: We should be careful not to dispose system fonts
                // For now, just assign the new font
                toggle.Font = newFont;
            }
        }
    }

    /// <summary>
    /// Toggle font element types
    /// </summary>
    public enum ToggleFontElement
    {
        /// <summary>
        /// ON/OFF label text
        /// </summary>
        Label,

        /// <summary>
        /// Button-style text
        /// </summary>
        Button,

        /// <summary>
        /// Icon label text
        /// </summary>
        IconLabel,

        /// <summary>
        /// Compact/small text
        /// </summary>
        Compact,

        /// <summary>
        /// Default/regular text
        /// </summary>
        Default
    }
}

