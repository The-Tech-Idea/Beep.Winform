using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Configuration options for layout templates.
    /// Provides theme, styling, spacing, and customization settings for layout helpers.
    /// </summary>
    public class LayoutOptions
    {
        /// <summary>
        /// Gets or sets the Beep control style to apply to created controls.
        /// </summary>
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Modern;

        /// <summary>
        /// Gets or sets the theme to use for styling. If null, uses the current theme from BeepThemesManager.
        /// </summary>
        public IBeepTheme Theme { get; set; }

        /// <summary>
        /// Gets or sets whether to use theme colors. Default is true.
        /// </summary>
        public bool UseThemeColors { get; set; } = true;

        /// <summary>
        /// Gets or sets the spacing between controls in pixels. Default is 5.
        /// </summary>
        public int Spacing { get; set; } = 5;

        /// <summary>
        /// Gets or sets the padding around the layout in pixels. Default is 8.
        /// </summary>
        public Padding Padding { get; set; } = new Padding(8);

        /// <summary>
        /// Gets or sets the background color override. If empty, uses theme background color.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground/text color override. If empty, uses theme foreground color.
        /// </summary>
        public Color ForegroundColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the border color override. If empty, uses theme border color.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the default font to use for labels and text. If null, uses theme font.
        /// </summary>
        public Font DefaultFont { get; set; }

        /// <summary>
        /// Gets or sets the default font size. Used when DefaultFont is null.
        /// </summary>
        public float DefaultFontSize { get; set; } = 9f;

        /// <summary>
        /// Gets or sets whether to show borders on panels and containers. Default is true.
        /// </summary>
        public bool ShowBorders { get; set; } = true;

        /// <summary>
        /// Gets or sets the border width in pixels. Default is 1.
        /// </summary>
        public int BorderWidth { get; set; } = 1;

        /// <summary>
        /// Gets or sets the corner radius for rounded borders. Default is 4.
        /// </summary>
        public int CornerRadius { get; set; } = 4;

        /// <summary>
        /// Creates a default LayoutOptions instance with sensible defaults.
        /// </summary>
        public static LayoutOptions Default => new LayoutOptions();

        /// <summary>
        /// Creates a LayoutOptions instance from a BaseControl, inheriting its theme and style settings.
        /// </summary>
        /// <param name="control">The BaseControl to inherit settings from.</param>
        /// <returns>A LayoutOptions instance configured from the control.</returns>
        public static LayoutOptions FromControl(Base.BaseControl control)
        {
            if (control == null)
                return Default;

            // Get theme from theme name using BeepThemesManager
            IBeepTheme theme = null;
            if (!string.IsNullOrEmpty(control.Theme))
            {
                theme = BeepThemesManager.GetTheme(control.Theme);
            }
            if (theme == null)
            {
                theme = BeepThemesManager.GetDefaultTheme();
            }

            return new LayoutOptions
            {
                ControlStyle = control.ControlStyle,
                Theme = theme,
                UseThemeColors = control.UseThemeColors,
                Spacing = 5,
                Padding = new Padding(8),
                ShowBorders = true,
                BorderWidth = 1,
                CornerRadius = 4
            };
        }
    }
}
