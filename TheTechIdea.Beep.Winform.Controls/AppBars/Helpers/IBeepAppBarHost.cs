using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Interface that BeepAppBar must implement to work with helper classes
    /// </summary>
    public interface IBeepAppBarHost
    {
        #region "Core Properties"
        Control AsControl { get; }
        IBeepTheme CurrentTheme { get; }
        string Theme { get; set; }
        Rectangle DrawingRect { get; }
        Size Size { get; }
        bool DesignMode { get; } // Use a different property name to avoid conflict with Component.DesignMode
        #endregion

        #region "AppBar Specific Properties"
        string Title { get; set; }
        bool ShowTitle { get; set; }
        bool ShowLogo { get; set; }
        bool ShowSearchBox { get; set; }
        bool ShowProfileIcon { get; set; }
        bool ShowNotificationIcon { get; set; }
        bool ShowThemeIcon { get; set; }
        bool ShowCloseIcon { get; set; }
        bool ShowMaximizeIcon { get; set; }
        bool ShowMinimizeIcon { get; set; }
        string LogoImage { get; set; }
        Size LogoSize { get; set; }
        Font TitleFont { get; set; }
        Font TextFont { get; set; }
        int SearchBoxWidth { get; }
        int TitleLabelWidth { get; }
        #endregion

        #region "Methods"
        void Invalidate();
        void InvalidateLayout();
        int ScaleValue(int value);
        Size ScaleSize(Size size);
        #endregion
    }
}