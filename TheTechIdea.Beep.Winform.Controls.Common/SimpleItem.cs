
using System.ComponentModel;
using System.Text.Json.Serialization;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Desktop.Common
{
    [Serializable]
    public class SimpleItem
    {
        public SimpleItem()
        {
            GuidId = Guid.NewGuid().ToString();
        }
        public int Id { get; set; }
        public string GuidId { get; set; }
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
        [NonSerialized]
        private SimpleItem _parentItem; // used for to store the parent item
        public SimpleItem ParentItem
        {
            get { return _parentItem; }
            set { _parentItem = value; }
        }
        [NonSerialized]
        private object _value; // used for to store the parent item
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Children { get; set; } = new BindingList<SimpleItem>();
        public string MenuID { get; set; }
        public string ActionID { get; set; }
        public string ReferenceID { get; set; }
        public int ParentID { get; set; }
        public string OwnerReferenceID { get; set; }
        public string OtherReferenceID { get; set; }
        //  public List<ToolStripMenuItem> Items { get; set; } //ToolStripMenuItem
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
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public string ContainerGuidID { get; set; }
        public int ContainerID { get; set; }

        public string RootContainerGuidID { get; set; }

        public int RootContainerID { get; set; }
        public bool IsDrawn { get; set; } = false;
        public string ComposedID { get; set; } // this helps to identify the item in the tree , so that RootnodeID.childid.childid.childid and so on

        public override string ToString()
        {
            return Name; // Display this value in the PropertyGrid
        }
    }
    [Serializable]
    public class SimpleItemCollection : BindingList<SimpleItem>
    {
        public SimpleItemCollection() : base() { }
        public SimpleItemCollection(BindingList<SimpleItem> list) : base(list) { }
        public BindingList<SimpleItem> Items { get; set; } = new BindingList<SimpleItem>();
        public EnumPointType PointType { get; set; }
        public string ObjectType { get; set; }
        public string BranchClass { get; set; }
        public string BranchName { get; set; }

    }
    public class SimpleMenuList
    {
        public SimpleMenuList(string objectType, string branchClass, EnumPointType branchType)
        {
            ObjectType = objectType;
            BranchClass = branchClass;
            BranchType = branchType;
        }
        public SimpleMenuList()
        {
            
        }
        public string GuidID { get; set; }
        public string Name { get; set; }
        public string MenuName { get; set; }
        public BindingList<SimpleItem> Items { get; set; } = new BindingList<SimpleItem>();
        public EnumPointType PointType { get; set; }
        public string ObjectType { get; set; }
        public string BranchClass { get; set; }
        public string BranchName { get; set; }
        public EnumPointType BranchType { get; }
    }

}
