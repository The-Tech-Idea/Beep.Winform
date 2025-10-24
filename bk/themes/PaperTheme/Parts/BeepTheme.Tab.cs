using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(250,250,250);
            this.TabForeColor = Color.FromArgb(33,33,33);
            this.TabBorderColor = Color.FromArgb(224,224,224);
            this.TabHoverBackColor = Color.FromArgb(250,250,250);
            this.TabHoverForeColor = Color.FromArgb(33,33,33);
            this.TabSelectedBackColor = Color.FromArgb(250,250,250);
            this.TabSelectedForeColor = Color.FromArgb(33,33,33);
            this.TabSelectedBorderColor = Color.FromArgb(224,224,224);
            this.TabHoverBorderColor = Color.FromArgb(224,224,224);
        }
    }
}