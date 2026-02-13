using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyButtons()
        {
            // Ubuntu buttons - Ubuntu Linux desktop aesthetic
            this.ButtonBackColor = Color.FromArgb(232, 228, 236);
            this.ButtonForeColor = ForeColor;
            this.ButtonBorderColor = BorderColor;

            this.ButtonHoverBackColor = SecondaryColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;

            this.ButtonSelectedBackColor = PrimaryColor;
            this.ButtonSelectedForeColor = OnPrimaryColor;
            this.ButtonSelectedBorderColor = PrimaryColor;

            this.ButtonSelectedHoverBackColor = AccentColor;
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = AccentColor;

            this.ButtonPressedBackColor = ThemeUtil.Darken(SurfaceColor, 0.08);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;

            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}











