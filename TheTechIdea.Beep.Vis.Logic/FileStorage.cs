using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Vis.Logic
{
    public class FileStorage : IFileStorage
    {
        public FileStorage()
        {
        }

        public string FileName { get ; set ; }
        public string Url { get ; set ; }
    }
}
