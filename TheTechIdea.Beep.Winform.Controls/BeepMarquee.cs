using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepMarquee:BeepControl    
    {
        private Dictionary<string, string> _MarqueeProperties = new Dictionary<string, string>();
        private Dictionary<string,IBeepUIComponent> _marqueeComponents = new Dictionary<string, IBeepUIComponent>();
        public BeepMarquee()
        {
            
        }
    }
}
