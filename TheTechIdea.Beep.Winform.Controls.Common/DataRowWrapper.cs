using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Desktop.Common
{
    public class DataRowWrapper
    {
        public int RowID { get; set; }
        public object OriginalData { get; set; }

        public DataRowWrapper(object data, int id)
        {
            OriginalData = data;
            RowID = id;
        }
    }
}
