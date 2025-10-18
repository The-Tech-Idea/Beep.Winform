using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(147,161,161);
            this.BackColor = Color.FromArgb(0,43,54);
            this.PanelBackColor = Color.FromArgb(0,43,54);
            this.PanelGradiantStartColor = Color.FromArgb(0,43,54);
            this.PanelGradiantEndColor = Color.FromArgb(0,43,54);
            this.PanelGradiantMiddleColor = Color.FromArgb(7,54,66);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(88,110,117);
            this.ActiveBorderColor = Color.FromArgb(88,110,117);
            this.InactiveBorderColor = Color.FromArgb(88,110,117);
            this.PrimaryColor = Color.FromArgb(38,139,210);
            this.SecondaryColor = Color.FromArgb(42,161,152);
            this.AccentColor = Color.FromArgb(108,113,196);
            this.BackgroundColor = Color.FromArgb(0,43,54);
            this.SurfaceColor = Color.FromArgb(7,54,66);
            this.ErrorColor = Color.FromArgb(220,50,47);
            this.WarningColor = Color.FromArgb(181,137,0);
            this.SuccessColor = Color.FromArgb(133,153,0);
            this.OnPrimaryColor = Color.FromArgb(0, 43, 54);
            this.OnBackgroundColor = Color.FromArgb(147,161,161);
            this.FocusIndicatorColor = Color.FromArgb(42,161,152);
        }
    }
}