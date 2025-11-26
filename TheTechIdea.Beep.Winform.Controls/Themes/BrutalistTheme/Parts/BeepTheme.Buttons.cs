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
            // Default: White background with black text
            this.ButtonBackColor = SurfaceColor;  // White
            this.ButtonForeColor = ForeColor;  // Black
            this.ButtonBorderColor = BorderColor;  // Black border
            
            // Hover: Slightly darker background for visibility
            this.ButtonHoverBackColor = Color.FromArgb(240, 240, 240);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = BorderColor;
            
            // Selected: Medium gray background for distinction
            this.ButtonSelectedBackColor = Color.FromArgb(200, 200, 200);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = BorderColor;
            
            // Selected hover: Slightly darker gray
            this.ButtonSelectedHoverBackColor = Color.FromArgb(180, 180, 180);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = BorderColor;
            
            // Pressed: Darker for feedback
            this.ButtonPressedBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = BorderColor;
        }
    }
}
