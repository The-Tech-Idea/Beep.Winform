using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(250,250,251);
            this.ButtonHoverForeColor = Color.FromArgb(31,41,55);
            this.ButtonHoverBorderColor = Color.FromArgb(229,231,235);
            this.ButtonSelectedBorderColor = Color.FromArgb(229,231,235);
            this.ButtonSelectedBackColor = Color.FromArgb(250,250,251);
            this.ButtonSelectedForeColor = Color.FromArgb(31,41,55);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(250,250,251);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(31,41,55);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(229,231,235);
            this.ButtonBackColor = Color.FromArgb(250,250,251);
            this.ButtonForeColor = Color.FromArgb(31,41,55);
            this.ButtonBorderColor = Color.FromArgb(229,231,235);
            this.ButtonErrorBackColor = Color.FromArgb(220,38,38);
            this.ButtonErrorForeColor = Color.FromArgb(31,41,55);
            this.ButtonErrorBorderColor = Color.FromArgb(229,231,235);
            this.ButtonPressedBackColor = Color.FromArgb(250,250,251);
            this.ButtonPressedForeColor = Color.FromArgb(31,41,55);
            this.ButtonPressedBorderColor = Color.FromArgb(229,231,235);
        }
    }
}