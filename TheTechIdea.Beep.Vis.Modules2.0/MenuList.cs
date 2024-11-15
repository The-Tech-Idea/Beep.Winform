using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Utilities    ;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class MenuList
    {
        public MenuList()
        {

        }
        public MenuList(string objectType, string branchClass, EnumPointType pointType)
        {
            ObjectType = objectType;
            BranchClass = branchClass;
            PointType = pointType;
        }
        public object Menu { get; set; } //ContextMenuStrip
                                         //  public List<ToolStripMenuItem> Items { get; set; } //ToolStripMenuItem
        public EnumPointType  PointType { get; set; }
        public string ObjectType { get; set; }
        public string BranchClass { get; set; } 
        public string branchname { get; set; }
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
        public List<AssemblyClassDefinition> classDefinitions { get; set; } = new List<AssemblyClassDefinition>();
    }
    [Serializable]
    public class MenuItem
    {
        public MenuItem()
        {
            ID = Guid.NewGuid().ToString();
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string MethodName { get; set; }
        public string ObjectType { get; set; }
        public string BranchClass { get; set; }
        public string branchname { get; set; }
        public MenuItemType itemType { get; set; }
        [JsonIgnore]
        public DatasourceCategory Category { get; set; }
        public EnumPointType PointType { get; set; }
        public string Uri { get; set; }
        [JsonIgnore]
        public IPassedArgs Parameters { get; set; }
        
        public CommandAttribute MethodAttribute { get; set; }
        public string imagename { get; set; }

        public AssemblyClassDefinition classDefinition { get; set; }
        public KeyCombination keyCombination { get; set; }
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();

        public override bool Equals(object obj)
        {
            if (obj is MenuItem other)
            {
                return this.ID == other.ID;
                       
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() ;
        }
    }
    public enum MenuItemType
    {
        Main,
        Child
    }
}
