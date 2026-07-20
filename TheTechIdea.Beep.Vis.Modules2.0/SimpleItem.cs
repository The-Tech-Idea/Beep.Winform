using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.Models

{
    public class SimpleItem : IEquatable<SimpleItem>, INotifyPropertyChanged
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        private PropertyChangedEventHandler _inpcHandler;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _inpcHandler += value; }
            remove { _inpcHandler -= value; }
        }

        private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _inpcHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SimpleItem()
        {
            Guid = Guid.NewGuid();
            GuidId = Guid.ToString();
        }

        public bool Equals(SimpleItem other)
        {
            if (other == null)
                return false;

            return this.GuidId == other.GuidId;
        }
        public Guid Guid { get; set; } 
        // Override Equals
        public override bool Equals(object obj)
        {
            if (obj is SimpleItem other)
            {
                return GuidId.Equals(other.GuidId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GuidId.GetHashCode();
        }
        public int ID { get; set; }
        public string GuidId { get; set; }
        public string Description { get; set; } // used for to store the selected item
        public string SubText { get; set; } // used for to store the selected item
        public string SubText2 { get; set; } // used for to store the selected item
        public string SubText3 { get; set; } // used for to store the selected item
        public string Name { get; set; }
        public string MenuName { get; set; }
        
        public bool IsCheckable { get; set; } = false;
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set { if (_text != value) { _text = value; RaisePropertyChanged(); } }
        }

        [Description("Select the image file (SVG, PNG, JPG, etc.) to load")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public string ImagePath { get; set; } = string.Empty;
        private string _displayField = string.Empty;
        public string DisplayField
        {
            get { return string.IsNullOrEmpty(_displayField) ? Text : _displayField; }
            set { _displayField = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ParentValue { get; set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleItem ParentItem { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Value { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Item { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BindingList<SimpleItem> Children { get; set; } = new BindingList<SimpleItem>();
        public string MenuID { get; set; }
        public string ActionID { get; set; }
        public string ReferenceID { get; set; }
        public int ParentID { get; set; }
        public string OwnerReferenceID { get; set; }
        public string OtherReferenceID { get; set; }
        public EnumPointType PointType { get; set; }
        public string ObjectType { get; set; }
        public string BranchClass { get; set; }
        public string BranchName { get; set; }
        public EnumPointType BranchType { get; set; }
        public string MethodName { get; set; }
        public MenuItemType ItemType { get; set; }
        public DatasourceCategory Category { get; set; }
        public string Uri { get; set; }
        public string KeyCombination { get; set; } = string.Empty;
        public string AssemblyClassDefinitionID { get; set; }
        public string ClassDefinitionID { get; set; }
        public string PackageName { get; set; }
        public string BranchID { get; set; }
        public string ToolTip { get; set; } = string.Empty;
        public string BadgeText { get; set; } = string.Empty;

        private BeepColor _badgeBackColor = BeepColor.Red;
        public BeepColor BadgeBackColor
        {
            get => _badgeBackColor;
            set => _badgeBackColor = value;
        }

        private BeepColor _badgeForeColor = BeepColor.White;
        public BeepColor BadgeForeColor
        {
            get => _badgeForeColor;
            set => _badgeForeColor = value;
        }

        public BadgeShape BadgeShape { get; set; } = BadgeShape.Circle;

        private string _shortcuttext = string.Empty;
        public string ShortcutText
        {
            get => _shortcuttext;
            set => _shortcuttext = value;
        }
        private string _shortcut = string.Empty;
        public string Shortcut
        {
            get => _shortcut;
            set => _shortcut = value;
        }
        public bool IsSelected { get; set; } = false;
        public bool IsChecked { get; set; } = false;
        public bool IsIndeterminate { get; set; } = false;
        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { if (_isExpanded != value) { _isExpanded = value; RaisePropertyChanged(); } }
        }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        public bool IsEditable { get; set; } = true; // used for to store the selected item
        public bool IsVisibleInTree { get; set; } = true; // used for to store the selected item
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BaseSize { get; set; } = 50; // Individual base size
        public float MaxScale { get; set; } = 1.4f; // Individual max scale
        public int RootContainerID { get; set; }
        public string ContainerGuidID { get; set; }
        public int ContainerID { get; set; }
        public string ClassName { get; set; }

        public bool IsClassDistinct { get; set; } = false;
        public string RootContainerGuidID { get; set; }

        // ── ComboBox / Menu display helpers ─────────────────────────────────
        public string GroupName { get; set; }
        public bool IsSeparator { get; set; } = false;

        public string ValueField { get; set; } = string.Empty;
        public bool IsDrawn { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Tag { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Human-readable label for this item: <see cref="DisplayField"/>, which is the item's
        /// own display value and already falls back to <see cref="Text"/>.
        ///
        /// Anything that renders a SimpleItem without an explicit template lands here — the
        /// PropertyGrid, a plain WinForms/WPF ComboBox or ListBox, debugger output. This used to
        /// return <see cref="Name"/>, which most items never set, so those consumers rendered
        /// blank rows. Name is now only the fallback, for items that carry a name and no text.
        ///
        /// Never returns null: callers concatenate and format this without null-checking.
        /// </summary>
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(DisplayField)) return DisplayField;
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return string.Empty;
        }
    }

    //public class SimpleItemCollection : BindingList<SimpleItem>
    //{
    //    public SimpleItemCollection() : base() { }
    //    public SimpleItemCollection(BindingList<SimpleItem> list) : base(list) { }
    //    public BindingList<SimpleItem> Items { get; set; } = new BindingList<SimpleItem>();
    //    public EnumPointType PointType { get; set; }
    //    public string ObjectType { get; set; }
    //    public string BranchClass { get; set; }
    //    public string BranchName { get; set; }
    //    public string MenuName { get; set; }
    //    public string MenuID { get; set; }
    //    public string ActionID { get; set; }
    //    public string ReferenceID { get; set; }
    //    public string ClassGuidID { get; set; }

    //}
    [Serializable]
    public class SimpleMenuList : BindingList<SimpleItem>
    {
        public SimpleMenuList(string objectType, string branchClass, EnumPointType branchType)
        {
            ObjectType = objectType;
            BranchClass = branchClass;
            PointType = branchType;
        }
        public SimpleMenuList(string objectType, string branchClass, EnumPointType branchType,string classid)
        {
            ObjectType = objectType;
            BranchClass = branchClass;
            PointType = branchType;
            ClassGuidID = classid;
        }
        public SimpleMenuList()
        {
            
        }
        public BindingList<SimpleItem> Items { get; set; } = new BindingList<SimpleItem>();
        public string GuidID { get; set; }
        public string Name { get; set; }
        public EnumPointType PointType { get; set; }
        public string ObjectType { get; set; }
        public string BranchClass { get; set; }
        public string BranchName { get; set; }
        public string MenuName { get; set; }
        public string MenuID { get; set; }
        public string ActionID { get; set; }
        public string ReferenceID { get; set; }
        public string ClassGuidID { get; set; }
        public string ClassDefinitionID { get; set; }
        public string ClassName { get; set; }
        public string BranchID { get; set; }
         public bool IsClassDistinct { get; set; } = false;
    }
    public class SelectedItemChangedEventArgs : EventArgs
    {
        public SimpleItem SelectedItem { get; }
        
        // Add Item property for backward compatibility and consistency with documentation
      //  public SimpleItem Item => SelectedItem;

        public SelectedItemChangedEventArgs(SimpleItem selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }

}
