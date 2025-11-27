using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyButtons()
        {
            // Cartoon buttons - playful with palette tokens
            this.ButtonBackColor = SurfaceColor;
            this.ButtonForeColor = ForeColor;
            this.ButtonBorderColor = BorderColor;

            this.ButtonHoverBackColor = SecondaryColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;

            this.ButtonSelectedBackColor = SecondaryColor;
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;

            this.ButtonSelectedHoverBackColor = AccentColor;
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;

            this.ButtonPressedBackColor = AccentColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;

            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;
            this.ButtonErrorBorderColor = BorderColor;
        }
    }
}
