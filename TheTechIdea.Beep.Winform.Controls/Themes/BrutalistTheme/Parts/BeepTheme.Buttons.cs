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
            this.ButtonBackColor = Color.FromArgb(255, 255, 255);  // White
            this.ButtonForeColor = Color.FromArgb(0, 0, 0);  // Black
            this.ButtonBorderColor = Color.FromArgb(0, 0, 0);  // Black border
            
            // Hover: Slightly darker background for visibility
            this.ButtonHoverBackColor = Color.FromArgb(240, 240, 240);
            this.ButtonHoverForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonHoverBorderColor = Color.FromArgb(0, 0, 0);
            
            // Selected: Medium gray background for distinction
            this.ButtonSelectedBackColor = Color.FromArgb(200, 200, 200);
            this.ButtonSelectedForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonSelectedBorderColor = Color.FromArgb(0, 0, 0);
            
            // Selected hover: Slightly darker gray
            this.ButtonSelectedHoverBackColor = Color.FromArgb(180, 180, 180);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(0, 0, 0);
            
            // Pressed: Darker for feedback
            this.ButtonPressedBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonPressedForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonPressedBorderColor = Color.FromArgb(0, 0, 0);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(220, 0, 0);
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(0, 0, 0);
        }
    }
}