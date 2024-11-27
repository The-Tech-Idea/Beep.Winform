﻿
using System.ComponentModel;


namespace TheTechIdea.Beep.Winform.Controls.Design
{
    [Serializable]

    public class SimpleItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Text { get; set; }
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public string Image { get; set; }
        public MenuItemType ItemType { get; set; }

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
        private SimpleItem _parentItem; // used for to store the parent item
        public SimpleItem ParentItem
        {
            get { return _parentItem; }
            set { _parentItem = value; }
        }

        public BindingList<SimpleItem> Children { get; set; } = new BindingList<SimpleItem>();


        public string ReferenceID { get; set; }
        public string OwnerReferenceID { get; set; }
        public string OtherReferenceID { get; set; }
        public override string ToString()
        {
            return Name; // Display this value in the PropertyGrid
        }
    }
    [Serializable]
    public class SimpleItemCollection : BindingList<SimpleItem>
    {
        public SimpleItemCollection() : base() { }
        public SimpleItemCollection(BindingList<SimpleItem> list) { }
    }
    public enum MenuItemType
    {
        Main,
        Child
    }

}