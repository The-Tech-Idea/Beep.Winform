using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyButtons()
        {
            // GNOME buttons - clean modern aesthetic
            // Default: White background with dark text
            this.ButtonBackColor = SurfaceColor;  // White
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = BorderColor;  // Light gray
            
            // Hover: Light gray background
            this.ButtonHoverBackColor = Color.FromArgb(240, 240, 240);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Darker border
            
            // Selected: Medium gray background
            this.ButtonSelectedBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Darker gray
            this.ButtonSelectedHoverBackColor = Color.FromArgb(200, 200, 200);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 250, 250);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
