using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    /// <summary>
    /// Helper class for managing fonts and typography in breadcrumb controls
    /// Integrates with BeepFontManager and StyleTypography for consistent font usage
    /// </summary>
    public static class BreadcrumbFontHelpers
    {
        /// <summary>
        /// Gets the font for breadcrumb item text
        /// Uses BeepFontManager with ControlStyle-aware sizing
        /// </summary>
        public static Font GetItemFont(
            BeepBreadcrump breadcrumb,
            BreadcrumbStyle breadcrumbStyle,
            BeepControlStyle controlStyle,
            bool isLast = false)
        {
            if (breadcrumb == null)
                return BeepFontManager.DefaultFont;

            // Base size from ControlStyle
            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Adjust size based on breadcrumb style
            float itemSize = breadcrumbStyle switch
            {
                BreadcrumbStyle.Classic => baseSize - 1f,      // Slightly smaller for classic
                BreadcrumbStyle.Modern => baseSize,            // Standard size
                BreadcrumbStyle.Pill => baseSize - 0.5f,       // Slightly smaller for pill
                BreadcrumbStyle.Flat => baseSize,              // Standard size
                _ => baseSize                                  // Default standard
            };

            // Last item can be slightly larger or bold
            if (isLast)
            {
                itemSize = Math.Min(itemSize + 0.5f, baseSize + 1f);
            }

            // Ensure minimum readable size
            itemSize = Math.Max(8f, itemSize);

            // Font style: Regular for items, can be customized
            FontStyle fontStyle = isLast ? FontStyle.Bold : FontStyle.Regular;

            // Get font family from ControlStyle
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            
            // Parse font family string (may contain fallbacks like "Roboto, Segoe UI")
            string primaryFont = fontFamily.Split(',')[0].Trim();

            // Use BeepFontManager to get the font
            return BeepFontManager.GetFont(primaryFont, itemSize, fontStyle);
        }

        /// <summary>
        /// Gets the font for breadcrumb separator text
        /// Typically smaller and lighter than item text
        /// </summary>
        public static Font GetSeparatorFont(
            BeepBreadcrump breadcrumb,
            BreadcrumbStyle breadcrumbStyle,
            BeepControlStyle controlStyle)
        {
            if (breadcrumb == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            
            // Separators are typically smaller
            float separatorSize = Math.Max(7f, baseSize - 2f);

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, separatorSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets the font for home icon label (if text-based)
        /// </summary>
        public static Font GetHomeIconFont(
            BeepBreadcrump breadcrumb,
            BeepControlStyle controlStyle)
        {
            if (breadcrumb == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float homeSize = Math.Max(8f, baseSize - 1f);

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, homeSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets the default font for the breadcrumb control
        /// Uses ControlStyle to determine appropriate font
        /// </summary>
        public static Font GetBreadcrumbFont(
            BeepBreadcrump breadcrumb,
            BreadcrumbStyle breadcrumbStyle,
            BeepControlStyle controlStyle)
        {
            if (breadcrumb == null)
                return BeepFontManager.DefaultFont;

            // Use ControlStyle-based font
            float fontSize = StyleTypography.GetFontSize(controlStyle);
            FontStyle fontStyle = StyleTypography.GetFontStyle(controlStyle);
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, fontSize, fontStyle);
        }

        /// <summary>
        /// Gets a compact font for small breadcrumb controls
        /// Used when breadcrumb size is constrained
        /// </summary>
        public static Font GetCompactFont(
            BeepBreadcrump breadcrumb,
            BeepControlStyle controlStyle)
        {
            if (breadcrumb == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            float compactSize = Math.Max(7f, baseSize - 2f); // Smaller for compact

            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, compactSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets a bold font for emphasized breadcrumb text (e.g., last item)
        /// </summary>
        public static Font GetBoldFont(
            BeepBreadcrump breadcrumb,
            BeepControlStyle controlStyle)
        {
            if (breadcrumb == null)
                return BeepFontManager.DefaultFont;

            float baseSize = StyleTypography.GetFontSize(controlStyle);
            string fontFamily = StyleTypography.GetFontFamily(controlStyle);
            string primaryFont = fontFamily.Split(',')[0].Trim();

            return BeepFontManager.GetFont(primaryFont, baseSize, FontStyle.Bold);
        }

        /// <summary>
        /// Gets font size for a specific breadcrumb element
        /// Returns size in points
        /// </summary>
        public static float GetFontSizeForElement(
            BreadcrumbStyle breadcrumbStyle,
            BeepControlStyle controlStyle,
            BreadcrumbFontElement element)
        {
            float baseSize = StyleTypography.GetFontSize(controlStyle);

            return element switch
            {
                BreadcrumbFontElement.Item => breadcrumbStyle switch
                {
                    BreadcrumbStyle.Classic => baseSize - 1f,
                    BreadcrumbStyle.Modern => baseSize,
                    BreadcrumbStyle.Pill => baseSize - 0.5f,
                    BreadcrumbStyle.Flat => baseSize,
                    _ => baseSize
                },
                BreadcrumbFontElement.Separator => Math.Max(7f, baseSize - 2f),
                BreadcrumbFontElement.HomeIcon => Math.Max(8f, baseSize - 1f),
                BreadcrumbFontElement.Compact => Math.Max(7f, baseSize - 2f),
                BreadcrumbFontElement.LastItem => Math.Min(baseSize + 0.5f, baseSize + 1f),
                _ => baseSize
            };
        }

        /// <summary>
        /// Gets font style for a specific breadcrumb element
        /// </summary>
        public static FontStyle GetFontStyleForElement(
            BreadcrumbFontElement element,
            bool isLast = false)
        {
            return element switch
            {
                BreadcrumbFontElement.Item => isLast ? FontStyle.Bold : FontStyle.Regular,
                BreadcrumbFontElement.LastItem => FontStyle.Bold,
                BreadcrumbFontElement.Separator => FontStyle.Regular,
                BreadcrumbFontElement.HomeIcon => FontStyle.Regular,
                BreadcrumbFontElement.Compact => FontStyle.Regular,
                _ => FontStyle.Regular
            };
        }

        /// <summary>
        /// Applies font theme to breadcrumb control
        /// Updates the control's TextFont property based on ControlStyle
        /// </summary>
        public static void ApplyFontTheme(
            BeepBreadcrump breadcrumb,
            BeepControlStyle controlStyle)
        {
            if (breadcrumb == null)
                return;

            // Get appropriate font for the control
            Font newFont = GetBreadcrumbFont(breadcrumb, breadcrumb.BreadcrumbStyle, controlStyle);

            // Update control font if different
            if (breadcrumb.TextFont != newFont && newFont != null)
            {
                // Dispose old font if it was created by us
                // Note: We should be careful not to dispose system fonts
                // For now, just assign the new font
                breadcrumb.TextFont = newFont;
            }
        }
    }

    /// <summary>
    /// Breadcrumb font element types
    /// </summary>
    public enum BreadcrumbFontElement
    {
        /// <summary>
        /// Regular breadcrumb item text
        /// </summary>
        Item,

        /// <summary>
        /// Last breadcrumb item (typically bold)
        /// </summary>
        LastItem,

        /// <summary>
        /// Separator text between items
        /// </summary>
        Separator,

        /// <summary>
        /// Home icon label (if text-based)
        /// </summary>
        HomeIcon,

        /// <summary>
        /// Compact/small text for constrained spaces
        /// </summary>
        Compact,

        /// <summary>
        /// Default/regular text
        /// </summary>
        Default
    }
}

