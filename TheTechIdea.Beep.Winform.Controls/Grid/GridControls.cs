using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    public class GridControls
    {
        public GridControls()
        {
            // Column = column;
          //  GuidID = Guid.NewGuid().ToString();
        }
        public int Index { get; set; }
        public string GuidID { get; set; }

        public TextBox FilterBox { get; set; } = new TextBox();
        public TextBox TotalBox { get; set; } = new TextBox();
    }
}
