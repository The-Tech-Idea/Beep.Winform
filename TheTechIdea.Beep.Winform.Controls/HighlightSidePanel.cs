using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class HighlightSidePanel : Panel
    {
        public HighlightSidePanel()
        {
            Width = 5; // Set width of highlight panel
            Visible = false;
        }

        public void ApplyTheme(BeepTheme theme)
        {
            BackColor = theme.AccentColor; // Theme accent color for the highlight
        }

        public void Highlight(Control button)
        {
            Height = button.Height;
            Top = button.Top;
            Visible = true;
        }

        public void RemoveHighlight()
        {
            Visible = false;
        }
    }

}
