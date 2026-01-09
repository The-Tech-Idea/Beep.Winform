using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Images.Helpers
{
    /// <summary>
    /// Centralized helper for managing image theme colors
    /// Integrates with ApplyTheme() pattern from BaseControl
    /// Maps ImageEmbededin to theme colors
    /// </summary>
    public static class ImageThemeHelpers
    {
        /// <summary>
        /// Gets the fill color for image based on ImageEmbededin
        /// </summary>
        public static Color GetImageFillColor(
            IBeepTheme theme,
            ImageEmbededin imageEmbededin,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (theme != null)
            {
                return imageEmbededin switch
                {
                    ImageEmbededin.TabPage => theme.TabForeColor != Color.Empty ? theme.TabForeColor : Color.Black,
                    ImageEmbededin.AppBar => theme.AppBarForeColor != Color.Empty ? theme.AppBarForeColor : Color.Black,
                    ImageEmbededin.Menu or ImageEmbededin.MenuBar => theme.MenuForeColor != Color.Empty ? theme.MenuForeColor : Color.Black,
                    ImageEmbededin.TreeView => theme.TreeForeColor != Color.Empty ? theme.TreeForeColor : Color.Black,
                    ImageEmbededin.SideBar => theme.SideMenuForeColor != Color.Empty ? theme.SideMenuForeColor : Color.Black,
                    ImageEmbededin.ListBox or ImageEmbededin.Form or ImageEmbededin.ListView => theme.ListForeColor != Color.Empty ? theme.ListForeColor : Color.Black,
                    ImageEmbededin.Label => theme.LabelForeColor != Color.Empty ? theme.LabelForeColor : Color.Black,
                    ImageEmbededin.TextBox => theme.TextBoxForeColor != Color.Empty ? theme.TextBoxForeColor : Color.Black,
                    ImageEmbededin.ComboBox => theme.ComboBoxForeColor != Color.Empty ? theme.ComboBoxForeColor : Color.Black,
                    ImageEmbededin.DataGridView => theme.GridHeaderForeColor != Color.Empty ? theme.GridHeaderForeColor : Color.Black,
                    ImageEmbededin.Button or _ => Color.Black
                };
            }

            return Color.Black;
        }

        /// <summary>
        /// Gets the stroke color for image based on ImageEmbededin
        /// </summary>
        public static Color GetImageStrokeColor(
            IBeepTheme theme,
            ImageEmbededin imageEmbededin,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            // Stroke color typically matches fill color
            return GetImageFillColor(theme, imageEmbededin, customColor);
        }

        /// <summary>
        /// Gets the background color for image based on ImageEmbededin
        /// </summary>
        public static Color GetImageBackgroundColor(
            IBeepTheme theme,
            ImageEmbededin imageEmbededin,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (theme != null)
            {
                return imageEmbededin switch
                {
                    ImageEmbededin.TabPage => theme.TabBackColor != Color.Empty ? theme.TabBackColor : Color.Transparent,
                    ImageEmbededin.AppBar => theme.AppBarBackColor != Color.Empty ? theme.AppBarBackColor : Color.Transparent,
                    ImageEmbededin.Menu or ImageEmbededin.MenuBar => theme.MenuBackColor != Color.Empty ? theme.MenuBackColor : Color.Transparent,
                    ImageEmbededin.TreeView => theme.TreeBackColor != Color.Empty ? theme.TreeBackColor : Color.Transparent,
                    ImageEmbededin.SideBar => theme.SideMenuBackColor != Color.Empty ? theme.SideMenuBackColor : Color.Transparent,
                    ImageEmbededin.ListBox or ImageEmbededin.Form or ImageEmbededin.ListView => theme.ListBackColor != Color.Empty ? theme.ListBackColor : Color.Transparent,
                    ImageEmbededin.Label => theme.LabelBackColor != Color.Empty ? theme.LabelBackColor : Color.Transparent,
                    ImageEmbededin.TextBox => theme.TextBoxBackColor != Color.Empty ? theme.TextBoxBackColor : Color.Transparent,
                    ImageEmbededin.ComboBox => theme.ComboBoxBackColor != Color.Empty ? theme.ComboBoxBackColor : Color.Transparent,
                    ImageEmbededin.DataGridView => theme.GridHeaderBackColor != Color.Empty ? theme.GridHeaderBackColor : Color.Transparent,
                    ImageEmbededin.Button or _ => Color.Transparent
                };
            }

            return Color.Transparent;
        }
    }
}
