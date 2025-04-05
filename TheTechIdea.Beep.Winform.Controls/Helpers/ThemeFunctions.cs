using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class ThemeFunctions
    {
        public static void ApplyRoundedToControl(Control control, bool isrounded)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["IsRounded"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, isrounded);
            }
            if (control.Controls.Count > 0 && !HasThemeProperty(control))
            {
                foreach (Control child in control.Controls)
                {
                    ApplyRoundedToControl(child, isrounded);
                }
            }

        }
        public static void ApplyBorderToControl(Control control, bool showborder)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowAllBorders"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, showborder);

            }
            if (control.Controls.Count > 0 && !HasThemeProperty(control))
            {
                foreach (Control child in control.Controls)
                {
                    ApplyBorderToControl(child, showborder);
                }
            }

        }
        public static void ApplyShadowToControl(Control control, bool showshadow)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowShadow"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, showshadow);
            }
            if (control.Controls.Count > 0 && !HasThemeProperty(control))
            {
                foreach (Control child in control.Controls)
                {
                    ApplyBorderToControl(child, showshadow);
                }
            }

        }
        public static void ApplyThemeOnImageControl(Control control, bool _applyonimage)
        {
            var ImageProperty = TypeDescriptor.GetProperties(control)["LogoImage"];
            if (ImageProperty != null && ImageProperty.PropertyType == typeof(string))
            {
                var ApplyThemeOnImage = TypeDescriptor.GetProperties(control)["ApplyThemeOnImage"];
                if (ApplyThemeOnImage != null && ApplyThemeOnImage.PropertyType == typeof(bool))
                {
                    ApplyThemeOnImage.SetValue(control, _applyonimage);
                }
            }
        }
        // Recursively apply the theme to all controls on the form and child containers
        public static bool HasApplyThemeToChildsProperty(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ApplyThemeToChilds"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(bool))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool HasThemeProperty(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        // Apply the theme to a single control and all its children recursively
        public static void SetThemePropertyinControl(Control control, EnumBeepThemes theme)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control), "The control parameter cannot be null.");
            }
            try
            {
                // Check if the control itself has a "Theme" property
                var themeProperty = control.GetType().GetProperty("Theme");
                if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
                {
                    // Set the "Theme" property on the control
                    themeProperty.SetValue(control, theme);
                   MiscFunctions.SendLog($"Theme property set on control: {control.Name}");
                    return; // Exit after setting the property
                }
                // Check if the control has a "Theme" property in its components
                var themePropertyInComponents = control.GetType().GetProperty("Theme", BindingFlags.Instance | BindingFlags.NonPublic);
                if (themePropertyInComponents != null && themePropertyInComponents.PropertyType == typeof(EnumBeepThemes))
                {
                    // Set the "Theme" property on the control's components
                    themePropertyInComponents.SetValue(control, theme);
                   MiscFunctions.SendLog($"Theme property set on control components: {control.Name}");
                    return; // Exit after setting the property
                }
               MiscFunctions.SendLog("No 'Theme' property found on the control or its components.");
            }
            catch (Exception ex)
            {
               MiscFunctions.SendLog($"Error setting theme property: {ex.Message}");
            }
        }
        public static void SetThemePropertyinControl(Control control, EnumBeepThemes theme, string propertyName)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control), "The control parameter cannot be null.");
            }
            try
            {
                // Check if the control itself has a "Theme" property
                var themeProperty = control.GetType().GetProperty(propertyName);
                if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
                {
                    // Set the "Theme" property on the control
                    themeProperty.SetValue(control, theme);
                   MiscFunctions.SendLog($"Theme property set on control: {control.Name}");
                    return; // Exit after setting the property
                }
                // Check if the control has a "Theme" property in its components
                var themePropertyInComponents = control.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (themePropertyInComponents != null && themePropertyInComponents.PropertyType == typeof(EnumBeepThemes))
                {
                    // Set the "Theme" property on the control's components
                    themePropertyInComponents.SetValue(control, theme);
                   MiscFunctions.SendLog($"Theme property set on control components: {control.Name}");
                    return; // Exit after setting the property
                }
               MiscFunctions.SendLog("No 'Theme' property found on the control or its components.");
            }
            catch (Exception ex)
            {
               MiscFunctions.SendLog($"Error setting theme property: {ex.Message}");
            }
        }

    }
}
