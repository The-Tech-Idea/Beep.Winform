
using System.ComponentModel.Design;

using TheTechIdea.Beep.Desktop.Common;


namespace TheTechIdea.Beep.Winform.Controls.Editors
{
    public class ColumnConfigCollectionEditor : CollectionEditor
    {

        public ColumnConfigCollectionEditor(Type type) : base(type) { }

        protected override Type CreateCollectionItemType()
        {
            return typeof(BeepColumnConfig);  // Specify the type for collection rootnodeitems
        }

        protected override object CreateInstance(Type itemType)
        {
            return new BeepColumnConfig();  // Create an instance of SimpleMenuItem
        }



    }



}
