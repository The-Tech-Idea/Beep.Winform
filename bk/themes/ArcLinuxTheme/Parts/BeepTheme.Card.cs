using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(43,45,48);
            this.CardBackColor = Color.FromArgb(245,246,247);
            this.CardTitleForeColor = Color.FromArgb(43,45,48);
            this.CardSubTitleForeColor = Color.FromArgb(43,45,48);
            this.CardrGradiantStartColor = Color.FromArgb(245,246,247);
            this.CardGradiantEndColor = Color.FromArgb(245,246,247);
            this.CardGradiantMiddleColor = Color.FromArgb(245,246,247);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}