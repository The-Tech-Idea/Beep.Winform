using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(147,161,161);
            this.CardBackColor = Color.FromArgb(0,43,54);
            this.CardTitleForeColor = Color.FromArgb(147,161,161);
            this.CardSubTitleForeColor = Color.FromArgb(147,161,161);
            this.CardrGradiantStartColor = Color.FromArgb(0,43,54);
            this.CardGradiantEndColor = Color.FromArgb(0,43,54);
            this.CardGradiantMiddleColor = Color.FromArgb(0,43,54);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}