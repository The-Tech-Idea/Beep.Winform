using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(32,32,32);
            this.CardBackColor = Color.FromArgb(245,246,248);
            this.CardTitleForeColor = Color.FromArgb(32,32,32);
            this.CardSubTitleForeColor = Color.FromArgb(32,32,32);
            this.CardrGradiantStartColor = Color.FromArgb(245,246,248);
            this.CardGradiantEndColor = Color.FromArgb(245,246,248);
            this.CardGradiantMiddleColor = Color.FromArgb(245,246,248);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}