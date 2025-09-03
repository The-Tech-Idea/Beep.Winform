using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public static class RibbonThemeMapper
    {
        public static RibbonTheme Map(IBeepTheme? theme, bool darkMode)
        {
            var t = new RibbonTheme();
            if (theme == null)
            {
                if (darkMode)
                {
                    ApplyDark(t);
                }
                return t;
            }

            if (darkMode)
            {
                ApplyDark(t);
                // keep text from theme if available
                t.Text = theme.AppBarTitleForeColor;
                return t;
            }

            // Light mapping from Beep theme
            t.Background = theme.AppBarBackColor;
            t.TabActiveBack = theme.ButtonBackColor;
            t.TabInactiveBack = ControlPaint.Light(theme.AppBarBackColor, .1f);
            t.TabBorder = theme.BorderColor;
            t.GroupBack = ControlPaint.Light(theme.AppBarBackColor, .15f);
            t.GroupBorder = ControlPaint.Dark(theme.BorderColor);
            t.Text = theme.AppBarTitleForeColor;
            t.QuickAccessBack = ControlPaint.Light(theme.AppBarBackColor, .2f);
            t.QuickAccessBorder = ControlPaint.Dark(theme.BorderColor);
            return t;
        }

        private static void ApplyDark(RibbonTheme t)
        {
            t.Background = Color.FromArgb(32, 32, 32);
            t.TabActiveBack = Color.FromArgb(45, 45, 45);
            t.TabInactiveBack = Color.FromArgb(38, 38, 38);
            t.TabBorder = Color.FromArgb(64, 64, 64);
            t.GroupBack = Color.FromArgb(42, 42, 42);
            t.GroupBorder = Color.FromArgb(70, 70, 70);
            t.Text = Color.WhiteSmoke;
            t.QuickAccessBack = Color.FromArgb(50, 50, 50);
            t.QuickAccessBorder = Color.FromArgb(80, 80, 80);
        }
    }
}
