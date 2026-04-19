using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System;
using System.Threading.Tasks;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IWaitForm
    {
        Progress<PassedArgs> Progress { get; }
        void SetText(string text);
        void SetTitle(string title);
        void SetTitle(string title, string text);
        void SetImage(string image);
        void UpdateProgress(int progress, string message = null);
        IErrorsInfo Config(PassedArgs Passedarguments);
        Task<IErrorsInfo> CloseAsync();
        void CloseForm();
        void SafeInvoke(Action action);
    }
}
