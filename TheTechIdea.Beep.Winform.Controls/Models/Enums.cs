using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
   public enum DataSourceMode
    {
        None,
        Table,
        Query,
        CascadingMap,
        View,
        StoredProc,
        File,
        WebService,
        RestAPI,
        OData,
        GraphQL,
        Custom
    }
    public enum GridDataSourceType
    {
        Fixed,
        BindingSource,
        IDataSource
    }
}
