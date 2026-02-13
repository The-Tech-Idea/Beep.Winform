using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyButtons()
        {
            // NeoMorphism buttons - soft neomorphic effect
            // Default: soft light-plate distinct from background/surface
            this.ButtonBackColor = Color.FromArgb(232, 236, 244);
            this.ButtonForeColor = ForeColor;
            this.ButtonBorderColor = BorderColor;

            // Hover: slightly cooler tint
            this.ButtonHoverBackColor = Color.FromArgb(220, 226, 240);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;

            this.ButtonSelectedBackColor = PrimaryColor;
            this.ButtonSelectedForeColor = OnPrimaryColor;
            this.ButtonSelectedBorderColor = PrimaryColor;

            this.ButtonSelectedHoverBackColor = AccentColor;
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = AccentColor;

            // Pressed: darkest neutral in the set
            this.ButtonPressedBackColor = Color.FromArgb(206, 212, 226);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;

            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}











