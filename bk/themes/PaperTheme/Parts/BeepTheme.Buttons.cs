using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(250,250,250);
            this.ButtonHoverForeColor = Color.FromArgb(33,33,33);
            this.ButtonHoverBorderColor = Color.FromArgb(224,224,224);
            this.ButtonSelectedBorderColor = Color.FromArgb(224,224,224);
            this.ButtonSelectedBackColor = Color.FromArgb(250,250,250);
            this.ButtonSelectedForeColor = Color.FromArgb(33,33,33);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(250,250,250);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(33,33,33);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(224,224,224);
            this.ButtonBackColor = Color.FromArgb(250,250,250);
            this.ButtonForeColor = Color.FromArgb(33,33,33);
            this.ButtonBorderColor = Color.FromArgb(224,224,224);
            this.ButtonErrorBackColor = Color.FromArgb(250,250,250);
            this.ButtonErrorForeColor = Color.FromArgb(33,33,33);
            this.ButtonErrorBorderColor = Color.FromArgb(224,224,224);
            this.ButtonPressedBackColor = Color.FromArgb(250,250,250);
            this.ButtonPressedForeColor = Color.FromArgb(33,33,33);
            this.ButtonPressedBorderColor = Color.FromArgb(224,224,224);
        }
    }
}