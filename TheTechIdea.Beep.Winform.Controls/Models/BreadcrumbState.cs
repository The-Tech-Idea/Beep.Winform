using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public class BreadcrumbState
    {
        public SimpleItem Item { get; set; }
        public bool IsHovered { get; set; }
        public bool IsSelected { get; set; }
    }
}
