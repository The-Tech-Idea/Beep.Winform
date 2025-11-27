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
            this.ButtonBackColor = SurfaceColor;
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

            this.ButtonPressedBackColor = SurfaceColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;

            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
