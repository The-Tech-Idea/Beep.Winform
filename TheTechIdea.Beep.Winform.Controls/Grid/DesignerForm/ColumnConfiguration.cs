using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DesignerForm
{
    [Serializable]
    public class ColumnConfiguration
    {
        public string HeaderText { get; set; }
        public Type ColumnType { get; set; }
        // Add other properties as needed
    }

}
