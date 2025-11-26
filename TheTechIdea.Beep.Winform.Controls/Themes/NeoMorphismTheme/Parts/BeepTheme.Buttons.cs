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
            // Default: Light gray background with dark text
            this.ButtonBackColor = BackgroundColor;  // Light gray-blue
            this.ButtonForeColor = ForeColor;  // Dark gray text
            this.ButtonBorderColor = BorderColor;  // Soft border
            
            // Hover: Slightly darker for recessed effect
            this.ButtonHoverBackColor = Color.FromArgb(228, 228, 233);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Blue border
            
            // Selected: Blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // Blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(90, 125, 255);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(90, 125, 255);
            
            // Pressed: Lighter for embossed effect
            this.ButtonPressedBackColor = Color.FromArgb(245, 245, 250);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
