using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(192,202,245);
            this.BackColor = Color.FromArgb(26,27,38);
            this.PanelBackColor = Color.FromArgb(26,27,38);
            this.PanelGradiantStartColor = Color.FromArgb(36,40,59);
            this.PanelGradiantEndColor = Color.FromArgb(36,40,59);
            this.PanelGradiantMiddleColor = Color.FromArgb(26,27,38);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(86,95,137);
            this.ActiveBorderColor = Color.FromArgb(86,95,137);
            this.InactiveBorderColor = Color.FromArgb(86,95,137);
            this.PrimaryColor = Color.FromArgb(122,162,247);
            this.SecondaryColor = Color.FromArgb(42,195,222);
            this.AccentColor = Color.FromArgb(187,154,247);
            this.BackgroundColor = Color.FromArgb(26,27,38);
            this.SurfaceColor = Color.FromArgb(36,40,59);
            this.ErrorColor = Color.FromArgb(247,118,142);
            this.WarningColor = Color.FromArgb(224,175,104);
            this.SuccessColor = Color.FromArgb(158,206,106);
            this.OnPrimaryColor = Color.FromArgb(26, 27, 38);
            this.OnBackgroundColor = Color.FromArgb(192,202,245);
            this.FocusIndicatorColor = Color.FromArgb(122,162,247);
        }
    }
}