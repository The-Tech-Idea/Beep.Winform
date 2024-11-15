using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class ImageConfiguration
    {
        public ImageConfiguration() { }
        public int Index { get; set; }
        public string GuidID { get; set; }=Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Ext { get; set; }
        public string Path { get; set; }
        public Assembly Assembly { get;set; }
    }
}
