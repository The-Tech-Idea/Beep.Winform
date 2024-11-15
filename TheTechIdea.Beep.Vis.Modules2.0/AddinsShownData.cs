
using System;
using System.Text.Json.Serialization;
using TheTechIdea;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Vis.Modules
{
    [Serializable]
    public class AddinsShownData
    {
        public AddinsShownData() { GuidID = Guid.NewGuid().ToString(); }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public IDM_Addin Addin { get; set; }
        public string GuidID { get; set; } 
        public bool IsSingleton { get; set; } = false;
        public bool IsShown { get; set; }=false;
        public bool IsHidden { get; set; } = false;
        public bool IsAdded { get; set; } = false;
        public bool IsRemoved { get; set; } = false;
       
    }
}
