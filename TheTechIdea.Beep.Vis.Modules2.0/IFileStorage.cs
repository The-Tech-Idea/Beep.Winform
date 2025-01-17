using System;
using System.Collections.Generic;
using System.Text;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IFileStorage
    {
        string FileName { get; set; }
        string Url { get; set; }
    }
    public class FileStorage : IFileStorage
    {
        public FileStorage()
        {
        }

        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
