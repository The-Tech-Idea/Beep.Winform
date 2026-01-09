using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers
{
    /// <summary>
    /// Validation result containing warnings and errors
    /// </summary>
    public class ValidationResult
    {
        public List<string> Warnings { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
        public bool IsValid => Errors.Count == 0;

        public void AddWarning(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                Warnings.Add(message);
        }

        public void AddError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                Errors.Add(message);
        }
    }

    /// <summary>
    /// Helper for validating control configurations and suggesting improvements
    /// Checks for common issues, accessibility problems, and missing properties
    /// </summary>
    public static class ControlValidationHelper
    {
        /// <summary>
        /// Validate a control's configuration
        /// </summary>
        /// <param name="control">The control to validate</param>
        /// <returns>Validation result with warnings and errors</returns>
        public static ValidationResult ValidateControl(BaseControl control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            var result = new ValidationResult();

            // Check accessibility
            ValidateAccessibility(control, result);

            // Check color contrast
            ValidateColorContrast(control, result);

            // Check required properties
            ValidateRequiredProperties(control, result);

            // Check size constraints
            ValidateSizeConstraints(control, result);

            return result;
        }

        /// <summary>
        /// Validate accessibility properties
        /// </summary>
        private static void ValidateAccessibility(BaseControl control, ValidationResult result)
        {
            // Check for accessible name
            var accessibleName = control.GetType().GetProperty("AccessibleName")?.GetValue(control) as string;
            if (string.IsNullOrWhiteSpace(accessibleName))
            {
                result.AddWarning("AccessibleName is not set. This may impact screen reader users.");
            }

            // Check for accessible description
            var accessibleDescription = control.GetType().GetProperty("AccessibleDescription")?.GetValue(control) as string;
            if (string.IsNullOrWhiteSpace(accessibleDescription) && !string.IsNullOrWhiteSpace(accessibleName))
            {
                result.AddWarning("AccessibleDescription is not set. Consider adding a description for better accessibility.");
            }

            // Check for accessible role
            var accessibleRole = control.GetType().GetProperty("AccessibleRole")?.GetValue(control);
            if (accessibleRole == null || accessibleRole.ToString() == "Default")
            {
                result.AddWarning("AccessibleRole is set to Default. Consider setting an appropriate role.");
            }
        }

        /// <summary>
        /// Validate color contrast for accessibility
        /// </summary>
        private static void ValidateColorContrast(BaseControl control, ValidationResult result)
        {
            try
            {
                var backColor = control.BackColor;
                var foreColor = control.ForeColor;

                if (backColor.IsEmpty || foreColor.IsEmpty)
                    return;

                // Calculate contrast ratio (simplified WCAG calculation)
                double contrastRatio = CalculateContrastRatio(backColor, foreColor);

                // WCAG AA requires 4.5:1 for normal text, 3:1 for large text
                if (contrastRatio < 3.0)
                {
                    result.AddError($"Color contrast ratio ({contrastRatio:F2}) is too low. WCAG requires at least 3:1 for large text and 4.5:1 for normal text.");
                }
                else if (contrastRatio < 4.5)
                {
                    result.AddWarning($"Color contrast ratio ({contrastRatio:F2}) may be insufficient for normal text. WCAG recommends 4.5:1 or higher.");
                }
            }
            catch
            {
                // Skip contrast validation if colors can't be accessed
            }
        }

        /// <summary>
        /// Calculate contrast ratio between two colors
        /// </summary>
        private static double CalculateContrastRatio(Color color1, Color color2)
        {
            double luminance1 = CalculateRelativeLuminance(color1);
            double luminance2 = CalculateRelativeLuminance(color2);

            double lighter = Math.Max(luminance1, luminance2);
            double darker = Math.Min(luminance1, luminance2);

            return (lighter + 0.05) / (darker + 0.05);
        }

        /// <summary>
        /// Calculate relative luminance of a color (WCAG formula)
        /// </summary>
        private static double CalculateRelativeLuminance(Color color)
        {
            double r = GetColorComponent(color.R);
            double g = GetColorComponent(color.G);
            double b = GetColorComponent(color.B);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        /// <summary>
        /// Get normalized color component for luminance calculation
        /// </summary>
        private static double GetColorComponent(byte component)
        {
            double value = component / 255.0;
            return value <= 0.03928 
                ? value / 12.92 
                : Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        /// <summary>
        /// Validate required properties are set
        /// </summary>
        private static void ValidateRequiredProperties(BaseControl control, ValidationResult result)
        {
            // Check if control has a name (important for designer)
            if (string.IsNullOrWhiteSpace(control.Name))
            {
                result.AddWarning("Control Name is not set. Consider setting a meaningful name.");
            }

            // Check if control is visible but has zero size
            if (control.Visible && (control.Width == 0 || control.Height == 0))
            {
                result.AddWarning("Control is visible but has zero width or height. It may not be visible to users.");
            }
        }

        /// <summary>
        /// Validate size constraints
        /// </summary>
        private static void ValidateSizeConstraints(BaseControl control, ValidationResult result)
        {
            // Check minimum size
            if (control.Width < 10 || control.Height < 10)
            {
                result.AddWarning("Control size is very small. Consider increasing width and height for better usability.");
            }

            // Check maximum reasonable size
            if (control.Width > 10000 || control.Height > 10000)
            {
                result.AddWarning("Control size is very large. This may cause performance issues.");
            }
        }

        /// <summary>
        /// Get validation summary as a formatted string
        /// </summary>
        public static string GetValidationSummary(ValidationResult result)
        {
            if (result.IsValid && result.Warnings.Count == 0)
                return "Validation passed. No issues found.";

            var summary = new System.Text.StringBuilder();

            if (result.Errors.Count > 0)
            {
                summary.AppendLine($"Errors ({result.Errors.Count}):");
                foreach (var error in result.Errors)
                {
                    summary.AppendLine($"  • {error}");
                }
            }

            if (result.Warnings.Count > 0)
            {
                if (summary.Length > 0)
                    summary.AppendLine();

                summary.AppendLine($"Warnings ({result.Warnings.Count}):");
                foreach (var warning in result.Warnings)
                {
                    summary.AppendLine($"  • {warning}");
                }
            }

            return summary.ToString();
        }
    }
}
