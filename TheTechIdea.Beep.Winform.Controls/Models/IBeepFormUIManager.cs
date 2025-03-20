using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Models
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

        void ApplyThemeToControl(Control control, EnumBeepThemes _theme, bool applytoimage);
        void FindBeepSideMenu();
        void ShowTitle(bool show);
    }
}