using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IBeepUIComponent
    {
        // Existing properties and methods
        EnumBeepThemes Theme { get; set; }
        public string ComponentName { get; set; }
        void ApplyTheme();
        void ApplyTheme(EnumBeepThemes theme);
        void ApplyTheme(BeepTheme theme);
        Size GetSize();
        string Text { get; set; }
        void ShowToolTip(string text);
        void HideToolTip();
        IBeepUIComponent Form { get; set; }
        string GuidID { get; set; }
        string BlockID { get; set; }
        string FieldID { get; set; }
        int Id { get; set; }
        string[] Items { get; set; }
        bool ValidateData(out string  messege);
         IContainer Components { get; }
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
        int Left { get; set; }
        int Top { get; set; }
        int Width { get; set; }
        int Height { get; set; }    
        DbFieldCategory Category { get; set; }
        void Draw(Graphics graphics,Rectangle rectangle);
        void SetBinding(string controlProperty, string dataSourceProperty); // Method to bind a control property
        event EventHandler<BeepComponentEventArgs> PropertyChanged; // Event to notify that a property has changed
        event EventHandler<BeepComponentEventArgs> PropertyValidate; // Event to notify that a property is being validated
        
    }
    public class BeepComponentEventArgs : EventArgs
    {
        public BeepComponentEventArgs(IBeepUIComponent component, string propertyName,string linkedproperty,object val)
        {
            Component = component;
            PropertyName = propertyName;
            PropertyValue = val;
             LinkedProperty= linkedproperty;
        }
        public IBeepUIComponent Component { get; }
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
        public string LinkedProperty { get; set; }

        public override string ToString() { return $"{Component.ComponentName} {PropertyName} {PropertyValue}"; }
        public bool Cancel { get; set; } = false;
        public string Message { get; set; }
    }
}
