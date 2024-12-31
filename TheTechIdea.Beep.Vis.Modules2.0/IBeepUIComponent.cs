using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IBeepUIComponent
    {
        // Existing properties and methods
        EnumBeepThemes Theme { get; set; }
        void ApplyTheme();
        void ApplyTheme(EnumBeepThemes theme);
        void ApplyTheme(BeepTheme theme);
        Size GetSize();
        string Text { get; set; }
        void ShowToolTip(string text);
        void HideToolTip();
        IBeepUIComponent Form { get; set; }
        string GuidID { get; }
        int Id { get; set; }
        string[] Items { get; set; }
        bool ValidateData(out string  messege);
     
        // New properties and methods for binding
        object DataContext { get; set; } // The source of data for binding
        string BoundProperty { get; set; } // The property of the Control to bind to  DataSourceProperty
        string DataSourceProperty { get; set; } // The property of the data source
        string LinkedProperty { get; set; }
        void RefreshBinding();
        void SetValue(object value);
        object GetValue();
        void ClearValue();
        bool HasFilterValue();
        AppFilter ToFilter();
        void SetBinding(string controlProperty, string dataSourceProperty); // Method to bind a control property
    }
}
