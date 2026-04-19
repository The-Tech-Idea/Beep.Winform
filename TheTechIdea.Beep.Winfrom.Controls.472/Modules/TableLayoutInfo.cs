using System;
using System.Collections.Generic;
using System.Drawing;


namespace TheTechIdea.Beep.Vis.Modules
{
    public class TableLayoutInfo
    {
        public List<int> Rows { get; set; }=new List<int>();
        public List<int> Columns { get; set; } = new List<int>();
        public List<ControlInfo> Controls { get; set; } = new List<ControlInfo>();
        public string ID { get; set; }= Guid.NewGuid().ToString();
        public Dictionary<Point, TableLayoutInfo> InLayout { get; set; } = new Dictionary<Point, TableLayoutInfo>();
        public TableLayoutInfo() {
            Rows = new List<int>();
            Columns = new List<int>();
            Controls = new List<ControlInfo>();
           

        }
    }
}
