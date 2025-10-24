using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(216,222,233);
            this.CardBackColor = Color.FromArgb(46,52,64);
            this.CardTitleForeColor = Color.FromArgb(216,222,233);
            this.CardSubTitleForeColor = Color.FromArgb(216,222,233);
            this.CardrGradiantStartColor = Color.FromArgb(46,52,64);
            this.CardGradiantEndColor = Color.FromArgb(46,52,64);
            this.CardGradiantMiddleColor = Color.FromArgb(46,52,64);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}