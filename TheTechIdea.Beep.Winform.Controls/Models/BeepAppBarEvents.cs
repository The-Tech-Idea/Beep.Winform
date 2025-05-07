using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public class BeepAppBarEventsArgs : EventArgs
    {
        public string ButtonName { get; set; }
        public BeepButton Beepbutton { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public string Selectedstring { get; set; }
        public SimpleItem SelectedItem { get; set; }

        public BeepAppBarEventsArgs(string buttonname)
        {
            ButtonName = buttonname;
        }
        public BeepAppBarEventsArgs(string buttonname, BeepButton button)
        {
            Beepbutton = button;
            ButtonName = buttonname;
        }
    }
}
