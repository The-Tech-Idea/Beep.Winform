using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IWizardState
    {
        PassedArgs args { get; set; }
        IWizardManager wizardManager { get; set; }
    }
}