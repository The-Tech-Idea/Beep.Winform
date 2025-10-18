using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(171,178,191);
            this.CardBackColor = Color.FromArgb(40,44,52);
            this.CardTitleForeColor = Color.FromArgb(171,178,191);
            this.CardSubTitleForeColor = Color.FromArgb(171,178,191);
            this.CardrGradiantStartColor = Color.FromArgb(40,44,52);
            this.CardGradiantEndColor = Color.FromArgb(40,44,52);
            this.CardGradiantMiddleColor = Color.FromArgb(40,44,52);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}