
using System.ComponentModel;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.Common
{
    [Serializable]
    public class SimpleItem
    {
        public SimpleItem()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Children { get; set; } = new BindingList<SimpleItem>();
        public string MenuID { get; set; }
        public string ActionID { get; set; }
        public string ReferenceID { get; set; }
        public string OwnerReferenceID { get; set; }
        public string OtherReferenceID { get; set; }
        //  public List<ToolStripMenuItem> Items { get; set; } //ToolStripMenuItem
        public EnumPointType PointType { get; set; }
        public string ObjectType { get; set; }
        public string BranchClass { get; set; }
        public string Branchname { get; set; }
        public MenuItemType ItemType { get; set; }
        public DatasourceCategory Category { get; set; }
        public string Uri { get; set; }
        public string MethodName { get; set; }
        public  string keyCombination { get; set; } //KeyCombination
        public string AssemblyClassDefinitionID { get; set; }
        public string BranchID { get; set; }
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

    }
   

}
