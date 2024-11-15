using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.ComponentModel;


namespace TheTechIdea.Beep.Winform.Controls.ModernSideMenu
{
    [Serializable]
   
    public class SimpleMenuItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Text { get; set; }
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load")]
        [Category("Appearance")]
        public string Image { get; set; }
        public MenuItemType ItemType { get; set; }

        private string _displayField; // used for to store the name of field that has value to display
        public string DisplayField
        {
            get { return string.IsNullOrEmpty(_displayField)? Text :_displayField; }
            set { _displayField = value; }
        }
        private string _valueField; // used for to store the name of field that has value to store
        public string ValueField
        {
            get { return _valueField; }
            set { _valueField = value; }
        }
        private SimpleMenuItem _parentItem; // used for to store the parent item
        public SimpleMenuItem ParentItem
        {
            get { return _parentItem; }
            set { _parentItem = value; }
        }
       
        public BindingList<SimpleMenuItem> Children { get; set; } = new BindingList<SimpleMenuItem>();
      

        public string ReferenceID { get; set; }
        public string OwnerReferenceID { get; set; }
        public string OtherReferenceID { get; set; }
        public override string ToString()
        {
            return Name; // Display this value in the PropertyGrid
        }
    }
    [Serializable]
    public class SimpleMenuItemCollection : BindingList<SimpleMenuItem>
    {
        public SimpleMenuItemCollection() : base() { }
        public SimpleMenuItemCollection(BindingList<SimpleMenuItem> list) { }
    }
    public enum MenuItemType
    {
        Main,
        Child
    }

}
