using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{

    public static class BeepGlobalThemeManager
    {
        public static event Action<EnumBeepThemes> OnThemeChanged;
        private static EnumBeepThemes _globalTheme = EnumBeepThemes.DefaultTheme;

        public static EnumBeepThemes GlobalTheme
        {
            get => _globalTheme;
            set
            {
                _globalTheme = value;
                OnThemeChanged?.Invoke(_globalTheme);
            }
        }
        public static void ApplyThemeToControl(Control control, EnumBeepThemes _theme,bool applytoimage)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
            {
                themeProperty.SetValue(control, _theme);
            }
            var ImageProperty = TypeDescriptor.GetProperties(control)["ImagePath"];
            if (ImageProperty != null && ImageProperty.PropertyType == typeof(string))
            {
                var ApplyThemeOnImage = TypeDescriptor.GetProperties(control)["ApplyThemeOnImage"];
                if (ApplyThemeOnImage != null && ApplyThemeOnImage.PropertyType == typeof(bool))
                {
                    ApplyThemeOnImage.SetValue(control, applytoimage);
                }
            }
        }
        public static void ApplyShadowToControl(Control control,  bool showshadow)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowShadow"];
            if (themeProperty != null )
            {
                themeProperty.SetValue(control, showshadow);
            }
          
        }
        //IsRounded
        public static void ApplyRoundedToControl(Control control, bool isrounded)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["IsRounded"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, isrounded);
            }

        }
        public static void ApplyBorderToControl(Control control, bool showborder)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowAllBorders"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, showborder);
            }

        }
    }
}
