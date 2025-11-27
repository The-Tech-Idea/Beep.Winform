using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyButtons()
        {
            // Cyberpunk buttons - neon aesthetic
            // Default: Dark with cyan borders
            this.ButtonBackColor = BackgroundColor;  // Dark background
            this.ButtonForeColor = ForeColor;  // Cyan text
            this.ButtonBorderColor = BorderColor;  // Cyan border
            
            // Hover: Glowing effect
            this.ButtonHoverBackColor = PanelGradiantMiddleColor;  // Dark cyan tint
            this.ButtonHoverForeColor = PrimaryColor;  // Bright cyan
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Bright border
            
            // Selected: Visible state
            this.ButtonSelectedBackColor = PanelBackColor;  // Medium cyan
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = BorderColor;
            
            // Selected hover: Brighter
            this.ButtonSelectedHoverBackColor = PanelGradiantMiddleColor;
            this.ButtonSelectedHoverForeColor = PrimaryColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = PanelGradiantStartColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red neon
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;  // Light on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
