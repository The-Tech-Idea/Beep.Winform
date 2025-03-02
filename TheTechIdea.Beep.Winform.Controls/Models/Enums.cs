using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public enum DateFormatStyle
    {
        ShortDate,        // "MM/dd/yyyy"
        LongDate,         // "dddd, MMMM dd, yyyy"
        YearMonth,        // "MMMM yyyy"
        Custom,           // Uses the DateFormat string
        FullDateTime,     // "dddd, MMMM dd, yyyy HH:mm:ss"
        ShortDateTime,    // "MM/dd/yyyy HH:mm"
        DayMonthYear,     // "dd MMMM yyyy"
        ISODate,          // "yyyy-MM-dd"
        ISODateTime,      // "yyyy-MM-dd HH:mm:ss"
        TimeOnly,         // "HH:mm:ss"
        ShortTime,        // "HH:mm"
        MonthDay,         // "MMMM dd"
        DayOfWeek,        // "dddd"
        RFC1123,          // "ddd, dd MMM yyyy HH:mm:ss GMT"
        UniversalSortable // "yyyy-MM-dd HH:mm:ssZ"
    }
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

 
}
