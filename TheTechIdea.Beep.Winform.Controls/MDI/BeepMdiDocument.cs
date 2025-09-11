using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class BeepMdiDocument
    {
        private string _name;
        private string _text;

        [Category("Design"), Description("Logical name (used in code).")]
        public string Name
        {
            get => string.IsNullOrWhiteSpace(_name) ? (_text ?? "Document") : _name;
            set => _name = value;
        }

        [Category("Appearance"), Description("Caption shown on the tab.")]
        public string Text
        {
            get => string.IsNullOrWhiteSpace(_text) ? (_name ?? "Document") : _text;
            set => _text = value;
        }

        [Category("Behavior"), Description("Optional Form type to instantiate at runtime when AutoCreateDocumentsOnLoad = true.")]
        [TypeConverter(typeof(TypeSelectorConverter))]
        public Type FormType { get; set; }

        [Category("Behavior")]
        public bool StartMaximized { get; set; } = true;

        public override string ToString() => $"{Text}";
    }

}
