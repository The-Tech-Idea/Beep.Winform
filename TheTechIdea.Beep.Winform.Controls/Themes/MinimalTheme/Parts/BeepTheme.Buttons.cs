using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyButtons()
        {
            // Minimal buttons - clean, minimal aesthetic
            // Keep buttons neutral but clearly separated from white background/surface
            this.ButtonBackColor = AccentColor;
            this.ButtonForeColor = ForeColor;  // Dark grey
            this.ButtonBorderColor = Color.FromArgb(180, 180, 180);
            
            // Hover: clear elevation from default
            this.ButtonHoverBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = Color.FromArgb(140, 140, 140);
            
            // Selected: stronger neutral emphasis
            this.ButtonSelectedBackColor = Color.FromArgb(210, 210, 210);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Slightly darker
            this.ButtonSelectedHoverBackColor = Color.FromArgb(198, 198, 198);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: strongest neutral
            this.ButtonPressedBackColor = Color.FromArgb(186, 186, 186);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = Color.FromArgb(120, 120, 120);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}




