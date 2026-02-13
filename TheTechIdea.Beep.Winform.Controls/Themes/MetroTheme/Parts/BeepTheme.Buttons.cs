using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyButtons()
        {
            // Metro buttons - Windows Metro design
            // Default: Light gray background with dark text
            this.ButtonBackColor = Color.FromArgb(240, 240, 240);
            this.ButtonForeColor = ForeColor;
            this.ButtonBorderColor = BorderColor;
            this.ButtonHoverBackColor = SecondaryColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;
            this.ButtonSelectedBackColor = PrimaryColor;
            this.ButtonSelectedForeColor = OnPrimaryColor;
            this.ButtonSelectedBorderColor = PrimaryColor;
            this.ButtonSelectedHoverBackColor = PrimaryColor;
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = PrimaryColor;
            this.ButtonPressedBackColor = ThemeUtil.Darken(SurfaceColor, 0.08);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}










