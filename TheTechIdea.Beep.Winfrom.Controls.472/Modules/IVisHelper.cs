

using System.Collections.Generic;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IVisHelper
    {
        IDMEEditor DMEEditor { get; set; }
        IAppManager Vismanager { get; set; }
        List<ImageConfiguration> ImgAssemblies { get; set; }

       // int GetImageIndex(string imagename);
        int GetImageIndexFromConnectioName(string Connectioname);
        List<ImageConfiguration> GetGraphicFilesLocations(string path);
        List<ImageConfiguration> GetGraphicFilesLocationsFromEmbedded(string[] namespacestoinclude);
        object GetImageFromName(string name);
        object GetImage(string imagename);
        object GetResource(string resource);
        object GetImage(string imagename,int size);
        object GetImageFromIndex(int index);
        int GetImageIndex(string name);
        List<string> GetImageNames();
        object LogoBigImage { get; set; }
        object LogoSmallImage { get; set; }

        void RefreshTreeView();
    }
}