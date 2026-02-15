using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    /// <summary>
    /// Font element types for stepper controls
    /// </summary>
    public enum StepperFontElement
    {
        StepNumber,
        StepLabel,
        StepText,
        Connector
    }

    /// <summary>
    /// Centralized font management for Stepper controls
    /// Integrates with BeepFontManager and StyleTypography
    /// </summary>
    public static class StepperFontHelpers
    {
        #region Font Retrieval Methods

        /// <summary>
        /// Get font for step numbers displayed inside step circles
        /// </summary>
        public static Font GetStepNumberFont(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null, Control ownerControl = null)
        {
            Font baseFont = textFont ?? (stepper != null ? stepper.Font : null) ?? BeepFontManager.DefaultFont;
            if (baseFont == null)
                baseFont = BeepFontManager.GetFont("Segoe UI", 10, FontStyle.Bold) ?? new Font("Segoe UI", 10, FontStyle.Bold);

            float dpiScale = ownerControl != null ? DpiScalingHelper.GetDpiScaleFactor(ownerControl) : 1f;
            int minSize = Math.Max(6, (int)(8 * dpiScale));
            int maxSize = Math.Max(minSize, (int)(16 * dpiScale));
            int fontSize = GetFontSizeForElement(controlStyle, StepperFontElement.StepNumber, ownerControl);
            
            if (stepper != null && HasProperty(stepper, "ButtonSize"))
            {
                var buttonSize = GetPropertyValue<Size?>(stepper, "ButtonSize");
                if (buttonSize.HasValue)
                    fontSize = Math.Max(minSize, Math.Min(maxSize, (int)(buttonSize.Value.Height * 0.35f)));
            }

            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StepperFontElement.StepNumber);
            
            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for step labels (text below/next to steps)
        /// </summary>
        public static Font GetStepLabelFont(dynamic stepper, BeepControlStyle controlStyle, StepState state = StepState.Pending, Font textFont = null, Control ownerControl = null)
        {
            Font baseFont = textFont ?? (stepper != null ? stepper.Font : null) ?? BeepFontManager.DefaultFont;
            if (baseFont == null)
                baseFont = BeepFontManager.GetFont("Segoe UI", 9, FontStyle.Regular) ?? new Font("Segoe UI", 9, FontStyle.Regular);
            
            int fontSize = GetFontSizeForElement(controlStyle, StepperFontElement.StepLabel, ownerControl);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StepperFontElement.StepLabel);
            if (state == StepState.Active)
            {
                fontStyle |= FontStyle.Bold;
                fontSize = (int)(fontSize * 1.1f);
            }

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get font for additional step text
        /// </summary>
        public static Font GetStepTextFont(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null, Control ownerControl = null)
        {
            Font baseFont = textFont ?? (stepper != null ? stepper.Font : null) ?? BeepFontManager.DefaultFont;
            if (baseFont == null)
                baseFont = BeepFontManager.GetFont("Segoe UI", 9, FontStyle.Regular) ?? new Font("Segoe UI", 9, FontStyle.Regular);
            int fontSize = GetFontSizeForElement(controlStyle, StepperFontElement.StepText, ownerControl);
            FontStyle fontStyle = GetFontStyleForElement(controlStyle, StepperFontElement.StepText);

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, fontSize, fontStyle);
            }
            catch
            {
                return new Font(baseFont.FontFamily, fontSize, fontStyle);
            }
        }

        /// <summary>
        /// Get general stepper font (for breadcrumb text)
        /// </summary>
        public static Font GetStepperFont(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null)
        {
            Font baseFont = textFont ?? (stepper != null ? stepper.Font : null) ?? BeepFontManager.DefaultFont;
            if (baseFont == null)
                baseFont = BeepFontManager.GetFont("Segoe UI", 9, FontStyle.Regular) ?? new Font("Segoe UI", 9, FontStyle.Regular);
            
            // Use StyleTypography for control style-based font
            try
            {
                Font styleFont = StyleTypography.GetFont(controlStyle);
                if (styleFont != null)
                    return styleFont;
            }
            catch
            {
                // Fallback to BeepFontManager
            }

            try
            {
                var fontFamily = BeepFontManager.GetFontFamily(controlStyle) ?? baseFont.FontFamily;
                return BeepFontManager.GetFont(fontFamily.Name, baseFont.Size, baseFont.Style);
            }
            catch
            {
                return baseFont;
            }
        }

        #endregion

        #region Font Size and Style Helpers

        /// <summary>
        /// Get font size for a specific stepper font element based on control style
        /// </summary>
        public static int GetFontSizeForElement(BeepControlStyle controlStyle, StepperFontElement element, Control ownerControl = null)
        {
            int baseSize = element switch
            {
                StepperFontElement.StepNumber => 10,
                StepperFontElement.StepLabel => 9,
                StepperFontElement.StepText => 9,
                StepperFontElement.Connector => 8,
                _ => 9
            };

            float multiplier = controlStyle switch
            {
                BeepControlStyle.Material3 => 1.1f,
                BeepControlStyle.MaterialYou => 1.1f,
                BeepControlStyle.iOS15 => 1.15f,
                BeepControlStyle.MacOSBigSur => 1.1f,
                BeepControlStyle.Fluent2 => 1.0f,
                BeepControlStyle.Windows11Mica => 1.0f,
                BeepControlStyle.Minimal => 0.95f,
                BeepControlStyle.NotionMinimal => 0.9f,
                BeepControlStyle.VercelClean => 0.95f,
                BeepControlStyle.Bootstrap => 1.0f,
                BeepControlStyle.TailwindCard => 1.0f,
                BeepControlStyle.StripeDashboard => 1.0f,
                BeepControlStyle.FigmaCard => 1.05f,
                BeepControlStyle.DiscordStyle => 0.95f,
                BeepControlStyle.AntDesign => 1.0f,
                BeepControlStyle.ChakraUI => 1.0f,
                BeepControlStyle.PillRail => 1.0f,
                BeepControlStyle.Metro => 1.0f,
                BeepControlStyle.Office => 1.0f,
                BeepControlStyle.NeoBrutalist => 1.1f,
                BeepControlStyle.HighContrast => 1.2f,
                _ => 1.0f
            };

            float dpiScale = ownerControl != null ? DpiScalingHelper.GetDpiScaleFactor(ownerControl) : 1f;
            int minSize = Math.Max(6, (int)(8 * dpiScale));
            int maxSize = Math.Max(minSize, (int)(16 * dpiScale));
            return Math.Max(minSize, Math.Min(maxSize, (int)(baseSize * multiplier * dpiScale)));
        }

        /// <summary>
        /// Get font style for a specific stepper font element based on control style
        /// </summary>
        public static FontStyle GetFontStyleForElement(BeepControlStyle controlStyle, StepperFontElement element)
        {
            // Step numbers are typically bold
            if (element == StepperFontElement.StepNumber)
                return FontStyle.Bold;

            // Labels can vary by style
            if (element == StepperFontElement.StepLabel)
            {
                return controlStyle switch
                {
                    BeepControlStyle.Material3 => FontStyle.Bold,
                    BeepControlStyle.MaterialYou => FontStyle.Bold,
                    BeepControlStyle.iOS15 => FontStyle.Regular,
                    BeepControlStyle.MacOSBigSur => FontStyle.Regular,
                    BeepControlStyle.Fluent2 => FontStyle.Regular,
                    BeepControlStyle.Windows11Mica => FontStyle.Regular,
                    BeepControlStyle.Minimal => FontStyle.Regular,
                    BeepControlStyle.NotionMinimal => FontStyle.Regular,
                    BeepControlStyle.VercelClean => FontStyle.Regular,
                    BeepControlStyle.Bootstrap => FontStyle.Regular,
                    BeepControlStyle.TailwindCard => FontStyle.Regular,
                    BeepControlStyle.StripeDashboard => FontStyle.Regular,
                    BeepControlStyle.FigmaCard => FontStyle.Regular,
                    BeepControlStyle.DiscordStyle => FontStyle.Regular,
                    BeepControlStyle.AntDesign => FontStyle.Regular,
                    BeepControlStyle.ChakraUI => FontStyle.Regular,
                    BeepControlStyle.PillRail => FontStyle.Regular,
                    BeepControlStyle.Metro => FontStyle.Regular,
                    BeepControlStyle.Office => FontStyle.Regular,
                    BeepControlStyle.NeoBrutalist => FontStyle.Bold,
                    BeepControlStyle.HighContrast => FontStyle.Bold, // Bold for accessibility
                    _ => FontStyle.Regular
                };
            }

            // Default to regular
            return FontStyle.Regular;
        }

        #endregion

        #region Font Theme Application

        /// <summary>
        /// Apply font theme to a stepper control
        /// Updates fonts based on control style
        /// </summary>
        public static void ApplyFontTheme(dynamic stepper, BeepControlStyle controlStyle, Font textFont = null)
        {
            if (stepper == null)
                return;

            try
            {
                Font stepperFont = GetStepperFont(stepper, controlStyle, textFont);
                
                // Update control font if property exists
                if (HasProperty(stepper, "Font"))
                {
                    var oldFont = GetPropertyValue<Font>(stepper, "Font");
                    if (oldFont != null)
                        oldFont.Dispose();
                    
                    SetPropertyValue(stepper, "Font", stepperFont);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StepperFontHelpers] Error applying font theme: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private static bool HasProperty(dynamic obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                return property != null;
            }
            catch
            {
                return false;
            }
        }

        private static T GetPropertyValue<T>(dynamic obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    if (value is T typedValue)
                        return typedValue;
                }
            }
            catch
            {
                // Ignore
            }
            return default(T);
        }

        private static void SetPropertyValue(dynamic obj, string propertyName, object value)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, value);
                }
            }
            catch
            {
                // Ignore
            }
        }

        #endregion
    }
}

