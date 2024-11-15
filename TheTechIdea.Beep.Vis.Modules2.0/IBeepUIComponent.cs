using System;
using System.ComponentModel;
using System.Drawing;

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

        // New properties and methods for binding
        object DataContext { get; set; } // The source of data for binding

        void SetBinding(string controlProperty, string dataSourceProperty); // Method to bind a control property
    }
}
