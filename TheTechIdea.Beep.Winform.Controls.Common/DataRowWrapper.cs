using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Desktop.Common
{
    public class DataRowWrapper
    {
        public Guid TrackingUniqueId { get; set; }
        public int RowID { get; set; }
        public object OriginalData { get; set; }
        public DataRowState RowState { get; set; }
        public DateTime? DateTimeChange { get; set; }= DateTime.Now;
        public DataRowWrapper(object data)
        {
            OriginalData = data;
        }

        public DataRowWrapper(object data, int id)
        {
            OriginalData = data;
            RowID = id;
        }
    }
   
}
