using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(40,42,54);
            this.ButtonHoverForeColor = Color.FromArgb(248,248,242);
            this.ButtonHoverBorderColor = Color.FromArgb(98,114,164);
            this.ButtonSelectedBorderColor = Color.FromArgb(98,114,164);
            this.ButtonSelectedBackColor = Color.FromArgb(40,42,54);
            this.ButtonSelectedForeColor = Color.FromArgb(248,248,242);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(40,42,54);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(248,248,242);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(98,114,164);
            this.ButtonBackColor = Color.FromArgb(40,42,54);
            this.ButtonForeColor = Color.FromArgb(248,248,242);
            this.ButtonBorderColor = Color.FromArgb(98,114,164);
            this.ButtonErrorBackColor = Color.FromArgb(40,42,54);
            this.ButtonErrorForeColor = Color.FromArgb(248,248,242);
            this.ButtonErrorBorderColor = Color.FromArgb(98,114,164);
            this.ButtonPressedBackColor = Color.FromArgb(40,42,54);
            this.ButtonPressedForeColor = Color.FromArgb(248,248,242);
            this.ButtonPressedBorderColor = Color.FromArgb(98,114,164);
        }
    }
}