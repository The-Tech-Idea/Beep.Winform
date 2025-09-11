using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.MDI;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    // Made public so external design-time assembly (VSIX) can instantiate via reflection
    public class BeepMdiDocumentCollectionEditor : System.ComponentModel.Design.CollectionEditor
    {
        public BeepMdiDocumentCollectionEditor(Type t) : base(t) { }
        protected override Type CreateCollectionItemType() => typeof(BeepMdiDocument);
        protected override Type[] CreateNewItemTypes() => new[] { typeof(BeepMdiDocument) };
        protected override object CreateInstance(Type itemType)
        {
            var doc = (BeepMdiDocument)base.CreateInstance(itemType);
            doc.Text = "New Document";
            doc.Name = "Document" + Guid.NewGuid().ToString("N").Substring(0, 5);
            return doc;
        }
    }
}
