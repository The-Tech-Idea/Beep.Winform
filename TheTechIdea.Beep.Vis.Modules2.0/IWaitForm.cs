using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IWaitForm
    {
        void SetText(string text);
        void SetTitle(string title);
        void SetTitle(string title, string text);
        void SetImage(string image);
        IErrorsInfo Show(PassedArgs Passedarguments);
        IErrorsInfo Close();
    }
}
