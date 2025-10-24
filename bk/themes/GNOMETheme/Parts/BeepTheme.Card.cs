using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(46,52,54);
            this.CardBackColor = Color.FromArgb(246,245,244);
            this.CardTitleForeColor = Color.FromArgb(46,52,54);
            this.CardSubTitleForeColor = Color.FromArgb(46,52,54);
            this.CardrGradiantStartColor = Color.FromArgb(246,245,244);
            this.CardGradiantEndColor = Color.FromArgb(246,245,244);
            this.CardGradiantMiddleColor = Color.FromArgb(246,245,244);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}