
using System.ComponentModel;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.Models

{
    [Serializable]
    public class SimpleItem: IEquatable<SimpleItem>
    {
        public SimpleItem()
        {
            GuidId = Guid.NewGuid().ToString();
        }
        public bool Equals(SimpleItem other)
        {
            if (other == null)
                return false;

            return this.Name == other.Name; // Assuming Name is unique
        }

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
        public int Id { get; set; }
        public string GuidId { get; set; }
        public string Description { get; set; } // used for to store the selected item
        public string SubText { get; set; } // used for to store the selected item
        public string SubText2 { get; set; } // used for to store the selected item
        public string SubText3 { get; set; } // used for to store the selected item
        public string Name { get; set; }
        public string MenuName { get; set; }
        public string Text { get; set; }
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public string ImagePath { get; set; }
        private string _displayField; // used for to store the name of field that has value to display
        public string DisplayField
        {
            get { return string.IsNullOrEmpty(_displayField) ? Text : _displayField; }
            set { _displayField = value; }
        }
        private string _valueField; // used for to store the name of field that has value to store
        public string ValueField
        {
            get { return _valueField; }
            set { _valueField = value; }
        }
        private string _parentvalue; // used for to store the name of field that has value to store
        public string ParentValue
        {
            get { return _parentvalue; }
            set { _parentvalue = value; }
        }
        [NonSerialized]
        private SimpleItem _parentItem; // used for to store the parent item
        public SimpleItem ParentItem
        {
            get { return _parentItem; }
            set { _parentItem = value; }
        }
        [NonSerialized]
        private object _value; // used for to store the Value item
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        [NonSerialized]
        private object _selecteditem; // used for to store the Selected Value item
        public object Item
        {
            get { return _selecteditem; }
            set { _selecteditem = value; }
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
        public  string KeyCombination { get; set; } //KeyCombination
        public string AssemblyClassDefinitionID { get; set; }
        public string ClassDefinitionID { get; set; }
        public string PackageName { get; set; }
        public string BranchID { get; set; }
       
        public bool IsSelected { get; set; } = false; // used for to store the selected item
        public bool IsChecked { get; set; } = false; // used for to store the selected item
        public bool IsExpanded { get; set; } = false; // used for to store the selected item
        public bool IsVisible { get; set; } = true; // used for to store the selected item
        public bool IsEnabled { get; set; } = true; // used for to store the selected item
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

        public SelectedItemChangedEventArgs(SimpleItem selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }

}
