using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public interface IBeepFormUIManager
    {
        bool ApplyThemeOnImage { get; set; }
      
        bool IsRounded { get; set; }
        string LogoImage { get; set; }
        bool ShowBorder { get; set; }
        bool ShowShadow { get; set; }
        ISite Site { get; set; }
        EnumBeepThemes Theme { get; set; }
        string Title { get; set; }

        event Action<EnumBeepThemes> OnThemeChanged;

        void ApplyBorderToControl(Control control, bool showborder);
        void ApplyRoundedToControl(Control control, bool isrounded);
        void ApplyShadowToControl(Control control, bool showshadow);
        void ApplyThemeOnImageControl(Control control, bool _applyonimage);
        void ApplyThemeToControl(Control control, EnumBeepThemes _theme, bool applytoimage);
        void FindBeepSideMenu();
        bool GetPropertyFromControl(Control control, string PropertyName);
       
        void ShowTitle(bool show);
    }
}