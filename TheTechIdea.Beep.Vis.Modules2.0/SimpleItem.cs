using System;
using System.ComponentModel;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.Models

{
    [Serializable]
    public class SimpleItem : IEquatable<SimpleItem>, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        public SimpleItem()
        {
            Guid = Guid.NewGuid();
            GuidId = Guid.ToString();
           
        }
        public bool Equals(SimpleItem other)
        {
            if (other == null)
                return false;

            return this.Name == other.Name; // Assuming Name is unique
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
        
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value, nameof(Text));
        }

        [Description("Select the image file (SVG, PNG, JPG, etc.) to load")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        private string _imagePath = string.Empty;
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value, nameof(ImagePath));
        }
        private string _displayField = string.Empty; // used for to store the name of field that has value to display
        public string DisplayField
        {
            get { return string.IsNullOrEmpty(_displayField) ? Text : _displayField; }
            set { _displayField = value; }
        }
        
        private string _valueField = string.Empty; // used for to store the name of field that has value to store
        public string ValueField
        {
            get => _valueField;
            set => SetProperty(ref _valueField, value, nameof(ValueField));
        }
        
        private string _parentvalue = string.Empty; // used for to store the name of field that has value to store
        public string ParentValue
        {
            get => _parentvalue;
            set => SetProperty(ref _parentvalue, value, nameof(ParentValue));
        }
        
        [NonSerialized]
        private SimpleItem _parentItem; // used for to store the parent item
        public SimpleItem ParentItem
        {
            get => _parentItem;
            set => SetProperty(ref _parentItem, value, nameof(ParentItem));
        }
        
        [NonSerialized]
        private object _value; // used for to store the Value item
        public object Value
        {
            get => _value;
            set => SetProperty(ref _value, value, nameof(Value));
        }
        
        [NonSerialized]
        private object _selecteditem; // used for to store the Selected Value item
        public object Item
        {
            get => _selecteditem;
            set => SetProperty(ref _selecteditem, value, nameof(Item));
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        public string KeyCombination { get; set; } //KeyCombination
        public string AssemblyClassDefinitionID { get; set; }
        public string ClassDefinitionID { get; set; }
        public string PackageName { get; set; }
        public string BranchID { get; set; }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value, nameof(IsSelected));
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value, nameof(IsChecked));
        }

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value, nameof(IsExpanded));
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value, nameof(IsVisible));
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, nameof(IsEnabled));
        }

        public bool IsEditable { get; set; } = true; // used for to store the selected item
        public bool IsVisibleInTree { get; set; } = true; // used for to store the selected item
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BaseSize { get; set; } = 50; // Individual base size
        public float MaxScale { get; set; } = 1.4f; // Individual max scale
        public string ContainerGuidID { get; set; }
        public int ContainerID { get; set; }
        public string ClassName { get; set; }

        public bool IsClassDistinct { get; set; } = false;
        public string RootContainerGuidID { get; set; }

        public int RootContainerID { get; set; }
        public bool IsDrawn { get; set; } = false;
        public string ComposedID { get; set; } // this helps to identify the item in the tree , so that RootnodeID.childid.childid.childid and so on
        public object Tag { get; set; }
       

        public override string ToString()
        {
            return Name; // DisplayField this value in the PropertyGrid
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
