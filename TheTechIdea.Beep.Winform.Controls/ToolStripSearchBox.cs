using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{

    [ToolboxItem(true)]

    [ToolboxBitmap(typeof(ToolStripSearchBox))] // Optional//"Resources.BeepButtonIcon.bmp"
    [Category("Beep Controls")]
    public class ToolStripSearchBox : ToolStripTextBox
    {
        public ToolStripSearchBox()
        {
            // Adjust these values as needed for proper alignment
            TextBox.Padding = new Padding(0, 0, 20, 0);
        }

        private Image searchIcon;

        public Image SearchIcon
        {
            get { return searchIcon; }
            set { searchIcon = value; Invalidate(); } // Invalidate to redraw
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (searchIcon != null)
            {
                // Calculate the position to draw the icon (e.g., right aligned)
                int iconX = Width - searchIcon.Width - 4;
                int iconY = (Height - searchIcon.Height) / 2;
                e.Graphics.DrawImage(searchIcon, iconX, iconY);
            }
        }
    }
}
