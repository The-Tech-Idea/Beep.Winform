
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{

    public partial class IBeepComponentForm : Form, IBeepUIComponent
    {
        public IBeepUIComponent Component { get; set; }
        public object Value { get; set; }
        public IBeepComponentForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TopMost = true;
        }
        public IBeepComponentForm(IBeepUIComponent component) : this()
        {
            AddComponent(component);
        }
        public void AddComponent(IBeepUIComponent component)
        {
           //MiscFunctions.SendLog($"🔄 Adding component {component.GetType().Name} to popup.");
            Component = component;
            Control ctl = (Control)component;

            if (!Controls.Contains(ctl))  // Prevent re-adding the control
            {
                Controls.Add(ctl);
                component.Form = this;
              
                ctl.Dock = DockStyle.Fill;
              // MiscFunctions.SendLog($"✅ Component added: {ctl.Name}");
            }
            else
            {
              // MiscFunctions.SendLog($"⚠️ Component already exists in popup: {ctl.Name}");
            }
        }
       
        public EnumBeepThemes Theme
        {
            get => Component?.Theme ?? EnumBeepThemes.DefaultTheme;
            set { if (Component != null) Component.Theme = value; }
        }

        public bool ApplyThemeToChilds
        {
            get => Component?.ApplyThemeToChilds ?? false;
            set { if (Component != null) Component.ApplyThemeToChilds = value; }
        }

        public string ComponentName
        {
            get => Component?.ComponentName ?? string.Empty;
            set { if (Component != null) Component.ComponentName = value; }
        }

        public IBeepUIComponent Form
        {
            get => Component?.Form;
            set { if (Component != null) Component.Form = value; }
        }

        public string GuidID
        {
            get => Component?.GuidID ?? string.Empty;
            set { if (Component != null) Component.GuidID = value; }
        }

        public string BlockID
        {
            get => Component?.BlockID ?? string.Empty;
            set { if (Component != null) Component.BlockID = value; }
        }

        public string FieldID
        {
            get => Component?.FieldID ?? string.Empty;
            set { if (Component != null) Component.FieldID = value; }
        }

        public int Id
        {
            get => Component?.Id ?? 0;
            set { if (Component != null) Component.Id = value; }
        }

        public List<object> Items
        {
            get => Component?.Items ?? new List<object>();
            set { if (Component != null) Component.Items = value; }
        }

        public string BoundProperty
        {
            get => Component?.BoundProperty ?? string.Empty;
            set { if (Component != null) Component.BoundProperty = value; }
        }

        public string DataSourceProperty
        {
            get => Component?.DataSourceProperty ?? string.Empty;
            set { if (Component != null) Component.DataSourceProperty = value; }
        }

        public string LinkedProperty
        {
            get => Component?.LinkedProperty ?? string.Empty;
            set { if (Component != null) Component.LinkedProperty = value; }
        }

        public string ToolTipText
        {
            get => Component?.ToolTipText ?? string.Empty;
            set { if (Component != null) Component.ToolTipText = value; }
        }

        public object Oldvalue
        {
            get => Component?.Oldvalue;

        }

        public Color BorderColor
        {
            get => Component?.BorderColor ?? Color.Empty;
            set { if (Component != null) Component.BorderColor = value; }
        }

        public bool IsRequired
        {
            get => Component?.IsRequired ?? false;
            set { if (Component != null) Component.IsRequired = value; }
        }

        public bool IsSelected
        {
            get => Component?.IsSelected ?? false;
            set { if (Component != null) Component.IsSelected = value; }
        }

        public bool IsDeleted
        {
            get => Component?.IsDeleted ?? false;
            set { if (Component != null) Component.IsDeleted = value; }
        }

        public bool IsNew
        {
            get => Component?.IsNew ?? false;
            set { if (Component != null) Component.IsNew = value; }
        }

        public bool IsDirty
        {
            get => Component?.IsDirty ?? false;
            set { if (Component != null) Component.IsDirty = value; }
        }

        public bool IsReadOnly
        {
            get => Component?.IsReadOnly ?? false;
            set { if (Component != null) Component.IsReadOnly = value; }
        }

        public bool IsEditable
        {
            get => Component?.IsEditable ?? false;
            set { if (Component != null) Component.IsEditable = value; }
        }

        public bool IsVisible
        {
            get => Component?.IsVisible ?? true;
            set { if (Component != null) Component.IsVisible = value; }
        }

        public bool IsFrameless
        {
            get => Component?.IsFrameless ?? false;
            set { if (Component != null) Component.IsFrameless = value; }
        }

        public DbFieldCategory Category
        {
            get => Component?.Category ?? DbFieldCategory.String;
            set { if (Component != null) Component.Category = value; }
        }

        public event EventHandler<BeepComponentEventArgs> PropertyChanged
        {
            add { if (Component != null) Component.PropertyChanged += value; }
            remove { if (Component != null) Component.PropertyChanged -= value; }
        }

        public event EventHandler<BeepComponentEventArgs> PropertyValidate
        {
            add { if (Component != null) Component.PropertyValidate += value; }
            remove { if (Component != null) Component.PropertyValidate -= value; }
        }

        public event EventHandler<BeepComponentEventArgs> OnSelected
        {
            add { if (Component != null) Component.OnSelected += value; }
            remove { if (Component != null) Component.OnSelected -= value; }
        }

        public event EventHandler<BeepComponentEventArgs> OnValidate
        {
            add { if (Component != null) Component.OnValidate += value; }
            remove { if (Component != null) Component.OnValidate -= value; }
        }

        public event EventHandler<BeepComponentEventArgs> OnValueChanged
        {
            add { if (Component != null) Component.OnValueChanged += value; }
            remove { if (Component != null) Component.OnValueChanged -= value; }
        }

        public event EventHandler<BeepComponentEventArgs> OnLinkedValueChanged
        {
            add { if (Component != null) Component.OnLinkedValueChanged += value; }
            remove { if (Component != null) Component.OnLinkedValueChanged -= value; }
        }

        public void ApplyTheme()
        {
            Component?.ApplyTheme();
        }

        public void ApplyTheme(EnumBeepThemes theme)
        {
            Component?.ApplyTheme(theme);
        }

        public void ApplyTheme(BeepTheme theme)
        {
            Component?.ApplyTheme(theme);
        }

        public void ClearValue()
        {
            Component?.ClearValue();
        }

        public void Draw(Graphics graphics, Rectangle rectangle)
        {
            Component?.Draw(graphics, rectangle);
        }

        public Size GetSize()
        {
            return Component?.GetSize() ?? Size.Empty;
        }

        public object GetValue()
        {
          // MiscFunctions.SendLog($"Getting Value from BeepTextBox in Beepiform");
            return Component?.GetValue();
        }

        public bool HasFilterValue()
        {
            return Component?.HasFilterValue() ?? false;
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
             //  MiscFunctions.SendLog($"⚠️ BeeIForm lost focus. Saving: {Value}");
      //      Component.SetValue(Value);  // Force saving before closing
            
        }

        public void HideToolTip()
        {
            Component?.HideToolTip();
        }

        public void RefreshBinding()
        {
            Component?.RefreshBinding();
        }

        public void SetBinding(string controlProperty, string dataSourceProperty)
        {
            Component?.SetBinding(controlProperty, dataSourceProperty);
        }

        public void SetValue(object value)
        {
           //MiscFunctions.SendLog($"SetValue: {value}");
            Component?.SetValue(value);
            Value = value;
        }
        public new void Show()
        {
          // MiscFunctions.SendLog($"🔄 Showing popup. Current editor text: {Component?.GetValue()}");
            base.Show();
         //  MiscFunctions.SendLog($"✅ After popup is visible, editor text: {Component?.GetValue()}");
        }

        protected override void OnDeactivate(EventArgs e)
        {
          // MiscFunctions.SendLog($"⚠️ Popup lost focus. Current text: {Component?.GetValue()}");
            base.OnDeactivate(e);
        }


        public void ShowToolTip(string text)
        {
            Component?.ShowToolTip(text);
        }

        public AppFilter ToFilter()
        {
            return Component?.ToFilter();
        }

        public bool ValidateData(out string message)
        {
            return Component != null ? Component.ValidateData(out message) : throw new NotImplementedException();
        }
    }
}
