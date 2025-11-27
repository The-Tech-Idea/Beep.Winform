using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyButtons()
        {
            // Brutalist buttons - bold, high-contrast
            // Default: Palette-driven
            this.ButtonBackColor = SurfaceColor;
            this.ButtonForeColor = ForeColor;
            this.ButtonBorderColor = BorderColor;

            // Hover: Use palette for hover state
            this.ButtonHoverBackColor = SurfaceColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = BorderColor;

            // Selected: Use secondary color for distinction
            this.ButtonSelectedBackColor = SecondaryColor;
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = BorderColor;

            // Selected hover: Use secondary color
            this.ButtonSelectedHoverBackColor = SecondaryColor;
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = BorderColor;

            // Pressed: Use primary color for feedback
            this.ButtonPressedBackColor = PrimaryColor;
            this.ButtonPressedForeColor = OnPrimaryColor;
            this.ButtonPressedBorderColor = BorderColor;

            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;
            this.ButtonErrorBorderColor = BorderColor;
        }
    }
}
