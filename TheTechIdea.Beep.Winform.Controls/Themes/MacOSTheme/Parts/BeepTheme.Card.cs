using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(28,28,30);
            this.CardBackColor = Color.FromArgb(250,250,252);
            this.CardTitleForeColor = Color.FromArgb(28,28,30);
            this.CardSubTitleForeColor = Color.FromArgb(28,28,30);
            this.CardrGradiantStartColor = Color.FromArgb(250,250,252);
            this.CardGradiantEndColor = Color.FromArgb(250,250,252);
            this.CardGradiantMiddleColor = Color.FromArgb(250,250,252);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}