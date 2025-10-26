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
            this.ButtonBackColor = BackgroundColor;
            this.ButtonForeColor = ForeColor;
            this.ButtonBorderColor = BorderColor;

            this.ButtonHoverBackColor = BackgroundColor;
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = BorderColor;

            this.ButtonPressedBackColor = BackgroundColor;
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;

            this.ButtonSelectedBackColor = BackgroundColor;
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = BorderColor;
            this.ButtonSelectedHoverBackColor = BackgroundColor;
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;

            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnBackgroundColor;
            this.ButtonErrorBorderColor = BorderColor;
        }
    }
}

