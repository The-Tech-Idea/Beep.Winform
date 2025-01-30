using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Editors
{
    public class ColumnConfigCollectionEditor : CollectionEditor
    {

        public ColumnConfigCollectionEditor(Type type) : base(type) { }

        protected override Type CreateCollectionItemType()
        {
            return typeof(BeepGridColumnConfig);  // Specify the type for collection rootnodeitems
        }

        protected override object CreateInstance(Type itemType)
        {
            return new BeepGridColumnConfig();  // Create an instance of SimpleMenuItem
        }



    }



}
