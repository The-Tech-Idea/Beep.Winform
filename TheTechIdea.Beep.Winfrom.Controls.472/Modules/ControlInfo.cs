

using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class ControlInfo
    {
        public ControlInfo() { }
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int ColumnSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
    }

}
