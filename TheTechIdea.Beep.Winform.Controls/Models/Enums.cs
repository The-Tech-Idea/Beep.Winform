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
    public enum CheckBoxState
    {
        Unchecked,
        Checked,
        Indeterminate
    }
    public enum TextAlignment
    {
        Right,
        Left,
        Above,
        Below
    }
    public enum CheckMarkShape
    {
        Square,
        Circle,
        CustomSvg
    }

    public enum BeepGridColumnType
    {
        Text,
        CheckBoxBool,
        CheckBoxChar,
        CheckBoxString,
        ComboBox,
        DateTime,
        Image,
        ProgressBar,
        Rating,
        StarRating,
        Button,
        Link,
        Switch,
        ListBox,
        NumericUpDown,
        Custom
    }
}
