// BeepComponentsLibrary/BeepComponents.cs
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using TheTechIdea.Beep.Utilities;


namespace TheTechIdea.Beep.Vis.Modules
{
    [Serializable]
    public class BeepComponents
    {
        public BeepComponents() { }

        public BeepComponents(string name, string description, Type type)
        {
            Name = name;
            Description = description;
            Type = type;
        }

        public BeepComponents(string name, string description, Type type, string assembly, string nameSpace, string className)
        {
            Name = name;
            Description = description;
            Type = type;
            Assembly = assembly;
            Namespace = nameSpace;
            ClassName = className;
        }

        public BeepComponents(
            string name, string description, Type type, string assembly, string nameSpace, string className,
            int id, string guid, string parentguid, string parentname, string parenttype,
            string parentassembly, string parentnamespace, string parentclassname,
            int left, int top, int width, int height)
        {
            Name = name;
            Description = description;
            Type = type;
            Assembly = assembly;
            Namespace = nameSpace;
            ClassName = className;
            Id = id;
            GUID = guid;
            ParentGUID = parentguid;
            ParentName = parentname;
            ParentType = parenttype;
            ParentAssembly = parentassembly;
            ParentNamespace = parentnamespace;
            ParentClassName = parentclassname;
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        [TypeConverter(typeof(BeepUIComponentTypeConverter))]
        [Browsable(true)]
        [Category("Component")]
        [DisplayName("Component Type")]
        [Description("Select the UI Component Type.")]
        [JsonIgnore] // Ignore the Type property during serialization
        public Type Type { get; set; } // Changed from string to Type

        // Helper property for serialization
      
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeFullName
        {
            get => Type?.FullName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Attempt to resolve the type by full name
                    Type = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.FullName == value);
                }
                else
                {
                    Type = null;
                }
            }
        }
        public string BlockID { get; set; }
        public string FieldID { get; set; }
        public string Assembly { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public int Id { get; set; }
        public string ControlGuidID { get; set; } = Guid.NewGuid().ToString();
        public string GUID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGUID { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string ParentAssembly { get; set; }
        public string ParentNamespace { get; set; }
        public string ParentClassName { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public  DbFieldCategory Category { get; set; }

        [Browsable(true)]
        [Category("Binding")]
        [DisplayName("Bound Property")]
        [Description("The property of the Control to bind to.")]
        public string BoundProperty { get; set; }

        [Browsable(true)]
        [Category("Binding")]
        [DisplayName("Data Source Property")]
        [Description("The property of the data source.")]
        public string DataSourceProperty { get; set; }
        public string LinkedProperty { get; set; }

        public override string ToString()
        {
            return Name;
        }
        public bool IsLovField { get; set; } = false;
        public bool IsFilterField { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;
        public bool IsRequired { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
        public bool IsEditable { get; set; } = true;
        public bool IsLovFromItems { get; set; } = false;
        public bool IsLovFromSQL { get; set; } = false;
        public string SQLDataSourceName { get; set; }
        public bool IsLovFromAPI { get; set; } = false;
        public string APIEndPoint { get; set; }
        public bool IsLovFromData { get; set; } = false;
     
        public string ValueMember { get; set; }
        public string Value { get; set; }
        public string DisplayMember { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayType { get; set; }
        public string DisplayFormat { get; set; }
        public string DisplayMask { get; set; }
        public string DisplayMaskType { get; set; }
        public string DisplayMaskFormat { get; set; }
        public string DisplayMaskCulture { get; set; }

        public string[] Items { get; set; }
    }
}
