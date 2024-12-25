using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepPopupForm : BeepiForm
    {
        public BeepPopupForm()
        {
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            InPopMode= true;
            _borderThickness = 2;
        }
        public int BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; }

        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            Console.WriteLine($"2 Control Added {e.Control.Text}");
            AdjustControls();
        }

    }

}
