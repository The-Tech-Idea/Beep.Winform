using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(31,41,55);
            this.CardBackColor = Color.FromArgb(255,255,255);
            this.CardTitleForeColor = Color.FromArgb(31,41,55);
            this.CardSubTitleForeColor = Color.FromArgb(31,41,55);
            this.CardrGradiantStartColor = Color.FromArgb(255,255,255);
            this.CardGradiantEndColor = Color.FromArgb(255,255,255);
            this.CardGradiantMiddleColor = Color.FromArgb(255,255,255);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}