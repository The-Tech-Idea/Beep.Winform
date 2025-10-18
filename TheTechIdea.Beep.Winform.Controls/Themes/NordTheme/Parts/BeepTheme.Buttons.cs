using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(46,52,64);
            this.ButtonHoverForeColor = Color.FromArgb(216,222,233);
            this.ButtonHoverBorderColor = Color.FromArgb(76,86,106);
            this.ButtonSelectedBorderColor = Color.FromArgb(76,86,106);
            this.ButtonSelectedBackColor = Color.FromArgb(46,52,64);
            this.ButtonSelectedForeColor = Color.FromArgb(216,222,233);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(46,52,64);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(216,222,233);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(76,86,106);
            this.ButtonBackColor = Color.FromArgb(46,52,64);
            this.ButtonForeColor = Color.FromArgb(216,222,233);
            this.ButtonBorderColor = Color.FromArgb(76,86,106);
            this.ButtonErrorBackColor = Color.FromArgb(46,52,64);
            this.ButtonErrorForeColor = Color.FromArgb(216,222,233);
            this.ButtonErrorBorderColor = Color.FromArgb(76,86,106);
            this.ButtonPressedBackColor = Color.FromArgb(46,52,64);
            this.ButtonPressedForeColor = Color.FromArgb(216,222,233);
            this.ButtonPressedBorderColor = Color.FromArgb(76,86,106);
        }
    }
}