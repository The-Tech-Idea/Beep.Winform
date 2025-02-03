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
        bool ApplyThemeToChilds { get; set; }
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
        // New properties and methods for binding
        object DataContext { get; set; } // The source of data for binding
        string BoundProperty { get; set; } // The property of the Control to bind to  DataSourceProperty
        string DataSourceProperty { get; set; } // The property of the data source
        string LinkedProperty { get; set; }
        string ToolTipText { get; set; }
        
        void RefreshBinding();
        void SetValue(object value);
        object GetValue();
        object Oldvalue { get; }
        void ClearValue();
        bool HasFilterValue();
        AppFilter ToFilter();
        int Left { get; set; }
        int Top { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Color BorderColor { get; set; }
        bool IsRequired { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
        DbFieldCategory Category { get; set; }
        void Draw(Graphics graphics,Rectangle rectangle);
        void SetBinding(string controlProperty, string dataSourceProperty); // Method to bind a control property
        event EventHandler<BeepComponentEventArgs> PropertyChanged; // Event to notify that a property has changed
        event EventHandler<BeepComponentEventArgs> PropertyValidate; // Event to notify that a property is being validated
        event EventHandler<BeepComponentEventArgs> OnSelected;
        event EventHandler<BeepComponentEventArgs> OnValidate;
        event EventHandler<BeepComponentEventArgs> OnValueChanged;
        event EventHandler<BeepComponentEventArgs> OnLinkedValueChanged;
  


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
