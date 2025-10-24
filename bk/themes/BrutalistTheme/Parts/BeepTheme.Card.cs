using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(20,20,20);
            this.CardBackColor = Color.FromArgb(250,250,250);
            this.CardTitleForeColor = Color.FromArgb(20,20,20);
            this.CardSubTitleForeColor = Color.FromArgb(20,20,20);
            this.CardrGradiantStartColor = Color.FromArgb(250,250,250);
            this.CardGradiantEndColor = Color.FromArgb(250,250,250);
            this.CardGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
    }
}